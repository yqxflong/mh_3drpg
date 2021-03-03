using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(FXBillboard))]
public class FXBillboardInspector : Editor
{
	public override void OnInspectorGUI()
	{
		FXBillboard billboard = target as FXBillboard;
		
		////////////////////////////////////////////////////////////////////////////////
		/// Atlas Settings
		GUILayout.BeginHorizontal();
		GUILayout.Label("Atlas Type", GUILayout.Width(76f));
		FXAtlasType atlas_type = (FXAtlasType)EditorGUILayout.EnumPopup("", billboard.AtlasType);
		GUILayout.EndHorizontal();

		if(atlas_type != billboard.AtlasType)
		{
			billboard.AtlasType = atlas_type;
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
				
				if(billboard.atlas != atlas)
				{
					billboard.atlas = atlas;
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
			eDynamicAtlasType dynamic_atlas_type = (eDynamicAtlasType)EditorGUILayout.EnumPopup("", billboard.dynamicAtlasType);
			GUILayout.EndHorizontal();

			if(dynamic_atlas_type != billboard.dynamicAtlasType)
			{
				billboard.dynamicAtlasType = dynamic_atlas_type;
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("Sprite", GUILayout.Width(76f));
			SerializedProperty sprite_name_prop = NGUIEditorTools.DrawProperty("", serializedObject, "m_SpriteName", GUILayout.MinWidth(10f));
			GUILayout.EndHorizontal();

			billboard.spriteName = sprite_name_prop.stringValue;
		}

		////////////////////////////////////////////////////////////////////////////////
		/// From Color
		GUILayout.BeginHorizontal();
		GUILayout.Label("From Color", GUILayout.Width(76f));
		SerializedProperty from_color_prop = NGUIEditorTools.DrawProperty("", serializedObject, "FromColor", GUILayout.MinWidth(10f));
		billboard.FromColor = from_color_prop.colorValue;
		GUILayout.EndHorizontal();
		
		////////////////////////////////////////////////////////////////////////////////
		/// To Color
		GUILayout.BeginHorizontal();
		GUILayout.Label("To Color", GUILayout.Width(76f));
		SerializedProperty to_color_prop = NGUIEditorTools.DrawProperty("", serializedObject, "ToColor", GUILayout.MinWidth(10f));
		billboard.ToColor = to_color_prop.colorValue;
		GUILayout.EndHorizontal();
		
		////////////////////////////////////////////////////////////////////////////////
		/// From Size
		GUILayout.BeginHorizontal();
		GUILayout.Label("From Size", GUILayout.Width(76f));
		SerializedProperty from_size_prop = NGUIEditorTools.DrawProperty("", serializedObject, "FromSize", GUILayout.MinWidth(10f));
		billboard.FromSize = from_size_prop.floatValue;
		GUILayout.EndHorizontal();
		
		////////////////////////////////////////////////////////////////////////////////
		/// To Size
		GUILayout.BeginHorizontal();
		GUILayout.Label("To Size", GUILayout.Width(76f));
		SerializedProperty to_size_prop = NGUIEditorTools.DrawProperty("", serializedObject, "ToSize", GUILayout.MinWidth(10f));
		billboard.ToSize = to_size_prop.floatValue;
		GUILayout.EndHorizontal();
		
		////////////////////////////////////////////////////////////////////////////////
		/// Tile X
		GUILayout.BeginHorizontal();
		GUILayout.Label("Tile X", GUILayout.Width(76f));
		SerializedProperty tilte_x_prop = NGUIEditorTools.DrawProperty("", serializedObject, "TileX", GUILayout.MinWidth(10f));
		billboard.TileX = tilte_x_prop.intValue;
		GUILayout.EndHorizontal();
		
		////////////////////////////////////////////////////////////////////////////////
		/// Tile Y
		GUILayout.BeginHorizontal();
		GUILayout.Label("Tile Y", GUILayout.Width(76f));
		SerializedProperty tilte_y_prop = NGUIEditorTools.DrawProperty("", serializedObject, "TileY", GUILayout.MinWidth(10f));
		billboard.TileY = tilte_y_prop.intValue;
		GUILayout.EndHorizontal();
		
		////////////////////////////////////////////////////////////////////////////////
		/// Max Frames
		GUILayout.BeginHorizontal();
		GUILayout.Label("Max Frames", GUILayout.Width(76f));
		SerializedProperty max_frames_prop = NGUIEditorTools.DrawProperty("", serializedObject, "MaxFrames", GUILayout.MinWidth(10f));
		billboard.MaxFrames = max_frames_prop.intValue;
		GUILayout.EndHorizontal();
		
		////////////////////////////////////////////////////////////////////////////////
		/// Duration
		GUILayout.BeginHorizontal();
		GUILayout.Label("Duration", GUILayout.Width(76f));
		SerializedProperty duration_prop = NGUIEditorTools.DrawProperty("", serializedObject, "Duration", GUILayout.MinWidth(10f));
		billboard.Duration = duration_prop.floatValue;
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
		FXBillboard billboard = target as FXBillboard;
		billboard.atlas = NGUISettings.atlas;
		SelectSprite(billboard.spriteName);
	}
	
	void SelectSprite (string spriteName)
	{
		serializedObject.Update();
		
		SerializedProperty property = serializedObject.FindProperty("m_SpriteName");
		property.stringValue = spriteName;
		
		serializedObject.ApplyModifiedProperties();
		NGUISettings.selectedSprite = spriteName;
		
		//
		FXBillboard billboard = target as FXBillboard;
		billboard.spriteName = spriteName;
	}
}