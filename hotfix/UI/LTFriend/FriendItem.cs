using UnityEngine;
using System.Collections;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI {

    // public class FriendItemRedPointRefresh : GameEvent
    // {
    //     public bool isShow;
    //
    //     public FriendItemRedPointRefresh(bool isShow)
    //     {
    //         this.isShow = isShow;
    //     }
    // }

    public class FriendItem : DynamicCellController<FriendData>
    {
    	public UILabel NameLabel;
        public UILabel LevelLabel;
        public UILabel OnlineStateLabel;
        public UIButton VigorBtn;
        public UISprite IconSprite;
        public UISprite FrameSprite;
        public UISprite BG;
        public Transform Redpoint;
        public FriendData Data;
        public GameObject SelectedFrame;
        public UIEventTrigger _uiEventTrigger;
        public FriendHudController _controller;

        public FriendHudController Controller
        {
            get
            {
                if (_controller == null)
                {
                    _controller = mDMono.transform.parent.parent.parent.parent.parent.parent.parent.parent
                        .GetUIControllerILRComponent<FriendHudController>();
                }
                return _controller;
            }

        }

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            NameLabel = t.GetComponent<UILabel>("Name");
            LevelLabel = t.GetComponent<UILabel>("Head/Level/Label");
            OnlineStateLabel = t.GetComponent<UILabel>("State");
            VigorBtn = t.GetComponent<UIButton>("Vigor");
            IconSprite = t.GetComponent<UISprite>("Head/Icon");
            FrameSprite= t.GetComponent<UISprite>("Head/Icon/Frame");
            BG = t.GetComponent<UISprite>("BG/Sprite");
            Redpoint = t.GetComponent<Transform>("RedPoint");
            SelectedFrame = t.FindEx("Border").gameObject;

            t.GetComponent<UIButton>("Vigor").onClick.Add(new EventDelegate(OnVigorBtnClick));
            _uiEventTrigger = t.Find("BG/Sprite").GetComponent<UIEventTrigger>();
            _uiEventTrigger.onClick.Add(new EventDelegate(() => { Controller.OnSelectFriendClick(this); }));
			Messenger.AddListener<bool>(Hotfix_LT.EventName.FriendItemRedPointRefresh,OnFriendItemRPRefresh);
        }
    
        public bool Contains(ArrayList list)
        {
    	    for (int i = 0; i < list.Count; i++)
    	    {
    	        long temp;
    	        long.TryParse(list[i].ToString(), out temp);
                if (temp == Data.Uid)
    		    {
    			    return true;
    		    }
    	    }
    
    	    return false;
        }
        
    
        public override void OnDestroy()
        {
            Messenger.RemoveListener<bool>(Hotfix_LT.EventName.FriendItemRedPointRefresh,OnFriendItemRPRefresh);
        }
    
        public override void Clean()
    	{
        }
        
        public override void Fill(FriendData data)
    	{
            Data = data;
            Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(int.Parse(data.Head), data.Skin);
            if (heroInfo != null)
            {
                IconSprite.spriteName = heroInfo.icon;
            }
            FrameSprite.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(data.Frame).iconId;
            SetItemBG(DataIndex % 2);
            LTUIUtil.SetText(NameLabel, data.Name);
            LTUIUtil.SetText(LevelLabel, data.Level.ToString());
            if (data.OfflineTime != 0)
    		{
                OnlineStateLabel.text = AlliancesManager.FormatOfflineDuration(data.OfflineTime);
    
    			IconSprite.color = FrameSprite.color = Color.magenta;
                
            }
    		else
    		{
                OnlineStateLabel.text = EB.Localizer.GetString("ID_ON_LINE");
                OnlineStateLabel.color = LT.Hotfix.Utility.ColorUtility.BrownColor;
    
    			IconSprite.color = FrameSprite.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
            }
    
            UpdateVigorState();
    		OnSelect(data.IsSelect);
    		Redpoint.gameObject.CustomSetActive(FriendManager.Instance.Info.GetIsUnreadMessageId(data.Uid));
            if (Data.IsCanReceiveVigor && FriendManager.Instance.IsResidueVigorReceiveNum())
            {
                Redpoint.gameObject.CustomSetActive(true);
            }
    
    	}
    
        public void SetItemBG(int index)
        {
            // index: 0：浅底，1：深底
            BG.spriteName = index == 0 ? "Ty_Mail_Di1" : "Ty_Mail_Di2";
        }
    
    	private void UpdateVigorState()
    	{
    		if (Data.IsFriend)
    		{
    			VigorBtn.gameObject.CustomSetActive(true);
    			if (Data.IsCanReceiveVigor)
    			{
    				VigorBtn.normalSprite = VigorBtn.transform.GetChild(0).GetComponent<UISprite>().spriteName = "Friends_Icon_Lingqutili";
    				VigorBtn.enabled = true;
    				VigorBtn.isEnabled = true;
    				VigorBtn.GetComponent<UISprite>().color = new Color(1, 1, 1, 1);
    			}
    			else
    			{
    				VigorBtn.normalSprite = VigorBtn.transform.GetChild(0).GetComponent<UISprite>().spriteName = "Friends_Icon_Zengsongtili";
    				if (!Data.IsHaveSendVigor)
    				{
    					VigorBtn.enabled = true;
    					VigorBtn.isEnabled = true;
    					VigorBtn.GetComponent<UISprite>().color = new Color(1, 1, 1, 1);
    				}
    				else
    				{
    					VigorBtn.enabled = false;
                        VigorBtn.gameObject.CustomSetActive(false);
                    }
    			}
    		}
    		else
    		{
    			VigorBtn.gameObject.CustomSetActive(false);
    		}
    	}
    
    	public void OnSelect(bool isSelect)
    	{
    		Data.IsSelect = isSelect;
    		SelectedFrame.gameObject.CustomSetActive(isSelect);
    	}
    
    	public void OnVigorBtnClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
    		if (Data.IsCanReceiveVigor)
    		{
    			if (!FriendManager.Instance.IsResidueVigorReceiveNum())
    			{
    				MessageTemplateManager.ShowMessage(FriendManager.CodeGetVitLimit);
    				return;
    			}
    
    			//接受体力有次数限制，无二次弹窗提示， 达到上限则无法领取且保	留
    			int vigor = BalanceResourceUtil.GetUserVigor();
    			if (vigor >= 9999)
    			{
    				MessageTemplateManager.ShowMessage(FriendManager.CodeVitMax, null, delegate (int result)
    				{
    					if (result == 0)
    					{
    						ReceiveVigor();
    					}
    				});
    			}
    			else
    			{
    				ReceiveVigor();
    			}
    		}
    		else if (!Data.IsHaveSendVigor)
    		{
    			if (!FriendManager.Instance.IsResidueVigorSendNum())
    			{
    				MessageTemplateManager.ShowMessage(FriendManager.CodeSendVigorLimit);
    				return;
    			}
    			//你已达到每日赠送体力上限，无法继续赠送
    
    			Data.IsHaveSendVigor = true;
    			UpdateVigorState();
    			Messenger.Raise(Hotfix_LT.EventName.FriendSendAllButtonState);
    			FriendManager.Instance.SendVigor(Data.Uid, delegate (bool successful)
    			{
    				if (successful)
    				{
    					var friend = FriendManager.Instance.MyFriends.Find(Data.Uid);
    					if (friend != null)
    						friend.IsHaveSendVigor = true;
    					var recently = FriendManager.Instance.Recentlys.Find(Data.Uid);
    					if (recently != null)
    						recently.IsHaveSendVigor = true;
    					var team = FriendManager.Instance.Teams.Find(Data.Uid);
    					if (team != null)
    						team.IsHaveSendVigor = true;
    
    					FriendManager.Instance.Info.HaveVigorSendNum++;
						var ht = Johny.HashtablePool.Claim();
						ht.Add("0", Data.Name);
						ht.Add("1", FriendManager.Instance.Config.SendVigorValue);
    					MessageTemplateManager.ShowMessage(FriendManager.CodeSendVigorSuccess, ht, null);
    					GameDataSparxManager.Instance.SetDirty(FriendManager.MyFriendListId);
    					GameDataSparxManager.Instance.SetDirty(FriendManager.RecentlyListId);
    					GameDataSparxManager.Instance.SetDirty(FriendManager.TeamListId);
    				}
    			});
    		}
    	}
    
    	private void ReceiveVigor()
    	{
    		Data.IsCanReceiveVigor = false;
    		UpdateVigorState();
    		Messenger.Raise(Hotfix_LT.EventName.FriendSendAllButtonState);
    		FriendManager.Instance.ReceiveVigor(Data.Uid, delegate (bool successful)
    		{
    			if (successful)
    			{
    				var friend = FriendManager.Instance.MyFriends.Find(Data.Uid);
    				if (friend != null)
    					friend.IsCanReceiveVigor = false;
    				var recently = FriendManager.Instance.Recentlys.Find(Data.Uid);
    				if (recently != null)
    					recently.IsCanReceiveVigor = false;
    				var team = FriendManager.Instance.Teams.Find(Data.Uid);
    				if (team != null)
    					team.IsCanReceiveVigor = false;
    				FriendManager.Instance.Info.HaveVigorReceiveNum++;
					var ht = Johny.HashtablePool.Claim();
					ht.Add("0", Data.Name);
					ht.Add("1", FriendManager.Instance.Config.GetVigorValue);
    				MessageTemplateManager.ShowMessage(FriendManager.CodeGetVigor, ht, null);
    
    				GameDataSparxManager.Instance.SetDirty(FriendManager.MyFriendListId);
    				GameDataSparxManager.Instance.SetDirty(FriendManager.RecentlyListId);
    				GameDataSparxManager.Instance.SetDirty(FriendManager.TeamListId);
    
                    if (!FriendManager.Instance.IsResidueVigorReceiveNum())
                    {
                        Messenger.Raise(Hotfix_LT.EventName.FriendItemRedPointRefresh,false);
                        Messenger.Raise(Hotfix_LT.EventName.FriendSendVigorEvent);
                    }
    			}
    		});
    	}
    
        private void OnFriendItemRPRefresh(bool isShow)
        {
            if (Redpoint!=null&&Data !=null)
            {
                Redpoint.gameObject.CustomSetActive(FriendManager.Instance.Info.GetIsUnreadMessageId(Data.Uid) || isShow);
            }
        }
    }
}
