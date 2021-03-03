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
    unsafe class MoveEditor_Move_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(MoveEditor.Move);

            field = type.GetField("_animationClip", flag);
            app.RegisterCLRFieldGetter(field, get__animationClip_0);
            app.RegisterCLRFieldSetter(field, set__animationClip_0);
            app.RegisterCLRFieldBinding(field, CopyToStack__animationClip_0, AssignFromStack__animationClip_0);


        }



        static object get__animationClip_0(ref object o)
        {
            return ((MoveEditor.Move)o)._animationClip;
        }

        static StackObject* CopyToStack__animationClip_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((MoveEditor.Move)o)._animationClip;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set__animationClip_0(ref object o, object v)
        {
            ((MoveEditor.Move)o)._animationClip = (UnityEngine.AnimationClip)v;
        }

        static StackObject* AssignFromStack__animationClip_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.AnimationClip @_animationClip = (UnityEngine.AnimationClip)typeof(UnityEngine.AnimationClip).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((MoveEditor.Move)o)._animationClip = @_animationClip;
            return ptr_of_this_method;
        }



    }
}
