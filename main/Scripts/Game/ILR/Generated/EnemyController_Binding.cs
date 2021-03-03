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
    unsafe class EnemyController_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::EnemyController);

            field = type.GetField("onDestroy", flag);
            app.RegisterCLRFieldGetter(field, get_onDestroy_0);
            app.RegisterCLRFieldSetter(field, set_onDestroy_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onDestroy_0, AssignFromStack_onDestroy_0);
            field = type.GetField("HotfixController", flag);
            app.RegisterCLRFieldGetter(field, get_HotfixController_1);
            app.RegisterCLRFieldSetter(field, set_HotfixController_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_HotfixController_1, AssignFromStack_HotfixController_1);


        }



        static object get_onDestroy_0(ref object o)
        {
            return ((global::EnemyController)o).onDestroy;
        }

        static StackObject* CopyToStack_onDestroy_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::EnemyController)o).onDestroy;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onDestroy_0(ref object o, object v)
        {
            ((global::EnemyController)o).onDestroy = (System.Action<global::EnemyController>)v;
        }

        static StackObject* AssignFromStack_onDestroy_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<global::EnemyController> @onDestroy = (System.Action<global::EnemyController>)typeof(System.Action<global::EnemyController>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::EnemyController)o).onDestroy = @onDestroy;
            return ptr_of_this_method;
        }

        static object get_HotfixController_1(ref object o)
        {
            return ((global::EnemyController)o).HotfixController;
        }

        static StackObject* CopyToStack_HotfixController_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::EnemyController)o).HotfixController;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_HotfixController_1(ref object o, object v)
        {
            ((global::EnemyController)o).HotfixController = (global::DynamicMonoILR)v;
        }

        static StackObject* AssignFromStack_HotfixController_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::DynamicMonoILR @HotfixController = (global::DynamicMonoILR)typeof(global::DynamicMonoILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::EnemyController)o).HotfixController = @HotfixController;
            return ptr_of_this_method;
        }



    }
}
