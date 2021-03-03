using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class HeroPulse : MonoBehaviour 
{
	public List<Material> materials;
	
	public AnimationCurve fresnelCurve;
	public float fresnelLoopTime = 3.0f;
	
	public Gradient emissiveGradient;
	public float emissiveLoopTime = 3.0f;

	private float lifetime;

	void Awake()
	{
		lifetime = 0.0f;
	}

	void Update () 
	{
		if (!Application.isPlaying)
		{
			foreach(Material material in materials)
			{
				material.SetFloat("_FresnelIntensity", 0.0f);
				material.SetColor("_EmissiveColor", Color.black);
			}
			return;
		}

		lifetime += Time.deltaTime;

		if (materials == null)
			return;
		
		float fresnelVal = fresnelCurve != null ? fresnelCurve.Evaluate(lifetime / fresnelLoopTime) : 0.0f;
		Color emissiveColor = emissiveGradient != null ? emissiveGradient.Evaluate((lifetime % emissiveLoopTime) / emissiveLoopTime) : Color.black;

		foreach(Material material in materials)
		{
			material.SetFloat("_FresnelIntensity", fresnelVal);
			material.SetColor("_EmissiveColor", emissiveColor);
		}
	}
}
