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
    unsafe class CombatCamera_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::CombatCamera);
            args = new Type[]{};
            method = type.GetMethod("EnableBlurEffect", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EnableBlurEffect_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_HoldingCamera", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_HoldingCamera_1);
            args = new Type[]{};
            method = type.GetMethod("DisableBlurEffect", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DisableBlurEffect_2);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_3);
            args = new Type[]{};
            method = type.GetMethod("OnTeamsVictoryListener", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnTeamsVictoryListener_4);

            field = type.GetField("isBoss", flag);
            app.RegisterCLRFieldGetter(field, get_isBoss_0);
            app.RegisterCLRFieldSetter(field, set_isBoss_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_isBoss_0, AssignFromStack_isBoss_0);


        }


        static StackObject* EnableBlurEffect_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CombatCamera instance_of_this_method = (global::CombatCamera)typeof(global::CombatCamera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.EnableBlurEffect();

            return __ret;
        }

        static StackObject* set_HoldingCamera_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::CombatCamera instance_of_this_method = (global::CombatCamera)typeof(global::CombatCamera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.HoldingCamera = value;

            return __ret;
        }

        static StackObject* DisableBlurEffect_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CombatCamera instance_of_this_method = (global::CombatCamera)typeof(global::CombatCamera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DisableBlurEffect();

            return __ret;
        }

        static StackObject* get_Instance_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::CombatCamera.Instance;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* OnTeamsVictoryListener_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::CombatCamera instance_of_this_method = (global::CombatCamera)typeof(global::CombatCamera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnTeamsVictoryListener();

            return __ret;
        }


        static object get_isBoss_0(ref object o)
        {
            return global::CombatCamera.isBoss;
        }

        static StackObject* CopyToStack_isBoss_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::CombatCamera.isBoss;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_isBoss_0(ref object o, object v)
        {
            global::CombatCamera.isBoss = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_isBoss_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @isBoss = ptr_of_this_method->Value == 1;
            global::CombatCamera.isBoss = @isBoss;
            return ptr_of_this_method;
        }



    }
}
