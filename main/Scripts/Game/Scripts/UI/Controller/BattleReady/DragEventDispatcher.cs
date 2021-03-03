using UnityEngine;
using System.Collections.Generic;

public class DragEventDispatcher : MonoBehaviour {

	public List<EventDelegate> onDragFunc = new List<EventDelegate>();
	public List<EventDelegate> onDragStartFunc = new List<EventDelegate>();
	public List<EventDelegate> onDragEndFunc = new List<EventDelegate>();
	//public List<EventDelegate> onDragOutFunc = new List<EventDelegate>();

	private void OnDrag()
	{
		EventDelegate.Execute(onDragFunc);
	}

	private void OnDragStart()
	{
		EventDelegate.Execute(onDragStartFunc);
	}

	private void OnDragEnd()
	{
		EventDelegate.Execute(onDragEndFunc);
	}

	//private void OnDragOut()
	//{
	//	EventDelegate.Execute(onDragOutFunc);
	//}
}
