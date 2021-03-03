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
    unsafe class EB_Replication_Manager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Replication.Manager);
            args = new Type[]{};
            method = type.GetMethod("get_IsLocalGame", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsLocalGame_0);
            args = new Type[]{};
            method = type.GetMethod("get_LocalPlayerIndex", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_LocalPlayerIndex_1);
            args = new Type[]{typeof(System.String), typeof(System.Delegate)};
            method = type.GetMethod("RegisterRPC", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterRPC_2);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("RegisterResource", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterResource_3);
            args = new Type[]{typeof(System.Type)};
            method = type.GetMethod("RegisterSerializable", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterSerializable_4);
            args = new Type[]{typeof(UnityEngine.Object), typeof(UnityEngine.Vector3), typeof(UnityEngine.Quaternion), typeof(System.String)};
            method = type.GetMethod("Instantiate", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Instantiate_5);

            field = type.GetField("InstantiateMethod", flag);
            app.RegisterCLRFieldGetter(field, get_InstantiateMethod_0);
            app.RegisterCLRFieldSetter(field, set_InstantiateMethod_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_InstantiateMethod_0, AssignFromStack_InstantiateMethod_0);


        }


        static StackObject* get_IsLocalGame_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = EB.Replication.Manager.IsLocalGame;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_LocalPlayerIndex_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = EB.Replication.Manager.LocalPlayerIndex;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* RegisterRPC_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Delegate @function = (System.Delegate)typeof(System.Delegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @name = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            EB.Replication.Manager.RegisterRPC(@name, @function);

            return __ret;
        }

        static StackObject* RegisterResource_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @path = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            EB.Replication.Manager.RegisterResource(@path);

            return __ret;
        }

        static StackObject* RegisterSerializable_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Type @type = (System.Type)typeof(System.Type).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            EB.Replication.Manager.RegisterSerializable(@type);

            return __ret;
        }

        static StackObject* Instantiate_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @pathName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Quaternion @rotation = new UnityEngine.Quaternion();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Quaternion_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Quaternion_Binding_Binder.ParseValue(ref @rotation, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @rotation = (UnityEngine.Quaternion)typeof(UnityEngine.Quaternion).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.Vector3 @position = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @position, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @position = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            UnityEngine.Object @prefab = (UnityEngine.Object)typeof(UnityEngine.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = EB.Replication.Manager.Instantiate(@prefab, @position, @rotation, @pathName);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_InstantiateMethod_0(ref object o)
        {
            return EB.Replication.Manager.InstantiateMethod;
        }

        static StackObject* CopyToStack_InstantiateMethod_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = EB.Replication.Manager.InstantiateMethod;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_InstantiateMethod_0(ref object o, object v)
        {
            EB.Replication.Manager.InstantiateMethod = (EB.Replication.Manager.InstantiateMethodDelegate)v;
        }

        static StackObject* AssignFromStack_InstantiateMethod_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            EB.Replication.Manager.InstantiateMethodDelegate @InstantiateMethod = (EB.Replication.Manager.InstantiateMethodDelegate)typeof(EB.Replication.Manager.InstantiateMethodDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            EB.Replication.Manager.InstantiateMethod = @InstantiateMethod;
            return ptr_of_this_method;
        }



    }
}
