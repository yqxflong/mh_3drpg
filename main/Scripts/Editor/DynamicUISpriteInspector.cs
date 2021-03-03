using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(DynamicUISprite), true)]
public class DynamicUISpriteInspector : UIBasicSpriteEditor
{
	/// <summary>
	/// Draw the atlas and sprite selection fields.
	/// </summary>
    /// 

    //private string m_spriteName = string.Empty;
	
	protected override bool ShouldDrawProperties ()
	{
		DynamicUISprite sprite = target as DynamicUISprite;
		GUILayout.BeginHorizontal();
		GUILayout.Label("DynamicAtlas Type", GUILayout.Width(76f));
		eDynamicAtlasType atlas_type = (eDynamicAtlasType)EditorGUILayout.EnumPopup("", sprite.DynamicAtlasType);
		GUILayout.EndHorizontal();
		
		if(atlas_type != sprite.DynamicAtlasType)
		{
			sprite.DynamicAtlasType = atlas_type;
		}

		NGUIEditorTools.DrawProperty("Material", serializedObject, "mSharedDynamicMaterial");
		NGUIEditorTools.DrawProperty("Sprite", serializedObject, "mSpriteName");
		NGUIEditorTools.DrawProperty("Spinning Render", serializedObject, "mSpinningRender");
		NGUIEditorTools.DrawProperty("Default Render", serializedObject, "mDefaultRender");

        if (sprite.type == UIBasicSprite.Type.Simple)
        {
            NGUIEditorTools.DrawProperty("Clip", serializedObject, "mClipType");
        }        

        //DrawDefaultInspector();
        
		return true;
	}

	/// <summary>
	/// All widgets have a preview.
	/// </summary>
	
	public override bool HasPreviewGUI ()
	{
		return (Selection.activeGameObject == null || Selection.gameObjects.Length == 1);
	}
	
	/// <summary>
	/// Draw the sprite preview.
	/// </summary>
	
	public override void OnPreviewGUI (Rect rect, GUIStyle background)
	{
		//UISprite sprite = target as UISprite;
		//if (sprite == null || !sprite.isValid) return;
		DynamicUISprite sprite = target as DynamicUISprite;
		if(sprite == null) return;
		//if(sprite.material != null) return;

		//Texture2D tex = sprite. as Texture2D;
		//if (tex == null) return;

		//UISpriteData sd = sprite.atlas.GetSprite(sprite.spriteName);
		//NGUIEditorTools.DrawSprite(tex, rect, sd, sprite.color);
	}
}
