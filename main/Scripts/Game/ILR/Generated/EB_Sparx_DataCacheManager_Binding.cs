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
    unsafe class EB_Sparx_DataCacheManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(EB.Sparx.DataCacheManager);
            args = new Type[]{typeof(System.String), typeof(EB.Sparx.DataCacheManager.OnJsonDataCacheUpdated)};
            method = type.GetMethod("RegisterJsonEntity", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterJsonEntity_0);
            args = new Type[]{typeof(System.String), typeof(EB.Sparx.DataCacheManager.OnFlatBuffersDataCacheUpdated)};
            method = type.GetMethod("RegisterFlatBuffersEntity", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterFlatBuffersEntity_1);
            args = new Type[]{};
            method = type.GetMethod("LoadAll", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LoadAll_2);
            args = new Type[]{};
            method = type.GetMethod("GetVersions", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetVersions_3);
            args = new Type[]{typeof(System.Collections.Hashtable), typeof(System.Action<System.String>), typeof(System.Action<System.Int32, System.String>)};
            method = type.GetMethod("ProcessCaches", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ProcessCaches_4);
            args = new Type[]{typeof(System.String), typeof(System.String), typeof(System.ArraySegment<System.Byte>)};
            method = type.GetMethod("ProcessCache", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ProcessCache_5);

            field = type.GetField("OnBufferHandler", flag);
            app.RegisterCLRFieldGetter(field, get_OnBufferHandler_0);
            app.RegisterCLRFieldSetter(field, set_OnBufferHandler_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_OnBufferHandler_0, AssignFromStack_OnBufferHandler_0);


        }


        static StackObject* RegisterJsonEntity_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.DataCacheManager.OnJsonDataCacheUpdated @onupdated = (EB.Sparx.DataCacheManager.OnJsonDataCacheUpdated)typeof(EB.Sparx.DataCacheManager.OnJsonDataCacheUpdated).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @name = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            EB.Sparx.DataCacheManager instance_of_this_method = (EB.Sparx.DataCacheManager)typeof(EB.Sparx.DataCacheManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RegisterJsonEntity(@name, @onupdated);

            return __ret;
        }

        static StackObject* RegisterFlatBuffersEntity_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.DataCacheManager.OnFlatBuffersDataCacheUpdated @onupdated = (EB.Sparx.DataCacheManager.OnFlatBuffersDataCacheUpdated)typeof(EB.Sparx.DataCacheManager.OnFlatBuffersDataCacheUpdated).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @name = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            EB.Sparx.DataCacheManager instance_of_this_method = (EB.Sparx.DataCacheManager)typeof(EB.Sparx.DataCacheManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RegisterFlatBuffersEntity(@name, @onupdated);

            return __ret;
        }

        static StackObject* LoadAll_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.DataCacheManager instance_of_this_method = (EB.Sparx.DataCacheManager)typeof(EB.Sparx.DataCacheManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.LoadAll();

            return __ret;
        }

        static StackObject* GetVersions_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            EB.Sparx.DataCacheManager instance_of_this_method = (EB.Sparx.DataCacheManager)typeof(EB.Sparx.DataCacheManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetVersions();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ProcessCaches_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.Int32, System.String> @addPrgAct = (System.Action<System.Int32, System.String>)typeof(System.Action<System.Int32, System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action<System.String> @callback = (System.Action<System.String>)typeof(System.Action<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Collections.Hashtable @configs = (System.Collections.Hashtable)typeof(System.Collections.Hashtable).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            EB.Sparx.DataCacheManager instance_of_this_method = (EB.Sparx.DataCacheManager)typeof(EB.Sparx.DataCacheManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ProcessCaches(@configs, @callback, @addPrgAct);

            return __ret;
        }

        static StackObject* ProcessCache_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.ArraySegment<System.Byte> @buffer = (System.ArraySegment<System.Byte>)typeof(System.ArraySegment<System.Byte>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @version = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @name = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            EB.Sparx.DataCacheManager instance_of_this_method = (EB.Sparx.DataCacheManager)typeof(EB.Sparx.DataCacheManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ProcessCache(@name, @version, @buffer);

            return __ret;
        }


        static object get_OnBufferHandler_0(ref object o)
        {
            return ((EB.Sparx.DataCacheManager)o).OnBufferHandler;
        }

        static StackObject* CopyToStack_OnBufferHandler_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((EB.Sparx.DataCacheManager)o).OnBufferHandler;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_OnBufferHandler_0(ref object o, object v)
        {
            ((EB.Sparx.DataCacheManager)o).OnBufferHandler = (System.Action<FlatBuffers.ByteBuffer>)v;
        }

        static StackObject* AssignFromStack_OnBufferHandler_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action<FlatBuffers.ByteBuffer> @OnBufferHandler = (System.Action<FlatBuffers.ByteBuffer>)typeof(System.Action<FlatBuffers.ByteBuffer>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((EB.Sparx.DataCacheManager)o).OnBufferHandler = @OnBufferHandler;
            return ptr_of_this_method;
        }



    }
}
