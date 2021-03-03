using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using Pathfinding.Ionic.Zip;

namespace EB.Editor
{
	public static partial class PostProcess
	{
		#region Jenkins参数
		//pkg_type
		private const string JPARAM_PKG_TYPE_INNER = "inner";
		private const string JPARAM_PKG_TYPE_WECHAT = "wechat";
		private const string JPARAM_PKG_TYPE_XK = "xk";
		private const string JPARAM_PKG_TYPE_XKGDT = "xkgdt";
		private const string JPARAM_PKG_TYPE_XKTAP = "xktap";
		private const string JPARAM_PKG_TYPE_XKFX = "xkfx";
		private const string JPARAM_PKG_TYPE_XKX = "xkx";
		private const string JPARAM_PKG_TYPE_XKSLHX = "xkslhx";
		private const string JPARAM_PKG_TYPE_XKMKHX = "xkmkhx";
		private const string JPARAM_PKG_TYPE_AST = "astgoogle";
        #endregion

		public static int MaxBundleSize = 0;

		private static string FindAAPT()
		{
			if( !Directory.Exists(AndroidUtils.SDK) )
			{
				string msg = "PostProcess in PostProcess.andriod.cs requires the AndriodSDK to be at " + AndroidUtils.SDK;
				throw new System.IO.DirectoryNotFoundException( msg );
			}

			var aaptPath = Path.Combine( AndroidUtils.SDK, "platform-tools/aapt" );
			if (!File.Exists(aaptPath))
			{
				var buildTools = new DirectoryInfo( Path.Combine( AndroidUtils.SDK, "build-tools" ) );
				aaptPath = buildTools.GetFiles("aapt", SearchOption.AllDirectories)[0].FullName;
			}
			if (!File.Exists(aaptPath))
			{
				UnityEngine.Debug.LogError("Didn't find aapt binary!");
				throw new System.Exception("AAPT binary not found. Please Install AAPT from the android sdk tools");
			}
			return aaptPath;
		}

		private static void ResignAPK(string path)
		{
			string res;
			
			#region zipalign
            string zipalign = Path.Combine(AndroidUtils.BUILD_TOOLS, "zipalign");
            res = CommandLineUtils.Run(zipalign, $"-f -v 4 {path}");
			UnityEngine.Debug.Log(res);
			#endregion

			#region apksigner
			string cmdline = "";
			string indef = PlayerSettings.applicationIdentifier;
			string pkg = EnvironmentUtils.Get("pkg_type", JPARAM_PKG_TYPE_INNER);
            if (pkg == JPARAM_PKG_TYPE_AST)//傲世堂用，ebg，v1,v2
            {
				string keystore = Path.GetFullPath("Assets/Editor/Android/android.keystore");
				cmdline = $"-keystore {keystore} -alias ebg -aliaspswd 123456 -pswd 123456 -v1_set true -v2 true {path}";
			}
			else if(pkg.Equals(JPARAM_PKG_TYPE_XKSLHX))//圣灵幻想，xinkuai,v1
			{
				string keystore = Path.GetFullPath("Assets/Editor/Android/key.keystore");
				cmdline = $"-keystore {keystore} -alias xinyou -aliaspswd xinyou -pswd xinyou -v1_set true -v2 false {path}";
			}
            else//其他，ebg,v1
            {
				string keystore = Path.GetFullPath("Assets/Editor/Android/android.keystore");
				cmdline = $"-keystore {keystore} -alias ebg -aliaspswd 123456 -pswd 123456 -v1_set true -v2 false {path}";
			}
			string apksigner = Path.GetFullPath("Assets/Editor/Android/apksigner.jar");
			res = CommandLineUtils.Run($"java -jar {apksigner}", cmdline);
			UnityEngine.Debug.Log(res);
			#endregion
		}

		[PostProcessBuild]
		public static void AndroidProcess( BuildTarget target, string path )
		{
			if ( target != BuildTarget.Android )
			{
				return;
			}

			if(EnvironmentUtils.Get("pkg_type", JPARAM_PKG_TYPE_INNER) == JPARAM_PKG_TYPE_AST 
				&& EnvironmentUtils.Get("Debug", "false") == "false")
			{
				ResignAPK(path);
			}
			else
			{
				UnityEngine.Debug.LogFormat("PostProcess {0}, {1}", target, path);
				AndroidPackageProcess(path);
				GM.RemoteBundleManager.IsDownLoadABInGame = false;
				UnityEngine.Debug.LogFormat("PostProcess {0}, {1} finished", target, path);
			}
		}

		private static void AndroidPackageProcess(string path)
		{
			#region 打开apk，装入bundles
			FileInfo bundle = new FileInfo(path);
			if (bundle.Length < MaxBundleSize || MaxBundleSize == 0)
			{
				using (var zipFile = new ZipFile(path))
				{
					string basePath = BMUtility.InterpretPath("$(Personal)/LTSites/", BuildPlatform.Android);
					string outputPath = BuildConfiger.InterpretedOutputPath;
					string shortPath = "assets/AssetBundles/" + outputPath.Substring(basePath.Length);
					if (zipFile.ContainsEntry(shortPath + "/" + "BundleShipInfo.json")){//已装过bundles
						UnityEngine.Debug.LogWarning("AssetBundles already exists");
					}
					else{
						//获取要装入的bundles
						// UnityEngine.Debug.LogError($"AndroidPackageProcess=====>max: {MaxBundleSize}==bundleLenght: {bundle.Length}");
						var assets = MaxBundleSize > 0 ? BuildHelper.GetBundlesByPriority(MaxBundleSize - bundle.Length) : BMDataAccessor.BundleShipInfos;
						zipFile.AddDirectoryByName(shortPath);
						//装入准备好的bundles
						foreach (var file in assets)
						{
							if (!GM.RemoteBundleManager.IsBuildBundle(file.BundleName))
							{
								continue;
							}
							var fullpath = Path.Combine(outputPath, file.BundleName.ToLower() + ".cz");
							var shortname = shortPath + "/" + file.BundleName.ToLower() + ".cz";
							if (zipFile.ContainsEntry(shortname))
							{
								UnityEngine.Debug.LogWarningFormat("{0} already exists", shortname);
							}
							else
							{
								zipFile.AddFile(fullpath, shortPath);
							}
						}

						//装入json等文件
						if (MaxBundleSize == 0){
							foreach (string s in Directory.GetFiles(outputPath, "*.json"))
							{
								string fileName = System.IO.Path.GetFileName(s);
								var shortname = shortPath + "/" + fileName;
								if (zipFile.ContainsEntry(shortname))
								{
									UnityEngine.Debug.LogWarningFormat("{0} already exists", shortname);
								}
								else
								{
									zipFile.AddFile(s, shortPath);
								}
							}

							string sourcePath = Path.GetDirectoryName(outputPath);
							string destinationPath = Path.GetDirectoryName(shortPath);
							foreach (string s in Directory.GetFiles(sourcePath))
							{
								string fileName = System.IO.Path.GetFileName(s);
								var shortname = destinationPath + "/" + fileName;
								if (zipFile.ContainsEntry(shortname))
								{
									UnityEngine.Debug.LogWarningFormat("{0} already exists", shortname);
								}
								else
								{
									zipFile.AddFile(s, destinationPath);
								}
							}
						}
						else{
							string shipInfoFile = shortPath + "/" + "BundleShipInfo.json";
							if (zipFile.ContainsEntry(shipInfoFile))
							{
								zipFile.UpdateEntry(shipInfoFile, EB.Encoding.GetBytes(GM.JSON.ToJson(assets)));
							}
							else
							{
								zipFile.AddEntry(shipInfoFile, EB.Encoding.GetBytes(GM.JSON.ToJson(assets)));
							}
						}

						//由于包体过大，压缩前需要设置这个
						zipFile.UseZip64WhenSaving = Zip64Option.AsNecessary;
						zipFile.Save();
					}
				}
			}
			#endregion

			//重新签名
			ResignAPK(path);
		}
	}
}
