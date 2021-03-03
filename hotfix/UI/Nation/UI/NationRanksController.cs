using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class NationRanksController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            MemberDynamicScroll = t.GetMonoILRComponent<NationMemberDynamicScroll>("Content/MembersScrollView/Placeholder/Grid");
            CurrentRankFlag = t.FindEx("Content/RanksList/2/CurrentFlag").gameObject;

            SelectGOs = new GameObject[6];
            SelectGOs[0] = t.FindEx("Content/RanksList/0").gameObject;
            SelectGOs[1] = t.FindEx("Content/RanksList/1").gameObject;
            SelectGOs[2] = t.FindEx("Content/RanksList/2").gameObject;
            SelectGOs[3] = t.FindEx("Content/RanksList/3").gameObject;
            SelectGOs[4] = t.FindEx("Content/RanksList/4").gameObject;
            SelectGOs[5] = t.FindEx("Content/RanksList/5").gameObject;

            controller.backButton = t.GetComponent<UIButton>("UINormalFrame/CancelBtn");

            t.GetComponent<UIEventTrigger>("Content/RanksList/0/BG").onClick.Add(new EventDelegate(() =>OnRanksBtnClick(t.FindEx("Content/RanksList/0").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/RanksList/1/BG").onClick.Add(new EventDelegate(() =>OnRanksBtnClick(t.FindEx("Content/RanksList/1").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/RanksList/2/BG").onClick.Add(new EventDelegate(() =>OnRanksBtnClick(t.FindEx("Content/RanksList/2").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/RanksList/3/BG").onClick.Add(new EventDelegate(() =>OnRanksBtnClick(t.FindEx("Content/RanksList/3").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/RanksList/4/BG").onClick.Add(new EventDelegate(() =>OnRanksBtnClick(t.FindEx("Content/RanksList/4").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/RanksList/5/BG").onClick.Add(new EventDelegate(() =>OnRanksBtnClick(t.FindEx("Content/RanksList/5").gameObject)));

        }


        
        private bool needVoice = true;
        //public UIButton ReceiveBtn;
        //public UIShowItem[] RanksRewardItemArrs;
        public NationMemberDynamicScroll MemberDynamicScroll;
    	public GameObject CurrentRankFlag;
    	public GameObject[] SelectGOs;
    	private GameObject mHaveSelectedGO;
    	private string mSelectRank;
    
    	public override IEnumerator OnAddToStack()
    	{
    		yield return base.OnAddToStack();
    
    		int myIndex = System.Array.IndexOf(NationUtil.RankArr, NationManager.Instance.Account.Rank);
    		if (myIndex < 0)
    		{
    			Debug.LogError("myRanks index < 0 ");
    			myIndex = 0;
    		}
    		//ShowReward(myIndex);
    		CurrentRankFlag.transform.SetParent(SelectGOs[5-myIndex].transform, false);
    	    needVoice = false;
            OnRanksBtnClick(SelectGOs[5 - myIndex]);
    	    needVoice = true;
    		GameDataSparxManager.Instance.RegisterListener(NationManager.MembersDataId, OnNationMemberListListener);
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
    		GameDataSparxManager.Instance.UnRegisterListener(NationManager.MembersDataId, OnNationMemberListListener);
    
    		DestroySelf();
    		yield break;
    	}
    
    	void OnNationMemberListListener(string path, INodeData data)
    	{
    		NationMembers members = data as NationMembers;
    		if (!members.Updated)
    			return;
    		members.Updated = false;
    
    		List<NationMember> rankMembers = members.GetMemberByRank(mSelectRank);
    		MemberDynamicScroll.SetItemDatas(rankMembers.ToArray());
    	}
    
    	//void ShowReward(int rankIndex)
    	//{
    	//	Hotfix_LT.Data.NationRankRewardTemplate rankRewardTpl = Hotfix_LT.Data.EventTemplateManager.Instance.GetNationRankRewardTpl(rankIndex+1);
    	//	for (int i = 0; i < rankRewardTpl.rewardDatas.Count; ++i)
    	//	{
    	//		RanksRewardItemArrs[i].ItemData = rankRewardTpl.rewardDatas[i];
    	//	}
    	//	for (int i = rankRewardTpl.rewardDatas.Count; i < RanksRewardItemArrs.Length; ++i)
    	//	{
    	//		RanksRewardItemArrs[i].gameObject.SetActive(false);
    	//	}
    	//}
    
    	//void SetReceiveBtnState(string ranks)
    	//{
    	//	string myRanks = NationManager.Instance.Account.Rank;
    	//	bool canReceive = ranks == myRanks && !NationManager.Instance.Account.HaveReceiveRankReward;
    	//	LTUIUtil.SetGreyButtonEnable(ReceiveBtn, canReceive);
    	//	string btnText = ranks == myRanks && NationManager.Instance.Account.HaveReceiveRankReward ?"����ȡ" : "��ȡ";
    	//	LTUIUtil.SetText(ReceiveBtn.GetComponentInChildren<UILabel>(),btnText);
    	//}
    
    	public void OnRanksBtnClick(GameObject selectGO)
    	{
    	    if (needVoice)
    	    {
    	        FusionAudio.PostEvent("UI/General/ButtonClick");
            }
    		if (selectGO == mHaveSelectedGO)
    			return;
    		if (mHaveSelectedGO != null)
    		{
    			if(int.Parse(mHaveSelectedGO.gameObject.name)%2==0)
    				mHaveSelectedGO.transform.Find("BG").GetComponent<UISprite>().spriteName = "Ty_Mail_Di1";
    			else
    				mHaveSelectedGO.transform.Find("BG").GetComponent<UISprite>().spriteName = "Ty_Mail_Di2";
    		}
    
    		mHaveSelectedGO = selectGO;
    		mHaveSelectedGO.transform.Find("BG").GetComponent<UISprite>().spriteName = "Ty_Mail_Di3";
    
    		int index = (5-int.Parse(selectGO.name));//name写反了，而且也不应该用name转int这种方式，临时解决方案
    		string rank = ((eRanks)index).ToString();
    		mSelectRank = rank;
    		if (Data.ZoneTimeDiff.GetServerTime() > NationManager.Instance.Members.ResetTime)
    		{
    			NationManager.Instance.Members.CleanUp();
    		}
    		
    		if (NationManager.Instance.Members.ReqMemberRecordDictionary.ContainsKey(rank))
    		{
    			if (EB.Time.Now - NationManager.Instance.Members.ReqMemberRecordDictionary[rank].Ts> 30*60)  //30 minute 
    			{
    				NationManager.Instance.Members.ReqMemberRecordDictionary[rank].DataList.Clear();
    				NationManager.Instance.Members.ReqMemberRecordDictionary.Remove(rank);
    				NationManager.Instance.GetMemberList(rank, null);  //bug fix
    			}
    			else
    				MemberDynamicScroll.SetItemDatas( NationManager.Instance.Members.GetMemberByRank(rank).ToArray());
    		}
    		else
    			NationManager.Instance.GetMemberList(rank, null);
    
    		//ShowReward(index);
    		//SetReceiveBtnState(rank);
    	}
    
    	//public void OnReceiveRewardBtnClick()
    	//{
    	//    FusionAudio.PostEvent("UI/General/ButtonClick");
    	//	if (!NationManager.Instance.Account.HaveReceiveRankReward)
    	//	{
    	//		NationManager.Instance.Account.HaveReceiveRankReward = true;
    	//		SetReceiveBtnState(NationManager.Instance.Account.Rank);
    	//		NationManager.Instance.ReceiveRankReward(delegate (bool successful)
    	//		{
    	//			if (successful)
    	//			{
    	//				int myIndex = System.Array.IndexOf(NationUtil.RankArr, NationManager.Instance.Account.Rank);
    	//				if (myIndex < 0)
    	//				{
    	//					Debug.LogError("myRank index < 0 ");
    	//					myIndex = 0;
    	//				}
    	//				Hotfix_LT.Data.NationRankRewardTemplate rankRewardTpl = Hotfix_LT.Data.EventTemplateManager.Instance.GetNationRankRewardTpl(myIndex + 1);
    	//				GlobalMenuManager.Instance.Open("LTShowRewardView", rankRewardTpl.rewardDatas);
    	//			}
    	//		});
    	//	}
    	//}
    
    	public void OnRuleBtnClick()
    	{
    		string text = EB.Localizer.GetString("ID_NATION_RANK_RULE");
    		if (text.Contains("ID_"))
    			text = EB.Localizer.GetString("ID_codefont_in_NationRanksController_5030");
    		GlobalMenuManager.Instance.Open("LTRuleUIView", text);
    	}
    }
}
