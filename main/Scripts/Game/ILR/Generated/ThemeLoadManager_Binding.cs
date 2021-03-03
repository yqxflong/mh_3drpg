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
    unsafe class ThemeLoadManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::ThemeLoadManager);
            args = new Type[]{};
            method = type.GetMethod("GetSceneRoot", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetSceneRoot_0);
            args = new Type[]{typeof(System.String), typeof(System.String), typeof(global::SceneRootEntry.Begin), typeof(global::SceneRootEntry.Failed), typeof(global::SceneRootEntry.Loading), typeof(global::SceneRootEntry.Finished)};
            method = type.GetMethod("LoadOTALevelAsync", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LoadOTALevelAsync_1);
            args = new Type[]{};
            method = type.GetMethod("GetSceneRootObject", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetSceneRootObject_2);


        }


        static StackObject* GetSceneRoot_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::ThemeLoadManager instance_of_this_method = (global::ThemeLoadManager)typeof(global::ThemeLoadManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetSceneRoot();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* LoadOTALevelAsync_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 7);

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
            System.String @levelPath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            System.String @levelName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 7);
            global::ThemeLoadManager instance_of_this_method = (global::ThemeLoadManager)typeof(global::ThemeLoadManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.LoadOTALevelAsync(@levelName, @levelPath, @begin, @failed, @loading, @finish);

            return __ret;
        }

        static StackObject* GetSceneRootObject_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::ThemeLoadManager instance_of_this_method = (global::ThemeLoadManager)typeof(global::ThemeLoadManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetSceneRootObject();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
