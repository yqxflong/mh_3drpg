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
    unsafe class HotfixILRManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::HotfixILRManager);
            args = new Type[]{};
            method = type.GetMethod("GetInstance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetInstance_0);

            field = type.GetField("IsInit", flag);
            app.RegisterCLRFieldGetter(field, get_IsInit_0);
            app.RegisterCLRFieldSetter(field, set_IsInit_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_IsInit_0, AssignFromStack_IsInit_0);


        }


        static StackObject* GetInstance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::HotfixILRManager.GetInstance();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_IsInit_0(ref object o)
        {
            return ((global::HotfixILRManager)o).IsInit;
        }

        static StackObject* CopyToStack_IsInit_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::HotfixILRManager)o).IsInit;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_IsInit_0(ref object o, object v)
        {
            ((global::HotfixILRManager)o).IsInit = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_IsInit_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @IsInit = ptr_of_this_method->Value == 1;
            ((global::HotfixILRManager)o).IsInit = @IsInit;
            return ptr_of_this_method;
        }



    }
}
