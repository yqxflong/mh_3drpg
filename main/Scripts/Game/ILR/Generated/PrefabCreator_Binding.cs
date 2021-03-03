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
    unsafe class PrefabCreator_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::PrefabCreator);
            args = new Type[]{};
            method = type.GetMethod("LoadAsset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LoadAsset_0);

            field = type.GetField("isCurrendAssetLoaded", flag);
            app.RegisterCLRFieldGetter(field, get_isCurrendAssetLoaded_0);
            app.RegisterCLRFieldSetter(field, set_isCurrendAssetLoaded_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_isCurrendAssetLoaded_0, AssignFromStack_isCurrendAssetLoaded_0);


        }


        static StackObject* LoadAsset_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::PrefabCreator instance_of_this_method = (global::PrefabCreator)typeof(global::PrefabCreator).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.LoadAsset();

            return __ret;
        }


        static object get_isCurrendAssetLoaded_0(ref object o)
        {
            return ((global::PrefabCreator)o).isCurrendAssetLoaded;
        }

        static StackObject* CopyToStack_isCurrendAssetLoaded_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::PrefabCreator)o).isCurrendAssetLoaded;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_isCurrendAssetLoaded_0(ref object o, object v)
        {
            ((global::PrefabCreator)o).isCurrendAssetLoaded = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_isCurrendAssetLoaded_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @isCurrendAssetLoaded = ptr_of_this_method->Value == 1;
            ((global::PrefabCreator)o).isCurrendAssetLoaded = @isCurrendAssetLoaded;
            return ptr_of_this_method;
        }



    }
}
