using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTWorldBossHudController : UIControllerHotfix, IHotfixUpdate
    {
        public override bool ShowUIBlocker
        {
            get
            {
                return false;
            }
        }
    
        public override bool IsFullscreen()
        {
            return true;
        }
    
        public static int ResetLastTime = -1;
    
        public UIServerRequest BuyTimesRequest;
        public UIServerRequest RollRequest;
        public UIServerRequest GetInfoRequest;
    
        public UILabel AttackTimesLabel;
        public UIButton AttackBtn;
        public List<LTWorldBossNodeRewardLogic> NodeRewardList;
        
        public UILabel NameLabel;
        public UISprite TypeSprite;
    
        private ParticleSystemUIComponent charFx;
        private EffectClip efClip;
    
        public UILabel OpenTimeLabel;
    
        public float mMaxTime = 60;
        private float mUpdataTime;
    
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            BuyTimesRequest = t.GetComponent<UIServerRequest>("BuyTimes");
            RollRequest = t.GetComponent<UIServerRequest>("GetRollInfo");
            GetInfoRequest = t.GetComponent<UIServerRequest>("GetInfo");
            AttackTimesLabel = t.GetComponent<UILabel>("Center/PutRewardView/AtkBtn/TimeLabel");
            AttackBtn = t.GetComponent<UIButton>("Center/PutRewardView/AtkBtn");

            NodeRewardList = new List<LTWorldBossNodeRewardLogic>();
            NodeRewardList.Add(t.GetMonoILRComponent<LTWorldBossNodeRewardLogic>("Center/PutRewardView/PutReward/Item"));
            NodeRewardList.Add(t.GetMonoILRComponent<LTWorldBossNodeRewardLogic>("Center/PutRewardView/PutReward/Item (1)"));
            NodeRewardList.Add(t.GetMonoILRComponent<LTWorldBossNodeRewardLogic>("Center/PutRewardView/PutReward/Item (2)"));

            NameLabel = t.GetComponent<UILabel>("Center/BossInfo/NameLabel");
            TypeSprite = t.GetComponent<UISprite>("Center/BossInfo/NameLabel/Sprite");
            OpenTimeLabel = t.GetComponent<UILabel>("Edge/Bottom");
            mMaxTime = 60f;
            LobbyTexture = t.GetComponent<UITexture>("Center/BossInfo/Lobby");
            controller.backButton = t.GetComponent<UIButton>("Edge/TopLeft/CancelBtn");

            t.GetComponent<UIButton>("Edge/TopLeft/Notice").onClick.Add(new EventDelegate(OnRuleBtnClick));

            var uiWorldBossRankLogic = t.GetDataLookupILRComponent<UIWorldBossRankLogic>("Edge/Right/LTWorldBossRankView");
            t.GetComponent<UIButton>("Edge/NewCurrency/BtnList/PreviewBtn").onClick.Add(new EventDelegate(uiWorldBossRankLogic.OnRewardBtnClick));
            t.GetComponent<UIButton>("Edge/NewCurrency/BtnList/RankListBtn").onClick.Add(new EventDelegate(uiWorldBossRankLogic.OnRankBtnClick));
            t.GetComponent<UIButton>("Edge/NewCurrency/BtnList/ReadyBtn").onClick.Add(new EventDelegate(uiWorldBossRankLogic.OnBattleReadyBtnClick));
            t.GetComponent<UIButton>("Edge/Right/LTWorldBossRankView/Rank/CancelBtn").onClick.Add(new EventDelegate(uiWorldBossRankLogic.OnRankCancelBtnClick));

            t.GetComponent<UIButton>("Center/PutRewardView/AtkBtn").onClick.Add(new EventDelegate(OnAtkBtnClick));

            t.GetComponent<UIServerRequest>("GetInfo").onResponse.Add(new EventDelegate(controller, "OnFetchData"));
            t.GetComponent<UIServerRequest>("BuyTimes").onResponse.Add(new EventDelegate(controller, "OnFetchData"));
            t.GetComponent<UIServerRequest>("GetRollInfo").onResponse.Add(new EventDelegate(controller, "OnFetchData"));

            Messenger.AddListener(Hotfix_LT.EventName.BossDieEvent,OnBossDieFunc);
        }

        public override void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
        {
            base.OnFetchData(res, reqInstanceID);

            if (reqInstanceID == GetInfoRequest.GetInstanceID())
            {
                OnGetInfoResponse(res);
            }
            if (reqInstanceID == BuyTimesRequest.GetInstanceID())
            {
                OnBuyResponse(res);
            }
            if (reqInstanceID == RollRequest.GetInstanceID())
            {
                OnInitRollDiceInfoResponse(res);
            }
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public void Update()
        {
            // base.Update();
            if(mUpdataTime< mMaxTime)
            {
                mUpdataTime += Time.deltaTime;
            }
            else
            {
                OnInitRollDiceInfo();
            }
        }

        public override void OnDestroy()
        {
           Messenger.RemoveListener(Hotfix_LT.EventName.BossDieEvent,OnBossDieFunc);
            base.OnDestroy();
        }
    
        public override IEnumerator OnAddToStack()
        {
            var curDailyData = LTDailyDataManager.Instance.GetDailyDataByActivityID(LTWorldBossDataManager.ActivityId);
            if (curDailyData != null)
            {
                OpenTimeLabel.text = string.Format("{0}{1}", EB.Localizer.GetString("ID_ACTIVITY_TIME_TITLE"), curDailyData.OpenTimeStr);
            }
            yield return base.OnAddToStack();
            OnGetInfo();
            yield return null;
            OnInitRollDiceInfo();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            if (Lobby != null)
            {
                Object.Destroy(Lobby.mDMono.gameObject);
                Lobby = null;
            }
            if (Loader != null)
            {
                EB.Assets.UnloadAssetByName("UI3DLobby", false);
                Loader = null;
            }
            ModelName = null;
            for (int i = 0; i < NodeRewardList.Count; i++)
            {
                NodeRewardList[i].CloseUI();
            }
    
            DestroySelf();
            yield return base.OnRemoveFromStack();
        }

        public override void Show(bool isShowing)
        {
            base.Show(isShowing);
            controller.transform.localPosition = isShowing ? Vector3.zero : Vector3.up * 9999;
        }

        private void InitUI()
        {
            InitBoss();
            InitLabel();
        }
    
        private void InitBoss()
        {
            int monsterID = LTWorldBossDataManager.Instance.GetWorldBossMonsterID();
            Hotfix_LT.Data.MonsterInfoTemplate monsterTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(monsterID);
            if (monsterTpl == null)
            {
                EB.Debug.LogError("LTWorldBossHudController InitBoss, monsterTpl is Error monsterID = {0}", monsterID);
                return;
            }
    
            Hotfix_LT.Data.HeroInfoTemplate info = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(monsterTpl.character_id);
            if (info == null)
            {
                EB.Debug.LogError("LTWorldBossHudController InitBoss, info is Error monsterTpl.character_id = {0}", monsterTpl.character_id);
                return;
            }
    
            StartCoroutine(CreateBossModel(info.model_name));
    
            LTUIUtil.SetText(NameLabel, info.name);
            TypeSprite.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[info.char_type]; 
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, TypeSprite.transform, (PartnerGrade)info.role_grade,info.char_type);
        }
    
        private void InitLabel()
        {
            bool isStart = LTWorldBossDataManager.Instance.IsWorldBossStart();
            MaxTimes = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("worldBossChallengeMaxTimes");
            //DataLookupsCache.Instance.SearchIntByID("world_boss.fightTimes", out ChallengeTime);
            //DataLookupsCache.Instance.SearchIntByID("world_boss.buyTimes", out BuyTimes);
            AttackTimesLabel.text = string.Format(EB.Localizer.GetString("ID_uifont_in_ArenaHudUI_LeftTimes_1") + "{0}/{1}", MaxTimes - ChallengeTime + BuyTimes, MaxTimes+ BuyTimes);
            AttackBtn.gameObject.CustomSetActive(isStart);
        }
    
        private void InitRollRewards()
        {
            var datas = Hotfix_LT.Data.EventTemplateManager.Instance.GetBossStages();
            for (int i = 0; i < NodeRewardList.Count; i++)
            {
                if (i < datas.Count)
                {
                    NodeRewardList[i].InitUI(datas[i]);
                }
                else
                {
                    NodeRewardList[i].CloseUI(); ;
                }
            }
        }
    
        public UITexture LobbyTexture;
        private UI3DLobby Lobby = null;
        private GM.AssetLoader<GameObject> Loader = null;
        private string ModelName = null;
        private const int CharacterPoolSize = 10;
        private bool isInitLobby;
    
        private IEnumerator CreateBossModel(string modelName)
        {
            isInitLobby = true;
            if (string.IsNullOrEmpty(modelName))
            {
                isInitLobby = false;
                yield break;
            }
    
            if (modelName == ModelName)
            {
                isInitLobby = false;
                yield break;
            }
    
            ModelName = modelName;
            if (Lobby == null && Loader == null)
            {
                Loader = new GM.AssetLoader<GameObject>("UI3DLobby", controller.gameObject);
                UI3DLobby.Preload(modelName);
                yield return Loader;
                if (Loader.Success)
                {
                    Loader.Instance.transform.parent = LobbyTexture.transform;
                    Lobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
                    Lobby.ConnectorTexture = LobbyTexture;
                    Lobby.CharacterPoolSize = CharacterPoolSize;
                    Camera Camera = Lobby.mDMono.transform.Find("UI3DCamera").GetComponent<Camera>();
                    Camera.orthographicSize = 4.2f;
                }
            }
    
            if (Lobby != null)
            {
                Lobby.VariantName = modelName;
                yield return null;
                Lobby.SetCharMoveState(MoveController.CombatantMoveState.kEntry, true);
            }
            isInitLobby = false;
        }
    
        private float clickTime;
        private bool isCouldClick()
        {
            if (Time.time - clickTime < 1f)
            {
                return false;
            }
            clickTime = Time.time;
            return true;
        }
    
        public void OnRewardBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTWorldBossRewardPreviewUI");
        }
    
        private int MaxTimes = 0;
        private int ChallengeTime=0;
        private int BuyTimes = 0;
        public void OnAtkBtnClick()
        {
            if (!isCouldClick())
            {
                return;
            }

            if (!LTWorldBossDataManager.Instance.IsLive())
            {
                MessageTemplateManager.ShowMessage(902185);
                return;
            }
    
            if (!LTWorldBossDataManager.Instance.IsWorldBossStart())
            {
                MessageTemplateManager.ShowMessage(902090);
                return;
            }
            if (MaxTimes - ChallengeTime + BuyTimes <= 0)
            {
                OnBuyTimes();
                return;
            }

            GameObject player = PlayerManager.LocalPlayerGameObject();

            if (player == null)
            {
                EB.Debug.LogError("Hotfix_LT.UI.LTWorldBossHudController.OnAtkBtnClick() -> player is null");
                return;
            }

            if (!MainLandLogic.GetInstance().EnemyControllers.ContainsKey("EnemySpawns_11"))
            {
                EB.Debug.LogError("Hotfix_LT.UI.LTWorldBossHudController.OnAtkBtnClick() -> The 'EnemySpawns_11' key was not present in the dictionary");
                return;
            }

            EnemyController emCtrl = MainLandLogic.GetInstance().EnemyControllers["EnemySpawns_11"];
            MainLandLogic.GetInstance().RequestCombatTransition(eBattleType.BossBattle, player.transform.position, player.transform.rotation, new List<EnemyController> { emCtrl });
        }
    
        private void OnBuyTimes()
        {
            if(BuyTimes >= Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("worldBossBuyChallengeMaxTimes"))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType .FloatingText ,EB.Localizer .GetString ("ID_codefont_in_LTResourceShopController_3145"));//��������
                return;
            }
            float cost= Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("worldBossBuyChallengeTimesBase")+ Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("worldBossBuyChallengeTimes")*BuyTimes;
            if (BalanceResourceUtil.GetUserDiamond() < cost)//��ʯ����
            {
                BalanceResourceUtil.HcLessMessage();
                return;
            }

            var ht = Johny.HashtablePool.Claim();
            ht.Add("0", cost);
            MessageTemplateManager.ShowMessage(902123, ht, delegate (int result) {
                if (result == 0)
                {
                    LoadingSpinner.Show();
                    BuyTimesRequest.SendRequest();
                }
            });
            Johny.HashtablePool.Release(ht);
        }
    
        public void OnBuyResponse(EB.Sparx.Response response)
        {
            LoadingSpinner.Hide();
            if (response.sucessful)
            {
                if(response.hashtable!=null) DataLookupsCache.Instance.CacheData(response.hashtable);
                BuyTimes = EB.Dot.Integer("world_boss.buyTimes", response.hashtable, 0);
                InitLabel();
                MessageTemplateManager.ShowMessage(eMessageUIType .FloatingText ,EB .Localizer .GetString ("ID_codefont_in_LTChallengeInstanceShopCtrl_4116"));
                LTDailyDataManager.Instance.SetDailyDataRefreshState();
            }
            else if (response.fatal)
            {
                SparxHub.Instance.FatalError(response.localizedError);
            }
            else
            {
                EB.Debug.Log("worldboss/buyChallengeTimes����response =>{0}", response.hashtable.ToString());
            }
        }
    
        private void OnInitRollDiceInfo()
        {
            //worldboss/getRollDiceInfo��ȡҡɫ������
            mUpdataTime = 0;
            RollRequest.SendRequest();
        }
    
        public void OnInitRollDiceInfoResponse(EB.Sparx.Response response)
        {
            if (response.sucessful)
            {
                if (response.hashtable != null) DataLookupsCache.Instance.CacheData(response.hashtable);
                InitRollRewards();
            }
            else if (response.fatal)
            {
                SparxHub.Instance.FatalError(response.localizedError);
            }
            else
            {
                EB.Debug.Log("worldboss/buyChallengeTimes����response =>{0}" ,response.hashtable.ToString());
            }
        }
    
        private void OnGetInfo()
        {
            LoadingSpinner.Show();
            GetInfoRequest.SendRequest();
        }
    
        public void OnGetInfoResponse(EB.Sparx.Response response)
        {
            if (response.sucessful)
            {
                if (response.hashtable != null)
                {
                    DataLookupsCache.Instance.CacheData(response.hashtable);
                    ChallengeTime = EB.Dot.Integer("world_boss.fightTimes", response.hashtable, 0);
                    BuyTimes = EB.Dot.Integer("world_boss.buyTimes", response.hashtable, 0);
                }
    
                InitUI();
            }
            else if (response.fatal)
            {
                SparxHub.Instance.FatalError(response.localizedError);
            }
            else
            {
                EB.Debug.Log("worldboss/getInfo����response =>{0}", response.hashtable.ToString());
            }
            LoadingSpinner.Hide();
        }
    
        public void OnRuleBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString("ID_codefont_in_LTAllianceCampaignHudCtrl_1852"));
        }
        
        private void OnBossDieFunc()
        {
            controller.Close();
        }
    
        public override void OnCancelButtonClick()
        {
            if (isInitLobby)
            {
                return;
            }
            base.OnCancelButtonClick();
        }
    }
}
