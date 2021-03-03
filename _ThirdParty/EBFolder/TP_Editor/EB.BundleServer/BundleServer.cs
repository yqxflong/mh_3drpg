using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace EB.BundleServer
{
	[InitializeOnLoad]
	public class BundleServer
	{
		static HttpListener _listener = null;
		static BundleServer _instance = null;
		
		public delegate void CountUpdate(int count);
		
		public CountUpdate CurrItemCountUpdated = null;
		public CountUpdate TotalCountUpdate = null;
		
		public bool StopBuild = false;

		private List<string> searchPaths = new List<string>();
		
		private IEnumerator runningProcess = null;
		
		static BundleServer()
		{
			if (_instance == null)
			{
				_instance = new BundleServer();			
			}
		}
		
		public static BundleServer Instance
		{
			get
			{
				System.Diagnostics.Debug.Assert(_instance != null);
				return _instance;
			}
		}
		
		BundleServer()
		{
			try
			{
				if (_listener == null)
				{
					_listener = new HttpListener();
					if (!UnityEditorInternal.InternalEditorUtility.inBatchMode)
					{
						UnityEngine.Debug.Log("BundleServer: Start at " + "http://*:" + BuildSettings.BundleServerPort + "/");
						_listener.Prefixes.Add("http://*:" + BuildSettings.BundleServerPort + "/");
						_listener.Start();
						_listener.BeginGetContext(GetContextCallback, _listener);
					}
				}
			}
			catch (System.Exception e)
			{
				UnityEngine.Debug.Log ("BundleServer: CAUGHT EXCEPTION E: " + e.Message);
			}
		}
		
		~BundleServer()
		{
			_listener.Close();
			_listener = null;
		}

		public void AddSearchPath(string path)
		{
			searchPaths.Add(path);
		}
		
		public void Update()
		{
			if (runningProcess != null)
			{
				if (StopBuild)
				{
					StopBuild = false;
					runningProcess = null;
				}
				else
				{
					if (!runningProcess.MoveNext())
					{
						//BuildUtils.ReturnResourceBundlesToResourcesDir();	
						runningProcess = null;
					}
				}
			}
		}
		
		public void GetContextCallback(System.IAsyncResult result)
		{
			HttpListener listener = (HttpListener) result.AsyncState;
			try
			{				
				if (listener != null && listener.IsListening)
				{				
					HttpListenerContext context = listener.EndGetContext(result);				
					HttpListenerRequest request = context.Request;
					HttpListenerResponse response = context.Response;

					UnityEngine.Debug.Log("Request: " + request.Url);
					string localPath = request.Url.LocalPath.ToLower();
					
					int startIndex = 0;
					int localPathLength = localPath.Length;
					if (localPath.Length > 0)
					{
						while(startIndex < localPathLength && localPath[startIndex] == '/')
						{
							startIndex++;
						}

						if (startIndex > 0)
						{
							localPath = localPath.Substring(startIndex);
						}
					}
					
					string buildFolder = BuildSettings.BaseBuildFolder;
					string localFile = Path.Combine(buildFolder, localPath);
					if (!File.Exists(localFile))
					{
						localFile = string.Empty;
						foreach (var path in searchPaths)
						{
							if (File.Exists(Path.Combine(path, localPath)))
							{
								localFile = Path.Combine(path, localPath);
								break;
							}
						}
					}

					if (!string.IsNullOrEmpty(localFile))
					{
						bool isAssetBundle = localFile.ToLower().EndsWith("assetbundle");

						UnityEngine.Debug.Log("BundleServer - sending: " + localPath);
						using (FileStream fs = File.OpenRead(localFile))
						{
							//response is HttpListenerContext.Response...
							response.ContentLength64 = fs.Length + (isAssetBundle ? EB.Loader.BundleCacheBlankHeaderSize : 0);
							response.SendChunked = false;
							response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
							response.AddHeader("Content-disposition", "attachment; filename="+localPath);
							response.StatusCode = (int)HttpStatusCode.OK;
							response.StatusDescription = "OK";        
					
							byte[] buffer = new byte[64 * 1024];
							int read;
							using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
							{
								if (isAssetBundle)
								{
									// need to add a fake header so that unity doesn't detect it's an asset bundle and block us from accessing it via WWW.bytes
									if (EB.Loader.BundleCacheBlankHeaderSize < 8 || EB.Loader.BundleCacheBlankHeaderSize % 8 != 0)
									{
										throw new System.Exception("EB.Assets.BundleCacheBlankHeaderSize must be > 8 bytes & divisible by 8");
									}
									for( int i = 0; i < EB.Loader.BundleCacheBlankHeaderSize/8; i++)
									{
										bw.Write(0L);
									}
								}
								while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
								{
									bw.Write(buffer, 0, read);
									bw.Flush();
								}
					
								bw.Close();
							}
						}
					}
					else
					{
						response.StatusCode = (int)HttpStatusCode.NotFound;
						response.Close();
					}
				}
			}
			catch (System.Exception e)
			{
				UnityEngine.Debug.LogError("Exception: " + e.Message);
				throw;
			}
			finally
			{
				if (listener != null)
				{
					listener.BeginGetContext(GetContextCallback, listener);
				}
			}
		}
	}	// BuildServer class
	
	[InitializeOnLoad]
	public class BundleServerWindow : EditorWindow
	{
		private int _currItem;
		private int _itemCount;
		private BundleServer _bundleServer;
		
		static string[] _bundleModeSelections = new string[] { "Use BundleServer Bundles", "Use No Bundles (Resources/Bundles)", "Use Standard Bundles" };
	
		[MenuItem("Window/Bundling Configuration")]
		public static void BundleServerWidget()
		{
			GetWindow(typeof(BundleServerWindow));	
		}
				
		private void ItemCountUpdated(int count) { _itemCount = count; }
		private void CurrItemCountUpdated(int count) { _currItem = count; }
		
		void OnEnable()
		{
			_bundleServer = BundleServer.Instance;
			_bundleServer.CurrItemCountUpdated += CurrItemCountUpdated;
			_bundleServer.TotalCountUpdate += ItemCountUpdated;
			
			BuildSettings.UpdateFromSettings();
		}
		
		void OnFocus()
		{
			BuildSettings.UpdateFromSettings();
		}
		
		void OnProjectChange()
		{
			BuildSettings.UpdateFromSettings();
		}
		
		public delegate bool OnListItemGUI( object item, bool selected, ICollection list );
		
		public static ArrayList SelectList( ICollection list, ICollection selected, OnListItemGUI itemHandler )
		{
			ArrayList itemList;
			ArrayList selectedList;
			ArrayList updatedSelectedList = new ArrayList();
 
			itemList = new ArrayList( list );
			selectedList = new ArrayList( selected );
 
			foreach( object item in itemList )
			{
				bool wasSelected = selectedList.Contains(item);
				if( itemHandler( item, wasSelected, list ) )
				{
					updatedSelectedList.Add(item);
				}
			}
 
			return updatedSelectedList;
		}
	 
		private bool OnCheckboxItemGUI( object item, bool selected, ICollection list )
		{
			return GUILayout.Toggle( selected, item.ToString() );
		}		
		
		void OnGUI ()
		{
			Color origColor = GUI.contentColor;
			
			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal();
			GUILayout.Space((Screen.width * 0.5f) - 55.0f);
			GUI.contentColor = Color.green;
			GUILayout.Label("Bundling Configuration", EditorStyles.boldLabel);
			GUILayout.EndHorizontal();
			
			GUI.contentColor = origColor;
			
			GUILayout.BeginHorizontal();			
			GUILayout.Label(string.Format("Progress: {0}/{1}", _currItem, _itemCount), EditorStyles.boldLabel );						
			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal();         
			GUILayout.Label(string.Format("Bundle Server Address:", _currItem, _itemCount), EditorStyles.boldLabel, new GUILayoutOption[] { GUILayout.Width(200) } );
			string bundleServerPrev = BuildSettings.BundleServerAddress;
			string updatedBundleServerAddress = GUILayout.TextField(BuildSettings.BundleServerAddress, 256, EditorStyles.boldLabel );
			if (bundleServerPrev != updatedBundleServerAddress)
			{
				string host = string.Empty;
				System.Uri bsURI;
				if (System.Uri.TryCreate(updatedBundleServerAddress, System.UriKind.RelativeOrAbsolute, out bsURI))
				{
					host = bsURI.Host;
				}
				BuildSettings.BundleServerAddress = "http://" + host + ":" + BuildSettings.BundleServerPort;
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal();
			int prevBundleMode = (int)BuildSettings.DevBundleMode;
			BuildSettings.DevBundleMode = (EB.Assets.DevBundleType)GUILayout.SelectionGrid(prevBundleMode, _bundleModeSelections, 1);			
			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();
					
			GUILayout.BeginVertical();			
			string[] buildOptionsNames = System.Enum.GetNames(typeof(BuildOptions));
			System.Array buildOptionsValues = System.Enum.GetValues(typeof(BuildOptions));
			int buildOptionsCount = buildOptionsNames.Length;
			
			BuildOptions prevBuildOptions = BuildSettings.Options;
			for (int i = 0; i < buildOptionsCount; i++)
			{
				string boName = buildOptionsNames[i];
				BuildOptions boValue = (BuildOptions)buildOptionsValues.GetValue(i);
				
				bool wasSet = EB.Flag.IsSet(prevBuildOptions, boValue);
				bool nowSet = GUILayout.Toggle(wasSet, boName);
				if (wasSet != nowSet)
				{
					BuildSettings.Options = EB.Flag.Set(BuildSettings.Options, boValue, nowSet);
				}
			}
			GUILayout.EndVertical();

			EditorGUILayout.Separator();

			GUILayout.BeginVertical();
			ArrayList prevDevLevel = BuildSettings.DevelopmentLevelsBuild;
			ArrayList available = new ArrayList(BuildSettings.GetScenesFromEditorSettings());
			BuildSettings.DevelopmentLevelsBuild = SelectList(available, prevDevLevel, OnCheckboxItemGUI);			
			GUILayout.EndVertical();
			
			bool levelsChanged = false;
			if (prevDevLevel.Count != BuildSettings.DevelopmentLevelsBuild.Count)
			{
				levelsChanged = true;
			}
									
			if ((BuildSettings.BundleServerAddress != bundleServerPrev)
				|| levelsChanged || (prevBundleMode != (int)BuildSettings.DevBundleMode))
			{
				BuildSettings.SaveSettings();
			}
		}
		
		public void Update()
		{
			if (_bundleServer != null)
			{
				_bundleServer.Update();
			}
		}
	}
}

