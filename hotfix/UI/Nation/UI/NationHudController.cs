using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;
using DG.Tweening;

namespace Hotfix_LT.UI
{
    public class NationHudController : UIControllerHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            NoJoinGORoot = t.FindEx("Content/NoJoinedRoot").gameObject;
            JoinedGORoot = t.FindEx("Content/JoinedRoot").gameObject;
            NationPosTrans = t.GetComponent<Transform>("Content/NationPos");

            NationGOs = new GameObject[3];
            NationGOs[0] = t.FindEx("Content/NationList/0").gameObject;
            NationGOs[1] = t.FindEx("Content/NationList/1").gameObject;
            NationGOs[2] = t.FindEx("Content/NationList/2").gameObject;


            SelectFXGOs = new GameObject[3];
            SelectFXGOs[0] = t.FindEx("Content/NationList/0/FXPos").gameObject;
            SelectFXGOs[1] = t.FindEx("Content/NationList/1/FXPos").gameObject;
            SelectFXGOs[2] = t.FindEx("Content/NationList/2/FXPos").gameObject;

            MoveSpeed = 1.5f;
            KingIconSprite = t.GetComponent<DynamicUISprite>("Content/JoinedRoot/Center/KingPortrait/Icon");
            KingNameLabel = t.GetComponent<UILabel>("Content/JoinedRoot/Center/KingPortrait/KingName");
            KingLevelLabel = t.GetComponent<UILabel>("Content/JoinedRoot/Center/KingPortrait/Level");
            NoKingFlagSprite = t.GetComponent<UISprite>("Content/JoinedRoot/Center/KingPortrait/?");
            ChangeTermCountdownLabel = t.GetComponent<UILabel>("Content/JoinedRoot/BottomAnchors/LeftTimeValue");
            RandomRewardHCLabel = t.GetComponent<UILabel>("Content/NoJoinedRoot/BottomAnchors/RandomBtn/CostHC");
            NoticeInput = t.GetComponent<UIInput>("Content/JoinedRoot/Center/InputNotice/Input");
            InputBlock = t.GetComponent<BoxCollider>("Content/JoinedRoot/Center/InputNotice/InputBlock");
            KingModelRootGO = t.FindEx("Content/JoinedRoot/Center/KingModel").gameObject;
            LobbyTexture = t.GetComponent<UITexture>("Content/JoinedRoot/Center/KingModel/LobbyTexture");
            RewardRPObj = t.FindEx("Content/JoinedRoot/UpAnchors/RewardBtn/RedPoint").gameObject;
            NationWarRPObj = t.FindEx("Content/JoinedRoot/BottomAnchors/TerritoryBtn/RedPoint").gameObject;
            controller.backButton = t.GetComponent<UIButton>("UINormalFrameBG/CancelBtn");
            t.GetComponent<UIButton>("Content/JoinedRoot/Center/InputNotice/PenSprite").onClick.Add(new EventDelegate(OnModifyNoticeClick));

            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/NoJoinedRoot/BottomAnchors/RandomBtn").clickEvent.Add(new EventDelegate(OnListenToDestinyClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/NoJoinedRoot/BottomAnchors/LTSureBtn").clickEvent.Add(new EventDelegate(OnSureBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/JoinedRoot/UpAnchors/RewardBtn").clickEvent.Add(new EventDelegate(OnRewardBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/JoinedRoot/UpAnchors/StoreBtn").clickEvent.Add(new EventDelegate(OnStoreBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/JoinedRoot/BottomAnchors/RankBtn").clickEvent.Add(new EventDelegate(OnRanksBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/JoinedRoot/BottomAnchors/TerritoryBtn").clickEvent.Add(new EventDelegate(OnTerritoryBtnClick));

            t.GetComponent<UIEventTrigger>("Content/NationList/0").onClick.Add(new EventDelegate(()=>OnSelectNationClick(t.FindEx("Content/NationList/0").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/NationList/1").onClick.Add(new EventDelegate(() => OnSelectNationClick(t.FindEx("Content/NationList/1").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/NationList/2").onClick.Add(new EventDelegate(() => OnSelectNationClick(t.FindEx("Content/NationList/2").gameObject)));
            t.GetComponent<UIEventTrigger>("Content/JoinedRoot/Center/InputNotice/InputBlock").onClick.Add(new EventDelegate(OnInputBlockDownClick));
            
            for (var i = 0; i < NationGOs.Length; i++)
            {
                NationGOs[i].SetActive(false);
            }
        }


    
    	public override bool IsFullscreen() { return true; }
    
    	public GameObject NoJoinGORoot;
    	public GameObject JoinedGORoot;
    	public Transform NationPosTrans;
    	public GameObject[] NationGOs;
    	public GameObject[] SelectFXGOs;
    	public float MoveSpeed=1;
    	private int mSelectedIndex=-1;
    
    	public DynamicUISprite KingIconSprite;
    	public UILabel KingNameLabel;
    	public UILabel KingLevelLabel;
    	//public UISprite KingLevelBGSprite;
    	//public UISprite KingAttrBGSprite;
    	public UISprite NoKingFlagSprite;
    	public UILabel ChangeTermCountdownLabel;
    	public UILabel RandomRewardHCLabel;
    	public UIInput NoticeInput;
    	public BoxCollider InputBlock;
    
    	public GameObject KingModelRootGO;
    	public UITexture LobbyTexture;
    	private UI3DLobby Lobby = null;
    	private GM.AssetLoader<GameObject> Loader = null;
    	private string ModelName = null;
    	bool mFirstOpen=true;
    	System.DateTime mResetTime;
    	bool mHasSendReq;
    
        public GameObject RewardRPObj, NationWarRPObj;
    
    	public override IEnumerator OnAddToStack()
    	{
    		NationManager.Instance.GetInfo();
    
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.RedPoint_Nation, SetRP);
    
            mResetTime = NationManager.Instance.Config.GetResetTime();
    
            return base.OnAddToStack();
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.RedPoint_Nation, SetRP);
            DestroyModel();
    		DestroySelf();
    		yield break;
    	}
    
    	public override void OnFocus()
    	{
    		GameDataSparxManager.Instance.RegisterListener(NationManager.ListDataId, OnNationListListener);
    		GameDataSparxManager.Instance.RegisterListener(NationManager.DetailDataId, OnNationDetailListener);
    		GameDataSparxManager.Instance.RegisterListener(NationManager.AccountDataId, OnNationAccountListener);
            SetRP();
    
            base.OnFocus();
    	}
    
    	public override void OnBlur()
    	{
    		GameDataSparxManager.Instance.UnRegisterListener(NationManager.ListDataId, OnNationListListener);
    		GameDataSparxManager.Instance.UnRegisterListener(NationManager.DetailDataId, OnNationDetailListener);
    		GameDataSparxManager.Instance.UnRegisterListener(NationManager.AccountDataId, OnNationAccountListener);
    
    		base.OnBlur();
    	}
    
    	void OnNationListListener(string path, INodeData data)
    	{
    		if (!NationManager.Instance.List.Updated)
    			return;
    		NationManager.Instance.List.Updated = false;
    	}
    
    	void OnNationDetailListener(string path, INodeData data)
    	{
    		if (!NationManager.Instance.Detail.Updated)
    			return;
    		NationManager.Instance.Detail.Updated = false;
    		ShowNationUI(NationManager.Instance.Detail);
    	}
    
    	void OnNationAccountListener(string path, INodeData data)
    	{
    		if (!NationManager.Instance.Account.Updated)
    			return;
    		NationManager.Instance.Account.Updated = false;
    
    		string nationName = NationManager.Instance.Account.NationName;
    		bool isJoined = !string.IsNullOrEmpty(nationName);
    		if (!isJoined)
    		{
    			NoJoinGORoot.SetActive(true);
    			JoinedGORoot.SetActive(false);

                for (var i = 0; i < NationGOs.Length; i++)
                {
                    NationGOs[i].SetActive(true);
                }

    			int nationIndex = System.Array.IndexOf(NationUtil.NameArr, NationManager.Instance.List.GetMinName());
    			OnSelectNationClick(NationGOs[nationIndex]);
    			LTUIUtil.SetText(RandomRewardHCLabel , NationManager.Instance.Config.RandomRewardHC.ToString());
    		}
    		else
    		{
    			ShowNation(nationName);
                //NationManager.Instance.GetMemberList(eRanks.king.ToString(), OnNationList);
                //OpenAppointUI();
                if (NationManager.Instance.Account.ShowRankChange)
                {
                    NationManager.Instance.Account.ShowRankChange = false;
                    GlobalMenuManager.Instance.Open("LTNationAppointResultUI");
                }
            }
    		mFirstOpen = false;
    	}
    
        private void OnNationList(bool param)
        {
            OpenAppointUI();
        }

        private void SetRP()
        {
            RewardRPObj.CustomSetActive(!NationManager.Instance.Account.HaveReceiveRankReward);
            NationWarRPObj.CustomSetActive(NationManager.Instance.BattleTimeConfig.GetIsBattleStay());
        }
    
        void ShowNation(string nationName)
    	{
    		int nationIndex = System.Array.IndexOf(NationUtil.NameArr, nationName);

    		for (var i = 0; i < NationGOs.Length; i++)
    		{
                NationGOs[i].SetActive(false);
    		}

    		NationGOs[nationIndex].SetActive(true);
    		//remove event and fx
    		NationGOs[nationIndex].GetComponent<BoxCollider>().enabled = false;
    		SelectFXGOs[nationIndex].SetActive(false);
    
    		NoJoinGORoot.SetActive(false);
    		//ShowNationUI(NationManager.Instance.Detail);
    
    		if (!mFirstOpen)
    		{
    			float dest = (NationGOs[nationIndex].transform.position - NationPosTrans.position).magnitude;
    			float time = dest / MoveSpeed;

				var option = NationGOs[nationIndex].transform.DOMove(NationPosTrans.position, time);
				option.SetEase( Ease.Linear );
				option.OnComplete(delegate ()
				{
					JoinedGORoot.SetActive(true);
				});
    		}
    		else
    		{
    			NationGOs[nationIndex].transform.position = NationPosTrans.position;
    			JoinedGORoot.SetActive(true);
    		}
    	}
    
    	void ShowNationUI(NationDetail detail)
    	{
    		string notice = NationManager.Instance.Detail.Notice;
    		if (detail.King == null)
    		{
    			KingModelRootGO.SetActive(false);
    			KingIconSprite.gameObject.SetActive(false);
    			KingLevelLabel.gameObject.SetActive(false);
    			//KingAttrBGSprite.gameObject.SetActive(false);
    			NoKingFlagSprite.gameObject.SetActive(true);
    			notice= EB.Localizer.GetString("ID_codefont_in_NationHudController_5628");
    		}
    		else
    		{
    			KingModelRootGO.SetActive(true);
    			KingIconSprite.gameObject.SetActive(true);
    			KingLevelLabel.gameObject.SetActive(true);
    			//KingAttrBGSprite.gameObject.SetActive(true);
    			NoKingFlagSprite.gameObject.SetActive(false);
    
    			LTUIUtil.SetText(KingNameLabel, detail.King.Name);
    			KingIconSprite.spriteName = detail.King.Portrait;
    			KingLevelLabel.text = detail.King.Level.ToString();
    			//KingLevelBGSprite.spriteName = UIBuddyShowItem.AttrToLevelBG(detail.King.Attr);
    			//KingAttrBGSprite.spriteName = UIBuddyShowItem.AttrToLogo(detail.King.Attr);
    			if (string.IsNullOrEmpty(NationManager.Instance.Detail.Notice))
    			{
    				notice = EB.Localizer.GetString("ID_codefont_in_NationHudController_6322");
    			}
    
    			if (!string.IsNullOrEmpty(detail.King.Model))
    			{
    				StartCoroutine(CreateBuddyModel(detail.King.Model));
    			}
    		}
    
    		InputBlock.gameObject.SetActive(!NationUtil.IsKing);
    		NoticeInput.value = notice;
    	}
    
    	private IEnumerator CreateBuddyModel(string modelName)
    	{
    		if (string.IsNullOrEmpty(modelName) || ModelName == modelName)
    		{
    			yield break;
    		}
    
    		ModelName = modelName;
    		if (Lobby == null && Loader == null)
    		{
    			Loader = new GM.AssetLoader<GameObject>("UI3DLobby", controller.gameObject);
    			UI3DLobby.Preload(ModelName);
    			yield return Loader;
    			if (Loader.Success)
    			{
                    Loader.Instance.transform.parent = LobbyTexture.transform;
                    Lobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
    				Lobby.ConnectorTexture = LobbyTexture;
    				Lobby.SetCameraMode(2, true);
    			}
    		}
    
    		if (Lobby != null)
    		{
    			Lobby.VariantName = ModelName;
    		}
    	}
    
    	private void DestroyModel()
    	{
    		if (Lobby != null)
    		{
                Object.Destroy(Lobby.mDMono.gameObject);
            }
    		if (Loader != null)
    		{
    			EB.Assets.UnloadAssetByName("UI3DLobby", false);
                Loader = null;
            }
            ModelName = null;
    		Lobby = null;
    		Loader = null;
    	}

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		float mReqExpectionCD=200;
    	public void Update()
    	{
    		// base.Update();
    
    		if (!string.IsNullOrEmpty(NationManager.Instance.Account.NationName) && mResetTime != null)
    		{
    			System.TimeSpan ts = mResetTime - Data.ZoneTimeDiff.GetServerTime();
    			string leftTimeStr = string.Format(EB.Localizer.GetString("ID_codefont_in_NationHudController_7771"), ts.Days, ts.Hours, ts.Minutes);
    			LTUIUtil.SetText(ChangeTermCountdownLabel, leftTimeStr);
    			if (ts.TotalSeconds <= 0)
    			{
    				mReqExpectionCD += Time.deltaTime;
    				mResetTime = NationManager.Instance.Config.GetResetTime();
    				if (mReqExpectionCD >= 200)
    				{
    					Debug.Log("nation CountdownEndTime so getinfo");
    					mReqExpectionCD = 0;
    					NationManager.Instance.GetInfo();
    				}
    			}
    		}
    	}
    
    
    	#region not joined
    	public void OnSelectNationClick(GameObject selectGO)
    	{
    	    FusionAudio.PostEvent("UI/New/HaoJiao");
    		int si= int.Parse(selectGO.name);
    		if (si == mSelectedIndex)
    			return;
    		if(mSelectedIndex!=-1)
    			SelectFXGOs[mSelectedIndex].SetActive(false);
    		mSelectedIndex = si;
    		SelectFXGOs[mSelectedIndex].SetActive(true);
    	}
    
    	public void OnListenToDestinyClick()
    	{
    		if (mHasSendReq)
    			return;
    		mHasSendReq = true;
    
    		NationManager.Instance.Select(null,delegate(bool successful) {
    			if (successful)
    			{
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,string.Format(EB.Localizer.GetString("ID_codefont_in_NationHudController_8999"),NationManager.Instance.Config.RandomRewardHC));
    			}
    		});
    	}
    
    	public void OnSureBtnClick()
    	{
    		if (mHasSendReq)
    			return;
    		mHasSendReq = true;
    
    		string nationName = NationUtil.NameArr[mSelectedIndex];
    		NationManager.Instance.Select(nationName, delegate (bool successful) {
    
    		});
    	}
    	#endregion
    
    	#region joined eventHandler
    	public void OnInputBlockDownClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_NationHudController_9536"));
    	}
    
    	public void OnModifyNoticeClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
    		if (NoticeInput.value == NationManager.Instance.Detail.Notice)
    			return;
    
    		if (!NationUtil.IsKing)
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_NationHudController_9536"));
    			return;
    		}
    
    		if (string.IsNullOrEmpty(NoticeInput.value))
    		{
    			MenuManager.Warning("ID_INPUT_EMPTY");
    			return;
    		}
    
    		if (NoticeInput.value.IndexOf(" ") >= 0)
    		{
    			MenuManager.Warning("ID_INPUT_CONTAINS_SPACE");
    			return;
    		}
    
    		NoticeInput.value = EB.ProfanityFilter.Filter(NoticeInput.value);
    
    		// update local first
    		NationManager.Instance.Detail.Notice = NoticeInput.value;
    		//GameDataSparxManager.Instance.SetDirty(AlliancesManager.detailDataId);
    
    		NationManager.Instance.ModifyNotice(NoticeInput.value,delegate(bool successful) {
    			if (successful)
    			{
    				MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_NationHudController_10523"));
    			}
    		});
    	}
    
    	public void OnRanksBtnClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
    		GlobalMenuManager.Instance.Open("LTNationRanksUI");
    	}
    
    	public void OnRewardBtnClick()
    	{
    		FusionAudio.PostEvent("UI/General/ButtonClick");
    		GlobalMenuManager.Instance.Open("LTNationRanksRewardUI");
    	}
    
    	public void OnStoreBtnClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.Open("LTStoreUI","nation");
        }
    
    	public void OnTerritoryBtnClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.Open("LTNationTerritoryUI");
    	}
    	#endregion
    
    	[ContextMenu("OpenAppointUI")]
    	public void OpenAppointUI()
    	{		
    		NationManager.Instance.IsGetMemberDataByGetInfo = true;
    		GlobalMenuManager.Instance.Open("LTNationAppointResultUI");
    	}
    }
}
