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
    unsafe class EB_Sparx_RoleData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.RoleData);

            field = type.GetField("code", flag);
            app.RegisterCLRFieldGetter(field, get_code_0);
            app.RegisterCLRFieldSetter(field, set_code_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_code_0, AssignFromStack_code_0);
            field = type.GetField("roleGid", flag);
            app.RegisterCLRFieldGetter(field, get_roleGid_1);
            app.RegisterCLRFieldSetter(field, set_roleGid_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_roleGid_1, AssignFromStack_roleGid_1);
            field = type.GetField("roleId", flag);
            app.RegisterCLRFieldGetter(field, get_roleId_2);
            app.RegisterCLRFieldSetter(field, set_roleId_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_roleId_2, AssignFromStack_roleId_2);
            field = type.GetField("roleName", flag);
            app.RegisterCLRFieldGetter(field, get_roleName_3);
            app.RegisterCLRFieldSetter(field, set_roleName_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_roleName_3, AssignFromStack_roleName_3);
            field = type.GetField("roleLevel", flag);
            app.RegisterCLRFieldGetter(field, get_roleLevel_4);
            app.RegisterCLRFieldSetter(field, set_roleLevel_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_roleLevel_4, AssignFromStack_roleLevel_4);
            field = type.GetField("serverId", flag);
            app.RegisterCLRFieldGetter(field, get_serverId_5);
            app.RegisterCLRFieldSetter(field, set_serverId_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_serverId_5, AssignFromStack_serverId_5);
            field = type.GetField("serverName", flag);
            app.RegisterCLRFieldGetter(field, get_serverName_6);
            app.RegisterCLRFieldSetter(field, set_serverName_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_serverName_6, AssignFromStack_serverName_6);
            field = type.GetField("coinNum", flag);
            app.RegisterCLRFieldGetter(field, get_coinNum_7);
            app.RegisterCLRFieldSetter(field, set_coinNum_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_coinNum_7, AssignFromStack_coinNum_7);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static object get_code_0(ref object o)
        {
            return ((EB.Sparx.RoleData)o).code;
        }

        static StackObject* CopyToStack_code_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.RoleData)o).code;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_code_0(ref object o, object v)
        {
            ((EB.Sparx.RoleData)o).code = (System.Int32)v;
        }

        static StackObject* AssignFromStack_code_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @code = ptr_of_this_method->Value;
            ((EB.Sparx.RoleData)o).code = @code;
            return ptr_of_this_method;
        }

        static object get_roleGid_1(ref object o)
        {
            return ((EB.Sparx.RoleData)o).roleGid;
        }

        static StackObject* CopyToStack_roleGid_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.RoleData)o).roleGid;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_roleGid_1(ref object o, object v)
        {
            ((EB.Sparx.RoleData)o).roleGid = (System.String)v;
        }

        static StackObject* AssignFromStack_roleGid_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @roleGid = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.RoleData)o).roleGid = @roleGid;
            return ptr_of_this_method;
        }

        static object get_roleId_2(ref object o)
        {
            return ((EB.Sparx.RoleData)o).roleId;
        }

        static StackObject* CopyToStack_roleId_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.RoleData)o).roleId;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_roleId_2(ref object o, object v)
        {
            ((EB.Sparx.RoleData)o).roleId = (System.String)v;
        }

        static StackObject* AssignFromStack_roleId_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @roleId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.RoleData)o).roleId = @roleId;
            return ptr_of_this_method;
        }

        static object get_roleName_3(ref object o)
        {
            return ((EB.Sparx.RoleData)o).roleName;
        }

        static StackObject* CopyToStack_roleName_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.RoleData)o).roleName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_roleName_3(ref object o, object v)
        {
            ((EB.Sparx.RoleData)o).roleName = (System.String)v;
        }

        static StackObject* AssignFromStack_roleName_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @roleName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.RoleData)o).roleName = @roleName;
            return ptr_of_this_method;
        }

        static object get_roleLevel_4(ref object o)
        {
            return ((EB.Sparx.RoleData)o).roleLevel;
        }

        static StackObject* CopyToStack_roleLevel_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.RoleData)o).roleLevel;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_roleLevel_4(ref object o, object v)
        {
            ((EB.Sparx.RoleData)o).roleLevel = (System.Int32)v;
        }

        static StackObject* AssignFromStack_roleLevel_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @roleLevel = ptr_of_this_method->Value;
            ((EB.Sparx.RoleData)o).roleLevel = @roleLevel;
            return ptr_of_this_method;
        }

        static object get_serverId_5(ref object o)
        {
            return ((EB.Sparx.RoleData)o).serverId;
        }

        static StackObject* CopyToStack_serverId_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.RoleData)o).serverId;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_serverId_5(ref object o, object v)
        {
            ((EB.Sparx.RoleData)o).serverId = (System.String)v;
        }

        static StackObject* AssignFromStack_serverId_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @serverId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.RoleData)o).serverId = @serverId;
            return ptr_of_this_method;
        }

        static object get_serverName_6(ref object o)
        {
            return ((EB.Sparx.RoleData)o).serverName;
        }

        static StackObject* CopyToStack_serverName_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.RoleData)o).serverName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_serverName_6(ref object o, object v)
        {
            ((EB.Sparx.RoleData)o).serverName = (System.String)v;
        }

        static StackObject* AssignFromStack_serverName_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @serverName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.RoleData)o).serverName = @serverName;
            return ptr_of_this_method;
        }

        static object get_coinNum_7(ref object o)
        {
            return ((EB.Sparx.RoleData)o).coinNum;
        }

        static StackObject* CopyToStack_coinNum_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.RoleData)o).coinNum;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_coinNum_7(ref object o, object v)
        {
            ((EB.Sparx.RoleData)o).coinNum = (System.Int32)v;
        }

        static StackObject* AssignFromStack_coinNum_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @coinNum = ptr_of_this_method->Value;
            ((EB.Sparx.RoleData)o).coinNum = @coinNum;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new EB.Sparx.RoleData();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
