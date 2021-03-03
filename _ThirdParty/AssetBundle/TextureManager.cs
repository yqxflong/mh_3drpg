using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GM
{
	public partial class TextureManager : MonoBehaviour
	{
		private string sDefaultTextureaName = "UI/Textures/Misc/MISSING";

		private static TextureManager _instance = null;
		public static TextureManager Instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject _go = new GameObject("~TEXTUREMANAGER");
					_go.hideFlags = HideFlags.HideAndDontSave;
					GameObject.DontDestroyOnLoad(_go);
					_instance = _go.AddComponent<TextureManager>();
					_instance.Initialize();
				}

				return _instance;
			}
		}

		public static Texture2D DefaultTexture2D
		{
			get
			{
				return Instance.mDefaultTexture2D;
			}
		}

		public static void GetTexture2DAsync(string texName, System.Action<string, Texture2D, bool> action, GameObject target)
		{
			Instance.GetTexture2DAsyncImpl(texName, action, target);
		}

		public static void ReleaseTexture(string texName)
		{
			Instance.ReleaseTextureImpl(texName);
        }

		public static void RemoveReference(string texName, GameObject target)
		{
			if (Instance.mLoadingTextures.ContainsKey(texName))
			{
				Instance.mLoadingTextures[texName].Remove(target);
			}
		}

		#region private
		private void GetTexture2DAsyncImpl(string texName, System.Action<string, Texture2D, bool> action, GameObject target)
		{
			if (mCachedTexture2DDict.ContainsKey(texName))
			{
				AssetUtils.DoAction<Texture2D>(action, texName, mCachedTexture2DDict[texName], true, target);
			}
			else
			{
#if UNITY_EDITOR
				EB.Assets.LoadAsyncAndInit<Texture2D>(texName, action, target, null, false);
#else
				EB.Coroutines.Run(DownloadTexture2DCoroutine(texName, action, target));
#endif
			}
		}

		private void ReleaseTextureImpl(string texName)
		{
			if (mCachedTexture2DDict.ContainsKey(texName))
			{
				EB.Debug.Log("ReleaseTextureImpl: release texture {0}", texName);
				Resources.UnloadAsset(mCachedTexture2DDict[texName]);
				mCachedTexture2DDict.Remove(texName);
                EB.Assets.UnloadAssetByName(texName, true);
			}
			else if (mLoadingTextures.ContainsKey(texName))
			{
				EB.Debug.LogWarning("ReleaseTextureImpl: texture {0} is loading", texName);
				mLoadingTextures.Remove(texName);
			}
		}

		private void Initialize()
		{
			mCachedTexture2DDict.Clear();
			if (!string.IsNullOrEmpty(sDefaultTextureaName))
			{
				mDefaultTexture2D = Resources.Load(sDefaultTextureaName, typeof(Texture2D)) as Texture2D;
				if (mDefaultTexture2D == null)
				{
					EB.Debug.LogWarning("Initialize: null default texture2d {0}", sDefaultTextureaName);
				}
			}
		}
#endregion

#region coroutine
		IEnumerator DownloadTexture2DCoroutine(string texName, System.Action<string, Texture2D, bool> action, GameObject target)
		{
			string bundlename = AssetManager.GetBundleNameContainsAsset(texName);
			if (string.IsNullOrEmpty(bundlename))
			{
				EB.Debug.LogWarning("DownloadTexture2DCoroutine: bundle not found for texture {0}", texName);
				AssetUtils.DoAction<Texture2D>(action, texName, null, false, target);
				yield break;
			}

			if (AssetManager.IsLocalBundleOutdated(bundlename))
			{
				yield return AssetManager.DownloadAssetBundle(bundlename);

				if (target == null)
				{
					EB.Debug.LogWarning("DownloadTexture2DCoroutine: target is null after download assetbundle {0} {1}, stop callback", texName, bundlename);
					yield break;
				}
			}

			// ensure add bundle reference by texName
			if (mLoadingTextures.ContainsKey(texName))
			{
				//EB.Debug.Log("DownloadTexture2DCoroutine: texture is loading {0}, push callback action", texName);
				mLoadingTextures[texName].Push(action, target);
				yield break;
			}

			Texture2D _tex = null;			
			if (!mCachedTexture2DDict.TryGetValue(texName, out _tex) || _tex == null)
			{
				UniqueAssetActionWrapper<Texture2D> wrapper = new UniqueAssetActionWrapper<Texture2D>(action, target);
				mLoadingTextures.Add(texName, wrapper);

				GM.BundleLoader loader = new GM.BundleLoader(bundlename, gameObject);
				if (loader.keepWaiting)
				{
					yield return loader;
				}
				
				if (!loader.Success)
				{
					EB.Debug.LogError("DownloadTexture2DCoroutine: load bundle {0} error", bundlename);
					mLoadingTextures.Remove(texName);
					AssetUtils.DoAction<Texture2D>(mLoadingTextures[texName], texName, null, false);
					yield break;
				}

				AssetBundleRequest waitasset = loader.Bundle.LoadAssetAsync(texName);
				if (!waitasset.isDone)
				{
					yield return waitasset;
				}
				
				_tex = waitasset.asset as Texture2D;
				if (_tex != null)
				{
					EB.Debug.Log("DownloadTexture2DCoroutine: loaded texture {0}", texName);
					if (!mCachedTexture2DDict.ContainsKey(texName))
					{
						mCachedTexture2DDict.Add(texName, _tex);
					}
					else if (mCachedTexture2DDict[texName] != _tex)
					{
						EB.Debug.LogError("DownloadTexture2DCoroutine: texture instance is different for {0}", texName);
						ReleaseTextureImpl(texName);
						mCachedTexture2DDict.Add(texName, _tex);
					}
				}

				if (!mLoadingTextures.ContainsKey(texName))
				{// already unload, stop callback
					EB.Debug.LogWarning("DownloadTexture2DCoroutine: texture released before loaded {0}, stop callback", texName);
					if (_tex != null)
					{
						ReleaseTextureImpl(texName);
					}
					yield break;
				}

				if (mLoadingTextures[texName] != wrapper)
				{// async load > release > async load problem
					EB.Debug.Log("DownloadTexture2DCoroutine: ABA problem {0}, stop callback", texName);
					if (_tex != null)
					{
						ReleaseTextureImpl(texName);
					}
					yield break;
				}

				int refCount = mLoadingTextures[texName].Simulate();
				if (refCount <= 0)
				{// nothing to execute
					EB.Debug.LogWarning("DownloadTexture2DCoroutine: nothing to callback {0}", texName);
					if (_tex != null)
					{
						ReleaseTextureImpl(texName);
					}
					mLoadingTextures.Remove(texName);
					yield break;
				}

				if (_tex != null)
				{
					AssetUtils.DoAction<Texture2D>(mLoadingTextures[texName], texName, _tex, true);
				}
				else
				{
					EB.Debug.LogWarning("DownloadTexture2DCoroutine: Get none texture {0} from bundle {1}", texName, bundlename);
					AssetUtils.DoAction<Texture2D>(mLoadingTextures[texName], texName, null, false);
				}

				mLoadingTextures.Remove(texName);
			}
			else
			{
				AssetUtils.DoAction<Texture2D>(action, texName, _tex, true, target);
			}
		}
#endregion coroutine

		private Dictionary<string, Texture2D> mCachedTexture2DDict = new Dictionary<string, Texture2D>();
		private Dictionary<string, UniqueAssetActionWrapper<Texture2D>> mLoadingTextures = new Dictionary<string, UniqueAssetActionWrapper<Texture2D>>();
		private Texture2D mDefaultTexture2D = null;
	}
}
