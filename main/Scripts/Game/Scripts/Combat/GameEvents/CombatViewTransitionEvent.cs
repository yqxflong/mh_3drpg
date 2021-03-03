using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatViewTransitionEvent : GameEvent
    {
        private string m_destination;

        public string Destination
        {
            get { return m_destination; }
        }

        public CombatViewTransitionEvent(string destination)
        {
            m_destination = destination;
        }
    }
}