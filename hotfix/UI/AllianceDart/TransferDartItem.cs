using UnityEngine;
using System.Collections;
using Hotfix_LT.Data;


namespace Hotfix_LT.UI
{
    public class TransferDartItem : DynamicMonoHotfix
    {
        #region TransferDartController
        public static int GetResidueTransferDartNum()
        {
            int residue = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.EscortTimes) - AlliancesManager.Instance.DartData.HaveEscortNum;

            if (residue < 0)
            {
                EB.Debug.LogError("transferDart ResidueTranferNum <0");
                return 0;
            }

            return residue;
        }
        #endregion

        public LTShowItem RewardItem1, RewardItem2;
        public UILabel HcRewardLabel;
        private TransferDartMember mItemData;
        public override void Awake()
        {
            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                if (mDMono.ObjectParamList.Count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    RewardItem1 = (mDMono.ObjectParamList[0] as GameObject).GetMonoILRComponent<LTShowItem>();
                }
                if (mDMono.ObjectParamList.Count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    RewardItem2 = (mDMono.ObjectParamList[1] as GameObject).GetMonoILRComponent<LTShowItem>();
                }
                if (mDMono.ObjectParamList.Count > 2 && mDMono.ObjectParamList[2] != null)
                {
                    HcRewardLabel = (mDMono.ObjectParamList[2] as GameObject).GetComponent<UILabel>();
                }

            }
        }
        public void Fill(TransferDartMember itemData)
        {
            mItemData = itemData;

            RewardItem1.LTItemData = new LTShowItemData(itemData.Award[0].id, itemData.Award[0].count, itemData.Award[0].type,true);
            RewardItem2.LTItemData = new LTShowItemData(itemData.Award[1].id, itemData.Award[1].count, itemData.Award[1].type, true);
            if (HcRewardLabel != null)
            {
                int hcCount = 0;
                for (var i = 0; i < mItemData.Award.Length; i++)
                {
                    var r = mItemData.Award[i];

                    if (r.id.CompareTo("hc") == 0)
                    {
                        hcCount = r.count;
                    }
                }
                if (hcCount <= 0)
                {
                   EB.Debug.LogError(" Hc Count <= 0");
                }
                LTUIUtil.SetText(HcRewardLabel, hcCount.ToString());
            }
        }

        public void OnReceiveBtnClick()
        {
            switch (AlliancesManager.Instance.DartData.State)
            {
                case eAllianceDartCurrentState.None:
                case eAllianceDartCurrentState.Robing:
                case eAllianceDartCurrentState.Rob:
                    //次数足够
                    //非活动时间内接取运镖任务时提示：当前不在活动时间，不能接受运镖任务。
                    if (GetResidueTransferDartNum() > 0)
                    {
                        if (mItemData != null)
                        {
                            AlliancesManager.Instance.Accept(mItemData.Id, delegate (bool successful) { });
                            AlliancesManager.Instance.DartData.CurrentDartId = mItemData.Id;
                        }
                        else
                            EB.Debug.LogError("transferDart data = null");
                    }
                    else
                    {
                        MessageTemplateManager.ShowMessage(902072);
                    }
                    break;
                case eAllianceDartCurrentState.Transfer:
                case eAllianceDartCurrentState.Transfering:
                    //运镖期间不能再领取运镖任务
                    MessageTemplateManager.ShowMessage(902071);
                    break;
                    //case eAllianceDartCurrentState.Robing:
                    //case eAllianceDartCurrentState.Rob:
                    //	MessageTemplateManager.ShowMessage(902055);  //已劫掠过，不能接受运镖任务。
                    //	break;
            }
        }

        public void OnApplyHelpBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("ApplyHelpView", mItemData.Id);
        }

        public void OnStartBtnClick()
        {
            switch (AlliancesManager.Instance.DartData.State)
            {
                case eAllianceDartCurrentState.Transfering:
                    WorldMapPathManager.Instance.StartPathFindToNpc(MainLandLogic.GetInstance().CurrentSceneName, mItemData.TargetScene, mItemData.TargetNpc);
                    //TransferDartController.Instance.Close();
                    break;
                default:
                    AlliancesManager.Instance.Start(mItemData.Id, delegate (bool successful)
                    {
                        if (successful)
                        {
                            WorldMapPathManager.Instance.StartPathFindToNpc(MainLandLogic.GetInstance().CurrentSceneName, mItemData.TargetScene, mItemData.TargetNpc);
                            //TransferDartController.Instance.Close();
                        }
                        else
                        {
                            EB.Debug.LogError("start transfer dart fail");
                        }
                    });
                    break;
            }
        }
    }
}