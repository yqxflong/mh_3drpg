using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class ParticleManager
{
	public enum PARAMETER
	{
		None,
		EmissionRate,
		GravityMultiplier,
		StartingColor,
		StartingLifeSpan,
		StartingRotation,
		StartingSize,
		StartingSpeed
	}

	public enum TRIGGER
	{
		Constant,
		Height,
		Velocity
	}

	public enum TUNING
	{
		Constant,
		Linear,
		Curve,
	}

	public static int PARAMETER_COUNT	= System.Enum.GetValues(typeof(ParticleManager.PARAMETER)).Length;
	public static int TRIGGER_COUNT		= System.Enum.GetValues(typeof(ParticleManager.TRIGGER)).Length;
	public static int TUNING_COUNT		= System.Enum.GetValues(typeof(ParticleManager.TUNING)).Length;
}

[ExecuteInEditMode]
public class ParticlePal : MonoBehaviour
{
	[System.Serializable]
	public class Condition
	{
		public bool expanded; // for the editor
		public ParticleManager.PARAMETER parameter;
		public ParticleManager.TRIGGER trigger;
		
		[System.Serializable]
		public class Tuning
		{
			public ParticleManager.TUNING type;
			public float constant;
			public Color constantColor;
			public AnimationCurve curve;
			public float minX;
			public float minY;
			public float maxX;
			public float maxY;
			public Color minColor;
			public Color maxColor;
			
			public Tuning()
			{
				type = ParticleManager.TUNING.Linear;
				constant = 0.0f;
				constantColor = Color.white;
				curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
				minX = 0.0f;
				minY = 0.0f;
				maxX = 10.0f;
				maxY = 10.0f;
				minColor = Color.white;
				maxColor = Color.white;
			}
		}
		
		public Tuning[] tunings;
		
		public Condition()
		{
			expanded = true;
			parameter = ParticleManager.PARAMETER.None;
			trigger = ParticleManager.TRIGGER.Constant;
			tunings = new ParticlePal.Condition.Tuning[PerformanceInfo.ePARTICLE_QUALITY_COUNT];
			for (var i = 0; i < PerformanceInfo.ePARTICLE_QUALITY_COUNT; ++i)
			{
				tunings[i] = new ParticlePal.Condition.Tuning();
			}
		}
	}

	[SerializeField]
	private ParticleSystem _particleSystem;
	public List<Condition> conditions = new List<Condition>(); 
	public float VelocityDamping = 0.5f;
	public List<bool> isEnabled = new List<bool>();

	void Awake()
	{
		if (isEnabled == null || isEnabled.Count != PerformanceInfo.ePARTICLE_QUALITY_COUNT)
		{
			isEnabled = new List<bool>() { true, true, true };
		}
		
		if (Application.isPlaying)
		{
			quality = PerformanceManager.Instance.CurrentEnvironmentInfo.particleQuality;
			DestroyIfPossible(gameObject);
		}
	}

	void Start () 
	{
		#if UNITY_EDITOR
		lastTime = EditorApplication.timeSinceStartup;
		#endif
		
		_particleSystem = GetComponent<ParticleSystem>();

		if (conditions == null)
		{
			conditions = new List<Condition>();
		}
	}
	
	private Vector3 lastPosition = Vector3.zero;
	private float velocity = 0.0f;

#if UNITY_EDITOR
	private double lastTime;
#endif
	
	[System.NonSerialized]
	private PerformanceInfo.ePARTICLE_QUALITY quality = PerformanceInfo.ePARTICLE_QUALITY.High;

	private void UpdateVelocity()
	{
		Vector3 positionDiff = this.transform.position - lastPosition;
		#if UNITY_EDITOR
			float deltaTime = (float)(EditorApplication.timeSinceStartup - lastTime);
			lastTime = EditorApplication.timeSinceStartup;
		#else
			float deltaTime = Time.deltaTime;
		#endif
		velocity = (VelocityDamping * velocity) + ((1 - VelocityDamping) * positionDiff.magnitude / deltaTime);
		lastPosition = this.transform.position;
	}
	
	void Update () 
	{
		if (_particleSystem == null) return;

		#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			quality = ParticlePalPreview.Quality;
			this.GetComponent<Renderer>().enabled = isEnabled[(int)quality];
		}
		#endif
		
		UpdateVelocity();

		bool allConstant = true;
		
		foreach (Condition condition in conditions)
		{
			Condition.Tuning tuning = condition.tunings[(int)quality];
			
			if (condition.trigger == ParticleManager.TRIGGER.Constant || tuning.type == ParticleManager.TUNING.Constant)
			{
				//constant value for all qualities or this particular quality, early out
				if (condition.parameter == ParticleManager.PARAMETER.StartingColor)
				{
					SetParameter(condition.parameter, tuning.constantColor);
				}
				else
				{
					SetParameter(condition.parameter, tuning.constant);
				}
				continue;
			}

			allConstant = false;
			
			//find the trigger value to use in the tuning phase
			var triggerVal = 0.0f;
			
			switch (condition.trigger)
			{
			case(ParticleManager.TRIGGER.Height):
				triggerVal = this.transform.position.y;
				break;
			case(ParticleManager.TRIGGER.Velocity):
				triggerVal = velocity;
				break;
			default:
				#if UNITY_EDITOR
				EB.Debug.LogError("ParticlePal trigger {0} is not recognized",condition.trigger.ToString());
				#endif
				break;
			}
			
			//find the interpolated key based of tuning type
			float interpolateKey = 0.0f;
	
			switch(tuning.type)
			{
			case(ParticleManager.TUNING.Linear):
				interpolateKey = Mathf.Clamp01((triggerVal - tuning.minX) / (tuning.maxX - tuning.minX));
				break;
			case(ParticleManager.TUNING.Curve):
				AnimationCurve curve = tuning.curve;
				interpolateKey = curve.Evaluate((triggerVal - tuning.minX) / (tuning.maxX - tuning.minX));
				break;
			default:
				#if UNITY_EDITOR
				EB.Debug.LogError("ParticlePal tuning {0} is not recognized", condition.tunings[(int)quality].ToString());
				#endif
				break;
			}
			
			//interpolate the float or color
			
			switch(condition.parameter)
			{
			case(ParticleManager.PARAMETER.StartingColor):
				Color interpolatedColor = Color.Lerp(tuning.minColor, tuning.maxColor, interpolateKey);
				SetParameter(condition.parameter, interpolatedColor);
				break;
				
			default:
				float iterpolatedFloat = interpolateKey * (tuning.maxY - tuning.minY) + tuning.minY;
				SetParameter(condition.parameter, iterpolatedFloat);
				break;
			}
		}
		
		if (allConstant && Application.isPlaying)
		{
			//in game, disable the particle pal script, as we have completed an update and set all the constants
			this.enabled = false;
		}
	}
	
	private void SetParameter(ParticleManager.PARAMETER parameter, float val)
	{
		#if UNITY_EDITOR
		if (this.GetComponent<ParticleSystem>() == null)
		{
			EB.Debug.LogError("Partical Pal is attached to something without a particle system");
			return;
		}
		#endif
	
		switch(parameter)
		{
		case(ParticleManager.PARAMETER.EmissionRate):
			var em = this.GetComponent<ParticleSystem>().emission;
			var rate = em.rate;
			rate.constantMin = rate.constantMax = val;
			em.rate = rate;
			break;
		case (ParticleManager.PARAMETER.StartingLifeSpan):
			this.GetComponent<ParticleSystem>().startLifetime = val;
			break;
		case(ParticleManager.PARAMETER.StartingSize):
			this.GetComponent<ParticleSystem>().startSize = val;
			break;
		case(ParticleManager.PARAMETER.StartingSpeed):
			this.GetComponent<ParticleSystem>().startSpeed = val;
			break;
		case(ParticleManager.PARAMETER.StartingRotation):
			this.GetComponent<ParticleSystem>().startRotation = val;
			break;
		case(ParticleManager.PARAMETER.GravityMultiplier):
			this.GetComponent<ParticleSystem>().gravityModifier = val;
			break;
		case(ParticleManager.PARAMETER.None):
			break;
		default:
			#if UNITY_EDITOR
			EB.Debug.LogError("ParticlePal float parameter {0} is not recognized", parameter.ToString());
			#endif
			break;
		}
	}
				
	private void SetParameter(ParticleManager.PARAMETER parameter, Color val)
	{
		switch(parameter)
		{
		case(ParticleManager.PARAMETER.StartingColor):
			this.GetComponent<ParticleSystem>().startColor = val;
			break;
		default:
			#if UNITY_EDITOR
			EB.Debug.LogError("ParticlePal color parameter {0} is not recognized", parameter.ToString());
			#endif
			break;
		}	
	}

	public static bool DisabledByParticlePal(GameObject gameObject)
	{
		ParticlePal particlePal = gameObject.GetComponent<ParticlePal>();
		
		if (particlePal == null) 
		{
			return false;
		}
		
		if (particlePal.isEnabled == null || particlePal.isEnabled.Count != PerformanceInfo.ePARTICLE_QUALITY_COUNT)
		{
			particlePal.isEnabled = new List<bool>() { true, true, true };
		}

		//Need to fix this performanceinfo data up mmcmanus
        if(PerformanceManager.Instance.CurrentEnvironmentInfo == null) //by pj 编辑场景没有会为空
        {
            return false;
        }

		return !particlePal.isEnabled[(int)PerformanceManager.Instance.CurrentEnvironmentInfo.particleQuality];
		//return !particlePal.isEnabled[(int)PerformanceInfo.ePARTICLE_QUALITY.High];
	}

	public static bool WillDelete(GameObject gameObject)
	{
		if(!DisabledByParticlePal(gameObject))
		{
			return false;
		}

		bool canDelete = true;

		for(int i = 0; i < gameObject.transform.childCount; ++i)
		{
			canDelete &= WillDelete(gameObject.transform.GetChild(i).gameObject);
		}
		
		return canDelete;
	}
	
	private static void DestroyIfPossible(GameObject gameObject)
	{

		ParticlePal particlePal = gameObject.GetComponent<ParticlePal>();
		
		if (particlePal == null)
		{
			return;
		}

		if (WillDelete(gameObject))
		{
			Destroy(gameObject);
		}
		else
		{
			//couldn't delete all the children; can't delete me
			//Need to fix this performanceinfo data up mmcmanus
			//if (!particlePal.isEnabled[(int)PerformanceManager.ParticleQuality])
			if (!particlePal.isEnabled[(int)PerformanceInfo.ePARTICLE_QUALITY.High])
			{
				//disable the particle system, then disable particlePal
				gameObject.GetComponent<ParticleSystem>().Pause();
				var ps = gameObject.GetComponent<ParticleSystem>();
				var em = ps.emission;
				em.enabled = false;
				particlePal.enabled = false;
			}
		}
	}			
	
#if UNITY_EDITOR
	public void AddCondition() 
	{
		conditions.Add(new Condition());
	}
	
	public void RemoveCondition(Condition condition) 
	{
		conditions.Remove(condition);
	}
#endif
}
