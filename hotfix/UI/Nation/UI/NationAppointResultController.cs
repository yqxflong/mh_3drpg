using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class NationAppointResultController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            PlayAnim = t.GetMonoILRComponent<BattleResultFXPlayAnimation>("VictoryAnimation");
            controller.backButton = t.GetComponent<UIButton>("Content/CancelBtn");
            t.GetComponent<UIButton>("Content/SureBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            KingShowItem.IconSprite = t.GetComponent<UISprite>("Content/MemberList/King/Icon");
            KingShowItem.NameLabel = t.GetComponent<UILabel>("Content/MemberList/King/Name");
            KingShowItem.LevelLabel = t.GetComponent<UILabel>("Content/MemberList/King/Level");

            MarshalItems = new List<PlayerShowItem>();
            PlayerShowItem item = new PlayerShowItem();
            item.IconSprite = t.GetComponent<UISprite>("Content/MemberList/Marshal1/Icon");
            item.NameLabel = t.GetComponent<UILabel>("Content/MemberList/Marshal1/Name");
            item.LevelLabel = t.GetComponent<UILabel>("Content/MemberList/Marshal1/Level");
            MarshalItems.Add(item);

            item = new PlayerShowItem();
            item.IconSprite = t.GetComponent<UISprite>("Content/MemberList/Marshal2/Icon");
            item.NameLabel = t.GetComponent<UILabel>("Content/MemberList/Marshal2/Name");
            item.LevelLabel = t.GetComponent<UILabel>("Content/MemberList/Marshal2/Level");
            MarshalItems.Add(item);

            GeneralItems = new List<PlayerShowItem>();
            item = new PlayerShowItem();
            item.IconSprite = t.GetComponent<UISprite>("Content/MemberList/Grid/General/Icon");
            item.NameLabel = t.GetComponent<UILabel>("Content/MemberList/Grid/General/Name");
            item.LevelLabel = t.GetComponent<UILabel>("Content/MemberList/Grid/General/Level");
            GeneralItems.Add(item);

            item = new PlayerShowItem();
            item.IconSprite = t.GetComponent<UISprite>("Content/MemberList/Grid/General (1)/Icon");
            item.NameLabel = t.GetComponent<UILabel>("Content/MemberList/Grid/General (1)/Name");
            item.LevelLabel = t.GetComponent<UILabel>("Content/MemberList/Grid/General (1)/Level");
            GeneralItems.Add(item);

            item = new PlayerShowItem();
            item.IconSprite = t.GetComponent<UISprite>("Content/MemberList/Grid/General (2)/Icon");
            item.NameLabel = t.GetComponent<UILabel>("Content/MemberList/Grid/General (2)/Name");
            item.LevelLabel = t.GetComponent<UILabel>("Content/MemberList/Grid/General (2)/Level");
            GeneralItems.Add(item);

            item = new PlayerShowItem();
            item.IconSprite = t.GetComponent<UISprite>("Content/MemberList/Grid/General (3)/Icon");
            item.NameLabel = t.GetComponent<UILabel>("Content/MemberList/Grid/General (3)/Name");
            item.LevelLabel = t.GetComponent<UILabel>("Content/MemberList/Grid/General (3)/Level");
            GeneralItems.Add(item);

        }


    
    	[System.Serializable]
    	public class PlayerShowItem
    	{
    		public UISprite IconSprite;
    		public UILabel NameLabel;
    		public UILabel LevelLabel;
    
    		public void SetNull()
    		{
    			NameLabel.transform.parent.gameObject.SetActive(false);
    		}
    
    		public void Fill(NationMember data)
    		{
    			IconSprite.spriteName = data.Portrait;
                LTUIUtil.SetText(NameLabel, data.Name);
    			LTUIUtil.SetText(LevelLabel, data.Level.ToString());
    			GameObject rootGO = NameLabel.transform.parent.gameObject;
    			rootGO.SetActive(true);
    			UITweener uit = rootGO.GetComponent<UITweener>();
    			uit.ResetToBeginning();
    			uit.PlayForward();
    		}
    	}
    
    	public override bool ShowUIBlocker { get { return true; } }
    
    	public BattleResultFXPlayAnimation PlayAnim;
    	public PlayerShowItem KingShowItem;
    	public List<PlayerShowItem> MarshalItems;
    	public List<PlayerShowItem> GeneralItems;
    	private WaitForSeconds wait02 = new WaitForSeconds(0.2f);
    	bool mPlayOver;
    
    	public override IEnumerator OnAddToStack()
    	{	
    		yield return base.OnAddToStack();
    
    	    FusionAudio.PostEvent("UI/New/WeiRen", true);
    		PlayAnim.mDMono.gameObject.SetActive(true);
            /*while (!PlayAnim.PlayOver)
    		{
    			yield return null;
    		}*/
            yield return new WaitForSeconds(1f);
    
            while (!NationManager.Instance.IsGetMemberDataByGetInfo)
    		{
    			yield return null;
    		}
    				
    		List<NationMember> kingList = NationManager.Instance.Members.GetMemberByRank(eRanks.king.ToString(), true);
    		if(kingList.Count>0)
    			KingShowItem.Fill(kingList[0]);
    
    		List<NationMember> marshalList = NationManager.Instance.Members.GetMemberByRank(eRanks.marshal.ToString(), true);
    		for (int viewIndex = 0; viewIndex < MarshalItems.Count; ++viewIndex)
    		{
    			if (viewIndex < marshalList.Count)
    			{
    				yield return wait02;
    				MarshalItems[viewIndex].Fill(marshalList[viewIndex]);
    			}
    			else
    				MarshalItems[viewIndex].SetNull();
    		}
    		List<NationMember> generalDatalList = NationManager.Instance.Members.GetMemberByRank(eRanks.general.ToString(), true);
    		for (int viewIndex = 0; viewIndex < GeneralItems.Count; ++viewIndex)
    		{
    			if (viewIndex < generalDatalList.Count)
    			{
    				yield return wait02;
    				GeneralItems[viewIndex].Fill(generalDatalList[viewIndex]);
    			}
    			else
    				GeneralItems[viewIndex].SetNull();
    		}
    		//NationManager.Instance.Members.CleanUp();
    		mPlayOver = true;
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
    		DestroySelf();
    		yield break;
    	}
    
    	public override void OnCancelButtonClick()
    	{
    		if (!mPlayOver)
    		{
    			return;
    		}
    
    		base.OnCancelButtonClick();
    	}
    }
}
