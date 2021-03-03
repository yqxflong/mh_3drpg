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
    unsafe class EB_Sparx_DataStore_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.DataStore);

            field = type.GetField("LoginDataStore", flag);
            app.RegisterCLRFieldGetter(field, get_LoginDataStore_0);
            app.RegisterCLRFieldSetter(field, set_LoginDataStore_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_LoginDataStore_0, AssignFromStack_LoginDataStore_0);


        }



        static object get_LoginDataStore_0(ref object o)
        {
            return ((EB.Sparx.DataStore)o).LoginDataStore;
        }

        static StackObject* CopyToStack_LoginDataStore_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.DataStore)o).LoginDataStore;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LoginDataStore_0(ref object o, object v)
        {
            ((EB.Sparx.DataStore)o).LoginDataStore = (EB.Sparx.LoginDataStore)v;
        }

        static StackObject* AssignFromStack_LoginDataStore_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.LoginDataStore @LoginDataStore = (EB.Sparx.LoginDataStore)typeof(EB.Sparx.LoginDataStore).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.DataStore)o).LoginDataStore = @LoginDataStore;
            return ptr_of_this_method;
        }



    }
}
