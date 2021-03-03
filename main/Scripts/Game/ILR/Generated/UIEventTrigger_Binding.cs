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
    unsafe class UIEventTrigger_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIEventTrigger);

            field = type.GetField("onClick", flag);
            app.RegisterCLRFieldGetter(field, get_onClick_0);
            app.RegisterCLRFieldSetter(field, set_onClick_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClick_0, AssignFromStack_onClick_0);
            field = type.GetField("onPress", flag);
            app.RegisterCLRFieldGetter(field, get_onPress_1);
            app.RegisterCLRFieldSetter(field, set_onPress_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onPress_1, AssignFromStack_onPress_1);
            field = type.GetField("onDragStart", flag);
            app.RegisterCLRFieldGetter(field, get_onDragStart_2);
            app.RegisterCLRFieldSetter(field, set_onDragStart_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragStart_2, AssignFromStack_onDragStart_2);
            field = type.GetField("onDragEnd", flag);
            app.RegisterCLRFieldGetter(field, get_onDragEnd_3);
            app.RegisterCLRFieldSetter(field, set_onDragEnd_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragEnd_3, AssignFromStack_onDragEnd_3);
            field = type.GetField("onDragOut", flag);
            app.RegisterCLRFieldGetter(field, get_onDragOut_4);
            app.RegisterCLRFieldSetter(field, set_onDragOut_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragOut_4, AssignFromStack_onDragOut_4);
            field = type.GetField("onDragOver", flag);
            app.RegisterCLRFieldGetter(field, get_onDragOver_5);
            app.RegisterCLRFieldSetter(field, set_onDragOver_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragOver_5, AssignFromStack_onDragOver_5);
            field = type.GetField("onDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onDrag_6);
            app.RegisterCLRFieldSetter(field, set_onDrag_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDrag_6, AssignFromStack_onDrag_6);
            field = type.GetField("onRelease", flag);
            app.RegisterCLRFieldGetter(field, get_onRelease_7);
            app.RegisterCLRFieldSetter(field, set_onRelease_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_onRelease_7, AssignFromStack_onRelease_7);
            field = type.GetField("onHoverOut", flag);
            app.RegisterCLRFieldGetter(field, get_onHoverOut_8);
            app.RegisterCLRFieldSetter(field, set_onHoverOut_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_onHoverOut_8, AssignFromStack_onHoverOut_8);
            field = type.GetField("onHoverOver", flag);
            app.RegisterCLRFieldGetter(field, get_onHoverOver_9);
            app.RegisterCLRFieldSetter(field, set_onHoverOver_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_onHoverOver_9, AssignFromStack_onHoverOver_9);
            field = type.GetField("onDeselect", flag);
            app.RegisterCLRFieldGetter(field, get_onDeselect_10);
            app.RegisterCLRFieldSetter(field, set_onDeselect_10);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDeselect_10, AssignFromStack_onDeselect_10);
            field = type.GetField("onDoubleClick", flag);
            app.RegisterCLRFieldGetter(field, get_onDoubleClick_11);
            app.RegisterCLRFieldSetter(field, set_onDoubleClick_11);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDoubleClick_11, AssignFromStack_onDoubleClick_11);


        }



        static object get_onClick_0(ref object o)
        {
            return ((global::UIEventTrigger)o).onClick;
        }

        static StackObject* CopyToStack_onClick_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onClick;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClick_0(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onClick = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onClick_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onClick = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onClick = @onClick;
            return ptr_of_this_method;
        }

        static object get_onPress_1(ref object o)
        {
            return ((global::UIEventTrigger)o).onPress;
        }

        static StackObject* CopyToStack_onPress_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onPress;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onPress_1(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onPress = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onPress_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onPress = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onPress = @onPress;
            return ptr_of_this_method;
        }

        static object get_onDragStart_2(ref object o)
        {
            return ((global::UIEventTrigger)o).onDragStart;
        }

        static StackObject* CopyToStack_onDragStart_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onDragStart;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragStart_2(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onDragStart = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDragStart_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDragStart = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onDragStart = @onDragStart;
            return ptr_of_this_method;
        }

        static object get_onDragEnd_3(ref object o)
        {
            return ((global::UIEventTrigger)o).onDragEnd;
        }

        static StackObject* CopyToStack_onDragEnd_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onDragEnd;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragEnd_3(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onDragEnd = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDragEnd_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDragEnd = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onDragEnd = @onDragEnd;
            return ptr_of_this_method;
        }

        static object get_onDragOut_4(ref object o)
        {
            return ((global::UIEventTrigger)o).onDragOut;
        }

        static StackObject* CopyToStack_onDragOut_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onDragOut;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragOut_4(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onDragOut = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDragOut_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDragOut = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onDragOut = @onDragOut;
            return ptr_of_this_method;
        }

        static object get_onDragOver_5(ref object o)
        {
            return ((global::UIEventTrigger)o).onDragOver;
        }

        static StackObject* CopyToStack_onDragOver_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onDragOver;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragOver_5(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onDragOver = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDragOver_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDragOver = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onDragOver = @onDragOver;
            return ptr_of_this_method;
        }

        static object get_onDrag_6(ref object o)
        {
            return ((global::UIEventTrigger)o).onDrag;
        }

        static StackObject* CopyToStack_onDrag_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onDrag;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDrag_6(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onDrag = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDrag_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDrag = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onDrag = @onDrag;
            return ptr_of_this_method;
        }

        static object get_onRelease_7(ref object o)
        {
            return ((global::UIEventTrigger)o).onRelease;
        }

        static StackObject* CopyToStack_onRelease_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onRelease;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onRelease_7(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onRelease = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onRelease_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onRelease = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onRelease = @onRelease;
            return ptr_of_this_method;
        }

        static object get_onHoverOut_8(ref object o)
        {
            return ((global::UIEventTrigger)o).onHoverOut;
        }

        static StackObject* CopyToStack_onHoverOut_8(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onHoverOut;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onHoverOut_8(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onHoverOut = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onHoverOut_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onHoverOut = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onHoverOut = @onHoverOut;
            return ptr_of_this_method;
        }

        static object get_onHoverOver_9(ref object o)
        {
            return ((global::UIEventTrigger)o).onHoverOver;
        }

        static StackObject* CopyToStack_onHoverOver_9(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onHoverOver;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onHoverOver_9(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onHoverOver = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onHoverOver_9(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onHoverOver = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onHoverOver = @onHoverOver;
            return ptr_of_this_method;
        }

        static object get_onDeselect_10(ref object o)
        {
            return ((global::UIEventTrigger)o).onDeselect;
        }

        static StackObject* CopyToStack_onDeselect_10(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onDeselect;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDeselect_10(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onDeselect = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDeselect_10(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDeselect = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onDeselect = @onDeselect;
            return ptr_of_this_method;
        }

        static object get_onDoubleClick_11(ref object o)
        {
            return ((global::UIEventTrigger)o).onDoubleClick;
        }

        static StackObject* CopyToStack_onDoubleClick_11(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIEventTrigger)o).onDoubleClick;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDoubleClick_11(ref object o, object v)
        {
            ((global::UIEventTrigger)o).onDoubleClick = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDoubleClick_11(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDoubleClick = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIEventTrigger)o).onDoubleClick = @onDoubleClick;
            return ptr_of_this_method;
        }



    }
}
