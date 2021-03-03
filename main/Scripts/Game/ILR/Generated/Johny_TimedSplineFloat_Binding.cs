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
    unsafe class Johny_TimedSplineFloat_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Johny.TimedSplineFloat);
            args = new Type[]{typeof(System.Single), typeof(System.Single)};
            method = type.GetMethod("addKey", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, addKey_0);
            args = new Type[]{};
            method = type.GetMethod("CalculateGradient", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CalculateGradient_1);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("getGlobalFrame", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, getGlobalFrame_2);

            args = new Type[]{typeof(System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<System.Single, System.Single>>)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* addKey_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @key = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @time = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Johny.TimedSplineFloat instance_of_this_method = (Johny.TimedSplineFloat)typeof(Johny.TimedSplineFloat).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.addKey(@time, @key);

            return __ret;
        }

        static StackObject* CalculateGradient_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Johny.TimedSplineFloat instance_of_this_method = (Johny.TimedSplineFloat)typeof(Johny.TimedSplineFloat).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CalculateGradient();

            return __ret;
        }

        static StackObject* getGlobalFrame_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @t = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Johny.TimedSplineFloat instance_of_this_method = (Johny.TimedSplineFloat)typeof(Johny.TimedSplineFloat).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.getGlobalFrame(@t);

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<System.Single, System.Single>> @list = (System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<System.Single, System.Single>>)typeof(System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<System.Single, System.Single>>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = new Johny.TimedSplineFloat(@list);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
