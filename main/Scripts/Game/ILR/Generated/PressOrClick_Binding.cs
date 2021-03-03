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
    unsafe class PressOrClick_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::PressOrClick);

            field = type.GetField("m_CallBackPress", flag);
            app.RegisterCLRFieldGetter(field, get_m_CallBackPress_0);
            app.RegisterCLRFieldSetter(field, set_m_CallBackPress_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_CallBackPress_0, AssignFromStack_m_CallBackPress_0);


        }



        static object get_m_CallBackPress_0(ref object o)
        {
            return ((global::PressOrClick)o).m_CallBackPress;
        }

        static StackObject* CopyToStack_m_CallBackPress_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PressOrClick)o).m_CallBackPress;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_CallBackPress_0(ref object o, object v)
        {
            ((global::PressOrClick)o).m_CallBackPress = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_m_CallBackPress_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @m_CallBackPress = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::PressOrClick)o).m_CallBackPress = @m_CallBackPress;
            return ptr_of_this_method;
        }



    }
}
