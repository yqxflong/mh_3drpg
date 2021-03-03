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
    unsafe class Hotfix_LT_Combat_CombatCharacterSyncData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Hotfix_LT.Combat.CombatCharacterSyncData);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("GetCanUseSkill", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCanUseSkill_0);
            args = new Type[]{};
            method = type.GetMethod("get_IndexOnTeam", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IndexOnTeam_1);
            args = new Type[]{typeof(Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)};
            method = type.GetMethod("IsSkillCanUse", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsSkillCanUse_2);
            args = new Type[]{};
            method = type.GetMethod("get_IsBoss", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsBoss_3);

            field = type.GetField("Hp", flag);
            app.RegisterCLRFieldGetter(field, get_Hp_0);
            app.RegisterCLRFieldSetter(field, set_Hp_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Hp_0, AssignFromStack_Hp_0);
            field = type.GetField("Index", flag);
            app.RegisterCLRFieldGetter(field, get_Index_1);
            app.RegisterCLRFieldSetter(field, set_Index_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Index_1, AssignFromStack_Index_1);
            field = type.GetField("ID", flag);
            app.RegisterCLRFieldGetter(field, get_ID_2);
            app.RegisterCLRFieldSetter(field, set_ID_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_ID_2, AssignFromStack_ID_2);
            field = type.GetField("IngameId", flag);
            app.RegisterCLRFieldGetter(field, get_IngameId_3);
            app.RegisterCLRFieldSetter(field, set_IngameId_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_IngameId_3, AssignFromStack_IngameId_3);
            field = type.GetField("SkillDataList", flag);
            app.RegisterCLRFieldGetter(field, get_SkillDataList_4);
            app.RegisterCLRFieldSetter(field, set_SkillDataList_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_SkillDataList_4, AssignFromStack_SkillDataList_4);
            field = type.GetField("Uid", flag);
            app.RegisterCLRFieldGetter(field, get_Uid_5);
            app.RegisterCLRFieldSetter(field, set_Uid_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_Uid_5, AssignFromStack_Uid_5);
            field = type.GetField("ScrollTimes", flag);
            app.RegisterCLRFieldGetter(field, get_ScrollTimes_6);
            app.RegisterCLRFieldSetter(field, set_ScrollTimes_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_ScrollTimes_6, AssignFromStack_ScrollTimes_6);
            field = type.GetField("NormalSkillData", flag);
            app.RegisterCLRFieldGetter(field, get_NormalSkillData_7);
            app.RegisterCLRFieldSetter(field, set_NormalSkillData_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_NormalSkillData_7, AssignFromStack_NormalSkillData_7);
            field = type.GetField("Attr", flag);
            app.RegisterCLRFieldGetter(field, get_Attr_8);
            app.RegisterCLRFieldSetter(field, set_Attr_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_Attr_8, AssignFromStack_Attr_8);
            field = type.GetField("Limits", flag);
            app.RegisterCLRFieldGetter(field, get_Limits_9);
            app.RegisterCLRFieldSetter(field, set_Limits_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_Limits_9, AssignFromStack_Limits_9);
            field = type.GetField("TplId", flag);
            app.RegisterCLRFieldGetter(field, get_TplId_10);
            app.RegisterCLRFieldSetter(field, set_TplId_10);
            app.RegisterCLRFieldBinding(field, CopyToStack_TplId_10, AssignFromStack_TplId_10);
            field = type.GetField("MaxHp", flag);
            app.RegisterCLRFieldGetter(field, get_MaxHp_11);
            app.RegisterCLRFieldSetter(field, set_MaxHp_11);
            app.RegisterCLRFieldBinding(field, CopyToStack_MaxHp_11, AssignFromStack_MaxHp_11);
            field = type.GetField("Skin", flag);
            app.RegisterCLRFieldGetter(field, get_Skin_12);
            app.RegisterCLRFieldSetter(field, set_Skin_12);
            app.RegisterCLRFieldBinding(field, CopyToStack_Skin_12, AssignFromStack_Skin_12);
            field = type.GetField("TeamId", flag);
            app.RegisterCLRFieldGetter(field, get_TeamId_13);
            app.RegisterCLRFieldSetter(field, set_TeamId_13);
            app.RegisterCLRFieldBinding(field, CopyToStack_TeamId_13, AssignFromStack_TeamId_13);
            field = type.GetField("Portrait", flag);
            app.RegisterCLRFieldGetter(field, get_Portrait_14);
            app.RegisterCLRFieldSetter(field, set_Portrait_14);
            app.RegisterCLRFieldBinding(field, CopyToStack_Portrait_14, AssignFromStack_Portrait_14);
            field = type.GetField("Default", flag);
            app.RegisterCLRFieldGetter(field, get_Default_15);
            app.RegisterCLRFieldSetter(field, set_Default_15);
            app.RegisterCLRFieldBinding(field, CopyToStack_Default_15, AssignFromStack_Default_15);


        }


        static StackObject* GetCanUseSkill_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @defaultNormal = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.Combat.CombatCharacterSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatCharacterSyncData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetCanUseSkill(@defaultNormal);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_IndexOnTeam_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatCharacterSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatCharacterSyncData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IndexOnTeam;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* IsSkillCanUse_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatCharacterSyncData.SkillData @skilldata = (Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData.SkillData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.Combat.CombatCharacterSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatCharacterSyncData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsSkillCanUse(@skilldata);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_IsBoss_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatCharacterSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatCharacterSyncData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsBoss;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_Hp_0(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Hp;
        }

        static StackObject* CopyToStack_Hp_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Hp;
            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_Hp_0(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Hp = (System.Int64)v;
        }

        static StackObject* AssignFromStack_Hp_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int64 @Hp = *(long*)&ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Hp = @Hp;
            return ptr_of_this_method;
        }

        static object get_Index_1(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Index;
        }

        static StackObject* CopyToStack_Index_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Index;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Index_1(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Index = (Hotfix_LT.Combat.CombatantIndex)v;
        }

        static StackObject* AssignFromStack_Index_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Hotfix_LT.Combat.CombatantIndex @Index = (Hotfix_LT.Combat.CombatantIndex)typeof(Hotfix_LT.Combat.CombatantIndex).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Index = @Index;
            return ptr_of_this_method;
        }

        static object get_ID_2(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).ID;
        }

        static StackObject* CopyToStack_ID_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).ID;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_ID_2(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).ID = (System.Int32)v;
        }

        static StackObject* AssignFromStack_ID_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @ID = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).ID = @ID;
            return ptr_of_this_method;
        }

        static object get_IngameId_3(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).IngameId;
        }

        static StackObject* CopyToStack_IngameId_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).IngameId;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_IngameId_3(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).IngameId = (System.Int32)v;
        }

        static StackObject* AssignFromStack_IngameId_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @IngameId = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).IngameId = @IngameId;
            return ptr_of_this_method;
        }

        static object get_SkillDataList_4(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).SkillDataList;
        }

        static StackObject* CopyToStack_SkillDataList_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).SkillDataList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_SkillDataList_4(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).SkillDataList = (System.Collections.Generic.Dictionary<System.Int32, Hotfix_LT.Combat.CombatCharacterSyncData.SkillData>)v;
        }

        static StackObject* AssignFromStack_SkillDataList_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.Int32, Hotfix_LT.Combat.CombatCharacterSyncData.SkillData> @SkillDataList = (System.Collections.Generic.Dictionary<System.Int32, Hotfix_LT.Combat.CombatCharacterSyncData.SkillData>)typeof(System.Collections.Generic.Dictionary<System.Int32, Hotfix_LT.Combat.CombatCharacterSyncData.SkillData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).SkillDataList = @SkillDataList;
            return ptr_of_this_method;
        }

        static object get_Uid_5(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Uid;
        }

        static StackObject* CopyToStack_Uid_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Uid;
            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_Uid_5(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Uid = (System.Int64)v;
        }

        static StackObject* AssignFromStack_Uid_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int64 @Uid = *(long*)&ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Uid = @Uid;
            return ptr_of_this_method;
        }

        static object get_ScrollTimes_6(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).ScrollTimes;
        }

        static StackObject* CopyToStack_ScrollTimes_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).ScrollTimes;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_ScrollTimes_6(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).ScrollTimes = (System.Int32)v;
        }

        static StackObject* AssignFromStack_ScrollTimes_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @ScrollTimes = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).ScrollTimes = @ScrollTimes;
            return ptr_of_this_method;
        }

        static object get_NormalSkillData_7(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).NormalSkillData;
        }

        static StackObject* CopyToStack_NormalSkillData_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).NormalSkillData;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_NormalSkillData_7(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).NormalSkillData = (Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)v;
        }

        static StackObject* AssignFromStack_NormalSkillData_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Hotfix_LT.Combat.CombatCharacterSyncData.SkillData @NormalSkillData = (Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData.SkillData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).NormalSkillData = @NormalSkillData;
            return ptr_of_this_method;
        }

        static object get_Attr_8(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Attr;
        }

        static StackObject* CopyToStack_Attr_8(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Attr;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_Attr_8(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Attr = (System.Int32)v;
        }

        static StackObject* AssignFromStack_Attr_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @Attr = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Attr = @Attr;
            return ptr_of_this_method;
        }

        static object get_Limits_9(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Limits;
        }

        static StackObject* CopyToStack_Limits_9(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Limits;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Limits_9(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Limits = (System.Collections.ArrayList)v;
        }

        static StackObject* AssignFromStack_Limits_9(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.ArrayList @Limits = (System.Collections.ArrayList)typeof(System.Collections.ArrayList).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Limits = @Limits;
            return ptr_of_this_method;
        }

        static object get_TplId_10(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).TplId;
        }

        static StackObject* CopyToStack_TplId_10(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).TplId;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_TplId_10(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).TplId = (System.Int32)v;
        }

        static StackObject* AssignFromStack_TplId_10(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @TplId = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).TplId = @TplId;
            return ptr_of_this_method;
        }

        static object get_MaxHp_11(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).MaxHp;
        }

        static StackObject* CopyToStack_MaxHp_11(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).MaxHp;
            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_MaxHp_11(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).MaxHp = (System.Int64)v;
        }

        static StackObject* AssignFromStack_MaxHp_11(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int64 @MaxHp = *(long*)&ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).MaxHp = @MaxHp;
            return ptr_of_this_method;
        }

        static object get_Skin_12(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Skin;
        }

        static StackObject* CopyToStack_Skin_12(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Skin;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_Skin_12(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Skin = (System.Int32)v;
        }

        static StackObject* AssignFromStack_Skin_12(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @Skin = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Skin = @Skin;
            return ptr_of_this_method;
        }

        static object get_TeamId_13(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).TeamId;
        }

        static StackObject* CopyToStack_TeamId_13(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).TeamId;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_TeamId_13(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).TeamId = (System.Int32)v;
        }

        static StackObject* AssignFromStack_TeamId_13(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @TeamId = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).TeamId = @TeamId;
            return ptr_of_this_method;
        }

        static object get_Portrait_14(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Portrait;
        }

        static StackObject* CopyToStack_Portrait_14(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Portrait;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Portrait_14(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Portrait = (System.String)v;
        }

        static StackObject* AssignFromStack_Portrait_14(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @Portrait = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatCharacterSyncData)o).Portrait = @Portrait;
            return ptr_of_this_method;
        }

        static object get_Default_15(ref object o)
        {
            return Hotfix_LT.Combat.CombatCharacterSyncData.Default;
        }

        static StackObject* CopyToStack_Default_15(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = Hotfix_LT.Combat.CombatCharacterSyncData.Default;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Default_15(ref object o, object v)
        {
            Hotfix_LT.Combat.CombatCharacterSyncData.Default = (Hotfix_LT.Combat.CombatCharacterSyncData.CombatCharacterComparer)v;
        }

        static StackObject* AssignFromStack_Default_15(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Hotfix_LT.Combat.CombatCharacterSyncData.CombatCharacterComparer @Default = (Hotfix_LT.Combat.CombatCharacterSyncData.CombatCharacterComparer)typeof(Hotfix_LT.Combat.CombatCharacterSyncData.CombatCharacterComparer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            Hotfix_LT.Combat.CombatCharacterSyncData.Default = @Default;
            return ptr_of_this_method;
        }



    }
}
