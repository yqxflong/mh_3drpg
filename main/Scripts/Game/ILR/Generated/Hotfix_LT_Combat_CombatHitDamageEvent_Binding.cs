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
    unsafe class Hotfix_LT_Combat_CombatHitDamageEvent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Hotfix_LT.Combat.CombatHitDamageEvent);
            args = new Type[]{};
            method = type.GetMethod("get_TargetCombatant", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TargetCombatant_0);
            args = new Type[]{};
            method = type.GetMethod("get_ShowDamage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ShowDamage_1);
            args = new Type[]{};
            method = type.GetMethod("get_IsCrit", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsCrit_2);
            args = new Type[]{};
            method = type.GetMethod("get_Shield", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Shield_3);
            args = new Type[]{};
            method = type.GetMethod("get_Damage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Damage_4);


        }


        static StackObject* get_TargetCombatant_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatHitDamageEvent instance_of_this_method = (Hotfix_LT.Combat.CombatHitDamageEvent)typeof(Hotfix_LT.Combat.CombatHitDamageEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TargetCombatant;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_ShowDamage_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatHitDamageEvent instance_of_this_method = (Hotfix_LT.Combat.CombatHitDamageEvent)typeof(Hotfix_LT.Combat.CombatHitDamageEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ShowDamage;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_IsCrit_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatHitDamageEvent instance_of_this_method = (Hotfix_LT.Combat.CombatHitDamageEvent)typeof(Hotfix_LT.Combat.CombatHitDamageEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsCrit;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_Shield_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatHitDamageEvent instance_of_this_method = (Hotfix_LT.Combat.CombatHitDamageEvent)typeof(Hotfix_LT.Combat.CombatHitDamageEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Shield;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_Damage_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatHitDamageEvent instance_of_this_method = (Hotfix_LT.Combat.CombatHitDamageEvent)typeof(Hotfix_LT.Combat.CombatHitDamageEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Damage;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }



    }
}
