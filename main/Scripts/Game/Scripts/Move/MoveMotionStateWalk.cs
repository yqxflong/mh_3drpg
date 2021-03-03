using UnityEngine;

public class MoveMotionStateWalk : MoveMotionState
{
	public MoveMotionStateWalk(Animator theAnimator)
	{
		ownerAnimator = theAnimator;
	}
	
	public override void OnAnimatorUpdated(float currentFrame)
	{
		ownerAnimator.gameObject.transform.rotation = ownerAnimator.rootRotation;
		if(freezeLocation == false)
		{
			// scale the distance curve
			float myDist = curveScale * distanceToDestination.Evaluate(currentFrame);
			// compute the walk portion
			Vector3 walkPortion = myDist * toDestinationNormalized;
			// add to destination and we're done
			ownerAnimator.gameObject.transform.position = destination + walkPortion;
		}

		isFinished = currentFrame > exitFrame && exitFrame != -1.0f;
    }
}

