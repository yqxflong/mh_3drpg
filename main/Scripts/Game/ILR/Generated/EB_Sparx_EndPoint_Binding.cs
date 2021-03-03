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
    unsafe class EB_Sparx_EndPoint_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.EndPoint);
            args = new Type[]{typeof(EB.Sparx.Request), typeof(System.Action<EB.Sparx.Response>)};
            method = type.GetMethod("Service", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Service_0);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("Post", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Post_1);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("Get", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Get_2);
            args = new Type[]{};
            method = type.GetMethod("get_Url", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Url_3);
            args = new Type[]{typeof(System.String), typeof(System.String)};
            method = type.GetMethod("AddData", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddData_4);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("GetData", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetData_5);
            args = new Type[]{};
            method = type.GetMethod("Dispose", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Dispose_6);

            field = type.GetField("PostHandler", flag);
            app.RegisterCLRFieldGetter(field, get_PostHandler_0);
            app.RegisterCLRFieldSetter(field, set_PostHandler_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_PostHandler_0, AssignFromStack_PostHandler_0);
            field = type.GetField("SuspendHandler", flag);
            app.RegisterCLRFieldGetter(field, get_SuspendHandler_1);
            app.RegisterCLRFieldSetter(field, set_SuspendHandler_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_SuspendHandler_1, AssignFromStack_SuspendHandler_1);


        }


        static StackObject* Service_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<EB.Sparx.Response> @callback = (System.Action<EB.Sparx.Response>)typeof(System.Action<EB.Sparx.Response>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.Request @request = (EB.Sparx.Request)typeof(EB.Sparx.Request).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            EB.Sparx.EndPoint instance_of_this_method = (EB.Sparx.EndPoint)typeof(EB.Sparx.EndPoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Service(@request, @callback);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* Post_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @path = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.EndPoint instance_of_this_method = (EB.Sparx.EndPoint)typeof(EB.Sparx.EndPoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Post(@path);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Get_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @path = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.EndPoint instance_of_this_method = (EB.Sparx.EndPoint)typeof(EB.Sparx.EndPoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Get(@path);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Url_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.EndPoint instance_of_this_method = (EB.Sparx.EndPoint)typeof(EB.Sparx.EndPoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Url;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* AddData_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @key = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            EB.Sparx.EndPoint instance_of_this_method = (EB.Sparx.EndPoint)typeof(EB.Sparx.EndPoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddData(@key, @value);

            return __ret;
        }

        static StackObject* GetData_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @key = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.EndPoint instance_of_this_method = (EB.Sparx.EndPoint)typeof(EB.Sparx.EndPoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetData(@key);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Dispose_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.EndPoint instance_of_this_method = (EB.Sparx.EndPoint)typeof(EB.Sparx.EndPoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Dispose();

            return __ret;
        }


        static object get_PostHandler_0(ref object o)
        {
            return ((EB.Sparx.EndPoint)o).PostHandler;
        }

        static StackObject* CopyToStack_PostHandler_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.EndPoint)o).PostHandler;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_PostHandler_0(ref object o, object v)
        {
            ((EB.Sparx.EndPoint)o).PostHandler = (System.Action<EB.Sparx.Response>)v;
        }

        static StackObject* AssignFromStack_PostHandler_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<EB.Sparx.Response> @PostHandler = (System.Action<EB.Sparx.Response>)typeof(System.Action<EB.Sparx.Response>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.EndPoint)o).PostHandler = @PostHandler;
            return ptr_of_this_method;
        }

        static object get_SuspendHandler_1(ref object o)
        {
            return ((EB.Sparx.EndPoint)o).SuspendHandler;
        }

        static StackObject* CopyToStack_SuspendHandler_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.EndPoint)o).SuspendHandler;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SuspendHandler_1(ref object o, object v)
        {
            ((EB.Sparx.EndPoint)o).SuspendHandler = (System.Action<EB.Sparx.Response>)v;
        }

        static StackObject* AssignFromStack_SuspendHandler_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<EB.Sparx.Response> @SuspendHandler = (System.Action<EB.Sparx.Response>)typeof(System.Action<EB.Sparx.Response>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.EndPoint)o).SuspendHandler = @SuspendHandler;
            return ptr_of_this_method;
        }



    }
}
