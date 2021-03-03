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
    unsafe class UIImageButton_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIImageButton);

            field = type.GetField("normalSprite", flag);
            app.RegisterCLRFieldGetter(field, get_normalSprite_0);
            app.RegisterCLRFieldSetter(field, set_normalSprite_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_normalSprite_0, AssignFromStack_normalSprite_0);
            field = type.GetField("hoverSprite", flag);
            app.RegisterCLRFieldGetter(field, get_hoverSprite_1);
            app.RegisterCLRFieldSetter(field, set_hoverSprite_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_hoverSprite_1, AssignFromStack_hoverSprite_1);
            field = type.GetField("pressedSprite", flag);
            app.RegisterCLRFieldGetter(field, get_pressedSprite_2);
            app.RegisterCLRFieldSetter(field, set_pressedSprite_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_pressedSprite_2, AssignFromStack_pressedSprite_2);


        }



        static object get_normalSprite_0(ref object o)
        {
            return ((global::UIImageButton)o).normalSprite;
        }

        static StackObject* CopyToStack_normalSprite_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIImageButton)o).normalSprite;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_normalSprite_0(ref object o, object v)
        {
            ((global::UIImageButton)o).normalSprite = (System.String)v;
        }

        static StackObject* AssignFromStack_normalSprite_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @normalSprite = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIImageButton)o).normalSprite = @normalSprite;
            return ptr_of_this_method;
        }

        static object get_hoverSprite_1(ref object o)
        {
            return ((global::UIImageButton)o).hoverSprite;
        }

        static StackObject* CopyToStack_hoverSprite_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIImageButton)o).hoverSprite;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_hoverSprite_1(ref object o, object v)
        {
            ((global::UIImageButton)o).hoverSprite = (System.String)v;
        }

        static StackObject* AssignFromStack_hoverSprite_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @hoverSprite = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIImageButton)o).hoverSprite = @hoverSprite;
            return ptr_of_this_method;
        }

        static object get_pressedSprite_2(ref object o)
        {
            return ((global::UIImageButton)o).pressedSprite;
        }

        static StackObject* CopyToStack_pressedSprite_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIImageButton)o).pressedSprite;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_pressedSprite_2(ref object o, object v)
        {
            ((global::UIImageButton)o).pressedSprite = (System.String)v;
        }

        static StackObject* AssignFromStack_pressedSprite_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @pressedSprite = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIImageButton)o).pressedSprite = @pressedSprite;
            return ptr_of_this_method;
        }



    }
}
