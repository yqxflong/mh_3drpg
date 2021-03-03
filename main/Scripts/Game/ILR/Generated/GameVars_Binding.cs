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
    unsafe class GameVars_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::GameVars);

            field = type.GetField("MainLightPosition", flag);
            app.RegisterCLRFieldGetter(field, get_MainLightPosition_0);
            app.RegisterCLRFieldSetter(field, set_MainLightPosition_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_MainLightPosition_0, AssignFromStack_MainLightPosition_0);
            field = type.GetField("MainLightRotation", flag);
            app.RegisterCLRFieldGetter(field, get_MainLightRotation_1);
            app.RegisterCLRFieldSetter(field, set_MainLightRotation_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_MainLightRotation_1, AssignFromStack_MainLightRotation_1);
            field = type.GetField("MainLightIntensity", flag);
            app.RegisterCLRFieldGetter(field, get_MainLightIntensity_2);
            app.RegisterCLRFieldSetter(field, set_MainLightIntensity_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_MainLightIntensity_2, AssignFromStack_MainLightIntensity_2);
            field = type.GetField("MainLightColor", flag);
            app.RegisterCLRFieldGetter(field, get_MainLightColor_3);
            app.RegisterCLRFieldSetter(field, set_MainLightColor_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_MainLightColor_3, AssignFromStack_MainLightColor_3);
            field = type.GetField("GlobalAmbient", flag);
            app.RegisterCLRFieldGetter(field, get_GlobalAmbient_4);
            app.RegisterCLRFieldSetter(field, set_GlobalAmbient_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_GlobalAmbient_4, AssignFromStack_GlobalAmbient_4);


        }



        static object get_MainLightPosition_0(ref object o)
        {
            return global::GameVars.MainLightPosition;
        }

        static StackObject* CopyToStack_MainLightPosition_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::GameVars.MainLightPosition;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_MainLightPosition_0(ref object o, object v)
        {
            global::GameVars.MainLightPosition = (UnityEngine.Vector3)v;
        }

        static StackObject* AssignFromStack_MainLightPosition_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector3 @MainLightPosition = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @MainLightPosition, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @MainLightPosition = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            }
            global::GameVars.MainLightPosition = @MainLightPosition;
            return ptr_of_this_method;
        }

        static object get_MainLightRotation_1(ref object o)
        {
            return global::GameVars.MainLightRotation;
        }

        static StackObject* CopyToStack_MainLightRotation_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::GameVars.MainLightRotation;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Quaternion_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Quaternion_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_MainLightRotation_1(ref object o, object v)
        {
            global::GameVars.MainLightRotation = (UnityEngine.Quaternion)v;
        }

        static StackObject* AssignFromStack_MainLightRotation_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Quaternion @MainLightRotation = new UnityEngine.Quaternion();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Quaternion_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Quaternion_Binding_Binder.ParseValue(ref @MainLightRotation, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @MainLightRotation = (UnityEngine.Quaternion)typeof(UnityEngine.Quaternion).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            }
            global::GameVars.MainLightRotation = @MainLightRotation;
            return ptr_of_this_method;
        }

        static object get_MainLightIntensity_2(ref object o)
        {
            return global::GameVars.MainLightIntensity;
        }

        static StackObject* CopyToStack_MainLightIntensity_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::GameVars.MainLightIntensity;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_MainLightIntensity_2(ref object o, object v)
        {
            global::GameVars.MainLightIntensity = (System.Single)v;
        }

        static StackObject* AssignFromStack_MainLightIntensity_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @MainLightIntensity = *(float*)&ptr_of_this_method->Value;
            global::GameVars.MainLightIntensity = @MainLightIntensity;
            return ptr_of_this_method;
        }

        static object get_MainLightColor_3(ref object o)
        {
            return global::GameVars.MainLightColor;
        }

        static StackObject* CopyToStack_MainLightColor_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::GameVars.MainLightColor;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_MainLightColor_3(ref object o, object v)
        {
            global::GameVars.MainLightColor = (UnityEngine.Color)v;
        }

        static StackObject* AssignFromStack_MainLightColor_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Color @MainLightColor = (UnityEngine.Color)typeof(UnityEngine.Color).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::GameVars.MainLightColor = @MainLightColor;
            return ptr_of_this_method;
        }

        static object get_GlobalAmbient_4(ref object o)
        {
            return global::GameVars.GlobalAmbient;
        }

        static StackObject* CopyToStack_GlobalAmbient_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::GameVars.GlobalAmbient;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GlobalAmbient_4(ref object o, object v)
        {
            global::GameVars.GlobalAmbient = (UnityEngine.Color)v;
        }

        static StackObject* AssignFromStack_GlobalAmbient_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Color @GlobalAmbient = (UnityEngine.Color)typeof(UnityEngine.Color).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::GameVars.GlobalAmbient = @GlobalAmbient;
            return ptr_of_this_method;
        }



    }
}
