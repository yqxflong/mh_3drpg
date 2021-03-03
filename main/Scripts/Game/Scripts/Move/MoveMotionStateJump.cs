using UnityEngine;

public class MoveMotionStateJump : MoveMotionState
{
	public MoveMotionStateJump(Animator theAnimator)
	{
		ownerAnimator = theAnimator;
	}
	
	public override void OnAnimatorUpdated(float currentFrame)
	{
		// accept the rotation we're given
		ownerAnimator.gameObject.transform.rotation = ownerAnimator.rootRotation;

		if(freezeLocation == false)
		{
			// compute nominal distance for the current frame, scale it to what we need, and apply it
			float nominalJumpDistance = distanceToDestination.Evaluate(currentFrame) - distanceToDestinationOffsetMagnitude;
			Vector3 jumpPortion = (curveScale * nominalJumpDistance) * toDestinationNormalized;
			Vector3 goHere = destination + jumpPortion + distanceToDestinationOffset;
	
			// maintain the y value from the animator - we are not scaling in y, just xz
			goHere.y = ownerAnimator.rootPosition.y;
	
			// we're done
			ownerAnimator.gameObject.transform.position = goHere;
			
			isFinished = currentFrame > exitFrame && exitFrame != -1.0f;
		}
	}
}

