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
    unsafe class UITabController_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UITabController);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SelectTab", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SelectTab_0);
            args = new Type[]{};
            method = type.GetMethod("InitTab", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InitTab_1);

            field = type.GetField("TabLibPrefabs", flag);
            app.RegisterCLRFieldGetter(field, get_TabLibPrefabs_0);
            app.RegisterCLRFieldSetter(field, set_TabLibPrefabs_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_TabLibPrefabs_0, AssignFromStack_TabLibPrefabs_0);


        }


        static StackObject* SelectTab_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UITabController instance_of_this_method = (global::UITabController)typeof(global::UITabController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SelectTab(@index);

            return __ret;
        }

        static StackObject* InitTab_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UITabController instance_of_this_method = (global::UITabController)typeof(global::UITabController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.InitTab();

            return __ret;
        }


        static object get_TabLibPrefabs_0(ref object o)
        {
            return ((global::UITabController)o).TabLibPrefabs;
        }

        static StackObject* CopyToStack_TabLibPrefabs_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UITabController)o).TabLibPrefabs;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_TabLibPrefabs_0(ref object o, object v)
        {
            ((global::UITabController)o).TabLibPrefabs = (System.Collections.Generic.List<global::UITabController.TabLibEntry>)v;
        }

        static StackObject* AssignFromStack_TabLibPrefabs_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<global::UITabController.TabLibEntry> @TabLibPrefabs = (System.Collections.Generic.List<global::UITabController.TabLibEntry>)typeof(System.Collections.Generic.List<global::UITabController.TabLibEntry>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UITabController)o).TabLibPrefabs = @TabLibPrefabs;
            return ptr_of_this_method;
        }



    }
}
