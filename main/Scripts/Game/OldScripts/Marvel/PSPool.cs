using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 特效缓存对象信息数据结构
/// </summary>
public class PSPool
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    private PSPoolManager.Persistence mPersistence = PSPoolManager.Persistence.Temp;

    /// <summary>
    /// 当前活动中的链表
    /// </summary>
	private List<ParticleSystem> _active = new List<ParticleSystem>();

    //特效模板
    private ParticleSystem _template;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="psObject">将要缓存的对象</param>
    /// <param name="count">实例化次数</param>
    /// <param name="persistence">缓存级别类型</param>
    public PSPool(GameObject psObject, int count, PSPoolManager.Persistence persistence)
    {
        mPersistence = persistence;
        count = count > 0 ? count : 1;

        System.DateTime start = System.DateTime.Now;
        GM.AssetUtils.FixShaderInEditor(psObject);
        System.DateTime end = System.DateTime.Now;
        EB.Assets.LoadCostLog("shaders", (end - start).TotalMilliseconds);

        PrepareTemplate(psObject);
    }

    private void PrepareTemplate(GameObject psObject)
    {
        bool needHDSDVer = false;
        if (psObject.transform.Find("HD") != null && psObject.GetComponent<VFXPerformanceControll>() == null)
        {
            needHDSDVer = true;
        }
        GameObject go = Object.Instantiate(psObject);
        if (go != null)
        {
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                AddParticleRecycleInTime(ps, psObject.name);
                if (needHDSDVer == true)
                {
                    ps.gameObject.AddComponent<VFXPerformanceControll>();
                }
                ps.playOnAwake = false;

                ResetParticleToPool(ps);
                ParticleAnimationPal.Bind(ps.gameObject);
                _template = ps;
            }
        }
    }

    private void AddParticleRecycleInTime(ParticleSystem ps, string poolPsName)
    {
        if (ps != null)
        {
            if (!ps.main.loop)
            {
                if (poolPsName == null)
                    return;

                var psInTime = ps.gameObject.GetComponent<ParticleRecycleInTime>();
                if (psInTime == null)
                {
                    psInTime = ps.gameObject.AddComponent<ParticleRecycleInTime>();
                }
                psInTime.RecycleTime = ps.main.duration;
                psInTime.poolPsName = poolPsName;
                psInTime.Particle = ps;
            }
        }
    }

    /// <summary>
    /// 将特效回收到ready池里
    /// </summary>
    /// <param name="ps">粒子系统</param>
    /// <param name="isSetParent">是否拉到缓存池的结点下面</param>
    public void MoveToRecyle(ParticleSystem ps, bool isSetParent)
    {
        if (ps == null)
        {
            return;
        }

        _active.Remove(ps);
        RemovePS(ps.gameObject);
    }


    /// <summary>
    /// 激动对像上的所有粒子系统
    /// </summary>
    /// <param name="ps"></param>
    private void ActiveAll(ParticleSystem ps)
    {
        var psArray = ps.GetComponentsInChildren<ParticleSystem>(true);
        for (int i = 0; i < psArray.Length; i++)
        {
            psArray[i].gameObject.CustomSetActive(true);
        }
    }

    /// <summary>
    /// 使用这个缓存对象(递归)
    /// </summary>
    /// <returns></returns>
    public ParticleSystem Use()
    {
        if(_template != null)
        {
            ParticleSystem ps = null;
            var go = Object.Instantiate(_template.gameObject);
            if(go != null)
            {
                ps = go.GetComponent<ParticleSystem>();
                //设置它的父级
                ps.gameObject.transform.SetParent(PSPoolManager.Instance.transform);
                _active.Add(ps);
                ActiveAll(ps);
            }
            return ps;
        }

        return null;
    }

    public void UpdateData()
    {
        for (int i = _active.Count - 1; i >= 0; --i)
        {
            ParticleSystem ps = _active[i];

            if (ps == null || ps.gameObject == null)
            {
                _active.RemoveAt(i);
                continue;
            }
            
            if (!ps.gameObject.activeInHierarchy)
            {
                MoveToRecyle(ps, false);
            }
            
            if (!ps.gameObject.activeSelf || ps.isStopped)
            {
                MoveToRecyle(ps, false);
            }
        }
    }

    /// <summary>
    /// 重置当前的粒子系统添加到缓存池
    /// </summary>
    /// <param name="ps">粒子系统对象</param>
    /// <param name="isSetParent">是否移动到缓存池对象下面</param>
    private void ResetParticleToPool(ParticleSystem ps, bool isSetParent = true)
    {
        if (ps != null)
        {
            if (isSetParent && PSPoolManager.Instance != null && PSPoolManager.Instance.transform != null)
            {
                ps.gameObject.transform.SetParent(PSPoolManager.Instance.transform);
            }

            //让粒子系统停止并清空
            if (ps.isPlaying)
            {
                ps.Stop(true);
                ps.Clear(true);
            }
            //隐藏
            ps.gameObject.CustomSetActive(false);
        }
    }

    private void RemovePS(GameObject go)
    {
        Object.Destroy(go);
        go = null;
    }

    public void Destroy() 
    {
        //Destroy Template
        RemovePS(_template.gameObject);

        #region Destroy Active List
        foreach(var ps in _active)
        {
            if(ps != null && ps.gameObject != null)
            {
                RemovePS(ps.gameObject);
            }
        }
        _active.Clear();
        #endregion
    }
}
