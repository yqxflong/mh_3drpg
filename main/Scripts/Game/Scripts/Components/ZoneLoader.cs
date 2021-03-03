using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 区域加载
/// </summary>
public class ZoneLoader : MonoBehaviour
{
    /// <summary>
    /// zones to load/turn on in current boundary
    /// 当前边界中要加载/打开的区域
    /// </summary>
	public GameObject[] m_Zones;
	/// <summary>
    /// 所有的区域边界信息
    /// </summary>
	private List<ZoneBoundary> m_Bounds = new List<ZoneBoundary>();

	/// <summary>
    /// 初始化的时候就获取了当前的所有区域边界信息
    /// </summary>
	void Start () 
	{
		ZoneBoundary[] boundaries = gameObject.GetComponentsInChildren<ZoneBoundary>();
		if(boundaries != null && boundaries.Length > 0)
		{
			int len = boundaries.Length;
			for(int i = 0; i < len; ++i)
			{
				if(boundaries[i] != null)
				{
					m_Bounds.Add(boundaries[i]);
				}
			}
		}
	}

	IEnumerator LoadZones()
	{
		if(m_Zones == null || m_Zones.Length == 0)
		{
			yield break;
		}

		int len = m_Zones.Length;
		for(int i = 0; i < len; ++i)
		{
			if(m_Zones[i] != null && !m_Zones[i].activeSelf)
			{
				m_Zones[i].SetActive(true);
				//Don't use static batching, this will cost cpu time!
				//StaticBatchingUtility.Combine(m_Zones[i]);
			}
			//yield return null;
		}

		yield break;
	}

	public void RegisterActiveZone(ref List<GameObject> zones)
	{
		if(m_Zones == null || m_Zones.Length == 0)
		{
			return;
		}

		for(int i = 0; i < m_Zones.Length; i++)
		{
			zones.Add(m_Zones[i]);
		}

		StartCoroutine(LoadZones());
	}

    /// <summary>
    /// 是否在边界内
    /// </summary>
    /// <param name="position">坐标</param>
    /// <returns></returns>
	public bool IsInBoundary(Vector3 position)
	{
		int bound_count = m_Bounds.Count;
		for(int i = 0; i < bound_count; ++i)
		{
			if(m_Bounds[i].IsInBoundary(position))
			{
				return true;
			}
		}
		return false;
	}
}
