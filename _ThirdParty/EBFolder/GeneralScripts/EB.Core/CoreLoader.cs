using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

namespace EB
{
	public static class Loader
	{	
		public static int BundleCacheBlankHeaderSize = 1024;	
		
		#if UNITY_ANDROID
		public static string[] SDCardPaths = { "/mnt/sdcard", "/storage/emulated/legacy", "/storage/sdcard0", "/storage/emulated/0", "/sdcard" };
		#endif
		
		public class CachedLoadHandler : YieldInstruction //IEnumerable
		{			
			private string 		_assetPath;
			private string 		_origURI;
			private AssetBundle _assetBundle = null;
			
			
			public AssetBundle assetBundle
			{
				get 
				{
					if (_assetBundle != null) 
					{
						return _assetBundle;
					}
					return null;
				}
			}
			
			public CachedLoadHandler(string filePath, bool useCache, long reqTimeStamp = 0, bool suppressErrors = false)
			{
				if (filePath.IndexOf("://") != -1)
				{
					_origURI = filePath;
				}
				else
				{
					_origURI = "file://"+filePath;
				}
				string uriString ="://";
				
				int uriIndex = filePath.ToLower().IndexOf(uriString);
				int skipIndex = (uriIndex == -1) ? 0 : uriIndex + uriString.Length;
				_assetPath = filePath.Substring(skipIndex);
				_assetPath = Path.GetFullPath(_assetPath); // normalize (remove .. etc)
				//Debug.Log("filepath:"+filePath+" _origURI:"+_origURI+" _assetPath:"+_assetPath);
			}
			

			#region Bundle Load Impl
			///纯Bundle缓存
			private Dictionary<string, AssetBundle> BundleCache = new Dictionary<string, AssetBundle>();

			///同步加载Bundle
			public AssetBundle AssetBundleLoadFromFile(string path)
			{
				path=path.Replace('/','\\');
				if (BundleCache.ContainsKey(path))
				{
					if (BundleCache[path] == null)
					{
						AssetBundle load = AssetBundle.LoadFromFile(path);
						BundleCache[path] = load;
					}
					return BundleCache[path];
				}
				else
				{
					AssetBundle load = AssetBundle.LoadFromFile(path);
					BundleCache.Add(path, load);
					return load;
				}
			}

			///异步加载Bundle
			public IEnumerable AssetBundleLoadFromFileAsync(string path, System.Action<AssetBundle> act)
			{
				path = path.Replace('/','\\');
				var request = AssetBundle.LoadFromFileAsync(path);
				yield return request;
				BundleCache[path] = request.assetBundle;
				act?.Invoke(BundleCache[path]);
			}
			#endregion

			/// <summary>
			/// 同步加载AsseetBundle的接口
			/// </summary>
			/// <param name="path"></param>
			/// <returns></returns>
			public IEnumerator LoadAssetBundle(string path)
			{
                EB.Debug.LogCoreAsset("加载指定的路径资源包:<color=#00ff00>{0}</color>", path);
#if UNITY_ANDROID
				if( path.StartsWith("jar:") )
				{
					//Debug.Log( "Trying to load from Jar: " + path );
					path = path.Replace("jar:file://", "");
					string[] parts = path.Split( '!' );
					if( parts.Length == 2 )
					{
						string jarPath = parts[ 0 ];
						string filePath = parts[ 1 ];
						if( filePath.StartsWith("/") == true )
						{
							filePath = filePath.Substring( 1 );
						}
						string outputPath = System.IO.Path.Combine( Application.persistentDataPath, System.IO.Path.GetFileName( filePath ) );
						
						bool success = false;
						yield return EB.Coroutines.Run( JarExtractor.SyncLoadFromJar( jarPath, filePath, /* info.size */ -1 , outputPath, /* info.hash */ -1, delegate(bool s) { success = s; } ) );
						
						if( success == true )
						{
                            _assetBundle = AssetBundleLoadFromFile(outputPath);
                        }
					}
				} 
				else
#endif
				{
					path = System.IO.Path.GetFullPath(path.Replace("file://", "")); // normalize the path
					_assetBundle = AssetBundleLoadFromFile(path);
                }

				Debug.Log("LoadAssetBundle.CreateFromFile result: "+(_assetBundle == null ? "FAILED":"SUCCESSFUL"));
				yield break;
			}
			
			
			public Coroutine Load()
			{
				return EB.Coroutines.Run(DoLoad());
			}
			
			private IEnumerator DoLoad()
			{
				bool isAssetBundle = _assetPath.ToLower().EndsWith("assetbundle");

				if (isAssetBundle)
				{
					//Debug.Log("Attempting to load assetbundle @"+_origURI);
					Coroutine coroutine = EB.Coroutines.Run(LoadAssetBundle(_origURI));
					if (_assetBundle == null)
					{
						yield return coroutine;
					}
				}
			}
		};		
	
		public static string Folder
		{
			get
			{
#if UNITY_IPHONE
				return "ios/";
#elif UNITY_ANDROID
				return "android/";
#else
				return "data/";			
#endif
			}
		}
		
		private static string _dataPath = string.Empty;
		private static Dictionary<string,string> _packs = new Dictionary<string,string>();
		
		public static string DataPath
		{
			get
			{
				if ( string.IsNullOrEmpty(_dataPath) )
				{
					if (Application.platform == RuntimePlatform.Android)
					{
						_dataPath = "jar:file://" + Application.dataPath + "!/assets/";
					}
					else if (Application.platform == RuntimePlatform.IPhonePlayer)
					{
						_dataPath = "file://" + Path.GetFullPath(Application.dataPath + "/" + Folder);
					}
					else if (Application.platform == RuntimePlatform.OSXPlayer)
					{
						_dataPath = "file://" + Path.GetFullPath(Application.dataPath + "/../../" + Folder);
					}
					else
					{
						_dataPath = "file://" + Path.GetFullPath(Application.dataPath + "/../" + Folder);
					}
				}
				return _dataPath;
			}
		}

		public static void OverridePath( string dp )
		{
			_dataPath = dp;
		}
		
		public static void OverridePath( string pack, string path )
		{
			_packs[pack] = path;
		}
		
		public static string GetBaseURL( string pack )
		{
			var path = string.Empty;
			if (_packs.TryGetValue(pack, out path))
			{
				return path;
			}
			return DataPath + pack + "/";
		}		
		
		public static string GetBundlePath( string bundleId )
		{
			string folder = string.Empty;
			string file = bundleId;
			
			int slash = bundleId.LastIndexOf('/');
			if ( slash >= 0 )
			{
				folder = bundleId.Substring(0,slash+1);
				file = bundleId.Substring(slash+1);
			}
			
			string path = string.Format("{0}{1}.assetbundle", folder, file);
			return path;
		}
		
		public static string GetSceneBundleName(string sceneName)
		{
			string path = string.Format("scene_{0}.assetbundle", sceneName);
			return path;
		}
		
		public static string GetBundlePath(string pack, string path)
		{
			var url = Path.Combine(GetBaseURL(pack),path);
			return url;
		}
	}
}

