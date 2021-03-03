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
    unsafe class AstarPath_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::AstarPath);
            args = new Type[]{typeof(Pathfinding.Path)};
            method = type.GetMethod("WaitForPath", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WaitForPath_0);
            args = new Type[]{typeof(UnityEngine.Vector3)};
            method = type.GetMethod("GetNearest", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetNearest_1);

            field = type.GetField("active", flag);
            app.RegisterCLRFieldGetter(field, get_active_0);
            app.RegisterCLRFieldSetter(field, set_active_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_active_0, AssignFromStack_active_0);


        }


        static StackObject* WaitForPath_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Pathfinding.Path @p = (Pathfinding.Path)typeof(Pathfinding.Path).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::AstarPath.WaitForPath(@p);

            return __ret;
        }

        static StackObject* GetNearest_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector3 @position = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @position, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @position = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
                __intp.Free(ptr_of_this_method);
            }

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::AstarPath instance_of_this_method = (global::AstarPath)typeof(global::AstarPath).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetNearest(@position);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_active_0(ref object o)
        {
            return global::AstarPath.active;
        }

        static StackObject* CopyToStack_active_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::AstarPath.active;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_active_0(ref object o, object v)
        {
            global::AstarPath.active = (global::AstarPath)v;
        }

        static StackObject* AssignFromStack_active_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::AstarPath @active = (global::AstarPath)typeof(global::AstarPath).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::AstarPath.active = @active;
            return ptr_of_this_method;
        }



    }
}
