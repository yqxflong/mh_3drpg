public enum eCharacterState
{
	Spawn,
	Idle,
	Move,
	AttackTarget,
	ChaseTarget,
	ChaseCast,
	PreparingToCast,
	Casting,
	Fear,
	MoveAndAttack,
	ChargeAttack,
	Death,
	Transform,
	Dash,
	ChaseTargetSlot,
	//AttackTargetFromSlot,
	//WaitForSlot,
	Interrupt,
	Interact,
	PostSpawnLoop,
	PostSpawnToIdle,
	SpecialAnimation, 
	Dialogue,
}

public enum eCharacterAnimationState
{
	[AnimationStringValue("Base Layer.Spawn")]
	Spawn,

	[AnimationStringValue("Base Layer.Idle")]
	Idle,

	[AnimationStringValue("Base Layer.Attack")]
	Attack,

	[AnimationStringValue("Base Layer.Attack Chain")]
	AttackChain,

	[AnimationStringValue("Base Layer.Preparing Intro")]
	PreparingIntro,

	[AnimationStringValue("Base Layer.Preparing Loop")]
	PreparingLoop,

	[AnimationStringValue("Base Layer.Death")]
	Death,

	[AnimationStringValue("Base Layer.Locomotion")]
	Locomotion,

	[AnimationStringValue("Base Layer.Transform")]
	Transform,

	[AnimationStringValue("Casting States.Cast (Start)")]
	CastStart,

	[AnimationStringValue("Casting States.Cast (Loop)")]
	CastLoop,

	[AnimationStringValue("Casting States.Cast (End)")]
	CastEnd,

	[AnimationStringValue("Strafing States.Strafe")]
	Strafe,

	[AnimationStringValue("Strafing States.Attack")]
	StrafeAttack,

	[AnimationStringValue("Charge Attack States.Begin Charge")]
	BeginCharge,

	[AnimationStringValue("Charge Attack States.Loop Charge")]
	LoopCharge,

	[AnimationStringValue("Charge Attack States.Release Charge")]
	ReleaseCharge,

	[AnimationStringValue("Base Layer.Interrupt")]
	Interrupt,

	[AnimationStringValue("Base Layer.Interact")]
	Interact,

	[AnimationStringValue("Base Layer.PostSpawnLoop")]
	PostSpawnLoop,

	[AnimationStringValue("Base Layer.PostSpawnToIdle")]
	PostSpawnToIdle,

	[AnimationStringValue("Base Layer.SpecialAnimation")]
	SpecialAnimation,

	[AnimationStringValue("Dialogue.Enter")]
	DialogueEnter,

	[AnimationStringValue("Dialogue.Exit")]
	DialogueExit,

	[AnimationStringValue("Dialogue.Idle")]
	DialogueIdle,

	[AnimationStringValue("Dialogue.FallbackIdle")]
	DialogueFallbackIdle,

	[AnimationStringValue("Dialogue.Calm")]
	DialogueCalm,

	[AnimationStringValue("Dialogue.Interested")]
	DialogueInterested,

	[AnimationStringValue("Dialogue.Exclamation")]
	DialogueExclamation,

	[AnimationStringValue("Dialogue.Listen")]
	DialogueListen,
}

public enum eCharacterAnimationParameter
{
	[AnimationStringValue("SpawnAnimationIndex")]
	SpawnAnimationIndex,

	[AnimationStringValue("ForwardVelocity")]
	ForwardVelocity,

	[AnimationStringValue("LateralVelocity")]
	LateralVelocity,

	[AnimationStringValue("CastingAnimation")]
	CastingAnimation,

	[AnimationStringValue("CastIsComplex")]
	CastIsComplex,

	[AnimationStringValue("InterruptCast")]
	InterruptCast,

	[AnimationStringValue("PreparingAnimation")]
	PreparingAnimation,

	[AnimationStringValue("Speed")]
	Speed,

	[AnimationStringValue("AttackVariety")]
	AttackVariety,

	[AnimationStringValue("Attacking")]
	Attacking,

	[AnimationStringValue("ChargeAttackRelease")]
	ChargeAttackRelease,

	[AnimationStringValue("DeathAnimStarted")]
	DeathAnimStarted,

	[AnimationStringValue("hit_e")]
	HitEvent,

	[AnimationStringValue("AttackAnimSpeed")]
	AttackAnimSpeed,

	[AnimationStringValue("RotationDistance")]
	RotationDistance,

	[AnimationStringValue("RunDistanceRemaining")]
	RunDistanceRemaining,

	[AnimationStringValue("RestartCast")]
	RestartCastingAnimation,

	[AnimationStringValue("NPCWalk")]
	NPCWalk,

	[AnimationStringValue("NPCSpecialIdle")]
	NPCSpecialIdle,

	[AnimationStringValue("InteractVariety")]
	InteractVariety,
	
	[AnimationStringValue("IdleVariety")]
	IdleVariety,

	[AnimationStringValue("SpecialAnimationVariety")]
	SpecialAnimationVariety,

	[AnimationStringValue("IsSpecialAnimationLooping")]
	IsSpecialAnimationLooping,

	[AnimationStringValue("DialogueAnimationSubState")]
	DialogueAnimationSubState,

	[AnimationStringValue("IsDialogueAnimationLooping")]
	IsDialogueAnimationLooping,
}

public enum eCharacterStateCommand 
{
	Idle = 0,
	Locomotion = 1,
	Attack = 2,
	Death = 3,
	PreparingToCast = 4,
	Casting = 5,
	ChargeAttack = 6,
	Spawn = 7,
	Transform = 8,
	IdleCustomize = 9,
	Interrupt = 10,
	Interact = 11,
	PostSpawnLoop = 12,
	PostSpawnToIdle = 13,
	SpecialAnimation = 14,
	Dialogue = 15,
}

public enum eInventoryControllerAnimationState
{
	[AnimationStringValue("Base Layer.Idle")]
	Idle,

	[AnimationStringValue("Base Layer.Equip")]
	Equip,

	[AnimationStringValue("Base Layer.Flourish")]
	Flourish,

	[AnimationStringValue("BaseLayer.Selected")]
	Selected,

	[AnimationStringValue("Base Layer.PortraitPose")]
	Portrait,
}

public enum eInventoryControllerParam
{
	[AnimationStringValue("State")]
	State,

	[AnimationStringValue("EquipVariety")]
	EquipVariety,

	[AnimationStringValue("Interrupt")]
	Interrupt,
}

public enum eInventoryControllerStateCommand
{
	Idle = 0,
	Flourish = 1,
	Equip = 2,
	Selected = 3,
	Portrait = 4,
}

public enum eInteractVariety
{
	OpenChest = 0,
	SwitchFloorPush = 1,
	SwitchFloorPull = 2,
	SwitchWallPush = 3,
	SwitchWallPull = 4
}

public enum eIdleVariety
{
	None = -1,
	Alert = 0,
	Relaxed = 1,
}

public enum eSpecialAnimationVariety
{
	//None = -1,
	//AlertToTalk = 0,
	//Listen = 1,
	//Calm = 2,
	//Interested = 3,
	//Exclamation = 4,
	//TalkToAlert = 5,
	//RelaxedToTalk = 6, 
	//TalkToRelaxed = 7,
	DungeonStart = 8, 
	DungeonEnd = 9,
}

public enum eDialogueAnimationSubState
{	
	None = -1,
	// = 0
	Idle = 1,
	Calm = 2,
	Interested = 3,
	Exclamation = 4,
	Enter = 5, 
	Exit = 6,
}
