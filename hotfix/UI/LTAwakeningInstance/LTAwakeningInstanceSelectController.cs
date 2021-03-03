using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTAwakeningInstanceSelectController : UIControllerHotfix
    {
        public UILabel TitleLabel;
        public UISprite TitleSprite;
        //物品ID
        public List<UISprite> TopAwakeningItemList;
        public List<InventoryItemNumDataLookup> TopItemlookupList;
        public LTAwakeningInstanceSelectDynamicScroll DynamicScroll;
        public ParticleSystemUIComponent[] mFX;
        private Hotfix_LT.Data.eRoleAttr type;
        public override bool IsFullscreen() { return true; }
        public override bool ShowUIBlocker { get { return false; } }
    
     
        private LTShowItem[] activeObj;
        private int cCount;
        private Hotfix_LT.Data.eRoleAttr m_ERoleAttr;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TitleLabel = t.GetComponent<UILabel>("Center/BG/Right/Name");
            TitleSprite = t.GetComponent<UISprite>("Center/BG/Right");

            TopAwakeningItemList = new List<UISprite>();
            TopAwakeningItemList.Add(t.GetComponent<UISprite>("Edge/TopRight/NewCurrency/Table/1/Bg/Icon"));
            TopAwakeningItemList.Add(t.GetComponent<UISprite>("Edge/TopRight/NewCurrency/Table/2/Bg/Icon"));
            TopAwakeningItemList.Add(t.GetComponent<UISprite>("Edge/TopRight/NewCurrency/Table/3/Bg/Icon"));

            TopItemlookupList = new List<InventoryItemNumDataLookup>();
            TopItemlookupList.Add(t.GetDataLookupILRComponent<InventoryItemNumDataLookup>("Edge/TopRight/NewCurrency/Table/1/Bg/Label"));
            TopItemlookupList.Add(t.GetDataLookupILRComponent<InventoryItemNumDataLookup>("Edge/TopRight/NewCurrency/Table/2/Bg/Label"));
            TopItemlookupList.Add(t.GetDataLookupILRComponent<InventoryItemNumDataLookup>("Edge/TopRight/NewCurrency/Table/3/Bg/Label"));

            mFX = new ParticleSystemUIComponent[3];
            mFX[0] = t.GetComponent<ParticleSystemUIComponent>("Center/BG/Right/fxfeng");
            mFX[1] = t.GetComponent<ParticleSystemUIComponent>("Center/BG/Right/fxshui");
            mFX[2] = t.GetComponent<ParticleSystemUIComponent>("Center/BG/Right/fxhuo");

            DynamicScroll = t.GetMonoILRComponent<LTAwakeningInstanceSelectDynamicScroll>("Center/LevelList/Placeholder/Grid");
            controller.backButton = t.GetComponent<UIButton>("Edge/TopLeft/BackBtn");

            t.GetComponent<UIButton>("Edge/TopRight/NewCurrency/Table/Notice").onClick.Add(new EventDelegate(OnRuleBtnClick));
            t.GetComponent<UIButton>("Edge/TopRight/NewCurrency/Table/2/Bg").onClick.Add(new EventDelegate(() => OnGenericTransBtnClick(TopAwakeningItemList[1].transform)));
            t.GetComponent<UIButton>("Edge/TopRight/NewCurrency/Table/3/Bg").onClick.Add(new EventDelegate(() => OnGenericTransBtnClick(TopAwakeningItemList[2].transform)));
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null) m_ERoleAttr = (Hotfix_LT.Data.eRoleAttr) param;
            
            switch (m_ERoleAttr)
            {
                case Hotfix_LT.Data.eRoleAttr.Feng:
                    type = Hotfix_LT.Data.eRoleAttr.Feng;
                    TitleSprite.spriteName = "Juexing_Icon_Fengzhishilian";
                    TitleLabel.text = EB.Localizer.GetString("ID_AWAKENDUNGEON_WIND");
                    SetFx(0);
                    break;
                case Hotfix_LT.Data.eRoleAttr.Shui:
                    type = Hotfix_LT.Data.eRoleAttr.Shui;
                    TitleSprite.spriteName = "Juexing_Icon_Shuizhishilian";
                    TitleLabel.text = EB.Localizer.GetString("ID_AWAKENDUNGEON_WATER");
                    SetFx(1);
                     break;
                case Hotfix_LT.Data.eRoleAttr.Huo:
                default:
                    type = Hotfix_LT.Data.eRoleAttr.Huo;
                    TitleSprite.spriteName = "Juexing_Icon_Huozhishilian";
                    TitleLabel.text = EB.Localizer.GetString("ID_AWAKENDUNGEON_FIRE");
                    SetFx(2);
                    break;
            }
    
           
            UpdateTopAwakeningItem();
            
            var temp =Hotfix_LT.Data.EventTemplateManager.Instance.GetAwakenDungeonsByType(type);
            DynamicScroll.SetItemDatas(temp.ToArray());
            
            int curShowSelectStage = LTAwakeningInstanceManager.Instance.GetMaxLayer(type);
            curShowSelectStage = Mathf.Min(curShowSelectStage+1, temp.Count);
    
            var VS=DynamicScroll.scrollView.GetComponent<UIPanel>().GetViewSize();
            //最大可见的物品数量
            int ActinCount =(int)( VS.y / DynamicScroll.mDMono.transform.GetComponent<UIGrid>().cellHeight);
            int showStage = Mathf.Min(curShowSelectStage - 1, temp.Count - 1 - ActinCount);
            DynamicScroll.MoveTo(showStage);
    
            if (curShowSelectStage==temp.Count)
            {
                DynamicScroll.MoveTo(showStage+1);
            }
            
        }
    
        public void SetFx(int index)
        {
            for (int i = 0; i < mFX.Length; i++)
            {
                if (i == index)
                {
                    mFX[i].gameObject.SetActive(true);
                }
                else
                {
                    mFX[i].gameObject.SetActive(false);
                }
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            GlobalMenuManager.Instance.PushCache("LTAwakeningInstanceSelectHud",m_ERoleAttr);
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            ResetTopAwakeningItemNum();
            DestroySelf();
            yield break;
        }
    
        public override void OnFocus()
        {
            base.OnFocus();
            if (hasBlur)
            {
                hasBlur =false;
                UpdateTopAwakeningItemNum();
            }
        }
    
        bool hasBlur = false;
        public override void OnBlur()
        {
            base.OnBlur();
            hasBlur = true;
        }
        public override void OnCancelButtonClick()
        {
            GlobalMenuManager.Instance.RemoveCache("LTAwakeningInstanceSelectHud");
            base.OnCancelButtonClick();
        }
    
        private void UpdateTopAwakeningItem()
        {
            for (int i = 0; i < TopAwakeningItemList.Count; ++i)
            {
                String Id = LTAwakeningInstanceConfig.GetAwakeningItemID(type, i ).ToString();
                String temp = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetCurrencyIconById(Id);
                if (temp != null)
                {
                    TopAwakeningItemList[i].name = Id;
                    TopAwakeningItemList[i].spriteName = temp;
                }
                TopItemlookupList[i].SetData(Id, BalanceResourceUtil.MaxNum);
            }
        }
    
        private void UpdateTopAwakeningItemNum()
        {
            for (int i = 0; i < TopItemlookupList.Count; ++i)
            {
                TopItemlookupList[i].UpdateData();
            }
        }
    
        private void ResetTopAwakeningItemNum()
        {
            for (int i = 0; i < TopItemlookupList.Count; ++i)
            {
                TopItemlookupList[i].RemoveData();
            }
        }
        public void OnRuleBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTRuleUI", EB.Localizer.GetString("ID_RULE_AWAKENING_INSTANCE"));
        }
    
        //点击合成精华
        public void OnGenericTransBtnClick(Transform item)
        {
            LTAwakeningInstanceConfig.OpenCompound(int.Parse(item.name)-1);
        }
    }
}
