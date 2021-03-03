using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CharacterPartition))]
public class CharacterPartitionInspector : Editor
{
	private static Vector2 m_ScrollPosition = Vector2.zero;
	private static GameObject m_FbxInstance = null;
	void OnEnable()
	{
		if(m_FbxInstance != null)
		{
			DestroyImmediate(m_FbxInstance);
			m_FbxInstance = null;
		}
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		GUILayout.Space(8);
		m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
		CharacterPartition partition = target as CharacterPartition;
		partition.m_FBXSource = EditorGUILayout.ObjectField("FBX Source", partition.m_FBXSource, typeof(GameObject), false) as GameObject;
		if(partition.m_FBXSource != null)
		{
			if(m_FbxInstance == null || m_FbxInstance.name != partition.m_FBXSource.name)
			{
				if(m_FbxInstance != null)
				{
					DestroyImmediate(m_FbxInstance);
					partition.ClearMeshMaterialInfos();
				}

				m_FbxInstance = GameObject.Instantiate(partition.m_FBXSource) as GameObject;
				m_FbxInstance.name = partition.m_FBXSource.name;
			}

			SkinnedMeshRenderer[] skinnedMeshes = m_FbxInstance.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach(SkinnedMeshRenderer mesh in skinnedMeshes)
			{
				NGUIEditorTools.BeginContents();
				{
					if(NGUIEditorTools.DrawHeader(string.Format("{0} Material Settings", mesh.name)))
					{
						if(!partition.IsMeshMaterialOverride(mesh.name))
						{
							partition.RegisterMesh(mesh.name, mesh.sharedMaterials);
						}

						CharacterPartition.MeshMaterialInfo meshMatInfo = partition.GetMeshMaterialInfo(mesh.name);
						if(string.IsNullOrEmpty(meshMatInfo.MeshName))
						{
							meshMatInfo.MeshName = mesh.name;
						}
						meshMatInfo.MaterialCount = EditorGUILayout.IntField("Material Count", meshMatInfo.MaterialCount);
						if(meshMatInfo.MaterialCount == 0)
						{
							meshMatInfo.Materials = null;
						}
						else
						{
							if(meshMatInfo.Materials == null)
							{
								meshMatInfo.Materials = new Material[meshMatInfo.MaterialCount];
							}
							else
							{
								if(meshMatInfo.Materials.Length != meshMatInfo.MaterialCount)
								{
									Material[] materials = new Material[meshMatInfo.MaterialCount];
									for(int i = 0; i < meshMatInfo.MaterialCount; i++)
									{
										if(i < meshMatInfo.Materials.Length)
										{
											materials[i] = meshMatInfo.Materials[i];
										}
										else
										{
											materials[i] = null;
										}
									}
									meshMatInfo.Materials = materials;
								}
							}
							//bool needUpdate = false;
							for(int i = 0; i < meshMatInfo.MaterialCount; i++)
							{
								//Material prevMat = meshMatInfo.Materials[i];
								meshMatInfo.Materials[i] = EditorGUILayout.ObjectField(meshMatInfo.Materials[i], typeof(Material), false) as Material;
								//if(prevMat != meshMatInfo.Materials[i])
								//{
									//needUpdate = true;
								//}
							}

							///if(needUpdate)
							//{
							if(m_FbxInstance != null)
							{
								bool needMaterialUpdate = false;
								if(mesh.sharedMaterials != null)
								{
									if(meshMatInfo.Materials == null)
									{
										needMaterialUpdate = true;
									}
									else
									{
										if(mesh.sharedMaterials.Length != meshMatInfo.Materials.Length)
										{
											needMaterialUpdate =  true;
										}
										else
										{
											for(int i = 0; i < mesh.sharedMaterials.Length; i++)
											{
												if(mesh.sharedMaterials[i] != meshMatInfo.Materials[i])
												{
													needMaterialUpdate = true;
													break;
												}
											}
										}
									}
								}
								else
								{
									if(meshMatInfo.Materials != null)
									{
										needMaterialUpdate = true;
									}
								}

								if(needMaterialUpdate)
								{
									mesh.sharedMaterials = meshMatInfo.Materials;
								}
							}
							//}
						}

						partition.RegisterMesh(meshMatInfo.MeshName, meshMatInfo.Materials);
					}

				}
				NGUIEditorTools.EndContents();
			}
		}
		else
		{
			if(m_FbxInstance != null)
			{
				DestroyImmediate(m_FbxInstance);
				m_FbxInstance = null;
				partition.ClearMeshMaterialInfos();
			}
		}

		GUILayout.EndScrollView();
	}
}
