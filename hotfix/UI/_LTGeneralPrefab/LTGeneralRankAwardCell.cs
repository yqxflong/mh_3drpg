using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTGeneralRankAwardCell : DynamicCellController<LTRankAwardData>
    {
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            RankLabel = t.GetComponent<UILabel>("Name");
            Items =new  List<LTShowItem>();
            Items.Add(t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem"));
            Items.Add(t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem (1)"));
            Items.Add(t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem (2)"));
        }

        public UILabel RankLabel;
        public List<LTShowItem> Items;

        public override void Clean()
        {
            Fill(null);
        }

        public override void Fill(LTRankAwardData itemData)
        {
            if (itemData == null)
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }
            mDMono.gameObject.CustomSetActive(true);

            if (itemData.down == -1)
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_ArenaRankAwardCell_386"), itemData.top);
            else if (itemData.top == itemData.down)
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_RANK_NUMBER"), itemData.top);
            else if (itemData.down >= 1000)
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_ArenaRankAwardCell_611"), itemData.top, itemData.down);
            else
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_ArenaRankAwardCell_716"), itemData.top, itemData.down);

            if (itemData != null&& itemData.items!=null)
            {
                for (int i=0;i< Items.Count;++i)
                {
                    if (itemData.items.Count > i)
                    {
                        Items[i].LTItemData = itemData.items[i];
                        Items[i].mDMono.gameObject.CustomSetActive(true);
                    }
                    else
                    {
                        Items[i].mDMono.gameObject.CustomSetActive(false);
                    }
                }
            }
            else
            {
                Items.ForEach(item => item.mDMono.gameObject.CustomSetActive(false));
            }
        }
    }
}