using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace GM
{
    /// <summary>
    /// 资源加载工具类(静态类)
    /// </summary>
	public static class AssetUtils
	{
		public static void DoAction(System.Action<bool> action, bool param1, GameObject target)
		{
			action?.Invoke(param1);
		}

		public static void DoAction<T>(System.Action<string, T, bool> action, string param1, T param2, bool param3, GameObject target) where T : UnityEngine.Object
		{
			action?.Invoke(param1, param2, param3);
		}

		public static void DoAction<T>(UniqueAssetActionWrapper<T> action, string assetname, T asset, bool success) where T : UnityEngine.Object
		{
			if (action?.Execute(assetname, asset, success) <= 0)
			{
				EB.Debug.LogError("Action[3] is Null! Asset is {0}, success is {1}", assetname, success);
			}
		}

		public static void DoAction<T>(InstantiatableAssetActionWrapper action, string assetname, UnityEngine.Object asset, bool success, bool isNeedInstantiate = true) where T : UnityEngine.Object
		{
			if (action?.Execute<T>(assetname, asset, success, isNeedInstantiate) <= 0)
			{
				EB.Debug.LogError("Action[4]  is Null! Asset is {0}, success is {1}", assetname, success);
			}
		}

		public static void DoAction(System.Action<List<string>, bool> action, List<string> param1, bool param2, GameObject target)
		{
			action?.Invoke(param1, param2);
		}

        /// <summary>
        /// 修正渲染材质
        /// </summary>
        /// <param name="obj"></param>
		public static void FixShaderInEditor(UnityEngine.Object obj)
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				UnityEngine.Object[] deps = UnityEditor.EditorUtility.CollectDependencies(new UnityEngine.Object[] { obj });
				List<UnityEngine.Object> objs = new List<Object>(deps);
				if (obj != null)
				{
					objs.Add(obj);
				}

				foreach (UnityEngine.Object asset in objs)
				{
					GameObject go = asset as GameObject;
					if (go != null)
					{
						Renderer[] skinnedMeshRenderer = go.GetComponentsInChildren<Renderer>(true);
						for (int ii = 0; ii < skinnedMeshRenderer.Length; ++ii)
						{
							for (int jj = 0; jj < skinnedMeshRenderer[ii].sharedMaterials.Length; ++jj)
							{
								Material myMaterial = skinnedMeshRenderer[ii].sharedMaterials[jj];
								if (myMaterial != null)
								{
									myMaterial.shader = Shader.Find(myMaterial.shader.name);
									skinnedMeshRenderer[ii].sharedMaterials[jj] = myMaterial;
								}
							}
						}

						UITexture[] texes = go.GetComponentsInChildren<UITexture>(true);
						for (int ii = 0; ii < texes.Length; ++ii)
						{
							if (texes[ii].material != null)
							{
								texes[ii].material.shader = Shader.Find(texes[ii].material.shader.name);
							}
							else if (texes[ii].shader != null)
							{
								texes[ii].shader = Shader.Find(texes[ii].shader.name);
							}
						}
					}

					Material mat = asset as Material;
					if (mat != null)
					{
						mat.shader = Shader.Find(mat.shader.name);
					}
				}

				//Shader.WarmupAllShaders();
			}
#elif UNITY_EDITOR
			if (Application.isPlaying)
			{
				EB.Assets.FixupEditorMaterials(obj);
			}
#endif
		}

		public static void ClearAssetsBundlesCache()
		{
			string path = AssetBundlePath;
			string localJson = LocalAssetBundleInfoFile;
			if (System.IO.File.Exists(localJson))
			{
				System.IO.File.Delete(localJson);
				EB.Debug.Log("Local json file cleared: {0}", localJson);
			}

			//clear Assets bundles
			if (System.IO.Directory.Exists(path))
			{
				System.IO.Directory.Delete(path, true);
				EB.Debug.Log("Assets cache cleared: {0}", path);
			}
		}

		public static void EmptyBundleCallback(string assetname, AssetBundle bundle, bool success)
		{
			EB.Debug.Log("EemptyBundleCallback: result = {0}, assetname = {1}", success, assetname);
		}

		private static string _assetBundlePath = System.IO.Path.Combine(Application.persistentDataPath, "AssetBundle");
        /// <summary>
        /// 本地可读取AssetBundle文件夹路径
        /// </summary>
		public static string AssetBundlePath
		{
			get
			{
				return _assetBundlePath;
			}
		}
        
        /// <summary>
        /// AppData/LocalLow/LostTemple/失落之城
        /// </summary>
		private static string _localAssetBundleInfoFile = System.IO.Path.Combine(Application.persistentDataPath, "LocalAssetBundleInfo.json");
        /// <summary>
        /// 本地资源包配置信息路径
        /// </summary>
		public static string LocalAssetBundleInfoFile
		{
			get
			{
				return _localAssetBundleInfoFile;
			}
		}

		private static string _remoteSuffix = ".cz";
		public static string RemoteBundleSuffix
		{
			get
			{
				return _remoteSuffix;
			}
		}

		private static string _suffix = ".assetbundle";
        /// <summary>
        /// 获取本地资源包的全路径
        /// </summary>
        /// <param name="bundlename"></param>
        /// <returns></returns>
		public static string GetFullBundlePath(string bundlename)
		{
			return System.IO.Path.Combine(AssetBundlePath, bundlename.ToLower() + _suffix);
		}
        /// <summary>
        /// 判断url是否为本地路径
        /// </summary>
        /// <param name="url">路径</param>
        /// <returns></returns>
		public static bool LoadFromLocalFile(string url)
		{
			return url.StartsWith("file://") || url.StartsWith("jar:file://");
		}

		public const string STREAMING_ASSETS_BUNDLE_FOLDER_NAME = "AssetBundles";
	}

	public class InstantiatableAssetActionWrapper
	{
		private class WrapperImpl
		{
			public object action;
			public GameObject target;
			public bool unload;
			public GameObject parent;
            public bool isParentNeed;

			public bool Execute<U>(string assetname, UnityEngine.Object asset, bool success, bool isNeedInstantiate = true) where U : UnityEngine.Object
			{
				if (target != null && action != null)
				{

					if (asset != null && success)
					{
                        if (!parent && isParentNeed)
                        {
                            ((System.Action<string, U, bool>)action)(assetname, null, false);
                        }
                        else
                        {
                            if (isNeedInstantiate)
                            {
                                U obj = (parent ? UnityEngine.Object.Instantiate(asset, parent.transform) : UnityEngine.Object.Instantiate(asset)) as U;
                                ((System.Action<string, U, bool>)action)(assetname, obj, true);
                                AssetUtils.FixShaderInEditor(obj);
                            }
                            else
                            {
                                ((System.Action<string, U, bool>)action)(assetname, asset as U, true);
                                AssetUtils.FixShaderInEditor(asset);
                            }
                        }
					}
					else
					{
						((System.Action<string, U, bool>)action)(assetname, null, false);
					}

					if (!success)
					{
						EB.Assets.UnloadAssetByName(assetname, false);
					}
					else if (unload)
					{
                        //针对战斗UI界面需要强制释放,放到：LTCombatHudController.ShowBattleResultScreenCoroutine()函数里边去强行释放
                        if (!assetname.Equals("CombatHudV4"))
                        {
                            EB.Assets.UnloadAssetByName(assetname, false);
                        }
					}

					return true;
				}

				EB.Assets.UnloadAssetByName(assetname, false);
				EB.Debug.LogWarning("Action is {0}! Target is {1} Param1 is {2}, param2 is {3}, param3 is {4}",
					action == null ? "NULL" : "NOT NULL",
					target == null ? "NULL" : "NOT NULL",
					assetname, asset, success);
				return false;
			}

			public bool Simualte()
			{
				return target != null && action != null;
			}

            public WrapperImpl(object action, GameObject target, bool unload, GameObject parent = null, bool isParentNeed = false)
			{
				this.action = action;
				this.target = target;
				this.unload = unload;
				this.parent = parent;
                this.isParentNeed = isParentNeed;
			}
		}

		private List<WrapperImpl> mActions = new List<WrapperImpl>();

        public InstantiatableAssetActionWrapper(object action, GameObject target, bool unload, GameObject parent = null, bool isParentNeed = false)
		{
			Push(action, target, unload, parent);
		}

        public void Push(object action, GameObject target, bool unload, GameObject parent = null, bool isParentNeed = false)
		{
            mActions.Add(new WrapperImpl(action, target, unload, parent, isParentNeed));
		}

		public int Simualte()
		{
			int result = 0;
			for (int i = 0; i < mActions.Count; ++i)
			{
				result += mActions[i].Simualte() ? 1 : 0;
			}
			return result;
		}

		public int Execute<T>(string param1, UnityEngine.Object param2, bool param3, bool isNeedInstantiate = true) where T : UnityEngine.Object
		{
			int result = 0;
			for (int i = 0; i < mActions.Count; ++i)
			{
				result += mActions[i].Execute<T>(param1, param2, param3, isNeedInstantiate) ? 1 : 0;
			}
			return result;
		}
	}

	public class UniqueAssetActionWrapper<T> where T : UnityEngine.Object
	{
		private class WrapperImpl<U>
		{
			public System.Action<string, U, bool> action;
			public GameObject target;

			public bool Execute(string param1, U param2, bool param3)
			{
				if (target != null && action != null)
				{
					action(param1, param2, param3);
					return true;
				}

				EB.Debug.LogWarning("Action is {0}! Target is {1} Param1 is {2}, param2 is {3}, param3 is {4}",
					action == null ? "NULL" : "NOT NULL",
					target == null ? "NULL" : "NOT NULL",
					param1, param2, param3);
				return false;
			}

			public bool Simualte()
			{
				return target != null && action != null;
			}

			public WrapperImpl(System.Action<string, U, bool> action, GameObject target)
			{
				this.action = action;
				this.target = target;
			}
		}

		private List<WrapperImpl<T>> mActions = new List<WrapperImpl<T>>();
		
		public UniqueAssetActionWrapper()
		{

		}

		public UniqueAssetActionWrapper(System.Action<string, T, bool> action, GameObject target)
		{
			Push(action, target);
		}

		public void Push(System.Action<string, T, bool> action, GameObject target)
		{
			mActions.Add(new WrapperImpl<T>(action, target));
		}

		public void Remove(GameObject target)
		{
			for (int i = mActions.Count - 1; i >= 0; --i)
			{
				if (mActions[i].target == target)
				{
					mActions.RemoveAt(i);
					return;
				}
			}
		}

		public void RemoveAll(GameObject target)
		{
			for (int i = mActions.Count - 1; i >= 0; --i)
			{
				if (mActions[i].target == target)
				{
					mActions.RemoveAt(i);
				}
			}
		}

		public void Clear()
		{
			mActions.Clear();
		}

		public int Simulate()
		{
			int result = 0;
			for (int i = 0; i < mActions.Count; ++i)
			{
				result += mActions[i].Simualte() ? 1 : 0;
			}
			return result;
		}

		public int Execute(string param1, T param2, bool param3)
		{
			int result = 0;
			for (int i = 0; i < mActions.Count; ++i)
			{
				result += mActions[i].Execute(param1, param2, param3) ? 1 : 0;
			}
			return result;
		}
	}

	#region 资源加载器,实现已改为CoreAssets
	public class AssetLoader<T> : CustomYieldInstruction where T : UnityEngine.Object
	{
		private bool? mSuccess;
		public bool Success
		{
			get { return mSuccess.HasValue && mSuccess.Value; }
		}

		public T Instance
		{
			get; private set;
		}

		public string AssetName
		{
			get; private set;
		}

		GameObject mTarget;

		public override bool keepWaiting
		{
			get { return !mSuccess.HasValue; }
		}

        public AssetLoader(string name, GameObject target)
        {
            AssetName = name;
            mSuccess = null;
			mTarget = target;
			// AssetManager.GetAsset<T>(name, LoadCallback, target, true, null, false);
			EB.Assets.LoadAsyncAndInit<T>(name, LoadCallback, target, null);
        }

        private void LoadCallback(string name, T instance, bool success)
		{
			mSuccess = success;

			if (success)
			{
				Instance = instance;
			}
			else
			{
				EB.Debug.LogWarning("AssetLoader.LoadCallback: failed to load asset {0}", name);
				Instance = null;
			}
		}
	}
	#endregion

	public class BundleLoader : CustomYieldInstruction
	{
		private bool? mSuccess;
		public bool Success
		{
			get { return mSuccess.HasValue && mSuccess.Value; }
		}

		public override bool keepWaiting
		{
			get { return !mSuccess.HasValue; }
		}

		private string mName;

		public AssetBundle Bundle
		{
			get; private set;
		}

		public BundleLoader(string name, GameObject target)
		{
			mSuccess = null;
			mName = name;
			EB.Assets.LoadBundle(name, LoadCallback, target);
		}

		private void LoadCallback(string name, AssetBundle bundle, bool success)
		{
			mSuccess = success;

			if (success)
			{
				Bundle = bundle;
			}
			else
			{
				EB.Debug.LogWarning("BundleLoader.LoadCallback: failed to load bundle {0}", name);
				Bundle = null;
			}
		}

		public void Unload()
		{
			if (Bundle != null)
			{
				Bundle = null;
				EB.Assets.UnloadBundle(mName);
			}
		}
	}

	public class BundleDownloader : CustomYieldInstruction, System.IDisposable
	{
		public static EB.ThreadPool threadPool = null;

		public bool isDone
		{
			get { return mDone; }
		}

		public float progress
		{
			get { return mLocal ? mWebRequest.downloadProgress : mRquest.Progress; }
		}

		public string error
		{
			get { return mError; }
		}

		public override bool keepWaiting
		{
			get { return !mDone; }
		}

		public bool decompressDone
		{
			get { return mDecompressDone.HasValue && mDecompressDone.Value; }
		}

		public long uncompressedSize
		{
			get { return mUncompressedSize; }
		}

		private UnityWebRequest mWebRequest;
		private HTTP.Request mRquest;
		private bool mDone;
		private bool mLocal;
		private string mUrl;
		private string mDestPath;
		private string mName;
		private string mError;
		private byte[] mBytes;
		private bool? mDecompressDone;
		private long mUncompressedSize;

		public BundleDownloader(string name, string url, string destpath)
		{
			mLocal = AssetUtils.LoadFromLocalFile(url);
			mUrl = url;
			mDestPath = destpath;
			mName = name;

			EB.Coroutines.Run(Send());
		}

		private IEnumerator Send()
		{
			EB.Debug.Log("Downloading asset bundle {0} from {1}", mName, mUrl);			

			if (mLocal)
			{
                mWebRequest = UnityWebRequest.Get(mUrl);
                yield return mWebRequest.SendWebRequest();

                mDone = true;
				if (mWebRequest.isHttpError || mWebRequest.isNetworkError)
				{
					EB.Debug.LogWarning("Failed to download asset bundle {0} from {1}, error: {2}", mName, mUrl, mWebRequest.error);
					mError = mWebRequest.error;
					yield break;
				}
				else
				{
					mBytes = mWebRequest.downloadHandler.data;
					EB.Debug.Log("Downloaded asset bundle {0} from {1}, {2}", mName, mUrl, mWebRequest.downloadedBytes);
				}
			}
			else
			{
				mRquest = new HTTP.Request("GET", mUrl);
				mRquest.acceptGzip = false;
				mRquest.useCache = false;
				mRquest.maximumRedirects = 2;
				yield return mRquest.Send();
				mDone = true;
				if (mRquest.exception != null)
				{
					mError = mRquest.exception.Message;
					EB.Debug.LogWarning("Failed to download asset bundle {0} from {1}, error: {2}", mName, mUrl, mRquest.exception.Message);
					yield break;
				}
				else
				{
					if (mRquest.response.status != 200)
					{
						mError = "Not 200 status error(" + mRquest.response.status + ")";
						EB.Debug.LogWarning("HTTP 404 error when download asset bundle {0} from {1}", mName, mUrl);
						yield break;
					}
					else
					{
						mBytes = mRquest.response.Bytes;
						EB.Debug.Log("Downloaded asset bundle {0} from {1}, {2}", mName, mUrl, mBytes.Length);
					}
				}
			}			

			if (mBytes == null || mBytes.Length == 0)
			{
				mError = "Invalid response data";
				EB.Debug.LogError("Downloaded asset bundle {0} error, invalid response data", mName);
			}
		}

		public void Save()
		{
			if (mDecompressDone.HasValue)
			{
				EB.Debug.LogWarning("Save: Bundle {0} already saved to {1}", mName, mDestPath);
				return;
			}

			mDecompressDone = false;
			DecompressWorkThread(this);
		}

		public void BeginSave()
		{
			if (mDecompressDone.HasValue)
			{
				EB.Debug.LogWarning("BeginSave: Bundle {0} already saved to {1}", mName, mDestPath);
				return;
			}

			mDecompressDone = false;
			if (threadPool == null)
			{
				EB.Debug.LogWarning("BundleDownloader.BeginSave: create thread pool, size = 1");
				threadPool = new EB.ThreadPool(8);
			}
			threadPool.Queue(DecompressWorkThread, this);
		}

		private static int downloadBytes;

		private static void DecompressWorkThread(object state)
		{
			BundleDownloader bdl = state as BundleDownloader;

			if (System.IO.File.Exists(bdl.mDestPath))
			{
				System.IO.File.Delete(bdl.mDestPath);
			}

			if (bdl.mBytes == null || bdl.mBytes.Length == 0)
			{
				EB.Debug.LogError("DecompressWorkThread: empty bytes for bundle {0}", bdl.mName);
				bdl.mError = "Decompress bundle failed, empty bytes";
				return;
			}

			try
			{
				using (System.IO.FileStream fs = new System.IO.FileStream(bdl.mDestPath, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.Write))
				{
					HTTP.Zlib.ZlibStream zo = new HTTP.Zlib.ZlibStream(fs, HTTP.Zlib.CompressionMode.Decompress);
					zo.Write(bdl.mBytes, 0, bdl.mBytes.Length);

					bdl.mUncompressedSize = zo.TotalOut;
					EB.Debug.LogWarning("Saved asset bundle {0} to {1}, Original Size:{2}, Uncompressed Size:{3}", bdl.mName, bdl.mDestPath, bdl.mBytes.Length, bdl.mUncompressedSize);

					zo.Flush();
					zo.Close();
					zo = null;

					fs.Close();
					fs.Dispose();

					if (bdl.mBytes != null)
					{
						downloadBytes += bdl.mBytes.Length;

						if (bdl.mBytes.Length > 10000000)
						{
							int gid = System.GC.GetGeneration(bdl.mBytes);
							bdl.mBytes = null;
							System.GC.Collect(gid);
						}
						else
						{
							bdl.mBytes = null;
						}
						if (downloadBytes > 30000000)
						{
							System.GC.Collect();
							downloadBytes = 0;
						}
					}
				}
			}
			catch (System.Exception ex)
			{
				bdl.mError = ex.Message;
				EB.Debug.LogError("Decompress bundle {0} error {1} stack {2}", bdl.mName, ex.Message, ex.StackTrace);
			}

			bdl.mDecompressDone = true;
		}

		public Coroutine WaitDecompress()
		{
			return EB.Coroutines.Run(WaitDecompressCoroutine());
		}

		private IEnumerator WaitDecompressCoroutine()
		{
			while (!decompressDone)
			{
				yield return null;
			}
		}

		public void Dispose()
		{
			if (mDecompressDone.HasValue && !mDecompressDone.Value)
			{
				// how to stop?
			}

			mBytes = null;

			if (mWebRequest != null)
			{
                mWebRequest.Dispose();
                mWebRequest = null;
			}

			if (mRquest != null)
			{
				mRquest = null;
			}
		}
	}
    
	/// <summary>
    /// AssetBundle资源包处理控件
    /// 带有引用计数
    /// </summary>
	public class AssetBundleHandle
	{
        /// <summary>
        /// 资源包名称
        /// </summary>
		public string bundleName;
        /// <summary>
        /// 资源包对象
        /// </summary>
		public AssetBundle assetBundle
        {
            get
            {
                return m_AssetBundle;
            }
            set
            {
                if (value != null)
                {
                    EB.Debug.LogAssetManager("<color=#00ffff>将这个资源包缓存:</color><color=#00ff00>{0}</color>", bundleName);
                }
                m_AssetBundle = value;
            }
        }

        private AssetBundle m_AssetBundle;
        /// <summary>
        /// 被引计数
        /// </summary>
		public int refCount;

		public AssetBundleHandle(string name, AssetBundle bundle)
		{
			bundleName = name;
			assetBundle = bundle;
			refCount = 1;
		}

        /// <summary>
        /// 被引用
        /// </summary>
        /// <returns></returns>
		public int Ref()
		{
			return ++refCount;
		}

        /// <summary>
        /// 去引用
        /// </summary>
        /// <returns>要是不没有引用了，返回true</returns>
		public bool UnRef()
		{
			if (--refCount <= 0)
			{
				if (refCount < 0)
				{
					EB.Debug.LogError("ReduceReference: refCount underflow {0}, assetbundle {1}", refCount, bundleName);
				}

				return true;
			}

			return false;
		}
	}
}
