using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{

    /// <summary>
    /// 军团副本界面的BOSS信息界面
    /// 对应挂载的预置体：LTLegionFBUI
    /// </summary>

    public class LTLegionFBBossInfo : DynamicMonoHotfix
    {

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            v_MonsterName = new UILabel[2];
            v_MonsterName[0] = t.GetComponent<UILabel>("RewardPanel/ItemPanel/TheTopOne/Name");
            v_MonsterName[1] = t.GetComponent<UILabel>("RewardPanel/ItemPanel/TheTopOne/Name/Label (1)");
            v_LeftBossBtn = t.FindEx("RewardPanel/ItemPanel/Left").gameObject;
            v_LeftBossRes = t.FindEx("RewardPanel/ItemPanel/Left/Sprite").gameObject;
            v_RightBossBtn = t.FindEx("RewardPanel/ItemPanel/Right").gameObject;
            v_RightBossRes = t.FindEx("RewardPanel/ItemPanel/Right/Sprite").gameObject;
            CreateModelFx = t.FindEx("Fx").gameObject;
            TemplateItem = t.GetMonoILRComponent<LTShowItem>("RewardPanel/ItemPanel/ScrollView/LTShowItem");
            TemplateItem.mDMono.gameObject.CustomSetActive(false);
            ItemGrid = t.GetComponent<UIGrid>("RewardPanel/ItemPanel/ScrollView/Grid");
            LobbyTexture = t.GetComponent<UITexture>("RewardPanel/Texture");
            v_ChallengeBtn = t.parent.FindEx("ChallengeButton").gameObject;
            v_WipeOutBtn = t.parent.FindEx("WipeOutBtn").gameObject;
            v_ChallengeTotal = new UILabel[2];
            v_ChallengeTotal[0] = t.parent.GetComponent<UILabel>("ChallengeButton/Total");
            v_ChallengeTotal[1] = t.parent.GetComponent<UILabel>("ChallengeButton/Total/Label");
            v_WipeOutTotal = new UILabel[2];
            v_WipeOutTotal[0] = t.parent.GetComponent<UILabel>("WipeOutBtn/Total");
            v_WipeOutTotal[1] = t.parent.GetComponent<UILabel>("WipeOutBtn/Total/Label");
            v_ProgressPanel = t.parent.FindEx("Progress").gameObject;
            v_ProgressLabel = new UILabel[2];
            v_ProgressLabel[0] = t.parent.GetComponent<UILabel>("Progress/Content");
            v_ProgressLabel[1] = t.parent.GetComponent<UILabel>("Progress/Content/Label");
            v_ProgressBar = t.parent.GetComponent<UIProgressBar>("Progress/Progress");
            v_UIController = t.parent.parent.parent.GetUIControllerILRComponent<LTLegionFBHudController>();
            v_RankController = t.parent.GetMonoILRComponentByClassPath<LTLegionFBRankController>("Level", "Hotfix_LT.UI.LTLegionFBRankController");
            UIEventListener.Get(v_LeftBossBtn).onClick += OnClickLeftBtn;
            UIEventListener.Get(v_RightBossBtn).onClick += OnClickRightBtn;
            UIEventListener.Get(v_ChallengeBtn).onClick += OnClickChallenBtn;
            UIEventListener.Get(v_WipeOutBtn).onClick += OnClickWipeOutBtn;
            m_AllReward = new List<LTShowItem>();
            m_ShowWipeOutList = new List<LTShowItemData>();

        }


        /// <summary>
        /// 当前的BOSS挑战次数<Key:bossid,value:挑战次数>
        /// </summary>
        private Hashtable m_BossChallengeNumInfo;
        /// <summary>
        /// 当前boss索引
        /// </summary>
        private int m_BossIndex;
        /// <summary>
        /// 怪物名称
        /// </summary>
        public UILabel[] v_MonsterName;
        public GameObject v_LeftBossBtn;
        public GameObject v_LeftBossRes;
        public GameObject v_RightBossBtn;
        public GameObject v_RightBossRes;
        /// <summary>
        /// 创建时的特效对象
        /// </summary>
        public GameObject CreateModelFx;
        /// <summary>
        /// 物件对象
        /// </summary>
        public LTShowItem TemplateItem;
        /// <summary>
        /// 网络控件
        /// </summary>
        public UIGrid ItemGrid;
        /// <summary>
        /// 图片
        /// </summary>
        public UITexture LobbyTexture;
        /// <summary>
        /// 挑战按钮
        /// </summary>
        public GameObject v_ChallengeBtn;
        /// <summary>
        /// 挑战次数文本
        /// </summary>
        public UILabel[] v_ChallengeTotal;
        /// 扫荡按钮
        /// </summary>
        public GameObject v_WipeOutBtn;
        /// <summary>
        /// 扫荡次数文本
        /// </summary>
        public UILabel[] v_WipeOutTotal;
        /// <summary>
        /// <summary>
        /// 进度条面板
        /// </summary>
        public GameObject v_ProgressPanel;
        /// <summary>
        /// 进条文本
        /// </summary>
        public UILabel[] v_ProgressLabel;
        /// <summary>
        /// 进度条
        /// </summary>
        public UIProgressBar v_ProgressBar;
        /// <summary>
        /// UI控件
        /// </summary>
        public LTLegionFBHudController v_UIController;
        /// <summary>
        /// 排行榜控件
        /// </summary>
        public LTLegionFBRankController v_RankController;

        private UI3DLobby Lobby = null;
        private GM.AssetLoader<GameObject> Loader = null;
        protected string ModelName = null;
        private TweenAlpha mTweenAlpha;
        /// <summary>
        /// 所有的奖励物件
        /// </summary>
        private List<LTShowItem> m_AllReward;
        /// <summary>
        /// 当前BOSS数据
        /// </summary>
        private Hotfix_LT.Data.AllianceFBBoss m_CurrentBoss;
        /// <summary>
        /// 是否可以挑战
        /// </summary>
        private bool m_CanChallenge;
        /// <summary>
        /// 最大宝箱数量
        /// </summary>
        private int MaxBoxCount;
        /// <summary>
        /// 扫荡奖励
        /// </summary>
        private List<LTShowItemData> m_ShowWipeOutList;
        /// <summary>
        /// 设置当前界面
        /// </summary>
        /// 
        private bool isCouldClickWipeBtn = true;
        public void F_SetMenuData(int bossIndex = 0)
        {
            m_BossIndex = bossIndex;
            //
            SetBoss(m_BossIndex, true);
        }

        private void OnClickLeftBtn(GameObject btn)
        {
            m_BossIndex--;
            if (m_BossIndex < 0)
            {
                m_BossIndex = 0;
                return;
            }
            SetBoss(m_BossIndex, false);
        }

        private void OnClickRightBtn(GameObject btn)
        {
            m_BossIndex++;
            int total = Hotfix_LT.Data.AllianceTemplateManager.Instance.mFBBossList.Count;
            if (m_BossIndex >= total)
            {
                m_BossIndex = total - 1;
                return;
            }
            SetBoss(m_BossIndex, false);
        }

        private void OnClickChallenBtn(GameObject go)
        {
            if (IsLoadModel) return;

            if (!m_CanChallenge)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_6819"));
                return;
            }
            //打开布阵界面
            BattleReadyHudController.Open(eBattleType.MainCampaignBattle, OnClickStart);

        }
        /// <summary>
        /// 请求扫荡
        /// </summary>
        /// <param name="go"></param>
        private void OnClickWipeOutBtn(GameObject go)
        {
            if (!m_CanChallenge)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_6819"));
                return;
            }
            //打开提示弹窗
            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, string.Format(EB.Localizer.GetString("ID_ALLIANCE_FB_TIP1"), MaxBoxCount), delegate (int result) {

                if (result == 0)
                {
                    //请求扫荡
                    v_UIController.v_Api.RequestBossWipeOut(m_CurrentBoss.monsterId, delegate (Hashtable data) {

                        if (data != null)
                        {
                            DataLookupsCache.Instance.CacheData(data);
                            //弹出奖励界面
                            ArrayList array = (ArrayList)data["reward"];
                            string id, type;
                            int num;
                            if (m_ShowWipeOutList.Count > 0)
                            {
                                m_ShowWipeOutList.Clear();
                            }
                            LTShowItemData golddata = new LTShowItemData("gold", 0, "res");
                            LTShowItemData alliancegolddata = new LTShowItemData("alliance-gold", 0, "res");
                            for (int i = 0; i < array.Count; i++)
                            {
                                id = EB.Dot.String("data", array[i], "");
                                num = EB.Dot.Integer("quantity", array[i], 0);
                                type = EB.Dot.String("type", array[i], LTShowItemType.TYPE_GAMINVENTORY);

                                if (id.Equals("gold"))
                                {
                                    golddata.count += num;
                                }
                                else if (id.Equals("alliance-gold"))
                                {
                                    alliancegolddata.count += num;
                                }
                                else
                                {
                                    m_ShowWipeOutList.Add(new LTShowItemData(id, num, type));
                                }
                            }
                            m_ShowWipeOutList.Insert(0, golddata);
                            m_ShowWipeOutList.Insert(1, alliancegolddata);                           
                            //赋值奖励链表
                            var ht = Johny.HashtablePool.Claim();
                            ht.Add("reward", m_ShowWipeOutList);
                            GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
                            SetBoss(m_BossIndex, true);
                            FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.alliance_camp_topic, 
                                FusionTelemetry.GamePlayData.alliance_camp_event_id, FusionTelemetry.GamePlayData.alliance_camp_umengId, "reward");
                        }

                    });
                }
            });
        }
        /// <summary>
        /// 点击布阵界面后的战斗按钮回调
        /// </summary>
        private void OnClickStart()
        {
            v_UIController.v_Api.RequestBossChallenge(m_CurrentBoss.monsterId, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
            });
            Hashtable data = new Hashtable();
            data.Add("legionData", LegionModel.GetInstance().legionData);
            data.Add("choiceState", ChoiceState.Activity);
            GlobalMenuManager.Instance.PushCache("LTLegionMainMenu", data);
            GlobalMenuManager.Instance.PushCache("LTLegionFBUI", m_BossIndex);
        }

        private void SetBoss(int index, bool updateInfo)
        {
            index = index % Hotfix_LT.Data.AllianceTemplateManager.Instance.mFBBossList.Count;
            m_CurrentBoss = Hotfix_LT.Data.AllianceTemplateManager.Instance.mFBBossList[index];
            SetBoss(m_CurrentBoss);
            //将左右按钮隐藏及打开设置
            bool leftBtnVisible = index != 0;
            bool rightBtnVisible = index != Hotfix_LT.Data.AllianceTemplateManager.Instance.mFBBossList.Count - 1;
            v_LeftBossBtn.SetActive(leftBtnVisible);
            v_RightBossBtn.SetActive(rightBtnVisible);

            //刷新排行榜
            v_RankController.F_UpdateBossRank(m_CurrentBoss.monsterId);
            int current = LegionModel.GetInstance().legionData.todayExp;
            if (updateInfo)
            {
                //获取当前BOSS的挑战次数
                LegionLogic.GetInstance().OnSendGetCurDonateInfo((Hashtable obj) =>
                {
                    DataLookupsCache.Instance.CacheData(obj);
                    Hashtable hashtable = EB.Dot.Object("alliance", obj, null);
                    hashtable = EB.Dot.Object("todayDonateTimes", hashtable, null);
                    m_BossChallengeNumInfo = EB.Dot.Object("boss", hashtable, null);
                    MaxBoxCount = EB.Dot.Integer("bossBoxCount." + m_CurrentBoss.monsterId.ToString(), hashtable, -1);
                    //设置扫荡相关
                    SetBossWipeInfo(MaxBoxCount > -1);
                    int challenge = EB.Dot.Integer(m_CurrentBoss.monsterId.ToString(), m_BossChallengeNumInfo, 0);
                    if (challenge >= 0)
                    {
                            //设置当前的BOSS挑战次数
                            SetChanllengeBtnTips(m_CurrentBoss.challenge - challenge, m_CurrentBoss.challenge);                         
                    }
                    LegionLogic.GetInstance().IsOpenLegionFB();
                    SetBossRedPoint(v_LeftBossRes, index - 1, current, leftBtnVisible);
                    SetBossRedPoint(v_RightBossRes, index + 1, current, rightBtnVisible);
                });
            }
            else
            {
                int challenge = EB.Dot.Integer(m_CurrentBoss.monsterId.ToString(), m_BossChallengeNumInfo, 0);
                //设置扫荡相关
                SetBossWipeInfo(DataLookupsCache.Instance.SearchDataByID<int>("alliance.todayDonateTimes.bossBoxCount." + m_CurrentBoss.monsterId.ToString(), out MaxBoxCount));
                if (challenge >= 0)
                {
                    //设置当前的BOSS挑战次数
                    SetChanllengeBtnTips(m_CurrentBoss.challenge - challenge, m_CurrentBoss.challenge);
                }
                //判断是否需要打开相应的红点
                SetBossRedPoint(v_LeftBossRes, index - 1, current, leftBtnVisible);
                SetBossRedPoint(v_RightBossRes, index + 1, current, rightBtnVisible);
            }

            //处理进度条的情况
            int total = m_CurrentBoss.donate;
            v_ChallengeBtn.SetActive(!(current < total));
            v_ProgressPanel.SetActive(current < total);
            SetProgressPanel(current, total);
        }

        /// <summary>
        /// 设置扫荡相关
        /// </summary>
        private void SetBossWipeInfo(bool isRewardCouldGot)
        {
            bool isCanWipeOut = isRewardCouldGot&&(Data.VIPTemplateManager.Instance.GetTotalNum(Data.VIPPrivilegeKey.CanBlitzAlliCampaign)>0);
            int current = LegionModel.GetInstance().legionData.todayExp;
            int total = m_CurrentBoss.donate;
            if (isCanWipeOut)
            {
                v_ChallengeBtn.transform.localPosition = new Vector3(-548, -600, 0);
            }
            else
            {
                v_ChallengeBtn.transform.localPosition = new Vector3(-265, -600, 0);
            }
            v_WipeOutBtn.SetActive(!(current < total) && isCanWipeOut);
        }

        /// <summary>
        /// 设置相应BOSS的红点
        /// </summary>
        /// <param name="red">红点</param>
        /// <param name="index">当前的BOSS索引</param>
        /// <param name="currentProgress">当前的开放的进度条</param>
        /// <param name="need">是否需要判断红点</param>
        private void SetBossRedPoint(GameObject red, int index, int currentProgress, bool need)
        {
            if (need)
            {
                Hotfix_LT.Data.AllianceFBBoss currentBoss = Hotfix_LT.Data.AllianceTemplateManager.Instance.mFBBossList[index];
                //获取当前BOSS的挑战次数
                int challenge = EB.Dot.Integer(currentBoss.monsterId.ToString(), m_BossChallengeNumInfo, 0);
                //
                red.SetActive(currentBoss.challenge > challenge && currentProgress >= currentBoss.donate);
            }
        }

        /// <summary>
        /// 设置挑战按钮的文本提示
        /// </summary>
        /// <param name="current">当前</param>
        /// <param name="total">最值</param>
        private void SetChanllengeBtnTips(int current, int total)
        {
            m_CanChallenge = current > 0;
            string str = EB.Localizer.Format("ID_codefont_in_LTBountyTaskOverController_477", "00ff00", current, total);
            //
            LTUIUtil.SetText(v_ChallengeTotal, str);
            //新增扫荡文本
            LTUIUtil.SetText(v_WipeOutTotal, str);
        }

        /// <summary>
        /// 设置进度条
        /// </summary>
        /// <param name="current">当前</param>
        /// <param name="total">最值</param>
        /// <returns></returns>
        private void SetProgressPanel(int current, int total)
        {
            v_ProgressBar.value = current / (float)total;
            LTUIUtil.SetText(v_ProgressLabel, current + "/" + total);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data">数据</param>
        private void SetBoss(Hotfix_LT.Data.AllianceFBBoss data)
        {
            //
            Hotfix_LT.Data.MonsterInfoTemplate monsterTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(data.monsterId);
            if (monsterTpl == null)
            {
                EB.Debug.LogError("LTLegionFBHudController InitBoss, monsterTpl is Error monsterID = {0}", data.monsterId);
                return;
            }

            Hotfix_LT.Data.HeroInfoTemplate info = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(monsterTpl.character_id);
            if (info == null)
            {
                EB.Debug.LogError("LTLegionFBHudController InitBoss, info is Error monsterTpl.character_id = {0}", monsterTpl.character_id);
                return;
            }

            if(mDMono.gameObject.activeInHierarchy)StartCoroutine(CreateBossModel(info.model_name));
            LTUIUtil.SetText(v_MonsterName, info.name);
            //
            SetReward(data.Rewards);
        }

        /// <summary>
        /// 设置奖励
        /// </summary>
        /// <param name="rewards">奖励列表</param>
        private void SetReward(List<LTShowItemData> rewards)
        {
            for (int i = 0; i < m_AllReward.Count; i++)
            {
                m_AllReward[i].mDMono.gameObject.SetActive(false);
            }
            //
            for (int i = 0; i < rewards.Count; i++)
            {
                LTShowItem si = GetShowItem();
                si.LTItemData = new LTShowItemData(rewards[i].id, rewards[i].count, rewards[i].type, true);
            }
            ItemGrid.Reposition();
        }

        private LTShowItem GetShowItem()
        {
            LTShowItem showItem = m_AllReward.Find(p => !p.mDMono.gameObject.activeSelf);
            if (showItem == null)
            {
                showItem = GameUtils.InstantiateEx(TemplateItem.mDMono, ItemGrid.transform).transform.GetMonoILRComponent<LTShowItem>();
                m_AllReward.Add(showItem);
            }
            showItem.mDMono.gameObject.SetActive(true);
            return showItem;
        }

        public bool IsLoadModel = false;

        private IEnumerator CreateBossModel(string modelName)
        {
            IsLoadModel = true;
            CreateModelFx.CustomSetActive(false);
            yield return new WaitForEndOfFrame();
            yield return null;
            CreateModelFx.CustomSetActive(true);
            if (mTweenAlpha == null)
            {
                mTweenAlpha = LobbyTexture.GetComponent<TweenAlpha>();
            }
            mTweenAlpha.ResetToBeginning();
            mTweenAlpha.PlayForward();
            if (string.IsNullOrEmpty(modelName) || ModelName == modelName)
            {
                IsLoadModel = false;
                yield break;
            }
            ModelName = modelName;
            if (Lobby == null && Loader == null)
            {
                Loader = new GM.AssetLoader<GameObject>("UI3DLobby", mDMono.gameObject);

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
            IsLoadModel = false;
        }

        protected void DestroyModel()
        {
            if (Lobby != null)
            {
                Object.Destroy(Lobby.mDMono.gameObject);
            }
            if (Loader != null)
            {
                EB.Assets.UnloadAssetByName("UI3DLobby", false);
            }
            Lobby = null;
            Loader = null;
            ModelName = null;
        }
    }
}

