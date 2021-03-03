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
    unsafe class PerformanceInfo_Binding_EnvironmentInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::PerformanceInfo.EnvironmentInfo);

            field = type.GetField("slowDevice", flag);
            app.RegisterCLRFieldGetter(field, get_slowDevice_0);
            app.RegisterCLRFieldSetter(field, set_slowDevice_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_slowDevice_0, AssignFromStack_slowDevice_0);
            field = type.GetField("lod", flag);
            app.RegisterCLRFieldGetter(field, get_lod_1);
            app.RegisterCLRFieldSetter(field, set_lod_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_lod_1, AssignFromStack_lod_1);
            field = type.GetField("blendWeights", flag);
            app.RegisterCLRFieldGetter(field, get_blendWeights_2);
            app.RegisterCLRFieldSetter(field, set_blendWeights_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_blendWeights_2, AssignFromStack_blendWeights_2);
            field = type.GetField("msaa", flag);
            app.RegisterCLRFieldGetter(field, get_msaa_3);
            app.RegisterCLRFieldSetter(field, set_msaa_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_msaa_3, AssignFromStack_msaa_3);
            field = type.GetField("aniso", flag);
            app.RegisterCLRFieldGetter(field, get_aniso_4);
            app.RegisterCLRFieldSetter(field, set_aniso_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_aniso_4, AssignFromStack_aniso_4);
            field = type.GetField("shadowQuality", flag);
            app.RegisterCLRFieldGetter(field, get_shadowQuality_5);
            app.RegisterCLRFieldSetter(field, set_shadowQuality_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_shadowQuality_5, AssignFromStack_shadowQuality_5);
            field = type.GetField("hiddenLayers", flag);
            app.RegisterCLRFieldGetter(field, get_hiddenLayers_6);
            app.RegisterCLRFieldSetter(field, set_hiddenLayers_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_hiddenLayers_6, AssignFromStack_hiddenLayers_6);


        }



        static object get_slowDevice_0(ref object o)
        {
            return ((global::PerformanceInfo.EnvironmentInfo)o).slowDevice;
        }

        static StackObject* CopyToStack_slowDevice_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PerformanceInfo.EnvironmentInfo)o).slowDevice;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_slowDevice_0(ref object o, object v)
        {
            ((global::PerformanceInfo.EnvironmentInfo)o).slowDevice = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_slowDevice_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @slowDevice = ptr_of_this_method->Value == 1;
            ((global::PerformanceInfo.EnvironmentInfo)o).slowDevice = @slowDevice;
            return ptr_of_this_method;
        }

        static object get_lod_1(ref object o)
        {
            return ((global::PerformanceInfo.EnvironmentInfo)o).lod;
        }

        static StackObject* CopyToStack_lod_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PerformanceInfo.EnvironmentInfo)o).lod;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_lod_1(ref object o, object v)
        {
            ((global::PerformanceInfo.EnvironmentInfo)o).lod = (System.Int32)v;
        }

        static StackObject* AssignFromStack_lod_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @lod = ptr_of_this_method->Value;
            ((global::PerformanceInfo.EnvironmentInfo)o).lod = @lod;
            return ptr_of_this_method;
        }

        static object get_blendWeights_2(ref object o)
        {
            return ((global::PerformanceInfo.EnvironmentInfo)o).blendWeights;
        }

        static StackObject* CopyToStack_blendWeights_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PerformanceInfo.EnvironmentInfo)o).blendWeights;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_blendWeights_2(ref object o, object v)
        {
            ((global::PerformanceInfo.EnvironmentInfo)o).blendWeights = (global::PerformanceInfo.eBLEND_WEIGHTS)v;
        }

        static StackObject* AssignFromStack_blendWeights_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::PerformanceInfo.eBLEND_WEIGHTS @blendWeights = (global::PerformanceInfo.eBLEND_WEIGHTS)typeof(global::PerformanceInfo.eBLEND_WEIGHTS).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::PerformanceInfo.EnvironmentInfo)o).blendWeights = @blendWeights;
            return ptr_of_this_method;
        }

        static object get_msaa_3(ref object o)
        {
            return ((global::PerformanceInfo.EnvironmentInfo)o).msaa;
        }

        static StackObject* CopyToStack_msaa_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PerformanceInfo.EnvironmentInfo)o).msaa;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_msaa_3(ref object o, object v)
        {
            ((global::PerformanceInfo.EnvironmentInfo)o).msaa = (global::PerformanceInfo.eMSAA)v;
        }

        static StackObject* AssignFromStack_msaa_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::PerformanceInfo.eMSAA @msaa = (global::PerformanceInfo.eMSAA)typeof(global::PerformanceInfo.eMSAA).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::PerformanceInfo.EnvironmentInfo)o).msaa = @msaa;
            return ptr_of_this_method;
        }

        static object get_aniso_4(ref object o)
        {
            return ((global::PerformanceInfo.EnvironmentInfo)o).aniso;
        }

        static StackObject* CopyToStack_aniso_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PerformanceInfo.EnvironmentInfo)o).aniso;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_aniso_4(ref object o, object v)
        {
            ((global::PerformanceInfo.EnvironmentInfo)o).aniso = (global::PerformanceInfo.eANISOTROPIC)v;
        }

        static StackObject* AssignFromStack_aniso_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::PerformanceInfo.eANISOTROPIC @aniso = (global::PerformanceInfo.eANISOTROPIC)typeof(global::PerformanceInfo.eANISOTROPIC).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::PerformanceInfo.EnvironmentInfo)o).aniso = @aniso;
            return ptr_of_this_method;
        }

        static object get_shadowQuality_5(ref object o)
        {
            return ((global::PerformanceInfo.EnvironmentInfo)o).shadowQuality;
        }

        static StackObject* CopyToStack_shadowQuality_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PerformanceInfo.EnvironmentInfo)o).shadowQuality;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_shadowQuality_5(ref object o, object v)
        {
            ((global::PerformanceInfo.EnvironmentInfo)o).shadowQuality = (global::PerformanceInfo.eSHADOW_QUALITY)v;
        }

        static StackObject* AssignFromStack_shadowQuality_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::PerformanceInfo.eSHADOW_QUALITY @shadowQuality = (global::PerformanceInfo.eSHADOW_QUALITY)typeof(global::PerformanceInfo.eSHADOW_QUALITY).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::PerformanceInfo.EnvironmentInfo)o).shadowQuality = @shadowQuality;
            return ptr_of_this_method;
        }

        static object get_hiddenLayers_6(ref object o)
        {
            return ((global::PerformanceInfo.EnvironmentInfo)o).hiddenLayers;
        }

        static StackObject* CopyToStack_hiddenLayers_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PerformanceInfo.EnvironmentInfo)o).hiddenLayers;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_hiddenLayers_6(ref object o, object v)
        {
            ((global::PerformanceInfo.EnvironmentInfo)o).hiddenLayers = (System.Int32)v;
        }

        static StackObject* AssignFromStack_hiddenLayers_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @hiddenLayers = ptr_of_this_method->Value;
            ((global::PerformanceInfo.EnvironmentInfo)o).hiddenLayers = @hiddenLayers;
            return ptr_of_this_method;
        }



    }
}
