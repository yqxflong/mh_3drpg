using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTHeroBattleHudCreater :DynamicMonoHotfix
    {
        public override void Start()
        {
            base.Start();
            string battleState;
            if (DataLookupsCache.Instance.SearchDataByID<string>("clashHeroBattleStatus.isInClash", out battleState) && battleState == "select")
            {
                LTHeroBattleLogic.GetInstance().PostGetMatchBaseInfo(delegate (bool isSuccessful)
                {
                    if (isSuccessful)
                    {
                        EB.Debug.LogError("Open LTHeroBattleMenu");
                        GlobalMenuManager.Instance.Open("LTHeroBattleMenu", true,true);
                    }
                	else
                		Debug.LogError("PostGetMatchBaseInfo Error");
                });
            }
        }
    }
}