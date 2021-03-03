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
    unsafe class UICamera_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UICamera);
            args = new Type[]{};
            method = type.GetMethod("get_lastEventPosition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_lastEventPosition_0);
            args = new Type[]{};
            method = type.GetMethod("get_mainCamera", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_mainCamera_1);
            args = new Type[]{};
            method = type.GetMethod("get_currentScheme", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_currentScheme_2);
            args = new Type[]{typeof(global::UICamera.ControlScheme)};
            method = type.GetMethod("set_currentScheme", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_currentScheme_3);
            args = new Type[]{typeof(global::UICamera.MouseOrTouch)};
            method = type.GetMethod("Raycast", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Raycast_4);
            args = new Type[]{};
            method = type.GetMethod("ProcessTouches", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ProcessTouches_5);
            args = new Type[]{};
            method = type.GetMethod("ProcessMouse", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ProcessMouse_6);

            field = type.GetField("currentCamera", flag);
            app.RegisterCLRFieldGetter(field, get_currentCamera_0);
            app.RegisterCLRFieldSetter(field, set_currentCamera_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_currentCamera_0, AssignFromStack_currentCamera_0);
            field = type.GetField("lastWorldPosition", flag);
            app.RegisterCLRFieldGetter(field, get_lastWorldPosition_1);
            app.RegisterCLRFieldSetter(field, set_lastWorldPosition_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_lastWorldPosition_1, AssignFromStack_lastWorldPosition_1);
            field = type.GetField("onDrag", flag);
            app.RegisterCLRFieldGetter(field, get_onDrag_2);
            app.RegisterCLRFieldSetter(field, set_onDrag_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDrag_2, AssignFromStack_onDrag_2);
            field = type.GetField("fallThrough", flag);
            app.RegisterCLRFieldGetter(field, get_fallThrough_3);
            app.RegisterCLRFieldSetter(field, set_fallThrough_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_fallThrough_3, AssignFromStack_fallThrough_3);
            field = type.GetField("useTouch", flag);
            app.RegisterCLRFieldGetter(field, get_useTouch_4);
            app.RegisterCLRFieldSetter(field, set_useTouch_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_useTouch_4, AssignFromStack_useTouch_4);
            field = type.GetField("useMouse", flag);
            app.RegisterCLRFieldGetter(field, get_useMouse_5);
            app.RegisterCLRFieldSetter(field, set_useMouse_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_useMouse_5, AssignFromStack_useMouse_5);


        }


        static StackObject* get_lastEventPosition_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::UICamera.lastEventPosition;

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* get_mainCamera_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::UICamera.mainCamera;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_currentScheme_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::UICamera.currentScheme;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_currentScheme_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UICamera.ControlScheme @value = (global::UICamera.ControlScheme)typeof(global::UICamera.ControlScheme).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::UICamera.currentScheme = value;

            return __ret;
        }

        static StackObject* Raycast_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UICamera.MouseOrTouch @touch = (global::UICamera.MouseOrTouch)typeof(global::UICamera.MouseOrTouch).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::UICamera.Raycast(@touch);

            return __ret;
        }

        static StackObject* ProcessTouches_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UICamera instance_of_this_method = (global::UICamera)typeof(global::UICamera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ProcessTouches();

            return __ret;
        }

        static StackObject* ProcessMouse_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UICamera instance_of_this_method = (global::UICamera)typeof(global::UICamera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ProcessMouse();

            return __ret;
        }


        static object get_currentCamera_0(ref object o)
        {
            return global::UICamera.currentCamera;
        }

        static StackObject* CopyToStack_currentCamera_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::UICamera.currentCamera;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_currentCamera_0(ref object o, object v)
        {
            global::UICamera.currentCamera = (UnityEngine.Camera)v;
        }

        static StackObject* AssignFromStack_currentCamera_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Camera @currentCamera = (UnityEngine.Camera)typeof(UnityEngine.Camera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::UICamera.currentCamera = @currentCamera;
            return ptr_of_this_method;
        }

        static object get_lastWorldPosition_1(ref object o)
        {
            return global::UICamera.lastWorldPosition;
        }

        static StackObject* CopyToStack_lastWorldPosition_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::UICamera.lastWorldPosition;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_lastWorldPosition_1(ref object o, object v)
        {
            global::UICamera.lastWorldPosition = (UnityEngine.Vector3)v;
        }

        static StackObject* AssignFromStack_lastWorldPosition_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector3 @lastWorldPosition = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @lastWorldPosition, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @lastWorldPosition = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            }
            global::UICamera.lastWorldPosition = @lastWorldPosition;
            return ptr_of_this_method;
        }

        static object get_onDrag_2(ref object o)
        {
            return global::UICamera.onDrag;
        }

        static StackObject* CopyToStack_onDrag_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::UICamera.onDrag;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDrag_2(ref object o, object v)
        {
            global::UICamera.onDrag = (global::UICamera.VectorDelegate)v;
        }

        static StackObject* AssignFromStack_onDrag_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UICamera.VectorDelegate @onDrag = (global::UICamera.VectorDelegate)typeof(global::UICamera.VectorDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::UICamera.onDrag = @onDrag;
            return ptr_of_this_method;
        }

        static object get_fallThrough_3(ref object o)
        {
            return global::UICamera.fallThrough;
        }

        static StackObject* CopyToStack_fallThrough_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::UICamera.fallThrough;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_fallThrough_3(ref object o, object v)
        {
            global::UICamera.fallThrough = (UnityEngine.GameObject)v;
        }

        static StackObject* AssignFromStack_fallThrough_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.GameObject @fallThrough = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::UICamera.fallThrough = @fallThrough;
            return ptr_of_this_method;
        }

        static object get_useTouch_4(ref object o)
        {
            return ((global::UICamera)o).useTouch;
        }

        static StackObject* CopyToStack_useTouch_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UICamera)o).useTouch;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_useTouch_4(ref object o, object v)
        {
            ((global::UICamera)o).useTouch = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_useTouch_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @useTouch = ptr_of_this_method->Value == 1;
            ((global::UICamera)o).useTouch = @useTouch;
            return ptr_of_this_method;
        }

        static object get_useMouse_5(ref object o)
        {
            return ((global::UICamera)o).useMouse;
        }

        static StackObject* CopyToStack_useMouse_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UICamera)o).useMouse;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_useMouse_5(ref object o, object v)
        {
            ((global::UICamera)o).useMouse = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_useMouse_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @useMouse = ptr_of_this_method->Value == 1;
            ((global::UICamera)o).useMouse = @useMouse;
            return ptr_of_this_method;
        }



    }
}
