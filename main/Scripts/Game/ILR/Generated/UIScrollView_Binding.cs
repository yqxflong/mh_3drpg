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
    unsafe class UIScrollView_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIScrollView);
            args = new Type[]{};
            method = type.GetMethod("ResetPosition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ResetPosition_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("RestrictWithinBounds", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RestrictWithinBounds_1);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("MoveRelative", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MoveRelative_2);
            args = new Type[]{};
            method = type.GetMethod("get_panel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_panel_3);
            args = new Type[]{};
            method = type.GetMethod("InvalidateBounds", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InvalidateBounds_4);
            args = new Type[]{};
            method = type.GetMethod("get_canMoveVertically", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_canMoveVertically_5);
            args = new Type[]{typeof(System.Single), typeof(System.Single), typeof(System.Boolean)};
            method = type.GetMethod("SetDragAmount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetDragAmount_6);
            args = new Type[]{};
            method = type.GetMethod("UpdatePosition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UpdatePosition_7);
            args = new Type[]{};
            method = type.GetMethod("UpdateScrollbars", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UpdateScrollbars_8);
            args = new Type[]{};
            method = type.GetMethod("get_shouldMoveVertically", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_shouldMoveVertically_9);

            field = type.GetField("verticalScrollBar", flag);
            app.RegisterCLRFieldGetter(field, get_verticalScrollBar_0);
            app.RegisterCLRFieldSetter(field, set_verticalScrollBar_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_verticalScrollBar_0, AssignFromStack_verticalScrollBar_0);
            field = type.GetField("horizontalScrollBar", flag);
            app.RegisterCLRFieldGetter(field, get_horizontalScrollBar_1);
            app.RegisterCLRFieldSetter(field, set_horizontalScrollBar_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_horizontalScrollBar_1, AssignFromStack_horizontalScrollBar_1);


        }


        static StackObject* ResetPosition_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIScrollView instance_of_this_method = (global::UIScrollView)typeof(global::UIScrollView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ResetPosition();

            return __ret;
        }

        static StackObject* RestrictWithinBounds_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @instant = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIScrollView instance_of_this_method = (global::UIScrollView)typeof(global::UIScrollView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.RestrictWithinBounds(@instant);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* MoveRelative_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @relative = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @relative, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @relative = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIScrollView instance_of_this_method = (global::UIScrollView)typeof(global::UIScrollView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.MoveRelative(@relative);

            return __ret;
        }

        static StackObject* get_panel_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIScrollView instance_of_this_method = (global::UIScrollView)typeof(global::UIScrollView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.panel;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* InvalidateBounds_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIScrollView instance_of_this_method = (global::UIScrollView)typeof(global::UIScrollView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.InvalidateBounds();

            return __ret;
        }

        static StackObject* get_canMoveVertically_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIScrollView instance_of_this_method = (global::UIScrollView)typeof(global::UIScrollView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.canMoveVertically;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* SetDragAmount_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @updateScrollbars = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @y = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @x = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            global::UIScrollView instance_of_this_method = (global::UIScrollView)typeof(global::UIScrollView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetDragAmount(@x, @y, @updateScrollbars);

            return __ret;
        }

        static StackObject* UpdatePosition_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIScrollView instance_of_this_method = (global::UIScrollView)typeof(global::UIScrollView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.UpdatePosition();

            return __ret;
        }

        static StackObject* UpdateScrollbars_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIScrollView instance_of_this_method = (global::UIScrollView)typeof(global::UIScrollView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.UpdateScrollbars();

            return __ret;
        }

        static StackObject* get_shouldMoveVertically_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIScrollView instance_of_this_method = (global::UIScrollView)typeof(global::UIScrollView).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.shouldMoveVertically;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_verticalScrollBar_0(ref object o)
        {
            return ((global::UIScrollView)o).verticalScrollBar;
        }

        static StackObject* CopyToStack_verticalScrollBar_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIScrollView)o).verticalScrollBar;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_verticalScrollBar_0(ref object o, object v)
        {
            ((global::UIScrollView)o).verticalScrollBar = (global::UIProgressBar)v;
        }

        static StackObject* AssignFromStack_verticalScrollBar_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIProgressBar @verticalScrollBar = (global::UIProgressBar)typeof(global::UIProgressBar).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIScrollView)o).verticalScrollBar = @verticalScrollBar;
            return ptr_of_this_method;
        }

        static object get_horizontalScrollBar_1(ref object o)
        {
            return ((global::UIScrollView)o).horizontalScrollBar;
        }

        static StackObject* CopyToStack_horizontalScrollBar_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIScrollView)o).horizontalScrollBar;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_horizontalScrollBar_1(ref object o, object v)
        {
            ((global::UIScrollView)o).horizontalScrollBar = (global::UIProgressBar)v;
        }

        static StackObject* AssignFromStack_horizontalScrollBar_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIProgressBar @horizontalScrollBar = (global::UIProgressBar)typeof(global::UIProgressBar).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIScrollView)o).horizontalScrollBar = @horizontalScrollBar;
            return ptr_of_this_method;
        }



    }
}
