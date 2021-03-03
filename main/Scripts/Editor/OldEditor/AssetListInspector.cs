using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(AssetList))]
public class AssetListInspector : Editor
{
	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Add Selected"))
		{
			string[] guids = Selection.assetGUIDs;

			List<UnityEngine.Object> assets = guids.Select(guid =>
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				var obj = AssetDatabase.LoadMainAssetAtPath(path);
				return obj;
			}).OrderBy(asset =>
			{
				if (asset is GameObject) return 0;
				if (asset is Material) return 1;
				if (asset is Shader) return 2;
				if (asset is Texture) return 3;
				return 4;
			}).ToList();

			foreach (var obj in assets)
			{
				if (ContainsAsset(obj))
				{
					Debug.Log("Add Selected: duplicate asset " + obj.name, obj);
					continue;
				}

				if (obj is GameObject)
				{
					AddPrefab(obj as GameObject);
				}
				else if (obj is Material)
				{
					AddMaterial(obj as Material);
				}
				else if (obj is Shader)
				{
					AddShader(obj as Shader);
				}
				else if (obj is Texture)
				{
					AddTexture(obj as Texture);
				}
				else
				{
					Debug.Log("Add Selected: Skip " + obj.name, obj);
				}
			}

			EditorUtility.SetDirty(target);
		}

		if (GUILayout.Button("Remove Selected"))
		{
			string[] guids = Selection.assetGUIDs;

			List<UnityEngine.Object> assets = guids.Select(guid =>
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				var obj = AssetDatabase.LoadMainAssetAtPath(path);
				return obj;
			}).ToList();

			foreach (var obj in assets)
			{
				if (obj is Material)
				{
					RemoveMaterial(obj as Material);
				}
				else if (obj is Shader)
				{
					RemoveShader(obj as Shader);
				}
				else if (obj is GameObject)
				{
					RemovePrefab(obj as GameObject);
				}
				else if (obj is Texture)
				{
					RemoveTexture(obj as Texture);
				}
				else
				{
					Debug.Log("Remove Selected: Skip " + obj.name, obj);
				}
			}

			EditorUtility.SetDirty(target);
		}

		if (GUILayout.Button("Collect FX From Selected"))
		{
			string[] guids = Selection.assetGUIDs;

			List<UnityEngine.GameObject> assets = guids.Select(guid =>
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				var obj = AssetDatabase.LoadMainAssetAtPath(path);
				return obj;
			}).Where(item => item is GameObject).Select(item => item as GameObject).ToList();

			foreach (var obj in assets)
			{
				foreach (var fxlib in obj.GetComponentsInChildren<FXLib>())
				{
					AddPrefabs(fxlib._fxParticleList);
					AddPrefabs(fxlib._fxTrailList);
					AddPrefabs(fxlib._fxLightList);
					AddPrefabs(fxlib._projectilePrefabList);
				}
			}
		}

		if (GUILayout.Button("Remove Empty"))
		{
			AssetList list = target as AssetList;

			list._materialList = list._materialList.Where(item => item != null).ToArray();
			list._shaderList = list._shaderList.Where(item => item != null).ToArray();
			list._prefabList = list._prefabList.Where(item => item != null).ToArray();
			list._textureList = list._textureList.Where(item => item != null).ToArray();
		}

		base.OnInspectorGUI();
	}

	bool ContainsAsset(UnityEngine.Object asset)
	{
		return EditorUtility.CollectDependencies(new Object[] { target }).Contains(asset);
	}

	void AddMaterial(Material mat)
	{
		AssetList list = target as AssetList;
		list._materialList = list._materialList.Concat(new Material[] { mat }).Where(item => item != null).ToArray();
	}

	void RemoveMaterial(Material mat)
	{
		AssetList list = target as AssetList;
		list._materialList = list._materialList.Where(item => item != mat).Where(item => item != null).ToArray();
	}

	void AddShader(Shader shader)
	{
		AssetList list = target as AssetList;
		list._shaderList = list._shaderList.Concat(new Shader[] { shader }).Where(item => item != null).ToArray();
	}

	void RemoveShader(Shader shader)
	{
		AssetList list = target as AssetList;
		list._shaderList = list._shaderList.Where(item => item != shader).Where(item => item != null).ToArray();
	}

	void AddPrefab(GameObject prefab)
	{
		AssetList list = target as AssetList;
		list._prefabList = list._prefabList.Concat(new GameObject[] { prefab }).Where(item => item != null).ToArray();
	}

	void AddPrefabs(IEnumerable<GameObject> prefabs)
	{
		AssetList list = target as AssetList;
		list._prefabList = list._prefabList.Concat(prefabs).Where(item => item != null).ToArray();
	}

	void RemovePrefab(GameObject prefab)
	{
		AssetList list = target as AssetList;
		list._prefabList = list._prefabList.Where(item => item != prefab).Where(item => item != null).ToArray();
	}

	void AddTexture(Texture tex)
	{
		AssetList list = target as AssetList;
		list._textureList = list._textureList.Concat(new Texture[] { tex }).Where(item => item != null).ToArray();
	}

	void RemoveTexture(Texture tex)
	{
		AssetList list = target as AssetList;
		list._textureList = list._textureList.Where(item => item != tex).Where(item => item != null).ToArray();
	}
}
