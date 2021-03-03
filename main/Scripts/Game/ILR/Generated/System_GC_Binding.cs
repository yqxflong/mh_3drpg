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
    unsafe class System_GC_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(System.GC);
            args = new Type[]{};
            method = type.GetMethod("get_MaxGeneration", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_MaxGeneration_0);
            args = new Type[]{typeof(System.Int32), typeof(System.GCCollectionMode)};
            method = type.GetMethod("Collect", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Collect_1);
            args = new Type[]{};
            method = type.GetMethod("WaitForPendingFinalizers", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WaitForPendingFinalizers_2);
            args = new Type[]{};
            method = type.GetMethod("Collect", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Collect_3);


        }


        static StackObject* get_MaxGeneration_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = System.GC.MaxGeneration;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* Collect_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.GCCollectionMode @mode = (System.GCCollectionMode)typeof(System.GCCollectionMode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @generation = ptr_of_this_method->Value;


            System.GC.Collect(@generation, @mode);

            return __ret;
        }

        static StackObject* WaitForPendingFinalizers_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            System.GC.WaitForPendingFinalizers();

            return __ret;
        }

        static StackObject* Collect_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            System.GC.Collect();

            return __ret;
        }



    }
}
