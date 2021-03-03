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
    unsafe class EB_Sparx_MHAuthenticator_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.MHAuthenticator);

            field = type.GetField("UserInfoList", flag);
            app.RegisterCLRFieldGetter(field, get_UserInfoList_0);
            app.RegisterCLRFieldSetter(field, set_UserInfoList_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_UserInfoList_0, AssignFromStack_UserInfoList_0);


        }



        static object get_UserInfoList_0(ref object o)
        {
            return EB.Sparx.MHAuthenticator.UserInfoList;
        }

        static StackObject* CopyToStack_UserInfoList_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = EB.Sparx.MHAuthenticator.UserInfoList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UserInfoList_0(ref object o, object v)
        {
            EB.Sparx.MHAuthenticator.UserInfoList = (System.Collections.Generic.List<EB.Sparx.MHAuthenticator.UserInfo>)v;
        }

        static StackObject* AssignFromStack_UserInfoList_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<EB.Sparx.MHAuthenticator.UserInfo> @UserInfoList = (System.Collections.Generic.List<EB.Sparx.MHAuthenticator.UserInfo>)typeof(System.Collections.Generic.List<EB.Sparx.MHAuthenticator.UserInfo>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            EB.Sparx.MHAuthenticator.UserInfoList = @UserInfoList;
            return ptr_of_this_method;
        }



    }
}
