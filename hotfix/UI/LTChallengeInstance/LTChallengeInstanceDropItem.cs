using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceDropItem : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            ItemList = new List<LTShowItem>();
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("Grid/GameItem"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("Grid/GameItem (1)"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("Grid/GameItem (2)"));
            ItemList.Add(t.GetMonoILRComponent<LTShowItem>("Grid/GameItem (3)"));

            Day = (DayOfWeek)mDMono.IntParamList[0];
        }

        public DayOfWeek Day;
    
        public List<LTShowItem> ItemList;

        private bool hasInit = false;
        public void SetDrop()
        {
            if (hasInit) return;
            hasInit =true;
            Hotfix_LT.Data.LostChallengeRewardTemplate temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeReward(Day, Hotfix_LT.Data.SceneTemplateManager.Instance.LostChallengeRewardMaxFloor);
            List<string> list = temp.DropList;
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (i < list.Count)
                {
                    ItemList[i].mDMono.gameObject.CustomSetActive(true);
                    ItemList[i].LTItemData = new LTShowItemData(list[i], 0, LTShowItemType.TYPE_GAMINVENTORY);
                }
                else
                {
                    ItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
        }
    }
}
