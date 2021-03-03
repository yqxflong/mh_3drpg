using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 夺宝奇兵主UI
    /// </summary>
    public class LTSpeedSnatchAwardInfoHudUI : UIControllerHotfix
    {
        public List<LTSpeedSnatchAwardItem> listItem;
        private List<Transform> _listTrans;
        LTSpeedSnatchAwardItem tempdata;
        public int behind = 245;

        public override void Awake()
        {
            base.Awake();
            UIButton backBtn = controller.transform.Find("CloseBtn").GetComponent<UIButton>();
            controller.backButton = backBtn;

            listItem = new List<LTSpeedSnatchAwardItem>();
            _listTrans = new List<Transform>();
            Transform item001 = controller.transform.Find("Scroll View/AwardItem001");
            tempdata = new LTSpeedSnatchAwardItem(item001);
            AddItem(tempdata);
        }

        void AddItem(LTSpeedSnatchAwardItem item)
        {
            listItem.Add(tempdata);
            _listTrans.Add(item.itemRoot);
        }
        

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            return base.OnRemoveFromStack();
        }

        public override void OnFocus()
        {
			PausePanelUpdate(UIPanel.PauseType.Others, false);
		}

        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
        
        public override void SetMenuData(object param)
        {
            List<GhostReward> tabl = param as List<GhostReward>;

            if (listItem.Count != tabl.Count)
            {
                LTUIUtil.SetNumTemplate<Transform>(_listTrans[0], _listTrans, tabl.Count, behind);

                if (_listTrans.Count > listItem.Count)
                {
                    while (_listTrans.Count != listItem.Count)
                    {
                        Transform item = _listTrans[listItem.Count];
                        tempdata = new LTSpeedSnatchAwardItem(item);
                        listItem.Add(tempdata);
                    }
                }
                else
                {
                    while (_listTrans.Count != listItem.Count)
                    {
                        listItem.RemoveAt(_listTrans.Count - 1);
                    }
                }
            }

            for (int i = 0; i < tabl.Count; i++)
            {
                LTSpeedSnatchAwardItem item = listItem[i];
                item.spt001.spriteName = tabl[i].spriteNames[0];
                item.spt002.spriteName = tabl[i].spriteNames[1];
                item.spt003.spriteName = tabl[i].spriteNames[2];

                item.showItem.LTItemData = tabl[i].rewards[0];// new ShowItemData(tabl[i].reward, 1, UIGameItem.TYPE_GAMINVENTORY);

                if(i==tabl.Count-1) //最后一组改成文字提示
                {
                    item.otherTipsLabel.gameObject.SetActive(true);
                    item.showAttrTrans.gameObject.SetActive(false);
                }
            }
        }
    }

    public class LTSpeedSnatchAwardItem
    {
        public Transform itemRoot;
        public UISprite spt001;
        public UISprite spt002;
        public UISprite spt003;
        public LTShowItem showItem;
        public UILabel otherTipsLabel;
        public Transform showAttrTrans;
        public LTSpeedSnatchAwardItem(Transform itemRoot)
        {
            this.itemRoot = itemRoot;

            spt001 = itemRoot.Find("ShowAttribute/AttributeIcon1").GetComponent <UISprite>();
            spt002 = itemRoot.Find("ShowAttribute/AttributeIcon2").GetComponent<UISprite>();
            spt003 = itemRoot.Find("ShowAttribute/AttributeIcon3").GetComponent<UISprite>();
            showItem = itemRoot.Find("LTShowItem").GetMonoILRComponent<LTShowItem>();
            
            otherTipsLabel = itemRoot.Find("OtherTipsLabel").GetComponent<UILabel>();
            showAttrTrans = itemRoot.Find("ShowAttribute");
        }


    }
}
