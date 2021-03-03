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
    unsafe class AreaTriggersManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::AreaTriggersManager);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);

            field = type.GetField("AreaTriggerDict", flag);
            app.RegisterCLRFieldGetter(field, get_AreaTriggerDict_0);
            app.RegisterCLRFieldSetter(field, set_AreaTriggerDict_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_AreaTriggerDict_0, AssignFromStack_AreaTriggerDict_0);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::AreaTriggersManager.Instance;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_AreaTriggerDict_0(ref object o)
        {
            return ((global::AreaTriggersManager)o).AreaTriggerDict;
        }

        static StackObject* CopyToStack_AreaTriggerDict_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::AreaTriggersManager)o).AreaTriggerDict;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_AreaTriggerDict_0(ref object o, object v)
        {
            ((global::AreaTriggersManager)o).AreaTriggerDict = (System.Collections.Generic.Dictionary<System.String, UnityEngine.Transform>)v;
        }

        static StackObject* AssignFromStack_AreaTriggerDict_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, UnityEngine.Transform> @AreaTriggerDict = (System.Collections.Generic.Dictionary<System.String, UnityEngine.Transform>)typeof(System.Collections.Generic.Dictionary<System.String, UnityEngine.Transform>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::AreaTriggersManager)o).AreaTriggerDict = @AreaTriggerDict;
            return ptr_of_this_method;
        }



    }
}
