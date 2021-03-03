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
    unsafe class Hotfix_LT_Combat_CombatSyncData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Hotfix_LT.Combat.CombatSyncData);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);
            args = new Type[]{};
            method = type.GetMethod("GetDiedCharacterList", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetDiedCharacterList_1);
            args = new Type[]{};
            method = type.GetMethod("CleanUp", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CleanUp_2);
            args = new Type[]{};
            method = type.GetMethod("get_EventQueue", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_EventQueue_3);
            args = new Type[]{};
            method = type.GetMethod("get_CombatId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_CombatId_4);
            args = new Type[]{typeof(Hotfix_LT.Combat.CombatSyncEventBase)};
            method = type.GetMethod("AddQueue", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddQueue_5);
            args = new Type[]{typeof(System.Collections.Hashtable), typeof(System.Boolean)};
            method = type.GetMethod("Parse", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Parse_6);
            args = new Type[]{};
            method = type.GetMethod("ParseAll", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ParseAll_7);
            args = new Type[]{};
            method = type.GetMethod("getCurrentSubEid", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, getCurrentSubEid_8);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("GetCharacterList", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCharacterList_9);
            args = new Type[]{};
            method = type.GetMethod("get_ActionId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ActionId_10);
            args = new Type[]{};
            method = type.GetMethod("GetMyTeamData", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetMyTeamData_11);
            args = new Type[]{};
            method = type.GetMethod("get_Turn", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Turn_12);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("GetCharacterData", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCharacterData_13);
            args = new Type[]{};
            method = type.GetMethod("get_TeamDataDic", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_TeamDataDic_14);
            args = new Type[]{};
            method = type.GetMethod("GetAliveCharacterList", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetAliveCharacterList_15);

            field = type.GetField("NeedSetSkill", flag);
            app.RegisterCLRFieldGetter(field, get_NeedSetSkill_0);
            app.RegisterCLRFieldSetter(field, set_NeedSetSkill_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_NeedSetSkill_0, AssignFromStack_NeedSetSkill_0);
            field = type.GetField("PendingEvents", flag);
            app.RegisterCLRFieldGetter(field, get_PendingEvents_1);
            app.RegisterCLRFieldSetter(field, set_PendingEvents_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_PendingEvents_1, AssignFromStack_PendingEvents_1);
            field = type.GetField("WaitCharacterData", flag);
            app.RegisterCLRFieldGetter(field, get_WaitCharacterData_2);
            app.RegisterCLRFieldSetter(field, set_WaitCharacterData_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_WaitCharacterData_2, AssignFromStack_WaitCharacterData_2);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = Hotfix_LT.Combat.CombatSyncData.Instance;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetDiedCharacterList_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetDiedCharacterList();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* CleanUp_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CleanUp();

            return __ret;
        }

        static StackObject* get_EventQueue_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.EventQueue;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_CombatId_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CombatId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* AddQueue_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncEventBase @eventBase = (Hotfix_LT.Combat.CombatSyncEventBase)typeof(Hotfix_LT.Combat.CombatSyncEventBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddQueue(@eventBase);

            return __ret;
        }

        static StackObject* Parse_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @directDeal = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Hashtable @data = (System.Collections.Hashtable)typeof(System.Collections.Hashtable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Parse(@data, @directDeal);

            return __ret;
        }

        static StackObject* ParseAll_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ParseAll();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* getCurrentSubEid_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.getCurrentSubEid();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetCharacterList_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @teamID = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetCharacterList(@teamID);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_ActionId_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ActionId;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetMyTeamData_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetMyTeamData();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Turn_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Turn;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetCharacterData_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @actor_id = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetCharacterData(@actor_id);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_TeamDataDic_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TeamDataDic;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetAliveCharacterList_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Hotfix_LT.Combat.CombatSyncData instance_of_this_method = (Hotfix_LT.Combat.CombatSyncData)typeof(Hotfix_LT.Combat.CombatSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetAliveCharacterList();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_NeedSetSkill_0(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatSyncData)o).NeedSetSkill;
        }

        static StackObject* CopyToStack_NeedSetSkill_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatSyncData)o).NeedSetSkill;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_NeedSetSkill_0(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatSyncData)o).NeedSetSkill = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_NeedSetSkill_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @NeedSetSkill = ptr_of_this_method->Value == 1;
            ((Hotfix_LT.Combat.CombatSyncData)o).NeedSetSkill = @NeedSetSkill;
            return ptr_of_this_method;
        }

        static object get_PendingEvents_1(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatSyncData)o).PendingEvents;
        }

        static StackObject* CopyToStack_PendingEvents_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatSyncData)o).PendingEvents;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_PendingEvents_1(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatSyncData)o).PendingEvents = (System.Collections.Generic.IDictionary<System.Int32, System.Collections.Hashtable>)v;
        }

        static StackObject* AssignFromStack_PendingEvents_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.IDictionary<System.Int32, System.Collections.Hashtable> @PendingEvents = (System.Collections.Generic.IDictionary<System.Int32, System.Collections.Hashtable>)typeof(System.Collections.Generic.IDictionary<System.Int32, System.Collections.Hashtable>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatSyncData)o).PendingEvents = @PendingEvents;
            return ptr_of_this_method;
        }

        static object get_WaitCharacterData_2(ref object o)
        {
            return ((Hotfix_LT.Combat.CombatSyncData)o).WaitCharacterData;
        }

        static StackObject* CopyToStack_WaitCharacterData_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Hotfix_LT.Combat.CombatSyncData)o).WaitCharacterData;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_WaitCharacterData_2(ref object o, object v)
        {
            ((Hotfix_LT.Combat.CombatSyncData)o).WaitCharacterData = (Hotfix_LT.Combat.CombatCharacterSyncData)v;
        }

        static StackObject* AssignFromStack_WaitCharacterData_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Hotfix_LT.Combat.CombatCharacterSyncData @WaitCharacterData = (Hotfix_LT.Combat.CombatCharacterSyncData)typeof(Hotfix_LT.Combat.CombatCharacterSyncData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Hotfix_LT.Combat.CombatSyncData)o).WaitCharacterData = @WaitCharacterData;
            return ptr_of_this_method;
        }



    }
}
