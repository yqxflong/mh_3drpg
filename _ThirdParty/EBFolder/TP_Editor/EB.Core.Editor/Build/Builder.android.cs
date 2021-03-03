using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public partial class Builder
{
#if UNITY_ANDROID
	static bool _debug = true;
	static bool _useobb = false;
	
	[MenuItem("Builder/Legacy/Build Debug APK")]
	public static void BuildAndroidDebug()
	{
		_debug = true;
		BuildAPK(BuildSettings.SuccessEmail);
	}
	
	[MenuItem("Builder/Legacy/Build Submission APK")]
	public static void BuildAndroidSubmission()
	{
		_debug = false;
		BuildAPK(BuildSettings.SuccessEmail);
	}
	
	[MenuItem("Builder/Legacy/Build OBB")]
	public static void BuildAndroidOBB()
	{
		_debug = true;
		_useobb = true;
		BuildOBB();
	}

	public static string BuildOBB()
	{
		try
		{			
			var tmpfoler = "/tmp/packs";
			GeneralUtils.DeleteDirectory(tmpfoler, true);   // mko: cleaning up build folder
			Directory.CreateDirectory(tmpfoler);
			
			var version = "1.0";
			try {
				var parts= PlayerSettings.bundleVersion.Split('.');
				version = parts[0] + "." + parts[1];
			}
			catch {
			}
			
			// moko: changed to do a debug dump of all builder job info first
			var date 	= System.DateTime.Now.ToString("dd/MM/yy HH:mm");
			string header = "*******************************************************************************\n";
			header += "Building to " + tmpfoler + " @" + date;
			Debug.Log(header);
			Debug.Log("Build Setting Parameters:\n" + BuildSettings.ToString());
			Debug.Log("Environment Setting Parameters:\n" + EnvironmentUtils.GetEnvirnomentDetails());
			
			var cl 		= EnvironmentUtils.Get("BUILD_CL", "0" );
			
			PlayerSettings.bundleVersion = version + "." + cl;
			
			// step1 build all the bundles (extended bundles)
			var options = Bundler.BundleOptions.Force | Bundler.BundleOptions.Extended | Bundler.BundleOptions.SkipBase;			
			var packs = Bundler.BuildAll( BuildSettings.BundlerConfigFolder, options);
			
			var tarPath 	= Path.Combine(tmpfoler, "packs.tar");
			var packPath	= BuildSettings.BuildFolder;
			var gzipPath 	= tarPath + ".gz";
			
			var filesPath	= Path.Combine(packPath, "files.json");
			FileUtil.DeleteFileOrDirectory(filesPath);
			
			// calculate the files
			var files = new Hashtable();
			foreach( var packFile in Directory.GetFiles(packPath,"*",SearchOption.AllDirectories) ) 	
			{
				var relativeName = packFile.Substring(packPath.Length+1);
				var finfo = new FileInfo(packFile);
				files[relativeName] = finfo.Length;
			}
			
			// write to the files.json
			var fileData = new Hashtable();
			fileData["packs"] = packs;
			fileData["files"] = files;
			File.WriteAllBytes(filesPath, EB.Encoding.GetBytes(EB.JSON.Stringify(fileData))) ;
				
			// turn into gz, tar archive
			using ( var gzFile = new FileStream(gzipPath, FileMode.Create, FileAccess.ReadWrite) )
			{
				using( var gzStream = new Ionic.Zlib.GZipStream(gzFile, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestCompression) )
				{
					var writer = new tar_cs.TarWriter(gzStream);
					foreach( var packFile in Directory.GetFiles(packPath,"*",SearchOption.AllDirectories) ) 	
					{
						var relativeName = packFile.Substring(packPath.Length+1);
						//Debug.Log("file: " + relativeName);	
						using( var f = new FileStream(packFile, FileMode.Open, FileAccess.Read) )
						{
							writer.Write(f, f.Length, relativeName, string.Empty, string.Empty, 511, System.DateTime.UtcNow);  
						}
					}
					writer.Close();
				}
			}

			//var url = S3Utils.Put(gzipPath, Path.Combine(cl,cl+".obb") );  

			//return url;
			return gzipPath;
		}
		catch( System.Exception ex )
		{
			Debug.Log("BuildOBB Failed: exception: " + ex.ToString());
			throw ex;
		}
	}
	
	public static void BuildAPK(string distributionList)
	{
		try
		{			
			var folder = BuildSettings.BaseBuildFolder;
			var platformFolder = BuildSettings.BuildFolder;
			
			// moko: changed to do a debug dump of all builder job info first
			var date 	= System.DateTime.Now.ToString("dd/MM/yy HH:mm");
			string header = "*******************************************************************************\n";
			header += "Building to " + folder + " @" + date;
			Debug.Log(header);
			Debug.Log("Build Setting Parameters:\n" + BuildSettings.ToString());
			Debug.Log("Environment Setting Parameters:\n" + EnvironmentUtils.GetEnvirnomentDetails());
			
			// cleanup
			GeneralUtils.DeleteDirectory(platformFolder, true);   // mko: cleaning up build folder
			GeneralUtils.DeleteDirectory("Android", true);   // mko: cleaning up build folder
			Directory.CreateDirectory(folder);
			Directory.CreateDirectory(platformFolder);
			
			var version = "1.0";
			try {
				var parts= PlayerSettings.bundleVersion.Split('.');
				version = parts[0] + "." + parts[1];
			}
			catch {
			}
			
			// moko: prepend #define instead of replace in case there is some specific settings stored in playerSetting
			string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
			if (_debug)
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "USE_DEBUG;" + defines);
			}
			else
			{
				if (_useobb)
				{
					PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "USE_OBB;" + defines);
				}
			}

			var cl 		= EnvironmentUtils.Get("BUILD_CL", "0" );
			var desc	= "Android " + date + " CL: " + cl + "\n";
			var notes	= (ArrayList)EB.JSON.Parse( EnvironmentUtils.Get("BUILD_NOTES", "[]") );
			
			PlayerSettings.bundleVersion = version + "." + cl;
			PlayerSettings.applicationIdentifier = BuildSettings.BundleIdentifier;
			PlayerSettings.use32BitDisplayBuffer = false;
			
			WriteVersionFile(version + "." + cl );
			
			Debug.Log("Desc: " + desc );
			
			// disable scenes that are the tracks
			var settings = new List<EditorBuildSettingsScene>();
			foreach ( var setting in EditorBuildSettings.scenes )
			{
				if (setting.path.Contains("tracks"))
				{
					setting.enabled = !_useobb;
				}
				
				if (_debug == false && setting.path.Contains("selector") )
				{
					setting.enabled = false;
				}
				
				settings.Add(setting);
			}
			EditorBuildSettings.scenes = settings.ToArray();
			
			// build bundles
			DisplayProgressBar("Building", "Building Bundles", 0.0f);
			Bundler.BundleOptions bundleOptions = Bundler.BundleOptions.Force | Bundler.BundleOptions.SkipOgg;
			if( _debug || ( _useobb == false ) )
			{
				bundleOptions |= Bundler.BundleOptions.Extended;
			}
			
			Bundler.BuildAll(BuildSettings.BundlerConfigFolder, bundleOptions);
			
			// build the player 
			var apkPath = Path.Combine(folder, cl + ".apk");
						
			PlayerSettings.Android.bundleVersionCode = int.Parse(cl);
			
			if (!_debug)
			{
				PlayerSettings.bundleVersion = BuildSettings.Version;
			}
			else
			{
				PlayerSettings.Android.bundleVersionCode = 1;
			}
			
			Debug.Log("Building to " + apkPath);
			
			AndroidUtils.BuildPlayer(apkPath, BuildOptions.None);
			
			if (!File.Exists(apkPath))
			{
				throw new System.Exception("Failed to build apk!");
			}

			// upload to s3
			//string url = S3Utils.Put( apkPath, string.Format("{0}-{1}-{2}.apk", PlayerSettings.bundleIdentifier, PlayerSettings.bundleVersion, cl) );
			string url = apkPath;

			// send email
			var data = new Hashtable();
			data["cl"] = cl;
			data["title"] = desc;
			data["url"] = url;
			data["notes"] = notes;
			data["obb"] = "";
			
			if (!_debug && _useobb)
			{
				data["obb"] = BuildOBB();
				UploadApkFiles(apkPath, WWWUtils.Environment.API);
			}
			
			Email( distributionList, BuildSettings.ProjectName + " - Android Build: " + PlayerSettings.bundleVersion + (_debug ? " DEBUG" : " SUBMISSION"), File.ReadAllText("Assets/Editor/EB.Core.Editor/Build/Email/androidbuild.txt"), data );  
		
			Done();
		}
		catch(System.Exception e)
		{
			Debug.Log("Build Failed: exception: " + e.ToString());
			Failed(e);
		}
		
		ClearProgressBar();
	}
	
	static void UploadApkFiles( string apkPath, WWWUtils.Environment env )
	{
		try 
		{
			var tmpDir = "/tmp/apk";
			GeneralUtils.DeleteDirectory(tmpDir, true);   // mko: cleaning up build folder
			Directory.CreateDirectory(tmpDir);
			
			// unzip
			var res = CommandLineUtils.Run("/usr/bin/unzip", string.Format("-d {0} {1}", tmpDir, apkPath) );
			Debug.Log(res);
				
			// generate the app.dll
			var files = new List<string>( Directory.GetFiles("/tmp/apk/assets/bin/Data/Managed","*.dll",SearchOption.TopDirectoryOnly) );
			files.Sort( System.StringComparer.OrdinalIgnoreCase );
			
			List<byte> bytes = new List<byte>();
			foreach (string filePath in files )
			{
				Debug.Log("Adding file " + filePath);
				bytes.AddRange( File.ReadAllBytes(filePath ) ); 
			}
			
			Debug.Log("MSIL size is " + EB.Localizer.FormatNumber(bytes.Count,true) );
			
			WWWForm form = new WWWForm();
			form.AddBinaryData("data", bytes.ToArray(), "data" );
			form.AddField("version", EB.Version.GetVersion() );
			form.AddField("platform", "android");
			form.AddField("sha1", EB.Encoding.ToBase64String( EB.Digest.Sha1().Hash(bytes.ToArray()) ));    
			
			var stoken = WWWUtils.AdminLogin(env);
			var postUrl = WWWUtils.GetAdminUrl(env) + "/protect/upload?stoken="+stoken;
			res = WWWUtils.Post(postUrl, form);
			Debug.Log("version: " + res);
		}
		catch(System.Exception e)
		{
			Debug.Log("Build Failed: exception: " + e.ToString());
			Failed(e);
		}
	}
#endif
	
}