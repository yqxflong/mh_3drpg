using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// 特效池管理器(已经标记长住内存)
/// </summary>
public class PSPoolManager : MonoBehaviour 
{
    /// <summary>
    /// 缓存级别类型
    /// </summary>
	public enum Persistence
    {
        /// <summary>
        /// 临时内存
        /// </summary>
        Temp,
        /// <summary>
        /// UI特效
        /// </summary>
        Hud,
        /// <summary>
        /// 主城需要的特效
        /// </summary>
        MainLand,
        /// <summary>
        /// 战斗需要的特效
        /// </summary>
        Combat,
    };
   
    /// <summary>
    /// 特效库类型
    /// </summary>
    public enum enFxType
    {
        /// <summary>
        /// UI（所有状态都用）
        /// </summary>
        StandardHudFX,
        /// <summary>
        /// 主城
        /// </summary>
        StandardMainlandFX,
        /// <summary>
        /// 人物
        /// </summary>
        StandardCharacterFX,
    }

    #region Member
    public static PSPoolManager Instance
	{
		get
		{
			if (_instance == null && !_created)
			{
				InitializeInstance();
			}
			return _instance;
		}
	}

    /// <summary>
    /// 已经添加过的特效名称(它身上会持着相应的特效)
    /// </summary>
    private List<string> m_HaveLoadFx;

    /// <summary>
    /// 相应的状态对应的挂载通用特效名称
    /// </summary>
    private Dictionary<enFxType, string> m_CommonFx;

    /// <summary>
    /// 当前的所有特效池
    /// </summary>
    private Dictionary<string, PSPool>[] _pools = new Dictionary<string, PSPool>[5];
    
    private static PSPoolManager _instance = null;

    private static bool _created = false;
    #endregion

    public static void InitializeInstance()
	{
		if (_instance == null)
		{
			GameObject psPool = new GameObject("PSPoolManager");
			if (Application.isPlaying)
			{
				DontDestroyOnLoad(psPool.gameObject);
			}
            psPool.CustomSetActive(false);
            _instance = psPool.AddComponent<PSPoolManager>();
            _instance.Awake();
			_created = true;
		}
	}

	private void Awake()
	{
		_pools[0] = new Dictionary<string, PSPool>(); 	// always
		_pools[1] = new Dictionary<string, PSPool>();   // Temp
        _pools[2] = new Dictionary<string, PSPool>();   // Hud
        _pools[3] = new Dictionary<string, PSPool>();   // MainLand
        _pools[4] = new Dictionary<string, PSPool>();   // Combat
        m_HaveLoadFx = new List<string>();
        m_CommonFx = new Dictionary<enFxType, string>();
        m_CommonFx.Add(enFxType.StandardHudFX, "Bundles/VFX/StandardHudFX");
        m_CommonFx.Add(enFxType.StandardMainlandFX, "Bundles/VFX/StandardMainlandFX");
        m_CommonFx.Add(enFxType.StandardCharacterFX, "Bundles/VFX/StandardCharacterFX");
    }

	void OnDestroy()
	{
		if (!Application.isPlaying)
		{
			_created = false;
		}
	}

    public void UpdateData()
    {
        for (int i = 0; i < _pools.Length; ++i)
        {
            if (_pools[i] != null)
            {
                foreach (KeyValuePair<string, PSPool> kvp in _pools[i])
                {
                    if (kvp.Value != null)
                    {
                        kvp.Value.UpdateData();
                    }
                }
            }
        }
    }

    #region 加载战斗特效
    /// <summary>
    /// 加载战斗通用特效
    /// </summary>
    /// <returns></returns>
    public Coroutine LoadStandardCombatFX()
    {
        if (m_HaveLoadFx.Contains(m_CommonFx[enFxType.StandardCharacterFX]))
        {
            return null;
        }
        m_HaveLoadFx.Add(m_CommonFx[enFxType.StandardCharacterFX]);
        return EB.Assets.LoadAsync(m_CommonFx[enFxType.StandardCharacterFX], typeof(GameObject), asset=>
        {
            if(asset){
                GameObject fxgo = Instantiate(asset) as GameObject;
                fxgo.transform.SetParent(_instance.transform);
                InitStandardFx(fxgo,Persistence.Combat);
            }
        });
    }
    
    /// <summary>
    /// 卸载战斗特效
    /// </summary>
	public void UnloadStandardCombatFX()
    {
        //销毁Combat池子所有paricle
        foreach(var it in _pools[(int)Persistence.Combat])
        {
            it.Value.Destroy();
        }
        _pools[(int)Persistence.Combat].Clear();
        //卸载对应的assets
        EB.Assets.Unload(m_CommonFx[enFxType.StandardCharacterFX]);
        EB.Assets.UnloadBundleFromResourcePath(m_CommonFx[enFxType.StandardCharacterFX]);
        m_HaveLoadFx.Remove(m_CommonFx[enFxType.StandardCharacterFX]);
    }
    #endregion

    #region 加载UI特效
    /// <summary>
    /// 加载UI界面通用特效
    /// </summary>
    public void LoadStandardHudFX()
    {
        if (m_HaveLoadFx.Contains(m_CommonFx[enFxType.StandardHudFX]))
        {
            return;
        }
        m_HaveLoadFx.Add(m_CommonFx[enFxType.StandardHudFX]);

        EB.Assets.LoadAsync(m_CommonFx[enFxType.StandardHudFX], typeof(GameObject), o =>
        {
            if (o != null)
            {
                GameObject go = Instantiate(o) as GameObject;
                go.transform.SetParent(_instance.transform);
                InitStandardFx(go, Persistence.Hud);
            }
        });
    }

    /// <summary>
    /// 卸载UI的特效(几乎常驻)
    /// </summary>
    public void UnloadStandardHudFX()
    {
        //销毁UI池子所有paricle
        foreach(var it in _pools[(int)Persistence.Hud])
        {
            it.Value.Destroy();
        }
        _pools[(int)Persistence.Hud].Clear();
        //卸载对应的assets
        EB.Assets.Unload( m_CommonFx[enFxType.StandardHudFX]);
        EB.Assets.UnloadBundleFromResourcePath(m_CommonFx[enFxType.StandardHudFX]);
        m_HaveLoadFx.Remove(m_CommonFx[enFxType.StandardHudFX]);
    }
    #endregion

    #region 加载主城的特效
    public void LoadStandardMainlandFX()
    {
        if (m_HaveLoadFx.Contains(m_CommonFx[enFxType.StandardMainlandFX]))
        {
            return;
        }
        m_HaveLoadFx.Add(m_CommonFx[enFxType.StandardMainlandFX]);
        //
        EB.Assets.LoadAsync(m_CommonFx[enFxType.StandardMainlandFX], typeof(GameObject), o=> {
            if (o != null)
            {
                GameObject standard_lib = Instantiate(o) as GameObject;
                standard_lib.transform.SetParent(_instance.transform);
                InitStandardFx(standard_lib, Persistence.MainLand);
            }
        });
    }

    /// <summary>
    /// 卸载主城
    /// </summary>
    public void UnloadStandardMainlandFX()
    {
        //销毁Mailand池子所有paricle
        foreach(var it in _pools[(int)Persistence.MainLand])
        {
            it.Value.Destroy();
        }
        _pools[(int)Persistence.MainLand].Clear();
        //卸载对应的assets
        EB.Assets.Unload(m_CommonFx[enFxType.StandardMainlandFX]);
        EB.Assets.UnloadBundleFromResourcePath(m_CommonFx[enFxType.StandardMainlandFX]);
        m_HaveLoadFx.Remove(m_CommonFx[enFxType.StandardMainlandFX]);
    }
    #endregion

    /// <summary>
    /// 将对象上引用的所有特效列表加入进入缓存池
    /// </summary>
    /// <param name="fxgo">特效对象</param>
    private void InitStandardFx(GameObject fxgo, Persistence persis)
    {
        var fxlib = fxgo.GetComponent<FXLib>();
        if (fxlib != null)
        {
            for (int i = 0; i < fxlib._fxParticleList.Count; i++)
            {
                this.Register(fxlib._fxParticleList[i], 1, persis);
            }
            GameObject.Destroy(fxgo);
        }
    }

    /// <summary>
    /// 注册特效缓存
    /// </summary>
    /// <param name="psobject">将要缓存的对象</param>
    /// <param name="count">实例化次数</param>
    /// <param name="ePersistence">缓存类型</param>
    public void Register(GameObject psobject, int count, Persistence ePersistence)
    {
        if (psobject == null)
        {
            EB.Debug.LogPSPoolAsset("<color=#ff0000>为什么注册特效的对象是空的呢?</color>");
            return;
        }
        //
        if (_pools[(int)ePersistence] != null)
        {
            string name = psobject.name.ToLower().Replace("(clone)", "");
            PSPool pool = Find(name);
            if (pool == null)
            {
                _pools[(int)ePersistence][name] = new PSPool(psobject, count, ePersistence);
            }
        }
    }
    
    /// <summary>
    /// 使用指定特效
    /// </summary>
    /// <param name="obj">借特效的对象</param>
    /// <param name="name">特效名称</param>
    /// <returns></returns>
    public ParticleSystem Use(Object obj, string name)
    {
        ParticleSystem ps = this.Use(name);
        return ps;
    }

    /// <summary>
    /// 使用指定特效
    /// </summary>
    /// <param name="name">特效名称</param>
    /// <returns></returns>
    private ParticleSystem Use(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
        }

        PSPool psPool = Find(name);
        if (psPool != null)
        {
            ParticleSystem ps = psPool.Use();
            if (ps != null)
            {
                //让他下面所有的对象都打开
                ps.name = name;
                var animators = ps.GetComponentsInChildren<Animator>(true);
                for (var i = 0; i < animators.Length; i++)
                {
                    animators[i].gameObject.CustomSetActive(true);
                }
                ps.transform.localPosition = Vector3.zero;
                ParticleSystem[] sys = ps.GetComponentsInChildren<ParticleSystem>();
                for (int i = 0; i < sys.Length; i++)
                {
                    if (sys[i] != null)
                    {
                        var main = sys[i].main;
                        main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                    }
                }
            }
            return ps;
		}

		return null;
	}
	
    /// <summary>
    /// 从缓存池里获取相应的特效
    /// </summary>
    /// <param name="name">特效名称</param>
    /// <returns></returns>
	public PSPool Find(string name)
	{
        name = name.ToLower().Replace("(clone)", "");
        PSPool psPool = null;
		for (int i=0; i<_pools.Length; ++i)
		{
			if (_pools[i] != null)
			{
				if (_pools[i].TryGetValue(name, out psPool))
				{
                    break;
				}
			}
        }
        return psPool;
	}

    /// <summary>
    /// 回收特效对象
    /// </summary>
    /// <param name="obj">特效对象</param>
    public void Recycle(Object obj)
    {
        if (obj == null)
        {
            EB.Debug.LogPSPoolAsset("<color=#ff0000>回收的对象为null</color>");
            return;
        }
        if (obj is GameObject)
        {
            GameObject go = obj as GameObject;
            ParticleSystem[] ps = go.GetComponentsInChildren<ParticleSystem>(true);
            if (ps.Length > 0)
            {
                for(int i = 0; i < ps.Length; i++)
                {
                    this.Recycle(ps[i]);
                }
            }else
            {
                EB.Debug.LogPSPoolAsset("<color=#00ff00>回收的对象身上<color=#ff0000>【没有】</color>特效:</color>" + obj.name);
            }
        }
        else if (obj is ParticleSystem)
        {
            this.Recycle(obj as ParticleSystem);
        }
    }
    
	/// <summary>
    /// 回收特效
    /// </summary>
    /// <param name="ps"></param>
	public void Recycle(ParticleSystem ps)
    {
        if (ps == null)
        {
            EB.Debug.LogError("为什么是空的特效?回收呢");
            return;
        }
        PSPool psPool = Find(ps.name);
        if (psPool != null)
		{
            psPool.MoveToRecyle(ps, true);
        }else{
            Register(ps.gameObject, 0, Persistence.Temp);
        }
	}
    
    ///压缩池子
    public void CompressPool()
    {
        //temp的全部清掉
        foreach(var it in _pools[(int)Persistence.Temp])
        {
            it.Value.Destroy();
        }
        _pools[(int)Persistence.Temp].Clear();
    }
}
