using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstacneBagCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            SkillSelectSprite = t.GetComponent<UISprite>("BottomRight/Left/SkillBtn/Select");
            SkillUnSelectSprite = t.GetComponent<UISprite>("BottomRight/Left/SkillBtn/UnSelect");
            PropSelectSprite = t.GetComponent<UISprite>("BottomRight/Left/PropBtn/Select");
            PropUnSelectSprite = t.GetComponent<UISprite>("BottomRight/Left/PropBtn/UnSelect");
            ItemSelectSprite = t.GetComponent<UISprite>("BottomRight/Left/ItemBtn/Select");
            ItemUnSelectSprite = t.GetComponent<UISprite>("BottomRight/Left/ItemBtn/UnSelect");
            SkillTips = t.GetMonoILRComponent<LTChallengeInstacneBagSkillTips>("BottomRight/SkillTips");
            FirstBagCell = t.GetMonoILRComponent<LTChallengeInstanceBagCell>("BottomRight/Right/Scroll/PlaceHolder/Grid/Row/Item");
            Scroll = t.GetMonoILRComponent<LTChallengeInstanceBagScroll>("BottomRight/Right/Scroll/PlaceHolder/Grid");
            Scroll.SetOnBtnClickAction(OnBagCellClick);
            controller.backButton = t.GetComponent<UIButton>("BottomRight/bg/Top/CancelBtn");

            t.GetComponent<UIButton>("BottomRight/Left/SkillBtn").onClick.Add(new EventDelegate(OnSkillBtnClick));
            t.GetComponent<UIButton>("BottomRight/Left/ItemBtn").onClick.Add(new EventDelegate(OnItemBtnClick));
            t.GetComponent<UIButton>("BottomRight/Left/PropBtn").onClick.Add(new EventDelegate(OnPropBtnClick));
        }
        
        public enum BagType
        {
            SKILL,
            PROP,
            ITEM,
        }
    
        private BagType type = BagType.ITEM;
    
        public BagType Type
        {
            set
            {
                type = value;
                InitUI();
            }
    
            get
            {
                return type;
            }
        }
    
        public UISprite SkillSelectSprite;
    
        public UISprite SkillUnSelectSprite;
    
        public UISprite PropSelectSprite;
    
        public UISprite PropUnSelectSprite;
    
        public UISprite ItemSelectSprite;
    
        public UISprite ItemUnSelectSprite;
    
        public LTChallengeInstacneBagSkillTips SkillTips;
    
        public LTChallengeInstanceBagCell FirstBagCell;
    
        private LTChallengeInstanceBagCell curBagCell;
    
        public override bool IsFullscreen()
        {
            return false;
        }
    
        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            Type = BagType.ITEM;
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            SkillTips.HideUI();
            StopAllCoroutines();
            DestroySelf();
            yield break;
        }
    
        private void InitUI()
        {
            SkillSelectSprite.gameObject.SetActive(Type == BagType.SKILL);
            SkillUnSelectSprite.gameObject.SetActive(Type != BagType.SKILL);
            PropSelectSprite.gameObject.SetActive(Type == BagType.PROP);
            PropUnSelectSprite.gameObject.SetActive(Type != BagType.PROP);
            ItemSelectSprite.gameObject.SetActive(Type == BagType.ITEM);
            ItemUnSelectSprite.gameObject.SetActive(Type != BagType.ITEM);
    
            if (Type == BagType.ITEM)
            {
                InitItemList();
            }
            else if (Type == BagType.SKILL)
            {
                InitSkillList();
            }
        }
    
        public LTChallengeInstanceBagScroll Scroll;
    
        public void InitItemList()
        {
            Hashtable rewardTable = Johny.HashtablePool.Claim();
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userCampaignStatus.challengeChapters.reward", out rewardTable);
            List<LTChallengeInstanceBagData> rewardList = ParseItemData(rewardTable);
            InitSkillTips(rewardList);
            if (rewardList.Count < 20)
            {
                for (int i = rewardList.Count; i < 20; i++)
                {
                    rewardList.Add(new LTChallengeInstanceBagData(LTShowItemType.TYPE_GAMINVENTORY, string.Empty, 0));
                }
            }
            Scroll.SetItemDatas(rewardList);
        }
    
        private void InitSkillList()
        {
            ArrayList list = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("userCampaignStatus.challengeChapters.majordata.scrolls", out list);
            List<LTChallengeInstanceBagData> skillList = ParseSkillData(list);
            InitSkillTips(skillList);
            if (skillList.Count < 20)
            {
                for (int i = skillList.Count; i < 20; i++)
                {
                    skillList.Add(new LTChallengeInstanceBagData(LTShowItemType.TYPE_GAMINVENTORY, string.Empty, 0));
                }
            }
            Scroll.SetItemDatas(skillList);
        }
    
        private List<LTChallengeInstanceBagData> ParseItemData(Hashtable table)
        {
            List<LTChallengeInstanceBagData> list = new List<LTChallengeInstanceBagData>();
            foreach (DictionaryEntry data in table)
            {
                string type = EB.Dot.String("type", data.Value, string.Empty);
                int num = EB.Dot.Integer("quantity", data.Value, 0);
                string id = EB.Dot.String("data", data.Value, string.Empty);
                bool fromWish = EB.Dot.Bool("wishReward", data.Value, false);
                var tpl = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(id.ToString());
                if (tpl != null && tpl is Hotfix_LT.Data.EquipmentItemTemplate)
                {
                    //装备需要做不叠加显示（服务器下发的是叠加的）
                    for (int i = 0; i < num; i++)
                    {
                        LTChallengeInstanceBagData bagData = new LTChallengeInstanceBagData(LTShowItemType.TYPE_GAMINVENTORY, id, 1, fromWish);
                        list.Add(bagData);
                    }
                }
                else if (type.Equals(LTShowItemType.TYPE_ACTIVITY))
                {
                    //野兽印记是一种特殊的物品，物品表里面有，但是却是一种运营活动，活动表里面没有任何配置是配的什么物品，这里特殊写死
                    if (id.Equals("2005"))
                    {
                        LTChallengeInstanceBagData bagData = new LTChallengeInstanceBagData(LTShowItemType.TYPE_ACTIVITY, "2012", num, fromWish);
                        list.Add(bagData);
                    }
                    else//非脚印的其他兑换活动
                    {
                        LTChallengeInstanceBagData bagData = new LTChallengeInstanceBagData(LTShowItemType.TYPE_ACTIVITY, id, num, fromWish);
                        list.Add(bagData);
                    }
                }
                else
                {
                    LTChallengeInstanceBagData bagData = new LTChallengeInstanceBagData(type, id, num, fromWish);
                    list.Add(bagData);
                }
                //list.Sort(0, list.Count, new LTChallengeBagComparer());
            }
            return list;
        }
    
        private List<LTChallengeInstanceBagData> ParseSkillData(ArrayList list)
        {
            List<LTChallengeInstanceBagData> skillList = new List<LTChallengeInstanceBagData>();
            for (int i = 0; i < list.Count; ++i)
            {
                string type = EB.Dot.String("type", list[i], string.Empty);
                int num = EB.Dot.Integer("quantity", list[i], 0);
                string id = EB.Dot.String("data", list[i], string.Empty);
                LTChallengeInstanceBagData bagData = new LTChallengeInstanceBagData(type, id, num);
                skillList.Add(bagData);
            }
            skillList.Sort((a, b) => {
                int aid = 0;
                int.TryParse(a.Id, out aid);
                int bid = 0;
                int.TryParse(b.Id, out bid);
                if (aid < bid)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
            return skillList;
        }
    
        private void InitSkillTips(List<LTChallengeInstanceBagData> list)
        {
            if (list.Count <= 0)
            {
                SkillTips.HideUI();
                return;
            }
    
            ShowSkillTips(list[0]);
    
            curBagCell = FirstBagCell;
            list[0].IsSelect = true;
        }
    
        private void ShowSkillTips(LTChallengeInstanceBagData data)
        {
            LTShowItemData itemData = new LTShowItemData(data.Id.ToString(), data.Num, data.Type);
            SkillTips.Init(itemData);
        }
    
        public void OnItemBtnClick()
        {
            Type = BagType.ITEM;
        }
    
        public void OnPropBtnClick()
        {
            //Type = BagType.PROP;
        }
    
        public void OnSkillBtnClick()
        {
            Type = BagType.SKILL;
        }
    
        public void OnBagCellClick(LTChallengeInstanceBagCell bagCell)
        {
            //FusionAudio.PostEvent("UI/General/ButtonClick", true);
            if (string.IsNullOrEmpty(bagCell.CellData.Id))
            {
                return;
            }
            FusionAudio.PostEvent("UI/General/ButtonClick", true);
            //FusionAudio.PostEvent("UI/General/ButtonClick");
            if (curBagCell != null)
            {
                curBagCell.CellData.IsSelect = false;
                curBagCell.SetItemSelect(false);
            }
    
            curBagCell = bagCell;
            curBagCell.CellData.IsSelect = true;
            curBagCell.SetItemSelect(true);
    
            SkillTips.Init(curBagCell.ShowItem.LTItemData);
        }
    }
    
    /*public class LTChallengeBagComparer : IComparer<LTChallengeInstanceBagData>
    {
        public int Compare(LTChallengeInstanceBagData x, LTChallengeInstanceBagData y)
        {
            int flg = 0;
    
            var xData = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(x.Id.ToString());
            var yData = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(y.Id.ToString());
    
            int xIsEquipVal = xData is Hotfix_LT.Data.EquipmentItemTemplate ? 1 : 0;
            int yIsEquipVal = yData is Hotfix_LT.Data.EquipmentItemTemplate ? 1 : 0;
    
            flg = xIsEquipVal - yIsEquipVal;
            if (flg != 0)
            {
                return flg;
            }
    
            flg = yData.QualityLevel - xData.QualityLevel;
            if (flg != 0)
            {
                return flg;
            }
    
            return x.Id - y.Id;
        }
    }*/
}
