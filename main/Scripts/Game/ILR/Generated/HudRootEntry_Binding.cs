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
    unsafe class HudRootEntry_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::HudRootEntry);
            args = new Type[]{};
            method = type.GetMethod("Show", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Show_0);

            field = type.GetField("m_Root", flag);
            app.RegisterCLRFieldGetter(field, get_m_Root_0);
            app.RegisterCLRFieldSetter(field, set_m_Root_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_Root_0, AssignFromStack_m_Root_0);


        }


        static StackObject* Show_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::HudRootEntry instance_of_this_method = (global::HudRootEntry)typeof(global::HudRootEntry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Show();

            return __ret;
        }


        static object get_m_Root_0(ref object o)
        {
            return ((global::HudRootEntry)o).m_Root;
        }

        static StackObject* CopyToStack_m_Root_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::HudRootEntry)o).m_Root;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_Root_0(ref object o, object v)
        {
            ((global::HudRootEntry)o).m_Root = (UnityEngine.GameObject)v;
        }

        static StackObject* AssignFromStack_m_Root_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.GameObject @m_Root = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::HudRootEntry)o).m_Root = @m_Root;
            return ptr_of_this_method;
        }



    }
}
