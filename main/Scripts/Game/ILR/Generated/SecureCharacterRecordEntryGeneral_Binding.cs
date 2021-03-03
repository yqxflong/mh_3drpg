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
    unsafe class SecureCharacterRecordEntryGeneral_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::SecureCharacterRecordEntryGeneral);

            field = type.GetField("gender", flag);
            app.RegisterCLRFieldGetter(field, get_gender_0);
            app.RegisterCLRFieldSetter(field, set_gender_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_gender_0, AssignFromStack_gender_0);


        }



        static object get_gender_0(ref object o)
        {
            return ((global::SecureCharacterRecordEntryGeneral)o).gender;
        }

        static StackObject* CopyToStack_gender_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::SecureCharacterRecordEntryGeneral)o).gender;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_gender_0(ref object o, object v)
        {
            ((global::SecureCharacterRecordEntryGeneral)o).gender = (global::eGender)v;
        }

        static StackObject* AssignFromStack_gender_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::eGender @gender = (global::eGender)typeof(global::eGender).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::SecureCharacterRecordEntryGeneral)o).gender = @gender;
            return ptr_of_this_method;
        }



    }
}
