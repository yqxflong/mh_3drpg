using UnityEngine;

// base class for all move motion states
public abstract class MoveMotionState
{
	public Animator ownerAnimator;
	public bool freezeLocation;

	public Vector3 destination;
	public Vector3 toDestinationNormalized;				// Vector3.Normalize(m_original_position - m_attack_location)
	
	public AnimationCurve distanceToDestination;		// distance TO destination @ frame X
	public float curveScale;							// map curve from nominal distance to the distance we need
	public Vector3 distanceToDestinationOffset;			// the distanceToDestination curve may not reflect a distance to jumpDestination, so we compensate with this vector
	public float distanceToDestinationOffsetMagnitude;	// magnitude of distanceToDestinationOffset
	
	public bool isFinished;
	public float exitFrame;
	
	public virtual void OnAnimatorUpdated(float currentFrame){}
}
