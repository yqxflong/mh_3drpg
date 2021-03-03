using EB;
using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class RecoverVigorViewController : UIControllerHotfix
    {
        private RecoverVigorGridScroll recoverVigorGridScroll;
        private List<RecoverVigorItemData> recoverVigorItemsData;
        public override void Awake()
        {
            var t = controller.transform;
            recoverVigorGridScroll = t.GetMonoILRComponent<RecoverVigorGridScroll>("Scroll View/Container/Grid");
        }

        public override void SetMenuData(object param)
        {
            if(param!=null&&param is List<RecoverVigorItemData>)
            {
                recoverVigorItemsData = param as List<RecoverVigorItemData>;
                recoverVigorGridScroll.SetItemDatas(recoverVigorItemsData.ToArray());
            }
               
        }
        public override IEnumerator OnAddToStack()
        {
            return base.OnAddToStack();
        }
        
        public override IEnumerator OnRemoveFromStack()
        {
            return base.OnRemoveFromStack();
        }

        public override bool IsFullscreen()
        {
            return false;
        }

        public override bool ShowUIBlocker => true;
    }


    public class RecoverVigorGridScroll : DynamicGridScroll<RecoverVigorItemData, RecoverVigorItem> 
    {

    }


    public class RecoverVigorItem : BaseItem<RecoverVigorItemData>
    {
        private LTShowItem lTShowItem;
        private LTShowItemData showdata;
        private UILabel addvigor;
        private ConsecutiveClickCoolTrigger useitemBtn;
        private RecoverVigorItemData curData;
        private string inventoryid;
        private GameObject mask;
        public override void Awake()
        {
            var t = mDMono.transform;
            lTShowItem = t.GetMonoILRComponent<LTShowItem>("Item");
            addvigor = t.GetComponent<UILabel>("StateShow/Label");
            useitemBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("StateShow/ToGetButton");
            useitemBtn.clickEvent.Add(new EventDelegate(OnUseRecoverItem));
            showdata = new LTShowItemData("vigor", 1, LTShowItemType.TYPE_RES);
            mask = t.FindEx("StateShow/Mask").gameObject;
        }
        public override void Clean()
        {
            Fill(null);
        }

        public override void Fill(RecoverVigorItemData itemData)
        {
            if (itemData != null)
            {
                curData = itemData;
                int count = GameItemUtil.GetInventoryItemNum(curData.itemdataId, out inventoryid);
                lTShowItem.LTItemData = new LTShowItemData(curData.itemdataId, count, LTShowItemType.TYPE_GAMINVENTORY);
                addvigor.text = string.Format("+{0}", itemData.vigor);
                showdata.count = itemData.vigor;
                LTUIUtil.SetGreyButtonEnable(useitemBtn, count > 0);
                mask.CustomSetActive(count <= 0);
            }
        } 
        
        private void OnUseRecoverItem()
        {
            InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
            invManager.useInventory(inventoryid, 1, delegate (bool isSucess){
                if (isSucess)
                {
                    GameUtils.ShowAwardMsg(showdata);
                    Fill(curData);
                }       
            });
        }

    }

    public class RecoverVigorItemData 
    {
        public string itemdataId;
        public int vigor;
        public int num;
        public RecoverVigorItemData(string itemdataid,int vigor,int num)
        {
            itemdataId = itemdataid;
            this.vigor = vigor;
            this.num = num;
        }
    }


}