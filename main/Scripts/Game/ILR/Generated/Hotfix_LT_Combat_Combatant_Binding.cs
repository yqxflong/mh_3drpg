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
    unsafe class Hotfix_LT_Combat_Combatant_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Hotfix_LT.Combat.Combatant);
            args = new Type[]{};
            method = type.GetMethod("get_Data", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Data_0);
            args = new Type[]{};
            method = type.GetMethod("get_damageTextOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_damageTextOffset_1);
            args = new Type[]{};
            method = type.GetMethod("get_healTextOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_healTextOffset_2);
            args = new Type[]{};
            method = type.GetMethod("get_floatBuffFontTextOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_floatBuffFontTextOffset_3);
            args = new Type[]{};
            method = type.GetMethod("GetHP", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetHP_4);
            args = new Type[]{};
            method = type.GetMethod("HideRestrainFlag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HideRestrainFlag_5);
            args = new Type[]{};
            method = type.GetMethod("get_Index", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Index_6);
            args = new Type[]{};
            method = type.GetMethod("IsDead", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsDead_7);
            args = new Type[]{};
            method = type.GetMethod("CanSelect", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CanSelect_8);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetRestrainFlag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetRestrainFlag_9);
            args = new Type[]{};
            method = type.GetMethod("SetGainFlag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetGainFlag_10);
            args = new Type[]{};
            method = type.GetMethod("get_myName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_myName_11);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("PlaySelectedFX", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlaySelectedFX_12);
            args = new Type[]{};
            method = type.GetMethod("get_Collider", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Collider_13);
            args = new Type[]{};
            method = type.GetMethod("get_OriginPosition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_OriginPosition_14);
            args = new Type[]{};
            method = type.GetMethod("IsLaunch", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsLaunch_15);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("RemoveSelectionFX", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RemoveSelectionFX_16);
            args = new Type[]{};
            method = type.GetMethod("SetupSelectFX", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetupSelectFX_17);
            args = new Type[]{};
            method = type.GetMethod("SetupSelectableFX", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetupSelectableFX_18);
            args = new Type[]{};
            method = type.GetMethod("GetMaxHP", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetMaxHP_19);
            args = new Type[]{};
            method = type.GetMethod("get_HealthBar", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_HealthBar_20);

            field = type.GetField("DamageTextTarget", flag);
            app.RegisterCLRFieldGetter(field, get_DamageTextTarget_0);
            app.RegisterCLRFieldSetter(field, set_DamageTextTarget_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_DamageTextTarget_0, AssignFromStack_DamageTextTarget_0);
            field = type.GetField("redRing", flag);
            app.RegisterCLRFieldGetter(field, get_redRing_1);
            app.RegisterCLRFieldSetter(field, set_redRing_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_redRing_1, AssignFromStack_redRing_1);
            field = type.GetField("orangeRing", flag);
            app.RegisterCLRFieldGetter(field, get_orangeRing_2);
            app.RegisterCLRFieldSetter(field, set_orangeRing_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_orangeRing_2, AssignFromStack_orangeRing_2);
            field = type.GetField("blackRing", flag);
            app.RegisterCLRFieldGetter(field, get_blackRing_3);
            app.RegisterCLRFieldSetter(field, set_blackRing_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_blackRing_3, AssignFromStack_blackRing_3);
            field = type.GetField("greenRing", flag);
            app.RegisterCLRFieldGetter(field, get_greenRing_4);
            app.RegisterCLRFieldSetter(field, set_greenRing_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_greenRing_4, AssignFromStack_greenRing_4);
            field = type.GetField("colorScale", flag);
            app.RegisterCLRFieldGetter(field, get_colorScale_5);
            app.RegisterCLRFieldSetter(field, set_colorScale_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_colorScale_5, AssignFromStack_colorScale_5);


        }


        static StackObject* get_Data_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Data;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_damageTextOffset_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.damageTextOffset;

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* get_healTextOffset_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.healTextOffset;

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* get_floatBuffFontTextOffset_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.floatBuffFontTextOffset;

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* GetHP_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetHP();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* HideRestrainFlag_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.HideRestrainFlag();

            return __ret;
        }

        static StackObject* get_Index_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Index;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* IsDead_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsDead();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* CanSelect_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CanSelect();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* SetRestrainFlag_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @attr = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetRestrainFlag(@attr);

            return __ret;
        }

        static StackObject* SetGainFlag_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetGainFlag();

            return __ret;
        }

        static StackObject* get_myName_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.myName;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* PlaySelectedFX_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @worldPos = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @worldPos, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @worldPos = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.PlaySelectedFX(@worldPos);

            return __ret;
        }

        static StackObject* get_Collider_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Collider;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_OriginPosition_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.OriginPosition;

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* IsLaunch_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsLaunch();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* RemoveSelectionFX_16(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @all = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveSelectionFX(@all);

            return __ret;
        }

        static StackObject* SetupSelectFX_17(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetupSelectFX();

            return __ret;
        }

        static StackObject* SetupSelectableFX_18(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetupSelectableFX();

            return __ret;
        }

        static StackObject* GetMaxHP_19(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetMaxHP();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_HealthBar_20(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.Combatant instance_of_this_method = (Hotfix_LT.Combat.Combatant)typeof(Hotfix_LT.Combat.Combatant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.HealthBar;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_DamageTextTarget_0(ref object o)
        {
            return ((Hotfix_LT.Combat.Combatant)o).DamageTextTarget;
        }

        static StackObject* CopyToStack_DamageTextTarget_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.Combatant)o).DamageTextTarget;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_DamageTextTarget_0(ref object o, object v)
        {
            ((Hotfix_LT.Combat.Combatant)o).DamageTextTarget = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_DamageTextTarget_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @DamageTextTarget = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.Combatant)o).DamageTextTarget = @DamageTextTarget;
            return ptr_of_this_method;
        }

        static object get_redRing_1(ref object o)
        {
            return ((Hotfix_LT.Combat.Combatant)o).redRing;
        }

        static StackObject* CopyToStack_redRing_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.Combatant)o).redRing;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_redRing_1(ref object o, object v)
        {
            ((Hotfix_LT.Combat.Combatant)o).redRing = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_redRing_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @redRing = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.Combatant)o).redRing = @redRing;
            return ptr_of_this_method;
        }

        static object get_orangeRing_2(ref object o)
        {
            return ((Hotfix_LT.Combat.Combatant)o).orangeRing;
        }

        static StackObject* CopyToStack_orangeRing_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.Combatant)o).orangeRing;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_orangeRing_2(ref object o, object v)
        {
            ((Hotfix_LT.Combat.Combatant)o).orangeRing = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_orangeRing_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @orangeRing = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.Combatant)o).orangeRing = @orangeRing;
            return ptr_of_this_method;
        }

        static object get_blackRing_3(ref object o)
        {
            return ((Hotfix_LT.Combat.Combatant)o).blackRing;
        }

        static StackObject* CopyToStack_blackRing_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.Combatant)o).blackRing;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_blackRing_3(ref object o, object v)
        {
            ((Hotfix_LT.Combat.Combatant)o).blackRing = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_blackRing_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @blackRing = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.Combatant)o).blackRing = @blackRing;
            return ptr_of_this_method;
        }

        static object get_greenRing_4(ref object o)
        {
            return ((Hotfix_LT.Combat.Combatant)o).greenRing;
        }

        static StackObject* CopyToStack_greenRing_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.Combatant)o).greenRing;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_greenRing_4(ref object o, object v)
        {
            ((Hotfix_LT.Combat.Combatant)o).greenRing = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_greenRing_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @greenRing = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.Combatant)o).greenRing = @greenRing;
            return ptr_of_this_method;
        }

        static object get_colorScale_5(ref object o)
        {
            return ((Hotfix_LT.Combat.Combatant)o).colorScale;
        }

        static StackObject* CopyToStack_colorScale_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.Combatant)o).colorScale;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_colorScale_5(ref object o, object v)
        {
            ((Hotfix_LT.Combat.Combatant)o).colorScale = (global::CharacterColorScale)v;
        }

        static StackObject* AssignFromStack_colorScale_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::CharacterColorScale @colorScale = (global::CharacterColorScale)typeof(global::CharacterColorScale).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.Combatant)o).colorScale = @colorScale;
            return ptr_of_this_method;
        }



    }
}
