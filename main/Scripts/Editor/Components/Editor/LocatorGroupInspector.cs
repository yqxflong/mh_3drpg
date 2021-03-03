using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LocatorGroup))]
public class LocatorGroupInspector : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		GUILayout.Space(8);
		if(GUILayout.Button("Add Locator"))
		{
			AddLocator();
		}
	}

	void AddLocator()
	{
		LocatorGroup group = target as LocatorGroup;

		if(group == null)
		{
			return;
		}

		int count = group.transform.childCount;
	
		GameObject go = new GameObject(string.Format("{0}", count + 1));
		go.AddComponent<LocatorComponent>();
		go.transform.SetParent(group.transform);
	}
}
