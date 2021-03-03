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
    unsafe class ConsecutiveClickCoolTrigger_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ConsecutiveClickCoolTrigger);
            args = new Type[]{};
            method = type.GetMethod("OnClick", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnClick_0);

            field = type.GetField("clickEvent", flag);
            app.RegisterCLRFieldGetter(field, get_clickEvent_0);
            app.RegisterCLRFieldSetter(field, set_clickEvent_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_clickEvent_0, AssignFromStack_clickEvent_0);


        }


        static StackObject* OnClick_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::ConsecutiveClickCoolTrigger instance_of_this_method = (global::ConsecutiveClickCoolTrigger)typeof(global::ConsecutiveClickCoolTrigger).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnClick();

            return __ret;
        }


        static object get_clickEvent_0(ref object o)
        {
            return ((global::ConsecutiveClickCoolTrigger)o).clickEvent;
        }

        static StackObject* CopyToStack_clickEvent_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ConsecutiveClickCoolTrigger)o).clickEvent;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_clickEvent_0(ref object o, object v)
        {
            ((global::ConsecutiveClickCoolTrigger)o).clickEvent = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_clickEvent_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @clickEvent = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::ConsecutiveClickCoolTrigger)o).clickEvent = @clickEvent;
            return ptr_of_this_method;
        }



    }
}
