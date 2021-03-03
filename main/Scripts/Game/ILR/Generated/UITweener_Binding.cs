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
    unsafe class UITweener_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UITweener);
            args = new Type[]{};
            method = type.GetMethod("ResetToBeginning", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ResetToBeginning_0);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_tweenFactor", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_tweenFactor_1);
            args = new Type[]{};
            method = type.GetMethod("PlayForward", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlayForward_2);
            args = new Type[]{};
            method = type.GetMethod("PlayReverse", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlayReverse_3);
            args = new Type[]{typeof(global::EventDelegate.Callback)};
            method = type.GetMethod("SetOnFinished", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetOnFinished_4);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Play", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Play_5);
            args = new Type[]{typeof(global::EventDelegate.Callback)};
            method = type.GetMethod("AddOnFinished", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddOnFinished_6);

            field = type.GetField("onFinished", flag);
            app.RegisterCLRFieldGetter(field, get_onFinished_0);
            app.RegisterCLRFieldSetter(field, set_onFinished_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onFinished_0, AssignFromStack_onFinished_0);
            field = type.GetField("duration", flag);
            app.RegisterCLRFieldGetter(field, get_duration_1);
            app.RegisterCLRFieldSetter(field, set_duration_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_duration_1, AssignFromStack_duration_1);
            field = type.GetField("delay", flag);
            app.RegisterCLRFieldGetter(field, get_delay_2);
            app.RegisterCLRFieldSetter(field, set_delay_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_delay_2, AssignFromStack_delay_2);
            field = type.GetField("ignoreTimeScale", flag);
            app.RegisterCLRFieldGetter(field, get_ignoreTimeScale_3);
            app.RegisterCLRFieldSetter(field, set_ignoreTimeScale_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_ignoreTimeScale_3, AssignFromStack_ignoreTimeScale_3);
            field = type.GetField("style", flag);
            app.RegisterCLRFieldGetter(field, get_style_4);
            app.RegisterCLRFieldSetter(field, set_style_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_style_4, AssignFromStack_style_4);

            app.RegisterCLRCreateArrayInstance(type, s => new global::UITweener[s]);


        }


        static StackObject* ResetToBeginning_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UITweener instance_of_this_method = (global::UITweener)typeof(global::UITweener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ResetToBeginning();

            return __ret;
        }

        static StackObject* set_tweenFactor_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UITweener instance_of_this_method = (global::UITweener)typeof(global::UITweener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.tweenFactor = value;

            return __ret;
        }

        static StackObject* PlayForward_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UITweener instance_of_this_method = (global::UITweener)typeof(global::UITweener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.PlayForward();

            return __ret;
        }

        static StackObject* PlayReverse_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UITweener instance_of_this_method = (global::UITweener)typeof(global::UITweener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.PlayReverse();

            return __ret;
        }

        static StackObject* SetOnFinished_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventDelegate.Callback @del = (global::EventDelegate.Callback)typeof(global::EventDelegate.Callback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UITweener instance_of_this_method = (global::UITweener)typeof(global::UITweener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetOnFinished(@del);

            return __ret;
        }

        static StackObject* Play_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @forward = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UITweener instance_of_this_method = (global::UITweener)typeof(global::UITweener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Play(@forward);

            return __ret;
        }

        static StackObject* AddOnFinished_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventDelegate.Callback @del = (global::EventDelegate.Callback)typeof(global::EventDelegate.Callback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UITweener instance_of_this_method = (global::UITweener)typeof(global::UITweener).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddOnFinished(@del);

            return __ret;
        }


        static object get_onFinished_0(ref object o)
        {
            return ((global::UITweener)o).onFinished;
        }

        static StackObject* CopyToStack_onFinished_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITweener)o).onFinished;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onFinished_0(ref object o, object v)
        {
            ((global::UITweener)o).onFinished = (System.Collections.Generic.List<global::EventDelegate>)v;
        }

        static StackObject* AssignFromStack_onFinished_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::EventDelegate> @onFinished = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UITweener)o).onFinished = @onFinished;
            return ptr_of_this_method;
        }

        static object get_duration_1(ref object o)
        {
            return ((global::UITweener)o).duration;
        }

        static StackObject* CopyToStack_duration_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITweener)o).duration;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_duration_1(ref object o, object v)
        {
            ((global::UITweener)o).duration = (System.Single)v;
        }

        static StackObject* AssignFromStack_duration_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @duration = *(float*)&ptr_of_this_method->Value;
            ((global::UITweener)o).duration = @duration;
            return ptr_of_this_method;
        }

        static object get_delay_2(ref object o)
        {
            return ((global::UITweener)o).delay;
        }

        static StackObject* CopyToStack_delay_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITweener)o).delay;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_delay_2(ref object o, object v)
        {
            ((global::UITweener)o).delay = (System.Single)v;
        }

        static StackObject* AssignFromStack_delay_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @delay = *(float*)&ptr_of_this_method->Value;
            ((global::UITweener)o).delay = @delay;
            return ptr_of_this_method;
        }

        static object get_ignoreTimeScale_3(ref object o)
        {
            return ((global::UITweener)o).ignoreTimeScale;
        }

        static StackObject* CopyToStack_ignoreTimeScale_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITweener)o).ignoreTimeScale;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_ignoreTimeScale_3(ref object o, object v)
        {
            ((global::UITweener)o).ignoreTimeScale = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_ignoreTimeScale_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @ignoreTimeScale = ptr_of_this_method->Value == 1;
            ((global::UITweener)o).ignoreTimeScale = @ignoreTimeScale;
            return ptr_of_this_method;
        }

        static object get_style_4(ref object o)
        {
            return ((global::UITweener)o).style;
        }

        static StackObject* CopyToStack_style_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITweener)o).style;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_style_4(ref object o, object v)
        {
            ((global::UITweener)o).style = (global::UITweener.Style)v;
        }

        static StackObject* AssignFromStack_style_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UITweener.Style @style = (global::UITweener.Style)typeof(global::UITweener.Style).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UITweener)o).style = @style;
            return ptr_of_this_method;
        }



    }
}
