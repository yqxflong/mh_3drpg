using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(PostFXInit))]
public class PostFXInitInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		PostFXInit postInit = (PostFXInit)target;

		if (!Application.isPlaying && GUILayout.Button("Active"))
		{
			Camera camera = null;
			if (SceneView.lastActiveSceneView != null)
			{
				camera = SceneView.lastActiveSceneView.camera;
			}

			if (camera != null)
			{
				PostFXManagerTrigger trigger = camera.GetComponent<PostFXManagerTrigger>();
				if (trigger == null)
				{
					trigger = camera.gameObject.AddComponent<PostFXManagerTrigger>();
				}

				RenderSettings.forceUpdate = true;

				List<PerformanceInfo.ePOSTFX> fxtypes = new List<PerformanceInfo.ePOSTFX>(3);
				if ((postInit.fxs & PerformanceInfo.ePOSTFX.Bloom) == PerformanceInfo.ePOSTFX.Bloom)
				{
					fxtypes.Add(PerformanceInfo.ePOSTFX.Bloom);
				}
				if ((postInit.fxs & PerformanceInfo.ePOSTFX.Vignette) == PerformanceInfo.ePOSTFX.Vignette)
				{
					fxtypes.Add(PerformanceInfo.ePOSTFX.Vignette);
				}
				if ((postInit.fxs & PerformanceInfo.ePOSTFX.Vignette2) == PerformanceInfo.ePOSTFX.Vignette2)
				{
					fxtypes.Add(PerformanceInfo.ePOSTFX.Vignette2);
				}
				if ((postInit.fxs & PerformanceInfo.ePOSTFX.ColorGrade) == PerformanceInfo.ePOSTFX.ColorGrade)
				{
					fxtypes.Add(PerformanceInfo.ePOSTFX.ColorGrade);
				}
				if ((postInit.fxs & PerformanceInfo.ePOSTFX.ToneMap) == PerformanceInfo.ePOSTFX.ToneMap)
				{
					fxtypes.Add(PerformanceInfo.ePOSTFX.ToneMap);
				}
				if ((postInit.fxs & PerformanceInfo.ePOSTFX.Warp) == PerformanceInfo.ePOSTFX.Warp)
				{
					fxtypes.Add(PerformanceInfo.ePOSTFX.Warp);
				}
				if ((postInit.fxs & PerformanceInfo.ePOSTFX.RadialBlur) == PerformanceInfo.ePOSTFX.RadialBlur)
				{
					fxtypes.Add(PerformanceInfo.ePOSTFX.RadialBlur);
				}
				PostFXManager.Instance.Init(camera, PerformanceInfo.ePOSTFX_QUALITY.High, fxtypes.ToArray());
			}
		}
	}
}
