using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTAwakeningInstanceTypeItem : DynamicMonoHotfix
    {
        //开启时间段
        public UILabel OpenTimeLabel;
        public UISprite MainTexture;
        //锁
        public GameObject LockObj;
        //觉醒副本种类
        public Hotfix_LT.Data.eRoleAttr AwakeningInstanceType;
        private GameObject fx;
        public bool isLock;

        private UITweener[] ts;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            OpenTimeLabel = t.GetComponent<UILabel>("Desc/Time");
            MainTexture = t.GetComponent<UISprite>("MainIcon");
            LockObj = t.FindEx("Lock").gameObject;
            isLock = false;
            fx = t.Find("fx").gameObject;

            AwakeningInstanceType = (Hotfix_LT.Data.eRoleAttr)mDMono.IntParamList[0];
            t.GetComponent<UIButton>("MainIcon").onClick.Add(new EventDelegate(OnBtnClick));
        }
    
        public void UpdateUI()
        {
            isLock = SetLock();
            if (isLock)
            {
                OpenTimeLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor;
                LockObj.CustomSetActive(true);
                fx.SetActive(false);
            }
            else
            {
                OpenTimeLabel.color = LT.Hotfix.Utility.ColorUtility.GreenColor;
                LockObj.CustomSetActive(false);
                fx.SetActive(true);
            }
    
            if (ts == null)
            {
                ts = mDMono.transform.GetComponents<UITweener>();
            }

            if (ts != null)
            {
                for (var i = 0; i < ts.Length; i++)
                {
                    ts[i].PlayForward();
                }
            }
        }
    
        public bool SetLock()
        {
            //判断当日是否开启
            DateTime datetime = TaskSystem.TimeSpanToDateTime(EB.Time.Now);
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            bool isLock = true;
    
            switch (AwakeningInstanceType)
            {
                case Hotfix_LT.Data.eRoleAttr.Feng:
                    string goodsStr = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("WindOpenWeek");
                    string[] tempGoodsStrs = goodsStr.Split(',');
                    isLock = !ContainWeek(tempGoodsStrs,weeknow);
                    OpenTimeLabel.text = string.Format(EB.Localizer.GetString("ID_EVERY_WANTONLY_DAY_OPEN"), ConvertDay(tempGoodsStrs));
                    break;
                case Hotfix_LT.Data.eRoleAttr.Huo:
                    string goodsStrH = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("FireOpenWeek");
                    string[] tempGoodsStrsH = goodsStrH.Split(',');
                    isLock = !ContainWeek(tempGoodsStrsH, weeknow);
                    OpenTimeLabel.text = string.Format(EB.Localizer.GetString("ID_EVERY_WANTONLY_DAY_OPEN"), ConvertDay(tempGoodsStrsH));
                    break;
                case Hotfix_LT.Data.eRoleAttr.Shui:
                    string goodsStrS = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("WaterOpenWeek");
                    string[] tempGoodsStrsS = goodsStrS.Split(',');
                    isLock = !ContainWeek(tempGoodsStrsS, weeknow);
                    OpenTimeLabel.text = string.Format(EB.Localizer.GetString("ID_EVERY_WANTONLY_DAY_OPEN"), ConvertDay(tempGoodsStrsS));
                    break;
            }
            return isLock;
        }
    
    
        public bool ContainWeek(string[] arrays,int day)
        {
            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i] == day.ToString())
                {
                    return true;
                }
            }
            return false;
        }
    
        public string  ConvertDay(string[] arrays)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < arrays.Length; i++)
            {
                stringBuilder.Append(EB.Localizer.GetString(string.Format("ID_WEEK_{0}", arrays[i])));
                stringBuilder.Append(EB.Localizer.GetString("、"));
            }
            return stringBuilder.ToString().Substring(0, stringBuilder.ToString().Length-1);
        }
    
        public void ResetUI()
        {
            if (ts != null)
            {
                for (var i = 0; i < ts.Length; i++)
                {
                    var t = ts[i];
                    t.tweenFactor = 0;
                    t.ResetToBeginning();
                }
            }
        }
    
        public void OnBtnClick()
        {
            if (isLock)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB .Localizer .GetString("ID_uifont_in_LTLegionMainMenu_Label_16"));
            }
            else
            {
                GlobalMenuManager.Instance.Open("LTAwakeningInstanceSelectHud", AwakeningInstanceType);
            }
        }
    }
}
