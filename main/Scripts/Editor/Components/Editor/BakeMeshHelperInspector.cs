using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BakeMeshHelper))]
public class BakeMeshHelperInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		BakeMeshHelper helper = target as BakeMeshHelper;

		if (GUILayout.Button("Group Geometry Meshes"))
		{
			helper.GroupGeometryMeshes();
		}

		if (GUILayout.Button("Reset Groups"))
		{
			helper.ResetGroup();
		}

		if (GUILayout.Button("Validate Groups"))
		{
			helper.ValidateGroup();
		}
	}
}
