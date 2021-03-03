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
    unsafe class EB_Sparx_LevelRewardsManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.LevelRewardsManager);
            args = new Type[]{};
            method = type.GetMethod("get_Level", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Level_0);
            args = new Type[]{};
            method = type.GetMethod("get_IsLevelUp", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsLevelUp_1);
            args = new Type[]{};
            method = type.GetMethod("GetLevelupXpInfo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetLevelupXpInfo_2);

            field = type.GetField("OnLevelChange", flag);
            app.RegisterCLRFieldGetter(field, get_OnLevelChange_0);
            app.RegisterCLRFieldSetter(field, set_OnLevelChange_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnLevelChange_0, AssignFromStack_OnLevelChange_0);
            field = type.GetField("CurLevel", flag);
            app.RegisterCLRFieldGetter(field, get_CurLevel_1);
            app.RegisterCLRFieldSetter(field, set_CurLevel_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_CurLevel_1, AssignFromStack_CurLevel_1);


        }


        static StackObject* get_Level_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.LevelRewardsManager instance_of_this_method = (EB.Sparx.LevelRewardsManager)typeof(EB.Sparx.LevelRewardsManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Level;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_IsLevelUp_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.LevelRewardsManager instance_of_this_method = (EB.Sparx.LevelRewardsManager)typeof(EB.Sparx.LevelRewardsManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsLevelUp;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* GetLevelupXpInfo_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.LevelRewardsManager instance_of_this_method = (EB.Sparx.LevelRewardsManager)typeof(EB.Sparx.LevelRewardsManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetLevelupXpInfo();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_OnLevelChange_0(ref object o)
        {
            return ((EB.Sparx.LevelRewardsManager)o).OnLevelChange;
        }

        static StackObject* CopyToStack_OnLevelChange_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.LevelRewardsManager)o).OnLevelChange;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnLevelChange_0(ref object o, object v)
        {
            ((EB.Sparx.LevelRewardsManager)o).OnLevelChange = (EB.Sparx.LevelRewardsManager.LevelRewardsChangeDel)v;
        }

        static StackObject* AssignFromStack_OnLevelChange_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.LevelRewardsManager.LevelRewardsChangeDel @OnLevelChange = (EB.Sparx.LevelRewardsManager.LevelRewardsChangeDel)typeof(EB.Sparx.LevelRewardsManager.LevelRewardsChangeDel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.LevelRewardsManager)o).OnLevelChange = @OnLevelChange;
            return ptr_of_this_method;
        }

        static object get_CurLevel_1(ref object o)
        {
            return ((EB.Sparx.LevelRewardsManager)o).CurLevel;
        }

        static StackObject* CopyToStack_CurLevel_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.LevelRewardsManager)o).CurLevel;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_CurLevel_1(ref object o, object v)
        {
            ((EB.Sparx.LevelRewardsManager)o).CurLevel = (System.Int32)v;
        }

        static StackObject* AssignFromStack_CurLevel_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @CurLevel = ptr_of_this_method->Value;
            ((EB.Sparx.LevelRewardsManager)o).CurLevel = @CurLevel;
            return ptr_of_this_method;
        }



    }
}
