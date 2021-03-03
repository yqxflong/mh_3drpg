using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Hotfix_LT.UI
{
    public class NationBattleRatingDialogController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            m_Heros = t.FindEx("Container/Heros").gameObject;
            m_Title = t.GetComponent<UILabel>("Container/Title");
            m_HerosGrid = t.GetComponent<UIGrid>("Container/Heros/Heros_Grid");
            ResidueTweener = t.GetComponent<UITweener>("Container/Result/ResidueHp");
            CityDefendTweener = t.GetComponent<UITweener>("Container/Result/CityDefend");
            GainDonateTweener = t.GetComponent<UITweener>("Container/Result/GainDonate");
            ResidueHpLabel = t.GetComponent<UILabel>("Container/Result/ResidueHp/Value");
            CityDefendTitleLabel = t.GetComponent<UILabel>("Container/Result/CityDefend/Title");
            CityDamageLabel = t.GetComponent<UILabel>("Container/Result/CityDefend/Value");
            GainDonateLabel = t.GetComponent<UILabel>("Container/Result/GainDonate/Value");
            ContinueTipsLabel = t.GetComponent<UILabel>("Container/Tip");
            m_HeroItems = new List<UIBuddyShowItem>();
            m_HeroItems.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Container/Heros/Heros_Grid/0_Pos"));
            m_HeroItems.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Container/Heros/Heros_Grid/0_Pos (1)"));
            m_HeroItems.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Container/Heros/Heros_Grid/0_Pos (2)"));
            m_HeroItems.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Container/Heros/Heros_Grid/0_Pos (3)"));
            m_HeroItems.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Container/Heros/Heros_Grid/0_Pos (4)"));
            m_HeroItems.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Container/Heros/Heros_Grid/0_Pos (5)"));
            m_HeroItems.Add(t.GetMonoILRComponent<UIBuddyShowItem>("Container/Heros/Heros_Grid/0_Pos (5)"));
            DelayHeroTweenAnimTime = 0.2f;
            ShowResultTweenInterval = 0.3f;
            t.GetComponent<UIEventTrigger>("Bg").onClick.Add(new EventDelegate(OnCancelButtonClick));
            Reset();
        }


        
    	public List<UIBuddyShowItem> m_HeroItems;
    	public GameObject m_Heros;
    	public UILabel m_Title;
    	public UIGrid m_HerosGrid;
    
    	public UITweener ResidueTweener;
    	public UITweener CityDefendTweener;
    	public UITweener GainDonateTweener;
    	public UILabel ResidueHpLabel;
    	public UILabel CityDefendTitleLabel;
    	public UILabel CityDamageLabel;
    	public UILabel GainDonateLabel;
    	public UILabel ContinueTipsLabel;
    	public float DelayHeroTweenAnimTime;
    	public float ShowResultTweenInterval=0.2f;
    	bool IsShowOver=false;
    	bool IsAttack;
        private System.Object mParam;

        void Reset()
    	{
    		m_Heros.transform.localScale = new Vector3(0.01f, 0.01f, 1.0f);
    		m_Heros.CustomSetActive(false);
            m_Title.gameObject.CustomSetActive(false);
    	}
    
    
    	public override void SetMenuData(object param)
    	{
    		base.SetMenuData(param);
            mParam = param;

        }
    
    	public override IEnumerator OnAddToStack()
    	{
    		yield return base.OnAddToStack();
    
    		if (NationBattleHudController.Instance != null)
    			IsAttack = NationBattleHudController.TerritoryData.ADType == eTerritoryAttackOrDefendType.Attack;
    		InitRating();
    		StartCoroutine(DoShownVictoryAnimation());
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
            m_Title.gameObject.CustomSetActive(false);
            ContinueTipsLabel.gameObject.CustomSetActive(false);
            ResidueTweener.gameObject.CustomSetActive(false);
            CityDefendTweener.gameObject.CustomSetActive(false);
            GainDonateTweener.gameObject.CustomSetActive(false);
            m_Heros.CustomSetActive(false);
            IsShowOver = false;
            StopAllCoroutines();
            DestroySelf();
    		yield break;
    	}
    
    	void InitRating()
    	{
    		ShowBuddyData();
    	}
    
    	private void ShowBuddyData()
    	{
    		Hashtable ht = mParam as Hashtable;
    		string teamName = ht["useTeam"].ToString();
    		if (string.IsNullOrEmpty(teamName))
    			teamName = NationManager.Instance.CurrentGoOnTeamName;
    
    		string teamDataPath = SmallPartnerPacketRule.USER_TEAM + "." + teamName + ".team_info";
    		ArrayList teamDatas;
    		List<string> heroIDs = new List<string>();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>(teamDataPath, out teamDatas);
            for (var i = 0; i < teamDatas.Count; i++)
            {
                var td = teamDatas[i];
                string heroid = EB.Dot.String("hero_id", td, "");
                if (!string.IsNullOrEmpty(heroid))
                {
                    heroIDs.Add(heroid);

                }
            }
    
            for (int i = 0; i < m_HeroItems.Count; i++)
    		{
    			if (i < heroIDs.Count)
    			{
    				string heroid = heroIDs[i];
    				m_HeroItems[i].mDMono.gameObject.CustomSetActive(true);
    				m_HeroItems[i].ShowUI(heroid, false);
    			}
    			else
    			{
    				m_HeroItems[i].mDMono.gameObject.CustomSetActive(false);
    			}
    		}
    	}
    
    	IEnumerator DoShownVictoryAnimation()
    	{
    		Hashtable ht = mParam as Hashtable;
    		int changeCityDefend = int.Parse(ht["changeCityDefend"].ToString());
    	    FusionAudio.PostEvent("UI/New/GuoZhanShengLi", true);
            LTUIUtil.SetText(m_Title,(IsAttack ? EB.Localizer.GetString("ID_codefont_in_NationBattleRatingDialogController_2704_1"): EB.Localizer.GetString("ID_codefont_in_NationBattleRatingDialogController_2704_2")));
            m_Title.gameObject.CustomSetActive(true);
    		yield return new WaitForSeconds(DelayHeroTweenAnimTime);
    		yield return StartCoroutine(DoHerosAnimation());
    	}
    
    	IEnumerator DoHerosAnimation()
    	{
    		yield return new WaitForEndOfFrame();
    		m_Heros.transform.localPosition = new Vector3(0.0f, -16.0f, 0.0f);
    		m_Heros.transform.localScale = new Vector3(0.01f, 0.01f, 1.0f);
    		m_Heros.CustomSetActive(true);
    		m_HerosGrid.Reposition();

			m_Heros.transform.DOScale(new Vector2(1f, 1f), 0.2f).SetEase(Ease.Linear);
			yield return new WaitForSeconds(0.5f);

			var option = m_Heros.transform.DOLocalMoveY(150.0f, 0.2f);
			option.SetEase( Ease.Linear );
			option.onComplete = OnHerosAnimFinished;
			
    		yield break;
    	}
    
    	void OnHerosAnimFinished()
    	{
    		if(controller.gameObject.activeInHierarchy)StartCoroutine(ShowResult());		
    	}
    
    	IEnumerator ShowResult()
    	{
    		Hashtable ht = mParam as Hashtable;
    	    float residueHpPercentNum = (float) ht["residueHp"] * 100;
            string residueHpPercent = residueHpPercentNum.ToString("0.##") + "%";
    	    int changeCityDefend = (int)ht["changeCityDefend"];
            int degree = (int)(ht["degree"]);
    		LTUIUtil.SetText(ResidueHpLabel, residueHpPercent);
    		PlayTweenAnim(ResidueTweener);
    		yield return new WaitForSeconds(ShowResultTweenInterval);
    		LTUIUtil.SetText(CityDefendTitleLabel, !IsAttack ? EB.Localizer.GetString("ID_codefont_in_NationBattleRatingDialogController_4067_1"): EB.Localizer.GetString("ID_codefont_in_NationBattleRatingDialogController_4067_2"));
    		LTUIUtil.SetText(CityDamageLabel, Mathf.Abs(changeCityDefend).ToString());
    		PlayTweenAnim(CityDefendTweener);
    		yield return new WaitForSeconds(ShowResultTweenInterval);
    		LTUIUtil.SetText(GainDonateLabel, degree.ToString());
    		PlayTweenAnim(GainDonateTweener);
    		yield return new WaitForSeconds(ShowResultTweenInterval);
    
    		ContinueTipsLabel.gameObject.CustomSetActive(true);
    		IsShowOver = true;
    	}
    
    	void PlayTweenAnim(UITweener tween)
    	{
    		tween.gameObject.CustomSetActive(true);
    		tween.ResetToBeginning();
    		tween.PlayForward();
    	}
    
    	public override void OnCancelButtonClick()
    	{
    		if (!IsShowOver)
    			return;
    		base.OnCancelButtonClick();
    	}
    }
}
