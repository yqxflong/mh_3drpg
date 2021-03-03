using UnityEngine;
using System.Collections.Generic;

public class GameCameraBehavior
{
	private Vector3 _gameCameraLookAt = Vector3.zero;
	private Quaternion _gameCameraRotation = Quaternion.identity;
	private float _gameCameraDistanceFromLookat = 1f;

	private List<GameObject> _targets;
	private GameCameraParams _params;

	public GameCameraParams Params { get { return _params; } }

	// are the input parameters to the two cameras the same, meaning the two cameras will have the same behavior
	static public bool AreInputParametersEqual(GameCameraBehavior behaviorOne, GameCameraBehavior behaviorTwo)
	{
		if (behaviorOne == behaviorTwo) // the camera instances are the same (could both be null)
		{
			return true;
		}
		else if (null == behaviorOne || null == behaviorTwo) // one is null
		{
			return false;
		}

		if (!AreInputParametersEquivalent(behaviorOne, behaviorTwo))
		{
			return false;
		}
		return AreInputTargetListsEquivalent(behaviorOne, behaviorTwo);
	}

#if UNITY_EDITOR
	// get a description of the camera
	public string GetCameraText()
	{
		string outString = "";
		if (IsGameCamera())
		{			
			outString += "game cam default";
			if (null == _params)
			{
				outString += " (normal)";
			}
		}
		else
		{
			outString += _params.name;
		}

		outString += ", Targets (" + _targets.Count + "):";

		for (int singleTarget = 0; singleTarget < _targets.Count; ++singleTarget)
		{
			outString += " " + _targets[singleTarget].name;
		}

		if (IsGameCamera() && 0 == _targets.Count)
		{
			outString += " player";
		}
		return outString;
	}
#endif

	public GameCameraBehavior()
	{
	}

	public GameCameraBehavior(ref List<GameObject> targets, ref GameCameraParams newParams, CameraBase playerCamera)
	{
		_targets = targets;
		_params = newParams;
		SetGameCameraPositionAndRotation(CalculateTargetPosition(), playerCamera);
	}

	public void OnDrawDebug()
	{
#if DEBUG
		Vector3 position = Vector3.zero;
		GetPosition(ref position);
		GLRenderingUtils.DoDrawLine(_gameCameraLookAt, position, Color.green);

		GLRenderingUtils.DoDrawSphere(_gameCameraLookAt, 0.8f, Color.yellow);
		GLRenderingUtils.DoDrawSphere(position, 0.8f, Color.red);
#endif
	}

	public void SetTargets(ref List<GameObject> targets)
	{
		_targets = targets;
	}

	public void Update(CameraBase playerCamera,float deltaTime)
	{
		LerpGameCameraToPositionAndRotation(CalculateTargetPosition(), playerCamera, deltaTime);
	}

	public void GetLookAt(ref Vector3 lookAt)
	{
        if (float.IsNaN( _gameCameraLookAt.x) || float.IsNaN(_gameCameraLookAt.y) || float.IsNaN(_gameCameraLookAt.z))
        {
            //
            EB.Debug.Log("{0},GetLookAt():_gameCameraLookAt:{1},Fixed:Vector3.zero",EB.Debug.ACCIDENTAL, _gameCameraLookAt);
            _gameCameraLookAt = Vector3.zero;
        }
		lookAt = _gameCameraLookAt;
	}

	public float GetDistFromLookAt()
	{
        if (float.IsNaN(_gameCameraDistanceFromLookat))
        {
            //
            EB.Debug.Log("{0},GetDistFromLookAt():_gameCameraDistanceFromLookat:{1},Fixed:15.66f", EB.Debug.ACCIDENTAL, _gameCameraDistanceFromLookat);
            _gameCameraDistanceFromLookat = 15.66f;
        }
        return _gameCameraDistanceFromLookat;
	}

	public void GetPosition(ref Vector3 position)
	{		
		position = _gameCameraLookAt - (GameUtils.CalculateForward(_gameCameraRotation) * _gameCameraDistanceFromLookat);
	}

	public void GetRotation(ref Quaternion rotation)
	{
        if (float.IsNaN(_gameCameraRotation.x) || float.IsNaN(_gameCameraRotation.y) || float.IsNaN(_gameCameraRotation.z) || float.IsNaN(_gameCameraRotation.w))
        {
            //
            EB.Debug.Log("{0},GetRotation():_gameCameraRotation:{1},Fixed: Quaternion(0, 0.93f, -0.34f, 0)", EB.Debug.ACCIDENTAL ,_gameCameraLookAt);
            _gameCameraRotation = new Quaternion(0, 0.93f, -0.34f, 0);
        }
        rotation = _gameCameraRotation;
	}

	public void SetPosition(Vector3 position)
	{
		_gameCameraLookAt = position + (GameUtils.CalculateForward(_gameCameraRotation) * _gameCameraDistanceFromLookat);
	}

	public void SetRotation(Quaternion rotation)
	{

		_gameCameraRotation = rotation;
	}

	// lerp to where we want to be
	public void LerpGameCameraToPositionAndRotation(Vector3 targetPosition, CameraBase playerCamera,float deltaTime)
	{
		Vector3 perfectCamPos;
		Quaternion perfectCamRotation;
		Vector3 perfectCamLookAt;
		CalculatePerfectPositionAndRotation(targetPosition, playerCamera.GetZoomDistance(), playerCamera, out perfectCamPos, out perfectCamRotation, out perfectCamLookAt);
				
		// lerp small changes (going up/down stairs etc)
		Vector3 diff = perfectCamLookAt - _gameCameraLookAt;
		const float SmoothingSpeed = 8f;
		float mult = Mathf.Min(SmoothingSpeed * deltaTime, 1f);
		_gameCameraLookAt = _gameCameraLookAt + mult * diff;
		_gameCameraRotation = Quaternion.RotateTowards(_gameCameraRotation, perfectCamRotation, 45.0f * deltaTime);

		float lookAtToPositionDist = (perfectCamPos - perfectCamLookAt).magnitude;
		_gameCameraDistanceFromLookat = _gameCameraDistanceFromLookat + ((lookAtToPositionDist - _gameCameraDistanceFromLookat) * mult);
	}

	// set our position/rotation
	public void SetGameCameraPositionAndRotation(Vector3 targetPosition, CameraBase playerCamera)
	{
		Vector3 perfectCamPos;
		Quaternion perfectCamRotation;
		Vector3 perfectCamLookAt;
		CalculatePerfectPositionAndRotation(targetPosition, playerCamera.GetZoomDistance(), playerCamera, out perfectCamPos, out perfectCamRotation, out perfectCamLookAt);

		_gameCameraLookAt = perfectCamLookAt;
		_gameCameraRotation = perfectCamRotation;
		_gameCameraDistanceFromLookat = (perfectCamPos - perfectCamLookAt).magnitude;
	}

	private Plane _plane = new Plane();
	private Ray _ray = new Ray();

	// calculate what the look at point is based on the inputs
	public Vector3 CalculateLookAt(Vector3 targetPosition, Vector3 camPos, Quaternion camRotation)
	{
		Vector3 planeNormal = targetPosition - camPos;
		if (GameUtils.NormalizeXZ(ref planeNormal))
		{
			_plane.SetNormalAndPosition(planeNormal, targetPosition);

			Vector3 forward = GameUtils.CalculateForward(camRotation);
			
			_ray.origin = camPos;
			_ray.direction = forward;
			
			float dist = 0f;
			if (_plane.Raycast(_ray, out dist))
			{
				return camPos + forward * dist;
			}
		}
		return targetPosition;		
	}

	private void CalculateOffsets(float zoom, CameraBase playerCamera, ref float PosVertical, ref float PosDist, ref float tiltDeg)
	{
		// put into graphing calculator: ( 1-sin((1-x)*1.57) )^1.6		
		const float Curve = 0f;
		float zoomDistanceCurved = Mathf.Lerp(zoom, Mathf.Pow(1.0f - Mathf.Sin((1.0f - zoom) * 1.57f), 1.6f), Curve);
		zoomDistanceCurved = Mathf.Clamp01(zoomDistanceCurved);

		if (!IsGameCamera())
		{
			if(_params != null && _params.useTargetLocalSpace)
			{
				PosVertical = _params.heightOffset;
				PosDist = _params.distance;
				tiltDeg = 0f;
			}
			else
			{
				PosVertical = GlobalCameraData.Instance.gameCameraFarVertical;
				PosDist = GlobalCameraData.Instance.gameCameraFarDist;
				tiltDeg = 0f;
			}
		}
		else
		{
			PosVertical = Mathf.Lerp(GlobalCameraData.Instance.gameCameraCloseVertical, GlobalCameraData.Instance.gameCameraFarVertical, zoomDistanceCurved);
			PosDist = Mathf.Lerp(GlobalCameraData.Instance.gameCameraCloseDist, GlobalCameraData.Instance.gameCameraFarDist, zoom);
			tiltDeg = Mathf.Lerp(GlobalCameraData.Instance.gameCameraClosePitch, GlobalCameraData.Instance.gameCameraFarPitch, zoomDistanceCurved);
		}
	}

	// is this the game camera
	private bool IsGameCamera()
	{
		return (null == _params || _params.gameCamera);
	}

	// get perfect normalized offset
	private Vector3 CalculatePerfectNormalizedGroundPlaneOffset(CameraBase playerCamera)
	{
		float yawOffset = !IsGameCamera() ? _params.yawOffset : 0f;
		const float FullCircle = 360f;
		float baseAngle = GlobalCameraData.Instance.gameCameraAngleDegrees;
		if(_params != null && _params.useTargetLocalSpace && _targets != null && _targets.Count > 0)
		{
			Vector3 targetAngle = _targets[0].transform.eulerAngles;
			baseAngle = targetAngle.y;
			
			if(_params.yawFlipped)
			{
                //Combatant combatant_obj = _targets[0].GetComponent<Combatant>();
                //if (combatant_obj != null)
                //{

                //    if (combatant_obj.IsAttackingRightSide())
                //    {
                //        yawOffset = -yawOffset;
                //    }
                //}

                DynamicMonoILRObject ilObj = _targets[0].GetMonoILRComponent<DynamicMonoILRObject>(false);
                if (ilObj != null)
                {
                    AttributeData attr = ilObj.OnGetAttributeData("IsAttackingRightSide");
                    if (attr.IsAttackingRightSide)
                    {
                        yawOffset = -yawOffset;
                    }
                }
            }
		}

		float camAngleRad = Mathf.Deg2Rad * ((baseAngle + yawOffset) % FullCircle);
		return new Vector3(Mathf.Sin(camAngleRad), 0f, Mathf.Cos(camAngleRad));
	}

	// get perfect rotation
	//private Matrix4x4 CalculatePerfectGroundPlaneRotation(CameraBase playerCamera)
	//{
	//    Vector3 camOffset = CalculatePerfectNormalizedGroundPlaneOffset(playerCamera);
	//    Quaternion rotationQuat = Quaternion.identity;
	//    if(_params != null && _params.useTargetLocalSpace)
	//    {
	//        rotationQuat = CalculatePerfectGroundPlaneRotation(camOffset);
	//    }
	//    else
	//    {
	//        rotationQuat = CalculatePerfectGroundPlaneRotation(-camOffset);
	//    }

	//    Matrix4x4 rotationMatrix = Matrix4x4.identity;
	//    rotationMatrix.SetTRS(Vector3.zero, rotationQuat, Vector3.one);
	//    return rotationMatrix;
	//}

	// get perfect rotation
	private Quaternion CalculatePerfectGroundPlaneRotation(Vector3 normalizedLook)
	{
		return Quaternion.LookRotation(Vector3.Normalize(normalizedLook));
	}

	// where should the camera be
	private void CalculatePerfectPositionAndRotation(Vector3 targetPosition, float zoom, CameraBase playerCamera, 
													out Vector3 newCamPos, out Quaternion newCamRotation, out Vector3 newCamLookAt)
	{
		float PosVertical = 0f;
		float PosDist = 0f;
		float tiltDeg = 0f;
		CalculateOffsets(zoom, playerCamera, ref PosVertical, ref PosDist, ref tiltDeg);

		Vector3 camOffset = CalculatePerfectNormalizedGroundPlaneOffset(playerCamera);
		if(_params != null && _params.useTargetLocalSpace)
		{
			newCamRotation = CalculatePerfectGroundPlaneRotation(camOffset);
		}
		else
		{
			newCamRotation = CalculatePerfectGroundPlaneRotation(-camOffset);
		}
		camOffset *= PosDist;

		camOffset.y = PosVertical;
		newCamPos = targetPosition + camOffset;

		RestrictCameraDistance(targetPosition, ref newCamPos);

		if (IsGameCamera())
		{
			newCamRotation *= Quaternion.AngleAxis(tiltDeg, Vector3.right);
		}
		else
		{
			if(!_params.useTargetLocalSpace)
			{
				newCamPos += Vector3.up * _params.heightOffset;
			}
			float acosInput = Vector3.Dot((targetPosition - newCamPos).normalized, -Vector3.up);
			float angleToLookDirectlyAtTargetRad = (Mathf.PI * 0.5f) - Mathf.Acos(acosInput);
			float angleToLookDirectlyAtTargetDeg = angleToLookDirectlyAtTargetRad * Mathf.Rad2Deg;

			const float QuarterCircleDeg = 90f;
			angleToLookDirectlyAtTargetDeg = Mathf.Clamp(angleToLookDirectlyAtTargetDeg + _params.pitch, -QuarterCircleDeg, QuarterCircleDeg);
			newCamRotation *= Quaternion.AngleAxis(angleToLookDirectlyAtTargetDeg, Vector3.right);
		}
		newCamLookAt = CalculateLookAt(targetPosition, newCamPos, newCamRotation);
		if(!IsGameCamera() && _params.lookAtY != 0.0f)
		{
			newCamLookAt.y = _params.lookAtY;
		}
	}

	// get the distance from the targets, based on how we are calculating distance
	private void RestrictCameraDistance(Vector3 targetPos, ref Vector3 camPos)
	{
		if (!IsGameCamera())
		{
			const float NormalizationMin = 0.01f;
			camPos = targetPos + Vector3.Normalize(camPos - targetPos) * Mathf.Max(NormalizationMin, _params.distance);
		}
	}

	// calculate a single target position from all our targets
	private Vector3 CalculateTargetPosition()
	{
		if (null == _targets || 0 == _targets.Count)
		{
			return Vector3.zero;
		}

		return GameUtils.CalculateBoundsWithOffset(_targets).center;
	}

	public bool IsFocusingTarget(GameObject target)
	{
		if(_targets != null && _targets.Count > 0)
		{
			return _targets.Contains(target);
		}

		return false;
	}

	// check the conversations are equivalent
	static private bool AreInputParametersEquivalent(GameCameraBehavior behaviorOne, GameCameraBehavior behaviorTwo)
	{
		if (null == behaviorOne._params && null == behaviorTwo._params) // both null
		{
			return true; // the same
		}
		else if (null != behaviorOne._params && null != behaviorTwo._params) // are both non null
		{
			return (behaviorOne._params == behaviorTwo._params); // the same
		}
		
		// one is null and one is not null		
		if (null == behaviorOne._params)
		{
			return behaviorTwo._params.gameCamera; // null is equivalent to game camera
		}
		// null == conversationtwo		
		return behaviorOne._params.gameCamera; // null is equivalent to game camera			
	}

	// check the target lists are the same
	private static bool AreInputTargetListsEquivalent(GameCameraBehavior behaviorOne, GameCameraBehavior behaviorTwo)
	{
		if (behaviorOne._targets == behaviorTwo._targets) // the camera target instances are the same
		{
			return true;
		}
		if (null != behaviorOne._targets && null != behaviorTwo._targets && behaviorOne._targets.Count == behaviorTwo._targets.Count) // target lists are the same length
		{
			for (int singleTarget = 0; singleTarget < behaviorOne._targets.Count; ++singleTarget)
			{
				if (!behaviorTwo._targets.Contains(behaviorOne._targets[singleTarget])) // if a target is in one list but not the other
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}
}
