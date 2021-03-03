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
    unsafe class EB_Sparx_ResourcesManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.ResourcesManager);
            args = new Type[]{typeof(System.String), typeof(System.Int32)};
            method = type.GetMethod("SetResRPC", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetResRPC_0);
            args = new Type[]{typeof(System.Int32), typeof(System.Action<EB.Sparx.Response>)};
            method = type.GetMethod("AddTicket", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddTicket_1);

            field = type.GetField("OnResourcesUpdateListener", flag);
            app.RegisterCLRFieldGetter(field, get_OnResourcesUpdateListener_0);
            app.RegisterCLRFieldSetter(field, set_OnResourcesUpdateListener_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnResourcesUpdateListener_0, AssignFromStack_OnResourcesUpdateListener_0);


        }


        static StackObject* SetResRPC_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @num = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @type = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            EB.Sparx.ResourcesManager instance_of_this_method = (EB.Sparx.ResourcesManager)typeof(EB.Sparx.ResourcesManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetResRPC(@type, @num);

            return __ret;
        }

        static StackObject* AddTicket_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<EB.Sparx.Response> @callback = (System.Action<EB.Sparx.Response>)typeof(System.Action<EB.Sparx.Response>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @ticket = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            EB.Sparx.ResourcesManager instance_of_this_method = (EB.Sparx.ResourcesManager)typeof(EB.Sparx.ResourcesManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddTicket(@ticket, @callback);

            return __ret;
        }


        static object get_OnResourcesUpdateListener_0(ref object o)
        {
            return ((EB.Sparx.ResourcesManager)o).OnResourcesUpdateListener;
        }

        static StackObject* CopyToStack_OnResourcesUpdateListener_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ResourcesManager)o).OnResourcesUpdateListener;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnResourcesUpdateListener_0(ref object o, object v)
        {
            ((EB.Sparx.ResourcesManager)o).OnResourcesUpdateListener = (System.Action<System.Object>)v;
        }

        static StackObject* AssignFromStack_OnResourcesUpdateListener_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.Object> @OnResourcesUpdateListener = (System.Action<System.Object>)typeof(System.Action<System.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ResourcesManager)o).OnResourcesUpdateListener = @OnResourcesUpdateListener;
            return ptr_of_this_method;
        }



    }
}
