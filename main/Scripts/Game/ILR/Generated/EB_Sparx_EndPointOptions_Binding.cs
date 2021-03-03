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
    unsafe class EB_Sparx_EndPointOptions_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.EndPointOptions);

            field = type.GetField("KeepAlive", flag);
            app.RegisterCLRFieldGetter(field, get_KeepAlive_0);
            app.RegisterCLRFieldSetter(field, set_KeepAlive_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_KeepAlive_0, AssignFromStack_KeepAlive_0);
            field = type.GetField("Key", flag);
            app.RegisterCLRFieldGetter(field, get_Key_1);
            app.RegisterCLRFieldSetter(field, set_Key_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_Key_1, AssignFromStack_Key_1);

            app.RegisterCLRMemberwiseClone(type, PerformMemberwiseClone);

            app.RegisterCLRCreateDefaultInstance(type, () => new EB.Sparx.EndPointOptions());


        }

        static void WriteBackInstance(ILRuntime.Runtime.Enviorment.AppDomain __domain, StackObject* ptr_of_this_method, IList<object> __mStack, ref EB.Sparx.EndPointOptions instance_of_this_method)
        {
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            switch(ptr_of_this_method->ObjectType)
            {
                case ObjectTypes.Object:
                    {
                        __mStack[ptr_of_this_method->Value] = instance_of_this_method;
                    }
                    break;
                case ObjectTypes.FieldReference:
                    {
                        var ___obj = __mStack[ptr_of_this_method->Value];
                        if(___obj is ILTypeInstance)
                        {
                            ((ILTypeInstance)___obj)[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            var t = __domain.GetType(___obj.GetType()) as CLRType;
                            t.SetFieldValue(ptr_of_this_method->ValueLow, ref ___obj, instance_of_this_method);
                        }
                    }
                    break;
                case ObjectTypes.StaticFieldReference:
                    {
                        var t = __domain.GetType(ptr_of_this_method->Value);
                        if(t is ILType)
                        {
                            ((ILType)t).StaticInstance[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            ((CLRType)t).SetStaticFieldValue(ptr_of_this_method->ValueLow, instance_of_this_method);
                        }
                    }
                    break;
                 case ObjectTypes.ArrayReference:
                    {
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as EB.Sparx.EndPointOptions[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = instance_of_this_method;
                    }
                    break;
            }
        }


        static object get_KeepAlive_0(ref object o)
        {
            return ((EB.Sparx.EndPointOptions)o).KeepAlive;
        }

        static StackObject* CopyToStack_KeepAlive_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.EndPointOptions)o).KeepAlive;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_KeepAlive_0(ref object o, object v)
        {
            EB.Sparx.EndPointOptions ins =(EB.Sparx.EndPointOptions)o;
            ins.KeepAlive = (System.Boolean)v;
            o = ins;
        }

        static StackObject* AssignFromStack_KeepAlive_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @KeepAlive = ptr_of_this_method->Value == 1;
            EB.Sparx.EndPointOptions ins =(EB.Sparx.EndPointOptions)o;
            ins.KeepAlive = @KeepAlive;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_Key_1(ref object o)
        {
            return ((EB.Sparx.EndPointOptions)o).Key;
        }

        static StackObject* CopyToStack_Key_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.EndPointOptions)o).Key;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Key_1(ref object o, object v)
        {
            EB.Sparx.EndPointOptions ins =(EB.Sparx.EndPointOptions)o;
            ins.Key = (System.Byte[])v;
            o = ins;
        }

        static StackObject* AssignFromStack_Key_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Byte[] @Key = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            EB.Sparx.EndPointOptions ins =(EB.Sparx.EndPointOptions)o;
            ins.Key = @Key;
            o = ins;
            return ptr_of_this_method;
        }


        static object PerformMemberwiseClone(ref object o)
        {
            var ins = new EB.Sparx.EndPointOptions();
            ins = (EB.Sparx.EndPointOptions)o;
            return ins;
        }


    }
}
