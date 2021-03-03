using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTNoticeGuideController : DataLookupHotfix
    {
        public override void Awake()
        {
            base.Awake();

            if (mDL.ObjectParamList != null)
            {
                var count = mDL.ObjectParamList.Count;

                if (count > 0 && mDL.ObjectParamList[0] != null)
                {
                    Icon = ((GameObject)mDL.ObjectParamList[0]).GetComponentEx<UISprite>();
                }
                if (count > 1 && mDL.ObjectParamList[1] != null)
                {
                    LevelLabel = ((GameObject)mDL.ObjectParamList[1]).GetComponentEx<UILabel>();
                }
                if (count > 2 && mDL.ObjectParamList[2] != null)
                {
                    UiProgressBar = ((GameObject)mDL.ObjectParamList[2]).GetComponentEx<UIProgressBar>();
                }
                if (count > 3 && mDL.ObjectParamList[3] != null)
                {
                    NameLabel = ((GameObject)mDL.ObjectParamList[3]).GetComponentEx<UILabel>();
                }
                if (count > 4 && mDL.ObjectParamList[4] != null)
                {
                    ProgressBarLabel = ((GameObject)mDL.ObjectParamList[4]).GetComponentEx<UILabel>();
                }
                if (count > 5 && mDL.ObjectParamList[5] != null)
                {
                    SpeakTween = ((GameObject)mDL.ObjectParamList[5]).GetComponentEx<UITweener>();
                }
                if (count > 6 && mDL.ObjectParamList[6] != null)
                {
                    DescLabel = ((GameObject)mDL.ObjectParamList[6]).GetComponentEx<UILabel>();
                }
                if (count > 7 && mDL.ObjectParamList[7] != null)
                {
                    ((GameObject)mDL.ObjectParamList[7]).GetComponentEx<UIButton>().onClick.Add(new EventDelegate(Click));
                }
                if (count > 8 && mDL.ObjectParamList[8] != null)
                {
                    ((GameObject)mDL.ObjectParamList[8]).GetComponentEx<UIButton>().onClick.Add(new EventDelegate(Click));
                }
                if (count > 9 && mDL.ObjectParamList[9] != null)
                {
                    ((GameObject)mDL.ObjectParamList[9]).GetComponentEx<UIButton>().onClick.Add(new EventDelegate(CloseSpeakBg));
                }
            }
            DescLabel.transform.parent.gameObject.SetActive(false);
        }
        public override void OnEnable()
        {
            SetDataId();
        }

        private void SetDataId()
        {
            mNoticeFuncTbl = Data.FuncTemplateManager.Instance.RefreshNoticeList();
            if (mNoticeFuncTbl == null || mNoticeFuncTbl.Count <= 0)
            {
                mDL.transform.gameObject.CustomSetActive(false);
                return;
            }
            curFuncTemplate = mNoticeFuncTbl[0];
            mDL.DataIDList.Clear();
            if (curFuncTemplate.condition.Contains("m-"))
            {
                mDL.DataIDList.Add("userCampaignStatus.normalChapters");
            }
            else
            {
                mDL.DataIDList.Add("level");
            }
        }

        public override void OnDisable()
        {
            mDL.DataIDList.Clear();
        }
        public UISprite Icon;
        public UILabel LevelLabel;
        public UIProgressBar UiProgressBar;
        public UILabel NameLabel;
        public UILabel ProgressBarLabel;
        public UITweener SpeakTween;
        public UILabel DescLabel;
        private List<Hotfix_LT.Data.FuncTemplate> mNoticeFuncTbl;
        private int currentLevel, mainFBprogress;
        private Hotfix_LT.Data.FuncTemplate curFuncTemplate;
        private object olddata;

        public void UpdateUI()
        {
            mNoticeFuncTbl = Data.FuncTemplateManager.Instance.RefreshNoticeList();
            if (mNoticeFuncTbl.Count > 1)
            {
                curFuncTemplate = mNoticeFuncTbl[0];
                int curNum = 0;
                int TargetNum = 0;
                if (curFuncTemplate.openType == Data.FuncOpenType.level)
                {
                    int level = curFuncTemplate.conditionParam;
                    if (level > 0 && level < 99)
                    {
                        curNum = currentLevel;
                        TargetNum = level;
                        LTUIUtil.SetText(LevelLabel, curFuncTemplate.GetConditionStrShort());
                        if (curNum >= TargetNum)
                        {
                            SetDataId();
                        }
                    }
                }
                else if (curFuncTemplate.openType == Data.FuncOpenType.maincampaign)
                {
                    int t_camid = curFuncTemplate.conditionParam;
                    var t_targetcampaign = Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(t_camid.ToString());
                    LTUIUtil.SetText(LevelLabel, string.Format(EB.Localizer.GetString("ID_FUNC_OPENTIP_2"), t_targetcampaign.Name));
                    //int maxCap = LTInstanceUtil.GetCurMaxCompleteCampaignId(olddata as Hashtable);
                    //var t_currcampaign = Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(maxCap.ToString());
                    //TargetNum = t_targetcampaign.totalcampaign;
                    //curNum = t_currcampaign == null?0:t_currcampaign.totalcampaign;
                }

                //UiProgressBar.value = (float)curNum / TargetNum;
                //ProgressBarLabel.text = string.Format("{0}/{1}", curNum, TargetNum);
                LTUIUtil.SetText(NameLabel, curFuncTemplate.display_name);
                Icon.spriteName = curFuncTemplate.iconName;
                DescLabel.text = curFuncTemplate.discript;
                return;
            }
            mDL.gameObject.SetActive(false);
        }

        private int IntToLong(int num)
        {
            if (num <= 0) return 0;
            return num;
        }
        //等级改变
        public override void OnLookupUpdate(string dataID, object value)
        {
            if (value == null)
            {
                return;
            }

            if (dataID.Equals("level") && int.TryParse(value.ToString(), out int temp) && temp != currentLevel)
            {
                currentLevel = temp;
                UpdateUI();
            }
            else if (dataID.Equals("userCampaignStatus.normalChapters") && !value.Equals(olddata))
            {
                olddata = value;
                UpdateUI();
            }
            if (dataID.Equals("userCampaignStatus.normalChapters"))
            {
                LTInstanceUtil.GetCurMaxCompleteCampaignId(value as Hashtable);
            }
        }

        public void Click()
        {
            DescLabel.transform.parent.gameObject.SetActive(true);
            SpeakTween.PlayForward();

            //Invoke("CloseSpeakBg",2);
        }


        public void CloseSpeakBg()
        {
            SpeakTween.PlayReverse();
        }
    }
}
