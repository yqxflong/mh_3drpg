using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LocatorHelper))]
public class LocatorHelperInspector : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		GUILayout.Space (8);

		if(GUILayout.Button("Add Locator Group"))
		{
			AddLocatorGroup();
		}
	}

	void AddLocatorGroup()
	{
		LocatorHelper helper = target as LocatorHelper;
		if(helper == null)
		{
			return;
		}

		GameObject go = new GameObject("NewLocatorGroup");
		go.AddComponent<LocatorGroup>();
		go.transform.parent = helper.transform;
	}
}
