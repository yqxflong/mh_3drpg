using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{

    public class LTWelfareGrowUpTaskItem : DynamicMonoHotfix
    {

        private UILabel TitleLabel;
        private UIGrid ItemGrid;
        private List<LTShowItem> ItemList;
        private ConsecutiveClickCoolTrigger ReceiveBtn;

        private UILabel BtnLabel;
        private UILabel TipLabel;

        /// <summary>
        /// 密令标识
        /// </summary>
        private GameObject BattlePassObj;

        private LTWelfareGrowUpTaskData Data;
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            TitleLabel = t.GetComponent<UILabel>("Label");
            ItemList = new List<LTShowItem>();
            ItemGrid = t.GetComponent<UIGrid>("GiftGrid");
            LTShowItem[] items = ItemGrid.transform.GetMonoILRComponentsInChildren<LTShowItem>("Hotfix_LT.UI.LTShowItem");

            for (int i = 0; i < items.Length; i++)
            {
                ItemList.Add(items[i]);
            }

            BattlePassObj= t.FindEx("GiftGrid/LTShowItem (2)/TipObj").gameObject;
            ReceiveBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("GetButton");
            ReceiveBtn.clickEvent.Add(new EventDelegate(OnReceiveBtnClick));
            BtnLabel = t.GetComponent<UILabel>("GetButton/Label");
            TipLabel = t.GetComponent<UILabel>("TipLabel");
        }

        public void InitData(LTWelfareGrowUpTaskData data)
        {
            Data = data;
            if (data == null)
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }
            Hotfix_LT.Data.TaskTemplate TaskTpl = data.TaskTpl;
            TitleLabel.text = data.TaskTpl.target_tips;
            TitleLabel.transform.GetComponent<UISprite>("LevelSprite").UpdateAnchors();
            List<LTShowItemData> rewardDatas = TaskStaticData.GetItemRewardList(Data.TaskId);
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (Data.hasBattlePass && i == ItemList.Count - 1)
                {
                    List<LTShowItemData> battlePassList = Hotfix_LT.Data.TaskTemplateManager.Instance.GetBattlePassByTaskId(Data.TaskId);
                    var item = battlePassList[0];
                    ItemList[i].LTItemData = new LTShowItemData(item.id, item.count, item.type, false);
                    ItemList[i].mDMono.gameObject.CustomSetActive(true);
                    BattlePassObj.CustomSetActive(true);
                }
                else if (rewardDatas.Count > i)
                {
                    var item = rewardDatas[i];
                    ItemList[i].LTItemData = new LTShowItemData(item.id, item.count, item.type, false);
                    ItemList[i].mDMono.gameObject.CustomSetActive(true);
                }
                else
                {
                    ItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
            ItemGrid.Reposition();
            TipLabel.text = string.Format("{0}/{1}",data.Finished?("[42fe79]"+ data.TargetNum) : data.CurNum.ToString(),data .TargetNum);
            SetReceiveBtn();

        }

        private void SetReceiveBtn()
        {
            if (Data!=null&&Data.State != null)
            {
                if(!LTWelfareGrowUpController.DayJudge())//明日开放
                {
                    TipLabel.text =EB .Localizer .GetString ("ID_OPEN_TOMORROW");
                    ReceiveBtn.transform.GetComponent<UISprite>().color = new Color(1, 0, 1);
                    ReceiveBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_2";
                    ReceiveBtn.GetComponent <BoxCollider>().enabled = false;
                    if (BtnLabel != null) BtnLabel.text = EB.Localizer.GetString("ID_DIALOG_BUTTON_GO");
                    return;
                }
                if (Data.Finished)//完成任务
                {
                    if (Data.State.Equals(TaskSystem.COMPLETED))//已领取奖励
                    {
                        if (Data.hasBattlePass && !LTWelfareModel.Instance.HasBattlePass)//&&//有密令,但没买
                        {
                            ReceiveBtn.transform.GetComponent<UISprite>().color = new Color(1, 1, 1);
                            ReceiveBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_3";
                            ReceiveBtn.GetComponent<BoxCollider>().enabled = true;
                            if (BtnLabel != null) BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_BATTLE_PASS_BUY");
                        }
                        else if (Data.hasBattlePass &&!Data .hasGetBattlePassReward) //买了密令，但未领取
                        {
                            ReceiveBtn.transform.GetComponent<UISprite>().color = new Color(1, 1, 1);
                            ReceiveBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_3";
                            ReceiveBtn.GetComponent<BoxCollider>().enabled = true;
                            if (BtnLabel != null) BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_BATTLE_PASS_PULL");
                        }
                        else
                        {
                            ReceiveBtn.transform.GetComponent<UISprite>().color = new Color(1, 0, 1);
                            ReceiveBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_2";
                            ReceiveBtn.GetComponent<BoxCollider>().enabled = false;
                            if (BtnLabel != null) BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
                        }
                    }
                    else//可领取奖励
                    {
                        ReceiveBtn.transform.GetComponent<UISprite>().color = new Color(1, 1, 1);
                        ReceiveBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_3";
                        ReceiveBtn.GetComponent<BoxCollider>().enabled = true;
                        if (BtnLabel != null) BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
                    }
                }
                else//未完成任务
                {
                    ReceiveBtn.transform.GetComponent<UISprite>().color = new Color(1, 1, 1);
                    ReceiveBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_2";
                    ReceiveBtn.GetComponent<BoxCollider>().enabled = true;
                    if (BtnLabel != null) BtnLabel.text = EB.Localizer.GetString("ID_DIALOG_BUTTON_GO");
                }
            }
        }

        private bool sTaskOver = true;
        public void OnReceiveBtnClick()
        {
            if (Data.State.Equals(TaskSystem.COMPLETED))
            {
                OnBattlePassBtnClick();
            }
            else if (Data.Finished)
            {
                OnCompleteBtnClick();
            }
            else
            {
                OnGoBtnClick();
            }
        }

        private void OnBattlePassBtnClick()
        {
            if (!sTaskOver|| LTWelfareGrowUpController.ActivityOver)
            {
                return;
            }
            sTaskOver = false;
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);

            if (!LTWelfareModel.Instance.HasBattlePass)//需要购买密令
            {
                EB.IAP.Item GiftItem = null;
                EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(6001, out GiftItem);
                //GlobalMenuManager.Instance.Open("LTChargeStoreGiftUI", GiftItem);
                GlobalMenuManager.Instance.Open("LTBattlePassTipsView", GiftItem);
            }
            else//领奖
            {
                LTWelfareModel.Instance.RequestBattlePassComplete(Data.TaskId.ToString(), delegate (bool success) {
                    if (success)
                    {
                        if (LTWelfareEvent.WelfareGrowUpUpdata != null)
                        {
                            LTWelfareEvent.WelfareGrowUpUpdata();
                        }

                        {
                            var ht = Johny.HashtablePool.Claim();
                            ht.Add("0", TitleLabel.text);
                            MessageTemplateManager.ShowMessage(901036, ht, null);
                            Johny.HashtablePool.Release(ht);
                        }

                        System.Action callback = delegate ()
                        {
                            if (EB.Sparx.Hub.Instance.LevelRewardsManager.IsLevelUp)
                            {
                                LTMainHudManager.Instance.CheckLevelUp(delegate () {
                                    sTaskOver = true;
                                });
                            }
                            else
                                sTaskOver = true;
                        };
                        List<LTShowItemData> ItemDatas = Hotfix_LT.Data.TaskTemplateManager.Instance.GetBattlePassByTaskId(Data.TaskId);
                        for (int i = 0; i < ItemDatas.Count; i++)
                        {
                            if (ItemDatas[i].id == "hc")
                                FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, ItemDatas[i].count, "福利成长获得");
                            if (ItemDatas[i].id == "gold")
                                FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, ItemDatas[i].count, "福利成长获得");
                        }

                        {
                            var ht = Johny.HashtablePool.Claim();
                            ht.Add("reward", ItemDatas );
                            ht.Add("callback", callback );
                            GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
                        }
                    }
                    sTaskOver = true;
                });
            }
            sTaskOver = true;
        }

        private void OnCompleteBtnClick()
        {
            if (!sTaskOver)
            {
                return;
            }

            sTaskOver = false;
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            LTWelfareModel.Instance.RequestComplete(Data.TaskId.ToString(), delegate (bool success) {
                if (success)
                {
                    if (LTWelfareEvent.WelfareGrowUpUpdata != null)
                    {
                        LTWelfareEvent.WelfareGrowUpUpdata();
                    }

                    {
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("0", TitleLabel.text);
                        MessageTemplateManager.ShowMessage(901036, ht, null);
                        Johny.HashtablePool.Release(ht);
                    }
                  
                    System.Action callback = delegate ()
                    {
                        if (EB.Sparx.Hub.Instance.LevelRewardsManager.IsLevelUp)
                        {
                            LTMainHudManager.Instance.CheckLevelUp(delegate () {
                                sTaskOver = true;
                            });
                        }
                        else
                            sTaskOver = true;
                    };
                    List<LTShowItemData> ItemDatas = TaskStaticData.GetItemRewardList(Data.TaskId);
                    for (int i = 0; i < ItemDatas.Count; i++)
                    {
                        if (ItemDatas[i].id == "hc")
                            FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, ItemDatas[i].count, "福利成长目标");
                        if (ItemDatas[i].id == "gold")
                            FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, ItemDatas[i].count, "福利成长目标");
                    }

                    {
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("reward", ItemDatas);
                        ht.Add("callback", callback);
                        GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
                    }
                }
                sTaskOver = true;
            });
        }

        private void OnGoBtnClick()
        {
            TaskSystem.ProcessTaskRunning(Data.TaskId.ToString());
        }



        public void ResetItem()
        {
            InitData(LTWelfareModel.Instance.FindGrowUpDataById(Data.TaskId));
        }
    }
}
