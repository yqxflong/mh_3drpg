///////////////////////////////////////////////////////////////////////
//
//  GameStateLogin.cs
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
using EB.Sparx;

[GameState(eGameState.Login)]
public class GameStateLogin : GameState
{
    public override IEnumerator Start(GameState oldState)
    {
        // Hotfix_LT.Messenger.Raise<eGameState>("GameStateChangeOnStart", eGameState.Login);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "GameStateChangeOnStart", eGameState.Login);
        yield break;
    }

    public override void End(GameState newState)
    {
        // Hotfix_LT.Messenger.Raise<eGameState>("GameStateChangeOnEnd", eGameState.Login);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "GameStateChangeOnEnd", eGameState.Login);
    }
}
