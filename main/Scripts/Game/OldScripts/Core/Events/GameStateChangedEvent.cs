using UnityEngine;
using System.Collections;

public class GameStateChangedEvent : GameEvent 
{
	public eGameState oldState;
	public eGameState newState;

	public GameStateChangedEvent(eGameState oldState, eGameState newState)
	{
		this.oldState = oldState;
		this.newState = newState;
	}
}
