using UnityEngine;
using System.Collections;

public class ColorCustomization : MonoBehaviour 
{
	[HideInInspector]
	public bool m_UseDefaultColor = true;
	[HideInInspector]
	public bool m_UseHeroColorPreset = false;
	[HideInInspector]
	public int m_SkinColorIndex = -1;
	[HideInInspector]
	public int m_HairColorIndex = -1;
	[HideInInspector]
	public int m_EyeColorIndex = -1;
	[HideInInspector]
	public int m_EquipmentColorIndex = -1;
	[HideInInspector]
	public Color m_TintColor1 = Color.white;
	[HideInInspector]
	public Color m_TintColor2 = Color.white;
	[HideInInspector]
	public Color m_TintColor3 = Color.white;
	[HideInInspector]
	public Color m_TintColor4 = Color.white;

	public bool UseHeroColorPreset
	{
		get
		{
			return m_UseHeroColorPreset;
		}
	}

	public void SetCharacterColor(int skinColorIndex, int hairColorIndex, int eyeColorIndex, int equipmenColorIndex = -1)
	{
		m_UseHeroColorPreset = true;
		m_SkinColorIndex = skinColorIndex;
		m_HairColorIndex = hairColorIndex;
		m_EyeColorIndex = eyeColorIndex;
		m_EquipmentColorIndex = equipmenColorIndex;
	}

	public void ApplyColor()
	{
		if(!Application.isPlaying)
		{
			return;
		}

		if(m_UseDefaultColor)
		{
			SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
			for(int i = renderers.Length - 1; i >= 0; i--)
			{
				Material[] mats = renderers[i].sharedMaterials;
				for(int j = mats.Length - 1; j >= 0; j--)
				{
					//mats[i].DisableKeyword("EBG_COLORCUSTOMIZATION_ON");
					//mats[i].EnableKeyword("EBG_COLORCUSTOMIZATION_OFF");
				}
			}
		}
		else if(m_UseHeroColorPreset)
		{
			if(HeroColorPresets.Instance == null)
			{
				return;
			}

			if(m_SkinColorIndex == -1 && m_HairColorIndex == -1 && m_EyeColorIndex == -1 && m_EquipmentColorIndex == -1)
			{
				SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
				for(int i = renderers.Length - 1; i >= 0; i--)
				{
					Material[] mats = renderers[i].sharedMaterials;
					for(int j = mats.Length - 1; j >= 0; j--)
					{
						//mats[i].DisableKeyword("EBG_COLORCUSTOMIZATION_ON");
						//mats[i].EnableKeyword("EBG_COLORCUSTOMIZATION_OFF");
					}
				}
			}
			else
			{
				SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
				for(int i = renderers.Length - 1; i >= 0; i--)
				{
					Material[] mats = renderers[i].sharedMaterials;
					for(int j = mats.Length - 1; j >= 0; j--)
					{
						//mats[j].DisableKeyword("EBG_COLORCUSTOMIZATION_OFF");
						//mats[j].EnableKeyword("EBG_COLORCUSTOMIZATION_ON");

						if(m_SkinColorIndex != -1)
						{
							m_TintColor1 = HeroColorPresets.Instance.GetSkinColor(m_SkinColorIndex);
							mats[j].SetColor("_Tint1", m_TintColor1);
						}

						if(m_HairColorIndex != -1)
						{
							m_TintColor2 = HeroColorPresets.Instance.GetHairColor(m_HairColorIndex);
							mats[j].SetColor("_Tint2", m_TintColor2);
						}

						if(m_EyeColorIndex != -1)
						{
							m_TintColor3 = HeroColorPresets.Instance.GetEyeColor(m_EyeColorIndex);
							mats[j].SetColor("_Tint3", m_TintColor3);
						}

						if(m_EquipmentColorIndex != -1)
						{
							m_TintColor4 = HeroColorPresets.Instance.GetEquipmentColor(m_EquipmentColorIndex);
							mats[j].SetColor("_Tint4", m_TintColor4);
						}
					}
				}
			}
		}
		else
		{
			SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
			for(int i = renderers.Length - 1; i >= 0; i--)
			{
				Material[] mats = renderers[i].sharedMaterials;
				for(int j = mats.Length - 1; j >= 0; j--)
				{
					//mats[j].DisableKeyword("EBG_COLORCUSTOMIZATION_OFF");
					//mats[j].EnableKeyword("EBG_COLORCUSTOMIZATION_ON");

					mats[j].SetColor("_Tint1", m_TintColor1);
					mats[j].SetColor("_Tint2", m_TintColor2);
					mats[j].SetColor("_Tint3", m_TintColor3);
					mats[j].SetColor("_Tint4", m_TintColor4);
				}
			}
		}
	}

	public static void ApplyHeroColor(GameObject hero, int skinColorIndex, int hairColorIndex, int eyeColorIndex)
	{
		if(hero == null)
		{
			return;
		}

		if(HeroColorPresets.Instance == null)
		{
			return;
		}

		SkinnedMeshRenderer[] renderers = hero.GetComponentsInChildren<SkinnedMeshRenderer>();
		for(int i = renderers.Length - 1; i >= 0; i--)
		{
			Material[] mats = renderers[i].materials;
			for(int j = mats.Length - 1; j >= 0; j--)
			{
				//mats[j].DisableKeyword("EBG_COLORCUSTOMIZATION_OFF");
				//mats[j].EnableKeyword("EBG_COLORCUSTOMIZATION_ON");
				
				//if(skinColorIndex != -1)
				{
					Color tintColor1 = HeroColorPresets.Instance.GetSkinColor(skinColorIndex);
					mats[j].SetColor("_Tint1", tintColor1);
				}
				
				//if(hairColorIndex != -1)
				{
					Color tintColor2 = HeroColorPresets.Instance.GetHairColor(hairColorIndex);
					mats[j].SetColor("_Tint2", tintColor2);
				}
				
				//if(eyeColorIndex != -1)
				{
					Color tintColor3 = HeroColorPresets.Instance.GetEyeColor(eyeColorIndex);
					mats[j].SetColor("_Tint3", tintColor3);
				}
			}
		}
	}

	public static void ApplySkinColor(GameObject hero, int skinColorIndex)
	{
		if(hero == null)
		{
			return;
		}
		
		if(HeroColorPresets.Instance == null)
		{
			return;
		}
		
		SkinnedMeshRenderer[] renderers = hero.GetComponentsInChildren<SkinnedMeshRenderer>();
		for(int i = renderers.Length - 1; i >= 0; i--)
		{
			Material[] mats = renderers[i].materials;
			for(int j = mats.Length - 1; j >= 0; j--)
			{
				//mats[j].DisableKeyword("EBG_COLORCUSTOMIZATION_OFF");
				//mats[j].EnableKeyword("EBG_COLORCUSTOMIZATION_ON");
				
				//if(skinColorIndex != -1)
				{
					Color tintColor1 = HeroColorPresets.Instance.GetSkinColor(skinColorIndex);
					mats[j].SetColor("_Tint1", tintColor1);
				}
			}
		}
	}

	public static void ApplyHairColor(GameObject hero, int hairColorIndex)
	{
		if(hero == null)
		{
			return;
		}
		
		if(HeroColorPresets.Instance == null)
		{
			return;
		}
		
		SkinnedMeshRenderer[] renderers = hero.GetComponentsInChildren<SkinnedMeshRenderer>();
		for(int i = renderers.Length - 1; i >= 0; i--)
		{
			Material[] mats = renderers[i].materials;
			for(int j = mats.Length - 1; j >= 0; j--)
			{
				//mats[j].DisableKeyword("EBG_COLORCUSTOMIZATION_OFF");
				//mats[j].EnableKeyword("EBG_COLORCUSTOMIZATION_ON");
				
				//if(hairColorIndex != -1)
				{
					Color tintColor2 = HeroColorPresets.Instance.GetHairColor(hairColorIndex);
					mats[j].SetColor("_Tint2", tintColor2);
				}
			}
		}
	}

	public static void ApplyEyeColor(GameObject hero, int eyeColorIndex)
	{
		if(hero == null)
		{
			return;
		}
		
		if(HeroColorPresets.Instance == null)
		{
			return;
		}
		
		SkinnedMeshRenderer[] renderers = hero.GetComponentsInChildren<SkinnedMeshRenderer>();
		for(int i = renderers.Length - 1; i >= 0; i--)
		{
			Material[] mats = renderers[i].materials;
			for(int j = mats.Length - 1; j >= 0; j--)
			{
				//mats[j].DisableKeyword("EBG_COLORCUSTOMIZATION_OFF");
				//mats[j].EnableKeyword("EBG_COLORCUSTOMIZATION_ON");

				//if(eyeColorIndex != -1)
				{
					Color tintColor3 = HeroColorPresets.Instance.GetEyeColor(eyeColorIndex);
					mats[j].SetColor("_Tint3", tintColor3);
				}
			}
		}
	}

	public static void ApplyEquipmentColor(GameObject equipmentMesh, int equipmentColorIndex)
	{
		if(equipmentMesh == null)
		{
			return;
		}
		
		if(HeroColorPresets.Instance == null)
		{
			return;
		}
		
		SkinnedMeshRenderer[] renderers = equipmentMesh.GetComponentsInChildren<SkinnedMeshRenderer>();
		for(int i = renderers.Length - 1; i >= 0; i--)
		{
			Material[] mats = renderers[i].materials;
			for(int j = mats.Length - 1; j >= 0; j--)
			{
				//mats[j].DisableKeyword("EBG_COLORCUSTOMIZATION_OFF");
				//mats[j].EnableKeyword("EBG_COLORCUSTOMIZATION_ON");
				
				//if(equipmentColorIndex != -1)
				{
					Color tintColor3 = HeroColorPresets.Instance.GetEquipmentColor(equipmentColorIndex);
					mats[j].SetColor("_Tint4", tintColor3);
				}
			}
		}
	}
}
