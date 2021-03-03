using UnityEngine;

public class MoveMotion
{
    Animator myAnimator;
    MoveController moveController;
    MoveMotionState currentMotionState;

    public bool FreezeLocation
    {
        set
        {
            if (currentMotionState != null)
            {
                currentMotionState.freezeLocation = value;
            }
        }
    }

    public MoveMotion(MoveController moveController)
    {
        this.moveController = moveController;
    }
	
	public void Awake()
	{
		myAnimator = moveController.GetComponent<Animator>();
	}

	public void OnAnimatorMove()
	{
		UpdateCurrentMoveState();
	}

	private void UpdateCurrentMoveState()
	{
		if(moveController != null && moveController.CurrentMove != null && currentMotionState != null)
		{
			AnimatorStateInfo stateInfo = moveController.GetCurrentStateInfo();
			if(stateInfo.fullPathHash == moveController.GetCurrentAnimHash())
			{
				float normalizedTime = stateInfo.normalizedTime;
				if(currentMotionState.distanceToDestination != null)
				{
					normalizedTime = Mathf.Min (normalizedTime, 1.0f);
				}
				float currentFrame = moveController.NormalizedTimeToFrame_Wrapped(normalizedTime);
				currentMotionState.OnAnimatorUpdated(currentFrame);
		
				//myAnimator.gameObject.SendMessage("OnCurrentMotionStateUpdated", SendMessageOptions.DontRequireReceiver);

				if (currentMotionState.isFinished)
				{
					ToNormalState();
				}
			}
		}
		else
		{
			myAnimator.gameObject.transform.rotation = myAnimator.rootRotation;
			myAnimator.gameObject.transform.position = myAnimator.rootPosition;
		}
	}
	
	public void WalkTo(Vector3 walkDestination, AnimationCurve distanceCurve, float scale)
	{
		currentMotionState = new MoveMotionStateWalk(myAnimator);
		currentMotionState.destination = walkDestination;
		currentMotionState.toDestinationNormalized = Vector3.Normalize(moveController.transform.position - currentMotionState.destination);
		currentMotionState.distanceToDestination = distanceCurve;
		currentMotionState.curveScale = scale;
		currentMotionState.distanceToDestinationOffset = Vector3.zero;
		currentMotionState.distanceToDestinationOffsetMagnitude = currentMotionState.distanceToDestinationOffset.magnitude;
		currentMotionState.freezeLocation = false;
		currentMotionState.isFinished = false;
		currentMotionState.exitFrame = -1.0f;
	}
	
	public void JumpTo(Vector3 jumpDestination, AnimationCurve distanceCurve, float scale, Vector3 destinationOffset, float exitFrame)
	{
		currentMotionState = new MoveMotionStateJump(myAnimator);
		currentMotionState.destination = jumpDestination;
		currentMotionState.toDestinationNormalized = Vector3.Normalize(moveController.transform.position - currentMotionState.destination);
		currentMotionState.distanceToDestination = distanceCurve;
		currentMotionState.curveScale = scale;
		currentMotionState.distanceToDestinationOffset = Vector3.zero;//fixme(troy) - this should be non-zero, but somehow it hitches with the original code destinationOffset;
		currentMotionState.distanceToDestinationOffsetMagnitude = currentMotionState.distanceToDestinationOffset.magnitude;
		currentMotionState.freezeLocation = false;
		currentMotionState.isFinished = false;
		currentMotionState.exitFrame = exitFrame;
	}
	
	public void ToNormalState()
	{
		currentMotionState = new MoveMotionStateNormal(myAnimator);
		if (currentMotionState == null)
			EB.Debug.LogError("currentMotionState == null");
		currentMotionState.freezeLocation = false;
		currentMotionState.isFinished = false;
		currentMotionState.exitFrame = -1.0f;
	}

	public bool IsJumping()
	{
		return currentMotionState != null && currentMotionState.GetType() == typeof(MoveMotionStateJump);
	}

	public bool IsWalking()
	{
		return currentMotionState != null && currentMotionState.GetType() == typeof(MoveMotionStateWalk);
	}
	
	public bool IsNormal()
	{
		return currentMotionState == null || currentMotionState.GetType() == typeof(MoveMotionStateNormal);
	}
}


