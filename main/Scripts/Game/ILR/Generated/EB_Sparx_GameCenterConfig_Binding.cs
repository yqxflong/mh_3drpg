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
    unsafe class EB_Sparx_GameCenterConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.GameCenterConfig);

            field = type.GetField("Enabled", flag);
            app.RegisterCLRFieldGetter(field, get_Enabled_0);
            app.RegisterCLRFieldSetter(field, set_Enabled_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Enabled_0, AssignFromStack_Enabled_0);
            field = type.GetField("LeaderboardPrefix", flag);
            app.RegisterCLRFieldGetter(field, get_LeaderboardPrefix_1);
            app.RegisterCLRFieldSetter(field, set_LeaderboardPrefix_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_LeaderboardPrefix_1, AssignFromStack_LeaderboardPrefix_1);


        }



        static object get_Enabled_0(ref object o)
        {
            return ((EB.Sparx.GameCenterConfig)o).Enabled;
        }

        static StackObject* CopyToStack_Enabled_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.GameCenterConfig)o).Enabled;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_Enabled_0(ref object o, object v)
        {
            ((EB.Sparx.GameCenterConfig)o).Enabled = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_Enabled_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @Enabled = ptr_of_this_method->Value == 1;
            ((EB.Sparx.GameCenterConfig)o).Enabled = @Enabled;
            return ptr_of_this_method;
        }

        static object get_LeaderboardPrefix_1(ref object o)
        {
            return ((EB.Sparx.GameCenterConfig)o).LeaderboardPrefix;
        }

        static StackObject* CopyToStack_LeaderboardPrefix_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.GameCenterConfig)o).LeaderboardPrefix;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LeaderboardPrefix_1(ref object o, object v)
        {
            ((EB.Sparx.GameCenterConfig)o).LeaderboardPrefix = (System.String)v;
        }

        static StackObject* AssignFromStack_LeaderboardPrefix_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @LeaderboardPrefix = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.GameCenterConfig)o).LeaderboardPrefix = @LeaderboardPrefix;
            return ptr_of_this_method;
        }



    }
}
