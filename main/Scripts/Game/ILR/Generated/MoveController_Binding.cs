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
    unsafe class MoveController_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::MoveController);
            args = new Type[]{};
            method = type.GetMethod("get_CurrentMove", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_CurrentMove_0);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("SetMove", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetMove_1);
            args = new Type[]{};
            method = type.GetMethod("get_CurrentState", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_CurrentState_2);
            args = new Type[]{typeof(global::MoveController.CombatantMoveState)};
            method = type.GetMethod("TransitionTo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, TransitionTo_3);
            args = new Type[]{};
            method = type.GetMethod("GetCurrentStateInfo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCurrentStateInfo_4);
            args = new Type[]{typeof(global::MoveController.CombatantMoveState)};
            method = type.GetMethod("GetMoveByState", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetMoveByState_5);
            args = new Type[]{typeof(MoveEditor.Move)};
            method = type.GetMethod("SetMove", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetMove_6);
            args = new Type[]{};
            method = type.GetMethod("GetCurrentAnimHash", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCurrentAnimHash_7);
            args = new Type[]{typeof(System.Int32), typeof(System.Single), typeof(System.Int32), typeof(System.Single)};
            method = type.GetMethod("CrossFade", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CrossFade_8);
            args = new Type[]{};
            method = type.GetMethod("get_IsInitialized", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsInitialized_9);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("RegisterInitSuccCallBack", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterInitSuccCallBack_10);
            args = new Type[]{typeof(global::MoveController.CombatantMoveState)};
            method = type.GetMethod("set_CurrentState", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_CurrentState_11);
            args = new Type[]{typeof(System.String), typeof(System.Boolean)};
            method = type.GetMethod("GetMoveIfExists", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetMoveIfExists_12);
            args = new Type[]{};
            method = type.GetMethod("InitAnimator", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InitAnimator_13);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("InTransition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InTransition_14);
            args = new Type[]{typeof(global::MoveController.CombatantMoveState)};
            method = type.GetMethod("GetMovesByState", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetMovesByState_15);

            field = type.GetField("m_lobby_hash", flag);
            app.RegisterCLRFieldGetter(field, get_m_lobby_hash_0);
            app.RegisterCLRFieldSetter(field, set_m_lobby_hash_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_lobby_hash_0, AssignFromStack_m_lobby_hash_0);
            field = type.GetField("m_idle_hash", flag);
            app.RegisterCLRFieldGetter(field, get_m_idle_hash_1);
            app.RegisterCLRFieldSetter(field, set_m_idle_hash_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_idle_hash_1, AssignFromStack_m_idle_hash_1);
            field = type.GetField("m_entry_hash", flag);
            app.RegisterCLRFieldGetter(field, get_m_entry_hash_2);
            app.RegisterCLRFieldSetter(field, set_m_entry_hash_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_entry_hash_2, AssignFromStack_m_entry_hash_2);
            field = type.GetField("m_ready_hash", flag);
            app.RegisterCLRFieldGetter(field, get_m_ready_hash_3);
            app.RegisterCLRFieldSetter(field, set_m_ready_hash_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_ready_hash_3, AssignFromStack_m_ready_hash_3);


        }


        static StackObject* get_CurrentMove_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CurrentMove;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetMove_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @move = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetMove(@move);

            return __ret;
        }

        static StackObject* get_CurrentState_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CurrentState;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* TransitionTo_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MoveController.CombatantMoveState @state = (global::MoveController.CombatantMoveState)typeof(global::MoveController.CombatantMoveState).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.TransitionTo(@state);

            return __ret;
        }

        static StackObject* GetCurrentStateInfo_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetCurrentStateInfo();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetMoveByState_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MoveController.CombatantMoveState @state = (global::MoveController.CombatantMoveState)typeof(global::MoveController.CombatantMoveState).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetMoveByState(@state);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetMove_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            MoveEditor.Move @theMove = (MoveEditor.Move)typeof(MoveEditor.Move).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetMove(@theMove);

            return __ret;
        }

        static StackObject* GetCurrentAnimHash_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetCurrentAnimHash();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* CrossFade_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @normalizedTime = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @layer = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @normalizedBlendTime = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Int32 @crossFadeHash = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CrossFade(@crossFadeHash, @normalizedBlendTime, @layer, @normalizedTime);

            return __ret;
        }

        static StackObject* get_IsInitialized_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsInitialized;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* RegisterInitSuccCallBack_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @fn = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RegisterInitSuccCallBack(@fn);

            return __ret;
        }

        static StackObject* set_CurrentState_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MoveController.CombatantMoveState @value = (global::MoveController.CombatantMoveState)typeof(global::MoveController.CombatantMoveState).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CurrentState = value;

            return __ret;
        }

        static StackObject* GetMoveIfExists_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isNew = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @move = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetMoveIfExists(@move, @isNew);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* InitAnimator_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.InitAnimator();

            return __ret;
        }

        static StackObject* InTransition_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @layer = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.InTransition(@layer);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* GetMovesByState_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MoveController.CombatantMoveState @state = (global::MoveController.CombatantMoveState)typeof(global::MoveController.CombatantMoveState).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::MoveController instance_of_this_method = (global::MoveController)typeof(global::MoveController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetMovesByState(@state);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_m_lobby_hash_0(ref object o)
        {
            return ((global::MoveController)o).m_lobby_hash;
        }

        static StackObject* CopyToStack_m_lobby_hash_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::MoveController)o).m_lobby_hash;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m_lobby_hash_0(ref object o, object v)
        {
            ((global::MoveController)o).m_lobby_hash = (System.Int32)v;
        }

        static StackObject* AssignFromStack_m_lobby_hash_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @m_lobby_hash = ptr_of_this_method->Value;
            ((global::MoveController)o).m_lobby_hash = @m_lobby_hash;
            return ptr_of_this_method;
        }

        static object get_m_idle_hash_1(ref object o)
        {
            return global::MoveController.m_idle_hash;
        }

        static StackObject* CopyToStack_m_idle_hash_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::MoveController.m_idle_hash;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m_idle_hash_1(ref object o, object v)
        {
            global::MoveController.m_idle_hash = (System.Int32)v;
        }

        static StackObject* AssignFromStack_m_idle_hash_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @m_idle_hash = ptr_of_this_method->Value;
            global::MoveController.m_idle_hash = @m_idle_hash;
            return ptr_of_this_method;
        }

        static object get_m_entry_hash_2(ref object o)
        {
            return global::MoveController.m_entry_hash;
        }

        static StackObject* CopyToStack_m_entry_hash_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::MoveController.m_entry_hash;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m_entry_hash_2(ref object o, object v)
        {
            global::MoveController.m_entry_hash = (System.Int32)v;
        }

        static StackObject* AssignFromStack_m_entry_hash_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @m_entry_hash = ptr_of_this_method->Value;
            global::MoveController.m_entry_hash = @m_entry_hash;
            return ptr_of_this_method;
        }

        static object get_m_ready_hash_3(ref object o)
        {
            return global::MoveController.m_ready_hash;
        }

        static StackObject* CopyToStack_m_ready_hash_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::MoveController.m_ready_hash;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m_ready_hash_3(ref object o, object v)
        {
            global::MoveController.m_ready_hash = (System.Int32)v;
        }

        static StackObject* AssignFromStack_m_ready_hash_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @m_ready_hash = ptr_of_this_method->Value;
            global::MoveController.m_ready_hash = @m_ready_hash;
            return ptr_of_this_method;
        }



    }
}
