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
    unsafe class UIRect_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIRect);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_alpha", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_alpha_0);
            args = new Type[]{};
            method = type.GetMethod("get_alpha", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_alpha_1);
            args = new Type[]{};
            method = type.GetMethod("ResetAnchors", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ResetAnchors_2);
            args = new Type[]{};
            method = type.GetMethod("UpdateAnchors", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UpdateAnchors_3);
            args = new Type[]{typeof(System.Single), typeof(System.Single), typeof(System.Single), typeof(System.Single)};
            method = type.GetMethod("SetRect", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetRect_4);
            args = new Type[]{};
            method = type.GetMethod("Update", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Update_5);
            args = new Type[]{};
            method = type.GetMethod("EmptyingAnchors", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EmptyingAnchors_6);
            args = new Type[]{typeof(UnityEngine.GameObject), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32)};
            method = type.GetMethod("SetAnchor", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetAnchor_7);

            field = type.GetField("leftAnchor", flag);
            app.RegisterCLRFieldGetter(field, get_leftAnchor_0);
            app.RegisterCLRFieldSetter(field, set_leftAnchor_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_leftAnchor_0, AssignFromStack_leftAnchor_0);
            field = type.GetField("updateAnchors", flag);
            app.RegisterCLRFieldGetter(field, get_updateAnchors_1);
            app.RegisterCLRFieldSetter(field, set_updateAnchors_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_updateAnchors_1, AssignFromStack_updateAnchors_1);
            field = type.GetField("rightAnchor", flag);
            app.RegisterCLRFieldGetter(field, get_rightAnchor_2);
            app.RegisterCLRFieldSetter(field, set_rightAnchor_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_rightAnchor_2, AssignFromStack_rightAnchor_2);
            field = type.GetField("bottomAnchor", flag);
            app.RegisterCLRFieldGetter(field, get_bottomAnchor_3);
            app.RegisterCLRFieldSetter(field, set_bottomAnchor_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_bottomAnchor_3, AssignFromStack_bottomAnchor_3);
            field = type.GetField("topAnchor", flag);
            app.RegisterCLRFieldGetter(field, get_topAnchor_4);
            app.RegisterCLRFieldSetter(field, set_topAnchor_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_topAnchor_4, AssignFromStack_topAnchor_4);


        }


        static StackObject* set_alpha_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIRect instance_of_this_method = (global::UIRect)typeof(global::UIRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.alpha = value;

            return __ret;
        }

        static StackObject* get_alpha_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIRect instance_of_this_method = (global::UIRect)typeof(global::UIRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.alpha;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ResetAnchors_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIRect instance_of_this_method = (global::UIRect)typeof(global::UIRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ResetAnchors();

            return __ret;
        }

        static StackObject* UpdateAnchors_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIRect instance_of_this_method = (global::UIRect)typeof(global::UIRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.UpdateAnchors();

            return __ret;
        }

        static StackObject* SetRect_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @height = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @width = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @y = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Single @x = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            global::UIRect instance_of_this_method = (global::UIRect)typeof(global::UIRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetRect(@x, @y, @width, @height);

            return __ret;
        }

        static StackObject* Update_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIRect instance_of_this_method = (global::UIRect)typeof(global::UIRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Update();

            return __ret;
        }

        static StackObject* EmptyingAnchors_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIRect instance_of_this_method = (global::UIRect)typeof(global::UIRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.EmptyingAnchors();

            return __ret;
        }

        static StackObject* SetAnchor_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 6);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @top = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @right = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @bottom = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Int32 @left = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            UnityEngine.GameObject @go = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            global::UIRect instance_of_this_method = (global::UIRect)typeof(global::UIRect).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetAnchor(@go, @left, @bottom, @right, @top);

            return __ret;
        }


        static object get_leftAnchor_0(ref object o)
        {
            return ((global::UIRect)o).leftAnchor;
        }

        static StackObject* CopyToStack_leftAnchor_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIRect)o).leftAnchor;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_leftAnchor_0(ref object o, object v)
        {
            ((global::UIRect)o).leftAnchor = (global::UIRect.AnchorPoint)v;
        }

        static StackObject* AssignFromStack_leftAnchor_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIRect.AnchorPoint @leftAnchor = (global::UIRect.AnchorPoint)typeof(global::UIRect.AnchorPoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIRect)o).leftAnchor = @leftAnchor;
            return ptr_of_this_method;
        }

        static object get_updateAnchors_1(ref object o)
        {
            return ((global::UIRect)o).updateAnchors;
        }

        static StackObject* CopyToStack_updateAnchors_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIRect)o).updateAnchors;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_updateAnchors_1(ref object o, object v)
        {
            ((global::UIRect)o).updateAnchors = (global::UIRect.AnchorUpdate)v;
        }

        static StackObject* AssignFromStack_updateAnchors_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIRect.AnchorUpdate @updateAnchors = (global::UIRect.AnchorUpdate)typeof(global::UIRect.AnchorUpdate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIRect)o).updateAnchors = @updateAnchors;
            return ptr_of_this_method;
        }

        static object get_rightAnchor_2(ref object o)
        {
            return ((global::UIRect)o).rightAnchor;
        }

        static StackObject* CopyToStack_rightAnchor_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIRect)o).rightAnchor;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_rightAnchor_2(ref object o, object v)
        {
            ((global::UIRect)o).rightAnchor = (global::UIRect.AnchorPoint)v;
        }

        static StackObject* AssignFromStack_rightAnchor_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIRect.AnchorPoint @rightAnchor = (global::UIRect.AnchorPoint)typeof(global::UIRect.AnchorPoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIRect)o).rightAnchor = @rightAnchor;
            return ptr_of_this_method;
        }

        static object get_bottomAnchor_3(ref object o)
        {
            return ((global::UIRect)o).bottomAnchor;
        }

        static StackObject* CopyToStack_bottomAnchor_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIRect)o).bottomAnchor;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_bottomAnchor_3(ref object o, object v)
        {
            ((global::UIRect)o).bottomAnchor = (global::UIRect.AnchorPoint)v;
        }

        static StackObject* AssignFromStack_bottomAnchor_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIRect.AnchorPoint @bottomAnchor = (global::UIRect.AnchorPoint)typeof(global::UIRect.AnchorPoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIRect)o).bottomAnchor = @bottomAnchor;
            return ptr_of_this_method;
        }

        static object get_topAnchor_4(ref object o)
        {
            return ((global::UIRect)o).topAnchor;
        }

        static StackObject* CopyToStack_topAnchor_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIRect)o).topAnchor;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_topAnchor_4(ref object o, object v)
        {
            ((global::UIRect)o).topAnchor = (global::UIRect.AnchorPoint)v;
        }

        static StackObject* AssignFromStack_topAnchor_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIRect.AnchorPoint @topAnchor = (global::UIRect.AnchorPoint)typeof(global::UIRect.AnchorPoint).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIRect)o).topAnchor = @topAnchor;
            return ptr_of_this_method;
        }



    }
}
