using UnityEngine;
using System.Collections;

public class DynamicAtlasCamera : MonoBehaviour {
	public DynamicAtlas		m_Atlas;

	void OnPostRender()
	{
		if(m_Atlas != null)
		{
			m_Atlas.OnPostRender();
		}
	}
}
