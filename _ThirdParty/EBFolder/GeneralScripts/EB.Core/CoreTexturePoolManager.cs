//#define TEXTUREPOOL_SPEW
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class TexturePoolManager : MonoBehaviour 
{
	static TexturePoolManager _this;
	public static TexturePoolManager Instance { get { return _this; } } 

	class TextureInfo
	{
		public enum eTextureSource
		{
			Resources,
			Other
		};

		public bool successfullyLoaded = false;
		public string textureName = null;
		public Texture2D texture = null;	
		public float time = 0;
		public List<System.Action<bool, Texture2D>> callbacks = new List<System.Action<bool, Texture2D>>();
		public int refCount = 0;
		public eTextureSource source = eTextureSource.Other;
		
		public int size
		{
			get
			{
				if ( texture != null )
				{
					return texture.width*texture.height;
				}
				return 0;
			}
		}
		
		public void DoCallbacks()
		{
			while (callbacks.Count>0)
			{
				var cb = callbacks[callbacks.Count-1];
				callbacks.RemoveAt(callbacks.Count-1);
				if ( cb != null )
				{
					cb(successfullyLoaded, texture);
				}
			}
		}
	}
	
#if UNITY_IPHONE
	const int 			kMaxConcurrentRequests  = 4;
	const int 			kPoolSize  	 			= 16;
	const int 			kGCAfterEvictions		= 4;
#else
	const int 			kMaxConcurrentRequests 	= 4;
	const int 			kPoolSize  	 			= 48;
	const int 			kGCAfterEvictions		= 16;
#endif

	const int 			kRetryIntervals			= 5;

	int					_concurrentRequests 	= 0;
	int					_evictions				= 0;
	
	List<TextureInfo> 	_all = new List<TextureInfo>();
	EB.Collections.Queue<string> 	_loads = new EB.Collections.Queue<string>();

	private List<WaitForSeconds> _waits = new List<WaitForSeconds>(kRetryIntervals);

	void Awake() 
	{
		_this = this;
	}

	public override string ToString()
	{
		string msg = "";
		for (int i = 0; i < _all.Count; ++i)
		{
			TextureInfo info = _all[i];
			msg += string.Format("{0} x{1} ({2})\n", info.textureName, info.refCount, info.size);
		}
		return msg;
	}

	void Start()
	{
		for (int i = 0; i < kRetryIntervals; i++)
		{
			_waits.Add(new WaitForSeconds(0.5f * i));
		}
	}

	void Update()
	{
		if (_loads.Count > 0)
		{
			if (_concurrentRequests < kMaxConcurrentRequests)
			{
				var textureName = _loads.Dequeue();
				var info = GetTextureInfo(textureName);
				if (info != null && info.texture == null)
				{
					_concurrentRequests++;
					StartCoroutine(ProcessNextTexture(info));
				}
			}
		}
	}
	
	private TextureInfo GetTextureInfo( string textureName )
	{
		for (int i = 0; i < _all.Count; ++i)
		{
			TextureInfo info = _all[i];

			if (info.textureName == textureName)
			{
				return info;
			}
		}

		return null;
	}
	
	private void Print()
	{
#if TEXTUREPOOL_SPEW
		for (int i = 0; i < _all.Count; ++i)
		{
			TextureInfo info = _all[i];

			EB.Debug.Log("{0}:{1},{2},{3}", info.textureName, info.size, info.time, info.refCount); 
		}
#endif					
	}
	
	private void Evict()
	{
		var dead = new List<TextureInfo>();
		for (int i = 0; i < _all.Count; ++i)
		{
			TextureInfo info = _all[i];
			if ( info.refCount <= 0 )
			{
				dead.Add(info);
			}
		}
		
		if ( dead.Count > kPoolSize )
		{
			// sort by size, then time
			dead.Sort( delegate(TextureInfo ti1, TextureInfo ti2 ){
				if ( ti1.size == ti2.size )
				{
					if ( ti1.time > ti2.time )
					{
						return 1;
					}
					else if ( ti1.time < ti2.time )
					{
						return -1;
					}
					return 0;
				}
				return ti2.size - ti1.size;
			});
			
			Print();
		
			int max = dead.Count - kPoolSize;
			for ( int i = 0; i < dead.Count && i < max; ++i )
			{
				var info = dead[i];
				// evict

				// Only release textures that were loaded properly
				if (info.successfullyLoaded)
				{
					// If we loaded the texture from resources, make sure we UNLOAD it
					if (info.source == TextureInfo.eTextureSource.Resources)
					{
						EB.Assets.Unload(info.texture);
					}
					else
					{
                        EB.Debug.LogCoreAsset("<color=#ff0000>XXX清除掉图片:{0}</color>", (info.texture == null) ? "空的图片引用" : info.texture.name);
                        Destroy(info.texture);
					}
				}

				_all.Remove(info);
				++_evictions;
			}
		}
		
		if ( _evictions >= kGCAfterEvictions )
		{
			_evictions = 0;
			//EB.Assets.UnloadUnusedAssets();
		}
		
	}
	
	public static string GetUrl(string texture)
	{
		if ( texture.StartsWith("http://") || texture.StartsWith("https://") || texture.StartsWith("file://") )
		{
			return texture;
		}
		return EB.Assets.FindDiskAsset(texture);
	}
	

	/// <summary>
	/// 异步加载Texture
	/// </summary>
	/// <param name="info"></param>
	/// <returns></returns>
	IEnumerator ProcessNextTexture( TextureInfo info )
	{				
		string url = GetUrl(info.textureName);

		// If we didn't find an url and our textures are prepended with our
		// Bundles/UI/ path, strip it and look for the path again
		if (string.IsNullOrEmpty(url))
		{
			url = GetUrl(info.textureName.Replace("Bundles/UI/", string.Empty));
		}

#if TEXTUREPOOL_SPEW
		EB.Debug.Log("TexturePoolManager - LOADING " + url);
#endif
		
		// evict any old textures before moving on
		Evict();

        UnityWebRequest textureLoader = null;
		bool disposable = true;
		EB.Uri uri = new EB.Uri();
		if (uri.Parse(url))
		{
			for( int i = 0; i < kRetryIntervals; ++i )
			{	
				textureLoader = EB.Cache.LoadFromCacheOrDownload(url, out disposable);
				if (textureLoader == null ) 
				{
					break;
				}

				yield return textureLoader.SendWebRequest();
				
				if ( string.IsNullOrEmpty(textureLoader.error) )
				{
					break;
				}
				else if (url.StartsWith("file://"))
				{
					// file error are going to make a difference with a retry
					break;
				}
				
#if UNITY_EDITOR
				// Since all the texture are in Resources folder when running in editor, we don't want to delay this
				if (Application.isEditor)
				{
					break;
				}
#else
				EB.Debug.LogWarning("COULD NOT FIND TEXTURE: " + url);
				yield return _waits[i];
#endif		
			}
		}
		
		if ( Application.isEditor )
		{
			yield return new WaitForSeconds(0.15f);
		}
			
		if ( textureLoader != null && (textureLoader.isHttpError || textureLoader.isNetworkError))
		{
			// If we loaded from cache or file, we want to mark the textures properly
			// so they are destroyed or released properly later on
			if (textureLoader.url.StartsWith("file://"))
			{
				info.source = TextureInfo.eTextureSource.Resources;
			}

			info.texture = DownloadHandlerTexture.GetContent(textureLoader);
			info.texture.wrapMode = TextureWrapMode.Clamp;
			info.texture.name = info.textureName;
			info.successfullyLoaded = true;
			info.DoCallbacks();
		
#if TEXTUREPOOL_SPEW	
			EB.Debug.LogWarning("ON-DEMAND LOAD FROM {0} THERE ARE NOW {1} IN THE POOL", url, _all.Count);
#endif
		}
		else
		{			
			// Texture wasn't found in the bundle - Look for it in the resources folder only if in editor
			// Resources load requires no extension present in the path name
			string strippedPath = info.textureName.Replace(System.IO.Path.GetExtension(info.textureName), string.Empty);
			info.texture = Resources.Load(strippedPath) as Texture2D;
			if (info.texture == null)
			{
				EB.Debug.LogWarning("Texture not found at '{0}' or '{1}'", info.textureName, strippedPath);
			}
			else
			{
				info.texture.wrapMode = TextureWrapMode.Clamp;
				info.texture.name = info.textureName;
				info.source = TextureInfo.eTextureSource.Resources;
				info.successfullyLoaded = true;
			}

			info.DoCallbacks();
		}
		
		if( textureLoader != null ) 
		{
			if( disposable == true )
			{
				textureLoader.Dispose();
			}
			else
			{
				EB.Cache.DisposeOnComplete( textureLoader );
			}
		}

		--_concurrentRequests;
	}
	
	public void ClearPool(bool forceGarbageCollect = false)
	{
#if TEXTUREPOOL_SPEW
		EB.Debug.Log("Clearing pool!");
#endif
		for (int i = 0; i < _all.Count; ++i)
		{
			TextureInfo info = _all[i];
            EB.Debug.LogCoreAsset("<color=#ff0000>XXX清除掉图片:{0}</color>", (info.texture == null) ? "空的图片引用" : info.texture.name);
            Destroy(info.texture);
		}		
		_all.Clear();
		_evictions = 0;
		
		//if (forceGarbageCollect && EB.Assets.RequiresUnloadAssets())
		//{
		//	EB.Assets.UnloadUnusedAssets();
		//}
	}
	
	public void ReleaseTexture( string textureName )
	{
		if (string.IsNullOrEmpty(textureName))
		{
			return;
		}
		
		var info = GetTextureInfo(textureName);
		if ( info != null )
		{
			info.refCount--;
#if TEXTUREPOOL_SPEW			
			EB.Debug.Log("Dec refcount for textue: {0}, ref count: {1}", textureName, info.refCount );
#endif
		}
	}

	public void LoadTexture(string textureName, MonoBehaviour obj, System.Action<bool, Texture2D> cb )
	{		
		LoadTexture(textureName, EB.SafeAction.Wrap(obj,cb) ); 
	}
	
	private void LoadTexture(string textureName, System.Action<bool, Texture2D> cb)
	{
#if TEXTUREPOOL_SPEW
		EB.Debug.Log("Trying to load texture " + textureName);
#endif
		if (string.IsNullOrEmpty(textureName))
		{
			EB.Debug.LogWarning("Attempt to load empty texture.");
			return;
		}
		
		var info = GetTextureInfo(textureName);
		if ( info != null )
		{
			info.refCount++; // inc the refcount
			info.time = Time.realtimeSinceStartup;
			
			if ( info.texture != null )
			{
				if ( cb != null )
				{
					cb(info.successfullyLoaded, info.texture);
				}
			}
			else
			{
				info.callbacks.Add(cb);
				_loads.Enqueue(textureName);
			}
			return;
		}
		
		// create new info
		info = new TextureInfo();
		info.textureName = textureName;
		info.refCount = 1;
		info.time = Time.realtimeSinceStartup;
		info.callbacks.Add(cb);
		_all.Add(info);
		_loads.Enqueue(textureName);
	}
	
}
