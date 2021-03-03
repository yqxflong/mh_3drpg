using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceQuitCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TipLabel = t.GetComponent<UILabel>("Content/TipLabel");
            controller.backButton = t.GetComponent<UIButton>("bg/Top/CancelBtn");


            t.GetComponent<ConsecutiveClickCoolTrigger>("ButtonGrid/CancelButton").clickEvent.Add(new EventDelegate(OnEndBtnClicl));
            t.GetComponent<ConsecutiveClickCoolTrigger>("ButtonGrid/OKButton").clickEvent.Add(new EventDelegate(OnOKBtnClick));

        }

        public UILabel TipLabel;
    
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
    
        private System.Action OnBeforeClose;
        private System.Action OnAfterClose;
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Hashtable tempTable = param as Hashtable;
    
            OnBeforeClose = tempTable["BeforeServerAction"] as System.Action;
            OnAfterClose = tempTable["AfterServerAction"] as System.Action;
    
            if (tempTable["isAlienMaze"] != null&& (bool)tempTable["isAlienMaze"])
            {
                TipLabel.text = EB.Localizer.GetString("ID_ALIEN_MAZE_LEAVE_TIP");
            }
            else
            {
                TipLabel.text = EB.Localizer.GetString("ID_uifont_in_LTChallengeInstanceQuitView_Label (1)_1");
                
            }
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    
        public void OnOKBtnClick()
        {
            ClickOnBeforeClose();// 离开副本的时候清除掉副本移动数据
            LTInstanceMapModel.Instance.IsTemporaryQuit = true;
            FusionTelemetry.CampaignDate.PostEvent(FusionTelemetry.CampaignDate.Challenge, LTInstanceMapModel.Instance.CurLevelNum.ToString(), 2, 1);
            LTInstanceMapModel.Instance.RequestLeaveChapter("chal", delegate
            {
                if (LTInstanceMapModel.Instance.IsInsatnceViewAction())
                {
                    LTInstanceMapModel.Instance.SwitchViewAction(false, true, delegate               
                    {
                        ClickOnAfterClose();
                        if(controller!=null) controller.Close();
                    });
                }
            });
        }
    
        public void OnEndBtnClicl()
        {
            ClickOnBeforeClose();// 离开副本的时候清除掉副本移动数据
            LTInstanceMapModel.Instance.IsTemporaryQuit = false;
            FusionTelemetry.CampaignDate.PostEvent(FusionTelemetry.CampaignDate.Challenge, LTInstanceMapModel.Instance.CurLevelNum.ToString(), 2, 2);
            LTInstanceMapModel.Instance.RequestLeaveChapter("", delegate
            {
                if (LTInstanceMapModel.Instance.IsInsatnceViewAction())
                {
                    LTInstanceMapModel.Instance.SwitchViewAction(false, true, delegate
                    {
                        ClickOnAfterClose();
                        if (controller != null) controller.Close();
                    });
                }
            });
        }
    
        private void ClickOnBeforeClose()
        {
            if (OnBeforeClose != null)
            {
                OnBeforeClose();
            }
        }
    
        private void ClickOnAfterClose()
        {
            if (OnAfterClose != null)
            {
                OnAfterClose();
            }
        }
    }
}
