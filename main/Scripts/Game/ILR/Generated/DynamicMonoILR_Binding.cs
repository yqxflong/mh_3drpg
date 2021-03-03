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
    unsafe class DynamicMonoILR_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::DynamicMonoILR);

            field = type.GetField("StringParamList", flag);
            app.RegisterCLRFieldGetter(field, get_StringParamList_0);
            app.RegisterCLRFieldSetter(field, set_StringParamList_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_StringParamList_0, AssignFromStack_StringParamList_0);
            field = type.GetField("BoolParamList", flag);
            app.RegisterCLRFieldGetter(field, get_BoolParamList_1);
            app.RegisterCLRFieldSetter(field, set_BoolParamList_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_BoolParamList_1, AssignFromStack_BoolParamList_1);
            field = type.GetField("ObjectParamList", flag);
            app.RegisterCLRFieldGetter(field, get_ObjectParamList_2);
            app.RegisterCLRFieldSetter(field, set_ObjectParamList_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_ObjectParamList_2, AssignFromStack_ObjectParamList_2);
            field = type.GetField("_ilrObject", flag);
            app.RegisterCLRFieldGetter(field, get__ilrObject_3);
            app.RegisterCLRFieldSetter(field, set__ilrObject_3);
            app.RegisterCLRFieldBinding(field, CopyToStack__ilrObject_3, AssignFromStack__ilrObject_3);
            field = type.GetField("IntParamList", flag);
            app.RegisterCLRFieldGetter(field, get_IntParamList_4);
            app.RegisterCLRFieldSetter(field, set_IntParamList_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_IntParamList_4, AssignFromStack_IntParamList_4);
            field = type.GetField("FloatParamList", flag);
            app.RegisterCLRFieldGetter(field, get_FloatParamList_5);
            app.RegisterCLRFieldSetter(field, set_FloatParamList_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_FloatParamList_5, AssignFromStack_FloatParamList_5);
            field = type.GetField("hotfixClassPath", flag);
            app.RegisterCLRFieldGetter(field, get_hotfixClassPath_6);
            app.RegisterCLRFieldSetter(field, set_hotfixClassPath_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_hotfixClassPath_6, AssignFromStack_hotfixClassPath_6);
            field = type.GetField("Vector3ParamList", flag);
            app.RegisterCLRFieldGetter(field, get_Vector3ParamList_7);
            app.RegisterCLRFieldSetter(field, set_Vector3ParamList_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_Vector3ParamList_7, AssignFromStack_Vector3ParamList_7);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static object get_StringParamList_0(ref object o)
        {
            return ((global::DynamicMonoILR)o).StringParamList;
        }

        static StackObject* CopyToStack_StringParamList_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DynamicMonoILR)o).StringParamList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_StringParamList_0(ref object o, object v)
        {
            ((global::DynamicMonoILR)o).StringParamList = (System.Collections.Generic.List<System.String>)v;
        }

        static StackObject* AssignFromStack_StringParamList_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<System.String> @StringParamList = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DynamicMonoILR)o).StringParamList = @StringParamList;
            return ptr_of_this_method;
        }

        static object get_BoolParamList_1(ref object o)
        {
            return ((global::DynamicMonoILR)o).BoolParamList;
        }

        static StackObject* CopyToStack_BoolParamList_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DynamicMonoILR)o).BoolParamList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_BoolParamList_1(ref object o, object v)
        {
            ((global::DynamicMonoILR)o).BoolParamList = (System.Collections.Generic.List<System.Boolean>)v;
        }

        static StackObject* AssignFromStack_BoolParamList_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<System.Boolean> @BoolParamList = (System.Collections.Generic.List<System.Boolean>)typeof(System.Collections.Generic.List<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DynamicMonoILR)o).BoolParamList = @BoolParamList;
            return ptr_of_this_method;
        }

        static object get_ObjectParamList_2(ref object o)
        {
            return ((global::DynamicMonoILR)o).ObjectParamList;
        }

        static StackObject* CopyToStack_ObjectParamList_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DynamicMonoILR)o).ObjectParamList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ObjectParamList_2(ref object o, object v)
        {
            ((global::DynamicMonoILR)o).ObjectParamList = (System.Collections.Generic.List<UnityEngine.Object>)v;
        }

        static StackObject* AssignFromStack_ObjectParamList_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<UnityEngine.Object> @ObjectParamList = (System.Collections.Generic.List<UnityEngine.Object>)typeof(System.Collections.Generic.List<UnityEngine.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DynamicMonoILR)o).ObjectParamList = @ObjectParamList;
            return ptr_of_this_method;
        }

        static object get__ilrObject_3(ref object o)
        {
            return ((global::DynamicMonoILR)o)._ilrObject;
        }

        static StackObject* CopyToStack__ilrObject_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DynamicMonoILR)o)._ilrObject;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set__ilrObject_3(ref object o, object v)
        {
            ((global::DynamicMonoILR)o)._ilrObject = (global::DynamicMonoILRObject)v;
        }

        static StackObject* AssignFromStack__ilrObject_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::DynamicMonoILRObject @_ilrObject = (global::DynamicMonoILRObject)typeof(global::DynamicMonoILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DynamicMonoILR)o)._ilrObject = @_ilrObject;
            return ptr_of_this_method;
        }

        static object get_IntParamList_4(ref object o)
        {
            return ((global::DynamicMonoILR)o).IntParamList;
        }

        static StackObject* CopyToStack_IntParamList_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DynamicMonoILR)o).IntParamList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_IntParamList_4(ref object o, object v)
        {
            ((global::DynamicMonoILR)o).IntParamList = (System.Collections.Generic.List<System.Int32>)v;
        }

        static StackObject* AssignFromStack_IntParamList_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<System.Int32> @IntParamList = (System.Collections.Generic.List<System.Int32>)typeof(System.Collections.Generic.List<System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DynamicMonoILR)o).IntParamList = @IntParamList;
            return ptr_of_this_method;
        }

        static object get_FloatParamList_5(ref object o)
        {
            return ((global::DynamicMonoILR)o).FloatParamList;
        }

        static StackObject* CopyToStack_FloatParamList_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DynamicMonoILR)o).FloatParamList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_FloatParamList_5(ref object o, object v)
        {
            ((global::DynamicMonoILR)o).FloatParamList = (System.Collections.Generic.List<System.Single>)v;
        }

        static StackObject* AssignFromStack_FloatParamList_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<System.Single> @FloatParamList = (System.Collections.Generic.List<System.Single>)typeof(System.Collections.Generic.List<System.Single>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DynamicMonoILR)o).FloatParamList = @FloatParamList;
            return ptr_of_this_method;
        }

        static object get_hotfixClassPath_6(ref object o)
        {
            return ((global::DynamicMonoILR)o).hotfixClassPath;
        }

        static StackObject* CopyToStack_hotfixClassPath_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DynamicMonoILR)o).hotfixClassPath;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_hotfixClassPath_6(ref object o, object v)
        {
            ((global::DynamicMonoILR)o).hotfixClassPath = (System.String)v;
        }

        static StackObject* AssignFromStack_hotfixClassPath_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @hotfixClassPath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DynamicMonoILR)o).hotfixClassPath = @hotfixClassPath;
            return ptr_of_this_method;
        }

        static object get_Vector3ParamList_7(ref object o)
        {
            return ((global::DynamicMonoILR)o).Vector3ParamList;
        }

        static StackObject* CopyToStack_Vector3ParamList_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DynamicMonoILR)o).Vector3ParamList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Vector3ParamList_7(ref object o, object v)
        {
            ((global::DynamicMonoILR)o).Vector3ParamList = (System.Collections.Generic.List<UnityEngine.Vector3>)v;
        }

        static StackObject* AssignFromStack_Vector3ParamList_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<UnityEngine.Vector3> @Vector3ParamList = (System.Collections.Generic.List<UnityEngine.Vector3>)typeof(System.Collections.Generic.List<UnityEngine.Vector3>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DynamicMonoILR)o).Vector3ParamList = @Vector3ParamList;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new global::DynamicMonoILR();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
