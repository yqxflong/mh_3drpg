namespace Hotfix_LT.UI
{
    public class CommonRankGridScroll : DynamicGridScroll<CommonRankItemData, CommonRankItem>
    {
        public CommonRankItemData[] dataItems
        {
            get { return mDataItems; }

            set
            {
                value = value ?? new CommonRankItemData[0];
                Dirty = true;
                mReposition = true;
                mDataItems = value;
            }
        }
    }
}