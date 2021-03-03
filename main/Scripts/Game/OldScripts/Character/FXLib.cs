using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using System.IO;
using GM;
using System.Text;

/// <summary>
/// 特效库组件
/// </summary>
public class FXLib : MonoBehaviour 
{
    public enum FXLibType
    {
        Normal,
        Campaign,
        Lobby,
    }

	public bool _temporary = true;
	public int _registerCount = 0;
	public int _fxTrailRegisterCount = 1;
	public int _fxLightRegisterCount = 1;
	public int _projectilePrefabRegisterCount = 1;
	public bool _destroy = false;
    
	public List<GameObject> _fxParticleList 	= new List<GameObject>();
	public GameObject[] _fxTrailList 		= new GameObject[0];
	public GameObject[] _fxLightList 		= new GameObject[0];
	public GameObject[] _projectilePrefabList = new GameObject[0];

    private string mEditorFolders = "Assets/Art/fx/particles/";
    private List<string> mPartilesUrls = new List<string>();
    private const int mIntervalTime = 1;
    private int mCurrentTime = 0;
    private bool isInit = false;
    private int loadingAssetCount = 0;

    private MoveController mMoveController = null;

    private FXLibType mType = FXLibType.Normal;
    public FXLibType Type
    {
        get
        {
            return mType;
        }

        set
        {
            mType = value;
        }
    }

    private string mBundleName = string.Empty;
    public string BundleName
    {
        get
        {
            return mBundleName;
        }

        set
        {
            mBundleName = value;
        }
    }

    private void Start()
    {
        RegisterAll();
        if (_destroy)
		{
			Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (isInit)
        {
            return;
        }
        //重新启动特效
        if (mMoveController == null)
        {
            mMoveController = gameObject.GetComponent<MoveController>();
        }
        if (mMoveController != null)
        {
            mMoveController.SetMove("Idle");
        }
        isInit = true;
    }

    private void OnDisable()
    {
        PSPoolManager.Instance.UpdateData();
        isInit = false;
    }

    private void Update()
    {
        if (mPartilesUrls.Count > 0)
        {
            loadingAssetCount++;
            var obj = mPartilesUrls[0];
            GetAsset<GameObject>(obj, OnAssetReady, gameObject, true, null, false, false);
            mPartilesUrls.RemoveAt(0);
        }
    }

    public bool NotLoadFX()
    {
        return isInit && mPartilesUrls.Count <= 0 && loadingAssetCount <= 0;
    }

    private void LoadCombatantFX()
    {
        mPartilesUrls.Clear();
        List<string> includes = AssetManager.Instance.GetBundleInCludes(mBundleName);
        StringBuilder str = new StringBuilder();

        if (includes != null)
        {
            for (int i = 0; i < includes.Count; i++)
            {
                string include = includes[i];
                if (!mPartilesUrls.Contains(include))
                {
                    str.Append(string.Format("[{0}]<color=#00ff00>{1}</color>", mPartilesUrls.Count, include));
                    
                    if (IsHeroStartEffect(include))
                    {
                        mPartilesUrls.Insert(0, include);
                    }
                    else
                    {
                        mPartilesUrls.Add(include);
                    }
                }
            }

            //为了让第一个特效一开始就加载
            mCurrentTime = 2;
        }

        //日志
        EB.Debug.LogPSPoolAsset(string.Format("<color=#00ff00>{0}</color>加载这个资源包:<color=#00ff00>{1}</color>相应的资源:{2}",this.name, mBundleName, str));
    }

    public bool IsHeroStartEffect(string assetName)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            return false;
        }
        //居然把出场写成chushang，我也没办法(之前的人留下注解)
        return assetName.Contains("chuchang") || assetName.Contains("chushang");
    }

    private void GetAsset<T>(string assetname, System.Action<string, T, bool> action, GameObject target, bool skipUnload = false, GameObject parent = null, bool isParentNeed = false, bool isNeedInstantiate = true) where T : UnityEngine.Object
    {
        ParticleSystem ps = PSPoolManager.Instance.Use(this,assetname);
        T data = default(T);
        if (ps != null)
        {
            data = ps.gameObject as T;
        }
        
        if (data != null)
        {
            if (action != null)
            {
                action(assetname, data, data != null);
                return;
            }
        }
        else
        {
            string path = GM.AssetManager.GetAssetFullPathByAssetName(assetname);
            EB.Assets.LoadAsyncAndInit<T>(assetname, action, target, parent, isNeedInstantiate);
        }
    }

    private void OnAssetReady(string assetname, GameObject obj, bool isSuccessed)
    {
        if (!isSuccessed)
        {
            EB.Debug.LogError(string.Format("Asset {0} dont find", assetname));
        }
        else
        {
            if (this == null)
            {
                EB.Debug.LogError("<color=#ff0000>加载的资源完成的~发现寄主对象已经不存在了，就直接清除掉刚才加载的对象:</color>"+ assetname);
                Destroy(obj);
            }
            else
            {
                if (IsHeroStartEffect(obj.name))
                {
                    GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "BossParticleInSceneComplete");
                }
                if (!_fxParticleList.Contains(obj))
                {
                    _fxParticleList.Add(obj);
                }
            }
        }
        TimerManager.instance.AddFramer(0, 1, RegisterFxParticles, obj);
        loadingAssetCount--;
    }

    private void RegisterFxParticles(int seq, object arg)
    {
        GameObject go = arg as GameObject;
        if (go != null && !ParticlePal.WillDelete(go))
        {
            PSPoolManager.Instance.Register(go, 1, PSPoolManager.Persistence.Temp);
        }
    }

    private void RegisterAllFxParticles()
    {
        for (int i = 0; i < _fxParticleList.Count; i++)
        {
            ParticleSystem[] ps = _fxParticleList[i].GetComponentsInChildren<ParticleSystem>();
            for (int j = 0; j < ps.Length; j++)
            {
                var main = ps[j].main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }
            RegisterFxParticles(0, _fxParticleList[i]);
        }
    }

    /// <summary>尝试重新注册一次丢失的特效</summary>
    public IEnumerator TryRegisterAll()
    {
        mPartilesUrls.Clear();
        //先获取当前模型需要加载的特效名称
        List<string> includes = AssetManager.Instance.GetBundleInCludes(mBundleName);
        if (includes != null && _fxParticleList != null)
        {
            for (int i = 0; i < includes.Count; i++)
            {
                string include = includes[i];
                if (mPartilesUrls.Contains(include))
                {
                    continue;
                }
                //更新一次特效池
                PSPoolManager.Instance.UpdateData();
                //不在特效池里面
                if (PSPoolManager.Instance.Find(include) == null)
                {
                    string lowerName = include.ToLower();
                    //尝试从当前的特效列表中获取相应的特效
                    GameObject fx = _fxParticleList.Find(p => p != null && p.name.ToLower().Replace("(clone)", "").Equals(lowerName));
                    if (fx != null)
                    {
                        PSPoolManager.Instance.Register(fx, 1, PSPoolManager.Persistence.Temp);
                        yield return null;
                    }
                    else
                    {
                        EB.Debug.LogPSPoolAsset(string.Format("<color=#ff0000>特效池里没有这个特效<color=#00ff00>{0}</color>,所有需要重新加载这个特效</color>", include));
                        mPartilesUrls.Add(include);
                    }
                }
            }
        }
    }

    private void RegisterAll()
	{
        if (PSPoolManager.Instance != null)
        {
            switch ((int)mType)
            {
                case (int)FXLibType.Normal:
                    {
                        LoadCombatantFX();
                        break;
                    }
                default:
                    {
                        RegisterAllFxParticles();
                        break;
                    }
            }
        }

        if (GenericPoolSingleton.Instance != null)
		{
			if (GenericPoolSingleton.Instance.trailPool != null)
			{
				for (int i = 0; i < _fxTrailList.Length; i++)
				{
					if (_fxTrailList[i] != null)
					{
						GenericPoolManager<TrailRendererInstance>.Persistence persistence = _temporary ? GenericPoolManager<TrailRendererInstance>.Persistence.Temporary : GenericPoolManager<TrailRendererInstance>.Persistence.Always;
						GenericPoolSingleton.Instance.trailPool.Register(_fxTrailList[i], _fxTrailRegisterCount, persistence);
					}
				}
			}

			if (GenericPoolSingleton.Instance.lightPool != null)
			{
				for (int i = 0; i < _fxLightList.Length; i++)
				{
					if (_fxLightList[i] != null)
					{
						GenericPoolManager<DynamicPointLightInstance>.Persistence persistence = _temporary ? GenericPoolManager<DynamicPointLightInstance>.Persistence.Temporary : GenericPoolManager<DynamicPointLightInstance>.Persistence.Always;
						GenericPoolSingleton.Instance.lightPool.Register(_fxLightList[i], _fxLightRegisterCount, persistence);
					}
				}
			}
		}
	}

#if UNITY_EDITOR
	[ContextMenu("AddToParticleList")]
	public void AddToParticleList()
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
        
        _fxParticleList = new List<GameObject>();
        for (int i = 0; i < assets.Count; i++)
        {
            _fxParticleList.Add(assets[i] as GameObject);
        }
    }
#endif
}
