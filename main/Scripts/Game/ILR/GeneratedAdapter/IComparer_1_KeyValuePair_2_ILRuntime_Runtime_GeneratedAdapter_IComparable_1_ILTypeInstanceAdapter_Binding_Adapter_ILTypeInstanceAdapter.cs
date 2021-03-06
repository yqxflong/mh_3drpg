using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntime.Runtime.GeneratedAdapter
{   
    public class IComparer_1_KeyValuePair_2_ILRuntime_Runtime_GeneratedAdapter_IComparable_1_ILTypeInstanceAdapter_Binding_Adapter_ILTypeInstanceAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<ILRuntime.Runtime.GeneratedAdapter.IComparable_1_ILTypeInstanceAdapter.Adapter, ILRuntime.Runtime.Intepreter.ILTypeInstance>>);
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

        public class Adapter : System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<ILRuntime.Runtime.GeneratedAdapter.IComparable_1_ILTypeInstanceAdapter.Adapter, ILRuntime.Runtime.Intepreter.ILTypeInstance>>, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            IMethod mCompare_0;
            bool mCompare_0_Got;
            bool mCompare_0_Invoking;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public System.Int32 Compare(System.Collections.Generic.KeyValuePair<ILRuntime.Runtime.GeneratedAdapter.IComparable_1_ILTypeInstanceAdapter.Adapter, ILRuntime.Runtime.Intepreter.ILTypeInstance> x, System.Collections.Generic.KeyValuePair<ILRuntime.Runtime.GeneratedAdapter.IComparable_1_ILTypeInstanceAdapter.Adapter, ILRuntime.Runtime.Intepreter.ILTypeInstance> y)
            {
                if (!mCompare_0_Got)
                {
                    mCompare_0 = instance.Type.GetMethod("Compare", 2);
                    mCompare_0_Got = true;
                }
                return (System.Int32)appdomain.Invoke(mCompare_0, this.instance, x, y);
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
