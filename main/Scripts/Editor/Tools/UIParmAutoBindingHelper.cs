using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class UIParmAutoBindingHelper
{
	[MenuItem("CONTEXT/UIControllerILR/自动绑定代码用到的节点组件")]
	public static void AutoBinding()
    {
#if !ILRuntime
		UIControllerILR current = UnityEditor.Selection.activeGameObject.GetComponent<UIControllerILR>();

		UIControllerILRObject instance = current.ilinstance;
		bool release = false;
        if (!string.IsNullOrEmpty(current.hotfixClassPath))
		{
			var type = HotfixILRManager.GetInstance().assembly.GetType(current.hotfixClassPath);
			if (instance == null)
			{
				instance = System.Activator.CreateInstance(type) as UIControllerILRObject;
				release = true;
			}

			FieldInfo[] fields = type.GetFields();
			TextAsset script = AssetDatabase.LoadAssetAtPath<TextAsset>(string.Concat("Assets/", current.FilePath));
			if (script == null || script.text == String.Empty)
			{
				Debug.LogError("Could not read text by " + string.Concat("Assets/", current.FilePath));
				return;
			}

			string[] rows = script.text.Split(new string[]{ "\n" }, StringSplitOptions.None);
			string targetRow = string.Empty;

			foreach (FieldInfo f in fields)
			{
				System.Type fieldType = f.FieldType;
				Debug.LogFormat("field.Name = {0},field.Type.Name = {1}", f.Name, fieldType.Name);
				UIControllerILR.ParmType fieldEnumType;
				if (System.Enum.TryParse(fieldType.Name, out fieldEnumType))
				{
					if (!current.ParmPathList.Exists(p => p.Name == f.Name))
					{
						UIControllerILR.ParmStruct structural = new UIControllerILR.ParmStruct();
						structural.Name = f.Name;
						structural.Type = fieldEnumType;

						for (int i = 0; i < rows.Length; i++)
						{
							string row = rows[i];
							if (row.Contains(f.Name + " = t.") || row.Contains(f.Name + " = controller.transform."))
							{
								targetRow = row;
								break;
							}
						}
						if(string.IsNullOrEmpty(targetRow)){ Debug.Log("<color=yellow>this filed has not found or got by awake method!</color>"); continue;}

						string[] parts = targetRow.TrimAll('"').Split('(');
						if (parts.Length > 1)
						{
							string text = parts[1].TrimAll('(',')',';','.').Replace("gameObject", String.Empty).TrimEnd();
							if(!string.IsNullOrEmpty(text)) structural.Path = text.Replace(String.Format("GetComponent<{0}>", structural.Type.ToString()), "");
							Debug.LogFormat("Create ParmStruct: <color=purple>Name = {0},Type = {1},Path = {2}</color>", structural.Name, structural.Type, structural.Path);
						}

						current.ParmPathList.Add(structural);
						EditorUtility.SetDirty(current);
					}
				}
				else
				{
					Debug.Log("<color=red>field type is not feasible!</color>");
				}
			}
		}

		if (release)
		{
			instance = null;
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
#else
        EB.Debug.LogError("ILR模式下不存在assembly，会报错！请自行解决后再去掉这个宏。");
#endif
    }

	[MenuItem("CONTEXT/UIControllerILR/自动替换代码中用到的节点组件的字段")]
	public static void AutoTrimAndReplace()
	{
		UIControllerILR current = UnityEditor.Selection.activeGameObject.GetComponent<UIControllerILR>();
		if (!string.IsNullOrEmpty(current.hotfixClassPath))
		{
			TextAsset script = AssetDatabase.LoadAssetAtPath<TextAsset>(string.Concat("Assets/", current.FilePath));
			if (script == null || script.text == String.Empty)
			{
				Debug.LogError("Could not read text by " + string.Concat("Assets/", current.FilePath));
				return;
			}

			string[] rows = script.text.Split(new string[] { "\n" }, StringSplitOptions.None);
			List<string> writeRows = new List<string>();

			current.ParmPathList.ForEach(p =>
			{
				// 删掉字段声明
				// ------------------------------------
				int rowIndex = -1;
				for (int i = 0; i < rows.Length; i++)
				{
					string row = rows[i];
					if (row.Contains(p.Type + " " + p.Name))
					{
						rowIndex = i;
						break;
					}
				}

				if (rowIndex >= 0)
				{
					rows[rowIndex] = string.Empty;
				}
				// ------------------------------------

				// 删掉初始化
				// Field = t.Get， Field = t.Find -----
				int rowIndex2 = -1;
				for (int i = 0; i < rows.Length; i++)
				{
					string row = rows[i];
					if (row.Contains(p.Name + " = t.Get") || row.Contains(p.Name + " = t.Find") || row.Contains(p.Name + " = controller.transform.Find") ||
					    row.Contains(p.Name + " = controller.transform.Get"))
					{
						rowIndex2 = i;
						break;
					}
				}
				if (rowIndex2 >= 0)
				{
					rows[rowIndex2] = string.Empty;
				}

				//List创建绑定无法自动分析,无法得知用到的List，因为这个方法没有反射，父类没有记录

				// 替换代码中用到的行
				for (int i = 0; i < rows.Length; i++)
				{
					string row = rows[i];
					if (i != rowIndex && i != rowIndex2 && !row.Contains("/" + p.Name) && !row.Contains(p.Name + "/"))
					{
						string typeName = p.Type == UIControllerILR.ParmType.GameObject ? "GObject" : p.Type.ToString();
						typeName = p.Type == UIControllerILR.ParmType.CampaignTextureCmp ? "TextureCmp" : typeName;
						typeName = p.Type == UIControllerILR.ParmType.ConsecutiveClickCoolTrigger ? "CoolTrigger" : typeName;
						string containerName = typeName.Contains("UI") ? p.Type.ToString().Replace("UI", "Ui") + "s" : typeName.ToString() + "s";
						rows[i] = rows[i].ReplaceWholeWord(p.Name, string.Concat("controller.", containerName, string.Format("[\"{0}\"]", p.Name)));
					}
				}
			});

			for (int i = 0; i < rows.Length; i++)
			{
				if(string.IsNullOrEmpty(rows[i]))continue;
				writeRows.Add(rows[i].Replace("\r",string.Empty));
			}

			string writePath = string.Concat(Application.dataPath, "/", current.FilePath);
			File.WriteAllLines(writePath, writeRows.ToArray());

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}