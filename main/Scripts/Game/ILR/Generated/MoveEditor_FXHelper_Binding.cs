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
    unsafe class MoveEditor_FXHelper_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(MoveEditor.FXHelper);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("StopAll", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StopAll_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_DisableFX", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_DisableFX_1);
            args = new Type[]{};
            method = type.GetMethod("get_HeadNubTransform", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_HeadNubTransform_2);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_CanPlayParticle", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_CanPlayParticle_3);
            args = new Type[]{typeof(MoveEditor.ParticleEventProperties), typeof(System.Boolean)};
            method = type.GetMethod("PlayParticle", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlayParticle_4);

            field = type.GetField("PlayParticleAction", flag);
            app.RegisterCLRFieldGetter(field, get_PlayParticleAction_0);
            app.RegisterCLRFieldSetter(field, set_PlayParticleAction_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_PlayParticleAction_0, AssignFromStack_PlayParticleAction_0);
            field = type.GetField("m_HealthBarFXAttachment", flag);
            app.RegisterCLRFieldGetter(field, get_m_HealthBarFXAttachment_1);
            app.RegisterCLRFieldSetter(field, set_m_HealthBarFXAttachment_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_HealthBarFXAttachment_1, AssignFromStack_m_HealthBarFXAttachment_1);


        }


        static StackObject* StopAll_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @clearParticles = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            MoveEditor.FXHelper instance_of_this_method = (MoveEditor.FXHelper)typeof(MoveEditor.FXHelper).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StopAll(@clearParticles);

            return __ret;
        }

        static StackObject* set_DisableFX_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            MoveEditor.FXHelper instance_of_this_method = (MoveEditor.FXHelper)typeof(MoveEditor.FXHelper).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DisableFX = value;

            return __ret;
        }

        static StackObject* get_HeadNubTransform_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            MoveEditor.FXHelper instance_of_this_method = (MoveEditor.FXHelper)typeof(MoveEditor.FXHelper).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.HeadNubTransform;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_CanPlayParticle_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            MoveEditor.FXHelper instance_of_this_method = (MoveEditor.FXHelper)typeof(MoveEditor.FXHelper).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CanPlayParticle = value;

            return __ret;
        }

        static StackObject* PlayParticle_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @forcePlay = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            MoveEditor.ParticleEventProperties @properties = (MoveEditor.ParticleEventProperties)typeof(MoveEditor.ParticleEventProperties).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            MoveEditor.FXHelper instance_of_this_method = (MoveEditor.FXHelper)typeof(MoveEditor.FXHelper).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.PlayParticle(@properties, @forcePlay);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_PlayParticleAction_0(ref object o)
        {
            return ((MoveEditor.FXHelper)o).PlayParticleAction;
        }

        static StackObject* CopyToStack_PlayParticleAction_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((MoveEditor.FXHelper)o).PlayParticleAction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_PlayParticleAction_0(ref object o, object v)
        {
            ((MoveEditor.FXHelper)o).PlayParticleAction = (System.Action<UnityEngine.ParticleSystem>)v;
        }

        static StackObject* AssignFromStack_PlayParticleAction_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<UnityEngine.ParticleSystem> @PlayParticleAction = (System.Action<UnityEngine.ParticleSystem>)typeof(System.Action<UnityEngine.ParticleSystem>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((MoveEditor.FXHelper)o).PlayParticleAction = @PlayParticleAction;
            return ptr_of_this_method;
        }

        static object get_m_HealthBarFXAttachment_1(ref object o)
        {
            return ((MoveEditor.FXHelper)o).m_HealthBarFXAttachment;
        }

        static StackObject* CopyToStack_m_HealthBarFXAttachment_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((MoveEditor.FXHelper)o).m_HealthBarFXAttachment;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_HealthBarFXAttachment_1(ref object o, object v)
        {
            ((MoveEditor.FXHelper)o).m_HealthBarFXAttachment = (UnityEngine.Transform)v;
        }

        static StackObject* AssignFromStack_m_HealthBarFXAttachment_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Transform @m_HealthBarFXAttachment = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((MoveEditor.FXHelper)o).m_HealthBarFXAttachment = @m_HealthBarFXAttachment;
            return ptr_of_this_method;
        }



    }
}
