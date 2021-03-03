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
    unsafe class EB_Sparx_PerformanceConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.PerformanceConfig);

            field = type.GetField("DataLoadedHandler", flag);
            app.RegisterCLRFieldGetter(field, get_DataLoadedHandler_0);
            app.RegisterCLRFieldSetter(field, set_DataLoadedHandler_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_DataLoadedHandler_0, AssignFromStack_DataLoadedHandler_0);
            field = type.GetField("GetPlatformHandler", flag);
            app.RegisterCLRFieldGetter(field, get_GetPlatformHandler_1);
            app.RegisterCLRFieldSetter(field, set_GetPlatformHandler_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_GetPlatformHandler_1, AssignFromStack_GetPlatformHandler_1);


        }



        static object get_DataLoadedHandler_0(ref object o)
        {
            return ((EB.Sparx.PerformanceConfig)o).DataLoadedHandler;
        }

        static StackObject* CopyToStack_DataLoadedHandler_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.PerformanceConfig)o).DataLoadedHandler;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_DataLoadedHandler_0(ref object o, object v)
        {
            ((EB.Sparx.PerformanceConfig)o).DataLoadedHandler = (EB.Sparx.PerformanceConfig.DataLoaded)v;
        }

        static StackObject* AssignFromStack_DataLoadedHandler_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.PerformanceConfig.DataLoaded @DataLoadedHandler = (EB.Sparx.PerformanceConfig.DataLoaded)typeof(EB.Sparx.PerformanceConfig.DataLoaded).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.PerformanceConfig)o).DataLoadedHandler = @DataLoadedHandler;
            return ptr_of_this_method;
        }

        static object get_GetPlatformHandler_1(ref object o)
        {
            return ((EB.Sparx.PerformanceConfig)o).GetPlatformHandler;
        }

        static StackObject* CopyToStack_GetPlatformHandler_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.PerformanceConfig)o).GetPlatformHandler;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GetPlatformHandler_1(ref object o, object v)
        {
            ((EB.Sparx.PerformanceConfig)o).GetPlatformHandler = (EB.Sparx.PerformanceConfig.GetPlatform)v;
        }

        static StackObject* AssignFromStack_GetPlatformHandler_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Sparx.PerformanceConfig.GetPlatform @GetPlatformHandler = (EB.Sparx.PerformanceConfig.GetPlatform)typeof(EB.Sparx.PerformanceConfig.GetPlatform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.PerformanceConfig)o).GetPlatformHandler = @GetPlatformHandler;
            return ptr_of_this_method;
        }



    }
}
