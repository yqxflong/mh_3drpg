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
    unsafe class FlatBuffers_FlatBufferBuilder_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(FlatBuffers.FlatBufferBuilder);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("StartObject", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StartObject_0);
            args = new Type[]{typeof(System.Int32), typeof(System.Int32), typeof(System.Int32)};
            method = type.GetMethod("AddInt", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddInt_1);
            args = new Type[]{typeof(System.Int32), typeof(System.Int32), typeof(System.Int32)};
            method = type.GetMethod("AddOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddOffset_2);
            args = new Type[]{};
            method = type.GetMethod("EndObject", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EndObject_3);
            args = new Type[]{typeof(System.Int32), typeof(System.Single), typeof(System.Double)};
            method = type.GetMethod("AddFloat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddFloat_4);
            args = new Type[]{typeof(System.Int32), typeof(System.Boolean), typeof(System.Boolean)};
            method = type.GetMethod("AddBool", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddBool_5);
            args = new Type[]{typeof(System.Int32), typeof(System.Int32), typeof(System.Int32)};
            method = type.GetMethod("StartVector", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StartVector_6);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("AddOffset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddOffset_7);
            args = new Type[]{};
            method = type.GetMethod("EndVector", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EndVector_8);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("Finish", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Finish_9);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("AddInt", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddInt_10);
            args = new Type[]{typeof(System.Int32), typeof(System.SByte), typeof(System.SByte)};
            method = type.GetMethod("AddSbyte", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddSbyte_11);
            args = new Type[]{typeof(System.SByte)};
            method = type.GetMethod("AddSbyte", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddSbyte_12);


        }


        static StackObject* StartObject_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @numfields = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StartObject(@numfields);

            return __ret;
        }

        static StackObject* AddInt_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @d = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @x = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @o = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddInt(@o, @x, @d);

            return __ret;
        }

        static StackObject* AddOffset_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @d = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @x = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @o = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddOffset(@o, @x, @d);

            return __ret;
        }

        static StackObject* EndObject_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.EndObject();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* AddFloat_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Double @d = *(double*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @x = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @o = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddFloat(@o, @x, @d);

            return __ret;
        }

        static StackObject* AddBool_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @d = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @x = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @o = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddBool(@o, @x, @d);

            return __ret;
        }

        static StackObject* StartVector_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @alignment = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @count = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @elemSize = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StartVector(@elemSize, @count, @alignment);

            return __ret;
        }

        static StackObject* AddOffset_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @off = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddOffset(@off);

            return __ret;
        }

        static StackObject* EndVector_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.EndVector();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Finish_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @rootTable = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Finish(@rootTable);

            return __ret;
        }

        static StackObject* AddInt_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @x = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddInt(@x);

            return __ret;
        }

        static StackObject* AddSbyte_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.SByte @d = (sbyte)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.SByte @x = (sbyte)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @o = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddSbyte(@o, @x, @d);

            return __ret;
        }

        static StackObject* AddSbyte_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.SByte @x = (sbyte)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            FlatBuffers.FlatBufferBuilder instance_of_this_method = (FlatBuffers.FlatBufferBuilder)typeof(FlatBuffers.FlatBufferBuilder).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddSbyte(@x);

            return __ret;
        }



    }
}
