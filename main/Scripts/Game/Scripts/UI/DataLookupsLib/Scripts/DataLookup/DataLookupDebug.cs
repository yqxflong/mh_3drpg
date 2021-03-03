using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;


public class DataLookupDebug : MonoBehaviour 
#if DEBUG
	, IDebuggable
#endif
{
	public static DataLookupDebug Create() {
		return new GameObject("DataLookupDebug").AddComponent<DataLookupDebug>();
	}

	public static void Dispose()
	{
		GameObject go = GameObject.Find("DataLookupDebug");
		if (go != null) GameObject.Destroy(go);
	}

#if DEBUG
	private void Awake()
	{
		DebugSystem.RegisterSystem("View Cache Data", this);
		gameObject.AddComponent<DebugHandler>();
		DontDestroyOnLoad(gameObject);
	}

	void OnDestroy()
	{
		DebugSystem.UnregisterSystem(this);
	}

	public void OnDrawDebug()
	{
	}
	
	public void OnDebugGUI()
	{
	}

	private Dictionary<string,bool> cacheDataToggleDic = new Dictionary<string,bool>();
	private bool isShowAll = false;
	private string cmdStr = string.Empty;
	private string resultStr = "Search Result";

	public object GetDataCache(Hashtable hash, bool outputToJSON = false)
	{
		return !outputToJSON ? (object)hash : (object)JsonMapper.ToJson(hash);
	}

	public void OnDebugPanelGUI()
	{
		GUIStyle btnStyle = new GUIStyle(GUI.skin.button);

		GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
		boxStyle.alignment = TextAnchor.UpperLeft;
		boxStyle.wordWrap = true;

		GUILayout.BeginVertical();

		// add cmd search cache data
		GUILayout.BeginHorizontal();
		
		cmdStr = GUILayout.TextField(cmdStr,100, GUILayout.Width(200));

		object result;

		if(GUILayout.Button("Search"))
		{
			if (DataLookupsCache.Instance.SearchDataByID(cmdStr, out result))
			{
				if(result is IDictionary)
				{
					resultStr = JsonFormatter.PrettyPrint(JsonMapper.ToJson(result).ToString());
				}
				else
				{
					resultStr = result.ToString();
				}
			}
			else
			{
				resultStr = "Invalid Path";
			}
		}

		GUILayout.EndHorizontal();

		//if(resultStr is IDictionary)
		//{
		//	resultStr = JsonFormatter.PrettyPrint(JsonMapper.ToJson(resultStr).ToString());
		//}
		GUILayout.Box(resultStr, boxStyle);
		
		IDictionary cacheDataHash = GetDataCache(DataLookupsCache.Instance.GetDataCache(), false) as IDictionary;

		List<string> nodeList = new List<string>();

		foreach(DictionaryEntry node in cacheDataHash)
		{
			if(node.Value is IDictionary || node.Value is ArrayList)
			{
				nodeList.Add(node.Key.ToString());
			}
		}

		nodeList.Sort();

		foreach(string keyString in nodeList)
		{
			if(!cacheDataToggleDic.ContainsKey(keyString))
			{
				cacheDataToggleDic.Add(keyString, false);
			}
			
			cacheDataToggleDic[keyString] = GUILayout.Toggle(cacheDataToggleDic[keyString], keyString, btnStyle);
			
			if(cacheDataToggleDic[keyString])
			{
				string jsonData = JsonMapper.ToJson(cacheDataHash[keyString]).ToString();

				if(keyString == "gems")
				{
					GUILayout.TextArea(jsonData, boxStyle);
				}
				else
				{
					GUILayout.TextArea(JsonFormatter.PrettyPrint(jsonData), boxStyle);
				}
			}
		}

		isShowAll = GUILayout.Toggle(isShowAll, "Show The Whole Cache Data", btnStyle);

		if(isShowAll)
		{
			GUILayout.TextArea(GetDataCache(DataLookupsCache.Instance.GetDataCache(), true).ToString(), boxStyle);
		}
		
		GUILayout.EndVertical();
	}
	
#endif
}

