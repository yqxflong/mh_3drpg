using System;

namespace Hotfix_LT.UI
{
  using UnityEngine;

public class FriendTabController : DynamicMonoHotfix {

    public GameObject ContentObj;
    public GameObject FriendSearchObj;
    public UILabel FriendNumLab;
    public Action SelectFriendAction;
	private string FriendNumStr;
    private string BlacklistNumStr;
    private string RecentlyNumStr;
    private string TeamNumStr;
    private int mSelectFriendIndex;
	private int mSelectRecentlyIndex;

	private eFriendType mType = eFriendType.None;
	public eFriendType Type
	{
		get { return mType; }
		set {
			mType = value;

            if (mType == eFriendType.My)
            {
                FriendNumLab.text = EB.Localizer.GetString("ID_MY_FRIEND")+ ":" + FriendNumStr;
            }
            else if (mType == eFriendType.Black)
            {
                FriendNumLab.text = EB.Localizer.GetString("ID_BLACKLIST") + ":" + BlacklistNumStr;
            }
            else if (mType == eFriendType.Recently)
            {
                FriendNumLab.text = EB.Localizer.GetString("ID_RECENT_CONTACTS") + ":" + RecentlyNumStr;
            }
            else if (mType == eFriendType.Team)
            {
                FriendNumLab.text = EB.Localizer.GetString("ID_FORM_TEAM") + ":" + TeamNumStr;
            }
        }
	}

	public void Open(eFriendType type)
	{
		if (type == eFriendType.My)
		{
			OnMyFriendTabClick();
		}
		else if (type == eFriendType.Black)
		{
			OnBlackListTabClick();
		}
		else if (type == eFriendType.Recently)
		{
			OnRecentlyContactTabClick();
		}
		else if (type == eFriendType.Team)
		{
			OnTeamTabClick();
		}
	}

	public void SetPeopleNum(string friendNumStr,string blacklistNumStr,string recentlyNumStr,string teamNumStr)
	{
		if(friendNumStr!=null)
			this.FriendNumStr = friendNumStr;
		if (blacklistNumStr != null)
			this.BlacklistNumStr = blacklistNumStr;
		if(recentlyNumStr!=null)
			this.RecentlyNumStr = recentlyNumStr;
		if(teamNumStr!=null)
			this.TeamNumStr = teamNumStr;

        if (mType == eFriendType.My)
        {
            FriendNumLab.text = EB.Localizer.GetString("ID_MY_FRIEND") + ":" + FriendNumStr;
        }
        else if (mType == eFriendType.Black)
        {
            FriendNumLab.text = EB.Localizer.GetString("ID_BLACKLIST") + ":" + BlacklistNumStr;
        }
        else if (mType == eFriendType.Recently)
        {
            FriendNumLab.text = EB.Localizer.GetString("ID_RECENT_CONTACTS") + ":" + RecentlyNumStr;
        }
        else if (mType == eFriendType.Team)
        {
            FriendNumLab.text = EB.Localizer.GetString("ID_FORM_TEAM") + ":" + TeamNumStr;
        }
    }

	private void Open(int index)
	{
		if (SelectFriendAction != null)
			SelectFriendAction();
	}

    #region event handler
    public void OnMyFriendTabClick()
    {
        if (mType == eFriendType.My && !FriendHudController.mFirstOpen) return;

        ContentObj.CustomSetActive(true);
        Type = eFriendType.My;
        Open(0);
        FriendSearchObj.CustomSetActive(false);
    }

    public void OnBlackListTabClick()
    {
        if (mType == eFriendType.Black) return;

        ContentObj.CustomSetActive(true);
        Type = eFriendType.Black;
        Open(1);
        FriendSearchObj.CustomSetActive(false);
    }

    public void OnRecentlyContactTabClick()
	{
		if (mType == eFriendType.Recently) return;

        ContentObj.CustomSetActive(true);
        Type = eFriendType.Recently;
        Open(0);
        FriendSearchObj.CustomSetActive(false);
    }

    public void OnTeamTabClick()
    {
        if (mType == eFriendType.Team) return;

        ContentObj.CustomSetActive(true);
        Type = eFriendType.Team;
        Open(1);
        FriendSearchObj.CustomSetActive(false);
    }

    public void OnSearchFriendClick()
    {
        if (mType == eFriendType.Search) return;

        ContentObj.CustomSetActive(false);
        Type = eFriendType.Search;
        FriendSearchObj.CustomSetActive(true);
    }

	public void OnOpenTabClick(GameObject go,GameObject go1)
	{
		int index = int.Parse(go.name);
        Open(index);
	}
	
	#endregion

	
	private Transform controller;
	private ShowFriendSearchContent _searchContent;
	private TitleListController _titleListController;
	private UIButton FriendBtn;
	private UIButton BlackListBtn;
	private UIButton RecentlyBtn;
	private UIButton TeamBtn;
	private UIButton SearchBtn;
	public override void Awake()
	{
		base.Awake();
		controller = mDMono.transform.parent.parent.parent;
		ContentObj = controller.Find("Content").gameObject;
		FriendSearchObj =  controller.Find("FriendSearch").gameObject;
		FriendNumLab =  controller.Find("Content/LeftSide/FriendNum").GetComponent<UILabel>();
		
		_titleListController = controller.Find("BGs/Middle/BGs/UpButtons/Title").GetMonoILRComponent<TitleListController>();
		_searchContent = FriendSearchObj.GetMonoILRComponent<ShowFriendSearchContent>();
	
		FriendBtn = controller.Find("BGs/Middle/BGs/UpButtons/Title/BtnList/FriendBtn").GetComponent<UIButton>();
		FriendBtn.onClick.Add(new EventDelegate(OnMyFriendTabClick));
		FriendBtn.onClick.Add(new EventDelegate(_searchContent.OnLeaveContent));
		FriendBtn.onClick.Add(new EventDelegate(() =>
		{
			_titleListController.OnTitleBtnClick(FriendBtn.transform.Find("Sprite").gameObject);
		}));
		
		BlackListBtn = controller.Find("BGs/Middle/BGs/UpButtons/Title/BtnList/BlackListBtn").GetComponent<UIButton>();
		BlackListBtn.onClick.Add(new EventDelegate(OnBlackListTabClick));
		BlackListBtn.onClick.Add(new EventDelegate(_searchContent.OnLeaveContent));
		BlackListBtn.onClick.Add(new EventDelegate(() =>
		{
			_titleListController.OnTitleBtnClick(BlackListBtn.transform.Find("Sprite").gameObject);
		}));
		
		RecentlyBtn = controller.Find("BGs/Middle/BGs/UpButtons/Title/BtnList/RecentlyBtn").GetComponent<UIButton>();
		RecentlyBtn.onClick.Add(new EventDelegate(OnRecentlyContactTabClick));
		RecentlyBtn.onClick.Add(new EventDelegate(_searchContent.OnLeaveContent));
		RecentlyBtn.onClick.Add(new EventDelegate(() =>
		{
			_titleListController.OnTitleBtnClick(RecentlyBtn.transform.Find("Sprite").gameObject);
		}));
		
		TeamBtn = controller.Find("BGs/Middle/BGs/UpButtons/Title/BtnList/TeamBtn").GetComponent<UIButton>();
		TeamBtn.onClick.Add(new EventDelegate(OnTeamTabClick));
		TeamBtn.onClick.Add(new EventDelegate(_searchContent.OnLeaveContent));
		TeamBtn.onClick.Add(new EventDelegate(() =>
		{
			_titleListController.OnTitleBtnClick(TeamBtn.transform.Find("Sprite").gameObject);
		}));
		
		SearchBtn = controller.Find("BGs/Middle/BGs/UpButtons/Title/BtnList/SearchBtn").GetComponent<UIButton>();
		SearchBtn.onClick.Add(new EventDelegate(OnSearchFriendClick));
		SearchBtn.onClick.Add(new EventDelegate(_searchContent.OnOpenContent));
		SearchBtn.onClick.Add(new EventDelegate(() =>
		{
			_titleListController.OnTitleBtnClick(SearchBtn.transform.Find("Sprite").gameObject);
		}));
	}
}

}