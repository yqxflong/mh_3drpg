using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceShopCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            Scroll = t.GetMonoILRComponent<UIStoreGridScroll>("Scroll/PlaceHolder/Grid");
            Scroll.columns = Columns = 3;
            controller.backButton = t.GetComponent<UIButton>("BG/Top/CloseBtn");
        }

        public override bool ShowUIBlocker{ get{ return true;}}
    
        public override bool IsFullscreen() { return false;}
    
        public UIStoreGridScroll Scroll;
    
        public int Columns = 3;
    
        public string StoreType = "challenge";
    
        private ArrayList itemList;
    
        private List<LTChallengeInstanceBagData> skillList;
    
        private int mx;
        private int my;
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            var data = (LTInstanceEvent)param;
            mx = data.x;
            my =data.y;
            itemList = data.Param as ArrayList;
        }
    
        public override void OnEnable()
        {
            Hotfix_LT.Messenger.AddListener<object>(EventName.ChallengeInstanceBuySucc, OnBuySucc);
            Hotfix_LT.Messenger.AddListener<StoreItemData>(EventName.StoreBuyEvent, OnStoreBuyEvent);
        }

        public override void OnDisable()
        {
            Hotfix_LT.Messenger.RemoveListener<object>(EventName.ChallengeInstanceBuySucc, OnBuySucc);
            Hotfix_LT.Messenger.RemoveListener<StoreItemData>(EventName.StoreBuyEvent, OnStoreBuyEvent);
        }
    
        public override IEnumerator OnAddToStack()
        {
            InitUI();
            yield return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield return base.OnRemoveFromStack();
        }
    
        private void InitUI()
        {
            InitSkill();
            List<StoreItemData> datas = new List<StoreItemData>();
            for (int i = 0; i < itemList.Count; i++)
            {
                int sid= EB.Dot.Integer("id", itemList[i], 0);
                string id = EB.Dot.String("redeems.data", itemList[i], "");
                string type = EB.Dot.String("redeems.type", itemList[i], "");
                int buy_num = EB.Dot.Integer("redeems.quantity", itemList[i], 0);
                int have = GetScrollNum(id);
                string cost_id = EB.Dot.String("spends.data", itemList[i], "");
                int cost_num = EB.Dot.Integer("spends.quantity", itemList[i], 0);
                int num = EB.Dot.Integer("num", itemList[i], 0);
                float mdiscount = EB.Dot.Single("discount", itemList[i], 1);
                bool sell_out = num > 0 || num == -1 ? false : true;
                int weight = 1;
                int buyId = EB.Dot.Integer("id", itemList[i], 0);
                if (!string.IsNullOrEmpty(id))
                {
                    StoreItemData itemdata = new StoreItemData(sid,id, type, buy_num, have, cost_id, cost_num, sell_out, weight, buyId, StoreType, mdiscount);
                    datas.Add(itemdata);
                }
            }
            int left = datas.Count % Columns;
            if (left > 0)
            {
                left = Columns - left;
                for (int i = 0; i < left; i++)
                {
                    StoreItemData itemdata = new StoreItemData(0,"", "", 1, 1, "", 1, true, 1, i, StoreType,1);
                    datas.Add(itemdata);
                }
            }
    
            Scroll.SetItemDatas(datas);
        }
    
        public void OnStoreBuyEvent(StoreItemData data)
        {
            if (data == null)
            {
                return;
            }
    
            BuyClick(data);
        }
        
        public void BuyClick(StoreItemData target)
        {
            if (target.sell_out)
            {
                return;
            }
            int isCanBuyMessageId = 0;
            if (!GameItemUtil.GetItemIsCanBuy(target.id, target.type, out isCanBuyMessageId))
            {
                MessageTemplateManager.ShowMessage(isCanBuyMessageId);
                return;
            }
    
            int resBalance = BalanceResourceUtil.GetResValue(target.cost_id);
            if (resBalance < target.cost)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceShopCtrl_3630"));
                return;
            }
            LTInstanceMapModel.Instance.RequestChallengeBuyScroll(new[] { mx, my }, target.buy_id, delegate
            {
                LTInstanceMapModel.Instance.RequestGetChapterState(() =>
                {
                });
                GlobalMenuManager.Instance.CloseMenu("LTStoreBuyUI");
            });
        }
    
        private void OnBuySucc(object  e)
        {

            itemList = e as ArrayList;
            InitUI();
        }
    
        private int GetScrollNum(string skillId)
        {
            for (int i = 0; i < skillList.Count; i++)
            {
                if (skillList[i].Id == skillId)
                {
                    return skillList[i].Num;
                }
            }
    
            return 0;
        }
    
        private void InitSkill()
        {
            ArrayList list = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("userCampaignStatus.challengeChapters.majordata.scrolls", out list);
            skillList = ParseSkillData(list);
        }
    
        private List<LTChallengeInstanceBagData> ParseSkillData(ArrayList list)
        {
            List<LTChallengeInstanceBagData> skillList = new List<LTChallengeInstanceBagData>();
            for (var i = 0; i < list.Count; i++)
            {
                var data = list[i];
                string type = EB.Dot.String("type", data, string.Empty);
                int num = EB.Dot.Integer("quantity", data, 0);
                string id = EB.Dot.String("data", data, string.Empty);
                LTChallengeInstanceBagData bagData = new LTChallengeInstanceBagData(type, id, num);
                skillList.Add(bagData);
            }
            return skillList;
        }
    }
}
