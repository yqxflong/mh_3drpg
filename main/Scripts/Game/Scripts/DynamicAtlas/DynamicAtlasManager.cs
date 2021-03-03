using UnityEngine;
using System.Collections;

//Temp template types.
public enum eDynamicAtlasType
{
	Icons,
	FX
}

[ExecuteInEditMode]
public class DynamicAtlasManager : MonoBehaviour 
{	
	public DynamicAtlas[] m_DynamicAtlases;

	private static DynamicAtlasManager	m_instance;

	public static DynamicAtlasManager GetInstance()
	{
		return m_instance;
	}

	public static bool IsAvailable(eDynamicAtlasType type)
	{
		if(m_instance == null)
		{
			return false;
		}

		return m_instance.IsDynamicAtlasAvailable(type);
	}

	protected bool IsDynamicAtlasAvailable(eDynamicAtlasType type)
	{
		DynamicAtlas atlas = GetDynamicAtlas(type);
		if(atlas != null)
		{
			return atlas.IsInitialized();
		}

		return false;
	}

	void Awake()
	{
		m_instance = this;

		if (Application.isPlaying)
		{
			DontDestroyOnLoad(gameObject);
		}
	}

	// Use this for initialization
	void Start ()
	{
		
	}

	void OnDestroy()
	{
		m_instance = null;
	}

	public DynamicAtlas GetDynamicAtlas(eDynamicAtlasType type)
	{
		int index = (int)type;
		if(index < 0 || index >= m_DynamicAtlases.Length)
		{
			return null;
		}

		return m_DynamicAtlases[index];
	}

	/// <summary>
	/// Loads the dynamic sprite.
	/// </summary>
	/// <param name="spriteName">Sprite name.</param>
	/// <param name="type">dynamic atlas type.</param>
	/// <param name="is_runtime">Set this to true if loading dynamic sprite at runtime(like uispirt & FX), set to false if trying to pre load dynamic sprite during loading process.</param>
	public void LoadDynamicSprite(string spriteName, eDynamicAtlasType type, bool is_runtime = true)
	{
		DynamicAtlas atlas = GetDynamicAtlas(type);

		if(atlas != null)
		{
			atlas.LoadDynamicSprite(spriteName, is_runtime);
		}
		else
		{
			EB.Debug.LogError("DynamicAtlasManager[LoadDynamicSprite]: Dynamic atlas does not exist!");
		}
	}

	public void UnloadDynamicSprite(string spriteName, eDynamicAtlasType type)
	{
		DynamicAtlas atlas = GetDynamicAtlas(type);

		if(atlas != null)
		{
			atlas.RemoveSpriteReference(spriteName);
		}
		else
		{
			EB.Debug.LogError("DynamicAtlasManager[UnloadDynamicSprite]: Dynamic atlas does not exist!");
		}
	}

	public void AddSpriteUVUpdateCallback(eDynamicAtlasType type, EventDelegate.Callback onSpriteUVUpdate)
	{
		DynamicAtlas atlas = GetDynamicAtlas(type);

		if(atlas != null)
		{
			EventDelegate.Add(atlas.onSpriteUVsUpdate, onSpriteUVUpdate);
		}
		else
		{
			EB.Debug.LogError("DynamicAtlasManager[AddSpriteUVUpdateCallback]: Dynamic atlas does not exist!");
		}
	}

	public void RemoveSpriteUVUpdateCallback(eDynamicAtlasType type, EventDelegate.Callback onSpriteUVUpdate)
	{
		DynamicAtlas atlas = GetDynamicAtlas(type);

		if(atlas != null)
		{
			EventDelegate.Remove(atlas.onSpriteUVsUpdate, onSpriteUVUpdate);
		}
		else
		{
			EB.Debug.LogError("DynamicAtlasManager[RemoveSpriteUVUpdateCallback]: Dynamic atlas does not exist!");
		}
	}

	public void AddTextureAsyncCallback(eDynamicAtlasType type, EventDelegate.Callback onTextureAsyncSucceeded, EventDelegate.Callback onTextureAsyncFailed)
	{
		DynamicAtlas atlas = GetDynamicAtlas(type);

		if(atlas != null)
		{
			EventDelegate.Add(atlas.onTextureAsyncSucceeded, onTextureAsyncSucceeded);
			EventDelegate.Add(atlas.onTextureAsyncFailed, onTextureAsyncFailed);
		}
		else
		{
			EB.Debug.LogError("DynamicAtlasManager[AddTextureAsyncCallback]: Dynamic atlas does not exist!");
		}
	}

	public void RemoveTextureAsyncCallback(eDynamicAtlasType type, EventDelegate.Callback onTextureAsyncSucceeded, EventDelegate.Callback onTextureAsyncFailed)
	{
		DynamicAtlas atlas = GetDynamicAtlas(type);

		if(atlas != null)
		{
			EventDelegate.Remove(atlas.onTextureAsyncSucceeded, onTextureAsyncSucceeded);
			EventDelegate.Remove(atlas.onTextureAsyncFailed, onTextureAsyncFailed);
		}
		else
		{
			EB.Debug.LogError("DynamicAtlasManager[RemoveTextureAsyncCallback]: Dynamic atlas does not exist!");
		}
	}

	public UISpriteData GetAtlasSprite(string spriteName, eDynamicAtlasType type)
	{
		DynamicAtlas atlas = GetDynamicAtlas(type);

		if(atlas != null)
		{
			return atlas.GetSpriteData(spriteName);
		}
		else
		{
			EB.Debug.LogError("DynamicAtlasManager[GetAtlasSprite]: Dynamic atlas does not exist!");
		}

		return null;
	}

	public void PreLoadSpriteFromFxPrefab(GameObject fxPrefab)
	{
		FXBillboard[] fx_billboards = fxPrefab.GetComponentsInChildren<FXBillboard>(true);

		int count = fx_billboards == null ? 0 : fx_billboards.Length;
		for(int i = 0; i < count; i++)
		{
			if(fx_billboards[i] != null && fx_billboards[i].AtlasType == FXAtlasType.Dynamic)
			{
				LoadDynamicSprite(fx_billboards[i].spriteName, fx_billboards[i].dynamicAtlasType, false);
			}
		}

		FXBillboardParticle[] fx_particles = fxPrefab.GetComponentsInChildren<FXBillboardParticle>(true);

		count = fx_particles == null ? 0 : fx_particles.Length;
		for(int i = 0; i < count; i++)
		{
			if(fx_particles[i] != null && fx_particles[i].AtlasType == FXAtlasType.Dynamic)
			{
				LoadDynamicSprite(fx_particles[i].spriteName, fx_particles[i].dynamicAtlasType, false);
			}
		}
	}

	public void DimissSpritesOfFxPrefab(GameObject fxPrefab)
	{
		FXBillboard[] fx_billboards = fxPrefab.GetComponentsInChildren<FXBillboard>(true);
		
		int count = fx_billboards == null ? 0 : fx_billboards.Length;
		for(int i = 0; i < count; i++)
		{
			if(fx_billboards[i] != null && fx_billboards[i].AtlasType == FXAtlasType.Dynamic)
			{
				UnloadDynamicSprite(fx_billboards[i].spriteName, fx_billboards[i].dynamicAtlasType);
			}
		}
		
		FXBillboardParticle[] fx_particles = fxPrefab.GetComponentsInChildren<FXBillboardParticle>(true);
		
		count = fx_particles == null ? 0 : fx_particles.Length;
		for(int i = 0; i < count; i++)
		{
			if(fx_particles[i] != null && fx_particles[i].AtlasType == FXAtlasType.Dynamic)
			{
				UnloadDynamicSprite(fx_particles[i].spriteName, fx_particles[i].dynamicAtlasType);
			}
		}
	}
}
