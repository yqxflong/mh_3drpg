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
    unsafe class PlayerController_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::PlayerController);
            args = new Type[]{};
            method = type.GetMethod("get_playerUid", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_playerUid_0);
            args = new Type[]{typeof(EB.Sparx.Player)};
            method = type.GetMethod("set_ReplicationPlayer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_ReplicationPlayer_1);
            args = new Type[]{};
            method = type.GetMethod("get_ReplicationPlayer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ReplicationPlayer_2);
            args = new Type[]{typeof(global::eGender)};
            method = type.GetMethod("set_Gender", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Gender_3);
            args = new Type[]{};
            method = type.GetMethod("get_Gender", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Gender_4);
            args = new Type[]{};
            method = type.GetMethod("LocalPlayerDisableNavigation", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LocalPlayerDisableNavigation_5);
            args = new Type[]{};
            method = type.GetMethod("LocalPlayerEnableNavigation", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LocalPlayerEnableNavigation_6);
            args = new Type[]{typeof(System.Int64)};
            method = type.GetMethod("set_playerUid", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_playerUid_7);
            args = new Type[]{};
            method = type.GetMethod("get_IsLocal", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsLocal_8);
            args = new Type[]{};
            method = type.GetMethod("Destroy", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Destroy_9);

            field = type.GetField("_isLocalPlayer", flag);
            app.RegisterCLRFieldGetter(field, get__isLocalPlayer_0);
            app.RegisterCLRFieldSetter(field, set__isLocalPlayer_0);
            app.RegisterCLRFieldBinding(field, CopyToStack__isLocalPlayer_0, AssignFromStack__isLocalPlayer_0);
            field = type.GetField("onCollisionExit", flag);
            app.RegisterCLRFieldGetter(field, get_onCollisionExit_1);
            app.RegisterCLRFieldSetter(field, set_onCollisionExit_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onCollisionExit_1, AssignFromStack_onCollisionExit_1);
            field = type.GetField("CurNpcCollision", flag);
            app.RegisterCLRFieldGetter(field, get_CurNpcCollision_2);
            app.RegisterCLRFieldSetter(field, set_CurNpcCollision_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_CurNpcCollision_2, AssignFromStack_CurNpcCollision_2);
            field = type.GetField("onCollisionOpen", flag);
            app.RegisterCLRFieldGetter(field, get_onCollisionOpen_3);
            app.RegisterCLRFieldSetter(field, set_onCollisionOpen_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_onCollisionOpen_3, AssignFromStack_onCollisionOpen_3);


        }


        static StackObject* get_playerUid_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::PlayerController instance_of_this_method = (global::PlayerController)typeof(global::PlayerController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.playerUid;

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_ReplicationPlayer_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.Player @value = (EB.Sparx.Player)typeof(EB.Sparx.Player).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::PlayerController instance_of_this_method = (global::PlayerController)typeof(global::PlayerController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ReplicationPlayer = value;

            return __ret;
        }

        static StackObject* get_ReplicationPlayer_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::PlayerController instance_of_this_method = (global::PlayerController)typeof(global::PlayerController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReplicationPlayer;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_Gender_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::eGender @value = (global::eGender)typeof(global::eGender).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::PlayerController instance_of_this_method = (global::PlayerController)typeof(global::PlayerController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Gender = value;

            return __ret;
        }

        static StackObject* get_Gender_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::PlayerController instance_of_this_method = (global::PlayerController)typeof(global::PlayerController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Gender;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* LocalPlayerDisableNavigation_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::PlayerController.LocalPlayerDisableNavigation();

            return __ret;
        }

        static StackObject* LocalPlayerEnableNavigation_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::PlayerController.LocalPlayerEnableNavigation();

            return __ret;
        }

        static StackObject* set_playerUid_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 @value = *(long*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::PlayerController instance_of_this_method = (global::PlayerController)typeof(global::PlayerController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.playerUid = value;

            return __ret;
        }

        static StackObject* get_IsLocal_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::PlayerController instance_of_this_method = (global::PlayerController)typeof(global::PlayerController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsLocal;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* Destroy_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::PlayerController instance_of_this_method = (global::PlayerController)typeof(global::PlayerController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Destroy();

            return __ret;
        }


        static object get__isLocalPlayer_0(ref object o)
        {
            return ((global::PlayerController)o)._isLocalPlayer;
        }

        static StackObject* CopyToStack__isLocalPlayer_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PlayerController)o)._isLocalPlayer;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set__isLocalPlayer_0(ref object o, object v)
        {
            ((global::PlayerController)o)._isLocalPlayer = (System.Boolean)v;
        }

        static StackObject* AssignFromStack__isLocalPlayer_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @_isLocalPlayer = ptr_of_this_method->Value == 1;
            ((global::PlayerController)o)._isLocalPlayer = @_isLocalPlayer;
            return ptr_of_this_method;
        }

        static object get_onCollisionExit_1(ref object o)
        {
            return global::PlayerController.onCollisionExit;
        }

        static StackObject* CopyToStack_onCollisionExit_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::PlayerController.onCollisionExit;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onCollisionExit_1(ref object o, object v)
        {
            global::PlayerController.onCollisionExit = (System.Action<System.String>)v;
        }

        static StackObject* AssignFromStack_onCollisionExit_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.String> @onCollisionExit = (System.Action<System.String>)typeof(System.Action<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::PlayerController.onCollisionExit = @onCollisionExit;
            return ptr_of_this_method;
        }

        static object get_CurNpcCollision_2(ref object o)
        {
            return global::PlayerController.CurNpcCollision;
        }

        static StackObject* CopyToStack_CurNpcCollision_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::PlayerController.CurNpcCollision;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_CurNpcCollision_2(ref object o, object v)
        {
            global::PlayerController.CurNpcCollision = (UnityEngine.Collider)v;
        }

        static StackObject* AssignFromStack_CurNpcCollision_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Collider @CurNpcCollision = (UnityEngine.Collider)typeof(UnityEngine.Collider).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::PlayerController.CurNpcCollision = @CurNpcCollision;
            return ptr_of_this_method;
        }

        static object get_onCollisionOpen_3(ref object o)
        {
            return global::PlayerController.onCollisionOpen;
        }

        static StackObject* CopyToStack_onCollisionOpen_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::PlayerController.onCollisionOpen;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onCollisionOpen_3(ref object o, object v)
        {
            global::PlayerController.onCollisionOpen = (System.Action<System.Collections.Hashtable>)v;
        }

        static StackObject* AssignFromStack_onCollisionOpen_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<System.Collections.Hashtable> @onCollisionOpen = (System.Action<System.Collections.Hashtable>)typeof(System.Action<System.Collections.Hashtable>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::PlayerController.onCollisionOpen = @onCollisionOpen;
            return ptr_of_this_method;
        }



    }
}
