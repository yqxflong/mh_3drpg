using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

public class DynamicMonoILRObjectAdaptor : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            return typeof(DynamicMonoILRObject);
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : DynamicMonoILRObject, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } }


        IMethod mSetMono;
        bool isSetMonoInvoking = false;

        public override void SetMono(DynamicMonoILR mono)
        {
            if (mSetMono == null)
            {
                mSetMono = instance.Type.GetMethod("SetMono", 1);
            }
            if (mSetMono != null && !isSetMonoInvoking)
            {
                isSetMonoInvoking = true;
                appdomain.Invoke(mSetMono, instance, mono);
                isSetMonoInvoking = false;
            }
            else
                base.SetMono(mono);
        }

        IMethod mAwake;
        bool isAwakeInvoking = false;

        public override void Awake()
        {
            if (mAwake == null)
            {
                mAwake = instance.Type.GetMethod("Awake", 0);
            }
            if (mAwake != null && !isAwakeInvoking)
            {
                isAwakeInvoking = true;
                appdomain.Invoke(mAwake, instance);
                isAwakeInvoking = false;
            }
            else
                base.Awake();
        }

        IMethod mStart;
        bool isStartInvoking = false;

        public override void Start()
        {
            if (mStart == null)
            {
                mStart = instance.Type.GetMethod("Start", 0);
            }
            if (mStart != null && !isStartInvoking)
            {
                isStartInvoking = true;
                appdomain.Invoke(mStart, instance);
                isStartInvoking = false;
            }
            else
                base.Start();
        }


        IMethod mOnEnable;
        bool isOnEnableInvoking = false;

        public override void OnEnable()
        {

            if (mOnEnable == null)
            {
                mOnEnable = instance.Type.GetMethod("OnEnable", 0);
            }
            if (mOnEnable != null && !isOnEnableInvoking)
            {
                isOnEnableInvoking = true;
                appdomain.Invoke(mOnEnable, instance);
                isOnEnableInvoking = false;
            }
            else
                base.OnEnable();
        }


        IMethod mOnDisable;
        bool isOnDisableInvoking = false;

        public override void OnDisable()
        {
            if (mOnDisable == null)
            {
                mOnDisable = instance.Type.GetMethod("OnDisable", 0);
            }
            if (mOnDisable != null && !isOnDisableInvoking)
            {
                isOnDisableInvoking = true;
                appdomain.Invoke(mOnDisable, instance);
                isOnDisableInvoking = false;
            }
            else
                base.OnDisable();
        }
        
        IMethod mOnFetchData;
        bool isOnFetchDataInvoking = false;

        public override void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
        {
            if (mOnFetchData == null)
            {
                mOnFetchData = instance.Type.GetMethod("OnFetchData", 2);
            }
            if (mOnFetchData != null && !isOnFetchDataInvoking)
            {
                isOnFetchDataInvoking = true;
                appdomain.Invoke(mOnFetchData, instance, res, reqInstanceID);
                isOnFetchDataInvoking = false;
            }
            else
                base.OnFetchData(res, reqInstanceID);
        }

        IMethod mOnDestroy;
        bool isOnDestroyInvoking = false;

        public override void OnDestroy()
        {
            if (mOnDestroy == null)
            {
                mOnDestroy = instance.Type.GetMethod("OnDestroy", 0);
            }
            if (mOnDestroy != null && !isOnDestroyInvoking)
            {
                isOnDestroyInvoking = true;
                appdomain.Invoke(mOnDestroy, instance);
                isOnDestroyInvoking = false;
            }
            else
                base.OnDestroy();
        }

        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return instance.ToString();
            }
            else
                return instance.Type.FullName;
        }


        #region NGUI Event
        IMethod mOnDrag;
        bool isOnDragInvoking = false;

        public override void OnDrag(Vector2 delta)
        {
            if (mOnDrag == null)
            {
                mOnDrag = instance.Type.GetMethod("OnDrag", 1);
            }
            if (mOnDrag != null && !isOnDragInvoking)
            {
                isOnDragInvoking = true;
                appdomain.Invoke(mOnDrag, instance, delta);
                isOnDragInvoking = false;
            }
            else
            {
                base.OnDrag(delta);
            }
        }
        #endregion

        #region iTween Event
        IMethod mITweenOnComplete;
        bool isITweenOnCompleteInvoking = false;

        public override void ITweenOnComplete()
        {
            if (mITweenOnComplete == null)
            {
                mITweenOnComplete = instance.Type.GetMethod("ITweenOnComplete", 0);
            }
            if (mITweenOnComplete != null && !isITweenOnCompleteInvoking)
            {
                isITweenOnCompleteInvoking = true;
                appdomain.Invoke(mITweenOnComplete, instance);
                isITweenOnCompleteInvoking = false;
            }
            else
            {
                base.ITweenOnComplete();
            }
        }
        #endregion

        #region HandleMessage
        IMethod mOnHandleMessage;
        bool isOnHandleMessageInvoking = false;

        public override void OnHandleMessage(string methodName, object value)
        {
            if (mOnHandleMessage == null)
            {
                mOnHandleMessage = instance.Type.GetMethod("OnHandleMessage", 2);
            }
            if (mOnHandleMessage != null && !isOnHandleMessageInvoking)
            {
                isOnHandleMessageInvoking = true;
                appdomain.Invoke(mOnHandleMessage, instance, methodName, value);
                isOnHandleMessageInvoking = false;
            }
            else
            {
                base.OnHandleMessage(methodName, value);
            }
        }
        #endregion

        #region GetShareData
        IMethod mOnGetAttributeData;
        bool isOnGetAttributeDataInvoking = false;

        public override AttributeData OnGetAttributeData(string key)
        {
            if (mOnGetAttributeData == null)
            {
                mOnGetAttributeData = instance.Type.GetMethod("OnGetAttributeData", 1);
            }
            if (mOnGetAttributeData != null && !isOnGetAttributeDataInvoking)
            {
                isOnGetAttributeDataInvoking = true;
                var res = (AttributeData)appdomain.Invoke(mOnGetAttributeData, instance, key);
                isOnGetAttributeDataInvoking = false;

                return res;
            }
            else
            {
                return base.OnGetAttributeData(key);
            }
        }
        #endregion

        IMethod mGetValueFrom;
        bool isGetValueFromInvoking = false;

        public override object GetValueFrom(string methodName, object args)
        {
            if (mGetValueFrom == null)
            {
                mGetValueFrom = instance.Type.GetMethod("GetValueFrom", 2);
            }
            if (mGetValueFrom != null && !isGetValueFromInvoking)
            {
                isGetValueFromInvoking = true;
                var res = appdomain.Invoke(mGetValueFrom, instance, methodName, args);
                isGetValueFromInvoking = false;

                return res;
            }
            else
            {
                return base.GetValueFrom(methodName, args);
            }
        }
    }
}