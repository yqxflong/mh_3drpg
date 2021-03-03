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
    unsafe class EB_IAP_Transaction_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.IAP.Transaction);

            field = type.GetField("platform", flag);
            app.RegisterCLRFieldGetter(field, get_platform_0);
            app.RegisterCLRFieldSetter(field, set_platform_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_platform_0, AssignFromStack_platform_0);


        }



        static object get_platform_0(ref object o)
        {
            return ((EB.IAP.Transaction)o).platform;
        }

        static StackObject* CopyToStack_platform_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.IAP.Transaction)o).platform;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_platform_0(ref object o, object v)
        {
            ((EB.IAP.Transaction)o).platform = (System.String)v;
        }

        static StackObject* AssignFromStack_platform_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @platform = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.IAP.Transaction)o).platform = @platform;
            return ptr_of_this_method;
        }



    }
}
