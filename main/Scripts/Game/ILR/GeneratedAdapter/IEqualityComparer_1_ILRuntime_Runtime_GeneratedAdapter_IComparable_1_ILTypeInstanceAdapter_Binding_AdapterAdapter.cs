using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntime.Runtime.GeneratedAdapter
{   
    public class IEqualityComparer_1_ILRuntime_Runtime_GeneratedAdapter_IComparable_1_ILTypeInstanceAdapter_Binding_AdapterAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Generic.IEqualityComparer<ILRuntime.Runtime.GeneratedAdapter.IComparable_1_ILTypeInstanceAdapter.Adapter>);
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

        public class Adapter : System.Collections.Generic.IEqualityComparer<ILRuntime.Runtime.GeneratedAdapter.IComparable_1_ILTypeInstanceAdapter.Adapter>, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            IMethod mEquals_0;
            bool mEquals_0_Got;
            bool mEquals_0_Invoking;
            IMethod mGetHashCode_1;
            bool mGetHashCode_1_Got;
            bool mGetHashCode_1_Invoking;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public System.Boolean Equals(ILRuntime.Runtime.GeneratedAdapter.IComparable_1_ILTypeInstanceAdapter.Adapter x, ILRuntime.Runtime.GeneratedAdapter.IComparable_1_ILTypeInstanceAdapter.Adapter y)
            {
                if (!mEquals_0_Got)
                {
                    mEquals_0 = instance.Type.GetMethod("Equals", 2);
                    mEquals_0_Got = true;
                }
                return (System.Boolean)appdomain.Invoke(mEquals_0, this.instance, x, y);
            }

            public System.Int32 GetHashCode(ILRuntime.Runtime.GeneratedAdapter.IComparable_1_ILTypeInstanceAdapter.Adapter obj)
            {
                if (!mGetHashCode_1_Got)
                {
                    mGetHashCode_1 = instance.Type.GetMethod("GetHashCode", 1);
                    mGetHashCode_1_Got = true;
                }
                return (System.Int32)appdomain.Invoke(mGetHashCode_1, this.instance, obj);
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
