using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceReviveCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            CostLabel = t.GetComponent<UILabel>("ButtonGrid/OKButton/NumLabel");
            TimesLabel = t.GetComponent<UILabel>("Content/TimesLabel");
            controller.backButton = t.GetComponent<UIButton>("bg/Top/CancelBtn");

            t.GetComponent<UIButton>("ButtonGrid/ResetButton").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButton>("ButtonGrid/OKButton").onClick.Add(new EventDelegate(OnOKBtnClick));

        }
        

        public UILabel CostLabel;
    
        public UILabel TimesLabel;
    
        private System.Action<bool> mCallback;
    
        private int mReviveCount;
    
        private int mCost;
    
        private int mFreeTimes;
    
        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            mCallback = param as System.Action<bool>;
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.freeRevive", out mFreeTimes);
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.reviveCount", out mReviveCount);
            mReviveCount += VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.LostChallengeReviveTimes);
    
            TimesLabel.text = Mathf.Max(0, mReviveCount).ToString();
            mCost = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("challRiviveCost");
            CostLabel.text = (mFreeTimes == 0) ? EB.Localizer.GetString("ID_FREE") : mCost.ToString();
            if (mFreeTimes != 0&&mCost > BalanceResourceUtil.GetUserDiamond())
            {
                CostLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor;
            }
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    
        public override void OnCancelButtonClick()
        {
            controller.Close();
            if (mCallback != null)
            {
                mCallback(false);
            }
        }
    
        public void OnOKBtnClick()
        {
            if (mFreeTimes != 0&&mCost > BalanceResourceUtil.GetUserDiamond())
            {
                BalanceResourceUtil.HcLessMessage();
                return;
            }
    
            if (mReviveCount <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceReviveCtrl_1859"));
                return;
            }
    
            LTInstanceMapModel.Instance.RequestChallengeRevive(delegate
            {
                LTChallengeInstanceHpCtrl.RestHpSum();
                if(mFreeTimes != 0)
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, -mCost, "挑战副本复活");
                controller.Close();
                if (mCallback != null)
                {
                    mCallback(true);
                }
            });
        }
    }
}
