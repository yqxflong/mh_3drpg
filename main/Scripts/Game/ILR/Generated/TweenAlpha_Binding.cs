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
    unsafe class TweenAlpha_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::TweenAlpha);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_value", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_value_0);

            field = type.GetField("from", flag);
            app.RegisterCLRFieldGetter(field, get_from_0);
            app.RegisterCLRFieldSetter(field, set_from_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_from_0, AssignFromStack_from_0);
            field = type.GetField("to", flag);
            app.RegisterCLRFieldGetter(field, get_to_1);
            app.RegisterCLRFieldSetter(field, set_to_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_to_1, AssignFromStack_to_1);


        }


        static StackObject* set_value_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::TweenAlpha instance_of_this_method = (global::TweenAlpha)typeof(global::TweenAlpha).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.value = value;

            return __ret;
        }


        static object get_from_0(ref object o)
        {
            return ((global::TweenAlpha)o).from;
        }

        static StackObject* CopyToStack_from_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::TweenAlpha)o).from;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_from_0(ref object o, object v)
        {
            ((global::TweenAlpha)o).from = (System.Single)v;
        }

        static StackObject* AssignFromStack_from_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @from = *(float*)&ptr_of_this_method->Value;
            ((global::TweenAlpha)o).from = @from;
            return ptr_of_this_method;
        }

        static object get_to_1(ref object o)
        {
            return ((global::TweenAlpha)o).to;
        }

        static StackObject* CopyToStack_to_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::TweenAlpha)o).to;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_to_1(ref object o, object v)
        {
            ((global::TweenAlpha)o).to = (System.Single)v;
        }

        static StackObject* AssignFromStack_to_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @to = *(float*)&ptr_of_this_method->Value;
            ((global::TweenAlpha)o).to = @to;
            return ptr_of_this_method;
        }



    }
}
