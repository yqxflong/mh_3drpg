namespace Hotfix_LT.UI
{
using UnityEngine;
using System.Collections;
using EB.Sparx;

/// <summary>
/// 天梯战斗的逻辑
/// </summary>
public class LTHeroBattleLogic : ManagerUnit
{
    public SubSystemState State {get;set;}
    private static LTHeroBattleLogic _instance;
    public static LTHeroBattleLogic GetInstance()
    {
        if (_instance == null)
        {
            _instance = LTHotfixManager.GetManager<LTHeroBattleLogic>();
        }
        return _instance;
    }

    private LTHeroBattleAPI api;
    private bool _HeroBattleFinish;    

    public override void Async(string message, object payload)
    {
        base.Async(message, payload);
        //if (payload != null)
        //{
        //    GameDataSparxManager.Instance.ProcessIncomingData(payload,true);
        //}
    }

    public override void Initialize(Config config)
    {
        GetInstance().api = new LTHeroBattleAPI();
        LTHeroBattleEvent.ConfirmChoiceHero += OnConfirmChoiceHero;
        LTHeroBattleEvent.ConfirmBanHero += OnConfirmBanHero;
        LTHeroBattleEvent.ChoiceHero += OnChoiceHero;
        // LTHeroBattleEvent.MatchOther += OnMatchOther;
        // LTHeroBattleEvent.QuitMatchOther += OnQuitMatchOther;
        // LTHeroBattleEvent.CloseAndQuitMatch += OnCloseAndQuitMatch;
        // LTHeroBattleEvent.GetReward += OnGetReward;
        // LTHeroBattleEvent.ChangeHeroType += OnChangeHeroType;
        LTHeroBattleEvent.ChoiceSuitIndex += OnChoiceSuitIndex;
        LTHeroBattleEvent.TouchSuitIndexTips += OnTouchSuitIndexTips;
        LTHeroBattleEvent.NotifyRefreshChoiceState += NotifyRefreshChoiceState;
        LTHeroBattleEvent.NotifyHeroBattleHudFinish += OnNotifyHeroBattleFinish;
        LTHeroBattleEvent.NotifyHeroBattleDelayToScene += OnNotifyHeroBattleDelayToScene;
        // LTHeroBattleEvent.GetMatchData = OnGetMatchData;
		LTHeroBattleEvent.GetReloadData = OnGetReloadData;
		//EB.Coroutines.Run(TestOpen());
	}

	IEnumerator TestOpen()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                GlobalMenuManager.Instance.Open("LTHeroBattleMatch",LTHeroBattleModel.GetInstance().matchData);
            }
            yield return null;
        }
    }

    public override void Dispose()
    {
        LTHeroBattleEvent.ConfirmChoiceHero -= OnConfirmChoiceHero;
        LTHeroBattleEvent.ConfirmBanHero -= OnConfirmBanHero;
        LTHeroBattleEvent.ChoiceHero -= OnChoiceHero;
        // LTHeroBattleEvent.MatchOther -= OnMatchOther;
        // LTHeroBattleEvent.QuitMatchOther -= OnQuitMatchOther;
        // LTHeroBattleEvent.CloseAndQuitMatch -= OnCloseAndQuitMatch;
        // LTHeroBattleEvent.GetReward -= OnGetReward;
        // LTHeroBattleEvent.ChangeHeroType -= OnChangeHeroType;
        LTHeroBattleEvent.ChoiceSuitIndex -= OnChoiceSuitIndex;
        LTHeroBattleEvent.TouchSuitIndexTips -= OnTouchSuitIndexTips;
        LTHeroBattleEvent.NotifyRefreshChoiceState -= NotifyRefreshChoiceState;
        LTHeroBattleEvent.NotifyHeroBattleHudFinish -= OnNotifyHeroBattleFinish;
        LTHeroBattleEvent.NotifyHeroBattleDelayToScene -= OnNotifyHeroBattleDelayToScene;
        // LTHeroBattleEvent.GetMatchData = null;
        LTHeroBattleEvent.GetReloadData = null;
        
        base.Dispose();
    }

    public override void Connect()
    {
        State = EB.Sparx.SubSystemState.Connected;
    }

    public override void Disconnect(bool isLogout)
    {
        State = EB.Sparx.SubSystemState.Disconnected;
    }

    void OnConfirmChoiceHero()
    {
        int choiceHeroTplID = LTHeroBattleModel.GetInstance().choiceData.choiceHeroTplID;
        int suitIndex = LTHeroBattleModel.GetInstance().choiceData.choiceSuitIndex;
        int suitID = LTHeroBattleModel.GetInstance().choiceData.choiceHeroCellData.suits[suitIndex];
        //LTHeroBattleModel.GetInstance().c
        PostChoiceHero(choiceHeroTplID, suitID);
    }

    void OnConfirmBanHero()
    {
        int banHeroTplID = LTHeroBattleModel.GetInstance().choiceData.choiceHeroTplID;
        PostBanHero(banHeroTplID);
    }

    void OnChoiceHero(int heroTplID)
    {
        LTHeroBattleModel.GetInstance().SetChoiceHero(heroTplID);
    }

    void OnChoiceSuitIndex(int suitIndex)
    {
        LTHeroBattleModel.GetInstance().choiceData.choiceSuitIndex = suitIndex;
    }


    void OnMatchOther()
    {
        PostStartMatch();
    }

    void OnQuitMatchOther()
    {
        PostQuitMatch();
    }

    void OnCloseAndQuitMatch()
    {
        PoseQuitMatchAndClose();
    }

    void OnGetReward()
    {
        PostGetReward(null);
    }

    void OnChangeHeroType(Hotfix_LT.Data.eRoleAttr type)
    {
        LTHeroBattleModel.GetInstance().SetChoiceHeroType(type);
    }

    HeroBattleMatchData OnGetMatchData()
    {
        PostGetMatchBaseInfo(); //申请最新数据
        return LTHeroBattleModel.GetInstance().matchData;
    }

	void OnGetReloadData()
	{
		PostGetReloadData();
	}

    void OnTouchSuitIndexTips(int index)
    {
        //获取到当前选择的伙伴对应套装数据并发给视图

        // LTHeroBattleEvent.ShowSuitTips(index,EB.Localizer.GetString("ID_codefont_in_LTHeroBattleLogic_4886"));
    }    

    void NotifyRefreshChoiceState(HeroBattleChoiceData data)
    {
        if (LTHeroBattleModel.GetInstance().isWaitingMatch) //如果是匹配后第一次收到数据 需要打开vs数据并2秒后打开
        {
            LTHeroBattleModel.GetInstance().isWaitingMatch = false;
            LTHeroBattleModel.GetInstance().matchData.isWaiting = false;
            LadderMatchSuccessUIController.InfoData info = new LadderMatchSuccessUIController.InfoData();
            info.type = 1;
            info.info = data;
            GlobalMenuManager.Instance.CloseMenu("LTHeroBattleMatch");
            GlobalMenuManager.Instance.Open("LTMatchSuccessUI", info);
        }
    }    

    void OnNotifyHeroBattleDelayToScene()
    {
		SceneLogic.SceneState = SceneLogic.eSceneState.DelayCombatTransition;//选英雄界面 延迟进战斗场景
    }

    void OnNotifyHeroBattleFinish()
    {
        if(_HeroBattleFinish)
        {
            return;
        }
        _HeroBattleFinish = true;
        SceneLogic.SceneState = SceneLogic.eSceneState.DelayCombatTransition;//选英雄界面 延迟进战斗场景
        MainLandLogic.GetInstance().DoDelayCombatTransition(3, 5,
            (bool isDO) => 
            {
                _HeroBattleFinish = false;
                SceneLogic.SceneState = SceneLogic.eSceneState.RequestingCombatTransition;
            }
            );
    }

    public void PostGetMatchBaseInfo(System.Action<bool> callback=null)
    {
        api.GetMatchBaseInfo((EB.Sparx.Response response) =>
        {
			if (response.sucessful)
            {
                FetchDataHandler(response.hashtable);
				if (callback != null)
					callback(true);
				return true;
            }
			if (callback != null)
				callback(false);
			return false;
        });
    }

	public void PostGetReloadData()
	{
		api.ReloadClash((EB.Sparx.Response response) =>
		{
			if (response.sucessful)
			{
				FetchDataHandler(response.hashtable);
				return true;
			}
			return false;
		});
	}

	public void PostStartMatch()
    {
        LTHeroBattleModel.GetInstance().isWaitingMatch = true;
        LTHeroBattleModel.GetInstance().matchData.isWaiting = true;
        api.StartMatchOther((EB.Sparx.Response response) =>		
        {
            if (response.sucessful)
            {
                // if (LTHeroBattleEvent.NotifyMatchOtherWaiting != null)
                // {
                //     LTHeroBattleEvent.NotifyMatchOtherWaiting();
                // }
                return true;
            }
            else if(response.error.ToString().Equals("Error: no server found")|| response.error.ToString().Equals("Error: service not found"))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleMatchHudController_10973"));
                LTHeroBattleModel.GetInstance().isWaitingMatch = false;
                LTHeroBattleModel.GetInstance().matchData.isWaiting = false;
                return true;
            }
            return false;
        });
    }

    public void PostQuitMatch()
    {
        api.QuitMatchOther((EB.Sparx.Response response) =>
        {
            if (response.sucessful)
            {
                LTHeroBattleModel.GetInstance().isWaitingMatch = false;
                LTHeroBattleModel.GetInstance().matchData.isWaiting = false;
                // if (LTHeroBattleEvent.NotifyQuitMatchOther != null)
                // {
                //     LTHeroBattleEvent.NotifyQuitMatchOther();
                // }
                return true;
            }
            return false;
        });
    }

    public void  PoseQuitMatchAndClose()
    {
        api.QuitMatchOther((EB.Sparx.Response response) =>
        {
            if (response.sucessful)
            {
                LTHeroBattleModel.GetInstance().isWaitingMatch = false;
                LTHeroBattleModel.GetInstance().matchData.isWaiting = false;
                // if (LTHeroBattleEvent.NotifyQuitMatchOther != null)
                // {
                //     LTHeroBattleEvent.NotifyQuitMatchOther();
                // }
                GlobalMenuManager.Instance.CloseMenu("LTHeroBattleMatch");
                return true;
            }
            return false;
        });
    }

    public void PostGetReward(System.Action<bool> callback)
    {		
        api.GetReward((EB.Sparx.Response response) =>
        {
			if(callback!=null)
				callback(response.sucessful);
            if (response.sucessful)
            {
                FetchDataHandler(response.hashtable);
                return true;
            }
            return false;
        });
    }

    /// <summary>
    /// 请求禁用的英雄
    /// </summary>
    /// <param name="heroTplID"></param>
    public void PostBanHero(int heroTplID)
    {
        api.BanOtherHero(heroTplID, (EB.Sparx.Response response) =>
        {
            if (response.sucessful)
            {
                return true;
            }

            if (response.error.Equals("fail"))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleLogic_9862"));
                return true;
            }
            return false;
        });
    }

    /// <summary>
    /// 请求选择的英雄
    /// </summary>
    /// <param name="heroTplID"></param>
    /// <param name="suitID"></param>
    public void PostChoiceHero(int heroTplID, int suitID)
    {
        api.ChoiceHero(heroTplID,suitID, (EB.Sparx.Response response) =>
        {
            if (response.sucessful)
            {
                return true;
            }
            if (response.error.Equals("fail"))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleLogic_10482"));
                return true;
            }
            return false;
        });
    }

    private void FetchDataHandler(Hashtable data)
    {
        if (data != null)
        {
            GameDataSparxManager.Instance.ProcessIncomingData(data, false);
        }
    }

    private void MergeDataHandler(Hashtable data)
    {
        if (data != null)
        {
            GameDataSparxManager.Instance.ProcessIncomingData(data, true);
        }
    }    
}

}