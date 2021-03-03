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
    unsafe class Umeng_GA_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Umeng.GA);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("ProfileSignIn", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ProfileSignIn_0);
            args = new Type[]{};
            method = type.GetMethod("ProfileSignOff", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ProfileSignOff_1);
            args = new Type[]{typeof(System.Double), typeof(Umeng.GA.PaySource), typeof(System.Double)};
            method = type.GetMethod("Pay", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Pay_2);
            args = new Type[]{typeof(System.String), typeof(System.Int32), typeof(System.Double)};
            method = type.GetMethod("Buy", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Buy_3);
            args = new Type[]{typeof(System.String), typeof(System.Int32), typeof(System.Double)};
            method = type.GetMethod("Use", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Use_4);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("StartLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StartLevel_5);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("FinishLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, FinishLevel_6);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("FailLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, FailLevel_7);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetUserLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetUserLevel_8);
            args = new Type[]{typeof(System.Double), typeof(Umeng.GA.BonusSource)};
            method = type.GetMethod("Bonus", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Bonus_9);


        }


        static StackObject* ProfileSignIn_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @userId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Umeng.GA.ProfileSignIn(@userId);

            return __ret;
        }

        static StackObject* ProfileSignOff_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            Umeng.GA.ProfileSignOff();

            return __ret;
        }

        static StackObject* Pay_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Double @coin = *(double*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Umeng.GA.PaySource @source = (Umeng.GA.PaySource)typeof(Umeng.GA.PaySource).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Double @cash = *(double*)&ptr_of_this_method->Value;


            Umeng.GA.Pay(@cash, @source, @coin);

            return __ret;
        }

        static StackObject* Buy_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Double @price = *(double*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @amount = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @item = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Umeng.GA.Buy(@item, @amount, @price);

            return __ret;
        }

        static StackObject* Use_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Double @price = *(double*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @amount = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @item = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Umeng.GA.Use(@item, @amount, @price);

            return __ret;
        }

        static StackObject* StartLevel_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @level = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Umeng.GA.StartLevel(@level);

            return __ret;
        }

        static StackObject* FinishLevel_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @level = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Umeng.GA.FinishLevel(@level);

            return __ret;
        }

        static StackObject* FailLevel_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @level = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Umeng.GA.FailLevel(@level);

            return __ret;
        }

        static StackObject* SetUserLevel_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @level = ptr_of_this_method->Value;


            Umeng.GA.SetUserLevel(@level);

            return __ret;
        }

        static StackObject* Bonus_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Umeng.GA.BonusSource @source = (Umeng.GA.BonusSource)typeof(Umeng.GA.BonusSource).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Double @coin = *(double*)&ptr_of_this_method->Value;


            Umeng.GA.Bonus(@coin, @source);

            return __ret;
        }



    }
}
