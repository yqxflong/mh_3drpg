using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GUIChecker : ResChecker
{
    public ResCheckResult Check(ResCheckerCallBack callbacker)
    {
        var prefabs = Directory.GetFiles(CatalogueConfig.UIPrefabPath, "*.prefab", SearchOption.AllDirectories).ToList<string>();
        callbacker.BeginCheck(this.Name(), prefabs.Count);
        List<string> wrongs = new List<string>();
        for (int i = 0; i < prefabs.Count; i++)
        {
            var p = prefabs[i];
            var prefab = AssetDatabase.LoadAssetAtPath(p, typeof(GameObject)) as GameObject;
            if (prefab != null)
            {
                var rigids = prefab.GetComponentsInChildren<Rigidbody>();
                if (rigids != null && rigids.Length > 0)
                {
                    foreach(var rigid in rigids)
                    {
                        wrongs.Add(string.Format("{0}的{1}节点不应该包含Rigidbody组件!", prefab.name, rigid.gameObject.name));
                    }
                }

                var fonts = prefab.GetComponentsInChildren<UILabel>();
                if (fonts != null && fonts.Length > 0)
                {
                    foreach (var font in fonts)
                    {
                        if (font.trueTypeFont == null)
                        {
                            wrongs.Add(string.Format("{0}的{1}节点有字体丢失!", prefab.name, font.gameObject.name));
                        }
                    }
                }

                var components = prefab.GetComponentsInChildren<Component>(true);
                foreach (var c in components)
                {
                    if (c == null)
                    {
                        wrongs.Add(string.Format("prefab名字：{0}有脚本丢失!", prefab.name));
                    }
                }

				//font check

				if (prefab.GetComponent<UIPanel>() == null)
				{
					continue;
				}
				UILabel[] labels = prefab.transform.GetComponentsInChildren<UILabel>(true);
				foreach (var label in labels)
				{
					if (label.fontStyle != FontStyle.Normal)
					{
						wrongs.Add(string.Format("{0}的{1}节点字体fontStyle错误", prefab.name, label.gameObject.name));
					}
					if (label.effectStyle != UILabel.Effect.None && label.effectDistance.x != 2 && label.effectDistance.x != 3)
					{						
						wrongs.Add(string.Format("{0}的{1}节点字体effectDistance错误", prefab.name, label.gameObject.name));
					}	
				}

				//btn check
				UIButton[] btns = prefab.transform.GetComponentsInChildren<UIButton>(true);
				foreach (var btn in btns)
				{
					if (btn.hover != Color.white || btn.pressed != Color.white)
					{
						wrongs.Add(string.Format("{0}的{1}节点按钮的hover pressed的状态错误", prefab.name, btn.gameObject.name));
					}
					if (btn.GetComponent<UIButtonScale>() == null)
					{
						wrongs.Add(string.Format("{0}的{1}节点按钮没有UIButtonScale组件", prefab.name, btn.gameObject.name));
					}
				}

				callbacker.OnCheckProgress(string.Format("Checking{0}", p), i, prefabs.Count);
            }
        }
        
        callbacker.OnCheckEnd();
        return new ResCheckResult(Name(), wrongs);
    }

	public string Name()
    {
        return "GUIChecker";
    }
}