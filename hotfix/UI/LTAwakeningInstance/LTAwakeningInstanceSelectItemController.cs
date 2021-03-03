using System.Collections.Generic;
using EB;
using Hotfix_LT.Data;
using UnityEngine;


namespace Hotfix_LT.UI
{
    public class LTAwakeningInstanceSelectItemController : DynamicCellController<Hotfix_LT.Data.AwakenDungeonTemplate>
    {
        public UILabel NameLabel;
        public GameObject OpenObj;
        public GameObject LockObj;
        public GameObject RedPoint;
        public UISprite BGSprite;
        public GameObject BlitzBtnObj;
        public GameObject EnterBtnObj;
        public List<LTShowItem> DropItemList;

        //进入所需门票数量
        public UILabel ticketneedNum;
        public UILabel ticketneedNum2;
        private Hotfix_LT.Data.AwakenDungeonTemplate data;
        private bool isLock;
        public EnterVigorController C_vigorController, B_vigorController;
        private int EnterVigor;
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            NameLabel = t.GetComponent<UILabel>("Label");
            OpenObj = t.FindEx("Unlock").gameObject;
            LockObj = t.FindEx("Lock").gameObject;
            RedPoint = t.FindEx("RedPoint").gameObject;
            BGSprite = t.GetComponent<UISprite>("Bg");
            BlitzBtnObj = t.FindEx("Unlock/BlitzBtn").gameObject;
            EnterBtnObj = t.FindEx("Unlock/EnterBtn").gameObject;
            C_vigorController = t.GetMonoILRComponent<EnterVigorController>("Unlock/EnterBtn/Sprite");
            B_vigorController = t.GetMonoILRComponent<EnterVigorController>("Unlock/BlitzBtn/Sprite");
            DropItemList = new List<LTShowItem>();
            var itemListRoot = t.FindEx("ItemList");

            for (var i = 0; i < itemListRoot.childCount; i++)
            {
                DropItemList.Add(itemListRoot.GetChild(i).GetMonoILRComponent<LTShowItem>());
            }

            ticketneedNum = t.GetComponent<UILabel>("Unlock/EnterBtn/Num");
            ticketneedNum2 = t.GetComponent<UILabel>("Unlock/BlitzBtn/Num (1)");

            t.GetComponent<ConsecutiveClickCoolTrigger>("Unlock/BlitzBtn").clickEvent.Add(new EventDelegate(OnBlitzBtnClick));
            t.GetComponent<UIButton>("Unlock/EnterBtn").onClick.Add(new EventDelegate(OnChallengeBtnClick));

            LTUIUtil.SetText(ticketneedNum, LTAwakeningInstanceConfig.GetCost().ToString());
            LTUIUtil.SetText(ticketneedNum2, LTAwakeningInstanceConfig.GetCost().ToString());
        }

        public override void Start()
        {
            RefreshView();
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnEnterTimeChange, RefreshView);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnEnterTimeChange, RefreshView);
        }

        public void RefreshView()
        {
            int dayDisCountTime = 0;
            int oldVigor = 0;
            int NewVigor = 0;
            NewGameConfigTemplateManager.Instance.GetEnterVigor(eBattleType.AwakeningBattle, out dayDisCountTime, out NewVigor, out oldVigor);
            int disCountTime = dayDisCountTime - LTAwakeningInstanceConfig.GetCurrencyTimes();
            disCountTime = disCountTime > 0 ? disCountTime : 0;
            EnterVigor = disCountTime > 0 ? NewVigor : oldVigor;
            LTUIUtil.SetText(ticketneedNum, string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_2985"), disCountTime));
            LTUIUtil.SetText(ticketneedNum2, string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_2985"), disCountTime));
            C_vigorController.Init(oldVigor, NewVigor, disCountTime > 0);
            B_vigorController.Init(oldVigor, NewVigor, disCountTime > 0);

        }

        private void InitDrop()
        {
            bool isFirst = LTAwakeningInstanceManager.Instance.JudgeIsFirst(data.ID, data.Stage);
            for (int i = 0; i < DropItemList.Count; ++i)
            {
                if (isFirst && data.DropAwardFirst.Count > 0)
                {
                    if (i < data.DropAwardFirst.Count)
                    {
                        DropItemList[i].LTItemData = data.DropAwardFirst[i];
                        DropItemList[i].mDMono.gameObject.CustomSetActive(true);
                        DropItemList[i].SetDropRateText(null);
                        DropItemList[i].mDMono.transform.Find("First").gameObject.CustomSetActive(true);
                        DropItemList[i].mDMono.transform.Find("First/Count").GetComponent<UILabel>().text =
                            data.DropAwardFirst[i].count.ToString();
                    }
                    else
                    {
                        DropItemList[i].mDMono.gameObject.CustomSetActive(false);
                    }
                }
                else
                {
                    if (i < data.DropAward.Count)
                    {
                        DropItemList[i].LTItemData = data.DropAward[i];
                        DropItemList[i].mDMono.gameObject.CustomSetActive(true);
                        DropItemList[i].mDMono.transform.Find("First").gameObject.CustomSetActive(false);
                        DropItemList[i].SetDropRateText(string.Format("{0}%", data.DropAward[i].multiple * 100));
                    }
                    else
                    {
                        DropItemList[i].mDMono.gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        public override void Clean()
        {
            data = null;
        }

        public override void Fill(Hotfix_LT.Data.AwakenDungeonTemplate itemData)
        {
            data = itemData;
            isLock = LTAwakeningInstanceManager.Instance.JudgeIsLock(data.ID, data.Stage);
            InitDrop();
            OpenObj.gameObject.CustomSetActive(!isLock);
            LockObj.gameObject.CustomSetActive(isLock);
            NameLabel.text = Localizer.GetString("ID_AWAKENDUNGEON_LAYER") + data.Name;
            if (!isLock)
            {
                BlitzBtnObj.CustomSetActive(LTAwakeningInstanceManager.Instance.JudgeFinish(
                    data.ID, data.Stage));
                EnterBtnObj.CustomSetActive(!LTAwakeningInstanceManager.Instance.JudgeFinish(
                    data.ID, data.Stage));
            }
            else
            {
                // RedPoint.CustomSetActive(false);
            }
        }

        //挑战副本
        public void OnChallengeBtnClick()
        {
            int num = 0;
            DataLookupsCache.Instance.SearchIntByID("userAwakenCampaign.ticket", out num);
            if (BalanceResourceUtil.EnterVigorCheck(EnterVigor))
            {
                //    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_5"));
                return;
            }

            if (LTAwakeningInstanceConfig.AwakeningIsLock(data.Type))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_7"));
                return;
            }

            int uid = 0;
            DataLookupsCache.Instance.SearchIntByID("userAwakenCampaign.uid", out uid);

            System.Action startCombatCallback = delegate ()
            {
                LTAwakeningInstanceManager.Instance.StartBattle(uid, data.ID);
            };
            BattleReadyHudController.Open(eBattleType.AwakeningBattle, startCombatCallback, data.CombatLayoutName, data.Type);
        }


        //扫荡副本
        public void OnBlitzBtnClick()
        {
            int num = 0;
            DataLookupsCache.Instance.SearchIntByID("userAwakenCampaign.ticket", out num);
            if (BalanceResourceUtil.EnterVigorCheck(EnterVigor))
            {
                //     MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_6"));
                return;
            }

            if (LTAwakeningInstanceConfig.AwakeningIsLock(data.Type))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_7"));
                return;
            }

            GlobalMenuManager.Instance.Open("LTAwakeningInstanceBlitzView", data);
        }
    }
}
