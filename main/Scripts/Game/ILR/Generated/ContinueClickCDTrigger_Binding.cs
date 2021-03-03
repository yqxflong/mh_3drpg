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
    unsafe class ContinueClickCDTrigger_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ContinueClickCDTrigger);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("OnPress", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnPress_0);

            field = type.GetField("m_CallBackPress", flag);
            app.RegisterCLRFieldGetter(field, get_m_CallBackPress_0);
            app.RegisterCLRFieldSetter(field, set_m_CallBackPress_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_CallBackPress_0, AssignFromStack_m_CallBackPress_0);


        }


        static StackObject* OnPress_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @ispressed = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::ContinueClickCDTrigger instance_of_this_method = (global::ContinueClickCDTrigger)typeof(global::ContinueClickCDTrigger).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnPress(@ispressed);

            return __ret;
        }


        static object get_m_CallBackPress_0(ref object o)
        {
            return ((global::ContinueClickCDTrigger)o).m_CallBackPress;
        }

        static StackObject* CopyToStack_m_CallBackPress_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ContinueClickCDTrigger)o).m_CallBackPress;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_CallBackPress_0(ref object o, object v)
        {
            ((global::ContinueClickCDTrigger)o).m_CallBackPress = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_m_CallBackPress_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @m_CallBackPress = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::ContinueClickCDTrigger)o).m_CallBackPress = @m_CallBackPress;
            return ptr_of_this_method;
        }



    }
}
