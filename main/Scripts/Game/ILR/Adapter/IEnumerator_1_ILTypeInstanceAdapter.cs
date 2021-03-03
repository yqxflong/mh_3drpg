using System;
using System.Collections;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntime.Runtime.GeneratedAdapter
{   
    public class IEnumerator_1_ILTypeInstanceAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Generic.IEnumerator<ILRuntime.Runtime.Intepreter.ILTypeInstance>);
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

        public class Adapter : System.Collections.Generic.IEnumerator<ILRuntime.Runtime.Intepreter.ILTypeInstance>, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            IMethod mget_Current_0;
            bool mget_Current_0_Got;
            bool mget_Current_0_Invoking;
            IMethod mMoveNext_1;
            bool mMoveNext_1_Got;
            bool mMoveNext_1_Invoking;
            IMethod mget_Current_2;
            bool mget_Current_2_Got;
            bool mget_Current_2_Invoking;
            IMethod mReset_3;
            bool mReset_3_Got;
            bool mReset_3_Invoking;
            IMethod mDispose_4;
            bool mDispose_4_Got;
            bool mDispose_4_Invoking;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public System.Boolean MoveNext()
            {
                if (!mMoveNext_1_Got)
                {
                    mMoveNext_1 = instance.Type.GetMethod("MoveNext", 0);
                    mMoveNext_1_Got = true;
                }
                return (System.Boolean)appdomain.Invoke(mMoveNext_1, this.instance);
            }

            public void Reset()
            {
                if (!mReset_3_Got)
                {
                    mReset_3 = instance.Type.GetMethod("Reset", 0);
                    mReset_3_Got = true;
                }
                appdomain.Invoke(mReset_3, this.instance);
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
                if (!mDispose_4_Got)
                {
                    mDispose_4 = instance.Type.GetMethod("Dispose", 0);
                    mDispose_4_Got = true;
                }
                appdomain.Invoke(mDispose_4, this.instance);
            }

            public ILRuntime.Runtime.Intepreter.ILTypeInstance Current
            {
            get
            {
                if (!mget_Current_2_Got)
                {
                    mget_Current_2 = instance.Type.GetMethod("get_Current", 0);
                    mget_Current_2_Got = true;
                }
                return (ILRuntime.Runtime.Intepreter.ILTypeInstance)appdomain.Invoke(mget_Current_2, this.instance);

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
