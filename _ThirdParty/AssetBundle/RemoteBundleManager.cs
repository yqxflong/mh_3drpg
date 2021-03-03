using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
#if UNITY_EDITOR || UNITY_IOS
using System.Runtime.Serialization.Json;
#endif

namespace GM
{
    /// <summary>
    /// 远程服务器的资源包管理器
    /// </summary>
	public class RemoteBundleManager
    {
        public static bool IsDownLoadABInGame = false;
        public static string[] Includes = new string[32] 
        {
            "UI_Login",
            "UIHelperBundle",
            "BM",
            "common",
            "ltinstance",
            "ltmaininstance",
            "s001a-prefab",
            "combat",
            "ltmainmenu",
            "ltpartnerhud",
            "chapter",
            "ltbattleresultscreen",
            "ltpartnerequipmenthud",
            "generalassets",
            "hudsharedassets",
            "chapter1_1",
            "ltchallengeinstancehud",
            "ltnationappointresultui",
            "ltnationbattleformationui",
            "ltranklisthud",
            "ltlegionwarqualify",
            "ltlegionwarfinal",
            "ltnationbattlehudui",
            "arenalogui",
            "ltnationbattleentryui",
            "ltcombatreadyui",
            "ltherobattlemenu",
            "ltgamesettinghud",
            "ltcheakenemyhud",
            "partner_head",
            "charactersharedassets",
            "ltnationhudui"
        };

        private static bool CheckBuildBundle(string fileName)
        {
            foreach (var b in Includes)
            {
                if (fileName.ToLower().IndexOf(b.ToLower()) != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsBuildBundle(string fileName)
        {
            if (!Application.isPlaying && IsDownLoadABInGame)
            {
                return CheckBuildBundle(fileName);
            }
            else if (Application.isPlaying)
            {
#if USE_DOWNLOAD_AB
                return CheckBuildBundle(fileName);
#endif
            }
            return true;
        }

        public void Initialize()
		{
			isGetting = false;
			isReady = false;
			mRemoteBundlesDict.Clear();
		}

		public bool IsReady
		{
			get
			{
				return isReady;
			}
		}

		public void GetRemoteBundlesInfoFile(string url, System.Action<bool> action, GameObject target)
		{
			if (!isGetting)
			{
				isGetting = true;
				isReady = false;
				EB.Coroutines.Run(_GetRemoteBundlesInfoFileCoroutine(url, action, target));
			}
			else
			{
				EB.Debug.LogError("GetRemoteBundlesInfoFile is downloading");
			}
		}

		/// <summary>
		/// Gets the bundle contains asset.
        /// 获取指定资源包名称的数据信息
		/// </summary>
		/// <returns>The bundle contains asset.</returns>
		/// <param name="assetname">资源名称Assetname.</param>
		public BundleInfo GetBundleInfoContainsAsset(string assetname)
		{
			if (!isReady)
			{
				EB.Debug.LogError("Attempt to get remote bundle data which contains asset {0} before ready", assetname);
				return null;
			}

			mRemoteAssetsDict.TryGetValue(assetname, out var bdl);
			return bdl;
		}

		/// <summary>
		/// Gets the bundle.
        /// 获取资源包的数据信息
		/// </summary>
		/// <returns>The bundle.</returns>
		/// <param name="bundlename">Bundlename.</param>
		public BundleInfo GetBundleInfo(string bundlename)
		{
			if (!isReady)
			{
				EB.Debug.LogError("Attempt to get remote bundle data which name is {0} before ready", bundlename);
				return null;
			}

			mRemoteBundlesDict.TryGetValue(bundlename, out var ret);
			return ret;
		}

		/// <summary>
		/// Get download size of all bundles
		/// </summary>
		/// <returns></returns>
		public long GetTotalDownloadSize()
		{
			if (!isReady)
			{
				EB.Debug.LogError("Attempt to get total size before ready");
				return 0;
			}

			long total = 0;
			foreach (var entry in mRemoteBundlesDict)
			{
				total += entry.Value.Size;
			}

			return total;
		}
        /// <summary>
        /// 获取下载的资源包大小
        /// </summary>
        /// <param name="bundlename"></param>
        /// <returns></returns>
		public long GetBundleDownloadSize(string bundlename)
		{
			if (!isReady)
			{
				EB.Debug.LogError("Attempt to get bundle size before ready");
				return 0;
			}

			if (mRemoteBundlesDict.TryGetValue(bundlename, out var item))
			{
				return item.Size;
			}

			return 0;
		}

		/// <summary>
		/// Gets the name of the parent bundle.
		/// </summary>
		/// <returns>The parent bundle name.</returns>
		/// <param name="bundlename">Bundlename.</param>
		public string GetParentBundleName(string bundlename)
		{
			if (!mRemoteBundlesDict.ContainsKey(bundlename))
			{
				return null;
			}

			return mRemoteBundlesDict[bundlename].Parent;
		}

		IEnumerator _GetRemoteBundlesInfoFileCoroutine(string url, System.Action<bool> action, GameObject target)
        {
#if USE_LOCAL_ABINFO //使用本地AB依赖包的配制信息表
            string textPath = "BundleShipInfo";
            TextAsset textAsset = Resources.Load<TextAsset>(textPath);
            if (textAsset == null)
            {
                EB.Debug.LogErrorFormat("使用读取本地依赖AB包信息表，发现指定不存在相应的文件textPath:{0}", textPath);
                yield break;
            }else{
                EB.Debug.LogFormat("使用本地AB依赖包的配制信息表,相应的文件textPath:{0}", textPath);
            }
            ParseRemoteBundlesInfoFile(textAsset.text);
            isReady = true;
            isGetting = false;
            AssetUtils.DoAction(action, true, target);
            yield break;
#endif
            yield return null;
            if (!AssetUtils.LoadFromLocalFile(url))
			{
				HTTP.Request r = new HTTP.Request("GET", url);
				r.acceptGzip = true;
				r.useCache = false;
				r.maximumRedirects = 2;

				EB.Debug.Log("Getting Remote Bundles Info File from: {0}", url);

				yield return r.Send();

				if (r.exception != null)
				{
					isReady = false;
					isGetting = false;
					EB.Debug.LogWarning("CAN NOT fetch scene description file from {0}, error = {1}", url, r.exception.Message);
					AssetUtils.DoAction(action, false, target);
				}
				else
				{
					if (r.response.status == 404)
					{
						EB.Debug.LogError("HTTP 404 error when gettings file from url: {0}", url);
					}
					else
					{
						EB.Debug.Log("_GetRemoteBundlesInfoFileCoroutine: success, length = {0}", r.response.Text.Length);
#if UNITY_EDITOR || UNITY_IOS
						byte[] data = r.response.Bytes;
						ParseRemoteBundlesInfoFile(data);
#else
						string data = r.response.Text;
						ParseRemoteBundlesInfoFile(data);
#endif
						GM.AssetManager.ResetDownloaBundleSize();
                        isReady = true;
						isGetting = false;
						if (data != null)
						{
							int gid = System.GC.GetGeneration(data);
							data = null;
							System.GC.Collect(gid);
						}
						AssetUtils.DoAction(action, true, target);						
					}
				}
			}
			else
			{
                UnityWebRequest request = UnityWebRequest.Get(url);
                EB.Debug.Log("Getting Remote Bundles Info File from: {0}", url);
                yield return request.SendWebRequest();
				if (request.isHttpError || request.isNetworkError)
				{
					isReady = false;
					isGetting = false;
					EB.Debug.LogError("CAN NOT fetch scene description file from {0}, error = {1}", url, request.error);
					AssetUtils.DoAction(action, false, target);
				}
				else
				{
					//EB.Debug.Log("_GetRemoteBundlesInfoFileCoroutine: success, length = {0}", request.downloadHandler.text.Length);
#if UNITY_EDITOR || UNITY_IOS
					byte[] data = request.downloadHandler.data;
					ParseRemoteBundlesInfoFile(data);
#else
					string data = request.downloadHandler.text;
					ParseRemoteBundlesInfoFile(data);
#endif
					GM.AssetManager.ResetDownloaBundleSize();
                    isReady = true;
					isGetting = false;
					if (data != null)
					{
						int gid = System.GC.GetGeneration(data);
						data = null;
						System.GC.Collect(gid);
					}
					AssetUtils.DoAction(action, true, target);
				}
                request.Dispose();
                request = null;
				yield return true;
			}
		}

#if UNITY_EDITOR || UNITY_IOS
		private void ParseRemoteBundlesInfoFile(byte[] data)
#else
		private void ParseRemoteBundlesInfoFile(string data)
#endif
		{
			Debug.LogWarning("ParseRemoteBundlesInfoFile");

#if UNITY_EDITOR || UNITY_IOS
			List<BundleInfo> bundleInfos = new List<BundleInfo>();
			using (var ms = new System.IO.MemoryStream(data))
			{
				DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(List<BundleInfo>));
				bundleInfos = (List<BundleInfo>)deseralizer.ReadObject(ms);// //反序列化ReadObject
			}
			mRemoteBundlesList = bundleInfos.Count > 0 ? bundleInfos : mRemoteBundlesList;
			bundleInfos = null;
#else
			mRemoteBundlesList = LitJson.JsonMapper.ToObject<List<BundleInfo>>(data);
#endif

			mRemoteDownLoadBundlesList = new List<BundleInfo>();
            for (int i = 0; i < mRemoteBundlesList.Count; i++)
            {
                if (!IsBuildBundle(mRemoteBundlesList[i].BundleName))
                {
                    mRemoteDownLoadBundlesList.Add(mRemoteBundlesList[i]);
                }
            }

            mRemoteBundlesDict.Clear();
			mRemoteBundlesCnt = mRemoteBundlesList.Count;
			for (int i = 0; i < mRemoteBundlesCnt; ++i)
			{
				var shipInfo = mRemoteBundlesList[i];
				if (!mRemoteBundlesDict.ContainsKey(shipInfo.BundleName))
				{
					mRemoteBundlesDict.Add(shipInfo.BundleName, shipInfo);

					for (int j = 0; j < shipInfo.Includes.Count; ++j)
					{
						string assetName = shipInfo.Includes[j];
						mRemoteAssetsDict[assetName] = shipInfo;
					}
				}
			}			
		}

		public List<string> GetBundleNames()
		{
			List<string> names = new List<string>();

			if(mRemoteBundlesList != null)
			{
				List<BundleInfo>.Enumerator iterator = mRemoteBundlesList.GetEnumerator();
				while(iterator.MoveNext())
				{
					names.Add(iterator.Current.BundleName);
				}
			}

			return names;
		}

		public List<string> GetBundleNames(int priority)
		{
			List<string> names = new List<string>();

			if (mRemoteBundlesList != null)
			{
				List<BundleInfo>.Enumerator iterator = mRemoteBundlesList.GetEnumerator();
				while (iterator.MoveNext())
				{
					if (priority < 0 || iterator.Current.Priority <= priority)
					{
						names.Add(iterator.Current.BundleName);
					}
				}
			}

			return names;
		}

        public long GetBundleSize()
        {
            long size = 0;
            if (mRemoteBundlesList != null)
            {
                var iterator = mRemoteBundlesList.GetEnumerator();
                while (iterator.MoveNext())
                {
                    size += iterator.Current.Size;
                }
            }
            return size;
        }

        public long GetBundleSize(int priority)
        {
            long size = 0;
            if (mRemoteBundlesList != null)
            {
                List<BundleInfo>.Enumerator iterator = mRemoteBundlesList.GetEnumerator();
                while (iterator.MoveNext())
                {
                    if (priority < 0 || iterator.Current.Priority <= priority)
                    {
                        size += iterator.Current.Size;
                    }
                }
            }
            return size;
        }

        public List<string> GetDownLoadBundleNames()
        {
#if !USE_DOWNLOAD_AB
            return GetBundleNames();
#else
            List<string> names = new List<string>();

            if (mRemoteDownLoadBundlesList != null)
            {
                List<BundleInfo>.Enumerator iterator = mRemoteDownLoadBundlesList.GetEnumerator();
                while (iterator.MoveNext())
                {
                    if (!IsBuildBundle(iterator.Current.BundleName))
                    {
                        names.Add(iterator.Current.BundleName);
                    }
                }
            }

            return names;
#endif

        }

        public List<string> GetDownLoadBundleNames(int priority)
        {
#if !USE_DOWNLOAD_AB
            return GetBundleNames(priority);
#else
            
            List<string> names = new List<string>();

            if (mRemoteDownLoadBundlesList != null)
            {
                List<BundleInfo>.Enumerator iterator = mRemoteDownLoadBundlesList.GetEnumerator();
                while (iterator.MoveNext())
                {
                    if (priority < 0 || iterator.Current.Priority <= priority)
                    {
                        if (!IsBuildBundle(iterator.Current.BundleName))
                        {
                            names.Add(iterator.Current.BundleName);
                        }
                    }
                }
            }

            return names;
#endif
        }
        /// <summary>
        /// 远程的包信息列表
        /// </summary>
        private List<BundleInfo> mRemoteBundlesList = null;
        /// <summary>
        /// 目前一直都是空,都没有地方引用?
        /// </summary>
        private List<BundleInfo> mRemoteDownLoadBundlesList = null;
        /// <summary>
        /// 远程服务器的资源包名称对应的包信息
        /// </summary>
        private Dictionary<string, BundleInfo> mRemoteBundlesDict = new Dictionary<string, BundleInfo>();
        /// <summary>
        /// 远程服务器的资源名称对应的包信息
        /// </summary>
		private Dictionary<string, BundleInfo> mRemoteAssetsDict = new Dictionary<string, BundleInfo>();
		private int mRemoteBundlesCnt = -1;
		private bool isGetting = false;
		private bool isReady = false;
	}
}
