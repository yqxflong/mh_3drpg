using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTBossChallengeStoreController : UIControllerHotfix
    {
        private bool isPlayedAnimation = false;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("UINormalFrameBG/CancelBtn");
            t.GetComponent<TweenPosition>("Store/NewBlacksmithView").onFinished.Add(new EventDelegate(() => { FxClipFun(); }));
        }

        private string _shopType;

        public override void SetMenuData(object param)
        {
            if (param != null)
            {
                _shopType = param.ToString();
                SetCurrency(_shopType);
                RequestData(_shopType);
                OnRefreshCallback(_shopType);
            }
        }

        private void OnRefreshCallback(string shopType)
        {
            var refresh = controller.transform.GetDataLookupILRComponent<RefreshFromDataLookup>("UINormalFrameBG/NewCurrency");

            if (refresh != null)
            {
                refresh.SetCallback(() => SetCurrency(shopType));
            }
        }

        private void RequestData(string shopType)
        {
            var t = controller.transform.FindEx("Store/NewBlacksmithView/BlacksmithViews");

            if (t == null)
            {
                return;
            }

            for (var i = 0; i < t.childCount; i++)
            {
                var child = t.GetChild(i);

                if (child.name.Equals("Shared"))
                {
                    continue;
                }

                child.gameObject.SetActive(child.name.Equals(shopType));
            }
        }

        public override bool IsFullscreen()
        {
            return true;
        }
    
        public override IEnumerator OnAddToStack() {
    
            yield return base.OnAddToStack();
            yield return null;
            StartCoroutine(StartAnimation());
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            TweenPosition tweener = controller.GObjects["mAniamtionTarget"].GetComponent<TweenPosition>();
            tweener.transform.localPosition = tweener.from;
            isPlayedAnimation = false;
            StopAllCoroutines();
            DestroySelf();

            // 离开首领兑换商店时保存可购买的商品数量
            PlayerPrefs.SetInt(LTActivityBodyItem_BossChallenge.CanBuyCountFromShopKey, LTActivityBodyItem_BossChallenge.GetCanBuyCount(_shopType));
            PlayerPrefs.Save();
            yield break;
        }       
    
    	IEnumerator StartAnimation()
    	{
    		yield return null;
    		if (isPlayedAnimation) yield break; ;
    		isPlayedAnimation = true;
            TweenPosition tweener = controller.GObjects["mAniamtionTarget"].GetComponent<TweenPosition>();
            tweener.ResetToBeginning();
            tweener.PlayForward();
        }

        public void FxClipFun()
        {
            EffectClip[] clips = controller.GObjects["mAniamtionTarget"].transform.GetComponentsInChildren<EffectClip>();
            for (int i = 0; i < clips.Length; i++)
            {
                clips[i].Init();
            }
        }

        private CurrencyDisplay _currencyDisplay1;
        private CurrencyDisplay _currencyDisplay2;

        private void SetCurrency(string shopType)
        {
            var shopTemplate = Data.ShopTemplateManager.Instance.GetShopByShopType(shopType);

            if (shopTemplate == null)
            {
                EB.Debug.LogError("LTBossChallengeStoreController.SetCurrency: shopTemplate is null");
                return;
            }

            string[] strs = shopTemplate.shop_balance_type.Split(',');

            if (_currencyDisplay1 == null)
            {
                _currencyDisplay1 = controller.transform.GetMonoILRComponent<CurrencyDisplay>("UINormalFrameBG/NewCurrency/Table/Currency_1");
            }

            if (_currencyDisplay1 != null && strs != null && strs.Length > 0)
            {
                var count = BalanceResourceUtil.NumFormat(BalanceResourceUtil.GetDataLookupValue(string.Format("res.{0}.v", strs[0])).ToString());
                _currencyDisplay1.SetData(count, BalanceResourceUtil.GetResSpriteName(strs[0]));
				_currencyDisplay1.SetPopTip(LTShowItemType.TYPE_RES, strs[0]);
			}

            if (_currencyDisplay2 == null)
            {
                _currencyDisplay2 = controller.transform.GetMonoILRComponent<CurrencyDisplay>("UINormalFrameBG/NewCurrency/Table/Currency_2");
            }

            if (_currencyDisplay2 != null && strs != null && strs.Length > 1)
            {
                var count = BalanceResourceUtil.NumFormat(BalanceResourceUtil.GetDataLookupValue(string.Format("res.{0}.v", strs[1])).ToString());
                _currencyDisplay2.SetData(count, BalanceResourceUtil.GetResSpriteName(strs[1]));
				_currencyDisplay2.SetPopTip(LTShowItemType.TYPE_RES, strs[1]);
			}
        }
    }
}
