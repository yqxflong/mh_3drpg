using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public enum LTParGradeStarUp
    {
        GradeUp,
        StarUp,
        MaxLevel,
    }
    
    public class ShowGradeStarUp
    {
        public LTParGradeStarUp ShowType;
        public LTPartnerData PartnerData;
        public LTAttributesData oldAttrData;
        public ShowGradeStarUp(LTParGradeStarUp ShowType,LTPartnerData PartnerData,LTAttributesData oldAttrData)
        {
            this.ShowType = ShowType;
            this.PartnerData = PartnerData;
            this.oldAttrData = oldAttrData;
        }
    }
    public class LTPartnerGradeStarUp : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            LifeLabel = t.GetComponent<UILabel>("Panel/Content/Attribute/Life/NumLabel");
            AtkLabel = t.GetComponent<UILabel>("Panel/Content/Attribute/Attack/NumLabel");
            CritLabel = t.GetComponent<UILabel>("Panel/Content/Attribute/Crit/NumLabel");
            DefLabel = t.GetComponent<UILabel>("Panel/Content/Attribute/Def/NumLabel");
            GradeUpLimitLv = t.GetComponent<UILabel>("Panel/Content/GradeUpLimitLvObj/GradeUpLimitLv");
            GradeUpLimitLvObj = t.FindEx("Panel/Content/GradeUpLimitLvObj").gameObject;
            showPanel = t.GetComponent<UIPanel>("Panel");
            CurItem = t.GetMonoILRComponent<LTPartnerListCellController>("Panel/Content/CurItem");
            UpItem = t.GetMonoILRComponent<LTPartnerListCellController>("Panel/Content/UpItem");
            IsAwakening = false;
            TitleLabel = t.GetComponent<UILabel>("Panel/Title");
            CurItemTP = t.GetComponent<TweenPosition>("Panel/Content/CurItem");
            UpItemTP = t.GetComponent<TweenPosition>("Panel/Content/UpItem");
            LifeLabelTS = t.GetComponent<TweenScale>("Panel/Content/Attribute/Life");
            AtkLabelTS = t.GetComponent<TweenScale>("Panel/Content/Attribute/Attack");
            CritLabelTS = t.GetComponent<TweenScale>("Panel/Content/Attribute/Crit");
            DefLabelTS = t.GetComponent<TweenScale>("Panel/Content/Attribute/Def");
            ContinueLabel = t.FindEx("Panel/Content/Tip").gameObject;
            Arrow = t.FindEx("Panel/Content/SpArrow").gameObject;
            ProficiencyTP = t.GetComponent<TweenPosition>("Panel/Content/ProficiencyView");

            ProficiencyIconList = new List<UISprite>();
            ProficiencyIconList.Add(t.GetComponent<UISprite>("Panel/Content/ProficiencyView/AllRoundItem"));
            ProficiencyIconList.Add(t.GetComponent<UISprite>("Panel/Content/ProficiencyView/AllRoundItem (1)"));
            ProficiencyIconList.Add(t.GetComponent<UISprite>("Panel/Content/ProficiencyView/AllRoundItem (2)"));
            ProficiencyIconList.Add(t.GetComponent<UISprite>("Panel/Content/ProficiencyView/AllRoundItem (3)"));
            ProficiencyIconList.Add(t.GetComponent<UISprite>("Panel/Content/ProficiencyView/AllRoundItem (4)"));


            ProficiencyNameLabel = new List<UILabel>();
            ProficiencyNameLabel.Add(t.GetComponent<UILabel>("Panel/Content/ProficiencyView/AllRoundItem/Label"));
            ProficiencyNameLabel.Add(t.GetComponent<UILabel>("Panel/Content/ProficiencyView/AllRoundItem (1)/Label"));
            ProficiencyNameLabel.Add(t.GetComponent<UILabel>("Panel/Content/ProficiencyView/AllRoundItem (2)/Label"));
            ProficiencyNameLabel.Add(t.GetComponent<UILabel>("Panel/Content/ProficiencyView/AllRoundItem (3)/Label"));
            ProficiencyNameLabel.Add(t.GetComponent<UILabel>("Panel/Content/ProficiencyView/AllRoundItem (4)/Label"));

            LevelUpMaxTP = t.GetComponent<TweenPosition>("Panel/Content/TopLevelAttrView");
            LevelUpMaxIcon = t.GetComponent<UISprite>("Panel/Content/TopLevelAttrView/Sprite");
            LevelUpMaxName = t.GetComponent<UILabel>("Panel/Content/TopLevelAttrView/NameLabel");
            LevelUpMaxDesc = t.GetComponent<UILabel>("Panel/Content/TopLevelAttrView/DesLabel");

            Time = new float[7];
            Time[0] = 0.2f;
            Time[1] = 0.2f;
            Time[2] = 0.3f;
            Time[3] = 0.2f;
            Time[4] = 0.2f;
            Time[5] = 0.2f;
            Time[6] = 0.2f;

            UIButton backButton = t.GetComponent<UIButton>("Bg");
            backButton.onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButton>("Panel/FormationBtn").onClick.Add(new EventDelegate(PlayAniTest));

        }


    
        //ID_OPEN_PROFICIENCY
        public UILabel LifeLabel;
        public UILabel AtkLabel;
        public UILabel CritLabel;
        public UILabel DefLabel;
        public UILabel GradeUpLimitLv;
        public GameObject GradeUpLimitLvObj;
        public UIPanel showPanel;
    
        public LTPartnerListCellController CurItem;
        public LTPartnerListCellController UpItem;
    
        private LTParGradeStarUp curType;
        private LTPartnerData curParData;
        private LTAttributesData oldAttrData;
        public bool IsAwakening;
    
        private bool isShowOpenProficiency;
        #region 动画参数
        public UILabel TitleLabel;
        public TweenPosition CurItemTP;
        public TweenPosition UpItemTP;
        public TweenScale LifeLabelTS;
        public TweenScale AtkLabelTS;
        public TweenScale CritLabelTS;
        public TweenScale DefLabelTS;
        public GameObject ContinueLabel;
        public GameObject Arrow;
    
        public TweenPosition ProficiencyTP;
        public List<UISprite> ProficiencyIconList;
        public List<UILabel> ProficiencyNameLabel;
    
        public TweenPosition LevelUpMaxTP;
        public UISprite LevelUpMaxIcon;
        public UILabel LevelUpMaxName;
        public UILabel LevelUpMaxDesc;
    
        public float[] Time;
        private bool playOver = false;
        #endregion
    
    
        public override void SetMenuData(object param)
        {
            if(param != null)
            {
                ShowGradeStarUp data = param as ShowGradeStarUp;
                curType = data.ShowType;
                curParData = data.PartnerData;
                oldAttrData = data.oldAttrData;
            }
            Init();
        }
        public override IEnumerator OnAddToStack()
        {
            return base.OnAddToStack();
        }
    
    
        public void Init()
        {
            if (curType == LTParGradeStarUp.StarUp && curParData.Star > 3) LTChargeManager.Instance.CheckLimitedTimeGiftTrigger(Hotfix_LT.Data.LTGTriggerType.StarUp, BalanceResourceUtil.GetUserLevel().ToString());
            isShowOpenProficiency = false;
            Show();
            if (curType == LTParGradeStarUp.GradeUp)
            {
                IsAwakening = true;
                ShowGradeUp();
            }
            else if (curType == LTParGradeStarUp.StarUp)
            {
                IsAwakening = false;
                isShowOpenProficiency = (curParData.Star == LTPartnerConfig.MAX_STAR);
                ShowStarUp();
            }else if (curType == LTParGradeStarUp.MaxLevel)
            {
                ShowMaxLevel();
                PlayMaxLevelAnimation();
                return;
            }
    
            ShowAttri();
            PlayAnimation();
        }
        private void Show()
        {
           controller.gameObject.CustomSetActive(true);
        }
        private void Hide()
        {
            controller.gameObject.CustomSetActive(false);
        }
        private void ShowGradeUp()
        {
            UpItem.SetItemDataOther(curParData);
            int oldQuality = 0;
            int oldAddLevel = 0;
            LTPartnerDataManager.GetPartnerQuality(curParData.UpGradeId - 1, out oldQuality, out oldAddLevel);
            CurItem.SetItemDataOther(curParData, oldQuality, oldAddLevel);
    
            Hotfix_LT.Data.UpGradeInfoTemplate tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(curParData.UpGradeId, curParData.HeroInfo.char_type);
            GradeUpLimitLv.text = string.Format(EB.Localizer.GetString("ID_LEVEL_FORMAT"), tpl.level_limit);
        }
    
        private void ShowStarUp()
        {
            FusionAudio.PostEvent("UI/Partner/StarUp");
            UpItem.SetItemDataOther(curParData);
            CurItem.SetItemDataByStarUp(curParData, curParData.Star - 1);
        }
    
        private void ShowMaxLevel()
        {
            UpItem.SetItemDataOther(curParData);
        }
    
    
        private void ShowAttri()
        {
            LTAttributesData attriData = AttributesManager.GetPartnerAttributesByParnterData(curParData);
    
            double addHP = 0;
            double addAtk = 0;
            double addDef = 0;
            double addSpeed = 0;
            if (curType == LTParGradeStarUp.GradeUp)
            {
                addHP = attriData.m_MaxHP - oldAttrData.m_MaxHP;
                addAtk = attriData.m_ATK - oldAttrData.m_ATK;
                addDef = attriData.m_DEF - oldAttrData.m_DEF;
                addSpeed = attriData.m_Speed - oldAttrData.m_Speed;
            }
            else if (curType == LTParGradeStarUp.StarUp)
            {
                addHP = attriData.m_MaxHP - oldAttrData.m_MaxHP;
                addAtk = attriData.m_ATK - oldAttrData.m_ATK;
                addDef = attriData.m_DEF - oldAttrData.m_DEF;
                addSpeed = attriData.m_Speed - oldAttrData.m_Speed;
            }
    
            string strHp = addHP >= 0 ? " [42fe79]+" : " [42fe79]-";
            string strAtk = addAtk >= 0 ? " [42fe79]+" : " [42fe79]-";
            string strDef = addDef >= 0 ? " [42fe79]+" : " [42fe79]-";
            string strSpeed = addSpeed >= 0 ? " [42fe79]+" : " [42fe79]-";
    
            // 加上0.5是为了做四舍五入
            LifeLabel.text = addHP == 0 ? ((int)(attriData.m_MaxHP + 0.5)).ToString() : (int)(attriData.m_MaxHP - addHP + 0.5) + strHp + Mathf.Abs((int)(attriData.m_MaxHP + 0.5) - (int)(attriData.m_MaxHP - addHP + 0.5));
            AtkLabel.text = addAtk == 0 ? ((int)(attriData.m_ATK + 0.5)).ToString() : (int)(attriData.m_ATK - addAtk + 0.5) + strAtk + Mathf.Abs((int)(attriData.m_ATK + 0.5) - (int)(attriData.m_ATK - addAtk + 0.5));
            DefLabel.text = addDef == 0 ? ((int)(attriData.m_DEF + 0.5)).ToString() : (int)(attriData.m_DEF - addDef + 0.5) + strDef + Mathf.Abs((int)(attriData.m_DEF + 0.5) - (int)(attriData.m_DEF - addDef + 0.5));
            CritLabel.text = addSpeed == 0 ? ((int)(attriData.m_Speed + 0.5)).ToString() : (int)(attriData.m_Speed - addSpeed + 0.5) + strSpeed + Mathf.Abs((int)(attriData.m_Speed + 0.5) - (int)(attriData.m_Speed - addSpeed + 0.5));
        }
    
        public override void OnCancelButtonClick()
        {
            if (!playOver) return;
    
            if (isShowOpenProficiency)
            {
                isShowOpenProficiency = false;
                PlayOpenProficiencyAnimation();
                return;
            }
    
            if (curType == LTParGradeStarUp.StarUp)
            {               
                Hotfix_LT.Messenger.Raise(EventName.OnPartnerStarUpSucc, curParData.Star);
            }
            Hide();
            base.OnCancelButtonClick();
        }
    
        private void PlayAnimation()
        {
            if (IsAwakening)
            {
                FusionAudio.PostEvent("UI/Partner/Awakening");
            }
            else
            {
                FusionAudio.PostEvent("UI/Partner/");
            }
            playOver = false;
            StartCoroutine(PlayAni());
        }
    
        private IEnumerator PlayAni()
        {
            TitleLabel.gameObject.CustomSetActive(false);
            CurItemTP.gameObject.CustomSetActive(false);
            UpItemTP.gameObject.CustomSetActive(false);
            LifeLabelTS.gameObject.CustomSetActive(false);
            AtkLabelTS.gameObject.CustomSetActive(false);
            CritLabelTS.gameObject.CustomSetActive(false);
            DefLabelTS.gameObject.CustomSetActive(false);
            ProficiencyTP.gameObject.CustomSetActive(false);
            LevelUpMaxTP.gameObject.CustomSetActive(false);
            ContinueLabel.CustomSetActive(false);
            GradeUpLimitLvObj.CustomSetActive(false);
            Arrow.CustomSetActive(false);
            yield return new WaitForSeconds(Time[0]);
            if (curType == LTParGradeStarUp.GradeUp)
            {
                TitleLabel.text = TitleLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_uifont_in_LTPartnerHud_GradeUpTitle_24");
            }
            else if (curType == LTParGradeStarUp.StarUp)
            {
                TitleLabel.text = TitleLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_uifont_in_LTPartnerHud_StarUpTitle_23");
            }
            TitleLabel.gameObject.CustomSetActive(true);
    
            yield return new WaitForSeconds(Time[1]);
            PlayAni(CurItemTP);
            PlayAni(UpItemTP);
    
            yield return new WaitForSeconds(Time[2]);
            Arrow.CustomSetActive(true);
            PlayAni(LifeLabelTS);
    
            yield return new WaitForSeconds(Time[3]);
            PlayAni(AtkLabelTS);
    
            yield return new WaitForSeconds(Time[4]);
            PlayAni(CritLabelTS);
    
            yield return new WaitForSeconds(Time[5]);
            PlayAni(DefLabelTS);
    
            yield return new WaitForSeconds(Time[6]);
    
            GradeUpLimitLvObj.CustomSetActive(false);
            ContinueLabel.CustomSetActive(true);
    
            playOver = true;
        }
    
        private void PlayOpenProficiencyAnimation()
        {
            playOver = false;
            StartCoroutine(PlayOpenProficiencyAni());
        }
    
        private IEnumerator PlayOpenProficiencyAni()
        {
            var temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetAllProficiencyDesc();
            if (temp != null)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    if (i < ProficiencyIconList.Count)
                    {
                        ProficiencyIconList[i].spriteName = temp[i].icon;
                        ProficiencyNameLabel[i].text = temp[i].name;
                    }
                }
            }
    
            TitleLabel.gameObject.CustomSetActive(false);
            CurItemTP.gameObject.CustomSetActive(false);
            UpItemTP.gameObject.CustomSetActive(false);
            LifeLabelTS.gameObject.CustomSetActive(false);
            AtkLabelTS.gameObject.CustomSetActive(false);
            CritLabelTS.gameObject.CustomSetActive(false);
            DefLabelTS.gameObject.CustomSetActive(false);
            ProficiencyTP.gameObject.CustomSetActive(false);
            LevelUpMaxTP.gameObject.CustomSetActive(false);
            ContinueLabel.CustomSetActive(false);
            GradeUpLimitLvObj.CustomSetActive(false);
            Arrow.CustomSetActive(false);
            yield return new WaitForSeconds(Time[0]);
            TitleLabel.text = TitleLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_OPEN_PROFICIENCY");
            TitleLabel.gameObject.CustomSetActive(true);
            yield return new WaitForSeconds(Time[1]);
            UpItemTP.transform.localPosition=new Vector3 (0, UpItemTP.transform.localPosition.y, UpItemTP.transform.localPosition.x);
            PlayAni(UpItemTP.transform .GetComponent <TweenScale>());
            yield return new WaitForSeconds(Time[2]);
            PlayAni(ProficiencyTP);
            yield return new WaitForSeconds(Time[6]);
            ContinueLabel.CustomSetActive(true);
            playOver = true;
        }
    
        private void PlayMaxLevelAnimation()
        {
            playOver = false;
            StartCoroutine(PlayMaxLevelAni());
        }
    
        private IEnumerator PlayMaxLevelAni()
        {
            var temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetProficiencyDescByType(Hotfix_LT.Data.ProficiencyType.AllRound);
            if (temp != null)
            {
                LevelUpMaxIcon.spriteName = temp.icon;
                LevelUpMaxName.text =temp.name;
                LevelUpMaxDesc.text =temp.desc;
            }
    
            TitleLabel.gameObject.CustomSetActive(false);
            CurItemTP.gameObject.CustomSetActive(false);
            UpItemTP.gameObject.CustomSetActive(false);
            LifeLabelTS.gameObject.CustomSetActive(false);
            AtkLabelTS.gameObject.CustomSetActive(false);
            CritLabelTS.gameObject.CustomSetActive(false);
            DefLabelTS.gameObject.CustomSetActive(false);
            ProficiencyTP.gameObject.CustomSetActive(false);
            LevelUpMaxTP.gameObject.CustomSetActive(false);
            ContinueLabel.CustomSetActive(false);
            GradeUpLimitLvObj.CustomSetActive(false);
            Arrow.CustomSetActive(false);
            yield return new WaitForSeconds(Time[0]);
            TitleLabel.text = TitleLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_OPEN_PARTNER_MAX_LEVEL");
            TitleLabel.gameObject.CustomSetActive(true);
            yield return new WaitForSeconds(Time[1]);
            UpItemTP.transform.localPosition = new Vector3(0, UpItemTP.transform.localPosition.y, UpItemTP.transform.localPosition.x);
            PlayAni(UpItemTP.transform.GetComponent<TweenScale>());
            yield return new WaitForSeconds(Time[2]);
            PlayAni(LevelUpMaxTP);
            yield return new WaitForSeconds(Time[6]);
            ContinueLabel.CustomSetActive(true);
            playOver = true;
        }
    
        private void PlayAni(UITweener tween)
        {
            tween.gameObject.CustomSetActive(true);
            tween.ResetToBeginning();
            tween.PlayForward();
        }
    
        public void PlayAniTest()
        {
            PlayAnimation();
        }
    }
}
