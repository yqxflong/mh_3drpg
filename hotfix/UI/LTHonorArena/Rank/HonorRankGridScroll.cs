namespace Hotfix_LT.UI
{
    public class HonorRankGridScroll : DynamicGridScroll<HonorArenaItemData, HonorArenaItem>
    {
        public new HonorArenaItemData[] dataItems
        {
            get { return mDataItems; }

            set
            {
                value = value ?? new HonorArenaItemData[0];
                Dirty = true;
                mReposition = true;
                mDataItems = value;
            }
        }
    }
}