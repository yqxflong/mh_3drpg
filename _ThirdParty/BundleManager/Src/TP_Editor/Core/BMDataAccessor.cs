using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

public class BMDataAccessor
{
	static public List<BundleData> Bundles
	{
		get
		{
			if(m_Bundles == null)
				m_Bundles = loadObjectFromJsonFile< List<BundleData> >(BundleDataPath);
			
			if(m_Bundles == null)
				m_Bundles = new List<BundleData>();
			
			return m_Bundles;
		}
	}
	
	static public List<BundleBuildState> BuildStates
	{
		get
		{
			if(m_BuildStates == null)
				m_BuildStates = loadObjectFromJsonFile< List<BundleBuildState> >(BundleBuildStatePath);
			
			if(m_BuildStates == null)
				m_BuildStates = new List<BundleBuildState>();
			
			return m_BuildStates;
		}
	}	
	
	static public BMConfiger BMConfiger
	{
		get
		{
			if(m_BMConfier == null)
				m_BMConfier = loadObjectFromJsonFile<BMConfiger>(BMConfigerPath);
			
			if(m_BMConfier == null)
				m_BMConfier = new BMConfiger();
			
			return m_BMConfier;
		}
	}
	
	static public BMUrls Urls
	{
		get
		{
			if(m_Urls == null)
				m_Urls = loadObjectFromJsonFile<BMUrls>(UrlDataPath);
			
			if(m_Urls == null)
				m_Urls = new BMUrls();
			
			return m_Urls;
		}
	}

	static public void Refresh()
	{
		m_Bundles = null;
		m_BuildStates = null;
		m_BMConfier = null;
		m_Urls = null;
	}
	
	static public void SaveBMConfiger()
	{
		saveObjectToJsonFile(BMConfiger, BMConfigerPath);
	}
	
	static public void SaveBundleData()
	{
		foreach(BundleData bundle in Bundles)
		{
			bundle.includeGUIDs.Sort(guidComp);
			bundle.includs = BundleManager.GUIDsToPaths(bundle.includeGUIDs);
			
			bundle.exIncludeGUIDs.Sort(guidComp);
			bundle.exIncludes = BundleManager.GUIDsToPaths(bundle.exIncludeGUIDs);

			bundle.dependGUIDs.Sort(guidComp);
			bundle.dependAssets = BundleManager.GUIDsToPaths(bundle.dependGUIDs);
		}
		saveObjectToJsonFile(Bundles, BundleDataPath);
	}
	
	static public void SaveBundleBuildeStates()
	{
		saveObjectToJsonFile(BuildStates, BundleBuildStatePath);
	}
	
	static public void SaveUrls()
	{
		saveObjectToJsonFile(Urls, UrlDataPath);
	}
	
		
	static private T loadObjectFromJsonFile<T>(string path)
	{
		TextReader reader = new StreamReader(path);
		if(reader == null)
		{
			EB.Debug.LogError("Cannot find {0}", path);
			reader.Close();
			return default(T);
		}
		
		T data = JsonMapper.ToObject<T>(reader.ReadToEnd());
		if(data == null)
		{
			EB.Debug.LogError("Cannot read data from {0}", path);
		}
		
		reader.Close();
		return data;
	}
	
	static private void saveObjectToJsonFile<T>(T data, string path)
	{
		TextWriter tw = new StreamWriter(path);
		if(tw == null)
		{
			Debug.LogError("Cannot write to " + path);
			return;
		}
		
		string jsonStr = JsonFormatter.PrettyPrint( JsonMapper.ToJson(data) );
		
		tw.Write(jsonStr);
		tw.Flush();
		tw.Close();

		BMDataWatcher.MarkChangeDate(path);
	}

	static private int guidComp(string guid1, string guid2)
	{
		string fileName1 = Path.GetFileNameWithoutExtension( AssetDatabase.GUIDToAssetPath(guid1) );
		string fileName2 = Path.GetFileNameWithoutExtension( AssetDatabase.GUIDToAssetPath(guid2) );
		int ret = fileName1.CompareTo(fileName2);

		if(ret == 0)
			return guid1.CompareTo(guid2);
		else
			return ret;
	}
	
	static private List<BundleData> m_Bundles = null;
	static private List<BundleBuildState> m_BuildStates = null;
	static private BMConfiger m_BMConfier = null;
	static private BMUrls m_Urls = null;
		
	public const string BundleDataPath = "Assets/_ThirdParty/BundleManager/BundleData.txt";
	public const string BMConfigerPath = "Assets/_ThirdParty/BundleManager/BMConfiger.txt";
	public const string BundleBuildStatePath = "Assets/_ThirdParty/BundleManager/BuildStates.txt";
	public const string UrlDataPath = "Assets/_ThirdParty/BundleManager/Resources/Urls.txt";

	static public List<GM.BundleInfo> BundleShipInfos
	{
		get
		{
			if (m_BundleShipInfos == null)
			{
				m_BundleShipInfos = loadObjectFromJsonFile<List<GM.BundleInfo>>(BundleShipInfoPath);
			}

			if (m_BundleShipInfos == null)
			{
				m_BundleShipInfos = new List<GM.BundleInfo>();
			}

			return m_BundleShipInfos;
		}
	}

	static public void ReloadBundleShipInfos()
	{
		if (m_BundleShipInfos != null)
		{
			m_BundleShipInfos.Clear();
		}
		m_BundleShipInfos = null;
		m_BundleShipInfos = loadObjectFromJsonFile<List<GM.BundleInfo>>(BundleShipInfoPath);
		if (m_BundleShipInfos == null)
		{
			m_BundleShipInfos = new List<GM.BundleInfo>();
		}
	}
	
	static public void SaveBundleShipInfoFile()
	{
		saveObjectToJsonFile(BundleShipInfos, BundleShipInfoPath);
	}
	
	static private List<GM.BundleInfo> m_BundleShipInfos = null;
	public const string BundleShipInfoPath = "Assets/_ThirdParty/Bundlemanager/BundleShipInfo.json";
	
	public static string[] HudConfigPath =
	{
        "Assets/_ThirdParty/BundleManager/HudLoad_High.json",
        "Assets/_ThirdParty/BundleManager/HudLoad_Medium.json",
        "Assets/_ThirdParty/BundleManager/HudLoad_Low.json",
	};

	public static string[] SceneLoadConfigPath =
	{
        "Assets/_ThirdParty/BundleManager/SceneLoad_High.json",
        "Assets/_ThirdParty/BundleManager/SceneLoad_Medium.json",
        "Assets/_ThirdParty/BundleManager/SceneLoad_Low.json",
	};
}
