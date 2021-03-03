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
    unsafe class Main_MainLand_HeadBars2DMonitor_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Main.MainLand.HeadBars2DMonitor);
            args = new Type[]{typeof(UnityEngine.Transform)};
            method = type.GetMethod("SetLocator", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetLocator_0);
            args = new Type[]{};
            method = type.GetMethod("UpdatePosition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, UpdatePosition_1);

            field = type.GetField("Bars", flag);
            app.RegisterCLRFieldGetter(field, get_Bars_0);
            app.RegisterCLRFieldSetter(field, set_Bars_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_Bars_0, AssignFromStack_Bars_0);


        }


        static StackObject* SetLocator_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Transform @tran = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Main.MainLand.HeadBars2DMonitor instance_of_this_method = (Main.MainLand.HeadBars2DMonitor)typeof(Main.MainLand.HeadBars2DMonitor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetLocator(@tran);

            return __ret;
        }

        static StackObject* UpdatePosition_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Main.MainLand.HeadBars2DMonitor instance_of_this_method = (Main.MainLand.HeadBars2DMonitor)typeof(Main.MainLand.HeadBars2DMonitor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.UpdatePosition();

            return __ret;
        }


        static object get_Bars_0(ref object o)
        {
            return ((Main.MainLand.HeadBars2DMonitor)o).Bars;
        }

        static StackObject* CopyToStack_Bars_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Main.MainLand.HeadBars2DMonitor)o).Bars;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Bars_0(ref object o, object v)
        {
            ((Main.MainLand.HeadBars2DMonitor)o).Bars = (System.Collections.Generic.Dictionary<System.Int32, Main.MainLand.HeadBarHUDMonitor>)v;
        }

        static StackObject* AssignFromStack_Bars_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.Int32, Main.MainLand.HeadBarHUDMonitor> @Bars = (System.Collections.Generic.Dictionary<System.Int32, Main.MainLand.HeadBarHUDMonitor>)typeof(System.Collections.Generic.Dictionary<System.Int32, Main.MainLand.HeadBarHUDMonitor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Main.MainLand.HeadBars2DMonitor)o).Bars = @Bars;
            return ptr_of_this_method;
        }



    }
}
