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
    unsafe class UIRoot_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIRoot);

            field = type.GetField("list", flag);
            app.RegisterCLRFieldGetter(field, get_list_0);
            app.RegisterCLRFieldSetter(field, set_list_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_list_0, AssignFromStack_list_0);
            field = type.GetField("manualWidth", flag);
            app.RegisterCLRFieldGetter(field, get_manualWidth_1);
            app.RegisterCLRFieldSetter(field, set_manualWidth_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_manualWidth_1, AssignFromStack_manualWidth_1);
            field = type.GetField("manualHeight", flag);
            app.RegisterCLRFieldGetter(field, get_manualHeight_2);
            app.RegisterCLRFieldSetter(field, set_manualHeight_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_manualHeight_2, AssignFromStack_manualHeight_2);
            field = type.GetField("fitHeight", flag);
            app.RegisterCLRFieldGetter(field, get_fitHeight_3);
            app.RegisterCLRFieldSetter(field, set_fitHeight_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_fitHeight_3, AssignFromStack_fitHeight_3);


        }



        static object get_list_0(ref object o)
        {
            return global::UIRoot.list;
        }

        static StackObject* CopyToStack_list_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::UIRoot.list;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_list_0(ref object o, object v)
        {
            global::UIRoot.list = (System.Collections.Generic.List<global::UIRoot>)v;
        }

        static StackObject* AssignFromStack_list_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::UIRoot> @list = (System.Collections.Generic.List<global::UIRoot>)typeof(System.Collections.Generic.List<global::UIRoot>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::UIRoot.list = @list;
            return ptr_of_this_method;
        }

        static object get_manualWidth_1(ref object o)
        {
            return ((global::UIRoot)o).manualWidth;
        }

        static StackObject* CopyToStack_manualWidth_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIRoot)o).manualWidth;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_manualWidth_1(ref object o, object v)
        {
            ((global::UIRoot)o).manualWidth = (System.Int32)v;
        }

        static StackObject* AssignFromStack_manualWidth_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @manualWidth = ptr_of_this_method->Value;
            ((global::UIRoot)o).manualWidth = @manualWidth;
            return ptr_of_this_method;
        }

        static object get_manualHeight_2(ref object o)
        {
            return ((global::UIRoot)o).manualHeight;
        }

        static StackObject* CopyToStack_manualHeight_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIRoot)o).manualHeight;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_manualHeight_2(ref object o, object v)
        {
            ((global::UIRoot)o).manualHeight = (System.Int32)v;
        }

        static StackObject* AssignFromStack_manualHeight_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @manualHeight = ptr_of_this_method->Value;
            ((global::UIRoot)o).manualHeight = @manualHeight;
            return ptr_of_this_method;
        }

        static object get_fitHeight_3(ref object o)
        {
            return ((global::UIRoot)o).fitHeight;
        }

        static StackObject* CopyToStack_fitHeight_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIRoot)o).fitHeight;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_fitHeight_3(ref object o, object v)
        {
            ((global::UIRoot)o).fitHeight = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_fitHeight_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @fitHeight = ptr_of_this_method->Value == 1;
            ((global::UIRoot)o).fitHeight = @fitHeight;
            return ptr_of_this_method;
        }



    }
}
