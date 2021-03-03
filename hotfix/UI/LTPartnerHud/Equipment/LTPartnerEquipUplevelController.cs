using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTPartnerEquipUplevelController : DynamicMonoHotfix
    {
     public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            nowLevelLabel = t.GetComponent<UILabel>("PartnerView/LevelInfo/CurrentLevelLabel");
            nextLevelLabel = t.GetComponent<UILabel>("PartnerView/LevelInfo/NextLevelLabel");
            nowExpSlider = t.GetComponent<UISlider>("PartnerView/CurrentProgressBar");
            nextExpSlider = t.GetComponent<UISlider>("PartnerView/CurrentProgressBar/NextProgressBar");
            MainAttrObj = new EquipLvUpAttrObj();
            MainAttrObj.AttrName = t.GetComponent<UILabel>("PartnerView/Info/Main/Label");
            MainAttrObj.CurValue = t.GetComponent<UILabel>("PartnerView/Info/Main/ValueLabel");
            MainAttrObj.NextValue = t.GetComponent<UILabel>("PartnerView/Info/Main/NextValueLabel");
            MainAttrObj.LockObj = t.Find("PartnerView/Info/Main/Lock").gameObject;
            // MainAttrObj.FxObj = t.Find("PartnerView/Info/Main/Fx").gameObject;
            
            ExAttrObj = new List<EquipLvUpAttrObj>();
            EquipLvUpAttrObj temp1 = new EquipLvUpAttrObj();
            temp1.AttrName = t.GetComponent<UILabel>("PartnerView/Info/Bg (1)/Label");
            temp1.CurValue = t.GetComponent<UILabel>("PartnerView/Info/Bg (1)/ValueLabel");
            temp1.NextValue = t.GetComponent<UILabel>("PartnerView/Info/Bg (1)/NextValueLabel");
            temp1.LockObj = t.Find("PartnerView/Info/Bg (1)/Lock").gameObject;
            // temp1.FxObj = t.Find("PartnerView/Info/Bg (1)/Fx").gameObject;
            ExAttrObj.Add(temp1);
            
            EquipLvUpAttrObj temp2 = new EquipLvUpAttrObj();
            temp2.AttrName = t.GetComponent<UILabel>("PartnerView/Info/Bg (2)/Label");
            temp2.CurValue = t.GetComponent<UILabel>("PartnerView/Info/Bg (2)/ValueLabel");
            temp2.NextValue = t.GetComponent<UILabel>("PartnerView/Info/Bg (2)/NextValueLabel");
            temp2.LockObj = t.Find("PartnerView/Info/Bg (2)/Lock").gameObject;
            // temp2.FxObj = t.Find("PartnerView/Info/Bg (2)/Fx").gameObject;
            ExAttrObj.Add(temp2);
            
            EquipLvUpAttrObj temp3 = new EquipLvUpAttrObj();
            temp3.AttrName = t.GetComponent<UILabel>("PartnerView/Info/Bg (3)/Label");
            temp3.CurValue = t.GetComponent<UILabel>("PartnerView/Info/Bg (3)/ValueLabel");
            temp3.NextValue = t.GetComponent<UILabel>("PartnerView/Info/Bg (3)/NextValueLabel");
            temp3.LockObj = t.Find("PartnerView/Info/Bg (3)/Lock").gameObject;
            // temp3.FxObj = t.Find("PartnerView/Info/Bg (3)/Fx").gameObject;
            ExAttrObj.Add(temp3);
            
            EquipLvUpAttrObj temp4 = new EquipLvUpAttrObj();
            temp4.AttrName = t.GetComponent<UILabel>("PartnerView/Info/Bg (4)/Label");
            temp4.CurValue = t.GetComponent<UILabel>("PartnerView/Info/Bg (4)/ValueLabel");
            temp4.NextValue = t.GetComponent<UILabel>("PartnerView/Info/Bg (4)/NextValueLabel");
            temp4.LockObj = t.Find("PartnerView/Info/Bg (4)/Lock").gameObject;
            // temp4.FxObj = t.Find("PartnerView/Info/Bg (4)/Fx").gameObject;
            ExAttrObj.Add(temp4);
            
            
            
            SliderLabel = t.GetComponent<UILabel>("PartnerView/CurrentProgressBar/NumLabel");
            //Center/UplevelView/PartnerView/UpLevelBtnRoot/LevelUpBtn/NumLabel
            CostLabel = t.GetComponent<UILabel>("PartnerView/UpLevelBtnRoot/LevelUpBtn/NumLabel");
            CostIcon = t.GetComponent<UISprite>("PartnerView/UpLevelBtnRoot/LevelUpBtn/NumLabel/CostGold");
			UpLeveItem = t.GetMonoILRComponent<LTPartnerEquipCellController>("PartnerView/AA");
            ItemFx = t.FindEx("PartnerView/AA/Fx").gameObject;
            ItemNameLabel = t.GetComponent<UILabel>("PartnerView/TitleName");
            UpLevelBtnRoot = t.Find("PartnerView/UpLevelBtnRoot").gameObject;
            UpLevelBtnSprite = t.GetComponent<UISprite>("PartnerView/UpLevelBtnRoot/LevelUpBtn");
            NextLevel = 0;
            SynthesisBtn = t.FindEx("PartnerView/SynthesisUI").gameObject;
            SynthesisBtnBG = t.FindEx("PartnerView/SynthesisUI/EquipSynthesisBtn").gameObject;
            SynthesisLabelTop = t.FindEx("PartnerView/SynthesisUI/EquipSynthesisBtn/Label").gameObject;
            SynthesisReflashTip = t.GetComponent<UILabel>("PartnerView/SynthesisUI/Label");
            SliderFx = t.GetMonoILRComponent<LTEquipSliderAni>("PartnerView/CurrentProgressBar");
            LabelFx = t.GetMonoILRComponent<LTEquipLabelAni>("PartnerView/CurrentProgressBar/NumLabel");
            maxLevelLabel= t.GetComponent<UILabel>("PartnerView/MaxLevelInfo/CurrentLevelLabel");
        }


        [System.Serializable]
        public class EquipLvUpAttrObj
        {
            public UILabel AttrName, CurValue, NextValue;
            public GameObject LockObj;
        }
        public UILabel nowLevelLabel, nextLevelLabel,maxLevelLabel;
        public UISlider nowExpSlider, nextExpSlider;
        public UILabel SliderLabel;
        public EquipLvUpAttrObj MainAttrObj;
        public List<EquipLvUpAttrObj> ExAttrObj;
        public UILabel CostLabel;
        public UISprite CostIcon;
        public LTPartnerEquipCellController UpLeveItem;
        public GameObject ItemFx;
        public UILabel ItemNameLabel;
        public static int NextLevel;
        public static int MaxLevel;
        public DetailedEquipmentInfo data;
        public GameObject UpLevelBtnRoot;
        public UISprite UpLevelBtnSprite;
        private BoxCollider UpLevelBtnBox;
    
        public GameObject SynthesisBtn;
        public GameObject SynthesisBtnBG;
        public GameObject SynthesisLabelTop;
        public UILabel SynthesisReflashTip;
        [HideInInspector]public List<Hotfix_LT.Data.EquipmentLevelUp> LevelUpList;
        
        public override void OnEnable()
        {
            ResetInfor();
        }
    
        public override void OnDisable()
        {
            //             //todo
             // StopCoroutine("LevelUpScrollAniPlay");
             // StopCoroutine("LevelUpAttrAniPlay");
             StopCoroutine(LevelUpScrollAniPlay());
             StopCoroutine(LevelUpAttrAniPlay());
            FusionAudio.PostEvent("UI/Partner/ExpSlider", false);
        }
    
        private void ResetInfor()
        {
            MainAttrObj.NextValue.gameObject.CustomSetActive(false);
            // MainAttrObj.FxObj.gameObject.CustomSetActive(false);
            for (int i = 0; i < ExAttrObj.Count; i++)
            {
                ExAttrObj[i].NextValue.gameObject.CustomSetActive(false);
                // ExAttrObj[i].FxObj.gameObject.CustomSetActive(false);
            }
            ItemFx.CustomSetActive(false);
        }
        /// <summary>
        /// 刷新强化所有属性
        /// </summary>
        /// <param name="eid"></param>
        public void Show(int eid)
        {
            StopAllCoroutines();
            SliderFx.ResetList();
            LabelFx.ResetList();
            data =LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eid);
            if(data == null)
            {
                EB.Debug.LogError("LTPartnerEquipUplevelController.Show data is null,Eid = {0}",eid);
                return;
            }
            if (data.Eid == 0) return;
            UpLeveItem.Fill(data);
            ItemNameLabel.applyGradient = true;
            ItemNameLabel.gradientTop = LT.Hotfix.Utility.ColorUtility.QualityToGradientTopColor(data.QualityLevel);
            ItemNameLabel.gradientBottom = LT.Hotfix.Utility.ColorUtility.QualityToGradientBottomColor(data.QualityLevel);
            ItemNameLabel.text = data.Name;
            LevelUpList = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetLevelUpListByQuality(data.QualityLevel);
            nowLevelLabel.text = nowLevelLabel.transform.GetChild(0).GetComponent<UILabel>().text =  nextLevelLabel.text =nextLevelLabel.transform.GetChild(0).GetComponent<UILabel>().text =data.EquipLevel.ToString();
            nextLevelLabel.gameObject.CustomSetActive(false);
            NextLevel = data.EquipLevel;
            MaxLevel = LevelUpList.Count;
            maxLevelLabel.text = maxLevelLabel.transform.GetChild(0).GetComponent<UILabel>().text = MaxLevel.ToString();
            if (data.MainAttributes.Name != null)
            {
                MainAttrObj.AttrName.text =AttrTypeTrans(data.MainAttributes.Name);
                MainAttrObj.CurValue.text = AttrTrans(data.MainAttributes); 
                // MainAttrObj.FxObj.CustomSetActive(false);
                if (MainAttrObj.NextValue.gameObject.activeSelf) {
                    MainAttrObj.CurValue.GetComponent<TweenScale>().ResetToBeginning();
                    MainAttrObj.CurValue.GetComponent<TweenScale>().PlayForward();
                    MainAttrObj.NextValue.gameObject.CustomSetActive(false);
                }
            }
            else
            {
                EB.Debug.LogError(string.Format("Equipment(id={0}) Main Attribbute is Missing", data.Eid));
                MainAttrObj.AttrName.gameObject.CustomSetActive(false);
                MainAttrObj.CurValue.gameObject.CustomSetActive(false);
                MainAttrObj.NextValue.gameObject.CustomSetActive(false);
                MainAttrObj.LockObj.CustomSetActive(false);
                // MainAttrObj.FxObj.CustomSetActive(false);
            }
            int UpLv = data.EquipLevel / 3;
            int high = 0;
            EquipAttributeRate rate = EconemyTemplateManager.Instance.GetEquipAttributeRate(data.QualityLevel);
            for (int i = rate.rating.Count - 1; i >= 0; i--)
            {
                if (rate.rating[i] > 0)
                {
                    high = i;
                    break;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if(i < data.ExAttributes.Count)
                {
                    ExAttrObj[i].AttrName.text = AttrTypeTrans(data.ExAttributes[i].Name);
                    ExAttrObj[i].CurValue.text = AttrTrans(data.ExAttributes[i]);
                    ExAttrObj[i].AttrName.gameObject.CustomSetActive(true);
                    ExAttrObj[i].CurValue.gameObject.CustomSetActive(true);
                    ExAttrObj[i].LockObj.CustomSetActive(false);
                    if (ExAttrObj[i].NextValue.gameObject.activeSelf)
                    {
                        ExAttrObj[i].CurValue.GetComponent<TweenScale>().ResetToBeginning();
                        ExAttrObj[i].CurValue.GetComponent<TweenScale>().PlayForward();
                        ExAttrObj[i].NextValue.gameObject.CustomSetActive(false);
                    }
                }
                else
                {
                    UpLv++;
                    if (i < high)
                    {
                        ExAttrObj[i].LockObj.GetComponent<UILabel>().text = ExAttrObj[i].LockObj.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipUplevelController_4355"), UpLv * 3);
                    }
                    else
                    {
                        ExAttrObj[i].LockObj.GetComponent<UILabel>().text = ExAttrObj[i].LockObj.transform.GetChild(0).GetComponent<UILabel>().text = string.Empty;
                    }
                    ExAttrObj[i].AttrName.gameObject.CustomSetActive(false);
                    ExAttrObj[i].CurValue.gameObject.CustomSetActive(false);
                    ExAttrObj[i].NextValue.gameObject.CustomSetActive(false);
                    ExAttrObj[i].LockObj.CustomSetActive(true);
                    // ExAttrObj[i].FxObj.CustomSetActive(false);
                }
            }
            int NowExp = 0;
            int NeedExp = 0;
            int OverPlueExp = 0;
            if (data.EquipLevel < MaxLevel)
            {
                Hotfix_LT.Data.EquipmentLevelUp LevelUpInfo = LevelUpList[data.EquipLevel];
                OverPlueExp = LevelUpInfo.TotalNeedExp - LevelUpInfo.needExp;
    
                NowExp = data.Exp - OverPlueExp;
                NeedExp = LevelUpInfo.needExp;
                SliderLabel.text = string.Format("{0}/{1}", NowExp, NeedExp);
            }
            else
            {
                SliderLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipLabelAni_1251"));
            }
    
            nowExpSlider.value = nextExpSlider.value = (data.EquipLevel >= MaxLevel) ? 1.0f : ((float)NowExp / (float)NeedExp);
            CostLabel.text=CostLabel .transform .GetChild (0).GetComponent <UILabel>().text= "0";
            //CostLabel.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
			CostIcon.UpdateAnchors();
			SetUplevelBtn();
        }
        /// <summary>
        /// 判断是否显示合成按钮，以及相关处理
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowSynthesis(DetailedEquipmentInfo data)
        {
            if (data.EquipLevel>=MaxLevel&&data.QualityLevel == 6)
            {
                //获取创建账号登录时间
                Hotfix_LT.Data.FuncTemplate tempFun = new Hotfix_LT.Data.FuncTemplate();
                tempFun = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10086);
                bool isUnLock = false;
                if (tempFun == null)
                {
                    isUnLock = true;
                }else
                {
                    isUnLock = tempFun.IsConditionOK();
                }
                UpLevelBtnRoot.CustomSetActive(false);
                SynthesisBtn.CustomSetActive(true);
                ///先做修改进行测试
                if(isUnLock)
                {
                    SynthesisLabelTop.transform.localPosition = new Vector3(0, 14, 0);
                    SynthesisLabelTop.transform.localScale = Vector3.one;
                    SynthesisReflashTip.text = "";
                    SynthesisBtnBG.GetComponent<BoxCollider>().enabled= isUnLock;
                    SynthesisBtnBG.GetComponent<UISprite>().color = new Color(1, 1, 1, 1);
                    SynthesisBtnBG.GetComponent<UISprite>().spriteName = "Ty_Button_3";
                }
                else
                {
                    SynthesisLabelTop.transform.localPosition = new Vector3(0, 50, 0);
                    SynthesisLabelTop.transform.localScale = Vector3.one;
                    SynthesisReflashTip.text = tempFun.GetConditionStr();   
                    SynthesisBtnBG.GetComponent<BoxCollider>().enabled = false;
                    SynthesisBtnBG.GetComponent<UISprite>().color = new Color(1, 0, 1, 1);
                    SynthesisBtnBG.GetComponent<UISprite>().spriteName = "Ty_Button_1";
                }
                            
            }
            else
            {
                UpLevelBtnRoot.CustomSetActive(true);
                SynthesisBtn.CustomSetActive(false);
            }                 
        }
    
        public void ShowLevelUp()
        {
            if(OldData !=data)
            {
                OldData =data;
                Show(data.Eid);
            }
            string colorStr =(LTPartnerEquipDataManager.Instance.UpLevelSelectList.Count >0)?"[42fe79]": "[ffffff]";
            int totalAddEXP = 0;
            Dictionary<string, int> Dics = LTPartnerEquipDataManager.Instance.getEquipUpItemNumDic();
            foreach (var item in Dics)
            {
                totalAddEXP += LTPartnerEquipDataManager.Instance.getEquipUpItemExp(item.Key) * item.Value;
            }
            for(int i=0;i< LTPartnerEquipDataManager.Instance.UpLevelSelectList.Count; i++)
            {
                totalAddEXP += LTPartnerEquipDataManager.Instance.GetTotleExpByEid(LTPartnerEquipDataManager.Instance.UpLevelSelectList[i]);
            }
            Hotfix_LT.Data.EquipmentLevelUp LevelUpInfo =new Hotfix_LT.Data.EquipmentLevelUp();
            bool GetExpInfo = false;
            for (int i = data.EquipLevel; i < MaxLevel; i++)
            {
                if (data.Exp+ totalAddEXP < LevelUpList[i].TotalNeedExp)
                {
                    GetExpInfo = true;
                    LevelUpInfo = LevelUpList[i];break;
                }
            }
            int OverPlueExp = GetExpInfo? (LevelUpInfo.TotalNeedExp - LevelUpInfo.needExp) : LevelUpList[MaxLevel - 1].TotalNeedExp;
            int costCount = totalAddEXP;
            if (totalAddEXP > 0 && !GetExpInfo)
            {
                costCount = OverPlueExp - data.Exp;
            }
    
            int temp1 = data.Exp - OverPlueExp+ totalAddEXP;
            int temp2 = LevelUpInfo.needExp;
            float tempValue =(float )temp1 /(float)temp2;
            nextExpSlider.value =(LevelUpInfo.level==0||(LevelUpInfo .level -1)>data.EquipLevel)?1: tempValue;
    
            
            nextLevelLabel.text = nextLevelLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}", colorStr,(LevelUpInfo.level - 1)>=0? (LevelUpInfo.level - 1) : LevelUpList.Count);
            nextLevelLabel.gameObject.CustomSetActive(((LevelUpInfo.level - 1) >= 0 ? (LevelUpInfo.level - 1) : MaxLevel) != data.EquipLevel);

            NextLevel = (LevelUpInfo.level - 1) >= 0 ? (LevelUpInfo.level - 1) : MaxLevel;
            SliderLabel.text =string.Format("{0}{1}/{2}", colorStr, temp1, LevelUpInfo.needExp);
    
            int resGold = BalanceResourceUtil.GetUserGold();
            string colorStr2 = (costCount > resGold) ? "[ff6699]" : "[ffffff]";
            CostLabel.text = CostLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}", colorStr2, costCount);
            //CostLabel.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
			CostIcon.UpdateAnchors();

			SetUplevelBtn(totalAddEXP>0);
        }
    
        private void SetUplevelBtn(bool show=false)
        {
            if (UpLevelBtnBox == null)
            {
                UpLevelBtnBox = UpLevelBtnSprite.GetComponent<BoxCollider>();
            }
            if (show)
            {
                UpLevelBtnBox.enabled = true;
                UpLevelBtnSprite.spriteName = "Ty_Button_3";
                UpLevelBtnSprite.color = Color.white;
            }
            else
            {
                UpLevelBtnBox.enabled = false;
                UpLevelBtnSprite.spriteName = "Ty_Button_2";
                UpLevelBtnSprite.color = Color.magenta;
            }
        }
    
        private DetailedEquipmentInfo OldData;
        public void PlayFxLevelUp()
        {
            OldData=data;
            data = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(data.Eid);
            StartCoroutine(LevelUpScrollAniPlay());
            StartCoroutine(LevelUpAttrAniPlay());
        }
    
        //public GameObject Fx;
        public LTEquipSliderAni  SliderFx;
        public LTEquipLabelAni  LabelFx;
        IEnumerator LevelUpScrollAniPlay()//装备经验条动效
        {       
            nextLevelLabel.gameObject.CustomSetActive(false);
            InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 1f);
            nextExpSlider.value = 0;
            CostLabel.text = CostLabel.transform.GetChild(0).GetComponent<UILabel>().text = "0";
            //CostLabel.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
			CostIcon.UpdateAnchors();
			SetUplevelBtn();
            int upLvTimes = data.EquipLevel - OldData.EquipLevel;//进行次数
            float maxTimer = 0.8f;//最大花费时间
            float minTimer = 0.1f;//最小花费时间
            int maxNum = 0;//最大值
            Hotfix_LT.Data.EquipmentLevelUp LevelUpInfo = LevelUpList[OldData.EquipLevel];
            int OverPlueExp = LevelUpInfo.TotalNeedExp - LevelUpInfo.needExp;
            int NowExp = OldData.Exp - OverPlueExp;
            int NeedExp = LevelUpInfo.needExp;
            float startValue =(float)NowExp/ (float)NeedExp;//开始经验条
            int startNum = NowExp;//开始数字
            float endValue;//结束经验条
            int endNum ;//结束数字
            if (data.EquipLevel != MaxLevel)
            {
                LevelUpInfo = LevelUpList[data.EquipLevel];
                OverPlueExp = LevelUpInfo.TotalNeedExp - LevelUpInfo.needExp;
                NowExp = data.Exp - OverPlueExp;
                NeedExp = LevelUpInfo.needExp;
                endValue = (float)NowExp / (float)NeedExp;
                endNum = NowExp;
            } else//达到最大强化等级
            {
                endValue = -1;
                endNum = -1;
            }
            if(upLvTimes!=0) FusionAudio.PostEvent("UI/Partner/ExpSlider", true);
            for (int i=0;i<= upLvTimes;i++)
            {
                if (i != upLvTimes)
                {
                    float timer =Mathf.Clamp( maxTimer * (1f - startValue)/(upLvTimes-i),minTimer ,maxTimer);
                    SliderFx.TimesList.Enqueue(new SliderAniData(startValue,1, timer));
                    maxNum = LevelUpList[OldData.EquipLevel + i].needExp;
                    LabelFx.TimesList.Enqueue(new LabelAniData(startNum , maxNum, maxNum, timer));
                    startNum = 0;
                    startValue = 0;
                    yield return new WaitForSeconds(timer - 0.02f);
                    nowLevelLabel .text = nowLevelLabel.transform.GetChild(0).GetComponent<UILabel>().text = (OldData.EquipLevel+1 + i).ToString();
                    UpLeveItem.LevelLabel.gameObject.CustomSetActive(OldData.EquipLevel+1 + i > 0);
                    UpLeveItem.LevelLabel.text = string.Format("+{0}", (OldData.EquipLevel + 1 + i));
                    ItemFx.CustomSetActive(false);
                    ItemFx.CustomSetActive(true);
                }
                else//最后一次时
                {
                    SliderFx.TimesList.Enqueue(new SliderAniData(startValue,endValue , maxTimer * endValue));
                    maxNum = data.EquipLevel != MaxLevel? LevelUpList[data.EquipLevel].needExp:0;
                    LabelFx.TimesList.Enqueue(new LabelAniData(startNum,endNum ,maxNum , maxTimer * endValue));
                    nowLevelLabel.text =nowLevelLabel.transform.GetChild(0).GetComponent<UILabel>().text =data.EquipLevel.ToString();
                    UpLeveItem.LevelLabel.gameObject.CustomSetActive(data.EquipLevel>0);
                    UpLeveItem.LevelLabel.text = string.Format("+{0}", data.EquipLevel);
                    
                }
            }
            
            if (data.EquipLevel != OldData.EquipLevel)
            {
                FusionAudio.PostEvent("UI/Equipment/LvlUp", mDMono.gameObject, true);
            }
            ShowSynthesis(data);
            yield break;
        }
        IEnumerator LevelUpAttrAniPlay()//装备基础属性动效
        {
            int upLvTimes = data.EquipLevel - OldData.EquipLevel;//进行次数
            yield return new WaitForSeconds(0.4f+upLvTimes*0.2f);
            if (data.EquipLevel != OldData.EquipLevel)
            {
                if (data.MainAttributes.Value != OldData.MainAttributes.Value)
                {
                    MainAttrObj.CurValue.text = AttrTrans(OldData.MainAttributes);
                    MainAttrObj.NextValue.text = AttrTrans(data.MainAttributes, true);
                    MainAttrObj.LockObj.CustomSetActive(false);
                    // MainAttrObj.FxObj.CustomSetActive(true);
                    MainAttrObj.NextValue.gameObject.CustomSetActive(true);
                    MainAttrObj.NextValue.GetComponent<TweenAlpha>().ResetToBeginning();
                    MainAttrObj.NextValue.GetComponent<TweenAlpha>().PlayForward();
                }
                for (int i = 0; i < data.ExAttributes.Count; ++i)
                {
                    if (OldData.ExAttributes.Count > i)
                    {
                        if (data.ExAttributes[i].Value != OldData.ExAttributes[i].Value)
                        {
                            yield return new WaitForSeconds(0.2f);
                            ExAttrObj[i].AttrName.text = AttrTypeTrans (data.ExAttributes[i].Name);
                            ExAttrObj[i].CurValue.text = AttrTrans(OldData.ExAttributes[i]);
                            ExAttrObj[i].NextValue.text = AttrTrans(data.ExAttributes[i], true);
                            ExAttrObj[i].AttrName.gameObject.CustomSetActive(true);
                            ExAttrObj[i].CurValue.gameObject.CustomSetActive(true);
                            ExAttrObj[i].LockObj.CustomSetActive(false);
                            // ExAttrObj[i].FxObj.CustomSetActive(true);
                            ExAttrObj[i].NextValue.gameObject.CustomSetActive(true);
                            ExAttrObj[i].NextValue.GetComponent<TweenAlpha>().ResetToBeginning();
                            ExAttrObj[i].NextValue.GetComponent<TweenAlpha>().PlayForward();
                        }
                    }
                    else//新增属性
                    {
                        yield return new WaitForSeconds(0.2f);
                        ExAttrObj[i].AttrName.text = AttrTypeTrans(data.ExAttributes[i].Name);
                        ExAttrObj[i].CurValue.text = "[fff348]New";
                        ExAttrObj[i].NextValue.text = AttrTrans(data.ExAttributes[i], true);
                        ExAttrObj[i].AttrName.gameObject.CustomSetActive(true);
                        ExAttrObj[i].CurValue.gameObject.CustomSetActive(true);
                        ExAttrObj[i].LockObj.CustomSetActive(false);
                        // ExAttrObj[i].FxObj.CustomSetActive(true);
                        ExAttrObj[i].NextValue.gameObject.CustomSetActive(true);
                        ExAttrObj[i].NextValue.GetComponent<TweenAlpha>().ResetToBeginning();
                        ExAttrObj[i].NextValue.GetComponent<TweenAlpha>().PlayForward();
                    }
                }
            }
            MainAttrObj.NextValue.transform.GetChild(0).GetComponent<TweenScale>().ResetToBeginning();
            MainAttrObj.NextValue.transform.GetChild(0).GetComponent<TweenPosition>().ResetToBeginning();
            MainAttrObj.NextValue.transform.GetChild(0).GetComponent<TweenScale>().PlayForward();
            MainAttrObj.NextValue.transform.GetChild(0).GetComponent<TweenPosition>().PlayForward();
            for (int i = 0; i < data.ExAttributes.Count; ++i)
            {
                if (ExAttrObj[i].NextValue.gameObject.activeSelf)
                {
                    ExAttrObj[i].NextValue.transform.GetChild(0).GetComponent<TweenScale>().ResetToBeginning();
                    ExAttrObj[i].NextValue.transform.GetChild(0).GetComponent<TweenPosition>().ResetToBeginning();
                    ExAttrObj[i].NextValue.transform.GetChild(0).GetComponent<TweenScale>().PlayForward();
                    ExAttrObj[i].NextValue.transform.GetChild(0).GetComponent<TweenPosition>().PlayForward();
                }
            }
            yield break;
        }
    
        public string AttrTypeTrans(string str)
        {
    		return LT.Hotfix.Utility.EquipmentUtility.AttrTypeTrans(str);
    	}
    
        public string AttrTrans(EquipmentAttr data, bool color = false)
        {
            string str = color ? "[42fe79]+" : "+";
            switch (data.Name)
            {
                case "ATK": return (str + Mathf.FloorToInt(data.Value).ToString());
                case "MaxHP": return (str + Mathf.FloorToInt(data.Value).ToString());
                case "DEF": return (str + Mathf.FloorToInt(data.Value).ToString());
                case "CritP": return (str + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "CritV": return (str + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "ChainAtk": return (str + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "SpExtra": return (str + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "SpRes": return (str + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "MaxHPrate": return (str + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "ATKrate": return (str + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "DEFrate": return (str + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "Speed": return (str + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "speedrate": return (str + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                default: return EB.Localizer.GetString("ID_ATTR_Unknown") + "：";
            }
        }
    
    }
}
