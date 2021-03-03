using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    // important: only for fte
    public class CombatGuideEvent : CombatEvent
    {
        public int GuideId
        {
            get;
            private set;
        }

        public bool Refresh
        {
            get { return GuideId == -1; }
        }

        public bool WaitingGuideFinish
        {
            get;
            private set;
        }

        public bool PauseAnimation
        {
            get;
            private set;
        }

        public bool Force
        {
            get;
            private set;
        }

        public CombatGuideEvent()
        {
            m_type = eCombatEventType.GUIDE;
        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                return false;
            }

            GuideId = EB.Dot.Integer("guide", info, -1);
            Force = EB.Dot.Bool("force", info, false);
            WaitingGuideFinish = EB.Dot.Bool("waitingGuideFinish", info, false);
            if (WaitingGuideFinish)
            {
                PauseAnimation = EB.Dot.Bool("pauseAnimation", info, false);
            }

            return true;
        }
    }
}