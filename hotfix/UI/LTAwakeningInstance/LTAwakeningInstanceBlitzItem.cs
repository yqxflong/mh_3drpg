using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTAwakeningInstanceBlitzData
    {
        public List<LTShowItemData> ItemList;
        public int Index;
    }
    
    public class LTAwakeningInstanceBlitzItem : DynamicCellController<LTAwakeningInstanceBlitzData>
    {
        public List<LTShowItem> ItemList;
        public UILabel Desc;

        private int Index = 0;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Desc = t.GetComponent<UILabel>("Item/Desc");
            ItemList = new List<LTShowItem>();

            var grid = t.FindEx("Item/Grid");

            for (var i = 0; i < grid.childCount; i++)
            {
                ItemList.Add(grid.GetChild(i).GetMonoILRComponent<LTShowItem>());
            }

            Hotfix_LT.Messenger.AddListener<int>(EventName.LTBlitzCellTweenEvent, OnTweenListener);
        }

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
                    TS = mDMono.transform.GetComponent<TweenScale>();
                }
                TS.ResetToBeginning();
                TS.PlayForward();
            }
        }
    
        public override void Clean()
        {
            Index = 0;
        }
    
        public override void Fill(LTAwakeningInstanceBlitzData itemData)
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
