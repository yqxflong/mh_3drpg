using UnityEngine;
using System.Collections;

public class DynamicAtlasRendererCamera : MonoBehaviour 
{
	public DynamicAtlasRenderer m_Renderer;

	void OnPostRender()
	{
		if(m_Renderer != null)
		{
			m_Renderer.OnPostRender();
		}
	}
}
