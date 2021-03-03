//#define AUTO_ADB_SYNC_ANDROID 

using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EB.BundleServer;
using System.Linq;

public static class AndroidUtils
{
	public static string SDK = string.Empty;
	public static string NDK = string.Empty;
	public static string JDK = string.Empty;
	public static string BUILD_TOOLS = string.Empty;

	static AndroidUtils()
	{
#if UNITY_ANDROID
#if UNITY_EDITOR_WIN
		SDK = ScanSDKRoot();
		NDK = Path.Combine(SDK, "ndk-bundle");

		string javaPath = EnvironmentUtils.Get("PROGRAMFILES", "C:\\Program Files") + "\\Java";
		string jdkDirectory = GetJavaInstallationPath();
        var jdkTrims = jdkDirectory.TrimEnd(new char[] { '\n' }).Split(new char[] { ' ', '\t', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (jdkTrims != null && jdkTrims.Length > 0)
        {
            JDK = Path.Combine(javaPath, jdkTrims.Last());
            CommandLineUtils.AddSearchPath(Path.Combine(JDK, "bin"));
        }
#endif

#if UNITY_EDITOR_OSX
		SDK = Path.Combine(EnvironmentUtils.Get("HOME", "/System"), "Library/Android/sdk");
		NDK = Path.Combine(SDK, "ndk-bundle");
		JDK = "/System/Library/Frameworks/JavaVM.framework/Versions/Current";
#endif

		string[] tools = Directory.GetDirectories(Path.Combine(SDK, "build-tools"));
		if (tools != null && tools.Length > 0)
		{
			BUILD_TOOLS = tools[tools.Length - 1];
		}

		Debug.Log("SDK: " + SDK);
		Debug.Log("NDK: " + NDK);
		Debug.Log("JDK: " + JDK);
		Debug.Log("BUILD_TOOLS: " + BUILD_TOOLS);
#endif
	}

#if UNITY_EDITOR_WIN
	private static string ScanSDKRoot()
	{
		string[] search = new string[]
		{
			Path.Combine(EnvironmentUtils.Get("LOCALAPPDATA", EnvironmentUtils.Get("SYSTEMDRIVE", "C:")), "Android/sdk"),
			"E:\\sdk",
			"E:\\Android\\sdk",
			"D:\\sdk",
			"D:\\Android\\sdk",
            "D:\\Android\\android-sdk",
			"E:\\adt-bundle-windows-x86_64-20140702\\sdk",
			"C:\\Users\\35946\\AppData\\Local\\Android\\Sdk",
			"D:\\sdk\\sdk",
		};
		foreach (string path in search)
		{
			string root = FindSDKRoot(path);
			if (!string.IsNullOrEmpty(root))
			{
				return root;
			}
		}

		// return [A:\\, B:\\]
		string[] drives = System.Environment.GetLogicalDrives();
		foreach (string drive in drives)
		{
			string root = FindSDKRoot(drive);
			if (!string.IsNullOrEmpty(root))
			{
				return root;
			}
		}

		Debug.LogWarning("ScanSDKRoot: sdk not found");
		return string.Empty;
	}

	private static string FindSDKRoot(string path)
	{
        string mintty = Path.Combine(path, "tools/android.bat");
		//string mintty = Path.Combine(path, "SDK Manager.exe");
		if (File.Exists(mintty))
		{
			return path;
		}

		if (!Directory.Exists(path))
		{
			return string.Empty;
		}

		foreach (string sub in Directory.GetDirectories(path))
		{
			if (Path.GetFileName(sub).ToLower().Contains("sdk"))
			{
				string result = FindSDKRoot(sub);
				if (!string.IsNullOrEmpty(result))
				{
					return result;
				}
			}
		}

		return string.Empty;
	}
#endif

	public static void BuildPlayer( string apkPath, BuildOptions options )
	{
		BuildSettings.Options = options;
#if USE_XINKUAISDK
        PlayerSettings.Android.keystorePass = "xinyou";
		PlayerSettings.Android.keyaliasName = "xinyou";
		PlayerSettings.Android.keyaliasPass = "xinyou";
		PlayerSettings.Android.keystoreName = "Assets/Editor/Android/key.keystore";

#else
        PlayerSettings.Android.keystorePass = "123456";
        PlayerSettings.Android.keyaliasName = "ebg";
        PlayerSettings.Android.keyaliasPass = "123456";
        PlayerSettings.Android.keystoreName = "Assets/Editor/Android/android.keystore";
#endif
       EB.Debug.Log("Building to " + apkPath);
		
		string[] originalScenes = BuildSettings.GetScenesFromEditorSettings();
		string[] buildScenes = originalScenes;
		
		if (BuildSettings.IsDevelopmentBuild)
		{		
			if (BuildSettings.DevBundleMode == EB.Assets.DevBundleType.BundleServer)	// only the basic levels are built in to the player, others are separate bundles for smaller packages while debugging
			{
				List<string> filteredScenes = new List<string>();
				foreach ( string scene in originalScenes )
				{
					string lowerScene = scene.ToLower();
					foreach (string playerBuilt in BuildSettings.PlayerBuiltInScenes)
					{
						if (lowerScene.Contains(playerBuilt))
						{
							filteredScenes.Add(scene);
							Debug.Log("Player Building scene: "+scene);
							break;
						}
					}
				}
				buildScenes = filteredScenes.ToArray();
			}
		}
		
		try {
			if (BuildSettings.DevBundleMode != EB.Assets.DevBundleType.NoBundles)
			{
				Debug.Log("Moving Resources/Bundles out of Assets/ to build Player");
				BuildUtils.HideBundlesDirs();
			}
			
			BuildPipeline.BuildPlayer( buildScenes, apkPath, BuildTarget.Android, options );  
		} finally {
			if (BuildSettings.DevBundleMode != EB.Assets.DevBundleType.NoBundles)
			{
				Debug.Log("Returning Resources/Bundles to Assets/ after building Player");
				BuildUtils.UnHideBundlesDirs();
			}
		}
		
		if (!File.Exists(apkPath))
		{
			throw new System.Exception("Failed to build apk!");
		}	
	}
	
	public static bool HasDevicesConnected()
	{
		string sdk = SDK;
		var res = CommandLineUtils.Run(Path.Combine(sdk, "platform-tools/adb"), "devices");
		var index = res.IndexOf("List of devices attached");
		if (index >= 0)
		{
			var endl = res.IndexOf('\n', index+1);
			if (endl >= 0)
			{
				var result = false;
				var devices = res.Substring(endl+1).Split('\n');
				foreach( var device in devices )
				{
					if (device.Contains("device") )
					{
						Debug.Log("device: " + device.Split(' ')[0] );
						result = true;
					}
				}
				return result;
			}
		}
		return false;
	}
	
	public static void Uninstall( string bundleIdentifier )
	{
		string cmd = Path.Combine(SDK, "platform-tools/adb");
		var res = CommandLineUtils.Run(cmd, "uninstall " + bundleIdentifier );
		Debug.Log(res);
	}
	
	public static void Install( string apk )
	{
		string cmd = Path.Combine(SDK, "platform-tools/adb");
		var res = CommandLineUtils.Run(cmd, "install " + apk );
		Debug.Log(res);
	}

	public static void Reinstall( string apk )
	{
		string cmd = Path.Combine(SDK, "platform-tools/adb");
		var res = CommandLineUtils.Run(cmd, "install -r " + apk );
		Debug.Log(res);
	}
			
#if UNITY_ANDROID

	public struct FileTimeEntry
	{
		public string 	filePath;
		public long		fileTime;
		public FileTimeEntry(string path, long time)
		{
			filePath = path;
			fileTime = time;
		}
	}

	public static void SyncFilesToSDCard(string localDir, string remoteSdcardDir)
	{
		System.Diagnostics.Debug.Assert(Directory.Exists(localDir));
		
		string adb = Path.Combine(SDK, "platform-tools/adb");

		Hashtable fileToTimestampHash = new Hashtable();
		
		if (!remoteSdcardDir.StartsWith("/"))
		{
			//remoteSdcardDir = remoteSdcardDir.Substring(1);
			remoteSdcardDir += "/";
		}
		if (remoteSdcardDir.EndsWith("/"))
		{
			remoteSdcardDir = remoteSdcardDir.Substring(0, remoteSdcardDir.Length - 1);
		}
		if (!localDir.EndsWith("/"))
		{
			//localDir = localDir.Substring(0, localDir.Length - 1);
			localDir += "/";
		}		
				
		bool foundSdCardDir = false;
		
		foreach (string sdcardPath in EB.Loader.SDCardPaths)
		{
			string remotePath = sdcardPath + remoteSdcardDir;
			int remotePathLength = remotePath.Length;
					
			string res = CommandLineUtils.Run(adb, "shell ls -laR " + sdcardPath + "/Android/data/" );
			
			string[] lines = res.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries );
			
			if (lines.Length == 1)
			{
				Debug.Log("adb listing failed: "+lines[0]);
				continue;
			}
			else
			{
				foundSdCardDir = true;
				
				res = CommandLineUtils.Run(adb, "shell mkdir -p " + remotePath);	// make sure the directory exists
				
				if (res != "")
				{
					Debug.LogError("Error creating remote dir: "+remotePath+" error: "+res);
				}
				
				
				res = CommandLineUtils.Run(adb, "shell ls -laR " + remotePath );
				
				lines = res.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries );
				
			
				string dirPath = null;
				foreach( string line in lines )
				{
					if (line.StartsWith("/"))
					{
						dirPath = line.Substring(remotePathLength);
						if (dirPath.EndsWith(":"))
						{
							dirPath = dirPath.Substring(0, dirPath.Length -1);
						}
						if (dirPath.StartsWith("/"))
						{
							dirPath = dirPath.Substring(1);
						}
						if (dirPath.Length > 0 && !dirPath.EndsWith("/"))
						{
							dirPath += "/";
						}
					}
					else
					{
						if (line.StartsWith("d"))
						{
							//skip directories
							continue;
						}
						string[] lineElems = line.Split(new char [] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
						string dateTimeString = string.Format("{0} {1}", lineElems[4], lineElems[5]);
						System.DateTime fileTime = System.DateTime.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm", null);
						string fileName = lineElems[6];
						
						string relFilePath = dirPath+fileName;
						string fullFilePath = remotePath+"/"+dirPath+fileName;
						FileTimeEntry entry = new FileTimeEntry(fullFilePath, fileTime.Ticks);
						
						fileToTimestampHash.Add(relFilePath, entry);	// using add because a hash collision would mean bad things so we want to know about it
						
					}
				}

				
				string[] localFiles = Directory.GetFiles(localDir, "*", SearchOption.AllDirectories);
				
				int localDirLength = localDir.Length;
				
				foreach( string localFile in localFiles )
				{
					string localRelPath = localFile.Substring(localDirLength);
					
					long localTime = System.IO.File.GetLastWriteTime(localFile).Ticks;
					
					bool updateFile = true;
					if (fileToTimestampHash.ContainsKey(localRelPath))
					{
						FileTimeEntry entry = (FileTimeEntry)fileToTimestampHash[localRelPath];

						if (localTime <= entry.fileTime)
						{
							updateFile = false;
						}
						fileToTimestampHash.Remove(localRelPath);
					}
					if (updateFile)
					{
						string adbArgs = "push "+localDir+localRelPath+" "+remotePath+"/"+localRelPath;
						Debug.Log("Updating file : adb "+adbArgs);
						string result = CommandLineUtils.Run(adb, adbArgs);
						if (!string.IsNullOrEmpty(result))
						{
							Debug.LogError("FAILED: adb "+adbArgs+" : ERROR: "+result);
						}
					}
				}
				
				for(var iter = fileToTimestampHash.GetEnumerator(); iter.MoveNext();)
				{
					FileTimeEntry curr = (FileTimeEntry)iter.Value;
					string adbArgs = "shell rm -f "+curr.filePath;
					Debug.Log("Removing file : adb "+adbArgs);
					string result = CommandLineUtils.Run(adb, adbArgs);
					if (!string.IsNullOrEmpty(result))
					{
						Debug.LogError("FAILED: adb "+adbArgs+" : ERROR: "+result);
					}
				}
				
				break;
			}
		}
		if (!foundSdCardDir)
		{
			Debug.LogError("Couldn't find remote directory in /mnt/sdcard (or variants of that) "+remoteSdcardDir);
		}
	}	
	
	[MenuItem("Builder/Legacy/Helper/Sync Android Files")]
	public static void SyncAndroidFiles()
	{
		System.DateTime startSync = System.DateTime.Now;
		SyncFilesToSDCard(BuildSettings.BuildFolder, "/Android/data/"+PlayerSettings.applicationIdentifier+"/files/");
		Debug.Log("Sync Files Time: "+(System.DateTime.Now - startSync).ToString()+" seconds.");
	}
	
	[MenuItem("Builder/Legacy/Build Data Only")]
	public static void BuildDataOnly()
	{
		BuildDataOnly(BuildOptions.Development | BuildOptions.UncompressedAssetBundle);
		if (BuildSettings.DevBundleMode == EB.Assets.DevBundleType.BundleServer) // relaunch the game if we built data for a bundleserver build.
		{
			LaunchGameAndroid();
		}
	}
	
	public static void BuildDataOnly(BuildOptions options)
	{
		try 
		{
			BuildSettings.Options = options;
			Bundler.BuildAll(BuildSettings.BundlerConfigFolder, Bundler.BundleOptions.None | Bundler.BundleOptions.Extended | Bundler.BundleOptions.Uncompressed);
		}
		finally
		{
			
		}
#if AUTO_ADB_SYNC_ANDROID
		if (BuildSettings.DevBundleMode == EB.Assets.DevBundleType.BundleServer)
		{
			System.DateTime startSync = System.DateTime.Now;
			SyncFilesToSDCard(BuildSettings.BuildFolder, "/Android/data/"+PlayerSettings.bundleIdentifier+"/files/");
			Debug.Log("Sync Files Time: "+(System.DateTime.Now - startSync).ToString()+" seconds.");
		}
#endif
	}

	// Just quickly checking that we've bundled at least once. not checking in depth (at least for now)
	public static void BundlesBuiltSanityCheck()
	{
		// make sure we are in the right state for bundling.
		//UnHideBundlesDirs();
		Bundler.BuildAll(BuildSettings.BundlerConfigFolder, Bundler.BundleOptions.None | Bundler.BundleOptions.Extended | Bundler.BundleOptions.Uncompressed | Bundler.BundleOptions.BuildNotExistsOnly);
	}

	[MenuItem("Builder/Legacy/Build And Run")]
	public static void BuildAndRun()
	{
		try 
		{
			//open the bundle server window for configuration
			BundleServerWindow.BundleServerWidget();
			
			if (BuildSettings.DevBundleMode != EB.Assets.DevBundleType.NoBundles)
			{
				BuildDataOnly(BuildOptions.Development | BuildOptions.UncompressedAssetBundle);
			}
	
			BuildCodeAndRun();
		}
		finally
		{
			
		}
	}

    [MenuItem("Builder/Legacy/Get Java Installation Path")]
    public static string GetJavaInstallationPath()
    {
        string jdkparent = @"C:/Program Files/Java";
        string jdk = @"C:/Program Files/Java/jdk";
        DirectoryInfo Dir = new DirectoryInfo(jdkparent);
        foreach (DirectoryInfo d in Dir.GetDirectories())//查找子目录 
        {
            if (d.Name.StartsWith("jdk"))
            {
                return d.FullName;
            }
        }

        Debug.LogError("not install jdk");
        return "";
    }



    [MenuItem("Builder/Legacy/Build Code Only And Run")]
	public static void BuildCodeAndRun()
	{
		try
		{
			//open the bundle server window for configuration
			if (BuildSettings.DevBundleMode != EB.Assets.DevBundleType.NoBundles)
			{
				BundlesBuiltSanityCheck();
			}

			BundleServerWindow.BundleServerWidget();
			BuildOptions options = BuildSettings.Options | BuildOptions.Development | BuildOptions.UncompressedAssetBundle;
			options = EB.Flag.Unset(options, BuildOptions.AutoRunPlayer);
			BuildPlayer( Directory.GetCurrentDirectory() +  "/android.apk", options);
			LaunchGameAndroid();
		}
		finally
		{
			
		}
	}
	
	[MenuItem("Builder/Uninstall Android Game")]
	public static void UnInstallApk()
	{
		Uninstall( PlayerSettings.applicationIdentifier );
	}

	[MenuItem("Builder/Launch Android Game")]
	public static void LaunchGameAndroid()
	{
		Debug.Log("LaunchGameAndroid");

		string adb = Path.Combine(SDK, "platform-tools/adb");
		CommandLineUtils.Run(adb, "shell am force-stop "+ PlayerSettings.applicationIdentifier);
		string res = CommandLineUtils.Run(adb, "shell am start "+ PlayerSettings.applicationIdentifier + "/com.unity3d.player.UnityPlayerNativeActivity");
		Debug.Log(res);
	}
#endif
}
