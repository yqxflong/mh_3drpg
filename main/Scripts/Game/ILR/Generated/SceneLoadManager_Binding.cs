using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class SceneLoadManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::SceneLoadManager);
            args = new Type[]{};
            method = type.GetMethod("get_CurrentSceneName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_CurrentSceneName_0);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("GetSceneRoot", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetSceneRoot_1);
            args = new Type[]{};
            method = type.GetMethod("DestroyAllLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DestroyAllLevel_2);
            args = new Type[]{};
            method = type.GetMethod("get_IsLoadingConfig", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsLoadingConfig_3);
            args = new Type[]{typeof(System.Action<System.Boolean>)};
            method = type.GetMethod("LoadConfigAsync", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LoadConfigAsync_4);
            args = new Type[]{};
            method = type.GetMethod("get_IsReady", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsReady_5);
            args = new Type[]{typeof(System.String), typeof(global::SceneRootEntry.Begin), typeof(global::SceneRootEntry.Failed), typeof(global::SceneRootEntry.Loading), typeof(global::SceneRootEntry.Finished)};
            method = type.GetMethod("LoadOTALevelGroupAsync", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LoadOTALevelGroupAsync_6);
            args = new Type[]{};
            method = type.GetMethod("get_CurrentStateName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_CurrentStateName_7);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("GetSceneLoadConfig", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetSceneLoadConfig_8);


        }


        static StackObject* get_CurrentSceneName_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SceneLoadManager.CurrentSceneName;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetSceneRoot_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @sceneFileName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::SceneLoadManager.GetSceneRoot(@sceneFileName);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* DestroyAllLevel_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::SceneLoadManager.DestroyAllLevel();

            return __ret;
        }

        static StackObject* get_IsLoadingConfig_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SceneLoadManager.IsLoadingConfig;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* LoadConfigAsync_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.Boolean> @fn = (System.Action<System.Boolean>)typeof(System.Action<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::SceneLoadManager.LoadConfigAsync(@fn);

            return __ret;
        }

        static StackObject* get_IsReady_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SceneLoadManager.IsReady;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* LoadOTALevelGroupAsync_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::SceneRootEntry.Finished @finish = (global::SceneRootEntry.Finished)typeof(global::SceneRootEntry.Finished).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::SceneRootEntry.Loading @loading = (global::SceneRootEntry.Loading)typeof(global::SceneRootEntry.Loading).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::SceneRootEntry.Failed @failed = (global::SceneRootEntry.Failed)typeof(global::SceneRootEntry.Failed).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            global::SceneRootEntry.Begin @begin = (global::SceneRootEntry.Begin)typeof(global::SceneRootEntry.Begin).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.String @stateName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::SceneLoadManager.LoadOTALevelGroupAsync(@stateName, @begin, @failed, @loading, @finish);

            return __ret;
        }

        static StackObject* get_CurrentStateName_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::SceneLoadManager.CurrentStateName;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetSceneLoadConfig_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @stateName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::SceneLoadManager.GetSceneLoadConfig(@stateName);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
