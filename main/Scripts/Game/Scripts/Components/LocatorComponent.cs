using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocatorManager
{
	public const string HERO_START_LOCATOR = "HeroStart";
	private Dictionary<string, GameObject> m_Locators;
	private static LocatorManager m_Instance;

	private LocatorManager()
	{
		m_Locators = new Dictionary<string, GameObject>();
	}

	public static LocatorManager Instance
	{
		get
		{
			if(m_Instance == null)
			{
				m_Instance = new LocatorManager();
			}

			return m_Instance;
		}
	}

	public GameObject GetHeroStartLocator()
	{
		GameObject go = null;
		if(m_Locators.TryGetValue(HERO_START_LOCATOR, out go))
		{
			return go;
		}
		EB.Debug.LogWarning("There is no HeroStart locator in current level!!!");
		return null;
	}

	public GameObject GetLocator(string name)
	{
		GameObject go = null;
		if(m_Locators.TryGetValue(name, out go))
		{
			return go;
		}

		EB.Debug.LogWarning(string.Format("No locator named {0} in current level!!!!", name));
		return null;
	}

	public void AddLoactor(string name, GameObject go)
	{
		m_Locators[name] = go;
	}

	public void RemoveLocator(string name, GameObject go)
	{
		if(m_Locators.ContainsKey(name) && m_Locators[name] == go)
		{
			m_Locators.Remove(name);
		}
	}
}

public class LocatorComponent : MonoBehaviour 
{
	private string m_LocatorName;
	void  OnEnable()
	{
		Transform parent = gameObject.transform.parent;
		if(parent != null && string.Compare(gameObject.name, LocatorManager.HERO_START_LOCATOR) != 0)
		{
			m_LocatorName = string.Format("{0}_{1}", parent.name, gameObject.name);
		}
		else
		{
			m_LocatorName = gameObject.name;
		}

		LocatorManager.Instance.AddLoactor(m_LocatorName, gameObject);
	}

	void OnDisable()
	{
		LocatorManager.Instance.RemoveLocator(m_LocatorName, gameObject);
	}
}
