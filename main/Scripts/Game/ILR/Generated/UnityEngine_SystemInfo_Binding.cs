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
    unsafe class UnityEngine_SystemInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.SystemInfo);
            args = new Type[]{};
            method = type.GetMethod("get_batteryLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_batteryLevel_0);
            args = new Type[]{};
            method = type.GetMethod("get_graphicsShaderLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_graphicsShaderLevel_1);
            args = new Type[]{};
            method = type.GetMethod("get_graphicsDeviceName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_graphicsDeviceName_2);
            args = new Type[]{};
            method = type.GetMethod("get_graphicsMemorySize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_graphicsMemorySize_3);
            args = new Type[]{};
            method = type.GetMethod("get_systemMemorySize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_systemMemorySize_4);
            args = new Type[]{};
            method = type.GetMethod("get_processorFrequency", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_processorFrequency_5);
            args = new Type[]{};
            method = type.GetMethod("get_processorType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_processorType_6);
            args = new Type[]{};
            method = type.GetMethod("get_graphicsDeviceType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_graphicsDeviceType_7);


        }


        static StackObject* get_batteryLevel_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.SystemInfo.batteryLevel;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_graphicsShaderLevel_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.SystemInfo.graphicsShaderLevel;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_graphicsDeviceName_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.SystemInfo.graphicsDeviceName;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_graphicsMemorySize_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.SystemInfo.graphicsMemorySize;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_systemMemorySize_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.SystemInfo.systemMemorySize;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_processorFrequency_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.SystemInfo.processorFrequency;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_processorType_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.SystemInfo.processorType;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_graphicsDeviceType_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.SystemInfo.graphicsDeviceType;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
