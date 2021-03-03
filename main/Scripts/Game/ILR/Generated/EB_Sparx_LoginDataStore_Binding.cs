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
    unsafe class EB_Sparx_LoginDataStore_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.LoginDataStore);

            field = type.GetField("LoginData", flag);
            app.RegisterCLRFieldGetter(field, get_LoginData_0);
            app.RegisterCLRFieldSetter(field, set_LoginData_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_LoginData_0, AssignFromStack_LoginData_0);
            field = type.GetField("LocalUserId", flag);
            app.RegisterCLRFieldGetter(field, get_LocalUserId_1);
            app.RegisterCLRFieldSetter(field, set_LocalUserId_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_LocalUserId_1, AssignFromStack_LocalUserId_1);


        }



        static object get_LoginData_0(ref object o)
        {
            return ((EB.Sparx.LoginDataStore)o).LoginData;
        }

        static StackObject* CopyToStack_LoginData_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.LoginDataStore)o).LoginData;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LoginData_0(ref object o, object v)
        {
            ((EB.Sparx.LoginDataStore)o).LoginData = (System.Collections.Hashtable)v;
        }

        static StackObject* AssignFromStack_LoginData_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Hashtable @LoginData = (System.Collections.Hashtable)typeof(System.Collections.Hashtable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.LoginDataStore)o).LoginData = @LoginData;
            return ptr_of_this_method;
        }

        static object get_LocalUserId_1(ref object o)
        {
            return ((EB.Sparx.LoginDataStore)o).LocalUserId;
        }

        static StackObject* CopyToStack_LocalUserId_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.LoginDataStore)o).LocalUserId;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LocalUserId_1(ref object o, object v)
        {
            ((EB.Sparx.LoginDataStore)o).LocalUserId = (EB.Sparx.Id)v;
        }

        static StackObject* AssignFromStack_LocalUserId_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.Id @LocalUserId = (EB.Sparx.Id)typeof(EB.Sparx.Id).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.LoginDataStore)o).LocalUserId = @LocalUserId;
            return ptr_of_this_method;
        }



    }
}
