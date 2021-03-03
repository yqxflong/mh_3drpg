
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTSSRWishPartnerHudCtrl : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }
        public LTSSRWishScroll Scroll;
      
        
        private BattleReadyTitle battleReady;
        
        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            Scroll =t.Find("Anchor_Mid/Content/Partners/Placeholder/Grid").GetMonoILRComponent<LTSSRWishScroll>();
            controller.backButton = t.Find("Anchor_Mid/Content/Top/CloseBtn").GetComponent<UIButton>();
            
            battleReady = t.GetMonoILRComponent<BattleReadyTitle>("Anchor_Mid/Content/Top/Title");
            
            t.GetComponent<UIButton>("Anchor_Mid/Content/Top/BtnList/FengBtn").onClick.Add(new EventDelegate(
                () =>
                {
                    battleReady.OnTitleBtnClick(t.FindEx("Anchor_Mid/Content/Top/BtnList/FengBtn/Sprite").gameObject);
                    RefreshPartnerList(eAttrTabType.Feng);
                }));
            t.GetComponent<UIButton>("Anchor_Mid/Content/Top/BtnList/HuoBtn").onClick.Add(new EventDelegate(
                () =>
                {
                    battleReady.OnTitleBtnClick(t.FindEx("Anchor_Mid/Content/Top/BtnList/HuoBtn/Sprite").gameObject);
                    RefreshPartnerList(eAttrTabType.Huo);
                }));
            t.GetComponent<UIButton>("Anchor_Mid/Content/Top/BtnList/ShuiBtn").onClick.Add(new EventDelegate(
                () =>
                {
                    battleReady.OnTitleBtnClick(t.FindEx("Anchor_Mid/Content/Top/BtnList/ShuiBtn/Sprite").gameObject);
                    RefreshPartnerList(eAttrTabType.Shui);
                }));
        }
        
        public override void SetMenuData(object param)
        {
            battleReady.OnTitleBtnClick(controller.transform.FindEx("Anchor_Mid/Content/Top/BtnList/FengBtn/Sprite").gameObject);
            RefreshPartnerList(eAttrTabType.Feng);
        }
        
        private void RefreshPartnerList(eAttrTabType tabType)
        {
            HeroInfoTemplate[] AllInfo = CharacterTemplateManager.Instance.GetHeroInfos();
            List<HeroInfoTemplate> temp = new List<HeroInfoTemplate>();
            for (int i = 0; i < AllInfo.Length; i++)
            {
                if (AllInfo[i].init_star == 3 && AllInfo[i].draw == 2)
                {
                    if (tabType == eAttrTabType.All)
                        temp.Add(AllInfo[i]);
                    else if (tabType == eAttrTabType.Feng && AllInfo[i].char_type == eRoleAttr.Feng)
                        temp.Add(AllInfo[i]);
                    else if (tabType == eAttrTabType.Huo && AllInfo[i].char_type == eRoleAttr.Huo)
                        temp.Add(AllInfo[i]);
                    else if (tabType == eAttrTabType.Shui && AllInfo[i].char_type == eRoleAttr.Shui)
                        temp.Add(AllInfo[i]);
                }
            }
            LTDrawCardLookupController.DrawType = DrawCardType.none;
            Scroll.SetItemDatas(temp);
        }


        public override void OnCancelButtonClick()
        {
            base.OnCancelButtonClick();
            
        }
    }
}