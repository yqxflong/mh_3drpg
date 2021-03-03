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
    unsafe class ToggleGroupState_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ToggleGroupState);

            field = type.GetField("m_Toggles", flag);
            app.RegisterCLRFieldGetter(field, get_m_Toggles_0);
            app.RegisterCLRFieldSetter(field, set_m_Toggles_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_Toggles_0, AssignFromStack_m_Toggles_0);


        }



        static object get_m_Toggles_0(ref object o)
        {
            return ((global::ToggleGroupState)o).m_Toggles;
        }

        static StackObject* CopyToStack_m_Toggles_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ToggleGroupState)o).m_Toggles;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_Toggles_0(ref object o, object v)
        {
            ((global::ToggleGroupState)o).m_Toggles = (System.Collections.Generic.List<global::UIToggle>)v;
        }

        static StackObject* AssignFromStack_m_Toggles_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::UIToggle> @m_Toggles = (System.Collections.Generic.List<global::UIToggle>)typeof(System.Collections.Generic.List<global::UIToggle>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::ToggleGroupState)o).m_Toggles = @m_Toggles;
            return ptr_of_this_method;
        }



    }
}
