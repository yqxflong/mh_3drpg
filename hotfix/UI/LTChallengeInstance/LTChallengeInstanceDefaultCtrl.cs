using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceDefaultCtrl : UIControllerHotfix
    {
        public static LTChallengeInstanceDefaultCtrl Instance { get; private set; }

        public override void Awake()
        {
            base.Awake();
            Instance = this;
            var t = controller.transform;
            UIButton bg = t.GetComponent<UIButton>("Bg");
            bg.onClick.Add(new EventDelegate ( OnCancelButtonClick));
            com = t.GetMonoILRComponent<CommonRatingDialogLT>("Content/CommonBuddyRatingGM");
            Tip = t.FindEx("Content/Tips").gameObject;
            ConfirmBtn = t.FindEx("Content/ConfirmBtn").gameObject;
            TryBtn = t.FindEx("Content/TryBtn").gameObject;

            t.GetComponent<UIButton>("Content/TryBtn").onClick.Add(new EventDelegate(OnTryBtnClick));
            t.GetComponent<UIButton>("Content/ConfirmBtn").onClick.Add(new EventDelegate(OnConfirmBtnClick));
        }

        public override void OnDestroy()
        {
            Instance = null;
        }

        private System.Action<int> callback;
    
        public CommonRatingDialogLT com;
    
        public GameObject Tip;
    
        public bool isWon;
    
        private bool isConfirm;
    
        private bool isOver;
    
        public GameObject ConfirmBtn;
    
        public GameObject TryBtn;
    
        public override bool IsFullscreen()
        {
            return true;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return false;
            }
        }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null)
            {
                Hashtable table = param as Hashtable;
                isWon = (bool)table["flag"];
                callback = table["action"] as System.Action<int>;
                isConfirm = (bool)table["isConfirm"];
            }
            Tip.CustomSetActive(false);
            TryBtn.CustomSetActive(false);
            ConfirmBtn.CustomSetActive(false);
        }
    
        public override IEnumerator OnAddToStack()
        {
    
            yield return base.OnAddToStack();
            isOver = false;
    
            com.IsWon = isWon;
            if (isWon)
            {
                FusionAudio.PostEvent("MUS/CombatView/Stinger/Victory",controller.gameObject, true);
            }
            else
            {
                FusionAudio.PostEvent("MUS/CombatView/Stinger/Defeat", controller.gameObject, true);
            }
            com.IsShowHp = true;
            com.IsShowTempHp = isConfirm;
            com.mDMono.gameObject.CustomSetActive(true);
            com.onShownAnimCompleted = () => 
            {
                Tip.CustomSetActive(!isConfirm);
                TryBtn.CustomSetActive(isConfirm);
                ConfirmBtn.CustomSetActive(isConfirm);
            };
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 1f);
            StopAllCoroutines();
            com.mDMono.gameObject.CustomSetActive(false);
            FusionAudio.PostEvent("MUS/CombatView/Stinger/Victory", controller.gameObject, false);
            FusionAudio.PostEvent("MUS/CombatView/Stinger/Defeat", controller.gameObject, false);
            DestroySelf();
            yield break;
        }
    
        public override void OnCancelButtonClick()
        {
            if (isConfirm) return;

            if (callback != null)
            {
                callback(1);
                callback = null;
            }
            base.OnCancelButtonClick();
        }
    
        public void OnTryBtnClick()
        {
            if (callback != null)
            {
                callback(0);
                callback = null;
            }
            base.OnCancelButtonClick();
        }
    
        public void OnConfirmBtnClick()
        {
            if (callback != null)
            {
                callback(1);
                callback = null;
            }
            base.OnCancelButtonClick();
        }
    }
}
