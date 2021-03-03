using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTBountyTaskConversationController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            SpeakNameLabel = t.GetComponent<UILabel>("DialogueFrame/Container/Container/BlurBG/Dialogue/Left/SpeakName");
            ContentLabel = t.GetMonoILRComponent<UILazyLabel>("DialogueFrame/Container/Container/BlurBG/Dialogue/Left/Context");
            GiveItem = t.GetMonoILRComponent<LTShowItem>("DialogueFrame/Container/Container/BlurBG/Dialogue/LTShowItem");
            GiveReq = t.GetComponent<UIServerRequest>("ReqServer/GiveItem");
            m_BGTexture = t.GetComponent<UITexture>("DialogueFrame/Container/Container/BlurBG");
            m_LobbyTexture = t.GetComponent<UITexture>("DialogueFrame/Container/Container/BlurBG/Dialogue/Left/Icon");
            m_SpriteIcon = t.GetComponent<CampaignTextureCmp>("DialogueFrame/Container/Container/BlurBG/Dialogue/Left/SpriteIcon");
            DialogueBlock = t.FindEx("DialogueFrame/Container/Container/BlurBG").gameObject;
            SpecialEventFuncNode = t.FindEx("DialogueFrame/Container/Container/BlurBG/Dialogue/DownBtnGrid").gameObject;
            VictoryAnimNode = t.FindEx("VicToryView").gameObject;
            ContentWidth=ContentLabel.mDMono.transform.GetComponent<UIWidget>().width;
            sRunAway = false;
            t.GetComponent<UIButton>("DialogueFrame/Container/CancelBtn").onClick.Add(new EventDelegate(base.OnCancelButtonClick));


            t.GetComponent<UIButton>("DialogueFrame/Container/Container/BlurBG/Dialogue/DownBtnGrid/Btn").onClick.Add(new EventDelegate(OnGiveItemBtnClick));
            t.GetComponent<UIButton>("DialogueFrame/Container/Container/BlurBG/Dialogue/DownBtnGrid/Btn (1)").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButton>("DialogueFrame/Container/Container/BlurBG/Dialogue/DownBtnGrid/Btn (2)").onClick.Add(new EventDelegate(OnEnterBattleBtnClick));

            t.GetComponent<UIEventTrigger>("DialogueFrame/Container/Container/BlurBG").onClick.Add(new EventDelegate(OnDialogueBlockClick));
            t.GetComponent<UIEventTrigger>("VicToryView/Bg").onClick.Add(new EventDelegate(OnVictoryBlockClick));


            t.GetComponent<UIServerRequest>("ReqServer/GiveItem").onResponse.Add(new EventDelegate(controller, "OnFetchData"));
        }

    	public UILabel SpeakNameLabel;
    	public UILazyLabel ContentLabel;
    	public LTShowItem GiveItem;
    	public UIServerRequest GiveReq;
    	public UITexture m_BGTexture;
    	public UITexture m_LobbyTexture;
        public CampaignTextureCmp m_SpriteIcon;
    
    	public GameObject DialogueBlock;
    	public GameObject SpecialEventFuncNode;
    	public GameObject VictoryAnimNode;
    
    	UI3DLobby Lobby;
    	GM.AssetLoader<GameObject> Loader;
    	string ModelName;
    	string mNpcLocator;
    	List<LTShowItemData> mGetRewardsByGiveItem;
    	bool mItemEnough;
    	DialogueData m_Dialogue_data;
    	int m_CurrentStepIndex = 1;
    	System.Action mDialogueFinishCallback;
    	int mNextRequestTime;

        bool BtnClickLimit = false;
    	public static bool sTriggerSpecialEvent {
    		get {
    			Hashtable giveItemData;
    			DataLookupsCache.Instance.SearchDataByID<Hashtable>(string.Format("tasks.{0}.event_count.give_item", LTBountyTaskHudController.TaskID()), out giveItemData);
    			int itemId = EB.Dot.Integer("item_id", giveItemData, 0);
    			return itemId > 0;
    		}
    	}
    	public static bool sRunAway;
    
    	public override bool ShowUIBlocker { get { return false; } }
    
        private bool m_IsInited = false;
        
        private int ContentWidth;
    
    	public override void SetMenuData(object param)
    	{
	        //Bug解决 
	        mDialogueFinishCallback = null;
	        ShowOK = false;
            BtnClickLimit = false;

            mNextRequestTime = AutoRefreshingManager.Instance.GetCronRefreshExcuter("refreshTaskState").NextRequestTime;
    
    		DataLookupsCache.Instance.SearchDataByID<string>(string.Format("tasks.{0}.event_count.locator", LTBountyTaskHudController.TaskID()), out mNpcLocator);
    		mDialogueFinishCallback = param as System.Action;
    		if (mDialogueFinishCallback != null)
    		{
    			InitDialogueData();
    			PlayDialogue();
    			SpecialEventFuncNode.gameObject.CustomSetActive(false);
    			GiveItem.mDMono.gameObject.CustomSetActive(false);
    			DialogueBlock.GetComponent<BoxCollider>().enabled = true;
    		}
    		else
    		{
    			InitSpecialEventData();
    			DialogueBlock.GetComponent<BoxCollider>().enabled = false;
    		}
    		base.SetMenuData(param);
    	}
    
    	void InitSpecialEventData()
    	{	
    		mGetRewardsByGiveItem = LTBountyTaskHudController.GetRewardDatas();
    		Hashtable giveItemData;
    		DataLookupsCache.Instance.SearchDataByID<Hashtable>(string.Format("tasks.{0}.event_count.give_item", LTBountyTaskHudController.TaskID()), out giveItemData);
    		int itemId = EB.Dot.Integer("item_id", giveItemData,0);
    		int needNum = EB.Dot.Integer("target_num",giveItemData,0);
    		int haveItemNum = GameItemUtil.GetInventoryItemNum(itemId);
            EconemyItemTemplate itemTpl = EconemyTemplateManager.Instance.GetItem(itemId);
            ContentLabel.Text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTBountyTaskConversationController_2609"), needNum, itemTpl.Name);
            ContentLabel.mDMono.transform.GetComponent<UIWidget>().width =ContentWidth- GiveItem.Frame.width;
            GiveItem.LTItemData = new LTShowItemData(itemId.ToString(), needNum, LTShowItemType.TYPE_GAMINVENTORY, false);

            mItemEnough = haveItemNum >= needNum;
    		GiveItem.Count.text = string.Format("[{0}]{1}[-]/{2}", mItemEnough ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal, haveItemNum,needNum);
            LTPartnerDataManager.Instance.itemNeedCount = needNum;
            if (mItemEnough)
            {
                SpecialEventFuncNode.transform.GetChild(0).gameObject.CustomSetActive(true);
                SpecialEventFuncNode.transform.GetChild(1).gameObject.CustomSetActive(false);
                SpecialEventFuncNode.GetComponent<UIGrid>().Reposition();
            }
            else
            {
                SpecialEventFuncNode.transform.GetChild(0).gameObject.CustomSetActive(false);
                SpecialEventFuncNode.transform.GetChild(1).gameObject.CustomSetActive(true);
                SpecialEventFuncNode.GetComponent<UIGrid>().Reposition();
            }
        }
    
    	public override IEnumerator OnAddToStack()
    	{
            yield return base.OnAddToStack();
            GaussianBlurRT.Capture((Texture tex) =>
    		{
    			m_BGTexture.mainTexture = tex;
    		});
            if(this.controller.gameObject!=null)StartCoroutine(CreateBuddyModel());
            GlobalMenuManager.Instance.PushCache("LTBountyTaskConversationUI");
        }
    
    	public override IEnumerator OnRemoveFromStack()
    	{
            LTPartnerDataManager.Instance.itemNeedCount = 0;
            m_SpriteIcon.spriteName = string.Empty;
            StopAllCoroutines();
            DestroySelf();
    
    		if (Lobby != null)
    		{
    			GameObject.Destroy(Lobby.mDMono.gameObject);
    			Lobby = null;
    		}
    		if (Loader != null)
    		{
    			EB.Assets.UnloadAssetByName("UI3DLobby", false);
    			Loader = null;
    		}
    		if (ModelName != null)
    		{
    			ModelName = null;
    		}
    		yield break;
    	}
    
      
        public override void OnFocus()
        {
            base.OnFocus();
            if (mDialogueFinishCallback == null)
            {
                InitSpecialEventData();
            }
        }
    
        public override void OnBlur()
        {
            base.OnBlur();
        }
    
        void InitDialogueData()
    	{
    		int dialogueId = Random.Range(900001,900005);
    		m_Dialogue_data = Hotfix_LT.Data.DialogueTemplateManager.Instance.GetDialogueData(dialogueId);
    		if (m_Dialogue_data == null)
    		{			
    			m_Dialogue_data = Hotfix_LT.Data.DialogueTemplateManager.Instance.GetDialogueData(10000101);
    			EB.Debug.LogError("m_Dialogue_data is null for dialogue_id={0}", dialogueId);
    		}
    	}
    
    	void PlayDialogue()
    	{
    		if (m_Dialogue_data == null)
    		{
    			if (mDialogueFinishCallback != null)
    				mDialogueFinishCallback();
	            OnCancelButtonClick();
    			return;
    		}
    
    		if (m_Dialogue_data.Steps.Length >= m_CurrentStepIndex)
    		{
    			DialogueStepData step = m_Dialogue_data.Steps[m_CurrentStepIndex - 1];
    			ContentLabel.Text=step.Context;
    		}
    		else
    		{
    			if (mDialogueFinishCallback != null)
    				mDialogueFinishCallback();
                OnCancelButtonClick();
    		}
    		m_CurrentStepIndex++;
    	}
    
    	public IEnumerator CreateBuddyModel()
    	{
    		m_LobbyTexture.uvRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
    		var charTpl = LTBountyTaskHudController.GetFirstMonsterInfoTpl();
            var heroInfo= Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(charTpl.id);
            ModelName = charTpl.model_name;
            LTUIUtil.SetText(SpeakNameLabel, charTpl.name);
            if (!string.IsNullOrEmpty ( heroInfo.portrait))
            {
                m_LobbyTexture.gameObject.CustomSetActive(false);
                m_SpriteIcon.gameObject.CustomSetActive(true);
                m_SpriteIcon.spriteName = heroInfo.portrait;
                m_IsInited = true;
                TweenPosition TP = m_SpriteIcon.transform.GetComponent<TweenPosition>();
                if(TP!=null) TP.PlayForward();
            }
            else
            {
                m_SpriteIcon.gameObject.CustomSetActive(false);
                m_LobbyTexture.gameObject.CustomSetActive(true);
                LobbyCameraData lobby_cam = heroInfo.lobby_camera;
    
                Loader = new GM.AssetLoader<GameObject>("UI3DLobby", controller.gameObject);
                UI3DLobby.Preload(ModelName);
                yield return Loader;
    
                if (Loader.Success)
                {
                    Loader.Instance.transform.parent = m_LobbyTexture.transform;
                    Lobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
                }
    
                if (Lobby != null)
                {
                    Lobby.VariantName = ModelName;
    
                    Lobby.SetCameraPos(lobby_cam.Position);
                    Lobby.SetCameraRot(lobby_cam.Rotation);
                    Lobby.SetCameraMode(lobby_cam.Size, lobby_cam.Orthographic);
    
                    Lobby.ConnectorTexture = m_LobbyTexture;
    
                    while (Lobby.Current.character == null)
                    {
                        yield return null;
                    }
                    m_IsInited = true;
                    TweenPosition TP = m_LobbyTexture.transform.GetComponent<TweenPosition>();
                    if (TP != null) TP.PlayForward();
                }
            }
    	}
    
    	public void OnGiveItemBtnClick()
        {
            GlobalMenuManager.Instance.RemoveCache("LTBountyTaskConversationUI");
            if (EB.Time.Now > mNextRequestTime)
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LTBountyTaskConversationController_6504"),delegate(int result) {
    				if (result == 0)
    				{
                       OnCancelButtonClick();
    				}
    			});
    			return;
    		}
    		if (mItemEnough)
    		{
                if (BtnClickLimit) return;
                BtnClickLimit = true;
                GiveReq.parameters[0].parameter = LTBountyTaskHudController.TaskID().ToString();
    			GiveReq.SendRequest();
    		}
    		else
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB.Localizer.GetString("ID_codefont_in_LTBountyTaskConversationController_6902"));
    		}
    	}
    
    	public void OnEnterBattleBtnClick()
    	{
            GlobalMenuManager.Instance.RemoveCache("LTBountyTaskConversationUI");
            if (EB.Time.Now > mNextRequestTime)
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LTBountyTaskConversationController_6504"), delegate (int result) {
    				if (result == 0)
    				{
    					OnCancelButtonClick();
    				}
    			});
    			return;
    		}
            if (BtnClickLimit) return;
            BtnClickLimit = true;
            MainLandLogic.GetInstance().RequestBountyTaskCombatTransition();
    	}
    
    	public override void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
    	{
            if (res.sucessful)
    		{
                VictoryAnimNode.CustomSetActive(true);
                FusionAudio.PostEvent("MUS/CombatView/Stinger/Victory", controller.gameObject, true);
                LTBountyTaskHudController.DeleteMonster(mNpcLocator);
                GameDataSparxManager.Instance.ProcessIncomingData(res.result,true);
            }
    		else
    		{
    			res.CheckAndShowModal();
                BtnClickLimit = false;
            }
        }
    
        private bool ShowOK=false;
        public void OnVictoryBlockClick()
    	{
    		if (!ShowOK)
    		{
    			VictoryAnimNode.gameObject.CustomSetActive(false);
                System.Action callback = delegate ()
                {
                    ShowOK = true;
                    OnVictoryBlockClick();
                };
				var ht = Johny.HashtablePool.Claim();
				ht.Add("reward", mGetRewardsByGiveItem);
				ht.Add("callback", callback);
                GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
            }
    		else
    		{
    			OnCancelButtonClick();
    			if (LTBountyTaskHudController.HantTimes < LTBountyTaskHudController.TotalHantTimes)
    			{
    				GlobalMenuManager.Instance.Open("LTBountyTaskOverUI");
                }
    		}	
    	}
    
    	public override void OnCancelButtonClick()
        {
            GlobalMenuManager.Instance.RemoveCache("LTBountyTaskConversationUI");
            //修复Bug 关闭界面 这些判断参数应该重置参数
            mDialogueFinishCallback = null;
            ShowOK = false;
	        controller.Close();
            DestroySelf();
            controller.DestroyControllerForm();
        }
    
    	public void OnDialogueBlockClick()
    	{
            GlobalMenuManager.Instance.RemoveCache("LTBountyTaskConversationUI");
            if (EB.Time.Now > mNextRequestTime)
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LTBountyTaskConversationController_6504"), delegate (int result) {
    				if (result == 0)
    				{
    					OnCancelButtonClick();
    				}
    			});
    			return;
    		}
            if (!m_IsInited)
            {
                return;
            }
    		PlayDialogue();
    	}

    }
}
