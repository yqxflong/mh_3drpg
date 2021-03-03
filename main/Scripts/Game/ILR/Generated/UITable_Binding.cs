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
    unsafe class UITable_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UITable);
            args = new Type[]{};
            method = type.GetMethod("Reposition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Reposition_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_repositionNow", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_repositionNow_1);
            args = new Type[]{};
            method = type.GetMethod("GetChildList", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetChildList_2);

            field = type.GetField("padding", flag);
            app.RegisterCLRFieldGetter(field, get_padding_0);
            app.RegisterCLRFieldSetter(field, set_padding_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_padding_0, AssignFromStack_padding_0);
            field = type.GetField("onRepositionOnce", flag);
            app.RegisterCLRFieldGetter(field, get_onRepositionOnce_1);
            app.RegisterCLRFieldSetter(field, set_onRepositionOnce_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_onRepositionOnce_1, AssignFromStack_onRepositionOnce_1);


        }


        static StackObject* Reposition_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UITable instance_of_this_method = (global::UITable)typeof(global::UITable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Reposition();

            return __ret;
        }

        static StackObject* set_repositionNow_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UITable instance_of_this_method = (global::UITable)typeof(global::UITable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.repositionNow = value;

            return __ret;
        }

        static StackObject* GetChildList_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UITable instance_of_this_method = (global::UITable)typeof(global::UITable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetChildList();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_padding_0(ref object o)
        {
            return ((global::UITable)o).padding;
        }

        static StackObject* CopyToStack_padding_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITable)o).padding;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_padding_0(ref object o, object v)
        {
            ((global::UITable)o).padding = (UnityEngine.Vector2)v;
        }

        static StackObject* AssignFromStack_padding_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector2 @padding = new UnityEngine.Vector2();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector2_Binding_Binder.ParseValue(ref @padding, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @padding = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            }
            ((global::UITable)o).padding = @padding;
            return ptr_of_this_method;
        }

        static object get_onRepositionOnce_1(ref object o)
        {
            return ((global::UITable)o).onRepositionOnce;
        }

        static StackObject* CopyToStack_onRepositionOnce_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITable)o).onRepositionOnce;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onRepositionOnce_1(ref object o, object v)
        {
            ((global::UITable)o).onRepositionOnce = (global::UITable.OnReposition)v;
        }

        static StackObject* AssignFromStack_onRepositionOnce_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UITable.OnReposition @onRepositionOnce = (global::UITable.OnReposition)typeof(global::UITable.OnReposition).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UITable)o).onRepositionOnce = @onRepositionOnce;
            return ptr_of_this_method;
        }



    }
}
