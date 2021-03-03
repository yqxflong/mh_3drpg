using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class ArenaRatingDialogLT : DynamicMonoHotfix
    {
    	public GameObject m_VictoryGO;
        public GameObject m_DefeatGO;
        public GameObject m_UpSprite;
        public GameObject m_DrawSprite;
        public UILabel m_PreRankLabel;
        public UILabel m_CurrentRankLabel;
        public UILabel m_TimerLabel;
        public UILabel m_ContinueTips;
        public List<RewardCardItem> m_CardList;
    	public bool IsWon;
    	public bool DrawOk;
    	public int m_Timer = 15;
    	public System.Action onShownAnimCompleted;
        public GameObject BgBtn;
        public GameObject CardView;
        public UIGrid ArenaTeam;
        public List<LTPartnerListCellController> TeamHeroItem;
        private List<int> TeamHerosID;
    
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_VictoryGO = t.parent.FindEx("VictoryObj").gameObject;
            m_DefeatGO = t.parent.FindEx("DefeatObj").gameObject;
            m_UpSprite = t.FindEx("Container/RankResult/GOTO").gameObject;
            m_DrawSprite = t.FindEx("Container/RankResult/Draw").gameObject;
            m_PreRankLabel = t.GetComponent<UILabel>("Container/RankResult/From");
            m_CurrentRankLabel = t.GetComponent<UILabel>("Container/RankResult/To");
            m_TimerLabel = t.GetComponent<UILabel>("CardView/TimerLabel");
            m_ContinueTips = t.GetComponent<UILabel>("Container/Tip");

            var cardList = t.FindEx("CardView/CardList");
            m_CardList = new List<RewardCardItem>();
            
            for (var i = 0; i < cardList.childCount; i++)
            {
                m_CardList.Add(cardList.GetChild(i).GetMonoILRComponent<RewardCardItem>());
            }

            IsWon = false;
            DrawOk = false;
            m_Timer = 15;
            BgBtn = t.FindEx("Container").gameObject;
            CardView = t.FindEx("CardView").gameObject;
            CardView.SetActive(false);
            ArenaTeam = t.GetComponent<UIGrid>("Container/Grid");

            TeamHeroItem = new List<LTPartnerListCellController>();

            for (var i = 0; i < ArenaTeam.transform.childCount; i++)
            {
	            TeamHeroItem.Add(ArenaTeam.transform.GetChild(i).GetMonoILRComponent<LTPartnerListCellController>());
            }

            t.GetComponent<ConsecutiveClickCoolTrigger>("Container").clickEvent.Add(new EventDelegate(OnBgBtnClick));

            t.GetComponent<UIEventTrigger>("CardView/CardList/0/CardNoOpen/Border").onClick.Add(new EventDelegate(() => OnCardClick(t.GetMonoILRComponent<RewardCardItem>("CardView/CardList/0"))));
            t.GetComponent<UIEventTrigger>("CardView/CardList/1/CardNoOpen/Border").onClick.Add(new EventDelegate(() => OnCardClick(t.GetMonoILRComponent<RewardCardItem>("CardView/CardList/1"))));
            t.GetComponent<UIEventTrigger>("CardView/CardList/2/CardNoOpen/Border").onClick.Add(new EventDelegate(() => OnCardClick(t.GetMonoILRComponent<RewardCardItem>("CardView/CardList/2"))));

         }

        public override void OnEnable()
    	{
    		m_VictoryGO.CustomSetActive(IsWon);
    		m_DefeatGO.CustomSetActive(!IsWon);
            m_UpSprite.CustomSetActive(IsWon);
            m_DrawSprite.CustomSetActive(!IsWon);
            m_TimerLabel.gameObject.CustomSetActive(true);
    		m_ContinueTips.gameObject.CustomSetActive(false);
    
    		m_PreRankLabel.text =EB.Localizer.GetString("ID_RANK_COLON") + ((ArenaManager.Instance.Info.preRank<0)?"10000+":(ArenaManager.Instance.Info.preRank+1).ToString());
            string colorStr = (ArenaManager.Instance.Info.preRank !=ArenaManager.Instance.Info.rank) ? "[42fe79]" : null;
    		m_CurrentRankLabel.text=(ArenaManager.Instance.Info.rank < 0) ?"10000+" : (colorStr+(ArenaManager.Instance.Info.rank + 1).ToString());
            TeamHerosID = new List<int>();
            ArrayList temp;
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("userTeam.arena.team_info", out temp);
            for (var i = 0; i < temp.Count; i++)
            {
                var obj = temp[i];
                Hashtable data = obj as Hashtable;
                int id = EB.Dot.Integer("hero_id", data, 0);
                if (id!=0)  TeamHerosID.Add(id); 
            }
            List<LTPartnerData> datas = new List<LTPartnerData>();
            for (var i = 0; i < TeamHerosID.Count; i++)
            {
                int obj = TeamHerosID[i];
                datas.Add(LTPartnerDataManager.Instance.GetPartnerByHeroId(obj));
            }
            int j = datas.Count < TeamHeroItem.Count ? datas.Count : TeamHeroItem.Count;
            for (int i=0;i< j; i++)
            {
                TeamHeroItem[i].Fill(datas[i]);
            }
            ArenaTeam.Reposition();
            //StartCoroutine(Timer());
            //if (IsWon)
            //{			
            //	StartCoroutine(Timer());
            //}
            //else
            //{			
            //	SetRewardData(1);
            //	m_CardList[1].OpenCard();
            //	m_CardList[0].DisableClick();
            //	m_CardList[2].DisableClick();
            //	m_TimerLabel.gameObject.CustomSetActive(false);
            //	m_ContinueTips.gameObject.CustomSetActive(true);
            //	DrawOk = true;
            //	if (onShownAnimCompleted != null)
            //		onShownAnimCompleted();
            //}
        }

        public void OnBgBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            BgBtn.CustomSetActive(false);
            m_VictoryGO.CustomSetActive(false );
            m_DefeatGO.CustomSetActive(false);
            CardView.CustomSetActive(true);
            StartCoroutine(Timer());
        }
    
    	IEnumerator Timer()
    	{
    		m_TimerLabel.text = m_Timer.ToString();
    		while (true)
    		{
    			yield return new WaitForSeconds(1);
    			if (m_Timer > 0)
    			{
    				m_Timer--;
    				m_TimerLabel.text = m_Timer.ToString();
    			}
    			else
    			{
    				int random_index = Random.Range(0, 3);
    				OnCardClick(m_CardList[random_index]);
    				yield break;
    			}
    		}
    	}
    
        private bool hasCardClick = false;
    	public void OnCardClick(RewardCardItem cardItem)
    	{
            if (hasCardClick)
            { 
                return; 
            }

            hasCardClick = true;
            int reward_idx = int.Parse(cardItem.mDMono.gameObject.name);
    		SetRewardData(reward_idx);
    	    FusionAudio.PostEvent("UI/Card/Rotation", mDMono.gameObject, true);
    		cardItem.OnCardClick();
            bool isGoldVIP = HaveDoubleTime();
            
            // m_CardList.ForEach(card => card.DisableClick(isGoldVIP));
            for (int i = 0; i < m_CardList.Count; i++)
            {
	            m_CardList[i].DisableClick(isGoldVIP);
            }
            cardItem.OnAnimFinished = delegate () {
    			m_TimerLabel.gameObject.CustomSetActive(false);
    			StartCoroutine(OpenOtherCard(reward_idx));
    			m_ContinueTips.gameObject.CustomSetActive(true);
    			DrawOk = true;

                if (onShownAnimCompleted != null)
                {
                    onShownAnimCompleted();
                }

                if (cardItem != null && cardItem.UIItem != null)
                {
                    var item = cardItem.UIItem.LTItemData;
                    LTShowItemData data;
                    if (isGoldVIP)
                    {
                        data = new LTShowItemData(item.id, item.count*2, item.type);
                    }
                    else
                    {
                        data = new LTShowItemData(item.id, item.count, item.type);
                    }                 
                    GameUtils.ShowAwardMsg(data);
                }
    		}; 
    	}

        public bool HaveDoubleTime()
        {
	        int time = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ArenaDoubleTimes);
	        if (time > 0)
	        {
		        int useTime = ArenaManager.Instance.Info.usedTimes;
		        return time - useTime >= 0;
	        }

	        return false;
        }
    
    	IEnumerator OpenOtherCard(int reward_index)
    	{
    		yield return new WaitForSeconds(0);
    
    		int index_1 = reward_index;
    		int index_2 = 1;
    		int index_3 = 2;
    		if (index_1 == 0)
    		{
    			index_2 = 1;
    			index_3 = 2;
    		}
    		else if (index_1 == 1)
    		{
    			index_2 = 0;
    			index_3 = 2;
    		}
    		else if (index_1 == 2)
    		{
    			index_2 = 0;
    			index_3 = 1;
    		}
    		m_CardList[index_2].OpenCard();
    		m_CardList[index_3].OpenCard();

            //m_CardList[index_3].OnAnimFinished = delegate () {
            //	//m_ContinueTips.gameObject.CustomSetActive(true);
            //	//DrawOk = true;
            //};
        }

        void SetRewardData(int reward_index)
    	{
    		ArrayList rewardListObj;
    		if (!DataLookupsCache.Instance.SearchDataByID<ArrayList>("arena.info.reward", out rewardListObj, null))
    		{
    			Debug.LogError("not find arena reward");
    			return;
    		}
    
    		List<LTShowItemData> rewardItemDatas = GameUtils.ParseAwardArr(rewardListObj);
    		if (rewardItemDatas.Count != 3)
    		{
    			EB.Debug.LogError("arena awardcount error count={0}" , rewardItemDatas.Count);
    		}
    
    		int index_1 = reward_index;
    		int index_2 = 1;
    		int index_3 = 2;
    		if (index_1 == 0)
    		{
    			index_2 = 1;
    			index_3 = 2;
    		}
    		else if (index_1 == 1)
    		{
    			index_2 = 0;
    			index_3 = 2;
    		}
    		else if (index_1 == 2)
    		{
    			index_2 = 0;
    			index_3 = 1;
    		}

            var item0 = rewardItemDatas[0];
            var data0 = new LTShowItemData(item0.id, item0.count, item0.type, false);
            var item1 = rewardItemDatas[1];
            var data1 = new LTShowItemData(item1.id, item1.count, item1.type, false);
            var item2 = rewardItemDatas[2];
            var data2 = new LTShowItemData(item2.id, item2.count, item2.type, false);

            if (rewardItemDatas.Count == 1)
    		{
                m_CardList[index_1].UIItem.LTItemData = data0;
    			m_CardList[index_2].UIItem.LTItemData = data0;
    			m_CardList[index_3].UIItem.LTItemData = data0;
    		}
    		else if (rewardItemDatas.Count == 2)
    		{
    			m_CardList[index_1].UIItem.LTItemData = data0;
    			m_CardList[index_2].UIItem.LTItemData = data1;
    			m_CardList[index_3].UIItem.LTItemData = data1;
    		}
    		else if (rewardItemDatas.Count == 3)
    		{
    			m_CardList[index_1].UIItem.LTItemData = data0;
    			m_CardList[index_2].UIItem.LTItemData = data1;
    			m_CardList[index_3].UIItem.LTItemData = data2;
    		}
    		else if(rewardItemDatas.Count>0)
    		{
                m_CardList[index_1].UIItem.LTItemData = data0;
    			m_CardList[index_2].UIItem.LTItemData = data0;
    			m_CardList[index_3].UIItem.LTItemData = data0;
    		}
    	}
    }
}
