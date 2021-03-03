using UnityEngine;
using System.Collections;

public class CinematicEvent : GameEvent
{
	public enum CinematicEventType
	{
		starting, 
		ending
	}
	private CinematicEventType _type = CinematicEventType.starting;
	private Cinematic.Switches _switches;

	public CinematicEvent(CinematicEventType setType, Cinematic.Switches switches) 
	{
		this._type = setType;
		this._switches = switches;
	}

	public CinematicEventType GetCinematicEventType()
	{
		return this._type;
	}

	public Cinematic.Switches GetSwitches()
	{
		return _switches;
	}
}
