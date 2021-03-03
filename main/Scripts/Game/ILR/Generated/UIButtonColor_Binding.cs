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
    unsafe class UIButtonColor_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIButtonColor);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_isEnabled", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_isEnabled_0);
            args = new Type[]{typeof(UnityEngine.Color)};
            method = type.GetMethod("set_defaultColor", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_defaultColor_1);
            args = new Type[]{typeof(global::UIButtonColor.State), typeof(System.Boolean)};
            method = type.GetMethod("SetState", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetState_2);
            args = new Type[]{};
            method = type.GetMethod("get_isEnabled", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_isEnabled_3);

            field = type.GetField("tweenTarget", flag);
            app.RegisterCLRFieldGetter(field, get_tweenTarget_0);
            app.RegisterCLRFieldSetter(field, set_tweenTarget_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_tweenTarget_0, AssignFromStack_tweenTarget_0);
            field = type.GetField("pressed", flag);
            app.RegisterCLRFieldGetter(field, get_pressed_1);
            app.RegisterCLRFieldSetter(field, set_pressed_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_pressed_1, AssignFromStack_pressed_1);
            field = type.GetField("hover", flag);
            app.RegisterCLRFieldGetter(field, get_hover_2);
            app.RegisterCLRFieldSetter(field, set_hover_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_hover_2, AssignFromStack_hover_2);
            field = type.GetField("disabledColor", flag);
            app.RegisterCLRFieldGetter(field, get_disabledColor_3);
            app.RegisterCLRFieldSetter(field, set_disabledColor_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_disabledColor_3, AssignFromStack_disabledColor_3);


        }


        static StackObject* set_isEnabled_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIButtonColor instance_of_this_method = (global::UIButtonColor)typeof(global::UIButtonColor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.isEnabled = value;

            return __ret;
        }

        static StackObject* set_defaultColor_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Color @value = (UnityEngine.Color)typeof(UnityEngine.Color).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIButtonColor instance_of_this_method = (global::UIButtonColor)typeof(global::UIButtonColor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.defaultColor = value;

            return __ret;
        }

        static StackObject* SetState_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @instant = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIButtonColor.State @state = (global::UIButtonColor.State)typeof(global::UIButtonColor.State).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIButtonColor instance_of_this_method = (global::UIButtonColor)typeof(global::UIButtonColor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetState(@state, @instant);

            return __ret;
        }

        static StackObject* get_isEnabled_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIButtonColor instance_of_this_method = (global::UIButtonColor)typeof(global::UIButtonColor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.isEnabled;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_tweenTarget_0(ref object o)
        {
            return ((global::UIButtonColor)o).tweenTarget;
        }

        static StackObject* CopyToStack_tweenTarget_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButtonColor)o).tweenTarget;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_tweenTarget_0(ref object o, object v)
        {
            ((global::UIButtonColor)o).tweenTarget = (UnityEngine.GameObject)v;
        }

        static StackObject* AssignFromStack_tweenTarget_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.GameObject @tweenTarget = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButtonColor)o).tweenTarget = @tweenTarget;
            return ptr_of_this_method;
        }

        static object get_pressed_1(ref object o)
        {
            return ((global::UIButtonColor)o).pressed;
        }

        static StackObject* CopyToStack_pressed_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButtonColor)o).pressed;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_pressed_1(ref object o, object v)
        {
            ((global::UIButtonColor)o).pressed = (UnityEngine.Color)v;
        }

        static StackObject* AssignFromStack_pressed_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Color @pressed = (UnityEngine.Color)typeof(UnityEngine.Color).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButtonColor)o).pressed = @pressed;
            return ptr_of_this_method;
        }

        static object get_hover_2(ref object o)
        {
            return ((global::UIButtonColor)o).hover;
        }

        static StackObject* CopyToStack_hover_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButtonColor)o).hover;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_hover_2(ref object o, object v)
        {
            ((global::UIButtonColor)o).hover = (UnityEngine.Color)v;
        }

        static StackObject* AssignFromStack_hover_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Color @hover = (UnityEngine.Color)typeof(UnityEngine.Color).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButtonColor)o).hover = @hover;
            return ptr_of_this_method;
        }

        static object get_disabledColor_3(ref object o)
        {
            return ((global::UIButtonColor)o).disabledColor;
        }

        static StackObject* CopyToStack_disabledColor_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButtonColor)o).disabledColor;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_disabledColor_3(ref object o, object v)
        {
            ((global::UIButtonColor)o).disabledColor = (UnityEngine.Color)v;
        }

        static StackObject* AssignFromStack_disabledColor_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Color @disabledColor = (UnityEngine.Color)typeof(UnityEngine.Color).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButtonColor)o).disabledColor = @disabledColor;
            return ptr_of_this_method;
        }



    }
}
