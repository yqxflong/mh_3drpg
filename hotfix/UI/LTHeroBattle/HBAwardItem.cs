using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class HBAwardItem : DynamicMonoHotfix
    {
        public List<LTShowItem> listShowItem;
        public int showItemBehind;
        public UILabel recordLabel;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            showItemBehind = 213;
            recordLabel = t.GetComponent<UILabel>("AwardRecordCell_Label");

            listShowItem = new List<LTShowItem>();
            listShowItem.Add(t.GetMonoILRComponent<LTShowItem>("LTShowItem"));
        }
    }
}
