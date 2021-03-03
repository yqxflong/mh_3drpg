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
    unsafe class UIServerRequest_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIServerRequest);
            args = new Type[]{};
            method = type.GetMethod("SendRequest", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SendRequest_0);

            field = type.GetField("onResponse", flag);
            app.RegisterCLRFieldGetter(field, get_onResponse_0);
            app.RegisterCLRFieldSetter(field, set_onResponse_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onResponse_0, AssignFromStack_onResponse_0);
            field = type.GetField("parameters", flag);
            app.RegisterCLRFieldGetter(field, get_parameters_1);
            app.RegisterCLRFieldSetter(field, set_parameters_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_parameters_1, AssignFromStack_parameters_1);


        }


        static StackObject* SendRequest_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIServerRequest instance_of_this_method = (global::UIServerRequest)typeof(global::UIServerRequest).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SendRequest();

            return __ret;
        }


        static object get_onResponse_0(ref object o)
        {
            return ((global::UIServerRequest)o).onResponse;
        }

        static StackObject* CopyToStack_onResponse_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIServerRequest)o).onResponse;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onResponse_0(ref object o, object v)
        {
            ((global::UIServerRequest)o).onResponse = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onResponse_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onResponse = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIServerRequest)o).onResponse = @onResponse;
            return ptr_of_this_method;
        }

        static object get_parameters_1(ref object o)
        {
            return ((global::UIServerRequest)o).parameters;
        }

        static StackObject* CopyToStack_parameters_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIServerRequest)o).parameters;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_parameters_1(ref object o, object v)
        {
            ((global::UIServerRequest)o).parameters = (global::UIServerRequest.ServerParameter[])v;
        }

        static StackObject* AssignFromStack_parameters_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIServerRequest.ServerParameter[] @parameters = (global::UIServerRequest.ServerParameter[])typeof(global::UIServerRequest.ServerParameter[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIServerRequest)o).parameters = @parameters;
            return ptr_of_this_method;
        }



    }
}
