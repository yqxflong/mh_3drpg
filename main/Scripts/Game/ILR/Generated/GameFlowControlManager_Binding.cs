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
    unsafe class GameFlowControlManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::GameFlowControlManager);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("SendEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SendEvent_1);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("IsInView", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsInView_2);

            field = type.GetField("m_PrevStateName", flag);
            app.RegisterCLRFieldGetter(field, get_m_PrevStateName_0);
            app.RegisterCLRFieldSetter(field, set_m_PrevStateName_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_PrevStateName_0, AssignFromStack_m_PrevStateName_0);
            field = type.GetField("m_StateMachine", flag);
            app.RegisterCLRFieldGetter(field, get_m_StateMachine_1);
            app.RegisterCLRFieldSetter(field, set_m_StateMachine_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_StateMachine_1, AssignFromStack_m_StateMachine_1);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::GameFlowControlManager.Instance;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SendEvent_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @eventName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::GameFlowControlManager instance_of_this_method = (global::GameFlowControlManager)typeof(global::GameFlowControlManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SendEvent(@eventName);

            return __ret;
        }

        static StackObject* IsInView_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @viewName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::GameFlowControlManager.IsInView(@viewName);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_m_PrevStateName_0(ref object o)
        {
            return ((global::GameFlowControlManager)o).m_PrevStateName;
        }

        static StackObject* CopyToStack_m_PrevStateName_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::GameFlowControlManager)o).m_PrevStateName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_PrevStateName_0(ref object o, object v)
        {
            ((global::GameFlowControlManager)o).m_PrevStateName = (System.String)v;
        }

        static StackObject* AssignFromStack_m_PrevStateName_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @m_PrevStateName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::GameFlowControlManager)o).m_PrevStateName = @m_PrevStateName;
            return ptr_of_this_method;
        }

        static object get_m_StateMachine_1(ref object o)
        {
            return ((global::GameFlowControlManager)o).m_StateMachine;
        }

        static StackObject* CopyToStack_m_StateMachine_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::GameFlowControlManager)o).m_StateMachine;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_StateMachine_1(ref object o, object v)
        {
            ((global::GameFlowControlManager)o).m_StateMachine = (global::PlayMakerFSM)v;
        }

        static StackObject* AssignFromStack_m_StateMachine_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::PlayMakerFSM @m_StateMachine = (global::PlayMakerFSM)typeof(global::PlayMakerFSM).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::GameFlowControlManager)o).m_StateMachine = @m_StateMachine;
            return ptr_of_this_method;
        }



    }
}
