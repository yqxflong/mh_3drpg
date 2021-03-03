using UnityEngine;
using System.Collections;

public class DynamicSpriteManagerPacker : DynamicSpriteManagerBase
{
	private DynamicSpritePacker m_SpritePacker;
	private bool m_Packed = false;

	public DynamicSpriteManagerPacker(int width, int height)
	{
		m_SpritePacker = new DynamicSpritePacker(width, height, false);
	}

	public override void AddDynamicSpriteData (string spriteName, Texture2D textureReference, DynamicSpriteData.eSpriteResTypes type, bool is_runtime = true)
	{
        //EB.Debug.LogError("AddDynamicSpriteData   spriteName  : {0}", spriteName);
		DynamicSpriteData data = GetSpriteData(spriteName);
		if (data != null)
		{
			EB.Debug.LogError("AddDynamicSpriteData: spriteName {0} exists", spriteName);
			data.AddReference();
			return;
		}

		data = new DynamicSpriteData();
		data.name = spriteName;
		data.SpriteResourceType = type;
		data.UsageReference = is_runtime ? 1 : 0;
		data.RuntimeFlag = is_runtime;
		data.TextureReference = textureReference;

		m_DynamicSpriteDatas.Add(data);
		if (!m_SpritePacker.PackSprites(m_DynamicSpriteDatas))
		{
			//Debug.Log("AddDynamicSpriteData: CAN NOT pack all of the sprites in the atlas, try reuse unused node");

			//failed to pack, get free space from current page.
			var freeData = FindFreeSpriteData(textureReference.width, textureReference.height);
			if (freeData == null)
			{
				//Debug.LogWarning("AddDynamicSpriteData: CAN NOT pack all of the sprites in the atlas, try remove unused node");

				for (int i = m_DynamicSpriteDatas.size - 1; i >= 0; --i)
				{
					if (m_DynamicSpriteDatas[i].RuntimeFlag && m_DynamicSpriteDatas[i].UsageReference <= 0)
					{
						m_DynamicSpriteDatas[i].UnloadTexture();
						m_DynamicSpriteDatas.RemoveAt(i);
					}
				}

                EB.Debug.LogError("==================   m_DynamicSpriteDatas : {0}", m_DynamicSpriteDatas.size);
			}
			else
			{
				//Debug.LogFormat("AddDynamicSpriteData: {0} reuse node {1}", spriteName, freeData.name);

				m_DynamicSpriteDatas.Remove(data);
				if (freeData.TextureReference != textureReference)
				{
					freeData.UnloadTexture();
				}

				freeData.name = spriteName;
				freeData.SpriteResourceType = type;
				freeData.UsageReference = is_runtime ? 1 : 0;
				freeData.RuntimeFlag = is_runtime;
				freeData.TextureReference = textureReference;
			}

			m_Packed = false;
		}
		else
		{
			m_Packed = true;
		}
	}

	private DynamicSpriteData FindFreeSpriteData(int width, int height)
	{
		int size = m_DynamicSpriteDatas.size;
		int deltaSqure = int.MaxValue;
		DynamicSpriteData result = null;
		for (int i = 0; i < size; i++)
		{
			var sprite = m_DynamicSpriteDatas[i];
			if (sprite != null && sprite.UsageReference <= 0 && sprite.RuntimeFlag && sprite.width >= width && sprite.height >= height)
			{
				int w = sprite.width - width;
				int h = sprite.height - height;
				int ds = w * w + h * h;
				if (ds < deltaSqure)
				{
					deltaSqure = ds;
					result = sprite;

					if (deltaSqure == 0)
					{
						break;
					}
				}
			}
		}
		return result;
	}

	public override void PackSprites()
	{
		if (m_Packed)
		{
			return;
		}

		if (!m_SpritePacker.PackSprites(m_DynamicSpriteDatas))
		{
			//Debug.LogWarning("PackSprites: CAN NOT pack all of the sprites in the atlas, try remove unused node");

			for (int i = m_DynamicSpriteDatas.size - 1; i >= 0; --i)
			{
				if (m_DynamicSpriteDatas[i].RuntimeFlag && m_DynamicSpriteDatas[i].UsageReference <= 0)
				{
					m_DynamicSpriteDatas[i].UnloadTexture();
					m_DynamicSpriteDatas.RemoveAt(i);
				}
			}

			// repack
			if (!m_SpritePacker.PackSprites(m_DynamicSpriteDatas))
			{
				EB.Debug.LogError("PackSprites: CAN NOT pack all of the sprites in the atlas!");
			}
			else
			{
				m_Packed = true;
			}
		}
		else
		{
			m_Packed = true;
		}
	}

	public override bool SynchronizeTexture(Texture2D texture)
	{
		int size = m_DynamicSpriteDatas.size;
		for(int i = 0; i < size; i++)
		{
			if(m_DynamicSpriteDatas[i] != null && string.Compare(m_DynamicSpriteDatas[i].name, texture.name) == 0)
			{
				//m_DynamicSpriteDatas[i].TextureReference = texture;
				//m_DynamicSpriteDatas[i].SpriteResourceType = DynamicSpriteData.eSpriteResTypes.Asset_Bundle;
				//PackSprites();

				if (m_DynamicSpriteDatas[i].TextureReference != texture)
				{
					m_DynamicSpriteDatas[i].UnloadTexture();
					m_DynamicSpriteDatas[i].TextureReference = texture;
					m_DynamicSpriteDatas[i].SpriteResourceType = DynamicSpriteData.eSpriteResTypes.Asset_Bundle;
					m_Packed = false;
					PackSprites();
				}

				return true;
			}
		}

		return false;
	}
}
