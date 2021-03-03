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
    unsafe class UICamera_Binding_MouseOrTouch_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UICamera.MouseOrTouch);

            field = type.GetField("pos", flag);
            app.RegisterCLRFieldGetter(field, get_pos_0);
            app.RegisterCLRFieldSetter(field, set_pos_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_pos_0, AssignFromStack_pos_0);
            field = type.GetField("current", flag);
            app.RegisterCLRFieldGetter(field, get_current_1);
            app.RegisterCLRFieldSetter(field, set_current_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_current_1, AssignFromStack_current_1);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static object get_pos_0(ref object o)
        {
            return ((global::UICamera.MouseOrTouch)o).pos;
        }

        static StackObject* CopyToStack_pos_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UICamera.MouseOrTouch)o).pos;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_pos_0(ref object o, object v)
        {
            ((global::UICamera.MouseOrTouch)o).pos = (UnityEngine.Vector2)v;
        }

        static StackObject* AssignFromStack_pos_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector2 @pos = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @pos, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @pos = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            }
            ((global::UICamera.MouseOrTouch)o).pos = @pos;
            return ptr_of_this_method;
        }

        static object get_current_1(ref object o)
        {
            return ((global::UICamera.MouseOrTouch)o).current;
        }

        static StackObject* CopyToStack_current_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UICamera.MouseOrTouch)o).current;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_current_1(ref object o, object v)
        {
            ((global::UICamera.MouseOrTouch)o).current = (UnityEngine.GameObject)v;
        }

        static StackObject* AssignFromStack_current_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.GameObject @current = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UICamera.MouseOrTouch)o).current = @current;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new global::UICamera.MouseOrTouch();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
