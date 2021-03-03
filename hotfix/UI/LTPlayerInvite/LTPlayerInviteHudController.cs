using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTPlayerInviteHudController : UIControllerHotfix
    {
        private enum PageType
        { 
            none = -1,
            myInviteCode = 0,
            inviteplayerReward = 1,
            invitedReward = 2,
        }

        private PageType curType;
        private InviteSharePage sharePage;
        private InviteRewardPage rewardpage;
        private GameObject[] redPoint;
        private GameObject[] Pressobj;
        private GameObject[] titleobj;

        public override bool IsFullscreen()
        {
            return true;
        }

        public override void Awake()
        {
            base.Awake();

            Transform t = controller.GetComponent<Transform>();
            sharePage = t.GetMonoILRComponent<InviteSharePage>("Content/ViewList/0_InviteCodeView");
            rewardpage = t.GetMonoILRComponent<InviteRewardPage>("Content/ViewList/1_Reward");
            redPoint = new GameObject[3];
            Pressobj = new GameObject[3];
            titleobj = new GameObject[3];

            t.GetComponent<UIEventTrigger>("Content/ButtonList/0_InviteCode/Btn").onClick.Add(new EventDelegate(delegate
            {
                OnClickButtonList(PageType.myInviteCode);
            }));
            redPoint[0] = t.GetComponent<Transform>("Content/ButtonList/0_InviteCode/Btn/RedPoint").gameObject;
            Pressobj[0] = t.GetComponent<Transform>("Content/ButtonList/0_InviteCode/Btn/Pressed").gameObject;
            titleobj[0] = t.GetComponent<Transform>("Content/ButtonList/0_InviteCode").gameObject;
            t.GetComponent<UIEventTrigger>("Content/ButtonList/1_Reward/Btn").onClick.Add(new EventDelegate(delegate
            {
                OnClickButtonList(PageType.inviteplayerReward);
            }));
            redPoint[1] = t.GetComponent<Transform>("Content/ButtonList/1_Reward/Btn/RedPoint").gameObject;
            Pressobj[1] = t.GetComponent<Transform>("Content/ButtonList/1_Reward/Btn/Pressed").gameObject;
            titleobj[1] = t.GetComponent<Transform>("Content/ButtonList/1_Reward").gameObject;
            t.GetComponent<UIEventTrigger>("Content/ButtonList/2_InvitedReward/Btn").onClick.Add(new EventDelegate(delegate
            {
                OnClickButtonList(PageType.invitedReward);
            }));
            redPoint[2] = t.GetComponent<Transform>("Content/ButtonList/2_InvitedReward/Btn/RedPoint").gameObject;
            Pressobj[2] = t.GetComponent<Transform>("Content/ButtonList/2_InvitedReward/Btn/Pressed").gameObject;
            titleobj[2] = t.GetComponent<Transform>("Content/ButtonList/2_InvitedReward").gameObject;
            

        }

        public override IEnumerator OnAddToStack()
        {
            Messenger.AddListener(EventName.OnInvitePlayerTaskStateChanged, OnInviteTaskViewChanged);
            curType = PageType.none;
            SetTitleState();
            OnClickButtonList(PageType.myInviteCode);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.invitereward, SetInviteTitleRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.invitedreward, SetInvitedTitleRP);
            return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            Messenger.RemoveListener(EventName.OnInvitePlayerTaskStateChanged, OnInviteTaskViewChanged);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.invitereward, SetInviteTitleRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.invitedreward, SetInvitedTitleRP);
            DestroySelf();
            return base.OnRemoveFromStack();
        }

        private void OnClickButtonList(PageType type)
        {
            for (int i = 0; i < Pressobj.Length; i++)
            {
                if (i == (int)type)
                {
                    Pressobj[i].CustomSetActive(true);
                }
                else
                {
                    Pressobj[i].CustomSetActive(false);
                }
            }
            SwitchToPage(type);       
        }

        private void SwitchToPage(PageType type)
        {
            //if (curType == type)
            //{
            //    return;
            //}
            curType = type;
            sharePage.Show(curType == PageType.myInviteCode);
            rewardpage.Show(curType != PageType.myInviteCode,(int)curType);

        }

        private void OnInviteTaskViewChanged()
        {
            SetTitleState();
            SwitchToPage(GetCurPage());
        }

        private void SetTitleState()
        {
            if (titleobj != null && titleobj.Length >= 3)
            {
                titleobj[1].CustomSetActive(PlayerInviteManager.Instance.invitenum > 0);
                titleobj[2].CustomSetActive(PlayerInviteManager.Instance.invitednum > 0);
            }
        }

        private PageType GetCurPage()
        {
            if(curType == PageType.inviteplayerReward)
            {
                return PlayerInviteManager.Instance.invitenum > 0 ? PageType.inviteplayerReward : PageType.myInviteCode;
            }
            if(curType == PageType.invitedReward)
            {
                return PlayerInviteManager.Instance.invitednum > 0 ? PageType.invitedReward : PageType.myInviteCode;
            }
            return PageType.myInviteCode;
        }
        private void SetInviteTitleRP(RedPointNode node) { redPoint[1].CustomSetActive(node.num > 0); }
        private void SetInvitedTitleRP(RedPointNode node) { redPoint[2].CustomSetActive(node.num > 0); }

    }
}