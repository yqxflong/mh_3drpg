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
    unsafe class EB_Sparx_LoginConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.LoginConfig);

            field = type.GetField("Listener", flag);
            app.RegisterCLRFieldGetter(field, get_Listener_0);
            app.RegisterCLRFieldSetter(field, set_Listener_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Listener_0, AssignFromStack_Listener_0);
            field = type.GetField("Authenticators", flag);
            app.RegisterCLRFieldGetter(field, get_Authenticators_1);
            app.RegisterCLRFieldSetter(field, set_Authenticators_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Authenticators_1, AssignFromStack_Authenticators_1);


        }



        static object get_Listener_0(ref object o)
        {
            return ((EB.Sparx.LoginConfig)o).Listener;
        }

        static StackObject* CopyToStack_Listener_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.LoginConfig)o).Listener;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Listener_0(ref object o, object v)
        {
            ((EB.Sparx.LoginConfig)o).Listener = (EB.Sparx.LoginConfigListener)v;
        }

        static StackObject* AssignFromStack_Listener_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.LoginConfigListener @Listener = (EB.Sparx.LoginConfigListener)typeof(EB.Sparx.LoginConfigListener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.LoginConfig)o).Listener = @Listener;
            return ptr_of_this_method;
        }

        static object get_Authenticators_1(ref object o)
        {
            return ((EB.Sparx.LoginConfig)o).Authenticators;
        }

        static StackObject* CopyToStack_Authenticators_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.LoginConfig)o).Authenticators;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Authenticators_1(ref object o, object v)
        {
            ((EB.Sparx.LoginConfig)o).Authenticators = (System.Type[])v;
        }

        static StackObject* AssignFromStack_Authenticators_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Type[] @Authenticators = (System.Type[])typeof(System.Type[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.LoginConfig)o).Authenticators = @Authenticators;
            return ptr_of_this_method;
        }



    }
}
