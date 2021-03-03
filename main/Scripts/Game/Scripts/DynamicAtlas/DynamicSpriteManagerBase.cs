using UnityEngine;
using System.Collections;

public class DynamicSpriteManagerBase
{
	protected BetterList<DynamicSpriteData> m_DynamicSpriteDatas;

	public BetterList<DynamicSpriteData> GetDynamicSpriteDatas()
	{
		return m_DynamicSpriteDatas;
	}

	public DynamicSpriteManagerBase()
	{
		m_DynamicSpriteDatas = new BetterList<DynamicSpriteData>();
	}

	public DynamicSpriteData GetSpriteData(string spriteName)
	{
		int size = m_DynamicSpriteDatas.size;
		for(int i = 0; i < size; i++)
		{
            if (m_DynamicSpriteDatas[i] != null && spriteName.Equals(m_DynamicSpriteDatas[i].name)/*string.Compare(m_DynamicSpriteDatas[i].name, spriteName) == 0*/)
			{
				return m_DynamicSpriteDatas[i];
			}
		}
		return null;
	}

	public DynamicSpriteData GetFreeSpriteData()
	{
		int size = m_DynamicSpriteDatas.size;
		for(int i = 0; i < size; i++)
		{
			if(m_DynamicSpriteDatas[i] != null && m_DynamicSpriteDatas[i].UsageReference <= 0 && m_DynamicSpriteDatas[i].RuntimeFlag)
			{
				return m_DynamicSpriteDatas[i];
			}
		}
		return null;
	}

	public void Release()
	{
		m_DynamicSpriteDatas.Release();
	}

	public DynamicSpriteData TryAddSpriteReference(string spriteName)
	{
		DynamicSpriteData sprite_data = GetSpriteData(spriteName);
		if(sprite_data != null)
		{
			sprite_data.AddReference();
			sprite_data.RuntimeFlag = true;
			return sprite_data;
		}
		return null;
	}

	public void RemoveSpriteReference(string spriteName)
	{
		DynamicSpriteData sprite_data = GetSpriteData(spriteName);
		if(sprite_data != null)
		{
			sprite_data.RemoveReference();
			sprite_data.RuntimeFlag = true;
		}
	}

	public virtual void AddDynamicSpriteData(string spriteName, Texture2D textureReference, DynamicSpriteData.eSpriteResTypes type, bool is_runtime = true)
	{
		
	}

	public virtual bool SynchronizeTexture(Texture2D texture)
	{
		return false;
	}

	public virtual  void PackSprites()
	{

	}
}
