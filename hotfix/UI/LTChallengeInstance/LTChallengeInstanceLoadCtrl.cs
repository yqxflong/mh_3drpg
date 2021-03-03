using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceLoadCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            LevelLabel = t.GetComponent<UILabel>("Content/LevelLabel");
            controller.backButton = t.GetComponent<UIButton>("bg/Top/CancelBtn");

            t.GetComponent<UIButton>("ButtonGrid/ResetButton").onClick.Add(new EventDelegate(OnResetBtnClick));
            t.GetComponent<UIButton>("ButtonGrid/OKButton").onClick.Add(new EventDelegate(OnOKBtnClick));

        }
        
        public UILabel LevelLabel;
    
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
    
        private System.Action m_OnOK;
        private System.Action m_OnCancel;
    
        private int m_CurLevel = 0;
    
        private TimeSpan m_LastTime;
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Hashtable actTable = param as Hashtable;
            m_OnOK = actTable["okAction"] as System.Action;
            m_OnCancel = actTable["cancelAction"] as System.Action;
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
    
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.curLevel", out m_CurLevel);
            
            int showLevel = LTInstanceUtil.GetChallengeLevel(m_CurLevel);
            LTUIUtil.SetText(LevelLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceLoadCtrl_1580"), showLevel));
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    
        public void OnOKBtnClick()
        {
            if (m_OnOK != null)
            {
                System.Action callback = delegate { m_OnOK(); };
                Hotfix_LT.Messenger.Raise(EventName.PlayCloudFXCallback, callback);
            }
            controller.Close();
        }
    
        public void OnResetBtnClick()
        {
            controller.Close();
        }
    
        public override void OnCancelButtonClick()
        {
            if (m_OnCancel != null)
            {
                m_OnCancel();
            }
            base.OnCancelButtonClick();
        }
    }
}
