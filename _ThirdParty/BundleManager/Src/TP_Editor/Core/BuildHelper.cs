using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#pragma warning disable 0618

/**
 * Build helper contains APIs for bundle building.
 * You can use this to custom your own build progress.
 */ 
public class BuildHelper 
{
	/**
	 * Copy the configuration files to target directory.
	 */ 
	public static void ExportBMDatasToOutput()
	{
		string exportPath = BuildConfiger.InterpretedOutputPath;
		if(!Directory.Exists(exportPath))
			Directory.CreateDirectory(exportPath);

		uint crc = 0;
		if(!BuildAssetBundle(new string[]{BMDataAccessor.BundleDataPath, BMDataAccessor.BundleBuildStatePath, BMDataAccessor.BMConfigerPath}, Path.Combine( exportPath, "BM.data" ), out crc))
			Debug.LogError("Failed to build bundle of config files.");

		BuildHelper.ExportBundleDataFileToOutput();
		BuildHelper.ExportBundleBuildDataFileToOutput();
		BuildHelper.ExportBMConfigerFileToOutput();
		BuildHelper.ExportBundleShipInfoFileToOutput();
	}
	
	/**
	 * Copy the bundle datas to target directory.
	 */ 
	public static void ExportBundleDataFileToOutput()
	{
		string exportPath = BuildConfiger.InterpretedOutputPath;
		if(!Directory.Exists(exportPath))
			Directory.CreateDirectory(exportPath);

		File.Copy( 	BMDataAccessor.BundleDataPath, 
					Path.Combine( exportPath, Path.GetFileName(BMDataAccessor.BundleDataPath) ), 
					true );
	}
	
	/**
	 * Copy the bundle build states to target directory.
	 */ 
	public static void ExportBundleBuildDataFileToOutput()
	{
		string exportPath = BuildConfiger.InterpretedOutputPath;
		if(!Directory.Exists(exportPath))
			Directory.CreateDirectory(exportPath);

		File.Copy( 	BMDataAccessor.BundleBuildStatePath, 
					Path.Combine( exportPath, Path.GetFileName(BMDataAccessor.BundleBuildStatePath) ), 
					true );
	}

	public static void ExportBundleShipInfoFileToOutput()
	{
		string exportPath = BuildConfiger.InterpretedOutputPath;
		if(!Directory.Exists(exportPath))
			Directory.CreateDirectory(exportPath);
			
		File.Copy(	BMDataAccessor.BundleShipInfoPath,
					Path.Combine( exportPath, Path.GetFileName(BMDataAccessor.BundleShipInfoPath) ),
					true );
	}
	
	/**
	 * Copy the bundle manager configeration file to target directory.
	 */ 
	public static void ExportBMConfigerFileToOutput()
	{
		string exportPath = BuildConfiger.InterpretedOutputPath;
		if(!Directory.Exists(exportPath))
			Directory.CreateDirectory(exportPath);

		File.Copy( 	BMDataAccessor.BMConfigerPath, 
					Path.Combine( exportPath, Path.GetFileName(BMDataAccessor.BMConfigerPath) ), 
					true );
	}
	
	/**
	 * Detect if the bundle need update.
	 */ 
	public static bool IsBundleNeedBunild(BundleData bundle)
	{
		if (bundle.name.StartsWith("+"))
		{
			return false;
		}

		string outputPath = GenerateOutputPathForBundle(bundle.name);
		if(!File.Exists(outputPath))
		{
			outputPath = Path.ChangeExtension(outputPath, "cz");
			if (!File.Exists(outputPath))
			{
				return true;
			}
		}

		BundleBuildState bundleBuildState = BundleManager.GetBuildStateOfBundle(bundle.name);

		FileInfo fi = new FileInfo(outputPath);
		if (fi.Length != bundleBuildState.size)
		{
			return true;
		}
		
		DateTime lastBuildTime = /*File.GetLastWriteTime(outputPath);*/fi.LastWriteTime;
		DateTime bundleChangeTime = bundleBuildState.changeTime == -1 ? DateTime.MaxValue : DateTime.FromBinary(bundleBuildState.changeTime);
		if( System.DateTime.Compare(lastBuildTime, bundleChangeTime) < 0 )
		{
			return true;
		}
		
		string[] assetPaths = GetAssetsFromPaths(BundleManager.GUIDsToPaths(bundle.includeGUIDs.ToArray()), bundle.sceneBundle);
		string[] dependencies = AssetDatabase.GetDependencies(assetPaths);
		if( !EqualStrArray(dependencies, bundleBuildState.lastBuildDependencies) )
			return true; // Build depenedencies list changed.
		
		foreach(string file in dependencies)
		{
			if(DateTime.Compare(lastBuildTime, File.GetLastWriteTime(file)) < 0)
				return true;
		}
		
		if(bundle.parent != "")
		{
			BundleData parentBundle = BundleManager.GetBundleData(bundle.parent);
			if(parentBundle != null)
			{
                if (IsBundleNeedBunild(parentBundle))
                {
                    return true;
                }
            }
			else
			{
				Debug.LogError("Cannot find bundle");
			}
		}
		
		return false;
	}
	
	/**
	 * Build all bundles.
	 */
	public static void BuildAll()
	{	
		BuildBundles(BundleManager.bundles.Select(bundle=>bundle.name).ToArray());
	}
	
	/**
	 * Force rebuild all bundles.
	 */
	public static void RebuildAll()
	{
		foreach(BundleBuildState bundle in BundleManager.buildStates)
			bundle.lastBuildDependencies = null;
		
		BuildAll();
	}
	
	/**
	 * Build bundles.
	 */
	public static void BuildBundles(string[] bundles)
	{
		Dictionary<string, List<string>> buildingRoutes = new Dictionary<string, List<string>>();
		foreach(string bundle in bundles)
			AddBundleToBuildList(bundle, ref buildingRoutes);
		
		foreach(var buildRoute in buildingRoutes)
		{
			BundleData bundle = BundleManager.GetBundleData( buildRoute.Key );
			if(bundle != null)
				BuildBundleTree(bundle, buildRoute.Value);
		}
	}

	public static void BuildAllLevels()
	{
		string absolutePath = Application.dataPath + "/_GameAssets/Res/Environment/";
		string resourcePath = "Assets/_GameAssets/Res/Environment/";

		BuildAllLevelsInFolder(absolutePath, resourcePath);

		Debug.Log("[BuildHelper]BuildAllLevels: All levels are done.");

		//for all levels
			//open the level
			//wait for it to open
			//run BuildLevel
	}

	static void BuildAllLevelsInFolder(string absolutePath, string resourcePath)
	{
		string[] files = Directory.GetFiles(absolutePath);
		for(int i = 0; i < files.Length; ++i)
		{
			FileInfo info = new FileInfo(files[i]);
			string extension = info.Extension.ToUpper();
			if(extension == ".UNITY")
			{
				string path = resourcePath + info.Name;
				EditorApplication.OpenScene(path);
				BuildLevel();
			}
		}

		string[] folders = Directory.GetDirectories(absolutePath);
		for(int i = 0; i < folders.Length; ++i)
		{
			DirectoryInfo info = new DirectoryInfo(folders[i]);
			BuildAllLevelsInFolder(absolutePath + info.Name + "/", resourcePath + info.Name + "/");
		}
	}

	public static void CreateAssetBundlesFromSelection(UnityEngine.Object[] objects, string bundleParent)
	{
		if(objects == null || objects.Length == 0)
		{
			return;
		}
		float progress = 0.0f;
		foreach(UnityEngine.Object obj in objects)
		{
			string assetPath = AssetDatabase.GetAssetPath(obj);
			if(string.IsNullOrEmpty(assetPath))
			{
				continue;
			}

			if(EditorUtility.DisplayCancelableProgressBar("Create AssetBundles from selection", string.Format("Adding {0} to bundle manager", assetPath), ++progress / objects.Length))
			{
				EditorUtility.ClearProgressBar();
				return;
			}
			string assetName = obj.name;
			if(BundleManager.GetBundleData(assetName) == null)
			{
				BundleManager.CreateNewBundle(assetName, bundleParent, false);
			}
			
			if(BundleManager.CanAddPathToBundle(assetPath, assetName))
			{
				BundleManager.AddPathToBundle(assetPath, assetName);
			}
		}

		EditorUtility.ClearProgressBar();
	}

	//you can use this to make prefabs & assetbundles, or just prefabs
	public static void CreateAssetBundlesFromScene(bool do_assetbundling = true) 
	{
		//BundleData[] bundles = BundleManager.bundles.Select();
		string[] names = BundleManager.bundles.Select(bundle=>bundle.name).ToArray();
		string active_level = EditorApplication.currentScene;

		active_level = System.IO.Path.GetFileName(active_level);
		string[] levelsplit = active_level.Split('.');
		active_level = levelsplit[0];

		//if scene doesnt have an assetbundle folder
		bool level_has_assetbundle_folder = false;
		for(int i = 0; i < names.Length; i++)
		{
			if(string.Equals(names[i], active_level))
			{
				level_has_assetbundle_folder = true;
				break;
			}
		}
		
		//create a scene assetbundle folder
		if(!level_has_assetbundle_folder)
		{
			CreateFolderNode("+" + active_level);
		}

		//get all the gameobjects that should be assetbundles in the scene
		GameObject[] obj = (GameObject[])GameObject.FindSceneObjectsOfType(typeof (GameObject));
		List<GameObject> assetbundle_list = new List<GameObject>();
		for(int i = 0; i < obj.Length; i++)
		{
			if(obj[i].GetComponent<AssetBundleHelper>() != null)
			{
				assetbundle_list.Add(obj[i]);
			}
		}

		//does the prefab exits?
		for(int i = 0; i < assetbundle_list.Count; i++)
		{
			GameObject assetbundle_obj = assetbundle_list[i];

			string prefab_path = "Assets/_GameAssets/Res/Environment/" + active_level + "/Prefabs/" + assetbundle_list[i].name + ".prefab";


			if(!Directory.Exists("Assets/_GameAssets/Res/Environment/" + active_level + "/Prefabs/"))
			{
				AssetDatabase.CreateFolder("Assets/_GameAssets/Res/Environment/" + active_level, "Prefabs");
			}

			//needs a prefab		
			//if(!AssetDatabase.Contains(assetbundle_list[i]))
			{
				//create a prefab!
				Transform parent = assetbundle_obj.transform.parent;
				Vector3 local_pos = assetbundle_obj.transform.localPosition;
				Vector3 local_scale = assetbundle_obj.transform.localScale;
				Quaternion local_rotation = assetbundle_obj.transform.localRotation;

				GameObject new_prefab = PrefabUtility.CreatePrefab(prefab_path, assetbundle_obj);				
				GameObject.DestroyImmediate(assetbundle_obj);

				GameObject new_obj = (GameObject)EditorUtility.InstantiatePrefab(new_prefab); 
				new_obj.transform.localPosition = local_pos;
				new_obj.transform.localScale = local_scale;
				new_obj.transform.localRotation = local_rotation;
				new_obj.transform.parent = parent;

				assetbundle_obj = new_obj;
			}

			if(do_assetbundling)
			{
				if(BundleManager.GetBundleData(assetbundle_obj.name) == null)
				{
					BundleManager.CreateNewBundle(assetbundle_obj.name, "+" + active_level, false);
				}
				
				//add the new prefab to the assetbundle
				if(BundleManager.CanAddPathToBundle(prefab_path, assetbundle_obj.name))
				{
					BundleManager.AddPathToBundle(prefab_path, assetbundle_obj.name);
				}
			}
		}
	}
	
	public static void CreateFolderNode(string name)
	{
		bool created = BundleManager.CreateNewBundle(name, "", false);
		if (!created)
		{
			Debug.LogWarning("Create Folder Node Failed");
		}
	}

	public static void BuildForDeploy(bool rebuild)
	{
		string outputPath = BuildConfiger.InterpretedOutputPath;
		if( !Directory.Exists(outputPath) )
		{
			Directory.CreateDirectory(outputPath);
		}

		// build bundles
		if (rebuild)
		{
			// rebuild ship info
			BMDataAccessor.BundleShipInfos.Clear();
			BuildHelper.RebuildAll();
		}
		else
		{
			BuildHelper.BuildAll();
		}
		BuildHelper.ExportBMDatasToOutput();
		BuildHelper.ExportHudConfig();
		BuildHelper.ExportSceneLoadConfig();
			
		// build all levels
		BuildHelper.BuildAllLevels();

		// update Item Priority Window
		if (ItemPriorityEditor.IsOpen)
		{
			ItemPriorityEditor.Instance.LoadBundleShipInfoFile();
		}
		else
		{
			EditorWindow.GetWindow<ItemPriorityEditor>().LoadBundleShipInfoFile();
			EditorWindow.GetWindow<ItemPriorityEditor>().SortByTreeAndPriority();
			EditorWindow.GetWindow<ItemPriorityEditor>().SaveBundleShipInfoFile();
			EditorWindow.GetWindow<ItemPriorityEditor>().Close();
		}
	}

	static void ExportHudConfig()
	{
		if(BMDataAccessor.HudConfigPath != null)
		{
			for(int i = 0; i < BMDataAccessor.HudConfigPath.Length; ++i)
			{
				string environment = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				string destPath = Path.Combine(environment + "/LTSites/" + EB.Version.GetVersion() + "/", Path.GetFileName(BMDataAccessor.HudConfigPath[i]));
				
				if(File.Exists(destPath))
				{
					File.SetAttributes(destPath, FileAttributes.Normal);
				}
				
				File.Copy(BMDataAccessor.HudConfigPath[i], destPath, true);
			}
		}
	}

	static void ExportSceneLoadConfig()
	{
		if(BMDataAccessor.SceneLoadConfigPath != null)
		{
			
			for(int i = 0; i < BMDataAccessor.SceneLoadConfigPath.Length; ++i)
			{
				string environment = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				string destPath = Path.Combine(environment + "/LTSites/" + EB.Version.GetVersion() + "/", Path.GetFileName(BMDataAccessor.SceneLoadConfigPath[i]));
				
				if(File.Exists(destPath))
				{
					File.SetAttributes(destPath, FileAttributes.Normal);
				}
				
				File.Copy(BMDataAccessor.SceneLoadConfigPath[i], destPath, true);
			}
		}
	}

	public static void BuildLevel()
	{
		//get all objects in level
		//if they are assetbundles output them in a json file
		//string[] names = BundleManager.bundles.Select(bundle=>bundle.name).ToArray();

		string output = "[\n";

		GameObject[] obj = (GameObject[])GameObject.FindSceneObjectsOfType(typeof (GameObject));
		Dictionary<GameObject,bool> assetbundle_dic = new Dictionary<GameObject,bool>();
		for(int i = 0; i < obj.Length; i++)
		{
			if(obj[i].GetComponent<AssetBundleHelper>() != null)
			{
				assetbundle_dic.Add(obj[i],obj[i].GetComponent<AssetBundleHelper>().m_blocking);
			}
		}

		for(int i = 0; i < assetbundle_dic.Count; i++)
		{
			GameObject go = assetbundle_dic.Keys.ElementAt(i);
			bool isBlocking = assetbundle_dic.Values.ElementAt(i);

			output += "\t{\n";

			string name = "\t\t\"name\" : \"" + go.name + "\",";
			//public string type;
			//public string defaultAsset;	// must be in Resource folder and shipped within App
			string parent = "\t\t\"parent\" : \"" + (go.transform.parent != null ? go.transform.parent.name : "null") + "\",";
			string localPosition = "\t\t\"localPosition\" : \"" + go.transform.localPosition.ToString() + "\",";
			string localScale = "\t\t\"localScale\" : \"" + go.transform.localScale.ToString() + "\",";
			string localRotation = "\t\t\"localRotation\" : \"" + go.transform.localRotation.ToString() + "\",";
			string blocking = "\t\t\"blocking\" : " + isBlocking.ToString().ToLower();
			output += name + "\n";
			output += parent + "\n";
			output += localPosition + "\n";
			output += localScale + "\n";
			output += localRotation + "\n";
			output += blocking + "\n";

			if(i == assetbundle_dic.Count-1)
			{
				output += "\t}\n";
			}
			else
			{
				output += "\t},\n";
			}
		}

			//if this thing is a assetbundle
				//1) Make sure it has a assetbundle component
				//2) Write it's info to a scene file

		output += "]";
		
		string active_level = EditorApplication.currentScene;
		active_level = System.IO.Path.GetFileName(active_level);
		string s = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		string save_level_json = "";
		#if UNITY_EDITOR
		save_level_json = s + "/LTSites/" + EB.Version.GetVersion() + "/" + active_level + ".json";
		#endif

		System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("ISO-8859-1"); //Or any other Encoding		
		using (FileStream fs = new FileStream(save_level_json, System.IO.FileMode.Create))
		{
			using (StreamWriter writer = new StreamWriter(fs, encoding))
			{
				writer.Write(output);
				writer.Flush();
				writer.Close();
			}
		}

		Debug.Log (output);
	}
	
    /// <summary>
    /// 压缩指定路径的Bundle文件为CZ格式
    /// </summary>
    /// <param name="bundlePath"></param>
    /// <param name="deleteSource"></param>
    /// <returns></returns>
	private static bool CompressBundle(ref string bundlePath, bool deleteSource = false)
	{
		string zipFilePath = Path.ChangeExtension(bundlePath, "cz");

		FileStream fin = null;
		FileStream fout = null;
		//ComponentAce.Compression.Libs.zlib.ZOutputStream zout = null;
		HTTP.Zlib.ZlibStream zout = null;

		bool result = false;

		int BUFFER_SIZE = 8192;

		try
		{
			if (File.Exists(zipFilePath))
			{
				File.Delete(zipFilePath);
			}

			fin = new FileStream(bundlePath, FileMode.Open, FileAccess.Read);
			fout = new FileStream(zipFilePath, FileMode.CreateNew, FileAccess.Write);
			// move Plugins/zlib to Plugins/Editor/zlib to reduce package size
			//zout = new ComponentAce.Compression.Libs.zlib.ZOutputStream(fout, -1);
			zout = new HTTP.Zlib.ZlibStream(fout, HTTP.Zlib.CompressionMode.Compress);

			int n = 0;
			byte[] buffer = new byte[BUFFER_SIZE];
			do
			{
				n = fin.Read(buffer, 0, BUFFER_SIZE);
				zout.Write(buffer, 0, n);
			} while (n > 0);

//			using (Ionic.Zip.ZipFile zp = new Ionic.Zip.ZipFile(zipFilePath))
//			{
//				zp.AddFile(bundlePath, ".");
//				zp.Save();
//			}

			if (deleteSource)
			{
				if (fin != null)
				{
					fin.Close();
				}
				System.IO.File.Delete(bundlePath);
			}

			result = true;
		}
		catch (Exception eError)
		{
			Debug.LogException(eError);
			Debug.LogError(System.String.Format("Compress {0} to {1} failed. Fatal Error!", bundlePath, zipFilePath));
			result = false;
		}
		finally
		{
			if (zout != null)
			{
				zout.Flush();
				zout.Close();
			}
			if (fin != null)
			{
				fin.Close();
			}
			if (fout != null)
			{
				fout.Close();
			}

			bundlePath = zipFilePath;
		}

		return result;
	}
	
	public static void SetAssetBundleBuildOutput()
	{
		BuildPlatform build_platform = BuildConfiger.BundleBuildTarget;

		string build_target_string = build_platform.ToString();
		//string build_output = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		string build_output = "$(Personal)";
		string download_url = "";

		// Joe Li: don't use '~', it only works in shell.
		build_output += "/LTSites/" + "$(Version)/$(Platform)";
		download_url += build_target_string;

		if (BuildConfiger.BuildOutputStr != build_output || DownloadConfiger.downloadUrl != download_url)
		{
			DownloadConfiger.downloadUrl = download_url;
			BuildConfiger.BuildOutputStr = build_output;			

			EB.Debug.Log("Output: {0}=>{1}", build_output ,BuildConfiger.InterpretedOutputPath);
			EB.Debug.Log("Download: {0}", download_url);
		}
	}	

	public static void DeployDataToStreamingAssetsFolder() {
		string sourcePath = 
			Environment.GetFolderPath (Environment.SpecialFolder.Personal) 
				+ "/LTSites/"
				+ EB.Version.GetVersion() 
				+ "/"
				+ BuildConfiger.BundleBuildTarget.ToString () 
				+ "/";

		string destinationPath = 
			Application.streamingAssetsPath + "/" 
				+ GM.AssetUtils.STREAMING_ASSETS_BUNDLE_FOLDER_NAME 
				+ "/" + EB.Version.GetVersion()
				+ "/"
				+ BuildConfiger.BundleBuildTarget.ToString ()
				+ "/";

		if (Directory.Exists (destinationPath))
			Directory.Delete (destinationPath, true);
		
		Directory.CreateDirectory (destinationPath);

		foreach (string s in Directory.GetFiles(sourcePath)) {
			string fileName = System.IO.Path.GetFileName(s);
			string destFile = System.IO.Path.Combine(destinationPath, fileName);
			System.IO.File.Copy(s, destFile, true);
		}

		Debug.Log("Finished to deploy asset bundles to StreamingAssets folder");

		//copy json files
		sourcePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) 
			+ "/LTSites/"
			+ EB.Version.GetVersion()
			+ "/";
		destinationPath = 
			Application.streamingAssetsPath + "/"
			+ GM.AssetUtils.STREAMING_ASSETS_BUNDLE_FOLDER_NAME
			+ "/" + EB.Version.GetVersion()
			+ "/";

		foreach (string s in Directory.GetFiles(sourcePath)) {
			string fileName = System.IO.Path.GetFileName(s);
			string destFile = System.IO.Path.Combine(destinationPath, fileName);
			System.IO.File.Copy(s, destFile, true);
		}
		Debug.Log("Finished to deploy json config files to StreamingAssets folder");

		AssetDatabase.Refresh();
	}

	public static void DeployVideoToStreamingAssetsFolder()
	{
		//NONE
	}

	///根据给定size，获取要装入的bundle
	public static List<GM.BundleInfo> GetBundlesByPriority(long size)
	{
		List<GM.BundleInfo> assets = new List<GM.BundleInfo>();
		foreach (var info in BMDataAccessor.BundleShipInfos)
		{
			long theBundleSize = info.Size;
			long willLeftSize = size - theBundleSize;
			if(willLeftSize >= 0 && info.Size > 0)
			{
				// UnityEngine.Debug.LogError($"GetBundlesByPriority====>size: {size}==info.size: {theBundleSize}==willLeftSize： {willLeftSize}");
				assets.Add(info);
				size = willLeftSize;
			}
		}
		return assets;
	}

	internal static void AddBundleToBuildList(string bundleName, ref Dictionary<string, List<string>> buildingRoutes)	
	{
		BundleData bundle = BundleManager.GetBundleData(bundleName);
		if(bundle == null)
		{
			EB.Debug.LogError("Cannot find bundle {0}" , bundleName);
			return;
		}
			
		if( BuildHelper.IsBundleNeedBunild(bundle) )
		{
			string rootName = BundleManager.GetRootOf(bundle.name);
			if(buildingRoutes.ContainsKey(rootName))
			{
				if(!buildingRoutes[rootName].Contains(bundle.name))
					buildingRoutes[rootName].Add(bundle.name);
				else
					EB.Debug.LogError("Bundle name duplicated: {0}" , bundle.name);
			}
			else
			{
				List<string> buildingList = new List<string>();
				buildingList.Add(bundle.name);
				buildingRoutes.Add(rootName, buildingList);
			}
		}
		else
		{
			EB.Debug.Log("Bundle {0} skiped." , bundle.name);
		}
	}
	
	internal static bool BuildBundleTree(BundleData bundle, List<string> requiredBuildList)
	{
		BuildPipeline.PushAssetDependencies();

		bool succ = false;
		if (bundle.name.StartsWith("+"))
		{
			succ = true;
			EB.Debug.Log("Skip Folder Node {0}" , bundle.name);
		}
		else
		{
			succ = BuildSingleBundle(bundle);
			if(!succ)
			{
				EB.Debug.LogError("{0} build failed.", bundle.name);
				BuildPipeline.PopAssetDependencies();
				return false;
			}
			else
			{
				EB.Debug.Log("{0} build succeed.", bundle.name);
			}
		}
		
		foreach(string childName in bundle.children)
		{
			BundleData child = BundleManager.GetBundleData(childName);
			if(child == null)
			{
				EB.Debug.LogError("Cannnot find bundle [{0}]. Sth wrong with the bundle config data.", childName);
				BuildPipeline.PopAssetDependencies();
				return false;
			}
			
			bool isDependingBundle = false;
			foreach(string requiredBundle in requiredBuildList)
			{
				if(BundleManager.IsBundleDependOn(requiredBundle, childName))
				{
					isDependingBundle = true;
					break;
				}
			}
			
			if(isDependingBundle || !BuildConfiger.DeterministicBundle)
			{
				succ = BuildBundleTree(child, requiredBuildList);
				if(!succ)
				{
					BuildPipeline.PopAssetDependencies();
					return false;
				}
			}
		}
		
		BuildPipeline.PopAssetDependencies();
		return true;
	}
	
	// Get scene or plain assets from include paths
	internal static string[] GetAssetsFromPaths(string[] includeList, bool onlySceneFiles)
	{
		// Get all the includes file's paths
		List<string> files = new List<string>();
		foreach(string includPath in includeList)
		{
			files.AddRange(GetAssetsFromPath(includPath, onlySceneFiles));
		}
		
		return files.ToArray();
	}

	// Get scene or plain assets from path
	internal static string[] GetAssetsFromPath(string path, bool onlySceneFiles)
	{
		if(!File.Exists(path) && !Directory.Exists(path))
			return new string[]{};
		
		bool isDir = (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
		bool isSceneFile = Path.GetExtension(path) == ".unity";
		if(!isDir)
		{
			if(onlySceneFiles && !isSceneFile)
				// If onlySceneFiles is true, we can't add file without "unity" extension
				return new string[]{};
			
			return new string[]{path};
		}
		else
		{
			string[] subFiles = null;
			if(onlySceneFiles)
				subFiles = FindSceneFileInDir(path, SearchOption.AllDirectories);
			else
				subFiles = FindAssetsInDir(path, SearchOption.AllDirectories);
			
			return subFiles;
		}
	}
	
	private static string[] FindSceneFileInDir(string dir, SearchOption option)
	{
		return Directory.GetFiles(dir, "*.unity", option);
	}
	
	private static string[] FindAssetsInDir(string dir, SearchOption option)
	{
		List<string> files = new List<string>( Directory.GetFiles(dir, "*.*", option) );
		files.RemoveAll(x => x.EndsWith(".meta", System.StringComparison.OrdinalIgnoreCase) || x.EndsWith(".unity", System.StringComparison.OrdinalIgnoreCase));
		return files.ToArray();
	}
	
    /// <summary>
    /// 生成Bundle文件
    /// </summary>
    /// <param name="assetsList"></param>
    /// <param name="outputPath"></param>
    /// <param name="crc"></param>
    /// <returns></returns>
	private static bool BuildAssetBundle(string[] assetsList, string outputPath, out uint crc)
	{
		crc = 0;

        BuildAssetBundleOptions buildABOpts = CurrentBuildAssetOpts;

        // Load all of assets in this bundle
        List<UnityEngine.Object> assets = new List<UnityEngine.Object>();
		foreach(string assetPath in assetsList)
		{
            //如果assetsList中有mp4，则不能压缩
            if (assetPath.Contains("mp4"))
            {
                buildABOpts = BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }

			UnityEngine.Object[] assetsAtPath = AssetDatabase.LoadAllAssetsAtPath(assetPath);
			if(assetsAtPath != null || assetsAtPath.Length != 0)
				assets.AddRange(assetsAtPath);
			else
				EB.Debug.LogError("Cannnot load [{0}] as asset object", assetPath);
		}

		// Build bundle
		bool succeed = BuildPipeline.BuildAssetBundle(	null, 
														assets.ToArray(), 
														outputPath,
														out crc,
                                                        buildABOpts,
														BuildConfiger.UnityBuildTarget);

		if(!succeed){
			Debug.LogError($"BuildAssetBundle===>Failed!=={outputPath}");
		}

		return succeed;
	}

	private static BuildAssetBundleOptions CurrentBuildAssetOpts
	{
		get
		{
			return	(BMDataAccessor.BMConfiger.compress ? BuildAssetBundleOptions.ChunkBasedCompression : BuildAssetBundleOptions.UncompressedAssetBundle) |
					(BMDataAccessor.BMConfiger.deterministicBundle ? 0 : BuildAssetBundleOptions.DeterministicAssetBundle) |
					BuildAssetBundleOptions.CollectDependencies;
		}
	}
	
	private static bool BuildSceneBundle(string[] sceneList, string outputPath, out uint crc)
	{
		crc = 0;

		if(sceneList.Length == 0)
		{
			Debug.LogError("No scenes were provided for the scene bundle");
			return false;
		}

#if UNITY_4_2 || UNITY_4_1 || UNITY_4_0
		string error = BuildPipeline.BuildPlayer (sceneList, outputPath, BuildConfiger.UnityBuildTarget, BuildOptions.BuildAdditionalStreamedScenes | CurrentBuildSceneOpts);
#else
		string error = BuildPipeline.BuildStreamedSceneAssetBundle(sceneList, outputPath, BuildConfiger.UnityBuildTarget, out crc, CurrentBuildSceneOpts);
#endif
		return error == "";
	}

	private static BuildOptions CurrentBuildSceneOpts
	{
		get
		{
			return	BMDataAccessor.BMConfiger.compress ? 0 : BuildOptions.UncompressedAssetBundle;
		}
	}
	

    /// <summary>
    /// 根据数据生成对应的Bundle压缩文件
    /// </summary>
    /// <param name="bundle"></param>
    /// <returns></returns>
	private static bool BuildSingleBundle(BundleData bundle)
	{
		// Prepare bundle output dictionary
		string outputPath = GenerateOutputPathForBundle(bundle.name);
		string bundleStoreDir = Path.GetDirectoryName(outputPath);
		if(!Directory.Exists(bundleStoreDir))
			Directory.CreateDirectory(bundleStoreDir);
		if(File.Exists(outputPath))
			File.Delete(outputPath);
		
		// Start build
		string[] assetPaths = GetAssetsFromPaths(BundleManager.GUIDsToPaths(bundle.includeGUIDs.ToArray().Concat(bundle.exIncludeGUIDs.ToArray()).ToArray()), bundle.sceneBundle);
		bool succeed = false;
		uint crc = 0;
		if(bundle.sceneBundle)
			succeed = BuildSceneBundle(assetPaths, outputPath, out crc);
		else
			succeed = BuildAssetBundle(assetPaths, outputPath, out crc);

		if (succeed/* && !BMDataAccessor.BMConfiger.compress*/)
		{
			succeed = CompressBundle(ref outputPath, true);
		}

		// Remember the assets for next time build test
		BundleBuildState buildState = BundleManager.GetBuildStateOfBundle(bundle.name);
		if(succeed)
		{
			buildState.lastBuildDependencies = AssetDatabase.GetDependencies(assetPaths);
			FileInfo bundleFileInfo = new FileInfo(outputPath);	
			
			//Only has bundle real change will change version	
			if(buildState.crc != crc || buildState.size!= bundleFileInfo.Length)
			{
				buildState.version++;
				buildState.crc = crc;
				buildState.size = bundleFileInfo.Length;
			}

			if (buildState.version == int.MaxValue)
				buildState.version = 0;

			// refresh depends
			//BundleManager.RefreshBundleDependencies(bundle);
			//BMDataAccessor.SaveBundleData();

			// fix build state
			if (buildState.changeTime == -1)
			{
				buildState.changeTime = bundleFileInfo.LastWriteTime.ToBinary();
			}
			if (string.IsNullOrEmpty(buildState.bundleName))
			{
				buildState.bundleName = bundle.name;
			}
			if (BMDataAccessor.BuildStates.Find(x => x.bundleName == bundle.name) == null)
			{
				BMDataAccessor.BuildStates.Add(buildState);
			}

			// generate bundle ship info
			if (BMDataAccessor.BundleShipInfos.Find(item => item.BundleName == bundle.name) == null)
			{
				GM.BundleInfo _tmp = new GM.BundleInfo();
				_tmp.BundleName = bundle.name;
				_tmp.Paths = new List<string>();
				_tmp.Includes = new List<string>();
				BMDataAccessor.BundleShipInfos.Add(_tmp);
			}
			GM.BundleInfo _shipinfo = BMDataAccessor.BundleShipInfos.Find(item => item.BundleName == bundle.name);
			_shipinfo.Paths.Clear();
			_shipinfo.Includes.Clear();
			foreach (string _i in bundle.includs.ToArray().Concat(bundle.exIncludes.ToArray()))
			{
                if (string.IsNullOrEmpty(_i) || string.IsNullOrEmpty(Path.GetExtension(_i)))
                {
                    _shipinfo.Paths.Add(_i);
                }
                else
                {
                    _shipinfo.Paths.Add(_i.Replace(Path.GetExtension(_i), string.Empty));
                }
				_shipinfo.Includes.Add(Path.GetFileNameWithoutExtension(_i));
			}
			_shipinfo.Parent = bundle.parent.StartsWith("+") ? string.Empty : bundle.parent;
			_shipinfo.Version = buildState.version;
			_shipinfo.MD5 = S3Utils.CalculateMD5(System.Text.Encoding.Default.GetBytes(buildState.size.ToString() + buildState.crc.ToString()));
			_shipinfo.Size = buildState.size;

			BMDataAccessor.SaveBundleShipInfoFile();
		}
		else
		{
			buildState.lastBuildDependencies = null;
		}
		
		BMDataAccessor.SaveBundleBuildeStates();
		return succeed;
	}
	
	private static bool EqualStrArray(string[] strList1, string[] strList2)
	{
		if(strList1 == null || strList2 == null)
			return false;
		
		if(strList1.Length != strList2.Length)
			return false;
		
		for(int i = 0; i < strList1.Length; ++i)
		{
			if(strList1[i] != strList2[i])
				return false;
		}
		
		return true;
	}
	
	private static string GenerateOutputPathForBundle(string bundleName)
	{
		return Path.Combine(BuildConfiger.InterpretedOutputPath, bundleName.ToLower() + "." + BuildConfiger.BundleSuffix);
	}

	public static void ProcessBuildState(bool save)
	{
		string platform = "";
#if UNITY_IPHONE
		platform= "ios";
#else
		platform= "android";
#endif
		string buildState_LastVersion = string.Format("Assets/_ThirdParty/BundleManager/LastVersion/{0}/BuildStates.txt", platform);
		string buildstate = "Assets/_ThirdParty/BundleManager/BuildStates.txt";
		string bundleShipInfo_LastVersion = string.Format("Assets/_ThirdParty/BundleManager/LastVersion/{0}/BundleShipInfo.json", platform);
		string bundleShipInfo = "Assets/_ThirdParty/BundleManager/BundleShipInfo.json";

		if (!save)
		{
			System.IO.File.Delete(buildstate);
			System.IO.File.Delete(bundleShipInfo);
			System.IO.File.Copy(buildState_LastVersion, buildstate);
			System.IO.File.Copy(bundleShipInfo_LastVersion, bundleShipInfo);
			BundleManager.RefreshAll();
		}
		else
		{
			System.IO.File.Delete(buildState_LastVersion);
			System.IO.File.Delete(bundleShipInfo_LastVersion);
			System.IO.File.Copy(buildstate, buildState_LastVersion);
			System.IO.File.Copy(bundleShipInfo, bundleShipInfo_LastVersion);
		}
		AssetDatabase.Refresh();
	}
}
