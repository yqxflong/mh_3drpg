using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using _HotfixScripts.Utils;
using EB;
using Debug = UnityEngine.Debug;
    
namespace Hotfix_LT.UI
{
    public enum SkillType
    {
        Common,
        Passive,
        Active,
    }

    public struct LTPartnerSkillBreskData
    {
        public SkillType skillType;
        public LTPartnerData partnerData;
    }

    public class LTPartnerSkillBreakController : UIControllerHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            CommonSkillIcon = t.GetComponent<DynamicUISprite>("SkillList/CommonSkill/Icon");
            CommonSelect = t.GetComponent<UISprite>("SkillList/CommonSkill/Select");
            CommonFrame = t.GetComponent<UISprite>("SkillList/CommonSkill");
            CommonSkillLevel = t.GetComponent<UILabel>("SkillList/CommonSkill/Level/Text");
            CommonSkillTypeLab = t.GetComponent<UILabel>("SkillList/CommonSkill/Label");
            CommonLevelUpFx = t.GetComponent<ParticleSystemUIComponent>("SkillList/CommonSkill/Fx");
            ActiveSkillIcon = t.GetComponent<DynamicUISprite>("SkillList/ActiveSkill/Icon");
            ActiveSelect = t.GetComponent<UISprite>("SkillList/ActiveSkill/Select");
            ActiveFrame = t.GetComponent<UISprite>("SkillList/ActiveSkill");
            ActiveSkillLevel = t.GetComponent<UILabel>("SkillList/ActiveSkill/Level/Text");
            ActiveSkillTypeLab = t.GetComponent<UILabel>("SkillList/ActiveSkill/Label");
            ActiveLevelUpFx = t.GetComponent<ParticleSystemUIComponent>("SkillList/ActiveSkill/Fx");
            PassiveSkillIcon = t.GetComponent<DynamicUISprite>("SkillList/PassiveSkill/Icon");
            PassiveSelect = t.GetComponent<UISprite>("SkillList/PassiveSkill/Select");
            PassiveFrame = t.GetComponent<UISprite>("SkillList/PassiveSkill");
            PassiveSkillLevel = t.GetComponent<UILabel>("SkillList/PassiveSkill/Level/Text");
            PassiveSkillTypeLab = t.GetComponent<UILabel>("SkillList/PassiveSkill/Label");
            PassiveLevelUpFx = t.GetComponent<ParticleSystemUIComponent>("SkillList/PassiveSkill/Fx");
            CurSelectSkillIcon = t.GetComponent<DynamicUISprite>("SkillInfo/Icon/Sprite");
            CurSelectSkillLevel = t.GetComponent<UILabel>("SkillInfo/Level/Text");
            NameLabel = t.GetComponent<UILabel>("SkillInfo/NameLabel");
            DescLabel = t.GetComponent<UILabel>("SkillInfo/SkillDesc/ScrollView/DescLabel");
            DescAdditionalLabel = t.GetComponent<UILabel>("SkillInfo/SkillDesc/ScrollView/DescAdditional");
            LevelLabel = t.GetComponent<UILabel>("SkillInfo/ProgressBar/LevelLab/LevelLabel");
            NextLevelLabel = t.GetComponent<UILabel>("SkillInfo/ProgressBar/LevelLab/NextLevel/NextLevelLabel");
            BarValueLabel = t.GetComponent<UILabel>("SkillInfo/ProgressBar/ProgressBarValue");
            NextLevelObj = t.FindEx("SkillInfo/ProgressBar/LevelLab/NextLevel").gameObject;
            CurExpSlider = t.GetComponent<UISlider>("SkillInfo/ProgressBar/Cur");
            NextExpSlider = t.GetComponent<UISlider>("SkillInfo/ProgressBar/Next");
            ExpSliderAni = t.GetMonoILRComponent<SliderAni>("SkillInfo/ProgressBar/Cur");
            ExpNumAni = t.GetMonoILRComponent<NumAni>("SkillInfo/ProgressBar/ProgressBarValue");
            LvUpLabelTweenTf = t.GetComponent<Transform>("SkillInfo/LvUpTextAni");
            SkillDescScroll = t.GetComponent<UIScrollView>("SkillInfo/SkillDesc/ScrollView");
            BreakBtn = t.GetComponent<UIButton>("BreakBtn");
            Scroll = t.GetMonoILRComponent<LTPartnerSkillBreakTableScroll>("ItemList/Container/Placeholder/Grid");
            isOpenLimit = false;
            limitValue = 0.02f;
            t.GetComponent<UIButton>("CancelBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));

            t.GetComponent<UIButton>("BreakBtnOneLevel").onClick.Add(new EventDelegate(OnSkillBreakOnLevel));
            t.GetComponent<UIButton>("BreakBtn").onClick.Add(new EventDelegate(OnSkillBreakClick));

            t.GetComponent<ConsecutiveClickCoolTrigger>("SkillList/CommonSkill").clickEvent.Add(new EventDelegate(OnCommonSkillBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("SkillList/ActiveSkill").clickEvent.Add(new EventDelegate(OnActiveSkillBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("SkillList/PassiveSkill").clickEvent.Add(new EventDelegate(OnPassiveSkillBtnClick));

            Messenger.AddListener<string,int,int>(Hotfix_LT.EventName.OnPartnerSkillBreakGoodsAddSucc,OnGoodsAddSucc);
            Messenger.AddListener<string,int>(Hotfix_LT.EventName.OnPartnerSkillBreakGoodsRemoveSucc,OnGoodsRemoveSucc);
        }

        public DynamicUISprite CommonSkillIcon;
        public UISprite CommonSelect,CommonFrame;
        public UILabel CommonSkillLevel;
        public UILabel CommonSkillTypeLab;
        public ParticleSystemUIComponent CommonLevelUpFx;
    
        public DynamicUISprite ActiveSkillIcon;
        public UISprite ActiveSelect,ActiveFrame;
        public UILabel ActiveSkillLevel;
        public UILabel ActiveSkillTypeLab;
        public ParticleSystemUIComponent ActiveLevelUpFx;
    
        public DynamicUISprite PassiveSkillIcon;
        public UISprite PassiveSelect,PassiveFrame;
        public UILabel PassiveSkillLevel;
        public UILabel PassiveSkillTypeLab;
        public ParticleSystemUIComponent PassiveLevelUpFx;
    
        public DynamicUISprite CurSelectSkillIcon;
        public UILabel CurSelectSkillLevel;
        public UILabel NameLabel;
        public UILabel DescLabel;
        public UILabel DescAdditionalLabel;
        public UILabel LevelLabel;
        public UILabel NextLevelLabel;
        public UILabel BarValueLabel;
        public GameObject NextLevelObj;
        public UISlider CurExpSlider;
        public UISlider NextExpSlider;
        public SliderAni ExpSliderAni;
        public NumAni ExpNumAni;
        public Transform LvUpLabelTweenTf;
        public UIScrollView SkillDescScroll;
        public UIButton BreakBtn;
        
        public LTPartnerSkillBreakTableScroll Scroll;
        private List<UIInventoryBagCellData> bagCellDataList;
        public bool isOpenLimit = false;
        public float limitValue = 0.02f;
        private int maxSkillLevel = 15;
        private int curSkillExp;
        private int addSkillExp;
        private int nextSkillExp;
        private int skillLevel;
        private bool isMaxSkillLevel;
        private Hotfix_LT.Data.HeroAwakeInfoTemplate curAwakenTemplate;
        private bool isPlayingAni;
        public bool IsPlayingAni { set { isPlayingAni = value; } get { return isPlayingAni; } }
    
        private LTPartnerData curPartnerData;
        private SkillType tempSelect = SkillType.Active;
        private SkillType curSelect = SkillType.Active;
        public SkillType CurSelect
        {
            get
            {
                return curSelect;
            }
            set
            {
                curSelect = value;
                UpdateSelect();
            }
        }
    
        private int curSelectSkillId = 0;
    
        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Messenger.RemoveListener<string,int,int>(Hotfix_LT.EventName.OnPartnerSkillBreakGoodsAddSucc,OnGoodsAddSucc);
            Messenger.RemoveListener<string,int>(Hotfix_LT.EventName.OnPartnerSkillBreakGoodsRemoveSucc,OnGoodsRemoveSucc);
        }
    
        public override void SetMenuData(object param)
        {
            LTPartnerSkillBreskData tempData = (LTPartnerSkillBreskData)param;
            tempSelect = tempData.skillType;
            curPartnerData = tempData.partnerData;
            curAwakenTemplate = Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(curPartnerData.InfoId);
        }
    
        public override IEnumerator OnAddToStack()
        {
            var coroutine = EB.Coroutines.Run(base.OnAddToStack());
            CurSelect = tempSelect;
            yield return coroutine;
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            ClearData();
            DestroySelf();
            yield break;
        }
    
       // public override void ClearData()
       public void ClearData()
        {
            LvUpLabelTweenTf.gameObject.CustomSetActive(false);
            ActiveLevelUpFx.gameObject.CustomSetActive(false);
            CommonLevelUpFx.gameObject.CustomSetActive(false);
            PassiveLevelUpFx.gameObject.CustomSetActive(false);
        }
    
        private void UpdateSelect()
        {
            CommonSelect.gameObject.SetActive(CurSelect == SkillType.Common);
            PassiveSelect.gameObject.SetActive(CurSelect == SkillType.Passive);
            ActiveSelect.gameObject.SetActive(CurSelect == SkillType.Active);
            UpdateUI();
        }
    
        private void UpdateUI()
        {
            if (curPartnerData == null)
            {
                return;
            }
            
            SkillSetTool.SkillFrameStateSet(CommonFrame, false);
            SkillSetTool.SkillFrameStateSet(PassiveFrame, false);
            SkillSetTool.SkillFrameStateSet(ActiveFrame, false);

            Data.SkillTemplate skill1 = Data.SkillTemplateManager.Instance.GetTemplateWithAwake(curPartnerData,
                curPartnerData.HeroStat.common_skill,
                () => { SkillSetTool.SkillFrameStateSet(CommonFrame, true); });
            Data.SkillTemplate skill2 = Data.SkillTemplateManager.Instance.GetTemplateWithAwake(curPartnerData,
                curPartnerData.HeroStat.active_skill,
                () => { SkillSetTool.SkillFrameStateSet(ActiveFrame, true); });
            Data.SkillTemplate skill3 = Data.SkillTemplateManager.Instance.GetTemplateWithAwake(curPartnerData,
                curPartnerData.HeroStat.passive_skill,
                () => { SkillSetTool.SkillFrameStateSet(PassiveFrame, true); });
            maxSkillLevel = LTPartnerConfig.MAX_SKILL_LEVEL;
            
            if (skill1 != null)
            {
                CommonSkillIcon.transform.parent.gameObject.CustomSetActive(true);
                CommonSkillIcon.spriteName = skill1.Icon;
                CommonSkillTypeLab.text = GetSkillTypeStr(skill1.Type);
            }
            else
            {
                CommonSkillIcon.transform.parent.gameObject.CustomSetActive(false);
            }
           
            if (skill2 != null)
            {
                ActiveSkillIcon.transform.parent.gameObject.CustomSetActive(true);
                ActiveSkillIcon.spriteName = skill2.Icon;
                ActiveSkillTypeLab.text = GetSkillTypeStr(skill2.Type);
            }
            else
            {
                ActiveSkillIcon.transform.parent.gameObject.CustomSetActive(false);
            }        
            if (skill3 != null)
            {
                PassiveSkillTypeLab.transform.parent.gameObject.CustomSetActive(true);
                PassiveSkillIcon.spriteName = skill3.Icon;
                PassiveSkillTypeLab.text = GetSkillTypeStr(skill3.Type);
            }
            else
            {
                PassiveSkillTypeLab.transform.parent.gameObject.CustomSetActive(false);
            }
    
            ActiveSkillLevel.text = curPartnerData.ActiveSkillLevel.ToString();
            CommonSkillLevel.text = curPartnerData.CommonSkillLevel.ToString();
            PassiveSkillLevel.text = curPartnerData.PassiveSkillLevel.ToString();
    
            curSkillExp = LTPartnerDataManager.Instance.GetPartnerCurSkillLevelExp(curPartnerData.StatId, CurSelect);
            
            addSkillExp = 0;
            LTPartnerDataManager.Instance.ClearSkillBreakDelGoodsDic();
            if (CurSelect == SkillType.Active)
            {
                skillLevel = curPartnerData.ActiveSkillLevel;
                curSelectSkillId = curPartnerData.HeroStat.active_skill;
                nextSkillExp = LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[curPartnerData.ActiveSkillLevel];
            }
            else if (CurSelect == SkillType.Common)
            {
                skillLevel = curPartnerData.CommonSkillLevel;
                curSelectSkillId = curPartnerData.HeroStat.common_skill;
                nextSkillExp = LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[curPartnerData.CommonSkillLevel];
            }
            else if (CurSelect == SkillType.Passive)
            {
                skillLevel = curPartnerData.PassiveSkillLevel;
                curSelectSkillId = curPartnerData.HeroStat.passive_skill;
                nextSkillExp = LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[curPartnerData.PassiveSkillLevel];
            }
    
            LevelLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_7019"), skillLevel);
            CurSelectSkillLevel.text = skillLevel.ToString();
    
            ShowGoodsList();
            ShowSkillBar();
    
            if (curSelectSkillId <= 0)
            {
                return;
            }
            SetSkillDes(skillLevel);
            //Hotfix_LT.Data.SkillTemplate tpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(curSelectSkillId);
            //if (tpl != null)
            //{
            //    //CurSelectSkillIcon.spriteName = tpl.Icon;
            //    //NameLabel.text = tpl.Name;
            //    DescLabel.text = GetContext(tpl, skillLevel);
            //}
            //LTUIUtil.SetText(DescAdditionalLabel, GetSkillAdditional(curSelectSkillId, skillLevel));
    
            Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(SkillDescScroll.transform);
            bool isCouldRoll = bounds.size.y + 40 > SkillDescScroll.panel.GetViewSize().y;
            SkillDescScroll.enabled = isCouldRoll;
    
            /*List<LTPartnerData> list = new List<LTPartnerData>();
            for (int i = 0; i < 13; i++)
            {
                LTPartnerData a = new LTPartnerData();
                list.Add(a);
            }
    
            Scroll.SetItemDatas(list);*/
        }
    
        public void ShowGoodsList()
        {
            IDictionary itemDataCollection;
    
            DataLookupsCache.Instance.SearchDataByID("inventory", out itemDataCollection);
    
            List<string> goodsIdList = new List<string>();
            foreach (var goodsId in itemDataCollection.Keys)
            {
                IDictionary dic = itemDataCollection[goodsId] as IDictionary;
                string ss = EB.Dot.String("location", dic, "");
                string typeStr = EB.Dot.String("system", dic, "");
                if (ss == "bag_items" && typeStr != "Equipment")
                {
                    int EconomyId = EB.Dot.Integer("economy_id", dic, 0);
                    Hotfix_LT.Data.GeneralItemTemplate itemData = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(EconomyId) as Hotfix_LT.Data.GeneralItemTemplate;
    
                    if (itemData != null && itemData.Exp > 0)
                    {
                        goodsIdList.Add(goodsId.ToString());
                    }
                }
            }
            goodsIdList.Sort(0, goodsIdList.Count, new LTSkillBreakGoodsComparer());
            bagCellDataList = new List<UIInventoryBagCellData>();
            for (int i = 0; i < goodsIdList.Count; i++)
            {
                UIInventoryBagCellData bagCellData = new UIInventoryBagCellData(goodsIdList[i]);
                bagCellDataList.Add(bagCellData);
            }
    
            if (bagCellDataList.Count < 25)
            {
                for (int i = bagCellDataList.Count; i < 25; i++)
                {
                    bagCellDataList.Add(null);
                }
            }
            
            Scroll.SetItemDatas(bagCellDataList);
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public void Update()
        {
            if (LTPartnerDataManager.Instance.GetSkillBreakDelGoodsDic()!=null)
            {
                if (BreakBtn.isEnabled !=LTPartnerDataManager.Instance.GetSkillBreakDelGoodsDic().Count > 0)
                {
                    BreakBtn.isEnabled = (LTPartnerDataManager.Instance.GetSkillBreakDelGoodsDic().Count > 0);
                }
            }
        }
    
        public void OnSkillBreakOnLevel()
        {
            if (IsMaxSkillLevel() || IsPlayingAni)
            {
                if (IsMaxSkillLevel())
                {
                   LTPartnerDataManager.Instance.ShowPartnerMessage(EB.Localizer.GetString("ID_codefont_in_LTPartnerDataManager_18538"));
                }
                return;
            }
    
            string[] strs= BarValueLabel.text.Split('/');
            if (strs!=null && strs.Length==2)
            {
                GetOneLevelGood(int.Parse(strs[1])-int.Parse(strs[0]));
            }
        }
    
        private void GetOneLevelGood(int offset)
        {
            //从后往前遍历
            for (int i = bagCellDataList.Count-1; i >=0 ; i--)
            {
                if (bagCellDataList[i]!=null)
                {
                    int exp = 0;
                    int goodsLeftCount = 0; 
                    LTPartnerDataManager.Instance.GetExpByGoodsId(bagCellDataList[i].m_DataID, ref exp, ref goodsLeftCount);
                    if (offset >= exp * goodsLeftCount)
                    {
                        BroadCastItemClick(goodsLeftCount,bagCellDataList[i].m_DataID);
                        offset = offset - exp * goodsLeftCount;
                    }
                    //如果当前项够升一级则直接跳出
                    else
                    {
                        int num = ((offset - 1) / exp) + 1;
                        BroadCastItemClick(num,bagCellDataList[i].m_DataID);
                        return;
                    }
                }
            }
        }
    
        private void BroadCastItemClick(int num,string goodsId)
        {
            Messenger.Raise<string,int>(Hotfix_LT.EventName.OnPartnerSkillBreakItemClick,goodsId,num);
        }
    
        public void ShowSkillBar()
        {
            //注：maxSkillLevel是指当前星级下所能达到的技能等级上限，LTPartnerConfig.MAX_SKILL_LEVEL是指该技能最高可达到的等级上限
            int skillExp = curSkillExp + addSkillExp;
            CurExpSlider.value = (float)curSkillExp / nextSkillExp;
    
            isMaxSkillLevel = skillLevel >= maxSkillLevel;
    
            if (skillLevel >= LTPartnerConfig.MAX_SKILL_LEVEL)
            {
                isMaxSkillLevel = true;
                CurExpSlider.value = 1;
                NextExpSlider.value = 0;
                LTUIUtil.SetText(BarValueLabel, "Max");
                NextLevelObj.SetActive(false);
                return;
            }
            //觉醒技能描述特殊处理
            int curSkillId;
            if (curPartnerData.IsAwaken > 0 && curAwakenTemplate != null && curAwakenTemplate.beforeSkill == curSelectSkillId)
            {
                curSkillId = curAwakenTemplate.laterSkill;
            }
            else
            {
                curSkillId = curSelectSkillId;
            }
            //
            if (skillExp >= nextSkillExp)
            {
                int addLevel = 1;
                int addExp = 0;
                int tempAdd = skillExp;
                for (int i = skillLevel; i < maxSkillLevel + 1; i++)
                {
                    tempAdd -= LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[skillLevel + addLevel - 1];
                    addExp += LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[skillLevel + addLevel - 1];
                    if (tempAdd >= LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[skillLevel + addLevel])
                    {
                        addLevel += 1;
                    }
                    else
                    {
                        break;
                    }
                }
    
                CurExpSlider.value = 0;
                int addSkillLevel = skillLevel + addLevel;           
                if (addSkillLevel >= LTPartnerConfig.MAX_SKILL_LEVEL)
                {
                    NextLevelLabel.text = LTPartnerConfig.MAX_SKILL_LEVEL.ToString();
                    LTUIUtil.SetText(DescAdditionalLabel, GetSkillAdditional(curSkillId, LTPartnerConfig.MAX_SKILL_LEVEL));
                    NextExpSlider.value = 1;
                    LTUIUtil.SetText(BarValueLabel, "Max");
                    isMaxSkillLevel = true;
                    if (skillLevel >= LTPartnerConfig.MAX_SKILL_LEVEL - 1)
                    {
                        CurExpSlider.value = (float)curSkillExp / nextSkillExp;
                    }
                }
                else
                {
                    if (addSkillLevel >= maxSkillLevel)
                    {
                        isMaxSkillLevel = true;
                    }
    
                    NextLevelLabel.text = addSkillLevel.ToString();
                    LTUIUtil.SetText(DescAdditionalLabel, GetSkillAdditional(curSkillId, addSkillLevel));
                    NextExpSlider.value = (float)tempAdd / LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[addSkillLevel];
                    LTUIUtil.SetText(BarValueLabel, string.Format("{0}/{1}", tempAdd, LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[addSkillLevel]));
                }
            }
            else
            {
                CurExpSlider.value = (float)curSkillExp / nextSkillExp;
                NextExpSlider.value = (float)skillExp / nextSkillExp;
                LTUIUtil.SetText(BarValueLabel, string.Format("{0}/{1}", skillExp, nextSkillExp));
                LTUIUtil.SetText(DescAdditionalLabel, GetSkillAdditional(curSkillId, skillLevel));
            }
            NextLevelObj.SetActive(skillExp >= nextSkillExp);
        }
    
        private string GetContext(Hotfix_LT.Data.SkillTemplate skillData, int skillLevel)
        {
            if (skillData == null) return "";
    
            if (!string.IsNullOrEmpty(skillData.Description))
            {
                return SkillDescTransverter.ChangeDescription(skillData.Description, skillLevel, skillData.ID);
    
            }
            return "";
        }
    
        private string GetSkillTypeStr(Hotfix_LT.Data.eSkillType skillType)
        {
            string str = "";
            if (skillType == Hotfix_LT.Data.eSkillType.ACTIVE)
            {
                str = EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_13041");
            }
            else if (skillType == Hotfix_LT.Data.eSkillType.NORMAL)
            {
                str = EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_13140");
            }
            else if (skillType == Hotfix_LT.Data.eSkillType.PASSIVE)
            {
                str = EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_13212");
            }
    
            return str;
        }
    
        //技能描述
        public void RefreshSkillLabel(int skillLv)
        {
            FusionAudio.PostEvent("UI/Partner/SkillBreakUp", true);
            LevelLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_7019"), skillLv);
            CurSelectSkillLevel.text = skillLv.ToString();
            if (CurSelect == SkillType.Active)
            {
                ActiveSkillLevel.text = skillLv.ToString();
                ActiveLevelUpFx.gameObject.CustomSetActive(true);
                ActiveLevelUpFx.Play();
            }
            else if (CurSelect == SkillType.Common)
            {
                CommonSkillLevel.text = skillLv.ToString();
                CommonLevelUpFx.gameObject.CustomSetActive(true);
                CommonLevelUpFx.Play();
            }
            else if (CurSelect == SkillType.Passive)
            {
                PassiveSkillLevel.text = skillLv.ToString();
                PassiveLevelUpFx.gameObject.CustomSetActive(true);
                PassiveLevelUpFx.Play();
            }
            SetSkillDes(skillLv);
            LvUpLabelTweenTf.gameObject.CustomSetActive(true);
            UITweener[] twe = LvUpLabelTweenTf.GetComponentsInChildren<UITweener>();
            for (int i = 0; i < twe.Length; i++)
            {
                twe[i].ResetToBeginning();
                twe[i].PlayForward();
            }
        }
    
        //技能描述设置
        private void SetSkillDes(int skillLv)
        {
            Hotfix_LT.Data.SkillTemplate tpl;
            if (curPartnerData.IsAwaken > 0 && curAwakenTemplate != null && curAwakenTemplate.beforeSkill == curSelectSkillId)
            {
                tpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(curAwakenTemplate.laterSkill);
            }
            else
            {
                tpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(curSelectSkillId);
            }
    
            if (tpl != null)
            {
                DescLabel.text = GetContext(tpl, skillLv);
                LTUIUtil.SetText(DescAdditionalLabel, GetSkillAdditional(tpl.ID, skillLv));
            }
            
        }
    
        public void SetStateMax()
        {
            NextExpSlider.value = 0;
            CurExpSlider.value = 1;
            LTUIUtil.SetText(BarValueLabel, "Max");
            isMaxSkillLevel = true;
        }
    
        public bool IsMaxSkillLevel()
        {
            return isMaxSkillLevel;
        }
    
        public bool IsMaxStar()
        {
            return curPartnerData.Star >= LTPartnerConfig.MAX_STAR;
        }
    
        private void AfterSkillUp()
        {
            NextLevelObj.SetActive(false);
            StartCoroutine(IEShowGoodsList());
            NextExpSlider.value = 0;
            addSkillExp = 0;
            nextSkillExp = LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[skillLevel];
            isMaxSkillLevel = skillLevel >= maxSkillLevel;
            LTPartnerDataManager.Instance.ClearSkillBreakDelGoodsDic();
        }
    
        private IEnumerator IEShowGoodsList()
        {
            // 容错，dataLookupsCache写入的时候有时会有延迟，此时如果去查找数据的时候会有问题，等待0.1s再查找
            yield return new WaitForSeconds(0.1f);
            ShowGoodsList();
        }
    
        public void OnActiveSkillBtnClick()
        {
            if (curPartnerData.HeroStat.active_skill <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_37369"));
                return;
            }
    
            if (CurSelect != SkillType.Active)
            {
                if (IsPlayingAni)
                {
                    LTPartnerSkillBreakAniManager.Instance.StopAni();
                }
                CurSelect = SkillType.Active;
            }
        }
    
        public void OnCommonSkillBtnClick()
        {
            if (curPartnerData.HeroStat.common_skill <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_37369"));
                return;
            }
    
            if (CurSelect != SkillType.Common)
            {
                if (IsPlayingAni)
                {
                    LTPartnerSkillBreakAniManager.Instance.StopAni();
                }
                CurSelect = SkillType.Common;
            }
        }
    
        public void OnPassiveSkillBtnClick()
        {
            if (curPartnerData.HeroStat.passive_skill <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_37369"));
                return;
            }
    
            if (CurSelect != SkillType.Passive)
            {
                if (IsPlayingAni)
                {
                    LTPartnerSkillBreakAniManager.Instance.StopAni();
                }
                CurSelect = SkillType.Passive;
            }
        }
    
        public void AddOrDelExp(int economyId, bool isAdd,int times)
        {
            int exp = 0;
            Hotfix_LT.Data.GeneralItemTemplate generalItemData = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(economyId) as Hotfix_LT.Data.GeneralItemTemplate;
            if (generalItemData == null)
            {
                Hotfix_LT.Data.EquipmentItemTemplate equipmentItemData = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(economyId) as Hotfix_LT.Data.EquipmentItemTemplate;
                exp = equipmentItemData.BaseExp;
            }
            else
            {
                exp = generalItemData.Exp;
            }
            if (isAdd)
            {
                addSkillExp += exp*times;
            }
            else
            {
                addSkillExp -= exp*times;
            }
        }
    
        public void OnSkillBreakClick()
        {
            if (IsPlayingAni)
            {
                return;
            }
    
            Dictionary<string, int> delGoodsDic = LTPartnerDataManager.Instance.GetSkillBreakDelGoodsDic();
            if (curSelectSkillId > 0)
            {
                ArrayList tempGoodsList = Johny.ArrayListPool.Claim();
                foreach (string goodsId in delGoodsDic.Keys)
                {
                    //IDictionary dic = new Dictionary<object , object> { { "inventoryId", goodsId }, { "num", delGoodsDic[goodsId] } };
                    Hashtable table = Johny.HashtablePool.Claim();
                    table.Add("inventoryId", goodsId);
                    table.Add("num", delGoodsDic[goodsId]);
                    tempGoodsList.Add(table);
                }
                if (tempGoodsList.Count <= 0)
                {
                    if (isMaxSkillLevel)
                    {
                        LTPartnerDataManager.Instance.ShowPartnerMessage(EB.Localizer.GetString("ID_codefont_in_LTPartnerDataManager_18538"));
                    }
                    else
                    {
                        LTPartnerDataManager.Instance.ShowPartnerMessage(EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_19195"));
                    }
                    return;
                }
    
                LTPartnerSkillBreakAniData aniData = new LTPartnerSkillBreakAniData(this, ExpSliderAni, ExpNumAni);
                aniData.oldExp = curSkillExp;
                aniData.oldLv = skillLevel;
                LTPartnerDataManager.Instance.SkillBreak(curPartnerData.HeroId, curSelectSkillId, tempGoodsList, delegate
                   {
                       //UpdateUI();
                       aniData.curExp = curSkillExp = LTPartnerDataManager.Instance.GetPartnerCurSkillLevelExp(curPartnerData.StatId, CurSelect);
    
                       aniData.curLv = skillLevel = CurSelect == SkillType.Common ? 
                       curPartnerData.CommonSkillLevel : CurSelect == SkillType.Active ?
                       curPartnerData.ActiveSkillLevel : curPartnerData.PassiveSkillLevel;
    
                       LTPartnerSkillBreakAniManager.Instance.SetAniData(aniData);
    
                       AfterSkillUp();
                       Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnParnerSkillChange);
                   });
            }
        }
    
        private void OnGoodsAddSucc(string goodId,int economyId,int times)
        {
            LTPartnerDataManager.Instance.AddSkillBreakDelGoods(goodId, times);
            AddOrDelExp(economyId, true,times);
            ShowSkillBar();
        }
    
        private void OnGoodsRemoveSucc(string goodsId, int economyId)
        {
            LTPartnerDataManager.Instance.RemoveSkillBreakDelGoods(goodsId, 1);
            AddOrDelExp(economyId, false,1);
            ShowSkillBar();
        }
    
        public static string GetSkillAdditional(int skillId, int skillLv)
        {
            StringBuilder strDesc = new StringBuilder();
    
            Hotfix_LT.Data.SkillLevelUpTemplate skillData = Hotfix_LT.Data.SkillTemplateManager.Instance.GetSkillLevelUpTemplate(skillId);
            if (skillData != null)
            {
                int param =  skillLv - 1;
                int maxLv = LTPartnerConfig.MAX_SKILL_LEVEL-1;
                string maxValue;
    
                if (skillData.DamageIncPercent != 0)
                {
                    if (param == maxLv)
                        maxValue = string.Empty;
                    else
                        maxValue = string.Format(EB.Localizer.GetString("ID_SKILL_MAX_LEVEL_TIP"), "+" + skillData.DamageIncPercent * maxLv * 100 + "%");
                    strDesc.Append(string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_21922"),
                          string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, "+" + (skillData.DamageIncPercent * param * 100) + "%" + maxValue)));
                }
    
                if (skillData.Rating != 0)
                {
                    if (param == maxLv)
                        maxValue = string.Empty;
                    else
                        maxValue = string.Format(EB.Localizer.GetString("ID_SKILL_MAX_LEVEL_TIP"), "+" + skillData.Rating * maxLv * 100 + "%");
                    strDesc.Append(string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_21723"),
                        string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, "+" + (skillData.Rating * param * 100) + "%" + maxValue)));
                }
                
                if (skillData.CDRating != 0)
                {
                    if (param == maxLv)
                        maxValue = string.Empty;
                    else
                        maxValue = string.Format(EB.Localizer.GetString("ID_SKILL_MAX_LEVEL_TIP"), skillData.CDRating * maxLv * 100 + "%");
                    strDesc.Append(string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_21526"),
                        string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, skillData.CDRating * param * 100+"%"),
                        skillData.CDCount,
                        string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, maxValue)));
                }
    
                if (skillData.MaxHpIncPercent != 0)
                {
                    if (param == maxLv)
                        maxValue = string.Empty;
                    else
                        maxValue = string.Format(EB.Localizer.GetString("ID_SKILL_MAX_LEVEL_TIP"), "+" + skillData.MaxHpIncPercent * maxLv * 100 + "%");
                    strDesc.Append(string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_22130"),
                        string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, "+" + skillData.MaxHpIncPercent * param * 100 + "%" + maxValue))); 
                }
    
                if (skillData.AtkIncPercent != 0)
                {
                    if (param == maxLv)
                        maxValue = string.Empty;
                    else
                        maxValue = string.Format(EB.Localizer.GetString("ID_SKILL_MAX_LEVEL_TIP"), "+" + skillData.AtkIncPercent * maxLv * 100 + "%");
                    strDesc.Append(string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_22311"),
                        string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, "+" + skillData.AtkIncPercent * param * 100 + "%" + maxValue)));
                }
    
                if (skillData.DefIncPercnet != 0)
                {
                    if (param == maxLv)
                        maxValue = string.Empty;
                    else
                        maxValue = string.Format(EB.Localizer.GetString("ID_SKILL_MAX_LEVEL_TIP"), "+" + skillData.DefIncPercnet * maxLv * 100 + "%");
                    strDesc.Append(string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerSkillBreakController_22490"),
                        string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, "+" + skillData.DefIncPercnet * param * 100 + "%" + maxValue)));
                }
    
                string str = strDesc.ToString();
                if (str.Substring(strDesc.Length - 1, 1) == "\n")
                {
                    str = str.Substring(0, strDesc.Length - 1);
                    return str;
                }
            }
    
            return strDesc.ToString();
        }
    }
}
