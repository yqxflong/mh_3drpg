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
    unsafe class AIHelp_AIHelpManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(AIHelp.AIHelpManager);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("setUnreadMessageFetchUid", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, setUnreadMessageFetchUid_0);
            args = new Type[]{typeof(System.String), typeof(System.String), typeof(System.String)};
            method = type.GetMethod("showConversation", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, showConversation_1);
            args = new Type[]{};
            method = type.GetMethod("showFAQSection", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, showFAQSection_2);

            field = type.GetField("IshaveUnreadMessage", flag);
            app.RegisterCLRFieldGetter(field, get_IshaveUnreadMessage_0);
            app.RegisterCLRFieldSetter(field, set_IshaveUnreadMessage_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_IshaveUnreadMessage_0, AssignFromStack_IshaveUnreadMessage_0);


        }


        static StackObject* setUnreadMessageFetchUid_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @playerUid = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            AIHelp.AIHelpManager.setUnreadMessageFetchUid(@playerUid);

            return __ret;
        }

        static StackObject* showConversation_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @serverId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @uId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @Username = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            AIHelp.AIHelpManager.showConversation(@Username, @uId, @serverId);

            return __ret;
        }

        static StackObject* showFAQSection_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            AIHelp.AIHelpManager.showFAQSection();

            return __ret;
        }


        static object get_IshaveUnreadMessage_0(ref object o)
        {
            return AIHelp.AIHelpManager.IshaveUnreadMessage;
        }

        static StackObject* CopyToStack_IshaveUnreadMessage_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = AIHelp.AIHelpManager.IshaveUnreadMessage;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_IshaveUnreadMessage_0(ref object o, object v)
        {
            AIHelp.AIHelpManager.IshaveUnreadMessage = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_IshaveUnreadMessage_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @IshaveUnreadMessage = ptr_of_this_method->Value == 1;
            AIHelp.AIHelpManager.IshaveUnreadMessage = @IshaveUnreadMessage;
            return ptr_of_this_method;
        }



    }
}
