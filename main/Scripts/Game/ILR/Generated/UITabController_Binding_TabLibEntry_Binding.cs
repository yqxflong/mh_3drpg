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
    unsafe class UITabController_Binding_TabLibEntry_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UITabController.TabLibEntry);

            field = type.GetField("TabObj", flag);
            app.RegisterCLRFieldGetter(field, get_TabObj_0);
            app.RegisterCLRFieldSetter(field, set_TabObj_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_TabObj_0, AssignFromStack_TabObj_0);
            field = type.GetField("PressedTabObj", flag);
            app.RegisterCLRFieldGetter(field, get_PressedTabObj_1);
            app.RegisterCLRFieldSetter(field, set_PressedTabObj_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_PressedTabObj_1, AssignFromStack_PressedTabObj_1);
            field = type.GetField("TabTitle", flag);
            app.RegisterCLRFieldGetter(field, get_TabTitle_2);
            app.RegisterCLRFieldSetter(field, set_TabTitle_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_TabTitle_2, AssignFromStack_TabTitle_2);
            field = type.GetField("GameViewObj", flag);
            app.RegisterCLRFieldGetter(field, get_GameViewObj_3);
            app.RegisterCLRFieldSetter(field, set_GameViewObj_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_GameViewObj_3, AssignFromStack_GameViewObj_3);

            app.RegisterCLRCreateDefaultInstance(type, () => new global::UITabController.TabLibEntry());


        }

        static void WriteBackInstance(ILRuntime.Runtime.Enviorment.AppDomain __domain, StackObject* ptr_of_this_method, IList<object> __mStack, ref global::UITabController.TabLibEntry instance_of_this_method)
        {
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            switch(ptr_of_this_method->ObjectType)
            {
                case ObjectTypes.Object:
                    {
                        __mStack[ptr_of_this_method->Value] = instance_of_this_method;
                    }
                    break;
                case ObjectTypes.FieldReference:
                    {
                        var ___obj = __mStack[ptr_of_this_method->Value];
                        if(___obj is ILTypeInstance)
                        {
                            ((ILTypeInstance)___obj)[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            var t = __domain.GetType(___obj.GetType()) as CLRType;
                            t.SetFieldValue(ptr_of_this_method->ValueLow, ref ___obj, instance_of_this_method);
                        }
                    }
                    break;
                case ObjectTypes.StaticFieldReference:
                    {
                        var t = __domain.GetType(ptr_of_this_method->Value);
                        if(t is ILType)
                        {
                            ((ILType)t).StaticInstance[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            ((CLRType)t).SetStaticFieldValue(ptr_of_this_method->ValueLow, instance_of_this_method);
                        }
                    }
                    break;
                 case ObjectTypes.ArrayReference:
                    {
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as global::UITabController.TabLibEntry[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = instance_of_this_method;
                    }
                    break;
            }
        }


        static object get_TabObj_0(ref object o)
        {
            return ((global::UITabController.TabLibEntry)o).TabObj;
        }

        static StackObject* CopyToStack_TabObj_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITabController.TabLibEntry)o).TabObj;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_TabObj_0(ref object o, object v)
        {
            global::UITabController.TabLibEntry ins =(global::UITabController.TabLibEntry)o;
            ins.TabObj = (UnityEngine.GameObject)v;
            o = ins;
        }

        static StackObject* AssignFromStack_TabObj_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.GameObject @TabObj = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::UITabController.TabLibEntry ins =(global::UITabController.TabLibEntry)o;
            ins.TabObj = @TabObj;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_PressedTabObj_1(ref object o)
        {
            return ((global::UITabController.TabLibEntry)o).PressedTabObj;
        }

        static StackObject* CopyToStack_PressedTabObj_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITabController.TabLibEntry)o).PressedTabObj;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_PressedTabObj_1(ref object o, object v)
        {
            global::UITabController.TabLibEntry ins =(global::UITabController.TabLibEntry)o;
            ins.PressedTabObj = (UnityEngine.GameObject)v;
            o = ins;
        }

        static StackObject* AssignFromStack_PressedTabObj_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.GameObject @PressedTabObj = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::UITabController.TabLibEntry ins =(global::UITabController.TabLibEntry)o;
            ins.PressedTabObj = @PressedTabObj;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_TabTitle_2(ref object o)
        {
            return ((global::UITabController.TabLibEntry)o).TabTitle;
        }

        static StackObject* CopyToStack_TabTitle_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITabController.TabLibEntry)o).TabTitle;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_TabTitle_2(ref object o, object v)
        {
            global::UITabController.TabLibEntry ins =(global::UITabController.TabLibEntry)o;
            ins.TabTitle = (global::UILabel)v;
            o = ins;
        }

        static StackObject* AssignFromStack_TabTitle_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UILabel @TabTitle = (global::UILabel)typeof(global::UILabel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::UITabController.TabLibEntry ins =(global::UITabController.TabLibEntry)o;
            ins.TabTitle = @TabTitle;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_GameViewObj_3(ref object o)
        {
            return ((global::UITabController.TabLibEntry)o).GameViewObj;
        }

        static StackObject* CopyToStack_GameViewObj_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITabController.TabLibEntry)o).GameViewObj;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GameViewObj_3(ref object o, object v)
        {
            global::UITabController.TabLibEntry ins =(global::UITabController.TabLibEntry)o;
            ins.GameViewObj = (UnityEngine.GameObject)v;
            o = ins;
        }

        static StackObject* AssignFromStack_GameViewObj_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.GameObject @GameViewObj = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::UITabController.TabLibEntry ins =(global::UITabController.TabLibEntry)o;
            ins.GameViewObj = @GameViewObj;
            o = ins;
            return ptr_of_this_method;
        }



    }
}
