using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MergeMeshHelper))]
public class MergeMeshHelperInspector : Editor 
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();

		MergeMeshHelper helper = target as MergeMeshHelper;

		if (GUILayout.Button("Group Geometry Objects"))
		{
			helper.GroupGeometryObjects();
		}

		if (GUILayout.Button("Reset Groups"))
		{
			helper.ResetGroup();
		}
	}
}
