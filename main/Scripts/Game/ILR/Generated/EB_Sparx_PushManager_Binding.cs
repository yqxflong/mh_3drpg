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
    unsafe class EB_Sparx_PushManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.PushManager);
            args = new Type[]{typeof(System.String), typeof(System.String), typeof(System.DateTime)};
            method = type.GetMethod("ScheduleOnceLocalNotification", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ScheduleOnceLocalNotification_0);

            field = type.GetField("OnScheduleLocalNotification", flag);
            app.RegisterCLRFieldGetter(field, get_OnScheduleLocalNotification_0);
            app.RegisterCLRFieldSetter(field, set_OnScheduleLocalNotification_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnScheduleLocalNotification_0, AssignFromStack_OnScheduleLocalNotification_0);
            field = type.GetField("OnHandleMessage", flag);
            app.RegisterCLRFieldGetter(field, get_OnHandleMessage_1);
            app.RegisterCLRFieldSetter(field, set_OnHandleMessage_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnHandleMessage_1, AssignFromStack_OnHandleMessage_1);
            field = type.GetField("OnDisconnected", flag);
            app.RegisterCLRFieldGetter(field, get_OnDisconnected_2);
            app.RegisterCLRFieldSetter(field, set_OnDisconnected_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnDisconnected_2, AssignFromStack_OnDisconnected_2);
            field = type.GetField("OnConnected", flag);
            app.RegisterCLRFieldGetter(field, get_OnConnected_3);
            app.RegisterCLRFieldSetter(field, set_OnConnected_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnConnected_3, AssignFromStack_OnConnected_3);


        }


        static StackObject* ScheduleOnceLocalNotification_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.DateTime @date = (System.DateTime)typeof(System.DateTime).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @message = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @title = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            EB.Sparx.PushManager instance_of_this_method = (EB.Sparx.PushManager)typeof(EB.Sparx.PushManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ScheduleOnceLocalNotification(@title, @message, @date);

            return __ret;
        }


        static object get_OnScheduleLocalNotification_0(ref object o)
        {
            return ((EB.Sparx.PushManager)o).OnScheduleLocalNotification;
        }

        static StackObject* CopyToStack_OnScheduleLocalNotification_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.PushManager)o).OnScheduleLocalNotification;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnScheduleLocalNotification_0(ref object o, object v)
        {
            ((EB.Sparx.PushManager)o).OnScheduleLocalNotification = (System.Action)v;
        }

        static StackObject* AssignFromStack_OnScheduleLocalNotification_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @OnScheduleLocalNotification = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.PushManager)o).OnScheduleLocalNotification = @OnScheduleLocalNotification;
            return ptr_of_this_method;
        }

        static object get_OnHandleMessage_1(ref object o)
        {
            return ((EB.Sparx.PushManager)o).OnHandleMessage;
        }

        static StackObject* CopyToStack_OnHandleMessage_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.PushManager)o).OnHandleMessage;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnHandleMessage_1(ref object o, object v)
        {
            ((EB.Sparx.PushManager)o).OnHandleMessage = (System.Action<System.String, System.String, System.Object>)v;
        }

        static StackObject* AssignFromStack_OnHandleMessage_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.String, System.String, System.Object> @OnHandleMessage = (System.Action<System.String, System.String, System.Object>)typeof(System.Action<System.String, System.String, System.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.PushManager)o).OnHandleMessage = @OnHandleMessage;
            return ptr_of_this_method;
        }

        static object get_OnDisconnected_2(ref object o)
        {
            return ((EB.Sparx.PushManager)o).OnDisconnected;
        }

        static StackObject* CopyToStack_OnDisconnected_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.PushManager)o).OnDisconnected;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnDisconnected_2(ref object o, object v)
        {
            ((EB.Sparx.PushManager)o).OnDisconnected = (System.Action)v;
        }

        static StackObject* AssignFromStack_OnDisconnected_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @OnDisconnected = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.PushManager)o).OnDisconnected = @OnDisconnected;
            return ptr_of_this_method;
        }

        static object get_OnConnected_3(ref object o)
        {
            return ((EB.Sparx.PushManager)o).OnConnected;
        }

        static StackObject* CopyToStack_OnConnected_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.PushManager)o).OnConnected;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnConnected_3(ref object o, object v)
        {
            ((EB.Sparx.PushManager)o).OnConnected = (System.Action)v;
        }

        static StackObject* AssignFromStack_OnConnected_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @OnConnected = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.PushManager)o).OnConnected = @OnConnected;
            return ptr_of_this_method;
        }



    }
}
