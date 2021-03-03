using UnityEditor;
using UnityEngine;
using System.Collections;

// Put inspector into /Assets/Editor
[CustomEditor(typeof(FXBillboardParticle))]
public class FXBillboardParticleInspector : Editor
{
	public override void OnInspectorGUI()
	{
		FXBillboardParticle billboards = target as FXBillboardParticle;

		////////////////////////////////////////////////////////////////////////////////
		/// Atlas Settings
		GUILayout.BeginHorizontal();
		GUILayout.Label("Atlas Type", GUILayout.Width(76f));
		FXAtlasType atlas_type = (FXAtlasType)EditorGUILayout.EnumPopup("", billboards.AtlasType);
		GUILayout.EndHorizontal();

		if(atlas_type != billboards.AtlasType)
		{
			billboards.AtlasType = atlas_type;
		}

		//
		if(atlas_type == FXAtlasType.Static)
		{
			GUILayout.BeginHorizontal();

			if (NGUIEditorTools.DrawPrefixButton("Atlas"))
			{
				ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
			}

			SerializedProperty atlas_prop = NGUIEditorTools.DrawProperty("", serializedObject, "m_Atlas", GUILayout.MinWidth(20f));
			
			if (GUILayout.Button("Edit", GUILayout.Width(40f)))
			{
				if (atlas_prop != null)
				{
					UIAtlas atlas = atlas_prop.objectReferenceValue as UIAtlas;
					NGUISettings.atlas = atlas;
					NGUIEditorTools.Select(atlas.gameObject);
				}
			}

			if (atlas_prop != null)
			{
				UIAtlas atlas = atlas_prop.objectReferenceValue as UIAtlas;
				
				if(billboards.atlas != atlas)
				{
					billboards.atlas = atlas;
				}
			}

			GUILayout.EndHorizontal();

			////////////////////////////////////////////////////////////////////////////////
			/// Sprite Settings
			SerializedProperty sprite_name_prop = serializedObject.FindProperty("m_SpriteName");
			NGUIEditorTools.DrawAdvancedSpriteField(atlas_prop.objectReferenceValue as UIAtlas, sprite_name_prop.stringValue, SelectSprite, false);
		}
		else if(atlas_type == FXAtlasType.Dynamic)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Atlas", GUILayout.Width(76f));
			eDynamicAtlasType dynamic_atlas_type = (eDynamicAtlasType)EditorGUILayout.EnumPopup("", billboards.dynamicAtlasType);
			GUILayout.EndHorizontal();
			
			if(dynamic_atlas_type != billboards.dynamicAtlasType)
			{
				billboards.dynamicAtlasType = dynamic_atlas_type;
			}
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Sprite", GUILayout.Width(76f));
			SerializedProperty sprite_name_prop = NGUIEditorTools.DrawProperty("", serializedObject, "m_SpriteName", GUILayout.MinWidth(10f));
			GUILayout.EndHorizontal();
			
			billboards.spriteName = sprite_name_prop.stringValue;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// From Color
		GUILayout.BeginHorizontal();
		GUILayout.Label("From Color", GUILayout.Width(76f));
		SerializedProperty from_color_prop = NGUIEditorTools.DrawProperty("", serializedObject, "FromColor", GUILayout.MinWidth(10f));
		billboards.FromColor = from_color_prop.colorValue;
		GUILayout.EndHorizontal();

		////////////////////////////////////////////////////////////////////////////////
		/// To Color
		GUILayout.BeginHorizontal();
		GUILayout.Label("To Color", GUILayout.Width(76f));
		SerializedProperty to_color_prop = NGUIEditorTools.DrawProperty("", serializedObject, "ToColor", GUILayout.MinWidth(10f));
		billboards.ToColor = to_color_prop.colorValue;
		GUILayout.EndHorizontal();

		////////////////////////////////////////////////////////////////////////////////
		/// From Size
		GUILayout.BeginHorizontal();
		GUILayout.Label("From Size", GUILayout.Width(76f));
		SerializedProperty from_size_prop = NGUIEditorTools.DrawProperty("", serializedObject, "FromSize", GUILayout.MinWidth(10f));
		billboards.FromSize = from_size_prop.floatValue;
		GUILayout.EndHorizontal();

		////////////////////////////////////////////////////////////////////////////////
		/// To Size
		GUILayout.BeginHorizontal();
		GUILayout.Label("To Size", GUILayout.Width(76f));
		SerializedProperty to_size_prop = NGUIEditorTools.DrawProperty("", serializedObject, "ToSize", GUILayout.MinWidth(10f));
		billboards.ToSize = to_size_prop.floatValue;
		GUILayout.EndHorizontal();

		////////////////////////////////////////////////////////////////////////////////
		/// Tile X
		GUILayout.BeginHorizontal();
		GUILayout.Label("Tile X", GUILayout.Width(76f));
		SerializedProperty tilte_x_prop = NGUIEditorTools.DrawProperty("", serializedObject, "TileX", GUILayout.MinWidth(10f));
		billboards.TileX = tilte_x_prop.intValue;
		GUILayout.EndHorizontal();

		////////////////////////////////////////////////////////////////////////////////
		/// Tile Y
		GUILayout.BeginHorizontal();
		GUILayout.Label("Tile Y", GUILayout.Width(76f));
		SerializedProperty tilte_y_prop = NGUIEditorTools.DrawProperty("", serializedObject, "TileY", GUILayout.MinWidth(10f));
		billboards.TileY = tilte_y_prop.intValue;
		GUILayout.EndHorizontal();
	}

	void OnSelectAtlas (Object option)
	{
		serializedObject.Update();

		SerializedProperty property = serializedObject.FindProperty("m_Atlas");
		property.objectReferenceValue = option;

		serializedObject.ApplyModifiedProperties();

		NGUISettings.atlas = option as UIAtlas;

		//
		FXBillboardParticle billboards = target as FXBillboardParticle;
		billboards.atlas = NGUISettings.atlas;
		SelectSprite(billboards.spriteName);
	}

	void SelectSprite (string spriteName)
	{
		serializedObject.Update();

		SerializedProperty property = serializedObject.FindProperty("m_SpriteName");
		property.stringValue = spriteName;

		serializedObject.ApplyModifiedProperties();
		NGUISettings.selectedSprite = spriteName;

		//
		FXBillboardParticle billboards = target as FXBillboardParticle;
		billboards.spriteName = spriteName;
	}
}
