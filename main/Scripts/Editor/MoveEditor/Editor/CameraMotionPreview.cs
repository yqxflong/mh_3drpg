using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MoveEditor
{
	public class CameraMotionPreview
	{
		private CameraMotionEventInfo	_event;
		//private GameObject				_focusObject;
		private float					_startTime;
		private bool					_played;

		private Vector3					m_target_angle;
		private Vector3					m_target_pos;
        private GameObject              _previewGameObject;
        private GameObject              _previewHitGameObject;

		public float StartTime
		{
			get
			{
				return _startTime;
			}
		}

		public CameraMotionPreview(CameraMotionEventInfo evt, GameObject focus,GameObject hit, float startTime)
		{
			_event = evt;
			_startTime = startTime;
            _previewGameObject = focus;
            _previewHitGameObject = hit;
			_played = false;
		}

		public void Update(float time, float deltaTime)
		{
			if(_event == null)
			{
				return;
			}

			if(IsPlaying())
			{
                if (Camera.main != null)
                {
                    CombatCamera combat_camera = Camera.main.GetComponent<CombatCamera>();
                    if (combat_camera != null)
                    {
                        combat_camera.EditorUpdate(deltaTime);
                    }
                }

                //CombatCamera combat_camera = CameraMotionPreviewManager.Instance._previewCamera;
                //if (combat_camera != null)
                //{
                //    combat_camera.EditorUpdate();
                //}

                //float t = time - _startTime;
                //CameraMotionEventProperties info = _event._cameraMotionProperties;

                //float delta = Mathf.Clamp01(t / info._lerpDuration);

                //Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, m_target_angle, info._lerpCurve.Evaluate(delta));
                //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, m_target_pos, info._lerpCurve.Evaluate(delta));

                //if (t > info._lerpDuration)
                //{
                //    EB.Debug.Log("STOP CAMERA MOTION PREVIEW");
                //    Stop();
                //}
            }
        }

		public void Reset()
		{
			_played = false;
		}

		public void Play()
		{
			_played = true;

			//CameraMotionEventProperties info = _event._cameraMotionProperties;

			//if(info._motionTarget == CameraMotionTarget.OriginView)
			//{
				//m_target_pos = CameraMotionPreviewManager.Instance.OriginPos;
				//m_target_angle = CameraMotionPreviewManager.Instance.OriginAngle;
			//}
			//else
			{
				//Vector3 pos_offset = info._posOffset;

				//if(info._useLocalOffset)
				//{
					//m_target_pos = _focusObject.transform.TransformPoint(pos_offset);
				//}
				//else
				//{
					//m_target_pos = _focusObject.transform.position + pos_offset;
				//}

				//m_target_angle = info._targetAngle;
			}
			
			
			//CombatCamera combat_camera = CameraMotionPreviewManager.Instance._previewCamera;
			//if(combat_camera != null)
			//{
			//	//combat_camera.OnCameraMotion(m_target_pos, m_target_angle, info._lerpCurve, info._lerpDuration);
			//}

            
            MoveEditor.CameraMotionEventInfo info = _event;
            MoveEditor.CameraMotionEventProperties motionProps = info._cameraMotionProperties;

            //if (motionProps._motionTrigger == MoveEditor.CameraMotionTrigger.LocalPlayerOnly)
            //{
            //    if (Index.TeamIndex != CombatLogic.Instance.LocalPlayerTeamIndex)
            //    {
            //        return;
            //    }
            //}
            //else if (motionProps._motionTrigger == MoveEditor.CameraMotionTrigger.EnemyOnly)
            //{
            //    if (CombatLogic.Instance.IsPlayerOrChallengerSide(Data.TeamId))
            //    {
            //        return;
            //    }
            //    //if (!Data.IsEnemy)
            //    //{
            //    //    return;
            //    //}
            //}

            if (Camera.main != null)
            {
                CombatCamera combat_camera = Camera.main.GetComponent<CombatCamera>();
                //if (!combat_camera.m_enablePlayerOrChallengerMotion && CombatLogic.Instance.IsPlayerOrChallengerSide(Index.TeamIndex))
                //{
                //    return;
                //}
                //if (!combat_camera.m_enableOpponentMotion && CombatLogic.Instance.IsOpponentSide(Index.TeamIndex))
                //{
                //    return;
                //}
                string camera_motion_json = motionProps._motionOptions;
                List<MoveEditor.CameraMotionOption> options = GM.JSON.ToObject<List<MoveEditor.CameraMotionOption>>(camera_motion_json);
                List<GameObject> targets = new List<GameObject>();
                if (motionProps._motionTarget == MoveEditor.CameraMotionTarget.Attacker)
                {
                    targets.Add(_previewGameObject);
                }
                else if (motionProps._motionTarget == MoveEditor.CameraMotionTarget.Defenders)
                {
                    targets.Add(_previewHitGameObject);
                }
                else if (motionProps._motionTarget == MoveEditor.CameraMotionTarget.All)
                {
                    targets.Add(_previewGameObject);
                    targets.Add(_previewHitGameObject);
                }
                else if (motionProps._motionTarget == MoveEditor.CameraMotionTarget.DefendersCameraAnchor)
                {
                    //Debug.LogError("现在不支持DefendersCameraAnchor这种类型");
                    //GameObject anchor = CombatEventReceiver.Instance.defenderSide.Find(CombatEventReceiver.Instance.defenderFormation + "/f2").gameObject;
                    //if (anchor != null)
                    //{
                    //    targets.Add(anchor);
                    //}
                    //else
                    //{
                    //    EB.Debug.LogError("Combatant.OnCameraMotion Can not find DefendersCameraAnchor GameObject");
                    //    return;
                    //}
                }

                if (combat_camera != null)
                {
                    if (motionProps._blendCurrentCamera)
                    {
                        if (motionProps._onlyLookAtTarget)
                        {
                            combat_camera.CurrentCameraLookAt(ref targets, motionProps._hangonDuration);
                        }
                        else
                        {
                            CameraLerp lerp = CameraLerp.Create();
                            lerp.lerpStyle = CameraLerp.LerpStyle.determineAtRunTime;
                            lerp.dialogueCameraLerpSmoothing = motionProps._blendLerpSmoothing;
                            lerp.animationCurve = motionProps._blendLerpCurve;
                            lerp.dialogueCameraLerpTime = motionProps._lerpDuration;
                            lerp.pitchLerpSmoothing = motionProps._blendPitchLerpSmoothing;
                            lerp.curvePitchLerp = motionProps._blendPitchLerpCurve;
                            lerp.yawLerpSmoothing = motionProps._blendYawLerpSmoothing;
                            lerp.curveYawLerp = motionProps._blendYawLerpCurve;
                            lerp.hangonTime = motionProps._hangonDuration;
                            combat_camera.BlendCurrentCamera(ref targets, motionProps._blendDistanceOffset, motionProps._blendPitchOffset,
                                                             motionProps._blendYawOffset, motionProps._blendHeightOffset, lerp);
                        }
                    }
                    else
                    {
                        //int count = options.Count;
                        MoveEditor.CameraMotionOption option = MoveEditor.MoveUtils.GetCamermotionLottery(ref options, false);
                        if (option != null)
                        {
                            //Debug.LogError("OnCameraMotion != null cbt=" + Data.ToString());
                            CameraMotion motion = GlobalCameraMotionData.Instance.GetCameraMotion(option._motionName);
                            if (motion != null)
                            {
                                combat_camera.State = CombatCamera.MotionState.Lerping;
                                //MyFollowCamera.Instance.isActive = false;
                                GameCameraParams gameCameraParams = (GameCameraParams)motion.camera;
                                CameraLerp motion_lerp = motion.cameraLerpOverride;
                                motion_lerp.dialogueCameraLerpTime = motionProps._lerpDuration;
                                motion_lerp.hangonTime = motionProps._hangonDuration;
                                combat_camera.EnterInteractionCamera(ref targets, ref gameCameraParams, motion_lerp);
                            }
                        }
                    }
                }

            }
        }

		public void Stop()
		{
			_played = false;
		}

		public bool IsPlaying()
		{
			return _played;
		}
	}

	public class CameraMotionPreviewManager
	{
		private static CameraMotionPreviewManager _instance;
		public static CameraMotionPreviewManager Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new CameraMotionPreviewManager();
				}

				return _instance;
			}
		}

		public Vector3 OriginPos = new Vector3(0f, 21, -18f);
		public Vector3 OriginAngle = new Vector3(21.0f, 320.0f, 0.0f);

		private List<CameraMotionPreview> _previews;
		private GameObject _focusObject;
        private GameObject _hitObject;
		private CameraMotionPreview _currentPreview;

		public CombatCamera _previewCamera;

		private CameraMotionPreviewManager()
		{
			_previews = new List<CameraMotionPreview>();
		}

		public void Init(GameObject focusObject,GameObject hitObject)
		{
			_focusObject = focusObject;
            _hitObject = hitObject;
            _previews.Clear();
			//Camera main_camera = Camera.main;
			//if(main_camera != null)
			//{
			//	main_camera.transform.position = OriginPos;
			//	main_camera.transform.eulerAngles = OriginAngle;
   //             _previewCamera = main_camera.gameObject.GetComponent<CombatCamera>();
   //             if(_previewCamera == null)
   //             {
   //                 _previewCamera = main_camera.gameObject.AddComponent<CombatCamera>();
   //             }
   //         }
		}

		public void RegisterPreview(CameraMotionEventInfo evt, float startTime )
		{
			CameraMotionPreview preview = new CameraMotionPreview(evt, _focusObject,_hitObject, startTime);
			_previews.Add (preview);
		}
        
		public void Update(float time,float deltaTime)
		{
            //Debug.Log(deltaTime);
			int count = _previews.Count;
			for(int i = 0; i < count; i++)
			{
				if(_previews[i].StartTime < time)
				{
					if(_currentPreview == null)
					{
						_currentPreview = _previews[i];
						_currentPreview.Play();
					}
					else
					{
						if(_currentPreview.StartTime < _previews[i].StartTime)
						{
							_currentPreview.Stop();
							_currentPreview = _previews[i];
							_currentPreview.Play();
						}
					}
				}
			}

			if(_currentPreview != null)
			{
				_currentPreview.Update(time, deltaTime);
			}
		}

		public void Reset()
		{
			if(_currentPreview != null)
			{
				_currentPreview.Stop();
				_currentPreview = null;
			}
			//Camera main_camera = Camera.main;
			//if(main_camera != null)
			//{
			//	main_camera.transform.position = OriginPos;
			//	main_camera.transform.eulerAngles = OriginAngle;
			//}
		}

		public void DeregisterAll()
		{
#if UNITY_EDITOR

            _previewCamera = null;
#else
            GameObject.DestroyImmediate(_previewCamera);
#endif

            Reset ();
			_previews.Clear();
		}
	}
}