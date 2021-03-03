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
    unsafe class EB_Sparx_ChatManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.ChatManager);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("HandleSystemMessage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HandleSystemMessage_0);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("HandlePublicMessage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HandlePublicMessage_1);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("HandleAllianceMessage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HandleAllianceMessage_2);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("HandleTeamMessage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HandleTeamMessage_3);
            args = new Type[]{};
            method = type.GetMethod("get_Messages", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Messages_4);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("IsJoined", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsJoined_5);
            args = new Type[]{typeof(System.String), typeof(System.Collections.Hashtable), typeof(System.Action<System.String, System.Object>)};
            method = type.GetMethod("Join", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Join_6);
            args = new Type[]{typeof(System.String), typeof(System.Collections.Hashtable), typeof(System.Action<System.String, System.Object>)};
            method = type.GetMethod("Leave", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Leave_7);
            args = new Type[]{typeof(System.String), typeof(System.String), typeof(System.Collections.Hashtable), typeof(System.Action<System.String, System.Object>)};
            method = type.GetMethod("SendText", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SendText_8);
            args = new Type[]{typeof(System.String), typeof(UnityEngine.AudioClip), typeof(System.Int32), typeof(System.Collections.Hashtable), typeof(System.Action<System.String, System.Object>)};
            method = type.GetMethod("SendAudio", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SendAudio_9);
            args = new Type[]{typeof(System.String), typeof(System.Collections.Hashtable), typeof(System.Action<System.String, System.Object>)};
            method = type.GetMethod("History", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, History_10);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("GetLastSendTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetLastSendTime_11);

            field = type.GetField("OnConnected", flag);
            app.RegisterCLRFieldGetter(field, get_OnConnected_0);
            app.RegisterCLRFieldSetter(field, set_OnConnected_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnConnected_0, AssignFromStack_OnConnected_0);
            field = type.GetField("OnMessages", flag);
            app.RegisterCLRFieldGetter(field, get_OnMessages_1);
            app.RegisterCLRFieldSetter(field, set_OnMessages_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnMessages_1, AssignFromStack_OnMessages_1);
            field = type.GetField("OnDisconnected", flag);
            app.RegisterCLRFieldGetter(field, get_OnDisconnected_2);
            app.RegisterCLRFieldSetter(field, set_OnDisconnected_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnDisconnected_2, AssignFromStack_OnDisconnected_2);


        }


        static StackObject* HandleSystemMessage_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.HandleSystemMessage(@text);

            return __ret;
        }

        static StackObject* HandlePublicMessage_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.HandlePublicMessage(@text);

            return __ret;
        }

        static StackObject* HandleAllianceMessage_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.HandleAllianceMessage(@text);

            return __ret;
        }

        static StackObject* HandleTeamMessage_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.HandleTeamMessage(@text);

            return __ret;
        }

        static StackObject* get_Messages_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Messages;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* IsJoined_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @channel = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsJoined(@channel);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* Join_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.String, System.Object> @callback = (System.Action<System.String, System.Object>)typeof(System.Action<System.String, System.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Hashtable @options = (System.Collections.Hashtable)typeof(System.Collections.Hashtable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @channel = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Join(@channel, @options, @callback);

            return __ret;
        }

        static StackObject* Leave_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.String, System.Object> @callback = (System.Action<System.String, System.Object>)typeof(System.Action<System.String, System.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Hashtable @options = (System.Collections.Hashtable)typeof(System.Collections.Hashtable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @channel = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Leave(@channel, @options, @callback);

            return __ret;
        }

        static StackObject* SendText_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.String, System.Object> @callback = (System.Action<System.String, System.Object>)typeof(System.Action<System.String, System.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Hashtable @attributes = (System.Collections.Hashtable)typeof(System.Collections.Hashtable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.String @channel = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SendText(@channel, @text, @attributes, @callback);

            return __ret;
        }

        static StackObject* SendAudio_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 6);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.String, System.Object> @callback = (System.Action<System.String, System.Object>)typeof(System.Action<System.String, System.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Hashtable @attributes = (System.Collections.Hashtable)typeof(System.Collections.Hashtable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @samplePos = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            UnityEngine.AudioClip @audio = (UnityEngine.AudioClip)typeof(UnityEngine.AudioClip).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.String @channel = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SendAudio(@channel, @audio, @samplePos, @attributes, @callback);

            return __ret;
        }

        static StackObject* History_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.String, System.Object> @callback = (System.Action<System.String, System.Object>)typeof(System.Action<System.String, System.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Hashtable @options = (System.Collections.Hashtable)typeof(System.Collections.Hashtable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @channel = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.History(@channel, @options, @callback);

            return __ret;
        }

        static StackObject* GetLastSendTime_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @channel = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            EB.Sparx.ChatManager instance_of_this_method = (EB.Sparx.ChatManager)typeof(EB.Sparx.ChatManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetLastSendTime(@channel);

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }


        static object get_OnConnected_0(ref object o)
        {
            return ((EB.Sparx.ChatManager)o).OnConnected;
        }

        static StackObject* CopyToStack_OnConnected_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatManager)o).OnConnected;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnConnected_0(ref object o, object v)
        {
            ((EB.Sparx.ChatManager)o).OnConnected = (System.Action)v;
        }

        static StackObject* AssignFromStack_OnConnected_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @OnConnected = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatManager)o).OnConnected = @OnConnected;
            return ptr_of_this_method;
        }

        static object get_OnMessages_1(ref object o)
        {
            return ((EB.Sparx.ChatManager)o).OnMessages;
        }

        static StackObject* CopyToStack_OnMessages_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatManager)o).OnMessages;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnMessages_1(ref object o, object v)
        {
            ((EB.Sparx.ChatManager)o).OnMessages = (System.Action<EB.Sparx.ChatMessage[]>)v;
        }

        static StackObject* AssignFromStack_OnMessages_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<EB.Sparx.ChatMessage[]> @OnMessages = (System.Action<EB.Sparx.ChatMessage[]>)typeof(System.Action<EB.Sparx.ChatMessage[]>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatManager)o).OnMessages = @OnMessages;
            return ptr_of_this_method;
        }

        static object get_OnDisconnected_2(ref object o)
        {
            return ((EB.Sparx.ChatManager)o).OnDisconnected;
        }

        static StackObject* CopyToStack_OnDisconnected_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.ChatManager)o).OnDisconnected;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnDisconnected_2(ref object o, object v)
        {
            ((EB.Sparx.ChatManager)o).OnDisconnected = (System.Action)v;
        }

        static StackObject* AssignFromStack_OnDisconnected_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @OnDisconnected = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.ChatManager)o).OnDisconnected = @OnDisconnected;
            return ptr_of_this_method;
        }



    }
}
