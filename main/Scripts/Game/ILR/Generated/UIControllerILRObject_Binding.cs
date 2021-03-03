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
    unsafe class UIControllerILRObject_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::UIControllerILRObject);
            args = new Type[]{};
            method = type.GetMethod("OnCancelButtonClick", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnCancelButtonClick_0);
            args = new Type[]{};
            method = type.GetMethod("IsFullscreen", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsFullscreen_1);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Show", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Show_2);
            args = new Type[]{};
            method = type.GetMethod("get_ShowUIBlocker", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ShowUIBlocker_3);
            args = new Type[]{};
            method = type.GetMethod("OnEnable", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnEnable_4);
            args = new Type[]{};
            method = type.GetMethod("OnDisable", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnDisable_5);
            args = new Type[]{};
            method = type.GetMethod("Awake", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Awake_6);
            args = new Type[]{};
            method = type.GetMethod("StartBootFlash", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StartBootFlash_7);
            args = new Type[]{};
            method = type.GetMethod("OnFocus", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnFocus_8);
            args = new Type[]{};
            method = type.GetMethod("get_Visibility", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Visibility_9);


        }


        static StackObject* OnCancelButtonClick_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIControllerILRObject instance_of_this_method = (global::UIControllerILRObject)typeof(global::UIControllerILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnCancelButtonClick();

            return __ret;
        }

        static StackObject* IsFullscreen_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIControllerILRObject instance_of_this_method = (global::UIControllerILRObject)typeof(global::UIControllerILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsFullscreen();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* Show_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isShowing = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIControllerILRObject instance_of_this_method = (global::UIControllerILRObject)typeof(global::UIControllerILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Show(@isShowing);

            return __ret;
        }

        static StackObject* get_ShowUIBlocker_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIControllerILRObject instance_of_this_method = (global::UIControllerILRObject)typeof(global::UIControllerILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ShowUIBlocker;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* OnEnable_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIControllerILRObject instance_of_this_method = (global::UIControllerILRObject)typeof(global::UIControllerILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnEnable();

            return __ret;
        }

        static StackObject* OnDisable_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIControllerILRObject instance_of_this_method = (global::UIControllerILRObject)typeof(global::UIControllerILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnDisable();

            return __ret;
        }

        static StackObject* Awake_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIControllerILRObject instance_of_this_method = (global::UIControllerILRObject)typeof(global::UIControllerILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Awake();

            return __ret;
        }

        static StackObject* StartBootFlash_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIControllerILRObject instance_of_this_method = (global::UIControllerILRObject)typeof(global::UIControllerILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StartBootFlash();

            return __ret;
        }

        static StackObject* OnFocus_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIControllerILRObject instance_of_this_method = (global::UIControllerILRObject)typeof(global::UIControllerILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnFocus();

            return __ret;
        }

        static StackObject* get_Visibility_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIControllerILRObject instance_of_this_method = (global::UIControllerILRObject)typeof(global::UIControllerILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Visibility;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }



    }
}
