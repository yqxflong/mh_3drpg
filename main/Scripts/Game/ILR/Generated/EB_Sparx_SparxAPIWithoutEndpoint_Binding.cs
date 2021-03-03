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
    unsafe class EB_Sparx_SparxAPIWithoutEndpoint_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.SparxAPIWithoutEndpoint);
            args = new Type[]{typeof(EB.Sparx.Response)};
            method = type.GetMethod("ProcessResponse", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ProcessResponse_0);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("CheckError", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CheckError_1);
            args = new Type[]{typeof(EB.Sparx.Response), typeof(EB.Sparx.eResponseCode)};
            method = type.GetMethod("ProcessError", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ProcessError_2);

            field = type.GetField("ErrorHandler", flag);
            app.RegisterCLRFieldGetter(field, get_ErrorHandler_0);
            app.RegisterCLRFieldSetter(field, set_ErrorHandler_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_ErrorHandler_0, AssignFromStack_ErrorHandler_0);
            field = type.GetField("GlobalErrorHandler", flag);
            app.RegisterCLRFieldGetter(field, get_GlobalErrorHandler_1);
            app.RegisterCLRFieldSetter(field, set_GlobalErrorHandler_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_GlobalErrorHandler_1, AssignFromStack_GlobalErrorHandler_1);
            field = type.GetField("GlobalResultHandler", flag);
            app.RegisterCLRFieldGetter(field, get_GlobalResultHandler_2);
            app.RegisterCLRFieldSetter(field, set_GlobalResultHandler_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_GlobalResultHandler_2, AssignFromStack_GlobalResultHandler_2);


        }


        static StackObject* ProcessResponse_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.Response @response = (EB.Sparx.Response)typeof(EB.Sparx.Response).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.SparxAPIWithoutEndpoint instance_of_this_method = (EB.Sparx.SparxAPIWithoutEndpoint)typeof(EB.Sparx.SparxAPIWithoutEndpoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ProcessResponse(@response);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* CheckError_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @err = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.SparxAPIWithoutEndpoint instance_of_this_method = (EB.Sparx.SparxAPIWithoutEndpoint)typeof(EB.Sparx.SparxAPIWithoutEndpoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CheckError(@err);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ProcessError_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.eResponseCode @errCode = (EB.Sparx.eResponseCode)typeof(EB.Sparx.eResponseCode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.Response @response = (EB.Sparx.Response)typeof(EB.Sparx.Response).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            EB.Sparx.SparxAPIWithoutEndpoint instance_of_this_method = (EB.Sparx.SparxAPIWithoutEndpoint)typeof(EB.Sparx.SparxAPIWithoutEndpoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ProcessError(@response, @errCode);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_ErrorHandler_0(ref object o)
        {
            return ((EB.Sparx.SparxAPIWithoutEndpoint)o).ErrorHandler;
        }

        static StackObject* CopyToStack_ErrorHandler_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.SparxAPIWithoutEndpoint)o).ErrorHandler;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ErrorHandler_0(ref object o, object v)
        {
            ((EB.Sparx.SparxAPIWithoutEndpoint)o).ErrorHandler = (System.Func<EB.Sparx.Response, EB.Sparx.eResponseCode, System.Boolean>)v;
        }

        static StackObject* AssignFromStack_ErrorHandler_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Func<EB.Sparx.Response, EB.Sparx.eResponseCode, System.Boolean> @ErrorHandler = (System.Func<EB.Sparx.Response, EB.Sparx.eResponseCode, System.Boolean>)typeof(System.Func<EB.Sparx.Response, EB.Sparx.eResponseCode, System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.SparxAPIWithoutEndpoint)o).ErrorHandler = @ErrorHandler;
            return ptr_of_this_method;
        }

        static object get_GlobalErrorHandler_1(ref object o)
        {
            return EB.Sparx.SparxAPIWithoutEndpoint.GlobalErrorHandler;
        }

        static StackObject* CopyToStack_GlobalErrorHandler_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = EB.Sparx.SparxAPIWithoutEndpoint.GlobalErrorHandler;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GlobalErrorHandler_1(ref object o, object v)
        {
            EB.Sparx.SparxAPIWithoutEndpoint.GlobalErrorHandler = (System.Func<EB.Sparx.Response, EB.Sparx.eResponseCode, System.Boolean>)v;
        }

        static StackObject* AssignFromStack_GlobalErrorHandler_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Func<EB.Sparx.Response, EB.Sparx.eResponseCode, System.Boolean> @GlobalErrorHandler = (System.Func<EB.Sparx.Response, EB.Sparx.eResponseCode, System.Boolean>)typeof(System.Func<EB.Sparx.Response, EB.Sparx.eResponseCode, System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            EB.Sparx.SparxAPIWithoutEndpoint.GlobalErrorHandler = @GlobalErrorHandler;
            return ptr_of_this_method;
        }

        static object get_GlobalResultHandler_2(ref object o)
        {
            return EB.Sparx.SparxAPIWithoutEndpoint.GlobalResultHandler;
        }

        static StackObject* CopyToStack_GlobalResultHandler_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = EB.Sparx.SparxAPIWithoutEndpoint.GlobalResultHandler;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GlobalResultHandler_2(ref object o, object v)
        {
            EB.Sparx.SparxAPIWithoutEndpoint.GlobalResultHandler = (System.Action<EB.Sparx.Response>)v;
        }

        static StackObject* AssignFromStack_GlobalResultHandler_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<EB.Sparx.Response> @GlobalResultHandler = (System.Action<EB.Sparx.Response>)typeof(System.Action<EB.Sparx.Response>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            EB.Sparx.SparxAPIWithoutEndpoint.GlobalResultHandler = @GlobalResultHandler;
            return ptr_of_this_method;
        }



    }
}
