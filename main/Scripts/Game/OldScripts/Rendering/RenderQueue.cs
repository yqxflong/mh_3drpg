using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RenderQueue : MonoBehaviour 
{
	public enum Queue
	{
		Background = 1000,
		Geometry = 2000,
		AlphaTest = 2450,
		Transparent = 3000,
		Overlay = 4000,
	}
	  		
	public Queue BaseQueue = Queue.Geometry;
	public int Offset = 0;
	
	void Awake()
	{
		UpdateQueue();
	}
	
	void UpdateQueue()
	{
		if ( GetComponent<Renderer>() )
		{
			var material = GetComponent<Renderer>().sharedMaterial;
			if ( material )
			{
				material.renderQueue = (int)BaseQueue + Offset;
			}
		}
	}
	
#if UNITY_EDITOR
	void Update()
	{
		UpdateQueue();
	}
#endif
	
}
