using System.Collections.Generic;
using ILR.HotfixManager;
using UnityEngine;
using ILRuntime.Other;

public class DynamicMonoILR : MonoBehaviour
{
    public string hotfixClassPath;
    
    public List<bool> BoolParamList;
    public List<int> IntParamList;
    public List<float> FloatParamList;
    public List<string> StringParamList;
    public List<UnityEngine.Object> ObjectParamList;
    public List<Vector3> Vector3ParamList;

    public DynamicMonoILRObject _ilrObject;

    /// <summary>
    /// 初始化ILR，可主动调用
    /// </summary>
    public void ILRObjInit()
    {
        if (_ilrObject == null && !string.IsNullOrEmpty(hotfixClassPath))
        {

#if ILRuntime
            _ilrObject = HotfixILRManager.GetInstance().appdomain. Instantiate<DynamicMonoILRObject>(hotfixClassPath);
#else
            var type = HotfixILRManager.GetInstance().assembly.GetType(hotfixClassPath);
            //UnityEngine.Debug.LogError("type :"+ type.Name);
            _ilrObject = System.Activator.CreateInstance(type) as DynamicMonoILRObject;
#endif

            _ilrObject.SetMono(this);

            if (_ilrObject != null) {
                _ilrObject.Awake();
            }
        }
    }

    private void Awake()
    {
        ILRObjInit();
    }

    private void Start()
    {
        if (_ilrObject != null) {
            _ilrObject.Start();           
        }
    }

    private void OnEnable()
    {
        if (_ilrObject != null)
        {
            _ilrObject.OnEnable ();       
        }
    }

	private void OnDisable()
    {
        if (_ilrObject != null)
        {
            _ilrObject.OnDisable();
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        if (_ilrObject != null){
            _ilrObject.OnDestroy();
        }
    }

    public void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
    {
        if (_ilrObject != null)
        {
            _ilrObject.OnFetchData(res, reqInstanceID);
        }
    }

    #region NGUI Event
    private void OnDrag(Vector2 delta)
    {
        if (_ilrObject != null) _ilrObject.OnDrag(delta);
    }
    #endregion

    #region iTween Event
    private void ITweenOnComplete()
    {
        if (_ilrObject != null) _ilrObject.ITweenOnComplete();
    }
    #endregion

    #region 处理消息
    public void OnHandleMessage(string methodName, object value)
    {
        if (_ilrObject != null)
        {
            _ilrObject.OnHandleMessage(methodName, value);
        }
    }
    #endregion

    #region 获取属性数据
    public AttributeData OnGetAttributeData(string key)
    {
        if (_ilrObject != null)
        {
            return _ilrObject.OnGetAttributeData(key);
        }
        return new AttributeData();
    }
    #endregion

    public object GetValueFrom(string methodName, object args)
    {
        if (_ilrObject != null)
        {
            _ilrObject.GetValueFrom(methodName, args);
        }

        return null;
    }
}

[NeedAdaptor]
public class DynamicMonoILRObject: IUpdateable
{
    public virtual void SetMono(DynamicMonoILR mono)
    {

    }

    public virtual void Awake()
    {

    }

    public virtual void Start()
    {

    }

    public virtual void OnEnable()
    {

    }

    public virtual void OnDisable()
    {

    }
    
    public virtual void OnDestroy()
    {

    }

    public virtual void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
    {
    }


    #region NGUI Event
    public virtual void OnDrag(Vector2 delta)
    {
        
    }
    #endregion

    #region iTween Event
    public virtual void ITweenOnComplete()
    {
        
    }
    #endregion

    #region 处理消息
    public virtual void OnHandleMessage(string methodName, object value)
    {
    }
    #endregion

    #region 处理消息
    public virtual AttributeData OnGetAttributeData(string key)
    {
        return new AttributeData();
    }
    #endregion

    public virtual object GetValueFrom(string methodName, object args)
    {
        return null;
    }
}