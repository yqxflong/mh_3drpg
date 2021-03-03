using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerTopLevelUIController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            CurTipLabel = t.GetComponent<UILabel>("Content/Cur/LevelLabel");
            NextTipLabel = t.GetComponent<UILabel>("Content/Next/NextLevelInfo/LevelLabel");
            CurATKTipLabel = t.GetComponent<UILabel>("Content/Cur/Grid/Attr/NumLabel");
            NextATKTipLabel = t.GetComponent<UILabel>("Content/Next/NextLevelInfo/Grid/Attr/NumLabel");
            CurHPTipLabel = t.GetComponent<UILabel>("Content/Cur/Grid/Attr (1)/NumLabel");
            NextHPTipLabel = t.GetComponent<UILabel>("Content/Next/NextLevelInfo/Grid/Attr (1)/NumLabel");
            CurDEFTipLabel = t.GetComponent<UILabel>("Content/Cur/Grid/Attr (2)/NumLabel");
            NextDEFTipLabel = t.GetComponent<UILabel>("Content/Next/NextLevelInfo/Grid/Attr (2)/NumLabel");
            TipLabel = t.GetComponent<UILabel>("Content/RequiredItems/Tip");
            MetCostLabel = t.GetComponent<UILabel>("Content/RequiredItems/Cost/NumLabel");
            GoldCostLabel = t.GetComponent<UILabel>("Content/RequiredItems/Cost/NumLabel (1)");
            NextInfoObj = t.FindEx("Content/Next/NextLevelInfo").gameObject;
            MaxLevelObj = t.FindEx("Content/Next/MaxLevel").gameObject;
            CostObj = t.FindEx("Content/RequiredItems/Cost").gameObject;

            FxList = new List<GameObject>();
            FxList.Add(t.Find("Content/Cur/Grid/Attr/BG/Fx").gameObject);
            FxList.Add(t.Find("Content/Cur/Grid/Attr (1)/BG/Fx").gameObject);
            FxList.Add(t.Find("Content/Cur/Grid/Attr (2)/BG/Fx").gameObject);

            UIButton backButton = t.GetComponent<UIButton>("BG/Top/CloseBtn");
            backButton.onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButton>("UpBtn").onClick.Add(new EventDelegate(OnUpBtnClick));
            
            
            TipLabel.text = string.Format("{0}:", EB.Localizer.GetString("ID_uifont_in_LTLegionManagerMenu_Label_4"));
            TweenList = new List<TweenScale>();
            TweenList.Add(CurATKTipLabel.GetComponent<TweenScale>());
            TweenList.Add(CurHPTipLabel.GetComponent<TweenScale>());
            TweenList.Add(CurDEFTipLabel.GetComponent<TweenScale>());
        }

        public override bool ShowUIBlocker { get { return true; } }
    
        public UILabel CurTipLabel, NextTipLabel;
        public UILabel CurATKTipLabel, NextATKTipLabel;
        public UILabel CurHPTipLabel, NextHPTipLabel;
        public UILabel CurDEFTipLabel, NextDEFTipLabel;
    
        public UILabel TipLabel, MetCostLabel, GoldCostLabel;
    
        public GameObject NextInfoObj,MaxLevelObj,CostObj;
    
        public List< GameObject> FxList;
    
        private bool IsUpRequesting;
        private Hotfix_LT.Data.ProficiencyType CurType= Hotfix_LT.Data.ProficiencyType.AllRound;
        private LTPartnerData partnerData;
        private Hotfix_LT.Data.ProficiencyUpTemplate CurUpData;
        private Hotfix_LT.Data.ProficiencyUpTemplate NextUpData;
    
        private List<TweenScale> TweenList;
        
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            partnerData=(LTPartnerData)param;
            SetInfo();
        }
    
        public override IEnumerator OnAddToStack()
        {
            IsUpRequesting = false;
            return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    
        private void SetInfo(bool showTween=false)
        {
            int curlevel = partnerData.GetProficiencyLevelByType(CurType);
            CurUpData = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetProficiencyUpByTypeAndLevel(CurType, curlevel);
            NextUpData = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetProficiencyUpByTypeAndLevel(CurType, curlevel + 1);
    
            CurTipLabel.text = string.Format("{0} Lv.{1}", EB.Localizer.GetString("ID_CURRENT_LEVEL"), curlevel);
            NextTipLabel.text = string.Format("{0} Lv.{1}", EB.Localizer.GetString("ID_NEXT_LEVEL"), curlevel+1);
    
            for (int i = 0; i < FxList.Count; i++)
            {
                FxList[i].CustomSetActive(false);
            }
            if (CurUpData != null)
            {
                string str = string.Format("+{0}%", CurUpData.ATK * 100);
                if (showTween && !CurATKTipLabel.text.Equals(str))
                {
                    ShowUpFx(0);
                }
                CurATKTipLabel.text = str;
    
                str = string.Format("+{0}%", CurUpData.maxHP * 100);
                if (showTween && !CurHPTipLabel.text.Equals(str))
                {
                    ShowUpFx(1);
                }
                CurHPTipLabel.text = str;
    
                str = string.Format("+{0}%", CurUpData.DEF * 100);
                if (showTween && !CurDEFTipLabel.text.Equals(str))
                {
                    ShowUpFx(2);
                }
                CurDEFTipLabel.text = str;
            }
            else
            {
                CurATKTipLabel.text = "+0%";
                CurHPTipLabel.text = "+0%";
                CurDEFTipLabel.text = "+0%";
            }
    
            if (NextUpData != null)
            {
                NextATKTipLabel.text = string.Format("+{0}%", NextUpData.ATK * 100); 
                NextHPTipLabel.text = string.Format("+{0}%", NextUpData.maxHP * 100); 
                NextDEFTipLabel.text = string.Format("+{0}%", NextUpData.DEF * 100);
    
                string color = LT.Hotfix.Utility.ColorUtility.WhiteColorHexadecimal;
                int resBalance = BalanceResourceUtil.GetUserPoten();
                if (resBalance < NextUpData.potenCost)
                {
                    color = LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
                }
                else
                {
                    color = LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
                }
    
                MetCostLabel.text = string.Format("[{0}]{1}[-]/{2}", color, BalanceResourceUtil.GetUserPoten(),NextUpData.potenCost);
    
                color = LT.Hotfix.Utility.ColorUtility.WhiteColorHexadecimal;
                resBalance = BalanceResourceUtil.GetUserGold();
				int grade = partnerData.HeroInfo.role_grade;
				if (grade > NextUpData.goldCost.Length) grade = NextUpData.goldCost.Length;
                if (resBalance < NextUpData.goldCost[grade - 1])
                {
                    color = LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
                }
                GoldCostLabel.text = string.Format("[{0}]{1}[-]", color,NextUpData .goldCost[grade-1]);
                NextInfoObj.CustomSetActive(true);
                MaxLevelObj.CustomSetActive(false);
                CostObj.CustomSetActive(true);
            }
            else
            {
                NextInfoObj.CustomSetActive(false);
                MaxLevelObj.CustomSetActive(true);
                CostObj.CustomSetActive(false);
            }
        }
    
        private void ShowUpFx(int index)
        {
            FxList[index].CustomSetActive(true);
            TweenList[index].ResetToBeginning();
            TweenList[index].PlayForward();
        }
    
        public void OnUpBtnClick()
        {
            if (NextUpData == null) return;
            if (IsUpRequesting) return;
            IsUpRequesting = true;
            int messageId = 0;
            if (LTPartnerDataManager.Instance.IsCanProficiencyUp(partnerData.StatId, CurType, out messageId))
            {
                LTPartnerDataManager.Instance.ProficiencyUp(partnerData.HeroId, NextUpData.form, NextUpData.type, NextUpData.level, delegate
                {
                    SetInfo(true);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerUIRefresh, CultivateType.Info);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnRefreshPartnerCellRP,true,false);//默认值
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerTopLevelUpSucc);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate,(int)PowerData.RefreshType.Attribute,true);
                    IsUpRequesting = false;
                });
            }
            else
            {
                IsUpRequesting = false;
                if (messageId != 0)
                {
                    if (messageId == 901031)
                    {
                        MessageTemplateManager.ShowMessage(901031, null, delegate (int result)
                        {
                            if (result == 0)
                            {
                                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                                GlobalMenuManager.Instance.Open("LTResourceShopUI");
                            }
                        });
                        return;
                    }
                    MessageTemplateManager.ShowMessage(messageId);
                }
            }
        }
    }
}
