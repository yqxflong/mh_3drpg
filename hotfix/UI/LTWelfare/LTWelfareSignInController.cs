using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTWelfareSignInController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return false; } }
        public override bool IsFullscreen() { return false; }

        private GameObject SignItem;
        private UIGrid SignItemGrid;

        //private UILabel MonthLabel;
        private UILabel SignTimesLabel;
        private UILabel SignChanceLabel;
        //private UILabel BottomTipLabel;
        private UIScrollView m_ScrollView;

        //private UIButton RuleBtn;

        private bool m_HaveCreate = false;
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            SignItem = t.FindEx("Content/Item").gameObject;
            SignItemGrid = t.GetComponent<UIGrid>("Content/ScrollView/Placeholder/Grid");
            SignTimesLabel = t.GetComponent<UILabel>("BG/SignTimesLabel");
            SignChanceLabel = t.GetComponent<UILabel>("BG/SignChanceLabel");
            m_ScrollView = t.GetComponent<UIScrollView>("Content/ScrollView");

            SignItemList = new List<LTWelfareSignInItem>();
        }


        public override IEnumerator OnAddToStack()
        {
            FusionAudio.PostEvent("UI/New/DiTu", true);
            yield return base.OnAddToStack();
            yield return null;
            InitData();
            LTWelfareEvent.WelfareSignInUpdata += UpdataData;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            LTWelfareEvent.WelfareSignInUpdata -= UpdataData;
            //SignItemList.Clear();
            yield return new WaitUntil(() => m_HaveCreate);
            StopCoroutine(InitItems());
            DestroySelf();
            yield break;
        }


        public override void OnCancelButtonClick()
        {
            if (!m_HaveCreate)
            {
                return;
            }
            base.OnCancelButtonClick();
        }
        public void InitData()
        {
            if (AutoRefreshingManager.Instance.GetRefreshed(AutoRefreshingManager.RefreshName.Signin))
            {
                LTWelfareDataManager.Instance.SignInData.IsSigned = false;
            }

            SignTimesLabel.text = string.Format(EB.Localizer.GetString("ID_WELFARE_SIGNIN_TIMES"), LTWelfareDataManager.Instance.SignInData.Num, EB.Time.LocalDaysInMonth);
            SignChanceLabel.text = string.Format(EB.Localizer.GetString("ID_WELFARE_SIGNIN_CHANCE"), (ResidueResigninTimes));

            StartCoroutine(InitItems());
        }

        public void UpdataData()
        {
            SignTimesLabel.text = string.Format(EB.Localizer.GetString("ID_WELFARE_SIGNIN_TIMES"), LTWelfareDataManager.Instance.SignInData.Num, EB.Time.LocalDaysInMonth);
            SignChanceLabel.text = string.Format(EB.Localizer.GetString("ID_WELFARE_SIGNIN_CHANCE"), (ResidueResigninTimes));
            int have_num = LTWelfareDataManager.Instance.SignInData.Num;
            int monthDay = EB.Time.LocalDaysInMonth;

            for (int i = 0; i < SignItemList.Count; i++)
            {
                if (i < have_num)
                {
                    SignItemList[i].UpdateReceiveState(eReceiveState.have);
                }
            }

            if (ResidueRealResigninDays > 1)
            {
                int index = have_num + Mathf.Min(ResidueRealResigninDays - 1, LTWelfareDataManager.Instance.SignInData.IsSigned ? 0 : 1);

                if (SignItemList.Count > index)
                {
                    SignItemList[index].UpdateReceiveState(eReceiveState.can, true);
                }
            }

            if (LTWelfareDataManager.Instance.GetIsHaveSigninAward())
            {
                LTWelfareSignInItem curItem = GetCurrent();

                if (curItem != null)
                {
                    curItem.UpdateReceiveState(eReceiveState.can);//这里设置可签到特效
                }
            }
        }

        List<LTWelfareSignInItem> SignItemList;
        private IEnumerator InitItems()
        {
            yield return null;
            int have_num = LTWelfareDataManager.Instance.SignInData.Num;
            int monthDay = EB.Time.LocalDaysInMonth;

            SetPlaceHolder(monthDay);
            int curNum = LTWelfareDataManager.Instance.SignInData.Num;
            int activeCount = (int)(m_ScrollView.GetComponent<UIPanel>().baseClipRegion.w / SignItemGrid.cellHeight);
            float value = (float)(((curNum + SignItemGrid.maxPerLine - 1) / SignItemGrid.maxPerLine) - activeCount) / (float)(((monthDay + SignItemGrid.maxPerLine - 1) / SignItemGrid.maxPerLine) - activeCount);
            float scrollValue = curNum > activeCount ? value : 0f;
            m_ScrollView.UpdatePosition();
            m_ScrollView.UpdateScrollbars();
            m_ScrollView.verticalScrollBar.value = scrollValue;

            if (!m_HaveCreate)
            {               
                for (int i = 0; i < SignItemList.Count; i++)
                {
                    GameObject.Destroy(SignItemList[i].mDMono.gameObject);
                }

                SignItemList.Clear();
                List<SigninAward> monthAwards = WelfareTemplateManager.Instance.GetTemplate(EB.Time.LocalMonth);

                for (int i = 0; i < monthDay; i++)
                {
                    if (SignItem == null)
                    {
                        break;
                    }

                    GameObject obj = GameObject.Instantiate(SignItem);
                    obj.SetActive(true);
                    obj.transform.SetParent(SignItemGrid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = new Vector3(1000, 0, 0);
                    LTWelfareSignInItem Ctrl = obj.GetMonoILRComponent<LTWelfareSignInItem>();
                    Ctrl.InitData(monthAwards[i], i + 1);

                    if (i < have_num)
                    {
                        Ctrl.UpdateReceiveState(eReceiveState.have);
                    }
                    else if (i > have_num && i <= have_num + Mathf.Min(ResidueRealResigninDays - 1, LTWelfareDataManager.Instance.SignInData.IsSigned ? 0 : 1))
                    {
                        Ctrl.UpdateReceiveState(eReceiveState.can, true);
                    }
                    
                    SignItemList.Add(Ctrl);

                    if ((i + 1) % SignItemGrid.maxPerLine == 0 || i == monthDay - 1)
                    {
                        SignItemGrid.Reposition();
                        yield return new WaitForSeconds(0.2f);
                    }
                }
                m_HaveCreate = true;
                EB.Debug.Log("Create SignItem Finished!!!!!!!");
            }

            if (LTWelfareDataManager.Instance.GetIsHaveSigninAward() && SignItem != null)
            {
                LTWelfareSignInItem curItem = GetCurrent();
                if (curItem != null) curItem.UpdateReceiveState(eReceiveState.can);//这里设置可签到特效
            }

        }

        private void SetPlaceHolder(int monthDay)
        {
            int row = Mathf.CeilToInt(monthDay / (float)SignItemGrid.maxPerLine);
            int height = (int)SignItemGrid.cellHeight * row;
            SignItemGrid.transform.parent.GetComponent<UIWidget>().height = height;

            SignItemGrid.Reposition();
        }

        private LTWelfareSignInItem GetCurrent()
        {
            int num = LTWelfareDataManager.Instance.SignInData.Num;

            if (SignItemList.Count > LTWelfareDataManager.Instance.SignInData.Num)
            {
                return SignItemList[num];
            }
            else
            {
                return null;
            }
        }

        static public bool IsCanResignin
        {
            get { return ResidueCanResigninDays > 0 && ResidueResigninTimes > 0; }
        }

        static public int ResidueRealResigninDays
        {
            get { return Mathf.Min(ResidueCanResigninDays, ResidueResigninTimes); }
        }

        static public int ResidueCanResigninDays
        {
            get
            {
                if (LTWelfareDataManager.Instance.SignInData.IsSigned)
                {
                    return EB.Time.LocalDay - LTWelfareDataManager.Instance.SignInData.Num;
                }
                else
                {
                    return EB.Time.LocalDay - LTWelfareDataManager.Instance.SignInData.Num - 1;
                }
            }
        }

        static public int ResidueResigninTimes
        {
            get
            {
                return VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ResignInTimes) - LTWelfareDataManager.Instance.SignInData.HaveResigninNum;
            }
        }

        public void OnRuleBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString("ID_RULE_SINGIN"));
        }
    }
}
