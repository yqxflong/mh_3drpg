using System.Collections.Generic;
using UnityEngine;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 限时礼包购买面板
    /// </summary>
    public class LTActivityBodyItem_Gift : LTActivityBodyItem
    {
        public UILabel BuyLimit;
        public UIGrid GiftGrid;
        public LTShowItem ItemTemplate;
        public override void Awake()
        {
            base.Awake();
            BuyLimit = mDMono.transform.Find("NavButton/BuyLimit").GetComponent<UILabel>();
            GiftGrid = mDMono.transform.Find("GiftGrid").GetComponent<UIGrid>();
            ItemTemplate = mDMono.transform.Find("GiftGrid/0").GetMonoILRComponent<LTShowItem>();
            ItemTemplate.mDMono.gameObject.CustomSetActive(false);
        }

        public override void SetData(object data)
        {
            base.SetData(data);

            if (NavString[0] == "P")
            {
                var payouts = SparxHub.Instance.GetManager<WalletManager>().Payouts;
                BuyLimit.text = "";
                for (int i = 0; i < payouts.Length; i++)
                {
                    if (payouts[i].payoutId == int.Parse(NavString[1]))
                    {
                        BuyLimit.text = string.Format(EB.Localizer.GetString("ID_PURCHASE_LIMIT"), payouts[i].buyLimit);//限购次数
                        for (var j = 0; j < payouts[i].redeemers.Count; j++)
                        {
                            var itemdata = payouts[i].redeemers[j];
                            var item = UIControllerHotfix.InstantiateEx(ItemTemplate, ItemTemplate.mDMono.transform.parent, (j + 1).ToString());
                            item.LTItemData = new LTShowItemData(itemdata.Data, itemdata.Quantity, itemdata.Type, false);
                        }
                        break;
                    }
                }
            }
            else
            {
                BuyLimit.text = "";
            }
            if (string.IsNullOrEmpty(BuyLimit.text))
            {
                NavLabel.transform.localPosition = new Vector3(0, 18, 0);
            }
            GiftGrid.Reposition();
        }
    }
}