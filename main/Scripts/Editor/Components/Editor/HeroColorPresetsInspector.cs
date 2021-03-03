using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(HeroColorPresets))]
public class HeroColorPresetsInspector : Editor 
{
	private static Vector2 m_ScrollPosition = Vector2.zero;
	private int m_PresetCount = 8;
	void OnEnable()
	{
		HeroColorPresets presets = target as HeroColorPresets;
		if(presets != null)
		{
			m_PresetCount = presets.PresetColorCount;
		}
	}
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		GUILayout.Space(8);
		HeroColorPresets presets = target as HeroColorPresets;

		m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

		GUILayout.BeginHorizontal();
		m_PresetCount = EditorGUILayout.IntField("Preset Count", m_PresetCount);
		if(GUILayout.Button("Resize"))
		{
			presets.PresetColorCount = m_PresetCount;
		}
		GUILayout.EndHorizontal();

		GUI.color = Color.red;
		if(NGUIEditorTools.DrawHeader("Skin Color Presets"))
		{
			DrawColorPresets(ref presets.m_SkinColorPresets);
		}
		
		GUI.color = Color.green;
		if(NGUIEditorTools.DrawHeader("Hair Color Presets"))
		{
			DrawColorPresets(ref presets.m_HairColorPresets);
		}
		GUI.color = new Color(0.0f, 0.5f, 1.0f);
		if(NGUIEditorTools.DrawHeader("Eye Color Presets"))
		{
			DrawColorPresets(ref presets.m_EyeColorPresets);
		}
		GUI.color = Color.grey;
		if(NGUIEditorTools.DrawHeader("Equipment Color Presets"))
		{
			DrawColorPresets(ref presets.m_EquipmentColorPresets);
		}

		GUILayout.EndScrollView();
	}

	void DrawColorPresets(ref Color[] presets)
	{
		int count = presets.Length;
		for(int i = 0; i < count; i++)
		{
			NGUIEditorTools.BeginContents();
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label(string.Format("Preset {0}", i + 1));
					presets[i] = EditorGUILayout.ColorField(presets[i]);
				}
				EditorGUILayout.EndHorizontal();
			}
			NGUIEditorTools.EndContents();
		}
	}
}
