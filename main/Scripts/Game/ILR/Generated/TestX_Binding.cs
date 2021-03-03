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
    unsafe class TestX_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::TestX);
            args = new Type[]{};
            method = type.GetMethod("get_Ins", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Ins_0);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("GetLoginUid", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetLoginUid_1);

            field = type.GetField("login", flag);
            app.RegisterCLRFieldGetter(field, get_login_0);
            app.RegisterCLRFieldSetter(field, set_login_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_login_0, AssignFromStack_login_0);


        }


        static StackObject* get_Ins_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::TestX.Ins;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetLoginUid_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @loginUid = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::TestX instance_of_this_method = (global::TestX)typeof(global::TestX).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetLoginUid(@loginUid);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_login_0(ref object o)
        {
            return ((global::TestX)o).login;
        }

        static StackObject* CopyToStack_login_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::TestX)o).login;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_login_0(ref object o, object v)
        {
            ((global::TestX)o).login = (System.String)v;
        }

        static StackObject* AssignFromStack_login_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @login = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::TestX)o).login = @login;
            return ptr_of_this_method;
        }



    }
}
