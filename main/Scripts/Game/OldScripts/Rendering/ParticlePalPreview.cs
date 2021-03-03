#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class ParticlePalPreview 
{
#if UNITY_EDITOR
	public static PerformanceInfo.ePARTICLE_QUALITY Quality = PerformanceInfo.ePARTICLE_QUALITY.High;

	[MenuItem("EBG/ParticlePal/Low")]
	public static void Low()
	{
		Quality = PerformanceInfo.ePARTICLE_QUALITY.Low;
	}

	[MenuItem("EBG/ParticlePal/Medium")]
	public static void Medium()
	{
		Quality = PerformanceInfo.ePARTICLE_QUALITY.Med;
	}

	[MenuItem("EBG/ParticlePal/High")]
	public static void High()
	{
		Quality = PerformanceInfo.ePARTICLE_QUALITY.High;
	}
#endif
}
