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
    unsafe class MyFollowCamera_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::MyFollowCamera);
            args = new Type[]{};
            method = type.GetMethod("ResetCameraView", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ResetCameraView_0);

            field = type.GetField("Instance", flag);
            app.RegisterCLRFieldGetter(field, get_Instance_0);
            app.RegisterCLRFieldSetter(field, set_Instance_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Instance_0, AssignFromStack_Instance_0);
            field = type.GetField("isActive", flag);
            app.RegisterCLRFieldGetter(field, get_isActive_1);
            app.RegisterCLRFieldSetter(field, set_isActive_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_isActive_1, AssignFromStack_isActive_1);
            field = type.GetField("delTouchDownInView", flag);
            app.RegisterCLRFieldGetter(field, get_delTouchDownInView_2);
            app.RegisterCLRFieldSetter(field, set_delTouchDownInView_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_delTouchDownInView_2, AssignFromStack_delTouchDownInView_2);
            field = type.GetField("delTouchCharacter", flag);
            app.RegisterCLRFieldGetter(field, get_delTouchCharacter_3);
            app.RegisterCLRFieldSetter(field, set_delTouchCharacter_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_delTouchCharacter_3, AssignFromStack_delTouchCharacter_3);
            field = type.GetField("CAMERA_ROTATIONAL_SPEED", flag);
            app.RegisterCLRFieldGetter(field, get_CAMERA_ROTATIONAL_SPEED_4);
            app.RegisterCLRFieldSetter(field, set_CAMERA_ROTATIONAL_SPEED_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_CAMERA_ROTATIONAL_SPEED_4, AssignFromStack_CAMERA_ROTATIONAL_SPEED_4);


        }


        static StackObject* ResetCameraView_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::MyFollowCamera instance_of_this_method = (global::MyFollowCamera)typeof(global::MyFollowCamera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ResetCameraView();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_Instance_0(ref object o)
        {
            return global::MyFollowCamera.Instance;
        }

        static StackObject* CopyToStack_Instance_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::MyFollowCamera.Instance;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Instance_0(ref object o, object v)
        {
            global::MyFollowCamera.Instance = (global::MyFollowCamera)v;
        }

        static StackObject* AssignFromStack_Instance_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::MyFollowCamera @Instance = (global::MyFollowCamera)typeof(global::MyFollowCamera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::MyFollowCamera.Instance = @Instance;
            return ptr_of_this_method;
        }

        static object get_isActive_1(ref object o)
        {
            return ((global::MyFollowCamera)o).isActive;
        }

        static StackObject* CopyToStack_isActive_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::MyFollowCamera)o).isActive;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_isActive_1(ref object o, object v)
        {
            ((global::MyFollowCamera)o).isActive = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_isActive_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @isActive = ptr_of_this_method->Value == 1;
            ((global::MyFollowCamera)o).isActive = @isActive;
            return ptr_of_this_method;
        }

        static object get_delTouchDownInView_2(ref object o)
        {
            return global::MyFollowCamera.delTouchDownInView;
        }

        static StackObject* CopyToStack_delTouchDownInView_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::MyFollowCamera.delTouchDownInView;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_delTouchDownInView_2(ref object o, object v)
        {
            global::MyFollowCamera.delTouchDownInView = (global::MyFollowCamera.TouchDownInView)v;
        }

        static StackObject* AssignFromStack_delTouchDownInView_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::MyFollowCamera.TouchDownInView @delTouchDownInView = (global::MyFollowCamera.TouchDownInView)typeof(global::MyFollowCamera.TouchDownInView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::MyFollowCamera.delTouchDownInView = @delTouchDownInView;
            return ptr_of_this_method;
        }

        static object get_delTouchCharacter_3(ref object o)
        {
            return global::MyFollowCamera.delTouchCharacter;
        }

        static StackObject* CopyToStack_delTouchCharacter_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::MyFollowCamera.delTouchCharacter;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_delTouchCharacter_3(ref object o, object v)
        {
            global::MyFollowCamera.delTouchCharacter = (global::MyFollowCamera.TouchCharacter)v;
        }

        static StackObject* AssignFromStack_delTouchCharacter_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::MyFollowCamera.TouchCharacter @delTouchCharacter = (global::MyFollowCamera.TouchCharacter)typeof(global::MyFollowCamera.TouchCharacter).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::MyFollowCamera.delTouchCharacter = @delTouchCharacter;
            return ptr_of_this_method;
        }

        static object get_CAMERA_ROTATIONAL_SPEED_4(ref object o)
        {
            return global::MyFollowCamera.CAMERA_ROTATIONAL_SPEED;
        }

        static StackObject* CopyToStack_CAMERA_ROTATIONAL_SPEED_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::MyFollowCamera.CAMERA_ROTATIONAL_SPEED;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_CAMERA_ROTATIONAL_SPEED_4(ref object o, object v)
        {
            global::MyFollowCamera.CAMERA_ROTATIONAL_SPEED = (System.Single)v;
        }

        static StackObject* AssignFromStack_CAMERA_ROTATIONAL_SPEED_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @CAMERA_ROTATIONAL_SPEED = *(float*)&ptr_of_this_method->Value;
            global::MyFollowCamera.CAMERA_ROTATIONAL_SPEED = @CAMERA_ROTATIONAL_SPEED;
            return ptr_of_this_method;
        }



    }
}
