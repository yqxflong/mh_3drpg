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
    unsafe class PerformanceManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::PerformanceManager);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);
            args = new Type[]{};
            method = type.GetMethod("get_CurrentEnvironmentInfo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_CurrentEnvironmentInfo_1);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("UseScene", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UseScene_2);

            field = type.GetField("PerformanceInfo", flag);
            app.RegisterCLRFieldGetter(field, get_PerformanceInfo_0);
            app.RegisterCLRFieldSetter(field, set_PerformanceInfo_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_PerformanceInfo_0, AssignFromStack_PerformanceInfo_0);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::PerformanceManager.Instance;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_CurrentEnvironmentInfo_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::PerformanceManager instance_of_this_method = (global::PerformanceManager)typeof(global::PerformanceManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CurrentEnvironmentInfo;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* UseScene_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @sceneName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::PerformanceManager instance_of_this_method = (global::PerformanceManager)typeof(global::PerformanceManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.UseScene(@sceneName);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_PerformanceInfo_0(ref object o)
        {
            return ((global::PerformanceManager)o).PerformanceInfo;
        }

        static StackObject* CopyToStack_PerformanceInfo_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PerformanceManager)o).PerformanceInfo;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_PerformanceInfo_0(ref object o, object v)
        {
            ((global::PerformanceManager)o).PerformanceInfo = (global::PerformanceInfo)v;
        }

        static StackObject* AssignFromStack_PerformanceInfo_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::PerformanceInfo @PerformanceInfo = (global::PerformanceInfo)typeof(global::PerformanceInfo).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::PerformanceManager)o).PerformanceInfo = @PerformanceInfo;
            return ptr_of_this_method;
        }



    }
}
