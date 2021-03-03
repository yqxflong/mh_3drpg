using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class HueShift : MonoBehaviour 
{
	private Material _Material;

	public enum HueShiftType
	{
		ParticleSystem,
		TrailRenderer,
		Renderer,
		CustomMaterial
	};

	public HueShiftType _Type = HueShiftType.CustomMaterial;

	void Awake()
	{
		if(this.gameObject.GetComponent<ParticleSystem>() != null)
		{
			_Type = HueShiftType.ParticleSystem;
		}
		else if(gameObject.GetComponent<TrailRendererInstance>() != null)
		{
			_Type = HueShiftType.TrailRenderer;
		}
		else if(gameObject.GetComponent<Renderer>() != null)
		{
			_Type = HueShiftType.Renderer;
		}
		DoHueShift();
	}

#if UNITY_EDITOR
	void Update() 
	{
		DoHueShift();
	}
#endif

	public void SetMaterial(Material m)
	{
		_Material = m;
	}

	public void DoHueShift()
	{
		switch(_Type)
		{
			case(HueShiftType.ParticleSystem):
				_Material = this.GetComponent<ParticleSystem>().GetComponent<Renderer>().sharedMaterial;
			break;
			case(HueShiftType.TrailRenderer):
				_Material = this.gameObject.GetComponent<TrailRendererInstance>()._Material;
			break;
			case(HueShiftType.Renderer):
				_Material = this.gameObject.GetComponent<Renderer>().sharedMaterial;
			break;
			case(HueShiftType.CustomMaterial):
			{
				if (_Material==null)
				{
					EB.Debug.LogWarning("Assign custom material for hue shift to work!");
					return;
				}
			}
			break;
		}

		if (_Material==null)
		{
			if (gameObject.transform.parent!=null)
			{
				EB.Debug.LogWarning("NO HUE SHIFT MATERIAL ASSIGNED {0} parent:{1}", gameObject.name,gameObject.transform.parent.name);
			}
			else
			{
				EB.Debug.LogWarning("NO HUE SHIFT MATERIAL ASSIGNED {0}",gameObject.name);
			}
			return;
		}
		if(_Material.HasProperty("_HueShift")&&_Material.HasProperty("_Sat")&&_Material.HasProperty("_Value"))
		{
			float hueShift = _Material.GetFloat("_HueShift");
			float sat = _Material.GetFloat("_Sat");
			float value = _Material.GetFloat("_Value");
			
			float V = value;
			float U = sat * value * Mathf.Cos(Mathf.Deg2Rad * hueShift);
			float W = sat * value * Mathf.Sin(Mathf.Deg2Rad * hueShift);
			
			Vector3 _ShiftR = new Vector4((+.299f * V + .701f * U + .168f * W), (+.587f * V - .587f * U + .330f * W), (+.114f * V - .114f * U - .497f * W), 0.0f);
			Vector3 _ShiftG = new Vector4((+.299f * V - .299f * U - .328f * W), (+.587f * V + .413f * U + .035f * W), (+.114f * V - .114f * U + .292f * W), 0.0f);
			Vector3 _ShiftB = new Vector4((+.299f * V - .300f * U + 1.25f * W), (+.587f * V - .588f * U - 1.05f * W), (+.114f * V + .886f * U - .203f * W), 0.0f);
			
			_Material.SetVector("_HueShiftR", _ShiftR);
			_Material.SetVector("_HueShiftG", _ShiftG);
			_Material.SetVector("_HueShiftB", _ShiftB);
		}
	
	}
}
