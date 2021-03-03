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
    unsafe class Johny_Action_ActionAlphaChange_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Johny.Action.ActionAlphaChange);
            args = new Type[]{typeof(Johny.Action.ActionAlphaChange.FinishStatus)};
            method = type.GetMethod("Stop", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Stop_0);
            args = new Type[]{typeof(System.Action<Johny.Action.ActionAlphaChange.FinishStatus>)};
            method = type.GetMethod("SetFinishHandler", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetFinishHandler_1);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_ForwardAndReverseLoop", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_ForwardAndReverseLoop_2);

            args = new Type[]{typeof(System.Single), typeof(System.Single), typeof(UnityEngine.SpriteRenderer), typeof(System.Single), typeof(System.Single)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* Stop_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Johny.Action.ActionAlphaChange.FinishStatus @status = (Johny.Action.ActionAlphaChange.FinishStatus)typeof(Johny.Action.ActionAlphaChange.FinishStatus).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Johny.Action.ActionAlphaChange instance_of_this_method = (Johny.Action.ActionAlphaChange)typeof(Johny.Action.ActionAlphaChange).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Stop(@status);

            return __ret;
        }

        static StackObject* SetFinishHandler_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<Johny.Action.ActionAlphaChange.FinishStatus> @handler = (System.Action<Johny.Action.ActionAlphaChange.FinishStatus>)typeof(System.Action<Johny.Action.ActionAlphaChange.FinishStatus>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Johny.Action.ActionAlphaChange instance_of_this_method = (Johny.Action.ActionAlphaChange)typeof(Johny.Action.ActionAlphaChange).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetFinishHandler(@handler);

            return __ret;
        }

        static StackObject* set_ForwardAndReverseLoop_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Johny.Action.ActionAlphaChange instance_of_this_method = (Johny.Action.ActionAlphaChange)typeof(Johny.Action.ActionAlphaChange).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ForwardAndReverseLoop = value;

            return __ret;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @to = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @from = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.SpriteRenderer @cell = (UnityEngine.SpriteRenderer)typeof(UnityEngine.SpriteRenderer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Single @during = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.Single @delay = *(float*)&ptr_of_this_method->Value;


            var result_of_this_method = new Johny.Action.ActionAlphaChange(@delay, @during, @cell, @from, @to);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
