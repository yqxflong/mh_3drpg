using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class NationBattleSpecialEventController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TipsLabel = t.GetComponent<UILabel>("Content/TipsLabel");
            SuccessTick = t.FindEx("Content/V").gameObject;
            FailTick = t.FindEx("Content/X").gameObject;
            YesBtnGO = t.FindEx("Content/YesBtn").gameObject;
            NOBtnGO = t.FindEx("Content/NoBtn").gameObject;
            SureBtnGO = t.FindEx("Content/SureBtn").gameObject;
            controller.backButton = t.GetComponent<UIButton>("UINormalFrame/CloseBtn");

            t.GetComponent<UIButton>("Content/YesBtn").onClick.Add(new EventDelegate(OnYesBtnClick));
            t.GetComponent<UIButton>("Content/NoBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButton>("Content/SureBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
        }



        public override bool ShowUIBlocker { get { return true; } }

        public UILabel TipsLabel;
        public UISprite iSprite;
        public GameObject SuccessTick, FailTick;
        public GameObject YesBtnGO, NOBtnGO, SureBtnGO;
        int mEventId;
        System.Action mCallBack;

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);

            Hashtable ht = param as Hashtable;
            mEventId = int.Parse(ht["eventId"].ToString());
            mCallBack = ht["cb"] as System.Action;
            Hotfix_LT.Data.NationSpecialEventTemplate tpl = Hotfix_LT.Data.EventTemplateManager.Instance.GetNationSpecialEventTpl(mEventId);
            if (tpl != null)
            {
                LTUIUtil.SetText(TipsLabel, tpl.desc);
            }
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            if (NationBattleHudController.sExitField)
            {
                controller.Close();
            }
        }

        public override IEnumerator OnRemoveFromStack()
        {
            if (mCallBack != null)
                mCallBack();
            DestroySelf();
            yield break;
        }

        public void OnYesBtnClick()
        {
            controller.transform.localScale = Vector3.zero;
            NationManager.Instance.SureEvent(delegate (Hashtable result)
            {
                UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
                if (result != null)
                {
                    Hotfix_LT.Data.NationSpecialEventTemplate tpl = Hotfix_LT.Data.EventTemplateManager.Instance.GetNationSpecialEventTpl(mEventId);
                    if (tpl != null)
                    {
                        LTUIUtil.SetText(TipsLabel, tpl.desc);
                    }
                    int eventResult = EB.Dot.Integer("nation.eventResult", result, -1);
                    if (eventResult == 0) //成功
                    {
                        LTUIUtil.SetText(TipsLabel, tpl.result_desc1);
                        TipsLabel.color = LT.Hotfix.Utility.ColorUtility.GreenColor;
                        FusionAudio.PostEvent("UI/New/GuoZhanHao", true);
                        SuccessTick.CustomSetActive(true);
                        FailTick.CustomSetActive(false);
                    }
                    else if (eventResult == 1)
                    {
                        LTUIUtil.SetText(TipsLabel, tpl.result_desc2);
                        TipsLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor;
                        FusionAudio.PostEvent("UI/New/GuoZhanHuai", true);
                        SuccessTick.CustomSetActive(false);
                        FailTick.CustomSetActive(true);
                    }
                    else
                    {
                       EB.Debug.LogError("nation.eventResult error : result={0}", eventResult);
                    }

                    YesBtnGO.CustomSetActive(false);
                    NOBtnGO.CustomSetActive(false);
                    SureBtnGO.CustomSetActive(true);
                }

                for (int j = 0; j < tweeners.Length; ++j)
                {
                    tweeners[j].tweenFactor = 0;
                    tweeners[j].PlayForward();
                }
            });
        }
    }
}
