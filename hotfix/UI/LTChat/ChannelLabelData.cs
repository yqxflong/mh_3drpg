using System;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class ChannelLabelData : DynamicMonoHotfix
    {
        public ChatRule.CHAT_CHANNEL channelType;
        public UIToggle tab;
        public GameObject newMessage;
        public float sendInterval;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;

            tab = t.GetComponent<UIToggle>();
            newMessage = t.FindEx("NewMessageSprite").gameObject;

            if (mDMono.FloatParamList != null)
            {
                var count = mDMono.FloatParamList.Count;

                if (count > 0)
                {
                    sendInterval = mDMono.FloatParamList[0];
                }
            }

            if (mDMono.StringParamList != null)
            {
                var count = mDMono.StringParamList.Count;

                if (count > 0)
                {
                    channelType = (ChatRule.CHAT_CHANNEL)Enum.Parse(typeof(ChatRule.CHAT_CHANNEL), mDMono.StringParamList[0]);
                }
            }
        }
    }
}
