using System.Collections;
using System.Text;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class SSRWishItem : DynamicCellController<Data.HeroInfoTemplate>
    {
        public LTpartnerInfoItem InfoItem;
        public UILabel NameLabel;
        public UIButton selectBtn;
        public GameObject selectFlag;
        private HeroInfoTemplate itemData;
        
        public static readonly int ssrWishActivityId = 6522;   // SSRWish活动id：6519

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            InfoItem = t.GetMonoILRComponent<LTpartnerInfoItem>("InfoItem");
            NameLabel = t.GetComponent<UILabel>("NameLabel");
            selectBtn = t.GetComponent<UIButton>("Buttons/SelectBtn");
            selectFlag = t.Find("Buttons/SelectFlag").gameObject;
            selectBtn.onClick.Add(new EventDelegate(OnSelectBtnClick));
        }

      
        public override void Fill(HeroInfoTemplate itemData)
        {
             RefreshData(itemData);
        }

        public override void Clean()
        {
            // RefreshData(null);
        }

        private void RefreshData(HeroInfoTemplate itemData)
        {
            mDMono.gameObject.CustomSetActive(itemData!=null);
            this.itemData = itemData;
            InfoItem.Fill(itemData);
            LTUIUtil.SetText(NameLabel,itemData.name);

            bool showSelectFlag = false;
            int statId;
            if (DataLookupsCache.Instance.SearchIntByID(string.Format("tl_acs.{0}.current", ssrWishActivityId), out statId))
            {
                if (statId-1==itemData.id)
                {
                    showSelectFlag = true;
                }
            }
            selectFlag.CustomSetActive(showSelectFlag);
            selectBtn.gameObject.CustomSetActive(!showSelectFlag);
        }


        public void OnSelectBtnClick()
        {
            SendHeroWishReq(itemData.id, (id) =>
            {
                Messenger.Raise(EventName.OnSSRWishRefresh,id);
            });
            
        }
        
        private void SendHeroWishReq(int infoId, System.Action<int> callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/ssrPartnerWish");
            request.AddData("activityId", ssrWishActivityId);
            request.AddData("itemId", infoId+1);
            
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "event is not running":
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleMatchHudController_10973"));
                            TimerManager.instance.AddTimer(300, 1, timeSeq => GlobalMenuManager.Instance.CloseMenu("LTSSRWishPartnerHud"));
                            return true;
                        }
                    }
                }
                return false;
            };
            
            LTHotfixApi.GetInstance().BlockService(request, (Hashtable data) =>
            {
                DataLookupsCache.Instance.CacheData(string.Format("tl_acs.{0}.current", ssrWishActivityId), infoId+1);
                callback?.Invoke(infoId);
                TimerManager.instance.AddTimer(300, 1, timeSeq => GlobalMenuManager.Instance.CloseMenu("LTSSRWishPartnerHud"));
            });
        }

    }
}