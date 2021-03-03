using UnityEngine;
using System.Collections.Generic;
using System.Linq;
//using UnityEngine.Profiling;
#if UNITY_EDITOR || UNITY_IOS
using System.Runtime.Serialization.Json;
#endif

namespace GM
{
    /// <summary>
    /// 本地资源管理器
    /// </summary>
	public class LocalBundleManager
	{
        /// <summary>
        /// 初始化
        /// </summary>
		public void Initialize()
		{
			mReady = false;
			LoadLocalBundlesInfoFile(AssetUtils.LocalAssetBundleInfoFile);

			if (!System.IO.Directory.Exists(AssetUtils.AssetBundlePath))
			{
				System.IO.Directory.CreateDirectory(AssetUtils.AssetBundlePath);
			}
		}

		public bool IsReady
		{
			get { return mReady; }
		}

		/// <summary>
		/// Gets the bundle contains asset.
		/// </summary>
		/// <returns>The bundle contains asset.</returns>
		/// <param name="assetName">Asset name.</param>
		public BundleInfo GetBundleInfoContainsAsset(string assetName)
		{
			if (!mReady)
			{
				EB.Debug.Log("Attempt to get local bundle data which contains asset {0} before ready", assetName);
				return null;
			}

			var iter = mLocalBundlesDict.GetEnumerator();
			while (iter.MoveNext())
			{
				BundleInfo _bundleInfo = iter.Current.Value;
				if (_bundleInfo.Includes.Contains(assetName))
				{
					iter.Dispose();
					return _bundleInfo;
				}
			}
			iter.Dispose();

			return null;
		}

		/// <summary>
		/// Gets the bundle.
		/// </summary>
		/// <returns>The bundle.</returns>
		/// <param name="bundleName">Bundle name.</param>
		public BundleInfo GetBundleInfo(string bundleName)
		{
			if (!mReady)
			{
				EB.Debug.Log("Attempt to get local bundle data which name is {0} before ready", bundleName);
				return null;
			}

			if (mLocalBundlesDict.ContainsKey(bundleName))
			{
				return mLocalBundlesDict[bundleName];
			}

			return null;
		}

		/// <summary>
		/// Adds the local bundle info.
		/// If the local bundle info with the same bundle name exists, replace it.
		/// </summary>
		/// <param name="bundleInfo">Bundle info.</param>
		public void AddLocalBundleInfo(BundleInfo bundleInfo)
		{
			mLocalBundlesDict[bundleInfo.BundleName] = bundleInfo;

			// whenever download an assetbundle succeeded, save the bundle local file
			EB.Coroutines.ClearTimeout(mSaveHandle);
			mSaveHandle = EB.Coroutines.SetTimeout(SaveLocalBundlesInfoFile, 3000);

			EB.Assets.AssetBundleInfo bundle = new EB.Assets.AssetBundleInfo();
			bundle.packId = System.IO.Path.GetFileName(GM.AssetUtils.AssetBundlePath);
			bundle.id = bundleInfo.BundleName.ToLower();
			bundle.uncompressed = true;
			bundle.paths = bundleInfo.Paths.Where(include => !string.IsNullOrEmpty(include)).Select(include => include.Replace("Assets/Resources/", string.Empty).Replace("Assets/", string.Empty).ToLower());
			bundle.parent = bundleInfo.Parent;
			bundle.size = bundleInfo.Size;
			bundle.hash = bundleInfo.Version;
			EB.Assets.AddAssetBundleInfo(bundle);
		}

		/// <summary>
		/// Saves the local bundles info file.
		/// </summary>
		public void SaveLocalBundlesInfoFile(string path)
		{
			Debug.LogWarning("SaveLocalBundlesInfoFile!");
			//Profiler.BeginSample("Save Bundle (JsonData)");
			System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
			if (sw == null)
			{
				EB.Debug.LogError("Can not write to local bundles info file: {0}", path);
				return;
			}

			List<BundleInfo> infos = new List<BundleInfo>(mLocalBundlesDict.Values);
			string json = LitJson.JsonMapper.ToJson(infos);
			string jsonStr = JsonFormatter.PrettyPrint(json);

			sw.Write(jsonStr);
			sw.Flush();
			sw.Close();
			sw.Dispose();
			sw = null;

			infos.Clear();
			infos = null;
			json = null;
			jsonStr = null;

			System.GC.Collect();

			//Profiler.EndSample();
#if UNITY_IPHONE
			UnityEngine.iOS.Device.SetNoBackupFlag(path);
#endif
		}
        /// <summary>
        /// 加载本地资源包信息
        /// </summary>
        /// <param name="path">指定的路径</param>
		private void LoadLocalBundlesInfoFile(string path)
		{
			//Profiler.BeginSample("Load Bundle (JsonData)");
			string packId = System.IO.Path.GetFileName(AssetUtils.AssetBundlePath);
			EB.Loader.OverridePath(System.IO.Path.GetDirectoryName(AssetUtils.AssetBundlePath) + System.IO.Path.DirectorySeparatorChar);
			EB.Loader.OverridePath(packId, AssetUtils.AssetBundlePath + System.IO.Path.DirectorySeparatorChar);

			if (System.IO.File.Exists(path))
			{
				System.IO.StreamReader sr = new System.IO.StreamReader(path);
				
				if (sr == null)
				{
					EB.Debug.LogError("Can not read local bundles info file: {0}", path);
				}
				else
				{
					//Profiler.BeginSample("Initialize Bundle (JsonData)");
#if UNITY_EDITOR || UNITY_IOS
					DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(List<BundleInfo>));
					List<BundleInfo> localBundlesList = (List<BundleInfo>)deseralizer.ReadObject(sr.BaseStream);
#else
					List <BundleInfo> localBundlesList = LitJson.JsonMapper.ToObject<List<BundleInfo>>(sr.ReadToEnd());
#endif
					if (localBundlesList != null)
					{
                        EB.Debug.LogCoreAsset("<color=#00ff00>初始化Bundle资源数据, path : {0}, AssetBundlePath : {1}, localBundlesList.Count : {2}</color>", path, AssetUtils.AssetBundlePath, localBundlesList.Count);
                        for (int i = 0; i < localBundlesList.Count; ++i)
						{
							mLocalBundlesDict.Add(localBundlesList[i].BundleName, localBundlesList[i]);
	
							BundleInfo bundleInfo = localBundlesList[i];
							EB.Assets.AssetBundleInfo bundle = new EB.Assets.AssetBundleInfo();
							bundle.packId = packId;
							bundle.id = bundleInfo.BundleName.ToLower();
							bundle.uncompressed = true;
							bundle.paths = bundleInfo.Paths.Where(include => !string.IsNullOrEmpty(include)).Select(include => include.Replace("Assets/Resources/", string.Empty).Replace("Assets/", string.Empty).ToLower());
							bundle.parent = bundleInfo.Parent;
							bundle.size = bundleInfo.Size;
							bundle.hash = bundleInfo.Version;
							EB.Assets.AddAssetBundleInfo(bundle);
						}
					}
					//Profiler.EndSample();

					localBundlesList.Clear();
					localBundlesList = null;

					sr.Close();
					sr.Dispose();
					sr = null;
					System.GC.Collect();
				}
			}
			//Profiler.EndSample();
			mReady = true;
		}

		public void SaveLocalBundlesInfoFile()
		{
			SaveLocalBundlesInfoFile(AssetUtils.LocalAssetBundleInfoFile);

			EB.Coroutines.ClearTimeout(mSaveHandle);
			mSaveHandle = null;
		}
        /// <summary>
        /// 本地资源包名称及包信息
        /// </summary>
		private Dictionary<string, BundleInfo> mLocalBundlesDict = new Dictionary<string, BundleInfo>();
		private bool mReady = false;
		private object mSaveHandle = null;
	}
}