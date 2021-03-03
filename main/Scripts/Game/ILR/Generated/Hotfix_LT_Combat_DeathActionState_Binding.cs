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
    unsafe class Hotfix_LT_Combat_DeathActionState_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Hotfix_LT.Combat.DeathActionState);

            field = type.GetField("HideCharacterPos", flag);
            app.RegisterCLRFieldGetter(field, get_HideCharacterPos_0);
            app.RegisterCLRFieldSetter(field, set_HideCharacterPos_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_HideCharacterPos_0, AssignFromStack_HideCharacterPos_0);


        }



        static object get_HideCharacterPos_0(ref object o)
        {
            return Hotfix_LT.Combat.DeathActionState.HideCharacterPos;
        }

        static StackObject* CopyToStack_HideCharacterPos_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = Hotfix_LT.Combat.DeathActionState.HideCharacterPos;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_HideCharacterPos_0(ref object o, object v)
        {
            Hotfix_LT.Combat.DeathActionState.HideCharacterPos = (UnityEngine.Vector3)v;
        }

        static StackObject* AssignFromStack_HideCharacterPos_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector3 @HideCharacterPos = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @HideCharacterPos, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @HideCharacterPos = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            }
            Hotfix_LT.Combat.DeathActionState.HideCharacterPos = @HideCharacterPos;
            return ptr_of_this_method;
        }



    }
}
