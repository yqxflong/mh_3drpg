using UnityEngine;

public class RenderGlobals 
{	
	public enum eADJUSTMENT_MODE
	{
		NONE,
		NO_REFLECTIONS,
		NO_REFLECTIONS_OR_SPECULAR
	}

	public static eADJUSTMENT_MODE EnvironmentAdjustmentMode { get; private set; }
	public static Color AdjustmentScale { get; private set; }
	public static Color AdjustmentOffset { get; private set; }
	
	public static float PostFXBloomMix { get; private set; }
	public static float PostFXBloomBlur { get; private set; }
	public static float PostFXBloomRamp { get; private set; }
	public static float PostFXBloomIntensity { get; private set; }
	public static Color PostFXBloomColor { get; private set; }

	public static float PostFXVignetteMix { get; private set; }
	public static float PostFXVignetteIntensity { get; private set; }
	public static Color PostFXVignetteColor { get; private set; }

	public static float PostFXWarpMix { get; private set; }
	public static Vector2 PostFXWarpIntensity { get; private set; }

	public static float PostFXRadialBlurMix { get; private set; }
	public static float PostFXRadialBlurSampleDist { get; private set; }

	static RenderGlobals()
	{
		EnvironmentAdjustmentMode = eADJUSTMENT_MODE.NONE;
		AdjustmentScale = Color.gray;
		AdjustmentOffset = Color.black;

		PostFXBloomMix = 0.0f;
		PostFXBloomBlur = 2.0f;
		PostFXBloomRamp = 2.0f;
		PostFXBloomIntensity = 1.0f;
		PostFXBloomColor = Color.black;
		
		PostFXVignetteMix = 0.0f;
		PostFXVignetteIntensity = 0.0f;
		PostFXVignetteColor = Color.white;

		PostFXWarpMix = 0.0f;
		PostFXWarpIntensity = Vector2.zero;

		PostFXRadialBlurMix = 0.0f;
		PostFXRadialBlurSampleDist = 0.0f;
	}
	
	public static void SetEnvironmentAdjustment(int LOD)
	{
		if (LOD >= 400)
		{
			EnvironmentAdjustmentMode = eADJUSTMENT_MODE.NONE;
		}
		else if (LOD >= 300)
		{
			EnvironmentAdjustmentMode = eADJUSTMENT_MODE.NO_REFLECTIONS;
		}
		else
		{
			EnvironmentAdjustmentMode = eADJUSTMENT_MODE.NO_REFLECTIONS_OR_SPECULAR;
		}
	}
	
	public static void SetEnvironmentAdjustments(Color multiply, Color add)
	{
		AdjustmentScale = multiply;
		AdjustmentOffset = add;
	}
	
	public static void SetBloom(float mix, float blur, float ramp, float intensity, Color color)
	{
		PostFXBloomMix = Mathf.Clamp01(mix);
		PostFXBloomBlur = Mathf.Clamp(mix, 0.0f, 2.0f);
		PostFXBloomRamp = ramp;
		PostFXBloomIntensity = intensity;
		PostFXBloomColor = color;
	}
	
	public static void SetVignette(float mix, float intensity, Color color)
	{
		PostFXVignetteMix = Mathf.Clamp01(mix);
		PostFXVignetteIntensity = intensity;
		PostFXVignetteColor = color;
	}
	
	public static void SetWarp(float mix, Vector2 intensityXY)
	{
		PostFXWarpMix = Mathf.Clamp01(mix);
		PostFXWarpIntensity = intensityXY;
	}

	public static void SetRadialBlur(float mix, float dist)
	{
		PostFXRadialBlurMix = mix;
		PostFXRadialBlurSampleDist = dist;
	}
}
