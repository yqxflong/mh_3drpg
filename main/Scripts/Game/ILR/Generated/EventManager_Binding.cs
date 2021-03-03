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
    unsafe class EventManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::EventManager);
            args = new Type[]{};
            method = type.GetMethod("get_instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_instance_0);
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
            args = new Type[]{typeof(global::TouchStartEvent)};
            if (genericMethods.TryGetValue("AddListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TouchStartEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, AddListener_1);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TouchUpdateEvent)};
            if (genericMethods.TryGetValue("AddListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TouchUpdateEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, AddListener_2);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TouchEndEvent)};
            if (genericMethods.TryGetValue("AddListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TouchEndEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, AddListener_3);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TapEvent)};
            if (genericMethods.TryGetValue("AddListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TapEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, AddListener_4);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::DoubleTapEvent)};
            if (genericMethods.TryGetValue("AddListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::DoubleTapEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, AddListener_5);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TwoFingerTouchStartEvent)};
            if (genericMethods.TryGetValue("AddListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TwoFingerTouchStartEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, AddListener_6);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TwoFingerTouchUpdateEvent)};
            if (genericMethods.TryGetValue("AddListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TwoFingerTouchUpdateEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, AddListener_7);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TwoFingerTouchEndEvent)};
            if (genericMethods.TryGetValue("AddListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TwoFingerTouchEndEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, AddListener_8);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TouchStartEvent)};
            if (genericMethods.TryGetValue("RemoveListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TouchStartEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, RemoveListener_9);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TouchUpdateEvent)};
            if (genericMethods.TryGetValue("RemoveListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TouchUpdateEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, RemoveListener_10);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TouchEndEvent)};
            if (genericMethods.TryGetValue("RemoveListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TouchEndEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, RemoveListener_11);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TapEvent)};
            if (genericMethods.TryGetValue("RemoveListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TapEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, RemoveListener_12);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::DoubleTapEvent)};
            if (genericMethods.TryGetValue("RemoveListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::DoubleTapEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, RemoveListener_13);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TwoFingerTouchStartEvent)};
            if (genericMethods.TryGetValue("RemoveListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TwoFingerTouchStartEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, RemoveListener_14);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TwoFingerTouchUpdateEvent)};
            if (genericMethods.TryGetValue("RemoveListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TwoFingerTouchUpdateEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, RemoveListener_15);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::TwoFingerTouchEndEvent)};
            if (genericMethods.TryGetValue("RemoveListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::TwoFingerTouchEndEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, RemoveListener_16);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::eSimpleEventID)};
            method = type.GetMethod("Raise", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Raise_17);
            args = new Type[]{typeof(global::LevelStartEvent)};
            if (genericMethods.TryGetValue("AddListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::LevelStartEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, AddListener_18);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(global::LevelStartEvent)};
            if (genericMethods.TryGetValue("RemoveListener", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(void), typeof(global::EventManager.EventDelegate<global::LevelStartEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, RemoveListener_19);

                        break;
                    }
                }
            }


        }


        static StackObject* get_instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::EventManager.instance;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* AddListener_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TouchStartEvent> @del = (global::EventManager.EventDelegate<global::TouchStartEvent>)typeof(global::EventManager.EventDelegate<global::TouchStartEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddListener<global::TouchStartEvent>(@del);

            return __ret;
        }

        static StackObject* AddListener_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TouchUpdateEvent> @del = (global::EventManager.EventDelegate<global::TouchUpdateEvent>)typeof(global::EventManager.EventDelegate<global::TouchUpdateEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddListener<global::TouchUpdateEvent>(@del);

            return __ret;
        }

        static StackObject* AddListener_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TouchEndEvent> @del = (global::EventManager.EventDelegate<global::TouchEndEvent>)typeof(global::EventManager.EventDelegate<global::TouchEndEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddListener<global::TouchEndEvent>(@del);

            return __ret;
        }

        static StackObject* AddListener_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TapEvent> @del = (global::EventManager.EventDelegate<global::TapEvent>)typeof(global::EventManager.EventDelegate<global::TapEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddListener<global::TapEvent>(@del);

            return __ret;
        }

        static StackObject* AddListener_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::DoubleTapEvent> @del = (global::EventManager.EventDelegate<global::DoubleTapEvent>)typeof(global::EventManager.EventDelegate<global::DoubleTapEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddListener<global::DoubleTapEvent>(@del);

            return __ret;
        }

        static StackObject* AddListener_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TwoFingerTouchStartEvent> @del = (global::EventManager.EventDelegate<global::TwoFingerTouchStartEvent>)typeof(global::EventManager.EventDelegate<global::TwoFingerTouchStartEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddListener<global::TwoFingerTouchStartEvent>(@del);

            return __ret;
        }

        static StackObject* AddListener_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TwoFingerTouchUpdateEvent> @del = (global::EventManager.EventDelegate<global::TwoFingerTouchUpdateEvent>)typeof(global::EventManager.EventDelegate<global::TwoFingerTouchUpdateEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddListener<global::TwoFingerTouchUpdateEvent>(@del);

            return __ret;
        }

        static StackObject* AddListener_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TwoFingerTouchEndEvent> @del = (global::EventManager.EventDelegate<global::TwoFingerTouchEndEvent>)typeof(global::EventManager.EventDelegate<global::TwoFingerTouchEndEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddListener<global::TwoFingerTouchEndEvent>(@del);

            return __ret;
        }

        static StackObject* RemoveListener_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TouchStartEvent> @del = (global::EventManager.EventDelegate<global::TouchStartEvent>)typeof(global::EventManager.EventDelegate<global::TouchStartEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveListener<global::TouchStartEvent>(@del);

            return __ret;
        }

        static StackObject* RemoveListener_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TouchUpdateEvent> @del = (global::EventManager.EventDelegate<global::TouchUpdateEvent>)typeof(global::EventManager.EventDelegate<global::TouchUpdateEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveListener<global::TouchUpdateEvent>(@del);

            return __ret;
        }

        static StackObject* RemoveListener_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TouchEndEvent> @del = (global::EventManager.EventDelegate<global::TouchEndEvent>)typeof(global::EventManager.EventDelegate<global::TouchEndEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveListener<global::TouchEndEvent>(@del);

            return __ret;
        }

        static StackObject* RemoveListener_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TapEvent> @del = (global::EventManager.EventDelegate<global::TapEvent>)typeof(global::EventManager.EventDelegate<global::TapEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveListener<global::TapEvent>(@del);

            return __ret;
        }

        static StackObject* RemoveListener_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::DoubleTapEvent> @del = (global::EventManager.EventDelegate<global::DoubleTapEvent>)typeof(global::EventManager.EventDelegate<global::DoubleTapEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveListener<global::DoubleTapEvent>(@del);

            return __ret;
        }

        static StackObject* RemoveListener_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TwoFingerTouchStartEvent> @del = (global::EventManager.EventDelegate<global::TwoFingerTouchStartEvent>)typeof(global::EventManager.EventDelegate<global::TwoFingerTouchStartEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveListener<global::TwoFingerTouchStartEvent>(@del);

            return __ret;
        }

        static StackObject* RemoveListener_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TwoFingerTouchUpdateEvent> @del = (global::EventManager.EventDelegate<global::TwoFingerTouchUpdateEvent>)typeof(global::EventManager.EventDelegate<global::TwoFingerTouchUpdateEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveListener<global::TwoFingerTouchUpdateEvent>(@del);

            return __ret;
        }

        static StackObject* RemoveListener_16(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::TwoFingerTouchEndEvent> @del = (global::EventManager.EventDelegate<global::TwoFingerTouchEndEvent>)typeof(global::EventManager.EventDelegate<global::TwoFingerTouchEndEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveListener<global::TwoFingerTouchEndEvent>(@del);

            return __ret;
        }

        static StackObject* Raise_17(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::eSimpleEventID @eventId = (global::eSimpleEventID)typeof(global::eSimpleEventID).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Raise(@eventId);

            return __ret;
        }

        static StackObject* AddListener_18(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::LevelStartEvent> @del = (global::EventManager.EventDelegate<global::LevelStartEvent>)typeof(global::EventManager.EventDelegate<global::LevelStartEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddListener<global::LevelStartEvent>(@del);

            return __ret;
        }

        static StackObject* RemoveListener_19(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::EventManager.EventDelegate<global::LevelStartEvent> @del = (global::EventManager.EventDelegate<global::LevelStartEvent>)typeof(global::EventManager.EventDelegate<global::LevelStartEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::EventManager instance_of_this_method = (global::EventManager)typeof(global::EventManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveListener<global::LevelStartEvent>(@del);

            return __ret;
        }



    }
}
