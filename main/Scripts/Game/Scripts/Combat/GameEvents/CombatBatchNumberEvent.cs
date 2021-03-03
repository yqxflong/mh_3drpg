using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatBatchNumberEvent : GameEvent
    {
        public TeamIndex Index
        {
            get;
            set;
        }

        public int Last
        {
            get;
            set;
        }

        public bool Changed
        {
            get { return Last != Index.Batch; }
        }

        public CombatBatchNumberEvent(TeamIndex index, int last)
        {
            Index = index;
            Last = last;
        }
    }
}