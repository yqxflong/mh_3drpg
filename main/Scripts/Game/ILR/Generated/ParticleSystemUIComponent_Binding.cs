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
    unsafe class ParticleSystemUIComponent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ParticleSystemUIComponent);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Play", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Play_0);
            args = new Type[]{};
            method = type.GetMethod("Stop", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Stop_1);

            field = type.GetField("panel", flag);
            app.RegisterCLRFieldGetter(field, get_panel_0);
            app.RegisterCLRFieldSetter(field, set_panel_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_panel_0, AssignFromStack_panel_0);
            field = type.GetField("sortingOrderOffset", flag);
            app.RegisterCLRFieldGetter(field, get_sortingOrderOffset_1);
            app.RegisterCLRFieldSetter(field, set_sortingOrderOffset_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_sortingOrderOffset_1, AssignFromStack_sortingOrderOffset_1);
            field = type.GetField("fx", flag);
            app.RegisterCLRFieldGetter(field, get_fx_2);
            app.RegisterCLRFieldSetter(field, set_fx_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_fx_2, AssignFromStack_fx_2);
            field = type.GetField("needFXScaleMode", flag);
            app.RegisterCLRFieldGetter(field, get_needFXScaleMode_3);
            app.RegisterCLRFieldSetter(field, set_needFXScaleMode_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_needFXScaleMode_3, AssignFromStack_needFXScaleMode_3);
            field = type.GetField("WaitFrame", flag);
            app.RegisterCLRFieldGetter(field, get_WaitFrame_4);
            app.RegisterCLRFieldSetter(field, set_WaitFrame_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_WaitFrame_4, AssignFromStack_WaitFrame_4);
            field = type.GetField("playOnEnable", flag);
            app.RegisterCLRFieldGetter(field, get_playOnEnable_5);
            app.RegisterCLRFieldSetter(field, set_playOnEnable_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_playOnEnable_5, AssignFromStack_playOnEnable_5);
            field = type.GetField("stopOnDisable", flag);
            app.RegisterCLRFieldGetter(field, get_stopOnDisable_6);
            app.RegisterCLRFieldSetter(field, set_stopOnDisable_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_stopOnDisable_6, AssignFromStack_stopOnDisable_6);
            field = type.GetField("playTime", flag);
            app.RegisterCLRFieldGetter(field, get_playTime_7);
            app.RegisterCLRFieldSetter(field, set_playTime_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_playTime_7, AssignFromStack_playTime_7);

            app.RegisterCLRCreateArrayInstance(type, s => new global::ParticleSystemUIComponent[s]);


        }


        static StackObject* Play_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @clear = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::ParticleSystemUIComponent instance_of_this_method = (global::ParticleSystemUIComponent)typeof(global::ParticleSystemUIComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Play(@clear);

            return __ret;
        }

        static StackObject* Stop_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::ParticleSystemUIComponent instance_of_this_method = (global::ParticleSystemUIComponent)typeof(global::ParticleSystemUIComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Stop();

            return __ret;
        }


        static object get_panel_0(ref object o)
        {
            return ((global::ParticleSystemUIComponent)o).panel;
        }

        static StackObject* CopyToStack_panel_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ParticleSystemUIComponent)o).panel;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_panel_0(ref object o, object v)
        {
            ((global::ParticleSystemUIComponent)o).panel = (global::UIPanel)v;
        }

        static StackObject* AssignFromStack_panel_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIPanel @panel = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::ParticleSystemUIComponent)o).panel = @panel;
            return ptr_of_this_method;
        }

        static object get_sortingOrderOffset_1(ref object o)
        {
            return ((global::ParticleSystemUIComponent)o).sortingOrderOffset;
        }

        static StackObject* CopyToStack_sortingOrderOffset_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ParticleSystemUIComponent)o).sortingOrderOffset;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_sortingOrderOffset_1(ref object o, object v)
        {
            ((global::ParticleSystemUIComponent)o).sortingOrderOffset = (System.Int32)v;
        }

        static StackObject* AssignFromStack_sortingOrderOffset_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @sortingOrderOffset = ptr_of_this_method->Value;
            ((global::ParticleSystemUIComponent)o).sortingOrderOffset = @sortingOrderOffset;
            return ptr_of_this_method;
        }

        static object get_fx_2(ref object o)
        {
            return ((global::ParticleSystemUIComponent)o).fx;
        }

        static StackObject* CopyToStack_fx_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ParticleSystemUIComponent)o).fx;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_fx_2(ref object o, object v)
        {
            ((global::ParticleSystemUIComponent)o).fx = (UnityEngine.ParticleSystem)v;
        }

        static StackObject* AssignFromStack_fx_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.ParticleSystem @fx = (UnityEngine.ParticleSystem)typeof(UnityEngine.ParticleSystem).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::ParticleSystemUIComponent)o).fx = @fx;
            return ptr_of_this_method;
        }

        static object get_needFXScaleMode_3(ref object o)
        {
            return ((global::ParticleSystemUIComponent)o).needFXScaleMode;
        }

        static StackObject* CopyToStack_needFXScaleMode_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ParticleSystemUIComponent)o).needFXScaleMode;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_needFXScaleMode_3(ref object o, object v)
        {
            ((global::ParticleSystemUIComponent)o).needFXScaleMode = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_needFXScaleMode_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @needFXScaleMode = ptr_of_this_method->Value == 1;
            ((global::ParticleSystemUIComponent)o).needFXScaleMode = @needFXScaleMode;
            return ptr_of_this_method;
        }

        static object get_WaitFrame_4(ref object o)
        {
            return ((global::ParticleSystemUIComponent)o).WaitFrame;
        }

        static StackObject* CopyToStack_WaitFrame_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ParticleSystemUIComponent)o).WaitFrame;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_WaitFrame_4(ref object o, object v)
        {
            ((global::ParticleSystemUIComponent)o).WaitFrame = (System.Int32)v;
        }

        static StackObject* AssignFromStack_WaitFrame_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @WaitFrame = ptr_of_this_method->Value;
            ((global::ParticleSystemUIComponent)o).WaitFrame = @WaitFrame;
            return ptr_of_this_method;
        }

        static object get_playOnEnable_5(ref object o)
        {
            return ((global::ParticleSystemUIComponent)o).playOnEnable;
        }

        static StackObject* CopyToStack_playOnEnable_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ParticleSystemUIComponent)o).playOnEnable;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_playOnEnable_5(ref object o, object v)
        {
            ((global::ParticleSystemUIComponent)o).playOnEnable = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_playOnEnable_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @playOnEnable = ptr_of_this_method->Value == 1;
            ((global::ParticleSystemUIComponent)o).playOnEnable = @playOnEnable;
            return ptr_of_this_method;
        }

        static object get_stopOnDisable_6(ref object o)
        {
            return ((global::ParticleSystemUIComponent)o).stopOnDisable;
        }

        static StackObject* CopyToStack_stopOnDisable_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ParticleSystemUIComponent)o).stopOnDisable;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_stopOnDisable_6(ref object o, object v)
        {
            ((global::ParticleSystemUIComponent)o).stopOnDisable = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_stopOnDisable_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @stopOnDisable = ptr_of_this_method->Value == 1;
            ((global::ParticleSystemUIComponent)o).stopOnDisable = @stopOnDisable;
            return ptr_of_this_method;
        }

        static object get_playTime_7(ref object o)
        {
            return ((global::ParticleSystemUIComponent)o).playTime;
        }

        static StackObject* CopyToStack_playTime_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ParticleSystemUIComponent)o).playTime;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_playTime_7(ref object o, object v)
        {
            ((global::ParticleSystemUIComponent)o).playTime = (System.Single)v;
        }

        static StackObject* AssignFromStack_playTime_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @playTime = *(float*)&ptr_of_this_method->Value;
            ((global::ParticleSystemUIComponent)o).playTime = @playTime;
            return ptr_of_this_method;
        }



    }
}
