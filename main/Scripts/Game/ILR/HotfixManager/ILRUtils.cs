using System.Reflection;
using ILRuntime.CLR.Method;

namespace ILR.HotfixManager
{
    public class ILRUtils
    {
        private const string mType = "Hotfix_LT.ILRUpdateManager";

        private static bool mRegisterNeedUpdateMono_Got;
#if ILRuntime
        private static IMethod mRegisterNeedUpdateMono;
#else
        private static MethodInfo mRegisterNeedUpdateMono;
        private static object[] parameters = new object[1];
#endif

        public static void RegisterNeedUpdateMono(IUpdateable ilrObject)
        {
            if (!mRegisterNeedUpdateMono_Got)
            {
#if ILRuntime
                var iType = HotfixILRManager.GetInstance().appdomain.GetType(mType);
                mRegisterNeedUpdateMono = iType.GetMethod("RegisterNeedUpdateMono", 1);
#else
                var t = HotfixILRManager.GetInstance().assembly?.GetType(mType);
                mRegisterNeedUpdateMono = t?.GetMethod("RegisterNeedUpdateMono");
#endif

                mRegisterNeedUpdateMono_Got = mRegisterNeedUpdateMono != null;
            }
#if ILRuntime
            HotfixILRManager.GetInstance().appdomain.Invoke(mRegisterNeedUpdateMono, null, ilrObject);
#else
            parameters[0] = ilrObject;
            mRegisterNeedUpdateMono?.Invoke(null, parameters);
#endif
            //
            // GlobalUtils.CallStaticHotfix("Hotfix_LT.ILRUpdateManager", "RegisterNeedUpdateMono", ilrObject);
        }
/// <summary>
/// /////////////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>

        private static bool mUnRegisterNeedUpdateMono_Got;
#if ILRuntime
        private static IMethod mUnRegisterNeedUpdateMono;
#else
        private static MethodInfo mUnRegisterNeedUpdateMono;
        private static object[] unParameters = new object[1];
#endif
        public static void UnRegisterNeedUpdateMono(IUpdateable ilrObject)
        {
            if (!mUnRegisterNeedUpdateMono_Got)
            {
#if ILRuntime
                var iType = HotfixILRManager.GetInstance().appdomain.GetType(mType);
                mUnRegisterNeedUpdateMono = iType.GetMethod("UnRegisterNeedUpdateMono", 1);
#else
                var t = HotfixILRManager.GetInstance().assembly?.GetType(mType);
                mUnRegisterNeedUpdateMono = t?.GetMethod("UnRegisterNeedUpdateMono");
#endif

                mUnRegisterNeedUpdateMono_Got = mUnRegisterNeedUpdateMono != null;
            }
#if ILRuntime
            HotfixILRManager.GetInstance().appdomain.Invoke(mUnRegisterNeedUpdateMono, null, ilrObject);
#else
            unParameters[0] = ilrObject;
            mUnRegisterNeedUpdateMono?.Invoke(null, unParameters);
#endif
            // GlobalUtils.CallStaticHotfix("Hotfix_LT.ILRUpdateManager", "UnRegisterNeedUpdateMono", ilrObject);
        }
        
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private static bool mCallUpdateMono_Got;
#if ILRuntime
        private static IMethod mCallUpdateMono;
#else
        private static MethodInfo mCallUpdateMono;
        private static object[] callUpdateMonoParameters = new object[0];
#endif
        public static void CallUpdateMono()
        {
            if (!mCallUpdateMono_Got)
            {
#if ILRuntime
                var iType = HotfixILRManager.GetInstance().appdomain?.GetType(mType);
                mCallUpdateMono = iType?.GetMethod("Update", 0);
#else
                var t = HotfixILRManager.GetInstance().assembly?.GetType(mType);
                mCallUpdateMono = t?.GetMethod("Update");
#endif

                mCallUpdateMono_Got = mCallUpdateMono != null;
            }
#if ILRuntime
            HotfixILRManager.GetInstance().appdomain.Invoke(mCallUpdateMono, null, null);
#else
            mCallUpdateMono?.Invoke(null, callUpdateMonoParameters);
#endif
            // GlobalUtils.CallStaticHotfix("Hotfix_LT.ILRUpdateManager", "Update", ilrObject);
        }
        
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private static bool mCallLateUpdateMono_Got;
#if ILRuntime
        private static IMethod mCallLateUpdateMono;
#else
        private static MethodInfo mCallLateUpdateMono;
        private static object[] callLateUpdateMonoParameters = new object[0];
#endif
        public static void CallLateUpdateMono()
        {
            if (!mCallLateUpdateMono_Got)
            {
#if ILRuntime
                var iType = HotfixILRManager.GetInstance().appdomain?.GetType(mType);
                mCallLateUpdateMono = iType?.GetMethod("LateUpdate", 0);
#else
                var t = HotfixILRManager.GetInstance().assembly?.GetType(mType);
                mCallLateUpdateMono = t?.GetMethod("LateUpdate");
#endif
                mCallLateUpdateMono_Got = mCallLateUpdateMono != null;
            }
#if ILRuntime
            HotfixILRManager.GetInstance().appdomain.Invoke(mCallLateUpdateMono, null, null);
#else
            mCallLateUpdateMono?.Invoke(null, callLateUpdateMonoParameters);
#endif
            // GlobalUtils.CallStaticHotfix("Hotfix_LT.ILRUpdateManager", "LateUpdate", ilrObject);
        }
        
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private static bool mCallFixedUpdateMono_Got;
#if ILRuntime
        private static IMethod mCallFixedUpdateMono;
#else
        private static MethodInfo mCallFixedUpdateMono;
        private static object[] callFixedUpdateMonoParameters = new object[0];
#endif
        public static void CallFixedUpdateMono()
        {
            if (!mCallFixedUpdateMono_Got)
            {
#if ILRuntime
                var iType = HotfixILRManager.GetInstance().appdomain?.GetType(mType);
                mCallFixedUpdateMono = iType?.GetMethod("FixedUpdate", 0);
#else
                var t = HotfixILRManager.GetInstance().assembly?.GetType(mType);
                mCallFixedUpdateMono = t?.GetMethod("FixedUpdate");
#endif
                mCallFixedUpdateMono_Got = mCallFixedUpdateMono != null;
            }
#if ILRuntime
            HotfixILRManager.GetInstance().appdomain.Invoke(mCallFixedUpdateMono, null, null);
#else
            mCallFixedUpdateMono?.Invoke(null, callFixedUpdateMonoParameters);
#endif
            // GlobalUtils.CallStaticHotfix("Hotfix_LT.ILRUpdateManager", "FixedUpdate", ilrObject);
        }
    }
    

    public interface IUpdateable
    {
    }
}