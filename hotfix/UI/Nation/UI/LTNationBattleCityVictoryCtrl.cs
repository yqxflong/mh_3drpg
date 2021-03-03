using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTNationBattleCityVictoryCtrl : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            VictoryCountryIcon = t.GetComponent<UISprite>("VictoryCountry/CountryIcon");
            LoseCountryIcon = t.GetComponent<UISprite>("LoseCountry/CountryIcon");
            VictoryFlagLeft = t.GetComponent<CampaignTextureCmp>("VictoryCountry/Texture");
            VictoryFlagRight = t.GetComponent<CampaignTextureCmp>("VictoryCountry/Texture (1)");
            LoseFlagLeft = t.GetComponent<CampaignTextureCmp>("LoseCountry/Texture");
            LoseFlagRight = t.GetComponent<CampaignTextureCmp>("LoseCountry/Texture (1)");
            VictoryCountryName = t.GetComponent<UILabel>("VictoryCountry/Name");
            VictoryCountryNameShd = t.GetComponent<UILabel>("VictoryCountry/Name/Label");
            LoseCountryName = t.GetComponent<UILabel>("LoseCountry/Name");
            LoseCountryNameShd = t.GetComponent<UILabel>("LoseCountry/Name/Label");
            FloatFontTweenerGO = t.FindEx("FloatFont").gameObject;

        }
    	public override bool ShowUIBlocker
    	{
    		get
    		{
    			return true;
    		}
    	}
    
    	public UISprite VictoryCountryIcon;
        public UISprite LoseCountryIcon;
        public CampaignTextureCmp VictoryFlagLeft;
        public CampaignTextureCmp VictoryFlagRight;
        public CampaignTextureCmp LoseFlagLeft;
        public CampaignTextureCmp LoseFlagRight;
        public UILabel VictoryCountryName;
        public UILabel VictoryCountryNameShd;
        public UILabel LoseCountryName;
        public UILabel LoseCountryNameShd;
    	public GameObject FloatFontTweenerGO;
    	bool FloatFontOver;
    
        public override void SetMenuData(object param)
        {
    		//Hashtable tb = param as Hashtable;
    		//eNationName winNation = (eNationName) tb["winNation"];
    		//eNationName loseNation = (eNationName)tb["loseNation"];
    		SetWinnerInfo(NationManager.Instance.WinNation);
    		SetLoserInfo(NationManager.Instance.LoseNation);
            FusionAudio.PostEvent("UI/New/GongXian", true);
            base.SetMenuData(param);
        }
    
    	public override void StartBootFlash()
    	{
    		base.StartBootFlash();
    
    		UITweener[] tweeners = controller.transform.GetComponentsInChildren<UITweener>(true);
    		float duration = 0.0f;
    		for (int i = 0; i < tweeners.Length; ++i)
    		{
    			tweeners[i].tweenFactor = 0;
    			tweeners[i].PlayForward();
    
    			duration = Mathf.Max(duration, tweeners[i].delay + tweeners[i].duration);
    		}
    
    		controller.Invoke("OnShowComplete", duration + 0.3f);
    	}
    
    	public override IEnumerator OnAddToStack()
    	{
            //SetWinnerInfo(eNationName.egypt.ToString());
            //SetLoserInfo(eNationName.roman.ToString());
    	    FusionAudio.PostEvent("UI/New/GongXian", true);
            return base.OnAddToStack();
    	}
    
    	public override IEnumerator OnRemoveFromStack()
        {
            FusionAudio.PostEvent("UI/New/GongXian", false);
            VictoryFlagLeft.spriteName = string.Empty;
            VictoryFlagRight.spriteName = string.Empty;
            LoseFlagLeft.spriteName = string.Empty;
            LoseFlagRight.spriteName = string.Empty;
            DestroySelf();
    		yield break;
            //return base.OnRemoveFromStack();
        }
    
    	public override void OnCancelButtonClick()
    	{
    		if (!FloatFontOver)
    			return;
    
    		GlobalMenuManager.Instance.CloseMenu("LTNationBattleHudUI");
    		GlobalMenuManager.Instance.Open("LTNationBattleEntryUI");
    		base.OnCancelButtonClick();
    	}
    
    	public void OnShowComplete()
    	{
    		FloatFontOver = true;
    	}
    
    	private void SetCountryInfo(UISprite CountryIcon, CampaignTextureCmp CountryFlagLeft, CampaignTextureCmp CountryFlagRight, UILabel CountryName, UILabel NameShd, string nationName)
        {
    		CountryIcon.spriteName = NationUtil.NationIcon(nationName);
    		CountryFlagLeft.spriteName = CountryFlagRight.spriteName = NationUtil.NationFlag(nationName);
    		CountryName.text = NameShd.text = NationUtil.LocalizeNationName(nationName);
        }
    
        public void SetWinnerInfo(string nationName)
        {
            SetCountryInfo(VictoryCountryIcon, VictoryFlagLeft, VictoryFlagRight, VictoryCountryName,VictoryCountryNameShd, nationName);
        }
    
        public void SetLoserInfo(string nationName)
        {
            SetCountryInfo(LoseCountryIcon, LoseFlagLeft, LoseFlagRight, LoseCountryName, LoseCountryNameShd, nationName);
        }
    }
}
