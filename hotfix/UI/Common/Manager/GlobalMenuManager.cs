using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using GM;
using System.Text;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// this is global MenuCreatorManager
    /// </summary>
    public class CacheFrameEntry
    {
        public string viewname;
        public object param;
    }

    /// <summary>
    /// 全局界面管理器
    /// </summary>
    public class GlobalMenuManager : MenuCreatorManager
    {
        private static List<CacheFrameEntry> s_cacheFrames = new List<CacheFrameEntry>();

        private List<string> mIgnoreController = new List<string>() { "LTMainMenu", "LTMainInstanceHud", "LTChallengeInstanceHud" };

        private bool mIsShowMainLand = true;
        public bool IsShowMainLand
        {
            get { return mIsShowMainLand; }
        }


        #region 显示窗口队列

        //当前是全屏的界面则其他在他前面被打开的界面都可以隐藏了
        private void uiControllerShowManager()
        {
            if (mOpenController.Count > 0)
            {
                for (int i = 0; i < mOpenController.Count; i++)
                {
                    if (i == mOpenController.Count - 1)
                    {
                        SetPanelEnabled(mOpenController[i], true);
                    }
                    else
                    {
                        UIController currentOpenWindow = mOpenController[mOpenController.Count - 1];
                        //因为当前打开的如果是全屏界面则不能显示
                        SetPanelEnabled(mOpenController[i], !currentOpenWindow.IsFullscreen());
                    }
                }
            }
        }

		private void SetRectEnabled(UIController ui, bool isShow)
		{
			UIRect[] mUIRects = ui.GetComponentsInChildren<UIRect>(true);
			for (int i = 0; i < mUIRects.Length; i++)
			{
				if (mUIRects[i].enabled != isShow && !mUIRects[i].name.Equals("Foreground") && !mUIRects[i].name.Equals("LobbyTexture"))//排除特殊的滚动条名称UI
					mUIRects[i].enabled = isShow;
			}
		}

        private void SetPanelEnabled(UIController ui, bool isShow)
        {
			if(!isShow)
			{
				TimerManager.instance.AddFramer(ui.WaitFrameForDisabled, 1, delegate {
					if (ui) SetRectEnabled(ui, isShow);
				});
			}
			else
				SetRectEnabled(ui, isShow);			
						
            Camera[] mCameras = ui.GetComponentsInChildren<Camera>(true);
            for (int i = 0; i < mCameras.Length; i++)
            {
                if (mCameras[i].enabled != isShow)
                    mCameras[i].enabled = isShow;
            }
        }

        private Camera mMainCamera = null;
        private Camera getMainCamera()
        {
            if (mMainCamera == null)
            {
                mMainCamera = Camera.main;
            }
            else
            {

            }
            return mMainCamera;
        }

        public static bool GetCurrentWinFromILR()
        {
            if (Instance != null && Instance.GetCurrentWin() != null)
            {
                return Instance.GetCurrentWin().name.IndexOf("LTCombatReadyUI") != -1;
            }

            return true;
        }

        public UIController GetCurrentWin()
        {
            if (mOpenController.Count > 0)
            {
                return mOpenController[mOpenController.Count - 1];
            }
            return null;
        }

        //打开得窗口得队列
        private List<UIController> mOpenController = new List<UIController>();

        public static void AddOpenControllerFromILR(UIController ui)
        {
            if (Instance != null)
            { 
                Instance.AddOpenController(ui);
            }
        }

        /// <summary>
        /// 加入一个窗口
        /// </summary>
        /// <param name="ui"></param>
        public void AddOpenController(UIController ui)
        {
            //如果名字不在忽略列表并且是全屏得才加入列表
            if (!mOpenController.Contains(ui) &&
                !mIgnoreController.Contains(ui.name.Replace("(Clone)", "")) &&
                ui.IsFullscreen())
            {
                mOpenController.Add(ui);
                StartCoroutine(ShowMainLand(false));
            }
            uiControllerShowManager();
        }

        public static void RemoveOpenControllerFromILR(UIController ui)
        {
            if (Instance != null)
            {
                Instance.RemoveOpenController(ui);
            }
        }

        /// <summary>
        /// 移除一个窗口
        /// </summary>
        /// <param name="ui"></param>
        public void RemoveOpenController(UIController ui)
        {
            if (mOpenController.Contains(ui))
            {
                mOpenController.Remove(ui);
                if (0 == mOpenController.Count)
                {
                    ShowMainLandHandler(true);
                }
            }
            uiControllerShowManager();
        }

        private IEnumerator ShowMainLand(bool isShow)
        {
            yield return new WaitForSeconds(0.3f);

            //如果是要求隐藏但又列表又没数据了则不执行隐藏了
            if (!isShow && mOpenController.Count == 0)
            {
                yield break;
            }
            ShowMainLandHandler(isShow);
        }

        public static void ShowMainLandHandlerFromILR(bool isShow)
        {
            if (Instance != null)
            {
                Instance.ShowMainLandHandler(isShow);
            }
        }

        public void ShowMainLandHandler(bool isShow)
        {
            mIsShowMainLand = isShow;
            var camera = getMainCamera();
            if (camera)
            {
                if (camera.enabled != isShow)
                {
                    camera.enabled = isShow;
                }
            }
            //
            //EB.Debug.Log("主城相机的设置,camera:" + camera + ",isShow:" + isShow);

            if (LTMainMenuHudController.Instance != null)
            {
                LTMainMenuHudController.Instance.ShowGO(isShow);
            }
            //ShowPoeple(isShow);

            ShowMainLandLight(isShow);
        }

        public void ShowMainLandLight(bool isShow)
        {
            if (!SceneLogicManager.isMainlands() ||
                MainLandLogic.GetInstance() == null ||
                MainLandLogic.GetInstance().ThemeLoadManager == null)
            {
                return;
            }

            SceneRootEntry sceneRoot = MainLandLogic.GetInstance().ThemeLoadManager.GetSceneRoot();

            if (sceneRoot != null && sceneRoot.m_SceneRoot != null)
            {
                var mainLight = sceneRoot.m_SceneRoot.GetComponentInChildren<Light>();

                if (mainLight != null)
                {
                    mainLight.enabled = isShow;
                }
            }
        }

        public void ShowPoeple(bool isShow)
        {
            if (!SceneLogicManager.isMainlands()) return;
            if (MainLandLogic.GetInstance()==null) return;
            if (!MainLandLogic.GetInstance().ThemeLoadManager) return;
            //var players = getPlayer(isShow);
            SceneRootEntry sceneRoot = MainLandLogic.GetInstance().ThemeLoadManager.GetSceneRoot();
            if (sceneRoot != null && sceneRoot.m_SceneRoot != null)
            {
                Transform playersList = sceneRoot.m_SceneRoot.transform.Find("PlayerList");
                if (playersList != null)
                {
                    //运镖的时候不能隐藏
                    if (!AllianceUtil.IsInTransferDart)
                    {
                        playersList.gameObject.CustomSetActive(isShow);
                    }
                    else
                    {
                        playersList.gameObject.CustomSetActive(true);
                    }
                }

                Transform enemysList = sceneRoot.m_SceneRoot.transform.Find("ObjectManager");
                if (enemysList != null)
                {
                    enemysList.gameObject.CustomSetActive(isShow);
                }
            }
        }

        #endregion

        #region Instance
        private static GlobalMenuManager instance;
        public static GlobalMenuManager Instance
        {
            get
            {
                if (instance == null)
                {
                    EB.Debug.LogWarning("GlobalMenuManager is not awake yet!");
                }
                return instance;
            }
        }
        #endregion

        public override void Awake()
        {
            base.Awake();
            InitUIMaxCount();
            instance = this;
        }

        public override void OnDestroy()
        {
            instance = null;
            s_cacheFrames.Clear();

            base.OnDestroy();
        }


        #region About 当前格子地图数据刷新
        public static void CurGridMap_OnFloorClicked(LTInstanceNode node, Transform tf){
            if(LTGeneralInstanceHudController.Instance != null)
            {
                LTGeneralInstanceHudController.Instance.OnFloorClickFunc(node, tf);
            }
        }

        /// <summary>
        /// 当前格子地图_角色数据刷新
        /// </summary>
        public static void CurGridMap_MajorDataUpdateFunc()
        {
            if(LTGeneralInstanceHudController.Instance != null)
            {
                LTGeneralInstanceHudController.Instance.MapCtrl.MajorDataUpdateFunc();
            }
        }
        #endregion


        #region About Cache
        /// <summary>
        /// 将viewname的预设推入Cache
        /// </summary>
        /// <param name="viewname">预设名</param>
        /// <param name="param">参数</param>
        public void PushCache(string viewname, object param = null)
        {
            if (s_cacheFrames.Find(m => m.viewname == viewname) == null)
            {
                s_cacheFrames.Add(new CacheFrameEntry() { viewname = viewname, param = param });
            }
        }
        /// <summary>
        /// 将viewname的预设从Cache中移除
        /// </summary>
        /// <param name="viewname">预设名</param>
        public void RemoveCache(string viewname)
        {
            s_cacheFrames.RemoveAll(m => m.viewname == viewname);
        }

        public static void ClearCacheFromILR()
        {
            if (Instance != null)
            {
                Instance.ClearCache();
            }
        }

        /// <summary>
        /// 清除Cache
        /// </summary>
        public void ClearCache()
        {
            s_cacheFrames.Clear();
        }

        /// <summary>
        /// 生成Cache中包含的预设，清除Cache
        /// </summary>
        public static void PopCaches()
        {
            var tmpCacheFrames = s_cacheFrames.ToArray();
            s_cacheFrames.Clear();
            if (instance != null)
            {
                for (int i = 0; i < tmpCacheFrames.Length; i++)
                {
                    //必须要先清缓存，否则同样的新缓存加不进来
                    var cacheFrame = tmpCacheFrames[i];
                    //s_cacheFrames.Remove(cacheFrame);
                    instance.Open(cacheFrame.viewname, cacheFrame.param, true);
                }
            }
            else
            {
                for (int i = 0; i < tmpCacheFrames.Length; i++)
                {
                    //必须要先清缓存，否则同样的新缓存加不进来
                    var cacheFrame = tmpCacheFrames[i];
                    //s_cacheFrames.Remove(cacheFrame);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.ShowMenu, cacheFrame.viewname, cacheFrame.param, true);
                }
            }
        }
        #endregion

        public override void PreloadMenu(string viewname)
        {
            if (m_creatorDict.ContainsKey(viewname))
            {
                // for dynamic menus
                m_creatorDict[viewname].PreloadMenu();
            }
            else if (!m_menuDict.ContainsKey(viewname))
            {
                // for independent menus
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PreloadMenu, viewname);
            }
        }

        public static void CloseMenuFromILR(string viewname)
        {
            if (Instance != null)
            {
                Instance.CloseMenu(viewname);
            }
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="viewname"></param>
        public override void CloseMenu(string viewname)
        {
            if (m_creatorDict.ContainsKey(viewname))
            {
                // for dynamic menus
                m_creatorDict[viewname].CloseMenu();
            }
            else if (m_menuDict.ContainsKey(viewname))
            {
                // for static menus
                UIController menu = m_menuDict[mDMono.name];
                menu.Close();
            }
            else
            {
                // for independent menus
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.CloseMenu, viewname);
            }
        }

        private List<UIController> mCurrentViews = new List<UIController>();

        public static void SetUIEnabledFromILR(string viewname, bool isShow)
        {
            if (Instance != null)
            {
                Instance.SetUIEnabled(viewname, isShow);
            }
        }

        /// <summary>
        /// UIController中的UIEnabled、UIDisabled方法相关，做UI3DLobbys相关的设置
        /// </summary>
        /// <param name="viewname"></param>
        /// <param name="isShow"></param>
        public void SetUIEnabled(string viewname, bool isShow)
        {
            if (!string.IsNullOrEmpty(viewname))
            {
                if (m_creatorDict.ContainsKey(viewname))
                {
                    var newMenu = m_creatorDict[viewname].Menu;
                    if (newMenu && !newMenu.IsFullscreen())
                    {
                        return;
                    }

                    UIController currentView = null;
                    if (mCurrentViews.Count > 0)
                    {
                        currentView = mCurrentViews[mCurrentViews.Count - 1];
                    }
                    if (isShow)
                    {
                        mCurrentViews.Add(newMenu);
                    }
                    else
                    {
                        if (currentView != null)
                        {
                            mCurrentViews.Remove(currentView);
                        }
                        if (mCurrentViews.Count > 0)
                        {
                            currentView = mCurrentViews[mCurrentViews.Count - 1];
                        }
                    }

                    if (newMenu != null)//新界面做显示
                    {
                        if (isShow)
                        {
                            SetUI3DLobbysEnabled(newMenu, true);
                        }
                        else
                        {
                            SetUI3DLobbysEnabled(newMenu, false);
                        }
                    }

                    if (currentView != null)//而旧界面做隐藏
                    {
                        if (isShow)
                        {
                            SetUI3DLobbysEnabled(currentView, false);
                        }
                        else
                        {
                            SetUI3DLobbysEnabled(currentView, true);
                        }
                    }
                }
            }
        }

        private void SetUI3DLobbysEnabled(UIController uIController, bool isShow)
        {
            UI3DLobby[] ui3DLobbys = uIController.transform.GetMonoILRComponentsInChildren<UI3DLobby>("Hotfix_LT.UI.UI3DLobby", showErrorTips:false);

            if (ui3DLobbys == null)
            {
                return;
            }

            for (int i = 0; i < ui3DLobbys.Length; i++)
            {
                ui3DLobbys[i].mDMono.gameObject.CustomSetActive(isShow);
            }
        }

        public T GetMenu<T>(string viewname) where T : UIController
        {
            UIController menu = GetMenu(viewname);
            return menu == null ? null : (T)menu;
        }

        public static void OpenFromILR(string viewname, object param = null, bool queue = false)
        {
            if (Instance != null)
            {
                Instance.Open(viewname, param, queue);
            }
        }


        #region 打开界面
        /// <summary>
        /// shortcut for OpenMenu，打开界面
        /// </summary>
        /// <param name="viewname"></param>
        /// <param name="param"></param>
        /// <param name="queue"></param>
        public void Open(string viewname, object param = null, bool queue = false)
        {
            OpenMenu(viewname, param, queue);
        }

        /// <summary>
        /// 打开界面Impl
        /// </summary>
        /// <param name="viewname">相应界面名称</param>
        /// <param name="param">参数</param>
        /// <param name="queue">是否队列</param>
        protected override void OpenMenu(string viewname, object param, bool queue)
        {
            if (m_creatorDict.ContainsKey(viewname))
            {
                // for dynamic menus
                m_creatorDict[viewname].CreateMenu(param, queue);
            }
            else if (m_menuDict.ContainsKey(viewname))
            {
                // for static menus
                UIController menu = m_menuDict[mDMono.name];
                menu.SetMenuData(param);
                menu.PlayTween();
                if (queue)
                {
                    menu.Queue();
                }
                else
                {
                    menu.Open();
                }
            }
            else
            {
                // for independent menus_通知打开界面事件
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.ShowMenu, viewname, param, queue);
            }
        }
        #endregion

        /// <summary>
        /// 获取m_creatorDict预设字典的数量
        /// </summary>
        /// <returns></returns>
        public int GetCreaterCount()
        {
            int count = 0;
            foreach (var obj in m_creatorDict.Values)
            {
                if (obj.MenuObject != null) count++;
            }
            return count;
        }

        /// <summary>
        /// Cache中是否包含了viewname的预设
        /// </summary>
        /// <param name="viewname">参数</param>
        /// <returns></returns>
        public bool IsContain(string viewname)
        {
            return s_cacheFrames.Find(m => m.viewname == viewname) != null;
        }


        //static Dictionary<string, List<UITexture>> LoadingList = new Dictionary<string, List<UITexture>>();
        static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

        static Queue<KeyValuePair<string, UITexture>> LoadingList = new Queue<KeyValuePair<string, UITexture>>();
        Coroutine RemoteLoadingCoroutine = null;
        public void LoadRemoteUITexture(string uri, UITexture target)
        {
            Texture2D t;
            if (textureCache.TryGetValue(uri, out t))
            {
                target.SetTexture(t);
                return;
            }

            LoadingList.Enqueue(new KeyValuePair<string, UITexture>(uri, target));
            if (RemoteLoadingCoroutine == null) RemoteLoadingCoroutine = StartCoroutine(_InnerLoadRemoteImage());
        }

        IEnumerator _InnerLoadRemoteImage()
        {
            while (LoadingList.Count > 0)
            {
                var data = LoadingList.Dequeue();

                Texture2D t;
                if (textureCache.TryGetValue(data.Key, out t))
                {
                    if (data.Value != null) data.Value.SetTexture(t);
                    continue;
                }

                int retries = 0;

                while (retries < 3)
                {
                    HTTP.Request req = new HTTP.Request("GET", data.Key);
                    yield return req.Send();
                    if (req.exception != null)
                    {
                        retries++;
                        yield return new WaitForSecondsRealtime(0.5f);
                        continue;
                    }

                    Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    texture.LoadImage(req.response.Bytes);
                    textureCache.Add(data.Key, texture);
                    if (data.Value != null) data.Value.SetTexture(texture);
                    break;
                }

                if (retries >= 3)
                {
                    EB.Debug.LogError("Cannot load remote image: {0}", data.Key);
                }
            }
            RemoteLoadingCoroutine = null;
        }

        public new void StopAllCoroutines()
        {
            RemoteLoadingCoroutine = null;
            base.StopAllCoroutines();
        }

        public static void ComebackToMianMenuFromILR()
        {
            if (Instance != null)
            {
                Instance.ComebackToMianMenu();
            }
        }

        /// <summary>
        /// 关闭所有界面回到主城主界面
        /// </summary>
        public void ComebackToMianMenu()
        {
            s_cacheFrames.Clear();
            foreach (var obj in m_creatorDict.Values)
            {
                if (obj.MenuObject != null)
                {
                    var main = obj.MenuObject.GetUIControllerILRComponent<LTMainInstanceHudController>();
                    if (main != null)
                    {     
                        main.OnCancelButtonClick();
                    }
                    else obj.CloseMenu();
                }
            }
            UITooltipManager.Instance.HideTooltipForPress();
        }

        #region 界面管理（用于维持一个打开关闭的一个队列，不常用的会优先destroy）

        /// <summary>
        /// ui列表
        /// </summary>
        private List<UIController> mUIControllerList = new List<UIController>();
        /// <summary>
        /// 队列上限
        /// </summary>
        private int mUIMaxCount = 10;

        public static void AddUIControllerFromILR(UIController uiController)
        {
            if (Instance != null)
            {
                Instance.AddUIController(uiController);
            }
        }

        /// <summary>
        /// 添加一个ui到列表中
        /// </summary>
        /// <param name="uiController"></param>
        public void AddUIController(UIController uiController)
        {
            if (mUIControllerList.Contains(uiController))
            {
                //如有该ui在列表里面，将其移动到列表尾端
                mUIControllerList.Remove(uiController);
                mUIControllerList.Add(uiController);
            }
            else
            {
                mUIControllerList.Add(uiController);
                if (mUIControllerList.Count > mUIMaxCount)
                {
                    for (int i = 0; i < mUIControllerList.Count; i++)
                    {
                        //在栈内的不能移除
                        if (!UIStack.Instance.IsStacked(mUIControllerList[i]))
                        {
                            UIController uiC = mUIControllerList[i];
                            EB.Debug.LogUI("-----------移除的ui：{0}", uiC.MCurrentViewName);
                            mUIControllerList.RemoveAt(i);
                            uiC.DestroyControllerForm();
                            break;
                        }
                    }
                }
            }

            EB.Debug.LogUI("-------------列表剩余：{0}, 添加的ui：{1}", mUIControllerList.Count, uiController.MCurrentViewName);
        }

        /// <summary>
        /// 初始化ui列表上限数量
        /// </summary>
        private void InitUIMaxCount()
        {
            // Debug.LogWarning("SystemInfo.systemMemorySize:"+SystemInfo.systemMemorySize);
#if UNITY_EDITOR
             // mUIMaxCount = 0;
#else
           if (SystemInfo.systemMemorySize <= 1024 ||IsLowIosDevice())
            {
                mUIMaxCount = 0;
            }
#endif
        }
        
        public static bool IsLowIosDevice()
        {
          //0 High 1Mid 2Low
            if (ILRDefine.UNITY_IPHONE && QualitySettings.GetQualityLevel()>1)
            {
                return true;
            }
            return false;
        }

        public static bool IsCouldFindControllerFromILR(UIController uiController)
        {
            return Instance != null && Instance.IsCouldFindController(uiController);
        }

        /// <summary>
        /// controller是否已在缓存列表中
        /// </summary>
        /// <param name="uiController"></param>
        /// <returns></returns>
        public bool IsCouldFindController(UIController uiController)
        {
            bool flag = false;
            if (mUIControllerList.Contains(uiController))
            {
                flag = true;
            }

            return flag;
        }

        #endregion

        #region ui资源释放策略
        private const float mTotalMemory = 100;
        private float mCurUIMemoryValue;
        private List<string> mCurUIInMemoryList = new List<string>();

        private readonly Dictionary<string, float> mUIMemoryDic = new Dictionary<string, float>()
    {
        {"LTNewInventoryView_Stretch_R", 2.5f },//背包
        {"LTFriendHud", 2.8f },//好友
        {"LTLegionMainMenu", 14.3f },//军团主界面
        {"LTNationHudUI", 8f },//国家主界面
        {"LTNationTerritoryUI", 2.5f },//国家二级界面
        {"LTNationBattleEntryUI", 2.5f },//国家城堡界面
        {"LTRankListHud", 2.9f },//排行榜
        {"LTPartnerHandbookHud", 6.7f },//图鉴
        {"LTGameSettingHud", 0f },//设置
        {"LTChargeStore", 8.3f },//充值商城
        {"LTSigninHud", 7.2f },//签到
        {"LTWelfareHud", 16.7f },//福利
        {"LTStoreUI", 3.6f },//商店
        {"LTDrawCardTypeHud", 12.4f },//抽奖主界面
        {"LTLookUpPartnerUI", 0f },//抽奖伙伴预览界面
        {"LTGetItemUI", 1.8f },//抽奖获得物品界面
        {"LTMailUI", 2.9f },//邮件
        {"LTDailyHud", 2.5f },//日常活动
        {"LTChatHud", 0f },//聊天
        {"LTTaskHud", 2.6f },//任务
        {"LTPartnerHud", 3.1f },//伙伴界面
        {"LTPartnerEquipmentHud", 1.6f },//伙伴装备界面
        {"LTPartnerSkillBreak", 0f },//伙伴技能突破界面
        {"LTInstanceMapHud", 5f },//世界地图
        {"LTCombatReadyUI", 2.5f },//布阵界面
        {"LTEquipmentInfoUI", 0f },//装备信息界面
        {"LTResourceShopUI", 4.1f },//资源购买界面
        {"LTResourceInstanceHud", 8.2f },//资源副本界面
        {"LTChallengeInstanceSelectHud", 2.5f },//挑战副本选关界面
        {"LTChallengeInstanceThemeHud", 3.3f },//挑战副本随卡界面
        {"LTInstanceSmallMapView", 0f },//副本小地图
        {"LTChallengeInstanceBag", 0f },//挑战副本背包
        {"LTShowRewardView", 0.3f },//恭喜获得界面（通用界面）
        {"LTRewardShowUI", 0f },//主线副本奖励显示界面
        {"LadderUI", 2.6f },//天梯界面
        {"LTAllianceEscortHud", 4.1f },//军团护送
        {"LTSpeedSnatchAwardInfoHudUI", 0f },//夺宝奇兵
        {"LTUltimateTrialHud", 0f },//极限试炼
        {"LTHeroBattleMatch", 4.4f },//英雄交锋
        {"LTRuleUIView", 0f },//规则界面
        {"LTBountyTaskHudUI", 5.3f },//悬赏界面
        {"ArenaHudUI", 2.1f },//角斗场
        {"LTWorldBossHud", 0f },//世界boss
        {"LTChallengeInstanceHud", 2.1f },//挑战副本主界面
        {"LTMainInstanceHud",2.0f },//主线副本主界面
        {"LTMainInstanceLampView", 0f },//主线副本扫荡界面
        {"LTStoreBuy", 0f },//商店购买界面
    };
        #endregion

        #region ui加载用时统计

        private float mStartTime;
        private float mEndTime;
        private float mEndPrefabTime;
        private string mCurPrefabName;
        private int writeTime = 20;
        private Dictionary<string, float> mFirstOpenUITimeDic = new Dictionary<string, float>();
        private Dictionary<string, List<float>> mOpenUIAverageTimeDic = new Dictionary<string, List<float>>();
        private Dictionary<string, float> mFirstOpenUIPrefabTimeDic = new Dictionary<string, float>();
        private Dictionary<string, List<float>> mOpenUIPrefabAverageTimeDic = new Dictionary<string, List<float>>();

        public void OpenUIStart(string prefabName)
        {
            mStartTime = (float)System.Math.Round(Time.realtimeSinceStartup * 1000, 2);
            mCurPrefabName = prefabName;
            EB.Debug.LogUIStatistics("<color=#d69d85>界面加载开始GlobalMenuManager.OpenUIStart, PrefabName : {0}, Time : {1}</color>", prefabName, mStartTime);
        }

        public void OpenUIEnd(string prefabName)
        {
            if (!mCurPrefabName.Equals(prefabName))
            {
                EB.Debug.LogUIStatistics("<color=#d69d85>界面加载结束GlobalMenuManager.OpenUIEnd出错，两次统计的预制不一样, PrefabName : {0}</color>", prefabName);
                return;
            }

            mEndTime = (float)System.Math.Round(Time.realtimeSinceStartup * 1000, 2);
            float useTime = (float)System.Math.Round(mEndTime - mStartTime, 2);
            EB.Debug.LogUIStatistics("<color=#d69d85>界面加载结束GlobalMenuManager.OpenUIEnd, PrefabName : {0}, Time : {1}, 加载用时 : {2}</color>", prefabName, mEndTime, useTime);

            if (!mFirstOpenUITimeDic.ContainsKey(prefabName))
            {
                EB.Debug.LogUIStatistics("<color=#d69d85>GlobalMenuManager.OpenUIEnd 第一次打开界面耗时, PrefabName : {0}, 加载用时 : {1}</color>", prefabName, useTime);
                mFirstOpenUITimeDic.Add(prefabName, useTime);
                return;
            }

            if (!mOpenUIAverageTimeDic.ContainsKey(prefabName))
            {
                mOpenUIAverageTimeDic.Add(prefabName, new List<float>());
            }

            EB.Debug.LogUIStatistics("<color=#d69d85>GlobalMenuManager.OpenUIEnd 非第一次打开界面耗时, PrefabName : {0}, 加载用时 : {1}</color>", prefabName, useTime);
            mOpenUIAverageTimeDic[prefabName].Add(useTime);

            EB.Debug.LogUIStatistics("<color=#d69d85>GlobalMenuManager.OpenUIEnd , 当前计数 : {0}</color>", mOpenUIAverageTimeDic[prefabName].Count);
            if (mOpenUIAverageTimeDic[prefabName].Count >= writeTime)
            {
                EB.Debug.LogUIStatistics("<color=#d69d85>GlobalMenuManager.OpenUIEnd 开始写入进本地Log中, PrefabName : {0}</color>", prefabName);
                WriteLocalLog(prefabName);
                mOpenUIAverageTimeDic[prefabName].Clear();
            }
        }

        public void OpenUIPrefabEnd(string prefabName)
        {
            if (!mCurPrefabName.Equals(prefabName))
            {
                EB.Debug.LogUIStatistics("<color=#d69d85>界面加载结束GlobalMenuManager.OpenUIPrefabEnd出错，两次统计的预制不一样, PrefabName : {0}</color>", prefabName);
                return;
            }

            mEndPrefabTime = (float)System.Math.Round(Time.realtimeSinceStartup * 1000, 2);
            float useTime = (float)System.Math.Round(mEndPrefabTime - mStartTime, 2);
            EB.Debug.LogUIStatistics("<color=#d69d85>界面加载结束GlobalMenuManager.OpenUIPrefabEnd, PrefabName : {0}, Time : {1}, 加载用时 : {2}</color>", prefabName, mEndPrefabTime, useTime);

            if (!mFirstOpenUIPrefabTimeDic.ContainsKey(prefabName))
            {
                EB.Debug.LogUIStatistics("<color=#d69d85>GlobalMenuManager.OpenUIPrefabEnd 第一次打开界面Prefab加载耗时, PrefabName : {0}, 加载用时 : {1}</color>", prefabName, useTime);
                mFirstOpenUIPrefabTimeDic.Add(prefabName, useTime);
                return;
            }

            if (!mOpenUIPrefabAverageTimeDic.ContainsKey(prefabName))
            {
                mOpenUIPrefabAverageTimeDic.Add(prefabName, new List<float>());
            }

            EB.Debug.LogUIStatistics("<color=#d69d85>GlobalMenuManager.OpenUIPrefabEnd 非第一次打开界面Prefab加载耗时, PrefabName : {0}, 加载用时 : {1}</color>", prefabName, useTime);
            mOpenUIPrefabAverageTimeDic[prefabName].Add(useTime);
        }

        private void WriteLocalLog(string prefabName)
        {
            string path="";
            if (ILRDefine.UNITY_EDITOR)
            {
                 path = Application.dataPath + "UIStatisticsLog.txt";
            }
            else
            {
                 path = Application.persistentDataPath  + "/UIStatisticsLog.txt";
            }
            string str = "预制体的名字 : " + prefabName + "\r\n";

            //总加载数据
            str += "\r\n    第一次加载用时 : " + mFirstOpenUITimeDic[prefabName];
            float totalUseTime = 0;
            str += "\r\n    所有的用时 : ";
            for (int i = 0; i < mOpenUIAverageTimeDic[prefabName].Count; i++)
            {
                str += "[" + i + " : " + mOpenUIAverageTimeDic[prefabName][i] + "];";
                totalUseTime += mOpenUIAverageTimeDic[prefabName][i];
            }
            str += "\r\n    总耗时 : " + totalUseTime;
            str += "\r\n    总次数 : " + mOpenUIAverageTimeDic[prefabName].Count;
            str += "\r\n    平均耗时 : " + totalUseTime / mOpenUIAverageTimeDic[prefabName].Count + "\r\n";

            //加载预制体数据
            str += "\r\n    第一次加载Prefab用时 : " + mFirstOpenUIPrefabTimeDic[prefabName];
            float totalPrefabUseTime = 0;
            str += "\r\n    加载所有的Prefab用时 : ";
            for (int i = 0; i < mOpenUIPrefabAverageTimeDic[prefabName].Count; i++)
            {
                str += "[" + i + " : " + mOpenUIPrefabAverageTimeDic[prefabName][i] + "];";
                totalPrefabUseTime += mOpenUIPrefabAverageTimeDic[prefabName][i];
            }
            str += "\r\n    加载Prefab总耗时 : " + totalPrefabUseTime;
            str += "\r\n    加载Prefab总次数 : " + mOpenUIPrefabAverageTimeDic[prefabName].Count;
            str += "\r\n    加载Prefab平均耗时 : " + totalPrefabUseTime / mOpenUIPrefabAverageTimeDic[prefabName].Count + "\r\n";

            //逻辑耗时数据
            str += "\r\n    第一次设置数据用时 : " + (mFirstOpenUITimeDic[prefabName] - mFirstOpenUIPrefabTimeDic[prefabName]);
            float totalDataUseTime = 0;
            str += "\r\n    设置所有的数据用时 : ";
            for (int i = 0; i < mOpenUIAverageTimeDic[prefabName].Count; i++)
            {
                float num = (float)System.Math.Round(mOpenUIAverageTimeDic[prefabName][i] - mOpenUIPrefabAverageTimeDic[prefabName][i], 2);
                str += "[" + i + " : " + num + "];";
                totalDataUseTime += num;
            }
            str += "\r\n    设置数据总耗时 : " + totalDataUseTime;
            str += "\r\n    设置数据总次数 : " + mOpenUIAverageTimeDic[prefabName].Count;
            str += "\r\n    设置数据平均耗时 : " + totalDataUseTime / mOpenUIAverageTimeDic[prefabName].Count + "\r\n";

            EB.Debug.LogUIStatistics("<color=#d69d85>GlobalMenuManager.WriteLocalLog 写入进本地Log的数据 : \r\n{0}</color>", str);

            if (!System.IO.File.Exists(path))
            {
                System.IO.FileStream stream = System.IO.File.Create(path);
                stream.Close();
                stream.Dispose();
            }
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path, true))
            {
                writer.WriteLine(str);
                writer.WriteLine();
                writer.WriteLine();
            }
        }

        #endregion
    }
}