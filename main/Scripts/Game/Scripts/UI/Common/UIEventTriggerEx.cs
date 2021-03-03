using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEventTriggerEx : MonoBehaviour {

	static public UIEventTriggerEx current;

	public List<EventDelegate> onPress = new List<EventDelegate>();
	public List<EventDelegate> onRelease = new List<EventDelegate>();

	void OnPress(bool pressed)
	{
		if (current != null) return;
		current = this;
		if (pressed) EventDelegate.Execute(onPress);
		else EventDelegate.Execute(onRelease);
		current = null;
	}
}
