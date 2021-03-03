using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIButtonText), true)]
public class UIButtonTextEditor : UIButtonEditor {
	protected override void DrawProperties ()
	{
		base.DrawProperties();
		NGUIEditorTools.DrawProperty("Text", serializedObject, "buttonLabel");
	}
}
