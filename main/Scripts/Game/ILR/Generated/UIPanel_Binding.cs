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
    unsafe class UIPanel_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIPanel);
            args = new Type[]{typeof(global::UIPanel.PauseType), typeof(System.Boolean)};
            method = type.GetMethod("SetPause", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetPause_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_IsPause", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_IsPause_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_sortingOrder", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_sortingOrder_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_depth", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_depth_3);
            args = new Type[]{};
            method = type.GetMethod("SetDirty", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetDirty_4);
            args = new Type[]{};
            method = type.GetMethod("GetViewSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetViewSize_5);
            args = new Type[]{};
            method = type.GetMethod("get_sortingOrder", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_sortingOrder_6);
            args = new Type[]{typeof(UnityEngine.Vector2)};
            method = type.GetMethod("set_clipOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_clipOffset_7);
            args = new Type[]{};
            method = type.GetMethod("get_baseClipRegion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_baseClipRegion_8);
            args = new Type[]{};
            method = type.GetMethod("Refresh", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Refresh_9);
            args = new Type[]{};
            method = type.GetMethod("get_depth", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_depth_10);
            args = new Type[]{};
            method = type.GetMethod("get_clipOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_clipOffset_11);

            field = type.GetField("startingRenderQueue", flag);
            app.RegisterCLRFieldGetter(field, get_startingRenderQueue_0);
            app.RegisterCLRFieldSetter(field, set_startingRenderQueue_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_startingRenderQueue_0, AssignFromStack_startingRenderQueue_0);
            field = type.GetField("onClipMove", flag);
            app.RegisterCLRFieldGetter(field, get_onClipMove_1);
            app.RegisterCLRFieldSetter(field, set_onClipMove_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onClipMove_1, AssignFromStack_onClipMove_1);
            field = type.GetField("widgetsAreStatic", flag);
            app.RegisterCLRFieldGetter(field, get_widgetsAreStatic_2);
            app.RegisterCLRFieldSetter(field, set_widgetsAreStatic_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_widgetsAreStatic_2, AssignFromStack_widgetsAreStatic_2);


        }


        static StackObject* SetPause_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIPanel.PauseType @type = (global::UIPanel.PauseType)typeof(global::UIPanel.PauseType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetPause(@type, @value);

            return __ret;
        }

        static StackObject* set_IsPause_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.IsPause = value;

            return __ret;
        }

        static StackObject* set_sortingOrder_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.sortingOrder = value;

            return __ret;
        }

        static StackObject* set_depth_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.depth = value;

            return __ret;
        }

        static StackObject* SetDirty_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetDirty();

            return __ret;
        }

        static StackObject* GetViewSize_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetViewSize();

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static StackObject* get_sortingOrder_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.sortingOrder;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_clipOffset_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector2 @value = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @value, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @value = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.clipOffset = value;

            return __ret;
        }

        static StackObject* get_baseClipRegion_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.baseClipRegion;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Refresh_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Refresh();

            return __ret;
        }

        static StackObject* get_depth_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.depth;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_clipOffset_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIPanel instance_of_this_method = (global::UIPanel)typeof(global::UIPanel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.clipOffset;

            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }


        static object get_startingRenderQueue_0(ref object o)
        {
            return ((global::UIPanel)o).startingRenderQueue;
        }

        static StackObject* CopyToStack_startingRenderQueue_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIPanel)o).startingRenderQueue;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_startingRenderQueue_0(ref object o, object v)
        {
            ((global::UIPanel)o).startingRenderQueue = (System.Int32)v;
        }

        static StackObject* AssignFromStack_startingRenderQueue_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @startingRenderQueue = ptr_of_this_method->Value;
            ((global::UIPanel)o).startingRenderQueue = @startingRenderQueue;
            return ptr_of_this_method;
        }

        static object get_onClipMove_1(ref object o)
        {
            return ((global::UIPanel)o).onClipMove;
        }

        static StackObject* CopyToStack_onClipMove_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIPanel)o).onClipMove;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onClipMove_1(ref object o, object v)
        {
            ((global::UIPanel)o).onClipMove = (global::UIPanel.OnClippingMoved)v;
        }

        static StackObject* AssignFromStack_onClipMove_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIPanel.OnClippingMoved @onClipMove = (global::UIPanel.OnClippingMoved)typeof(global::UIPanel.OnClippingMoved).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIPanel)o).onClipMove = @onClipMove;
            return ptr_of_this_method;
        }

        static object get_widgetsAreStatic_2(ref object o)
        {
            return ((global::UIPanel)o).widgetsAreStatic;
        }

        static StackObject* CopyToStack_widgetsAreStatic_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIPanel)o).widgetsAreStatic;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_widgetsAreStatic_2(ref object o, object v)
        {
            ((global::UIPanel)o).widgetsAreStatic = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_widgetsAreStatic_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @widgetsAreStatic = ptr_of_this_method->Value == 1;
            ((global::UIPanel)o).widgetsAreStatic = @widgetsAreStatic;
            return ptr_of_this_method;
        }



    }
}
