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
    unsafe class Hotfix_LT_Combat_CombatTeamSyncData_Binding_LTShowItemData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData);

            field = type.GetField("count", flag);
            app.RegisterCLRFieldGetter(field, get_count_0);
            app.RegisterCLRFieldSetter(field, set_count_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_count_0, AssignFromStack_count_0);
            field = type.GetField("type", flag);
            app.RegisterCLRFieldGetter(field, get_type_1);
            app.RegisterCLRFieldSetter(field, set_type_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_type_1, AssignFromStack_type_1);
            field = type.GetField("id", flag);
            app.RegisterCLRFieldGetter(field, get_id_2);
            app.RegisterCLRFieldSetter(field, set_id_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_id_2, AssignFromStack_id_2);


        }



        static object get_count_0(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).count;
        }

        static StackObject* CopyToStack_count_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).count;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_count_0(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).count = (System.Int32)v;
        }

        static StackObject* AssignFromStack_count_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @count = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).count = @count;
            return ptr_of_this_method;
        }

        static object get_type_1(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).type;
        }

        static StackObject* CopyToStack_type_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).type;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_type_1(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).type = (System.String)v;
        }

        static StackObject* AssignFromStack_type_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @type = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).type = @type;
            return ptr_of_this_method;
        }

        static object get_id_2(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).id;
        }

        static StackObject* CopyToStack_id_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).id;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_id_2(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).id = (System.String)v;
        }

        static StackObject* AssignFromStack_id_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @id = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData)o).id = @id;
            return ptr_of_this_method;
        }



    }
}
