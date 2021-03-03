using UnityEngine;
using System.Collections.Generic;
using ILR.HotfixManager;


public class DataLookupILR : DataLookup
{
    public string hotfixClassPath;

    public List<bool> BoolParamList;
    public List<int> IntParamList;
    public List<float> FloatParamList;
    public List<string> StringParamList;
    public List<Object> ObjectParamList;

    public DataLookILRObject dlinstance;

    public void ILRObjInit()
    {
        if (dlinstance == null && !string.IsNullOrEmpty(hotfixClassPath))
        {
#if ILRuntime
            dlinstance = HotfixILRManager.GetInstance().appdomain.Instantiate<DataLookILRObject>(hotfixClassPath);
#else
            var type = HotfixILRManager.GetInstance().assembly.GetType(hotfixClassPath);
            dlinstance = System.Activator.CreateInstance(type) as DataLookILRObject;
#endif
            dlinstance.SetDataLookup(this);

            base.Awake();
            if (dlinstance != null)
            {
                dlinstance.Awake();
            }
        }
    }


    public override void Awake()
    {
        ILRObjInit();
    }

    public override void Start()
    {
        base.Start();

        if (dlinstance != null)
        {
            dlinstance.Start();
        }
    }
    
    public override void OnDestroy()
    {
        if (dlinstance != null)
        {
            dlinstance.OnDestroy();
        }

        base.OnDestroy();
    }
    
    public override void OnEnable()
    {
        if (dlinstance != null)
        {
            dlinstance.OnEnable();
            ILRUtils.RegisterNeedUpdateMono(dlinstance);
        }

        base.OnEnable();
    }

    private void OnDisable()
    {
        if (dlinstance != null)
        {
            dlinstance.OnDisable();
            ILRUtils.UnRegisterNeedUpdateMono(dlinstance);
        }
    }

    public override void OnLookupUpdate(string dataID, object value)
    {
        if (dlinstance != null)
        {
            dlinstance.OnLookupUpdate(dataID, value);
        }
    }

    public void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
    {
        if (dlinstance != null)
        {
            dlinstance.OnFetchData(res, reqInstanceID);
        }
    }
}


public class DataLookILRObject: IUpdateable
{
    public virtual void SetDataLookup(DataLookupILR dataLookup)
    {

    }


    public virtual void Awake()
    {
    }

    public virtual void Start()
    {
    }

    // public virtual void Update(int e)
    // {
    // }

    public virtual void OnEnable()
    {

    }

    public virtual void OnDisable()
    {

    }

    public virtual void OnDestroy()
    {
    }

    public virtual void OnLookupUpdate(string dataID, object value)
    {
        
    }

    public virtual void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
    {
    }
}