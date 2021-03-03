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
    unsafe class LTCameraTrigger_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::LTCameraTrigger);

            field = type.GetField("triggerGameCameraParamName", flag);
            app.RegisterCLRFieldGetter(field, get_triggerGameCameraParamName_0);
            app.RegisterCLRFieldSetter(field, set_triggerGameCameraParamName_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_triggerGameCameraParamName_0, AssignFromStack_triggerGameCameraParamName_0);


        }



        static object get_triggerGameCameraParamName_0(ref object o)
        {
            return ((global::LTCameraTrigger)o).triggerGameCameraParamName;
        }

        static StackObject* CopyToStack_triggerGameCameraParamName_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::LTCameraTrigger)o).triggerGameCameraParamName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_triggerGameCameraParamName_0(ref object o, object v)
        {
            ((global::LTCameraTrigger)o).triggerGameCameraParamName = (System.String)v;
        }

        static StackObject* AssignFromStack_triggerGameCameraParamName_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @triggerGameCameraParamName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::LTCameraTrigger)o).triggerGameCameraParamName = @triggerGameCameraParamName;
            return ptr_of_this_method;
        }



    }
}
