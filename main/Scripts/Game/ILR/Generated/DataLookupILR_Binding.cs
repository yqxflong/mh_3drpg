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
    unsafe class DataLookupILR_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::DataLookupILR);

            field = type.GetField("ObjectParamList", flag);
            app.RegisterCLRFieldGetter(field, get_ObjectParamList_0);
            app.RegisterCLRFieldSetter(field, set_ObjectParamList_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_ObjectParamList_0, AssignFromStack_ObjectParamList_0);
            field = type.GetField("BoolParamList", flag);
            app.RegisterCLRFieldGetter(field, get_BoolParamList_1);
            app.RegisterCLRFieldSetter(field, set_BoolParamList_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_BoolParamList_1, AssignFromStack_BoolParamList_1);
            field = type.GetField("StringParamList", flag);
            app.RegisterCLRFieldGetter(field, get_StringParamList_2);
            app.RegisterCLRFieldSetter(field, set_StringParamList_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_StringParamList_2, AssignFromStack_StringParamList_2);
            field = type.GetField("hotfixClassPath", flag);
            app.RegisterCLRFieldGetter(field, get_hotfixClassPath_3);
            app.RegisterCLRFieldSetter(field, set_hotfixClassPath_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_hotfixClassPath_3, AssignFromStack_hotfixClassPath_3);


        }



        static object get_ObjectParamList_0(ref object o)
        {
            return ((global::DataLookupILR)o).ObjectParamList;
        }

        static StackObject* CopyToStack_ObjectParamList_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DataLookupILR)o).ObjectParamList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ObjectParamList_0(ref object o, object v)
        {
            ((global::DataLookupILR)o).ObjectParamList = (System.Collections.Generic.List<UnityEngine.Object>)v;
        }

        static StackObject* AssignFromStack_ObjectParamList_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<UnityEngine.Object> @ObjectParamList = (System.Collections.Generic.List<UnityEngine.Object>)typeof(System.Collections.Generic.List<UnityEngine.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DataLookupILR)o).ObjectParamList = @ObjectParamList;
            return ptr_of_this_method;
        }

        static object get_BoolParamList_1(ref object o)
        {
            return ((global::DataLookupILR)o).BoolParamList;
        }

        static StackObject* CopyToStack_BoolParamList_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DataLookupILR)o).BoolParamList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_BoolParamList_1(ref object o, object v)
        {
            ((global::DataLookupILR)o).BoolParamList = (System.Collections.Generic.List<System.Boolean>)v;
        }

        static StackObject* AssignFromStack_BoolParamList_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<System.Boolean> @BoolParamList = (System.Collections.Generic.List<System.Boolean>)typeof(System.Collections.Generic.List<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DataLookupILR)o).BoolParamList = @BoolParamList;
            return ptr_of_this_method;
        }

        static object get_StringParamList_2(ref object o)
        {
            return ((global::DataLookupILR)o).StringParamList;
        }

        static StackObject* CopyToStack_StringParamList_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DataLookupILR)o).StringParamList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_StringParamList_2(ref object o, object v)
        {
            ((global::DataLookupILR)o).StringParamList = (System.Collections.Generic.List<System.String>)v;
        }

        static StackObject* AssignFromStack_StringParamList_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<System.String> @StringParamList = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DataLookupILR)o).StringParamList = @StringParamList;
            return ptr_of_this_method;
        }

        static object get_hotfixClassPath_3(ref object o)
        {
            return ((global::DataLookupILR)o).hotfixClassPath;
        }

        static StackObject* CopyToStack_hotfixClassPath_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::DataLookupILR)o).hotfixClassPath;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_hotfixClassPath_3(ref object o, object v)
        {
            ((global::DataLookupILR)o).hotfixClassPath = (System.String)v;
        }

        static StackObject* AssignFromStack_hotfixClassPath_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @hotfixClassPath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::DataLookupILR)o).hotfixClassPath = @hotfixClassPath;
            return ptr_of_this_method;
        }



    }
}
