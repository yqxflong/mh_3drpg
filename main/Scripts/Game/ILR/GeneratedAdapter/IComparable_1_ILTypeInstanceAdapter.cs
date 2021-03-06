using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntime.Runtime.GeneratedAdapter
{   
    public class IComparable_1_ILTypeInstanceAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.IComparable<ILRuntime.Runtime.Intepreter.ILTypeInstance>);
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

        public class Adapter : System.IComparable<ILRuntime.Runtime.Intepreter.ILTypeInstance>, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            IMethod mCompareTo_0;
            bool mCompareTo_0_Got;
            bool mCompareTo_0_Invoking;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public System.Int32 CompareTo(ILRuntime.Runtime.Intepreter.ILTypeInstance other)
            {
                if (!mCompareTo_0_Got)
                {
                    mCompareTo_0 = instance.Type.GetMethod("CompareTo", 1);
                    mCompareTo_0_Got = true;
                }
                return (System.Int32)appdomain.Invoke(mCompareTo_0, this.instance, other);
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
