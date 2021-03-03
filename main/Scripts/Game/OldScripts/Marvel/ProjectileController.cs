using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ProjectileController : MonoBehaviour
{
	[System.Serializable]
	public class ProjectileEvent
	{
		public List<MoveEditor.ParticleEventInfo> _particles = new List<MoveEditor.ParticleEventInfo>();
		public List<MoveEditor.TrailRendererEventInfo> _trails = new List<MoveEditor.TrailRendererEventInfo>();
		public List<MoveEditor.DynamicLightEventInfo> _dynamicLights = new List<MoveEditor.DynamicLightEventInfo>();
		public List<MoveEditor.CameraShakeEventInfo> _cameraShakes = new List<MoveEditor.CameraShakeEventInfo>();
	}

	public delegate void OnProjectileDeactivated();

	private MoveEditor.ProjectileEventInfo _projectileEventInfo;
	private DynamicMonoILRObject _owner;
	private MoveEditor.FXHelper _fxHelper;
	private ProjectileState _state;
	private float _speed = 0.0f;
	private OnProjectileDeactivated _deactivateCallback = null;

	private float _startTime = 0.0f;
	private float _duration = 0.0f;

	public ProjectileEvent _onSpawnEvents;
	public AnimationCurve  _motionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f);
	public ProjectileEvent _onHitEvents;

	private enum ProjectileState
	{
		kInactive,
		kActive,
		kReturn,

		#region [GM]
		kSpwaned
		#endregion
	};

	// Use this for initialization
	void Awake()
	{
		_fxHelper = GetComponent<MoveEditor.FXHelper>();
		_state = ProjectileState.kInactive;
	}

	private Vector3 GetCenter()
	{
		if (GetComponent<Renderer>() != null)
		{
			return GetComponent<Renderer>().bounds.center;
		}
		return transform.position;
	}

	// Update is called once per frame
	private void Update()
	{
		if (IsAutoVelocity) AutoVelocityUpdate();
		else DefaultUpdate();
	}

	private void DefaultUpdate()
	{
		// don't update if player's frozen
		//if( _owner != null && _owner.Animator.speed == 0.0f )
		//{
		//return;
		//}

		//_previousPosition = GetCenter();

		switch (_state)
		{
			case ProjectileState.kSpwaned:
				_state = ProjectileState.kActive;
				break;
			case ProjectileState.kActive:
				{
					AttributeData attr = _owner.OnGetAttributeData("GetTargetPosition");
					Vector3 dir = _owner != null ? attr.TargetPosition - GetCenter() : Vector3.right;
					//dir.y = 0.0f;//set y to zero to calculate motion on xoz plane.
					Vector3 displacement = dir * _speed * Time.deltaTime;
					Vector3 pos = transform.position;
					pos += displacement;
					//caculate motion on y 
					float offset_y = _motionCurve.Evaluate(System.Math.Min(Time.realtimeSinceStartup - _startTime / _duration, 1.0f));
					pos.y += offset_y;
					Quaternion rotation = Quaternion.LookRotation(pos - transform.position);
					transform.rotation = rotation;
					transform.position = pos;

					if (_owner != null)
					{
						//float prevPosX = _previousPosition.x;
						//float currPosX = GetCenter().x;

						//float minPosX = Mathf.Min(prevPosX, currPosX);
						//float maxPosX = Mathf.Max(prevPosX, currPosX);
						float distance = Vector3.Distance(GetCenter(), attr.TargetPosition);

						if (distance < 0.05f)
						{
							OnHit();
							break;
						}
					}

					// TODO: is it safe to assume that projectiles will never hit walls - since it'll always hit a player first?

					break;
				}
			case ProjectileState.kReturn:
				{
					if (UpdateReturn())
					{
						Deactivate();
					}
					break;
				}
			default:
				break;
		}
	}

	#region new logic for GM
	private void AutoVelocityUpdate()
	{
		// don't update if player's frozen
		//if( _owner != null && _owner.Animator.speed == 0.0f )
		//{
		//return;
		//}

		//_previousPosition = GetCenter();

		switch (_state)
		{
			case ProjectileState.kSpwaned: _state = ProjectileState.kActive; break; // don't displace on spawn frame

			case ProjectileState.kActive:
				{
					#region [GM]
					/*Vector3 dir = _owner != null ? _owner.transform.forward : Vector3.right;
					dir.y = 0.0f;//set y to zero to calculate motion on xoz plane.*/

					AttributeData attr = _owner.OnGetAttributeData("GetTargetRadius");

					Vector3 dir = (attr.TargetPosition + new Vector3(0, attr.TargetRadius, 0)  - transform.position).normalized;
					#endregion

					Vector3 displacement = dir * _speed * Time.deltaTime;
					Vector3 pos = transform.position;
					pos += displacement;
					//calculate motion on y 
					float offset_y = _motionCurve.Evaluate(System.Math.Min(Time.realtimeSinceStartup - _startTime / _duration, 1.0f));
					pos.y += offset_y;
					Quaternion rotation = Quaternion.LookRotation(pos - transform.position);
					transform.rotation = rotation;
					transform.position = pos;

					if (_owner != null)
					{
						#region [GM]
						/*float prevPosX = _previousPosition.x;
						float currPosX = GetCenter().x;

						float minPosX = Mathf.Min(prevPosX, currPosX);
						float maxPosX = Mathf.Max(prevPosX, currPosX);
						float opponentPosX = _owner.GetTargetPosition().x;
						float opponentRadius = _owner.GetTargetRadius();

						if( GameplayUtils.RangeIntersect( minPosX, maxPosX, opponentPosX - opponentRadius, opponentPosX + opponentRadius ) )
						{
							OnHit();
							break;
						}*/

						// [GM] to simplify hit detection, we check the time in place of complicated distance measurement
						if (Time.time >= impactTime)
						{
							OnHit();
							break;
						}
						#endregion
					}

					// TODO: is it safe to assume that projectiles will never hit walls - since it'll always hit a player first?

					break;
				}
			case ProjectileState.kReturn:
				{
					if (UpdateReturn())
					{
						Deactivate();
					}
					break;
				}
			default:
				break;
		}
	}
	#endregion

	private bool UpdateReturn()
	{
		MoveEditor.ProjectileEventProperties projectileProps = _projectileEventInfo._projectileProperties;
		float distanceDelta = projectileProps._reattachmentVelocity * Time.deltaTime;

		AttributeData attr = _owner.OnGetAttributeData("GetAnimator");
		Animator ownerAnimator = attr.Animator;
		Transform attachmentTransform = MoveEditor.MoveUtils.GetBodyPartTransform( ownerAnimator, projectileProps._reattachment, projectileProps._reattachmentPath );

		Vector3 attachmentPosition = attachmentTransform.TransformPoint(projectileProps._reattachmentOffset);

		bool flipped = false;//_owner.ID == 1;
		Quaternion rotation = Quaternion.Euler(projectileProps._reattachmentAngles);
		Quaternion attachmentOrientation = Quaternion.identity;
		if (projectileProps._reattachmentIsWorldSpace)
		{
			attachmentOrientation = rotation;
			if (flipped)
			{
				// rotate this by 180
				transform.RotateAround(transform.position, Vector3.up, 180.0f);
			}
		}
		else
		{
			attachmentOrientation = attachmentTransform.rotation * rotation;
		}

		if (flipped)
		{
			// mirror!
			attachmentOrientation = FlipRotation(attachmentOrientation);
		}

		float distance = Vector3.Distance( attachmentPosition, transform.position);

		if (distance > distanceDelta)
		{
			Vector3 pos =  Vector3.MoveTowards( transform.position, attachmentPosition, distanceDelta);
			Quaternion q = Quaternion.RotateTowards( transform.rotation, attachmentOrientation, projectileProps._reattachmentAngularVelocity );

			transform.position = pos;
			transform.rotation = q;
		}
		else
		{
			transform.position = attachmentPosition;
			transform.rotation = attachmentOrientation;

			return true;
		}

		return false;
	}

	// TODO: Move this to a common place as this is duplicated in MoveUtils
	private Quaternion FlipRotation(Quaternion rotation)
	{
		Vector3 xAxis = rotation * Vector3.right;
		Vector3 yAxis = rotation * Vector3.up;
		Vector3 zAxis = rotation * Vector3.forward;

		Vector3 newXAxis = xAxis;
		Vector3 newYAxis = yAxis;
		Vector3 newZAxis = zAxis;

		float absDotX = Mathf.Abs(Vector3.Dot(xAxis, Vector3.right));
		float absDotY = Mathf.Abs(Vector3.Dot(yAxis, Vector3.right));
		float absDotZ = Mathf.Abs(Vector3.Dot(zAxis, Vector3.right));

		if (absDotX > absDotY && absDotX > absDotZ)
		{
			newXAxis = Vector3.Reflect(xAxis, Vector3.forward);

			// figure out what's up
			if (Mathf.Abs(Vector3.Dot(yAxis, Vector3.up)) > Mathf.Abs(Vector3.Dot(zAxis, Vector3.up)))
			{
				//			EB.Debug.Log ( "x y z");
				newYAxis = Vector3.Reflect(yAxis, Vector3.forward);
				newZAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newXAxis), Vector3.Normalize(newYAxis)));
			}
			else
			{
				//			EB.Debug.Log ( "x z y");
				newZAxis = Vector3.Reflect(zAxis, Vector3.forward);
				newYAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newZAxis), Vector3.Normalize(newXAxis)));
			}

		}
		else if (absDotY > absDotZ && absDotY > absDotX)
		{
			newYAxis = Vector3.Reflect(yAxis, Vector3.forward);

			// figure out what's up
			if (Mathf.Abs(Vector3.Dot(xAxis, Vector3.up)) > Mathf.Abs(Vector3.Dot(zAxis, Vector3.up)))
			{
				//			EB.Debug.Log ( "y x z");
				newXAxis = Vector3.Reflect(xAxis, Vector3.forward);
				newZAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newXAxis), Vector3.Normalize(newYAxis)));
			}
			else
			{
				//			EB.Debug.Log ( "y z x");
				newZAxis = Vector3.Reflect(zAxis, Vector3.forward);
				newXAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newYAxis), Vector3.Normalize(newZAxis)));
			}
		}
		else
		{
			newZAxis = Vector3.Reflect(zAxis, Vector3.forward);

			// figure out what's up
			if (Mathf.Abs(Vector3.Dot(xAxis, Vector3.up)) > Mathf.Abs(Vector3.Dot(yAxis, Vector3.up)))
			{
				//		EB.Debug.Log ( "z1 x y");
				newXAxis = Vector3.Reflect(xAxis, Vector3.forward);
				newYAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newZAxis), Vector3.Normalize(newXAxis)));
			}
			else
			{
				//		EB.Debug.Log ( "z y x");
				newYAxis = Vector3.Reflect(yAxis, Vector3.forward);
				newXAxis = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(newYAxis), Vector3.Normalize(newZAxis)));
			}
		}

		//EB.Debug.Log ( newXAxis+" " + newYAxis + " "+newZAxis );
		Vector3.OrthoNormalize(ref newXAxis, ref newYAxis, ref newZAxis);
		//EB.Debug.Log ( newXAxis+" " + newYAxis + " "+newZAxis );
		return Quaternion.LookRotation(newZAxis, newYAxis);
	}

	private void LerpTransformLocal(Transform current, Transform target, float speed, float angularSpeed)
	{
		Vector3 pos = Vector3.MoveTowards( current.localPosition, target.localPosition, speed );
		Quaternion q = Quaternion.RotateTowards ( current.localRotation, target.localRotation, angularSpeed );

		current.localPosition = pos;
		current.localRotation = q;
	}

    public void Activate(GameObject owner, MoveEditor.ProjectileEventInfo projectileEventInfo, OnProjectileDeactivated callback)
    {
        _owner = owner.GetMonoILRComponent<DynamicMonoILRObject>();

		AttributeData attr = _owner.OnGetAttributeData("GetAnimator");

		if (_fxHelper != null)
        {
            //_fxHelper.Init(owner);
        }

        if (owner != null)
        {
            Activate(attr.Animator, projectileEventInfo, callback, false); //owner.ID == 1 );
        }
        else
        {
            EB.Debug.LogWarning("Failed to activate projectile!");
        }
    }

    #region [GM] Projectile.
    private float impactTime;

	private bool IsAutoVelocity
	{
		get { return Application.isPlaying && _projectileEventInfo != null && _projectileEventInfo._projectileProperties._autoVelocity; }
	}

	private void CalculateAutoVelocity()
	{
		// calculate distance to target
		Vector3 ownPosition = GetCenter();
		AttributeData attr = _owner.OnGetAttributeData("GetCurrentMove");

		Vector3 targetPosition = attr.TargetPosition;

		//ownPosition.y = targetPosition.y = 0;
		float distanceToTarget = Vector3.Distance(ownPosition, targetPosition);

		// calculate time to impact
		float createProjectileFrame = _projectileEventInfo._frame;

		float hitReactionFrame =
			attr.CurrentMove._hitEvents
				.First(arg => arg._frame > createProjectileFrame)
				._frame;

		float numFramesToImpact = hitReactionFrame - createProjectileFrame;
		float timeToImpact = numFramesToImpact / attr.CurrentMove._animationClip.frameRate;

		// calculate speed and impactTime;
		_speed = distanceToTarget / timeToImpact;
		impactTime = Time.time + timeToImpact;
	}
	#endregion

	public void Activate(Animator ownerAnimator, MoveEditor.ProjectileEventInfo projectileEventInfo, OnProjectileDeactivated callback, bool flipped)
	{
		_projectileEventInfo = projectileEventInfo;
		_deactivateCallback = callback;
		MoveEditor.ProjectileEventProperties projectileProperties = projectileEventInfo._projectileProperties;

		Transform spawnTransform = MoveEditor.MoveUtils.GetBodyPartTransform( ownerAnimator, projectileProperties._spawnAttachment, projectileProperties._spawnAttachmentPath );
		_speed = projectileProperties._initialVelocity;

		transform.parent = spawnTransform;
        transform.localPosition = projectileProperties._spawnOffset;

		if (projectileProperties._worldSpace)
		{
			transform.eulerAngles = projectileProperties._spawnAngles;

			if (flipped)
			{
				// rotate this by 180
				transform.RotateAround(transform.position, Vector3.up, 180.0f);
			}
		}
		else
		{
			transform.localEulerAngles = projectileProperties._spawnAngles;
		}

		// mirror the effect, for parented effects, this is done inside the attach transform
		if (flipped)
		{
			transform.rotation = FlipRotation(transform.rotation);
		}

		transform.parent = null;
		transform.localScale = Vector3.one;

		// play all on spawn events
		PlayEffects(_onSpawnEvents);

		//_previousPosition = transform.position;

		_state = ProjectileState.kSpwaned;

		#region [GM] Projectile.
		if (_projectileEventInfo._projectileProperties._autoVelocity)
		{
			CalculateAutoVelocity();
			_state = ProjectileState.kSpwaned; // don't displace on kSpwaned (will go to kActive on next frame)
		}
		#endregion

		_startTime = Time.realtimeSinceStartup;
		_duration = 1.0f;
		if (_owner != null)
		{
			AttributeData attr = _owner.OnGetAttributeData("GetTargetPosition");
			float distance = Vector3.Distance(GetCenter(), attr.TargetPosition);
			_duration = distance / _speed;
		}
	}

	private void OnHit()
	{
		if (_owner != null)
		{
			Vector3 hitPosition = transform.position;

			AttributeData attr = _owner.OnGetAttributeData("GetHitPoint");
			hitPosition.z = attr.HitPoint.z;  // this will move the hit point to the center of the body, much more accurate for "spawn at hit point" effects
		}

		StopEffects(_onSpawnEvents);
		PlayEffects(_onHitEvents);

		if (_projectileEventInfo._projectileProperties._isBoomerang)
		{
			_state = ProjectileState.kReturn;
		}
		else
		{
			Deactivate();
		}
	}

	public void Deactivate()
	{
		if (_fxHelper != null)
		{
			//_fxHelper.StopAll();
		}

		_state = ProjectileState.kInactive;

		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;

		if (_deactivateCallback != null)
		{
			_deactivateCallback();
			_deactivateCallback = null;
		}
	}

	private void PlayEffects(ProjectileEvent projectileEvent)
	{
		if (_fxHelper == null)
		{
			return;
		}

		for (int j = 0; j < projectileEvent._particles.Count; ++j)
		{
			MoveEditor.ParticleEventInfo particleEventInfo = projectileEvent._particles[j];

			// Dirty hack here - if the particle's set to spawn at hit point, we call the player's FXHelper
			// the reason being that if we don't then the effects will get destroyed since the projectile would likely be deactivated on hit, thus destroying all its fx
			if (particleEventInfo._particleProperties._spawnAtHitPoint)
			{
				MoveEditor.MoveAnimationEvent moveEvent = new MoveEditor.MoveAnimationEvent() { EventRef = particleEventInfo };
				_owner.OnHandleMessage("OnPlayParticle", moveEvent);
				//_owner.FXHelper.PlayParticle(particleEventInfo._particleProperties);
			}
			else
			{
				_fxHelper.PlayParticle(particleEventInfo._particleProperties);
			}
		}

		for (int j = 0; j < projectileEvent._trails.Count; ++j)
		{
			MoveEditor.TrailRendererEventInfo trailEventInfo = projectileEvent._trails[j];
			_fxHelper.PlayTrailRenderer(trailEventInfo._trailRendererProperties);
		}

		for (int j = 0; j < projectileEvent._dynamicLights.Count; ++j)
		{
			MoveEditor.DynamicLightEventInfo dynamicLights = projectileEvent._dynamicLights[j];
			_fxHelper.PlayDynamicLight(dynamicLights._dynamicLightProperties);
		}

		AttributeData attr = _owner.OnGetAttributeData("IsCurrentAttackCritical");
		for (int i = 0; i < projectileEvent._cameraShakes.Count; ++i)
		{
			MoveEditor.CameraShakeEventInfo shakeInfo = projectileEvent._cameraShakes[i];
			MoveCameraShakeHelper.Shake(shakeInfo, _owner != null && attr.IsCurrentAttackCritical);
		}
	}

	private void StopEffects(ProjectileEvent events)
	{
		if (_fxHelper == null)
		{
			return;
		}

		// TODO this should stop only the events specified in the events but this might be good enough
		_fxHelper.StopAll(true); // [GM] set ClearParticles argument to TRUE in order to clear projectile particle on OnHit()
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(GetCenter(), 0.1f);
	}
}
