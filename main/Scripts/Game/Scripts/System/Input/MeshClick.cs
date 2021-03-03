using UnityEngine;
using System.Collections.Generic;

public class MeshClick : MonoBehaviour
{
	//[HideInInspector]public List<EventDelegate> _delegate = new List<EventDelegate>();
	public List<EventDelegate> _delegate = new List<EventDelegate>();

	public virtual void OnMeshClick (GameObject go)
	{
		EventDelegate.Execute(_delegate);
	}
}
