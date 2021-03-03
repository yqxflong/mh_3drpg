using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTConcernHudController : UIControllerHotfix
    {
        public List<LTShowItem> ShowItems;
        public UIGrid ItemGrid;
        public List<LTShowItem> ShowItems2;
        public UIGrid ItemGrid2;
        public override bool IsFullscreen()
        {
            return true;
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            ItemGrid = t.GetComponent<UIGrid>("Content/ViewList/0_QQ/GiftGrid");
            ShowItems = new List<LTShowItem>();

            for (var i = 0; i < ItemGrid.transform.childCount; i++) 
            {
                ShowItems.Add(ItemGrid.transform.GetChild(i).GetMonoILRComponent<LTShowItem>());
            }

            ItemGrid2 = t.GetComponent<UIGrid>("Content/ViewList/1_Weixin/GiftGrid");
            ShowItems2 = new List<LTShowItem>();

            for (var i = 0; i < ItemGrid2.transform.childCount; i++)
            {
                ShowItems2.Add(ItemGrid2.transform.GetChild(i).GetMonoILRComponent<LTShowItem>());
            }

            controller.backButton = t.GetComponent<UIButton>("FrameBG/Panel/CancelBtn");

            t.GetComponent<UIButton>("Content/ViewList/0_QQ/BuyButton").onClick.Add(new EventDelegate(OnJoinQQGroup));
        }

        public override IEnumerator OnAddToStack()
        {
            UpdateUI(ShowItems, ItemGrid, "JoinQQGroupGift");
            UpdateUI(ShowItems2, ItemGrid2, "JoinWeixinGift");
            yield return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    
        public void UpdateUI(List<LTShowItem> items, UIGrid grid, string giftkey)
        {
            ArrayList aList = EB.JSON.Parse(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue(giftkey)) as ArrayList;// "JoinQQGroupGift")) as ArrayList;//JoinWeixinGift
            if (aList != null)
            {
                for (int i = 0; i < aList.Count; i++)
                {
                    string id = EB.Dot.String("data", aList[i], string.Empty);
                    int count = EB.Dot.Integer("quantity", aList[i], 0);
                    string type = EB.Dot.String("type", aList[i], string.Empty);
                    if (!string.IsNullOrEmpty(id))
                    {
                        items[i].LTItemData = new LTShowItemData(id, count, type, true);
                        items[i].mDMono.gameObject.CustomSetActive(true);
                    }
                }
                for (int i = aList.Count; i < items.Count; i++)
                {
                    items[i].mDMono.gameObject.CustomSetActive(false);
                }
                grid.repositionNow = true;
            }
            else
            {
                grid.gameObject.CustomSetActive(false);
            }
        }
    
        public void OnJoinQQGroup()
        {
            if (ILRDefine.USE_VFPKSDK)
            {
                if (SparxHub.Instance != null)
                {
                    if (ILRDefine.UNITY_ANDROID)
                    {
                        //TODO VFPKSDKManager获取不到 暂时屏蔽
                        // string key = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("JoinQQGroupKey");
                        // if(!string.IsNullOrEmpty( key)) SparxHub.Instance.VFPKSDKManager.OnJoinQQGroup(key);
                    }
                    
                    if (ILRDefine.UNITY_IPHONE)
                    {
                        //TODO VFPKSDKManager获取不到 暂时屏蔽
                        // string key = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("JoinQQGroupKey");
                        // SparxHub.Instance.VFPKSDKManager.OnJoinQQGroup(key);
                    }
                }
            }
        }
    
    }
}
