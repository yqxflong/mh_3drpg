using UnityEngine;
using System.Collections;

public class HeroColorPresets : MonoBehaviour 
{
	public const int DEFAULT_COLOR_PRESET_COUNT = 8;
	[HideInInspector]
	public Color[]	m_SkinColorPresets = new Color[DEFAULT_COLOR_PRESET_COUNT];
	[HideInInspector]
	public Color[]	m_HairColorPresets = new Color[DEFAULT_COLOR_PRESET_COUNT];
	[HideInInspector]
	public Color[]	m_EyeColorPresets = new Color[DEFAULT_COLOR_PRESET_COUNT];
	[HideInInspector]
	public Color[]	m_EquipmentColorPresets = new Color[DEFAULT_COLOR_PRESET_COUNT];

	[HideInInspector]
	public int m_MaxPresetCount = DEFAULT_COLOR_PRESET_COUNT;

	protected static HeroColorPresets m_instance;
	public static HeroColorPresets Instance
	{
		get
		{
			return m_instance;
		}
	}

	void Awake()
	{
		m_instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		
	}
	
	public int PresetColorCount
	{
		get
		{
			return m_MaxPresetCount;
		}
#if UNITY_EDITOR
		set
		{
			ResetPresetCount(value);
		}
#endif
	}

#if UNITY_EDITOR

	public void ResetPresetCount(int count)
	{
		if(m_MaxPresetCount == count)
		{
			return;
		}

		Color[]	skinColorPresets = new Color[count];
		for(int i = 0; i < count; i++)
		{
			if(i <= m_MaxPresetCount - 1)
			{
				skinColorPresets[i] = m_SkinColorPresets[i];
			}
			else
			{
				skinColorPresets[i] = Color.white;
			}
		}
		m_SkinColorPresets = skinColorPresets;

		Color[] hairColorPresets = new Color[count];
		for(int i = 0; i < count; i++)
		{
			if(i <= m_MaxPresetCount - 1)
			{
				hairColorPresets[i] = m_HairColorPresets[i];
			}
			else
			{
				hairColorPresets[i] = Color.white;
			}
		}
		m_HairColorPresets = hairColorPresets;

		Color[] eyeColorPresets = new Color[count];
		for(int i = 0; i < count; i++)
		{
			if(i <= m_MaxPresetCount - 1)
			{
				eyeColorPresets[i] = m_EyeColorPresets[i];
			}
			else
			{
				eyeColorPresets[i] = Color.white;
			}
		}
		m_EyeColorPresets = eyeColorPresets;

		Color[] equipmentColorPresets = new Color[count];
		for(int i = 0; i < count; i++)
		{
			if(i <= m_MaxPresetCount - 1)
			{
				equipmentColorPresets[i] = m_EquipmentColorPresets[i];
			}
			else
			{
				equipmentColorPresets[i] = Color.white;
			}
		}
		m_EquipmentColorPresets = equipmentColorPresets;

		m_MaxPresetCount = count;
	}
#endif

	public Color GetSkinColor(int index)
	{
		if(index < 0 || index >= m_MaxPresetCount)
		{
			EB.Debug.LogWarning("Skin color index is out of range!");
			return Color.white;
		}
		return m_SkinColorPresets[index];
	}

	public Color GetHairColor(int index)
	{
		if(index < 0 || index >= m_MaxPresetCount)
		{
			EB.Debug.LogWarning("Hair color index is out of range!");
			return Color.white;
		}
		return m_HairColorPresets[index];
	}

	public Color GetEyeColor(int index)
	{
		if(index < 0 || index >= m_MaxPresetCount)
		{
			EB.Debug.LogWarning("Eye color index is out of range!");
			return Color.white;
		}
		return m_EyeColorPresets[index];
	}

	public Color GetEquipmentColor(int index)
	{
		if(index < 0 || index >= m_MaxPresetCount)
		{
			EB.Debug.LogWarning("Equipment color index is out of range!");
			return Color.white;
		}
		return m_EquipmentColorPresets[index];
	}
}
