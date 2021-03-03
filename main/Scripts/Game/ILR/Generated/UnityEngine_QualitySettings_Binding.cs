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
    unsafe class UnityEngine_QualitySettings_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.QualitySettings);
            args = new Type[]{typeof(UnityEngine.BlendWeights)};
            method = type.GetMethod("set_blendWeights", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_blendWeights_0);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_antiAliasing", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_antiAliasing_1);
            args = new Type[]{typeof(UnityEngine.AnisotropicFiltering)};
            method = type.GetMethod("set_anisotropicFiltering", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_anisotropicFiltering_2);
            args = new Type[]{typeof(System.Int32), typeof(System.Boolean)};
            method = type.GetMethod("SetQualityLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetQualityLevel_3);
            args = new Type[]{};
            method = type.GetMethod("GetQualityLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetQualityLevel_4);


        }


        static StackObject* set_blendWeights_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.BlendWeights @value = (UnityEngine.BlendWeights)typeof(UnityEngine.BlendWeights).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            UnityEngine.QualitySettings.blendWeights = value;

            return __ret;
        }

        static StackObject* set_antiAliasing_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;


            UnityEngine.QualitySettings.antiAliasing = value;

            return __ret;
        }

        static StackObject* set_anisotropicFiltering_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.AnisotropicFiltering @value = (UnityEngine.AnisotropicFiltering)typeof(UnityEngine.AnisotropicFiltering).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            UnityEngine.QualitySettings.anisotropicFiltering = value;

            return __ret;
        }

        static StackObject* SetQualityLevel_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @applyExpensiveChanges = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @index = ptr_of_this_method->Value;


            UnityEngine.QualitySettings.SetQualityLevel(@index, @applyExpensiveChanges);

            return __ret;
        }

        static StackObject* GetQualityLevel_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = UnityEngine.QualitySettings.GetQualityLevel();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }



    }
}
