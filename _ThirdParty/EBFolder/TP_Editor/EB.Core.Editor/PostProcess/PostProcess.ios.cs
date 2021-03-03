using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Collections.Generic;
using UnityEditor.iOS.XcodeCustom;

namespace EB.Editor
{
    public static partial class PostProcess
    {
        #region Jenkins参数
        private const string IOS_JPARAM_PKG_TYPE_INNER = "inner";
	    private const string IOS_JPARAM_PKG_TYPE_XK = "xk";
        private const string IOS_JPARAM_PKG_TYPE_AST = "ast";
        #endregion

        public static string Signer = string.Empty;
        public static string Profile = string.Empty;

        [PostProcessBuild(100)]
        public static void IOSProcess(BuildTarget target, string path)
        {

            EB.Debug.Log("IOSProcess111111111111111111");

            if (target != BuildTarget.iOS)
            {
                return;
            }

            UnityEngine.Debug.LogFormat("PostProcess {0}, {1}", target, path);

            DeployData(path);

            PlistDocument plist = new PlistDocument();
            string info_plist_file_name = Path.Combine(path, "Info.plist");
            plist.ReadFromFile(info_plist_file_name);

#region  ios localization
            plist.root["CFBundleDevelopmentRegion"] = new PlistElementString("en");
            plist.root["LSHasLocalizedDisplayName"] = new PlistElementString("Yes");
            PlistElementArray localizations = plist.root.CreateArray("CFBundleLocalizations");
            localizations.AddString("en");
            localizations.AddString("zh-Hans");
            localizations.AddString("zh-Hant");
#endregion

#region device capabilities
            PlistElementArray capabilities = plist.root.CreateArray("UIRequiredDeviceCapabilities");
            if (!StringExists(capabilities, "arm64"))
            {
                capabilities.AddString("arm64");
            }
            if (!StringExists(capabilities, "gamekit"))
            {
                capabilities.AddString("gamekit");
            }
            if (!StringExists(capabilities, "gyroscope"))
            {
                capabilities.AddString("gyroscope");
            }
#endregion

            // see: http://docs.jpush.cn/display/dev/iOS+7+Background+Remote+Notification
            PlistElementArray uibg = (plist.root["UIBackgroundModes"] ?? plist.root.CreateArray("UIBackgroundModes")) as PlistElementArray;
            if (!StringExists(uibg, "remote-notification"))
            {
                uibg.AddString("remote-notification");
            }
            plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            plist.WriteToFile(info_plist_file_name);

            PBXProject proj = new PBXProject();
            string project_file_name = PBXProject.GetPBXProjectPath(path);
            proj.ReadFromFile(project_file_name);

            string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

            proj.RemoveFile(proj.FindFileGuidByProjectPath("libiconv.2.dylib"));

#region 自动添加framework
            proj.AddFrameworkToProject(guid, "StoreKit.framework", false);
            proj.AddFrameworkToProject(guid, "CoreTelephony.framework", false);
            proj.AddFrameworkToProject(guid, "libz.tbd", false);
            proj.AddFrameworkToProject(guid, "libc++.tbd", false);
            proj.AddFrameworkToProject(guid, "libsqlite3.0.tbd", false);
            proj.AddFrameworkToProject(guid, "libsqlite3.tbd", false);
#endregion

#region UseSymbols
            string releaseGuid = proj.BuildConfigByName(guid, "Release");
            string debugGuid = proj.BuildConfigByName(guid, "Debug");
            proj.SetBuildPropertyForConfig(releaseGuid, "DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");
            proj.SetBuildPropertyForConfig(debugGuid, "DEBUG_INFORMATION_FORMAT", "dwarf");
            proj.SetBuildProperty(guid, "GCC_GENERATE_DEBUGGING_SYMBOLS", "YES");
            proj.SetBuildProperty(guid, "COPY_PHASE_STRIP", "NO");
            proj.SetBuildProperty(guid, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
            proj.SetBuildProperty(guid, "GCC_ENABLE_CPP_EXCEPTIONS", "YES");
            proj.SetBuildProperty(guid, "ENABLE_BITCODE", "NO");
            proj.AddBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC -lz");
#endregion

#region set code sign identity & provisioning profile
            if (!string.IsNullOrEmpty(Signer))
            {
                proj.SetBuildProperty(guid, "CODE_SIGN_IDENTITY", Signer);
            }
            if (!string.IsNullOrEmpty(Profile))
            {
                proj.SetBuildProperty(guid, "PROVISIONING_PROFILE", Profile);
            }
#endregion

#region localization
        CopyAndReplaceDirectory(Path.Combine(Application.dataPath, "Plugins/iOS/Localization"), Path.Combine(path, "Localization"));
        AddLocalizationFolderToBuild(proj, path, "Localization");
        CopyAndReplaceDirectory(Path.Combine(Application.dataPath, "Plugins/iOS/Appirater/Localization"), Path.Combine(path, "Appirater/Localization"));
        AddLocalizationFolderToBuild(proj, path, "Appirater/Localization");
#endregion

            proj.WriteToFile(project_file_name);

            // Get root
            PlistElementDict rootDict = plist.root;

            // remove exit on suspend if it exists.
            string exitsOnSuspendKey = "UIApplicationExitsOnSuspend";
            if (rootDict.values.ContainsKey(exitsOnSuspendKey))
            {
                rootDict.values.Remove(exitsOnSuspendKey);
            }

#region SupportSDK
            string useBugly = EnvironmentUtils.Get("use_bugly", "true");
            if(useBugly.Equals("true"))
            {
                ProcessBuglySDK(path);
            }
            ProcessUmengSDK(path);
#endregion

#region 渠道SDK，选择性添加
            string pkg = EnvironmentUtils.Get("pkg_type", IOS_JPARAM_PKG_TYPE_INNER);
            if(pkg.Equals(IOS_JPARAM_PKG_TYPE_INNER) || pkg.Equals(IOS_JPARAM_PKG_TYPE_XK))
            {
                ProcessXinkuaiMHSDK(path);
            }
            else if(pkg.Equals(IOS_JPARAM_PKG_TYPE_AST))
            {
                ProcessAoshitangSDK(path);
                ProcessAIHelpSDK(path);
                ProcessAppsFlyerSDK(path);
            }
#endregion
            
            UnityEngine.Debug.LogFormat("PostProcess {0}, {1} finished", target, path);
        }

#region Support SDK
        private static void ProcessBuglySDK(string path)
        {
            PBXProject proj = new PBXProject();
            string project_file_name = PBXProject.GetPBXProjectPath(path);
			proj.ReadFromFile(project_file_name);
            string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            
            #region framework
            CopyAndReplaceDirectory("Sdk/iOS/BuglySDK/Bugly.framework", Path.Combine(path, "Libraries/BuglySDK/Bugly.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/BuglySDK/Bugly.framework", "Libraries/BuglySDK/Bugly.framework", PBXSourceTree.Source));
            #endregion

            #region libs
            CopyAndReplaceDirectory("Sdk/iOS/BuglySDK/BuglyBridge", Path.Combine(path, "Libraries/BuglySDK/BuglyBridge"));
            AddFolderToBuild(proj, guid, path, "Libraries/BuglySDK/BuglyBridge");
            #endregion

            //Systemframework
            proj.AddFrameworkToProject(guid, "libz.dylib", false);
            proj.AddFrameworkToProject(guid, "libc++.dylib", false);
            proj.AddFrameworkToProject(guid, "SystemConfiguration.framework", false);
            proj.AddFrameworkToProject(guid, "Security.framework", false);
            proj.AddFrameworkToProject(guid, "JavaScriptCore.framework", false);

            //property
            proj.AddBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC");
            proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Libraries/BuglySDK");
            proj.AddBuildProperty(guid, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/BuglySDK/BuglyBridge");
            proj.AddBuildProperty(guid, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries/BuglySDK/BuglyBridge");

            proj.WriteToFile(project_file_name);
        }
        
        private static void ProcessUmengSDK(string path)
        {
            EB.Debug.Log("----------ProcessEssential_ProcessUmengSDK");
            PBXProject proj = new PBXProject();
			string project_file_name = PBXProject.GetPBXProjectPath(path);
			proj.ReadFromFile(project_file_name);

			string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            //Add code 
            string asPorjectPath = Path.GetFullPath(path);
            WriteBelow(asPorjectPath + "/Classes/UnityAppController.mm", "#include \"PluginBase/AppDelegateListener.h\"", "#import \"UMCommon/UMConfigure.h\"");
            WriteBelow(asPorjectPath + "/Classes/UnityAppController.mm", "::printf(\"-> applicationDidFinishLaunching()\\n\");",
                "[UMConfigure initWithAppkey:@\"5f60cedab473963242a000c1\" channel:@\"App Store\"];");
            //Add UmengSDK FrameWork
            CopyAndReplaceDirectory("Sdk/iOS/UMengSDK/UMAnalyticsGame.framework", Path.Combine(path, "Libraries/UMengSDK/UMAnalyticsGame.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/UMengSDK/UMAnalyticsGame.framework", "Libraries/UMengSDK/UMAnalyticsGame.framework", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/UMengSDK/UMCommon.framework", Path.Combine(path, "Libraries/UMengSDK/UMCommon.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/UMengSDK/UMCommon.framework", "Libraries/UMengSDK/UMCommon.framework", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/UMengSDK/UMCommonLog.bundle", Path.Combine(path, "Libraries/UMengSDK/UMCommonLog.bundle"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/UMengSDK/UMCommonLog.bundle", "Libraries/UMengSDK/UMCommonLog.bundle", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/UMengSDK/UMCommonLog.framework", Path.Combine(path, "Libraries/UMengSDK/UMCommonLog.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/UMengSDK/UMCommonLog.framework", "Libraries/UMengSDK/UMCommonLog.framework", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/UMengSDK/Source", Path.Combine(path, "Libraries/UMengSDK/Source"));
            AddFolderToBuild(proj, guid, path, "Libraries/UMengSDK/Source");

            //CopyAndReplaceDirectory("Sdk/iOS/UMengSDK/UTDID.framework", Path.Combine(path, "Libraries/UMengSDK/UTDID.framework"));
            //proj.AddFileToBuild(guid, proj.AddFile("Libraries/UMengSDK/UTDID.framework", "Libraries/UMengSDK/UTDID.framework", PBXSourceTree.Source));

            //Add Systemframework
            //Add System tbd
            proj.AddFrameworkToProject(guid, "libz.dylib", false);
            proj.AddFrameworkToProject(guid, "libsqlite3.tbd", false);
            proj.AddFrameworkToProject(guid, "CoreTelephony.framework", false);
            //proj.AddBuildProperty(guid, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries/Plugins/UMengSDK");

            proj.AddBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC");
            proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Libraries/UmengSDK");
            proj.AddBuildProperty(guid, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/UmengSDK");
            proj.AddBuildProperty(guid, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/UmengSDK/UMCommon.framework");


            //// Get root
            //PlistElementDict kyRootDict = kyPlist.root;
            //kyRootDict.SetString("UmengAppkey", "5f60cedab473963242a000c1");
            proj.WriteToFile(project_file_name);
        }

        private static void ProcessAIHelpSDK(string path)
        {
            PBXProject proj = new PBXProject();
            string project_file_name = PBXProject.GetPBXProjectPath(path);
            proj.ReadFromFile(project_file_name);
            string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

            #region framework
            CopyAndReplaceDirectory("Sdk/iOS/AIHelpSDK/MQTTFramework.framework", Path.Combine(path, "Libraries/AIHelpSDK/MQTTFramework.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/AIHelpSDK/MQTTFramework.framework", "Libraries/AIHelpSDK/MQTTFramework.framework", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/AIHelpSDK/ElvaChatServiceSDK.framework", Path.Combine(path, "Libraries/AIHelpSDK/ElvaChatServiceSDK.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/AIHelpSDK/ElvaChatServiceSDK.framework", "Libraries/AIHelpSDK/ElvaChatServiceSDK.framework", PBXSourceTree.Source));
            #endregion

            #region bundle
            CopyAndReplaceDirectory("Sdk/iOS/AIHelpSDK/ElvaChatServiceSDK.bundle", Path.Combine(path, "Libraries/AIHelpSDK/ElvaChatServiceSDK.bundle"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/AIHelpSDK/ElvaChatServiceSDK.bundle", "Libraries/AIHelpSDK/ElvaChatServiceSDK.bundle", PBXSourceTree.Source));
            AddLocalizationFolderToBuild(proj, path, "Libraries/AIHelpSDK/ElvaChatServiceSDK.bundle");
            #endregion

            #region Source
            CopyAndReplaceDirectory("Sdk/iOS/AIHelpSDK/Source", Path.Combine(path, "Libraries/AIHelpSDK/Source"));
            AddFolderToBuild(proj, guid, path, "Libraries/AIHelpSDK/Source");
            #endregion

            #region searchpath
            proj.AddBuildProperty(guid, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/AIHelpSDK/ElvaChatServiceSDK.framework");
            proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Libraries/AIHelpSDK");
            #endregion

            #region systemframework
            proj.AddFrameworkToProject(guid, "libresolv.9.tbd", false);
            #endregion
            
            proj.WriteToFile(project_file_name);

            #region info.plist
            PlistDocument kyPlist = new PlistDocument();
            string info_plist_file_name = Path.Combine(path, "Info.plist");
            kyPlist.ReadFromFile(info_plist_file_name);
            // Get root
            PlistElementDict kyRootDict = kyPlist.root;
            kyRootDict.SetString("NSPhotoLibraryUsageDescription", "需要访问您的相册权限，才能将图片上传反馈给客服");
            kyRootDict.SetString("NSCameraUsageDescription", "需要访问您的相机权限，才能拍摄问题图片并反馈给客服");
            kyRootDict.SetString("NSPhotoLibraryAddUsageDescription", "需要照片添加权限，才能保存图片到相册");
            kyRootDict.SetString("NSMicrophoneUsageDescription", "需要访问您的麦克风权限，才能在表单页上传视频录像并反馈给客服");
            kyPlist.WriteToFile(info_plist_file_name);
            #endregion
        }

        private static void ProcessAppsFlyerSDK(string path)
        {
            PBXProject proj = new PBXProject();
            string project_file_name = PBXProject.GetPBXProjectPath(path);
            proj.ReadFromFile(project_file_name);
            string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

            #region libs
            CopyAndReplaceDirectory("Sdk/iOS/AppsFlyerSDK/libs", Path.Combine(path, "Libraries/AppsFlyerSDK/libs"));
            AddFolderToBuild(proj, guid, path, "Libraries/AppsFlyerSDK/libs");
            #endregion

            #region Source
            CopyAndReplaceDirectory("Sdk/iOS/AppsFlyerSDK/Source", Path.Combine(path, "Libraries/AppsFlyerSDK/Source"));
            AddFolderToBuild(proj, guid, path, "Libraries/AppsFlyerSDK/Source");
            #endregion

            #region searchpath
            proj.AddBuildProperty(guid, "HEADER_SEARCH_PATHS", "$(SRCROOT)/Libraries/AppsFlyerSDK/libs");
            proj.AddBuildProperty(guid, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries/AppsFlyerSDK/libs");
            #endregion

            proj.WriteToFile(project_file_name);
        }
#endregion

#region ChannelSDK
        private static void ProcessIFlySDK(string path)
        {
			PBXProject proj = new PBXProject();
			string project_file_name = PBXProject.GetPBXProjectPath(path);
			proj.ReadFromFile(project_file_name);

			string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

			CopyAndReplaceDirectory("Sdk/iOS/iFlySDK/Lib/iflyMSC.framework", Path.Combine(path, "iflyMSC.framework"));
			proj.AddFileToBuild(guid, proj.AddFile("iflyMSC.framework", "Frameworks/iflyMSC.framework", PBXSourceTree.Source));

			CopyAndReplaceDirectory("Sdk/iOS/iFlySDK/Plugin", Path.Combine(path, "Libraries/iFlySDK"));
			AddFolderToBuild(proj, guid, path, "Libraries/iFlySDK");

			proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
			proj.AddFrameworkToProject(guid, "AVFoundation.framework", false);
			proj.AddFrameworkToProject(guid, "SystemConfiguration.framework", false);
			proj.AddFrameworkToProject(guid, "Foundation.framework", false);
			proj.AddFrameworkToProject(guid, "CoreTelephony.framework", false);
			proj.AddFrameworkToProject(guid, "AudioToolbox.framework", false);
			proj.AddFrameworkToProject(guid, "UIKit.framework", false);
			proj.AddFrameworkToProject(guid, "CoreLocation.framework", false);
			proj.AddFrameworkToProject(guid, "AddressBook.framework", false);
			proj.AddFrameworkToProject(guid, "QuartzCore.framework", false);
			proj.AddFrameworkToProject(guid, "CoreGraphics.framework", false);

			proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)");

			proj.WriteToFile(project_file_name);
        }

        private static void ProcessAsSDK(string path)
        {
			PBXProject proj = new PBXProject();
			string project_file_name = PBXProject.GetPBXProjectPath(path);
			proj.ReadFromFile(project_file_name);

			string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

			//Add AsSDK FrameWork
			CopyAndReplaceDirectory("Sdk/iOS/AsSdkFWMK/AlipaySDK.bundle", Path.Combine(path, "AlipaySDK.bundle"));
			proj.AddFileToBuild(guid, proj.AddFile("AlipaySDK.bundle", "AlipaySDK.bundle", PBXSourceTree.Source));
			CopyAndReplaceDirectory("Sdk/iOS/AsSdkFWMK/AsImage.bundle", Path.Combine(path, "AsImage.bundle"));
			proj.AddFileToBuild(guid, proj.AddFile("AsImage.bundle", "AsImage.bundle", PBXSourceTree.Source));
			CopyAndReplaceDirectory("Sdk/iOS/AsSdkFWMK/AsSdkFMWK.framework", Path.Combine(path, "Libraries/AsSdkFMWK.framework"));
			proj.AddFileToBuild(guid, proj.AddFile("Libraries/AsSdkFMWK.framework", "Frameworks/AsSdkFMWK.framework", PBXSourceTree.Source));

			//Add System tdb
			proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
			proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libsqlite3.0.tbd", "Frameworks/libsqlite3.0.tbd", PBXSourceTree.Sdk));
			proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libstdc++.6.tbd", "Frameworks/libstdc++.6.tbd", PBXSourceTree.Sdk));

			//Add System FrameWork
			proj.AddFrameworkToProject(guid, "CoreTelephony.framework", false);
			proj.AddFrameworkToProject(guid, "Security.framework", false);

			//Copy Connector
			CopyAndReplaceDirectory("Sdk/iOS/AsSdkFWMK/AsSDK", Path.Combine(path, "Libraries/AsSDK"));
			AddFolderToBuild(proj, guid, path, "Libraries/AsSDK");

			//AddBuildProperty
			proj.AddBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC");
			//proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
			proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Libraries");

			proj.WriteToFile(project_file_name);

			//Add Code to UnityAppController.mm
			string asPorjectPath = Path.GetFullPath(path);
			WriteBelow(asPorjectPath + "/Classes/UnityAppController.mm", "#include \"PluginBase/AppDelegateListener.h\"", "#import <AsSdkFMWK/AsInfoKit.h>\n#import <AsSdkFMWK/AsPlatformSDK.h>");
			WriteBelow(asPorjectPath + "/Classes/UnityAppController.mm", "SensorsCleanup();\n}", "- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation \n{\n    return (toInterfaceOrientation == UIInterfaceOrientationLandscapeLeft || toInterfaceOrientation == UIInterfaceOrientationLandscapeRight);\n}\n\n-(UIInterfaceOrientationMask)supportedInterfaceOrientations \n{\n    return UIInterfaceOrientationMaskLandscape;\n}\n\n-(BOOL)shouldAutorotate \n{\n    return YES;\n}");

			// Get plist
			PlistDocument asPlist = new PlistDocument();
			string info_plist_file_name = Path.Combine(path, "Info.plist");
			asPlist.ReadFromFile(info_plist_file_name);

			// Get root
			PlistElementDict asRootDict = asPlist.root;

			var asDict = asRootDict.CreateDict("NSAppTransportSecurity");
			asDict.SetBoolean("NSAllowsArbitraryLoads", true);

			PlistElementArray tmpArray = asRootDict.CreateArray("LSApplicationQueriesSchemes");
			tmpArray.AddString("alipay");
			tmpArray.AddString("weixin");
			tmpArray.AddString("mqqapiwallet");
			tmpArray.AddString("i4Tool4008227229");

			PlistElementArray asUrls = (asPlist.root["CFBundleURLTypes"] ?? asPlist.root.CreateArray("CFBundleURLTypes")) as PlistElementArray;

			List<PlistElement> asItems = asUrls.values ?? new List<PlistElement>();
			int j = 0;
			for (; j < asItems.Count; ++j)
			{
				var dict = asItems[j].AsDict();
				if (dict == null)
				{
					continue;
				}

				var content = dict["CFBundleURLName"];
				if (content == null)
				{
					continue;
				}

				if (content.AsString() == "com.hnjumper.manhuang.i4")
				{
					break;
				}
			}
			if (j >= asItems.Count)
			{
				var asUrl = asUrls.AddDict();
				asUrl.SetString("CFBundleTypeRole", "Editor");
				asUrl.SetString("CFBundleURLName", "com.hnjumper.manhuang.i4");
				var asSchemes = asUrl.CreateArray("CFBundleURLSchemes");
				asSchemes.AddString("As2356");
			}

			asPlist.WriteToFile(info_plist_file_name);
        }

        private static void ProcessYiJieSDK(string path)
        {
			PBXProject proj = new PBXProject();
			string project_file_name = PBXProject.GetPBXProjectPath(path);
			proj.ReadFromFile(project_file_name);

			string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

			//Add YiJieSDK FrameWork
			CopyAndReplaceDirectory("Sdk/iOS/YiJieSDK/OnlineAHelper.framework", Path.Combine(path, "Libraries/YiJieSDK/OnlineAHelper.framework"));
			proj.AddFileToBuild(guid, proj.AddFile("Libraries/YiJieSDK/OnlineAHelper.framework", "Libraries/YiJieSDK/OnlineAHelper.framework", PBXSourceTree.Source));

			CopyAndReplaceDirectory("Sdk/iOS/YiJieSDK/Lib", Path.Combine(path, "Libraries/YiJieSDK/Lib"));
			AddFolderToBuild(proj, guid, path, "Libraries/YiJieSDK/Lib");

			//Copy Connector
			CopyAndReplaceDirectory("Sdk/iOS/YiJieSDK/Source", Path.Combine(path, "Libraries/YiJieSDK/Source"));
			AddFolderToBuild(proj, guid, path, "Libraries/YiJieSDK/Source");

			//AddBuildProperty
			proj.AddBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC");
			proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Libraries/YiJieSDK");
            proj.AddBuildProperty(guid, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries/YiJieSDK/Lib");

			proj.WriteToFile(project_file_name);

			PlistDocument plist = new PlistDocument();
			string info_plist_file_name = Path.Combine(path, "Info.plist");
			plist.ReadFromFile(info_plist_file_name);

			PlistElementDict rootDict = plist.root;

			rootDict.SetString("com.snowfish.customer", "SNOWFISH");
			rootDict.SetString("com.snowfish.channel", "SNOWFISH");
			rootDict.SetString("com.snowfish.sdk.version", "2");
			rootDict.SetString("com.snowfish.appid", "{8AC62662-7E1C3AAA}");
			rootDict.SetString("com.snowfish.channelid", "{4ff036a1-3254eafe}");

            plist.WriteToFile(info_plist_file_name);
        }

        private static void ProcessWinnerSDK(string path)
        {
			PBXProject proj = new PBXProject();
			string project_file_name = PBXProject.GetPBXProjectPath(path);
			proj.ReadFromFile(project_file_name);

			string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

			//Add WinnerSDK FrameWork
			CopyAndReplaceDirectory("Sdk/iOS/WinnerSDK/Jar.framework", Path.Combine(path, "Libraries/WinnerSDK/Jar.framework"));
			proj.AddFileToBuild(guid, proj.AddFile("Libraries/WinnerSDK/Jar.framework", "Libraries/WinnerSDK/Jar.framework", PBXSourceTree.Source));

			//Copy Connector
			CopyAndReplaceDirectory("Sdk/iOS/WinnerSDK/Source", Path.Combine(path, "Libraries/WinnerSDK/Source"));
			AddFolderToBuild(proj, guid, path, "Libraries/WinnerSDK/Source");

			CopyAndReplaceDirectory("Sdk/iOS/WinnerSDK/JAR_Common.bundle", Path.Combine(path, "Libraries/WinnerSDK/JAR_Common.bundle"));
			proj.AddFileToBuild(guid, proj.AddFile("Libraries/WinnerSDK/JAR_Common.bundle", "Libraries/WinnerSDK/JAR_Common.bundle", PBXSourceTree.Source));

			CopyAndReplaceDirectory("Sdk/iOS/WinnerSDK/JAR_Login.bundle", Path.Combine(path, "Libraries/WinnerSDK/JAR_Login.bundle"));
			proj.AddFileToBuild(guid, proj.AddFile("Libraries/WinnerSDK/JAR_Login.bundle", "Libraries/WinnerSDK/JAR_Login.bundle", PBXSourceTree.Source));
			
			//AddBuildProperty
			proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libsqlite3.0.tbd", "Frameworks/libsqlite3.0.tbd", PBXSourceTree.Sdk));
			proj.AddBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC");
			proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Libraries/WinnerSDK");

			proj.WriteToFile(project_file_name);
        }

        private static void ProcessK7KSDK(string path)
        {
			PBXProject proj = new PBXProject();
			string project_file_name = PBXProject.GetPBXProjectPath(path);
			proj.ReadFromFile(project_file_name);

			string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

			//Add WinnerSDK FrameWork
			CopyAndReplaceDirectory("Sdk/iOS/K7KSDK/GameUserCenterSDK.framework", Path.Combine(path, "Libraries/K7KSDK/GameUserCenterSDK.framework"));
			proj.AddFileToBuild(guid, proj.AddFile("Libraries/K7KSDK/GameUserCenterSDK.framework", "Libraries/K7KSDK/GameUserCenterSDK.framework", PBXSourceTree.Source));

			//Copy Connector
			CopyAndReplaceDirectory("Sdk/iOS/K7KSDK/Source", Path.Combine(path, "Libraries/K7KSDK/Source"));
			AddFolderToBuild(proj, guid, path, "Libraries/K7KSDK/Source");

			CopyAndReplaceDirectory("Sdk/iOS/K7KSDK/GameUserCenterBundle.bundle", Path.Combine(path, "Libraries/K7KSDK/GameUserCenterBundle.bundle"));
			proj.AddFileToBuild(guid, proj.AddFile("Libraries/K7KSDK/GameUserCenterBundle.bundle", "Libraries/K7KSDK/GameUserCenterBundle.bundle", PBXSourceTree.Source));

			//AddBuildProperty
			proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libsqlite3.0.tbd", "Frameworks/libsqlite3.0.tbd", PBXSourceTree.Sdk));
			proj.AddBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC");
			proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Libraries/K7KSDK");

			proj.WriteToFile(project_file_name);
        }

        private static void ProcessQingYuanSDK(string path)
        {
			PBXProject proj = new PBXProject();
			string project_file_name = PBXProject.GetPBXProjectPath(path);
			proj.ReadFromFile(project_file_name);

			string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

			//Add WinnerSDK FrameWork
			CopyAndReplaceDirectory("Sdk/iOS/QingYuanSDK/QY39SDK.framework", Path.Combine(path, "Libraries/QingYuanSDK/QY39SDK.framework"));
			proj.AddFileToBuild(guid, proj.AddFile("Libraries/QingYuanSDK/QY39SDK.framework", "Libraries/QingYuanSDK/QY39SDK.framework", PBXSourceTree.Source));

			//Copy Connector
			CopyAndReplaceDirectory("Sdk/iOS/QingYuanSDK/Source", Path.Combine(path, "Libraries/QingYuanSDK/Source"));
			AddFolderToBuild(proj, guid, path, "Libraries/QingYuanSDK/Source");


			//AddBuildProperty
			proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libsqlite3.0.tbd", "Frameworks/libsqlite3.0.tbd", PBXSourceTree.Sdk));
			proj.AddBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC");
			proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Libraries/QingYuanSDK");

			proj.WriteToFile(project_file_name);
        }

        private static void ProcessAibeiSDK(string path)
        {
            PBXProject proj = new PBXProject();
			string project_file_name = PBXProject.GetPBXProjectPath(path);
			proj.ReadFromFile(project_file_name);

			string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

            //Add SDK FrameWork
            CopyAndReplaceDirectory("Sdk/iOS/AibeiSDK/IappOpenIdResources.bundle", Path.Combine(path, "Libraries/AibeiSDK/IappOpenIdResources.bundle"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/AibeiSDK/IappOpenIdResources.bundle", "Libraries/AibeiSDK/IappOpenIdResources.bundle", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/AibeiSDK/Lib", Path.Combine(path, "Libraries/AibeiSDK/Lib"));
            AddFolderToBuild(proj, guid, path, "Libraries/AibeiSDK/Lib");

            //Add System tdb
            proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));

            //Add System FrameWork
            proj.AddFrameworkToProject(guid, "CoreLocation.framework", false);
            proj.AddFrameworkToProject(guid, "CoreTelephony.framework", false);

            //Copy Connector
            CopyAndReplaceDirectory("Sdk/iOS/AibeiSDK/Source", Path.Combine(path, "Libraries/AibeiSDK/Source"));
			AddFolderToBuild(proj, guid, path, "Libraries/AibeiSDK/Source");


			//AddBuildProperty
            proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Libraries/AibeiSDK");
            proj.AddBuildProperty(guid, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries/AibeiSDK/Lib");

            PlistDocument plist = new PlistDocument();
            string info_plist_file_name = Path.Combine(path, "Info.plist");
            plist.ReadFromFile(info_plist_file_name);

            PlistElementDict rootDict = plist.root;
            PlistElementDict queriesSchemesDic = rootDict.CreateDict("URL types");
            queriesSchemesDic.SetBoolean("Allow Arbitrary Loads", true);

            rootDict.SetString("NSLocationAlwaysUsageDescription", null);

            proj.WriteToFile(project_file_name);
        }

        private static void ProcessXinkuaiMHSDK(string path)
        {
            EB.Debug.Log("ProcessXinkuaiMHSDK111111111111111111");
            PBXProject proj = new PBXProject();
            string project_file_name = PBXProject.GetPBXProjectPath(path);
            proj.ReadFromFile(project_file_name);

            string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

            //Add WinnerSDK FrameWork
            CopyAndReplaceDirectory("Sdk/iOS/XinkuaiMHSDK/mhhx.framework", Path.Combine(path, "Libraries/XinkuaiMHSDK/mhhx.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/XinkuaiMHSDK/mhhx.framework", "Libraries/XinkuaiMHSDK/mhhx.framework", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/XinkuaiMHSDK/mhhx.bundle", Path.Combine(path, "Libraries/XinkuaiMHSDK/mhhx.bundle"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/XinkuaiMHSDK/mhhx.bundle", "Libraries/XinkuaiMHSDK/mhhx.bundle", PBXSourceTree.Source));

            //Add System tdb
            proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
            proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libc++.tbd", "Frameworks/libc++.tbd", PBXSourceTree.Sdk));

            //Add System FrameWork
            //proj.AddFrameworkToProject(guid, "JavaScriptCore.framework", false);
            proj.AddFrameworkToProject(guid, "CoreTelephony.framework", false);

            //Copy Connector
            CopyAndReplaceDirectory("Sdk/iOS/XinkuaiMHSDK/Source", Path.Combine(path, "Libraries/XinkuaiMHSDK/Source"));
            AddFolderToBuild(proj, guid, path, "Libraries/XinkuaiMHSDK/Source");


            //AddBuildProperty
            proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libsqlite3.0.tbd", "Frameworks/libsqlite3.0.tbd", PBXSourceTree.Sdk));
            proj.AddBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC -lstdc++");
            proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Libraries/XinkuaiMHSDK");

            proj.WriteToFile(project_file_name);

            // Get plist
            PlistDocument kyPlist = new PlistDocument();
            string info_plist_file_name = Path.Combine(path, "Info.plist");
            kyPlist.ReadFromFile(info_plist_file_name);


            // Get root
            PlistElementDict kyRootDict = kyPlist.root;

            PlistElementDict allowsDict = kyRootDict.CreateDict("NSAppTransportSecurity");
            allowsDict.SetBoolean("NSAllowsArbitraryLoads", true);

            kyRootDict.SetString("ELandscape", "YES");
            kyRootDict.SetString("AppId", "1799");
            kyRootDict.SetString("AppKey", "qviisi3yrz8h9l8h764x75231vts4fvh");
            kyRootDict.SetString("NSCameraUsageDescription", "需要访问相机，保存用户名密码");
            kyRootDict.SetString("NSPhotoLibraryAddUsageDescription", "需要访问相册，保存用户名密码");
            kyRootDict.SetString("NSPhotoLibraryUsageDescription", "需要访问相册，保存用户名密码");
            kyRootDict.SetString("NSUserTrackingUsageDescription", "允许App请求权限以在拥有的App跟踪您的活动");


            PlistElementArray queriesSchemes = kyRootDict.CreateArray("LSApplicationQueriesSchemes");
            queriesSchemes.AddString("mqq");


            kyPlist.WriteToFile(info_plist_file_name);
        }

        private static void ProcessAoshitangSDK(string path)
        {
            UnityEngine.Debug.Log("----------ProcessAoshitangSDK111111111");
            PBXProject proj = new PBXProject();
            string project_file_name = PBXProject.GetPBXProjectPath(path);
            proj.ReadFromFile(project_file_name);
            string guid = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
            
            #region ASTSDK
            CopyAndReplaceDirectory("Sdk/iOS/ASTSDK/ASTSDK.framework", Path.Combine(path, "Libraries/ASTSDK/ASTSDK.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/ASTSDK/ASTSDK.framework", "Libraries/ASTSDK/ASTSDK.framework", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/ASTSDK/astsdk_icons.bundle", Path.Combine(path, "Libraries/ASTSDK/astsdk_icons.bundle"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/ASTSDK/astsdk_icons.bundle", "Libraries/ASTSDK/astsdk_icons.bundle", PBXSourceTree.Source));
            #endregion

            #region FBSDK
            CopyAndReplaceDirectory("Sdk/iOS/ASTSDK/FBSDKCoreKit.framework", Path.Combine(path, "Libraries/ASTSDK/FBSDKCoreKit.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/ASTSDK/FBSDKCoreKit.framework", "Libraries/ASTSDK/FBSDKCoreKit.framework", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/ASTSDK/FBSDKGamingServicesKit.framework", Path.Combine(path, "Libraries/ASTSDK/FBSDKGamingServicesKit.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/ASTSDK/FBSDKGamingServicesKit.framework", "Libraries/ASTSDK/FBSDKGamingServicesKit.framework", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/ASTSDK/FBSDKLoginKit.framework", Path.Combine(path, "Libraries/ASTSDK/FBSDKLoginKit.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/ASTSDK/FBSDKLoginKit.framework", "Libraries/ASTSDK/FBSDKLoginKit.framework", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/ASTSDK/FBSDKShareKit.framework", Path.Combine(path, "Libraries/ASTSDK/FBSDKShareKit.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/ASTSDK/FBSDKShareKit.framework", "Libraries/ASTSDK/FBSDKShareKit.framework", PBXSourceTree.Source));
            #endregion

            #region GOOGLESDK
            CopyAndReplaceDirectory("Sdk/iOS/ASTSDK/GoogleSignIn.bundle", Path.Combine(path, "Libraries/ASTSDK/GoogleSignIn.bundle"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/ASTSDK/GoogleSignIn.bundle", "Libraries/ASTSDK/GoogleSignIn.bundle", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/ASTSDK/GoogleSignIn.framework", Path.Combine(path, "Libraries/ASTSDK/GoogleSignIn.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/ASTSDK/GoogleSignIn.framework", "Libraries/ASTSDK/GoogleSignIn.framework", PBXSourceTree.Source));

            CopyAndReplaceDirectory("Sdk/iOS/ASTSDK/GoogleSignInDependencies.framework", Path.Combine(path, "Libraries/ASTSDK/GoogleSignInDependencies.framework"));
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/ASTSDK/GoogleSignInDependencies.framework", "Libraries/ASTSDK/GoogleSignInDependencies.framework", PBXSourceTree.Source));
            #endregion
            
            #region AddLocalization
            CopyAndReplaceDirectory("Sdk/iOS/ASTSDK/ASTSDKLocalization.bundle", Path.Combine(path, "Libraries/ASTSDK/ASTSDKLocalization.bundle"));
            //AddLocalizationFolderToBuild(proj, path, "Libraries/ASTSDK/ASTSDKLocalization.bundle"); 无效
            proj.AddFileToBuild(guid, proj.AddFile("Libraries/ASTSDK/ASTSDKLocalization.bundle", "Libraries/ASTSDK/ASTSDKLocalization.bundle", PBXSourceTree.Source));
            #endregion

            #region Dependency
            //Add System tdb
            proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));
            proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libc++.tbd", "Frameworks/libc++.tbd", PBXSourceTree.Sdk));
            //Add depende FrameWork
            //google
            proj.AddFrameworkToProject(guid, "LocalAuthentication.framework", false);
            proj.AddFrameworkToProject(guid, "SafariServices.framework", false);
            proj.AddFrameworkToProject(guid, "SystemConfiguration.framework", false);
            proj.AddFrameworkToProject(guid, "AuthenticationServices.framework", false);
            //gamecenter/appstore
            proj.AddFrameworkToProject(guid, "StoreKit.framework", false);
            proj.AddFrameworkToProject(guid, "GameKit.framework", false);
            //facebook
            proj.AddFrameworkToProject(guid, "Accelerate.framework", false);
            proj.AddFrameworkToProject(guid, "Webkit.framework", false);
            #endregion

            #region Source
            CopyAndReplaceDirectory("Sdk/iOS/ASTSDK/Source", Path.Combine(path, "Libraries/ASTSDK/Source"));
            AddFolderToBuild(proj, guid, path, "Libraries/ASTSDK/Source");
            #endregion

            #region AddBuildProperty
            // proj.AddFileToBuild(guid, proj.AddFile("usr/lib/libsqlite3.0.tbd", "Frameworks/libsqlite3.0.tbd", PBXSourceTree.Sdk));
            // proj.AddBuildProperty(guid, "OTHER_LDFLAGS", "-ObjC");
            //proj.SetBuildProperty(guid, "ENABLE_BITCODE", "NO");
            proj.AddBuildProperty(guid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Libraries/ASTSDK");
            #endregion

            proj.WriteToFile(project_file_name);

            #region info.plist
            //获取info.plist的rootNode
            PlistDocument kyPlist = new PlistDocument();
            string info_plist_file_name = Path.Combine(path, "Info.plist");
            kyPlist.ReadFromFile(info_plist_file_name);
            var dict_root = kyPlist.root;
            var arr_urltypes = dict_root.CreateArray("CFBundleURLTypes");

            //NSAllowsArbitraryLoads
            var allowsDict = dict_root.CreateDict("NSAppTransportSecurity");
            allowsDict.SetBoolean("NSAllowsArbitraryLoads", true);

            //fb
            {
                var dict_urlschemes = arr_urltypes.AddDict();
                var arr_urlschemes = dict_urlschemes.CreateArray("CFBundleURLSchemes");
                arr_urlschemes.AddString("fb435128011183100");
                dict_root.SetString("FacebookAppID", "435128011183100");
                dict_root.SetString("FacebookDisplayName", "Divinity's Rise");

                var queriesSchemes = dict_root.CreateArray("LSApplicationQueriesSchemes");
                queriesSchemes.AddString("fbapi");
                queriesSchemes.AddString("fb-messenger-share-api");
                queriesSchemes.AddString("fbauth2");
                queriesSchemes.AddString("fbshareextension");
            }

            //google
            {
                var dict_urlschemes = arr_urltypes.AddDict();
                dict_urlschemes.SetString("CFBundleTypeRole", "Editor");
                var arr_urlschemes = dict_urlschemes.CreateArray("CFBundleURLSchemes");
                arr_urlschemes.AddString("com.googleusercontent.apps.17708967760-na11h6tennuvkijvt7bb0t25ngrpv1ua");
                dict_urlschemes = arr_urltypes.AddDict();
                dict_urlschemes.SetString("CFBundleTypeRole", "Editor");
                arr_urlschemes = dict_urlschemes.CreateArray("CFBundleURLSchemes");
                arr_urlschemes.AddString("Prefs");
            }

            //final save
            kyPlist.WriteToFile(info_plist_file_name);
            #endregion
        }
#endregion

#region Other Private
        private static void DeployData(string path)
        {
            var start = System.DateTime.Now;

            string basePath = BMUtility.InterpretPath("$(Personal)/LTSites/", BuildPlatform.IOS);
            string outputPath = BuildConfiger.InterpretedOutputPath;
            string shortPath = "Data/Raw/AssetBundles/" + outputPath.Substring(basePath.Length);

            string sourcePath = outputPath;
            string destinationPath = Path.Combine(path, shortPath);

            if (Directory.Exists(destinationPath))
            {
                UnityEngine.Debug.Log("AssetBundles already exists");
            }
            else
            {
                Directory.CreateDirectory(destinationPath);
                foreach (string s in Directory.GetFiles(sourcePath))
                {
                    string fileName = System.IO.Path.GetFileName(s);
                    string destFile = System.IO.Path.Combine(destinationPath, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }

                UnityEngine.Debug.Log("Finished to deploy asset bundles to Data folder");

                //copy json files
                sourcePath = Path.GetDirectoryName(sourcePath);
                destinationPath = Path.GetDirectoryName(destinationPath);

                foreach (string s in Directory.GetFiles(sourcePath))
                {
                    string fileName = System.IO.Path.GetFileName(s);
                    string destFile = System.IO.Path.Combine(destinationPath, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
                UnityEngine.Debug.Log("Finished to deploy json config files to Data folder");
            }

            var diff = System.DateTime.Now - start;
            UnityEngine.Debug.Log("Inject assets took: " + diff.ToString());
        }

        private static bool StringExists(PlistElementArray array, string value)
        {
            List<PlistElement> items = array.values ?? new List<PlistElement>();

            return items.Exists(e => e.AsString() == value);
        }

        private static bool KeyExists(PlistElementArray array, string key, string value)
        {
            List<PlistElement> items = array.values ?? new List<PlistElement>();

            return items.Exists(e => (e.AsDict()[key] ?? new PlistElementString(string.Empty)).AsString() == value);
        }

        /// <summary>
        /// written into the code at the specified location
        /// </summary>
        private static void WriteBelow(string varFilePath, string below, string text)
        {
            StreamReader streamReader = new StreamReader(varFilePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            if (text_all.IndexOf(text) >= 0)
            {
                return;
            }

            int beginIndex = text_all.IndexOf(below);
            if (beginIndex == -1)
            {
                UnityEngine.Debug.LogErrorFormat("PostProcess.ios.WriteBelow get beginIndex Failed, beginIndex = {0} ", beginIndex);
                return;
            }

            int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);
            if (endIndex == -1)
            {
                UnityEngine.Debug.LogErrorFormat("PostProcess.ios.WriteBelow get endIndex Failed, endIndex = {0} ", endIndex);
            }

            text_all = text_all.Substring(0, endIndex) + "\n" + text + "\n" + text_all.Substring(endIndex);

            StreamWriter streamWriter = new StreamWriter(varFilePath);
            streamWriter.Write(text_all);
            streamWriter.Close();
        }

        /// <summary>
        /// copy the file to the specified location
        /// </summary>
        private static void CopyAndReplaceDirectory(string srcPath, string dstPath)
        {
            if (Directory.Exists(dstPath))
            {
                Directory.Delete(dstPath, true);
            }

            if (File.Exists(dstPath))
            {
                File.Delete(dstPath);
            }

            Directory.CreateDirectory(dstPath);

            foreach (var file in Directory.GetFiles(srcPath))
            {
                if (Path.GetExtension(file) != ".meta")
                {
                    File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
                }
            }

            foreach (var dir in Directory.GetDirectories(srcPath))
            {
                CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
            }
        }

        /// <summary>
        /// add all files in folder (include all sub folder) to project
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="targetGuid"></param>
        /// <param name="projectPath"></param>
        /// <param name="subFolderPath"></param>
        private static void AddFolderToBuild(PBXProject proj, string targetGuid, string projectPath, string subFolderPath)
        {
            projectPath = projectPath.Replace("\\", "/");
            string folder = Path.Combine(projectPath, subFolderPath);
            foreach (var file in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
            {
                if (Path.GetExtension(file) != ".meta")
                {
                    string uniPath = file.Replace("\\", "/");
                    string relativePath = uniPath.Replace(projectPath, "").Trim(new char[] { '/' });
                    string fileGuid = proj.FindFileGuidByRealPath(relativePath, PBXSourceTree.Source);
                    if (string.IsNullOrEmpty(fileGuid))
                    {
                        proj.AddFileToBuild(targetGuid, proj.AddFile(relativePath, relativePath, PBXSourceTree.Source));
                    }
                }
            }
        }

        /// <summary>
        /// remove all files in folder (include all sub folder) from project
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="targetGuid"></param>
        /// <param name="projectPath"></param>
        /// <param name="subFolderPath"></param>
        private static void RemoveFolderFromBuild(PBXProject proj, string targetGuid, string projectPath, string subFolderPath)
        {
            projectPath = projectPath.Replace("\\", "/");
            string folder = Path.Combine(projectPath, subFolderPath);
            foreach (var file in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
            {
                if (Path.GetExtension(file) != ".meta")
                {
                    string uniPath = file.Replace("\\", "/");
                    string relativePath = uniPath.Replace(projectPath, "").Trim(new char[] { '/' });
                    string fileGuid = proj.FindFileGuidByRealPath(relativePath, PBXSourceTree.Source);
                    if (!string.IsNullOrEmpty(fileGuid))
                    {
                        proj.RemoveFileFromBuild(targetGuid, fileGuid);
                        proj.RemoveFile(fileGuid);
                    }
                }
            }
        }

        /// <summary>
        /// add all files in folder (include all sub folder) to project
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="targetGuid"></param>
        /// <param name="projectPath"></param>
        /// <param name="subFolderPath"></param>
        /// <param name="group"></param>
        private static void AddFolderToBuild(PBXProject proj, string targetGuid, string projectPath, string subFolderPath, string group)
        {
            projectPath = projectPath.Replace("\\", "/");
            string folder = Path.Combine(projectPath, subFolderPath).Replace("\\", "/");
            foreach (var file in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
            {
                if (Path.GetExtension(file) != ".meta")
                {
                    string uniPath = file.Replace("\\", "/");
                    string relativePath = uniPath.Replace(projectPath, "").Trim(new char[] { '/' });
                    string subPath = uniPath.Replace(folder, "").Trim(new char[] { '/' });
                    string groupPath = Path.Combine(group, subPath).Replace("\\", "/").Trim(new char[] { '/' });

                    string fileGuid = proj.FindFileGuidByRealPath(relativePath, PBXSourceTree.Source);
                    if (string.IsNullOrEmpty(fileGuid))
                    {
                        proj.AddFileToBuild(targetGuid, proj.AddFile(relativePath, groupPath, PBXSourceTree.Source));
                    }
                }
            }
        }

        // see: http://forum.unity3d.com/threads/pbxproject-and-localizable-ressources.376626/
        // see: https://bitbucket.org/Unity-Technologies/xcodeapi/pull-requests/13/creation-of-variantgroup-and/diff
        private static void AddLocalizationFolderToBuild(PBXProject proj, string projectPath, string localizationSubPath)
        {
            string localizationPath = Path.Combine(projectPath, localizationSubPath);

            foreach (var dir in Directory.GetDirectories(localizationPath))
            {
                if (Path.GetExtension(dir) == ".lproj")
                {
                    foreach (var file in Directory.GetFiles(dir))
                    {
                        proj.AddLocalization(localizationSubPath, Path.GetFileName(dir), Path.GetFileName(file));
                    }
                }
            }
        }
#endregion
    }
}