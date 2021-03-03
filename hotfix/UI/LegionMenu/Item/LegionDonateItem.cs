using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionDonateItem : DynamicMonoHotfix
    {
        public UILabel name;
        public UILabel expNumLabel;
        public UILabel legionCoinNumLabel,legionMedalNumLabel;
        public UILabel donateNumLabel;
        private UILabel candonateTimes, candonateTimesShadow;
        public UISprite donateCoinIcon;

        public UIButton donateBtn;

        public GameObject effect;

        public Action donateActoin;

        private float time;
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            name = mDMono.transform.Find("Name").GetComponent<UILabel>();
            expNumLabel = mDMono.transform.Find("Exp/Num").GetComponent<UILabel>();
            legionCoinNumLabel = mDMono.transform.Find("LegionCoin/Num").GetComponent<UILabel>();
            legionMedalNumLabel = mDMono.transform.Find("LegionMedal/Num").GetComponent<UILabel>();
            donateNumLabel = mDMono.transform.Find("DonateBtn/Sprite/CoinNum").GetComponent<UILabel>();
            candonateTimes = t.GetComponent<UILabel>("DonateBtn/Sprite/Label");
            candonateTimesShadow = t.GetComponent<UILabel>("DonateBtn/Sprite/Label/Label(Clone)");
            donateCoinIcon = mDMono.transform.Find("DonateBtn/Sprite/CoinIcon").GetComponent<UISprite>();
            donateBtn = mDMono.transform.Find("DonateBtn").GetComponent<UIButton>();
            effect = mDMono.transform.Find("Effect").gameObject;
            if (donateBtn != null) donateBtn.onClick.Add(new EventDelegate(OnDonateBtn));
        }


        public override void OnDestroy()
        {
            if (donateBtn != null) donateBtn.onClick.Clear();
        }
        string colorstr;
        private int typeindex = -1, costperTime;
        public void ShowUI(LegionPageDonate.DonateType type, int coinNum, int exp, int LegionCoinNum)
        {
            costperTime = coinNum;
            typeindex = (int)type;
            SetColorShow();            
            LTUIUtil.SetText(expNumLabel, exp.ToString());
            LTUIUtil.SetText(legionCoinNumLabel, LegionCoinNum.ToString());
            LTUIUtil.SetText(legionMedalNumLabel, LegionCoinNum.ToString());
        }

        public void RefreshData(int useTimes,int totalTimes)
        {
            string colorStr = useTimes >= totalTimes ? "[ff6699]" : "[42fe79]";
            candonateTimes.text = candonateTimesShadow.text = string.Format("{0}{1}/{2}[-]", colorStr, totalTimes - useTimes, totalTimes);
            int times = Data.VIPTemplateManager.Instance.GetTotalNum(Data.VIPPrivilegeKey.FirstDonateDouble);
            ShowEffect(useTimes == 0&& times>0);
            SetColorShow();
        }
        private void SetColorShow()
        {
            switch (typeindex)
            {
                case -1:return;
                case 2:
                    colorstr = BalanceResourceUtil.GetUserGold() >= costperTime ? "[ffffff]" : "[ff6699]";
                    break;
                default:
                    colorstr = BalanceResourceUtil.GetUserDiamond() >= costperTime ? "[ffffff]" : "[ff6699]";
                    break;
            }
            LTUIUtil.SetText(donateNumLabel, string.Format("{0}{1}[-]",colorstr,costperTime));
        }
        private void ShowEffect(bool isShow)
        {
            if (effect != null)
            {
                effect.SetActive(isShow);
            }
        }

        private bool IsRepeatClick
        {
            get
            {
                if (Time.time - time < 1)
                {
                    return true;
                }
                time = Time.time;
                return false;
            }
        }

        public void OnDonateBtn()
        {
            if (IsRepeatClick) return;
            if (donateActoin != null)
            {
                donateActoin();
            }
        }
    }
}
