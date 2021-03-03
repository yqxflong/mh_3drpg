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
    unsafe class UIControllerILR_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::UIControllerILR);
            args = new Type[]{typeof(System.Collections.Generic.List<System.String>), typeof(System.Collections.Generic.List<global::EventDelegate>)};
            method = type.GetMethod("FindAndBindingBtnEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, FindAndBindingBtnEvent_0);
            args = new Type[]{typeof(System.Collections.Generic.List<System.String>), typeof(System.Collections.Generic.List<global::EventDelegate>)};
            method = type.GetMethod("FindAndBindingCoolTriggerEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, FindAndBindingCoolTriggerEvent_1);
            args = new Type[]{};
            method = type.GetMethod("ILRObjInit", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ILRObjInit_2);
            args = new Type[]{typeof(System.Collections.Generic.List<System.String>), typeof(System.Collections.Generic.List<global::EventDelegate>)};
            method = type.GetMethod("BindingBtnEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BindingBtnEvent_3);
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
            args = new Type[]{typeof(global::UISprite)};
            if (genericMethods.TryGetValue("FetchComponentList", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(System.Collections.Generic.List<global::UISprite>), typeof(System.String[]), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, FetchComponentList_4);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(UnityEngine.GameObject)};
            if (genericMethods.TryGetValue("FetchComponentList", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(System.Collections.Generic.List<UnityEngine.GameObject>), typeof(System.String[]), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, FetchComponentList_5);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::UIToggle)};
            if (genericMethods.TryGetValue("FetchComponentList", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(System.Collections.Generic.List<global::UIToggle>), typeof(System.String[]), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, FetchComponentList_6);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::UIToggle[]), typeof(System.Collections.Generic.List<global::EventDelegate>)};
            method = type.GetMethod("BindingToggleEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BindingToggleEvent_7);
            args = new Type[]{typeof(global::UILabel)};
            if (genericMethods.TryGetValue("FetchComponentList", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(System.Collections.Generic.List<global::UILabel>), typeof(System.String[]), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, FetchComponentList_8);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(System.Collections.Generic.List<System.String>), typeof(System.Collections.Generic.List<global::EventDelegate>)};
            method = type.GetMethod("FindAndBindingTweenFinishedEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, FindAndBindingTweenFinishedEvent_9);
            args = new Type[]{typeof(System.Collections.Generic.List<System.String>), typeof(System.Collections.Generic.List<global::EventDelegate>)};
            method = type.GetMethod("BindingCoolTriggerEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BindingCoolTriggerEvent_10);
            args = new Type[]{typeof(UnityEngine.Transform)};
            if (genericMethods.TryGetValue("FetchComponentList", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(System.Collections.Generic.List<UnityEngine.Transform>), typeof(System.String[]), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, FetchComponentList_11);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::UIButton)};
            if (genericMethods.TryGetValue("FetchComponentList", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(System.Collections.Generic.List<global::UIButton>), typeof(System.String[]), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, FetchComponentList_12);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TweenPosition)};
            if (genericMethods.TryGetValue("FetchComponentList", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(System.Collections.Generic.List<global::TweenPosition>), typeof(System.String[]), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, FetchComponentList_13);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(UnityEngine.BoxCollider)};
            if (genericMethods.TryGetValue("FetchComponentList", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(System.Collections.Generic.List<UnityEngine.BoxCollider>), typeof(System.String[]), typeof(System.Boolean)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, FetchComponentList_14);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(System.Collections.Generic.List<System.String>), typeof(System.Collections.Generic.List<global::EventDelegate>), typeof(System.String)};
            method = type.GetMethod("FindAndBindingEventTriggerEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, FindAndBindingEventTriggerEvent_15);

            field = type.GetField("UiLabels", flag);
            app.RegisterCLRFieldGetter(field, get_UiLabels_0);
            app.RegisterCLRFieldSetter(field, set_UiLabels_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiLabels_0, AssignFromStack_UiLabels_0);
            field = type.GetField("CoolTriggers", flag);
            app.RegisterCLRFieldGetter(field, get_CoolTriggers_1);
            app.RegisterCLRFieldSetter(field, set_CoolTriggers_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_CoolTriggers_1, AssignFromStack_CoolTriggers_1);
            field = type.GetField("UiButtons", flag);
            app.RegisterCLRFieldGetter(field, get_UiButtons_2);
            app.RegisterCLRFieldSetter(field, set_UiButtons_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiButtons_2, AssignFromStack_UiButtons_2);
            field = type.GetField("GObjects", flag);
            app.RegisterCLRFieldGetter(field, get_GObjects_3);
            app.RegisterCLRFieldSetter(field, set_GObjects_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_GObjects_3, AssignFromStack_GObjects_3);
            field = type.GetField("hotfixClassPath", flag);
            app.RegisterCLRFieldGetter(field, get_hotfixClassPath_4);
            app.RegisterCLRFieldSetter(field, set_hotfixClassPath_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_hotfixClassPath_4, AssignFromStack_hotfixClassPath_4);
            field = type.GetField("UiGrids", flag);
            app.RegisterCLRFieldGetter(field, get_UiGrids_5);
            app.RegisterCLRFieldSetter(field, set_UiGrids_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiGrids_5, AssignFromStack_UiGrids_5);
            field = type.GetField("UiTables", flag);
            app.RegisterCLRFieldGetter(field, get_UiTables_6);
            app.RegisterCLRFieldSetter(field, set_UiTables_6);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiTables_6, AssignFromStack_UiTables_6);
            field = type.GetField("BoxColliders", flag);
            app.RegisterCLRFieldGetter(field, get_BoxColliders_7);
            app.RegisterCLRFieldSetter(field, set_BoxColliders_7);
            app.RegisterCLRFieldBinding(field, CopyToStack_BoxColliders_7, AssignFromStack_BoxColliders_7);
            field = type.GetField("UiSymbolInputs", flag);
            app.RegisterCLRFieldGetter(field, get_UiSymbolInputs_8);
            app.RegisterCLRFieldSetter(field, set_UiSymbolInputs_8);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiSymbolInputs_8, AssignFromStack_UiSymbolInputs_8);
            field = type.GetField("UiWidgets", flag);
            app.RegisterCLRFieldGetter(field, get_UiWidgets_9);
            app.RegisterCLRFieldSetter(field, set_UiWidgets_9);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiWidgets_9, AssignFromStack_UiWidgets_9);
            field = type.GetField("UiScrollViews", flag);
            app.RegisterCLRFieldGetter(field, get_UiScrollViews_10);
            app.RegisterCLRFieldSetter(field, set_UiScrollViews_10);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiScrollViews_10, AssignFromStack_UiScrollViews_10);
            field = type.GetField("UiSprites", flag);
            app.RegisterCLRFieldGetter(field, get_UiSprites_11);
            app.RegisterCLRFieldSetter(field, set_UiSprites_11);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiSprites_11, AssignFromStack_UiSprites_11);
            field = type.GetField("TweenPositions", flag);
            app.RegisterCLRFieldGetter(field, get_TweenPositions_12);
            app.RegisterCLRFieldSetter(field, set_TweenPositions_12);
            app.RegisterCLRFieldBinding(field, CopyToStack_TweenPositions_12, AssignFromStack_TweenPositions_12);
            field = type.GetField("ilinstance", flag);
            app.RegisterCLRFieldGetter(field, get_ilinstance_13);
            app.RegisterCLRFieldSetter(field, set_ilinstance_13);
            app.RegisterCLRFieldBinding(field, CopyToStack_ilinstance_13, AssignFromStack_ilinstance_13);
            field = type.GetField("DragEventDispatchers", flag);
            app.RegisterCLRFieldGetter(field, get_DragEventDispatchers_14);
            app.RegisterCLRFieldSetter(field, set_DragEventDispatchers_14);
            app.RegisterCLRFieldBinding(field, CopyToStack_DragEventDispatchers_14, AssignFromStack_DragEventDispatchers_14);
            field = type.GetField("ParticleSystemUiComponents", flag);
            app.RegisterCLRFieldGetter(field, get_ParticleSystemUiComponents_15);
            app.RegisterCLRFieldSetter(field, set_ParticleSystemUiComponents_15);
            app.RegisterCLRFieldBinding(field, CopyToStack_ParticleSystemUiComponents_15, AssignFromStack_ParticleSystemUiComponents_15);
            field = type.GetField("UiProgressBars", flag);
            app.RegisterCLRFieldGetter(field, get_UiProgressBars_16);
            app.RegisterCLRFieldSetter(field, set_UiProgressBars_16);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiProgressBars_16, AssignFromStack_UiProgressBars_16);
            field = type.GetField("TweenAlphas", flag);
            app.RegisterCLRFieldGetter(field, get_TweenAlphas_17);
            app.RegisterCLRFieldSetter(field, set_TweenAlphas_17);
            app.RegisterCLRFieldBinding(field, CopyToStack_TweenAlphas_17, AssignFromStack_TweenAlphas_17);
            field = type.GetField("Transforms", flag);
            app.RegisterCLRFieldGetter(field, get_Transforms_18);
            app.RegisterCLRFieldSetter(field, set_Transforms_18);
            app.RegisterCLRFieldBinding(field, CopyToStack_Transforms_18, AssignFromStack_Transforms_18);
            field = type.GetField("UiPanels", flag);
            app.RegisterCLRFieldGetter(field, get_UiPanels_19);
            app.RegisterCLRFieldSetter(field, set_UiPanels_19);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiPanels_19, AssignFromStack_UiPanels_19);
            field = type.GetField("TweenScales", flag);
            app.RegisterCLRFieldGetter(field, get_TweenScales_20);
            app.RegisterCLRFieldSetter(field, set_TweenScales_20);
            app.RegisterCLRFieldBinding(field, CopyToStack_TweenScales_20, AssignFromStack_TweenScales_20);
            field = type.GetField("ObjectParamList", flag);
            app.RegisterCLRFieldGetter(field, get_ObjectParamList_21);
            app.RegisterCLRFieldSetter(field, set_ObjectParamList_21);
            app.RegisterCLRFieldBinding(field, CopyToStack_ObjectParamList_21, AssignFromStack_ObjectParamList_21);
            field = type.GetField("TextureCmps", flag);
            app.RegisterCLRFieldGetter(field, get_TextureCmps_22);
            app.RegisterCLRFieldSetter(field, set_TextureCmps_22);
            app.RegisterCLRFieldBinding(field, CopyToStack_TextureCmps_22, AssignFromStack_TextureCmps_22);
            field = type.GetField("UiSliders", flag);
            app.RegisterCLRFieldGetter(field, get_UiSliders_23);
            app.RegisterCLRFieldSetter(field, set_UiSliders_23);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiSliders_23, AssignFromStack_UiSliders_23);
            field = type.GetField("UiEventTriggers", flag);
            app.RegisterCLRFieldGetter(field, get_UiEventTriggers_24);
            app.RegisterCLRFieldSetter(field, set_UiEventTriggers_24);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiEventTriggers_24, AssignFromStack_UiEventTriggers_24);
            field = type.GetField("UiTextures", flag);
            app.RegisterCLRFieldGetter(field, get_UiTextures_25);
            app.RegisterCLRFieldSetter(field, set_UiTextures_25);
            app.RegisterCLRFieldBinding(field, CopyToStack_UiTextures_25, AssignFromStack_UiTextures_25);


        }


        static StackObject* FindAndBindingBtnEvent_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Generic.List<global::EventDelegate> @eventList = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Generic.List<System.String> @pathList = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.FindAndBindingBtnEvent(@pathList, @eventList);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* FindAndBindingCoolTriggerEvent_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Generic.List<global::EventDelegate> @eventList = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Generic.List<System.String> @pathList = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.FindAndBindingCoolTriggerEvent(@pathList, @eventList);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ILRObjInit_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ILRObjInit();

            return __ret;
        }

        static StackObject* BindingBtnEvent_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Generic.List<global::EventDelegate> @eventList = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Generic.List<System.String> @keyList = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.BindingBtnEvent(@keyList, @eventList);

            return __ret;
        }

        static StackObject* FetchComponentList_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isGameObject = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String[] @paths = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.FetchComponentList<global::UISprite>(@paths, @isGameObject);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* FetchComponentList_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isGameObject = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String[] @paths = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.FetchComponentList<UnityEngine.GameObject>(@paths, @isGameObject);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* FetchComponentList_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isGameObject = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String[] @paths = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.FetchComponentList<global::UIToggle>(@paths, @isGameObject);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* BindingToggleEvent_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Generic.List<global::EventDelegate> @eventList = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::UIToggle[] @toggles = (global::UIToggle[])typeof(global::UIToggle[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.BindingToggleEvent(@toggles, @eventList);

            return __ret;
        }

        static StackObject* FetchComponentList_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isGameObject = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String[] @paths = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.FetchComponentList<global::UILabel>(@paths, @isGameObject);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* FindAndBindingTweenFinishedEvent_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Generic.List<global::EventDelegate> @eventList = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Generic.List<System.String> @pathList = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.FindAndBindingTweenFinishedEvent(@pathList, @eventList);

            return __ret;
        }

        static StackObject* BindingCoolTriggerEvent_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Generic.List<global::EventDelegate> @eventList = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Generic.List<System.String> @keyList = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.BindingCoolTriggerEvent(@keyList, @eventList);

            return __ret;
        }

        static StackObject* FetchComponentList_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isGameObject = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String[] @paths = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.FetchComponentList<UnityEngine.Transform>(@paths, @isGameObject);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* FetchComponentList_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isGameObject = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String[] @paths = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.FetchComponentList<global::UIButton>(@paths, @isGameObject);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* FetchComponentList_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isGameObject = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String[] @paths = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.FetchComponentList<global::TweenPosition>(@paths, @isGameObject);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* FetchComponentList_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isGameObject = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String[] @paths = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.FetchComponentList<UnityEngine.BoxCollider>(@paths, @isGameObject);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* FindAndBindingEventTriggerEvent_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @type = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Generic.List<global::EventDelegate> @eventList = (System.Collections.Generic.List<global::EventDelegate>)typeof(System.Collections.Generic.List<global::EventDelegate>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Collections.Generic.List<System.String> @pathList = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            global::UIControllerILR instance_of_this_method = (global::UIControllerILR)typeof(global::UIControllerILR).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.FindAndBindingEventTriggerEvent(@pathList, @eventList, @type);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_UiLabels_0(ref object o)
        {
            return ((global::UIControllerILR)o).UiLabels;
        }

        static StackObject* CopyToStack_UiLabels_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiLabels;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiLabels_0(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiLabels = (System.Collections.Generic.Dictionary<System.String, global::UILabel>)v;
        }

        static StackObject* AssignFromStack_UiLabels_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UILabel> @UiLabels = (System.Collections.Generic.Dictionary<System.String, global::UILabel>)typeof(System.Collections.Generic.Dictionary<System.String, global::UILabel>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiLabels = @UiLabels;
            return ptr_of_this_method;
        }

        static object get_CoolTriggers_1(ref object o)
        {
            return ((global::UIControllerILR)o).CoolTriggers;
        }

        static StackObject* CopyToStack_CoolTriggers_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).CoolTriggers;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_CoolTriggers_1(ref object o, object v)
        {
            ((global::UIControllerILR)o).CoolTriggers = (System.Collections.Generic.Dictionary<System.String, global::ConsecutiveClickCoolTrigger>)v;
        }

        static StackObject* AssignFromStack_CoolTriggers_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::ConsecutiveClickCoolTrigger> @CoolTriggers = (System.Collections.Generic.Dictionary<System.String, global::ConsecutiveClickCoolTrigger>)typeof(System.Collections.Generic.Dictionary<System.String, global::ConsecutiveClickCoolTrigger>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).CoolTriggers = @CoolTriggers;
            return ptr_of_this_method;
        }

        static object get_UiButtons_2(ref object o)
        {
            return ((global::UIControllerILR)o).UiButtons;
        }

        static StackObject* CopyToStack_UiButtons_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiButtons;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiButtons_2(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiButtons = (System.Collections.Generic.Dictionary<System.String, global::UIButton>)v;
        }

        static StackObject* AssignFromStack_UiButtons_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UIButton> @UiButtons = (System.Collections.Generic.Dictionary<System.String, global::UIButton>)typeof(System.Collections.Generic.Dictionary<System.String, global::UIButton>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiButtons = @UiButtons;
            return ptr_of_this_method;
        }

        static object get_GObjects_3(ref object o)
        {
            return ((global::UIControllerILR)o).GObjects;
        }

        static StackObject* CopyToStack_GObjects_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).GObjects;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_GObjects_3(ref object o, object v)
        {
            ((global::UIControllerILR)o).GObjects = (System.Collections.Generic.Dictionary<System.String, UnityEngine.GameObject>)v;
        }

        static StackObject* AssignFromStack_GObjects_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, UnityEngine.GameObject> @GObjects = (System.Collections.Generic.Dictionary<System.String, UnityEngine.GameObject>)typeof(System.Collections.Generic.Dictionary<System.String, UnityEngine.GameObject>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).GObjects = @GObjects;
            return ptr_of_this_method;
        }

        static object get_hotfixClassPath_4(ref object o)
        {
            return ((global::UIControllerILR)o).hotfixClassPath;
        }

        static StackObject* CopyToStack_hotfixClassPath_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).hotfixClassPath;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_hotfixClassPath_4(ref object o, object v)
        {
            ((global::UIControllerILR)o).hotfixClassPath = (System.String)v;
        }

        static StackObject* AssignFromStack_hotfixClassPath_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @hotfixClassPath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).hotfixClassPath = @hotfixClassPath;
            return ptr_of_this_method;
        }

        static object get_UiGrids_5(ref object o)
        {
            return ((global::UIControllerILR)o).UiGrids;
        }

        static StackObject* CopyToStack_UiGrids_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiGrids;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiGrids_5(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiGrids = (System.Collections.Generic.Dictionary<System.String, global::UIGrid>)v;
        }

        static StackObject* AssignFromStack_UiGrids_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UIGrid> @UiGrids = (System.Collections.Generic.Dictionary<System.String, global::UIGrid>)typeof(System.Collections.Generic.Dictionary<System.String, global::UIGrid>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiGrids = @UiGrids;
            return ptr_of_this_method;
        }

        static object get_UiTables_6(ref object o)
        {
            return ((global::UIControllerILR)o).UiTables;
        }

        static StackObject* CopyToStack_UiTables_6(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiTables;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiTables_6(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiTables = (System.Collections.Generic.Dictionary<System.String, global::UITable>)v;
        }

        static StackObject* AssignFromStack_UiTables_6(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UITable> @UiTables = (System.Collections.Generic.Dictionary<System.String, global::UITable>)typeof(System.Collections.Generic.Dictionary<System.String, global::UITable>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiTables = @UiTables;
            return ptr_of_this_method;
        }

        static object get_BoxColliders_7(ref object o)
        {
            return ((global::UIControllerILR)o).BoxColliders;
        }

        static StackObject* CopyToStack_BoxColliders_7(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).BoxColliders;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_BoxColliders_7(ref object o, object v)
        {
            ((global::UIControllerILR)o).BoxColliders = (System.Collections.Generic.Dictionary<System.String, UnityEngine.BoxCollider>)v;
        }

        static StackObject* AssignFromStack_BoxColliders_7(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, UnityEngine.BoxCollider> @BoxColliders = (System.Collections.Generic.Dictionary<System.String, UnityEngine.BoxCollider>)typeof(System.Collections.Generic.Dictionary<System.String, UnityEngine.BoxCollider>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).BoxColliders = @BoxColliders;
            return ptr_of_this_method;
        }

        static object get_UiSymbolInputs_8(ref object o)
        {
            return ((global::UIControllerILR)o).UiSymbolInputs;
        }

        static StackObject* CopyToStack_UiSymbolInputs_8(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiSymbolInputs;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiSymbolInputs_8(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiSymbolInputs = (System.Collections.Generic.Dictionary<System.String, global::UISymbolInput>)v;
        }

        static StackObject* AssignFromStack_UiSymbolInputs_8(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UISymbolInput> @UiSymbolInputs = (System.Collections.Generic.Dictionary<System.String, global::UISymbolInput>)typeof(System.Collections.Generic.Dictionary<System.String, global::UISymbolInput>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiSymbolInputs = @UiSymbolInputs;
            return ptr_of_this_method;
        }

        static object get_UiWidgets_9(ref object o)
        {
            return ((global::UIControllerILR)o).UiWidgets;
        }

        static StackObject* CopyToStack_UiWidgets_9(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiWidgets;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiWidgets_9(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiWidgets = (System.Collections.Generic.Dictionary<System.String, global::UIWidget>)v;
        }

        static StackObject* AssignFromStack_UiWidgets_9(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UIWidget> @UiWidgets = (System.Collections.Generic.Dictionary<System.String, global::UIWidget>)typeof(System.Collections.Generic.Dictionary<System.String, global::UIWidget>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiWidgets = @UiWidgets;
            return ptr_of_this_method;
        }

        static object get_UiScrollViews_10(ref object o)
        {
            return ((global::UIControllerILR)o).UiScrollViews;
        }

        static StackObject* CopyToStack_UiScrollViews_10(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiScrollViews;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiScrollViews_10(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiScrollViews = (System.Collections.Generic.Dictionary<System.String, global::UIScrollView>)v;
        }

        static StackObject* AssignFromStack_UiScrollViews_10(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UIScrollView> @UiScrollViews = (System.Collections.Generic.Dictionary<System.String, global::UIScrollView>)typeof(System.Collections.Generic.Dictionary<System.String, global::UIScrollView>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiScrollViews = @UiScrollViews;
            return ptr_of_this_method;
        }

        static object get_UiSprites_11(ref object o)
        {
            return ((global::UIControllerILR)o).UiSprites;
        }

        static StackObject* CopyToStack_UiSprites_11(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiSprites;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiSprites_11(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiSprites = (System.Collections.Generic.Dictionary<System.String, global::UISprite>)v;
        }

        static StackObject* AssignFromStack_UiSprites_11(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UISprite> @UiSprites = (System.Collections.Generic.Dictionary<System.String, global::UISprite>)typeof(System.Collections.Generic.Dictionary<System.String, global::UISprite>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiSprites = @UiSprites;
            return ptr_of_this_method;
        }

        static object get_TweenPositions_12(ref object o)
        {
            return ((global::UIControllerILR)o).TweenPositions;
        }

        static StackObject* CopyToStack_TweenPositions_12(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).TweenPositions;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_TweenPositions_12(ref object o, object v)
        {
            ((global::UIControllerILR)o).TweenPositions = (System.Collections.Generic.Dictionary<System.String, global::TweenPosition>)v;
        }

        static StackObject* AssignFromStack_TweenPositions_12(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::TweenPosition> @TweenPositions = (System.Collections.Generic.Dictionary<System.String, global::TweenPosition>)typeof(System.Collections.Generic.Dictionary<System.String, global::TweenPosition>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).TweenPositions = @TweenPositions;
            return ptr_of_this_method;
        }

        static object get_ilinstance_13(ref object o)
        {
            return ((global::UIControllerILR)o).ilinstance;
        }

        static StackObject* CopyToStack_ilinstance_13(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).ilinstance;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ilinstance_13(ref object o, object v)
        {
            ((global::UIControllerILR)o).ilinstance = (global::UIControllerILRObject)v;
        }

        static StackObject* AssignFromStack_ilinstance_13(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::UIControllerILRObject @ilinstance = (global::UIControllerILRObject)typeof(global::UIControllerILRObject).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).ilinstance = @ilinstance;
            return ptr_of_this_method;
        }

        static object get_DragEventDispatchers_14(ref object o)
        {
            return ((global::UIControllerILR)o).DragEventDispatchers;
        }

        static StackObject* CopyToStack_DragEventDispatchers_14(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).DragEventDispatchers;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_DragEventDispatchers_14(ref object o, object v)
        {
            ((global::UIControllerILR)o).DragEventDispatchers = (System.Collections.Generic.Dictionary<System.String, global::DragEventDispatcher>)v;
        }

        static StackObject* AssignFromStack_DragEventDispatchers_14(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::DragEventDispatcher> @DragEventDispatchers = (System.Collections.Generic.Dictionary<System.String, global::DragEventDispatcher>)typeof(System.Collections.Generic.Dictionary<System.String, global::DragEventDispatcher>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).DragEventDispatchers = @DragEventDispatchers;
            return ptr_of_this_method;
        }

        static object get_ParticleSystemUiComponents_15(ref object o)
        {
            return ((global::UIControllerILR)o).ParticleSystemUiComponents;
        }

        static StackObject* CopyToStack_ParticleSystemUiComponents_15(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).ParticleSystemUiComponents;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ParticleSystemUiComponents_15(ref object o, object v)
        {
            ((global::UIControllerILR)o).ParticleSystemUiComponents = (System.Collections.Generic.Dictionary<System.String, global::ParticleSystemUIComponent>)v;
        }

        static StackObject* AssignFromStack_ParticleSystemUiComponents_15(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::ParticleSystemUIComponent> @ParticleSystemUiComponents = (System.Collections.Generic.Dictionary<System.String, global::ParticleSystemUIComponent>)typeof(System.Collections.Generic.Dictionary<System.String, global::ParticleSystemUIComponent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).ParticleSystemUiComponents = @ParticleSystemUiComponents;
            return ptr_of_this_method;
        }

        static object get_UiProgressBars_16(ref object o)
        {
            return ((global::UIControllerILR)o).UiProgressBars;
        }

        static StackObject* CopyToStack_UiProgressBars_16(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiProgressBars;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiProgressBars_16(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiProgressBars = (System.Collections.Generic.Dictionary<System.String, global::UIProgressBar>)v;
        }

        static StackObject* AssignFromStack_UiProgressBars_16(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UIProgressBar> @UiProgressBars = (System.Collections.Generic.Dictionary<System.String, global::UIProgressBar>)typeof(System.Collections.Generic.Dictionary<System.String, global::UIProgressBar>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiProgressBars = @UiProgressBars;
            return ptr_of_this_method;
        }

        static object get_TweenAlphas_17(ref object o)
        {
            return ((global::UIControllerILR)o).TweenAlphas;
        }

        static StackObject* CopyToStack_TweenAlphas_17(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).TweenAlphas;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_TweenAlphas_17(ref object o, object v)
        {
            ((global::UIControllerILR)o).TweenAlphas = (System.Collections.Generic.Dictionary<System.String, global::TweenAlpha>)v;
        }

        static StackObject* AssignFromStack_TweenAlphas_17(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::TweenAlpha> @TweenAlphas = (System.Collections.Generic.Dictionary<System.String, global::TweenAlpha>)typeof(System.Collections.Generic.Dictionary<System.String, global::TweenAlpha>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).TweenAlphas = @TweenAlphas;
            return ptr_of_this_method;
        }

        static object get_Transforms_18(ref object o)
        {
            return ((global::UIControllerILR)o).Transforms;
        }

        static StackObject* CopyToStack_Transforms_18(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).Transforms;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_Transforms_18(ref object o, object v)
        {
            ((global::UIControllerILR)o).Transforms = (System.Collections.Generic.Dictionary<System.String, UnityEngine.Transform>)v;
        }

        static StackObject* AssignFromStack_Transforms_18(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, UnityEngine.Transform> @Transforms = (System.Collections.Generic.Dictionary<System.String, UnityEngine.Transform>)typeof(System.Collections.Generic.Dictionary<System.String, UnityEngine.Transform>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).Transforms = @Transforms;
            return ptr_of_this_method;
        }

        static object get_UiPanels_19(ref object o)
        {
            return ((global::UIControllerILR)o).UiPanels;
        }

        static StackObject* CopyToStack_UiPanels_19(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiPanels;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiPanels_19(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiPanels = (System.Collections.Generic.Dictionary<System.String, global::UIPanel>)v;
        }

        static StackObject* AssignFromStack_UiPanels_19(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UIPanel> @UiPanels = (System.Collections.Generic.Dictionary<System.String, global::UIPanel>)typeof(System.Collections.Generic.Dictionary<System.String, global::UIPanel>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiPanels = @UiPanels;
            return ptr_of_this_method;
        }

        static object get_TweenScales_20(ref object o)
        {
            return ((global::UIControllerILR)o).TweenScales;
        }

        static StackObject* CopyToStack_TweenScales_20(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).TweenScales;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_TweenScales_20(ref object o, object v)
        {
            ((global::UIControllerILR)o).TweenScales = (System.Collections.Generic.Dictionary<System.String, global::TweenScale>)v;
        }

        static StackObject* AssignFromStack_TweenScales_20(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::TweenScale> @TweenScales = (System.Collections.Generic.Dictionary<System.String, global::TweenScale>)typeof(System.Collections.Generic.Dictionary<System.String, global::TweenScale>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).TweenScales = @TweenScales;
            return ptr_of_this_method;
        }

        static object get_ObjectParamList_21(ref object o)
        {
            return ((global::UIControllerILR)o).ObjectParamList;
        }

        static StackObject* CopyToStack_ObjectParamList_21(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).ObjectParamList;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_ObjectParamList_21(ref object o, object v)
        {
            ((global::UIControllerILR)o).ObjectParamList = (System.Collections.Generic.List<UnityEngine.Object>)v;
        }

        static StackObject* AssignFromStack_ObjectParamList_21(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<UnityEngine.Object> @ObjectParamList = (System.Collections.Generic.List<UnityEngine.Object>)typeof(System.Collections.Generic.List<UnityEngine.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).ObjectParamList = @ObjectParamList;
            return ptr_of_this_method;
        }

        static object get_TextureCmps_22(ref object o)
        {
            return ((global::UIControllerILR)o).TextureCmps;
        }

        static StackObject* CopyToStack_TextureCmps_22(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).TextureCmps;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_TextureCmps_22(ref object o, object v)
        {
            ((global::UIControllerILR)o).TextureCmps = (System.Collections.Generic.Dictionary<System.String, global::CampaignTextureCmp>)v;
        }

        static StackObject* AssignFromStack_TextureCmps_22(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::CampaignTextureCmp> @TextureCmps = (System.Collections.Generic.Dictionary<System.String, global::CampaignTextureCmp>)typeof(System.Collections.Generic.Dictionary<System.String, global::CampaignTextureCmp>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).TextureCmps = @TextureCmps;
            return ptr_of_this_method;
        }

        static object get_UiSliders_23(ref object o)
        {
            return ((global::UIControllerILR)o).UiSliders;
        }

        static StackObject* CopyToStack_UiSliders_23(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiSliders;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiSliders_23(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiSliders = (System.Collections.Generic.Dictionary<System.String, global::UISlider>)v;
        }

        static StackObject* AssignFromStack_UiSliders_23(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UISlider> @UiSliders = (System.Collections.Generic.Dictionary<System.String, global::UISlider>)typeof(System.Collections.Generic.Dictionary<System.String, global::UISlider>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiSliders = @UiSliders;
            return ptr_of_this_method;
        }

        static object get_UiEventTriggers_24(ref object o)
        {
            return ((global::UIControllerILR)o).UiEventTriggers;
        }

        static StackObject* CopyToStack_UiEventTriggers_24(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiEventTriggers;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiEventTriggers_24(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiEventTriggers = (System.Collections.Generic.Dictionary<System.String, global::UIEventTrigger>)v;
        }

        static StackObject* AssignFromStack_UiEventTriggers_24(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UIEventTrigger> @UiEventTriggers = (System.Collections.Generic.Dictionary<System.String, global::UIEventTrigger>)typeof(System.Collections.Generic.Dictionary<System.String, global::UIEventTrigger>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiEventTriggers = @UiEventTriggers;
            return ptr_of_this_method;
        }

        static object get_UiTextures_25(ref object o)
        {
            return ((global::UIControllerILR)o).UiTextures;
        }

        static StackObject* CopyToStack_UiTextures_25(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::UIControllerILR)o).UiTextures;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_UiTextures_25(ref object o, object v)
        {
            ((global::UIControllerILR)o).UiTextures = (System.Collections.Generic.Dictionary<System.String, global::UITexture>)v;
        }

        static StackObject* AssignFromStack_UiTextures_25(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, global::UITexture> @UiTextures = (System.Collections.Generic.Dictionary<System.String, global::UITexture>)typeof(System.Collections.Generic.Dictionary<System.String, global::UITexture>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::UIControllerILR)o).UiTextures = @UiTextures;
            return ptr_of_this_method;
        }



    }
}
