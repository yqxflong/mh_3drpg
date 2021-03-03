//仅用于管理解压与下载Bundle相关
//Edit By Johny

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace GM
{
	public partial class AssetManager : MonoBehaviour
	{

		#region Instance
		private static AssetManager _instance = null;

        /// <summary>
        /// 资源管理器
        /// </summary>
		public static AssetManager Instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject _go = new GameObject("~ASSETMANAGER");
                    _go.hideFlags = HideFlags.HideAndDontSave;
					if (Application.isPlaying)
					{
						GameObject.DontDestroyOnLoad(_go);
					}
					_instance = _go.AddComponent<AssetManager>();
					_instance.Initialize();
				}
                return _instance;
			}
		}
		#endregion

		#region Public
		/// <summary>
		/// Before starting download assetbundle, make sure AssetManager is ready.
		/// </summary>
		/// <value><c>true</c> if is ready; otherwise, <c>false</c>.</value>
		public static bool IsReady
		{
			get { return Instance.mLocalBundleManager.IsReady && Instance.mRemoteBundleManager.IsReady; }
		}

		/// <summary>
		/// Their is no loading AssetBundle and Asset
		/// </summary>
		public static bool IsIdle
		{
			get { return true; }
		}

		/// <summary>
		/// Download memory size limit
		/// </summary>
		public static long MaxMemorySize { get; set; }

		/// <summary>
		/// Download decompress queue size limit
		/// </summary>
		public static int MaxDecompressQueueSize { get; set; }

		/// <summary>
		/// Download decompress thread pool
		/// </summary>
		public static void InitilizeThreadPool()
		{
			int size = Mathf.Max(2, SystemInfo.processorCount / 2);
			BundleDownloader.threadPool = new EB.ThreadPool(size);
		}

		/// <summary>
		/// Gets the remote bundles info file.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="action">Action.</param>
		/// <param name="target">Target.</param>
		public static void GetRemoteBundlesInfoFile(string url, System.Action<bool> action, GameObject target)
		{
			Instance.mRemoteBundleManager.GetRemoteBundlesInfoFile(url, action, target);
		}

		/// <summary>
		/// Sets the remote bundle file base URL.
		/// </summary>
		/// <param name="baseUrl">
		/// Base URL --- The actual bundle url is combined by this baseUrl and bundle name.
		/// And This base url should be set by app within corresponding rules.
		/// </param>
		public static void SetRemoteBundleFileBaseUrl(string baseUrl)
		{
			Instance.mBaseUrl = baseUrl;
		}

		/// <summary>
		/// Gets the remote bundle file base URL.
		/// </summary>
		/// <returns></returns>
		public static string GetRemoteBundleFileBaseUrl()
		{
			return Instance.mBaseUrl;
		}

		public static string GetBundleNameContainsAsset(string assetname)
		{
			if (IsReady)
			{
				BundleInfo _info = Instance.mRemoteBundleManager.GetBundleInfoContainsAsset(assetname);
				if (_info != null)
				{
					return _info.BundleName;
				}
			}

			return null;
		}

		///是否存在对应此asset的bundle
		public static bool HasTheAssetInBundle(string assetName)
		{
			BundleInfo info = Instance.mRemoteBundleManager.GetBundleInfoContainsAsset(assetName);
			return info != null;
		}
        
		///通过assetName获取assetPath
		public static string GetAssetFullPathByAssetName(string assetName)
		{
			BundleInfo info = Instance.mRemoteBundleManager.GetBundleInfoContainsAsset(assetName);
			if(info != null)
			{
				int idx = info.Includes.IndexOf(assetName);
				if(idx >= 0)
				{
					return info.Paths[idx];
				}
#if !UNITY_EDITOR
				else
				{
					EB.Debug.LogError("未找到对应{0}的Path!", assetName);
				}
#endif
			}
#if !UNITY_EDITOR
			else
			{
				EB.Debug.LogError("未找到对应{0}的Bundle!", assetName);
			}
#endif
			
			return assetName;
		}

		/// <summary>
        /// 通过资源包名称获取这个包里的所有资源名称
        /// </summary>
        /// <param name="bundleName">资源包名称</param>
        /// <returns></returns>
        public List<string> GetBundleInCludes(string bundleName)
        {
            BundleInfo _info = Instance.mLocalBundleManager.GetBundleInfo(bundleName);
            if (_info != null)
            {
                return _info.Includes;
            }

            return null;
        }
        
		/// <summary>
        /// 是否为本地过时的资源包
        /// </summary>
        /// <param name="bundlename">资源包名称</param>
        /// <returns></returns>
        public static bool IsLocalBundleOutdated(string bundlename)
		{
			return Instance.IsLocalBundleOutdatedImpl(bundlename);
		}

		public static void ResetDownloaBundleSize()
        {
            Instance.mDownloadedBundleSize = 0;
            Instance.mDownloadBundleSize = 0;
        }

		public static long GetDownloadedSize()
		{
			return Instance.GetDownloadedSizeImpl();
		}
		
		public static long GetTotalDownloadSize()
		{
			return Instance.GetTotalDownloadSizeImpl();
		}
		
		public static Coroutine DownloadLocalAssetBundles()
		{
			return EB.Coroutines.Run(Instance.DownloadLocalAssetBundlesCoroutine());
		}

		public static Coroutine DownloadRequiredAssetBundles(int priority)
		{
			return EB.Coroutines.Run(Instance.DownloadRequiredAssetBundlesCoroutine(priority));
		}

		public static Coroutine DownloadAssetBundle(string bundlename)
		{
			return EB.Coroutines.Run(Instance.DownloadAssetBundleCoroutine(bundlename));
		}

        /// <summary>
        /// 获取指定场景JSON细节描述文件
        /// </summary>
        /// <param name="levelName">场景名称</param>
        /// <param name="url">url路径</param>
        /// <param name="callback">完成回调</param>
        /// <returns></returns>
		public static Coroutine FetchSceneDescriptionFile(string levelName, string url, System.Action<string, string> callback)
		{
			return EB.Coroutines.Run(Instance.FetchSceneDescriptionFileCoroutine(levelName, url, callback));
		}

#if UNITY_EDITOR && !USE_ASSETBUNDLE_IN_EDITOR
		public static string GetPathByAssetName(string assetName)
			=> Instance._GetPathByAssetName(assetName);
#endif
#endregion


#region Instance Func
        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
		{
			mRemoteBundleManager = new RemoteBundleManager();
			mRemoteBundleManager.Initialize();
			mLocalBundleManager = new LocalBundleManager();
			mLocalBundleManager.Initialize();

			mDownloadedBundleList.Clear();
			mDownloadedBundleSize = 0;

#if UNITY_EDITOR && !USE_ASSETBUNDLE_IN_EDITOR
			LoadBundlesData();
#endif			
		}

        private bool IsLocalBundleExisted(string bundlename)
		{
			return System.IO.File.Exists(AssetUtils.GetFullBundlePath(bundlename));
		}

        /// <summary>
        /// 是否为本地的资源包
        /// </summary>
        /// <param name="bundlename"></param>
        /// <returns></returns>
		private bool IsLocalBundleOutdatedImpl(string bundlename)
		{
			BundleInfo _local = mLocalBundleManager.GetBundleInfo(bundlename);
			if (_local == null)
			{
				//EB.Debug.Log ("Local Bundle has not beed downloaded, bundlename = {0}", bundlename);
				return true;
			}

			BundleInfo _remote = mRemoteBundleManager.GetBundleInfo(bundlename);
			if (_remote == null)
			{
				//EB.Debug.Log ("There is no corresponding remote assetbundle file. bundlename = {0}", bundlename);
				return false;
			}

			if (_local.Version < _remote.Version)
			{
				//EB.Debug.Log ("local version {0} < remote version {1}, {2}", _local.Version, _remote.Version, bundlename);
				return true;
			}

			if ( _local.Version >= _remote.Version && _local.MD5 != _remote.MD5)
			{
				//EB.Debug.Log ("local md5 {0} != remote md5 {1}, {2}", _local.MD5, _remote.MD5, bundlename);
				return true;
			}

			if (!IsLocalBundleExisted(bundlename))
			{
				//EB.Debug.Log ("local bundle file not exists, {0}", bundlename);
				return true;
			}

			return false;
		}

		private string GetRemoteBundleUrl(string bundlename)
		{
			return mBaseUrl + bundlename.ToLower() + AssetUtils.RemoteBundleSuffix;
		}


		#region 获取当前已下载Size
		private int _GetDownloadedSizeImpl_BundleCount = 0;
		private float _GetDownloadedSizeImpl_TotalSize = 0;
		private long GetDownloadedSizeImpl()
		{
			int boundleCount = mDownloadInstances.Count;
			bool shouldCalTotalSize = false;
			if(_GetDownloadedSizeImpl_BundleCount != boundleCount)
			{
				_GetDownloadedSizeImpl_BundleCount = boundleCount;
				shouldCalTotalSize = true;
				_GetDownloadedSizeImpl_TotalSize = 0;
			}
			float size = 0;
			var it = mDownloadInstances.GetEnumerator();
			float totalProgress = 0;
			while(it.MoveNext())
			{
				var crr = it.Current;
				totalProgress += crr.Value.progress;
				if(shouldCalTotalSize)
				{
					_GetDownloadedSizeImpl_TotalSize += mRemoteBundleManager.GetBundleDownloadSize(crr.Key);
				}
			}
			float avgProgress = totalProgress / _GetDownloadedSizeImpl_BundleCount;
			size = avgProgress * _GetDownloadedSizeImpl_TotalSize;

			return (long)size + mDownloadedBundleSize + mDownloadBundleSize;
		}
		#endregion

		private long GetTotalDownloadSizeImpl()
		{
			return mRemoteBundleManager.GetTotalDownloadSize();
		}

        /// <summary>
        /// 下载资源包协程
        /// </summary>
        /// <param name="bundlename"></param>
        /// <returns></returns>
		private IEnumerator DownloadAssetBundleCoroutine(string bundlename)
		{
			EB.Collections.Stack<string> toBeDowload = new EB.Collections.Stack<string>();
			List<string> isDownloading = new List<string>();
			toBeDowload.Push(bundlename);
            //获取这个资源包的父包(依赖包)
			string _parent = mRemoteBundleManager.GetParentBundleName(bundlename);
			while (!string.IsNullOrEmpty(_parent))
			{
				if (mDownloadedBundleList.Contains(_parent))
				{
					break;
				}
				else if (!IsLocalBundleOutdated(_parent))
				{
					mDownloadedBundleList.Add(_parent);
					mDownloadedBundleSize += mRemoteBundleManager.GetBundleDownloadSize(_parent);
					break;
				}
				else if (mDownloadingList.Contains(_parent))
				{
					isDownloading.Add(_parent);
				}
				else
				{
					toBeDowload.Push(_parent);
				}

				_parent = mRemoteBundleManager.GetParentBundleName(_parent);
			}

			// wait until isDownloading list is empty
			while (isDownloading.Count > 0)
			{
				int _downloadingCnt = isDownloading.Count;
				for (int i = _downloadingCnt - 1; i >= 0; --i)
				{
					if (!mDownloadingList.Contains(isDownloading[i]))
					{
						//DownloadingBar.Finish(isDownloading[i]);
						isDownloading.RemoveAt(i);
					}
				}
				yield return null;
			}

			while (toBeDowload.Count > 0)
			{
				string _bundle = toBeDowload.Pop();
				mDownloadingList.Add(_bundle);
				yield return EB.Coroutines.Run(DownloadSingleAssetBundleCoroutine(_bundle));
				mDownloadingList.Remove(_bundle);
			}
		}

		private IEnumerator DownloadSingleAssetBundleCoroutine(string bundlename)
		{
			string url = GetRemoteBundleUrl(bundlename);
			string destpath = AssetUtils.GetFullBundlePath(bundlename);

			BundleDownloader bdl = new BundleDownloader(bundlename, url, destpath);
            if (!mDownloadInstances.ContainsKey(bundlename))
            {
                mDownloadInstances.Add(bundlename, bdl);
            }
			yield return bdl;

			if (!string.IsNullOrEmpty(bdl.error))
			{
				EB.Debug.LogWarning("DownloadSingleAssetBundleCoroutine: download {0} error = {1}", bundlename, bdl.error);
				bdl.Dispose();
				bdl = null;
                if (mDownloadInstances.ContainsKey(bundlename))
                    mDownloadInstances.Remove(bundlename);
				yield break;
			}
			
			mDownloadBundleSize += mRemoteBundleManager.GetBundleDownloadSize(bundlename);
            EB.Assets.DownloadedBundles++;

			bdl.BeginSave();
			yield return bdl.WaitDecompress();
            if (mDownloadInstances.ContainsKey(bundlename))
                mDownloadInstances.Remove(bundlename);
			if (!string.IsNullOrEmpty(bdl.error))
			{
				EB.Debug.LogWarning("DownloadSingleAssetBundleCoroutine: decompress {0} error = {1}", bundlename, bdl.error);
				bdl.Dispose();
				bdl = null;
				yield break;
			}

			bdl.Dispose();
			bdl = null;

#if UNITY_IPHONE
			UnityEngine.iOS.Device.SetNoBackupFlag(destpath);
#endif
			mLocalBundleManager.AddLocalBundleInfo(mRemoteBundleManager.GetBundleInfo(bundlename));
		}


        /// <summary>
        /// 获取指定场景描述文件
        /// </summary>
        /// <param name="levelName">场景名称</param>
        /// <param name="url">url路径</param>
        /// <param name="callback">完成回调</param>
        /// <returns></returns>
		private IEnumerator FetchSceneDescriptionFileCoroutine(string levelName, string url, System.Action<string, string> callback)
		{
			//use level description pool manager to avoid downloading json file each time!
			string descrJson = string.Empty;
			if (!LevelDescriptionPoolManager.Instance.TryGetLevelDescription(levelName, out descrJson))
			{
				if (!AssetUtils.LoadFromLocalFile(url))
				{// UniWeb support retry times
					HTTP.Request r = new HTTP.Request("GET", url);
					r.acceptGzip = true;
					r.useCache = false;
					r.maximumRedirects = 2;
					yield return r.Send();

					if (r.exception != null)
					{
						EB.Debug.LogError("Can not fetch scene description file from {0}", url);
						yield break;
					}
					else
					{
						string response = r.response.Text;
						if (!LevelDescriptionPoolManager.Instance.IsLevelRegitered(levelName))
						{
							LevelDescriptionPoolManager.Instance.RegisterLevelDescription(levelName, response);
						}
						callback(levelName, response);
						yield break;
					}
				}
				else
				{
                    //var uri = new System.Uri(url);
                    UnityWebRequest request = UnityWebRequest.Get(url);

					yield return request.SendWebRequest();
					if (request.isHttpError || request.isNetworkError)
					{
						EB.Debug.LogError("CAN NOT fetch scene description file from {0}, error = {1}", url, request.error);
						callback(levelName, null);
						yield break;
					}
					else
					{
						string response = request.downloadHandler.text;
						if (!LevelDescriptionPoolManager.Instance.IsLevelRegitered(levelName))
						{
							LevelDescriptionPoolManager.Instance.RegisterLevelDescription(levelName, response);
						}

						callback(levelName, response);
					}
				}
			}
			else
			{
				//wait one frame for unloading previous level's lightmap.
				yield return new WaitForEndOfFrame();
				callback(levelName, descrJson);
				yield break;
			}
		}


		#region 下载远程AB
		private IEnumerator DownloadAssetBundlesCoroutine_Remote(List<string> bundlenames, System.Action<List<string>, bool> action, GameObject target, bool background)
		{
#if USE_AOSHITANGSDK
			EB.Sparx.AoshitangSDKManager.getInstance().UploadLog("checkUpdate", null);
#endif
			#region 找出已下载的数量和需要下载的数量
			EB.Collections.Stack<string> deps = new EB.Collections.Stack<string>();
			EB.Collections.Queue<string> toBeDownload = new EB.Collections.Queue<string>();
			HashSet<string> toBeDownloadSet = new HashSet<string>(); // speed up lookup
			List<string> isDownloading = new List<string>();
			EB.Assets.DownloadedBundles = 0;
			int totalBundles = bundlenames.Count;
			for (int i = 0, cnt = totalBundles; i < cnt; ++i)
			{
				string _bn = bundlenames[i];

				deps.Clear();
				do
				{
					deps.Push(_bn);
					_bn = mRemoteBundleManager.GetParentBundleName(_bn);
				} while (!string.IsNullOrEmpty(_bn) && !bundlenames.Contains(_bn));

				while (deps.Count > 0)
				{
					_bn = deps.Pop();

					if (mDownloadedBundleList.Contains(_bn))
					{
						break;
					}
					else if (!IsLocalBundleOutdated(_bn))
					{
						mDownloadedBundleList.Add(_bn);
						mDownloadedBundleSize += mRemoteBundleManager.GetBundleDownloadSize(_bn);
						break;
					}
					else if (mDownloadingList.Contains(_bn))
					{
						isDownloading.Add(_bn);
					}
					else if (!toBeDownloadSet.Contains(_bn))
					{
						toBeDownloadSet.Add(_bn);
						toBeDownload.Enqueue(_bn);
					}
				}
			}
			EB.Assets.TotalBundles = totalBundles;
			EB.Assets.ToDownloadBundles = toBeDownload.Count;
			#endregion

			#region wait until isDownloading list is empty
            while (isDownloading.Count > 0)
			{
				int _downloadingCnt = isDownloading.Count;
				for (int i = _downloadingCnt - 1; i >= 0; --i)
				{
					if (!mDownloadingList.Contains(isDownloading[i]))
					{
						isDownloading.RemoveAt(i);
					}
				}
                
                yield return null;
			}
			#endregion

			#region 并行下载
			List<BundleDownloader> bdlDownloadingList = new List<BundleDownloader>();
			Dictionary<BundleDownloader, string> bdlDownloadingDic = new Dictionary<BundleDownloader, string>();
			bool ToSave = false;
			while (true)
			{
				//由于请求数限制，只允许1个请求同时下载
				if(toBeDownload.Count > 0 && bdlDownloadingList.Count < 1)
				{
					string bundle = toBeDownload.Dequeue();
					string servPath = GetRemoteBundleUrl(bundle);
					//EB.Debug.LogWarning("Bundle Serv Path = "+servPath);
					ToSave = true;
					BundleDownloader bdl = new BundleDownloader(bundle, servPath, AssetUtils.GetFullBundlePath(bundle));
					mDownloadingList.Add(bundle);
					mDownloadInstances.Add(bundle, bdl);
					bdlDownloadingList.Add(bdl);
					bdlDownloadingDic[bdl] = bundle;
				}
				else
				{
					//需要下载的列表为空，正在下载的列表为空，则跳出循环
					if(bdlDownloadingList.Count == 0 && toBeDownload.Count == 0)
					{
						break;
					}

					//移除所有下载列表中已结束的请求
					for(int i = 0; i < bdlDownloadingList.Count;)
					{
						var tmpBdl = bdlDownloadingList[i];
						var tmpBundle = bdlDownloadingDic[tmpBdl];
						if(tmpBdl.isDone){
							bdlDownloadingList.RemoveAt(i);
							if (!string.IsNullOrEmpty(tmpBdl.error))
							{
								EB.Debug.LogError("DownloadAssetBundlesCoroutine: download {0} error = {1}", tmpBundle, tmpBdl.error);
								mDownloadingList.Remove(tmpBundle);
								mDownloadInstances.Remove(tmpBundle);
								tmpBdl.Dispose();tmpBdl = null;
							}
							else
							{
								// download success				
								EB.Assets.DownloadedBundles++;
								mDownloadBundleSize += mRemoteBundleManager.GetBundleDownloadSize(tmpBundle);
								//开始解压
								tmpBdl.BeginSave();
							}
						}
						else
						{
							i++;
						}
					}
					yield return null;
				}
			}
			bdlDownloadingDic.Clear();
			#endregion
            
			#region 等待所有解压结束
            while (mDownloadingList.Count > 0)
            {
				for(int i = 0; i < mDownloadingList.Count;)
				{
					string bundlename = mDownloadingList[i];
					BundleDownloader bdl = null;
					mDownloadInstances.TryGetValue(bundlename, out bdl);
					if (bdl != null && bdl.decompressDone)
					{
						#region 对已解压结束的处理后事
						mDownloadingList.RemoveAt(i);
						mDownloadInstances.Remove(bundlename);

						if (!string.IsNullOrEmpty(bdl.error))
						{
							EB.Debug.LogError("DownloadAssetBundlesCoroutine: decompress {0} error = {1}", bundlename, bdl.error);
							bdl.Dispose();
							bdl = null;
							EB.Assets.DownloadedBundles--;
							continue;
						}

						bdl.Dispose();bdl = null;
#if UNITY_IPHONE
						UnityEngine.iOS.Device.SetNoBackupFlag(AssetUtils.GetFullBundlePath(bundlename));
#endif
						// success at last
						if(ToSave) mLocalBundleManager.AddLocalBundleInfo(mRemoteBundleManager.GetBundleInfo(bundlename));
						#endregion
					}
					else
					{
						i++;
					}
				}
			}
			#endregion

			if(ToSave) mLocalBundleManager.SaveLocalBundlesInfoFile();
			AssetUtils.DoAction(action, bundlenames, true, target);
#if USE_AOSHITANGSDK
			EB.Sparx.AoshitangSDKManager.getInstance().UploadLog("updateOver", null);
#endif
		}

		//下载远程的AB
		private IEnumerator DownloadRequiredAssetBundlesCoroutine(int priority)
		{
			while (mRemoteBundleManager == null || !mRemoteBundleManager.IsReady)
			{
				yield return null;
			}

			while (EB.Assets.IsDownloadingOnLoad)
			{
				yield return null;
			}

			EB.Assets.IsDownloadingOnLoad = true;
            yield return EB.Coroutines.Run(DownloadAssetBundlesCoroutine_Remote(Instance.mRemoteBundleManager.GetDownLoadBundleNames(priority), null, null, false));
			EB.Assets.IsDownloadingOnLoad = false;
		}
		#endregion

		#region 解压本地AB
		private IEnumerator DownloadAssetBundlesCoroutine_Local(List<string> bundlenames, System.Action<List<string>, bool> action, GameObject target, bool background)
		{
			#region 找出已下载的数量和需要下载的数量
			EB.Collections.Stack<string> deps = new EB.Collections.Stack<string>();
			EB.Collections.Queue<string> toBeDownload = new EB.Collections.Queue<string>();
			HashSet<string> toBeDownloadSet = new HashSet<string>(); // speed up lookup
			List<string> isDownloading = new List<string>();
			EB.Assets.DownloadedBundles = 0;
			int totalBundles = bundlenames.Count;
			for (int i = 0, cnt = totalBundles; i < cnt; ++i)
			{
				string _bn = bundlenames[i];

				deps.Clear();
				do
				{
					deps.Push(_bn);
					_bn = mRemoteBundleManager.GetParentBundleName(_bn);
				} while (!string.IsNullOrEmpty(_bn) && !bundlenames.Contains(_bn));

				while (deps.Count > 0)
				{
					_bn = deps.Pop();

					if (mDownloadedBundleList.Contains(_bn))
					{
						break;
					}
					else if (!IsLocalBundleOutdated(_bn))
					{
						mDownloadedBundleList.Add(_bn);
						mDownloadedBundleSize += mRemoteBundleManager.GetBundleDownloadSize(_bn);
						break;
					}
					else if (mDownloadingList.Contains(_bn))
					{
						isDownloading.Add(_bn);
					}
					else if (!toBeDownloadSet.Contains(_bn))
					{
						toBeDownloadSet.Add(_bn);
						toBeDownload.Enqueue(_bn);
					}
				}
			}
			EB.Assets.TotalBundles = totalBundles;
			EB.Assets.ToDownloadBundles = toBeDownload.Count;
			#endregion

			#region 并行下载
			List<BundleDownloader> bdlDownloadingList = new List<BundleDownloader>();
			Dictionary<BundleDownloader, string> bdlDownloadingDic = new Dictionary<BundleDownloader, string>();
			while (toBeDownload.Count > 0)
			{
				string bundle = toBeDownload.Dequeue();
				BundleDownloader bdl = new BundleDownloader(bundle, GetRemoteBundleUrl(bundle), AssetUtils.GetFullBundlePath(bundle));
				mDownloadingList.Add(bundle);
				mDownloadInstances.Add(bundle, bdl);
				bdlDownloadingList.Add(bdl);
				bdlDownloadingDic[bdl] = bundle;
				if(toBeDownload.Count % 20 == 0)
				{
					yield return null;
				}
			}

			while(bdlDownloadingList.Count > 0)
			{
				var bdl = bdlDownloadingList[0];
				var bundle = bdlDownloadingDic[bdl];

				if(!bdl.isDone)
				{
					yield return null;
					continue;
				}
				else
				{
					bdlDownloadingList.RemoveAt(0);
				}

				if (!string.IsNullOrEmpty(bdl.error))
				{
					EB.Debug.LogError("DownloadAssetBundlesCoroutine: download {0} error = {1}", bundle, bdl.error);
					mDownloadingList.Remove(bundle);
					mDownloadInstances.Remove(bundle);
					bdl.Dispose();bdl = null;
				}
				else
				{
					// download success				
					EB.Assets.DownloadedBundles++;
					mDownloadBundleSize += mRemoteBundleManager.GetBundleDownloadSize(bundle);
					//开始解压
					bdl.BeginSave();
				}

				if(bdlDownloadingList.Count % 20 == 0)
				{
					yield return null;
				}
			}

			bdlDownloadingDic.Clear();
			#endregion
            
			#region 等待所有解压结束
            while (mDownloadingList.Count > 0)
            {
				for(int i = 0; i < mDownloadingList.Count;)
				{
					string bundlename = mDownloadingList[i];
					BundleDownloader bdl = null;
					mDownloadInstances.TryGetValue(bundlename, out bdl);
					if (bdl != null && bdl.decompressDone)
					{
						#region 对已解压结束的处理后事
						mDownloadingList.RemoveAt(i);
						mDownloadInstances.Remove(bundlename);

						if (!string.IsNullOrEmpty(bdl.error))
						{
							EB.Debug.LogError("DownloadAssetBundlesCoroutine: decompress {0} error = {1}", bundlename, bdl.error);
							bdl.Dispose();
							bdl = null;
							EB.Assets.DownloadedBundles--;
							continue;
						}

						bdl.Dispose();bdl = null;
#if UNITY_IPHONE
						UnityEngine.iOS.Device.SetNoBackupFlag(AssetUtils.GetFullBundlePath(bundlename));
#endif
						// success at last
						mLocalBundleManager.AddLocalBundleInfo(mRemoteBundleManager.GetBundleInfo(bundlename));
						#endregion
					}
					else
					{
						i++;
					}
				}
			}
			#endregion

			mLocalBundleManager.SaveLocalBundlesInfoFile();
			AssetUtils.DoAction(action, bundlenames, true, target);
		}

		private IEnumerator DownloadLocalAssetBundlesCoroutine()
		{
			while (mRemoteBundleManager == null || !mRemoteBundleManager.IsReady)
			{
				yield return null;
			}

			while (EB.Assets.IsDownloadingOnLoad)
			{
				yield return null;
			}

			EB.Assets.IsDownloadingOnLoad = true;
            yield return EB.Coroutines.Run(DownloadAssetBundlesCoroutine_Local(Instance.mRemoteBundleManager.GetDownLoadBundleNames(), null, null, false));
			EB.Assets.IsDownloadingOnLoad = false;
		}
		#endregion
#endregion

#region debug

		public static void RecordLoadStart(string tag,string assetname)
		{
#if DEBUG_LOAD_ASSET
			float time = ((float)((System.DateTime.UtcNow.Ticks / 10000) % 100000) / 1000);
			EB.Debug.Log(string.Format("LoadRecord--{0}====start {1} frame ={2} time={3}", tag, assetname, UnityEngine.Time.frameCount, time));
#endif
			}

		public static void RecordLoadEnd(string tag, string assetname)
		{
#if DEBUG_LOAD_ASSET
			float time = ((float)((System.DateTime.UtcNow.Ticks / 10000) % 100000) / 1000);
			EB.Debug.Log(string.Format("LoadRecord--{0}====end {1} frame ={2} time={3}", tag, assetname, UnityEngine.Time.frameCount, time));
#endif
		}
#endregion debug
        
#region Private Member
		/// <summary>
        /// 远程服务器资源管理器
        /// </summary>
		private RemoteBundleManager mRemoteBundleManager = null;
        /// <summary>
        /// 本地资源包数据管理器
        /// </summary>
		private LocalBundleManager mLocalBundleManager = null;
        /// <summary>
        /// 下载的资源包名称列表
        /// </summary>
		private HashSet<string> mDownloadedBundleList = new HashSet<string>();
		private long mDownloadedBundleSize = 0;
		private long mDownloadBundleSize = 0;
		private List<string> mDownloadingList = new List<string>();
		private Dictionary<string, BundleDownloader> mDownloadInstances = new Dictionary<string, BundleDownloader>();
		private string mBaseUrl = null;
#endregion
	}


#if UNITY_EDITOR && !USE_ASSETBUNDLE_IN_EDITOR
	public partial class AssetManager : MonoBehaviour
	{
		private string mBundlesDataFile = "Assets/_ThirdParty/BundleManager/BundleData.txt";
		private List<BundleData> mBundles = null;
		private Dictionary<string, string> mAssetDict = new Dictionary<string, string>();
        private Dictionary<string, string> mDependDict = new Dictionary<string, string>();

		private void LoadBundlesData()
		{
			if (!System.IO.File.Exists(mBundlesDataFile))
			{
				mBundles = new List<BundleData>();
				return;
			}

			System.IO.TextReader reader = new System.IO.StreamReader(mBundlesDataFile);
			if(reader == null)
			{
				EB.Debug.LogError("Cannot find {0}", mBundlesDataFile);
				reader.Close();
				mBundles = new List<BundleData>();
			}
			else
			{
				mBundles = LitJson.JsonMapper.ToObject<List<BundleData>>(reader.ReadToEnd());
				if(mBundles == null)
				{
					EB.Debug.LogError("Cannot read data from {0}", mBundlesDataFile);
				}
				reader.Close();
			}

			foreach (var bd in mBundles)
			{
				foreach(var include in bd.includs)
				{
					string assetName = System.IO.Path.GetFileNameWithoutExtension(include).ToLower();
					mAssetDict[assetName] = include;

                }
                foreach (var depend in bd.dependAssets)
                {
					string assetName = System.IO.Path.GetFileNameWithoutExtension(depend).ToLower();
                    mDependDict[assetName] = depend;
                }

            }
		}

		private string _GetPathByAssetName(string assetName)
        {
            string path;
            if (mAssetDict.TryGetValue(assetName, out path) || mDependDict.TryGetValue(assetName, out path))
            {
                return path;
            }

            return string.Empty;
        }
	}
#endif
}

