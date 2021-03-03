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
    unsafe class EB_Sparx_GachaConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.GachaConfig);

            field = type.GetField("Groups", flag);
            app.RegisterCLRFieldGetter(field, get_Groups_0);
            app.RegisterCLRFieldSetter(field, set_Groups_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Groups_0, AssignFromStack_Groups_0);


        }



        static object get_Groups_0(ref object o)
        {
            return ((EB.Sparx.GachaConfig)o).Groups;
        }

        static StackObject* CopyToStack_Groups_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.GachaConfig)o).Groups;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Groups_0(ref object o, object v)
        {
            ((EB.Sparx.GachaConfig)o).Groups = (System.String[])v;
        }

        static StackObject* AssignFromStack_Groups_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String[] @Groups = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.GachaConfig)o).Groups = @Groups;
            return ptr_of_this_method;
        }



    }
}
