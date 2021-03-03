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
    unsafe class UISymbolInput_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UISymbolInput);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_value", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_value_0);
            args = new Type[]{};
            method = type.GetMethod("get_value", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_value_1);

            field = type.GetField("characterLimit", flag);
            app.RegisterCLRFieldGetter(field, get_characterLimit_0);
            app.RegisterCLRFieldSetter(field, set_characterLimit_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_characterLimit_0, AssignFromStack_characterLimit_0);
            field = type.GetField("label", flag);
            app.RegisterCLRFieldGetter(field, get_label_1);
            app.RegisterCLRFieldSetter(field, set_label_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_label_1, AssignFromStack_label_1);


        }


        static StackObject* set_value_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UISymbolInput instance_of_this_method = (global::UISymbolInput)typeof(global::UISymbolInput).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.value = value;

            return __ret;
        }

        static StackObject* get_value_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UISymbolInput instance_of_this_method = (global::UISymbolInput)typeof(global::UISymbolInput).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.value;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_characterLimit_0(ref object o)
        {
            return ((global::UISymbolInput)o).characterLimit;
        }

        static StackObject* CopyToStack_characterLimit_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UISymbolInput)o).characterLimit;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_characterLimit_0(ref object o, object v)
        {
            ((global::UISymbolInput)o).characterLimit = (System.Int32)v;
        }

        static StackObject* AssignFromStack_characterLimit_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @characterLimit = ptr_of_this_method->Value;
            ((global::UISymbolInput)o).characterLimit = @characterLimit;
            return ptr_of_this_method;
        }

        static object get_label_1(ref object o)
        {
            return ((global::UISymbolInput)o).label;
        }

        static StackObject* CopyToStack_label_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UISymbolInput)o).label;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_label_1(ref object o, object v)
        {
            ((global::UISymbolInput)o).label = (global::UISymbolLabel)v;
        }

        static StackObject* AssignFromStack_label_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UISymbolLabel @label = (global::UISymbolLabel)typeof(global::UISymbolLabel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UISymbolInput)o).label = @label;
            return ptr_of_this_method;
        }



    }
}
