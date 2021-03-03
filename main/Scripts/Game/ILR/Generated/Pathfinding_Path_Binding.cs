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
    unsafe class Pathfinding_Path_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Pathfinding.Path);

            field = type.GetField("vectorPath", flag);
            app.RegisterCLRFieldGetter(field, get_vectorPath_0);
            app.RegisterCLRFieldSetter(field, set_vectorPath_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_vectorPath_0, AssignFromStack_vectorPath_0);


        }



        static object get_vectorPath_0(ref object o)
        {
            return ((Pathfinding.Path)o).vectorPath;
        }

        static StackObject* CopyToStack_vectorPath_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Pathfinding.Path)o).vectorPath;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_vectorPath_0(ref object o, object v)
        {
            ((Pathfinding.Path)o).vectorPath = (System.Collections.Generic.List<UnityEngine.Vector3>)v;
        }

        static StackObject* AssignFromStack_vectorPath_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<UnityEngine.Vector3> @vectorPath = (System.Collections.Generic.List<UnityEngine.Vector3>)typeof(System.Collections.Generic.List<UnityEngine.Vector3>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Pathfinding.Path)o).vectorPath = @vectorPath;
            return ptr_of_this_method;
        }



    }
}
