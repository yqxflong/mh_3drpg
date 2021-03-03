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
    unsafe class UserData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::UserData);
            args = new Type[]{};
            method = type.GetMethod("get_Locale", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Locale_0);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_PlayerNum", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_PlayerNum_1);
            args = new Type[]{};
            method = type.GetMethod("SerializePrefs", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SerializePrefs_2);
            args = new Type[]{};
            method = type.GetMethod("get_PlayerNum", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_PlayerNum_3);
            args = new Type[]{};
            method = type.GetMethod("get_MusicVolume", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_MusicVolume_4);
            args = new Type[]{};
            method = type.GetMethod("get_SFXVolume", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_SFXVolume_5);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_MusicVolume", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_MusicVolume_6);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_SFXVolume", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_SFXVolume_7);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_UserQualitySet", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_UserQualitySet_8);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetUserQuality", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetUserQuality_9);
            args = new Type[]{typeof(EB.Language)};
            method = type.GetMethod("set_Locale", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Locale_10);


        }


        static StackObject* get_Locale_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::UserData.Locale;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_PlayerNum_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;


            global::UserData.PlayerNum = value;

            return __ret;
        }

        static StackObject* SerializePrefs_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::UserData.SerializePrefs();

            return __ret;
        }

        static StackObject* get_PlayerNum_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::UserData.PlayerNum;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_MusicVolume_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::UserData.MusicVolume;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_SFXVolume_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::UserData.SFXVolume;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_MusicVolume_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;


            global::UserData.MusicVolume = value;

            return __ret;
        }

        static StackObject* set_SFXVolume_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;


            global::UserData.SFXVolume = value;

            return __ret;
        }

        static StackObject* set_UserQualitySet_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::UserData.UserQualitySet = value;

            return __ret;
        }

        static StackObject* SetUserQuality_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index = ptr_of_this_method->Value;


            global::UserData.SetUserQuality(@index);

            return __ret;
        }

        static StackObject* set_Locale_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Language @value = (EB.Language)typeof(EB.Language).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::UserData.Locale = value;

            return __ret;
        }



    }
}
