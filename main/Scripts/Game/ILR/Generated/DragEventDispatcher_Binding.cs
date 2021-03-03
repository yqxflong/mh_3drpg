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
    unsafe class DragEventDispatcher_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::DragEventDispatcher);

            field = type.GetField("onDragFunc", flag);
            app.RegisterCLRFieldGetter(field, get_onDragFunc_0);
            app.RegisterCLRFieldSetter(field, set_onDragFunc_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragFunc_0, AssignFromStack_onDragFunc_0);
            field = type.GetField("onDragStartFunc", flag);
            app.RegisterCLRFieldGetter(field, get_onDragStartFunc_1);
            app.RegisterCLRFieldSetter(field, set_onDragStartFunc_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragStartFunc_1, AssignFromStack_onDragStartFunc_1);
            field = type.GetField("onDragEndFunc", flag);
            app.RegisterCLRFieldGetter(field, get_onDragEndFunc_2);
            app.RegisterCLRFieldSetter(field, set_onDragEndFunc_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDragEndFunc_2, AssignFromStack_onDragEndFunc_2);


        }



        static object get_onDragFunc_0(ref object o)
        {
            return ((global::DragEventDispatcher)o).onDragFunc;
        }

        static StackObject* CopyToStack_onDragFunc_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DragEventDispatcher)o).onDragFunc;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragFunc_0(ref object o, object v)
        {
            ((global::DragEventDispatcher)o).onDragFunc = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDragFunc_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDragFunc = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DragEventDispatcher)o).onDragFunc = @onDragFunc;
            return ptr_of_this_method;
        }

        static object get_onDragStartFunc_1(ref object o)
        {
            return ((global::DragEventDispatcher)o).onDragStartFunc;
        }

        static StackObject* CopyToStack_onDragStartFunc_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DragEventDispatcher)o).onDragStartFunc;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragStartFunc_1(ref object o, object v)
        {
            ((global::DragEventDispatcher)o).onDragStartFunc = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDragStartFunc_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDragStartFunc = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DragEventDispatcher)o).onDragStartFunc = @onDragStartFunc;
            return ptr_of_this_method;
        }

        static object get_onDragEndFunc_2(ref object o)
        {
            return ((global::DragEventDispatcher)o).onDragEndFunc;
        }

        static StackObject* CopyToStack_onDragEndFunc_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DragEventDispatcher)o).onDragEndFunc;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragEndFunc_2(ref object o, object v)
        {
            ((global::DragEventDispatcher)o).onDragEndFunc = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDragEndFunc_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDragEndFunc = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DragEventDispatcher)o).onDragEndFunc = @onDragEndFunc;
            return ptr_of_this_method;
        }



    }
}
