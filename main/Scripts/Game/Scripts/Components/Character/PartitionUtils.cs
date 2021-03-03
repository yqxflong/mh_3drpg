using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 部件信息
/// </summary>
[System.Serializable]
public struct PartitionInfo
{
    /// <summary>
    /// 名称
    /// </summary>
	public string Name;
    /// <summary>
    /// 资源名称
    /// </summary>
	public string AssetName;
	#if UNITY_EDITOR
	[System.NonSerialized]
	public GameObject AssetObject;
	#endif
}

#if UNITY_EDITOR
public class PartitionAssetManager
{
	private Dictionary<string, string> m_AssetRegistry = new Dictionary<string, string>();
	private Dictionary<string, GameObject> m_AssetObjectRegistry = new Dictionary<string, GameObject>();
	public const string CHARACTER_PARTITION_ASSET_PATH = "_GameAssets/Res/Character/CharacterPartitions";

	private PartitionAssetManager()
	{
	}

	private static PartitionAssetManager m_Instance;
	public static PartitionAssetManager Instance
	{
		get
		{
			if(m_Instance == null)
			{
				m_Instance = new PartitionAssetManager();
			}
			return m_Instance;
		}
	}

	public void ScanCharacterPartitionAssetPath()
	{
		m_AssetRegistry.Clear();

		string fullAssetPath = string.Format("{0}/{1}", Application.dataPath, CHARACTER_PARTITION_ASSET_PATH);
		IEnumerable<string> allPartitionAssets = System.IO.Directory.GetFiles(fullAssetPath, "*.prefab", System.IO.SearchOption.AllDirectories);

		foreach(string filePath in allPartitionAssets)
		{
			string assetName = System.IO.Path.GetFileNameWithoutExtension(filePath);
			string assetPath = string.Format("Assets/{0}", filePath.Substring(Application.dataPath.Length + 1));
			m_AssetRegistry.Add(assetName, assetPath);
		}
	}

	public string GetAssetPath(string assetName)
	{
		if(!m_AssetRegistry.ContainsKey(assetName))
		{
			ScanCharacterPartitionAssetPath();
		}

		if(m_AssetRegistry.ContainsKey(assetName))
		{
			return m_AssetRegistry[assetName];
		}
		else
		{
			EB.Debug.LogError(string.Format("Asset named {0} does NOT exist", assetName));
			return string.Empty;
		}
	}

	public void RegisterAssetObject(string assetName, GameObject obj)
	{
		if(m_AssetObjectRegistry.ContainsKey(assetName))
		{
			m_AssetObjectRegistry[assetName] = obj;
		}
		else
		{
			m_AssetObjectRegistry.Add(assetName, obj);
		}
	}

	public GameObject GetAssetObject(string assetName)
	{
		if(m_AssetObjectRegistry.ContainsKey(assetName))
		{
			return m_AssetObjectRegistry[assetName];
		}

		return null;
	}
}
#endif

/// <summary>
/// 部件管理器
/// </summary>
public class PartitionObject
{
    /// <summary>
    /// 加载过的部件资源名称
    /// </summary>
	private static HashSet<string> Loaded = new HashSet<string>();
    /// <summary>
    /// 英雄部件资源名称列表(目前游戏里没有添加过了英雄部件，为空的状态)
    /// </summary>
	public static HashSet<string> PlayerPartitions = new HashSet<string>();

    /// <summary>
    /// 释放与卸载
    /// </summary>
	public static void FlushAllAndUnload()
	{
        if (Loaded.Count == 0)
        {
            return;
        }
        EB.Debug.LogObjectMgrAsset("<color=#00ffff>****************打算-卸载之前加载过的所有身体部件****************</color>,Loaded.Count:{0}", Loaded.Count);
        //判断加载过的部件列表是否达到上限数 (10个模型*2个部件)
        if (Loaded.Count >= 0)
        {
#if USE_DEBUG

            StringBuilder str = new StringBuilder();
            int i = 0;
            foreach (string assetName in Loaded)
            {
                str.Append(string.Format(",[{0}]<color=#00ff00>{1}</color>", i, assetName));
                i++;
            }
            EB.Debug.LogObjectMgrAsset("<color=#ff0000>*****卸载之前加载过的所有身体部件*****</color>,个数:<color=#00ff00>{0}</color>,str:{1}", Loaded.Count, str);
#endif
            foreach (string assetName in Loaded)
            {
                PoolModel.ClearResource(assetName);
            }
            Loaded.Clear();
			EB.Assets.UnloadUnusedAssets();
        }
    }

    public delegate void PartitionAssetLoaded(string name, string assetName, GameObject obj, bool isSuccess, bool isLinkedObj);
	//3D model partition name: Armor, Face, Head, etc
	private string _name = string.Empty;
	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}
	private string _assetName = string.Empty;
    /// <summary>
    /// 资源名称
    /// </summary>
	public string AssetName
	{
		get
		{
			return _assetName;
		}
		set
		{
			_assetName = value;
		}
	}

	private List<GameObject> MeshObjects = new List<GameObject>();

	private PartitionAssetLoaded _onAssetLoaded;
    /// <summary>
    /// 挂载的父级对象
    /// </summary>
	private GameObject _target = null;

    /// <summary>
    /// 清除Mesh对象
    /// </summary>
	public void ClearMeshObjects()
	{
		if (MeshObjects == null || MeshObjects.Count == 0)
		{
			return;
		}

        for (int i = MeshObjects.Count - 1; i >= 0; i--)
        {
            PoolModel.DestroyModel(MeshObjects[i]);
            //#if UNITY_EDITOR
            //			GameObject.DestroyImmediate(MeshObjects[i]);
            //#else
            //			GameObject.Destroy(MeshObjects[i]);
            //#endif
        }
        
        MeshObjects.Clear();
	}

    /// <summary>
    /// 添加Mesh对象
    /// </summary>
    /// <param name="go"></param>
	public void RegisterMeshObjects(GameObject go)
	{
		if (go == null)
		{
			return;
		}
        
		MeshObjects.Add(go);
	}

    /// <summary>
    /// 清除部件对象
    /// </summary>
    /// <param name="go"></param>
	public void UnregisterObject(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		MeshObjects.Remove(go);
        
        //不要再去缓存池里移除
        PoolModel.DestroyModel(go);
    }

	public void UpdateEquipmentColor(int colorIndex)
	{
		for (int i = MeshObjects.Count - 1; i >= 0; i--)
		{
			ColorCustomization.ApplyEquipmentColor(MeshObjects[i], colorIndex);
		}
	}

	public void UpdateMipMapBias(float mipMapBias)
	{
		for (int i = MeshObjects.Count - 1; i >= 0; i--)
		{
			Renderer renderer = MeshObjects[i].GetComponent<Renderer>();
			if (renderer!=null && renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null)
			{
				renderer.sharedMaterial.mainTexture.mipMapBias = mipMapBias;
			}
		}
	}
    /// <summary>
    /// 加载指定的部件资源
    /// </summary>
    /// <param name="onAssetLoaded">资源加载完成回调</param>
    /// <param name="target">挂载的父级对象</param>
	public void LoadAsset(PartitionAssetLoaded onAssetLoaded, GameObject target)
	{
		if (string.IsNullOrEmpty(_assetName))
		{
			EB.Debug.LogError("[PartitionObject]Asset name is empty or null!");
			return;
		}
		_onAssetLoaded = onAssetLoaded;
		_target = target;
		PoolModel.GetModelAsync(_assetName, Vector3.zero, Quaternion.identity, (obj, prm)=>
		{
			OnOTALoaded(_assetName, obj as GameObject, true);
		}, null);
	}

	public void ShowMeshObjects()
	{
		if (MeshObjects == null)
		{
			return;
		}
		
		for (int i = MeshObjects.Count - 1; i >= 0; i--)
		{
			if (MeshObjects[i] != null)
			{
				MeshObjects[i].SetActive(true);
			}
		}
	}

    /// <summary>
    /// 部件加载成完
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <param name="partitionObj">部件数据对象</param>
    /// <param name="isSuccess">是否成功</param>
	void OnOTALoaded(string assetName, GameObject partitionObj, bool isSuccess)
	{
		Loaded.Add(assetName);
        List<PartitionInfo> linkedPartionInfos = null;
		if (isSuccess && partitionObj != null)
		{
            //从代码上看~现在应该是没有这个组件再挂载在部件上面了，要不就成了部件上面再加载部件的信息
			CharacterPartitionLink partitionLink = partitionObj.GetComponent<CharacterPartitionLink>();
			if (partitionLink != null)
			{
				linkedPartionInfos = new List<PartitionInfo>();
				for (int i = partitionLink.LinkedPartitions.Count - 1; i >= 0; i--)
				{
					PartitionInfo info = partitionLink.LinkedPartitions[i];
					linkedPartionInfos.Add(info);
				}
			}
		}

		if (_onAssetLoaded != null)
		{
			_onAssetLoaded(_name, assetName, partitionObj, isSuccess, false);
		}

        //要是部件信息存在的情况，就加载一个一个加载需要的部件,目前上看~是不会运行这段代码
		if (linkedPartionInfos != null)
		{
			for (int i = linkedPartionInfos.Count - 1; i >= 0; i--)
			{
				PartitionInfo info = linkedPartionInfos[i];
				// GM.AssetManager.GetAsset<GameObject>(info.AssetName, OnLinkedAssetLoaded, _target, true);
				EB.Assets.LoadAsyncAndInit<GameObject>(info.AssetName, OnLinkedAssetLoaded, _target);
			}
		}
	}

    /// <summary>
    /// 加载部件组件完成的情况
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="linkedObject"></param>
    /// <param name="isSuccess"></param>
	void OnLinkedAssetLoaded(string assetName, GameObject linkedObject, bool isSuccess)
	{
		Loaded.Add(assetName);

		if (_onAssetLoaded != null)
		{
			_onAssetLoaded(_name, assetName, linkedObject, isSuccess, true);
		}
	}
}