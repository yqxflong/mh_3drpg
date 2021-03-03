using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CharacterPartitionLink))]
public class CharacterPartitionLinkInspector : Editor 
{
	private static Vector2 m_ScrollPosition = Vector2.zero;

	void OnEnable()
	{
		CharacterPartitionLink link = target as CharacterPartitionLink;
		for(int i = 0; i < link.LinkedPartitions.Count; i++)
		{
			string assetPath = PartitionAssetManager.Instance.GetAssetPath(link.LinkedPartitions[i].AssetName);
			if(string.IsNullOrEmpty(assetPath))
			{
				continue;
			}
			PartitionInfo partition = link.LinkedPartitions[i];
			partition.AssetObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
			link.LinkedPartitions[i] = partition;
		}
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		GUILayout.Space(8);
		m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
		DrawLinkedPartitionList();
		GUILayout.EndScrollView();
	}

	void DrawLinkedPartitionList()
	{
		CharacterPartitionLink link = target as CharacterPartitionLink;
		if(NGUIEditorTools.DrawHeader(string.Format("Linked Partition List ({0})", link.LinkedPartitions.Count), "CharacterPartitionLink_Partitions"))
		{
			NGUIEditorTools.BeginContents();
			{
				bool delete = false;
				for(int i = 0; i < link.LinkedPartitions.Count; i++)
				{
					if(i > 0)
					{
						GUILayout.Space(5);
					}
					
					
					NGUIEditorTools.BeginContents();
					{
						DrawPartition(i, ref delete);
					}
					NGUIEditorTools.EndContents();
					
					if(delete)
					{
						link.LinkedPartitions.RemoveAt(i);
						break;
					}
				}
				
				if (GUILayout.Button("Add New Partition"))
				{
					PartitionInfo newPartition = new PartitionInfo();
					int count = 1;
					string name = string.Format("Partition_{0}", count);
					while(true)
					{
						bool nameExisted = false;
						foreach(PartitionInfo partition in link.LinkedPartitions)
						{
							if(partition.Name == name)
							{
								nameExisted = true;
								break;
							}
						}
						if(nameExisted)
						{
							count++;
							name = string.Format("Partition_{0}", count);
						}
						else
						{
							break;
						}
					}
					
					newPartition.Name = name;
					link.LinkedPartitions.Add(newPartition);
				}
			}
			NGUIEditorTools.EndContents();
		}
	}
	
	void DrawPartition(int index, ref bool delete)
	{
		CharacterPartitionLink link = target as CharacterPartitionLink;
		string prefsKey = string.Format("CharacterPartitionLink_Partition_{0}", index);
		bool view = EditorPrefs.GetBool(prefsKey, true);
		
		PartitionInfo partition = link.LinkedPartitions[index];
		GUILayout.BeginHorizontal();
		{
			GUILayout.Space(12);
			view = EditorGUILayout.Foldout(view, partition.Name);
			
			if (GUILayout.Button("X", GUILayout.Width(20)))
				delete = true;
		}
		GUILayout.EndHorizontal();
		EditorPrefs.SetBool(prefsKey, view);
		
		if(view)
		{
			partition.Name = EditorGUILayout.TextField("Name", partition.Name);
			GUILayout.BeginHorizontal();
			{
				GUI.enabled = false;
				partition.AssetName = EditorGUILayout.TextField("Asset Name", partition.AssetName);
				GUI.enabled = true;
				
				partition.AssetObject = EditorGUILayout.ObjectField(partition.AssetObject, typeof(GameObject), false, GUILayout.Width(160)) as GameObject;
				if(partition.AssetObject != null)
				{
					partition.AssetName = partition.AssetObject.name;
				}
				
			}
			GUILayout.EndHorizontal();
			link.LinkedPartitions[index] = partition;
		}
		
	}
}
