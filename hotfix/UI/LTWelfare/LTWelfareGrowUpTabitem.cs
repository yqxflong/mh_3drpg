using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTWelfareGrowUpTabitem : DynamicMonoHotfix
    {
        private UIButton DayBtn;
        private UISprite BGSprite;
        private UILabel DayLabel;
        private GameObject LockObj;
        private GameObject RedPointObj;
        private GameObject PrefectObj;

        private int index=-1;
        private bool isLock = false;

        public override void Awake()
        {
            base.Awake();
            DayBtn = mDMono.transform.GetComponent <UIButton>();
            DayBtn.onClick.Add(new EventDelegate(OnTabClick));
            BGSprite = mDMono.transform.Find("BG").GetComponent<UISprite>();
            DayLabel = mDMono.transform.Find("DayLabel").GetComponent<UILabel>();
            LockObj = mDMono.transform.Find("LockObj").gameObject;
            RedPointObj = mDMono.transform.Find("RedPoint").gameObject;
            PrefectObj= mDMono.transform.Find("Prefect").gameObject;
            LTWelfareEvent.WelfareGrowUpReset += InitData;
        }

        public override void OnDestroy()
        {
            LTWelfareEvent.WelfareGrowUpReset -= InitData;
            base.OnDestroy();
        }

        public void InitData(int i)
        {
            index = i;
            InitData();
        }
        private void InitData()
        {
            isLock = index > LTWelfareGrowUpController.GrowUpDayNum;
            DayLabel.text = string.Format(EB.Localizer.GetString("ID_DAY"), index + 1);
            LockObj.CustomSetActive(isLock);
            SetStates();
        }

        public void SetStates()
        {
            RedPointObj.CustomSetActive(index< LTWelfareGrowUpController.GrowUpDayNum && LTWelfareModel.Instance.DayTabRPList.Contains(index));
            PrefectObj.CustomSetActive(!LTWelfareModel.Instance.DayTabUnPrefectList.Contains(index));
        }

        public void SetSelectColor(bool isSelect = false)
        {
            BGSprite.color = isSelect ? new Color(1, 1, 1) : new Color(1, 0, 1);
        }

        private void OnTabClick()
        {
            if (isLock)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB.Localizer.GetString("ID_TARGET_LOCKED"));
                return;
            }
            if(LTWelfareEvent.WelfareGrowUpTabClick != null)
            {
                LTWelfareEvent.WelfareGrowUpTabClick(index);
            }
        }
    }
}
