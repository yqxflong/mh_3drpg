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
    unsafe class TapEvent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::TapEvent);

            field = type.GetField("hasValidNavPoint", flag);
            app.RegisterCLRFieldGetter(field, get_hasValidNavPoint_0);
            app.RegisterCLRFieldSetter(field, set_hasValidNavPoint_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_hasValidNavPoint_0, AssignFromStack_hasValidNavPoint_0);
            field = type.GetField("target", flag);
            app.RegisterCLRFieldGetter(field, get_target_1);
            app.RegisterCLRFieldSetter(field, set_target_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_target_1, AssignFromStack_target_1);
            field = type.GetField("groundPosition", flag);
            app.RegisterCLRFieldGetter(field, get_groundPosition_2);
            app.RegisterCLRFieldSetter(field, set_groundPosition_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_groundPosition_2, AssignFromStack_groundPosition_2);


        }



        static object get_hasValidNavPoint_0(ref object o)
        {
            return ((global::TapEvent)o).hasValidNavPoint;
        }

        static StackObject* CopyToStack_hasValidNavPoint_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::TapEvent)o).hasValidNavPoint;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_hasValidNavPoint_0(ref object o, object v)
        {
            ((global::TapEvent)o).hasValidNavPoint = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_hasValidNavPoint_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @hasValidNavPoint = ptr_of_this_method->Value == 1;
            ((global::TapEvent)o).hasValidNavPoint = @hasValidNavPoint;
            return ptr_of_this_method;
        }

        static object get_target_1(ref object o)
        {
            return ((global::TapEvent)o).target;
        }

        static StackObject* CopyToStack_target_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::TapEvent)o).target;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_target_1(ref object o, object v)
        {
            ((global::TapEvent)o).target = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_target_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @target = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::TapEvent)o).target = @target;
            return ptr_of_this_method;
        }

        static object get_groundPosition_2(ref object o)
        {
            return ((global::TapEvent)o).groundPosition;
        }

        static StackObject* CopyToStack_groundPosition_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::TapEvent)o).groundPosition;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_groundPosition_2(ref object o, object v)
        {
            ((global::TapEvent)o).groundPosition = (UnityEngine.Vector3)v;
        }

        static StackObject* AssignFromStack_groundPosition_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector3 @groundPosition = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @groundPosition, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @groundPosition = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            }
            ((global::TapEvent)o).groundPosition = @groundPosition;
            return ptr_of_this_method;
        }



    }
}
