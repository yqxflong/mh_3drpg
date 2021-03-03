///////////////////////////////////////////////////////////////////////
//
//  LocomotionComponentAPP.cs (AStar Pathfinding Pro Project)
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
using Pathfinding;
using Pathfinding.RVO;

public class LocomotionComponentAPP : BaseComponent, ILocomotionComponent
#if DEBUG
	, IDebuggableEx
#endif
{
	public delegate void OnNewDestinationDelegate();
	public delegate void OnStopDelegate();

	// a callback for when a new destination has been sought
	public OnNewDestinationDelegate onNewDestination = null;
	// a callback for when we stop
	public OnStopDelegate onStop = null;

	/* when dragging the player around, if the length of the player's path increases by more than this amount, a new path will not be generated and the player will walk directly to the target point. 
	 this is because it's assumed that the target point has gone (accidentally on the player's part) to another part of the nav mesh (walking directly will usually result in the player being blocked 
	 by the nav mesh edge) */
	public float maxPathIncrease = 40.0f;
	public float searchAgainTol = 1.5f; // how much our position or the target position must change to perform another search
	public bool useCalculateVelocityOnSightOnly = true;
	public bool tryToAddToExistingPaths = false; // based on the values of pathChangeDirectionDegrees and pathChangeDirectionDistancePercent, try to add points to an exisiting path 
	public float pathChangeDirectionDegrees = 180f; // if the new path requires a direction change of more than this
	public float pathChangeDirectionDistancePercent = 100000f; // if the new path is more than this percentage of the distance of the current path 

	public bool drawDebug = false;
	public bool recordPositionHistory = false;
	public bool drawPositionHistory = false;
	public float positionHistoryDrawScale = 1.0f;
	public bool alwaysStraightLine = false;
	public int maxPositionHistory = 30 * 5; // 30 fps for 5 seconds
	private Vector3 _unmodifiedDestination; // the destination we were told to go to before clamping it to the nav mesh
	
	private float _distanceToTargetSqr;
	private Vector3 _destination;
	private Vector3 _destinationOffset;
	private bool _didTouchBegin = false;
	private bool _isTouchInProgress = false; 
	private bool _mostRecentPathWasTooLong = false;
	private float _touchDistance = -1.0f;
	private Vector3 _previousPosition;
	private bool _doStraightLineMode = false;
	private bool _ignorePendingPaths = false;
	private Vector3 _lastSearchFromPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
	private Vector3 _lastSearchTargetPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

	private float _RVONeighbourDist = 3f;
	private float _RVOObstacleTimeHorizon = 0.2f;
	private float _RVORadiusAgainstObstacles = 0.2f;

	private bool _isAvoiding = true;

	/** Reference to the rvo _simulator */
	private Simulator _simulator;
	public Simulator Simulator
	{
		get
		{
			return _simulator;
		}
	}

	/** Determines how often it will search for new paths. 
	 * If you have fast moving targets or AIs, you might want to set it to a lower value.
	 * The value is in seconds between _path requests.
	 */
	private float _repathRate = 0.5F;

	/** Enables or disables searching for paths.
	 * Setting this to false does not stop any active _path requests from being calculated or stop it from continuing to follow the current _path.
	 * \see #_canMove
	 */
	private bool _canSearch = true;

	/** Enables or disables movement.
	  * \see #_canSearch */
	private bool _canMove = false;

	/** Distance from the target point where the AI will start to slow down.
	 * Note that this doesn't only affect the end point of the _path
	 * but also any intermediate points, so be sure to set #forwardLook and #_pickNextWaypointDist to a higher value than this
	 */
	private float _slowdownDistance = 1.5f;

	/** Determines within what range it will switch to target the next waypoint in the _path */
	private float _pickNextWaypointDist = 2;

	/** Distance to the end point to consider the end of _path to be reached.
	 * When this has been reached, the AI will not move anymore until the target changes and OnTargetReached will be called.
	 */
	private float _endReachedDistance = 0.2f;

	/** Cached Seeker component */
	private Seeker _seeker;

	/** Time when the last _path request was sent */
	private float _lastRepath = -9999;

	/** Current _path which is followed */
	private Path _path;

	private RVOController _rvoController;

	/** Current index in the _path which is current target */
	private int _currentWaypointIndex = 0;

	/** Holds if the end-of-_path is reached
	 * \see TargetReached */
	private bool _targetReached = false;

	/** Only when the previous _path has been returned should be search for a new _path */
	private bool _canSearchAgain = true;

	/** Point to where the AI is heading.
	 * Filled in by #CalculateVelocity */
	private Vector3 _targetPoint;

	/** Relative direction to where the AI is heading.
	 * Filled in by #CalculateVelocity */
	private Vector3 _targetDirection;

	/** If we're not pathing and this variable is not null, we will rotate towards that position */
	private Vector3? _lookAtPosition;

	// If we're rotating towards looking at _lookAtPosition, clear the value of _lookAtPosition when we become aligned with it
	private bool _clearLookAtPositionWhenAligned = false;

	/** Maximum velocity.
	 * This is the maximum _speed in world units per second.
	 */
	private float _speed = 3;

	private ReplicationView _viewRPC; // networking remote procedure calls

	private NetworkLocomotionComponent _networkLocomotion = null;

	// do we want to control the character locally, regardless of whether the character is not local
	private bool _overrideHostControl = false;

	private bool _sendExtraordinaryNetworkUpdateOnNextMovement = false;	// this sends an extra network update the next time we move (this ensures starting movement is not delayed)

	private bool _checkforArrivalInStraightLineMode = true; // whether we want to check the arrival conditions during straight line mode or let it be controlled externally

	private const float MaxDistanceForRotation = 0.5f; // if we are any closer than this, we will not reorient the character to face the target (avoids oscilation when close)

	private bool _isAlreadyBeingRotated = false;

#if DEBUG
	private class FrameInfo
	{
		public Vector3 position = new Vector3();
		public float distToPrev = -1f;
		public float delta = 0f;
		public float fps = 0f;
		public float averageFps = 0f; 
	}
	private List<FrameInfo> positionHistory = null; // for debugging, has the history of player positions
#endif

	public float MaxSpeed
	{
		get
		{
			return _speed;
		}
		
		set
		{
			_speed = value;
			AngularSpeed = value; // for now we're gonna set the turning _speed to the same as the linear _speed
			if (null != _rvoController)
			{
				_rvoController.maxSpeed = value;
			}
		}
	}	

	public float CurrentSpeed
	{
		get
		{
			return Velocity.magnitude;         
		}
	}

	public Vector3 TargetDirection
	{
		get
		{
			return _targetDirection;
		}
	}

	public float AngularSpeed
	{
		get;
		set;
	}

	public bool ShouldIgnoreRotation
	{
		get; set;
	}

	public bool PathPending
	{
		get
		{
			return _canSearch || !_canSearchAgain;
		}
	}

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		
		set
		{
			enabled = value;
		}
	}	

	public Vector3 Velocity
	{
		get
		{
			if (!IsCurrentlyLocallyControlled() && _networkLocomotion != null)
			{
				return _networkLocomotion.CalculateHostVelocity();
			}

			return (null != _rvoController) ? _rvoController.velocity : Vector3.zero;   
		}
	}

	public float RemainingDistance 
	{   
		get
		{
			if (!_canMove)
			{
				return 0.0f;
			}

			if (_doStraightLineMode)
			{
				return float.MaxValue; // if we're walking in a straight line to nowhere, we won't ever get their
			}
			return CalculateDistanceOnPath(_path, _currentWaypointIndex, transform.position);		   
		}
	}
   
	public float ArrivalThreshold
	{
		get
		{
			return _endReachedDistance;
		}
	}
	
	public Vector3 Destination
	{
		get
		{
			return _destination;
		}
		
		set
		{
			/*if(GetComponent<PlayerController>()!=null)
			{
				EB.Debug.LogError("LocomotionComponentAPP::Destination change!" + value.ToString());
			}*/

			if (null != onNewDestination)
			{
				onNewDestination();
			}

			if (null == AstarPath.active || null == AstarPath.active.graphs || 0 == AstarPath.active.graphs.Length)
			{
				EB.Debug.LogError("LocomotionComponentAPP::Destination No nav msh exists!");
				OnTargetReached();
				return;
			}

			if (PlayerManager.IsLocalPlayer(gameObject) && _didTouchBegin)
			{
				_didTouchBegin = false;
				_touchDistance = -1.0f; // we have not got an original touch distance for this touch yet
				_mostRecentPathWasTooLong = false;
				_doStraightLineMode = false;
			}

			const float MovementPadding = 0.1f;
			float paddingAmt = Mathf.Max(_endReachedDistance, MaxDistanceForRotation) + MovementPadding;

			Vector3 modifiedDestination = value; // just here for debug rendering
#if DEBUG
			_unmodifiedDestination = value;
#endif

			Vector3 diffToDestXZ = GameUtils.SubXZ(modifiedDestination, transform.position);
			// this is here to stop the player jittering if we are continually picking targets very close by
			if (diffToDestXZ.sqrMagnitude <= GameUtils.Square(paddingAmt))
			{
				modifiedDestination = transform.position + _destinationOffset * paddingAmt;				
			}

			const float NudgeIntoNavMesh = 0.1f;
			_destination = AStarPathfindingUtils.CalculatePointOnNavMesh(modifiedDestination, _simulator, NudgeIntoNavMesh);
			UpdateDestinationOffset();
			_canSearch = true; // we've got a different target, so let's allow searching again
			_ignorePendingPaths = false;
		}
	}

	// look at the passed in position
	public bool IsAlreadyBeingRotated()
	{
		return _isAlreadyBeingRotated;
	}

	// look at the passed in position
	public void ExclusivelyRotate(bool exclusive)
	{
		_isAlreadyBeingRotated = exclusive;
	}

	// look at the passed in position
	public void LookAtPosition(Vector3? lookAt, bool clearWhenAligned)
	{
		if (IsAlreadyBeingRotated())
		{
			return;
		}

		_lookAtPosition = lookAt;
		_clearLookAtPositionWhenAligned = clearWhenAligned;
	}

	// don't want to look at our look at position anymore
	public void ClearLookAtPosition()
	{
		if (IsAlreadyBeingRotated())
		{
			return;
		}

		_lookAtPosition = null;
	}

	// do we want local regardless of whether the character is not local
	public void OverrideHostControl(bool doOverride)
	{
		_overrideHostControl = doOverride;
	}

	// use this function to locomote directly to destination in a straight line (no pathfinding)
	public void GoStraightLineToDestination(Vector3 destination)
	{
		_canMove = true;
		_destination = destination;
		UpdateDestinationOffset();
		_doStraightLineMode = true;
		_checkforArrivalInStraightLineMode = false;
		_ignorePendingPaths = true;
		_targetReached = false;
	}

	// do we want all avoidance off or on
	public void TurnOnAvoidance(bool on)
	{
		if (null != _rvoController && on != _isAvoiding)
		{
			_isAvoiding = on;
			if (on)
			{
				_rvoController.neighbourDist = _RVONeighbourDist;				
				_rvoController.obstacleTimeHorizon = _RVOObstacleTimeHorizon;
				_rvoController.radiusAgainstObstacles = _RVORadiusAgainstObstacles;
			}
			else
			{
				_rvoController.neighbourDist = 0f;
				_rvoController.obstacleTimeHorizon = 0f;
				_rvoController.radiusAgainstObstacles = 0f;
			}
		}
	}



	/** Tries to search for a _path.
	 * Will search for a new _path if there was a sufficient time since the last repath and both
	 * #_canSearchAgain and #_canSearch are true.
	 * Otherwise will start WaitForPath function.
	 */
	public void TrySearchPath()
	{
		if (Time.time - _lastRepath >= _repathRate && _canSearchAgain && _canSearch)
		{
			SearchPath(false);
		}

	}


	// this function is called when we start to search for a _path
	private void SearchPath(bool imediate)
	{
		_canSearch = false; // we don't want to search again until we get a different target

		float searchTolSqr = GameUtils.Square(searchAgainTol);
		// this section stops us performing another search if the input parameters are exactly the same as last time
		if (GameUtils.GetDistSqXZ(_lastSearchFromPosition, transform.position) < searchTolSqr &&
			GameUtils.GetDistSqXZ(_lastSearchTargetPosition, _destination) < searchTolSqr)
		{
			return;
		}

		_lastSearchFromPosition = transform.position;
		_lastSearchTargetPosition = _destination;

		_lastRepath = Time.time;
		_canSearchAgain = false;

		if (imediate)
		{
			Path p = _seeker.StartPath(GetFeetPosition(), _destination);
			AstarPath.WaitForPath(p);
		}
		else
		{
			_seeker.StartPath(GetFeetPosition(), _destination);
		}
	}

	// this function is called when we acquire a _path, which we then follow
	public void OnPathComplete(Path _p)
	{
		if (_ignorePendingPaths)
		{
			_canSearchAgain = true;
			return;
		}

		ABPath p = _p as ABPath;
		if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special _path types");

		if (tryToAddToExistingPaths && !_doStraightLineMode && _touchDistance >= 0f && _p.vectorPath.Count > 0)
		{
			Vector3 currentDestination = _path.vectorPath[_path.vectorPath.Count - 1]; // can't use _destination as that may have already been reset since the path search was requested
			Vector3 newDestination = _p.vectorPath[_p.vectorPath.Count - 1];

			Vector3 dirToTargetPoint = GameUtils.SubXZ(_p.vectorPath[_p.vectorPath.Count > 1 ? 1 : 0], transform.position).normalized;
			// check the normalize succeeded and that the direction is changing by a significant amount
			if (dirToTargetPoint.sqrMagnitude > GameUtils.Square(0.5f) && GameUtils.DotXZ(dirToTargetPoint, transform.forward) < Mathf.Cos(pathChangeDirectionDegrees)) // big direction change
			{
				float distanceOnCurrentPath = GameUtils.SubXZ(currentDestination, newDestination).magnitude + CalculateDistanceOnPath(_path, _path.vectorPath.Count > 1 ? 1 : 0, transform.position);
				float distanceOnNewPath = CalculateDistanceOnPath(_p, _p.vectorPath.Count > 1 ? 1 : 0, transform.position);

				if (distanceOnCurrentPath > 0f && distanceOnNewPath / distanceOnCurrentPath > pathChangeDirectionDistancePercent) // not too much of a distance win
				{
					if (AStarPathfindingUtils.IsVisible(currentDestination, newDestination, _simulator))
					{
						_path.vectorPath.Add(newDestination);
						_canSearchAgain = true;
						return;
					}
				}
			}
		}

		//Release the previous _path
		if (_path != null) _path.Release(this);

		//Claim the new _path
		p.Claim(this);

		//Replace the old _path
		_path = p;

		//Reset some variables
		_currentWaypointIndex = 0;
		_targetReached = false;
		_canSearchAgain = true;

		// if we are no longer touching the screen and we were in straight line mode when we released touch, don't start moving again
		if (PlayerManager.IsLocalPlayer(gameObject) && !_isTouchInProgress && _doStraightLineMode)
		{
			return;
		}

		// if this character is local and _canMove is false, it means we are currently not moving, and on receiving this path, we will start to move
		if (!_canMove && _networkLocomotion != null && _networkLocomotion.AmILocal())
		{
			_sendExtraordinaryNetworkUpdateOnNextMovement = true;			
		}

		_canMove = true;
		if (_path.vectorPath.Count > 1)
		{
			_currentWaypointIndex = 1; // it's better to set the current point to point one because in the time it takes to get the _path
			// back, the character may have moved past point zero, meaning they will turn around to walk back to it (which is undesirable)
		}

		_doStraightLineMode = false;
#if DEBUG
		if (alwaysStraightLine)
		{
			_doStraightLineMode = true;
		}
#endif
		if (PlayerManager.IsLocalPlayer(gameObject))
		{            
			_mostRecentPathWasTooLong = false;
			float pathDist = RemainingDistance;
			if (_touchDistance < 0.0f) // this is the first _path we have received back for this touch
			{
				_touchDistance = pathDist;
			}
			else if ((pathDist - _touchDistance) > maxPathIncrease) // our _path distance during the touch has changed significantly
			{
				_doStraightLineMode = true;
				_checkforArrivalInStraightLineMode = true;
				_mostRecentPathWasTooLong = true;
			}
			else
			{
				_touchDistance = pathDist;
			}
		}
	}

	// called when we have followed our _path and reached our destination
	private void OnTargetReached()
	{
		Stop();
		//EventManager.instance.Raise(new PathTargetReachedEvent(gameObject, Destination));
	}

	// was the most recent destination request accepted i.e it was not rejected because it was out of range
	public bool WasMostRecentDestinationRequestAccepted()
	{
		return !_mostRecentPathWasTooLong;
	}

	// a tap to the screen
	public void TouchTap()
	{
		_didTouchBegin = true;
	}

	// touch and hold started
	public void TouchStarted()
	{
		_didTouchBegin = true;
		_isTouchInProgress = true;
	}

	// touch and hold ended
	public void TouchEnded()
	{
		_isTouchInProgress = false;
		if (_doStraightLineMode)
		{
			Stop();			
		}
	}

	public void Stop()
	{
		if (null != onStop)
		{
			onStop();
		}

		//Should reset _destination value to current position.
		_destination = transform.position;
		_ignorePendingPaths = true;
		_canSearch = false;
		_canMove = false; // no need to move anymore, we have arrived

		// reset the values here so path searches are not rejected on account that the player and target positions haven't changed
		_lastSearchFromPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		_lastSearchTargetPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
	}

	// this will stop the player moving completely, removing all previous desired velocity
	public void HardStop()
	{
		Stop();
		if (null != _rvoController)
		{
			_rvoController.Move(Vector3.zero);
			_rvoController.Teleport(transform.position);
		}
	}

	public void Update()
	{
        if (!GameEngine.Instance.IsTimeToRootScene)
        {
            return;
        }
        TrySearchPath();

		Vector3 dir = Vector3.zero;
		_targetDirection = Vector3.zero;
		if (_canMove) 
		{
			if (_clearLookAtPositionWhenAligned && null != _lookAtPosition)
			{
				_lookAtPosition = null; // we're moving now, we're not interested in the look at direction
			}

			dir = CalculateVelocity(GetFeetPosition());

			if (!ShouldIgnoreRotation)
			{				
				//Rotate towards _targetDirection (filled in by CalculateVelocity)
				if (_targetDirection != Vector3.zero && _distanceToTargetSqr > GameUtils.Square(MaxDistanceForRotation))
				{
					bool notUsed = false;
					transform.rotation = RotateTowards(_targetDirection, transform.rotation, AngularSpeed, ref notUsed);
				}
			}
		}
		else if (_lookAtPosition != null)
		{
			bool isAligned = false;
			transform.rotation = RotateTowards(_lookAtPosition.Value - transform.position, transform.rotation, AngularSpeed, ref isAligned);
			if (isAligned && _clearLookAtPositionWhenAligned)
			{
				_lookAtPosition = null;
			}
		}

		if (_rvoController != null)
		{
			_rvoController.Move(dir);
		}
		else
		{
			transform.Translate(dir * Time.deltaTime, Space.World);
		}
		_previousPosition = transform.position;

		if (null == _rvoController)
		{
			OnPositionChanged();
		}

		if (!IsCurrentlyLocallyControlled() && _networkLocomotion != null) // if this is not a local character, we run code to match the host position
		{
			_networkLocomotion.NetworkUpdate();		
		}			
	}

	public void Initialize()
	{
		if (AstarPath.active == null)
		{
			EB.Debug.LogError("LocomotionComponentAPP::Initialize AstarPath was null!");
			return;
		}

		// reduce and smooth waypoints
		FunnelModifier funnel = gameObject.GetComponent<FunnelModifier>();
		if (null != funnel)
		{
			funnel.obstaclePadding = 1.0f;
		}

		_previousPosition = _destination = transform.position;
		// start with the _destination being different to the transform.position (this is because (_destination-position).normalized is used as a fallback in Destination property
		_destination += Vector3.right;
		UpdateDestinationOffset();
		Stop(); // this is here so that we are not moving until we get a direction
	}

	public void OnDestroy()
	{
		if (_path != null)
		{
			_path.Release(this);
		}
	}

	public bool HasLineOfSight(Vector3 to, GameObject ignore)
	{
		RaycastHit hit;
		Vector3 centerOffset = new Vector3(0.0f, 0.5f, 0.0f);
		// if we haven't hit anything, we have line of sight, if we hit something we can ignore, we still have line of sight
		return !Physics.Linecast(transform.position + centerOffset, to + centerOffset, out hit, 0xFFFFFF) || hit.collider.gameObject == ignore;
	}

	// is the character controlled localled at the moment
	public bool IsCurrentlyLocallyControlled()
	{
		return _overrideHostControl || (_networkLocomotion != null && _networkLocomotion.IsCurrentlyLocal);
	}

	public void OnPreviousValuesLoaded()
	{
#if DEBUG
		recordPositionHistory = false;
		alwaysStraightLine = false;
#endif
	}

	public void OnValueChanged(System.Reflection.FieldInfo field, object oldValue, object newValue)
	{
	}

	public void OnDrawDebug()
	{
#if DEBUG
		MainDebugDraw();
		PositionHistoryDebugDraw();
#endif
	}

	private const float PATH_END_THRESHOLD_SQ = 2.0f * 2.0f;

	public bool CanPathTo(Vector3 dest)
	{
		return CanPathTo(GetFeetPosition(), dest);
	}

	public bool CanPathTo(Vector3 start, Vector3 dest)
	{
		NNInfo startInfo = AstarPath.active.GetNearest(start);
		NNInfo destInfo = AstarPath.active.GetNearest(dest);

		if (startInfo.node == null || destInfo.node == null || !startInfo.node.Walkable || !destInfo.node.Walkable)
		{
			return false;
		}

		Vector3 closestPointOnNavMesh = AStarPathfindingUtils.CalculatePointOnNavMesh(dest, _simulator, 0f, destInfo);
		const float NavMeshPerimeterTol = 1.5f;
		// if we have a dest which is too far from the closest point on the nav mesh
		if (GameUtils.GetDistSqXZ(dest, closestPointOnNavMesh) > GameUtils.Square(NavMeshPerimeterTol))
		{
			return false; // the point is off the nav mesh
		}

		// if different regions which are not linked, we cannot path between them
		if (startInfo.node.Area != destInfo.node.Area && !AStarPathfindingAbilityBridge.IsLinked(startInfo.node.Area, destInfo.node.Area))
		{
			return false;
		}
		return true;
	}

	// do an imediate path search
	public void SetDestinationImediately(Vector3 to)
	{
		Destination = to;
		SearchPath(true);
	}

	// get a referance to our current path
	public void GetPathRef(ref List<Vector3> outPath)
	{
		if (_path == null || null == _path.vectorPath)
		{
			return;
		}
		outPath = _path.vectorPath;		
	}

#if DEBUG
	// draw all the positions in the position history
	protected void PositionHistoryDebugDraw()
	{
		if (!drawPositionHistory || null == positionHistory || 0 == positionHistory.Count)
		{
			return;
		}

		// the positions are scaled up for viewing
		Vector3 pivotPointOrig = positionHistory[positionHistory.Count / 2].position;
		pivotPointOrig.y += 1f;
		Vector3 pivotPointScaled = pivotPointOrig * positionHistoryDrawScale;

		for (int pos = 0; pos < positionHistory.Count - 1; ++pos)
		{
			// scale up the points, but keep them relative to the pivot point
			Vector3 thisPos = positionHistory[pos].position;
			thisPos.y += 1f;
			thisPos *= positionHistoryDrawScale;

			Vector3 offset = thisPos - pivotPointScaled;
			thisPos = pivotPointOrig + offset;

			Vector3 nextPos = positionHistory[pos + 1].position;            
			nextPos.y += 1f;
			nextPos *= positionHistoryDrawScale;

			offset = nextPos - pivotPointScaled;
			nextPos = pivotPointOrig + offset;

			GLRenderingUtils.DoDrawLine(thisPos, nextPos, Color.green);

			GLRenderingUtils.DoDrawSphere(thisPos, 0.1f, Color.yellow);
			GLRenderingUtils.DoDrawSphere(nextPos, 0.1f, Color.yellow);
		}				
	}

	protected void MainDebugDraw()
	{
		if (!drawDebug)
		{
			return;
		}
		
		if (_path != null && _path.vectorPath.Count > 1) // if we have a path
		{
			// draw all the points on the current path
			for (int point = 0; point < _path.vectorPath.Count - 1; ++point)
			{
				Vector3 thisPos = _path.vectorPath[point];
				Vector3 nextPos = _path.vectorPath[point + 1];
				thisPos.y += 1f;
				nextPos.y += 1f;

				GLRenderingUtils.DoDrawLine(thisPos, nextPos, Color.green);

				GLRenderingUtils.DoDrawSphere(thisPos, 0.1f, Color.yellow);
				GLRenderingUtils.DoDrawSphere(nextPos, 0.1f, Color.yellow);
			}

			// draw all the nodes on the current path
			for (int nodeIndex = 0; nodeIndex < _path.path.Count; ++nodeIndex)
			{
				TriangleMeshNode pathNode = _path.path[nodeIndex] as TriangleMeshNode;
				if (null != pathNode)
				{
					GLRenderingUtils.DoDrawSphere((Vector3)pathNode.position, 0.2f, Color.blue); // draw the center of the node being stood on
					for (int i = 0; i < 3; ++i)
					{
						int next = (i + 1) % 3;
						Vector3 one = (Vector3)pathNode.GetVertex(i);
						Vector3 two = (Vector3)pathNode.GetVertex(next);
						GLRenderingUtils.DoDrawLine(one, two, Color.blue);
					}					
				}
			}
		}

		GL.Begin(GL.LINES);
			GL.Color(Color.red); // draw in red a vertical line indicating our destination
			Vector3 from = Destination + new Vector3(0f, 2.0f, 0f);
			Vector3 to = Destination + new Vector3(0f, -2.0f, 0f);
			GL.Vertex(from); GL.Vertex(to);

			GL.Color(Color.yellow);
			GL.Vertex(_unmodifiedDestination + new Vector3(0f, 2.0f, 0f));
			GL.Vertex(_unmodifiedDestination + new Vector3(0f, -2.0f, 0f));

			// draw a line linking the destination to the unmodified destination
			GL.Color(Color.cyan);
			GL.Vertex(Destination + new Vector3(0f, 2.0f, 0f));
			GL.Vertex(_unmodifiedDestination + new Vector3(0f, 2.0f, 0f));

			GL.Color(Color.blue); // draw in blue a vertical line showing our player position
			GL.Vertex(transform.position + new Vector3(0f, 3.0f, 0f));
			GL.Vertex(transform.position + new Vector3(0f, -3.0f, 0f));

			Vector3 clampedPos = AStarPathfindingUtils.CalculatePointOnNavMesh(transform.position, _simulator);
			clampedPos.y = transform.position.y;
			GL.Color(Color.white);
			GL.Vertex(clampedPos + new Vector3(0f, 3.0f, 0f));
			GL.Vertex(clampedPos + new Vector3(0f, -3.0f, 0f));

			// draw the obstacles which the character is currently considering
			if (null != _rvoController)
			{
				List<Pathfinding.RVO.ObstacleVertex> obst = _rvoController.RvoAgent.NeighbourObstacles;
				for (int i = 0; i < obst.Count; i++)
				{
					GL.Color(Color.black);
					GL.Vertex(obst[i].position);
					GL.Vertex(obst[i].next.position);
				}
			}
		GL.End();
		
		// draw the velocity direction or a big sphere to indicate no velocity
		if (GameUtils.GetDistXZ(Vector3.zero, Velocity) == 0f)
		{
			GLRenderingUtils.DoDrawSphere(transform.position + new Vector3(0f, 4.0f, 0f), _rvoController.radius + 0.1f, Color.white);
		}
		else
		{
			const float VelocityVisualMaxLength = 2.0f;
			Vector3 VelocityScaled = (Velocity / MaxSpeed) * VelocityVisualMaxLength;
			VelocityScaled.y = 0f;
			GLRenderingUtils.DoDrawLine(transform.position + new Vector3(0f, 4f, 0f),
													transform.position + new Vector3(0f, 4f, 0f) + VelocityScaled, Color.cyan);
		}

		// draw our target movement direction
		if (GameUtils.GetDistSqXZ(Vector3.zero, _targetDirection) > 0f)
		{
			GLRenderingUtils.DoDrawLine(transform.position + new Vector3(0f, 4.5f, 0f),
													transform.position + new Vector3(0f, 4.5f, 0f) + _targetDirection.normalized * 2f, Color.white);
		}

		if (_canMove && null != _rvoController)
		{
			GLRenderingUtils.DoDrawSphere(Destination, _rvoController.radius, Color.white);
		}

		if (null != _rvoController)
		{
			GLRenderingUtils.DoDrawSphere(transform.position + new Vector3(0f, 4.0f, 0f), _rvoController.radius, Color.red);
		}

		NNInfo info = AstarPath.active.GetNearest(transform.position);
		TriangleMeshNode node = info.node as TriangleMeshNode;
		if (null != node)
		{
			GLRenderingUtils.DoDrawSphere((Vector3)info.node.position, 0.2f, Color.magenta); // draw the center of the node being stood on
			for (int i = 0; i < 3; ++i)
			{
				int next = (i + 1) % 3;
				Vector3 one = (Vector3)node.GetVertex(i);
				Vector3 two = (Vector3)node.GetVertex(next);
				GLRenderingUtils.DoDrawLine(one, two, Color.magenta);
			}			
		}
	}
#endif

	protected void Awake()
	{
		_viewRPC = FindReplicationViewForComponent<LocomotionComponentAPP>();

		_networkLocomotion = GetComponent<NetworkLocomotionComponent>();

		_seeker = GetComponent<Seeker>();
		//Make sure we receive callbacks when paths complete
		_seeker.pathCallback += OnPathComplete;

		_rvoController = GetComponent<RVOController>();
		if (null != _rvoController && _isAvoiding)
		{
			_RVONeighbourDist = _rvoController.neighbourDist;
			_RVOObstacleTimeHorizon = _rvoController.obstacleTimeHorizon;
			_RVORadiusAgainstObstacles = _rvoController.radiusAgainstObstacles;
		}
		
		if (!PlayerManager.IsPlayer(gameObject))
		{
			_repathRate = 0.3f; // ai doesn't need such imediate response, saves on computation
		}
		else
		{
			_repathRate = 0.0f; // this is required to be zero, so their is no delay in selecting a new _path
		}

		if (null != _rvoController)
		{
			TurnOnAvoidance(true);
			_rvoController.wallAvoidForce = 0f;			
		}
		MaxSpeed = _speed;
		Stop(); // this is here so that we are not moving until we get a direction

		_simulator = AStarPathfindingUtils.GetSimulator();
	}

	/** Starts searching for paths.
	 * \see OnEnable
	 * \see RepeatTrySearchPath
	 */
	protected void Start()
	{
		//_startHasRun = true;
		OnEnable();
	}

	/** Run at start and when reenabled.
	 * Starts RepeatTrySearchPath.
	 * 
	 * \see Start
	 */
	protected void OnEnable()
	{
		Stop();
		_canSearchAgain = true;

		if (null != _rvoController)
		{
			_rvoController.enabled = true;
			_rvoController.AddPreUpdateCallback(PreRVOUpdate);
			_rvoController.AddPositionChangedCallback(OnPositionChanged);
		}
		//if (_startHasRun)
		//{
		//	StartCoroutine(RepeatTrySearchPath());
		//}

#if DEBUG
		if (PlayerManager.IsPlayer(gameObject))
		{
			//DebugSystem.RegisterInstance(name, this, "Pathfinding");
		}
		else
		{
		   // DebugSystem.RegisterInstance(name, this, "Pathfinding", false);
		}
#endif
		EventManager.instance.AddListener<CinematicEvent>(OnCinematicEvent);
	}

	void OnDisable()
	{
#if DEBUG
		//DebugSystem.UnregisterSystem(this);
#endif
		if (null != _rvoController)
		{
			_rvoController.RemovePositionChangedCallback(OnPositionChanged);
			_rvoController.RemovePreUpdateCallback(PreRVOUpdate);
			_rvoController.Teleport(transform.position);
			_rvoController.enabled = false;
		}
		EventManager.instance.RemoveListener<CinematicEvent>(OnCinematicEvent);
	}

	// do we need to take any action due to cinematics playing
	private void OnCinematicEvent(CinematicEvent evt)
	{
		if (CinematicEvent.CinematicEventType.starting == evt.GetCinematicEventType()) // the cinematic is starting
		{
			if (PlayerManager.IsPlayer(gameObject))
			{
				if (!evt.GetSwitches().IsPlayerNavigationAllowedDuringCinematic())
				{
					Stop(); // this should stop the player moving if it was on the way to some far off destination
				}
			}
			else // AI  
			{
				if (!evt.GetSwitches().IsNormalNPCBehaviourAllowed())
				{
					Stop(); // this should stop the NPC moving towards its destination
				}
			}
		}
	}

	// update the destination offset assuming the new destination is not in the same position as the character
	private void UpdateDestinationOffset()
	{
		Vector3 newDestinationOffset = GameUtils.SubXZ(_destination, transform.position);
		newDestinationOffset.Normalize();
		if (newDestinationOffset.sqrMagnitude > GameUtils.Square(0.5f)) // if the normalize succeeded
		{
			_destinationOffset = newDestinationOffset;
		}
	}

	/** Rotates in the specified direction.
	 * Rotates around the Y-axis.
	 */
	private static Quaternion RotateTowards(Vector3 dir, Quaternion rot, float angularSpeed, ref bool isAligned)
	{
		Quaternion toTarget = Quaternion.LookRotation(dir);
		float angleDeg = Quaternion.Angle(rot, toTarget);

		const float RotationThresholdDeg = 1f;
		if (angleDeg > RotationThresholdDeg) // need to rotate if not pointing in the required direction
		{
			const float RotationSlowdownAngleDeg = 15f; // if we're within this angle of our target direction, we want to start slowing down rotation _speed
			float slowdown = Mathf.Clamp01(angleDeg / RotationSlowdownAngleDeg);
			rot = Quaternion.Slerp(rot, toTarget, angularSpeed * slowdown * Time.deltaTime);
			Vector3 euler = rot.eulerAngles;
			euler.z = 0;
			euler.x = 0;
			rot = Quaternion.Euler(euler);
			isAligned = false;
		}
		else
		{
			isAligned = true;
		}
		return rot;
	}

	public static float CalculateDistanceOnPath(Path inputPath, int wayPoint, Vector3 position)
	{
		if (inputPath == null || inputPath.vectorPath.Count == 0 || wayPoint >= inputPath.vectorPath.Count)
		{
			return 0.0f;
		}

		Vector3 pos = position;
		float accumulatedDist = 0f;
		for (int point = wayPoint; point < inputPath.vectorPath.Count; ++point)
		{
			accumulatedDist += GameUtils.GetDistXZ(pos, inputPath.vectorPath[point]);
			pos = inputPath.vectorPath[point];
		}
		return accumulatedDist;
	}
	
	private Vector3 CalculateVelocity(Vector3 currentPosition)
	{
		if (useCalculateVelocityOnSightOnly)
		{
			return CalculateVelocityOnSightOnly(currentPosition);
		}
		else
		{
			return CalculateVelocityOriginal(currentPosition);
		}
	}

	private Vector3 CalculateStraightLineVelocity(Vector3 currentPosition)
	{
		_targetPoint = _destination;
		_targetDirection = _targetPoint - currentPosition;
		_targetDirection.y = 0f;
		_distanceToTargetSqr = _targetDirection.sqrMagnitude;
		
		if (_checkforArrivalInStraightLineMode)
		{
			bool arrival = false;
			if (useCalculateVelocityOnSightOnly)
			{
				arrival = HasArrivedOnSightOnly(currentPosition, new Vector3(), _targetPoint);
			}
			else
			{
				arrival = HasArrivedOriginal(currentPosition, _targetDirection, _targetPoint);
			}

			if (arrival)
			{
				if (!_targetReached)
				{
					_targetReached = true;
					OnTargetReached();
				}
				return Vector3.zero;
			}
		}

		_targetDirection.Normalize();
		return _targetDirection * MaxSpeed;		
	}
	private Vector3 CalculateVelocityOriginal(Vector3 currentPosition)
	{		
		if (_doStraightLineMode)
		{
			return CalculateStraightLineVelocity(currentPosition);
		}

		if (_path == null || _currentWaypointIndex >= _path.vectorPath.Count)
		{
			return Vector3.zero;
		}

		// ignore height for the moment, until player can fly or levitate or climb etc
		_targetPoint = new Vector3(_path.vectorPath[_currentWaypointIndex].x, currentPosition.y, _path.vectorPath[_currentWaypointIndex].z);
		_targetDirection = _targetPoint - currentPosition;
		_distanceToTargetSqr = _targetDirection.sqrMagnitude;

		// avoid visiting a waypoint in the wrong direction
		while (_distanceToTargetSqr <= _pickNextWaypointDist * _pickNextWaypointDist ||
			(_currentWaypointIndex == _path.vectorPath.Count - 2 && AStarPathfindingUtils.IsVisible(currentPosition, _path.vectorPath[_path.vectorPath.Count - 1], _simulator)))
		{
			if (_currentWaypointIndex == _path.vectorPath.Count - 1)
			{
				if (HasArrivedOriginal(currentPosition, _targetDirection, _path.vectorPath[_path.vectorPath.Count - 1]))
				{
					if (!_targetReached)
					{
						_targetReached = true;
						OnTargetReached();
					}
					return Vector3.zero;
				}                                
				break;                
			}
			_currentWaypointIndex++;
			_targetPoint.x = _path.vectorPath[_currentWaypointIndex].x;
			_targetPoint.z = _path.vectorPath[_currentWaypointIndex].z;
			_targetDirection = _targetPoint - currentPosition;
			_distanceToTargetSqr = _targetDirection.sqrMagnitude;
		}
		
		_targetDirection.Normalize();
		// will be at max _speed unless within slowdown distance of the target
		return _targetDirection * MaxSpeed * Mathf.Clamp01(RemainingDistance / _slowdownDistance); 
	}

	private Vector3 CalculateVelocityOnSightOnly(Vector3 currentPosition)
	{
		if (_doStraightLineMode)
		{
			return CalculateStraightLineVelocity(currentPosition);
		}

		if (_path == null || _currentWaypointIndex >= _path.vectorPath.Count)
		{
			return Vector3.zero;
		}

		// ignore height for the moment, until player can fly or levitate or climb etc
		_targetPoint = new Vector3(_path.vectorPath[_currentWaypointIndex].x, currentPosition.y, _path.vectorPath[_currentWaypointIndex].z);
		_targetDirection = _targetPoint - currentPosition;
		_distanceToTargetSqr = _targetDirection.sqrMagnitude;

		Vector3 nextTargetPoint = new Vector3();
		if (_currentWaypointIndex < _path.vectorPath.Count - 1)
		{
			nextTargetPoint = new Vector3(_path.vectorPath[_currentWaypointIndex + 1].x, currentPosition.y, _path.vectorPath[_currentWaypointIndex + 1].z); 
		}

		if (HasArrivedOnSightOnly(currentPosition, nextTargetPoint, _targetPoint))
		{
			if (_currentWaypointIndex < _path.vectorPath.Count - 1)
			{
				_currentWaypointIndex++;
				_targetPoint.x = _path.vectorPath[_currentWaypointIndex].x;
				_targetPoint.z = _path.vectorPath[_currentWaypointIndex].z;
				_targetDirection = _targetPoint - currentPosition;
				_distanceToTargetSqr = _targetDirection.sqrMagnitude;
			}
			else // final point
			{
				if (!_targetReached)
				{
					_targetReached = true;
					OnTargetReached();
				}
				return Vector3.zero;
			}
		}
				
		_targetDirection.Normalize();
		// will be at max _speed unless within slowdown distance of the target
		return _targetDirection * MaxSpeed * Mathf.Clamp01(RemainingDistance / _slowdownDistance); 
	}

	// check conditions for determining that the character has arrived at the destination
	private bool HasArrivedOriginal(Vector3 character, Vector3 characterToTarget, Vector3 currentTarget)
	{
		// calculate the closest we got to the end of the _path over the last frame
		Vector3 nearestToTarget = VectorMath.ClosestPointOnSegmentXZ(_previousPosition, transform.position, currentTarget);
		float nearestToTargetDistSqr = GameUtils.GetDistSqXZ(nearestToTarget, currentTarget);

		if (nearestToTargetDistSqr <= ArrivalThreshold * ArrivalThreshold)
		{ // we were within the arrival threshold at some point over the last frame
			return true;
		}
		// the target position is inside our radius and we're travelling off the nav mesh

		// in the code below _rvoController.radius should be _rvoController.radiusAgainstObstacles, but whilst _rvoController.radius is greater than 
		// _rvoController.radiusAgainstObstacles, this makes  it less likely that the character cannot get to there target and less likely that they 
		// will walk animate in place at the edge of the nav mesh - which is a good thing

		else if (null != _rvoController && characterToTarget.magnitude <= (_rvoController.obstacleTimeHorizon + _rvoController.radius))
		{
			return !AStarPathfindingUtils.IsPointOnNavMesh(character + (characterToTarget.normalized * (_rvoController.obstacleTimeHorizon + _rvoController.radius)), _simulator);
		}
		return false;
	}

	private static List<AStarPathfindingUtils.NavMeshQueryInput> queryInput;
	private static AStarPathfindingUtils.NavMeshQueryInput primarySearch;
	private static AStarPathfindingUtils.NavMeshQueryInput secondarySearch;
	
	// check conditions for determining that the character has arrived at the destination
	private bool HasArrivedOnSightOnly(Vector3 currentPosition, Vector3 nextTargetPoint, Vector3 currentTarget)
	{
		if (_currentWaypointIndex < _path.vectorPath.Count - 1 && !_doStraightLineMode)
		{
			if (null == queryInput)
			{
				const int MaxSimultaneousQueries = 2;
				queryInput = new List<AStarPathfindingUtils.NavMeshQueryInput>(MaxSimultaneousQueries);
			}

			if (null == primarySearch)
			{
				primarySearch = new AStarPathfindingUtils.NavMeshQueryInput();
			}

			queryInput.Clear();

			primarySearch.Reset(nextTargetPoint, currentPosition);
			queryInput.Add(primarySearch);

			if (_distanceToTargetSqr < _pickNextWaypointDist * _pickNextWaypointDist)
			{
				if (null == secondarySearch)
				{
					secondarySearch = new AStarPathfindingUtils.NavMeshQueryInput();
				}

				secondarySearch.Reset(nextTargetPoint, _targetPoint);
				queryInput.Add(secondarySearch);
			}

			AstarPath.active.StartUsingObstaclesScratchList();
			AStarPathfindingUtils.CalculateNavMeshQueryObstacles(ref queryInput, ref AstarPath.active.obstaclesScratchList, _simulator, true);

			if (AStarPathfindingUtils.IsVisible(queryInput[0], AstarPath.active.obstaclesScratchList, _simulator) || // see if the next point is visible
				// if we're close to the next point and our path goes off the nav mesh (which would mean we'll never see our next target point)
				(_distanceToTargetSqr < _pickNextWaypointDist * _pickNextWaypointDist && !AStarPathfindingUtils.IsVisible(queryInput[1], AstarPath.active.obstaclesScratchList, _simulator)))
			{
				AstarPath.active.StopUsingObstaclesScratchList();
				return true;
			}
			AstarPath.active.StopUsingObstaclesScratchList();
		}
		else
		{
			// calculate the closest we got to the end of the _path over the last frame
			Vector3 nearestToTarget = VectorMath.ClosestPointOnSegmentXZ(_previousPosition, transform.position, currentTarget);
			float nearestToTargetDistSqr = GameUtils.GetDistSqXZ(nearestToTarget, currentTarget);

			if (nearestToTargetDistSqr <= ArrivalThreshold * ArrivalThreshold)
			{ // we were within the arrival threshold at some point over the last frame
				return true;
			}
			return (_distanceToTargetSqr < _pickNextWaypointDist * _pickNextWaypointDist) && !AStarPathfindingUtils.IsVisible(currentTarget, currentPosition, _simulator);			
		}
		return false;
	}

	private Vector3 GetFeetPosition()
	{
		if (_rvoController != null)
		{
			return transform.position - Vector3.up * _rvoController.height * 0.5f;
		}
		return transform.position;
	}

	public void OnDebugGUI()
	{
	}

	public void OnDebugPanelGUI()
	{
	}

	// Networking synchronization --------------------------------------------------------------------------------------------------------------------------------------------

	// this is called before the rvo controller update takes place
	private void PreRVOUpdate()
	{
		if (!IsCurrentlyLocallyControlled() && _networkLocomotion != null)
		{
			_rvoController.Teleport(_networkLocomotion.GetHostPosition(), false); // don't reset velocity, so that we have a valid velocity which can be used to send to others should this character
																					// become local (ownership transfer can happen at any time)
		}
	}

	// this is called everytime the rvo controller updates the position of a character
	private void OnPositionChanged()
	{
#if DEBUG
		if (recordPositionHistory)
		{
			if (null == positionHistory)
			{
				positionHistory = new List<FrameInfo>();
			}
			
			while (positionHistory.Count >= maxPositionHistory) // we only keep a history of size _numNetworkTransformHistory
			{
				positionHistory.RemoveAt(0); // the list wil be too big when we add a new entry, so remove the oldest
			}

			FrameInfo info = new FrameInfo();
			info.position = transform.position;

			if (positionHistory.Count > 0)
			{
				info.distToPrev = (transform.position - positionHistory[positionHistory.Count - 1].position).magnitude;
				info.delta = Time.deltaTime;
				info.fps = 1f / info.delta;

				float sum = 0f;
				for (int i = 0; i < positionHistory.Count; ++i)
				{
					sum += positionHistory[i].delta;
				}
				info.averageFps = 1f / (sum / positionHistory.Count);
			}
			positionHistory.Add(info);
		}
#endif
		//optimize if faster
		if (_sendExtraordinaryNetworkUpdateOnNextMovement && _networkLocomotion != null&& IsCurrentlyLocallyControlled()  ) //  something has happened, which means we want to get an update out fast
		{
			const float MovementTolSqr = 0.001f * 0.001f;
			if (GameUtils.GetDistSqXZ(_previousPosition, transform.position) > MovementTolSqr)
			{
				_sendExtraordinaryNetworkUpdateOnNextMovement = false;
				Vector2 position = new Vector2();
				Vector2 velocity = new Vector2();
				float distToDestination = 0f;
				_networkLocomotion.CalculatePositionAndVelocityToSend(ref position, ref velocity, ref distToDestination);

				if (_viewRPC != null)
				{
					//fixme: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
					_viewRPC.RPC("SetExtraordinaryTransformRPC", EB.RPCMode.Others, position, velocity, transform.rotation, distToDestination, System.DateTime.Now.Second);
					// _viewRPC.RPC("SetExtraordinaryTransformRPC", EB.RPCMode.Others, position, velocity, transform.rotation, distToDestination, Network.time);
				}
			}
		}
	}

	//[RPC]
	private void SetExtraordinaryTransformRPC(Vector2 position, Vector2 velocity, Quaternion rotation, float distToDestination, double networkTime)
	{
		_networkLocomotion.SetTransformInterpolated(position, velocity, rotation, distToDestination, networkTime);
	}
}
