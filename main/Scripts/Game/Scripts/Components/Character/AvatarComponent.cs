using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 角色组件[都是代码动态挂载上去的]
/// </summary>
[ExecuteInEditMode]
public class AvatarComponent : MonoBehaviour
{
	public Dictionary<string, PartitionObject> Partitions
	{
		get { return m_Partitions; }
	}

	public bool Ready
	{
		get { return m_Partitions.Count > 0 && m_ToLoad.Count == 0; }
	}
    /// <summary>
    /// 预先Transform的名称[因为是代码挂载上去的所有这个属性为空]
    /// </summary>
	public string PreviewTransformName
	{
		get; set;
	}

	public string RootBoneTransformName
	{
		get; set;
	}
    /// <summary>
    /// 是否播放动画
    /// </summary>
	public bool SyncLoad
	{
		get; set;
	}
    /// <summary>
    /// 全路径hash值
    /// </summary>
	public int FullPathHash
	{
		get; set;
	}
    /// <summary>
    /// 所有的部件[]
    /// </summary>
	private Dictionary<string, PartitionObject> m_Partitions = new Dictionary<string, PartitionObject>();
    /// <summary>
    /// 所有的骨骼[名称，对象]
    /// </summary>
	private Dictionary<string, Transform> m_SkeletonBonesDic = new Dictionary<string, Transform>();
	private List<Transform> m_CombinedBones = new List<Transform>();
    /// <summary>
    /// 记录已经加载过的部件资源名称[印象最多是3个]
    /// </summary>
	private List<string> m_ToLoad = new List<string>(3);

    public int GetToLoadCount() //add by pj 检测模型是否加载完成的终极函数
    {
        return m_ToLoad.Count;
    }

    private void OnEnable()
    {
        setShaderTime = 0;
        //renderers = this.GetComponentsInChildren<SkinnedMeshRenderer>(true);
    }
    /// <summary>
    /// 初始化角色组件
    /// </summary>
    public void InitAvatar()
	{
        //获取所有骨骼对象
		Transform[] skeletonBones = gameObject.GetComponentsInChildren<Transform>();

		int skeletonCount = skeletonBones.Length;
		m_SkeletonBonesDic.Clear();
		for (int i = 0; i < skeletonCount; i++)
		{
            if (m_SkeletonBonesDic.ContainsKey(skeletonBones[i].name))
            {
                m_SkeletonBonesDic[skeletonBones[i].name] = skeletonBones[i];
            }
            else
            {
                m_SkeletonBonesDic.Add(skeletonBones[i].name, skeletonBones[i]);
            }
		}

		Animator animator = GetComponent<Animator>();
		if (animator != null)
		{
			animator.enabled = !SyncLoad;
		}
		UpdatePartitions();
        //Debug.LogError("init avatar");
        setShaderTime = 0;
    }
    /// <summary>
    /// 设置Shader次数，达到最大次数将不再设置
    /// </summary>
    private int setShaderTime = 0;
    private const int TotalShaderTime = 200;

    private eLightDir mCurrLightDir = eLightDir.nullData;

    private Animator mAnimator = null;

    private bool needUpdateShader = true;
    
    private int RenderCount = 0;

    private SkinnedMeshRenderer[] renderers;

    private void Update()
    {
        //CheckRender();
        SetShaderVar();
    }

    private void CheckRender()
    {
        int currentRenderCount = 0;
        renderers = this.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            currentRenderCount++;
        }

        if (currentRenderCount > RenderCount)
        {
            needUpdateShader = true;
            RenderCount = currentRenderCount;
        }
        //else
        //{
        //    needUpdateShader = false;
        //}
    }


    private void SetShaderVar()
    {
        if (renderers == null)
        {
            return;
        }

        RenderSettingsBase setting = RenderSettingsManager.Instance.GetCurrentRenderSettings();
        if (mCurrLightDir == setting.LightDir && !needUpdateShader)
        {
            return;
        }
        
        needUpdateShader = false;

        if (this == null)
        {
            return;
        }

        if (setShaderTime >= TotalShaderTime)
        {
            return;
        }

		bool isCombat = Hotfix_LT.UI.LTCombatEventReceiver.Instance == null ? false : Hotfix_LT.UI.LTCombatEventReceiver.Instance.Ready;
		bool isReadyUI = (bool)GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.GlobalMenuManager", "GetCurrentWinFromILR");

		if (renderers.Length == 0)
        {
            setShaderTime++;
        }
        else
        {
            mCurrLightDir = setting.LightDir;
        }
        for (int i = 0; i < renderers.Length; i++)
        {
            Vector4 thisRotation = setting.GetLightDir();
            for (int j = 0; j < renderers[i].materials.Length; j++)
            {
                var mat = renderers[i].materials[j];
                mat.SetVector("_LightDir", thisRotation);
                
                if (isReadyUI)
                {
                    mat.DisableKeyword("EBG_RIM_MAP_ON");
                    mat.EnableKeyword("EBG_RIM_MAP_OFF");
                }
                else
                {
                    mat.EnableKeyword("EBG_RIM_MAP_ON");
                    mat.DisableKeyword("EBG_RIM_MAP_OFF");
                }
                if (isCombat)
                {
                    mat.SetFloat("_UseViewDir", 1.0f);
                }
                else
                {
                    mat.SetFloat("_UseViewDir", 0.0f);
                }
            }
        }
    }
    /// <summary>
    /// 更新部件
    /// </summary>
    void UpdatePartitions()
	{
		Dictionary<string, PartitionObject>.Enumerator enumerator = m_Partitions.GetEnumerator();
        //应该是不存在的~因为这个属性不存在
		Transform prevTransform = gameObject.transform.Find(PreviewTransformName);
		if (prevTransform != null)
		{
#if UNITY_EDITOR
			GameObject.DestroyImmediate(prevTransform.gameObject);
#else
			GameObject.Destroy(prevTransform.gameObject);
#endif
		}

		while (enumerator.MoveNext())
		{
			PartitionObject partition = enumerator.Current.Value;
			if (partition != null)
			{
				m_ToLoad.Add(partition.AssetName);
				partition.LoadAsset(OnAssetLoaded, gameObject);
			}
		}
		enumerator.Dispose();
	}

    /// <summary>
    /// 加载完成（有可能是从缓存池拿出来的）
    /// </summary>
    /// <param name="name">部件名称</param>
    /// <param name="assetName">资源名称</param>
    /// <param name="partitionObj">部件对象</param>
    /// <param name="isSuccess">是否加载成功</param>
    /// <param name="isLinkObj">是否有部件信息组件(现在应该没有预置上挂载有这个部件组件的了:false)</param>
	void OnAssetLoaded(string name, string assetName, GameObject partitionObj, bool isSuccess, bool isLinkObj)
	{
		m_ToLoad.Remove(assetName);

		if (!isSuccess)
		{
			EB.Debug.LogError(string.Format("[AvatarComponent]Failed to load asset {0}", assetName));
			return;
		}

		if (partitionObj == null)
		{
			EB.Debug.LogError(string.Format("[AvatarComponent]Load asset {0}  to null instance.", assetName));
			return;
		}

        EB.Debug.LogObjectMgrAsset("<color=#00ff00>拿到</color>相应的资源,name:<color=#00ff00>{0}</color>,assetName:<color=#00ff00>{1}</color>,partitionObj:<color=#00ff00>{2}</color>", name, assetName, partitionObj);
		UpdatePartition(name, assetName, partitionObj, isLinkObj);

		//GM.AssetManager.UnRefAsset(assetName, false);

		if (m_ToLoad.Count == 0)
		{
			Rebind(FullPathHash);
		}
	}
	
    /// <summary>
    /// 更新部件
    /// </summary>
    /// <param name="name">部件名称</param>
    /// <param name="assetName">资源名称</param>
    /// <param name="partitionObj">部件数据对象</param>
    /// <param name="isLinkObj">是否有部件信息组件</param>
	void UpdatePartition(string name, string assetName, GameObject partitionObj, bool isLinkObj = false)
	{
		if (!m_Partitions.ContainsKey(name))
		{
			EB.Debug.LogError(string.Format("更新部件,发现这个部件不存在部件列表当中,: {0} ,部件的资源名称: {1}!", name, assetName));
		}

		PartitionObject partition = m_Partitions[name];
		if (partition == null)
		{
			return;
		}
        
		if (!isLinkObj)//现在默认都是没有部件列表的组件了,每次都进去
        {
			partition.ClearMeshObjects();
		}

		//initialize character partition: load skinnedmeshes and set materials
		CharacterPartition characterPartition = partitionObj.GetComponent<CharacterPartition>();
		if (characterPartition != null)
		{
			characterPartition.Initialize();
		}

		ColorCustomization customization = partitionObj.GetComponent<ColorCustomization>();
		if (customization != null)
		{
			customization.ApplyColor();
		}
        bool isChildrenPatition = IsChildrenPartition(name, assetName);
        //日志
        EB.Debug.LogObjectMgrAsset("<color=#00ff00>更新合并部件的骨骼:{0}</color>,,assetName:<color=#00ff00>{1}</color>是否为这个角色的子级:<color=#00ff00>{2}</color>,isLinkObj:<color=#00ff00>{3}</color>"
            , name, assetName, isChildrenPatition, isLinkObj);
        //
        if (isChildrenPatition)
		{
            //更新部件,先注掉，因为业务逻辑已经过老了
			UpdateChildrenPartition(partition, name, assetName, partitionObj);
        }
		else
		{
			try
			{
				UpdateCombineBonePartition(partition, name, assetName, partitionObj, isLinkObj);
			}
			catch(System.NullReferenceException e)
			{
				EB.Debug.LogError(e.ToString());
			}
			partition.UnregisterObject(partitionObj);
		}

		if (SyncLoad && m_ToLoad.Count == 0)
		{
			var iter = m_Partitions.GetEnumerator();
			while (iter.MoveNext())
			{
				iter.Current.Value.ShowMeshObjects();
			}
			iter.Dispose();
		}

		CharacterColorScale colorScale = GetComponent<CharacterColorScale>();
		if (colorScale != null)
		{
			colorScale.ForceUpdateColorScale();
		}
	}

	bool IsChildrenPartition(string name,string assetName)
	{
		try
		{
			Transform partitionAnchor = (transform == null) ? null : transform.Find(name);

			if (partitionAnchor == null)
			{
				return false;
			}

			return true;
		}
		catch(NullReferenceException exception)
		{
			EB.Debug.LogWarning("已捕获空引用异常,直接返回false。 exception: {0}", exception.ToString());
			return false;
		}
	}

    /// <summary>
    /// need combine bone partition example：Armor
    /// 合并骨骼部件
    /// </summary>
    /// <param name="partition"></param>
    /// <param name="name"></param>
    /// <param name="assetName"></param>
    /// <param name="partitionObj">部件数据组件</param>
    /// <param name="isLinkObj"></param>
    void UpdateCombineBonePartition(PartitionObject partition, string name, string assetName, GameObject partitionObj, bool isLinkObj = false)
	{
		if (gameObject == null)
		{
			EB.Debug.LogError("This AvatarComponent has been destroyed!");
			return;
		}

        //获取数据组件身上挂载的部件预置体相应的蒙皮
		SkinnedMeshRenderer[] meshes = partitionObj.GetComponentsInChildren<SkinnedMeshRenderer>();
		if (meshes == null || meshes.Length == 0)
		{
			if (partitionObj.GetComponentInChildren<CharacterPartitionLink>() == null)
			{
				EB.Debug.LogWarning("UpdatePartition: SkinnedMeshRenderer not found in {0}", assetName);
			}

			partition.UnregisterObject(partitionObj);
			return;
		}

		for (int meshIndex = meshes.Length - 1; meshIndex >= 0; meshIndex--)
		{
			SkinnedMeshRenderer smr = meshes[meshIndex];
			Transform root = null;
			m_CombinedBones.Clear();
			Transform[] bones = smr.bones;

			//reskin this thing to the new skeleton
			if (!m_SkeletonBonesDic.ContainsKey(smr.rootBone.name))
			{
				EB.Debug.LogError("No rootBone for {0}", assetName);
				return;
			}
			root = m_SkeletonBonesDic[smr.rootBone.name];
			if (!m_SkeletonBonesDic.ContainsKey(RootBoneTransformName))//for Optimized Game Object 
			{
				smr.bones = new Transform[0];
			}
			else
			{
				int boneCount = bones.Length;
				for (int boneIndex = 0; boneIndex < boneCount; boneIndex++)
				{
					Transform bone = bones[boneIndex];
					Transform combinedBone = null;
					if (m_SkeletonBonesDic.TryGetValue(bone.name, out combinedBone) && combinedBone != null)
					{
						m_CombinedBones.Add(combinedBone);
					}
				}
				smr.bones = m_CombinedBones.ToArray();
			}

			smr.rootBone = root;
			smr.transform.SetParent(gameObject.transform);
			smr.gameObject.CustomSetActive(!SyncLoad);
			partition.RegisterMeshObjects(smr.gameObject);

			smr.gameObject.layer = gameObject.layer;

            CheckRender();
        }
	}

	/// <summary>
	/// only put in one transform child partition example：Wing
	/// Anchor Name = name
	/// </summary>
	void UpdateChildrenPartition(PartitionObject partition, string name, string assetName, GameObject partitionObj)
	{
		Transform partitionAnchorDataTransform = transform.Find(name);
		if (partitionAnchorDataTransform == null)
		{
			EB.Debug.LogWarning("UpdateChildrenPartition:Partition Anchor Data lost  Need to Add!!", assetName);
			partition.UnregisterObject(partitionObj);
			return;
		}

		ChildPartitionAnchorDataComponent anchordata = partitionAnchorDataTransform.GetComponent<ChildPartitionAnchorDataComponent>();
		Transform partitionAnchor = anchordata.mAnchorBone;
		if (partitionAnchor == null)
		{
			EB.Debug.LogWarning("UpdateChildrenPartition:Partition Anchor Bone: anchordata.mAnchorBone lost!!", assetName);
			partition.UnregisterObject(partitionObj);
			return;
		}

		//Vector3 pos = partitionObj.transform.localPosition;
		//partitionObj.transform.parent = partitionAnchor;
		partitionObj.transform.SetParent(partitionAnchor, false);
		partitionObj.transform.localPosition = partitionAnchorDataTransform.localPosition;
		partitionObj.transform.localScale= partitionAnchorDataTransform.localScale;
		partitionObj.CustomSetActive(!SyncLoad);
		partition.RegisterMeshObjects(partitionObj);
		SetLayer(partitionObj, gameObject.layer);
    }

	public void RemovePartition(string partitionName)
	{
		if (m_Partitions.ContainsKey(partitionName))
		{
			m_Partitions[partitionName].ClearMeshObjects();
			m_Partitions.Remove(partitionName);
		}
	}

	public bool LoadEquipment(string name, string assetName)
	{
		if (m_Partitions.ContainsKey(name))
		{
			PartitionObject partition1 = m_Partitions[name];
			if (partition1.AssetName.Equals(assetName))
				return false;
		}
		AddPartition(name, assetName);
		PartitionObject partition = m_Partitions[name];
		if (partition == null)
		{
			return false;
		}

		partition.AssetName = assetName;

		m_ToLoad.Add(partition.AssetName);
		partition.LoadAsset(OnAssetLoaded, gameObject);
		return true;
	}

	public void AddPartition(string partitionName, string assetName)
	{
		PartitionObject partition = null;
		if (m_Partitions.ContainsKey(partitionName))
		{
			partition = m_Partitions[partitionName];
			if (partition == null)
			{
				partition = CreatePartitionObject(partitionName, assetName);
				m_Partitions[partitionName] = partition;
			}
			else
			{
				m_Partitions[partitionName].AssetName = assetName;
			}
		}
		else
		{
			partition = CreatePartitionObject(partitionName, assetName);
			m_Partitions.Add(partitionName, partition);
		}
	}

	PartitionObject CreatePartitionObject(string name, string assetName)
	{
		PartitionObject partition = new PartitionObject();
		partition.Name = name;
		partition.AssetName = assetName;

		return partition;
	}

	public void UpdateEquipmentColor(string name, int colorIndex)
	{
		if (!m_Partitions.ContainsKey(name))
		{
			EB.Debug.LogError(string.Format("[AvatarComponent]Current character does NOT contain partition named {0}", name));
			return;
		}

		PartitionObject partition = m_Partitions[name];
		partition.UpdateEquipmentColor(colorIndex);
	}

    /// <summary>
    /// 重新绑定
    /// </summary>
    /// <param name="fullPathHash">资源相应的hash值</param>
	private void Rebind(int fullPathHash)
	{
		Animator animator = GetComponent<Animator>();
		if (animator != null)
		{
			animator.enabled = true;
			if (animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
			{
				int layer = 0;

				// store state
				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
				if (animator.IsInTransition(layer))
				{
					stateInfo = animator.GetNextAnimatorStateInfo(layer);
				}
				fullPathHash = fullPathHash == 0 ? stateInfo.fullPathHash : fullPathHash;
				float normalizedTime = stateInfo.fullPathHash == fullPathHash ? stateInfo.normalizedTime : 0;

				int paramerterCount = animator.parameterCount;
				int[] parameters = new int[paramerterCount];

				// store state
				for (int i = 0; i < paramerterCount; ++i)
				{
					if (animator.parameters[i].type == AnimatorControllerParameterType.Int)
					{
						int value = animator.GetInteger(animator.parameters[i].name);
						parameters[i] = value;
					}
				}

				// rebind
				animator.Rebind();

				// restore state
				for (int i = 0; i < paramerterCount; ++i)
				{
					if (animator.parameters[i].type == AnimatorControllerParameterType.Int)
					{
						animator.SetInteger(animator.parameters[i].name, parameters[i]);
					}
				}

				animator.Play(fullPathHash, layer, normalizedTime);
			}
		}
	}

	static public void SetLayer(GameObject go, int layer)
	{
		go.layer = layer;

		Transform t = go.transform;

		for (int i = 0, imax = t.childCount; i < imax; ++i)
		{
			Transform child = t.GetChild(i);
			SetLayer(child.gameObject, layer);
		}
	}

#if UNITY_EDITOR
	public void PreviewCharacter(List<PartitionInfo> infos)
	{
		Transform[] skeletonBones = gameObject.GetComponentsInChildren<Transform>();
		int skeletonCount = skeletonBones.Length;
		m_SkeletonBonesDic.Clear();
		for (int i = 0; i < skeletonCount; i++)
		{
			m_SkeletonBonesDic.Add(skeletonBones[i].name, skeletonBones[i]);
		}

		Transform prevTransform = gameObject.transform.Find(PreviewTransformName);
		if (prevTransform != null)
		{
			GameObject.DestroyImmediate(prevTransform.gameObject);
		}

		for (int i = 0; i < infos.Count; i++)
		{
			PartitionInfo info  = infos[i];
			PartitionObject partition = new PartitionObject();
			partition.Name = info.Name;
			partition.AssetName = info.AssetName;
			m_Partitions.Add(info.Name, partition);
			if (info.AssetObject == null)
			{
				info.AssetObject = PartitionAssetManager.Instance.GetAssetObject(info.AssetName);

				if (info.AssetObject == null)
				{
					string assetPath = PartitionAssetManager.Instance.GetAssetPath(info.AssetName);
					if (!string.IsNullOrEmpty(assetPath))
					{
						info.AssetObject = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
						if (info.AssetObject != null)
						{
							PartitionAssetManager.Instance.RegisterAssetObject(info.AssetName, info.AssetObject);
						}
					}
				}
			}
			GameObject partitionObject = GameObject.Instantiate(info.AssetObject) as GameObject;
			List<PartitionInfo> linkedPartitionInfos = null;

			CharacterPartitionLink partitionLink = partitionObject.GetComponent<CharacterPartitionLink>();

			if (partitionLink != null)
			{
				linkedPartitionInfos = new List<PartitionInfo>();
				foreach (PartitionInfo linkedInfo in partitionLink.LinkedPartitions)
				{
					linkedPartitionInfos.Add(linkedInfo);
				}
			}

			UpdatePartition(info.Name, info.AssetName, partitionObject, false);

			if (linkedPartitionInfos != null)
			{
				for (int j = 0; j < linkedPartitionInfos.Count; j++)
				{
					PartitionInfo linkedInfo = linkedPartitionInfos[j];
					if (linkedInfo.AssetObject == null)
					{
						linkedInfo.AssetObject = PartitionAssetManager.Instance.GetAssetObject(linkedInfo.AssetName);
					}
					GameObject linkedObj = GameObject.Instantiate(linkedInfo.AssetObject) as GameObject;
					UpdatePartition(info.Name, linkedInfo.AssetName, linkedObj, true);
				}
			}
		}
	}
#endif
}
