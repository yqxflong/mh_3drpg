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
    unsafe class EB_Sparx_WalletConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.WalletConfig);

            field = type.GetField("IAPPublicKey", flag);
            app.RegisterCLRFieldGetter(field, get_IAPPublicKey_0);
            app.RegisterCLRFieldSetter(field, set_IAPPublicKey_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_IAPPublicKey_0, AssignFromStack_IAPPublicKey_0);
            field = type.GetField("Listener", flag);
            app.RegisterCLRFieldGetter(field, get_Listener_1);
            app.RegisterCLRFieldSetter(field, set_Listener_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Listener_1, AssignFromStack_Listener_1);


        }



        static object get_IAPPublicKey_0(ref object o)
        {
            return ((EB.Sparx.WalletConfig)o).IAPPublicKey;
        }

        static StackObject* CopyToStack_IAPPublicKey_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.WalletConfig)o).IAPPublicKey;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_IAPPublicKey_0(ref object o, object v)
        {
            ((EB.Sparx.WalletConfig)o).IAPPublicKey = (System.String)v;
        }

        static StackObject* AssignFromStack_IAPPublicKey_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @IAPPublicKey = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.WalletConfig)o).IAPPublicKey = @IAPPublicKey;
            return ptr_of_this_method;
        }

        static object get_Listener_1(ref object o)
        {
            return ((EB.Sparx.WalletConfig)o).Listener;
        }

        static StackObject* CopyToStack_Listener_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.WalletConfig)o).Listener;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Listener_1(ref object o, object v)
        {
            ((EB.Sparx.WalletConfig)o).Listener = (EB.Sparx.WalletListener)v;
        }

        static StackObject* AssignFromStack_Listener_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.WalletListener @Listener = (EB.Sparx.WalletListener)typeof(EB.Sparx.WalletListener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.WalletConfig)o).Listener = @Listener;
            return ptr_of_this_method;
        }



    }
}
