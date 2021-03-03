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
    unsafe class GameEngine_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::GameEngine);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);
            args = new Type[]{};
            method = type.GetMethod("get_IsTimeToRootScene", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsTimeToRootScene_1);
            args = new Type[]{};
            method = type.GetMethod("get_defaultLayer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_defaultLayer_2);
            args = new Type[]{};
            method = type.GetMethod("get_GameListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_GameListener_3);
            args = new Type[]{typeof(UnityEngine.GameObject)};
            method = type.GetMethod("SetHideColorTarget", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetHideColorTarget_4);
            args = new Type[]{};
            method = type.GetMethod("get_OutlineColor", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_OutlineColor_5);
            args = new Type[]{typeof(global::GameEngine.eOutlineColor)};
            method = type.GetMethod("SetOutlineColor", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetOutlineColor_6);
            args = new Type[]{};
            method = type.GetMethod("get_LoginListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_LoginListener_7);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("ShowRequiredUpdate", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ShowRequiredUpdate_8);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("SetStreamingAssetsBG", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetStreamingAssetsBG_9);
            args = new Type[]{};
            method = type.GetMethod("get_WalletListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_WalletListener_10);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_IsRunFromEnterGameBtn", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_IsRunFromEnterGameBtn_11);
            args = new Type[]{};
            method = type.GetMethod("get_transparentUI3DLayer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_transparentUI3DLayer_12);
            args = new Type[]{};
            method = type.GetMethod("get_ui3dLayer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ui3dLayer_13);
            args = new Type[]{};
            method = type.GetMethod("get_uiLayer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_uiLayer_14);
            args = new Type[]{};
            method = type.GetMethod("get_transparentFXLayer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_transparentFXLayer_15);
            args = new Type[]{};
            method = type.GetMethod("GetAuthAPIAddress", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetAuthAPIAddress_16);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_IsTimeToRootScene", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_IsTimeToRootScene_17);

            field = type.GetField("TextureDic", flag);
            app.RegisterCLRFieldGetter(field, get_TextureDic_0);
            app.RegisterCLRFieldSetter(field, set_TextureDic_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_TextureDic_0, AssignFromStack_TextureDic_0);
            field = type.GetField("IsResetUserData", flag);
            app.RegisterCLRFieldGetter(field, get_IsResetUserData_1);
            app.RegisterCLRFieldSetter(field, set_IsResetUserData_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_IsResetUserData_1, AssignFromStack_IsResetUserData_1);
            field = type.GetField("LoginBGPath", flag);
            app.RegisterCLRFieldGetter(field, get_LoginBGPath_2);
            app.RegisterCLRFieldSetter(field, set_LoginBGPath_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_LoginBGPath_2, AssignFromStack_LoginBGPath_2);
            field = type.GetField("BrandPath", flag);
            app.RegisterCLRFieldGetter(field, get_BrandPath_3);
            app.RegisterCLRFieldSetter(field, set_BrandPath_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_BrandPath_3, AssignFromStack_BrandPath_3);
            field = type.GetField("ApiServerAddress", flag);
            app.RegisterCLRFieldGetter(field, get_ApiServerAddress_4);
            app.RegisterCLRFieldSetter(field, set_ApiServerAddress_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_ApiServerAddress_4, AssignFromStack_ApiServerAddress_4);
            field = type.GetField("IsFTE", flag);
            app.RegisterCLRFieldGetter(field, get_IsFTE_5);
            app.RegisterCLRFieldSetter(field, set_IsFTE_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_IsFTE_5, AssignFromStack_IsFTE_5);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::GameEngine.Instance;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_IsTimeToRootScene_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsTimeToRootScene;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_defaultLayer_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.defaultLayer;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_GameListener_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GameListener;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetHideColorTarget_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject @inTarget = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetHideColorTarget(@inTarget);

            return __ret;
        }

        static StackObject* get_OutlineColor_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.OutlineColor;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetOutlineColor_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine.eOutlineColor @color = (global::GameEngine.eOutlineColor)typeof(global::GameEngine.eOutlineColor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetOutlineColor(@color);

            return __ret;
        }

        static StackObject* get_LoginListener_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.LoginListener;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ShowRequiredUpdate_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @storeUrl = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ShowRequiredUpdate(@storeUrl);

            return __ret;
        }

        static StackObject* SetStreamingAssetsBG_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @path = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SetStreamingAssetsBG(@path);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_WalletListener_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.WalletListener;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_IsRunFromEnterGameBtn_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.IsRunFromEnterGameBtn = value;

            return __ret;
        }

        static StackObject* get_transparentUI3DLayer_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.transparentUI3DLayer;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ui3dLayer_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ui3dLayer;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_uiLayer_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.uiLayer;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_transparentFXLayer_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.transparentFXLayer;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetAuthAPIAddress_16(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetAuthAPIAddress();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_IsTimeToRootScene_17(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::GameEngine instance_of_this_method = (global::GameEngine)typeof(global::GameEngine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.IsTimeToRootScene = value;

            return __ret;
        }


        static object get_TextureDic_0(ref object o)
        {
            return ((global::GameEngine)o).TextureDic;
        }

        static StackObject* CopyToStack_TextureDic_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::GameEngine)o).TextureDic;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_TextureDic_0(ref object o, object v)
        {
            ((global::GameEngine)o).TextureDic = (System.Collections.Generic.Dictionary<System.String, UnityEngine.Texture2D>)v;
        }

        static StackObject* AssignFromStack_TextureDic_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, UnityEngine.Texture2D> @TextureDic = (System.Collections.Generic.Dictionary<System.String, UnityEngine.Texture2D>)typeof(System.Collections.Generic.Dictionary<System.String, UnityEngine.Texture2D>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::GameEngine)o).TextureDic = @TextureDic;
            return ptr_of_this_method;
        }

        static object get_IsResetUserData_1(ref object o)
        {
            return ((global::GameEngine)o).IsResetUserData;
        }

        static StackObject* CopyToStack_IsResetUserData_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::GameEngine)o).IsResetUserData;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_IsResetUserData_1(ref object o, object v)
        {
            ((global::GameEngine)o).IsResetUserData = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_IsResetUserData_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @IsResetUserData = ptr_of_this_method->Value == 1;
            ((global::GameEngine)o).IsResetUserData = @IsResetUserData;
            return ptr_of_this_method;
        }

        static object get_LoginBGPath_2(ref object o)
        {
            return ((global::GameEngine)o).LoginBGPath;
        }

        static StackObject* CopyToStack_LoginBGPath_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::GameEngine)o).LoginBGPath;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LoginBGPath_2(ref object o, object v)
        {
            ((global::GameEngine)o).LoginBGPath = (System.String)v;
        }

        static StackObject* AssignFromStack_LoginBGPath_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @LoginBGPath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::GameEngine)o).LoginBGPath = @LoginBGPath;
            return ptr_of_this_method;
        }

        static object get_BrandPath_3(ref object o)
        {
            return ((global::GameEngine)o).BrandPath;
        }

        static StackObject* CopyToStack_BrandPath_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::GameEngine)o).BrandPath;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_BrandPath_3(ref object o, object v)
        {
            ((global::GameEngine)o).BrandPath = (System.String)v;
        }

        static StackObject* AssignFromStack_BrandPath_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @BrandPath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::GameEngine)o).BrandPath = @BrandPath;
            return ptr_of_this_method;
        }

        static object get_ApiServerAddress_4(ref object o)
        {
            return ((global::GameEngine)o).ApiServerAddress;
        }

        static StackObject* CopyToStack_ApiServerAddress_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::GameEngine)o).ApiServerAddress;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ApiServerAddress_4(ref object o, object v)
        {
            ((global::GameEngine)o).ApiServerAddress = (System.String)v;
        }

        static StackObject* AssignFromStack_ApiServerAddress_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @ApiServerAddress = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::GameEngine)o).ApiServerAddress = @ApiServerAddress;
            return ptr_of_this_method;
        }

        static object get_IsFTE_5(ref object o)
        {
            return ((global::GameEngine)o).IsFTE;
        }

        static StackObject* CopyToStack_IsFTE_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::GameEngine)o).IsFTE;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_IsFTE_5(ref object o, object v)
        {
            ((global::GameEngine)o).IsFTE = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_IsFTE_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @IsFTE = ptr_of_this_method->Value == 1;
            ((global::GameEngine)o).IsFTE = @IsFTE;
            return ptr_of_this_method;
        }



    }
}
