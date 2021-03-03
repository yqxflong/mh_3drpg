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
    unsafe class SceneRootEntry_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::SceneRootEntry);
            args = new Type[]{};
            method = type.GetMethod("IsLoaded", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsLoaded_0);
            Dictionary<string, List<MethodInfo>> genericMethods = new Dictionary<string, List<MethodInfo>>();
            List<MethodInfo> lst = null;                    
            foreach(var m in type.GetMethods())
            {
                if(m.IsGenericMethodDefinition)
                {
                    if (!genericMethods.TryGetValue(m.Name, out lst))
                    {
                        lst = new List<MethodInfo>();
                        genericMethods[m.Name] = lst;
                    }
                    lst.Add(m);
                }
            }
            args = new Type[]{typeof(global::ThemeLoadManager)};
            if (genericMethods.TryGetValue("GetComponentInChildren", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(global::ThemeLoadManager)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, GetComponentInChildren_1);

                        break;
                    }
                }
            }
            args = new Type[]{};
            method = type.GetMethod("ShowLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ShowLevel_2);
            args = new Type[]{};
            method = type.GetMethod("SetZonesTag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetZonesTag_3);
            args = new Type[]{};
            method = type.GetMethod("SetMainLight", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetMainLight_4);
            args = new Type[]{typeof(global::PlayerCameraComponent)};
            if (genericMethods.TryGetValue("GetComponentInChildren", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(global::PlayerCameraComponent)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, GetComponentInChildren_5);

                        break;
                    }
                }
            }

            field = type.GetField("m_SceneRoot", flag);
            app.RegisterCLRFieldGetter(field, get_m_SceneRoot_0);
            app.RegisterCLRFieldSetter(field, set_m_SceneRoot_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_SceneRoot_0, AssignFromStack_m_SceneRoot_0);
            field = type.GetField("m_LevelAssets", flag);
            app.RegisterCLRFieldGetter(field, get_m_LevelAssets_1);
            app.RegisterCLRFieldSetter(field, set_m_LevelAssets_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_m_LevelAssets_1, AssignFromStack_m_LevelAssets_1);


        }


        static StackObject* IsLoaded_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::SceneRootEntry instance_of_this_method = (global::SceneRootEntry)typeof(global::SceneRootEntry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsLoaded();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* GetComponentInChildren_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::SceneRootEntry instance_of_this_method = (global::SceneRootEntry)typeof(global::SceneRootEntry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetComponentInChildren<global::ThemeLoadManager>();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ShowLevel_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::SceneRootEntry instance_of_this_method = (global::SceneRootEntry)typeof(global::SceneRootEntry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ShowLevel();

            return __ret;
        }

        static StackObject* SetZonesTag_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::SceneRootEntry instance_of_this_method = (global::SceneRootEntry)typeof(global::SceneRootEntry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetZonesTag();

            return __ret;
        }

        static StackObject* SetMainLight_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::SceneRootEntry instance_of_this_method = (global::SceneRootEntry)typeof(global::SceneRootEntry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetMainLight();

            return __ret;
        }

        static StackObject* GetComponentInChildren_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::SceneRootEntry instance_of_this_method = (global::SceneRootEntry)typeof(global::SceneRootEntry).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetComponentInChildren<global::PlayerCameraComponent>();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_m_SceneRoot_0(ref object o)
        {
            return ((global::SceneRootEntry)o).m_SceneRoot;
        }

        static StackObject* CopyToStack_m_SceneRoot_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::SceneRootEntry)o).m_SceneRoot;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_SceneRoot_0(ref object o, object v)
        {
            ((global::SceneRootEntry)o).m_SceneRoot = (UnityEngine.GameObject)v;
        }

        static StackObject* AssignFromStack_m_SceneRoot_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.GameObject @m_SceneRoot = (UnityEngine.GameObject)typeof(UnityEngine.GameObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::SceneRootEntry)o).m_SceneRoot = @m_SceneRoot;
            return ptr_of_this_method;
        }

        static object get_m_LevelAssets_1(ref object o)
        {
            return ((global::SceneRootEntry)o).m_LevelAssets;
        }

        static StackObject* CopyToStack_m_LevelAssets_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::SceneRootEntry)o).m_LevelAssets;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_m_LevelAssets_1(ref object o, object v)
        {
            ((global::SceneRootEntry)o).m_LevelAssets = (global::SceneRootEntry.PlacedAsset[])v;
        }

        static StackObject* AssignFromStack_m_LevelAssets_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::SceneRootEntry.PlacedAsset[] @m_LevelAssets = (global::SceneRootEntry.PlacedAsset[])typeof(global::SceneRootEntry.PlacedAsset[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::SceneRootEntry)o).m_LevelAssets = @m_LevelAssets;
            return ptr_of_this_method;
        }



    }
}
