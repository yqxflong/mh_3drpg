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
    unsafe class TouchUpdateEvent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::TouchUpdateEvent);

            field = type.GetField("hasValidNavPoint", flag);
            app.RegisterCLRFieldGetter(field, get_hasValidNavPoint_0);
            app.RegisterCLRFieldSetter(field, set_hasValidNavPoint_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_hasValidNavPoint_0, AssignFromStack_hasValidNavPoint_0);


        }



        static object get_hasValidNavPoint_0(ref object o)
        {
            return ((global::TouchUpdateEvent)o).hasValidNavPoint;
        }

        static StackObject* CopyToStack_hasValidNavPoint_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::TouchUpdateEvent)o).hasValidNavPoint;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_hasValidNavPoint_0(ref object o, object v)
        {
            ((global::TouchUpdateEvent)o).hasValidNavPoint = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_hasValidNavPoint_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @hasValidNavPoint = ptr_of_this_method->Value == 1;
            ((global::TouchUpdateEvent)o).hasValidNavPoint = @hasValidNavPoint;
            return ptr_of_this_method;
        }



    }
}
