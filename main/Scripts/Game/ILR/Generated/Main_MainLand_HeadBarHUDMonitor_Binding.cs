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
    unsafe class Main_MainLand_HeadBarHUDMonitor_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Main.MainLand.HeadBarHUDMonitor);

            field = type.GetField("m_Offset", flag);
            app.RegisterCLRFieldGetter(field, get_m_Offset_0);
            app.RegisterCLRFieldSetter(field, set_m_Offset_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_Offset_0, AssignFromStack_m_Offset_0);


        }



        static object get_m_Offset_0(ref object o)
        {
            return ((Main.MainLand.HeadBarHUDMonitor)o).m_Offset;
        }

        static StackObject* CopyToStack_m_Offset_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Main.MainLand.HeadBarHUDMonitor)o).m_Offset;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_m_Offset_0(ref object o, object v)
        {
            ((Main.MainLand.HeadBarHUDMonitor)o).m_Offset = (UnityEngine.Vector2)v;
        }

        static StackObject* AssignFromStack_m_Offset_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector2 @m_Offset = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @m_Offset, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @m_Offset = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            }
            ((Main.MainLand.HeadBarHUDMonitor)o).m_Offset = @m_Offset;
            return ptr_of_this_method;
        }



    }
}
