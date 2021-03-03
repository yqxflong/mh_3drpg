using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    //public class OnLegionDonateSucc : GameEvent
    //{ }
    public class LegionPageDonate : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            nameLab = t.Find("Left/Top/NameLabel").GetComponent<UILabel>();
            levelLab = t.Find("Left/Top/LevelSprite/LevelLabel").GetComponent<UILabel>();
            curLevelLab = t.Find("Left/Level/CurLevel").GetComponent<UILabel>();
            levelUpLab = t.Find("Left/Level/NewLevelObj/NewLevel").GetComponent<UILabel>();
            progressLab = t.Find("Left/ProgressBar/ProgressLabel").GetComponent<UILabel>();
            curMemberNumLab = t.Find("Left/PeopleMaxNum/Num").GetComponent<UILabel>();
            levelUpMemberNumLab = t.Find("Left/PeopleMaxNum/LvUpNum").GetComponent<UILabel>();
            peoplevelup = t.GetComponent<Transform>("Left/PeopleMaxNum/Sprite").gameObject;
            techlevelup = t.GetComponent<Transform>("Left/TechnologyMaxLv/Sprite").gameObject;
            curTechMaxLevel = t.GetComponent<UILabel>("Left/TechnologyMaxLv/Num");
            NextTechMaxLevel = t.GetComponent<UILabel>("Left/TechnologyMaxLv/LvUpNum");
            lessDonateTips = t.Find("Left/TimesLabel").GetComponent<UILabel>();
            legionIconSp = t.Find("Left/Top/Badge/Icon").GetComponent<UISprite>();
            legioinIconBGSp = t.Find("Left/Top/Badge/IconBG").GetComponent<UISprite>();
            expProgress = t.Find("Left/ProgressBar").GetComponent<UIProgressBar>();
            donateAni = t.Find("Left/AniObj").GetMonoILRComponent<LegionDonateAni>();
            newLevelObj = t.Find("Left/Level/NewLevelObj").gameObject;
            items[0] = t.Find("Right/Grid/HighestDonate").GetMonoILRComponent<LegionDonateItem>();
            items[1] = t.Find("Right/Grid/DiamondDonate").GetMonoILRComponent<LegionDonateItem>();
            items[2] = t.Find("Right/Grid/GoldDonate").GetMonoILRComponent<LegionDonateItem>();
            donateChest = new LegionDonateChest[4];
            donateChest[0] = t.GetMonoILRComponent<LegionDonateChest>("Right/RightTop/Grid/Chest");
            donateChest[1] = t.GetMonoILRComponent<LegionDonateChest>("Right/RightTop/Grid/Chest (1)");
            donateChest[2] = t.GetMonoILRComponent<LegionDonateChest>("Right/RightTop/Grid/Chest (2)");
            donateChest[3] = t.GetMonoILRComponent<LegionDonateChest>("Right/RightTop/Grid/Chest (3)");
            todayMedal = t.GetComponent<UILabel>("Right/RightTop/Total/TotalMedal");
            todayMedalShadow = t.GetComponent<UILabel>("Right/RightTop/Total/TotalMedal/Label");
            medalProgresssp = t.GetComponent<UISprite>("Right/RightTop/Sprite");
            maxmedalbarwidth = 900;
            maxScorewidth = 840;
        }


        public enum DonateType
        {
            luxury = 0,
            diamond = 1,
            gold = 2
        }

        public UILabel nameLab;
        public UILabel levelLab;
        public UILabel curLevelLab;
        public UILabel levelUpLab;
        public UILabel progressLab;
        public UILabel curMemberNumLab,curTechMaxLevel;
        public UILabel levelUpMemberNumLab,NextTechMaxLevel;
        public UILabel lessDonateTips;
        public UISprite legionIconSp;
        public UISprite legioinIconBGSp;
        public UIProgressBar expProgress;
        public LegionDonateAni donateAni;

        public GameObject newLevelObj,peoplevelup,techlevelup;

        public LegionDonateItem[] items = new LegionDonateItem[3];

        private LegionDonateChest[] donateChest;

        private DonateType curChooseDonateType;

        private int donateCoinAmount;
        private int donateExpAmount;
        private int maxmedalbarwidth;
        private int maxScorewidth;

        private bool isFirstLuxuryDonate = false;
        private bool isFirstDiamondDonate = false;
        private bool isFirstGoldDonate = false;

        private UILabel todayMedal, todayMedalShadow;
        private UISprite medalProgresssp;

        public override void OnEnable()
        {

            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnLegionDonateSucc, OnLegionDonateSuccFunc);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnLegionDonateTimesChaged, RefreshDonateTimes);
        }

        public override void OnDisable()
        {
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnLegionDonateSucc, OnLegionDonateSuccFunc);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnLegionDonateTimesChaged, RefreshDonateTimes);
        }

        public void SetData(LegionData legionData)
        {
            if (legionData == null || legionData.userMemberData == null)
                return;

            if (!legionData.legionIconSptName.Equals(""))
            {
                legionIconSp.spriteName = legionData.legionIconSptName;
            }

            if (!legionData.legionIconBGSptName.Equals(""))
            {
                legioinIconBGSp.spriteName = legionData.legionIconBGSptName;
            }

            nameLab.text = legionData.legionName;
            levelLab.text = legionData.legionLevel.ToString();
            curLevelLab.text = legionData.legionLevel.ToString();
            levelUpLab.text = (legionData.legionLevel + 1).ToString();

            if (legionData.legionLevel >= AlliancesManager.Instance.Config.MaxAllianceLevel)
            {
                curMemberNumLab.text = legionData.maxPeopleNum.ToString();
                curTechMaxLevel.text = legionData.legionLevel.ToString();
                levelUpMemberNumLab.text = "";
                NextTechMaxLevel.text = "";
                newLevelObj.CustomSetActive(false);
                peoplevelup.CustomSetActive(false);
                techlevelup.CustomSetActive(false);
            }
            else
            {
                curMemberNumLab.text = legionData.maxPeopleNum.ToString();
                levelUpMemberNumLab.text = string.Format("[42FE79]{0}[-]", legionData.growupMaxPeopleNum);
                curTechMaxLevel.text = legionData.legionLevel.ToString();
                NextTechMaxLevel.text = string.Format("[42FE79]{0}[-]", legionData.legionLevel + 1);
                newLevelObj.CustomSetActive(true);
                peoplevelup.CustomSetActive(true);
                techlevelup.CustomSetActive(true);
            }
            //levelUpMemberNumLab.text = "+" + (legionData.growupMaxPeopleNum - legionData.maxPeopleNum);

            float value = ((float)legionData.currentExp) / legionData.growupExp;
            expProgress.value = value > 1 ? 1 : value;

            progressLab.text = string.Format("{0}/{1}", legionData.currentExp, legionData.growupExp);

            //lessDonateTips.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LegionPageDonate_2747"), (AlliancesManager.Instance.Config.DonateMaxTimes - legionData.userMemberData.todayDonateTimes).ToString(), AlliancesManager.Instance.Config.DonateMaxTimes);

            //LegionLogic.GetInstance().HaveDonateRP();
            SetDonateChest();
        }

        public void SetLuxuryDonateAction(Action action)
        {
            if (items[(int)DonateType.luxury] != null)
            {
                items[(int)DonateType.luxury].donateActoin = () =>
                {
                    curChooseDonateType = DonateType.luxury;
                    action();
                };
            }
        }

        public void SetDiamondDonateAction(Action action)
        {
            if (items[(int)DonateType.diamond] != null)
            {
                items[(int)DonateType.diamond].donateActoin = () =>
                {
                    curChooseDonateType = DonateType.diamond;
                    action();
                };
            }
        }

        public void SetGoldDonateAction(Action action)
        {
            if (items[(int)DonateType.gold] != null)
            {
                items[(int)DonateType.gold].donateActoin = () =>
                {
                    curChooseDonateType = DonateType.gold;
                    action();
                };
            }
        }

        public void ShowUI(bool isShow)
        {
            mDMono.gameObject.CustomSetActive(isShow);

            if (isShow)
            {
                ShowDonateItem();
                RefreshFirstDonateFlag();
                SetDonateChestData();
                RefreshDonateTimes();
            }
        }

        public void ShowDonateItem()
        {
            items[(int)DonateType.luxury].ShowUI(DonateType.luxury,AlliancesManager.Instance.Config.DonateLuxurySpend, AlliancesManager.Instance.Config.DonateLuxuryRedeem, AlliancesManager.Instance.Config.DonateLuxuryRedeem);
            items[(int)DonateType.diamond].ShowUI(DonateType.diamond, AlliancesManager.Instance.Config.DonateHcSpend, AlliancesManager.Instance.Config.DonateHcRedeem, AlliancesManager.Instance.Config.DonateHcRedeem);
            items[(int)DonateType.gold].ShowUI(DonateType.gold, AlliancesManager.Instance.Config.DonateGoldSpend, AlliancesManager.Instance.Config.DonateGoldRedeem, AlliancesManager.Instance.Config.DonateGoldRedeem);
        }

        private void SetDonateChestData()
        {
            if (chestList == null)
            {
                chestList = AllianceTemplateManager.Instance.mDonateChestList;
            }
            for (int i = 0; i < donateChest.Length; i++)
            {
                if (i < chestList.Count)
                {
                    var tempDonateChest = chestList[i];
                    donateChest[i].Fill(chestList[i]);
                    maxMedalShow = tempDonateChest.score > maxMedalShow ? tempDonateChest.score : maxMedalShow;
                }
            }
            maxMedalShow = maxMedalShow * maxmedalbarwidth / maxScorewidth;
            SetDonateChest();
        }

        private void RefreshDonateTimes()
        {
            //int LuxuryTimes = 0, HcTimes = 0, GoldTime = 0;
            //if (isDonate && userdata != null)
            //{
            //    LuxuryTimes = userdata.todayLuxuryDonateTimes;
            //    HcTimes = userdata.todayHcDonateTimes;
            //    GoldTime = userdata.todayGoldDonateTimes;
            //}
            //else if (!isDonate)
            //{
            //    LuxuryTimes = AlliancesManager.Instance.CurDonateInfo.luxuryDonateTimes;
            //    HcTimes = AlliancesManager.Instance.CurDonateInfo.hcDonateTimes;
            //    GoldTime = AlliancesManager.Instance.CurDonateInfo.goldDonateTimes;
            //} 
            items[(int)DonateType.luxury].RefreshData(AlliancesManager.Instance.CurDonateInfo.luxuryDonateTimes, AlliancesManager.Instance.Config.LuxuryDonateMaxTimes);
            items[(int)DonateType.diamond].RefreshData(AlliancesManager.Instance.CurDonateInfo.hcDonateTimes, AlliancesManager.Instance.Config.DiamondDonateMaxTimes);
            items[(int)DonateType.gold].RefreshData(AlliancesManager.Instance.CurDonateInfo.goldDonateTimes, AlliancesManager.Instance.Config.GoldDonateMaxTimes);         
        }

        private List<AllianceDonateChest> chestList;
        private int maxMedalShow = 0;
        private int pretodaydonate = -1;
        private void SetDonateChest()
        {
            int todayMedalNum = AlliancesManager.Instance.Detail.TodayTotalExp;
            if(todayMedalNum == pretodaydonate|| maxMedalShow == 0)
            {
                return;
            }           
            todayMedal.text = todayMedalShadow.text = todayMedalNum.ToString();
            if (maxMedalShow != 0 && todayMedalNum != 0)
            {
                if (todayMedalNum >= maxMedalShow)
                {
                    medalProgresssp.width = maxmedalbarwidth;
                }
                else
                {
                    medalProgresssp.width = todayMedalNum * maxmedalbarwidth / maxMedalShow;
                }
                medalProgresssp.transform.gameObject.CustomSetActive(true);
            }
            else
            {
                medalProgresssp.transform.gameObject.CustomSetActive(false);
            }
            pretodaydonate = todayMedalNum;
        }

        //private void FetchDataHandler(Hashtable alliance)
        //{
        //    if (alliance != null)
        //    {
        //        GameDataSparxManager.Instance.ProcessIncomingData(alliance, false);
        //        RefreshFirstDonateFlag();
        //        RefreshDonateTimes();
        //        SetDonateChestData();
        //        SetDonateChest();
        //    }
        //}

        private void RefreshFirstDonateFlag()
        {
            isFirstLuxuryDonate = AlliancesManager.Instance.CurDonateInfo.luxuryDonateTimes == 0;
            isFirstDiamondDonate = AlliancesManager.Instance.CurDonateInfo.hcDonateTimes == 0;
            isFirstGoldDonate = AlliancesManager.Instance.CurDonateInfo.goldDonateTimes == 0;
        }

        private void OnLegionDonateSuccFunc()
        {
            //items[(int)curChooseDonateType].ShowEffect(false);
            int times = Data.VIPTemplateManager.Instance.GetTotalNum(Data.VIPPrivilegeKey.FirstDonateDouble);
            bool isDouble = times>0;
            if (curChooseDonateType == DonateType.luxury)
            {
                //友盟统计
                UmnegPostEvent(FusionTelemetry.CurrencyChangeData.hc, AlliancesManager.Instance.Config.DonateLuxurySpend);
                donateCoinAmount = AlliancesManager.Instance.Config.DonateLuxuryRedeem * (isFirstLuxuryDonate&& isDouble ? 2 : 1);
                donateExpAmount = AlliancesManager.Instance.Config.DonateLuxuryRedeem * (isFirstLuxuryDonate && isDouble ? 2 : 1);
                isFirstLuxuryDonate = false;
            }
            else if (curChooseDonateType == DonateType.diamond)
            {
                //友盟统计
                UmnegPostEvent(FusionTelemetry.CurrencyChangeData.hc, AlliancesManager.Instance.Config.DonateHcSpend);
                donateCoinAmount = AlliancesManager.Instance.Config.DonateHcRedeem * (isFirstDiamondDonate && isDouble ? 2 : 1);
                donateExpAmount = AlliancesManager.Instance.Config.DonateHcRedeem * (isFirstDiamondDonate && isDouble ? 2 : 1);
                isFirstDiamondDonate = false;
            }
            else if (curChooseDonateType == DonateType.gold)
            {
                //友盟统计
                UmnegPostEvent(FusionTelemetry.CurrencyChangeData.gold, AlliancesManager.Instance.Config.DonateGoldSpend);

                donateCoinAmount = AlliancesManager.Instance.Config.DonateGoldRedeem * (isFirstGoldDonate && isDouble ? 2 : 1);
                donateExpAmount = AlliancesManager.Instance.Config.DonateGoldRedeem * (isFirstGoldDonate && isDouble ? 2 : 1);
                isFirstGoldDonate = false;
            }
            donateAni.PlayAni(donateExpAmount, donateCoinAmount);
        }

        void UmnegPostEvent(string currency_type, int currency_count)
        {
            FusionTelemetry.CurrencyChangeData.PostEvent(currency_type, -currency_count, "军团捐献");
        }
    }
    
}
