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
    unsafe class EB_Symbols_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Symbols);

            field = type.GetField("LanguageList", flag);
            app.RegisterCLRFieldGetter(field, get_LanguageList_0);
            app.RegisterCLRFieldSetter(field, set_LanguageList_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_LanguageList_0, AssignFromStack_LanguageList_0);


        }



        static object get_LanguageList_0(ref object o)
        {
            return EB.Symbols.LanguageList;
        }

        static StackObject* CopyToStack_LanguageList_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = EB.Symbols.LanguageList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_LanguageList_0(ref object o, object v)
        {
            EB.Symbols.LanguageList = (System.Collections.Generic.Dictionary<EB.Language, System.String>)v;
        }

        static StackObject* AssignFromStack_LanguageList_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<EB.Language, System.String> @LanguageList = (System.Collections.Generic.Dictionary<EB.Language, System.String>)typeof(System.Collections.Generic.Dictionary<EB.Language, System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            EB.Symbols.LanguageList = @LanguageList;
            return ptr_of_this_method;
        }



    }
}
