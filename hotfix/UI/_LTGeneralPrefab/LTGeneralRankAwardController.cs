using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTRankAwardData
    {
        public int top;
        public int down;

        public List<LTShowItemData> items;
        
        public LTRankAwardData(int top, int down, int gold, int hc, LTShowItemData spectial_gold, LTShowItemData item)
        {
            this.top = top;
            this.down = down;

            items = new List<LTShowItemData>();
            if (item != null) items.Add(item);
            if (hc > 0) items.Add(new LTShowItemData(LTResID.HcName, hc, LTShowItemType.TYPE_RES));
            if (spectial_gold != null) items.Add(spectial_gold);
            if (item != null) items.Add(new LTShowItemData(LTResID.GoldName, gold, LTShowItemType.TYPE_RES));
        }

        public LTRankAwardData(int top, int down,List<LTShowItemData> items)
        {
            this.top = top;
            this.down = down;
            this.items = items;
        }
    }

    public class LTGeneralRankAwardController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();
            Transform t = controller.transform;
            controller .backButton = t.GetComponent<UIButton>("Frame/BG/Top/CloseBtn");
            TipLabel = t.GetComponent<UILabel>("Frame/BG/Top/SendTips");
            DynamicScroll = t.GetMonoILRComponent<LTGeneralRankAwardDynamicScroll>("Content/ScrollView/Placehodler/Grid");
        }
        
        public override bool ShowUIBlocker { get { return true; } }
        public override bool IsFullscreen() { return false; }

        public UILabel TipLabel;
        public LTGeneralRankAwardDynamicScroll DynamicScroll;
        public List<LTRankAwardData> data;
        public override void SetMenuData(object param)
        {
            Hashtable ht = param as Hashtable;
            string tip = string.Empty;
            tip = EB.Dot.String("tip", ht, string.Empty);
            data = ht["itemDatas"] as List<LTRankAwardData>;

            TipLabel.text = tip;
        }

        public override IEnumerator OnAddToStack()
        {
            DynamicScroll.SetItemDatas (data.ToArray());
            yield return base.OnAddToStack();
        }
    }
}