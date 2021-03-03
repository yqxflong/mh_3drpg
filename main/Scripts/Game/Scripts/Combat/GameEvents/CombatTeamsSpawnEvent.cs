using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
	public class CombatTeamsSpawnEvent : GameEvent
	{

		public bool Spawned
		{
			get; private set;
		}

		public CombatTeamsSpawnEvent(bool spawned)
		{
			Spawned = spawned;
		}
	}

	public class CombatTeamsVictoryEvent : GameEvent
	{

	}

	public class CombatBossCameraEvent : GameEvent
	{
		public string Name;

		public CombatBossCameraEvent(string name)
		{
			Name = name;
		}
	}

}