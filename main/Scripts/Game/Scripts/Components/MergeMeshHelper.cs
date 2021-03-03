using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MergeMeshHelper : MonoBehaviour {
	public GameObject m_GeometryRoot = null;
	public Vector2 m_ZoneStart = Vector2.zero;
	public Vector2 m_ZoneEnd = Vector2.zero;
	public int m_SplitUnitsX = 8;
	public int m_SplitUnitsY = 8;

	public bool m_DrawSplitGrids = false;

	private Dictionary<GameObject, GameObject> m_GeometryParents = new Dictionary<GameObject, GameObject>();

#if UNITY_EDITOR
	public void GroupGeometryObjects()
	{
		if(m_GeometryRoot == null)
		{
			EB.Debug.LogError("Geometry root is not assigned yet!");
			return;
		}

		int gridsX = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.x - m_ZoneStart.x) / m_SplitUnitsX);
		int gridsZ = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.y - m_ZoneStart.y) / m_SplitUnitsY);
		for(int i = 0; i < gridsX; i++)
		{
			for(int j = 0; j < gridsZ; j++)
			{
				string groupName = string.Format("MergedMeshGroup{0}{1}", i, j);
				Transform tr = transform.Find(groupName);
				if(tr == null)
				{
					GameObject go = new GameObject(groupName);
					go.transform.parent = transform;
				}
			}
		}
		MeshRenderer[] gemometries = m_GeometryRoot.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer geo in gemometries)
		{
			int offx = Mathf.FloorToInt((geo.bounds.center.x - m_ZoneStart.x) / m_SplitUnitsX);
			int offz = Mathf.FloorToInt((geo.bounds.center.z - m_ZoneStart.y) / m_SplitUnitsY);
			if(offx >= 0 && offx < gridsX && offz >= 0 && offz < gridsZ && ((GameObjectUtility.GetStaticEditorFlags(geo.gameObject) & StaticEditorFlags.LightmapStatic) != 0))
			{
				m_GeometryParents.Add(geo.gameObject, geo.transform.parent.gameObject);
				string groupName = string.Format("MergedMeshGroup{0}{1}", offx, offz);
				geo.transform.parent = transform.Find(groupName);
			}
		}
	}
#endif

	public void ResetGroup()
	{
		Dictionary<GameObject, GameObject>.Enumerator enumerator = m_GeometryParents.GetEnumerator();
		while(enumerator.MoveNext())
		{
			KeyValuePair<GameObject, GameObject> pair = enumerator.Current;
			pair.Key.transform.parent = pair.Value.transform;
		}
		m_GeometryParents.Clear();
	}

	void OnDrawGizmos()
	{
		if(!m_DrawSplitGrids)
		{
			return;
		}

		int gridsX = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.x - m_ZoneStart.x) / m_SplitUnitsX);
		int gridsZ = Mathf.CeilToInt(Mathf.Abs(m_ZoneEnd.y - m_ZoneStart.y) / m_SplitUnitsY);
		
		Gizmos.color = Color.red;
		for(int i = 0; i <= gridsX; i++)
		{
			float posX = m_ZoneStart.x + i * m_SplitUnitsX;
			if(posX > m_ZoneEnd.x)
			{
				posX = m_ZoneEnd.x;
			}
			Gizmos.DrawLine(new Vector3(posX, 0, m_ZoneStart.y), new Vector3(posX, 0, m_ZoneEnd.y));
		}

		for(int i = 0; i <= gridsZ; i++)
		{
			float posZ = m_ZoneStart.y + i* m_SplitUnitsY;
			if(posZ > m_ZoneEnd.y)
			{
				posZ = m_ZoneEnd.y;
			}
			Gizmos.DrawLine(new Vector3(m_ZoneStart.x, 0, posZ), new Vector3(m_ZoneEnd.x, 0, posZ));
		}
	}
}
