using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EB.Perforce;

[InitializeOnLoad]
[ExecuteInEditMode]
public static class BuildSettings 
{
	// use lower case only names here. if listed here in build settings, these will not be built into bundles but always packaged with the player for development builds
	public static string[] PlayerBuiltInScenes = new string[] { };

	#region Class Functions
	public static BuildTarget Target		{ get { return EditorUserBuildSettings.activeBuildTarget; } set { EditorUserBuildSettings.SwitchActiveBuildTarget(value); }  }
	public static string BuildFolder 		{ get { return GetBuildFolder(Target); } }
	public static string PlatformFolder 	{ get { return GetPlatformFolder(Target); } }
	public static BuildOptions Options 		{ get { return _buildOptions; } set { _buildOptions = value; } }
	
	public static ArrayList DevelopmentLevelsBuild = new ArrayList();
	
	public static string BundleServerPort = "9091";
	public static string BundleServerAddress = "http://127.0.0.1:"+BundleServerPort;
		
	public static EB.Assets.DevBundleType DevBundleMode = EB.Assets.DevBundleType.NoBundles;
	
	public static bool IsDevelopmentBuild
	{ 
		get 
		{
			return (BuildSettings.Options & BuildOptions.Development) != 0; 
		}
		set
		{
			if (value)
			{
				_buildOptions |= BuildOptions.Development;
			}
			else
			{
				_buildOptions &= ~BuildOptions.Development;
			}
		}
	}
	private static BuildOptions _buildOptions = BuildOptions.None | BuildOptions.Development;	// defaulting to development build.
	
	private static string _buildConfigPath = null;
	public static string BuildConfigPath
	{
		get 
		{ 
			if (_buildConfigPath == null)
			{
				_buildConfigPath = Path.Combine(Application.dataPath, "Resources/BuildConfig.txt");			
			}
			return _buildConfigPath;
		}
	}	
	
	public static string BaseBuildFolder
	{
		get
		{
#if UNITY_IPHONE
			// ios doesn't support different build folders with the current project file system
			return System.IO.Directory.GetCurrentDirectory();
#else
			return EnvironmentUtils.Get("BUILD_FOLDER", System.IO.Directory.GetCurrentDirectory() );
#endif
		}
	}
	
	public static string GetBuildVersion( string cl )
	{
		var current = PlayerSettings.bundleVersion;
		var parts = new List<string>(current.Split('.'));
		if (parts.Count > 0)
		{
			parts[parts.Count-1] = cl;
		}
		return EB.ArrayUtils.Join(parts, '.');
	}
	
	public static string GetPlatformFolder( BuildTarget target )
	{
		var folder = "out";
		
		switch(target)
		{
		//case BuildTarget.WebPlayer:
		//case BuildTarget.WebPlayerStreamed:
		//	folder = "web"; break;
		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			folder = "data"; break;
		case BuildTarget.iOS:
			folder = "ios"; break;
		case BuildTarget.Android:
			folder = "android"; break;
		}
		return folder;
	}
	
	public static string GetBuildFolder( BuildTarget target )
	{
		return System.IO.Path.Combine(BaseBuildFolder, GetPlatformFolder(target));
	}
	
	public static string[] GetScenesFromEditorSettings()
	{
		List<string> scenes= new List<string>();
		
		foreach( var scene in EditorBuildSettings.scenes )
		{
			if ( scene.enabled && System.IO.File.Exists(scene.path) && !scenes.Contains(scene.path) )
			{
				scenes.Add( scene.path );
			}
		}
		
		return scenes.ToArray();
	}
	
	// moko: added a 'toString' method to get the buildsetting details. a bit abused the name 'toString' ;-P
	public static new string ToString()	
	{
		System.Type buildType = typeof(BuildSettings);
		string result = string.Format("Class: {0} [{1}]", buildType.Name, buildType.AssemblyQualifiedName);
		var flags = System.Reflection.BindingFlags.NonPublic
			|System.Reflection.BindingFlags.Public
			|System.Reflection.BindingFlags.Static
			|System.Reflection.BindingFlags.Instance;			
		System.Reflection.PropertyInfo[] props = buildType.GetProperties(flags);
		foreach (var p in props)
		{
			result += string.Format("\nProperty: {0}.{1} [{2}] = {3}", buildType.Name, p.Name, p.PropertyType.ToString(), p.GetValue(null, null));
		}	
		System.Reflection.FieldInfo[] fields = buildType.GetFields(flags);
		foreach (var f in fields)
		{
			result += string.Format("\nField: {0}.{1} [{2}] = {3}", buildType.Name, f.Name, f.FieldType.ToString(), f.GetValue(null));
		}	
		return result;
	}
	#endregion

	#region Project Specific Settings	
	public const string ProjectName = "ManHuangJi";

	public const string Version =
		#if UNITY_IPHONE
				"0.1.6";
		#elif UNITY_ANDROID
				"0.1.6";
		#else
				"0.1.6";
		#endif
	
	// iOS deployment settings
	public const string ProfileAdHoc = "67a0de34-cff8-48ae-91b6-fe6294370042";
	public const string ProfileSubmission = "d566833c-b448-4f3f-b6f6-cd4367335214";
	
	public const string CertDist	 = "'iPhone Developer: tang baobiao (6PF59CK3XQ)'";
	public const string CertSubmission = "'iPhone Distribution: tang baobiao (C6S7QQJ99N)'";
	
	public const string BundleIdentifier = "org.manhuang.mhj";

	public const string SuccessEmail = "huangzhijun@manhuang.org";

	// bundles
	public const string BundlerConfigFolder 			= "Assets/_GameAssets/Res/MISC/Config";
	public const Bundler.BundleOptions BundlerOptions 	= Bundler.BundleOptions.Force | Bundler.BundleOptions.SkipOgg | Bundler.BundleOptions.Extended;	 	// moko: added extended bundle to the web player build
	
	static BuildSettings()
	{
		UpdateFromSettings();
	}
	
	public static void SaveSettings()
	{
		Hashtable bundleServerConfig = new Hashtable();
		bundleServerConfig["BundleServerAddress"] = BuildSettings.BundleServerAddress;
		bundleServerConfig["BundleMode"] = (int)BuildSettings.DevBundleMode;
		bundleServerConfig["AttachProfiler"] = (BuildSettings.Options & BuildOptions.ConnectWithProfiler) != 0;
		bundleServerConfig["DevLevelsBuild"] = BuildSettings.DevelopmentLevelsBuild;
		File.WriteAllText(BuildConfigPath, EB.JSON.Stringify(bundleServerConfig));
	}

	public static void UpdateFromSettings()
	{
		Hashtable bundleServerConfig = null;
		if (File.Exists(BuildConfigPath))
		{
			bundleServerConfig = (Hashtable)EB.JSON.Parse(File.ReadAllText(BuildConfigPath));
		}
		else
		{
			bundleServerConfig = new Hashtable();
		}
		BuildSettings.BundleServerAddress = EB.Dot.String("BundleServerAddress", bundleServerConfig, "http://127.0.0.1:" + BuildSettings.BundleServerPort);
		BuildSettings.DevBundleMode = (EB.Assets.DevBundleType)EB.Dot.Integer("BundleMode", bundleServerConfig, (int)EB.Assets.DevBundleType.NoBundles);
		bool attachProfiler = EB.Dot.Bool("AttachProfiler", bundleServerConfig, false);
		BuildSettings.Options = EB.Flag.Set(BuildSettings.Options, BuildOptions.ConnectWithProfiler, attachProfiler);
		BuildSettings.DevelopmentLevelsBuild = EB.Dot.Array("DevLevelsBuild", bundleServerConfig, null);
		if (BuildSettings.DevelopmentLevelsBuild == null)
		{
			BuildSettings.DevelopmentLevelsBuild = new ArrayList(GetScenesFromEditorSettings());
		}
	}

	#endregion
}
