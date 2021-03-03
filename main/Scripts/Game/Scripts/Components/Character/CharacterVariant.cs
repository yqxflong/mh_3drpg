using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 角色变体数据组件
/// [有三种:1战斗使用,2主城使用,3:UI使用]
/// </summary>
[ExecuteInEditMode]
public class CharacterVariant : MonoBehaviour 
{
	[HideInInspector][SerializeField]
	public string PreviewTransform = DefaultPreviewTransform;
	[HideInInspector][SerializeField]
	public string RootBoneTransform = DefaultRootBoneTransform;
    /// <summary>
    /// 是否为英雄部件
    /// </summary>
	[HideInInspector][SerializeField]
	public bool IsPlayer = false;

	const string DefaultPreviewTransform = "Preview";
	const string DefaultRootBoneTransform = "Bip001";
    /// <summary>
    /// 部件信息列表
    /// </summary>
	[SerializeField]
	List<PartitionInfo> m_Partitions = new List<PartitionInfo>();
    /// <summary>
    /// 引用的预置体
    /// </summary>
	[SerializeField]
	GameObject m_MoveSetPrefab;


    private FXLib mFxLib = null;

	public GameObject MoveSetPrefab
	{
		get
		{
			return m_MoveSetPrefab;
		}
		set
		{
			m_MoveSetPrefab = value;
		}
	}

	public List<PartitionInfo> Partitions
	{
		get
		{
			return m_Partitions;
		}
		set
		{
			m_Partitions = value;
		}
	}
    /// <summary>
    /// 是否播放动画
    /// </summary>
	public bool SyncLoad
	{
		get;
		set;
	}
    /// <summary>
    /// 变体引用的预置体实例化对象
    /// </summary>
	private GameObject m_CharacterInstance = null;
    /// <summary>
    /// 变体引用的预置体实例化对象
    /// </summary>
	public GameObject CharacterInstance
	{
		get
		{
			return m_CharacterInstance;
		}
	}

	void Start()
	{
#if UNITY_EDITOR
		if(Application.isEditor && !Application.isPlaying)
		{
			PreviewCharacter();
		}
#endif
	}

	void OnDestroy()
	{
#if UNITY_EDITOR
		if(Application.isEditor && !Application.isPlaying)
		{
			string previewName = string.Format("__Preview_{0}", gameObject.name);
			GameObject go = GameObject.Find(previewName);
			if(go != null)
			{
				GameObject.DestroyImmediate(go);
			}
		}

        for (int i=0;i< m_Partitions.Count;i++)
        {
            DestroyImmediate(m_Partitions[i].AssetObject);
        }
#endif
	}


    private void OnTimerUpInitCharacter(int seq, object arg)
    {
        TimerManager.instance.RemoveTimer(OnTimerUpInitCharacter);

        IDictionary partitions = arg as IDictionary;
        if (m_CharacterInstance == null)
        {
            return;
        }
        AvatarComponent avatar = m_CharacterInstance.GetComponent<AvatarComponent>();
        if (avatar == null)
        {
            avatar = m_CharacterInstance.AddComponent<AvatarComponent>();
            int partitionCount = m_Partitions.Count;
            for (int i = 0; i < partitionCount; i++)
            {
                if (partitions != null && partitions.Contains(m_Partitions[i].Name))
                {
                    avatar.AddPartition(m_Partitions[i].Name, partitions[m_Partitions[i].Name].ToString());

                    if (IsPlayer)
                    {
                        PartitionObject.PlayerPartitions.Add(partitions[m_Partitions[i].Name].ToString());
                    }
                }
                else
                {
                    avatar.AddPartition(m_Partitions[i].Name, m_Partitions[i].AssetName);

                    if (IsPlayer)
                    {
                        PartitionObject.PlayerPartitions.Add(m_Partitions[i].AssetName);
                    }
                }
            }
        }
        OnTimerUpInitAvatar(0, avatar);
    }

    private void OnTimerUpInitAvatar(int seq, object arg)
    {
        AvatarComponent avatar = arg as AvatarComponent;
        if (avatar == null)
            return;

        avatar.PreviewTransformName = string.IsNullOrEmpty(PreviewTransform) ? DefaultPreviewTransform : PreviewTransform;
        avatar.RootBoneTransformName = string.IsNullOrEmpty(RootBoneTransform) ? DefaultRootBoneTransform : RootBoneTransform;
        avatar.SyncLoad = SyncLoad;
        avatar.InitAvatar();
        m_CharacterInstance.transform.SetParent(transform.parent);
    }

    /// <summary>
    /// 初始化特效
    /// </summary>
    /// <param name="pName">预置体的名称</param>
    private void InitFX(string pName)
    {
        if (m_CharacterInstance == null)
        {
            return;
        }

        if (mFxLib == null)
        {
            mFxLib = m_CharacterInstance.GetComponent<FXLib>();
        }

        if (mFxLib == null)
        {
            return;
        }

        if (pName.IndexOf('P') == 0 || pName.IndexOf('M') == 0)
        {
            if (pName.Contains("Campaign"))
            {
                mFxLib.Type = FXLib.FXLibType.Campaign;
            }
            else if (pName.Contains("Lobby"))
            {
                mFxLib.Type = FXLib.FXLibType.Lobby;
            }
            else
            {
                mFxLib.Type = FXLib.FXLibType.Normal;
                string num = pName.Substring(1, pName.Length - 1);
                if(num.Contains("_"))//去除皮肤后缀
                {
                    string[] spl = num.Split('_');
                    num = spl[0];
                }
                mFxLib.BundleName = string.Format("{0}{1}", pName.Substring(0, 1).ToLower(), num.TrimStart('0'));
            }
            //
            EB.Debug.LogObjectMgrAsset(string.Format("通过模型变体组件<color=#00ff00>{0}</color>来设置<color=#ffff00>FxLib特效库</color>需要属性:Type:<color=#00ff00>{1}</color>,特效资源包:<color=#00ff00>{2}</color>"
                , pName, mFxLib.Type, mFxLib.BundleName));
        }
    }

    public void InstantiateCharacter(IDictionary partitions)
    {
        if (m_MoveSetPrefab != null)
        {
            if (m_CharacterInstance == null)
            {
                m_CharacterInstance = GameObject.Instantiate(m_MoveSetPrefab, transform.position, transform.rotation) as GameObject;
                m_CharacterInstance.name = m_MoveSetPrefab.name;
                //
                OnTimerUpInitCharacter(0,partitions);
                //
                EB.Debug.LogObjectMgrAsset("<color=#ff0000>克隆CharacterVariant变体身上记录的预置体</color>同时身上的部件进行初始化,<color=#00ff00>{0}</color>", m_MoveSetPrefab.name);
            }
            else
            {
                EB.Debug.LogPSPoolAsset(string.Format("<color=#00ff00>已经存在这个角色变体<color=#000000>{0}</color>,开启协程重新注册一次身上的所有特效</color>", this.name));
                //需要重新将特效列表~注册进入特效缓存池
                FXLib fxLib = m_CharacterInstance.GetComponent<FXLib>();
                if (fxLib != null)
                {
                    StartCoroutine(fxLib.TryRegisterAll());
                }
                else
                {
                    EB.Debug.LogError("为什么~模型对象身上没有这个FXLib特效组件了?");
                }

                m_CharacterInstance.transform.SetParent(transform.parent);
                m_CharacterInstance.CustomSetActive(true);
                AvatarComponent avatar = m_CharacterInstance.GetComponent<AvatarComponent>();
                if (avatar != null)
                {
                    avatar.SyncLoad = SyncLoad;

                    int partitionCount = m_Partitions.Count;
                    for (int i = 0; i < partitionCount; i++)
                    {
                        if (partitions != null && partitions.Contains(m_Partitions[i].Name))
                        {
                            string assetname = partitions[m_Partitions[i].Name].ToString();
                            avatar.LoadEquipment(m_Partitions[i].Name, assetname);

                            if (IsPlayer)
                            {
                                PartitionObject.PlayerPartitions.Add(assetname);
                            }
                        }
                    }
                }
            }
            InitFX(m_MoveSetPrefab.name);
        }
    }
    
    /// <summary>
    /// 实例化角色.将根据身上记录的部件数据组装
    /// </summary>
	public void InstantiateCharacter()
    {
        if (m_MoveSetPrefab != null)
        {
            if (m_CharacterInstance == null)
            {
                //
                EB.Debug.LogObjectMgrAsset("<color=#ff0000>克隆CharacterVariant变体身上记录的预置体</color>同时身上的部件进行初始化,<color=#00ff00>{0}</color>", m_MoveSetPrefab.name);

                m_CharacterInstance = GameObject.Instantiate(m_MoveSetPrefab, transform.position, transform.rotation) as GameObject;
                m_CharacterInstance.name = m_MoveSetPrefab.name;
                AvatarComponent avatar = m_CharacterInstance.GetComponent<AvatarComponent>();
                if (avatar == null)
                {
                    avatar = m_CharacterInstance.AddComponent<AvatarComponent>();
                    int partitionCount = m_Partitions.Count;
                    for (int i = 0; i < partitionCount; i++)
                    {
                        avatar.AddPartition(m_Partitions[i].Name, m_Partitions[i].AssetName);

                        if (IsPlayer)
                        {
                            PartitionObject.PlayerPartitions.Add(m_Partitions[i].AssetName);
                        }
                    }
                }
                avatar.PreviewTransformName = string.IsNullOrEmpty(PreviewTransform) ? DefaultPreviewTransform : PreviewTransform;
                avatar.RootBoneTransformName = string.IsNullOrEmpty(RootBoneTransform) ? DefaultRootBoneTransform : RootBoneTransform;
                avatar.SyncLoad = SyncLoad;
                avatar.InitAvatar();
                m_CharacterInstance.transform.SetParent(transform.parent);
            }
            else
            {
                //
                EB.Debug.LogObjectMgrAsset("<color=#00ffff>已经_克隆过这个预置体</color>:<color=#00ff00>{0}</color>,将其设置可见", m_MoveSetPrefab.name);

                m_CharacterInstance.transform.SetParent(transform.parent);
                m_CharacterInstance.CustomSetActive(true);
                AvatarComponent avatar = m_CharacterInstance.GetComponent<AvatarComponent>();
                if (avatar != null)
                {
                    avatar.SyncLoad = SyncLoad;

                    int partitionCount = m_Partitions.Count;
                    for (int i = 0; i < partitionCount; i++)
                    {
                        avatar.LoadEquipment(m_Partitions[i].Name, m_Partitions[i].AssetName);

                        if (IsPlayer)
                        {
                            PartitionObject.PlayerPartitions.Add(m_Partitions[i].AssetName);
                        }
                    }
                }
            }

            InitFX(m_MoveSetPrefab.name);
        }else
        {
            EB.Debug.LogError("[" + Time.frameCount + "]为什么角色变体没有相应的预置体呢?");
        }
        
        //TimerManager.instance.AddLateUpdateTimer(70, 1, OnTimerUp);
    }
    //private void OnTimerUp(int seq)
    //{
    //    int layer = transform.parent.gameObject.layer;
    //    //设置layer
    //    transform.parent.SetChildLayer(layer);
    //}

#if UNITY_EDITOR
	public void PreviewCharacter()
	{
		if(m_MoveSetPrefab == null)
		{
			return;
		}
		string previewName = string.Format("__Preview_{0}", gameObject.name);
		GameObject go = GameObject.Find(previewName);
		if(go != null)
		{
			GameObject.DestroyImmediate(go);
		}

		go = GameObject.Instantiate(m_MoveSetPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
		go.name = previewName;

		AvatarComponent avatar = go.AddComponent<AvatarComponent>();
		avatar.PreviewTransformName = string.IsNullOrEmpty(PreviewTransform) ? DefaultPreviewTransform : PreviewTransform;
		avatar.RootBoneTransformName = string.IsNullOrEmpty(RootBoneTransform) ? DefaultRootBoneTransform : RootBoneTransform;
		avatar.PreviewCharacter(m_Partitions);

        InitFX(m_MoveSetPrefab.name);
    }
#endif

	public string[] GetPartitionNames()
	{
		List<string> names = new List<string> ();

		foreach (PartitionInfo p in m_Partitions) 
		{
			names.Add (p.Name);
		}

		return names.ToArray();
	}

	public void Recycle()
	{
		if (m_CharacterInstance != null)
		{
			if (!m_CharacterInstance.activeSelf)
			{
				m_CharacterInstance.CustomSetActive(true);
			}
			m_CharacterInstance.transform.SetParent(transform);
			m_CharacterInstance.transform.localPosition = Vector3.zero;
		}
	}
}
