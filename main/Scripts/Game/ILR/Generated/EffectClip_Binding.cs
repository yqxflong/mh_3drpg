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
    unsafe class EffectClip_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::EffectClip);
            args = new Type[]{};
            method = type.GetMethod("Init", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Init_0);

            field = type.GetField("HasInitialized", flag);
            app.RegisterCLRFieldGetter(field, get_HasInitialized_0);
            app.RegisterCLRFieldSetter(field, set_HasInitialized_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_HasInitialized_0, AssignFromStack_HasInitialized_0);


        }


        static StackObject* Init_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EffectClip instance_of_this_method = (global::EffectClip)typeof(global::EffectClip).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Init();

            return __ret;
        }


        static object get_HasInitialized_0(ref object o)
        {
            return ((global::EffectClip)o).HasInitialized;
        }

        static StackObject* CopyToStack_HasInitialized_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::EffectClip)o).HasInitialized;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_HasInitialized_0(ref object o, object v)
        {
            ((global::EffectClip)o).HasInitialized = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_HasInitialized_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @HasInitialized = ptr_of_this_method->Value == 1;
            ((global::EffectClip)o).HasInitialized = @HasInitialized;
            return ptr_of_this_method;
        }



    }
}
