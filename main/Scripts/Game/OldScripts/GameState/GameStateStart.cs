///////////////////////////////////////////////////////////////////////
//
//  GameStateStart.cs
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
using EB.Sequence.Runtime;
using System;

[GameState(eGameState.Start)]
public class GameStateStart : GameState
{
	public override IEnumerator Start(GameState oldState)
	{
        // Hotfix_LT.Messenger.Raise<eGameState>("GameStateChangeOnStart", eGameState.Start);
		 GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "GameStateChangeOnStart", eGameState.Start);
        yield break;
	}

	public override void End(GameState newState)
	{
        // Hotfix_LT.Messenger.Raise<eGameState>("GameStateChangeOnEnd", eGameState.Start);
		GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "GameStateChangeOnEnd", eGameState.Start);
    }
}
