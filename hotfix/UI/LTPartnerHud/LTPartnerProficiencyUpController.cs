using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerProficiencyUpController:DynamicMonoHotfix 
    {
        private Hotfix_LT.Data.ProficiencyType CurType = Hotfix_LT.Data.ProficiencyType.Control;
        
        public List<UILabel> LevelLabelList;
        public List<GameObject> SelectObjList;
    
        public GameObject CostObj;
        public List<UISprite> CostIconList;
        public List<UILabel> CostLabelList;
        
        public LTPartnerProficiencyDesc ProficiencyDescView;
    
        public UILabel TipLabel;
    
        public UIButton UpBtn;
        public UISprite UpSprite;
    
        private LTPartnerData partnerData;
        private Hotfix_LT.Data.ProficiencyUpTemplate CurUpData;
        private Hotfix_LT.Data.ProficiencyUpTemplate NextUpData;
    
        public override void Awake()
        {
            base.Awake();
            LevelLabelList = new List<UILabel>();
            LevelLabelList.Add(mDMono.transform.GetComponent<UILabel>("1/Label"));
            LevelLabelList.Add(mDMono.transform.GetComponent<UILabel>("2/Label"));
            LevelLabelList.Add(mDMono.transform.GetComponent<UILabel>("3/Label"));
            LevelLabelList.Add(mDMono.transform.GetComponent<UILabel>("4/Label"));
            LevelLabelList.Add(mDMono.transform.GetComponent<UILabel>("5/Label"));
            SelectObjList = new List<GameObject>();
            SelectObjList.Add(mDMono.transform.Find("1/SelectBG").gameObject);
            SelectObjList.Add(mDMono.transform.Find("2/SelectBG").gameObject);
            SelectObjList.Add(mDMono.transform.Find("3/SelectBG").gameObject);
            SelectObjList.Add(mDMono.transform.Find("4/SelectBG").gameObject);
            SelectObjList.Add(mDMono.transform.Find("5/SelectBG").gameObject);
            CostObj = mDMono.transform.Find("RequiredItems").gameObject;
            CostIconList = new List<UISprite>();
            CostIconList.Add(mDMono.transform.GetComponent<UISprite>("RequiredItems/Item1"));
            CostIconList.Add(mDMono.transform.GetComponent<UISprite>("RequiredItems/Item2"));
            CostLabelList = new List<UILabel>();
            CostLabelList.Add(mDMono.transform.GetComponent<UILabel>("RequiredItems/NumLabel1"));
            CostLabelList.Add(mDMono.transform.GetComponent<UILabel>("RequiredItems/NumLabel2"));
            ProficiencyDescView = mDMono.transform.parent.parent.GetMonoILRComponent<LTPartnerProficiencyDesc>("ProficiencyUpDesc");
            TipLabel = mDMono.transform.GetComponent<UILabel>("RequiredItems/Tip");
            UpBtn = mDMono.transform.GetComponent<UIButton>("ProficiencyUpBtn");
            UpSprite = mDMono.transform.GetComponent<UISprite>("ProficiencyUpBtn/Sprite");
            TipLabel.text = string.Format("{0}:", EB.Localizer.GetString("ID_uifont_in_LTLegionManagerMenu_Label_4"));
            CurType = Hotfix_LT.Data.ProficiencyType.Control;
            
            mDMono.transform.GetComponent<UIButton>("1/AllRound").onClick.Add(new EventDelegate(() => OnProficiencyItemClick(mDMono.transform.GetComponent<UIButton>("1/AllRound"))));
            mDMono.transform.GetComponent<UIButton>("2/Control").onClick.Add(new EventDelegate(() => OnProficiencyItemClick(mDMono.transform.GetComponent<UIButton>("2/Control"))));
            mDMono.transform.GetComponent<UIButton>("3/Strong").onClick.Add(new EventDelegate(() => OnProficiencyItemClick(mDMono.transform.GetComponent<UIButton>("3/Strong"))));
            mDMono.transform.GetComponent<UIButton>("4/Rage").onClick.Add(new EventDelegate(() => OnProficiencyItemClick(mDMono.transform.GetComponent<UIButton>("4/Rage"))));
            mDMono.transform.GetComponent<UIButton>("5/Absorbed").onClick.Add(new EventDelegate(() => OnProficiencyItemClick(mDMono.transform.GetComponent<UIButton>("5/Absorbed"))));
            
            mDMono.transform.GetComponent<ConsecutiveClickCoolTrigger>("ProficiencyUpBtn").clickEvent.Add(new EventDelegate(OnProficiencyUpBtnClick));

        }
    
        public void Show(LTPartnerData data)
        {
            if (data == null)
            {
                Hide();
                return;
            }
            partnerData = data;
            SetInfo();
            SetSelectUI();
            mDMono.gameObject.CustomSetActive(true);
        }
    
        private void SetInfo()
        {
            var list =Hotfix_LT.Data.CharacterTemplateManager .Instance .GetAllProficiencyDesc();
            for (int i=0;i< LevelLabelList.Count; i++)
            {
                LevelLabelList[i].text = string.Format("{0} Lv.{1}", ((i < list.Count) ? list[i].name : string.Empty), partnerData.GetProficiencyLevelByType(list[i].id));
            }
    
            int curlevel = partnerData.GetProficiencyLevelByType(CurType);
            CurUpData = Data.CharacterTemplateManager.Instance.GetProficiencyUpByTypeAndLevel(CurType, curlevel);
            NextUpData = Data.CharacterTemplateManager.Instance.GetProficiencyUpByTypeAndLevel(CurType, curlevel+1);

			var desc = Data.CharacterTemplateManager.Instance.GetProficiencyDescByType(CurType);
			int grade = partnerData.HeroInfo.role_grade > desc.limit.Length ? desc.limit.Length+1 : partnerData.HeroInfo.role_grade;
			int levelMax = desc.limit[grade - 1];

			int cost = 0;
            if (NextUpData != null && curlevel < levelMax)
            {

                if (NextUpData.chipCost.Length >= grade && NextUpData.chipCost[0] >= 0 && cost < CostIconList.Count)
                {
                    CostIconList[cost].spriteName = "Ty_Icon_Suipian";
                    CostLabelList[cost].text = NextUpData.chipCost[grade-1].ToString();
                    cost++;
                }
                if (NextUpData.potenCost > 0 && cost < CostIconList.Count)
                {
                    CostIconList[cost].spriteName = "Ty_Icon_Qianneng";
                    CostLabelList[cost].text = NextUpData.potenCost.ToString();
                    cost++;
                }
                if (NextUpData.goldCost.Length >= grade && NextUpData.goldCost[0] >= 0 && cost < CostIconList.Count)
                {
                    CostIconList[cost].spriteName = "Ty_Icon_Gold";
                    CostLabelList[cost].text = NextUpData.goldCost[grade-1].ToString();
                    cost++;
                }
                CostObj.CustomSetActive(true);
                UpBtn.isEnabled = true;
                UpSprite.color =  new Color(1, 1, 1) ;
            }
            else
            {
                CostObj.CustomSetActive(false);
                UpBtn.isEnabled = false;
                UpSprite.color = new Color(1, 0, 1);
            }
        }
    
        private void SetSelectUI()
        {
            for(int i = 0; i < SelectObjList.Count; i++)
            {
                SelectObjList[i].CustomSetActive((int)CurType==i+1);
            }
        }
    
        private void PlayUpFx()
        {
            Data.ProficiencyUpTemplate data = new Data.ProficiencyUpTemplate(CurUpData, NextUpData);
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, LTPartnerProficiencyDesc.GetDesc(data));

            for (int i = 0; i < SelectObjList.Count; i++)
            {
                if ((int)CurType == i + 1)
                {
                    var comp = SelectObjList[i].GetComponent<ParticleSystemUIComponent>();

                    if (comp != null)
                    {
                        comp.Play();
                    }
                }
            }
        }
    
        public void Hide()
        {
            mDMono.gameObject.CustomSetActive(false);
        }
    
        private bool IsUpRequesting = false;
        public void OnProficiencyUpBtnClick()
        {
            if (NextUpData == null) return;
            if (IsUpRequesting) return;
            IsUpRequesting = true;
            int messageId = 0;
            if (LTPartnerDataManager .Instance .IsCanProficiencyUp(partnerData.StatId, CurType, out messageId))
            {
                LTPartnerDataManager.Instance.ProficiencyUp(partnerData.HeroId, NextUpData.form, NextUpData.type , NextUpData.level, delegate
                {
                    PlayUpFx();
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate,1,true);
                    SetInfo();
                    IsUpRequesting = false;
                });
            }
            else
            {
                if (messageId != 0)
                {
                    IsUpRequesting = false;
                    if (messageId == 902310)
                    {
                        Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerHoleUpError);
                        return;
                    }
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
    
        public void OnProficiencyItemClick(UIButton btn)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            var type =btn.name;
            if (!CurType.ToString().Equals(type))
            {
                CurType =(Hotfix_LT.Data.ProficiencyType)Enum.Parse(typeof (Hotfix_LT.Data.ProficiencyType),btn.name);
                SetInfo();
                SetSelectUI();
            }
            ProficiencyDescView.ShowUI(CurUpData, NextUpData);
        }
    }
}
