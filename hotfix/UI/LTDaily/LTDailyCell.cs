using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTDailyCell : DynamicCellController<LTDailyData>
    {
        public UISprite BGSp;
        public UISprite IconSp;
        public UISprite TagSp;
        public UILabel NameLab;
        public UILabel OpenTimeLab;
        public UILabel LimitLab;
        private UISprite sprite;
        public UILabel TagLab;
        public GameObject SelectObj;
        public GameObject RPObj;
    
        private int activetyID;
        private LTDailyData curDailyData;
        private Hotfix_LT.Data.SpecialActivityTemplate curActData;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            BGSp = t.GetComponent<UISprite>("BG");
            IconSp = t.GetComponent<UISprite>("Icon");
            TagSp = t.GetComponent<UISprite>("TagSp");
            NameLab = t.GetComponent<UILabel>("Name");
            OpenTimeLab = t.GetComponent<UILabel>("OpenTime");
            LimitLab = t.GetComponent<UILabel>("Limit");
            sprite = t.GetComponent<UISprite>("Limit/Sprite");
            TagLab = t.GetComponent<UILabel>("TagSp/Tag");
            SelectObj = t.FindEx("SelectObj").gameObject;
            RPObj = t.FindEx("RedPoint").gameObject;

            var dailyController = t.parent.parent.parent.parent.parent.parent.GetUIControllerILRComponent<LTDailyHudController>();
            t.GetComponentEx<UIEventTrigger>().onClick.Add(new EventDelegate(() => dailyController.OnDailyItemClick(this)));
        }

        public override void Clean()
        {
    
        }
    
        public override void Fill(LTDailyData itemData)
        {
            if (itemData == null)
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }
            mDMono.gameObject.CustomSetActive(true);
            curDailyData = itemData;
            curActData = itemData.ActivityData;
            activetyID = curActData.id;
    
            InitItem();
        }
    
        private void InitItem()
        {
            NameLab.text = curActData.display_name;
            IconSp.spriteName = curActData.icon;
            BGSp.spriteName = DataIndex % 2 == 0 ? "Ty_Mail_Di1" : "Ty_Mail_Di2";
            SelectObj.CustomSetActive(curDailyData.IsSelected);
    
            SetTag();
            SetLimit();
            SetOpenTime();
        }
    
        private void SetTag()
        {
            if (curDailyData.DailyType == EDailyType.AllDay)
            {
                TagSp.gameObject.CustomSetActive(false);
            }
            else if (curDailyData.OpenTimeWeek.Contains(EB.Time.LocalWeek.ToString()) || curDailyData.OpenTimeWeek.Contains("*"))
            {
                if (curDailyData.OpenTimeValue < LTDailyDataManager.TimeNow && curDailyData.StopTimeValue > LTDailyDataManager.TimeNow)
                {
                    TagSp.gameObject.CustomSetActive(true);
                    TagSp.spriteName = "Ty_Login_Label_Red";
                    TagLab.text = EB.Localizer.GetString("ID_TASK_RUNNING");
                }
                else
                {
                    TagSp.gameObject.CustomSetActive(true);
                    TagSp.spriteName = "Ty_Login_Label_Green";
                    TagLab.text = EB.Localizer.GetString("ID_codefont_in_LTDailyCell_2276");
                }
            }
            else
            {
                TagSp.gameObject.CustomSetActive(false);
            }
        }
    
        private void SetLimit()
        {

            var func =  Data.FuncTemplateManager.Instance.GetFunc(curActData.funcId);
            if (func!=null && !func.IsConditionOK())
            {
                RPObj.CustomSetActive(false);
                LimitLab.gameObject.CustomSetActive(true);
                LimitLab.text = func.GetConditionStrShort();
                sprite.ResetAnchors();
                return;
            }
            int totalCount = curActData.times + LTDailyDataManager.Instance.GetVIPAdditionTimes(activetyID);
            if (totalCount <= 0)
            {
                RPObj.CustomSetActive(false);
                LimitLab.gameObject.CustomSetActive(false);
    
                if (curActData.id == 9013 && LTDailyDataManager.Instance.IsCouldReceiveVit())
                {
                    RPObj.CustomSetActive(true);
                }
                if (curActData.id == 9009 && LTDailyDataManager.Instance.GetActivityCount(9009) > 0)
                {
                    RPObj.CustomSetActive(true);
                }
                if (curActData.id == 9016 && LTDailyDataManager.Instance.GetActivityCount(9016) > 0)
                {
                    RPObj.CustomSetActive(true);
                }
                //极限试炼
                if (curActData.id == 9011 && LTDailyDataManager.Instance.GetActivityCount(9011) > 0)
                {
                    RPObj.CustomSetActive(true);
                }
                return;
            }
    
            LimitLab.gameObject.CustomSetActive(true);
            int count = LTDailyDataManager.Instance.GetActivityCount(activetyID);
            LimitLab.text = string.Format("{0}/{1}", count, totalCount);
            sprite.ResetAnchors();
            RPObj.CustomSetActive(count > 0);
        }
    
        private void SetOpenTime()
        {
            string colorStr = LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
    
            if (curDailyData.ActivityData.id == 9004&& !LTLegionWarManager.Instance.IsOpenWarTime())//����ս�����ж�
            {
                OpenTimeLab.text = string.Format("[{0}]{1}", colorStr, curDailyData.OpenTimeStr.Replace(EB.Localizer.GetString("ID_codefont_in_EventTemplateManager_43848"), EB.Localizer.GetString("ID_NEXT_WEEK")));
                return;
            }
    
            if ((curDailyData.OpenTimeWeek.Contains(EB.Time.LocalWeek.ToString()) || curDailyData.OpenTimeWeek.Contains("*")) &&
                ((curDailyData.OpenTimeValue < LTDailyDataManager.TimeNow && curDailyData.StopTimeValue > LTDailyDataManager.TimeNow) || curDailyData.OpenTimeValue <= 0))
            {
                colorStr = LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
            }
            OpenTimeLab.text = string.Format("[{0}]{1}", colorStr, curDailyData.OpenTimeStr);
        }
    
    
        public void SetSelectStatus(bool isSelected)
        {
            SelectObj.CustomSetActive(isSelected);
        }
    
        public LTDailyData GetDailyData()
        {
            return curDailyData;
        }
    }
}
