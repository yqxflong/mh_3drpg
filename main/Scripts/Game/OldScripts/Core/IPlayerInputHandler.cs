///////////////////////////////////////////////////////////////////////
//
//  IPlayerInputHandler.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////


public interface IPlayerInputHandler
{
	// Return true if this handler has run its course and should be removed.
	bool ShouldBeRemoved { get; }

	void Start(PlayerController playerController);

	// Return true if this handler should consume the input event.
	bool HandleInputEvent(GameEvent e);

	void Update();

	void End();

	void OnAbilityEvent(eEffectEvent eventType);
}
