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
    unsafe class Hotfix_LT_UI_LTCombatEventReceiver_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Hotfix_LT.UI.LTCombatEventReceiver);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);
            args = new Type[]{};
            method = type.GetMethod("get_Ready", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Ready_1);
            args = new Type[]{};
            method = type.GetMethod("OnBoss_ParticleInSceneComplete", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnBoss_ParticleInSceneComplete_2);
            args = new Type[]{};
            method = type.GetMethod("OnExitCombat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnExitCombat_3);
            args = new Type[]{};
            method = type.GetMethod("OnCombatViewLoaded", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnCombatViewLoaded_4);
            args = new Type[]{};
            method = type.GetMethod("IsAnimStateIdle", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsAnimStateIdle_5);
            args = new Type[]{typeof(System.Object)};
            method = type.GetMethod("OnCombatEventListReceived", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnCombatEventListReceived_6);
            args = new Type[]{};
            method = type.GetMethod("get_IsBattleOver", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsBattleOver_7);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_Ready", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Ready_8);
            args = new Type[]{typeof(Hotfix_LT.UI.LTCombatEventReceiver.CombatantCallbackVoid)};
            method = type.GetMethod("ForEach", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ForEach_9);
            args = new Type[]{typeof(System.Int32), typeof(System.Int32)};
            method = type.GetMethod("GetCombatant", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCombatant_10);
            args = new Type[]{};
            method = type.GetMethod("IsCombatInit", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsCombatInit_11);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("GetCombatantByIngameId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCombatantByIngameId_12);
            args = new Type[]{};
            method = type.GetMethod("get_TimeScale", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TimeScale_13);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_TimeScale", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_TimeScale_14);
            args = new Type[]{};
            method = type.GetMethod("CancelBtnClick", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CancelBtnClick_15);
            args = new Type[]{};
            method = type.GetMethod("DoReadyActionOver", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DoReadyActionOver_16);
            args = new Type[]{};
            method = type.GetMethod("OnBattleResultScreen", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnBattleResultScreen_17);
            args = new Type[]{};
            method = type.GetMethod("get_Conversationing", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Conversationing_18);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("SetWinningTeam", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetWinningTeam_19);
            args = new Type[]{};
            method = type.GetMethod("PlayVictoryDance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlayVictoryDance_20);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("StoryOver", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StoryOver_21);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Hotfix_LT.UI.LTCombatEventReceiver.Instance;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Ready_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Ready;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* OnBoss_ParticleInSceneComplete_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnBoss_ParticleInSceneComplete();

            return __ret;
        }

        static StackObject* OnExitCombat_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnExitCombat();

            return __ret;
        }

        static StackObject* OnCombatViewLoaded_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnCombatViewLoaded();

            return __ret;
        }

        static StackObject* IsAnimStateIdle_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsAnimStateIdle();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* OnCombatEventListReceived_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @event_obj = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnCombatEventListReceived(@event_obj);

            return __ret;
        }

        static StackObject* get_IsBattleOver_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsBattleOver;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* set_Ready_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Ready = value;

            return __ret;
        }

        static StackObject* ForEach_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver.CombatantCallbackVoid @callback = (Hotfix_LT.UI.LTCombatEventReceiver.CombatantCallbackVoid)typeof(Hotfix_LT.UI.LTCombatEventReceiver.CombatantCallbackVoid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ForEach(@callback);

            return __ret;
        }

        static StackObject* GetCombatant_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index_on_team = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @team_index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetCombatant(@team_index, @index_on_team);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* IsCombatInit_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Hotfix_LT.UI.LTCombatEventReceiver.IsCombatInit();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* GetCombatantByIngameId_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @idx = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetCombatantByIngameId(@idx);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_TimeScale_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TimeScale;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_TimeScale_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.TimeScale = value;

            return __ret;
        }

        static StackObject* CancelBtnClick_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CancelBtnClick();

            return __ret;
        }

        static StackObject* DoReadyActionOver_16(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DoReadyActionOver();

            return __ret;
        }

        static StackObject* OnBattleResultScreen_17(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnBattleResultScreen();

            return __ret;
        }

        static StackObject* get_Conversationing_18(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Conversationing;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* SetWinningTeam_19(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isVictory = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetWinningTeam(@isVictory);

            return __ret;
        }

        static StackObject* PlayVictoryDance_20(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.PlayVictoryDance();

            return __ret;
        }

        static StackObject* StoryOver_21(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @pausedMusic = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.UI.LTCombatEventReceiver instance_of_this_method = (Hotfix_LT.UI.LTCombatEventReceiver)typeof(Hotfix_LT.UI.LTCombatEventReceiver).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StoryOver(@pausedMusic);

            return __ret;
        }



    }
}
