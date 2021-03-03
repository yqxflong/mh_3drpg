#define USE_CACHE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

#if USE_CACHE
using System.IO;
#endif

namespace EB
{
	// a really simple cache
	// makes zero attempts to purge any data right now
	public static class Cache
	{
#if USE_CACHE
		private static List<UnityWebRequest> _ActivelyLoading = new List<UnityWebRequest>();
		private static List<UnityWebRequest> _DisposeOnRelease = new List<UnityWebRequest>();
		static string CacheFolder
		{
			get
			{
				return Path.Combine(Application.temporaryCachePath,"asset_cache");
			}
		}

		private static IEnumerator SaveToCache( string url, UnityWebRequest www )
		{
			while(www.isDone == false)
			{
				yield return null;
			}

			byte[] bytes = null;

			try 
			{
				if (string.IsNullOrEmpty(www.error))
				{
					bytes = www.downloadHandler.data;
				}
			}
			catch 
			{

			}

			if (bytes != null) 
			{
				SaveToCache( url, bytes );
			}
			
			_ActivelyLoading.Remove(www);
			bool dispose = _DisposeOnRelease.Remove( www );
			if( dispose == true )
			{
				www.Dispose();
			}
		}

		private static void SaveToCache( string url, byte[] bytes )
		{
			try 
			{
				if (Directory.Exists(CacheFolder)==false) 
				{
					Directory.CreateDirectory(CacheFolder);
				}

				var filename = GetCacheFile(url);
				File.WriteAllBytes(filename,  bytes);
				Debug.Log("Cached asset: {0}->{1}", url, filename);
			}
			catch (System.Exception ex)
			{
				Debug.LogError("Failed to write to cache folder! url:{0}, ex:{1}", url, ex);
			}
		}

		private static string GetCacheFile( string url ) 
		{
			var ext = Path.GetExtension(url);
			var name = Hash.StringHash(url);
			return Path.Combine( CacheFolder, name+ext); 
		}

		public static UnityWebRequest LoadFromCacheOrDownload(string url, out bool disposable)
		{
			disposable = true;
			if( url.StartsWith("jar:") == true )
			{
				return UnityWebRequest.Get(url); 
			}
			
			var uri = new Uri();
			if (!uri.Parse(url))
			{
				EB.Debug.LogError( "LoadFromCacheOrDownload Parse Failed {0}", url );
				return null;
			}

			if (uri.Scheme == "file")
			{
				return UnityWebRequest.Get(url);
            }

			// assume network
			var fileName = GetCacheFile(url);
			if ( File.Exists(fileName) )
			{
				var cachedPath = "file://"+fileName;
				return UnityWebRequest.Get(cachedPath);
            }

			// load it and save it to the cache
			var www = UnityWebRequest.Get(url);
            disposable = false;
			EB.Debug.Log( "Loading to Cache {0}", url );
			_ActivelyLoading.Add(www);
			Coroutines.Run(SaveToCache(url,www));
			return www;
		}
		
		public static void DisposeOnComplete(UnityWebRequest www)
		{
			if( www != null )
			{
				if( _ActivelyLoading.Contains( www ) == false )
				{
					EB.Debug.Log( "Cache Not Using Dispose" );
					www.Dispose();
				}
				else
				{
					EB.Debug.Log( "Cache Is Loading Defer Dispose" );
					_DisposeOnRelease.Add( www );
				}
			}
		}
#else
		public static WWW LoadFromCacheOrDownload(string url, out bool disposable)
		{
			disposable = false;
			return WWW(url);
		}
		
		public static void DisposeOnComplete(WWW www)
		{
		}
#endif

		public static void Precache(string url)
		{
			bool disposable = false;
			LoadFromCacheOrDownload(url, out disposable);
		}
		private static IEnumerator DoPreCache(string url, System.Action<string> cb)
		{
			bool disposable = false;
			var www = LoadFromCacheOrDownload(url, out disposable);
			yield return www.SendWebRequest();
			yield return null;
#if USE_CACHE	
			var fileName = GetCacheFile(url);
			if ( File.Exists(fileName) )
			{
				
				var cachedPath = "file://"+fileName;
				cb(cachedPath);
			}
			else
#endif
			{
				cb(url);
			}			
		}
		
		public static Coroutine Precache(string url, System.Action<string> cb)
		{
			return Coroutines.Run( DoPreCache(url,cb) );
		}
	}
	
}
