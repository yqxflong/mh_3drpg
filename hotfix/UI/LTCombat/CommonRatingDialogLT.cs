using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace Hotfix_LT.UI
{
    public class CommonRatingDialogLT : DynamicMonoHotfix
    {
    	public List<UIBuddyShowItem> m_HeroItems;
    	public GameObject m_Heros;
    	public GameObject m_VicFx;
    	public GameObject m_DefeatFx;
        public GameObject m_DefeatFx2;
        public UIGrid m_HerosGrid;
    	public bool IsWon = false;
        public bool IsShowHp = false;
        public bool IsShowTempHp = false;
    	public System.Action onShownAnimCompleted;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Heros = t.FindEx("Container/Heros").gameObject;
            m_VicFx = t.parent.FindEx("VictoryObj").gameObject;
            m_DefeatFx = t.parent.FindEx("DefeatObj").gameObject;
            m_DefeatFx2 = t.parent.FindEx("DefeatObj2").gameObject;
            m_HerosGrid = t.GetComponent<UIGrid>("Container/Heros/Heros_Grid");
            IsWon = false;
            IsShowHp = false;
            IsShowTempHp = false;

            m_HeroItems = new List<UIBuddyShowItem>();

            for (var i = 0; i < m_HerosGrid.transform.childCount; i++)
            {
                m_HeroItems.Add(m_HerosGrid.transform.GetChild(i).GetMonoILRComponent<UIBuddyShowItem>());
            }

            t.GetComponent<TweenScale>("Container/Heros/Heros_Grid/0_Pos").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/Heros/Heros_Grid/0_Pos/BuddyBaseStats").OnTSFinished));
            t.GetComponent<TweenScale>("Container/Heros/Heros_Grid/0_Pos (1)").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/Heros/Heros_Grid/0_Pos (1)/BuddyBaseStats").OnTSFinished));
            t.GetComponent<TweenScale>("Container/Heros/Heros_Grid/0_Pos (2)").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/Heros/Heros_Grid/0_Pos (2)/BuddyBaseStats").OnTSFinished));
            t.GetComponent<TweenScale>("Container/Heros/Heros_Grid/0_Pos (3)").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/Heros/Heros_Grid/0_Pos (3)/BuddyBaseStats").OnTSFinished));
            t.GetComponent<TweenScale>("Container/Heros/Heros_Grid/0_Pos (4)").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/Heros/Heros_Grid/0_Pos (4)/BuddyBaseStats").OnTSFinished));
            t.GetComponent<TweenScale>("Container/Heros/Heros_Grid/0_Pos (5)").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/Heros/Heros_Grid/0_Pos (5)/BuddyBaseStats").OnTSFinished));
        }

        public override void OnEnable()
    	{
    		Reset();
    
    		m_VicFx.CustomSetActive(IsWon);
            if (IsShowTempHp&& LTHotfixManager.GetManager<SceneManager>().GetChallengeHasLive())
            {
                m_DefeatFx2.CustomSetActive(!IsWon);
            }
            else
            {
                m_DefeatFx.CustomSetActive(!IsWon);
            }
    
            InitRating();
    		StartCoroutine(DoShownVictoryAnimation());
    	}

        public override void OnDisable()
    	{
            IsWon = false;
            StopAllCoroutines();
    	}
    
    	void Reset()
    	{
            mDMono.transform.localScale = Vector3.one;
    		m_Heros.transform.localScale = new Vector3(0.01f, 0.01f, 1.0f);
    		m_Heros.CustomSetActive(false);
    		m_VicFx.CustomSetActive(false);
    		m_DefeatFx.CustomSetActive(false);
            m_DefeatFx2.CustomSetActive(false);
    
        }
    
    	/// <summary>
    	/// 
    	/// </summary>
    	void InitRating()
    	{
    		ShowBuddyData();
    	}
    
    	private void ShowBuddyData()
    	{
    		string teamName = FormationUtil.GetCurrentTeamName(SceneLogic.BattleType);
    
    		string teamDataPath = SmallPartnerPacketRule.USER_TEAM + "." + teamName + ".team_info";
    		ArrayList teamDatas;
    		List<string> heroIDs = new List<string>();
            if (!IsShowTempHp)
            {
                LTChallengeInstanceHpCtrl.UpdateInstanceHpFromCombat();
            }
            DataLookupsCache.Instance.SearchDataByID<ArrayList>(teamDataPath, out teamDatas);
    		if (teamDatas != null)
    		{
                for (var i = 0; i < teamDatas.Count; i++)
                {
                    var td = teamDatas[i];
                    string heroid = EB.Dot.String("hero_id", td, "");
                    if (!string.IsNullOrEmpty(heroid))
                    {
                        heroIDs.Add(heroid);
                    }
                }
            }
               
            for (int i = 0; i < m_HeroItems.Count; i++)
            {
                if (i < heroIDs.Count)
                {
                    string heroid = heroIDs[i];
                    m_HeroItems[i].mDMono.gameObject.CustomSetActive(true);
                    m_HeroItems[i].ShowUI(heroid, IsShowHp, IsShowTempHp);
                }
                else
                {
                    m_HeroItems[i].mDMono.gameObject.CustomSetActive(false);
                }
            }

            if (LTFormationDataManager.Instance.CurTeamMemberData!=null && BattleReadyHudController.ShowMercenary())
            {
	            m_HeroItems[heroIDs.Count].mDMono.gameObject.CustomSetActive(true);
	            m_HeroItems[heroIDs.Count].ShowUI(LTFormationDataManager.Instance.CurTeamMemberData, false, false);
            }
    	}
    
    	IEnumerator DoShownVictoryAnimation()
    	{
    		yield return new WaitForEndOfFrame();
    		OnVictoryAnimFinished();
    		yield break;
    	}
    
    	void OnVictoryAnimFinished()
    	{
    		StartCoroutine(DoHerosAnimation());
    	}
    
    	IEnumerator DoHerosAnimation()
    	{
    		yield return new WaitForEndOfFrame();
    		m_Heros.transform.localPosition = new Vector3(0.0f, -280.0f, 0.0f);
    		m_Heros.transform.localScale = new Vector3(0.01f, 0.01f, 1.0f);
    		m_Heros.SetActive(true);
    		m_HerosGrid.Reposition();

			m_Heros.transform.DOScale(new Vector2(1f, 1f), 0.2f).SetEase(Ease.Linear);
			yield return new WaitForSeconds(0.5f);

			var option = m_Heros.transform.DOLocalMoveY(-140f, 0.2f);
			option.SetEase(Ease.Linear);
			option.onComplete = OnHerosAnimFinished;
			yield break;
    	}
    
    	void OnHerosAnimFinished()
    	{
            if (mDMono.gameObject != null && mDMono.gameObject.activeSelf)
            {
                StartCoroutine(DoDropAnimation());
            }
    	}
    
    	IEnumerator DoDropAnimation()
    	{
    		yield return new WaitForEndOfFrame();
    		if (onShownAnimCompleted != null)
    		{
    			onShownAnimCompleted();
    		}
    		yield break;
    	}
    
    	public static IEnumerator DoUICameraShake(UICamera camera, Vector3[] offsets, float delay)
    	{
    		yield return new WaitForSeconds(delay);
    
    		Vector3 org_pos = camera.transform.localPosition;
    
    		int offset_len = offsets.Length;
    		for (int i = 0; i < offset_len; i++)
    		{
    			camera.transform.localPosition = org_pos + offsets[i];
    
    			yield return new WaitForEndOfFrame();
    		}
    		camera.transform.localPosition = org_pos;
    
    		yield break;
    	}
    }
}
