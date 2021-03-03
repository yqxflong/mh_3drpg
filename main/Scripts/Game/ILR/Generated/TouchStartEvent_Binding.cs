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
    unsafe class TouchStartEvent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::TouchStartEvent);

            field = type.GetField("deltaPosition", flag);
            app.RegisterCLRFieldGetter(field, get_deltaPosition_0);
            app.RegisterCLRFieldSetter(field, set_deltaPosition_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_deltaPosition_0, AssignFromStack_deltaPosition_0);


        }



        static object get_deltaPosition_0(ref object o)
        {
            return ((global::TouchStartEvent)o).deltaPosition;
        }

        static StackObject* CopyToStack_deltaPosition_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::TouchStartEvent)o).deltaPosition;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_deltaPosition_0(ref object o, object v)
        {
            ((global::TouchStartEvent)o).deltaPosition = (UnityEngine.Vector3)v;
        }

        static StackObject* AssignFromStack_deltaPosition_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector3 @deltaPosition = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @deltaPosition, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @deltaPosition = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            }
            ((global::TouchStartEvent)o).deltaPosition = @deltaPosition;
            return ptr_of_this_method;
        }



    }
}
