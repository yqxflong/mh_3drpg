using System;
using System.Collections;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntime.Runtime.GeneratedAdapter
{   
    public class IEnumerable_1_ILTypeInstanceAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Generic.IEnumerable<ILRuntime.Runtime.Intepreter.ILTypeInstance>);
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

        public class Adapter : System.Collections.Generic.IEnumerable<ILRuntime.Runtime.Intepreter.ILTypeInstance>, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            IMethod mGetEnumerator_0;
            bool mGetEnumerator_0_Got;
            bool mGetEnumerator_0_Invoking;
            IMethod mGetEnumerator_1;
            bool mGetEnumerator_1_Got;
            bool mGetEnumerator_1_Invoking;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public System.Collections.Generic.IEnumerator<ILRuntime.Runtime.Intepreter.ILTypeInstance> GetEnumerator()
            {
                if (!mGetEnumerator_0_Got)
                {
                    mGetEnumerator_0 = instance.Type.GetMethod("GetEnumerator", 0);
                    mGetEnumerator_0_Got = true;
                }
                return (System.Collections.Generic.IEnumerator<ILRuntime.Runtime.Intepreter.ILTypeInstance>)appdomain.Invoke(mGetEnumerator_0, this.instance);
            }

            // public System.Collections.IEnumerator GetEnumerator()
            // {
            //     if (!mGetEnumerator_1_Got)
            //     {
            //         mGetEnumerator_1 = instance.Type.GetMethod("GetEnumerator", 0);
            //         mGetEnumerator_1_Got = true;
            //     }
            //     return (System.Collections.IEnumerator)appdomain.Invoke(mGetEnumerator_1, this.instance);
            // }

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

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
