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
    unsafe class EB_Sparx_MHAuthenticator_Binding_UserInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.MHAuthenticator.UserInfo);

            field = type.GetField("phone", flag);
            app.RegisterCLRFieldGetter(field, get_phone_0);
            app.RegisterCLRFieldSetter(field, set_phone_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_phone_0, AssignFromStack_phone_0);
            field = type.GetField("password", flag);
            app.RegisterCLRFieldGetter(field, get_password_1);
            app.RegisterCLRFieldSetter(field, set_password_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_password_1, AssignFromStack_password_1);


        }



        static object get_phone_0(ref object o)
        {
            return ((EB.Sparx.MHAuthenticator.UserInfo)o).phone;
        }

        static StackObject* CopyToStack_phone_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.MHAuthenticator.UserInfo)o).phone;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_phone_0(ref object o, object v)
        {
            ((EB.Sparx.MHAuthenticator.UserInfo)o).phone = (System.String)v;
        }

        static StackObject* AssignFromStack_phone_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @phone = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.MHAuthenticator.UserInfo)o).phone = @phone;
            return ptr_of_this_method;
        }

        static object get_password_1(ref object o)
        {
            return ((EB.Sparx.MHAuthenticator.UserInfo)o).password;
        }

        static StackObject* CopyToStack_password_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.MHAuthenticator.UserInfo)o).password;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_password_1(ref object o, object v)
        {
            ((EB.Sparx.MHAuthenticator.UserInfo)o).password = (System.String)v;
        }

        static StackObject* AssignFromStack_password_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @password = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.MHAuthenticator.UserInfo)o).password = @password;
            return ptr_of_this_method;
        }



    }
}
