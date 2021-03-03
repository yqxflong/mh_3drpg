using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTriggersManager {

	public Dictionary<string, Transform> AreaTriggerDict=new Dictionary<string, Transform>();

	static AreaTriggersManager _Instance;
	public static AreaTriggersManager Instance
	{
		get {
			if (_Instance == null)
			{
				_Instance = new AreaTriggersManager();
			}
			return _Instance;
		}
	}

	public void Register(string name,Transform trans)
	{
		if (!AreaTriggerDict.ContainsKey(name))
		{
			AreaTriggerDict.Add(name, trans);
		}
	}

	public void UnRegister(string name, Transform trans)
	{
		if (AreaTriggerDict.ContainsKey(name))
		{
			AreaTriggerDict.Remove(name);
		}
	}
}
