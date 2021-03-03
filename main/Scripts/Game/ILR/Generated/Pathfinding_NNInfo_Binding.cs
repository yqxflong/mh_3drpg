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
    unsafe class Pathfinding_NNInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Pathfinding.NNInfo);

            field = type.GetField("node", flag);
            app.RegisterCLRFieldGetter(field, get_node_0);
            app.RegisterCLRFieldSetter(field, set_node_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_node_0, AssignFromStack_node_0);
            field = type.GetField("clampedPosition", flag);
            app.RegisterCLRFieldGetter(field, get_clampedPosition_1);
            app.RegisterCLRFieldSetter(field, set_clampedPosition_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_clampedPosition_1, AssignFromStack_clampedPosition_1);

            app.RegisterCLRCreateDefaultInstance(type, () => new Pathfinding.NNInfo());


        }

        static void WriteBackInstance(ILRuntime.Runtime.Enviorment.AppDomain __domain, StackObject* ptr_of_this_method, IList<object> __mStack, ref Pathfinding.NNInfo instance_of_this_method)
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
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as Pathfinding.NNInfo[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = instance_of_this_method;
                    }
                    break;
            }
        }


        static object get_node_0(ref object o)
        {
            return ((Pathfinding.NNInfo)o).node;
        }

        static StackObject* CopyToStack_node_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Pathfinding.NNInfo)o).node;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_node_0(ref object o, object v)
        {
            Pathfinding.NNInfo ins =(Pathfinding.NNInfo)o;
            ins.node = (Pathfinding.GraphNode)v;
            o = ins;
        }

        static StackObject* AssignFromStack_node_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            Pathfinding.GraphNode @node = (Pathfinding.GraphNode)typeof(Pathfinding.GraphNode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            Pathfinding.NNInfo ins =(Pathfinding.NNInfo)o;
            ins.node = @node;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_clampedPosition_1(ref object o)
        {
            return ((Pathfinding.NNInfo)o).clampedPosition;
        }

        static StackObject* CopyToStack_clampedPosition_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Pathfinding.NNInfo)o).clampedPosition;
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.PushValue(ref result_of_this_method, __intp, __ret, __mStack);
                return __ret + 1;
            } else {
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
            }
        }

        static void set_clampedPosition_1(ref object o, object v)
        {
            Pathfinding.NNInfo ins =(Pathfinding.NNInfo)o;
            ins.clampedPosition = (UnityEngine.Vector3)v;
            o = ins;
        }

        static StackObject* AssignFromStack_clampedPosition_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector3 @clampedPosition = new UnityEngine.Vector3();
            if (ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder != null) {
                ILRuntime.Runtime.Generated.CLRBindings.s_UnityEngine_Vector3_Binding_Binder.ParseValue(ref @clampedPosition, __intp, ptr_of_this_method, __mStack, true);
            } else {
                @clampedPosition = (UnityEngine.Vector3)typeof(UnityEngine.Vector3).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            }
            Pathfinding.NNInfo ins =(Pathfinding.NNInfo)o;
            ins.clampedPosition = @clampedPosition;
            o = ins;
            return ptr_of_this_method;
        }



    }
}
