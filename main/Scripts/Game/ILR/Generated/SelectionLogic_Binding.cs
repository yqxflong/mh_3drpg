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
    unsafe class SelectionLogic_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::SelectionLogic);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_MaxTouches", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_MaxTouches_0);

            field = type.GetField("IsShowJoystick", flag);
            app.RegisterCLRFieldGetter(field, get_IsShowJoystick_0);
            app.RegisterCLRFieldSetter(field, set_IsShowJoystick_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_IsShowJoystick_0, AssignFromStack_IsShowJoystick_0);


        }


        static StackObject* set_MaxTouches_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;


            global::SelectionLogic.MaxTouches = value;

            return __ret;
        }


        static object get_IsShowJoystick_0(ref object o)
        {
            return global::SelectionLogic.IsShowJoystick;
        }

        static StackObject* CopyToStack_IsShowJoystick_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::SelectionLogic.IsShowJoystick;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_IsShowJoystick_0(ref object o, object v)
        {
            global::SelectionLogic.IsShowJoystick = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_IsShowJoystick_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @IsShowJoystick = ptr_of_this_method->Value == 1;
            global::SelectionLogic.IsShowJoystick = @IsShowJoystick;
            return ptr_of_this_method;
        }



    }
}
