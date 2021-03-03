using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;
using System;

[GameState(eGameState.LoadGame)]
public class GameStateLoadGame : GameState
{
	public override IEnumerator Start(GameState oldState)
	{
        // Hotfix_LT.Messenger.Raise<eGameState>("GameStateChangeOnStart", eGameState.LoadGame);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "GameStateChangeOnStart", eGameState.LoadGame);
        yield break;
    }
    
	public override void End(GameState newState)
	{
        // Hotfix_LT.Messenger.Raise<eGameState>("GameStateChangeOnEnd", eGameState.LoadGame);
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "GameStateChangeOnEnd", eGameState.LoadGame);
    }
}
