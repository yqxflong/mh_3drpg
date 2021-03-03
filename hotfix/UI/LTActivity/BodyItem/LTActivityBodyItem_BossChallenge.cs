using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
	/// <summary>
	/// 首领挑战
	/// </summary>
	public class LTActivityBodyItem_BossChallenge : LTActivityBodyItem
    {
        private UIGrid _uiGrid;
        private Transform _parent;
        private GameObject _itemTemplate;

        private static string HasEnterShopKey { 
            get { 
                return LoginManager.Instance.LocalUserId.Value.ToString() + "HasEnterBossChallengeShop"; 
            } 
        }

        public static string HasEnterActivityKey { 
            get { 
                return LoginManager.Instance.LocalUserId.Value.ToString() + "HasEnterBossChallengeActivity"; 
            } 
        }

        public static string CanBuyCountFromShopKey  { 
            get  { 
                return LoginManager.Instance.LocalUserId.Value.ToString() + "CanBuyCountFromBossChallengeShop";
            }
        }

        public override void Awake()
        {
            base.Awake();

            _uiGrid = mDMono.transform.GetComponent<UIGrid>("Scroll View/Grid");
            _parent = _uiGrid.transform;
            _itemTemplate = mDMono.gameObject.FindEx("Scroll View/Grid/Item");

            UIButton btnShop = mDMono.transform.GetComponent<UIButton>("Btn_Shop");
			UIButton btnReward = mDMono.transform.GetComponent<UIButton>("Btn_Rewards");

			btnShop?.onClick.Add(new EventDelegate(OnClickShopButton));
			btnReward?.onClick.Add(new EventDelegate(OnClickRewardsButton));
		}

        private static string _eventType;
        private static int _endTime;
		private int activityId;

        public override void SetData(object data)
        {
            base.SetData(data);

            _endTime = fintime;
            _eventType = eventType;
            int timeZone = ZoneTimeDiff.GetTimeZone();

            if (desc != null)
            {
                desc.text = EB.Time.FromPosixTime(starttime + timeZone * 3600).ToString("yyyy.MM.dd") + " - " + EB.Time.FromPosixTime(disappeartime + timeZone * 3600).ToString("yyyy.MM.dd");
            }

            int timeTemp = Mathf.Max(fintime - EB.Time.Now, 0);
            System.TimeSpan ts = new System.TimeSpan(0, 0, timeTemp);
            PlayerPrefs.SetInt(HasEnterActivityKey, ts.Days);
            PlayerPrefs.Save();

			activityId = EB.Dot.Integer("activity_id", data, 0);

			if (state.Equals("pending")) 
            {
                PlayerPrefs.DeleteKey(HasEnterShopKey);
                PlayerPrefs.DeleteKey(CanBuyCountFromShopKey);
                PlayerPrefs.DeleteKey(HasEnterActivityKey);
                _parent.gameObject.SetActive(false);
            }
            else if (state.Equals("running"))
            {
                _parent.gameObject.SetActive(true);
                RefreshShopRedPoint();
				RefreshRewardRedPoint();
            }
            else
            {
                if (_tips1 != null)
                {
                    _tips1.gameObject.SetActive(false);
                }

                if (_tips2Label != null)
                {
                    _tips2Label.transform.parent.gameObject.SetActive(true);
                }

                if (EB.Time.Now >= disappeartime)
                {
                    PlayerPrefs.DeleteKey(HasEnterShopKey);
                    PlayerPrefs.DeleteKey(CanBuyCountFromShopKey);
                    PlayerPrefs.DeleteKey(HasEnterActivityKey);
                }

                _parent.gameObject.SetActive(false);
                Event_Time.color = Color.red;
            }

            SetCurrency(_eventType);
            
            InitBtnList(activityId, fintime);

            if (title != null)
            {
                title.UpdateRedPoint();
            }
        }

        private bool _isInit = false;

        private void InitBtnList(int activityId, int endTime)
        {
            if (_isInit)
            {
                return;
            }

            _isInit = true;

            var list = EventTemplateManager.Instance.GetBossChallengeActivityConfigs(activityId);

            list.Sort((left, right) => 
            {
                if (left.difficulty < right.difficulty)
                {
                    return -1;
                }
                else if (left.difficulty > right.difficulty)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });

            var modelName = string.Empty;
            int passDifficulty;
            
            if (!DataLookupsCache.Instance.SearchIntByID(string.Format("time_limit_activity.{0}.currentPassDifficulty", activityId), out passDifficulty))
            {
                DataLookupsCache.Instance.SearchIntByID(string.Format("tl_acs.{0}.current_pass_difficulty", activityId), out passDifficulty);
            }

            for (var i = 0; i < list.Count; i++)
            {
                var go = GameObject.Instantiate(_itemTemplate, _parent);
                go.SetActive(true);
                var item = go.GetMonoILRComponent<LTBossChallengeActivityItem>();
                var lastLevelName = (i - 1) >= 0 ? list[i - 1].name : string.Empty;
                var data = list[i];
                item.SetData(data.difficulty > passDifficulty + 1, lastLevelName, data, title, endTime);

                if (string.IsNullOrEmpty(modelName))
                {
                    var layoutTemplate = SceneTemplateManager.Instance.GetMaxWaveLayout(data.layout);

                    if (layoutTemplate != null)
                    {
                        var monsterTemplate = CharacterTemplateManager.Instance.GetMonsterInfo(int.Parse(layoutTemplate.Model));

                        if (monsterTemplate != null)
                        {
                            var heroInfo = CharacterTemplateManager.Instance.GetHeroInfo(monsterTemplate.character_id.ToString());

                            if (heroInfo != null) 
                            {
                                modelName = heroInfo.model_name;
                            }
                        }
                    }

                    SetTips(data.partner_attr, data.bonus_add);
                }
            }

            _uiGrid.Reposition();

            if (mDMono.gameObject.activeInHierarchy)
            { 
                StartCoroutine(SetModel(modelName)); 
            }
        }

        UITexture _lobbyTex;
        UI3DLobby _ui3DLobby;

        private IEnumerator SetModel(string modelName)
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            if (string.IsNullOrEmpty(modelName))
            {
                EB.Debug.LogError("LTActivityBodyItem_BossChallenge.SetModel: modelName is null or empty"); 
                yield break;
            }

            if (_lobbyTex == null)
            {
                _lobbyTex = mDMono.transform.GetComponent<UITexture>("LobbyTexture");
            }

            if (_ui3DLobby == null)
            {
                var Loader = new GM.AssetLoader<GameObject>("UI3DLobby", mDMono.gameObject);
                UI3DLobby.Preload(modelName);
                yield return Loader;

                if (Loader.Success)
                {
                    Loader.Instance.transform.parent = _lobbyTex.transform;
                    _ui3DLobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
                    _ui3DLobby.ConnectorTexture = _lobbyTex;
                    _ui3DLobby.VariantName = modelName;
                    _ui3DLobby.SetCameraRot(new Vector3(14f, 0f, 0f));

                    switch (_eventType) 
                    {
                        case "bosschallenge1":  //地狱三头犬
                            _ui3DLobby.SetCameraMode(4f, true);
                            _ui3DLobby.SetCameraPos(new Vector3(0.3f, 1.4f, -0.5f));
                            break;
                        case "bosschallenge2":  //猛犸巨象
                            _ui3DLobby.SetCameraMode(4.5f, true);
                            _ui3DLobby.SetCameraPos(new Vector3(0.3f, 1.82f, -1.34f));
                            break;
                        case "bosschallenge3":  //死亡镰刀
                            _ui3DLobby.SetCameraMode(5f, true);
                            _ui3DLobby.SetCameraPos(new Vector3(-0.25f, 2.44f, -2.16f));
                            break;
                    }
                }
            }
        }

        private UISprite _tips1;
        private UILabel _tips1Label1;
        private UILabel _tips1Label2;
        private UILabel _tips1Label3;
        private UILabel _tips2Label;

        private void SetTips(int type, int num)
        {
            if (_tips1 == null)
            {
                _tips1 = mDMono.transform.GetComponent<UISprite>("Tips_1");
                _tips1.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[(eRoleAttr)type];
            }

            if (_tips1Label1 == null)
            {
                _tips1Label1 = mDMono.transform.GetComponent<UILabel>("Tips_1/Label_1");
                _tips1Label1.text = EB.Localizer.GetString("ID_BOSS_CHALLENGE_PER_PLAYER");
            }

            if (_tips1Label2 == null)
            {
                _tips1Label2 = mDMono.transform.GetComponent<UILabel>("Tips_1/Label_2");
                _tips1Label2.text = EB.Localizer.GetString("ID_PARTNER");
            }

            if (_tips1Label3 == null)
            {
                _tips1Label3 = mDMono.transform.GetComponent<UILabel>("Tips_1/Label_3");
            }
            
            _tips1Label3.text = EB.Localizer.GetString("ID_LEGION_CHALLENGE_REWARD") + string.Format("+{0}%", num);

            if (_tips2Label == null)
            {
                _tips2Label = mDMono.transform.GetComponent<UILabel>("Tips_2/Label");
                _tips2Label.text = EB.Localizer.GetString("ID_BOSS_CHALLENGE_END_TIPS");
            }
        }

        private CurrencyDisplay _currencyDisplay1;
        private CurrencyDisplay _currencyDisplay2;

        private void SetCurrency(string shopType)
        {
            var shopTemplate = ShopTemplateManager.Instance.GetShopByShopType(shopType);
            
            if (shopTemplate == null)
            {
                EB.Debug.LogError("LTActivityBodyItem_BossChallenge.SetCurrency: shopTemplate is null");
                return;
            }

            string[] strs = shopTemplate.shop_balance_type.Split(',');

            if (_currencyDisplay1 == null)
            {
                _currencyDisplay1 = mDMono.transform.GetMonoILRComponent<CurrencyDisplay>("Currency_1");
            }

            if (_currencyDisplay1 != null && strs != null && strs.Length > 0)
            {
                var count = BalanceResourceUtil.NumFormat(BalanceResourceUtil.GetDataLookupValue(string.Format("res.{0}.v", strs[0])).ToString());
                _currencyDisplay1.SetData(count, BalanceResourceUtil.GetResSpriteName(strs[0]));
				_currencyDisplay1.SetPopTip(LTShowItemType.TYPE_RES, strs[0]);
			}

            if (_currencyDisplay2 == null)
            {
                _currencyDisplay2 = mDMono.transform.GetMonoILRComponent<CurrencyDisplay>("Currency_2");
            }

            if (_currencyDisplay2 != null && strs != null && strs.Length > 1)
            {
                var count = BalanceResourceUtil.NumFormat(BalanceResourceUtil.GetDataLookupValue(string.Format("res.{0}.v", strs[1])).ToString());
                _currencyDisplay2.SetData(count, BalanceResourceUtil.GetResSpriteName(strs[1]));
				_currencyDisplay2.SetPopTip(LTShowItemType.TYPE_RES, strs[1]);
			}
        }

		private void OnClickShopButton()
        {
            GlobalMenuManager.Instance.Open("LTBossChallengeStoreUI", _eventType);
            PlayerPrefs.SetInt(HasEnterShopKey, 1);
            PlayerPrefs.SetInt(CanBuyCountFromShopKey, _canBuyCount);
            _canBuyCount = 0;
            PlayerPrefs.Save();
        }

        private GameObject _shopRedPoint;

		private void OnClickRewardsButton()
		{
			GlobalMenuManager.Instance.Open("LTActivityChiefRewardHud", activityId);
		}

		private GameObject _rewardRedPoint;

		private void RefreshShopRedPoint()
        {
            if (_shopRedPoint == null)
            {
                _shopRedPoint = mDMono.transform.gameObject.FindEx("Btn_Shop/RedPoint");
            }

            if (_shopRedPoint != null)
            {
                _shopRedPoint.SetActive(IsShowRedPointInShop());
            }
        }

		private void RefreshRewardRedPoint()
		{
			if (_rewardRedPoint == null)
			{
				_rewardRedPoint = mDMono.transform.gameObject.FindEx("Btn_Rewards/RedPoint");
			}

			if (_rewardRedPoint != null)
			{
				_rewardRedPoint.SetActive(IsShowRedPointOnReward(activityId));
			}
		}

		private static int _canBuyCount;

		public static bool IsShowRedPointOnReward(int activityId)
		{
			List<TimeLimitActivityStageTemplate> templates = Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(activityId);

			DataLookupsCache.Instance.SearchDataByID("tl_acs." + activityId, out Hashtable activityData);

			return templates.Exists(
				t => {
					int receivedValue = EB.Dot.Integer(string.Format("stages.{0}", t.id), activityData, 0);
					int winCount = EB.Dot.Integer("current", activityData, 0);
					return receivedValue == 0 && winCount >= t.stage;
				}
			);
		}

        public static bool IsShowRedPointInShop()
        {
            _canBuyCount = GetCanBuyCount(_eventType);
            bool showRedPoint;

            // 0：未进入过商店；1：进入过商店
            if (PlayerPrefs.GetInt(HasEnterShopKey, 0) == 0)
            {
                showRedPoint = _canBuyCount > 0;
            }
            else
            {
                var lastCount = PlayerPrefs.GetInt(CanBuyCountFromShopKey, 0);
                showRedPoint = _canBuyCount > lastCount;
            }

            return showRedPoint;
        }

        public static int GetCanBuyCount(string eventType)
        {
            var shopTemplate = ShopTemplateManager.Instance.GetShopByShopType(eventType);

            if (shopTemplate == null)
            {
                return 0;
            }

            string[] strs = shopTemplate.shop_balance_type.Split(',');

            if (strs == null)
            {
                return 0;
            }

            var list = new List<string>(strs);
            int canBuyCount = 0;
            ArrayList itemList;

            if (DataLookupsCache.Instance.SearchDataByID(string.Format("userShops.{0}.itemList", eventType), out itemList) && itemList != null)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    int id = EB.Dot.Integer("id", itemList[i], 0);
                    int num = EB.Dot.Integer("num", itemList[i], 0);
                    BossChallengeTemplate tpl = null;

                    switch (_eventType)
                    {
                        case "bosschallenge1":
                            tpl = ShopTemplateManager.Instance.GetBossChallenge1Template(id);
                            break;
                        case "bosschallenge2":
                            tpl = ShopTemplateManager.Instance.GetBossChallenge2Template(id);
                            break;
                        case "bosschallenge3":
                            tpl = ShopTemplateManager.Instance.GetBossChallenge3Template(id);
                            break;
                    }

                    if (num > 0 && tpl != null && list.Contains(tpl.balance_name))
                    {
                        var count = BalanceResourceUtil.GetDataLookupValue(string.Format("res.{0}.v", tpl.balance_name));

                        if (count >= tpl.balance_num)
                        {
                            canBuyCount += 1;
                        }
                    }
                }
            }
            else
            {
                Dictionary<int, BossChallengeTemplate> dict = null;

                switch (_eventType)
                {
                    case "bosschallenge1":
                        dict = ShopTemplateManager.Instance.GetAllBossChallenge1Template();
                        break;
                    case "bosschallenge2":
                        dict = ShopTemplateManager.Instance.GetAllBossChallenge2Template();
                        break;
                    case "bosschallenge3":
                        dict = ShopTemplateManager.Instance.GetAllBossChallenge3Template();
                        break;
                }

                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        if (list.Contains(kvp.Value.balance_name))
                        {
                            var count = BalanceResourceUtil.GetDataLookupValue(string.Format("res.{0}.v", kvp.Value.balance_name));

                            if (count >= kvp.Value.balance_num)
                            {
                                canBuyCount += 1;
                            }
                        }
                    }
                }
            }

            return canBuyCount;
        }

        private static string BossChallengeSpecialCameraKey
        {
            get
            {
                return string.Format("BossChallengeSpecialCamera_{0}_{1}", LoginManager.Instance.LocalUserId.Value, _endTime);
            }
        }

        public static bool IsPlayBossChallengeSpecialCamera()
        {
            bool isPlay = false;
            int flag = PlayerPrefs.GetInt(BossChallengeSpecialCameraKey, 0);

            if (flag == 0)
            {
                isPlay = true;
                PlayerPrefs.SetInt(BossChallengeSpecialCameraKey, 1);
                PlayerPrefs.Save();
            }

            return isPlay;
        }
    }
}