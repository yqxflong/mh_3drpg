using UnityEngine;
using System.Collections;

public class GlobalNavHelper : MonoBehaviour 
{
	public Vector3 m_Range = new Vector3(64, 64, 64);

	public bool m_ShowNavRangeFlag = true;

	void OnDrawGizmos()
	{
		if(m_ShowNavRangeFlag)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(transform.position + new Vector3(m_Range.x / 2, 0, m_Range.z / 2), m_Range);
		}
	}
}
