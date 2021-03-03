using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatTurnNumberEvent : GameEvent
    {
        public int CurrentTurn
        {
            get;
            set;
        }

        public int LastTurn
        {
            get;
            set;
        }

        public bool Changed
        {
            get { return CurrentTurn != LastTurn; }
        }

        public CombatTurnNumberEvent(int last, int current)
        {
            LastTurn = last;
            CurrentTurn = current;
        }
    }
}