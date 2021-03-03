using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntime.Runtime.GeneratedAdapter
{   
    public class UIControllerILRObjectAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(global::UIControllerILRObject);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : global::UIControllerILRObject, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            IMethod mSetUIController_0;
            bool mSetUIController_0_Got;
            bool mSetUIController_0_Invoking;
            IMethod mGetUIController_1;
            bool mGetUIController_1_Got;
            bool mGetUIController_1_Invoking;
            IMethod mAwake_2;
            bool mAwake_2_Got;
            bool mAwake_2_Invoking;
            IMethod mStart_3;
            bool mStart_3_Got;
            bool mStart_3_Invoking;
            IMethod mOnEnable_4;
            bool mOnEnable_4_Got;
            bool mOnEnable_4_Invoking;
            IMethod mOnDisable_5;
            bool mOnDisable_5_Got;
            bool mOnDisable_5_Invoking;
            IMethod mUpdate_6;
            bool mUpdate_6_Got;
            bool mUpdate_6_Invoking;
            IMethod mOnFocus_7;
            bool mOnFocus_7_Got;
            bool mOnFocus_7_Invoking;
            IMethod mOnDestroy_8;
            bool mOnDestroy_8_Got;
            bool mOnDestroy_8_Invoking;
            IMethod mOnPrepareAddToStack_9;
            bool mOnPrepareAddToStack_9_Got;
            bool mOnPrepareAddToStack_9_Invoking;
            IMethod mOnAddToStack_10;
            bool mOnAddToStack_10_Got;
            bool mOnAddToStack_10_Invoking;
            IMethod mOnRemoveFromStack_11;
            bool mOnRemoveFromStack_11_Got;
            bool mOnRemoveFromStack_11_Invoking;
            IMethod mIsFullscreen_12;
            bool mIsFullscreen_12_Got;
            bool mIsFullscreen_12_Invoking;
            IMethod mSetMenuData_13;
            bool mSetMenuData_13_Got;
            bool mSetMenuData_13_Invoking;
            IMethod mShow_14;
            bool mShow_14_Got;
            bool mShow_14_Invoking;
            IMethod mOnBlur_15;
            bool mOnBlur_15_Got;
            bool mOnBlur_15_Invoking;
            IMethod mOnPrefabSave_16;
            bool mOnPrefabSave_16_Got;
            bool mOnPrefabSave_16_Invoking;
            IMethod mCanAutoBackstack_17;
            bool mCanAutoBackstack_17_Got;
            bool mCanAutoBackstack_17_Invoking;
            IMethod mIsRenderingWorldWhileFullscreen_18;
            bool mIsRenderingWorldWhileFullscreen_18_Got;
            bool mIsRenderingWorldWhileFullscreen_18_Invoking;
            IMethod mget_Visibility_19;
            bool mget_Visibility_19_Got;
            bool mget_Visibility_19_Invoking;
            IMethod mget_BackgroundUIFadeTime_20;
            bool mget_BackgroundUIFadeTime_20_Got;
            bool mget_BackgroundUIFadeTime_20_Invoking;
            IMethod mget_ShowUIBlocker_21;
            bool mget_ShowUIBlocker_21_Got;
            bool mget_ShowUIBlocker_21_Invoking;
            IMethod mStartBootFlash_22;
            bool mStartBootFlash_22_Got;
            bool mStartBootFlash_22_Invoking;
            IMethod mOnCancelButtonClick_23;
            bool mOnCancelButtonClick_23_Got;
            bool mOnCancelButtonClick_23_Invoking;
            IMethod mOnFetchData_24;
            bool mOnFetchData_24_Got;
            bool mOnFetchData_24_Invoking;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void SetUIController(global::UIControllerILR uicontroller)
            {
                if (!mSetUIController_0_Got)
                {
                    mSetUIController_0 = instance.Type.GetMethod("SetUIController", 1);
                    mSetUIController_0_Got = true;
                }
                if (mSetUIController_0 != null && !mSetUIController_0_Invoking)
                {
                    mSetUIController_0_Invoking = true;
                    appdomain.Invoke(mSetUIController_0, this.instance, uicontroller);
                    mSetUIController_0_Invoking = false;
                }
                else
                {
                    base.SetUIController(uicontroller);
                }
            }

            public override global::UIControllerILR GetUIController()
            {
                if (!mGetUIController_1_Got)
                {
                    mGetUIController_1 = instance.Type.GetMethod("GetUIController", 0);
                    mGetUIController_1_Got = true;
                }
                if (mGetUIController_1 != null && !mGetUIController_1_Invoking)
                {
                    mGetUIController_1_Invoking = true;
                    var returnValue = appdomain.Invoke(mGetUIController_1, this.instance);
                    mGetUIController_1_Invoking = false;
                    return (global::UIControllerILR)returnValue;
                }
                else
                {
                    return base.GetUIController();
                }
            }

            public override void Awake()
            {
                if (!mAwake_2_Got)
                {
                    mAwake_2 = instance.Type.GetMethod("Awake", 0);
                    mAwake_2_Got = true;
                }
                if (mAwake_2 != null && !mAwake_2_Invoking)
                {
                    mAwake_2_Invoking = true;
                    appdomain.Invoke(mAwake_2, this.instance);
                    mAwake_2_Invoking = false;
                }
                else
                {
                    base.Awake();
                }
            }

            public override void Start()
            {
                if (!mStart_3_Got)
                {
                    mStart_3 = instance.Type.GetMethod("Start", 0);
                    mStart_3_Got = true;
                }
                if (mStart_3 != null && !mStart_3_Invoking)
                {
                    mStart_3_Invoking = true;
                    appdomain.Invoke(mStart_3, this.instance);
                    mStart_3_Invoking = false;
                }
                else
                {
                    base.Start();
                }
            }

            public override void OnEnable()
            {
                if (!mOnEnable_4_Got)
                {
                    mOnEnable_4 = instance.Type.GetMethod("OnEnable", 0);
                    mOnEnable_4_Got = true;
                }
                if (mOnEnable_4 != null && !mOnEnable_4_Invoking)
                {
                    mOnEnable_4_Invoking = true;
                    appdomain.Invoke(mOnEnable_4, this.instance);
                    mOnEnable_4_Invoking = false;
                }
                else
                {
                    base.OnEnable();
                }
            }

            public override void OnDisable()
            {
                if (!mOnDisable_5_Got)
                {
                    mOnDisable_5 = instance.Type.GetMethod("OnDisable", 0);
                    mOnDisable_5_Got = true;
                }
                if (mOnDisable_5 != null && !mOnDisable_5_Invoking)
                {
                    mOnDisable_5_Invoking = true;
                    appdomain.Invoke(mOnDisable_5, this.instance);
                    mOnDisable_5_Invoking = false;
                }
                else
                {
                    base.OnDisable();
                }
            }

            public override void Update(System.Int32 e)
            {
                if (!mUpdate_6_Got)
                {
                    mUpdate_6 = instance.Type.GetMethod("Update", 1);
                    mUpdate_6_Got = true;
                }
                if (mUpdate_6 != null && !mUpdate_6_Invoking)
                {
                    mUpdate_6_Invoking = true;
                    appdomain.Invoke(mUpdate_6, this.instance, e);
                    mUpdate_6_Invoking = false;
                }
                else
                {
                    base.Update(e);
                }
            }

            public override void OnFocus()
            {
                if (!mOnFocus_7_Got)
                {
                    mOnFocus_7 = instance.Type.GetMethod("OnFocus", 0);
                    mOnFocus_7_Got = true;
                }
                if (mOnFocus_7 != null && !mOnFocus_7_Invoking)
                {
                    mOnFocus_7_Invoking = true;
                    appdomain.Invoke(mOnFocus_7, this.instance);
                    mOnFocus_7_Invoking = false;
                }
                else
                {
                    base.OnFocus();
                }
            }

            public override void OnDestroy()
            {
                if (!mOnDestroy_8_Got)
                {
                    mOnDestroy_8 = instance.Type.GetMethod("OnDestroy", 0);
                    mOnDestroy_8_Got = true;
                }
                if (mOnDestroy_8 != null && !mOnDestroy_8_Invoking)
                {
                    mOnDestroy_8_Invoking = true;
                    appdomain.Invoke(mOnDestroy_8, this.instance);
                    mOnDestroy_8_Invoking = false;
                }
                else
                {
                    base.OnDestroy();
                }
            }

            public override System.Collections.IEnumerator OnPrepareAddToStack()
            {
                if (!mOnPrepareAddToStack_9_Got)
                {
                    mOnPrepareAddToStack_9 = instance.Type.GetMethod("OnPrepareAddToStack", 0);
                    mOnPrepareAddToStack_9_Got = true;
                }
                if (mOnPrepareAddToStack_9 != null && !mOnPrepareAddToStack_9_Invoking)
                {
                    mOnPrepareAddToStack_9_Invoking = true;
                    var returnValue = appdomain.Invoke(mOnPrepareAddToStack_9, this.instance);
                    mOnPrepareAddToStack_9_Invoking = false;
                    return (System.Collections.IEnumerator)returnValue;
                }
                else
                {
                    return base.OnPrepareAddToStack();
                }
            }

            public override System.Collections.IEnumerator OnAddToStack()
            {
                if (!mOnAddToStack_10_Got)
                {
                    mOnAddToStack_10 = instance.Type.GetMethod("OnAddToStack", 0);
                    mOnAddToStack_10_Got = true;
                }
                if (mOnAddToStack_10 != null && !mOnAddToStack_10_Invoking)
                {
                    mOnAddToStack_10_Invoking = true;
                    var returnValue = appdomain.Invoke(mOnAddToStack_10, this.instance);
                    mOnAddToStack_10_Invoking = false;
                    return (System.Collections.IEnumerator)returnValue;
                }
                else
                {
                    return base.OnAddToStack();
                }
            }

            public override System.Collections.IEnumerator OnRemoveFromStack()
            {
                if (!mOnRemoveFromStack_11_Got)
                {
                    mOnRemoveFromStack_11 = instance.Type.GetMethod("OnRemoveFromStack", 0);
                    mOnRemoveFromStack_11_Got = true;
                }
                if (mOnRemoveFromStack_11 != null && !mOnRemoveFromStack_11_Invoking)
                {
                    mOnRemoveFromStack_11_Invoking = true;
                    var returnValue = appdomain.Invoke(mOnRemoveFromStack_11, this.instance);
                    mOnRemoveFromStack_11_Invoking = false;
                    return (System.Collections.IEnumerator)returnValue;
                }
                else
                {
                    return base.OnRemoveFromStack();
                }
            }

            public override System.Boolean IsFullscreen()
            {
                if (!mIsFullscreen_12_Got)
                {
                    mIsFullscreen_12 = instance.Type.GetMethod("IsFullscreen", 0);
                    mIsFullscreen_12_Got = true;
                }
                if (mIsFullscreen_12 != null && !mIsFullscreen_12_Invoking)
                {
                    mIsFullscreen_12_Invoking = true;
                    var returnValue = appdomain.Invoke(mIsFullscreen_12, this.instance);
                    mIsFullscreen_12_Invoking = false;
                    return (System.Boolean)returnValue;
                }
                else
                {
                    return base.IsFullscreen();
                }
            }

            public override void SetMenuData(System.Object param)
            {
                if (!mSetMenuData_13_Got)
                {
                    mSetMenuData_13 = instance.Type.GetMethod("SetMenuData", 1);
                    mSetMenuData_13_Got = true;
                }
                if (mSetMenuData_13 != null && !mSetMenuData_13_Invoking)
                {
                    mSetMenuData_13_Invoking = true;
                    appdomain.Invoke(mSetMenuData_13, this.instance, param);
                    mSetMenuData_13_Invoking = false;
                }
                else
                {
                    base.SetMenuData(param);
                }
            }

            public override void Show(System.Boolean isShowing)
            {
                if (!mShow_14_Got)
                {
                    mShow_14 = instance.Type.GetMethod("Show", 1);
                    mShow_14_Got = true;
                }
                if (mShow_14 != null && !mShow_14_Invoking)
                {
                    mShow_14_Invoking = true;
                    appdomain.Invoke(mShow_14, this.instance, isShowing);
                    mShow_14_Invoking = false;
                }
                else
                {
                    base.Show(isShowing);
                }
            }

            public override void OnBlur()
            {
                if (!mOnBlur_15_Got)
                {
                    mOnBlur_15 = instance.Type.GetMethod("OnBlur", 0);
                    mOnBlur_15_Got = true;
                }
                if (mOnBlur_15 != null && !mOnBlur_15_Invoking)
                {
                    mOnBlur_15_Invoking = true;
                    appdomain.Invoke(mOnBlur_15, this.instance);
                    mOnBlur_15_Invoking = false;
                }
                else
                {
                    base.OnBlur();
                }
            }

            public override void OnPrefabSave()
            {
                if (!mOnPrefabSave_16_Got)
                {
                    mOnPrefabSave_16 = instance.Type.GetMethod("OnPrefabSave", 0);
                    mOnPrefabSave_16_Got = true;
                }
                if (mOnPrefabSave_16 != null && !mOnPrefabSave_16_Invoking)
                {
                    mOnPrefabSave_16_Invoking = true;
                    appdomain.Invoke(mOnPrefabSave_16, this.instance);
                    mOnPrefabSave_16_Invoking = false;
                }
                else
                {
                    base.OnPrefabSave();
                }
            }

            public override System.Boolean CanAutoBackstack()
            {
                if (!mCanAutoBackstack_17_Got)
                {
                    mCanAutoBackstack_17 = instance.Type.GetMethod("CanAutoBackstack", 0);
                    mCanAutoBackstack_17_Got = true;
                }
                if (mCanAutoBackstack_17 != null && !mCanAutoBackstack_17_Invoking)
                {
                    mCanAutoBackstack_17_Invoking = true;
                    var returnValue = appdomain.Invoke(mCanAutoBackstack_17, this.instance);
                    mCanAutoBackstack_17_Invoking = false;
                    return (System.Boolean)returnValue;
                }
                else
                {
                    return base.CanAutoBackstack();
                }
            }

            public override System.Boolean IsRenderingWorldWhileFullscreen()
            {
                if (!mIsRenderingWorldWhileFullscreen_18_Got)
                {
                    mIsRenderingWorldWhileFullscreen_18 = instance.Type.GetMethod("IsRenderingWorldWhileFullscreen", 0);
                    mIsRenderingWorldWhileFullscreen_18_Got = true;
                }
                if (mIsRenderingWorldWhileFullscreen_18 != null && !mIsRenderingWorldWhileFullscreen_18_Invoking)
                {
                    mIsRenderingWorldWhileFullscreen_18_Invoking = true;
                    var returnValue = appdomain.Invoke(mIsRenderingWorldWhileFullscreen_18, this.instance);
                    mIsRenderingWorldWhileFullscreen_18_Invoking = false;
                    return (System.Boolean)returnValue;
                }
                else
                {
                    return base.IsRenderingWorldWhileFullscreen();
                }
            }

            public override void StartBootFlash()
            {
                if (!mStartBootFlash_22_Got)
                {
                    mStartBootFlash_22 = instance.Type.GetMethod("StartBootFlash", 0);
                    mStartBootFlash_22_Got = true;
                }
                if (mStartBootFlash_22 != null && !mStartBootFlash_22_Invoking)
                {
                    mStartBootFlash_22_Invoking = true;
                    appdomain.Invoke(mStartBootFlash_22, this.instance);
                    mStartBootFlash_22_Invoking = false;
                }
                else
                {
                    base.StartBootFlash();
                }
            }

            public override void OnCancelButtonClick()
            {
                if (!mOnCancelButtonClick_23_Got)
                {
                    mOnCancelButtonClick_23 = instance.Type.GetMethod("OnCancelButtonClick", 0);
                    mOnCancelButtonClick_23_Got = true;
                }
                if (mOnCancelButtonClick_23 != null && !mOnCancelButtonClick_23_Invoking)
                {
                    mOnCancelButtonClick_23_Invoking = true;
                    appdomain.Invoke(mOnCancelButtonClick_23, this.instance);
                    mOnCancelButtonClick_23_Invoking = false;
                }
                else
                {
                    base.OnCancelButtonClick();
                }
            }

            public override void OnFetchData(EB.Sparx.Response res, System.Int32 reqInstanceID)
            {
                if (!mOnFetchData_24_Got)
                {
                    mOnFetchData_24 = instance.Type.GetMethod("OnFetchData", 2);
                    mOnFetchData_24_Got = true;
                }
                if (mOnFetchData_24 != null && !mOnFetchData_24_Invoking)
                {
                    mOnFetchData_24_Invoking = true;
                    appdomain.Invoke(mOnFetchData_24, this.instance, res, reqInstanceID);
                    mOnFetchData_24_Invoking = false;
                }
                else
                {
                    base.OnFetchData(res, reqInstanceID);
                }
            }

            public override System.Boolean Visibility
            {
            get
            {
                if (!mget_Visibility_19_Got)
                {
                    mget_Visibility_19 = instance.Type.GetMethod("get_Visibility", 0);
                    mget_Visibility_19_Got = true;
                }
                if (mget_Visibility_19 != null && !mget_Visibility_19_Invoking)
                {
                    mget_Visibility_19_Invoking = true;
                    var returnValue = appdomain.Invoke(mget_Visibility_19, this.instance);
                    mget_Visibility_19_Invoking = false;
                    return (System.Boolean)returnValue;
                }
                else
                {
                    return base.Visibility;
                }

            }
            }

            public override System.Single BackgroundUIFadeTime
            {
            get
            {
                if (!mget_BackgroundUIFadeTime_20_Got)
                {
                    mget_BackgroundUIFadeTime_20 = instance.Type.GetMethod("get_BackgroundUIFadeTime", 0);
                    mget_BackgroundUIFadeTime_20_Got = true;
                }
                if (mget_BackgroundUIFadeTime_20 != null && !mget_BackgroundUIFadeTime_20_Invoking)
                {
                    mget_BackgroundUIFadeTime_20_Invoking = true;
                    var returnValue = appdomain.Invoke(mget_BackgroundUIFadeTime_20, this.instance);
                    mget_BackgroundUIFadeTime_20_Invoking = false;
                    return (System.Single)returnValue;
                }
                else
                {
                    return base.BackgroundUIFadeTime;
                }

            }
            }

            public override System.Boolean ShowUIBlocker
            {
            get
            {
                if (!mget_ShowUIBlocker_21_Got)
                {
                    mget_ShowUIBlocker_21 = instance.Type.GetMethod("get_ShowUIBlocker", 0);
                    mget_ShowUIBlocker_21_Got = true;
                }
                if (mget_ShowUIBlocker_21 != null && !mget_ShowUIBlocker_21_Invoking)
                {
                    mget_ShowUIBlocker_21_Invoking = true;
                    var returnValue = appdomain.Invoke(mget_ShowUIBlocker_21, this.instance);
                    mget_ShowUIBlocker_21_Invoking = false;
                    return (System.Boolean)returnValue;
                }
                else
                {
                    return base.ShowUIBlocker;
                }

            }
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}
