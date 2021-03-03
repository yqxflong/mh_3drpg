using UnityEngine;

namespace Hotfix_LT.UI
{
    public class RedPointListenTool : DynamicMonoHotfix
    {
        public GameObject FriendRPObj;
        public GameObject ChatRPObj;
        public override void Awake()
        {
            base.Awake();


            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                int count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    FriendRPObj = mDMono.ObjectParamList[0] as GameObject;
                    
//                   FriendRPObj.transform.parent.GetComponentEx<UIButton>().onClick.Add(new EventDelegate(OnFriendBtnClick));
                }
                if (count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    ChatRPObj = mDMono.ObjectParamList[1] as GameObject;
//                    ChatRPObj.transform.parent.GetComponentEx<UIButton>().onClick.Add(new EventDelegate(OnChatBtnClick));
                }
            }
        }

        public override void Start()
        {
            InitRP();
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.RedPoint_Chat, SetChatRP);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.RedPoint_Friend, SetFriendRP);
            Hotfix_LT.Messenger.AddListener<bool>(Hotfix_LT.EventName.FriendApplyEvent, OnFriendApplyListener);
            Hotfix_LT.Messenger.AddListener<long>(Hotfix_LT.EventName.FriendMessageEvent, OnFriendMessageListener);
        }

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.RedPoint_Chat, SetChatRP);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.RedPoint_Friend, SetFriendRP);
            Hotfix_LT.Messenger.RemoveListener<bool>(Hotfix_LT.EventName.FriendApplyEvent, OnFriendApplyListener);
            Hotfix_LT.Messenger.RemoveListener<long>(Hotfix_LT.EventName.FriendMessageEvent, OnFriendMessageListener);
        }

        private void InitRP()
        {
            SetChatRP();
            SetFriendRP();
        }

        private void SetChatRP()
        {
            ChatRPObj.CustomSetActive(LTChatManager.GetMainHudRP());
        }

        private void SetFriendRP()
        {
            if (FriendRPObj != null)
            {
                bool isFriendRedpoint = FriendManager.Instance.Info.IsHaveNewMessage || FriendManager.Instance.Info.ApplyCount > 0 || FriendManager.Instance.MyFriends.canReceiveVigor;
                FriendRPObj.gameObject.CustomSetActive(isFriendRedpoint);
            }
        }

        private void OnFriendApplyListener(bool isShow)
        {
            if (FriendRPObj != null)
            {
                FriendRPObj.gameObject.CustomSetActive(isShow);
            }
        }

        private void OnFriendMessageListener(long FromUid)
        {
            if (FriendRPObj != null)
            {
                FriendRPObj.gameObject.CustomSetActive(true);
            }
        }

        public void OnChatBtnClick()
        {
            GlobalMenuManager.Instance.Open("ChatHudView", null);
        }

        public void OnFriendBtnClick()
        {
            GlobalMenuManager.Instance.Open("FriendHud", null);
        }
    }
}

