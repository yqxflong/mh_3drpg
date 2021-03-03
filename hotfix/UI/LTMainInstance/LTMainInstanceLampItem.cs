using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTMainInstanceLampItem : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            item = t.GetMonoILRComponent<LTShowItem>("Item");
            amountLab = t.GetComponent<UILabel>("Num/Label");
            titleLab = t.GetComponent<UILabel>("Label");
            CoinSp = t.GetComponent<UISprite>("Num/Icon");

            t.GetComponent<UIButton>("Btn").onClick.Add(new EventDelegate(() =>OnItemClick(mDMono.gameObject)));
        }

        public LTShowItem item;
        public UILabel amountLab;
        public UILabel titleLab;
        public UISprite CoinSp;
    
        private LTShowItemData data;   
    
        public void Init()
        {
            
        }
    
        public void SetTitle(string title)
        {
            titleLab.text = title;
        }
    
        public void SetItem(LTShowItemData data)
        {
            this.data = data;
            LTShowItemData tempData = new LTShowItemData(data.id, 1, data.type, false);
            item.LTItemData = tempData;
            amountLab.text =string.Format ( "x{0}",data.count);
    
            SetTilteAndIcon();
        }
    
        public void OnItemClick(GameObject obj)
        {
            int index = int.Parse(obj.name);
            FusionAudio.PostEvent("UI/General/ButtonClick",true);
            GlobalMenuManager.Instance.CloseMenu("LTMainInstanceLampView");
            LTInstanceMapModel.Instance.RequestMainPray(index, delegate
            {
                //上传友盟获得钻石，主线
                FusionTelemetry.ItemsUmengCurrency(new List<LTShowItemData>() { data }, "主线副本");
                GlobalMenuManager.Instance.Open("LTShowRewardView", new List<LTShowItemData>() {data });
    
                LTInstanceMapModel.Instance.PrayPoint -= (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("prayPointCost");
                
                GlobalMenuManager.CurGridMap_MajorDataUpdateFunc();
                Hotfix_LT.Messenger.Raise(EventName.UpDatePraypointUI);
            });
            
        }
    
        private void SetTilteAndIcon()
        {
            if (data.type == "res")
            {
                int tplID = BalanceResourceUtil.GetResID(data.id);
                Hotfix_LT.Data.EconemyItemTemplate itemTpl = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(tplID);
                titleLab.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTMainInstanceLampItem_1974"), itemTpl.Name);
                CoinSp.spriteName = BalanceResourceUtil.GetResSpriteName(data.id);
            }
            else if (data.type == "heroshard")
            {
                titleLab.text = EB.Localizer.GetString("ID_codefont_in_LTMainInstanceLampItem_2168");
                CoinSp.spriteName = BalanceResourceUtil.GetResSpriteName(data.type);
            }
        }
    }
}
