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
    unsafe class PlayerManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::PlayerManager);
            args = new Type[]{typeof(global::PlayerController)};
            method = type.GetMethod("RegisterPlayerController", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterPlayerController_0);
            args = new Type[]{typeof(UnityEngine.GameObject)};
            method = type.GetMethod("IsLocalPlayer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsLocalPlayer_1);
            args = new Type[]{typeof(global::PlayerController)};
            method = type.GetMethod("UnregisterPlayerController", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UnregisterPlayerController_2);
            args = new Type[]{};
            method = type.GetMethod("LocalPlayerController", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LocalPlayerController_3);
            args = new Type[]{};
            method = type.GetMethod("LocalPlayerGameObject", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LocalPlayerGameObject_4);
            args = new Type[]{};
            method = type.GetMethod("UnregsiterLocalPlayer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UnregsiterLocalPlayer_5);
            args = new Type[]{};
            method = type.GetMethod("DestroyPlayerControllers", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DestroyPlayerControllers_6);
            args = new Type[]{typeof(System.Int64)};
            method = type.GetMethod("GetPlayerController", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetPlayerController_7);
            args = new Type[]{};
            method = type.GetMethod("get_sPlayerControllers", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_sPlayerControllers_8);


        }


        static StackObject* RegisterPlayerController_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::PlayerController @player = (global::PlayerController)typeof(global::PlayerController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::PlayerManager.RegisterPlayerController(@player);

            return __ret;
        }

        static StackObject* IsLocalPlayer_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject @player = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::PlayerManager.IsLocalPlayer(@player);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* UnregisterPlayerController_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::PlayerController @player = (global::PlayerController)typeof(global::PlayerController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::PlayerManager.UnregisterPlayerController(@player);

            return __ret;
        }

        static StackObject* LocalPlayerController_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::PlayerManager.LocalPlayerController();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* LocalPlayerGameObject_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::PlayerManager.LocalPlayerGameObject();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* UnregsiterLocalPlayer_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::PlayerManager.UnregsiterLocalPlayer();

            return __ret;
        }

        static StackObject* DestroyPlayerControllers_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::PlayerManager.DestroyPlayerControllers();

            return __ret;
        }

        static StackObject* GetPlayerController_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 @userid = *(long*)&ptr_of_this_method->Value;


            var result_of_this_method = global::PlayerManager.GetPlayerController(@userid);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_sPlayerControllers_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::PlayerManager.sPlayerControllers;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
