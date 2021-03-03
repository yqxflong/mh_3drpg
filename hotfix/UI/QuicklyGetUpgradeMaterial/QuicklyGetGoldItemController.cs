using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class QuicklyGetGoldItemController : DynamicMonoHotfix
    {

        private LTShowItem showItem;
        private ConsecutiveClickCoolTrigger clickTrigger;
        public override void Awake()
        {
            var t = mDMono.transform;
            showItem = t.GetMonoILRComponent<LTShowItem>("LTShowItem");
            clickTrigger = t.GetComponent<ConsecutiveClickCoolTrigger>("ToGetButton");
        }


        public void Fill(LTShowItemData data, System.Action callback)
        {
            if (data == null)
            {
                Close();
                return;
            }
            showItem.LTItemData = data;
            mDMono.gameObject.CustomSetActive(true);
            clickTrigger.clickEvent.Clear();
            clickTrigger.clickEvent.Add(new EventDelegate(delegate { callback(); }));
        }

        public void Close()
        {
            mDMono.gameObject.CustomSetActive(false);
        }
    }
}
