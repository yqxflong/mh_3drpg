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
    unsafe class RenderSettingsManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::RenderSettingsManager);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);
            args = new Type[]{typeof(System.String), typeof(global::RenderSettings)};
            method = type.GetMethod("SetActiveRenderSettings", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetActiveRenderSettings_1);
            args = new Type[]{};
            method = type.GetMethod("ResetRecoverSetting", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ResetRecoverSetting_2);

            field = type.GetField("defaultSettings", flag);
            app.RegisterCLRFieldGetter(field, get_defaultSettings_0);
            app.RegisterCLRFieldSetter(field, set_defaultSettings_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_defaultSettings_0, AssignFromStack_defaultSettings_0);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::RenderSettingsManager.Instance;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetActiveRenderSettings_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::RenderSettings @rs = (global::RenderSettings)typeof(global::RenderSettings).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @name = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::RenderSettingsManager instance_of_this_method = (global::RenderSettingsManager)typeof(global::RenderSettingsManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetActiveRenderSettings(@name, @rs);

            return __ret;
        }

        static StackObject* ResetRecoverSetting_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::RenderSettingsManager instance_of_this_method = (global::RenderSettingsManager)typeof(global::RenderSettingsManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ResetRecoverSetting();

            return __ret;
        }


        static object get_defaultSettings_0(ref object o)
        {
            return ((global::RenderSettingsManager)o).defaultSettings;
        }

        static StackObject* CopyToStack_defaultSettings_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::RenderSettingsManager)o).defaultSettings;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_defaultSettings_0(ref object o, object v)
        {
            ((global::RenderSettingsManager)o).defaultSettings = (System.String)v;
        }

        static StackObject* AssignFromStack_defaultSettings_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @defaultSettings = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::RenderSettingsManager)o).defaultSettings = @defaultSettings;
            return ptr_of_this_method;
        }



    }
}
