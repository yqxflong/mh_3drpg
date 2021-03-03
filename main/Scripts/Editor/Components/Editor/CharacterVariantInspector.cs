using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CharacterVariant))]
public class CharacterVariantInspector : Editor 
{
	private static Vector2 m_ScrollPosition = Vector2.zero;
	void OnEnable()
	{
		CharacterVariant variant = target as CharacterVariant;
		for(int i = 0; i < variant.Partitions.Count; i++)
		{
			string assetPath = PartitionAssetManager.Instance.GetAssetPath(variant.Partitions[i].AssetName);
			if(string.IsNullOrEmpty(assetPath))
			{
				continue;
			}
			PartitionInfo partition = variant.Partitions[i];
			partition.AssetObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
			variant.Partitions[i] = partition;
			PartitionAssetManager.Instance.RegisterAssetObject(partition.AssetName, partition.AssetObject);

			CharacterPartitionLink link = partition.AssetObject.GetComponent<CharacterPartitionLink>();
			if(link != null)
			{
				for(int j = 0; j < link.LinkedPartitions.Count; j++)
				{
					string linkedAssetPath = PartitionAssetManager.Instance.GetAssetPath(link.LinkedPartitions[j].AssetName);
					if(string.IsNullOrEmpty(linkedAssetPath))
					{
						continue;
					}
					PartitionInfo linkedPartition = link.LinkedPartitions[j];
					linkedPartition.AssetObject = AssetDatabase.LoadAssetAtPath(linkedAssetPath, typeof(GameObject)) as GameObject;
					link.LinkedPartitions[j] = linkedPartition;
					PartitionAssetManager.Instance.RegisterAssetObject(linkedPartition.AssetName, linkedPartition.AssetObject);
				}
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		GUILayout.Space(8);
		m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
		CharacterVariant variant = target as CharacterVariant;
		variant.PreviewTransform = EditorGUILayout.TextField("Preview Transform", variant.PreviewTransform);
		variant.RootBoneTransform = EditorGUILayout.TextField("RootBone Transform", variant.RootBoneTransform);
		variant.IsPlayer = EditorGUILayout.Toggle("Is Player", variant.IsPlayer);
		variant.MoveSetPrefab = EditorGUILayout.ObjectField("Move Set Prefab", variant.MoveSetPrefab, typeof(GameObject), false) as GameObject;
		if(GUILayout.Button("Preview Character"))
		{
			variant.PreviewCharacter();
		}
		DrawPartitionList();

		GUILayout.EndScrollView();
	}

	void DrawPartitionList()
	{
		CharacterVariant variant = target as CharacterVariant;
		if(NGUIEditorTools.DrawHeader(string.Format("Partition List ({0})", variant.Partitions.Count), "CharacterVariant_Partitions"))
		{
			NGUIEditorTools.BeginContents();
			{
				bool delete = false;
				for(int i = 0; i < variant.Partitions.Count; i++)
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
						variant.Partitions.RemoveAt(i);
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
						foreach(PartitionInfo partition in variant.Partitions)
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
					variant.Partitions.Add(newPartition);
				}
			}
			NGUIEditorTools.EndContents();
		}
	}

	void DrawPartition(int index, ref bool delete)
	{
		CharacterVariant variant = target as CharacterVariant;
		string prefsKey = string.Format("CharacterVariant_Partition_{0}", index);
		bool view = EditorPrefs.GetBool(prefsKey, true);

		PartitionInfo partition = variant.Partitions[index];
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
			variant.Partitions[index] = partition;
		}

	}
}