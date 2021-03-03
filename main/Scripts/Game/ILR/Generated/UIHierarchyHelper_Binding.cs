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
    unsafe class UIHierarchyHelper_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIHierarchyHelper);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);
            args = new Type[]{typeof(UnityEngine.GameObject), typeof(global::UIHierarchyHelper.eUIType), typeof(global::UIAnchor.Side)};
            method = type.GetMethod("Place", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Place_1);
            args = new Type[]{typeof(System.Boolean), typeof(System.Single)};
            method = type.GetMethod("ShowRegularHUD", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ShowRegularHUD_2);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("SetBlockPanel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetBlockPanel_3);

            field = type.GetField("MainUICamera", flag);
            app.RegisterCLRFieldGetter(field, get_MainUICamera_0);
            app.RegisterCLRFieldSetter(field, set_MainUICamera_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_MainUICamera_0, AssignFromStack_MainUICamera_0);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::UIHierarchyHelper.Instance;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Place_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIAnchor.Side @side = (global::UIAnchor.Side)typeof(global::UIAnchor.Side).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIHierarchyHelper.eUIType @type = (global::UIHierarchyHelper.eUIType)typeof(global::UIHierarchyHelper.eUIType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.GameObject @instance = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            global::UIHierarchyHelper instance_of_this_method = (global::UIHierarchyHelper)typeof(global::UIHierarchyHelper).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Place(@instance, @type, @side);

            return __ret;
        }

        static StackObject* ShowRegularHUD_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @alphaFadeTime = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @show = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIHierarchyHelper instance_of_this_method = (global::UIHierarchyHelper)typeof(global::UIHierarchyHelper).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ShowRegularHUD(@show, @alphaFadeTime);

            return __ret;
        }

        static StackObject* SetBlockPanel_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isPanelOn = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIHierarchyHelper instance_of_this_method = (global::UIHierarchyHelper)typeof(global::UIHierarchyHelper).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetBlockPanel(@isPanelOn);

            return __ret;
        }


        static object get_MainUICamera_0(ref object o)
        {
            return ((global::UIHierarchyHelper)o).MainUICamera;
        }

        static StackObject* CopyToStack_MainUICamera_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIHierarchyHelper)o).MainUICamera;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_MainUICamera_0(ref object o, object v)
        {
            ((global::UIHierarchyHelper)o).MainUICamera = (global::UICamera)v;
        }

        static StackObject* AssignFromStack_MainUICamera_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UICamera @MainUICamera = (global::UICamera)typeof(global::UICamera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIHierarchyHelper)o).MainUICamera = @MainUICamera;
            return ptr_of_this_method;
        }



    }
}
