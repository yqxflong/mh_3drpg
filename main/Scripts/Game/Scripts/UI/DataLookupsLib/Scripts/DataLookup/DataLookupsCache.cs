using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// 数据缓存管理器
/// </summary>
public class DataLookupsCache
{
	private static DataLookupsCache instance;
	public static DataLookupsCache Instance {
		get {
			if(instance == null) instance = new DataLookupsCache();
			return instance;
		}
	}
	/// <summary>
    /// 当前的数据缓存
    /// </summary>
	private Hashtable dataCache = Johny.HashtablePool.Claim();

    /// <summary>
    /// 清空缓存
    /// </summary>
	public void ClearCache() {
		dataCache.Clear();
		lookups.Clear();
		inlineLookups.Clear();
	}


	#region 添加数据监听
	private Dictionary<string, List<DataLookup>> lookups = new Dictionary<string, List<DataLookup>> ();
    /// <summary>
    /// 添加缓存组件
    /// </summary>
    /// <param name="dataID">缓存的唯一id</param>
    /// <param name="lookup">缓存组件</param>
	public void AddLookup(string dataID, DataLookup lookup)
	{
        //
        //EB.Debug.LogLookup("<<添加_缓存组件_AddLookup:" + lookup.name + "___" + ((lookup.GetComponentsInChildren<UnityEngine.Renderer>(true).Length > 0) ? "带有渲染器" : "无"));
        //
        if (dataID == null || IsLookupRegistered(lookup, dataID)) return;

		if (!lookups.ContainsKey (dataID))
			lookups.Add(dataID, new List<DataLookup>());

		lookups [dataID].Add (lookup);		
		
		foreach(string inlineDataID in GetDataIDInlineParts(dataID)) {
			AddLookup(inlineDataID, lookup);
			
			if(!inlineLookups.ContainsKey(inlineDataID))
				inlineLookups.Add(inlineDataID, new InlineLookupEntry() {Lookups = new List<DataLookup>()});
			
			inlineLookups[inlineDataID].Lookups.Add(lookup);
		}
	}

    /// <summary>
    /// 移除缓存组件
    /// </summary>
    /// <param name="dataID">移动的唯一id</param>
    /// <param name="lookup">缓存组件</param>
	public void RemoveLookup(string dataID, DataLookup lookup)
	{
        //EB.Debug.LogLookup(">>移除_缓存组件_RemoveLookup:" + lookup.name + "___" + ((lookup.GetComponentsInChildren<UnityEngine.Renderer>(true).Length > 0) ? "带有渲染器" : "无"));

        if (dataID == null || !IsLookupRegistered(lookup, dataID))
			return;

		lookups [dataID].Remove(lookup);

		foreach(string inlineDataID in GetDataIDInlineParts(dataID))
			inlineLookups[inlineDataID].Lookups.Remove(lookup);
	}

    /// <summary>
    /// 更新缓存数据
    /// </summary>
    /// <param name="dataID">数据的唯一id</param>
    /// <param name="incomingValue">刷新的数据对象</param>
	private void UpdateLookups(string dataID, object incomingValue)
	{
        //刷新的缓存数据一定要先存在
		if(dataID != null && lookups.ContainsKey(dataID))
		{
            //获取所有在激活状态及允许更新的缓存对象
			IEnumerable<DataLookup> lookupsToUpdate = lookups[dataID].Where(arg => arg != null && arg.gameObject.activeInHierarchy && arg.AllowLookupUpdate);

			//foreach(DataLookup lookup in lookupsToUpdate)
			for(int i = 0; i < lookupsToUpdate.Count(); i++)
			{
				DataLookup lookup = lookupsToUpdate.ElementAt(i);

				if(lookup != null)
					lookup.OnLookupUpdate(dataID, incomingValue);
			}
		}
		
		if(dataID != null && inlineLookups.ContainsKey(dataID)) {
			string finalDataID;
			bool inlineFound = SearchDataByID<string>(dataID, out finalDataID);
			if (!inlineFound)
			{
				// todo
			}

			InlineLookupEntry inlineEntry = inlineLookups[dataID];

			string previousCachedInlineValue = inlineEntry.CachedValue;

			if(!AreDataIDEqual(previousCachedInlineValue, finalDataID)) {
				inlineEntry.CachedValue = finalDataID;
				inlineLookups[dataID] = inlineEntry;
				
				foreach(DataLookup lookup in inlineEntry.Lookups.Where(arg => arg != null))
				{
					if(lookup != null) {
						RemoveLookup(previousCachedInlineValue, lookup);
						AddLookup(finalDataID, lookup);
					}
				}
			}
		}

		//背包数据变更通知
		if(dataID == "inventory")
		{
			GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "Data_InventoryChanged", incomingValue);
		}
	}

	public void ClearLookup(DataLookup lookup) {
        //
        //EB.Debug.LogLookup("移除_缓存组件_ClearLookup_name:" + lookup.name);

        UnityEngine.Object.Destroy(lookup);

        foreach (List<DataLookup> lookupsList in lookups.Values)
        {
            lookupsList.Remove(lookup);
        }
		foreach(InlineLookupEntry entry in inlineLookups.Values)
        {
            entry.Lookups.Remove(lookup);
        }
	}

    /// <summary>
    /// 判断是否注册过这个缓存组件
    /// </summary>
    /// <param name="lookup">缓存组件</param>
    /// <param name="dataID">缓存的唯一id</param>
    /// <returns></returns>
	public bool IsLookupRegistered(DataLookup lookup, string dataID) {
		return lookups.ContainsKey(dataID) && lookups[dataID].Contains(lookup);
	}
	
	private Dictionary<string, InlineLookupEntry> inlineLookups = new Dictionary<string, InlineLookupEntry>();

	private struct InlineLookupEntry {
		public List<DataLookup> Lookups;
		public string CachedValue;
	}
	#endregion

	#region Update lookups call stash
    /// <summary>
    /// 搜索的数据存储结构
    /// </summary>
	private struct UpdateLookupsCall {
		public string DataID;
		public object Value;
	}
	/// <summary>
    /// 将要更新的缓存数据队列
    /// </summary>
	private EB.Collections.Queue<UpdateLookupsCall> updateLookupsCallsStack = new EB.Collections.Queue<UpdateLookupsCall>();
    /// <summary>
    /// 添加进入将要更新的缓存数据队列
    /// </summary>
    /// <param name="dataID">数据的唯一id</param>
    /// <param name="value">数据对象</param>
    private void StashUpdateLookupsCall(string dataID, object value) {
		updateLookupsCallsStack.Enqueue(new UpdateLookupsCall() {DataID = dataID, Value = value});
	}
	/// <summary>
    /// 更新数据:从刷新列表拿去刷新之前是的数据
    /// </summary>
	private void ProcessUpdateLookupsCallsStack() {
		while(updateLookupsCallsStack.Count > 0) {
			UpdateLookupsCall call = updateLookupsCallsStack.Dequeue();
			UpdateLookups(call.DataID, call.Value);
		}
	}
	#endregion

	public Hashtable GetDataCache() {
		return Johny.HashtablePool.Claim(dataCache);
	}
    
	/// <summary>
    /// 是否在缓存数据中
    /// </summary>
	private bool isCaching;
	

	#region CacheData
	public void CacheData<T>(T data) {
        if (data == null)
        {
            return;
        }
		
		bool hasPendingCaching = this.isCaching;
		isCaching = true;

		if(data is IDictionary){
			CacheIncomingHash(data as IDictionary, dataCache);
		}

		if (!hasPendingCaching)
			ProcessUpdateLookupsCallsStack ();

		isCaching = false;
	}

	public void CacheData(string dataID, object value) {
		dataID = NormalizeDataID (dataID);

		bool replaceInlineDataIDSuccess = ReplaceInlineDataIDWithCacheValue(ref dataID);

		if (!replaceInlineDataIDSuccess)
			throw new System.ArgumentException ("Some dataID's inline parts cannot be found in the cache", "dataID");

		IList<string> dataIDSplit = SplitDataID(dataID);

		Dictionary<string, object> rootNode = new Dictionary<string, object>();
		object currentNode = rootNode;

		int numSplits = dataIDSplit.Count;

		for(int i = 0; i < numSplits; i++) {
			string currentSplit = dataIDSplit[i];

			if(DataIDSeparators.All.Contains(currentSplit)) continue;

			object currentNodeValue;

			if(i < numSplits - 2) {
				if(dataIDSplit[i + 1] == DataIDSeparators.DOT) 
					currentNodeValue = new Dictionary<string, object>(); // currentNode is IDictionary => create an empty Dictionary

				else {
					// currentNode is IList => retrieve existing list from the cache
					// NOTE : this can be inefficient. Better use IDictionary if possible !!!
					string currentNodeValueDataID = string.Concat(dataIDSplit.Take(i + 1).ToArray());
					SearchDataByID(currentNodeValueDataID, out currentNodeValue);

					if(currentNodeValue == null)
						currentNodeValue = new ArrayList();
				}
			}

			else currentNodeValue = value;

			if(currentNode != null)
			{
				if(currentNode is IDictionary) (currentNode as IDictionary)[currentSplit] = currentNodeValue;
				
				else if(currentNode is IList) 
				{
					IList currentListNode = currentNode as IList;
					int incomingListIndex = int.Parse(currentSplit);
					int currentListNodeCount = currentListNode.Count;

					if(currentListNodeCount < incomingListIndex + 1)
					{
						IEnumerable<object> listExtraEmptySlots = Enumerable.Repeat<object>(null, incomingListIndex + 1 - currentListNodeCount);

						foreach(object emptySlot in listExtraEmptySlots)
							currentListNode.Add(emptySlot);
					}

					currentListNode[incomingListIndex] = currentNodeValue;
				}

				else EB.Debug.LogWarning("Unknown node type!");

				currentNode = currentNodeValue;
			}
		}

		CacheData(rootNode);
	}
	#endregion

	public void CacheDataIgnoreNull(string dataID, object value)
	{
		if (value == null)
			return;
		CacheData(dataID, value);
	}

	private bool NextIsArray(IList<string> splits, int current)
	{
		UnityEngine.Debug.Assert(splits != null && splits.Count > 0);
		UnityEngine.Debug.Assert(current >= 0 && current < splits.Count);

		if (current >= splits.Count - 1)
		{
			return false;
		}

		string next = splits[current + 1];
		if (next == DataIDSeparators.SQUARE_BRACKET_LEFT)
		{
			return true;
		}

		return false;
	}

	private bool NextIsHash(IList<string> splits, int current)
	{
		UnityEngine.Debug.Assert(splits != null && splits.Count > 0);
		UnityEngine.Debug.Assert(current >= 0 && current < splits.Count);

		if (current >= splits.Count - 1)
		{
			return false;
		}

		string next = splits[current + 1];
		if (next == DataIDSeparators.DOT)
		{
			return true;
		}

		return false;
	}

	private object DefaultValue(IList<string> splits, int current)
	{
		if (NextIsHash(splits, current))
		{
			return new Dictionary<string, object>();
		}

		if (NextIsArray(splits, current))
		{
			return new ArrayList();
		}

		return null;
	}

	private object ExtendStruct(object data, string extend, object defaultValue)
	{
		UnityEngine.Debug.Assert(data != null);
		UnityEngine.Debug.Assert(data is IDictionary || data is IList);

		if (data is IDictionary)
		{
			IDictionary dic = data as IDictionary;
			if (!dic.Contains(extend) && defaultValue != null)
			{
				dic[extend] = defaultValue;                
			}
			return dic.Contains(extend) ? dic[extend] : null;
		}
		else if (data is IList)
		{
			IList list = data as IList;
			int index = int.Parse(extend);
			if (list.Count < index + 1 && defaultValue != null)
			{
				IEnumerable<object> listExtraEmptySlots = Enumerable.Repeat<object>(null, index + 1 - list.Count);
				foreach (object emptySlot in listExtraEmptySlots)
				{
					list.Add(emptySlot);
				}

				list[index] = defaultValue;
			}
			return list.Count < index + 1 ? null : list[index];
		}
		else
		{
			EB.Debug.LogError("Unknown node type!");
			return null;
		}
	}

	private void FillStruct(object data, string extend, object value)
	{
		UnityEngine.Debug.Assert(data != null);
		UnityEngine.Debug.Assert(data is IDictionary || data is IList);

		if (data is IDictionary)
		{
			IDictionary dic = data as IDictionary;            
			dic[extend] = value;

			if (value == null)
			{
				dic.Remove(extend);
			}
		}
		else if (data is IList)
		{
			IList list = data as IList;
			int index = int.Parse(extend);
			if (list.Count < index + 1)
			{
				IEnumerable<object> listExtraEmptySlots = Enumerable.Repeat<object>(null, index + 1 - list.Count);
				foreach (object emptySlot in listExtraEmptySlots)
				{
					list.Add(emptySlot);
				}
			}

			list[index] = value;
		}
		else
		{
			EB.Debug.LogError("Unknown node type!");
		}
	}

	private void RefreshStruct(string dataID, object cacheNode)
	{
		if (cacheNode == null)
		{
			StashUpdateLookupsCall(dataID, null);
		}
		else if (cacheNode is IDictionary)
		{
			IDictionary dic = cacheNode as IDictionary;
			List<string> keys = new List<string>();
			foreach (string key in dic.Keys)
			{
				keys.Add(key);
			}

			foreach (string key in keys)
			{
				string loopID = string.IsNullOrEmpty(dataID) ? key : dataID + "." + key;

				if (dic[key] == null)
				{
					dic.Remove(key);
					StashUpdateLookupsCall(loopID, null);
				}
				else
				{
					RefreshStruct(loopID, dic[key]);
					StashUpdateLookupsCall(loopID, dic[key]);
				}
			}
		}
		else if (cacheNode is ArrayList)
		{
			ArrayList al = cacheNode as ArrayList;
			for (int i = 0; i < al.Count; ++i)
			{
				string loopID = dataID + "[" + i + "]";

				if (al[i] != null && (al[i] is IDictionary || al[i] is ArrayList))
				{
					RefreshStruct(loopID, al[i]);
				}

				StashUpdateLookupsCall(loopID, al[i]);
			}
		}
		else
		{
			StashUpdateLookupsCall(dataID, cacheNode);
		}
	}

	public void SetCache(string dataID, object value, bool refresh)
	{
		dataID = NormalizeDataID(dataID);
		if (!ReplaceInlineDataIDWithCacheValue(ref dataID))
		{
			throw new System.ArgumentException("Some dataID's inline parts cannot be found in the cache", "dataID");
		}

		IList<string> dataIDSplit = SplitDataID(dataID);

		object parentNode = null;
		object currentNode = dataCache;
		string currentSplit = "";
		string loopID = "";
		EB.Collections.Stack<KeyValuePair<string, object>> ids = new EB.Collections.Stack<KeyValuePair<string, object>>();
		for (int i = 0; i < dataIDSplit.Count; i++)
		{
			if (DataIDSeparators.All.Contains(dataIDSplit[i]))
			{
				continue;
			}

			currentSplit = dataIDSplit[i];
			parentNode = currentNode;
			object defaultValue = DefaultValue(dataIDSplit, i);
			currentNode = ExtendStruct(currentNode, currentSplit, defaultValue);

			if (string.IsNullOrEmpty(loopID))
			{
				loopID = currentSplit;
			}
			else if (parentNode is IDictionary)
			{
				loopID += "." + currentSplit;
			}
			else if (parentNode is IList)
			{
				loopID += "[" + currentSplit + "]";
			}
			ids.Push(new KeyValuePair<string, object>(loopID, currentNode));
		}
		FillStruct(parentNode, currentSplit, value);

		// drop old value
		ids.Pop();
		if (refresh)
		{
			// refresh children
			RefreshStruct(dataID, value);
		}
		else
		{
			// refresh new value
			ids.Push(new KeyValuePair<string, object>(loopID, value));
		}

		// refresh parents
		while (ids.Count > 0)
		{
			var pair = ids.Pop();
			StashUpdateLookupsCall(pair.Key, pair.Value);
		}

		// trigger callbacks
		ProcessUpdateLookupsCallsStack();
	}


	#region cpu 100ms(string.format)
    /// <summary>
    /// 处理将要缓存的数据
    /// </summary>
    /// <param name="incomingHash">准备添加添加的哈希数据</param>
    /// <param name="cacheNode">当前缓存的哈希数据</param>
    /// <param name="dataID">数据唯一id</param>
	private void CacheIncomingHash(IDictionary incomingHash, IDictionary cacheNode, string dataID = null)
	{
		foreach (DictionaryEntry pair in incomingHash)
		{
			object incomingValue = pair.Value;
			string incomingKey = pair.Key.ToString();
            //唯一id
			string loopID;
			if(string.IsNullOrEmpty(dataID)){
				loopID = incomingKey;
			}
			else
			{
				loopID = $"{dataID}.{incomingKey}";
			}

			if (incomingValue == null)
			{
				if (cacheNode.Contains(incomingKey))
				{
					cacheNode.Remove(incomingKey);
					StashUpdateLookupsCall(loopID, incomingValue);
				}
			}
			else if (!(incomingValue is IDictionary))
			{
				cacheNode[incomingKey] = incomingValue;

				IList myList = incomingValue as IList;
				if (myList != null)
				{
					int i = 0;
					foreach (object element in myList)
					{
						var tmp = $"{loopID}.{i}";
						StashUpdateLookupsCall(tmp, element);
						i++;
					}

					StashUpdateLookupsCall(loopID, incomingValue);
				}
				else
				{
					StashUpdateLookupsCall(loopID, incomingValue);
				}
			}
			else
			{
                //value转换成之前的哈希数据
				IDictionary incomingValueAsCacheHash = incomingValue as IDictionary;
                
				if (!cacheNode.Contains(incomingKey.ToString()) || !(cacheNode[incomingKey] is IDictionary))//缓存哈希数据当中没有这个数据，或者这个数据不是一个哈希数据
				{
					cacheNode[incomingKey] = CloneData(incomingValueAsCacheHash) as IDictionary;// incomingValueAsCacheHash;
					this.CacheIncomingHash(incomingValueAsCacheHash, cacheNode[incomingKey] as IDictionary, loopID);
					StashUpdateLookupsCall(loopID, incomingValue);
				}
				else
				{
					this.CacheIncomingHash(incomingValueAsCacheHash, cacheNode[incomingKey] as IDictionary, loopID);
					StashUpdateLookupsCall(loopID, incomingValueAsCacheHash);
				}
			}
		}
	}
	#endregion

	#region hash searching
	/// <summary>
	/// Search the cache for given dataID.
    /// 从缓存当中获取相应数据id
	/// </summary>
	/// <returns><c>true</c>, if data found in the cache, <c>false</c> otherwise.</returns>
	/// <param name="dataID">Data ID.数据名称</param>
	/// <param name="foundObject">if data found in the cache, found value is assigned to foundObject, otherwise default value for type T找到就返相应的对象，没有就返回默认对象</param>
	/// <typeparam name="T">The type of the dataID value</typeparam>
	public bool SearchDataByID<T>(string dataID, out T foundObject, IDictionary hashtableToSearchIn = null) {
		if (hashtableToSearchIn == null)
			hashtableToSearchIn = dataCache;

		object searchFoundObject;
		bool result = SearchHashByDataID(dataID, hashtableToSearchIn, out searchFoundObject);

		if(result){
			foundObject = (searchFoundObject != null && searchFoundObject.GetType().IsPrimitive)
				? (T) System.Convert.ChangeType(searchFoundObject, typeof(T))
				: (T) searchFoundObject;
		}
		else{
			foundObject = default(T); 
		}
		
		return result;
	}
	
	public bool SearchHashtableByID(string dataID, out Hashtable foundObject, IDictionary hashtableToSearchIn = null){
		if (hashtableToSearchIn == null)
			hashtableToSearchIn = dataCache;

		object searchFoundObject;
		bool result = SearchHashByDataID(dataID, hashtableToSearchIn, out searchFoundObject);

		if(result){
			foundObject = searchFoundObject as Hashtable;
		}
		else{
			foundObject = Johny.HashtablePool.Claim(); 
		}
		
		return result;
	}

	public bool SearchIntByID(string dataID, out int foundInt, IDictionary hashtableToSearchIn = null){
		if (hashtableToSearchIn == null)
			hashtableToSearchIn = dataCache;

		object searchFoundObject;
		bool result = SearchHashByDataID(dataID, hashtableToSearchIn, out searchFoundObject);

		if(result){
			if(searchFoundObject is string){
				int.TryParse(searchFoundObject as string, out foundInt);
			}
			else{
				foundInt = Convert.ToInt32(searchFoundObject);
			}
		}
		else{
			foundInt = default(int); 
		}
		
		return result;
	}

	/// <summary>
	/// Searchs the hash by data ID.
    /// 从哈希表当中搜索相应的数据
	/// </summary>
	/// <returns><c>true</c> if data found, <c>false</c> otherwise.</returns>
	/// <param name="dataID">Data ID_数据的唯一id</param>
	/// <param name="hash">Hash_搜索的哈希表</param>
	/// <param name="foundObject">Found object_搜索的对象</param>
	/// <param name="considerNullParent">If set to <c>true</c>, this function will return TRUE if searched dataID is child of a NULL parent (this is useful to update children when deleting a parent node)</param>
	private bool SearchHashByDataID(string dataID, IDictionary hash, out object foundObject, bool considerNullParent = false) {
		bool replaceInlineDataIDSuccess = ReplaceInlineDataIDWithCacheValue(ref dataID);

		foundObject = null;

		if (!replaceInlineDataIDSuccess)
			return false;

		IEnumerator dataIDSplitEnumerator = SplitDataID(dataID, false).GetEnumerator();

		object currentCacheNode = hash;
		
		while(dataIDSplitEnumerator.MoveNext()) {
			bool dataIDFound = false;
			object currentSplit = dataIDSplitEnumerator.Current;
			
			if(currentCacheNode is IDictionary) {	
				if(((IDictionary) currentCacheNode).Contains(currentSplit.ToString())) {
					dataIDFound = true;
					currentCacheNode = (currentCacheNode as IDictionary)[currentSplit.ToString()];
				}
			}
			else if(currentCacheNode is IList) {
				IList currentArrayCacheNode = (IList) currentCacheNode;
				int index = System.Convert.ToInt32(currentSplit);
				
				if(index >= 0 && index < currentArrayCacheNode.Count) {
					currentCacheNode = currentArrayCacheNode[index];
					dataIDFound = true;
				}

				else if(considerNullParent)
					return true;
			}
			else if(currentCacheNode == null && considerNullParent)
				return true;
			
			if(!dataIDFound)
				return false;
		}
		
		foundObject = currentCacheNode;
		return true;
	}
	#endregion

	private struct DataIDSeparators {
		public const string DOT = ".";
		public const string SQUARE_BRACKET_LEFT = "[";
		public const string SQUARE_BRACKET_RIGHT = "]";
		public const string DOUBLE_QUOTES = "\"";

		public static readonly IList<string> All = new List<string> {DOT, SQUARE_BRACKET_LEFT, SQUARE_BRACKET_RIGHT, DOUBLE_QUOTES};
	}

	#region Split DataID
	private static Regex splitDataIDRegex;

	private static IList<string> SplitDataID(string dataID, bool includeSeparatorsInResult = true) {
		string[] split;

		if(splitDataIDRegex == null) {
			// convert separators list to "([\.\[\]\"])" string
			string pattern = string.Join(
				"", DataIDSeparators.All.Select(
					arg => string.Format("\\{0}",arg)
				).ToArray()
			);

			pattern = string.Format("([{0}])",pattern);

			splitDataIDRegex = new Regex(pattern);
		}
		
		split = splitDataIDRegex.Split(dataID);


		IList<string> allSeparators = DataIDSeparators.All;

		split = split.Where(
			arg => (arg != string.Empty) && (includeSeparatorsInResult || !allSeparators.Contains(arg))
		).ToArray();
		
		return split;
	}
	#endregion

    /// <summary>
    /// 克隆数据
    /// </summary>
    /// <param name="source">数据源</param>
    /// <returns></returns>
	public static object CloneData<T>(T source) {
		if(source is IDictionary) {
			IDictionary sourceDic = source as IDictionary;
			Hashtable cloneDic= Johny.HashtablePool.Claim();
			foreach (DictionaryEntry entry in sourceDic)
				cloneDic.Add(entry.Key.ToString(), CloneData(entry.Value));

			return cloneDic;
		}

		if(source is IList) {
			IList sourceList = source as IList;
			ArrayList cloneList = Johny.ArrayListPool.Claim();
			foreach(object entry in sourceList)
				cloneList.Add(CloneData(entry));

			return cloneList;
		}

		return source;
	}

	#region Inline dataID
	/// <summary>
	/// In 'inventory[{selection.someItemID}].name', replaces 'selection.someItemID' by its lookup value
	/// </summary>
	/// <returns>The inline data identifier with cache value.</returns>
	/// <param name="dataID">Data I.</param>
	private static Regex inlinePartRegex = new Regex (@"[\{\<]([^\{\}]+)[\}\>]"); // in "{node1.node2}.node3.{node4}" => matches "node1.node2" and "node4";
	/// <summary>
	/// Replaces the inline data identifier with cache value.
	/// </summary>
	/// <returns><c>true</c>, if all inline parts could be found in the cache, otherwise <c>false</c>
	/// <param name="dataID">Data I.</param>
	private bool ReplaceInlineDataIDWithCacheValue(ref string dataID) {
		bool hasNotFoundInCacheInlinePart = false;

		dataID = inlinePartRegex.Replace(dataID, arg => { 
					object data;
					bool isDataFound = SearchDataByID(arg.Groups[1].Value, out data);

					if(!isDataFound)
						hasNotFoundInCacheInlinePart = true;

					string id = "";

					if(isDataFound && data != null)
					{
						id = data.ToString();
					}

					return id;
				});

		return !hasNotFoundInCacheInlinePart;
	}

	private static IEnumerable<string> GetDataIDInlineParts(string dataID) {
		return inlinePartRegex.Matches(dataID).Cast<Match>()
			.Select<Match, string>(arg => arg.Groups[1].Value)
				.Where(arg => arg != null);
	}
	#endregion

	#region DataID normalization
	/// <summary>
	/// Convert 'someData["ok"]' to 'someData.ok'
	/// </summary>
	/// <returns>The normalized dataID</returns>
	/// <param name="dataID">Un-normalized dataID</param>
	private static Regex normalizeDataIDRegex = new Regex(@"\[""(.+)""\]");
	public static string NormalizeDataID(string dataID) {
		return normalizeDataIDRegex.Replace(dataID, arg => "." + arg.Groups[1].Value);
	}
	
	/// <summary>
	/// Normalize given dataIDs and then check if they're equal. Ex. 'someData["ok"]' will be EQUAL to 'someData.ok'
	/// </summary>
	/// <returns><c>true</c>, if the two normalized dataID are equal or both NULL, <c>false</c> otherwise.</returns>
	/// <param name="dataIDA">Data identifier A</param>
	/// <param name="dataIDB">Data identifier B</param>
	public static bool AreDataIDEqual(string dataIDA, string dataIDB) {
		bool isANull = string.IsNullOrEmpty(dataIDA);
		bool isBNull = string.IsNullOrEmpty(dataIDB);

		if(isANull || isBNull) return (isANull == isBNull);

		return NormalizeDataID(dataIDA) == NormalizeDataID(dataIDB);
	}

	/// <summary>
	/// 对于某一方的id是需要替换的情况  如角色面板中装备的穿和脱 由于多个角色的存在 装备槽的dataid 存在{} 
	/// </summary>
	/// <param name="dataIDA"></param>
	/// <param name="dataIDB"></param>
	/// <returns></returns>
	public static bool AreDataIDEqualInLine(string dataIDA, string dataIDB)
	{
		bool isANull = string.IsNullOrEmpty(dataIDA);
		bool isBNull = string.IsNullOrEmpty(dataIDB);

		if (isANull || isBNull) return (isANull == isBNull);

		//return NormalizeDataID(dataIDA) == NormalizeDataID(dataIDB);

		if (NormalizeDataID(dataIDA) == NormalizeDataID(dataIDB)) return true;
		string tmp_DataIDA = new string(dataIDA.ToCharArray());
		string tmp_DataIDB = new string(dataIDB.ToCharArray());
		DataLookupsCache.instance.ReplaceInlineDataIDWithCacheValue(ref tmp_DataIDA);
		DataLookupsCache.instance.ReplaceInlineDataIDWithCacheValue(ref tmp_DataIDB);
		return (tmp_DataIDA == tmp_DataIDB);

	}
   
	#endregion
}
