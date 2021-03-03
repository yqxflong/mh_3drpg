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
    unsafe class UIGrid_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIGrid);
            args = new Type[]{};
            method = type.GetMethod("Reposition", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Reposition_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_repositionNow", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_repositionNow_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("GetChild", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetChild_2);
            args = new Type[]{};
            method = type.GetMethod("GetChildList", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetChildList_3);
            args = new Type[]{typeof(UnityEngine.Transform)};
            method = type.GetMethod("AddChild", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddChild_4);

            field = type.GetField("cellWidth", flag);
            app.RegisterCLRFieldGetter(field, get_cellWidth_0);
            app.RegisterCLRFieldSetter(field, set_cellWidth_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_cellWidth_0, AssignFromStack_cellWidth_0);
            field = type.GetField("maxPerLine", flag);
            app.RegisterCLRFieldGetter(field, get_maxPerLine_1);
            app.RegisterCLRFieldSetter(field, set_maxPerLine_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_maxPerLine_1, AssignFromStack_maxPerLine_1);
            field = type.GetField("cellHeight", flag);
            app.RegisterCLRFieldGetter(field, get_cellHeight_2);
            app.RegisterCLRFieldSetter(field, set_cellHeight_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_cellHeight_2, AssignFromStack_cellHeight_2);
            field = type.GetField("arrangement", flag);
            app.RegisterCLRFieldGetter(field, get_arrangement_3);
            app.RegisterCLRFieldSetter(field, set_arrangement_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_arrangement_3, AssignFromStack_arrangement_3);
            field = type.GetField("hideInactive", flag);
            app.RegisterCLRFieldGetter(field, get_hideInactive_4);
            app.RegisterCLRFieldSetter(field, set_hideInactive_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_hideInactive_4, AssignFromStack_hideInactive_4);


        }


        static StackObject* Reposition_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIGrid instance_of_this_method = (global::UIGrid)typeof(global::UIGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Reposition();

            return __ret;
        }

        static StackObject* set_repositionNow_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIGrid instance_of_this_method = (global::UIGrid)typeof(global::UIGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.repositionNow = value;

            return __ret;
        }

        static StackObject* GetChild_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIGrid instance_of_this_method = (global::UIGrid)typeof(global::UIGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetChild(@index);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetChildList_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIGrid instance_of_this_method = (global::UIGrid)typeof(global::UIGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetChildList();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* AddChild_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Transform @trans = (UnityEngine.Transform)typeof(UnityEngine.Transform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIGrid instance_of_this_method = (global::UIGrid)typeof(global::UIGrid).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddChild(@trans);

            return __ret;
        }


        static object get_cellWidth_0(ref object o)
        {
            return ((global::UIGrid)o).cellWidth;
        }

        static StackObject* CopyToStack_cellWidth_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIGrid)o).cellWidth;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_cellWidth_0(ref object o, object v)
        {
            ((global::UIGrid)o).cellWidth = (System.Single)v;
        }

        static StackObject* AssignFromStack_cellWidth_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @cellWidth = *(float*)&ptr_of_this_method->Value;
            ((global::UIGrid)o).cellWidth = @cellWidth;
            return ptr_of_this_method;
        }

        static object get_maxPerLine_1(ref object o)
        {
            return ((global::UIGrid)o).maxPerLine;
        }

        static StackObject* CopyToStack_maxPerLine_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIGrid)o).maxPerLine;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_maxPerLine_1(ref object o, object v)
        {
            ((global::UIGrid)o).maxPerLine = (System.Int32)v;
        }

        static StackObject* AssignFromStack_maxPerLine_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @maxPerLine = ptr_of_this_method->Value;
            ((global::UIGrid)o).maxPerLine = @maxPerLine;
            return ptr_of_this_method;
        }

        static object get_cellHeight_2(ref object o)
        {
            return ((global::UIGrid)o).cellHeight;
        }

        static StackObject* CopyToStack_cellHeight_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIGrid)o).cellHeight;
            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_cellHeight_2(ref object o, object v)
        {
            ((global::UIGrid)o).cellHeight = (System.Single)v;
        }

        static StackObject* AssignFromStack_cellHeight_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Single @cellHeight = *(float*)&ptr_of_this_method->Value;
            ((global::UIGrid)o).cellHeight = @cellHeight;
            return ptr_of_this_method;
        }

        static object get_arrangement_3(ref object o)
        {
            return ((global::UIGrid)o).arrangement;
        }

        static StackObject* CopyToStack_arrangement_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIGrid)o).arrangement;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_arrangement_3(ref object o, object v)
        {
            ((global::UIGrid)o).arrangement = (global::UIGrid.Arrangement)v;
        }

        static StackObject* AssignFromStack_arrangement_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIGrid.Arrangement @arrangement = (global::UIGrid.Arrangement)typeof(global::UIGrid.Arrangement).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIGrid)o).arrangement = @arrangement;
            return ptr_of_this_method;
        }

        static object get_hideInactive_4(ref object o)
        {
            return ((global::UIGrid)o).hideInactive;
        }

        static StackObject* CopyToStack_hideInactive_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIGrid)o).hideInactive;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_hideInactive_4(ref object o, object v)
        {
            ((global::UIGrid)o).hideInactive = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_hideInactive_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @hideInactive = ptr_of_this_method->Value == 1;
            ((global::UIGrid)o).hideInactive = @hideInactive;
            return ptr_of_this_method;
        }



    }
}
