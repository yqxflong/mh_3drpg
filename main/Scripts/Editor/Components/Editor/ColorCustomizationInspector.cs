using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ColorCustomization))]
public class ColorCustomizationInspector : Editor 
{
	public static HeroColorPresets m_HeroColorPresets = null;
	private static List<string> m_PresetList = new List<string>();
	public static string GetHeroPresetsAssetPath()
	{
		return "Assets/_GameAssets/Res/MISC/Customization/HeroColorPresets.prefab";
	}

	void OnEnable()
	{
		if(HeroColorPresets.Instance == null)
		{
			GameObject go = GameObject.Find("__HeroColorPreset");
			if(go == null)
			{
				GameObject prefab = AssetDatabase.LoadAssetAtPath(GetHeroPresetsAssetPath(), typeof(GameObject)) as GameObject;
				if(prefab == null)
				{
					Debug.LogError("Failed to load hero color preset prefab");
					return;
				}
				go = GameObject.Instantiate(prefab) as GameObject;
				go.name = "__HeroColorPreset";
				m_HeroColorPresets = go.GetComponent<HeroColorPresets>();
			}
		}
		else
		{
			m_HeroColorPresets = HeroColorPresets.Instance;
		}
		if(m_HeroColorPresets != null)
		{
			m_PresetList.Clear();
			m_PresetList.Add("None");
			int presetCount = m_HeroColorPresets.PresetColorCount;
			for(int i = 0; i < presetCount; i++)
			{
				m_PresetList.Add (string.Format("Preset {0}", i + 1));
			}

			ColorCustomization customization = target as ColorCustomization;
			if(customization != null)
			{
				if(customization.m_SkinColorIndex >= presetCount)
				{
					customization.m_SkinColorIndex = -1;
				}
				if(customization.m_HairColorIndex >= presetCount)
				{
					customization.m_HairColorIndex = -1;
				}
				if(customization.m_EyeColorIndex >= presetCount)
				{
					customization.m_EyeColorIndex = -1;
				}
				if(customization.m_EquipmentColorIndex >= presetCount)
				{
					customization.m_EquipmentColorIndex = -1;
				}
			}
		}
	}

	void OnDisable()
	{
		GameObject go = GameObject.Find("__HeroColorPreset");
		if(go != null)
		{
			DestroyImmediate(go);
		}
		m_HeroColorPresets = null;
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		ColorCustomization customization = target as ColorCustomization;
		if(customization == null)
		{
			return;
		}

		customization.m_UseDefaultColor = GUILayout.Toggle(customization.m_UseDefaultColor, "Use Default Color");
		if(customization.m_UseDefaultColor)
		{
			return;
		}

		customization.m_UseHeroColorPreset = GUILayout.Toggle(customization.m_UseHeroColorPreset, "Use Hero Color Presets");
		if(customization.m_UseHeroColorPreset)
		{
			GUILayout.Label("Select preset colors");
			NGUIEditorTools.BeginContents();
			{
				GUILayout.BeginHorizontal();
				{
					customization.m_SkinColorIndex = EditorGUILayout.Popup("Skin Color Preset", customization.m_SkinColorIndex + 1, m_PresetList.ToArray()) - 1;

					if(customization.m_SkinColorIndex >= 0 && m_HeroColorPresets != null)
					{
						GUI.enabled = false;
						EditorGUILayout.ColorField(m_HeroColorPresets.GetSkinColor(customization.m_SkinColorIndex));
						GUI.enabled = true;
					}
				}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				{
					customization.m_HairColorIndex = EditorGUILayout.Popup("Hair Color Preset", customization.m_HairColorIndex + 1, m_PresetList.ToArray()) - 1;

					if(customization.m_HairColorIndex >= 0 && m_HeroColorPresets != null)
					{
						GUI.enabled = false;
						EditorGUILayout.ColorField(m_HeroColorPresets.GetHairColor(customization.m_HairColorIndex));
						GUI.enabled = true;
					}
				}
				GUILayout.EndHorizontal();


				GUILayout.BeginHorizontal();
				{
					customization.m_EyeColorIndex = EditorGUILayout.Popup("Eye Color Preset", customization.m_EyeColorIndex + 1, m_PresetList.ToArray()) - 1;
					if(customization.m_EyeColorIndex >= 0 && m_HeroColorPresets != null)
					{
						GUI.enabled = false;
						EditorGUILayout.ColorField(m_HeroColorPresets.GetEyeColor(customization.m_EyeColorIndex));
						GUI.enabled = true;
					}
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				{
					customization.m_EquipmentColorIndex = EditorGUILayout.Popup("Equipment Color Preset", customization.m_EquipmentColorIndex + 1, m_PresetList.ToArray()) - 1;

					if(customization.m_EquipmentColorIndex >= 0 && m_HeroColorPresets != null)
					{
						GUI.enabled = false;
						EditorGUILayout.ColorField(m_HeroColorPresets.GetEquipmentColor(customization.m_EquipmentColorIndex));
						GUI.enabled = true;
					}
				}
				GUILayout.EndHorizontal();

				if(GUILayout.Button("Enable Color Customization"))
				{
					SkinnedMeshRenderer[] renderers = customization.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
					foreach(SkinnedMeshRenderer renderer in renderers)
					{
						if(renderer != null)
						{
							foreach(Material mat in renderer.sharedMaterials)
							{
								mat.SetFloat("EBG_COLORCUSTOMIZATION", 1);
								mat.DisableKeyword("EBG_COLORCUSTOMIZATION_OFF");
								mat.EnableKeyword("EBG_COLORCUSTOMIZATION_ON");
							}
						}
					}
				}
				if(GUILayout.Button("Disable Color Customization"))
				{
					SkinnedMeshRenderer[] renderers = customization.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
					foreach(SkinnedMeshRenderer renderer in renderers)
					{
						if(renderer != null)
						{
							foreach(Material mat in renderer.sharedMaterials)
							{
								mat.SetFloat("EBG_COLORCUSTOMIZATION", 0);
								mat.DisableKeyword("EBG_COLORCUSTOMIZATION_ON");
								mat.EnableKeyword("EBG_COLORCUSTOMIZATION_OFF");
							}
						}
					}
				}
			}
			NGUIEditorTools.EndContents();
		}
		else
		{
			GUILayout.Label("Set tint colors");
			NGUIEditorTools.BeginContents();
			{
				customization.m_TintColor1 = EditorGUILayout.ColorField("Tint Color 1 (R Channel)", customization.m_TintColor1); 
				customization.m_TintColor2 = EditorGUILayout.ColorField("Tint Color 2 (G Channel)", customization.m_TintColor2);
				customization.m_TintColor3 = EditorGUILayout.ColorField("Tint Color 3 (B Channel)", customization.m_TintColor3);
				customization.m_TintColor4 = EditorGUILayout.ColorField("Tint Color 4 (A Channel)", customization.m_TintColor4);
			}
			NGUIEditorTools.EndContents();
		}
	}
}
