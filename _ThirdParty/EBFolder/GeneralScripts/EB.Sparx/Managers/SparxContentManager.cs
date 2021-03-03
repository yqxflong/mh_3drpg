using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace EB.Sparx
{
	/// <summary>
	/// Content manager.
	/// Manages downloading and updating content packs
	/// Packs are stored to the cache folder of the application
	/// </summary>
	public class ContentManager : System.IDisposable
	{
		class ContentPack
		{
			public string folder;
			public List<string> packs = new List<string>();
			public int version;
	
			public void Delete()
			{
				EB.Debug.Log("Deleting pack " + folder);
				try {
					Directory.Delete(folder, true);
				}
				catch {
				}
			}
		}
		
		private string _baseUrl;
		private Sparx.HttpEndPoint _endpoint;
		private List<ContentPack> _packs = new List<ContentPack>();
		private static ContentPack _mounted = null;
		
		// do we do content updates over cellular?
		public bool DownloadOverCarrierDataNetwork= true;
		public System.Action<string> ProgessCallback = null;
		
		// put the extract on its own thread
		private static ThreadPool _pool;
		
		public string Error = string.Empty;
		
		public bool IsMounted { get { return _mounted != null; } }
	
		public ContentManager( string baseUrl )
		{
			_baseUrl = baseUrl;
		}
		
		public Coroutine Check()
		{
			return EB.Coroutines.Run(DoCheck());
		}
		
		string BasePath
		{
			get
			{
#if UNITY_ANDROID
				return Path.Combine( Application.persistentDataPath, "Content");	
#else
				return Path.Combine( Application.temporaryCachePath, "Content");	
#endif
			}
		}
		
		ContentPack GetPack( int version )
		{
			foreach( var pack in _packs )
			{
				if ( pack.version == version )
				{
					return pack;
				}
			}
			return null;
		}
		
		void Mount( ContentPack pack )
		{
			if (_mounted == null)
			{
				_mounted = pack;
				
				foreach( string p in pack.packs )
				{
					Loader.OverridePath(p, "file://"+Path.Combine(pack.folder,p+Path.DirectorySeparatorChar)); 
				}
				
				// add the dlc packs to the assets manager
				Assets.AddDlcPacks( pack.packs.ToArray() );
				
				// delete everything else
				foreach ( var old in _packs )				
				{
					if (old.folder != pack.folder)
					{
						old.Delete();
					}
				}
				_packs.Clear();
			}
		}
		
		void Notify( string txt )
		{
			if ( ProgessCallback != null)
			{
				ProgessCallback(txt);
			}
		}
		
		bool CheckFile( string localpath, long size )
		{
			try {
				var info = new FileInfo(localpath);
				if ( info != null )
				{
					return info.Length == size;
				}
			}
			catch (System.Exception ex)
			{
				EB.Debug.Log("Failed to check file " + localpath + " " + ex.ToString() );
			}
			return false;
		}
		
		object ReadJsonFile( string path ) 
		{
			try {
				var bytes = File.ReadAllBytes(path);
				var info  = JSON.Parse( Encoding.GetString(bytes) ); 
				return info;
			}
			catch {
				EB.Debug.Log("failed to load json file at path: " + path);
			}
			return null;	
		}
		
		bool WriteJsonFile( string path, object data )
		{
			try
			{
				Directory.CreateDirectory( Path.GetDirectoryName(path) ); 
			}
			catch {}
			
			try 
			{
				var str= JSON.Stringify(data);
				File.WriteAllBytes( path, Encoding.GetBytes(str) );
				return true;
			}
			catch 
			{
				EB.Debug.Log("failed to write json file at path: " + path);
				return false;	
			}
		}
		
		IEnumerator DoCheck()
		{
			// enumerate local content
			Notify( Localizer.GetString("ID_SPARX_CONTENT_CHECKING") ); 
			yield return Coroutines.Run(DoEnumerate());
			
			// check server
			yield return Coroutines.Run(DoCheckServer());
			
			// mount if we haven't already
			if ( _mounted == null && _packs.Count > 0 )
			{
				Mount(_packs[0]);
			}
			
			Notify(string.Empty);
		}
		
		IEnumerator DoEnumerate()
		{
			string [] directories;
			_packs.Clear();
			
			try {
				directories = Directory.GetDirectories(BasePath,"*", SearchOption.TopDirectoryOnly);
			}
			catch {
				directories = new string[0];
			}
			
			foreach( var directory in directories)
			{
				var pack = new ContentPack();
				pack.folder = directory;
				yield return Coroutines.Run( DoVerify(pack) );
			}
			
			// sort highest version first
			_packs.Sort(delegate(ContentPack x, ContentPack y){
				return y.version.CompareTo(x.version);
			});
			
			EB.Debug.Log("Enumerated  " + _packs.Count + " packs " );
			
			yield break;
		}
		
		IEnumerator DoVerify( ContentPack pack )
		{
			var infoPath = Path.Combine(pack.folder, "info.txt");
			var info = ReadJsonFile(infoPath);
			
			if (info == null)
			{
				EB.Debug.Log("failed to load info file for content path: " + pack.folder);
				yield break;
			}
			
			yield return null;
			
			pack.version = Dot.Integer("cl", info, -1);
			foreach( var file in Dot.Array("files", info, new ArrayList() ))
			{
				pack.packs.Add( Dot.String("pack", file, string.Empty)); 
			}
			
			if (pack.version < Version.GetChangeList() )
			{
				// too old
				EB.Debug.LogWarning("pack is too old, deleting " + pack.version);
				pack.Delete();
				yield break;
			}
			
			if (pack.packs.Count == 0 )
			{
				EB.Debug.LogWarning("pack is empty, removing ");
				pack.Delete();
				yield break;
			}
			
			foreach( string p in pack.packs )
			{
				var folder = Path.Combine(pack.folder, p);
				if (!Verify(folder))
				{
					EB.Debug.LogWarning("pack verification failed for pack " + p);
					pack.Delete();
					yield break;
				}
			}
			
			EB.Debug.Log("content is ok at " + pack.folder);
			_packs.Add(pack);
			
			yield break;
		}
		
		bool Verify( string folder )
		{
			var filesPath   = Path.Combine(folder, "files.txt");
			ArrayList files = (ArrayList)ReadJsonFile(filesPath);;
			if (files == null)
			{
				return false;
			}
			
			foreach( var file in files )
			{
				var fn = Dot.String("0", file, string.Empty);
				var sz = Dot.Long("1", file, 0);
				
				var localPath = Path.Combine(folder, fn);
				
				if (!CheckFile(localPath, sz))
				{
					EB.Debug.LogError("failed verification on file  " + localPath);
					return false;
				}
			}
			return true;
		}
		
		public static Coroutine CheckForNewContent( System.Action<bool> callback ) 
		{
			if ( Application.internetReachability == NetworkReachability.NotReachable )
			{
				callback(false);
				return null;
			}
			
			var ep = SparxHub.Instance.ApiEndPoint;
			var request = ep.Get("/content");
			request.AddQuery("platform", Sparx.Device.MobilePlatform);
			request.AddQuery("cl",  Mathf.Max(1,EB.Version.GetChangeList()) );
			request.AddQuery("udid", Sparx.Device.UniqueIdentifier );
			request.AddQuery("check", 1);
			var co = ep.ServiceCoroutine(request, delegate(Response r){
				if (r.sucessful)
				{
					var version = Dot.Integer("cl", r.hashtable, 0);
					if ( version > 0 )
					{
						if ( _mounted == null || _mounted.version != version )
						{
							EB.Debug.Log("New Content Available!");
							callback(true);
							return;
						}
					}
				}
				callback(false);
			});
			return co;
		}
		
		IEnumerator DoCheckServer()
		{
			if ( string.IsNullOrEmpty(_baseUrl) )
			{
				yield break;
			}
			
#if UNITY_IPHONE
			if ( Application.internetReachability == NetworkReachability.NotReachable )
			{
				yield break;
			}
#endif
			
			if (!DownloadOverCarrierDataNetwork && Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork )
			{
				// todo: prompt?
				yield break;
			}
			
			_endpoint = new Sparx.HttpEndPoint(_baseUrl, new Sparx.EndPointOptions{ KeepAlive = false } );
			
			var request = _endpoint.Get("/content");
			request.AddQuery("platform", Sparx.Device.MobilePlatform);
			request.AddQuery("cl",  Mathf.Max(1,EB.Version.GetChangeList()) );
			request.AddQuery("udid", Sparx.Device.UniqueIdentifier );
			request.numRetries = 2;
			
			Response r = null;
			yield return _endpoint.ServiceCoroutine(request, delegate(Response result){
				r = result;
			});
			
			if (r.sucessful)
			{
				if (r.hashtable == null)
				{
					EB.Debug.Log("server has no content packs, removing old ones");
					_packs.Clear();
					yield break;
				}
				
				var version = Dot.Integer("cl", r.hashtable, 0);
				var pack = GetPack(version);
				if ( pack == null )
				{
					EB.Debug.Log("Downloading new pack " + version);
					yield return Coroutines.Run(DoDownload(r.hashtable));	
				}
				else
				{
					EB.Debug.Log("already have pack, mounting " + version);
					Mount(pack);
				}
				yield break;
			}
			else 
			{
				EB.Debug.Log("Check for content failed: " + r.localizedError);
				yield break;
			}
		}
		
		IEnumerator DoDownload( Hashtable info )
		{
			var version = Dot.Integer("cl", info, 0);
			var files = Dot.Array("files", info, null);
			if ( files == null || files.Count == 0 )
			{
				EB.Debug.LogError("missing file list!");
				yield break;
			}
			
			var pack = new ContentPack();
			pack.version = version;
			pack.folder = Path.Combine(BasePath,version.ToString());
			pack.packs = new List<string>();
			
			try 
			{
				Directory.CreateDirectory(pack.folder);	
			}
			catch (System.Exception ex)
			{
				EB.Debug.LogError("failed to create directory  " + pack.folder  + " " + ex);
				yield break;
			}
			
			// write the info file
			if (!WriteJsonFile( Path.Combine(pack.folder, "info.txt"), info)) 
			{
				yield break;
			}
		
			var cdn = Dot.String("cdn",info,string.Empty);
			
			using( var endpoint = new HttpEndPoint(cdn, new EndPointOptions() ) )
			{
				var filesToDownload = new ArrayList();
				
				foreach( var file in files ) 
				{
					var url = Dot.String("url", file, string.Empty);
					var size = Dot.Long("size", file, 0);
					var filename = Path.GetFileName(url);
					var localPath= Path.Combine(pack.folder, filename);
					if (!CheckFile(localPath, size))
					{
						filesToDownload.Add(file);
					}
					
					pack.packs.Add( Dot.String("pack", file, string.Empty) );
				}
				
				// download all the files
				for (int i = 0; i < filesToDownload.Count; ++i)
				{
					var file = filesToDownload[i];
					var url = Dot.String("url", file, string.Empty);
					var size = Dot.Long("size", file, 0);
					//var md5 = Dot.String("md5", file, string.Empty);
					
					var filename = Path.GetFileName(url);
					var localPath= Path.Combine(pack.folder, filename);
					
					// download
					yield return Coroutines.Run(DoDownloadFile(i, filesToDownload.Count, size, endpoint, localPath, url)); 
					if (!CheckFile(localPath, size))
					{
						EB.Debug.LogError("Failed to download file! " + url);
						yield break;
					}					
				}
			}
			
			Notify(Localizer.GetString("ID_SPARX_CONTENT_EXTRACTING"));
			yield return new WaitForFixedUpdate();
			
			if (_pool == null) 
			{
				_pool = new ThreadPool(1);
			}
			ThreadPool.AsyncTask async = _pool.Queue(this.ThreadTask,pack);

			while(!async.done)
			{
				yield return new WaitForFixedUpdate();
			}
			
			if (async.exception != null)
			{
				Error = "ID_SPARX_CONTENT_FAILED_EXTRACT";
				Notify(Localizer.GetString("ID_SPARX_CONTENT_FAILED_EXTRACT"));	
				yield break;
			}
						
			// mount it!
			Mount(pack);
			
			yield break;
		}
						
		private void ThreadTask( object packObj )
		{
			var pack = (ContentPack)packObj;
			if(pack != null)
			{
				// extract files
				foreach( var p in pack.packs ) 
				{
					var folder = Path.Combine(pack.folder, p);
					var localPath= Path.Combine(pack.folder, p+".tar.gz");
					if ( !Extract( folder, localPath) )
					{
						EB.Debug.LogError("Extraction failed!");
						throw new System.IO.IOException("Failed to extract bundle");
					}
				}
			}
		}
		
		bool Extract( string directory, string localPath ) 
		{
			EB.Debug.Log("extracting " + localPath + " to " + directory);
			try
			{
				var files = new ArrayList();

				var filestream = new FileStream(localPath, FileMode.Open, FileAccess.Read );
				{
					var gzipstream = new Pathfinding.Ionic.Zlib.GZipStream( filestream, Pathfinding.Ionic.Zlib.CompressionMode.Decompress );
					{
						var tarstream  = new tar_cs.TarReader(gzipstream);
						while (tarstream.MoveNext(false))
						{
							var fileInfo = tarstream.FileInfo;

							string totalPath    = Path.Combine(directory,fileInfo.FileName);
							string fileName     = Path.GetFileName(totalPath);
							string dir          = totalPath.Remove(totalPath.Length - fileName.Length);
							Directory.CreateDirectory(dir);

							EB.Debug.Log("extracting " + fileName);
							using (FileStream file = new FileStream(totalPath, FileMode.Create, FileAccess.Write))
							{
								tarstream.Read(file);
							}

							files.Add(new object[] { fileInfo.FileName, fileInfo.SizeInBytes });
						}
					}
				}

				// write the verification file
				if (!WriteJsonFile(Path.Combine(directory, "files.txt"), files))
				{
					return false;
				}

				return true;
			}
			catch (System.Exception ex)
			{
				EB.Debug.LogError("extraction failed " + ex);	
				return false;
			}
		}
	
		IEnumerator DoDownloadFile( int index, int count, long size, EndPoint endpoint, string localPath, string remoteUrl )
		{
			EB.Debug.Log("Do Download file " + remoteUrl);
			
			var uri = new Uri(remoteUrl);
			
			FileStream stream = null;
			var md5 = Digest.MD5();
			string etag = string.Empty;
			long downloaded = 0;
			
			EB.Debug.Log("downloading to  " + localPath);
			
			var onHeaders = (System.Action<Net.WebRequest>)delegate(Net.WebRequest r){
				if (r.statusCode != 200)
				{
					EB.Debug.LogError("NON-200 " + r.statusCode);
					return;
				}
			
				etag = r.GetResponseHeader("etag").Replace("\"", "").ToUpper();
				md5.Reset();
				downloaded = 0;
				
				EB.Debug.Log("etag: " + etag);
				
				try {
					Directory.CreateDirectory( Path.GetDirectoryName(localPath) );			
				}
				catch (System.Exception ex) {
					EB.Debug.Log("Failed to create directory " + ex.ToString());
				}
				
				try 
				{
					if (stream != null)
					{
						stream.Close();
						stream = null;
					}
					stream = new FileStream(localPath, FileMode.Create, FileAccess.ReadWrite);

				}
				catch (System.Exception ex)
				{
					EB.Debug.LogError("Failed to create file " + localPath + " " + ex);
				}
			};
			
			var onData = (System.Action<byte[]>)delegate(byte[] data)
			{
				try {
					if ( stream != null )
					{
						stream.Write(data, 0, data.Length);
						
						// add to md5
						md5.Update(data, 0, data.Length);
						//md5.TransformBlock(data, 0, data.Length, null, 0);
						
						downloaded += data.Length;
					}
				}
				catch (System.Exception ex) {
					EB.Debug.Log("Write Failed!! " + ex.ToString() );
					Error = "ID_SPARX_CONTENT_FAILED_EXTRACT";
					// re-throw
					throw ex;
				}
			};
			
			var request = endpoint.Get(uri.Path);
			request.dataCallback = onData;
			request.headersCallback = onHeaders;
			
			var id = endpoint.Service(request, delegate(Response r){
				if (stream != null)
				{
					stream.Flush();
					stream.Close();
				}
				
				if (r.sucessful)
				{
					// check md5
					//md5.TransformFinalBlock(new byte[0], 0, 0);
					//var hash = Encoding.ToHexString(md5.Hash);
					var digest = md5.Final();
					var hash = Encoding.ToHexString(digest);
					if (hash != etag)
					{
						EB.Debug.LogError("md5 failure " + hash + "!=" + etag);
					}
					else
					{
						return;
					}
				}				
				EB.Debug.LogError("download failed, removing file " + localPath);
				try {
					File.Delete(localPath);
				}
				catch {}
			});
			
			var waiter = new WaitForFixedUpdate();
			var info = "";
			while( string.IsNullOrEmpty(Error) == true )
			{
				var progress = (int)(100*(float)downloaded/(float)size);
				info = Localizer.Format("ID_SPARX_CONTENT_DOWNLOAD_PROGRESS", (index+1), count, progress );
				Notify(info);
				yield return waiter;
				
				if (endpoint.IsDone(id))
				{
					break;
				}
			}
			
			yield break;
		}
		
		#region IDisposable implementation
		public void Dispose ()
		{
			if (_endpoint != null)
			{
				_endpoint.Dispose();
				_endpoint = null;
			}
		}
		#endregion
	}	
}