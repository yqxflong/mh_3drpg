//LTRacingBetDataLookup
//赛跑活动下注界面货币变化接收器
//Johny
    
namespace Hotfix_LT.UI
{
    public class LTRacingBetDataLookup : DataLookupHotfix
    {
        public LTActivityRacingBetController holder{get;set;}

        public override void OnLookupUpdate(string dataID, object value)
        {
            holder?.OnDataListener();
        }
    }
}
    
