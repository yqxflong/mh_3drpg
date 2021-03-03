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
    unsafe class UIController_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIController);
            args = new Type[]{};
            method = type.GetMethod("get_IsHudUI", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsHudUI_0);
            args = new Type[]{};
            method = type.GetMethod("IsFullscreen", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsFullscreen_1);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("DestroySelf", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DestroySelf_2);
            args = new Type[]{};
            method = type.GetMethod("Close", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Close_3);
            args = new Type[]{};
            method = type.GetMethod("get_IsTweenAlphaOnMainPanel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsTweenAlphaOnMainPanel_4);
            args = new Type[]{};
            method = type.GetMethod("ResetUIBlockerArgs", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ResetUIBlockerArgs_5);
            args = new Type[]{typeof(System.Object)};
            method = type.GetMethod("SetMenuData", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetMenuData_6);
            args = new Type[]{};
            method = type.GetMethod("PlayTween", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlayTween_7);
            args = new Type[]{};
            method = type.GetMethod("Queue", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Queue_8);
            args = new Type[]{};
            method = type.GetMethod("Open", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Open_9);
            args = new Type[]{};
            method = type.GetMethod("get_MCurrentViewName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_MCurrentViewName_10);
            args = new Type[]{};
            method = type.GetMethod("DestroyControllerForm", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DestroyControllerForm_11);
            args = new Type[]{typeof(global::UIController)};
            method = type.GetMethod("IsDestroyed", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsDestroyed_12);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_MCurrentViewName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_MCurrentViewName_13);
            args = new Type[]{};
            method = type.GetMethod("IsOpen", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsOpen_14);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Show", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Show_15);
            args = new Type[]{};
            method = type.GetMethod("OnCancelButtonClick", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnCancelButtonClick_16);
            args = new Type[]{};
            method = type.GetMethod("get_Visibility", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Visibility_17);

            field = type.GetField("WaitFrameForBoot", flag);
            app.RegisterCLRFieldGetter(field, get_WaitFrameForBoot_0);
            app.RegisterCLRFieldSetter(field, set_WaitFrameForBoot_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_WaitFrameForBoot_0, AssignFromStack_WaitFrameForBoot_0);
            field = type.GetField("HasPlayedTween", flag);
            app.RegisterCLRFieldGetter(field, get_HasPlayedTween_1);
            app.RegisterCLRFieldSetter(field, set_HasPlayedTween_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_HasPlayedTween_1, AssignFromStack_HasPlayedTween_1);
            field = type.GetField("WaitFrameForDisabled", flag);
            app.RegisterCLRFieldGetter(field, get_WaitFrameForDisabled_2);
            app.RegisterCLRFieldSetter(field, set_WaitFrameForDisabled_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_WaitFrameForDisabled_2, AssignFromStack_WaitFrameForDisabled_2);
            field = type.GetField("HasAnimatedFadeIn", flag);
            app.RegisterCLRFieldGetter(field, get_HasAnimatedFadeIn_3);
            app.RegisterCLRFieldSetter(field, set_HasAnimatedFadeIn_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_HasAnimatedFadeIn_3, AssignFromStack_HasAnimatedFadeIn_3);
            field = type.GetField("hudRoot", flag);
            app.RegisterCLRFieldGetter(field, get_hudRoot_4);
            app.RegisterCLRFieldSetter(field, set_hudRoot_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_hudRoot_4, AssignFromStack_hudRoot_4);
            field = type.GetField("mBlocker", flag);
            app.RegisterCLRFieldGetter(field, get_mBlocker_5);
            app.RegisterCLRFieldSetter(field, set_mBlocker_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_mBlocker_5, AssignFromStack_mBlocker_5);
            field = type.GetField("mCollider", flag);
            app.RegisterCLRFieldGetter(field, get_mCollider_6);
            app.RegisterCLRFieldSetter(field, set_mCollider_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_mCollider_6, AssignFromStack_mCollider_6);
            field = type.GetField("mTrigger", flag);
            app.RegisterCLRFieldGetter(field, get_mTrigger_7);
            app.RegisterCLRFieldSetter(field, set_mTrigger_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_mTrigger_7, AssignFromStack_mTrigger_7);
            field = type.GetField("backButton", flag);
            app.RegisterCLRFieldGetter(field, get_backButton_8);
            app.RegisterCLRFieldSetter(field, set_backButton_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_backButton_8, AssignFromStack_backButton_8);
            field = type.GetField("destroyHandler", flag);
            app.RegisterCLRFieldGetter(field, get_destroyHandler_9);
            app.RegisterCLRFieldSetter(field, set_destroyHandler_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_destroyHandler_9, AssignFromStack_destroyHandler_9);


        }


        static StackObject* get_IsHudUI_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsHudUI;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* IsFullscreen_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsFullscreen();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* DestroySelf_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isGc = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DestroySelf(@isGc);

            return __ret;
        }

        static StackObject* Close_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Close();

            return __ret;
        }

        static StackObject* get_IsTweenAlphaOnMainPanel_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsTweenAlphaOnMainPanel;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* ResetUIBlockerArgs_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ResetUIBlockerArgs();

            return __ret;
        }

        static StackObject* SetMenuData_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @param = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetMenuData(@param);

            return __ret;
        }

        static StackObject* PlayTween_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.PlayTween();

            return __ret;
        }

        static StackObject* Queue_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Queue();

            return __ret;
        }

        static StackObject* Open_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Open();

            return __ret;
        }

        static StackObject* get_MCurrentViewName_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.MCurrentViewName;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* DestroyControllerForm_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DestroyControllerForm();

            return __ret;
        }

        static StackObject* IsDestroyed_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController @ui = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::UIController.IsDestroyed(@ui);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* set_MCurrentViewName_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.MCurrentViewName = value;

            return __ret;
        }

        static StackObject* IsOpen_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsOpen();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* Show_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isShowing = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Show(@isShowing);

            return __ret;
        }

        static StackObject* OnCancelButtonClick_16(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnCancelButtonClick();

            return __ret;
        }

        static StackObject* get_Visibility_17(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIController instance_of_this_method = (global::UIController)typeof(global::UIController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Visibility;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_WaitFrameForBoot_0(ref object o)
        {
            return ((global::UIController)o).WaitFrameForBoot;
        }

        static StackObject* CopyToStack_WaitFrameForBoot_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIController)o).WaitFrameForBoot;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_WaitFrameForBoot_0(ref object o, object v)
        {
            ((global::UIController)o).WaitFrameForBoot = (UnityEngine.Vector2Int)v;
        }

        static StackObject* AssignFromStack_WaitFrameForBoot_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector2Int @WaitFrameForBoot = (UnityEngine.Vector2Int)typeof(UnityEngine.Vector2Int).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIController)o).WaitFrameForBoot = @WaitFrameForBoot;
            return ptr_of_this_method;
        }

        static object get_HasPlayedTween_1(ref object o)
        {
            return ((global::UIController)o).HasPlayedTween;
        }

        static StackObject* CopyToStack_HasPlayedTween_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIController)o).HasPlayedTween;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_HasPlayedTween_1(ref object o, object v)
        {
            ((global::UIController)o).HasPlayedTween = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_HasPlayedTween_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @HasPlayedTween = ptr_of_this_method->Value == 1;
            ((global::UIController)o).HasPlayedTween = @HasPlayedTween;
            return ptr_of_this_method;
        }

        static object get_WaitFrameForDisabled_2(ref object o)
        {
            return ((global::UIController)o).WaitFrameForDisabled;
        }

        static StackObject* CopyToStack_WaitFrameForDisabled_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIController)o).WaitFrameForDisabled;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_WaitFrameForDisabled_2(ref object o, object v)
        {
            ((global::UIController)o).WaitFrameForDisabled = (System.Int32)v;
        }

        static StackObject* AssignFromStack_WaitFrameForDisabled_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @WaitFrameForDisabled = ptr_of_this_method->Value;
            ((global::UIController)o).WaitFrameForDisabled = @WaitFrameForDisabled;
            return ptr_of_this_method;
        }

        static object get_HasAnimatedFadeIn_3(ref object o)
        {
            return ((global::UIController)o).HasAnimatedFadeIn;
        }

        static StackObject* CopyToStack_HasAnimatedFadeIn_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIController)o).HasAnimatedFadeIn;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_HasAnimatedFadeIn_3(ref object o, object v)
        {
            ((global::UIController)o).HasAnimatedFadeIn = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_HasAnimatedFadeIn_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @HasAnimatedFadeIn = ptr_of_this_method->Value == 1;
            ((global::UIController)o).HasAnimatedFadeIn = @HasAnimatedFadeIn;
            return ptr_of_this_method;
        }

        static object get_hudRoot_4(ref object o)
        {
            return ((global::UIController)o).hudRoot;
        }

        static StackObject* CopyToStack_hudRoot_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIController)o).hudRoot;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_hudRoot_4(ref object o, object v)
        {
            ((global::UIController)o).hudRoot = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_hudRoot_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @hudRoot = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIController)o).hudRoot = @hudRoot;
            return ptr_of_this_method;
        }

        static object get_mBlocker_5(ref object o)
        {
            return ((global::UIController)o).mBlocker;
        }

        static StackObject* CopyToStack_mBlocker_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIController)o).mBlocker;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mBlocker_5(ref object o, object v)
        {
            ((global::UIController)o).mBlocker = (UnityEngine.GameObject)v;
        }

        static StackObject* AssignFromStack_mBlocker_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.GameObject @mBlocker = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIController)o).mBlocker = @mBlocker;
            return ptr_of_this_method;
        }

        static object get_mCollider_6(ref object o)
        {
            return ((global::UIController)o).mCollider;
        }

        static StackObject* CopyToStack_mCollider_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIController)o).mCollider;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mCollider_6(ref object o, object v)
        {
            ((global::UIController)o).mCollider = (UnityEngine.BoxCollider)v;
        }

        static StackObject* AssignFromStack_mCollider_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.BoxCollider @mCollider = (UnityEngine.BoxCollider)typeof(UnityEngine.BoxCollider).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIController)o).mCollider = @mCollider;
            return ptr_of_this_method;
        }

        static object get_mTrigger_7(ref object o)
        {
            return ((global::UIController)o).mTrigger;
        }

        static StackObject* CopyToStack_mTrigger_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIController)o).mTrigger;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_mTrigger_7(ref object o, object v)
        {
            ((global::UIController)o).mTrigger = (global::ConsecutiveClickCoolTrigger)v;
        }

        static StackObject* AssignFromStack_mTrigger_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::ConsecutiveClickCoolTrigger @mTrigger = (global::ConsecutiveClickCoolTrigger)typeof(global::ConsecutiveClickCoolTrigger).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIController)o).mTrigger = @mTrigger;
            return ptr_of_this_method;
        }

        static object get_backButton_8(ref object o)
        {
            return ((global::UIController)o).backButton;
        }

        static StackObject* CopyToStack_backButton_8(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIController)o).backButton;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_backButton_8(ref object o, object v)
        {
            ((global::UIController)o).backButton = (global::UIButton)v;
        }

        static StackObject* AssignFromStack_backButton_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIButton @backButton = (global::UIButton)typeof(global::UIButton).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIController)o).backButton = @backButton;
            return ptr_of_this_method;
        }

        static object get_destroyHandler_9(ref object o)
        {
            return ((global::UIController)o).destroyHandler;
        }

        static StackObject* CopyToStack_destroyHandler_9(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIController)o).destroyHandler;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_destroyHandler_9(ref object o, object v)
        {
            ((global::UIController)o).destroyHandler = (System.Action<global::UIController>)v;
        }

        static StackObject* AssignFromStack_destroyHandler_9(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<global::UIController> @destroyHandler = (System.Action<global::UIController>)typeof(System.Action<global::UIController>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIController)o).destroyHandler = @destroyHandler;
            return ptr_of_this_method;
        }



    }
}
