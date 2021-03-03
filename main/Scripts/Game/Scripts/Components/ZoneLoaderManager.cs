using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 区域加载管理器
/// </summary>
public class ZoneLoaderManager : MonoBehaviour 
{
    /// <summary>
    /// 所有加载的区域控件
    /// </summary>
	private List<ZoneLoader> m_ZoneLoaders = new List<ZoneLoader>();
	private List<GameObject> m_PrevActiveZones = new List<GameObject>();
    /// <summary>
    /// 当前激活的所有区域
    /// </summary>
	private List<GameObject> m_CurrActiveZones = new List<GameObject>();
    /// <summary>
    /// 玩家当前坐标
    /// </summary>
	private Vector3 m_PrevPlayerPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

	public static string ZoneLoaderRootName
	{
		get
		{
			return "ZoneLoaders";
		}
	}

	void Start()
	{
		ZoneLoader[] zone_loaders = GetComponentsInChildren<ZoneLoader>();
		if(zone_loaders != null && zone_loaders.Length > 0)
		{
			int len = zone_loaders.Length;
			for(int i = 0; i < len; ++i)
			{
				if(zone_loaders[i] != null)
				{
					m_ZoneLoaders.Add(zone_loaders[i]);
				}
			}
		}
	}

	void Update()
    {
        if (!GameEngine.Instance.IsTimeToRootScene)
        {
            return;
        }
        if (m_ZoneLoaders == null || m_ZoneLoaders.Count == 0)
		{
			return;
		}
		PlayerController player = PlayerManager.LocalPlayerController();
		if(player == null)
		{
			return;
		}

		Vector3 pos = player.transform.position;
		if(Vector3.Distance(m_PrevPlayerPosition, pos) > 1.0f)
		{
			UpdateZones();
		}
	}

    /// <summary>
    /// 刷新区域
    /// </summary>
	void UpdateZones()
	{
		PlayerController player = PlayerManager.LocalPlayerController();
		if(player == null)
		{
			return;
		}

		m_PrevPlayerPosition = player.transform.position;

		m_CurrActiveZones.Clear();
        //是否在边界内
		bool isInBoundary = false;
		for(int i = 0; i < m_ZoneLoaders.Count; i++)
		{
			if(m_ZoneLoaders[i] == null)
			{
				continue;
			}
			
			if(m_ZoneLoaders[i].IsInBoundary(player.transform.position))
			{
				m_ZoneLoaders[i].RegisterActiveZone(ref m_CurrActiveZones);
				isInBoundary = true;
			}
		}

		if(!isInBoundary)
		{
			//Keep what current likes if character move to none boundaried area!
			return;
		}
		
		for(int i = 0; i < m_PrevActiveZones.Count; ++i)
		{
			if(m_PrevActiveZones[i] != null && !m_CurrActiveZones.Contains(m_PrevActiveZones[i]))
			{
				m_PrevActiveZones[i].SetActive(false);
			}
		}

		m_PrevActiveZones.Clear ();
		m_PrevActiveZones.AddRange(m_CurrActiveZones);
	}
}
