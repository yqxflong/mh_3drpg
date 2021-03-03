using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntime.Runtime.GeneratedAdapter
{   
    public class LogicILRObjectAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(global::LogicILRObject);
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

        public class Adapter : global::LogicILRObject, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            IMethod mInitialize_0;
            bool mInitialize_0_Got;
            bool mInitialize_0_Invoking;
            IMethod mDispose_1;
            bool mDispose_1_Got;
            bool mDispose_1_Invoking;
            IMethod mAsync_2;
            bool mAsync_2_Got;
            bool mAsync_2_Invoking;
            IMethod mDisconnect_3;
            bool mDisconnect_3_Got;
            bool mDisconnect_3_Invoking;
            IMethod mConnect_4;
            bool mConnect_4_Got;
            bool mConnect_4_Invoking;
            IMethod mOnLoggedIn_5;
            bool mOnLoggedIn_5_Got;
            bool mOnLoggedIn_5_Invoking;
            IMethod mUpdate_6;
            bool mUpdate_6_Got;
            bool mUpdate_6_Invoking;
            IMethod mOnEnteredBackground_7;
            bool mOnEnteredBackground_7_Got;
            bool mOnEnteredBackground_7_Invoking;
            IMethod mOnEnteredForeground_8;
            bool mOnEnteredForeground_8_Got;
            bool mOnEnteredForeground_8_Invoking;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void Initialize(EB.Sparx.Config config)
            {
                if (!mInitialize_0_Got)
                {
                    mInitialize_0 = instance.Type.GetMethod("Initialize", 1);
                    mInitialize_0_Got = true;
                }
                if (mInitialize_0 != null && !mInitialize_0_Invoking)
                {
                    mInitialize_0_Invoking = true;
                    appdomain.Invoke(mInitialize_0, this.instance, config);
                    mInitialize_0_Invoking = false;
                }
                else
                {
                    base.Initialize(config);
                }
            }

            public override void Dispose()
            {
                if (!mDispose_1_Got)
                {
                    mDispose_1 = instance.Type.GetMethod("Dispose", 0);
                    mDispose_1_Got = true;
                }
                if (mDispose_1 != null && !mDispose_1_Invoking)
                {
                    mDispose_1_Invoking = true;
                    appdomain.Invoke(mDispose_1, this.instance);
                    mDispose_1_Invoking = false;
                }
                else
                {
                    base.Dispose();
                }
            }

            public override void Async(System.String message, System.Object payload)
            {
                if (!mAsync_2_Got)
                {
                    mAsync_2 = instance.Type.GetMethod("Async", 2);
                    mAsync_2_Got = true;
                }
                if (mAsync_2 != null && !mAsync_2_Invoking)
                {
                    mAsync_2_Invoking = true;
                    appdomain.Invoke(mAsync_2, this.instance, message, payload);
                    mAsync_2_Invoking = false;
                }
                else
                {
                    base.Async(message, payload);
                }
            }

            public override void Disconnect(System.Boolean isLogout)
            {
                if (!mDisconnect_3_Got)
                {
                    mDisconnect_3 = instance.Type.GetMethod("Disconnect", 1);
                    mDisconnect_3_Got = true;
                }
                if (mDisconnect_3 != null && !mDisconnect_3_Invoking)
                {
                    mDisconnect_3_Invoking = true;
                    appdomain.Invoke(mDisconnect_3, this.instance, isLogout);
                    mDisconnect_3_Invoking = false;
                }
                else
                {
                    base.Disconnect(isLogout);
                }
            }

            public override void Connect()
            {
                if (!mConnect_4_Got)
                {
                    mConnect_4 = instance.Type.GetMethod("Connect", 0);
                    mConnect_4_Got = true;
                }
                if (mConnect_4 != null && !mConnect_4_Invoking)
                {
                    mConnect_4_Invoking = true;
                    appdomain.Invoke(mConnect_4, this.instance);
                    mConnect_4_Invoking = false;
                }
                else
                {
                    base.Connect();
                }
            }

            public override void OnLoggedIn()
            {
                if (!mOnLoggedIn_5_Got)
                {
                    mOnLoggedIn_5 = instance.Type.GetMethod("OnLoggedIn", 0);
                    mOnLoggedIn_5_Got = true;
                }
                if (mOnLoggedIn_5 != null && !mOnLoggedIn_5_Invoking)
                {
                    mOnLoggedIn_5_Invoking = true;
                    appdomain.Invoke(mOnLoggedIn_5, this.instance);
                    mOnLoggedIn_5_Invoking = false;
                }
                else
                {
                    base.OnLoggedIn();
                }
            }

            public override void Update()
            {
                if (!mUpdate_6_Got)
                {
                    mUpdate_6 = instance.Type.GetMethod("Update", 0);
                    mUpdate_6_Got = true;
                }
                if (mUpdate_6 != null && !mUpdate_6_Invoking)
                {
                    mUpdate_6_Invoking = true;
                    appdomain.Invoke(mUpdate_6, this.instance);
                    mUpdate_6_Invoking = false;
                }
                else
                {
                    base.Update();
                }
            }

            public override void OnEnteredBackground()
            {
                if (!mOnEnteredBackground_7_Got)
                {
                    mOnEnteredBackground_7 = instance.Type.GetMethod("OnEnteredBackground", 0);
                    mOnEnteredBackground_7_Got = true;
                }
                if (mOnEnteredBackground_7 != null && !mOnEnteredBackground_7_Invoking)
                {
                    mOnEnteredBackground_7_Invoking = true;
                    appdomain.Invoke(mOnEnteredBackground_7, this.instance);
                    mOnEnteredBackground_7_Invoking = false;
                }
                else
                {
                    base.OnEnteredBackground();
                }
            }

            public override void OnEnteredForeground()
            {
                if (!mOnEnteredForeground_8_Got)
                {
                    mOnEnteredForeground_8 = instance.Type.GetMethod("OnEnteredForeground", 0);
                    mOnEnteredForeground_8_Got = true;
                }
                if (mOnEnteredForeground_8 != null && !mOnEnteredForeground_8_Invoking)
                {
                    mOnEnteredForeground_8_Invoking = true;
                    appdomain.Invoke(mOnEnteredForeground_8, this.instance);
                    mOnEnteredForeground_8_Invoking = false;
                }
                else
                {
                    base.OnEnteredForeground();
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
