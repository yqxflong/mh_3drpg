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
    unsafe class Hotfix_LT_Combat_CombatCharacterSyncData_Binding_BuffData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Hotfix_LT.Combat.CombatCharacterSyncData.BuffData);
            args = new Type[]{};
            method = type.GetMethod("GetMaxTurnStr", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetMaxTurnStr_0);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("GetOverlying", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetOverlying_1);

            field = type.GetField("Id", flag);
            app.RegisterCLRFieldGetter(field, get_Id_0);
            app.RegisterCLRFieldSetter(field, set_Id_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Id_0, AssignFromStack_Id_0);
            field = type.GetField("LeftTurnArray", flag);
            app.RegisterCLRFieldGetter(field, get_LeftTurnArray_1);
            app.RegisterCLRFieldSetter(field, set_LeftTurnArray_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_LeftTurnArray_1, AssignFromStack_LeftTurnArray_1);


        }


        static StackObject* GetMaxTurnStr_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatCharacterSyncData.BuffData instance_of_this_method = (Hotfix_LT.Combat.CombatCharacterSyncData.BuffData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData.BuffData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetMaxTurnStr();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetOverlying_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @SackNum = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.Combat.CombatCharacterSyncData.BuffData instance_of_this_method = (Hotfix_LT.Combat.CombatCharacterSyncData.BuffData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData.BuffData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetOverlying(@SackNum);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_Id_0(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData.BuffData)o).Id;
        }

        static StackObject* CopyToStack_Id_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData.BuffData)o).Id;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_Id_0(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData.BuffData)o).Id = (System.Int32)v;
        }

        static StackObject* AssignFromStack_Id_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @Id = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatCharacterSyncData.BuffData)o).Id = @Id;
            return ptr_of_this_method;
        }

        static object get_LeftTurnArray_1(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatCharacterSyncData.BuffData)o).LeftTurnArray;
        }

        static StackObject* CopyToStack_LeftTurnArray_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatCharacterSyncData.BuffData)o).LeftTurnArray;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LeftTurnArray_1(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatCharacterSyncData.BuffData)o).LeftTurnArray = (System.Int32[])v;
        }

        static StackObject* AssignFromStack_LeftTurnArray_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32[] @LeftTurnArray = (System.Int32[])typeof(System.Int32[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatCharacterSyncData.BuffData)o).LeftTurnArray = @LeftTurnArray;
            return ptr_of_this_method;
        }



    }
}
