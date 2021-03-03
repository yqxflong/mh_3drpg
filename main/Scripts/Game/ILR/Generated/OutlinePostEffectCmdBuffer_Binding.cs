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
    unsafe class OutlinePostEffectCmdBuffer_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::OutlinePostEffectCmdBuffer);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("SetDir", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetDir_0);
            args = new Type[]{typeof(UnityEngine.GameObject), typeof(UnityEngine.GameObject)};
            method = type.GetMethod("RefreshTargetObject", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RefreshTargetObject_1);

            field = type.GetField("outLineColor", flag);
            app.RegisterCLRFieldGetter(field, get_outLineColor_0);
            app.RegisterCLRFieldSetter(field, set_outLineColor_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_outLineColor_0, AssignFromStack_outLineColor_0);
            field = type.GetField("shader", flag);
            app.RegisterCLRFieldGetter(field, get_shader_1);
            app.RegisterCLRFieldSetter(field, set_shader_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_shader_1, AssignFromStack_shader_1);
            field = type.GetField("outlineShader", flag);
            app.RegisterCLRFieldGetter(field, get_outlineShader_2);
            app.RegisterCLRFieldSetter(field, set_outlineShader_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_outlineShader_2, AssignFromStack_outlineShader_2);
            field = type.GetField("samplerScale", flag);
            app.RegisterCLRFieldGetter(field, get_samplerScale_3);
            app.RegisterCLRFieldSetter(field, set_samplerScale_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_samplerScale_3, AssignFromStack_samplerScale_3);
            field = type.GetField("iteration", flag);
            app.RegisterCLRFieldGetter(field, get_iteration_4);
            app.RegisterCLRFieldSetter(field, set_iteration_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_iteration_4, AssignFromStack_iteration_4);
            field = type.GetField("outLineStrength", flag);
            app.RegisterCLRFieldGetter(field, get_outLineStrength_5);
            app.RegisterCLRFieldSetter(field, set_outLineStrength_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_outLineStrength_5, AssignFromStack_outLineStrength_5);


        }


        static StackObject* SetDir_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @dir = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::OutlinePostEffectCmdBuffer instance_of_this_method = (global::OutlinePostEffectCmdBuffer)typeof(global::OutlinePostEffectCmdBuffer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetDir(@dir);

            return __ret;
        }

        static StackObject* RefreshTargetObject_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.GameObject @OtherTarget = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.GameObject @myTarget = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::OutlinePostEffectCmdBuffer instance_of_this_method = (global::OutlinePostEffectCmdBuffer)typeof(global::OutlinePostEffectCmdBuffer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RefreshTargetObject(@myTarget, @OtherTarget);

            return __ret;
        }


        static object get_outLineColor_0(ref object o)
        {
            return ((global::OutlinePostEffectCmdBuffer)o).outLineColor;
        }

        static StackObject* CopyToStack_outLineColor_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::OutlinePostEffectCmdBuffer)o).outLineColor;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_outLineColor_0(ref object o, object v)
        {
            ((global::OutlinePostEffectCmdBuffer)o).outLineColor = (UnityEngine.Color)v;
        }

        static StackObject* AssignFromStack_outLineColor_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Color @outLineColor = (UnityEngine.Color)typeof(UnityEngine.Color).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::OutlinePostEffectCmdBuffer)o).outLineColor = @outLineColor;
            return ptr_of_this_method;
        }

        static object get_shader_1(ref object o)
        {
            return ((global::OutlinePostEffectCmdBuffer)o).shader;
        }

        static StackObject* CopyToStack_shader_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::OutlinePostEffectCmdBuffer)o).shader;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_shader_1(ref object o, object v)
        {
            ((global::OutlinePostEffectCmdBuffer)o).shader = (UnityEngine.Shader)v;
        }

        static StackObject* AssignFromStack_shader_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Shader @shader = (UnityEngine.Shader)typeof(UnityEngine.Shader).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::OutlinePostEffectCmdBuffer)o).shader = @shader;
            return ptr_of_this_method;
        }

        static object get_outlineShader_2(ref object o)
        {
            return ((global::OutlinePostEffectCmdBuffer)o).outlineShader;
        }

        static StackObject* CopyToStack_outlineShader_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::OutlinePostEffectCmdBuffer)o).outlineShader;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_outlineShader_2(ref object o, object v)
        {
            ((global::OutlinePostEffectCmdBuffer)o).outlineShader = (UnityEngine.Shader)v;
        }

        static StackObject* AssignFromStack_outlineShader_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Shader @outlineShader = (UnityEngine.Shader)typeof(UnityEngine.Shader).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::OutlinePostEffectCmdBuffer)o).outlineShader = @outlineShader;
            return ptr_of_this_method;
        }

        static object get_samplerScale_3(ref object o)
        {
            return ((global::OutlinePostEffectCmdBuffer)o).samplerScale;
        }

        static StackObject* CopyToStack_samplerScale_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::OutlinePostEffectCmdBuffer)o).samplerScale;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_samplerScale_3(ref object o, object v)
        {
            ((global::OutlinePostEffectCmdBuffer)o).samplerScale = (System.Single)v;
        }

        static StackObject* AssignFromStack_samplerScale_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @samplerScale = *(float*)&ptr_of_this_method->Value;
            ((global::OutlinePostEffectCmdBuffer)o).samplerScale = @samplerScale;
            return ptr_of_this_method;
        }

        static object get_iteration_4(ref object o)
        {
            return ((global::OutlinePostEffectCmdBuffer)o).iteration;
        }

        static StackObject* CopyToStack_iteration_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::OutlinePostEffectCmdBuffer)o).iteration;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_iteration_4(ref object o, object v)
        {
            ((global::OutlinePostEffectCmdBuffer)o).iteration = (System.Int32)v;
        }

        static StackObject* AssignFromStack_iteration_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @iteration = ptr_of_this_method->Value;
            ((global::OutlinePostEffectCmdBuffer)o).iteration = @iteration;
            return ptr_of_this_method;
        }

        static object get_outLineStrength_5(ref object o)
        {
            return ((global::OutlinePostEffectCmdBuffer)o).outLineStrength;
        }

        static StackObject* CopyToStack_outLineStrength_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::OutlinePostEffectCmdBuffer)o).outLineStrength;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_outLineStrength_5(ref object o, object v)
        {
            ((global::OutlinePostEffectCmdBuffer)o).outLineStrength = (System.Single)v;
        }

        static StackObject* AssignFromStack_outLineStrength_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @outLineStrength = *(float*)&ptr_of_this_method->Value;
            ((global::OutlinePostEffectCmdBuffer)o).outLineStrength = @outLineStrength;
            return ptr_of_this_method;
        }



    }
}
