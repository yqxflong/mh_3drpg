using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTMainInstanceBlitzCell : DynamicCellController<LTMainInstanceBlitzData>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Desc = t.GetComponent<UILabel>("Item/Desc");
            ItemList = new List<LTShowItem>();
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("Item/Grid/GameItem"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("Item/Grid/GameItem (1)"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("Item/Grid/GameItem (2)"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("Item/Grid/GameItem (3)"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("Item/Grid/GameItem (4)"));
            Hotfix_LT.Messenger.AddListener<int>(EventName.LTBlitzCellTweenEvent, OnTweenListener);
        }
        public List<LTShowItem> ItemList;
    
        public UILabel Desc;
    
        private int Index=0;
    
        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener<int>(EventName.LTBlitzCellTweenEvent, OnTweenListener);
        }
    
        private TweenScale TS;
        private void OnTweenListener(int index)
        {
            if (Index > 0 && index == Index)
            {
                if (TS == null)
                {
                    TS = mDMono.GetComponent<TweenScale>();
                }
                TS.ResetToBeginning();
                TS.PlayForward();
            }
        }
    
        public override void Clean()
        {
            Index = 0;
        }
    
        public override void Fill(LTMainInstanceBlitzData itemData)
        {
            Index = itemData.Index;
            Desc.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTMainInstanceBlitzCell_594"), itemData.Index);
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (i < itemData.ItemList.Count)
                {
                    ItemList[i].mDMono.gameObject.CustomSetActive(true);
                    ItemList[i].LTItemData = itemData.ItemList[i];
                }
                else
                {
                    ItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
        }
    }
}
