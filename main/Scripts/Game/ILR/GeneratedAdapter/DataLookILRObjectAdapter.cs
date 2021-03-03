using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntime.Runtime.GeneratedAdapter
{   
    public class DataLookILRObjectAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(global::DataLookILRObject);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : global::DataLookILRObject, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            IMethod mSetDataLookup_0;
            bool mSetDataLookup_0_Got;
            bool mSetDataLookup_0_Invoking;
            IMethod mAwake_1;
            bool mAwake_1_Got;
            bool mAwake_1_Invoking;
            IMethod mStart_2;
            bool mStart_2_Got;
            bool mStart_2_Invoking;
            IMethod mOnEnable_3;
            bool mOnEnable_3_Got;
            bool mOnEnable_3_Invoking;
            IMethod mOnDisable_4;
            bool mOnDisable_4_Got;
            bool mOnDisable_4_Invoking;
            IMethod mOnDestroy_5;
            bool mOnDestroy_5_Got;
            bool mOnDestroy_5_Invoking;
            IMethod mOnLookupUpdate_6;
            bool mOnLookupUpdate_6_Got;
            bool mOnLookupUpdate_6_Invoking;
            IMethod mOnFetchData_7;
            bool mOnFetchData_7_Got;
            bool mOnFetchData_7_Invoking;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void SetDataLookup(global::DataLookupILR dataLookup)
            {
                if (!mSetDataLookup_0_Got)
                {
                    mSetDataLookup_0 = instance.Type.GetMethod("SetDataLookup", 1);
                    mSetDataLookup_0_Got = true;
                }
                if (mSetDataLookup_0 != null && !mSetDataLookup_0_Invoking)
                {
                    mSetDataLookup_0_Invoking = true;
                    appdomain.Invoke(mSetDataLookup_0, this.instance, dataLookup);
                    mSetDataLookup_0_Invoking = false;
                }
                else
                {
                    base.SetDataLookup(dataLookup);
                }
            }

            public override void Awake()
            {
                if (!mAwake_1_Got)
                {
                    mAwake_1 = instance.Type.GetMethod("Awake", 0);
                    mAwake_1_Got = true;
                }
                if (mAwake_1 != null && !mAwake_1_Invoking)
                {
                    mAwake_1_Invoking = true;
                    appdomain.Invoke(mAwake_1, this.instance);
                    mAwake_1_Invoking = false;
                }
                else
                {
                    base.Awake();
                }
            }

            public override void Start()
            {
                if (!mStart_2_Got)
                {
                    mStart_2 = instance.Type.GetMethod("Start", 0);
                    mStart_2_Got = true;
                }
                if (mStart_2 != null && !mStart_2_Invoking)
                {
                    mStart_2_Invoking = true;
                    appdomain.Invoke(mStart_2, this.instance);
                    mStart_2_Invoking = false;
                }
                else
                {
                    base.Start();
                }
            }

            public override void OnEnable()
            {
                if (!mOnEnable_3_Got)
                {
                    mOnEnable_3 = instance.Type.GetMethod("OnEnable", 0);
                    mOnEnable_3_Got = true;
                }
                if (mOnEnable_3 != null && !mOnEnable_3_Invoking)
                {
                    mOnEnable_3_Invoking = true;
                    appdomain.Invoke(mOnEnable_3, this.instance);
                    mOnEnable_3_Invoking = false;
                }
                else
                {
                    base.OnEnable();
                }
            }

            public override void OnDisable()
            {
                if (!mOnDisable_4_Got)
                {
                    mOnDisable_4 = instance.Type.GetMethod("OnDisable", 0);
                    mOnDisable_4_Got = true;
                }
                if (mOnDisable_4 != null && !mOnDisable_4_Invoking)
                {
                    mOnDisable_4_Invoking = true;
                    appdomain.Invoke(mOnDisable_4, this.instance);
                    mOnDisable_4_Invoking = false;
                }
                else
                {
                    base.OnDisable();
                }
            }

            public override void OnDestroy()
            {
                if (!mOnDestroy_5_Got)
                {
                    mOnDestroy_5 = instance.Type.GetMethod("OnDestroy", 0);
                    mOnDestroy_5_Got = true;
                }
                if (mOnDestroy_5 != null && !mOnDestroy_5_Invoking)
                {
                    mOnDestroy_5_Invoking = true;
                    appdomain.Invoke(mOnDestroy_5, this.instance);
                    mOnDestroy_5_Invoking = false;
                }
                else
                {
                    base.OnDestroy();
                }
            }

            public override void OnLookupUpdate(System.String dataID, System.Object value)
            {
                if (!mOnLookupUpdate_6_Got)
                {
                    mOnLookupUpdate_6 = instance.Type.GetMethod("OnLookupUpdate", 2);
                    mOnLookupUpdate_6_Got = true;
                }
                if (mOnLookupUpdate_6 != null && !mOnLookupUpdate_6_Invoking)
                {
                    mOnLookupUpdate_6_Invoking = true;
                    appdomain.Invoke(mOnLookupUpdate_6, this.instance, dataID, value);
                    mOnLookupUpdate_6_Invoking = false;
                }
                else
                {
                    base.OnLookupUpdate(dataID, value);
                }
            }

            public override void OnFetchData(EB.Sparx.Response res, System.Int32 reqInstanceID)
            {
                if (!mOnFetchData_7_Got)
                {
                    mOnFetchData_7 = instance.Type.GetMethod("OnFetchData", 2);
                    mOnFetchData_7_Got = true;
                }
                if (mOnFetchData_7 != null && !mOnFetchData_7_Invoking)
                {
                    mOnFetchData_7_Invoking = true;
                    appdomain.Invoke(mOnFetchData_7, this.instance, res, reqInstanceID);
                    mOnFetchData_7_Invoking = false;
                }
                else
                {
                    base.OnFetchData(res, reqInstanceID);
                }
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
        }
    }
}
