using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTWelfareDiamondTaskItem : DynamicMonoHotfix
    {
        private ConsecutiveClickCoolTrigger _btnBuy;
        private UISprite _btnSprite;
        private UILabel _labActivityPrice;
        private UILabel _labOriginalPrice;
        private LTShowItem _uiShowItem;
        private GameObject _diamondIcon;
        private Hotfix_LT.Data.DiamondGiftTemplate _data;

        public override void Awake()
        {
            base.Awake();

            Transform t = mDMono.transform;
            _labActivityPrice = t.GetComponent<UILabel>("Btn_Buy/Lab_Price");
            _labOriginalPrice = t.GetComponent<UILabel>("Price/Lab_Price");
            _uiShowItem = t.GetMonoILRComponent<LTShowItem>("LTShowItem");
            _btnBuy = t.GetComponent<ConsecutiveClickCoolTrigger>("Btn_Buy");
            _btnBuy.clickEvent.Add(new EventDelegate(OnBuyBtnClicked));
            _btnSprite = _btnBuy.GetComponent<UISprite>();
            _diamondIcon = _btnBuy.transform.FindEx("Sprite").gameObject;
        }

        public void InitData(Hotfix_LT.Data.DiamondGiftTemplate data)
        {
            _data = data;

            var item = data.lTShowItemData;
            _uiShowItem.LTItemData = new LTShowItemData(item.id, item.count, item.type, false);
            _labOriginalPrice.text = data.originalPrice.ToString();
            _labActivityPrice.text = data.activityPrice.ToString();

            SetBuyBtnStatus();
        }

        private void OnBuyBtnClicked()
        {
            // 拥有的钻石
            int diamondAmount;
            DataLookupsCache.Instance.SearchIntByID("res.hc.v", out diamondAmount);

            if (_data.activityPrice <= diamondAmount)
            {
                EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/sign_in/buyDiamondGift");
                request.AddData("id", _data.id);
                LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
                {
                    if (data != null)
                    {
                        FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, -_data.activityPrice, "购买七日钻石礼包");

                        DataLookupsCache.Instance.CacheData(data);
                        SetBuyBtnStatus();
                        GlobalMenuManager.Instance.Open("LTShowRewardView", new List<LTShowItemData>() { _data.lTShowItemData });
                    }
                });
            }
            else
            {
                ///MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceReviveCtrl_1696"));
                BalanceResourceUtil.HcLessMessage();
            }
        }

        private void SetBuyBtnStatus()
        {
            bool isBuy;
            DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.diamond_gift." + _data.id, out isBuy);
            var pos = _labActivityPrice.transform.localPosition;

            if (isBuy)
            {
                SetInfo(
                    new Color(1, 0, 1),
                    "Ty_Button_2",
                    false,
                    EB.Localizer.GetString("ID_PURCHASED"),
                    new Vector3(0f, pos.y, pos.z),
                    false
                );
            }
            else
            {
                if (_data.days == LTWelfareDiamondController.nextDayIndex + 1)
                {
                    SetInfo(
                        new Color(1, 0, 1),
                        "Ty_Button_2",
                        false,
                        EB.Localizer.GetString("ID_TOMORROW_UNLOCK"),
                        new Vector3(0f, pos.y, pos.z),
                        false
                    );
                }
                else
                {
                    SetInfo(
                        new Color(1, 1, 1),
                        "Ty_Button_3",
                        true,
                        _data.activityPrice.ToString(),
                        new Vector3(36f, pos.y, pos.z),
                        true
                    );
                }
            }
        }

        private void SetInfo(Color color, string spriteName, bool enableBtn, string text, Vector3 pos, bool showDiamond)
        {
            _btnSprite.color = color;
            _btnSprite.spriteName = spriteName;
            _btnBuy.GetComponent <BoxCollider >().enabled  = enableBtn;
            _labActivityPrice.text = text;
            _labActivityPrice.transform.localPosition = pos;
            _diamondIcon.SetActive(showDiamond);
        }
    }
}
