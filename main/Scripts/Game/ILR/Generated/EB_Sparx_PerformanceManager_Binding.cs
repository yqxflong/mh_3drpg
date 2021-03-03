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
    unsafe class EB_Sparx_PerformanceManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.PerformanceManager);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("ResetPerformanceData", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ResetPerformanceData_0);

            field = type.GetField("PerformanceUserSetting", flag);
            app.RegisterCLRFieldGetter(field, get_PerformanceUserSetting_0);
            app.RegisterCLRFieldSetter(field, set_PerformanceUserSetting_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_PerformanceUserSetting_0, AssignFromStack_PerformanceUserSetting_0);


        }


        static StackObject* ResetPerformanceData_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @cb = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.PerformanceManager instance_of_this_method = (EB.Sparx.PerformanceManager)typeof(EB.Sparx.PerformanceManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ResetPerformanceData(@cb);

            return __ret;
        }


        static object get_PerformanceUserSetting_0(ref object o)
        {
            return EB.Sparx.PerformanceManager.PerformanceUserSetting;
        }

        static StackObject* CopyToStack_PerformanceUserSetting_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = EB.Sparx.PerformanceManager.PerformanceUserSetting;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_PerformanceUserSetting_0(ref object o, object v)
        {
            EB.Sparx.PerformanceManager.PerformanceUserSetting = (System.String)v;
        }

        static StackObject* AssignFromStack_PerformanceUserSetting_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @PerformanceUserSetting = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            EB.Sparx.PerformanceManager.PerformanceUserSetting = @PerformanceUserSetting;
            return ptr_of_this_method;
        }



    }
}
