using System;
using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class MenuCreator : DynamicMonoHotfix
    {
        public string menuPrefabName;
        public string hotfixClassPath;
        public Transform parentTransform;
        public float preloadDelay;

        protected object mParam = null;
        /// <summary> 界面控制器的GameObject </summary>
        protected GameObject mMenuObject = null;
        protected UIController mMenu = null;
        protected bool mQueue = false;
        protected bool mLoading = false;
        protected bool mShow = false;
        protected System.Action<GameObject> mOnReady = null;

        public GameObject MenuObject
        {
            get { return mMenuObject; }
        }

        public UIController Menu
        {
            get { return mMenu; }
        }

        public bool Ready
        {
            get { return mMenuObject != null && !mLoading; }
        }

        public object Param
        {
            get { return mParam; }
        }

        public bool Queue
        {
            get { return mQueue; }
        }

        public override void Awake()
        {
            if (mDMono.StringParamList != null)
            {
                var count = mDMono.StringParamList.Count;

                if (count > 0)
                {
                    menuPrefabName = mDMono.StringParamList[0];
                }
                if (count > 1)
                {
                    hotfixClassPath = mDMono.StringParamList[1];
                }
            }

            if (string.IsNullOrEmpty(menuPrefabName))
            {
                menuPrefabName = mDMono.name;
            }

            if (parentTransform == null)
            {
                parentTransform = mDMono.transform;
            }
        }

        public override void Start()
        {
            #region 注册监听器
            Hotfix_LT.Messenger.AddListener<string, object, bool>(Hotfix_LT.EventName.ShowMenu, OnShowMenuListener);
            Hotfix_LT.Messenger.AddListener<string>(Hotfix_LT.EventName.CloseMenu, OnCloseMenuListener);
            Hotfix_LT.Messenger.AddListener<string>(Hotfix_LT.EventName.PreloadMenu, OnPreloadMenuListener);
            #endregion
        }

        public override void OnEnable()
        {
            TryPreloadMenu();
        }
        
        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener<string, object, bool>(Hotfix_LT.EventName.ShowMenu, OnShowMenuListener);
            Hotfix_LT.Messenger.RemoveListener<string>(Hotfix_LT.EventName.CloseMenu, OnCloseMenuListener);
            Hotfix_LT.Messenger.RemoveListener<string>(Hotfix_LT.EventName.PreloadMenu, OnPreloadMenuListener);
            CloseAndDestroyMenu();
        }

        protected virtual void OnPreloadMenuListener(string menuName)
        {
            if (menuName == mDMono.name || menuName == menuPrefabName)
            {
                PreloadMenu();
            }
        }

        protected virtual void OnShowMenuListener(string menuName, object menuParam, bool queue)
        {
            if (menuName == mDMono.name || menuName == menuPrefabName)
            {
                CreateMenu(menuParam, queue, null);
            }
        }

        protected virtual void OnCloseMenuListener(string menuName)
        {
            if (menuName == mDMono.name || menuName == menuPrefabName)
            {
                CloseMenu();
            }
        }

        protected virtual void TryPreloadMenu()
        {
            if (preloadDelay > 0.0f && mMenuObject == null && !mLoading)
            {
                PreloadMenu();
            }
        }

        public void PreloadMenu()
        {
            if (mMenuObject == null && !mLoading)
            {
                mLoading = true;
                mShow = false;
                // GM.AssetManager.GetAsset<GameObject>(menuPrefabName, OnAssetReady, mDMono.gameObject, true);
                EB.Assets.LoadAsyncAndInit<GameObject>(menuPrefabName, OnAssetReady, mDMono.gameObject);
            }
        }

        public void CreateMenu()
        {
            CreateMenu(null, false, null);
        }

        /// <summary>
        /// 创建界面(完成无回调)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="queue"></param>
        public void CreateMenu(object param, bool queue)
        {
            CreateMenu(param, queue, null);
        }

        /// <summary>
        /// 创建界面(完成有回调)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="queue"></param>
        /// <param name="ready"></param>
        public void CreateMenu(object param, bool queue, System.Action<GameObject> ready)
        {
            mQueue = queue;
            mParam = param;
            mShow = true;

            GlobalMenuManager.Instance.OpenUIStart(menuPrefabName);

            if (mMenu != null && !UIController.IsDestroyed(mMenu))
            {
                mMenu.SetMenuData(mParam);
                mMenu.PlayTween();
                if (mQueue)
                {
                    mMenu.Queue();
                }
                else
                {
                    mMenu.Open();
                }

                if (ready != null)
                {
                    ready(mMenuObject);
                }
                return;
            }
            else if (mMenuObject != null)
            {
                mMenuObject.CustomSetActive(true);
                mMenuObject.BroadcastMessage("SetMenuData", mParam, SendMessageOptions.DontRequireReceiver);

                if (ready != null)
                {
                    ready(mMenuObject);
                }
                return;
            }
            else if (mLoading)
            {
                EB.Debug.LogWarning("CreateMenuWithParam: menu {0} is loading", menuPrefabName);

                if (ready != null)
                {
                    mOnReady += ready;
                }

                return;
            }
            else
            {
                if (ready != null)
                {
                    mOnReady += ready;
                }

                mLoading = true;
                
                // GM.AssetManager.GetAsset<GameObject>(menuPrefabName, OnAssetReady, mDMono.gameObject, true);
                EB.Assets.LoadAsyncAndInit<GameObject>(menuPrefabName, OnAssetReady, mDMono.gameObject);
            }
        }


        /// <summary>
        /// 加载预制完成的回调
        /// </summary>
        /// <param name="assetname"></param>
        /// <param name="go"></param>
        /// <param name="successed"></param>
        protected virtual void OnAssetReady(string assetName, GameObject go, bool succ)
        {
            try
            {
                if (!succ)
                {
                    //如果不成功 go 是null
                    mLoading = false;
                    EB.Debug.LogError("MenuCreater.OnAssetReady: load {0} failed", go.ToString());

                    if (mOnReady != null)
                    {
                        mOnReady(null);
                        mOnReady = null;
                    }
                    return;
                }

                GlobalMenuManager.Instance.OpenUIPrefabEnd(menuPrefabName);

                if (!mLoading)
                {
                    GameObject.Destroy(go);

                    if (mOnReady != null)
                    {
                        mOnReady(null);
                        mOnReady = null;
                    }
                    return;
                }

                PloadData data;
                data.AssetName = menuPrefabName;
                data.Go = go;

                if (!string.IsNullOrEmpty(hotfixClassPath))
                {
                    //EB.Coroutines.Run(Process(true, menuPrefabName, go));

                    if (HotfixILRManager.GetInstance().IsInit)
                    {
                        data.IsHotfix = true;
                        OnPloadProcess(data);
                    }
                    else
                    {
                        if (m_IsManagerReadedHandler == 0)
                        {
                            m_IsManagerReadedHandler = ILRTimerManager.instance.AddTimer(50, int.MaxValue,
                                delegate(int sequence)
                                {
                                    if (HotfixILRManager.GetInstance().IsInit)
                                    {
                                        ILRTimerManager.instance.RemoveTimer(m_IsManagerReadedHandler);
                                        m_IsManagerReadedHandler = 0;

                                        data.IsHotfix = true;
                                        OnPloadProcess(data);
                                    }
                                }
                            );
                        }
                    }
                }
                else
                {
                    //EB.Coroutines.Run(Process(false, menuPrefabName, go));

                    data.IsHotfix = false;
                    OnPloadProcess(data);
                }
            }
            catch(System.NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
            }
        }

        private int m_IsManagerReadedHandler = 0;
		private int m_IsPloadedHandler = 0;
		private int m_IsLoaderLoadedHandler = 0;

		public struct PloadData
		{
			public string AssetName;
			public GameObject Go;
			public bool IsHotfix;
		}

		private void OnPloadProcess(PloadData data)
        {
	        PrefabCreator[] pcs = data.Go.GetComponentsInChildren<PrefabCreator>(true);
	        for (int i = 0; i < pcs.Length; i++)
	        {
		        pcs[i].LoadAsset();
	        }

	        PrefabLoader[] loaders = data.Go.GetComponentsInChildren<PrefabLoader>(true);
			for (int i = 0; i < loaders.Length; i++)
			{
				loaders[i].LoadPrefab();
			}

			if (!CheckPloaded(pcs, loaders, data))
			{
				if (m_IsPloadedHandler == 0)
				{
					m_IsPloadedHandler = ILRTimerManager.instance.AddTimer(50, int.MaxValue, delegate (int sequence)
					{
						CheckPloaded(pcs, loaders, data);
					});
				}
			}
		}

		bool CheckPloaded(PrefabCreator[] pcs, PrefabLoader[] loaders, PloadData data)
		{
			bool isPloaded = true;
			for (int i = 0; i < pcs.Length; i++)
			{
				if (!pcs[i].isCurrendAssetLoaded)
				{
					isPloaded = false;
					break;
				}
			}

			if (isPloaded)
			{
				for (int i = 0; i < loaders.Length; i++)
				{
					if (!loaders[i].IsAssetLoaded)
					{
						isPloaded = false;
						break;
					}
				}
			}

			if (isPloaded)
			{
				if (m_IsPloadedHandler != 0)
				{
					ILRTimerManager.instance.RemoveTimer(m_IsPloadedHandler);
					m_IsPloadedHandler = 0;
				}

				if (data.IsHotfix)
				{
					if (data.Go.GetComponent<UIControllerILR>() == null)
					{
						UIControllerILR ucr = data.Go.AddComponent<UIControllerILR>();
						ucr.hotfixClassPath = hotfixClassPath;
						ucr.ILRObjInit();
					}
					else
					{
						UIControllerILR ucr = data.Go.GetComponent<UIControllerILR>();
						ucr.ILRObjInit();
					}
				}

				OnAssetProcess(data.AssetName, data.Go);
			}
			return isPloaded;
		}

		/// <summary>
		/// 对加载预制完成的数据进行处理（热更的处理，预制嵌套的处理等）
		/// </summary>
		/// <param name="isHotfix"></param>
		/// <param name="assetName"></param>
		/// <param name="go"></param>
		/// <returns></returns>
		IEnumerator Process(bool isHotfix, string assetName, GameObject go)
        {
            if (isHotfix)
            {
                while (!HotfixILRManager.GetInstance().IsInit)
                {
                    yield return null;
                }
            }

            PrefabCreator[] pcs = go.GetComponentsInChildren<PrefabCreator>(true);
            for (int i = 0; i < pcs.Length; i++)
            {
                pcs[i].LoadAsset();
            }

            PrefabLoader[] loaders = go.GetComponentsInChildren<PrefabLoader>(true);
            for (int i = 0; i < loaders.Length; i++)
            {
                loaders[i].LoadPrefab();
            }

            bool isPloaded = false;
            while (!isPloaded)
            {
                isPloaded = true;
                for (int i = 0; i < pcs.Length; i++)
                {
                    if (!pcs[i].isCurrendAssetLoaded)
                    {
                        isPloaded = false;
                        break;
                    }
                }
                if (!isPloaded)
                {
                    yield return null;
                }
            }

            bool isLoaded = false;
            while (!isLoaded)
            {
                isLoaded = true;
                for (int i = 0; i < loaders.Length; i++)
                {
                    if (!loaders[i].IsAssetLoaded)
                    {
                        isLoaded = false;
                        break;
                    }
                }
                if (!isLoaded)
                {
                    yield return null;
                }
            }

            if (isHotfix)
            {
                if (go.GetComponent<UIControllerILR>() == null)
                {
                    UIControllerILR ucr = go.AddComponent<UIControllerILR>();
                    ucr.hotfixClassPath = hotfixClassPath;
                    ucr.ILRObjInit();
                }
                else
                {
                    UIControllerILR ucr = go.GetComponent<UIControllerILR>();
                    ucr.ILRObjInit();
                }
            }

            OnAssetProcess(assetName, go);
        }

        /// <summary>
        /// 对加载好的预制做处理
        /// </summary>
        /// <param name="asetName"></param>
        /// <param name="go"></param>
        void OnAssetProcess(string asetName, GameObject go)
        {
            mLoading = false;

            mMenuObject = go;
            mMenuObject.transform.SetParent(parentTransform == null ? mDMono.transform.parent : parentTransform);
            mMenuObject.transform.position = Vector3.zero;
            mMenuObject.transform.localScale = Vector3.one;
            mMenuObject.transform.rotation = Quaternion.identity;

            mMenu = mMenuObject.GetComponent<UIController>();
            if (mMenu != null)
            {
                mMenu.MCurrentViewName = asetName;
                mMenu.destroyHandler = new System.Action<UIController>(DestroyMenuHandler);
            }

            if (mShow)
            {
                if (mMenu != null)
                {
                    mMenu.SetMenuData(mParam);
                    mMenu.PlayTween();
                    if (mQueue)
                    {
                        mMenu.Queue();
                    }
                    else
                    {
                        mMenu.Open();
                    }
                }
                else
                {
                    mMenuObject.CustomSetActive(true);
                    mMenuObject.BroadcastMessage("SetMenuData", mParam, SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
                if (mMenu != null)
                {
                    UIStack.Close(mMenu);
                }
                else
                {
                    mMenuObject.CustomSetActive(false);
                }
            }

            if (mOnReady != null)
            {
                mOnReady(mMenuObject);
                mOnReady = null;
            }

            GlobalMenuManager.Instance.OpenUIEnd(menuPrefabName);
        }

        /// <summary>
        /// 关闭界面（不销毁界面）
        /// </summary>
        public void CloseMenu()
        {
            if (mMenu != null && mMenu.IsOpen())
            {
                mMenu.Close();
            }
            else if (mMenu == null && mMenuObject != null)
            {
                mMenuObject.CustomSetActive(false);
            }

            mShow = false;
        }

        /// <summary>
        /// 关闭界面并且销毁界面
        /// </summary>
        public void CloseAndDestroyMenu()
        {
            if (mMenuObject == null && !mLoading)
            {
                return;
            }

            if (mMenu != null && mMenu.IsOpen())
            {
                mMenu.Close();
            }

            if (mMenuObject != null)
            {// if Close doesn't call DestroyMenuHandler
                DestroyMenu();
            }
            else
            {// clean loading
                mQueue = false;
                mParam = null;
                mLoading = false;
                mShow = false;
            }
        }

        /// <summary>
        /// 销毁界面
        /// </summary>
        protected virtual void DestroyMenu()
        {
            if (mMenuObject == null && !mLoading)
            {
                return;
            }

            if (mMenuObject != null)
            {
                if (mMenu != null)
                {
                    UIPanel panel = mMenu.GetComponent<UIPanel>();
                    if (panel != null)
                    {
                        panel.enabled = false;
                    }

                    mMenu.destroyHandler = null;
                }
                GameObject.Destroy(mMenuObject);

                EB.Debug.LogUI("<color=#ff0000>销毁UI界面_到UI释放策略</color>_MenuCreator.DestroyMenu(), PrefabName : <color=#00ff00>{0}</color>", menuPrefabName);
                EB.Assets.UnloadAssetByName(menuPrefabName, true);
                
                mMenuObject = null;
                mMenu = null;
            }

            mQueue = false;
            mParam = null;
            mLoading = false;
            mShow = false;
        }

        protected void DestroyMenuHandler(UIController ui)
        {
            if (ui != mMenu)
            {
                EB.Debug.LogError("DestroyMenuHandler: invalid ui = {0} != {1}", ui.name, mMenu.name);
                return;
            }

            DestroyMenu();
        }
    }
}