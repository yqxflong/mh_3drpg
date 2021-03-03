using System;
using UnityEngine;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTUltimateTrialSelectRightCtrl : DynamicCellController<InfiniteChallengeTemplate>
    {
        private UILabel layerLabel;

        private LTShowItem[] mItemList;

        private UIButton sweepBtn;

        private UIButton gotoBtn;

        private GameObject lockBtnObj;

        private UIButton getBtn;

        private Transform mask;

        private Transform curTran;

        private int mLayer;

        private int mHeightestLayer;
        private UILabel mUILabelSweepVigor;

        private UILabel mUILabelGotoVigor;


        public override void Awake()
        {
            base.Awake();
            LTUltimateTrialDataManager.Instance.OnResetTimesLabel += new Action(RefreshView);
            
            sweepBtn = mDMono.transform.Find("SweepBtn").GetComponent<UIButton>();
            sweepBtn.onClick.Add(new EventDelegate(OnSweepClick));
            mUILabelSweepVigor = sweepBtn.transform.Find("Sprite/New").GetComponent<UILabel>();
            gotoBtn = mDMono.transform.Find("GotoBtn").GetComponent<UIButton>();
            gotoBtn.onClick.Add(new EventDelegate(OnGotoBtnClick));
            mUILabelGotoVigor = gotoBtn.transform.Find("Sprite/New").GetComponent<UILabel>();
            lockBtnObj = mDMono.transform.Find("LockBtn").gameObject;

            getBtn = mDMono.transform.Find("GetBtn").GetComponent<UIButton>();
            getBtn.onClick.Add(new EventDelegate(OnGotoBtnClick));

            layerLabel = mDMono.transform.Find("LevelLabel").GetComponent<UILabel>();
            mItemList = mDMono.transform.Find("ItemList").GetMonoILRComponentsInChildren<LTShowItem>("Hotfix_LT.UI.LTShowItem");
            mask = mDMono.transform.Find("Mask");
            curTran= mDMono.transform.Find("Cur");
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
            if (LTUltimateTrialDataManager.Instance.OnResetTimesLabel != null)
                LTUltimateTrialDataManager.Instance.OnResetTimesLabel -= new Action(RefreshView);
        }
        
        public void RefreshView()
        {
            int dayDisCountTime = 0;
            int oldVigor = 0;
            int NewVigor = 0;
            NewGameConfigTemplateManager.Instance.GetEnterVigor(eBattleType.InfiniteChallenge, out dayDisCountTime, out NewVigor, out oldVigor);
            int curDisCountTime = dayDisCountTime - LTUltimateTrialDataManager.Instance.GetCurrencyTimes();
            int EnterVigor = curDisCountTime > 0 ? NewVigor : oldVigor;

            mUILabelSweepVigor.text = mUILabelSweepVigor.transform.GetChild(0).GetComponent<UILabel>().text = EnterVigor.ToString();
            mUILabelGotoVigor.text = mUILabelGotoVigor.transform.GetChild(0).GetComponent<UILabel>().text = EnterVigor.ToString();
        }

        public override void Clean()
        {
            mLayer = 0;
            mDMono.gameObject.CustomSetActive(false);
        }

        public override void Fill(InfiniteChallengeTemplate tpl)
        {
            if (tpl == null)
            {
                Clean();
                return;
            }
            mLayer = tpl.layer;
            mHeightestLayer = LTUltimateTrialDataManager.Instance.GetCurLayer();
            RefreshView();
            
            bool topLayer = LTUltimateTrialDataManager.Instance.IsTopLayer(mLayer);
            bool overTopLayer = LTUltimateTrialDataManager.Instance.IsOverMaxLayer();

            if (mLayer < mHeightestLayer || (mLayer == mHeightestLayer && LTUltimateTrialDataManager.Instance.IsLayerComplete() && LTUltimateTrialDataManager.Instance.IsGetReward()))
            {
                InitReward(tpl.award, topLayer && overTopLayer);
            }
            else
            {
                if (topLayer && overTopLayer)   //通关最后一层后就代表领取了奖励,历史原因，逻辑如此
                {
                    InitReward(tpl.award, topLayer && overTopLayer);
                }
                else
                {
                    InitReward(tpl.first_award, topLayer && overTopLayer);
                }
            }

            layerLabel.text = string.Format(EB.Localizer.GetString("ID_LEVEL_TEXT_FORMAT"), (EventTemplateManager.Instance.CalculCurLayer(tpl)).ToString());

            if (topLayer && overTopLayer)   
            {
                sweepBtn.gameObject.CustomSetActive(true);
                gotoBtn.gameObject.CustomSetActive(false);
            }
            else
            {
                sweepBtn.gameObject.CustomSetActive(mLayer < mHeightestLayer || (mLayer == mHeightestLayer &&
                    LTUltimateTrialDataManager.Instance.IsLayerComplete() && LTUltimateTrialDataManager.Instance.IsGetReward()));
                gotoBtn.gameObject.CustomSetActive(mLayer == mHeightestLayer && !LTUltimateTrialDataManager.Instance.IsLayerComplete());
            }

            lockBtnObj.CustomSetActive(mLayer > mHeightestLayer);

            getBtn.gameObject.CustomSetActive(mLayer == mHeightestLayer && LTUltimateTrialDataManager.Instance.IsLayerComplete() && !LTUltimateTrialDataManager.Instance.IsGetReward());

            mask.gameObject.CustomSetActive(mLayer > mHeightestLayer);

            curTran.gameObject.CustomSetActive(mLayer == mHeightestLayer);
            
            mDMono.gameObject.CustomSetActive(true);
        }

        private void InitReward(List<LTShowItemData> dataList, bool overLastLayer) //通关最后一层后就代表领取了奖励,历史原因，逻辑如此
        {
            for (int i = 0; i < mItemList.Length; i++)
            {
                if (i < dataList.Count)
                {
                    mItemList[i].mDMono.gameObject.CustomSetActive(true);
                    if (overLastLayer)
                    {
                        mItemList[i].mDMono.transform.Find("First").gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        mItemList[i].mDMono.transform.Find("First").gameObject.CustomSetActive(mLayer == mHeightestLayer && !LTUltimateTrialDataManager.Instance.IsGetReward());
                    }
                    mItemList[i].LTItemData = dataList[i];
                }
                else
                {
                    mItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
        }

       private void OnSweepClick()
        {
            int dayDisCountTime = 0;
            int oldVigor = 0;
            int NewVigor = 0;
            NewGameConfigTemplateManager.Instance.GetEnterVigor(eBattleType.InfiniteChallenge, out dayDisCountTime, out NewVigor, out oldVigor);
            int curDisCountTime = dayDisCountTime - LTUltimateTrialDataManager.Instance.GetCurrencyTimes();
            int EnterVigor = curDisCountTime > 0 ? NewVigor : oldVigor;
            if (BalanceResourceUtil.EnterVigorCheck(EnterVigor))
            {
                return;
            }

            FusionAudio.PostEvent("UI/General/ButtonClick");

            bool topLayer = LTUltimateTrialDataManager.Instance.IsTopLayer(mLayer);  //最后一关特殊处理
            bool overTopLayer = LTUltimateTrialDataManager.Instance.IsOverMaxLayer();

            if (mLayer < mHeightestLayer 
                || (mLayer == mHeightestLayer && LTUltimateTrialDataManager.Instance.IsLayerComplete() 
                && LTUltimateTrialDataManager.Instance.IsGetReward())
                || (topLayer && overTopLayer))
            {
                LTUltimateTrialDataManager.Instance.RequestSweepByIndex(mLayer,delegate 
                {
                    if (LTUltimateTrialDataManager.Instance.OnResetTimesLabel != null)
                    {
                        LTUltimateTrialDataManager.Instance.OnResetTimesLabel();
                    }
                    List<LTShowItemData>  list = LTUltimateTrialDataManager.Instance.GetSweepReward();
                    if (list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].id == "hc")
                                FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, list[i].count, "极限试炼获得");
                            if (list[i].id == "gold")
                                FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, list[i].count, "极限试炼获得");
                        }
                        GlobalMenuManager.Instance.Open("LTShowRewardView", list);
                    }
                    if(curDisCountTime > 0)
                    {
                        LTDailyDataManager.Instance.SetDailyDataRefreshState();
                    }

                });
            }
        }


        private void OnGotoBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (LTUltimateTrialDataManager.Instance.OnGotoBtnClick != null)
            {
                LTUltimateTrialDataManager.Instance.OnGotoBtnClick();
            }
        }
    }
}
