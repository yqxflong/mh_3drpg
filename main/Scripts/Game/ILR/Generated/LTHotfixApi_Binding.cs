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
    unsafe class LTHotfixApi_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::LTHotfixApi);
            args = new Type[]{};
            method = type.GetMethod("GetInstance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetInstance_0);
            args = new Type[]{typeof(EB.Sparx.Request), typeof(System.Action<System.Collections.Hashtable>)};
            method = type.GetMethod("BlockService", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BlockService_1);
            args = new Type[]{typeof(System.Collections.Hashtable)};
            method = type.GetMethod("FetchDataHandler", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, FetchDataHandler_2);

            field = type.GetField("ExceptionFunc", flag);
            app.RegisterCLRFieldGetter(field, get_ExceptionFunc_0);
            app.RegisterCLRFieldSetter(field, set_ExceptionFunc_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_ExceptionFunc_0, AssignFromStack_ExceptionFunc_0);


        }


        static StackObject* GetInstance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::LTHotfixApi.GetInstance();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* BlockService_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.Collections.Hashtable> @dataHandler = (System.Action<System.Collections.Hashtable>)typeof(System.Action<System.Collections.Hashtable>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.Request @request = (EB.Sparx.Request)typeof(EB.Sparx.Request).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::LTHotfixApi instance_of_this_method = (global::LTHotfixApi)typeof(global::LTHotfixApi).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.BlockService(@request, @dataHandler);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* FetchDataHandler_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Hashtable @data = (System.Collections.Hashtable)typeof(System.Collections.Hashtable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::LTHotfixApi instance_of_this_method = (global::LTHotfixApi)typeof(global::LTHotfixApi).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.FetchDataHandler(@data);

            return __ret;
        }


        static object get_ExceptionFunc_0(ref object o)
        {
            return ((global::LTHotfixApi)o).ExceptionFunc;
        }

        static StackObject* CopyToStack_ExceptionFunc_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::LTHotfixApi)o).ExceptionFunc;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ExceptionFunc_0(ref object o, object v)
        {
            ((global::LTHotfixApi)o).ExceptionFunc = (System.Func<EB.Sparx.Response, System.Boolean>)v;
        }

        static StackObject* AssignFromStack_ExceptionFunc_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Func<EB.Sparx.Response, System.Boolean> @ExceptionFunc = (System.Func<EB.Sparx.Response, System.Boolean>)typeof(System.Func<EB.Sparx.Response, System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::LTHotfixApi)o).ExceptionFunc = @ExceptionFunc;
            return ptr_of_this_method;
        }



    }
}
