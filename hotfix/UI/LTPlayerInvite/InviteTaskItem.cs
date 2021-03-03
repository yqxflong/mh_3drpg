using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{

    public class InviteTaskItem : BaseItem<InviteTaskItemData>
    {
        private UILabel taskdes,taskdesShadow;
        private LTShowItem rewardItem;
        private InviteTaskDataLookUp Itemdatalookup;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.GetComponent<Transform>();
            taskdes = t.GetComponent<UILabel>("DesLabel");
            taskdesShadow = t.GetComponent<UILabel>("DesLabel/Label(Clone)");           
            rewardItem = t.GetMonoILRComponent<LTShowItem>("GiftGrid/LTShowItem");
            Itemdatalookup = t.GetDataLookupILRComponent<InviteTaskDataLookUp>();
        }

        public override void Clean()
        {
            Fill(null);
        }

        public override void Fill(InviteTaskItemData itemData)
        {
            if(itemData == null)
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }
            taskdes.text = taskdesShadow.text = itemData.taskdes;
            if (itemData.rewarddata != null && itemData.rewarddata.Count > 0)
            {
                rewardItem.LTItemData = itemData.rewarddata[0];
            }
            //设置itemDatalookupID
            Itemdatalookup.SetTaskData(itemData.taskid,string.Format("tasks.{0}", itemData.taskid));

        }

    }

}