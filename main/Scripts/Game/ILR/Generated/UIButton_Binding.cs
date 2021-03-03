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
    unsafe class UIButton_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIButton);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_normalSprite", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_normalSprite_0);

            field = type.GetField("onClick", flag);
            app.RegisterCLRFieldGetter(field, get_onClick_0);
            app.RegisterCLRFieldSetter(field, set_onClick_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClick_0, AssignFromStack_onClick_0);
            field = type.GetField("OnClickAction", flag);
            app.RegisterCLRFieldGetter(field, get_OnClickAction_1);
            app.RegisterCLRFieldSetter(field, set_OnClickAction_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnClickAction_1, AssignFromStack_OnClickAction_1);
            field = type.GetField("disabledSprite", flag);
            app.RegisterCLRFieldGetter(field, get_disabledSprite_2);
            app.RegisterCLRFieldSetter(field, set_disabledSprite_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_disabledSprite_2, AssignFromStack_disabledSprite_2);
            field = type.GetField("pressedSprite", flag);
            app.RegisterCLRFieldGetter(field, get_pressedSprite_3);
            app.RegisterCLRFieldSetter(field, set_pressedSprite_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_pressedSprite_3, AssignFromStack_pressedSprite_3);
            field = type.GetField("hoverSprite", flag);
            app.RegisterCLRFieldGetter(field, get_hoverSprite_4);
            app.RegisterCLRFieldSetter(field, set_hoverSprite_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_hoverSprite_4, AssignFromStack_hoverSprite_4);
            field = type.GetField("OnDragOutAction", flag);
            app.RegisterCLRFieldGetter(field, get_OnDragOutAction_5);
            app.RegisterCLRFieldSetter(field, set_OnDragOutAction_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnDragOutAction_5, AssignFromStack_OnDragOutAction_5);
            field = type.GetField("OnDragOverAction", flag);
            app.RegisterCLRFieldGetter(field, get_OnDragOverAction_6);
            app.RegisterCLRFieldSetter(field, set_OnDragOverAction_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnDragOverAction_6, AssignFromStack_OnDragOverAction_6);
            field = type.GetField("OnPressAction", flag);
            app.RegisterCLRFieldGetter(field, get_OnPressAction_7);
            app.RegisterCLRFieldSetter(field, set_OnPressAction_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnPressAction_7, AssignFromStack_OnPressAction_7);

            app.RegisterCLRCreateArrayInstance(type, s => new global::UIButton[s]);


        }


        static StackObject* set_normalSprite_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIButton instance_of_this_method = (global::UIButton)typeof(global::UIButton).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.normalSprite = value;

            return __ret;
        }


        static object get_onClick_0(ref object o)
        {
            return ((global::UIButton)o).onClick;
        }

        static StackObject* CopyToStack_onClick_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButton)o).onClick;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClick_0(ref object o, object v)
        {
            ((global::UIButton)o).onClick = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onClick_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onClick = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButton)o).onClick = @onClick;
            return ptr_of_this_method;
        }

        static object get_OnClickAction_1(ref object o)
        {
            return ((global::UIButton)o).OnClickAction;
        }

        static StackObject* CopyToStack_OnClickAction_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButton)o).OnClickAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnClickAction_1(ref object o, object v)
        {
            ((global::UIButton)o).OnClickAction = (System.Action)v;
        }

        static StackObject* AssignFromStack_OnClickAction_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @OnClickAction = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButton)o).OnClickAction = @OnClickAction;
            return ptr_of_this_method;
        }

        static object get_disabledSprite_2(ref object o)
        {
            return ((global::UIButton)o).disabledSprite;
        }

        static StackObject* CopyToStack_disabledSprite_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButton)o).disabledSprite;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_disabledSprite_2(ref object o, object v)
        {
            ((global::UIButton)o).disabledSprite = (System.String)v;
        }

        static StackObject* AssignFromStack_disabledSprite_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @disabledSprite = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButton)o).disabledSprite = @disabledSprite;
            return ptr_of_this_method;
        }

        static object get_pressedSprite_3(ref object o)
        {
            return ((global::UIButton)o).pressedSprite;
        }

        static StackObject* CopyToStack_pressedSprite_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButton)o).pressedSprite;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_pressedSprite_3(ref object o, object v)
        {
            ((global::UIButton)o).pressedSprite = (System.String)v;
        }

        static StackObject* AssignFromStack_pressedSprite_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @pressedSprite = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButton)o).pressedSprite = @pressedSprite;
            return ptr_of_this_method;
        }

        static object get_hoverSprite_4(ref object o)
        {
            return ((global::UIButton)o).hoverSprite;
        }

        static StackObject* CopyToStack_hoverSprite_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButton)o).hoverSprite;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_hoverSprite_4(ref object o, object v)
        {
            ((global::UIButton)o).hoverSprite = (System.String)v;
        }

        static StackObject* AssignFromStack_hoverSprite_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @hoverSprite = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButton)o).hoverSprite = @hoverSprite;
            return ptr_of_this_method;
        }

        static object get_OnDragOutAction_5(ref object o)
        {
            return ((global::UIButton)o).OnDragOutAction;
        }

        static StackObject* CopyToStack_OnDragOutAction_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButton)o).OnDragOutAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnDragOutAction_5(ref object o, object v)
        {
            ((global::UIButton)o).OnDragOutAction = (System.Action)v;
        }

        static StackObject* AssignFromStack_OnDragOutAction_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @OnDragOutAction = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButton)o).OnDragOutAction = @OnDragOutAction;
            return ptr_of_this_method;
        }

        static object get_OnDragOverAction_6(ref object o)
        {
            return ((global::UIButton)o).OnDragOverAction;
        }

        static StackObject* CopyToStack_OnDragOverAction_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButton)o).OnDragOverAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnDragOverAction_6(ref object o, object v)
        {
            ((global::UIButton)o).OnDragOverAction = (System.Action)v;
        }

        static StackObject* AssignFromStack_OnDragOverAction_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @OnDragOverAction = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButton)o).OnDragOverAction = @OnDragOverAction;
            return ptr_of_this_method;
        }

        static object get_OnPressAction_7(ref object o)
        {
            return ((global::UIButton)o).OnPressAction;
        }

        static StackObject* CopyToStack_OnPressAction_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIButton)o).OnPressAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnPressAction_7(ref object o, object v)
        {
            ((global::UIButton)o).OnPressAction = (System.Action<System.Boolean>)v;
        }

        static StackObject* AssignFromStack_OnPressAction_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.Boolean> @OnPressAction = (System.Action<System.Boolean>)typeof(System.Action<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIButton)o).OnPressAction = @OnPressAction;
            return ptr_of_this_method;
        }



    }
}
