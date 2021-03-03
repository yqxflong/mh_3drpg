using UnityEngine;
using System.Collections;

public enum eCampaignCharacterState
{
	Idle,
	Move,
	AttackTarget,
	ChaseTarget,
	Interact,
}

public enum eCampaignCharacterAnimationState
{
	[AnimationStringValue("Base Layer.Idle")]
	Idle,
	
	[AnimationStringValue("Base Layer.Locomotion")]
	Locomotion,
	
	[AnimationStringValue("Base Layer.Attack")]
	Attack,
	
	[AnimationStringValue("Base Layer.Interact")]
	Interact,
}
