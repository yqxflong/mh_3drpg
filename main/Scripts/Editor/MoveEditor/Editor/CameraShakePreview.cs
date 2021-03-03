using UnityEngine;
using System.Collections.Generic;
using Thinksquirrel.CShake;

namespace MoveEditor
{
    public class CameraShakePreviewTrack
    {
        public CameraShakePreviewTrack(CameraShakeEventInfo evt, float startTime)
        {
            _event		= evt;
            _startTime 	= startTime;
            _played		= false;
        }

        public void Update(float time)
        {
            if (Application.isPlaying)
            {
                if (!_played)
                {
                    if (time >= _startTime)
                    {
                        MoveCameraShakeHelper.Shake(_event);
                        _played = true;
                    }
                }
            }
            else
            {
                if (!_played)
                {
                    if (time >= _startTime)
                    {
                        MoveCameraShakeHelper.Shake(_event);
                        _played = true;
                    }
                }
            }
        }

        public void Reset()
        {
            _played = false;
        }

        private CameraShakeEventInfo	_event 		= null;
        public float					_startTime	= 0;
        private bool					_played		= false;
    }

    public class CameraShakePreviewTrackManager
    {
        public static void Init()
        {
            CameraShake instance = Camera.main.gameObject.GetComponent<CameraShake>();

            if (instance == null)
            {
                instance = Camera.main.gameObject.AddComponent<CameraShake>();
            }
        }

        public static void RegisterTrack(CameraShakeEventInfo evt, float startTime)
        {
            CameraShakePreviewTrack preview = new CameraShakePreviewTrack(evt, startTime);
            _activeTracks.Add(preview);
        }
        
        public static void UpdateTracks(float time)
        {
            for (int i = 0; i < _activeTracks.Count; i++)
            {
                _activeTracks[i].Update(time);
            }
        }

        public static void ResetAll()
        {
            for (int i = 0; i < _activeTracks.Count; i++)
            {
                _activeTracks[i].Reset();
            }
        }
        
        public static void DeregisterAll()
        {
            CameraShake.CancelAllShakes();
            _activeTracks.Clear();
        }

        public static float GetTracksMaxLengh()
        {
            float maxLength = 0;
            for (int i = 0; i < _activeTracks.Count; i++)
            {
                if (_activeTracks[i]._startTime> maxLength)
                {
                    maxLength = _activeTracks[i]._startTime;
                }
            }

            return maxLength;
        }

        private static List<CameraShakePreviewTrack> _activeTracks = new List<CameraShakePreviewTrack>();
    }
}