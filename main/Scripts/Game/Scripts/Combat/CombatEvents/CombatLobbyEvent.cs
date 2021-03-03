using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatLobbyEvent : CombatEvent
    {
        public int LobbyCharacterId
        {
            get;
            private set;
        }

        public bool PauseAnimation
        {
            get;
            private set;
        }

        public CombatLobbyEvent()
        {
            m_type = eCombatEventType.LOBBY;
            m_timing = eCombatEventTiming.AUTO;
        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                EB.Debug.LogError("CombatLobbyEvent.Parse: parse combat event failed");
                return false;
            }

            LobbyCharacterId = EB.Dot.Integer("characterId", info, -1);
            if (LobbyCharacterId < 0)
            {
                EB.Debug.LogError("CombatLobbyEvent.Parse: parse conversationGroupId failed");
                return false;
            }

            PauseAnimation = EB.Dot.Bool("pauseAnimation", info, false);

            return true;
        }
    }
}