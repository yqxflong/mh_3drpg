using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerGetParExp : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TitleDesLabel = t.GetComponent<UILabel>("Label");
            GetExpObj = t.FindEx("GetExp").gameObject;
            PlayerLevelUpObj = t.FindEx("PlayerLevelUp").gameObject;
            controller.backButton = t.GetComponent<UIButton>("CloseBtn");
            t.GetComponent<UIButton>("GetExp/MainFBBtn").onClick.Add(new EventDelegate(OnMainFBBtnClick));
            t.GetComponent<UIButton>("GetExp/ExpFBBtn").onClick.Add(new EventDelegate(OnExpFBBtnClick));
            t.GetComponent<UIButton>("GetExp/OpenParExpBtn").onClick.Add(new EventDelegate(OnOpenParExpBtnClick));
            t.GetComponent<UIButton>("GetExp/OpenGiftBtn").onClick.Add(new EventDelegate(OnOpenGiftBtnClick));
            t.GetComponent<UIButton>("PlayerLevelUp/MainFBBtn").onClick.Add(new EventDelegate(OnMainFBBtnClick));
            t.GetComponent<UIButton>("PlayerLevelUp/DailyTask").onClick.Add(new EventDelegate(OnOpenDailyBtnClick));
            t.GetComponent<UIButton>("PlayerLevelUp/Upgrade").onClick.Add(new EventDelegate(OnOpenPartnerUpgradeBtnClick));
        }

        private enum EUpTipType
        {
            GetExp = 0,
            PlayerLevelUp,
        }
        public UILabel TitleDesLabel;
        public GameObject GetExpObj, PlayerLevelUpObj;
        private EUpTipType curType = EUpTipType.GetExp;
        public override bool ShowUIBlocker { get { return true; } }
        public override bool IsFullscreen()
        {
            return false;
        }
        public override void SetMenuData(object param)
        {
            if (param != null)
            {
                string temp = param.ToString();
                if (temp.Equals("GetExp"))
                {
                    curType = EUpTipType.GetExp;
                }
                else if (temp.Equals("PlayerLevelUp"))
                {
                    curType = EUpTipType.PlayerLevelUp;
                }
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            
            if(curType == EUpTipType.GetExp)
            {
                TitleDesLabel.text = TitleDesLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_uifont_in_LTPartnerHud_Label_30");
                GetExpObj.CustomSetActive(true);
                PlayerLevelUpObj.CustomSetActive(false);
            }
            else if(curType == EUpTipType.PlayerLevelUp)
            {
                TitleDesLabel.text = TitleDesLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_uifont_in_LTPartnerHud_Label_34");
                GetExpObj.CustomSetActive(false);
                PlayerLevelUpObj.CustomSetActive(true);
            }
            return base.OnAddToStack();
    
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            return base.OnRemoveFromStack();
        }
        
        public void OnMainFBBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTInstanceMapHud", 1);
            controller.Close();
        }
    
        public void OnExpFBBtnClick()
        {
            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10021);
            if (!ft.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                return;
            }
            GlobalMenuManager.Instance.Open("LTResourceInstanceUI", "Exp");
    
            controller.Close();
        }
    
        public void OnOpenParExpBtnClick()
        {
            
            GlobalMenuManager.Instance.Open("LTResourceShopUI");
            controller.Close();
        }
    
        public void OnOpenGiftBtnClick()
        {      
            GlobalMenuManager.Instance.Open("LTChargeStoreHud");
            controller.Close();
        }
    
        public void OnOpenPartnerUpgradeBtnClick()
        {
            Hotfix_LT.Messenger.Raise<int>(Hotfix_LT.EventName.OnPartnerTurnToUpgrade, 1);
            controller.Close();
        }
    
        public void OnOpenDailyBtnClick()
        {
            GlobalMenuManager.Instance.Open("NormalTaskView", null);
            controller.Close();
        }
    }
}
