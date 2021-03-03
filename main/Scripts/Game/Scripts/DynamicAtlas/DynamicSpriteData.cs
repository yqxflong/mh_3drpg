using UnityEngine;
using System.Collections;
using GM;

public class DynamicSpriteData : UISpriteData
{
	public enum eSpriteResTypes
	{
		Local_Resource, 
		Asset_Bundle,
	}

	public object UserData = null;
	
	private int m_UsageReference;
	private eSpriteResTypes m_SpriteResType;
	private Texture2D	m_TextureReference;
	private bool		m_RuntimeFlag;
	
	public Texture2D TextureReference
	{
		get
		{
			return m_TextureReference;
		}
		set
		{
			m_TextureReference = value;
			if(m_TextureReference == null)
			{
				width = 0;
				height = 0;
			}
			else
			{
				width = m_TextureReference.width;
				height = m_TextureReference.height;	
			}
		}
	}
	
	public eSpriteResTypes SpriteResourceType
	{
		get
		{
			return m_SpriteResType;
		}
		set
		{
			m_SpriteResType = value;
		}
	}
	
	public int UsageReference
	{
		get
		{
			return m_UsageReference;
		}
		set
		{
			m_UsageReference = value;
		}
	}

	public bool RuntimeFlag
	{
		get
		{
			return m_RuntimeFlag;
		}
		set
		{
			m_RuntimeFlag = value;
		}
	}
	
	public DynamicSpriteData()
	{
		m_UsageReference = -1;
		m_SpriteResType = eSpriteResTypes.Local_Resource;
		m_TextureReference = null;
		//name = DEFAULT_SPRITE_NAME;
		m_RuntimeFlag = true;
	}
	
	public bool IsInUsage()
	{
		return m_UsageReference != -1;
	}
	
	public void AddReference()
	{
		//Debug.LogWarning(string.Format("Add texture reference! {0}", TextureReference != null ? TextureReference.name : "null"));
		if(m_UsageReference < 0) 
		{
			m_UsageReference = 0;
		}
		m_UsageReference++;
        //EB.Debug.LogError("++++++++++++++++++++++ m_TextureReference : {0}, ref : {1}", m_TextureReference == null ? "Null" : m_TextureReference.name, m_UsageReference);
	}
	
	public void RemoveReference()
	{
		//Debug.LogWarning(string.Format("Remove texture reference! {0}", TextureReference != null ? TextureReference.name : "null"));
		m_UsageReference--;
        //EB.Debug.LogError("--------------------- m_TextureReference : {0}, ref : {1}", m_TextureReference == null ? "Null" : m_TextureReference.name, m_UsageReference);
        if (m_UsageReference < 0)
		{
			m_UsageReference = 0;
		}
	}
	
	public void UnloadTexture()
	{
		if(m_TextureReference == null)
		{
			return;
		}
		if(m_SpriteResType == eSpriteResTypes.Asset_Bundle)
		{
			TextureManager.ReleaseTexture(name);
			m_TextureReference = null;
		}
		else if(m_SpriteResType == eSpriteResTypes.Local_Resource)
		{
			Resources.UnloadAsset(m_TextureReference);
			m_TextureReference = null;
		}

		// can't find by name any more after unload
		name = string.Empty;

		// keep rectangle info
	}
}


