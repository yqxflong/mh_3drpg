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
    unsafe class EB_Sparx_Config_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.Config);

            field = type.GetField("ApiEndpoint", flag);
            app.RegisterCLRFieldGetter(field, get_ApiEndpoint_0);
            app.RegisterCLRFieldSetter(field, set_ApiEndpoint_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_ApiEndpoint_0, AssignFromStack_ApiEndpoint_0);
            field = type.GetField("ApiKey", flag);
            app.RegisterCLRFieldGetter(field, get_ApiKey_1);
            app.RegisterCLRFieldSetter(field, set_ApiKey_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_ApiKey_1, AssignFromStack_ApiKey_1);
            field = type.GetField("Locale", flag);
            app.RegisterCLRFieldGetter(field, get_Locale_2);
            app.RegisterCLRFieldSetter(field, set_Locale_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_Locale_2, AssignFromStack_Locale_2);
            field = type.GetField("WalletConfig", flag);
            app.RegisterCLRFieldGetter(field, get_WalletConfig_3);
            app.RegisterCLRFieldSetter(field, set_WalletConfig_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_WalletConfig_3, AssignFromStack_WalletConfig_3);
            field = type.GetField("LoginConfig", flag);
            app.RegisterCLRFieldGetter(field, get_LoginConfig_4);
            app.RegisterCLRFieldSetter(field, set_LoginConfig_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_LoginConfig_4, AssignFromStack_LoginConfig_4);
            field = type.GetField("GameManagerConfig", flag);
            app.RegisterCLRFieldGetter(field, get_GameManagerConfig_5);
            app.RegisterCLRFieldSetter(field, set_GameManagerConfig_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_GameManagerConfig_5, AssignFromStack_GameManagerConfig_5);
            field = type.GetField("GameCenterConfig", flag);
            app.RegisterCLRFieldGetter(field, get_GameCenterConfig_6);
            app.RegisterCLRFieldSetter(field, set_GameCenterConfig_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_GameCenterConfig_6, AssignFromStack_GameCenterConfig_6);
            field = type.GetField("GachaConfig", flag);
            app.RegisterCLRFieldGetter(field, get_GachaConfig_7);
            app.RegisterCLRFieldSetter(field, set_GachaConfig_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_GachaConfig_7, AssignFromStack_GachaConfig_7);
            field = type.GetField("PerformanceConfig", flag);
            app.RegisterCLRFieldGetter(field, get_PerformanceConfig_8);
            app.RegisterCLRFieldSetter(field, set_PerformanceConfig_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_PerformanceConfig_8, AssignFromStack_PerformanceConfig_8);
            field = type.GetField("GameComponents", flag);
            app.RegisterCLRFieldGetter(field, get_GameComponents_9);
            app.RegisterCLRFieldSetter(field, set_GameComponents_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_GameComponents_9, AssignFromStack_GameComponents_9);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static object get_ApiEndpoint_0(ref object o)
        {
            return ((EB.Sparx.Config)o).ApiEndpoint;
        }

        static StackObject* CopyToStack_ApiEndpoint_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.Config)o).ApiEndpoint;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ApiEndpoint_0(ref object o, object v)
        {
            ((EB.Sparx.Config)o).ApiEndpoint = (System.String)v;
        }

        static StackObject* AssignFromStack_ApiEndpoint_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @ApiEndpoint = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.Config)o).ApiEndpoint = @ApiEndpoint;
            return ptr_of_this_method;
        }

        static object get_ApiKey_1(ref object o)
        {
            return ((EB.Sparx.Config)o).ApiKey;
        }

        static StackObject* CopyToStack_ApiKey_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.Config)o).ApiKey;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ApiKey_1(ref object o, object v)
        {
            ((EB.Sparx.Config)o).ApiKey = (EB.Sparx.Key)v;
        }

        static StackObject* AssignFromStack_ApiKey_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.Key @ApiKey = (EB.Sparx.Key)typeof(EB.Sparx.Key).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.Config)o).ApiKey = @ApiKey;
            return ptr_of_this_method;
        }

        static object get_Locale_2(ref object o)
        {
            return ((EB.Sparx.Config)o).Locale;
        }

        static StackObject* CopyToStack_Locale_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.Config)o).Locale;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Locale_2(ref object o, object v)
        {
            ((EB.Sparx.Config)o).Locale = (EB.Language)v;
        }

        static StackObject* AssignFromStack_Locale_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Language @Locale = (EB.Language)typeof(EB.Language).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.Config)o).Locale = @Locale;
            return ptr_of_this_method;
        }

        static object get_WalletConfig_3(ref object o)
        {
            return ((EB.Sparx.Config)o).WalletConfig;
        }

        static StackObject* CopyToStack_WalletConfig_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.Config)o).WalletConfig;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_WalletConfig_3(ref object o, object v)
        {
            ((EB.Sparx.Config)o).WalletConfig = (EB.Sparx.WalletConfig)v;
        }

        static StackObject* AssignFromStack_WalletConfig_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.WalletConfig @WalletConfig = (EB.Sparx.WalletConfig)typeof(EB.Sparx.WalletConfig).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.Config)o).WalletConfig = @WalletConfig;
            return ptr_of_this_method;
        }

        static object get_LoginConfig_4(ref object o)
        {
            return ((EB.Sparx.Config)o).LoginConfig;
        }

        static StackObject* CopyToStack_LoginConfig_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.Config)o).LoginConfig;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LoginConfig_4(ref object o, object v)
        {
            ((EB.Sparx.Config)o).LoginConfig = (EB.Sparx.LoginConfig)v;
        }

        static StackObject* AssignFromStack_LoginConfig_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.LoginConfig @LoginConfig = (EB.Sparx.LoginConfig)typeof(EB.Sparx.LoginConfig).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.Config)o).LoginConfig = @LoginConfig;
            return ptr_of_this_method;
        }

        static object get_GameManagerConfig_5(ref object o)
        {
            return ((EB.Sparx.Config)o).GameManagerConfig;
        }

        static StackObject* CopyToStack_GameManagerConfig_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.Config)o).GameManagerConfig;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GameManagerConfig_5(ref object o, object v)
        {
            ((EB.Sparx.Config)o).GameManagerConfig = (EB.Sparx.GameManagerConfig)v;
        }

        static StackObject* AssignFromStack_GameManagerConfig_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.GameManagerConfig @GameManagerConfig = (EB.Sparx.GameManagerConfig)typeof(EB.Sparx.GameManagerConfig).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.Config)o).GameManagerConfig = @GameManagerConfig;
            return ptr_of_this_method;
        }

        static object get_GameCenterConfig_6(ref object o)
        {
            return ((EB.Sparx.Config)o).GameCenterConfig;
        }

        static StackObject* CopyToStack_GameCenterConfig_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.Config)o).GameCenterConfig;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GameCenterConfig_6(ref object o, object v)
        {
            ((EB.Sparx.Config)o).GameCenterConfig = (EB.Sparx.GameCenterConfig)v;
        }

        static StackObject* AssignFromStack_GameCenterConfig_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.GameCenterConfig @GameCenterConfig = (EB.Sparx.GameCenterConfig)typeof(EB.Sparx.GameCenterConfig).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.Config)o).GameCenterConfig = @GameCenterConfig;
            return ptr_of_this_method;
        }

        static object get_GachaConfig_7(ref object o)
        {
            return ((EB.Sparx.Config)o).GachaConfig;
        }

        static StackObject* CopyToStack_GachaConfig_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.Config)o).GachaConfig;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GachaConfig_7(ref object o, object v)
        {
            ((EB.Sparx.Config)o).GachaConfig = (EB.Sparx.GachaConfig)v;
        }

        static StackObject* AssignFromStack_GachaConfig_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.GachaConfig @GachaConfig = (EB.Sparx.GachaConfig)typeof(EB.Sparx.GachaConfig).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.Config)o).GachaConfig = @GachaConfig;
            return ptr_of_this_method;
        }

        static object get_PerformanceConfig_8(ref object o)
        {
            return ((EB.Sparx.Config)o).PerformanceConfig;
        }

        static StackObject* CopyToStack_PerformanceConfig_8(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.Config)o).PerformanceConfig;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_PerformanceConfig_8(ref object o, object v)
        {
            ((EB.Sparx.Config)o).PerformanceConfig = (EB.Sparx.PerformanceConfig)v;
        }

        static StackObject* AssignFromStack_PerformanceConfig_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.PerformanceConfig @PerformanceConfig = (EB.Sparx.PerformanceConfig)typeof(EB.Sparx.PerformanceConfig).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.Config)o).PerformanceConfig = @PerformanceConfig;
            return ptr_of_this_method;
        }

        static object get_GameComponents_9(ref object o)
        {
            return ((EB.Sparx.Config)o).GameComponents;
        }

        static StackObject* CopyToStack_GameComponents_9(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.Config)o).GameComponents;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GameComponents_9(ref object o, object v)
        {
            ((EB.Sparx.Config)o).GameComponents = (System.Collections.Generic.List<System.Type>)v;
        }

        static StackObject* AssignFromStack_GameComponents_9(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<System.Type> @GameComponents = (System.Collections.Generic.List<System.Type>)typeof(System.Collections.Generic.List<System.Type>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.Config)o).GameComponents = @GameComponents;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new EB.Sparx.Config();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
