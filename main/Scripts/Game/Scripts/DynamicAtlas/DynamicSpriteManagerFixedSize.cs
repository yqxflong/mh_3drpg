using UnityEngine;
using System.Collections;

public class DynamicSpriteManagerFixedSize : DynamicSpriteManagerBase
{
	public DynamicSpriteManagerFixedSize(int mat_width, int mat_height, int sprite_width, int sprite_height)
	{
		int column = mat_width / sprite_width;
		int row = mat_height / sprite_height;
		int total_count = column * row;

		for(int i = 0; i < total_count; i++)
		{
			DynamicSpriteData sprite_data = new DynamicSpriteData();
			sprite_data.width = sprite_width;
			sprite_data.height = sprite_height;
			sprite_data.x = (i % column) * sprite_width;
			sprite_data.y = (i / column) * sprite_height;
			m_DynamicSpriteDatas.Add (sprite_data);
		}
	}

	public override void AddDynamicSpriteData (string spriteName, Texture2D textureReference, DynamicSpriteData.eSpriteResTypes type, bool is_runtime = true)
	{
		DynamicSpriteData sprite_data = GetFreeSpriteData();
		if(sprite_data != null)
		{
			if (sprite_data.TextureReference != textureReference)
			{
				sprite_data.UnloadTexture();
			}
			
			sprite_data.name = spriteName;
			sprite_data.TextureReference = textureReference;
			sprite_data.UsageReference = is_runtime ? 1 : 0;
			sprite_data.RuntimeFlag = is_runtime;
			sprite_data.SpriteResourceType = type;
		}
		else
		{
			EB.Debug.LogError("Try to load two many fixed-size textures in dynamic atlas");
		}
	}

	public override bool SynchronizeTexture (Texture2D texture)
	{
		int size = m_DynamicSpriteDatas.size;
		for(int i = 0; i < size; i++)
		{
			if(m_DynamicSpriteDatas[i] != null && string.Compare(m_DynamicSpriteDatas[i].name, texture.name) == 0)
			{
				//m_DynamicSpriteDatas[i].TextureReference = texture;
				//m_DynamicSpriteDatas[i].SpriteResourceType = DynamicSpriteData.eSpriteResTypes.Asset_Bundle;

				if (m_DynamicSpriteDatas[i].TextureReference != texture)
				{
					m_DynamicSpriteDatas[i].UnloadTexture();
					m_DynamicSpriteDatas[i].TextureReference = texture;
					m_DynamicSpriteDatas[i].SpriteResourceType = DynamicSpriteData.eSpriteResTypes.Asset_Bundle;
				}

				return true;
			}
		}
		return false;
	}
}
