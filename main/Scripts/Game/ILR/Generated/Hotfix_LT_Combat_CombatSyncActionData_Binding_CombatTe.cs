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
    unsafe class Hotfix_LT_Combat_CombatSyncActionData_Binding_CombatTeamInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo);

            field = type.GetField("teamWin", flag);
            app.RegisterCLRFieldGetter(field, get_teamWin_0);
            app.RegisterCLRFieldSetter(field, set_teamWin_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_teamWin_0, AssignFromStack_teamWin_0);
            field = type.GetField("teamIndex", flag);
            app.RegisterCLRFieldGetter(field, get_teamIndex_1);
            app.RegisterCLRFieldSetter(field, set_teamIndex_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_teamIndex_1, AssignFromStack_teamIndex_1);
            field = type.GetField("worldId", flag);
            app.RegisterCLRFieldGetter(field, get_worldId_2);
            app.RegisterCLRFieldSetter(field, set_worldId_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_worldId_2, AssignFromStack_worldId_2);
            field = type.GetField("teamName", flag);
            app.RegisterCLRFieldGetter(field, get_teamName_3);
            app.RegisterCLRFieldSetter(field, set_teamName_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_teamName_3, AssignFromStack_teamName_3);


        }



        static object get_teamWin_0(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamWin;
        }

        static StackObject* CopyToStack_teamWin_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamWin;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_teamWin_0(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamWin = (System.Int32)v;
        }

        static StackObject* AssignFromStack_teamWin_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @teamWin = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamWin = @teamWin;
            return ptr_of_this_method;
        }

        static object get_teamIndex_1(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamIndex;
        }

        static StackObject* CopyToStack_teamIndex_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamIndex;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_teamIndex_1(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamIndex = (System.Int32)v;
        }

        static StackObject* AssignFromStack_teamIndex_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @teamIndex = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamIndex = @teamIndex;
            return ptr_of_this_method;
        }

        static object get_worldId_2(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).worldId;
        }

        static StackObject* CopyToStack_worldId_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).worldId;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_worldId_2(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).worldId = (System.Int32)v;
        }

        static StackObject* AssignFromStack_worldId_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @worldId = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).worldId = @worldId;
            return ptr_of_this_method;
        }

        static object get_teamName_3(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamName;
        }

        static StackObject* CopyToStack_teamName_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_teamName_3(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamName = (System.String)v;
        }

        static StackObject* AssignFromStack_teamName_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @teamName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo)o).teamName = @teamName;
            return ptr_of_this_method;
        }



    }
}
