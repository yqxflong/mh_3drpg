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
    unsafe class PerformanceInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::PerformanceInfo);

            field = type.GetField("CpuProfileName", flag);
            app.RegisterCLRFieldGetter(field, get_CpuProfileName_0);
            app.RegisterCLRFieldSetter(field, set_CpuProfileName_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_CpuProfileName_0, AssignFromStack_CpuProfileName_0);


        }



        static object get_CpuProfileName_0(ref object o)
        {
            return ((global::PerformanceInfo)o).CpuProfileName;
        }

        static StackObject* CopyToStack_CpuProfileName_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PerformanceInfo)o).CpuProfileName;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_CpuProfileName_0(ref object o, object v)
        {
            ((global::PerformanceInfo)o).CpuProfileName = (System.String)v;
        }

        static StackObject* AssignFromStack_CpuProfileName_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @CpuProfileName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::PerformanceInfo)o).CpuProfileName = @CpuProfileName;
            return ptr_of_this_method;
        }



    }
}
