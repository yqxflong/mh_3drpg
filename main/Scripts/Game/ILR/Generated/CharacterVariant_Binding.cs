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
    unsafe class CharacterVariant_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::CharacterVariant);
            args = new Type[]{};
            method = type.GetMethod("Recycle", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Recycle_0);
            args = new Type[]{};
            method = type.GetMethod("get_CharacterInstance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_CharacterInstance_1);
            args = new Type[]{};
            method = type.GetMethod("InstantiateCharacter", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InstantiateCharacter_2);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_SyncLoad", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_SyncLoad_3);
            args = new Type[]{typeof(System.Collections.IDictionary)};
            method = type.GetMethod("InstantiateCharacter", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InstantiateCharacter_4);


        }


        static StackObject* Recycle_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CharacterVariant instance_of_this_method = (global::CharacterVariant)typeof(global::CharacterVariant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Recycle();

            return __ret;
        }

        static StackObject* get_CharacterInstance_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CharacterVariant instance_of_this_method = (global::CharacterVariant)typeof(global::CharacterVariant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CharacterInstance;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* InstantiateCharacter_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CharacterVariant instance_of_this_method = (global::CharacterVariant)typeof(global::CharacterVariant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.InstantiateCharacter();

            return __ret;
        }

        static StackObject* set_SyncLoad_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CharacterVariant instance_of_this_method = (global::CharacterVariant)typeof(global::CharacterVariant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SyncLoad = value;

            return __ret;
        }

        static StackObject* InstantiateCharacter_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.IDictionary @partitions = (System.Collections.IDictionary)typeof(System.Collections.IDictionary).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CharacterVariant instance_of_this_method = (global::CharacterVariant)typeof(global::CharacterVariant).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.InstantiateCharacter(@partitions);

            return __ret;
        }



    }
}
