///////////////////////////////////////////////////////////////////////
//
//  SpawnStateHandler.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////


public enum eSpawnAnimation
{
	None = -1,
	Default = 0,
	Transform = 1,
}
/*
[CharacterStateHandler(eCampaignCharacterState.Spawn)]
public class SpawnStateHandler : InvulnerableStateHandler 
{
	private GameObject _shadow;
	private SkinnedMeshRenderer _enemyRenderer;

	protected override void Reset() //Clear State
	{
		base.Reset();
		_shadow = null;
		_enemyRenderer = null;
	}

	public override void Begin(eCampaignCharacterState previousState)
	{
		_stateCommand = eCampaignCharacterStateCommand.Spawn;
		
		Transform shadowTransform = Character.transform.FindChild("shadow");
		if (shadowTransform != null)
		{
			_shadow = shadowTransform.gameObject;
		}

		_enemyRenderer = Character.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

		base.Begin(previousState);

		if (_shadow != null)
		{
			_shadow.SetActive(false);
		}
	}

	public override void End(eCampaignCharacterState newState)
	{
		base.End(newState);

		EventManager.instance.Raise(new SpawnEndedEvent(Controller.gameObject));
	}

	public override void Update()
	{
		base.Update();

		if (AnimController.IsInCharacterState(eCharacterAnimationState.Idle))
		{
			SetState(eCampaignCharacterState.Idle);
		}
		else if (AnimController.IsInCharacterState(eCharacterAnimationState.PostSpawnLoop))
		{
			SetState(eCampaignCharacterState.PostSpawnLoop);
		}
		else if (AnimController.IsInCharacterState(eCharacterAnimationState.PostSpawnToIdle))
		{
			SetState(eCampaignCharacterState.PostSpawnToIdle);
		}

		if (_shadow != null && !_shadow.activeSelf)
		{
			_shadow.SetActive(true);
			
			Vector3 shadowPosition = _enemyRenderer.rootBone.transform.position;
			shadowPosition.y = _shadow.transform.position.y;
			_shadow.transform.position = shadowPosition;
		}
	}
}
*/