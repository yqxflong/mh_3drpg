using UnityEngine;

public class MoveMotionStateNormal : MoveMotionState
{
    public MoveMotionStateNormal(Animator theAnimator)
    {
        ownerAnimator = theAnimator;
    }

    //private static long tick = 0;
    public override void OnAnimatorUpdated(float currentFrame)
    {
        ownerAnimator.gameObject.transform.rotation = ownerAnimator.rootRotation;
        if(freezeLocation == false)
        {
            if (ownerAnimator.gameObject.transform.position != ownerAnimator.rootPosition)
            {
                //Debug.Log(ownerAnimator.gameObject.GetHashCode() + "OnAnimatorUpdated" + (tick++) +":"+ ownerAnimator.gameObject.transform.position + "=>" + ownerAnimator.rootPosition);
                ownerAnimator.gameObject.transform.position = ownerAnimator.rootPosition;                
            }			
        }
    }
}

