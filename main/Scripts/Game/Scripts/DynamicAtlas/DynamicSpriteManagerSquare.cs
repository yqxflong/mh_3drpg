using UnityEngine;

public class DynamicSpriteManagerSquare : DynamicSpriteManagerBase
{
	SquareAllocator m_Allocator;

	public DynamicSpriteManagerSquare(int width, int height)
	{
		m_Allocator = new SquareAllocator(System.Math.Min(width, height));
	}

	public override void AddDynamicSpriteData (string spriteName, Texture2D textureReference, DynamicSpriteData.eSpriteResTypes type, bool is_runtime = true)
	{
		DynamicSpriteData data = GetFreeSpriteData();
		if(data == null)
		{
			data = new DynamicSpriteData();
			m_DynamicSpriteDatas.Add(data);
		}
		else
		{
			SquareAllocator.Node node = data.UserData as SquareAllocator.Node;
			if(node != null)
			{
				m_Allocator.Free(node);
			}
		}

		if (textureReference != data.TextureReference)
		{
			data.UnloadTexture();
		}
		
		data.name = spriteName;
		data.SpriteResourceType = type;
		data.UsageReference = is_runtime ? 1 : 0;
		data.RuntimeFlag = is_runtime;
		data.TextureReference = textureReference;

		CalculateSpriteData(data);
	}
	
	public override bool SynchronizeTexture(Texture2D texture)
	{
		if(texture != null)
		{
			DynamicSpriteData sprite_data = GetSpriteData(texture.name);
			if(sprite_data != null)
			{
				//sprite_data.UnloadTexture();
				//sprite_data.TextureReference = texture;
				//sprite_data.SpriteResourceType = DynamicSpriteData.eSpriteResTypes.Asset_Bundle;

				//CalculateSpriteData(sprite_data);

				if (sprite_data.TextureReference != texture)
				{
					sprite_data.UnloadTexture();
					sprite_data.TextureReference = texture;
					sprite_data.SpriteResourceType = DynamicSpriteData.eSpriteResTypes.Asset_Bundle;

					CalculateSpriteData(sprite_data);
				}

				return true;
			}
		}

		return false;
	}
	
	void CalculateSpriteData (DynamicSpriteData data)
	{
		if(data != null)
		{
			Texture2D texture = data.TextureReference;
			if (texture != null)
			{
				SquareAllocator.Node node = m_Allocator.Allocate (texture.width, texture.height);
				if(node != null)
				{
					data.x = node.X;
					data.y = node.Y;
					data.width = texture.width;
					data.height = texture.height;
					data.UserData = node;
				}
				else
				{
					EB.Debug.LogError("Failed to allocate squre![" + texture.name + "]");
				}
			}
		}
	}
}
