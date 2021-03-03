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
    unsafe class Hotfix_LT_Combat_CombatTeamSyncData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Hotfix_LT.Combat.CombatTeamSyncData);

            field = type.GetField("CharList", flag);
            app.RegisterCLRFieldGetter(field, get_CharList_0);
            app.RegisterCLRFieldSetter(field, set_CharList_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_CharList_0, AssignFromStack_CharList_0);
            field = type.GetField("TeamId", flag);
            app.RegisterCLRFieldGetter(field, get_TeamId_1);
            app.RegisterCLRFieldSetter(field, set_TeamId_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_TeamId_1, AssignFromStack_TeamId_1);
            field = type.GetField("SPoint", flag);
            app.RegisterCLRFieldGetter(field, get_SPoint_2);
            app.RegisterCLRFieldSetter(field, set_SPoint_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_SPoint_2, AssignFromStack_SPoint_2);
            field = type.GetField("CurWave", flag);
            app.RegisterCLRFieldGetter(field, get_CurWave_3);
            app.RegisterCLRFieldSetter(field, set_CurWave_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_CurWave_3, AssignFromStack_CurWave_3);
            field = type.GetField("MaxWave", flag);
            app.RegisterCLRFieldGetter(field, get_MaxWave_4);
            app.RegisterCLRFieldSetter(field, set_MaxWave_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_MaxWave_4, AssignFromStack_MaxWave_4);
            field = type.GetField("ScrollList", flag);
            app.RegisterCLRFieldGetter(field, get_ScrollList_5);
            app.RegisterCLRFieldSetter(field, set_ScrollList_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_ScrollList_5, AssignFromStack_ScrollList_5);


        }



        static object get_CharList_0(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatTeamSyncData)o).CharList;
        }

        static StackObject* CopyToStack_CharList_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatTeamSyncData)o).CharList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_CharList_0(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).CharList = (System.Collections.Generic.List<Hotfix_LT.Combat.CombatCharacterSyncData>)v;
        }

        static StackObject* AssignFromStack_CharList_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<Hotfix_LT.Combat.CombatCharacterSyncData> @CharList = (System.Collections.Generic.List<Hotfix_LT.Combat.CombatCharacterSyncData>)typeof(System.Collections.Generic.List<Hotfix_LT.Combat.CombatCharacterSyncData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).CharList = @CharList;
            return ptr_of_this_method;
        }

        static object get_TeamId_1(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatTeamSyncData)o).TeamId;
        }

        static StackObject* CopyToStack_TeamId_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatTeamSyncData)o).TeamId;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_TeamId_1(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).TeamId = (System.Int32)v;
        }

        static StackObject* AssignFromStack_TeamId_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @TeamId = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).TeamId = @TeamId;
            return ptr_of_this_method;
        }

        static object get_SPoint_2(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatTeamSyncData)o).SPoint;
        }

        static StackObject* CopyToStack_SPoint_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatTeamSyncData)o).SPoint;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_SPoint_2(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).SPoint = (System.Int32)v;
        }

        static StackObject* AssignFromStack_SPoint_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @SPoint = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).SPoint = @SPoint;
            return ptr_of_this_method;
        }

        static object get_CurWave_3(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatTeamSyncData)o).CurWave;
        }

        static StackObject* CopyToStack_CurWave_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatTeamSyncData)o).CurWave;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_CurWave_3(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).CurWave = (System.Int32)v;
        }

        static StackObject* AssignFromStack_CurWave_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @CurWave = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).CurWave = @CurWave;
            return ptr_of_this_method;
        }

        static object get_MaxWave_4(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatTeamSyncData)o).MaxWave;
        }

        static StackObject* CopyToStack_MaxWave_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatTeamSyncData)o).MaxWave;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_MaxWave_4(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).MaxWave = (System.Int32)v;
        }

        static StackObject* AssignFromStack_MaxWave_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @MaxWave = ptr_of_this_method->Value;
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).MaxWave = @MaxWave;
            return ptr_of_this_method;
        }

        static object get_ScrollList_5(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatTeamSyncData)o).ScrollList;
        }

        static StackObject* CopyToStack_ScrollList_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatTeamSyncData)o).ScrollList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ScrollList_5(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).ScrollList = (System.Collections.Generic.List<Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData>)v;
        }

        static StackObject* AssignFromStack_ScrollList_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData> @ScrollList = (System.Collections.Generic.List<Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData>)typeof(System.Collections.Generic.List<Hotfix_LT.Combat.CombatTeamSyncData.LTShowItemData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatTeamSyncData)o).ScrollList = @ScrollList;
            return ptr_of_this_method;
        }



    }
}
