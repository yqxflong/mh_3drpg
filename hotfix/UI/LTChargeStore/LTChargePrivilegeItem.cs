using EB;
using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChargePrivilegeItem : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            DiamondAllLab = t.GetComponent<UILabel>("TotalGain/TotalGainLab");
            TipLabel = t.GetComponent<UILabel>("TipPanel/Label");
            remainTimeLab = t.GetComponent<UILabel>("RemainTime/RemainTimeLab");
            PriceLab = t.GetComponent<UILabel>("PriceBtn/PriceValue");
            TitleLab = t.GetComponent<UILabel>("Title");
            DrawCardNumLabel = t.GetComponent<UILabel>("TotalGain/CardIcon/Num");
            ShowDrawCard = t.FindEx("TotalGain/CardIcon").gameObject;
            RedPoint = t.FindEx("PriceBtn/RedPoint").gameObject;
            BtnBoxCol = t.GetComponent<BoxCollider>("PriceBtn");
            BtnBg = t.GetComponent<UISprite>("PriceBtn/BG");
            t.GetComponent<ConsecutiveClickCoolTrigger>("PriceBtn").clickEvent.Add(new EventDelegate(OnClickItem));
        }
   
        public UILabel DiamondAllLab;
        public UILabel TipLabel;
    
        public UILabel remainTimeLab;
        public UILabel PriceLab;
        public UILabel TitleLab;
        public UILabel DrawCardNumLabel;
    
        public GameObject ShowDrawCard;
        public GameObject RedPoint;
        public BoxCollider BtnBoxCol;
        public UISprite BtnBg;
    
        private LTChargePrivilege.EMonthCardType mcType;
    
        private EB.IAP.Item curChargeData;
    
        private int everydayGotNum;
        private int remainDay;
        private bool isHasMonthCard;
        private bool isGetReward;
    
        public void ShowUI(EB.IAP.Item data)
        {
            if (data == null)
            {
                EB.Debug.LogError("ChargeData is Null !!!!");
                return;
            }
    
            curChargeData = data;
    
            InitData();
            RefreshUI();
        }
    
        public void SetMonthCardType(LTChargePrivilege.EMonthCardType mcType)
        {
            this.mcType = mcType;
        }
    
        private void InitData()
        {
            string path = mcType == LTChargePrivilege.EMonthCardType.eSilver ? LTChargeManager.SilverMonthCardPath : LTChargeManager.GoldMonthCardPath;
    
            isHasMonthCard = mcType == LTChargePrivilege.EMonthCardType.eSilver ? LTChargeManager.Instance.IsSilverVIP() : LTChargeManager.Instance.IsGoldVIP();
    
            if (!isHasMonthCard)
            {
                remainDay = 0;
                isGetReward = false;
            }
            else
            {
                DataLookupsCache.Instance.SearchDataByID(string.Format("{0}.is_draw", path), out isGetReward);
    
                double expire_time = 0;
                DataLookupsCache.Instance.SearchDataByID(string.Format("{0}.expire_time", path), out expire_time);
                if ((int)expire_time > 0)
                {
                    int day = System.TimeSpan.FromSeconds(expire_time - EB.Time.Now - 1).Days;
                    remainDay = day > 0 ? day : 0;
                }
                else if ((int)expire_time == -1)
                {
                    remainDay = -1;
                }
            }
    
            string eType = mcType == LTChargePrivilege.EMonthCardType.eSilver ? "silver_month_card" : "gold_month_card";
    
            string str = mcType == LTChargePrivilege.EMonthCardType.eSilver ? "ID_CHARGE_SILVER_PRIVILEGE" : "ID_CHARGE_GOLD_PRIVILEGE";
            TipLabel.text = EB.Localizer.GetString(str);
            TipLabel.GetComponent<BoxCollider>().size = new Vector3(TipLabel.width, TipLabel.height, 0);
            TipLabel.GetComponent<BoxCollider>().center = new Vector3(0, -TipLabel.height/2, 0);
    
            TitleLab.text = curChargeData.longName;
    
            EverydayAward everydayAward = WelfareTemplateManager.Instance.GetEverydayAwardByType(eType);
            if (everydayAward != null && everydayAward.AwardItem != null)
            {
                everydayGotNum = everydayAward.AwardItem.count;
            }
        }
    
        private void RefreshUI()
        {
            string Compensate = string.Empty;
            if (!isHasMonthCard && curChargeData.payoutId == 2001)
            {
                var world = System.Array.Find(LoginManager.Instance.GameWorlds, w => w.Default);
                if (world.OpenTime > 0)
                {
                    int timeZone = Hotfix_LT.Data.ZoneTimeDiff.GetTimeZone();//需处理时区问题
                    int day = System.TimeSpan.FromSeconds(EB.Time.Now + timeZone * 3600).Days- System.TimeSpan.FromSeconds(world.OpenTime + timeZone * 3600).Days;
                    var reward = Hotfix_LT.Data.EventTemplateManager.Instance.GetDailyRewardByType("silver_month_card");
                    int hcNum = 0;
                    if (reward != null&&day>0)
                    {
                        for(int i=0;i< reward.ItemList.Count; i++)
                        {
                            if(reward.ItemList[i].id == "hc") hcNum+= reward.ItemList[i].count;
                        }
                    }
                    if(hcNum>0) Compensate =string.Format("[42fe79] + {0}[-]", day* hcNum);
                }
            }
    
            DiamondAllLab.text =curChargeData.value.ToString()+ Compensate;
            if(curChargeData.payoutId == 2002)
            {
                
                int drawCardNum = 0;
                for (int i = 0; i < curChargeData.redeemers.Count; i++)
                {
                    if (curChargeData.redeemers[i].Data.Equals("1062")) drawCardNum = curChargeData.redeemers[i].Quantity;
                }           
                if (drawCardNum > 0)
                {
                    DiamondAllLab.text = curChargeData.value.ToString() + "[42fe79] + [-]";
                    ShowDrawCard.CustomSetActive(true);
                    DrawCardNumLabel.text = "[42fe79]x"+drawCardNum.ToString()+"[-]";
                }
                else ShowDrawCard.CustomSetActive(false);
    
            }
            string str = string.Format(Localizer.GetString("ID_CHARGE_DAY"), 30);
    
            RedPoint.CustomSetActive(isHasMonthCard && !isGetReward);
            BtnBoxCol.enabled = true;
            BtnBg.color = Color.white;
            BtnBg.spriteName = "Ty_Button_3";
    
            if (isHasMonthCard)
            {
                remainTimeLab.text = remainDay==-1? Localizer.GetString("ID_DAY_PERMANENT") : string.Format(Localizer.GetString("ID_DAY_FORMAT"),"("+ remainDay,")");
                if (isGetReward)
                {
                    PriceLab.text = Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
                    BtnBoxCol.enabled = false;
                    BtnBg.color = Color.magenta;
                    BtnBg.spriteName = "Ty_Button_2";
                }
                else
                {
                    PriceLab.text = Localizer.GetString("ID_BUTTON_LABEL_PULL");
                }
    
            }
            else
            {
                remainTimeLab.text = string.Format("({0})", Localizer.GetString("ID_CHARGE_NONACTIVATED"));
                PriceLab.text = curChargeData.localizedCost;
            }
        }
    
        public void OnClickItem()
        {
            if (isHasMonthCard)
            {
                if (!isGetReward)
                {
                    string cType = mcType == LTChargePrivilege.EMonthCardType.eSilver ? "silver" : "gold";
                    LTChargeManager.Instance.ReceiveMonthCard(cType, delegate
                    {
                        isGetReward = true;
                        string path = mcType == LTChargePrivilege.EMonthCardType.eSilver ? LTChargeManager.SilverMonthCardPath : LTChargeManager.GoldMonthCardPath;
                        DataLookupsCache.Instance.CacheData(string.Format("{0}.is_draw", path), isGetReward);
                        PriceLab.text = Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
                        BtnBoxCol.enabled = false;
                        BtnBg.color = Color.magenta;
                        BtnBg.spriteName = "Ty_Button_2";
                        RedPoint.CustomSetActive(false);
                        string eType = mcType == LTChargePrivilege.EMonthCardType.eSilver ? "silver_month_card" : "gold_month_card";
                        EverydayAward everydayAward = WelfareTemplateManager.Instance.GetEverydayAwardByType(eType);
                        GlobalMenuManager.Instance.Open("LTShowRewardView", new List<LTShowItemData>() { everydayAward.AwardItem });
                    });
                }
                else
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, Localizer.GetString("ID_CHARGE_MONTHCAEDGET"));
                }
            }
            else
            {
                LTChargeManager.Instance.PurchaseOfferExpand(curChargeData, LTChargeStoreController.EventTable);
            }
        }
    }
}
