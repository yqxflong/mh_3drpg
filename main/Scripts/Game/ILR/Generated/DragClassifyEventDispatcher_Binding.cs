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
    unsafe class DragClassifyEventDispatcher_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::DragClassifyEventDispatcher);

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
            return ((global::DragClassifyEventDispatcher)o).onDragFunc;
        }

        static StackObject* CopyToStack_onDragFunc_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DragClassifyEventDispatcher)o).onDragFunc;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragFunc_0(ref object o, object v)
        {
            ((global::DragClassifyEventDispatcher)o).onDragFunc = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDragFunc_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDragFunc = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DragClassifyEventDispatcher)o).onDragFunc = @onDragFunc;
            return ptr_of_this_method;
        }

        static object get_onDragStartFunc_1(ref object o)
        {
            return ((global::DragClassifyEventDispatcher)o).onDragStartFunc;
        }

        static StackObject* CopyToStack_onDragStartFunc_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DragClassifyEventDispatcher)o).onDragStartFunc;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragStartFunc_1(ref object o, object v)
        {
            ((global::DragClassifyEventDispatcher)o).onDragStartFunc = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDragStartFunc_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDragStartFunc = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DragClassifyEventDispatcher)o).onDragStartFunc = @onDragStartFunc;
            return ptr_of_this_method;
        }

        static object get_onDragEndFunc_2(ref object o)
        {
            return ((global::DragClassifyEventDispatcher)o).onDragEndFunc;
        }

        static StackObject* CopyToStack_onDragEndFunc_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DragClassifyEventDispatcher)o).onDragEndFunc;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDragEndFunc_2(ref object o, object v)
        {
            ((global::DragClassifyEventDispatcher)o).onDragEndFunc = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onDragEndFunc_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onDragEndFunc = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DragClassifyEventDispatcher)o).onDragEndFunc = @onDragEndFunc;
            return ptr_of_this_method;
        }



    }
}
