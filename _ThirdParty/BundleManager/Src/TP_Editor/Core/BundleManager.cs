using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 资源包管理器
/// </summary>
public class BundleManager
{
    /// <summary>
    /// Get the BundleData by bundle's name. Method will return null  if there's no such bundle.
    /// 获取指定资源包数据信息
    /// </summary>
    /// <param name="name">资源包名称</param>
    /// <returns></returns>
    static public BundleData GetBundleData(string name)
	{
		if(getInstance().bundleDict.ContainsKey(name))
			return getInstance().bundleDict[name];
		else
			return null;
	}
	
	/**
	 * Get the build state by bundle's name. Method will return null if there's no such bundle.
	 */
	static public BundleBuildState GetBuildStateOfBundle(string name)
	{
		if(getInstance().statesDict.ContainsKey(name))
			return getInstance().statesDict[name];
		else
			return null;
	}
	
	/**
	 * Return array of all bundles.
	 */
	static public BundleData[] bundles
	{
		get{return BMDataAccessor.Bundles.ToArray();}
	}	
	
	/**
	 * Return array of all bundle build states.
	 */
	static public BundleBuildState[] buildStates
	{
		get{return BMDataAccessor.BuildStates.ToArray();}
	}
	
	/**
	 * Return the list of all root bundles.
	 */
	static public List<BundleData> Roots
	{
		get
		{
			var bundleList = BMDataAccessor.Bundles;
			int childBundleStartIndex = bundleList.FindIndex(x => x.parent != "");
			List<BundleData> parents = null;
			if(childBundleStartIndex != -1)
				parents = bundleList.GetRange(0, childBundleStartIndex);
			else
				parents = bundleList.GetRange(0, bundleList.Count);
			
			parents.Sort((x,y)=>x.name.CompareTo(y.name));
			return parents;
		}
	}
	
	/**
	 * Detect if the the set parent operation is valid.
	 */
	static public bool CanBundleParentTo(string child, string newParent)
	{
		if(child == newParent)
			return false;

		if(child == null || child.StartsWith("+"))
		{
			return false;
		}
		
		if(newParent == "")
			return true;
		
		var bundleDict = getInstance().bundleDict;
		if( !bundleDict.ContainsKey(child) )
			return false;
		
		if( newParent != "" && !bundleDict.ContainsKey(newParent))
			return false;
		
		string tarParent = bundleDict[newParent].parent;
		while(bundleDict.ContainsKey(tarParent))
		{
			if(tarParent == child)
				// Parent's tree cannot contains this child
				return false;
			
			tarParent = bundleDict[tarParent].parent;
		}
		
		return true;
	}
	
	/**
	 * Set the parent of the bundle.
	 * @param parent New parent bundle's name. Set the parent to empty if you want the childe bundle become a root bundle.
	 */
	static public void SetParent(string childe, string parent)
	{
		if(!CanBundleParentTo(childe, parent))
			return;

		var bundleDict = getInstance().bundleDict;
		if(!bundleDict.ContainsKey(childe) || (parent != "" && !bundleDict.ContainsKey(parent)))
			return;
		
		BundleData childeBundle = bundleDict[childe];
		
		if(bundleDict.ContainsKey(childeBundle.parent))
			bundleDict[childeBundle.parent].children.Remove(childe);
				
		string origParent = childeBundle.parent;
		childeBundle.parent = parent;
		
		if(parent != "")
		{
			BundleData newParentBundle = bundleDict[parent];
			newParentBundle.children.Add(childe);
			newParentBundle.children.Sort();
		}
		
		if(parent == "" || origParent == "")
		{
			BMDataAccessor.Bundles.Remove(childeBundle);
			InsertBundleToBundleList(childeBundle);
		}
		
		UpdateBundleChangeTime(childeBundle.name);
		BMDataAccessor.SaveBundleData();
		
		GM.BundleInfo bundleInfo = BMDataAccessor.BundleShipInfos.Find(item => item.BundleName == childe);
		if (bundleInfo != null)
		{
			bundleInfo.Parent = parent.StartsWith("+") ? "" : parent;
		}
		BMDataAccessor.SaveBundleShipInfoFile();
    }

    /// <summary>
    /// Create a new bundle.
    /// 创建一个新的资源包
    /// </summary>
    /// <param name="name">资源包名称_Name of the bundle name.</param>
    /// <param name="parent">资源包的父级资源包_New parent's name. Set the parent to empty string if you want create a new root bundle.</param>
    /// <param name="sceneBundle">是否为场景资源包_Is the bundle a scene bundle? </param>
    /// <returns></returns>
    static public bool CreateNewBundle(string name, string parent, bool sceneBundle)
	{
		var bundleDict = getInstance().bundleDict;

		if(bundleDict.ContainsKey(name))
			return false;
		
		BundleData newBundle = new BundleData();
		newBundle.name = name;
		newBundle.sceneBundle = sceneBundle;
		
		if(parent != "")
		{
			if(!bundleDict.ContainsKey(parent))
				return false;
			else
				bundleDict[parent].children.Add(name);
			
			newBundle.parent = parent;
		}
	
		bundleDict.Add(name, newBundle);
		InsertBundleToBundleList(newBundle);
		
		BundleBuildState newBuildState = new BundleBuildState();
		newBuildState.bundleName = name;
		getInstance().statesDict.Add(name, newBuildState);
		BMDataAccessor.BuildStates.Add(newBuildState);
		
		UpdateBundleChangeTime(newBundle.name);
		
		BMDataAccessor.SaveBundleData();
		BMDataAccessor.SaveBundleBuildeStates();
		
		if (!name.StartsWith("+"))
		{
			GM.BundleInfo newBundleInfo = new GM.BundleInfo();
			newBundleInfo.BundleName = name;
			newBundleInfo.Parent = parent.StartsWith("+") ? "" : parent;
			newBundleInfo.Paths = new List<string>();
			newBundleInfo.Includes = new List<string>();
			newBundleInfo.Version = -1;
			BMDataAccessor.BundleShipInfos.Add(newBundleInfo);
			
			BMDataAccessor.SaveBundleShipInfoFile();
		}		
		
		return true;
	}
	
	/**
	 * Remove the bundle by the given name.
	 * @Return Return false if no such bundle.
	 */
	static public bool RemoveBundle(string name)
	{
		var bundleDict = getInstance().bundleDict;
		var bundlelist = BMDataAccessor.Bundles;
		var dependRefDict = getInstance().dependRefDict;
		var includeRefDict = getInstance().includeRefDict;
		
		if(!bundleDict.ContainsKey(name))
			return false;
		
		BundleData bundle = bundleDict[name];
		bundlelist.Remove(bundle);
		bundleDict.Remove(name);
		
		var buildStatesDict = getInstance().statesDict;
		BMDataAccessor.BuildStates.Remove(buildStatesDict[name]);
		buildStatesDict.Remove(name);
		
		// Remove parent ref
		if(bundle.parent != "" && bundleDict.ContainsKey(bundle.parent))
		{
			bundleDict[bundle.parent].children.Remove(name);
		}
		
		// Remove include ref
		foreach(string guid in bundle.includeGUIDs)
		{
			if(includeRefDict.ContainsKey(guid))
				includeRefDict[guid].Remove(bundle);
		}
		
		// Remove depend asssets ref
		foreach(string guid in bundle.dependGUIDs)
		{
			dependRefDict[guid].Remove(bundle);
		}
		
		// Delete children recursively
		foreach(string childName in bundle.children)
		{
			RemoveBundle(childName);
		}
		
		BMDataAccessor.SaveBundleData();
		BMDataAccessor.SaveBundleBuildeStates();

		GM.BundleInfo bundleInfo = BMDataAccessor.BundleShipInfos.Find(item => item.BundleName == name);
		if (bundleInfo != null)
		{
			BMDataAccessor.BundleShipInfos.Remove(bundleInfo);
			BMDataAccessor.SaveBundleShipInfoFile();
		}		
		
		return true;
	}
	
	/**
	 * Get the root of the give bundle.
	 */
	public static string GetRootOf(string bundleName)
	{
		BundleData root = GetBundleData(bundleName);
		while(root.parent != "" && !root.parent.StartsWith("+"))
		{
			root = GetBundleData(root.parent);
			if(root == null)
			{
				EB.Debug.LogError("Cannnot find root of [{0}]. Sth wrong with the bundle config data.", bundleName);
				return "";
			}
		}
		
		return root.name;
	}
	
	/**
	 * Rename the bundle.
	 * @Return Return false if there's no such bundle, or the new name is used.
	 */
	static public bool RenameBundle(string origName, string newName)
	{
		if (origName.StartsWith("+") && !newName.StartsWith("+"))
		{
			UnityEditor.EditorUtility.DisplayDialog("Message", "You can not rename a folder node to a normal bundle node", "OK");
			return false;
		}
		else if (!origName.StartsWith("+") && newName.StartsWith("+"))
		{
			UnityEditor.EditorUtility.DisplayDialog("Message", "You can not rename a normal bundle node to a folder node", "OK");
			return false;
		}

		if(newName == "" || origName == newName || getInstance().bundleDict.ContainsKey(newName) || !getInstance().bundleDict.ContainsKey(origName))
			return false;
		
		BundleData bundle = getInstance().bundleDict[origName];
		bundle.name = newName;
		
		Dictionary<string, BundleData> bundleDict = getInstance().bundleDict;
		bundleDict.Remove(origName);
		bundleDict.Add(newName, bundle);
		
		if(bundle.parent != "")
		{
			BundleData parentBundle = bundleDict[bundle.parent];
			parentBundle.children.Remove(origName);
			parentBundle.children.Add(newName);
		}
		
		foreach(string childName in bundle.children)
			getInstance().bundleDict[childName].parent = newName;
		
		var buildStatesDic = getInstance().statesDict;
		BundleBuildState buildState = buildStatesDic[origName];
		buildState.bundleName = newName;
		buildStatesDic.Remove(origName);
		buildStatesDic.Add(newName, buildState);
		
		BMDataAccessor.SaveBundleData();
		BMDataAccessor.SaveBundleBuildeStates();
		
		List<GM.BundleInfo> childBundleInfo = BMDataAccessor.BundleShipInfos.FindAll(item => item.Parent == origName);
		GM.BundleInfo bundleInfo = BMDataAccessor.BundleShipInfos.Find(item => item.BundleName == origName);
		if (bundleInfo != null)
		{
			bundleInfo.BundleName = newName;

			foreach (GM.BundleInfo _child in childBundleInfo)
			{
				_child.Parent = newName;
			}
			
			BMDataAccessor.SaveBundleShipInfoFile();
		}		
		
		return true;
	}
	
	/**
	 * Test if path can be added to bundle.
	 * @param path The path must be a path under Asset. Can be path of diretory or file.
	 * @param bundleName The bundle's name.
	 */
	public static bool CanAddPathToBundle(string path, string bundleName)
	{
		if (bundleName.StartsWith("+"))
		{
			return false;
		}

		BundleData bundle = GetBundleData(bundleName);
		if(bundle == null || Path.IsPathRooted(path) || (!File.Exists(path) && !Directory.Exists(path)))
		{
			return false;
		}

		string guid = AssetDatabase.AssetPathToGUID(path);
		if(bundle.includeGUIDs.Contains(guid))
			return false;
		
		if(ContainsFileInPath(path, sceneDetector) && !bundle.sceneBundle)
			return false;
		else if(ContainsFileInPath(path, assetDetector) && bundle.sceneBundle)
			return false;
		else
			return true;
	}
	
	/**
	 * Add a path to bundle's include list.
	 * @param path The path must be a path under Asset. Can be path of diretory or file.
	 * @param bundleName The bundle's name.
	 */
	public static void AddPathToBundle(string path, string bundleName)
	{
		BundleData bundle = GetBundleData(bundleName);

		if(IsNameDuplicatedAsset(bundle, path))
			EB.Debug.LogWarning("Name of new add asset will be duplicate with asset in bundle {0}. This may cause problem when you trying to load them.", bundleName);

		string guid = AssetDatabase.AssetPathToGUID(path);

		bundle.includeGUIDs.Add(guid);

		AddIncludeRef(guid, bundle);
		RefreshBundleDependencies(bundle);
		UpdateBundleChangeTime(bundle.name);
		
		BMDataAccessor.SaveBundleData();
	}
	
	/**
	 * Remove asset from bundle's include list by path.
	 */
	public static void RemovePathFromBundle(string path, string bundleName)
	{
		string guid = AssetDatabase.AssetPathToGUID(path);
		RemoveAssetFromBundle(guid, bundleName);
	}

	/**
	 * Remove asset from bundle's include list by guid.
	 */
	public static void RemoveAssetFromBundle(string guid, string bundleName)
	{
		BundleData bundle = GetBundleData(bundleName);
		bundle.includeGUIDs.Remove(guid);
		
		RemoveIncludeRef(guid, bundle);
		RefreshBundleDependencies(bundle);
		UpdateBundleChangeTime(bundle.name);
		
		BMDataAccessor.SaveBundleData();
	}
	
	/**
	 * Test if the bundle is depend on another.
	 */
	public static bool IsBundleDependOn(string bundleName, string dependence)
	{
		BundleData bundle = GetBundleData(bundleName);
		if(bundle != null && bundleName == dependence)
			return true;
		
		while(bundle != null && bundle.parent != "")
		{
			if(bundle.parent == dependence)
				return true;
			
			bundle = GetBundleData(bundle.parent);
		}
		
		return false;
	}

	/**
	 * Force refresh references of all bundles
	 */
	public static void RefreshAll()
	{
		getInstance().Init();
	}
	
	/**
	 * Refresh bundle dependencies
	 */
	public static void RefreshBundleDependencies(BundleData bundle)
	{
		// Remove the old refs
		foreach(string guid in bundle.dependGUIDs)
		{
			if(getInstance().dependRefDict.ContainsKey(guid))
				getInstance().dependRefDict[guid].Remove(bundle);
		}
		
		// Get all the includes files path
		string[] files = BuildHelper.GetAssetsFromPaths( GUIDsToPaths(bundle.includeGUIDs).ToArray(), bundle.sceneBundle );
		string[] dependGUIDs = PathsToGUIDs( AssetDatabase.GetDependencies(files) );
		
		// New refs
		bundle.dependGUIDs = new List<string>(dependGUIDs);
		bundle.dependGUIDs.RemoveAll(x=>bundle.includeGUIDs.Contains(x));

		AddDependRefs(bundle);
	}
	
	/////////////////////////////////////////////////////////////////////////
	/// ExInclude
	public static void RefreshExInclude()
	{
		for (int i = 0; i < bundles.Length; i++)
		{
			bundles[i].exIncludes.Clear();
			bundles[i].exIncludeGUIDs.Clear();
		}

		if (!BMDataAccessor.BMConfiger.optimizeTree)
		{
			return;
		}
		foreach (BundleData rootBundle in Roots)
		{ 
			Dictionary<string, List<BundleData>> treeDependRefDict = new Dictionary<string, List<BundleData>>();
			HandleExInclude(rootBundle, treeDependRefDict);
//			bundle.dependGUIDs.RemoveAll(x=>bundle.exIncludeGUIDs.Contains(x));
		}
		
		for (int i = 0; i < bundles.Length; i++)
		{
			bundles[i].dependGUIDs
				.RemoveAll(dependGUID => bundles[i].exIncludeGUIDs.Contains(dependGUID));
		}
	}

	internal static BundleData GetParentNode(BundleData bundle)
	{
		if (bundle.parent != "" && !bundle.parent.StartsWith("+"))
		{
			return GetBundleData(bundle.parent);
		}

		return null;
	}

	internal static bool IsIncludeOrExIncludeInParent(string guid, BundleData parent)
	{
		if (parent.includeGUIDs.Contains(guid))
		{
			return true;
		}
		if (parent.exIncludeGUIDs.Contains(guid))
		{
			return true;
		}

		if (GetParentNode(parent) != null)
		{
			return IsIncludeOrExIncludeInParent(guid, GetBundleData(parent.parent));
		}
		else
		{
			return false;
		}
	}

	internal static void AddTreeDependRefs(string guid, BundleData bundle, Dictionary<string, List<BundleData>> treeDependRefDict)
	{
		if (GetParentNode(bundle) != null)
		{
			if (IsIncludeOrExIncludeInParent(guid, GetBundleData(bundle.parent)))
			{
				return;
			}
		}

		if(!treeDependRefDict.ContainsKey(guid))
		{
			List<BundleData> sharedBundleList = new List<BundleData>();
			sharedBundleList.Add(bundle);
			treeDependRefDict.Add(guid, sharedBundleList);
		}
		else
		{
			if (!treeDependRefDict[guid].Contains(bundle))
			{
				treeDependRefDict[guid].Add(bundle);
			}
		}

		AdjustExIncludeGUIDs(guid, treeDependRefDict);
	}

	internal static void AdjustExIncludeGUIDs(string guid, Dictionary<string, List<BundleData>> treeDependRefDict)
	{
		var hasParentNodes = treeDependRefDict[guid].FindAll(bundle => GetParentNode(bundle) != null);
		if (hasParentNodes.Count > 1)
		{
			foreach (BundleData bundleData in hasParentNodes.ToArray())
			{
				if (!IsIncludeOrExIncludeInParent(guid, GetBundleData(bundleData.parent)))
				{
					GetBundleData(bundleData.parent).exIncludeGUIDs.Add(guid);
					bundleData.exIncludeGUIDs.Remove(guid);
					treeDependRefDict[guid].Remove(bundleData);
					if (!treeDependRefDict[guid].Contains(GetBundleData(bundleData.parent)))
					{
						treeDependRefDict[guid].Add(GetBundleData(bundleData.parent));
					}
				}
			}

			AdjustExIncludeGUIDs(guid, treeDependRefDict);
		}
	}
	
	private static void HandleExInclude(BundleData bundle, Dictionary<string, List<BundleData>> treeDependRefDict)
	{
		foreach(string guid in bundle.dependGUIDs)
		{
			AddTreeDependRefs(guid, bundle, treeDependRefDict);
//			if (treeDependRefDict[guid].Count > 1)
//			{
//				if (bundle.parent != "")
//				{
//					
//				}
//			}
//			if(getInstance().dependRefDict.ContainsKey(guid))
//				getInstance().dependRefDict[guid].Remove(bundle);
		}

		foreach (var child in bundle.children)
		{
			HandleExInclude(GetBundleData(child), treeDependRefDict);
		}
	}
	

	/////////////////////////////////////////////////////////////////////////

	internal static void UpdateAllBundleChangeTime()
	{
		foreach(BundleData bundle in bundles)
			UpdateBundleChangeTime(bundle.name);
		
		BMDataAccessor.SaveBundleBuildeStates();
	}
	
	public static List<BundleData> GetIncludeBundles(string guid)
	{
		var assetDict = getInstance().includeRefDict;
		if(!assetDict.ContainsKey(guid))
			return null;
		else
			return assetDict[guid];
	}
	
	internal static List<BundleData> GetExIncludeBundles(string guid)
	{
		var assetDict = getInstance().exIncludeRefDict;
		if(!assetDict.ContainsKey(guid))
			return null;
		else
			return assetDict[guid];
	}
	
	internal static List<BundleData> GetRelatedBundles(string guid)
	{
		string path = AssetDatabase.GUIDToAssetPath(guid);
		if(!File.Exists(path))
			return null;

		var includeDict = getInstance().includeRefDict;
		var exIncludeDict = getInstance().exIncludeRefDict;
		var dependDict = getInstance().dependRefDict;

		List<BundleData> result = new List<BundleData>();

		bool isDirectory = (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
		if(isDirectory)
		{
			// Add every asset under this folder into list
			foreach(var pair in includeDict)
			{
				string curPath = AssetDatabase.GUIDToAssetPath(pair.Key);
				if( curPath.Contains(path) )
					result.AddRange(pair.Value);
			}
			
			foreach(var pair in exIncludeDict)
			{
				string curPath = AssetDatabase.GUIDToAssetPath(pair.Key);
				if( curPath.Contains(path) )
					result.AddRange(pair.Value);
			}

			foreach(var pair in dependDict)
			{
				string curPath = AssetDatabase.GUIDToAssetPath(pair.Key);
				if( curPath.Contains(path) )
					result.AddRange(pair.Value);
			}
		}
		else
		{
			if(includeDict.ContainsKey(guid))
				result.AddRange(includeDict[guid]);
			
			if(exIncludeDict.ContainsKey(guid))
				result.AddRange(exIncludeDict[guid]);

			if(dependDict.ContainsKey(guid))
				result.AddRange(dependDict[guid]);
		}

		return result;
	}

	internal static void AddDependRefs(BundleData bundle)
	{
		foreach(string guid in bundle.dependGUIDs)
		{
			if(!getInstance().dependRefDict.ContainsKey(guid))
			{
				List<BundleData> sharedBundleList = new List<BundleData>();
				sharedBundleList.Add(bundle);
				getInstance().dependRefDict.Add(guid, sharedBundleList);
			}
			else
			{
				getInstance().dependRefDict[guid].Add(bundle);
			}
		}
	}
	
	internal static void AddIncludeRef(string guid, BundleData bundle)
	{
		string path = AssetDatabase.GUIDToAssetPath(guid);
		foreach(string subPath in BuildHelper.GetAssetsFromPath(path, bundle.sceneBundle))
		{
			string subGuid = AssetDatabase.AssetPathToGUID(subPath);

			if(!getInstance().includeRefDict.ContainsKey(subGuid))
			{
				List<BundleData> sharedBundleList = new List<BundleData>(){bundle};
				getInstance().includeRefDict.Add(subGuid, sharedBundleList);
			}
			else if(!getInstance().includeRefDict[subGuid].Contains(bundle))
			{
				getInstance().includeRefDict[subGuid].Add(bundle);
			}
		}
	}
	
	internal static void AddExIncludeRef(string guid, BundleData bundle)
	{
		string path = AssetDatabase.GUIDToAssetPath(guid);
		foreach(string subPath in BuildHelper.GetAssetsFromPath(path, bundle.sceneBundle))
		{
			string subGuid = AssetDatabase.AssetPathToGUID(subPath);

			if(!getInstance().exIncludeRefDict.ContainsKey(subGuid))
			{
				List<BundleData> sharedBundleList = new List<BundleData>(){bundle};
				getInstance().exIncludeRefDict.Add(subGuid, sharedBundleList);
			}
			else if(!getInstance().exIncludeRefDict[subGuid].Contains(bundle))
			{
				getInstance().exIncludeRefDict[subGuid].Add(bundle);
			}
		}
	}

	internal static void RemoveIncludeRef(string guid, BundleData bundle)
	{
		string path = AssetDatabase.GUIDToAssetPath(guid);
		foreach(string subPath in BuildHelper.GetAssetsFromPath(path, bundle.sceneBundle))
		{
			string subGuid = AssetDatabase.AssetPathToGUID(subPath);
			getInstance().includeRefDict[subGuid].Remove(bundle);
		}
	}
	
	internal static void RemoveExIncludeRef(string guid, BundleData bundle)
	{
		string path = AssetDatabase.GUIDToAssetPath(guid);
		foreach(string subPath in BuildHelper.GetAssetsFromPath(path, bundle.sceneBundle))
		{
			string subGuid = AssetDatabase.AssetPathToGUID(subPath);
			getInstance().exIncludeRefDict[subGuid].Remove(bundle);
		}
	}

	internal static List<string> GUIDsToPaths(List<string> guids)
	{
		if(guids == null)
			return null;
		
		List<string> ret = new List<string>(guids);
		for(int i = 0; i < ret.Count; ++i)
			ret[i] = AssetDatabase.GUIDToAssetPath(ret[i]);
		
		return ret;
	}
	
	internal static string[] GUIDsToPaths(string[] guids)
	{
		if(guids == null)
			return null;
		
		string[] ret = new string[guids.Length];
		for(int i = 0; i < ret.Length; ++i)
			ret[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
		
		return ret;
	}
	
	internal static List<string> PathsToGUIDs(List<string> paths)
	{
		if(paths == null)
			return null;
		
		List<string> ret = new List<string>(paths);
		for(int i = 0; i < ret.Count; ++i)
			ret[i] = AssetDatabase.AssetPathToGUID(ret[i]);
		
		return ret;
	}
	
	internal static string[] PathsToGUIDs(string[] paths)
	{
		if(paths == null)
			return null;
		
		string[] ret = new string[paths.Length];
		for(int i = 0; i < ret.Length; ++i)
			ret[i] = AssetDatabase.AssetPathToGUID(paths[i]);
		
		return ret;
	}

	private static bool assetDetector(string filePath)
	{
		return !filePath.EndsWith(".unity", System.StringComparison.OrdinalIgnoreCase) && 
			!filePath.EndsWith(".meta", System.StringComparison.OrdinalIgnoreCase);
	}
	
	private static bool sceneDetector(string filePath)
	{
		return filePath.EndsWith(".unity", System.StringComparison.OrdinalIgnoreCase);
	}
	
	private delegate bool FileTypeDetector(string filePath);
	private static bool ContainsFileInPath(string path, FileTypeDetector fileDetector)
	{
		bool isDir = (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
		if(!isDir)
		{
			return fileDetector(path);
		}
		else
		{
			foreach(string subPath in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
			{
				if(fileDetector(subPath))
					return true;
			}
			
			return false;
		}
	}
	
	private static void UpdateBundleChangeTime(string bundleName)
	{
		GetBuildStateOfBundle(bundleName).changeTime = DateTime.Now.ToBinary();
	}
	
	private static void InsertBundleToBundleList(BundleData bundle)
	{
		List<BundleData> bundleList = BMDataAccessor.Bundles;
		if(bundleList.Contains(bundle))
			return;
		
		if(bundle.parent == "")
		{
			int childBundleStartIndex = bundleList.FindIndex(x => x.parent != "");
			childBundleStartIndex = childBundleStartIndex == -1 ? bundleList.Count : childBundleStartIndex;
			bundleList.Insert(childBundleStartIndex, bundle);
		}
		else
			bundleList.Add(bundle);
	}

	private static bool IsNameDuplicatedAsset(BundleData bundle, string newAsset)
	{
		string newName = Path.GetFileNameWithoutExtension(newAsset);
		foreach(string guid in bundle.includeGUIDs)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			string file = Path.GetFileNameWithoutExtension(path);
			if(file == newName)
				return true;
		}

		return false;
	}
    /// <summary>
    /// 检查老的版本
    /// </summary>
	private static void checkOldVersion()
	{
		if(BMDataAccessor.BMConfiger.bmVersion == 0)
		{
			EditorUtility.DisplayDialog("Bundle Manager Upgrade", 
			                            "Welcome to new version. Bundle Manager will upgrade your Bundle Data files to new version.", 
			                            "OK");
			BMDataAccessor.BMConfiger.bmVersion = 1;
			foreach(BundleData bundle in BMDataAccessor.Bundles)
			{
				bundle.includeGUIDs = PathsToGUIDs(bundle.includs);
				bundle.dependGUIDs = PathsToGUIDs(bundle.dependAssets);
			}

			BMDataAccessor.SaveBundleData();
			BMDataAccessor.SaveBMConfiger();
		}
	}

	private void Init()
	{
		BMDataAccessor.Refresh();

		statesDict.Clear();
		includeRefDict.Clear();
		dependRefDict.Clear();

		foreach(BundleBuildState buildState in BMDataAccessor.BuildStates)
		{
			if(!statesDict.ContainsKey(buildState.bundleName))
				statesDict.Add(buildState.bundleName, buildState);
			else
				EB.Debug.LogError("Bundle manger -- Cannot have two build states with same name [{0}]",buildState.bundleName);
		}

		bundleDict.Clear();

		foreach(BundleData bundle in BMDataAccessor.Bundles)
		{
			if(!bundleDict.ContainsKey(bundle.name))
				bundleDict.Add(bundle.name, bundle);
			else
				EB.Debug.LogError("Bundle manger -- Cannot have two bundle with same name [{0}]", bundle.name);
			
			if(!statesDict.ContainsKey(bundle.name))
				statesDict.Add(bundle.name, new BundleBuildState()); // Don't have build state of the this bundle. Add a new one.
			
			foreach(string guid in bundle.includeGUIDs)
				AddIncludeRef(guid, bundle);
			
			foreach(string guid in bundle.exIncludeGUIDs)
				AddExIncludeRef(guid, bundle);

			AddDependRefs(bundle);
		}
	}

	static private BundleManager instance = null;
	static private BundleManager getInstance()
	{
		if(instance != null)
			return instance;

		checkOldVersion();

		instance = new BundleManager();
		
		instance.Init();
		return instance;
	}
	/// <summary>
    /// 资源包数据
    /// </summary>
	private Dictionary<string, BundleData> bundleDict = new Dictionary<string, BundleData>();
	private Dictionary<string, BundleBuildState> statesDict = new Dictionary<string, BundleBuildState>();
	private Dictionary<string, List<BundleData>> dependRefDict = new Dictionary<string, List<BundleData>>();// key: asset path, value: bundles depend this asset
	private Dictionary<string, List<BundleData>> includeRefDict = new Dictionary<string, List<BundleData>>();// key: asset path, value: bundles include this asset
	private Dictionary<string, List<BundleData>> exIncludeRefDict = new Dictionary<string, List<BundleData>>();
}
