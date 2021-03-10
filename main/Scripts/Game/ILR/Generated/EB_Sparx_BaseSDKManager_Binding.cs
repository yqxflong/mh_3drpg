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
    unsafe class EB_Sparx_BaseSDKManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(EB.Sparx.BaseSDKManager);
            args = new Type[]{};
            method = type.GetMethod("SwitchAccount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SwitchAccount_0);
            args = new Type[]{typeof(EB.Sparx.RoleData)};
            method = type.GetMethod("SetRoleData", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetRoleData_1);
            args = new Type[]{};
            method = type.GetMethod("GetAppid", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetAppid_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetCurHCCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetCurHCCount_3);
            args = new Type[]{};
            method = type.GetMethod("IsGuest", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsGuest_4);


        }


        static StackObject* SwitchAccount_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.BaseSDKManager instance_of_this_method = (EB.Sparx.BaseSDKManager)typeof(EB.Sparx.BaseSDKManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SwitchAccount();

            return __ret;
        }

        static StackObject* SetRoleData_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.RoleData @roleData = (EB.Sparx.RoleData)typeof(EB.Sparx.RoleData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.BaseSDKManager instance_of_this_method = (EB.Sparx.BaseSDKManager)typeof(EB.Sparx.BaseSDKManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetRoleData(@roleData);

            return __ret;
        }

        static StackObject* GetAppid_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.BaseSDKManager instance_of_this_method = (EB.Sparx.BaseSDKManager)typeof(EB.Sparx.BaseSDKManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetAppid();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* SetCurHCCount_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @curHCCount = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.BaseSDKManager instance_of_this_method = (EB.Sparx.BaseSDKManager)typeof(EB.Sparx.BaseSDKManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetCurHCCount(@curHCCount);

            return __ret;
        }

        static StackObject* IsGuest_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.BaseSDKManager instance_of_this_method = (EB.Sparx.BaseSDKManager)typeof(EB.Sparx.BaseSDKManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsGuest();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }



    }
}
