using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
    * Campaign Rating data format
    campaignRating
    {
        "deaths" : 0.0,    
        "inflicted" : 14071.0,    
        "rating" : 
        {
            "deaths" : 0.0,        
            "campaign" : 2.0,        
            "inflicted" : 0.0,        
            "taken" : 2.0
        },    
        "taken" : 5861.0
    }
    * 
    */
    
//胜利图标 副本名称  星级评价  
//游戏币奖励
//经验奖励
//物品奖励
namespace Hotfix_LT.UI
{
    public class CampaignRatingDialogMH : DynamicMonoHotfix
    {
        public GameObject m_XP;
        public UIGrid m_XPGrid;
        public GameObject m_Victory;
        public GameObject m_Defeat;
        public GameObject m_Stars;
        public List<UIBuddyShowItem> m_BuddyShowItems;
        //public List<CamapignVictoryExpDataSet> m_ExpSetItems;
        public bool IsWon;
        public string m_RewardListDataID;

        public System.Action onShownAnimCompleted;
        //Camera Shake offsets
        public Vector3[] m_RankShakeOffsets = new Vector3[] { new Vector3(2.0f, 2.0f, 0.0f), new Vector3(-2.0f, -2.0f, 0.0f) };
        public Vector3[] m_MedalShakeOffsets = new Vector3[] { new Vector3(10.0f, 10.0f, 0.0f), new Vector3(-10.0f, -10.0f, 0.0f) };

        public bool IsShowHp = false;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_XP = t.FindEx("Container/XP/XPGrid").gameObject;
            m_XPGrid = t.GetComponent<UIGrid>("Container/XP/XPGrid");
            m_Victory = t.parent.FindEx("VictoryObj").gameObject;
            m_Defeat = t.parent.FindEx("DefeatObj").gameObject;
            m_Stars = t.FindEx("Container/Stars").gameObject;
            IsWon = false;
            IsShowHp = false;
            m_RewardListDataID = "combat.rewards[0].rewardItems";
            m_BuddyShowItems = new List<UIBuddyShowItem>();

            for (var i = 0; i < m_XP.transform.childCount; i++)
            {
                m_BuddyShowItems.Add(m_XP.transform.GetChild(i).GetMonoILRComponent<UIBuddyShowItem>());
            }

            t.GetComponent<TweenScale>("Container/XP/XPGrid/0_Pos").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/XP/XPGrid/0_Pos/BuddyBaseStats").OnTSFinished));
            t.GetComponent<TweenScale>("Container/XP/XPGrid/0_Pos (1)").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/XP/XPGrid/0_Pos (1)/BuddyBaseStats").OnTSFinished));
            t.GetComponent<TweenScale>("Container/XP/XPGrid/0_Pos (2)").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/XP/XPGrid/0_Pos (2)/BuddyBaseStats").OnTSFinished));
            t.GetComponent<TweenScale>("Container/XP/XPGrid/0_Pos (3)").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/XP/XPGrid/0_Pos (3)/BuddyBaseStats").OnTSFinished));
            t.GetComponent<TweenScale>("Container/XP/XPGrid/0_Pos (4)").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/XP/XPGrid/0_Pos (4)/BuddyBaseStats").OnTSFinished));
            t.GetComponent<TweenScale>("Container/XP/XPGrid/0_Pos (5)").onFinished.Add(new EventDelegate(t.GetMonoILRComponent<CamapignVictoryExpDataSet>("Container/XP/XPGrid/0_Pos (5)/BuddyBaseStats").OnTSFinished));
        }

        public override void OnEnable()
        {
            Reset();
    		StartCoroutine(DoShownVictoryAnimation());
    	}

        public override void OnDisable()
        {
            StopAllCoroutines();
        }
    
        void Reset()
        {
            mDMono.transform.localScale = Vector3.one;
            m_XP.CustomSetActive(false);
            m_Victory.CustomSetActive(false);
    		m_Defeat.CustomSetActive(false);
    		m_Stars.CustomSetActive(false);
        }
    
    	private void ShowBuddyData()
    	{
    		string teamName=FormationUtil.GetCurrentTeamName(SceneLogic.BattleType);		
    
    		string teamDataPath = SmallPartnerPacketRule.USER_TEAM + "." + teamName + ".team_info";
    		ArrayList teamDatas;
    		List<string> heroIDs=new List<string>();
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
    
            for (int i = 0; i < m_BuddyShowItems.Count; i++)
            {
                if (m_BuddyShowItems[i] == null)
                {
                    continue;
                }
                if (i < heroIDs.Count)
                {
                    string heroid = heroIDs[i];
                    m_BuddyShowItems[i].mDMono.gameObject.CustomSetActive(true);
                    m_BuddyShowItems[i].ShowUI(heroid, IsShowHp);
                }
                else
                {
                    m_BuddyShowItems[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
        }
    
        IEnumerator DoShownVictoryAnimation()
        {
            yield return new WaitForEndOfFrame();
    		m_Defeat.CustomSetActive(!IsWon);
    		m_Victory.CustomSetActive(IsWon);
            yield return new WaitForSeconds(0.6f);
            OnFXAnimFinished();
            yield break;
        }
    
        void OnFXAnimFinished()
        {
            ShowXP();
        }
    
        private void ShowXP()
        {
    		ShowBuddyData();
    		m_XP.CustomSetActive(true);
            m_XPGrid.Reposition();
            OnXPAnimFinished();
        }
    
        void OnXPAnimFinished()
        {
            StartCoroutine(DoShownStarAnimation());
        }
    
        private WaitForSeconds wait08 = new WaitForSeconds(0.8f);
        IEnumerator DoShownStarAnimation()
    	{
    		if (m_Stars == null) yield break;
    
    		int stars = 0;
    		DataLookupsCache.Instance.SearchIntByID("stars", out stars);
    		if (stars < 0)
    		{
    			m_Stars.CustomSetActive(false);
    			yield break;
    		}
    
    		yield return wait08;
    		m_Stars.CustomSetActive(true);
    		m_Stars.GetMonoILRComponent<CampaignStarController>().LightStarsAnimation(stars);
    
    		if (onShownAnimCompleted != null)
    		{
    			onShownAnimCompleted();
    		}
    		yield break;
    	}
    
    	public static IEnumerator DoUICameraShake(UICamera camera, Vector3[] offsets, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (camera == null) yield break;
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
    
        IEnumerator SpawnMedalVFX(GameObject vfxPrefab, GameObject medal, float delay)
        {
            if (vfxPrefab == null)
            {
                yield break;
            }
            yield return new WaitForSeconds(delay);
    
            GameObject vfxObj = GameObject.Instantiate(vfxPrefab, Vector3.zero, Quaternion.identity) as GameObject;
    
            if (vfxObj != null)
            {
                vfxObj.transform.parent = medal.transform;
                vfxObj.transform.localScale = Vector3.one;
                vfxObj.transform.localPosition = Vector3.zero;
                vfxObj.transform.localRotation = Quaternion.identity;
                yield return new WaitForSeconds(vfxObj.GetComponent<ParticleSystem>().duration);
                Object.Destroy(vfxObj);
                yield break;
            }
            else
            {
                yield break;
            }
        }
    
        string GetStarRatingText(int star)
        {
            switch(star)
            {
                case 1:
                    return EB.Localizer.GetString("ID_RATING_STAR_1");
                case 2:
                    return EB.Localizer.GetString("ID_RATING_STAR_2");
                case 3:
                    return EB.Localizer.GetString("ID_RATING_STAR_3");
                default:
                    return EB.Localizer.GetString("ID_RATING_STAR_1");
            }
        }
    }
}
