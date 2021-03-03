using System.Collections;
using System.Collections.Generic;
using GM;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstancePortalCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            NormalContend = t.FindEx("Contend").gameObject;
            BossContend = t.FindEx("BossContend").gameObject;
            TitleLabel = t.GetComponent<UILabel>("Bg/Top/Title");
            controller.backButton = t.GetComponent<UIButton>("Bg/Top/CancelBtn");


            t.GetComponent<ConsecutiveClickCoolTrigger>("Contend/BtnList/KeepBtn").clickEvent.Add(new EventDelegate(OnKeepBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Contend/BtnList/LeaveBtn").clickEvent.Add(new EventDelegate(OnLeaveBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Contend/BtnList/NextBtn").clickEvent.Add(new EventDelegate(OnNextBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("BossContend/BtnList/KeepBtn").clickEvent.Add(new EventDelegate(OnKeepBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("BossContend/BtnList/LeaveBtn").clickEvent.Add(new EventDelegate(OnLeaveBtnClick));

        }
        
        public GameObject NormalContend;
    
        public GameObject BossContend;
    
        public UILabel TitleLabel;
    
        private bool mIsBoss = false;
    
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
            mIsBoss = (bool)param;
        }
    
        public override IEnumerator OnAddToStack()
        {
            TitleLabel.text = mIsBoss ? EB.Localizer.GetString("ID_CHALLENGE_PORTAL_BOSS") : EB.Localizer.GetString("ID_CHALLENGE_PORTAL_GENERAL");
            BossContend.CustomSetActive(mIsBoss);
            NormalContend.CustomSetActive(!mIsBoss);
            yield return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            StopAllCoroutines();
            DestroySelf();
            yield break;
        }
    
        public override void OnCancelButtonClick()
        {
            controller .Close();
        }
    
        public void OnKeepBtnClick()
        {
            OnCancelButtonClick();
        }
    
        public void OnLeaveBtnClick()
        {
            if (mIsBoss)
            {
                LTInstanceMapModel.Instance.RequestChallengeFinshChapter(0, LTInstanceMapModel.Instance.CurLevelNum, delegate
                {
                    FusionAudio.PostEvent("UI/Floor/Transfer");
                    OnCancelButtonClick();
                });
                return;
            }
    
            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstancePortalCtrl_1452"), (int result) =>
            {
                switch (result)
                {
                    case 0:
                        LTInstanceMapModel.Instance.RequestChallengeFinshChapter(0, LTInstanceMapModel.Instance.CurLevelNum, delegate
                        {
                            FusionAudio.PostEvent("UI/Floor/Transfer");
                            OnCancelButtonClick();
                        });
                        break;
                    case 1:
                    case 2:
                        break;
                }
            });
        }
    
        private bool btnLimit = false;
        public void OnNextBtnClick()
        {
            if (btnLimit) return;
            btnLimit = true;
            LTInstanceMapModel.Instance.RequestChallengeFinshChapter(1, LTInstanceMapModel.Instance.CurLevelNum, delegate
            {
                FusionAudio.PostEvent("UI/Floor/Transfer");
                if (controller.gameObject != null)
                {
                    OnCancelButtonClick();
                }
                btnLimit = false;
            });
        }
    }
}
