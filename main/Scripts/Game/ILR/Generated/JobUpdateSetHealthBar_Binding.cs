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
    unsafe class JobUpdateSetHealthBar_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::JobUpdateSetHealthBar);

            field = type.GetField("m_currentHP", flag);
            app.RegisterCLRFieldGetter(field, get_m_currentHP_0);
            app.RegisterCLRFieldSetter(field, set_m_currentHP_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_currentHP_0, AssignFromStack_m_currentHP_0);
            field = type.GetField("m_maxHP", flag);
            app.RegisterCLRFieldGetter(field, get_m_maxHP_1);
            app.RegisterCLRFieldSetter(field, set_m_maxHP_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_maxHP_1, AssignFromStack_m_maxHP_1);

            app.RegisterCLRMemberwiseClone(type, PerformMemberwiseClone);

            app.RegisterCLRCreateDefaultInstance(type, () => new global::JobUpdateSetHealthBar());


        }

        static void WriteBackInstance(ILRuntime.Runtime.Enviorment.AppDomain __domain, StackObject* ptr_of_this_method, IList<object> __mStack, ref global::JobUpdateSetHealthBar instance_of_this_method)
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
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as global::JobUpdateSetHealthBar[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = instance_of_this_method;
                    }
                    break;
            }
        }


        static object get_m_currentHP_0(ref object o)
        {
            return ((global::JobUpdateSetHealthBar)o).m_currentHP;
        }

        static StackObject* CopyToStack_m_currentHP_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::JobUpdateSetHealthBar)o).m_currentHP;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m_currentHP_0(ref object o, object v)
        {
            global::JobUpdateSetHealthBar ins =(global::JobUpdateSetHealthBar)o;
            ins.m_currentHP = (System.Int32)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m_currentHP_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @m_currentHP = ptr_of_this_method->Value;
            global::JobUpdateSetHealthBar ins =(global::JobUpdateSetHealthBar)o;
            ins.m_currentHP = @m_currentHP;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_m_maxHP_1(ref object o)
        {
            return ((global::JobUpdateSetHealthBar)o).m_maxHP;
        }

        static StackObject* CopyToStack_m_maxHP_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::JobUpdateSetHealthBar)o).m_maxHP;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_m_maxHP_1(ref object o, object v)
        {
            global::JobUpdateSetHealthBar ins =(global::JobUpdateSetHealthBar)o;
            ins.m_maxHP = (System.Int32)v;
            o = ins;
        }

        static StackObject* AssignFromStack_m_maxHP_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @m_maxHP = ptr_of_this_method->Value;
            global::JobUpdateSetHealthBar ins =(global::JobUpdateSetHealthBar)o;
            ins.m_maxHP = @m_maxHP;
            o = ins;
            return ptr_of_this_method;
        }


        static object PerformMemberwiseClone(ref object o)
        {
            var ins = new global::JobUpdateSetHealthBar();
            ins = (global::JobUpdateSetHealthBar)o;
            return ins;
        }


    }
}
