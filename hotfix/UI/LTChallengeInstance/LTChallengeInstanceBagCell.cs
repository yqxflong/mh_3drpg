using UnityEngine;
using System.Collections;
using System;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceBagData
    {
    	public string Type;
    	public string Id;
        public int Num;
    	public bool IsSelect;
        public bool isFromWish;
    
    	public LTChallengeInstanceBagData(string type, string id, int num, bool isFromWish = false)
        {
    		Type = type;
            Id = id;
            Num = num;
            this.isFromWish = isFromWish;
        }
    }
    
    //object[0]
    public class LTChallengeInstanceBagCell : DynamicCellController<LTChallengeInstanceBagData>
    {
        private System.Action<LTChallengeInstanceBagCell> OnClick;
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            ShowItem = ((GameObject)mDMono.ObjectParamList[0]).GetMonoILRComponent<LTShowItem>();
            if (mDMono.ObjectParamList[1] != null)
            {
                ShortShadow = ((GameObject)mDMono.ObjectParamList[1]).GetComponent<UISprite>();
            }
            Shadow = ((GameObject)mDMono.ObjectParamList[2]).GetComponent<UISprite>();
            SelectedGO = (GameObject)mDMono.ObjectParamList[3];//. t.FindEx("SelectObj").gameObject;
            UIEventTrigger trigger = t.GetComponent<UIEventTrigger>();
            if (trigger != null) trigger.onClick.Add(new EventDelegate (OnBtnClick));
        }

        public void SetOnBtnClickAction(System.Action<LTChallengeInstanceBagCell> OnClick)
        {
            this.OnClick = OnClick;
        }
        
        private void OnBtnClick()
        {
            if (OnClick != null) OnClick(this);
        }

        public LTChallengeInstanceBagData CellData;
    	public LTShowItem ShowItem;
        public UISprite ShortShadow;
        public UISprite Shadow;
    	public GameObject SelectedGO;
    
        public override void Clean()
        {
            SetItemData(null);
        }
    
        public override void Fill(LTChallengeInstanceBagData itemData)
        {
            SetItemData(itemData);
        }
    
        private void SetItemData(LTChallengeInstanceBagData itemData)
        {
    		CellData = itemData;

    		if (itemData == null || string.IsNullOrEmpty(itemData.Id))
            {
                ShowItem.mDMono.gameObject.CustomSetActive(false);

                if (ShortShadow)
                {
                    ShortShadow.gameObject.CustomSetActive(true);
                }

                Shadow.gameObject.CustomSetActive(false);

                if (SelectedGO != null)
                {
                    SelectedGO.gameObject.CustomSetActive(false);
                }

    			return;
            }

            ShowItem.mDMono.gameObject.CustomSetActive(true);

            if (ShortShadow)
            {
                ShortShadow.gameObject.CustomSetActive(false);
            }

            Shadow.gameObject.CustomSetActive(true);

            if (SelectedGO != null)
            {
                SelectedGO.gameObject.CustomSetActive(itemData.IsSelect);
            }

    		ShowItem.LTItemData = new LTShowItemData(itemData.Id.ToString(), itemData.Num, itemData.Type, isFromWish:itemData.isFromWish);
        }
    
        public void SetItemSelect(bool isShow)
        {
            SelectedGO.CustomSetActive(isShow);
        }
    }
}
