using UnityEngine;
using System.Collections;

public class GenericPoolSingleton : MonoBehaviour 
{
	static GenericPoolSingleton _instance = null;
	static bool                 _created = false;

	public GenericPoolManager<TrailRendererInstance> 		trailPool;
	public GenericPoolManager<DynamicPointLightInstance> 	lightPool;

	public static GenericPoolSingleton Instance
	{
		get
		{
			if (_instance == null && !_created)
			{
				GameObject pool = new GameObject("GenericPoolSingleton");
				if (Application.isPlaying)
				{
					DontDestroyOnLoad(pool);
				}
				_instance = pool.AddComponent<GenericPoolSingleton>();
				_created = true;

				_instance.Init();
			}
			return _instance;
		}
	}

	public static bool IsInitialized()
	{
		return _instance != null;
	}

	public void Init() 
	{
		trailPool = new GenericPoolManager<TrailRendererInstance>();
		lightPool = new GenericPoolManager<DynamicPointLightInstance>();
		trailPool.Start();
		lightPool.Start();
	}
	
	public void Update() 
	{
		if (_instance != null)
		{
			trailPool.Update();
			lightPool.Update();
		}
	}

	void OnDestroy()
	{
		if (!Application.isPlaying)
		{
			_created = false;
		}
	}
}
