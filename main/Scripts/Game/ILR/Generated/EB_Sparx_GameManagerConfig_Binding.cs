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
    unsafe class EB_Sparx_GameManagerConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.GameManagerConfig);

            field = type.GetField("Listener", flag);
            app.RegisterCLRFieldGetter(field, get_Listener_0);
            app.RegisterCLRFieldSetter(field, set_Listener_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Listener_0, AssignFromStack_Listener_0);


        }



        static object get_Listener_0(ref object o)
        {
            return ((EB.Sparx.GameManagerConfig)o).Listener;
        }

        static StackObject* CopyToStack_Listener_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.GameManagerConfig)o).Listener;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Listener_0(ref object o, object v)
        {
            ((EB.Sparx.GameManagerConfig)o).Listener = (EB.Sparx.GameListener)v;
        }

        static StackObject* AssignFromStack_Listener_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.GameListener @Listener = (EB.Sparx.GameListener)typeof(EB.Sparx.GameListener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.GameManagerConfig)o).Listener = @Listener;
            return ptr_of_this_method;
        }



    }
}
