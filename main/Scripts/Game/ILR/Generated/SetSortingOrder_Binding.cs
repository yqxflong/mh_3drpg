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
    unsafe class SetSortingOrder_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::SetSortingOrder);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetLayer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetLayer_0);

            field = type.GetField("SortingOrder", flag);
            app.RegisterCLRFieldGetter(field, get_SortingOrder_0);
            app.RegisterCLRFieldSetter(field, set_SortingOrder_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_SortingOrder_0, AssignFromStack_SortingOrder_0);


        }


        static StackObject* SetLayer_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @sortingOrder = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::SetSortingOrder instance_of_this_method = (global::SetSortingOrder)typeof(global::SetSortingOrder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetLayer(@sortingOrder);

            return __ret;
        }


        static object get_SortingOrder_0(ref object o)
        {
            return ((global::SetSortingOrder)o).SortingOrder;
        }

        static StackObject* CopyToStack_SortingOrder_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::SetSortingOrder)o).SortingOrder;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_SortingOrder_0(ref object o, object v)
        {
            ((global::SetSortingOrder)o).SortingOrder = (System.Int32)v;
        }

        static StackObject* AssignFromStack_SortingOrder_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @SortingOrder = ptr_of_this_method->Value;
            ((global::SetSortingOrder)o).SortingOrder = @SortingOrder;
            return ptr_of_this_method;
        }



    }
}
