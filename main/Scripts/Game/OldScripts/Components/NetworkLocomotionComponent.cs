///////////////////////////////////////////////////////////////////////
//
//  NetworkLocomotionComponent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.RVO;

public class NetworkLocomotionComponent : BaseComponent
#if DEBUG
	, IDebuggableEx
#endif
{	
	public int maxNetworkTransformsToDraw = 8; // if debugging is turned on, how many network transforms from our history should we draw
	public bool allowNetworkUpdates = true; // set this to false (for debugging) if we want to ignore incoming network transforms
	public bool loopMode = false; // set to true and no new network transforms will be acceptedm, and the current ones will be looped over
	public bool drawFutureUpdatesInLoopMode = false; // if we're in loop mode and debug drawing, should updates newer than the current lop time be drawn	
	public bool fakeLocalDebugging = false; // if true, we send network updates to ourselves locally (to debug the interpolation)

	// if current time plus networkDisplaytime is more recent than the most recent network transform time, then positions must be extrapolated from the mostRecentUpdate time
	// if current time plus networkDisplaytime is older than the most recent network update time, then interpolate between previous update and most recent update
	// networkDisplaytime cannot be greater than zero (as that would be a future time)
	public float networkDisplayTime = 0f;
	public float velocityBlendMult = -1f; // how long should velocity take to blend in as a proportion of the network update interval (less than zero means use internal preferred values)
	public float positionBlendMult = -1f; // how long should position take to blend in as a proportion of the network update interval (less than zero means use internal preferred values)
	public float rotationBlendMult = -1f; // how long should rotation take to blend in as a proportion of the network update interval (less than zero means use internal preferred values) 
											// will be capped to less than _positionBlendMult as rotation should be blended in before or at same point as position (so everything meets up)	

	public bool drawDebug = false; // do we want debugging on

	private ReplicationView _networkLocomotionComponentViewRPC = null; // networking remote procedure calls
	private LocomotionComponentAPP _locomotion = null;	

#if DEBUG		
	private float _lastFakeLocalUpdateTime = -999f;
	private bool _wasLoopMode = false; // were we in loopMode last frame
	private float _loopModeEndTime = -1f; // if we're in loop mode, when did the loop period start
	private float _loopModeStartTime = -1f; // if we're in loop mode, when did the loop period end
	private int _howManyTimesHasLoopModeLooped = -1; // we keep track of how many loops have been made, this is so we know when we restart the loop
#endif

	private Vector3 _previousHostPosition = new Vector3(); // what was the host position last frame
	private Vector3 _hostPosition = new Vector3(); // what host position have we calculated this frame
	private Quaternion _hostRotation = Quaternion.identity;

	private RVOController _rvoController = null;

	private class NetworkTransform
	{
		public float time = -1f; // the game time we got the update
		public Vector3 position = new Vector3(); // the position of the update
		public Vector3 predictedVelocity = new Vector3(); // the velocity of the update
		public Vector3 correctVelocity = new Vector3(); // the velocity, which will get corrected when we get the update after this one
		
		public float predictedDistToDestination = DistanceUnlimited;
		public float correctedDistToDestination = DistanceUnlimited;
		public bool hasDestinationBeenReached = false; // true after we have extrapolated to correctedDistToDestination
		
		public Quaternion rotation = new Quaternion(); // the rotation of the update
		public Quaternion predictedRotation = new Quaternion(); // prediction of what rotation should be at time of update following this
		public Quaternion correctedRotation = new Quaternion();	 // predicted rotation corrected over time when we know what the next updates rotation is

		public bool isFirstUsage = true; // set to false after first usage
		public bool hasTransformBeenFullyBlendedIn = false; // if true, all updates before this one are nt effecting the characters position anymore
		public bool havePredictionsBeenFullyCorrected = false; // we have the update which follows this one, and have completely corrected
		public bool isTransitionToNextReady = false; // if true, we should stop extrapolating when we get to the time of the next network transform
		public double networkTime = 0;

		public float rotationPeriod = 0.5f;

		public float rotationBlendInTime = 0.5f;
		public float positionBlendInTime = 0.5f;

		// debugging ------------------------------------------------------------
		public Vector3 extrapolatedPosition = new Vector3();
		public float expectedNextUpdateTime = -1f;
		public float actualNextUpdateTime = -1f;
		public float angleError = -999f;
		public float positionalError = -1f;
		// debugging ------------------------------------------------------------

		public const float DistanceUnlimited = -10f;
		public static bool IsDistanceUnlimited(float distance)
		{
			const float Tol = 0.1f;
			return Mathf.Abs(distance - DistanceUnlimited) < Tol; // check that the parameter passed is equal to DistanceUnlimited (within floating point error)
		}

		public static bool IsMovement(Vector3 veloc) // is this network transform predicting the character will move or stop
		{
			const float TolSqr =  0.001f * 0.001f;
			return veloc.sqrMagnitude > TolSqr;
		}
	}
	const int NumNetworkTransforms = 40;
	private List<NetworkTransform> _networkTransforms = new List<NetworkTransform>(NumNetworkTransforms);
	private int _numNetworkTransformHistory = 8; // how many network transforms should be stored
	private const int MinNetworkTransformHistoryTime = 8; // how many seconds worth of network transforms should be stored

	private bool _isCurrentlyLocal = false; // caching this as we need to know if it changes
	public  bool IsCurrentlyLocal
	{
		get { return _isCurrentlyLocal; }
	}
	private uint _currentOwnerId = 0; // caching this as we need to know if it changes

	// is this character owned locally
	public bool AmILocal()
	{
#if DEBUG
		if (loopMode)
		{
			return false;
		}
#endif
		return _networkLocomotionComponentViewRPC.isMine;
	}

	public void EnableNetworkSynchronization(bool enable)
	{
		_networkLocomotionComponentViewRPC.stateSynchronization = enable ? EB.NetworkStateSynchronization.Unreliable : EB.NetworkStateSynchronization.Off;
	}

	public void Awake()
	{
		_networkLocomotionComponentViewRPC = FindReplicationViewForComponent<NetworkLocomotionComponent>();
		_locomotion = GetComponent<LocomotionComponentAPP>();
		_rvoController = GetComponent<RVOController>();

		_isCurrentlyLocal = AmILocal();
		_currentOwnerId = _networkLocomotionComponentViewRPC.ownerId;
	}

	// calculate what position and velocity we should send to peers who do not own the characters
	public void CalculatePositionAndVelocityToSend(ref Vector2 position, ref Vector2 velocity, ref float distToDestination)
	{
		// to save some bandwidth we're sending 2d vectors. y is calculated procedurally based on the ground height at the point you are standing on so isn't needed to be sent
		position.x = transform.position.x;
		position.y = transform.position.z;
		Vector3 velocity3d = new Vector3(_rvoController.newVelocity.x, 0f, _rvoController.newVelocity.z);

		Vector3 positionToDestinationXZ = GameUtils.SubXZ(_locomotion.Destination, transform.position);
		distToDestination = Mathf.Max(positionToDestinationXZ.magnitude - _locomotion.ArrivalThreshold, 0f);
		// check if our current rvo controller velocity is gonna make us overshoot the target
		if (velocity3d.magnitude * GetNetworkUpdateInterval() > distToDestination)
		{ // set our speed so we do not overshoot
			velocity3d = velocity3d.normalized * (distToDestination / GetNetworkUpdateInterval());
		}

		const float DirectionTolerance = 0.819f; // 0.819f == cos(35 degrees)
		const float StoppingTime = 4f; // seconds
		// if the character is not heading towards the destination, or the destination is far away then it's not gonna stop soon and the distance can be unlimited
		if (GameUtils.DotXZ(velocity3d.normalized, positionToDestinationXZ.normalized) < DirectionTolerance || // character is not traveling towards target (probably RVO related)
			distToDestination > velocity3d.magnitude * GetNetworkUpdateInterval() * StoppingTime) // the destination is far away, so no stop is imminent
		{
			distToDestination = NetworkTransform.DistanceUnlimited;
		}

		// this section makes sure our velocity would not take us off the nav mesh
		Vector3 from = transform.position;
		Vector3 targetPosition = AStarPathfindingUtils.CalculateExitPoint(ref from, from + velocity3d, _locomotion.Simulator, true); // see if the target poition would cause us to exit the nav mesh

		float distToOriginalTargetSqr = GameUtils.GetDistSqXZ(from, from + velocity3d);
		float distToNewTargetSqr = GameUtils.GetDistSqXZ(from, targetPosition);
		if (distToNewTargetSqr < distToOriginalTargetSqr) // if the returned length is nearer, it means we hit an edge
		{
			const float NudgeIntoNavMeshDist = 0.1f;
			if (distToNewTargetSqr > NudgeIntoNavMeshDist * NudgeIntoNavMeshDist)
			{
				float distToNewTarget = Mathf.Sqrt(distToNewTargetSqr);
				// here we're moving targetPosition slightly back onto the nav mesh away from the nav mesh perimeter, this is to stop the character being moved slightly off mesh
				targetPosition = from + (((targetPosition - from) / distToNewTarget) * (distToNewTarget - NudgeIntoNavMeshDist));
			}
			else
			{
				targetPosition = from; // edge is very close, so just use the from position as the target position
			}
			velocity3d = targetPosition - transform.position;
		}
		velocity.x = velocity3d.x;
		velocity.y = velocity3d.z;
		
		if (!NetworkTransform.IsMovement(new Vector3(velocity3d.x, 0f, velocity3d.z))) // is no movement being sent
		{ // if we're sending a velocity of zero, we can have distance unlimited because there will be no movement anyway, so having distance unlimited means it can start moving quickly when a movement begins
			distToDestination = NetworkTransform.DistanceUnlimited;
		}
	}

	public void Update()
	{ // calculate how many network transforms we should keep around
		int numUpdatesPerSecond = (int)((1f/GetNetworkUpdateInterval())+1f);
		_numNetworkTransformHistory = Mathf.Max(MinNetworkTransformHistoryTime, MinNetworkTransformHistoryTime*numUpdatesPerSecond);

		_isCurrentlyLocal = AmILocal();
		_currentOwnerId = _networkLocomotionComponentViewRPC.ownerId;

#if DEBUG
		if (fakeLocalDebugging)
		{
			if (Time.time - _lastFakeLocalUpdateTime >= GetNetworkUpdateInterval())
			{
				_lastFakeLocalUpdateTime = Time.time;

				Vector2 position = Vector2.zero;
				Vector2 velocity = Vector2.zero;
				float distToDestination = NetworkTransform.DistanceUnlimited;
				CalculatePositionAndVelocityToSend(ref position, ref velocity, ref distToDestination);

				float yaw = transform.rotation.eulerAngles.y;
				
				//fixme: xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
				double networkTime = System.DateTime.Now.Second;
				// double networkTime = Network.time;

				Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
				SetTransformInterpolated(position, velocity, rotation, distToDestination, networkTime);
			}
			NetworkUpdate();
		}
#endif		
	}

	// we're starting to interpolate network positions again
	public void Reset()
	{
#if DEBUG
		if (!loopMode)
#endif
		{
			_previousHostPosition = _hostPosition = transform.position;
		}

		ClearNetworkTransforms(); // clearing our history means we have nothing to interpolate between, causing a teleport
		Vector2 position2d = new Vector2(transform.position.x, transform.position.z);
		SetTransformInterpolated(position2d, new Vector2(0f, 0f), transform.rotation, 0f, -double.MaxValue);
		EnableNetworkSynchronization(true);
	}

#if DEBUG
	public void OnPreviousValuesLoaded()
	{
		loopMode = false;
		fakeLocalDebugging = false;
	}

	public void OnValueChanged(System.Reflection.FieldInfo field, object oldValue, object newValue)
	{
	}

	public void OnDebugGUI()
	{
	}

	public void OnDebugPanelGUI()
	{
	}

	public void OnDrawDebug()
	{
		if (!drawDebug || (AmILocal() && !fakeLocalDebugging))
		{
			return;
		}

		const float SphereRad = 0.3f;
		GLRenderingUtils.DoDrawSphere(_hostPosition, SphereRad * 0.3f, Color.green);
		const float AxisScale = 1.5f;
		GLRenderingUtils.DoDrawScaledAxes(_hostRotation, _hostPosition, AxisScale);

		int trans = Mathf.Max(0, _networkTransforms.Count - maxNetworkTransformsToDraw); // don't draw more network transforms than required
		for (; trans < _networkTransforms.Count; ++trans)
		{
			if (!drawFutureUpdatesInLoopMode && loopMode && CalculateLoopModeTime() < _networkTransforms[trans].time)
			{
				break;
			}

			NetworkTransform temp = _networkTransforms[trans];

			GLRenderingUtils.DoDrawSphere(temp.position, SphereRad, Color.red); // draw the position of this network transform

			GLRenderingUtils.DoDrawLine(temp.position, temp.position + temp.predictedVelocity * GetNetworkUpdateInterval(), Color.green); // draw a line from this network transform position to predicted velocity end
			GLRenderingUtils.DoDrawSphere(temp.position + temp.predictedVelocity * GetNetworkUpdateInterval(), SphereRad * 0.5f, Color.yellow); // draw a line at the end of the predicted velocity

			GLRenderingUtils.DoDrawSphere(temp.extrapolatedPosition, SphereRad * 0.5f, Color.blue); // where hs the position been extrapolated to upto now	
								
			if (trans > 0)
			{
				NetworkTransform prev = _networkTransforms[trans - 1];
				GLRenderingUtils.DoDrawLine(temp.position, prev.position, Color.white); // draw a line to the previous network update position
			}
		}		
	}
#endif

	// get the position we are interpolating to follow the host
	public Vector3 GetHostPosition()
	{
		Vector3 result = new Vector3(_hostPosition.x, transform.position.y, _hostPosition.z);
		return result;
	}

	public Vector3 CalculateHostVelocity()
	{
		if (Time.deltaTime > 0f)
		{
			return (_hostPosition - _previousHostPosition) / Time.deltaTime;
		}
		return Vector3.zero;
	}

	// add a new network transform
	public void SetTransformInterpolated(Vector2 position, Vector2 velocity, Quaternion rotation, float distToDestination, double networkTime)
	{		
		if (!allowNetworkUpdates || loopMode) // for debugging
		{
			return;
		}

		if (_networkTransforms.Count > 0 && GetMostRecentUpdate().networkTime > networkTime) // this checks if we are receiving an update which is older than our most recent
		{
			return; // we're not interested in out of order network updates
		}

		NetworkTransform temp = null;
		if (_networkTransforms.Count >= _numNetworkTransformHistory) // we only keep a history of size _numNetworkTransformHistory
		{
			temp = _networkTransforms[0];

			temp.isFirstUsage = true;
			temp.hasTransformBeenFullyBlendedIn = false;
			temp.havePredictionsBeenFullyCorrected = false;
			temp.isTransitionToNextReady = false;
			temp.hasDestinationBeenReached = false;
			
			// debugging
			temp.actualNextUpdateTime = -1f;
			temp.angleError = -999f;
			temp.positionalError = -1f;
			// debugging
		}
		else
		{
			temp = new NetworkTransform();
		}
		
		temp.time = Time.time;
		temp.predictedDistToDestination = temp.correctedDistToDestination = distToDestination;
		// we are unpacking the 2d vectors into 3d vectors, the y elements are actually the z elements - see OnSerializeView for packing
		temp.position = new Vector3(position.x, 0f, position.y);
		temp.predictedVelocity = temp.correctVelocity = new Vector3(velocity.x, 0f, velocity.y);
		temp.rotation = rotation;
		if (!_locomotion.ShouldIgnoreRotation) 
		{
			temp.correctedRotation = temp.predictedRotation = NetworkTransform.IsMovement(temp.predictedVelocity) ? Quaternion.LookRotation(temp.predictedVelocity.normalized) : rotation;
		}
		else 
		{
			temp.correctedRotation = temp.predictedRotation = rotation;
		}
		temp.networkTime = networkTime;

		temp.rotationPeriod = GetNetworkUpdateInterval();

		float notUsed = 0f;
		float positionBlendInTimeSetting = 0f;
		float rotationBlendInTimeSetting = 0f;
		CalculateBlendTimes(ref notUsed, ref positionBlendInTimeSetting, ref rotationBlendInTimeSetting);
		temp.positionBlendInTime = positionBlendInTimeSetting;
		temp.rotationBlendInTime = rotationBlendInTimeSetting;

		//  debugging ----------------------------------------------------------------------------------------------------------------------
		temp.extrapolatedPosition = temp.position;
		temp.expectedNextUpdateTime = temp.time + GetNetworkUpdateInterval();
		if (_networkTransforms.Count > 0)
		{
			_networkTransforms[_networkTransforms.Count - 1].actualNextUpdateTime = Time.time;
			Vector3 predictedPosition = _networkTransforms[_networkTransforms.Count - 1].position + _networkTransforms[_networkTransforms.Count - 1].predictedVelocity;
			_networkTransforms[_networkTransforms.Count - 1].positionalError = GameUtils.GetDistXZ(predictedPosition, temp.position);
			float dot = GameUtils.DotXZ((temp.position - _networkTransforms[_networkTransforms.Count - 1].position).normalized, _networkTransforms[_networkTransforms.Count - 1].predictedVelocity.normalized);
			_networkTransforms[_networkTransforms.Count - 1].angleError = Mathf.Rad2Deg * Mathf.Acos(dot);
		}
		// -----------------------------------------------------------------------------------------------------------------------------------

		int _networkTransformsCount = _networkTransforms.Count;
		if (_networkTransformsCount >= _numNetworkTransformHistory) // we only keep a history of size _numNetworkTransformHistory
		{
			for (int netTrans = 0; netTrans < _networkTransformsCount-1; ++netTrans)
			{
				_networkTransforms[netTrans] = _networkTransforms[netTrans + 1];
			}
			_networkTransforms[_networkTransformsCount - 1] = temp;
		}
		else
		{
			_networkTransforms.Add(temp);
		}		
	}

	public void NetworkUpdate()
	{
#if DEBUG
		DebuggingUpdate();
#endif
		_previousHostPosition = _hostPosition;
		_hostRotation = Quaternion.identity;
		NetworkInterpolationAndExtrapolation(ref _hostPosition, ref _hostRotation);

#if DEBUG
		if (!fakeLocalDebugging)
		{
#endif
			transform.rotation = _hostRotation;
#if DEBUG
		}
#endif
	}

	// called from replication view, frequency of call determined by update interval
	public void OnSerializeView(EB.BitStream bs)
	{
		Vector2 position = new Vector2();
		Vector2 velocity = new Vector2();
		float distToDestination = NetworkTransform.DistanceUnlimited;
			
		if (bs.isWriting)
		{
			//DebugSystem.Log("Writing Writing " + name + " " + Network.time + " " + _networkLocomotionComponentViewRPC.isMine);
			CalculatePositionAndVelocityToSend(ref position, ref velocity, ref distToDestination);
		}

		float yaw = transform.rotation.eulerAngles.y;
		
		//fixme: xxxxxxxxxxxxxxxxxxxxxxxxx
		double networkTime = System.DateTime.Now.Second;
		// double networkTime = Network.time;

		bs.Serialize(ref position.x);
		bs.Serialize(ref position.y);
		bs.Serialize(ref velocity.x);
		bs.Serialize(ref velocity.y);
		bs.Serialize(ref yaw);
		bs.Serialize(ref distToDestination);
		bs.Serialize(ref networkTime);

		if (bs.isReading)
		{
			//DebugSystem.Log("Reading Reading " + name + " " + Network.time + " " + _networkLocomotionComponentViewRPC.isMine);
			Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
			SetTransformInterpolated(position, velocity, rotation, distToDestination, networkTime);
		}			
	}

	// interpolate or extrapolate to approximate the host position
	public void NetworkInterpolationAndExtrapolation(ref Vector3 setPosition, ref Quaternion setRotation)
	{
		if (0 == _networkTransforms.Count)
		{
			return; // we have received no network updates, there isn't much we can do yet.....
		}

		float currentTime = CalculateLoopModeTime(); // returns current time if not in loop mode

		float velocityBlendInTimeSetting = 0f;
		float positionBlendInTimeSetting = 0f;
		float rotationBlendInTimeSetting = 0f;
		CalculateBlendTimes(ref velocityBlendInTimeSetting, ref positionBlendInTimeSetting, ref rotationBlendInTimeSetting);

		float displayTime = currentTime + networkDisplayTime;
		for (int update = 0; update < _networkTransforms.Count - 1; ++update)
		{
#if DEBUG
			if (loopMode && currentTime < _networkTransforms[update + 1].time)
			{
				break; // we're in loop mode and we haven't got to this update yet
			}
#endif
			if (_networkTransforms[update].havePredictionsBeenFullyCorrected) // if we've already corrected the velocity, there's no more work to do
			{
				continue;
			}

			float distance = GameUtils.GetDistXZ(_networkTransforms[update].position, _networkTransforms[update + 1].position); // distance between this and the next update positions
			float timeDiff = _networkTransforms[update + 1].time - _networkTransforms[update].time; // time between the updates
			// calculate the velocity required to perfectly get between the two positions in the time between the transform updates
			Vector3 correctVelocity = (_networkTransforms[update + 1].position - _networkTransforms[update].position);
			if (timeDiff > 0f)
			{
				correctVelocity = correctVelocity.normalized * (distance / timeDiff);
			}

			if (displayTime <= _networkTransforms[update].time) // if we haven't yet started to consider this update yet, we can clobber the velocity rather than change it over time
			{
				_networkTransforms[update].correctedRotation = _networkTransforms[update + 1].rotation;
				_networkTransforms[update].correctVelocity = correctVelocity;
				_networkTransforms[update].rotationPeriod = timeDiff;
				_networkTransforms[update].havePredictionsBeenFullyCorrected = true; // before this is set to true, correctVelocity is somewhere between predictedVelocity and correctVelocity
			}
			else // change to the correct velocity over time to avoid any jumps in position
			{
				float lerpToCorrected = (currentTime - _networkTransforms[update + 1].time) / velocityBlendInTimeSetting;
				_networkTransforms[update].correctVelocity = Vector3.Lerp(_networkTransforms[update].predictedVelocity, correctVelocity, Mathf.Min(lerpToCorrected, 1f));
				if (lerpToCorrected >= 1f)
				{
					_networkTransforms[update].havePredictionsBeenFullyCorrected = true;
				}
				_networkTransforms[update].correctedRotation = Quaternion.Slerp(_networkTransforms[update].predictedRotation, _networkTransforms[update + 1].rotation, Mathf.Min(lerpToCorrected, 1f));

				if (timeDiff < _networkTransforms[update].rotationPeriod) // only reduce the time to get us rotated quicker, extending the time could cause a backward rotation
				{
					_networkTransforms[update].rotationPeriod = Mathf.Lerp(GetNetworkUpdateInterval(), timeDiff, Mathf.Min(lerpToCorrected, 1f));
				}
			}
		}

		if (displayTime > GetOldestUpdate().time)
		{
			Vector3 extrapolatedPosition = new Vector3();
			Quaternion extrapolatedRotation = new Quaternion();
			_networkTransforms[0].hasTransformBeenFullyBlendedIn = true;

			for (int update = 0; update < _networkTransforms.Count; ++update)
			{
				if (displayTime < _networkTransforms[update].time)
				{
					break;
				}

				const float ArriveBefore = 0.85f;
				if (_networkTransforms[update].isFirstUsage)
				{
					_networkTransforms[update].isFirstUsage = false;

					if (_networkTransforms[update].havePredictionsBeenFullyCorrected) // this means we already have the update following this one
					{ // make sure we're fully blended in before we get to the next update
						_networkTransforms[update].positionBlendInTime = _networkTransforms[update].rotationBlendInTime = (_networkTransforms[update + 1].time - _networkTransforms[update].time) * ArriveBefore;
					}
					else
					{
						_networkTransforms[update].positionBlendInTime = positionBlendInTimeSetting;
						_networkTransforms[update].rotationBlendInTime = rotationBlendInTimeSetting;
					}
				}

				// rotation
				float extrapolationTime = displayTime - _networkTransforms[update].time;
				float rotationExtrapolationPeriod = Mathf.Min(_networkTransforms[update].rotationPeriod * ArriveBefore, _networkTransforms[update].rotationBlendInTime); // we should be rotated fully into projected rotation before we get the next network update
				Quaternion toRotationExtrapolated = Quaternion.Slerp(_networkTransforms[update].rotation, _networkTransforms[update].correctedRotation, Mathf.Min(extrapolationTime / rotationExtrapolationPeriod, 1f));

				float rotationLerp = _networkTransforms[update].hasTransformBeenFullyBlendedIn ? 1f : extrapolationTime / _networkTransforms[update].rotationBlendInTime;
				extrapolatedRotation = Quaternion.Slerp(extrapolatedRotation, toRotationExtrapolated, Mathf.Min(rotationLerp, 1f));

				// position	extrapolation			
				float positionLerp = _networkTransforms[update].hasTransformBeenFullyBlendedIn ? 1f : extrapolationTime / _networkTransforms[update].positionBlendInTime;
				if (positionLerp >= 1f)
				{
					_networkTransforms[update].hasTransformBeenFullyBlendedIn = true; // rotation lerp time is less than position lerp time, so if position has been fully blended in, rotation must also have
				}

				// this checks whether there is an update after this one which is about to be used for the first time
				if (update + 1 < _networkTransforms.Count && _networkTransforms[update + 1].isFirstUsage && displayTime >= _networkTransforms[update + 1].time)
				{ // check whether this udate has been fully blended in and corrected (and not had it's motion stopped by reaching destination), meaning it will meet up perfectly wth the next update
					if (_networkTransforms[update].havePredictionsBeenFullyCorrected && _networkTransforms[update].hasTransformBeenFullyBlendedIn && !_networkTransforms[update].hasDestinationBeenReached)
					{						
						_networkTransforms[update + 1].hasTransformBeenFullyBlendedIn = true;
						_networkTransforms[update].isTransitionToNextReady = true; // we will interpolate to next perfectly, so don't need to extrapolate past it	
						_networkTransforms[update].correctedDistToDestination = NetworkTransform.DistanceUnlimited; // we know we will meet up with next perfectly, so no need to cap anything
					}									
				}

				// isTransitionToNextReady will only be true if we already have the next update (so update + 1 is safe)
				extrapolationTime = _networkTransforms[update].isTransitionToNextReady ? _networkTransforms[update + 1].time - _networkTransforms[update].time : extrapolationTime;
				Vector3 movement = (_networkTransforms[update].correctVelocity * extrapolationTime);

				if (!NetworkTransform.IsDistanceUnlimited(_networkTransforms[update].correctedDistToDestination) &&
					GameUtils.Square(_networkTransforms[update].correctedDistToDestination) < movement.sqrMagnitude)
				{
					movement = movement.normalized * _networkTransforms[update].correctedDistToDestination;
					_networkTransforms[update].hasDestinationBeenReached = true;
				}
				if (!_networkTransforms[update].isTransitionToNextReady && IsStopAhead(update + 1, displayTime)) // we are not perfectly aligned to transition to the next update and we're stopping ahead
				{
					_networkTransforms[update].hasDestinationBeenReached = true; // let's stop moving seeings as there is a stop ahead (this makes sure we don not extrapolate past the stop)
					_networkTransforms[update].correctedDistToDestination = movement.magnitude;
				}

				Vector3 toPositionExtrapolated = _networkTransforms[update].extrapolatedPosition = (_networkTransforms[update].position + movement);

				// lerp to our most recent extrapolated position
				extrapolatedPosition = Vector3.Lerp(extrapolatedPosition, toPositionExtrapolated, Mathf.Min(positionLerp, 1f));
			}
			setPosition = extrapolatedPosition;
			setRotation = extrapolatedRotation;
		}
		else
		{
			setPosition = GetOldestUpdate().position;
			setRotation = GetOldestUpdate().rotation;
		}
	}

	protected void OnEnable()
	{
#if DEBUG
		if (PlayerManager.IsPlayer(gameObject))
		{
			//DebugSystem.RegisterInstance("NetworkLocomotion " + name, this, "NetworkLocomotion");
		}
		else
		{
			//DebugSystem.RegisterInstance("NetworkLocomotion " + name, this, "NetworkLocomotion", false);
		}
#endif
		// after an enemy respawns, clearing the network transform history and calculated host position, stops the charcter lerping from it's old stale position
		ClearNetworkTransforms();
		_previousHostPosition = _hostPosition = transform.position;

		_isCurrentlyLocal = AmILocal();
		_currentOwnerId = _networkLocomotionComponentViewRPC.ownerId;
	}

	protected void OnDisable()
	{
#if DEBUG
		//DebugSystem.UnregisterSystem(this);
#endif
	}

	// do we want to do anything when our owner changes?
	void OnTransferedOwnership()
	{
#if DEBUG
		if (loopMode)
		{
			return;
		}
#endif
		if (_currentOwnerId == _networkLocomotionComponentViewRPC.ownerId)
		{
			return;
		}

		bool isNewOwnerLocal = AmILocal();
		if (_isCurrentlyLocal && !isNewOwnerLocal) // in the case we are going from local to external control, we want to make sure the character keeps moving smoothly
		{
			_previousHostPosition = _hostPosition = transform.position; // clear any stale values
			Vector2 position2d = new Vector2();
			Vector2 velocity2d = new Vector2();
			float distToDestination = 0f;
			CalculatePositionAndVelocityToSend(ref position2d, ref velocity2d, ref distToDestination);
			ClearNetworkTransforms(); // clearing our history as anything in there will be stale
			// fake a network update else the character will stop moving   
			SetTransformInterpolated(position2d, velocity2d, transform.rotation, distToDestination, -double.MaxValue); // set the update time to as old as possible, so the new owners updates will not be rejected
		}
		else if (!isNewOwnerLocal && _networkTransforms.Count > 0) // previous owner was not local and this new owner is also not local (and we received an update from the previous owner)
		{
			GetMostRecentUpdate().networkTime = -double.MaxValue; // setting the most recent update time to this means the new owners updates will not be rejected due to timing issues
		}
		_isCurrentlyLocal = isNewOwnerLocal;
		_currentOwnerId = _networkLocomotionComponentViewRPC.ownerId;
	}

	// looks to see if updates are coming to a hard stoppage ahead
	private bool IsStopAhead(int update, float displayTime)
	{
		for (; update < _networkTransforms.Count; ++update)
		{
			NetworkTransform netTrans = _networkTransforms[update];
			if (displayTime < netTrans.time)
			{
				break;
			}

			// this update has reached its destination
			if (netTrans.hasDestinationBeenReached || !NetworkTransform.IsMovement(netTrans.correctVelocity))
			{
				return true;
			}
		}
		return false;
	}

	private void ClearNetworkTransforms()
	{
#if DEBUG
		if (loopMode) // loopMode is a debugging state where we are looking at our history, so we don't want anything deleted
		{
			return;
		}
#endif
		_networkTransforms.Clear();		
	}

#if DEBUG
	private void DebuggingUpdate()
	{
		if (!_wasLoopMode && loopMode && _networkTransforms.Count > 0) // just gone into loop mode, so set some loop mode state variables
		{
			_loopModeEndTime = Time.time;
			_loopModeStartTime = GetOldestUpdate().time;
			_howManyTimesHasLoopModeLooped = -1;
		}
		_wasLoopMode = loopMode;

		if (loopMode)
		{
			int prevNumPeriods = _howManyTimesHasLoopModeLooped;
			float loopModePeriod = _loopModeEndTime - _loopModeStartTime;
			_howManyTimesHasLoopModeLooped = (int)((Time.time - _loopModeStartTime) / loopModePeriod);
			if (_howManyTimesHasLoopModeLooped > prevNumPeriods) // we are restarting a loop
			{
				for (int i = 0; i < _networkTransforms.Count; ++i)
				{
					_networkTransforms[i].correctVelocity = _networkTransforms[i].predictedVelocity;
					_networkTransforms[i].correctedRotation = _networkTransforms[i].predictedRotation;

					_networkTransforms[i].isFirstUsage = true;
					_networkTransforms[i].hasTransformBeenFullyBlendedIn = false;
					_networkTransforms[i].havePredictionsBeenFullyCorrected = false;
					_networkTransforms[i].correctedDistToDestination = _networkTransforms[i].predictedDistToDestination;
					_networkTransforms[i].hasDestinationBeenReached = false;

					_networkTransforms[i].isTransitionToNextReady = false;
					_networkTransforms[i].extrapolatedPosition = _networkTransforms[i].position;

					_networkTransforms[i].rotationPeriod = GetNetworkUpdateInterval();					

					float notUsed = 0f;
					float positionBlendInTimeSetting = 0f;
					float rotationBlendInTimeSetting = 0f;
					CalculateBlendTimes(ref notUsed, ref positionBlendInTimeSetting, ref rotationBlendInTimeSetting);
					_networkTransforms[i].positionBlendInTime = positionBlendInTimeSetting;
					_networkTransforms[i].rotationBlendInTime = rotationBlendInTimeSetting;
				}
			}
		}
	}
#endif

	private NetworkTransform GetMostRecentUpdate()
	{
		return _networkTransforms[_networkTransforms.Count - 1]; // the most recent update is on the end of the list
	}

	private NetworkTransform GetOldestUpdate()
	{
		return _networkTransforms[0]; // the oldest update is at the start of the list
	}

	// if we're in debug loop mode, this calculates the current time in the loop
	private float CalculateLoopModeTime()
	{
		float currentTime = Time.time;
#if DEBUG
		if (loopMode)
		{
			float loopModePeriod = _loopModeEndTime - _loopModeStartTime;
			currentTime -= (loopModePeriod * ((float)_howManyTimesHasLoopModeLooped));
		}
#endif
		return currentTime;
	}

	private void CalculateBlendTimes(ref float velocityBlendTime, ref float positionBlendTime, ref float rotationBlendTime)
	{
		const float PreferedBlendTime = 0.5f;

		velocityBlendTime = PreferedBlendTime;
		positionBlendTime = PreferedBlendTime;
		rotationBlendTime = PreferedBlendTime;

		if (velocityBlendMult > 0f)
		{
			velocityBlendTime = velocityBlendMult * GetNetworkUpdateInterval(); 
		}
		if (positionBlendMult > 0f)
		{
			positionBlendTime = positionBlendMult * GetNetworkUpdateInterval(); 
		}
		if (rotationBlendMult > 0f)
		{
			rotationBlendTime = rotationBlendMult * GetNetworkUpdateInterval(); 
		}
		// rotation should be blended in before or at same point as position (so everything meets up)
		rotationBlendTime = Mathf.Min(rotationBlendTime, positionBlendTime);
	}

	// returns how long there is between sending transform updates
	private float GetNetworkUpdateInterval()
	{
		const float MinNetworkUpdateInterval = 0.05f;
		return Mathf.Max(MinNetworkUpdateInterval, _networkLocomotionComponentViewRPC.updateInterval);
	}
}
