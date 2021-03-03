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
    unsafe class EB_Sparx_Device_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(EB.Sparx.Device);
            args = new Type[]{};
            method = type.GetMethod("get_MobilePlatform", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_MobilePlatform_0);
            args = new Type[]{};
            method = type.GetMethod("get_UniqueIdentifier", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_UniqueIdentifier_1);


        }


        static StackObject* get_MobilePlatform_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = EB.Sparx.Device.MobilePlatform;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_UniqueIdentifier_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = EB.Sparx.Device.UniqueIdentifier;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}