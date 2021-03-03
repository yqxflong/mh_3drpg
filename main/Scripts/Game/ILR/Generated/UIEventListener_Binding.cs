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
    unsafe class UIEventListener_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIEventListener);
            args = new Type[]{typeof(UnityEngine.GameObject)};
            method = type.GetMethod("Get", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Get_0);

            field = type.GetField("onClick", flag);
            app.RegisterCLRFieldGetter(field, get_onClick_0);
            app.RegisterCLRFieldSetter(field, set_onClick_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClick_0, AssignFromStack_onClick_0);
            field = type.GetField("onDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onDrag_1);
            app.RegisterCLRFieldSetter(field, set_onDrag_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDrag_1, AssignFromStack_onDrag_1);
            field = type.GetField("onPress", flag);
            app.RegisterCLRFieldGetter(field, get_onPress_2);
            app.RegisterCLRFieldSetter(field, set_onPress_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_onPress_2, AssignFromStack_onPress_2);
            field = type.GetField("onDragStart", flag);
            app.RegisterCLRFieldGetter(field, get_onDragStart_3);
            app.RegisterCLRFieldSetter(field, set_onDragStart_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragStart_3, AssignFromStack_onDragStart_3);
            field = type.GetField("onDragEnd", flag);
            app.RegisterCLRFieldGetter(field, get_onDragEnd_4);
            app.RegisterCLRFieldSetter(field, set_onDragEnd_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragEnd_4, AssignFromStack_onDragEnd_4);
            field = type.GetField("onDoubleClick", flag);
            app.RegisterCLRFieldGetter(field, get_onDoubleClick_5);
            app.RegisterCLRFieldSetter(field, set_onDoubleClick_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDoubleClick_5, AssignFromStack_onDoubleClick_5);
            field = type.GetField("onDragOut", flag);
            app.RegisterCLRFieldGetter(field, get_onDragOut_6);
            app.RegisterCLRFieldSetter(field, set_onDragOut_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragOut_6, AssignFromStack_onDragOut_6);
            field = type.GetField("onDragOver", flag);
            app.RegisterCLRFieldGetter(field, get_onDragOver_7);
            app.RegisterCLRFieldSetter(field, set_onDragOver_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragOver_7, AssignFromStack_onDragOver_7);
            field = type.GetField("onDrop", flag);
            app.RegisterCLRFieldGetter(field, get_onDrop_8);
            app.RegisterCLRFieldSetter(field, set_onDrop_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDrop_8, AssignFromStack_onDrop_8);
            field = type.GetField("onHover", flag);
            app.RegisterCLRFieldGetter(field, get_onHover_9);
            app.RegisterCLRFieldSetter(field, set_onHover_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_onHover_9, AssignFromStack_onHover_9);
            field = type.GetField("onKey", flag);
            app.RegisterCLRFieldGetter(field, get_onKey_10);
            app.RegisterCLRFieldSetter(field, set_onKey_10);
            app.RegisterCLRFieldBinding(field, CopyToStack_onKey_10, AssignFromStack_onKey_10);
            field = type.GetField("onScroll", flag);
            app.RegisterCLRFieldGetter(field, get_onScroll_11);
            app.RegisterCLRFieldSetter(field, set_onScroll_11);
            app.RegisterCLRFieldBinding(field, CopyToStack_onScroll_11, AssignFromStack_onScroll_11);
            field = type.GetField("onSelect", flag);
            app.RegisterCLRFieldGetter(field, get_onSelect_12);
            app.RegisterCLRFieldSetter(field, set_onSelect_12);
            app.RegisterCLRFieldBinding(field, CopyToStack_onSelect_12, AssignFromStack_onSelect_12);
            field = type.GetField("onSubmit", flag);
            app.RegisterCLRFieldGetter(field, get_onSubmit_13);
            app.RegisterCLRFieldSetter(field, set_onSubmit_13);
            app.RegisterCLRFieldBinding(field, CopyToStack_onSubmit_13, AssignFromStack_onSubmit_13);
            field = type.GetField("onTooltip", flag);
            app.RegisterCLRFieldGetter(field, get_onTooltip_14);
            app.RegisterCLRFieldSetter(field, set_onTooltip_14);
            app.RegisterCLRFieldBinding(field, CopyToStack_onTooltip_14, AssignFromStack_onTooltip_14);

            app.RegisterCLRCreateArrayInstance(type, s => new global::UIEventListener[s]);


        }


        static StackObject* Get_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject @go = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::UIEventListener.Get(@go);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_onClick_0(ref object o)
        {
            return ((global::UIEventListener)o).onClick;
        }

        static StackObject* CopyToStack_onClick_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onClick;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClick_0(ref object o, object v)
        {
            ((global::UIEventListener)o).onClick = (global::UIEventListener.VoidDelegate)v;
        }

        static StackObject* AssignFromStack_onClick_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.VoidDelegate @onClick = (global::UIEventListener.VoidDelegate)typeof(global::UIEventListener.VoidDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onClick = @onClick;
            return ptr_of_this_method;
        }

        static object get_onDrag_1(ref object o)
        {
            return ((global::UIEventListener)o).onDrag;
        }

        static StackObject* CopyToStack_onDrag_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDrag_1(ref object o, object v)
        {
            ((global::UIEventListener)o).onDrag = (global::UIEventListener.VectorDelegate)v;
        }

        static StackObject* AssignFromStack_onDrag_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.VectorDelegate @onDrag = (global::UIEventListener.VectorDelegate)typeof(global::UIEventListener.VectorDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onDrag = @onDrag;
            return ptr_of_this_method;
        }

        static object get_onPress_2(ref object o)
        {
            return ((global::UIEventListener)o).onPress;
        }

        static StackObject* CopyToStack_onPress_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onPress;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onPress_2(ref object o, object v)
        {
            ((global::UIEventListener)o).onPress = (global::UIEventListener.BoolDelegate)v;
        }

        static StackObject* AssignFromStack_onPress_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.BoolDelegate @onPress = (global::UIEventListener.BoolDelegate)typeof(global::UIEventListener.BoolDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onPress = @onPress;
            return ptr_of_this_method;
        }

        static object get_onDragStart_3(ref object o)
        {
            return ((global::UIEventListener)o).onDragStart;
        }

        static StackObject* CopyToStack_onDragStart_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onDragStart;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragStart_3(ref object o, object v)
        {
            ((global::UIEventListener)o).onDragStart = (global::UIEventListener.VoidDelegate)v;
        }

        static StackObject* AssignFromStack_onDragStart_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.VoidDelegate @onDragStart = (global::UIEventListener.VoidDelegate)typeof(global::UIEventListener.VoidDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onDragStart = @onDragStart;
            return ptr_of_this_method;
        }

        static object get_onDragEnd_4(ref object o)
        {
            return ((global::UIEventListener)o).onDragEnd;
        }

        static StackObject* CopyToStack_onDragEnd_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onDragEnd;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragEnd_4(ref object o, object v)
        {
            ((global::UIEventListener)o).onDragEnd = (global::UIEventListener.VoidDelegate)v;
        }

        static StackObject* AssignFromStack_onDragEnd_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.VoidDelegate @onDragEnd = (global::UIEventListener.VoidDelegate)typeof(global::UIEventListener.VoidDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onDragEnd = @onDragEnd;
            return ptr_of_this_method;
        }

        static object get_onDoubleClick_5(ref object o)
        {
            return ((global::UIEventListener)o).onDoubleClick;
        }

        static StackObject* CopyToStack_onDoubleClick_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onDoubleClick;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDoubleClick_5(ref object o, object v)
        {
            ((global::UIEventListener)o).onDoubleClick = (global::UIEventListener.VoidDelegate)v;
        }

        static StackObject* AssignFromStack_onDoubleClick_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.VoidDelegate @onDoubleClick = (global::UIEventListener.VoidDelegate)typeof(global::UIEventListener.VoidDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onDoubleClick = @onDoubleClick;
            return ptr_of_this_method;
        }

        static object get_onDragOut_6(ref object o)
        {
            return ((global::UIEventListener)o).onDragOut;
        }

        static StackObject* CopyToStack_onDragOut_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onDragOut;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragOut_6(ref object o, object v)
        {
            ((global::UIEventListener)o).onDragOut = (global::UIEventListener.VoidDelegate)v;
        }

        static StackObject* AssignFromStack_onDragOut_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.VoidDelegate @onDragOut = (global::UIEventListener.VoidDelegate)typeof(global::UIEventListener.VoidDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onDragOut = @onDragOut;
            return ptr_of_this_method;
        }

        static object get_onDragOver_7(ref object o)
        {
            return ((global::UIEventListener)o).onDragOver;
        }

        static StackObject* CopyToStack_onDragOver_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onDragOver;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragOver_7(ref object o, object v)
        {
            ((global::UIEventListener)o).onDragOver = (global::UIEventListener.VoidDelegate)v;
        }

        static StackObject* AssignFromStack_onDragOver_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.VoidDelegate @onDragOver = (global::UIEventListener.VoidDelegate)typeof(global::UIEventListener.VoidDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onDragOver = @onDragOver;
            return ptr_of_this_method;
        }

        static object get_onDrop_8(ref object o)
        {
            return ((global::UIEventListener)o).onDrop;
        }

        static StackObject* CopyToStack_onDrop_8(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onDrop;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDrop_8(ref object o, object v)
        {
            ((global::UIEventListener)o).onDrop = (global::UIEventListener.ObjectDelegate)v;
        }

        static StackObject* AssignFromStack_onDrop_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.ObjectDelegate @onDrop = (global::UIEventListener.ObjectDelegate)typeof(global::UIEventListener.ObjectDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onDrop = @onDrop;
            return ptr_of_this_method;
        }

        static object get_onHover_9(ref object o)
        {
            return ((global::UIEventListener)o).onHover;
        }

        static StackObject* CopyToStack_onHover_9(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onHover;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onHover_9(ref object o, object v)
        {
            ((global::UIEventListener)o).onHover = (global::UIEventListener.BoolDelegate)v;
        }

        static StackObject* AssignFromStack_onHover_9(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.BoolDelegate @onHover = (global::UIEventListener.BoolDelegate)typeof(global::UIEventListener.BoolDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onHover = @onHover;
            return ptr_of_this_method;
        }

        static object get_onKey_10(ref object o)
        {
            return ((global::UIEventListener)o).onKey;
        }

        static StackObject* CopyToStack_onKey_10(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onKey;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onKey_10(ref object o, object v)
        {
            ((global::UIEventListener)o).onKey = (global::UIEventListener.KeyCodeDelegate)v;
        }

        static StackObject* AssignFromStack_onKey_10(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.KeyCodeDelegate @onKey = (global::UIEventListener.KeyCodeDelegate)typeof(global::UIEventListener.KeyCodeDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onKey = @onKey;
            return ptr_of_this_method;
        }

        static object get_onScroll_11(ref object o)
        {
            return ((global::UIEventListener)o).onScroll;
        }

        static StackObject* CopyToStack_onScroll_11(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onScroll;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onScroll_11(ref object o, object v)
        {
            ((global::UIEventListener)o).onScroll = (global::UIEventListener.FloatDelegate)v;
        }

        static StackObject* AssignFromStack_onScroll_11(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.FloatDelegate @onScroll = (global::UIEventListener.FloatDelegate)typeof(global::UIEventListener.FloatDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onScroll = @onScroll;
            return ptr_of_this_method;
        }

        static object get_onSelect_12(ref object o)
        {
            return ((global::UIEventListener)o).onSelect;
        }

        static StackObject* CopyToStack_onSelect_12(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onSelect;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onSelect_12(ref object o, object v)
        {
            ((global::UIEventListener)o).onSelect = (global::UIEventListener.BoolDelegate)v;
        }

        static StackObject* AssignFromStack_onSelect_12(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.BoolDelegate @onSelect = (global::UIEventListener.BoolDelegate)typeof(global::UIEventListener.BoolDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onSelect = @onSelect;
            return ptr_of_this_method;
        }

        static object get_onSubmit_13(ref object o)
        {
            return ((global::UIEventListener)o).onSubmit;
        }

        static StackObject* CopyToStack_onSubmit_13(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onSubmit;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onSubmit_13(ref object o, object v)
        {
            ((global::UIEventListener)o).onSubmit = (global::UIEventListener.VoidDelegate)v;
        }

        static StackObject* AssignFromStack_onSubmit_13(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.VoidDelegate @onSubmit = (global::UIEventListener.VoidDelegate)typeof(global::UIEventListener.VoidDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onSubmit = @onSubmit;
            return ptr_of_this_method;
        }

        static object get_onTooltip_14(ref object o)
        {
            return ((global::UIEventListener)o).onTooltip;
        }

        static StackObject* CopyToStack_onTooltip_14(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventListener)o).onTooltip;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onTooltip_14(ref object o, object v)
        {
            ((global::UIEventListener)o).onTooltip = (global::UIEventListener.BoolDelegate)v;
        }

        static StackObject* AssignFromStack_onTooltip_14(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIEventListener.BoolDelegate @onTooltip = (global::UIEventListener.BoolDelegate)typeof(global::UIEventListener.BoolDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventListener)o).onTooltip = @onTooltip;
            return ptr_of_this_method;
        }



    }
}
