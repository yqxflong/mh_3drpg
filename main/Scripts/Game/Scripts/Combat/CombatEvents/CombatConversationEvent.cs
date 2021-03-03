using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatConversationEvent : CombatEvent
    {
        public enum eSkillTipPause
        {
            None = 0,
            Pause = 1,
            PauseDontRestore = 2,
        }

        public int ConversationGroupId
        {
            get;
            private set;
        }

        public bool PauseAnimation
        {
            get;
            private set;
        }

        public eSkillTipPause PauseSkillTip
        {
            get;
            private set;
        }

        public CombatConversationEvent()
        {
            m_type = eCombatEventType.CONVERSATION;
            m_timing = eCombatEventTiming.AUTO;
        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                EB.Debug.LogError("CombatConversationEvent.Parse: parse combat event failed");
                return false;
            }

            ConversationGroupId = EB.Dot.Integer("conversationGroupId", info, -1);
            if (ConversationGroupId < 0)
            {
                EB.Debug.LogError("CombatConversationEvent.Parse: parse conversationGroupId failed");
                return false;
            }

            PauseAnimation = EB.Dot.Bool("pauseAnimation", info, false);
            PauseSkillTip = (eSkillTipPause)EB.Dot.Integer("pauseSkillTip", info, (int)PauseSkillTip);

            return true;
        }
    }
}