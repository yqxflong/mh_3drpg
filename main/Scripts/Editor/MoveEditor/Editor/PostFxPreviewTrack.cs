using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MoveEditor
{
	public interface IPostFxPreviewTrack
	{
		void Update(float time);
	}

	public class PostFxPreviewTrack<T> : IPostFxPreviewTrack where T : PostFxEventInfo
	{
		public PostFxPreviewTrack(T evt, float startTime)
		{
			_event		= evt;
			_startTime 	= startTime;
		}

		public void Update(float time)
		{
			float t = time - _startTime;
			
			if (t >= 0 && t <= _event.Duration)
			{
				_event.Sim(t);
			}
		}

		private T		_event 		= null;
		private float	_startTime	= 0;
	}

	public class PostFxPreviewTrackManager
	{
		public static void Init()
		{
			// troyhack
			//PostFXManager.Instance.Init(Camera.main, PerformanceInfo.ePOSTFX_QUALITY.High,new PerformanceInfo.ePOSTFX[] { PerformanceInfo.ePOSTFX.Bloom, PerformanceInfo.ePOSTFX.Vignette, PerformanceInfo.ePOSTFX.Warp });
#if UNITY_EDITOR
			if(Camera.main == null)
			{
				GameObject go = new GameObject("MainCamera");
				Camera main_camera = go.AddComponent<Camera>();
				main_camera.tag = "MainCamera";
			}
#endif
			PerformanceInfo.ePOSTFX[] postFX = (PerformanceInfo.ePOSTFX[])System.Enum.GetValues(typeof(PerformanceInfo.ePOSTFX));
			postFX = postFX.Where(p => p != PerformanceInfo.ePOSTFX.FakeVignette).ToArray();
			PostFXManager.Instance.Init(Camera.main, PerformanceInfo.ePOSTFX_QUALITY.High, postFX);

			PostFXManagerTrigger trigger = Camera.main.gameObject.GetComponent<PostFXManagerTrigger>();
			if (trigger == null)
			{
				Camera.main.gameObject.AddComponent<PostFXManagerTrigger>();
			}

			RenderSettings.forceUpdate = true;
			RenderSettings renderSettings = Object.FindObjectOfType<RenderSettings>();
			if (renderSettings == null)
			{
				GameObject go = new GameObject("RenderSettings");
				renderSettings = go.AddComponent<RenderSettings>();
			}
		}

		public static void RegisterTrack<T>(T evt, float startTime) where T : PostFxEventInfo
		{
			PostFxPreviewTrack<T> preview = new PostFxPreviewTrack<T>(evt, startTime);
			_activeTracks.Add(preview);
		}
		
		public static void UpdateTracks(float time)
		{
			for (int i = 0; i < _activeTracks.Count; i++)
			{
				_activeTracks[i].Update(time);
			}
		}
		
		public static void DeregisterAll()
		{
			RenderGlobals.SetBloom(0, 2, 2, 1, Color.white);
			RenderGlobals.SetVignette(0, 0.3f, Color.white);
			RenderGlobals.SetWarp(0, new Vector2(0.05f, 0.05f));

			if (PostFXManager.Instance != null && Camera.main != null)
			{
				PostFXManager.Instance.Init(Camera.main, PerformanceInfo.ePOSTFX_QUALITY.Off, null);
			}

			if(Camera.main != null)
			{
				PostFXManagerTrigger trigger = Camera.main.gameObject.GetComponent<PostFXManagerTrigger>();
				if (trigger != null)
				{
					Object.DestroyImmediate(trigger);
				}
			}

			RenderSettings.forceUpdate = false;

			_activeTracks.Clear();
		}

		private static List<IPostFxPreviewTrack> _activeTracks = new List<IPostFxPreviewTrack>();
	}
}