using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicPointLightManager : MonoBehaviour 
{
	static DynamicPointLightManager _this = null;
	public static DynamicPointLightManager Instance
	{ 
		get 
		{ 
			if (_this == null)
			{
				InitializeInstance();
			}

			return _this; 
		}
	} 

	public static void InitializeInstance()
	{
		_this = FindObjectOfType<DynamicPointLightManager>();

		if (_this == null)
		{
			GameObject go = new GameObject("DynamicPointLightManager");
			go.hideFlags = HideFlags.HideAndDontSave;
			_this = go.AddComponent<DynamicPointLightManager>();
			if (Application.isPlaying)
			{
				DontDestroyOnLoad(go);
			}			
		}

		_this.Init();
	}

	public static bool IsInitialized()
	{
		return _this != null;
	}

	void OnDisable()
	{
		if(_this != null)
		{
			Destroy(_this.gameObject);
		}
	}

	List<DynamicPointLightInstance> lights = new List<DynamicPointLightInstance>(4);
	
	public void Init() 
	{
		lights = new List<DynamicPointLightInstance>(4);
	}

	public void Sim() 
	{

		Matrix4x4 lighting = new Matrix4x4();
		Matrix4x4 position = new Matrix4x4();
		Vector4 multiplier = new Vector4();
		Vector4 intensity = new Vector4();

		for(int i = 0; i < lights.Count; ++i)
		{
			DynamicPointLightInstance light = lights[i];

			float t = (light.Lifetime % light.CycleTime) / light.CycleTime;


			Color col = light.Gradient.Evaluate(t);
			lighting.SetColumn(i, col);

			Vector3 pos = light.gameObject.transform.position;
			position.SetRow(i, new Vector4( pos.x, pos.y, pos.z, 0.0f));

			intensity[i] = light.Intensity.Evaluate(t) * light.IntensityMultiplier;

			float fallOff = Mathf.Max(0.01f, light.IntensityFallOffDistance);
			multiplier[i] = 25.0f / (fallOff * fallOff);
		}

		for (int i = lights.Count; i < 4; ++i)
		{	
			position.SetRow(i, Vector4.zero);
			lighting.SetColumn(i, Vector4.zero);
			multiplier[i] = 0;
			intensity[i] = 1;
		}

		Shader.SetGlobalMatrix( "_EBGPointLightColor", lighting );
		Shader.SetGlobalMatrix( "_EBGPointLightPosition", position );
		Shader.SetGlobalVector( "_EBGPointLightMultiplier", multiplier );
		Shader.SetGlobalVector( "_EBGPointLightIntensity", intensity );
	}

	void Update() 
	{
		Sim();
	}

	public void Register(DynamicPointLightInstance light)
	{
		if (lights.Contains(light))
		{
			EB.Debug.LogWarning("DynamicPointLight trying to register the same light!");
			return;
		}
		if (lights.Count < lights.Capacity)
		{
			lights.Add(light);
		}
		else
		{
			EB.Debug.LogWarning("Too many DynamicPointLights!");
		}
	}

	public void DeRegister(DynamicPointLightInstance light)
	{
		if (lights.Contains(light))
		{
			//EB.Debug.Log("light deregistered!");
			lights.Remove(light);
		}
	}

	public void DeRegisterAll()
	{
		lights.Clear();
	}
	
#if UNITY_EDITOR
	public void Clear()
	{
		if(_this != null)
		{
			GameObject.DestroyImmediate(_this.gameObject);
		}
	}
#endif
}
