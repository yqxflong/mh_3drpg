using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 数据缓存组件
/// </summary>
public class DataLookup : MonoBehaviour
{
    /// <summary>
    /// 数据唯一id列表
    /// </summary>
	[SerializeField] private List<string> dataIDList = new List<string>();
    /// <summary>
    /// 数据唯一id列表
    /// </summary>
	public List<string> DataIDList { get { return dataIDList; } }

	/// <summary>
	/// Registers DataID
	/// </summary>
	/// <param name="dataID">Data ID</param>
	/// <param name="isSecondaryDataID">If set to <c>true</c> skip dataID list's first index (reserved for defaultDataID)</param>
	public void RegisterDataID(string dataID, bool isSecondaryDataID = false) {
		if(isSecondaryDataID && dataIDList.Count == 0) dataIDList.Add(null);

		dataIDList.Add(dataID);
		OnLookupUpdate(dataID, GetLookupData<object>(dataID));
		DataLookupsCache.Instance.AddLookup(dataID, this);
	}

	/// <summary>
	/// Unregisters data ID. Note : if dataID is the default one, set corresponding row to NULL (don't remove the row from ID list)
	/// </summary>
	/// <param name="dataID">Data ID</param>
	public void UnregisterDataID(string dataID) {
		if (!IsDataIDRegistered (dataID))
			return;		

		dataIDList.Remove(dataID);
		OnLookupUpdate(null, null); // TODO : investigate if sending actual data value in place NULL is better

		DataLookupsCache.Instance.RemoveLookup(dataID, this);
	}
	
	public void ClearRegisteredDataIDs()
	{
		foreach (string dataID in dataIDList)
		{
			DataLookupsCache.Instance.RemoveLookup(dataID, this);
		}
		dataIDList.Clear();
		OnLookupUpdate(null, null); // TODO : investigate if sending actual data value in place NULL is better
	}
	
	public IList<string> RegisteredDataIDList {
		get { return new List<string>(dataIDList); }
	}

	public bool IsDataIDRegistered(string dataID) {
		return dataIDList.Contains (dataID);
	}
	
	/// <summary>
	/// Gets the first DataID registered in the list
	/// </summary>
	/// <value>The default dataID or NULL if none registered</value>
	public virtual string DefaultDataID {
		get { return dataIDList.Count > 0 ? dataIDList[0] : null; }

		set {
			if(dataIDList.Count == 0) dataIDList.Add(value);

			else {
				if(!string.IsNullOrEmpty(DefaultDataID))
					DataLookupsCache.Instance.RemoveLookup(DefaultDataID, this);

				dataIDList[0] = value;
			}

			DataLookupsCache.Instance.AddLookup(value, this);
			OnLookupUpdate(value, GetDefaultLookupData<object>());
		}
	}
	
	/// <summary>
	/// Gets the lookup data for the given dataID.
    /// 获取相应的缓存数据对象
	/// </summary>
	/// <returns>The lookup data if found in the cache, otherwise default value for given type T</returns>
	/// <param name="dataID">Data ID.</param>
	/// <typeparam name="T">The type of the dataID value</typeparam>
	public T GetLookupData<T>(string dataID) {
		T foundObject;
		DataLookupsCache.Instance.SearchDataByID<T>(dataID, out foundObject);
		return foundObject;
	}
	
	/// <summary>
	/// Gets the lookup data for the first dataID registered in the list.
    /// 获取默认的缓存数据
	/// </summary>
	/// <returns>The lookup data if found in the cache, otherwise default value for given type T</returns>
	/// <typeparam name="T">The type of the dataID value</typeparam>
	public T GetDefaultLookupData<T>() {
		if(dataIDList.Count > 0 && !string.IsNullOrEmpty(dataIDList[0]))
		{
			return GetLookupData<T>(dataIDList[0]);
		}
		else
		{
			return default(T);
		}
	}
	
	/// <summary>
	/// Called :
	///  - on Start()
	///  - when a dataID is registered / unregistered on this lookup
	///  - when registered data in cache has been updated 
    ///  刷新缓存的数据
	/// </summary>
	/// <param name="dataID">Data ID数据的唯一id</param>
	/// <param name="value">Value数据对象</param>
	public virtual void OnLookupUpdate(string dataID, object value) {
	}

	public virtual void Awake() {}

	public bool hasStarted;
	public virtual void Start() {
		hasStarted = true;			

		// register dataIDs already set in the prefab's list
		foreach(string dataID in dataIDList.Where(arg => !string.IsNullOrEmpty(arg)))
			DataLookupsCache.Instance.AddLookup(dataID, this);

		FullLookupDataUpdate ();
	}
	
	public virtual void OnDestroy() {
		DataLookupsCache.Instance.ClearLookup(this);
	}

	public virtual void OnEnable() {
		if(hasStarted)
			FullLookupDataUpdate ();
	}

	private void FullLookupDataUpdate() {
		if (dataIDList.Count == 0)
			OnLookupUpdate (null, null);
		
		else {
            
			for (int i = 0; i < dataIDList.Count; i++) {
                if (i < dataIDList.Count)
                {
                    string dataID = dataIDList[i];
                    if (!string.IsNullOrEmpty(dataID))
                        OnLookupUpdate(dataID, GetLookupData<object>(dataID));
                }
			}
		}
	}

	private bool allowLookupUpdate = true;
	public bool AllowLookupUpdate {
		get { return allowLookupUpdate; }

		set {
			allowLookupUpdate = value;

			if(value)
				FullLookupDataUpdate();
		}
	}
}
