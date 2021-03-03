using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPartnerGuardAttrController:DynamicMonoHotfix
    {
        private LTPartnerGuardUIController mc;
        
        public UILabel Level_Courage_01;
        public UILabel Level_Loyalty_02;
        public UILabel Level_Team_03;
        
        public UILabel Condition_Courage_01;
        public UILabel Condition_Loyalty_02;
        public UILabel Condition_Team_03;
        
        public UILabel[] Start_AttackLabel_01;
        public UILabel[] Start_HpLabel_02;
        public UILabel[] Start_DefLabel_03;
        public UILabel[] Start_AttackLabel_04;
        public UILabel[] Start_HpLabel_05;
        public UILabel[] Start_DefLabel_06;
        public UILabel[] Start_AttackLabel_07;
        public UILabel[] Start_HpLabel_08;
        public UILabel[] Start_DefLabel_09;
        
        public UILabel[] End_AttackLabel_01;
        public UILabel[] End_HpLabel_02;
        public UILabel[] End_DefLabel_03;
        public UILabel[] End_AttackLabel_04;
        public UILabel[] End_HpLabel_05;
        public UILabel[] End_DefLabel_06;
        public UILabel[] End_AttackLabel_07;
        public UILabel[] End_HpLabel_08;
        public UILabel[] End_DefLabel_09;

        public ParticleSystemUIComponent[] fxs;
        public ParticleSystemUIComponent[] fxs_label;
        
        //1级获得 2级觉醒
        public int num_Level_Courage_01
        {
            get
            {
                int num = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.fetter.{1}.{2}", mc.ownData.HeroId, m_CurrentSelectIndex,1), out num);
                return num;
            }
        } 
        //1级品阶5 2级品阶8 3级品阶11 4级品阶13
        public int num_Level_Loyalty_02  {
            get
            {
                int num = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.fetter.{1}.{2}", mc.ownData.HeroId, m_CurrentSelectIndex,2), out num);
                return num;
            }
        } 
        //1级星级3 2级星级4  3级新级星级5 4级新级6
        public int num_Level_Team_03  {
            get
            {
                int num = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.fetter.{1}.{2}", mc.ownData.HeroId, m_CurrentSelectIndex,3), out num);
                return num;
            }
        }     

        private int m_CurrentSelectIndex;
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            mc = t.parent.GetUIControllerILRComponent<LTPartnerGuardUIController>();
            Level_Courage_01= t.GetComponent<UILabel>("Item/NextLevelInfo/SignLabel/LevelLabel");
            Level_Loyalty_02= t.GetComponent<UILabel>("Item (1)/NextLevelInfo/SignLabel/LevelLabel");
            Level_Team_03= t.GetComponent<UILabel>("Item (2)/NextLevelInfo/SignLabel/LevelLabel");
            
            Condition_Courage_01= t.GetComponent<UILabel>("Item/NextLevelInfo/Condition");
            Condition_Loyalty_02= t.GetComponent<UILabel>("Item (1)/NextLevelInfo/Condition");
            Condition_Team_03= t.GetComponent<UILabel>("Item (2)/NextLevelInfo/Condition");
            
            fxs = new ParticleSystemUIComponent[3];
            fxs[0] = t.GetComponent<ParticleSystemUIComponent>("Item/NextLevelInfo/fx");
            fxs[1] = t.GetComponent<ParticleSystemUIComponent>("Item (1)/NextLevelInfo/fx");
            fxs[2] = t.GetComponent<ParticleSystemUIComponent>("Item (2)/NextLevelInfo/fx");
            
            fxs_label = new ParticleSystemUIComponent[3];//fx_hb_ui_huwei
            fxs_label[0] = t.GetComponent<ParticleSystemUIComponent>("Item/NextLevelInfo/UpBtn/Label/fx");
            fxs_label[1] = t.GetComponent<ParticleSystemUIComponent>("Item (1)/NextLevelInfo/UpBtn/Label/fx");
            fxs_label[2] = t.GetComponent<ParticleSystemUIComponent>("Item (2)/NextLevelInfo/UpBtn/Label/fx");
            
            Start_AttackLabel_01 = t.Find("Item/NextLevelInfo/Grid/Attr/From").GetComponents<UILabel>();
            Start_HpLabel_02 = t.Find("Item/NextLevelInfo/Grid/Attr (1)/From").GetComponents<UILabel>();
            Start_DefLabel_03 = t.Find("Item/NextLevelInfo/Grid/Attr (2)/From").GetComponents<UILabel>();
            Start_AttackLabel_04 = t.Find("Item (1)/NextLevelInfo/Grid/Attr/From").GetComponents<UILabel>();
            Start_HpLabel_05 = t.Find("Item (1)/NextLevelInfo/Grid/Attr (1)/From").GetComponents<UILabel>();
            Start_DefLabel_06 = t.Find("Item (1)/NextLevelInfo/Grid/Attr (2)/From").GetComponents<UILabel>();
            Start_AttackLabel_07 = t.Find("Item (2)/NextLevelInfo/Grid/Attr/From").GetComponents<UILabel>();
            Start_HpLabel_08 = t.Find("Item (2)/NextLevelInfo/Grid/Attr (1)/From").GetComponents<UILabel>();
            Start_DefLabel_09 = t.Find("Item (2)/NextLevelInfo/Grid/Attr (2)/From").GetComponents<UILabel>();
            
            End_AttackLabel_01 = t.Find("Item/NextLevelInfo/Grid/Attr/To").GetComponents<UILabel>();
            End_HpLabel_02 = t.Find("Item/NextLevelInfo/Grid/Attr (1)/To").GetComponents<UILabel>();
            End_DefLabel_03 = t.Find("Item/NextLevelInfo/Grid/Attr (2)/To").GetComponents<UILabel>();
            End_AttackLabel_04 = t.Find("Item (1)/NextLevelInfo/Grid/Attr/To").GetComponents<UILabel>();
            End_HpLabel_05 = t.Find("Item (1)/NextLevelInfo/Grid/Attr (1)/To").GetComponents<UILabel>();
            End_DefLabel_06 = t.Find("Item (1)/NextLevelInfo/Grid/Attr (2)/To").GetComponents<UILabel>();
            End_AttackLabel_07 = t.Find("Item (2)/NextLevelInfo/Grid/Attr/To").GetComponents<UILabel>();
            End_HpLabel_08 = t.Find("Item (2)/NextLevelInfo/Grid/Attr (1)/To").GetComponents<UILabel>();
            End_DefLabel_09 = t.Find("Item (2)/NextLevelInfo/Grid/Attr (2)/To").GetComponents<UILabel>();
            
            
            t.GetComponent<UIButton>("Item/NextLevelInfo/UpBtn").onClick.Add(new EventDelegate(() =>
            {
                Activation(Condition_Courage_01,1,num_Level_Courage_01+1); }));
            t.GetComponent<UIButton>("Item (1)/NextLevelInfo/UpBtn").onClick.Add(new EventDelegate(() =>
            {
                Activation(Condition_Loyalty_02,2,num_Level_Loyalty_02+1); }));
            t.GetComponent<UIButton>("Item (2)/NextLevelInfo/UpBtn").onClick.Add(new EventDelegate(() =>
            {
                Activation(Condition_Team_03,3,num_Level_Team_03+1); }));
        }

        private void Activation(UILabel label,int condition,int toLevel)
        {
            if (label.color==Color.clear)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_GUARD_TIP_02"), null);
                return;
            }
            bool finish = (label.color == Color.black ? true : false);
            if (!finish)
            {
                //"提升条件尚未满足，无法提升!"
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_GUARD_TIP_01"), null);
            }
            else
            {
                LTPartnerDataManager.Instance.RequestLevelupFetterBuddy(mc.ownData.HeroId,m_CurrentSelectIndex,
                    condition, toLevel, (suc) =>
                    {
                        if (suc)
                        {
                            fxs[condition-1].gameObject.SetActive(false);
                            fxs[condition-1].gameObject.SetActive(true);
                            SetData(m_CurrentSelectIndex);
                            //红点
                            Messenger.Raise(Hotfix_LT.EventName.PartnerCultivateRP, true);
                            Messenger.Raise(Hotfix_LT.EventName.OnParnerGuardChange);
                            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnRefreshPartnerCellRP, true,true);
                            mc.HeroItem[m_CurrentSelectIndex-1].SetRedPoint();
                            //属性变化
                            Messenger.Raise(Hotfix_LT.EventName.OnPartnerUIRefresh, CultivateType.Info);
                            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate,1,true);
                        }
                    });
                
            }
        }


        private HeroGuardTemplate heroGuardTemplate_01;
        private HeroGuardTemplate heroGuardTemplate_02;
        private HeroGuardTemplate heroGuardTemplate_03;
        
        private HeroGuardTemplate next_heroGuardTemplate_01;
        private HeroGuardTemplate next_heroGuardTemplate_02;
        private HeroGuardTemplate next_heroGuardTemplate_03;
        
        /// <summary>
        /// 羁绊位置 按配置表中的来 1，2，3
        /// </summary>
        /// <param name="index"></param>
        public void SetData(int index)
        {
            m_CurrentSelectIndex = index;
            heroGuardTemplate_01=CharacterTemplateManager.Instance.GetGuard(index,1,num_Level_Courage_01);
            heroGuardTemplate_02=CharacterTemplateManager.Instance.GetGuard(index,2,num_Level_Loyalty_02);
            heroGuardTemplate_03=CharacterTemplateManager.Instance.GetGuard(index,3,num_Level_Team_03);
            
            next_heroGuardTemplate_01=CharacterTemplateManager.Instance.GetGuard(index,1,num_Level_Courage_01+1);
            next_heroGuardTemplate_02=CharacterTemplateManager.Instance.GetGuard(index,2,num_Level_Loyalty_02+1);
            next_heroGuardTemplate_03=CharacterTemplateManager.Instance.GetGuard(index,3,num_Level_Team_03+1);

            SetUILabel();
        }
        
        

        private void SetUILabel()
        {
            // ID_codefont_in_LadderMatchSuccessUIController_3473
            Level_Courage_01.text =EB.Localizer.GetString("ID_codefont_in_LadderMatchSuccessUIController_3473")+num_Level_Courage_01;
            Level_Loyalty_02.text =EB.Localizer.GetString("ID_codefont_in_LadderMatchSuccessUIController_3473")+num_Level_Loyalty_02;
            Level_Team_03.text =EB.Localizer.GetString("ID_codefont_in_LadderMatchSuccessUIController_3473")+num_Level_Team_03;
            
            LTUIUtil.SetText(Start_AttackLabel_01,ADDStringFormat(heroGuardTemplate_01.IncATK));
            LTUIUtil.SetText(Start_HpLabel_02,ADDStringFormat(heroGuardTemplate_01.IncMaxHP));
            LTUIUtil.SetText(Start_DefLabel_03,ADDStringFormat(heroGuardTemplate_01.IncDEF));
            LTUIUtil.SetText(Start_AttackLabel_04,ADDStringFormat(heroGuardTemplate_02.IncATK));
            LTUIUtil.SetText(Start_HpLabel_05,ADDStringFormat(heroGuardTemplate_02.IncMaxHP));
            LTUIUtil.SetText(Start_DefLabel_06,ADDStringFormat(heroGuardTemplate_02.IncDEF));
            LTUIUtil.SetText(Start_AttackLabel_07,ADDStringFormat(heroGuardTemplate_03.IncATK));
            LTUIUtil.SetText(Start_HpLabel_08,ADDStringFormat(heroGuardTemplate_03.IncMaxHP));
            LTUIUtil.SetText(Start_DefLabel_09,ADDStringFormat(heroGuardTemplate_03.IncDEF));
            
            LTUIUtil.SetText(End_AttackLabel_01,StringFormat(next_heroGuardTemplate_01.IncATK));
            LTUIUtil.SetText(End_HpLabel_02,StringFormat(next_heroGuardTemplate_01.IncMaxHP));
            LTUIUtil.SetText(End_DefLabel_03,StringFormat(next_heroGuardTemplate_01.IncDEF));
            LTUIUtil.SetText(End_AttackLabel_04,StringFormat(next_heroGuardTemplate_02.IncATK));
            LTUIUtil.SetText(End_HpLabel_05,StringFormat(next_heroGuardTemplate_02.IncMaxHP));
            LTUIUtil.SetText(End_DefLabel_06,StringFormat(next_heroGuardTemplate_02.IncDEF));
            LTUIUtil.SetText(End_AttackLabel_07,StringFormat(next_heroGuardTemplate_03.IncATK));
            LTUIUtil.SetText(End_HpLabel_08,StringFormat(next_heroGuardTemplate_03.IncMaxHP));
            LTUIUtil.SetText(End_DefLabel_09,StringFormat(next_heroGuardTemplate_03.IncDEF));

            SetConditionText();
        }

        private void SetConditionText()
        { 
            //"拥有该护卫":"觉醒该护卫"
            if (next_heroGuardTemplate_01==heroGuardTemplate_01)
            {
                Condition_Courage_01.color=Color.clear;
                SetMaxState(1,false);
                fxs_label[0].gameObject.SetActive(false);
            }
            else
            {
                string param_01= next_heroGuardTemplate_01.Param;
                Condition_Courage_01.text = int.Parse(param_01)==0?EB.Localizer.GetString("ID_PARTNER_GUARD_CONDITION_01"):EB.Localizer.GetString("ID_PARTNER_GUARD_CONDITION_02");
                bool finish_01 = ((int.Parse(param_01) == 0) && (mc.datas[m_CurrentSelectIndex - 1].heroLevel > 0)) ||
                                 ((int.Parse(param_01) == 1) && (mc.datas[m_CurrentSelectIndex - 1].isawaken > 0));
                SetTextColor(Condition_Courage_01,finish_01,1);  
                SetMaxState(1,true);
            }
            //"该护卫达到紫色品阶"
          if (next_heroGuardTemplate_02==heroGuardTemplate_02)
          {
              Condition_Loyalty_02.color=Color.clear;
              SetMaxState(2,false);
              fxs_label[1].gameObject.SetActive(false);
          }
          else
          {
              string param_02= next_heroGuardTemplate_02.Param;
              Condition_Loyalty_02.text =GetUpgradeNameByLevel(param_02);
              bool finish_02 = mc.datas[m_CurrentSelectIndex - 1].UpGradeId >= int.Parse(param_02);
              SetTextColor(Condition_Loyalty_02,finish_02,2);  
              SetMaxState(2,true);
          }
          //"该护卫达到4星"  
          if (next_heroGuardTemplate_03==heroGuardTemplate_03)
          {
              Condition_Team_03.color=Color.clear;
              SetMaxState(3,false);
              fxs_label[2].gameObject.SetActive(false);
          }
          else
          {
              string param_03= next_heroGuardTemplate_03.Param;
              Condition_Team_03.text =string.Format(EB.Localizer.GetString("ID_PARTNER_GUARD_CONDITION_04"), param_03);
              bool finish_03 = mc.datas[m_CurrentSelectIndex - 1].star >= int.Parse(param_03);
              SetTextColor(Condition_Team_03,finish_03,3);  
              SetMaxState(3,true);
          }
         
        }

        private string StringFormat(float num)
        {
            return string.Format("{0}%", (num * 100));
        }
        private string ADDStringFormat(float num)
        {
            return string.Format("+{0}%", (num * 100));
        }

        private void SetTextColor(UILabel label,bool finish,int con)
        {
            if(mc.datas[m_CurrentSelectIndex - 1].heroLevel<=0)
            {
                finish = false;
            }
            label.color = finish ? Color.black : Color.red;
            fxs_label[con-1].gameObject.SetActive(finish);
        }
        
        private string GetUpgradeNameByLevel(string upgradeIdStr)
        {
            int upgradeId;
            int.TryParse(upgradeIdStr,out upgradeId);
      
            string color=  CharacterTemplateManager.Instance.GetUpGradeInfo(upgradeId,eRoleAttr.Feng).name;
            return string.Format(EB.Localizer.GetString("ID_PARTNER_GUARD_CONDITION_03"), color);
        }

        private void SetMaxState(int condition,bool show)
        {
            switch (condition)
            {
                case 1:
                    End_AttackLabel_01[0].gameObject.SetActive(show);
                    End_HpLabel_02[0].gameObject.SetActive(show);
                    End_DefLabel_03[0].gameObject.SetActive(show);
                    break;
                case 2:
                    End_AttackLabel_04[0].gameObject.SetActive(show);
                    End_HpLabel_05[0].gameObject.SetActive(show);
                    End_DefLabel_06[0].gameObject.SetActive(show);
                    break;
                case 3:
                    End_AttackLabel_07[0].gameObject.SetActive(show);
                    End_HpLabel_08[0].gameObject.SetActive(show);
                    End_DefLabel_09[0].gameObject.SetActive(show);
                    break;
            }
        }
        
    }
}