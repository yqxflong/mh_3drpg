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
    unsafe class Hotfix_LT_Combat_CombatCharacterSyncData_Binding_SkillData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Hotfix_LT.Combat.CombatCharacterSyncData.SkillData);
            args = new Type[]{};
            method = type.GetMethod("get_SkillType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_SkillType_0);
            args = new Type[]{};
            method = type.GetMethod("get_Icon", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Icon_1);
            args = new Type[]{};
            method = type.GetMethod("get_TypeName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TypeName_2);
            args = new Type[]{};
            method = type.GetMethod("get_MaxCooldown", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_MaxCooldown_3);

            field = type.GetField("Index", flag);
            app.RegisterCLRFieldGetter(field, get_Index_0);
            app.RegisterCLRFieldSetter(field, set_Index_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Index_0, AssignFromStack_Index_0);
            field = type.GetField("ID", flag);
            app.RegisterCLRFieldGetter(field, get_ID_1);
            app.RegisterCLRFieldSetter(field, set_ID_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_ID_1, AssignFromStack_ID_1);
            field = type.GetField("CD", flag);
            app.RegisterCLRFieldGetter(field, get_CD_2);
            app.RegisterCLRFieldSetter(field, set_CD_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_CD_2, AssignFromStack_CD_2);
            field = type.GetField("Level", flag);
            app.RegisterCLRFieldGetter(field, get_Level_3);
            app.RegisterCLRFieldSetter(field, set_Level_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_Level_3, AssignFromStack_Level_3);


        }


        static StackObject* get_SkillType_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatCharacterSyncData.SkillData instance_of_this_method = (Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData.SkillData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SkillType;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_Icon_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatCharacterSyncData.SkillData instance_of_this_method = (Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData.SkillData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Icon;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_TypeName_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatCharacterSyncData.SkillData instance_of_this_method = (Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData.SkillData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TypeName;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_MaxCooldown_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatCharacterSyncData.SkillData instance_of_this_method = (Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData.SkillData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.MaxCooldown;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }


        static object get_Index_0(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).Index;
        }

        static StackObject* CopyToStack_Index_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).Index;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_Index_0(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).Index = (System.Int32)v;
        }

        static StackObject* AssignFromStack_Index_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @Index = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).Index = @Index;
            return ptr_of_this_method;
        }

        static object get_ID_1(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).ID;
        }

        static StackObject* CopyToStack_ID_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).ID;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_ID_1(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).ID = (System.Int32)v;
        }

        static StackObject* AssignFromStack_ID_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @ID = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).ID = @ID;
            return ptr_of_this_method;
        }

        static object get_CD_2(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).CD;
        }

        static StackObject* CopyToStack_CD_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).CD;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_CD_2(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).CD = (System.Int32)v;
        }

        static StackObject* AssignFromStack_CD_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @CD = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).CD = @CD;
            return ptr_of_this_method;
        }

        static object get_Level_3(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).Level;
        }

        static StackObject* CopyToStack_Level_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).Level;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_Level_3(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).Level = (System.Int32)v;
        }

        static StackObject* AssignFromStack_Level_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @Level = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData.SkillData)o).Level = @Level;
            return ptr_of_this_method;
        }



    }
}
