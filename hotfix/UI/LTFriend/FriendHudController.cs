using UnityEngine;
using System.Collections;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{

    public class FriendHudController : UIControllerHotfix
    {
        public override bool IsFullscreen() { return true; }

        public UILabel FriendNumLabel;
        public FriendTabController TabController;
        public FriendChatController ChatController;
        public FriendItemDynamicScrollManager DynamicScroll;
        public TitleListController titleCon;
        private FriendData mSelectMyFriendData;
        private FriendData mSelectBlacklistData;
        private FriendData mSelectRecentlyFriendData;
        private FriendData mSelectTeamFriendData;

        private int mSelectLast;
        private FriendItem mCurrentItem;
        private long mTargetUid;
        public static bool mFirstOpen = true;


        public static bool sOpen = false;
        protected object mParam = null;
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            mParam = param;
            controller.GObjects["FriendRedPoint"].CustomSetActive(false);
            controller.GObjects["RecentRedPoint"].CustomSetActive(false);
            controller.GObjects["ApplyRedpoint"].CustomSetActive(false);
            controller.UiButtons["HotfixBtn4"].gameObject.CustomSetActive(SceneLogicManager.getSceneType() != SceneLogicManager.SCENE_COMBAT);
            TabController.SelectFriendAction = delegate ()
            {
                DoSelectFriendAction();
            };
            FriendManager.Instance.MyFriends.IsHaveUpdate = false;
            FriendManager.Instance.BlackLists.IsHaveUpdate = false;
            FriendManager.Instance.Recentlys.IsHaveUpdate = false;
            FriendManager.Instance.Teams.IsHaveUpdate = false;
            ResetVigorTime();
        }

        public void ResetVigorTime()
        {
            FriendManager.Instance.ResetVigorTime(
                delegate
                {
                    controller.GObjects["FriendRedPoint"].CustomSetActive(FriendManager.Instance.MyFriends.canReceiveVigor);
                    if (FriendManager.Instance.Info.UnreadMessageUIds.Count > 0)
                    {
                        controller.GObjects["RecentRedPoint"].SetActive(true);
                        for (int i = 0; i < FriendManager.Instance.Info.UnreadMessageUIds.Count; i++)
                        {
                            bool isFriend = FriendManager.Instance.MyFriends.Find(FriendManager.Instance.Info.UnreadMessageUIds[i]) != null;
                            controller.GObjects["FriendRedPoint"].SetActive(isFriend || FriendManager.Instance.MyFriends.canReceiveVigor);
                        }
                    }

                    if (mParam != null)
                    {
                        Hashtable ht = mParam as Hashtable;
                        if (ht != null)
                        {
                            mTargetUid = ht["uid"] != null ? (long)ht["uid"] : 0;
                            // if (ht["type"] != null)
                            // {
                            //     eFriendType friType = (eFriendType)ht["type"];
                            //     if (forceSetType) friType = eFriendType.My;
                            //     int index = (int)friType - 1;
                            //     titleCon.SetTitleBtn(index);
                            // }
                            FriendManager.Instance.GetInfo(mTargetUid);
                        }
                    }
                    else
                        FriendManager.Instance.GetInfo(0);
                });
        }

        public override IEnumerator OnAddToStack()
        {
            GameDataSparxManager.Instance.RegisterListener(FriendManager.MyFriendListId, OnMyFriendListener);
            GameDataSparxManager.Instance.RegisterListener(FriendManager.BlacklistListId, OnBlacklistListener);
            GameDataSparxManager.Instance.RegisterListener(FriendManager.RecentlyListId, OnRecentlyListener);
            GameDataSparxManager.Instance.RegisterListener(FriendManager.TeamListId, OnTeamListener);
            Messenger.AddListener<bool>(Hotfix_LT.EventName.FriendApplyEvent, OnFriendApplyListener);
            Messenger.AddListener<long>(Hotfix_LT.EventName.FriendAddToBlacklistEvent, OnFriendAddToBlacklistListener);
            Messenger.AddListener<long>(Hotfix_LT.EventName.FriendMessageEvent, OnFriendMessageListener);
            Messenger.AddListener(Hotfix_LT.EventName.OtherPlayerDelectFriend, OnOtherPlayerDelectFriend);
            Messenger.AddListener<long>(Hotfix_LT.EventName.FriendOpenRecentlyEvent, OnFriendNeedOpenRecently);
            Messenger.AddListener(Hotfix_LT.EventName.FriendSendVigorEvent, OnFriendSendVigorListener);
            Messenger.AddListener(Hotfix_LT.EventName.FriendSendAllButtonState, OnFriendSendAllButtonState);
            mFirstOpen = true;
            sOpen = true;
            ChatController.OnAddToStack();
            AddEventListener();

            yield return null;
            yield return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            TabController.Type = eFriendType.None;
            GameDataSparxManager.Instance.UnRegisterListener(FriendManager.MyFriendListId, OnMyFriendListener);
            GameDataSparxManager.Instance.UnRegisterListener(FriendManager.BlacklistListId, OnBlacklistListener);
            GameDataSparxManager.Instance.UnRegisterListener(FriendManager.RecentlyListId, OnRecentlyListener);
            GameDataSparxManager.Instance.UnRegisterListener(FriendManager.TeamListId, OnTeamListener);
            Messenger.RemoveListener<bool>(Hotfix_LT.EventName.FriendApplyEvent, OnFriendApplyListener);
            Messenger.RemoveListener<long>(Hotfix_LT.EventName.FriendAddToBlacklistEvent, OnFriendAddToBlacklistListener);
            Messenger.RemoveListener<long>(Hotfix_LT.EventName.FriendMessageEvent, OnFriendMessageListener);
            Messenger.RemoveListener(Hotfix_LT.EventName.OtherPlayerDelectFriend, OnOtherPlayerDelectFriend);
            Messenger.RemoveListener<long>(Hotfix_LT.EventName.FriendOpenRecentlyEvent, OnFriendNeedOpenRecently);
            Messenger.RemoveListener(Hotfix_LT.EventName.FriendSendVigorEvent, OnFriendSendVigorListener);
            Messenger.RemoveListener(Hotfix_LT.EventName.FriendSendAllButtonState, OnFriendSendAllButtonState);
            SetFriendData(null);
            ChatController.OnRemoveFromStack();
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Friend);
            sOpen = false;
            DestroySelf();
            yield break;
        }

        public override void OnFocus()
        {
            base.OnFocus();

            controller.GObjects["ApplyRedpoint"].CustomSetActive(FriendManager.Instance.Info.ApplyCount > 0);
        }

        public override void StartBootFlash()
        {
			SetCurrentPanelAlpha(1);
			UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }

        private int FriendNum, BlackNum, RecentNum, TeamNum;

        private void OnMyFriendListener(string path, INodeData data)
        {
            MyFriendList list = data as MyFriendList;

            if (list == null || !list.IsHaveUpdate)
            {
                return;
            }

            FriendNum = list.List.Count;

            if (FriendNumLabel != null)
            {
                FriendNumLabel.text = string.Format("{0}/{1}", FriendNum, FriendManager.Instance.Config.MaxFriendNum);
            }

            TabController.SetPeopleNum(string.Format("{0}/{1}", FriendNum, FriendManager.Instance.Config.MaxFriendNum), null, null, null);
            list.List.Sort(new FriendComparer());
        }

        private void OnBlacklistListener(string path, INodeData data)
        {
            BlackList list = data as BlackList;

            if (list == null || !list.IsHaveUpdate)
            {
                return;
            }

            BlackNum = list.List.Count;
            FriendManager.Instance.Info.BlacklistNum = BlackNum;
            TabController.SetPeopleNum(null, string.Format("{0}/{1}", BlackNum, FriendManager.Instance.Config.MaxBlacklistNum), null, null);
            list.List.Sort(new FriendComparer());
        }

        private void OnRecentlyListener(string path, INodeData data)
        {
            RecentlyList list = data as RecentlyList;

            if (list == null || !list.IsHaveUpdate)
            {
                return;
            }

            RecentNum = list.List.Count;
            TabController.SetPeopleNum(null, null, string.Format("{0}/{1}", RecentNum, FriendManager.Instance.Config.MaxRecentlyNum), null);
            list.List.Sort(new FriendComparer());

            if (isNeedOpenRecently)
            {
                isNeedOpenRecently = false;
                titleCon.SetTitleBtn(2);
                TabController.Open(eFriendType.Recently);
            }
        }

        private void OnTeamListener(string path, INodeData data)
        {
            TeamList list = data as TeamList;

            if (list == null || !list.IsHaveUpdate)
            {
                return;
            }

            TeamNum = list.List.Count;
            TabController.SetPeopleNum(null, null, null, string.Format("{0}/{1}", list.List.Count, FriendManager.Instance.Config.MaxTeamNum));
            list.List.Sort(new FriendComparer());

            if (mFirstOpen)
            {

                if (mParam != null)
                {
                    Hashtable ht = mParam as Hashtable;
                    eFriendType type = (eFriendType)ht["type"];
                    TabController.Open(type);
                    int index = (int)type - 1;
                    titleCon.SetTitleBtn(index);
                }
                else
                {
                    TabController.Open(eFriendType.My);
                    titleCon.SetTitleBtn(0);
                }
                mFirstOpen = false;
            }
            else
            {
                DoSelectFriendAction();
                if (TabController.Type == eFriendType.My && FriendManager.Instance.MyFriends.List.Count <= 0)
                {
                    controller.GObjects["FriendRedPoint"].CustomSetActive(false);
                }
            }
        }

        private void OnFriendApplyListener(bool isShow)
        {
            controller.GObjects["ApplyRedpoint"].CustomSetActive(isShow);
        }


        private void OnFriendMessageListener(long FromUid)
        {
            controller.GObjects["FriendRedPoint"].CustomSetActive(FriendManager.Instance.MyFriends.canReceiveVigor);
            if (CurrentSelectData != null && CurrentSelectData.Uid == FromUid)
            {
                FriendManager.Instance.Info.RemoveUnreadMessageId(FromUid);
                if (FriendManager.Instance.Info.UnreadMessageUIds.Count <= 0)
                {
                    controller.GObjects["RecentRedPoint"].CustomSetActive(false);
                    controller.GObjects["FriendRedPoint"].CustomSetActive(false || FriendManager.Instance.MyFriends.canReceiveVigor);
                }
            }

            if (FriendManager.Instance.Info.UnreadMessageUIds.Count > 0)
            {
                controller.GObjects["RecentRedPoint"].CustomSetActive(true);
                for (int i = 0; i < FriendManager.Instance.Info.UnreadMessageUIds.Count; i++)
                {
                    bool isFriend = FriendManager.Instance.MyFriends.Find(FriendManager.Instance.Info.UnreadMessageUIds[i]) != null;
                    controller.GObjects["FriendRedPoint"].CustomSetActive(isFriend || FriendManager.Instance.MyFriends.canReceiveVigor);
                }
            }

            if (TabController.Type == eFriendType.My)
            {
                DynamicScroll.SetDataItems(FriendManager.Instance.MyFriends.List, true);
                OnFriendSendAllButtonState();
            }
            else if (TabController.Type == eFriendType.Black)
            {
                DynamicScroll.SetDataItems(FriendManager.Instance.BlackLists.List, false);
            }
            else if (TabController.Type == eFriendType.Recently)
            {
                if (FriendManager.Instance.Info.NewestSendId != 0)
                {
                    MoveToFirst(eFriendType.Recently, FriendManager.Instance.Info.NewestSendId);
                }
                DynamicScroll.SetDataItems(FriendManager.Instance.Recentlys.List, false);

            }
            else if (TabController.Type == eFriendType.Black)
            {
                DynamicScroll.SetDataItems(FriendManager.Instance.Teams.List, false);
            }
        }

        private void OnFriendAddToBlacklistListener(long TargetUid)
        {
            if (mSelectMyFriendData != null && mSelectMyFriendData.Uid == TargetUid)
            {
                FriendManager.Instance.MyFriends.Remove(TargetUid);
                mSelectMyFriendData = null;
            }
            if (mSelectRecentlyFriendData != null && mSelectRecentlyFriendData.Uid == TargetUid)
            {
                FriendManager.Instance.Recentlys.Remove(TargetUid);
                mSelectRecentlyFriendData = null;
            }
            if (mSelectTeamFriendData != null && mSelectTeamFriendData.Uid == TargetUid)
            {
                FriendManager.Instance.Teams.Remove(TargetUid);
                mSelectTeamFriendData = null;
            }

            FriendManager.Instance.MarkDirty(FriendManager.MyFriendListId);
        }

        private void OnFriendSendVigorListener()
        {
            bool isFriend = false;
            for (int i = 0; i < FriendManager.Instance.Info.UnreadMessageUIds.Count; i++)
            {
                isFriend = FriendManager.Instance.MyFriends.Find(FriendManager.Instance.Info.UnreadMessageUIds[i]) != null;
                if (isFriend)
                {
                    break;
                }
            }
            controller.GObjects["FriendRedPoint"].CustomSetActive(isFriend || FriendManager.Instance.IsResidueVigorReceiveNum());
        }

        private void OnFriendSendAllButtonState()
        {
            if (FriendManager.Instance.CanAllReceive() && FriendManager.Instance.IsResidueVigorReceiveNum())
            {
                controller.UiButtons["ReceiveVigorBtn"].isEnabled = true;
                controller.UiButtons["ReceiveVigorBtn"].transform.Find("Vigor").GetComponent<UISprite>().color = Color.white;
            }
            else
            {
                controller.UiButtons["ReceiveVigorBtn"].isEnabled = false;
                controller.UiButtons["ReceiveVigorBtn"].transform.Find("Vigor").GetComponent<UISprite>().color = Color.magenta;
            }

            if (FriendManager.Instance.CanAllSend() && FriendManager.Instance.IsResidueVigorSendNum())
            {
                controller.UiButtons["SendVigorBtn"].isEnabled = true;
                controller.UiButtons["SendVigorBtn"].transform.Find("Vigor").GetComponent<UISprite>().color = Color.white;
            }
            else
            {
                controller.UiButtons["SendVigorBtn"].isEnabled = false;
                controller.UiButtons["SendVigorBtn"].transform.Find("Vigor").GetComponent<UISprite>().color = Color.magenta;
            }
        }


        private bool isNeedOpenRecently = false;
        private void OnFriendNeedOpenRecently(long targetUid)
        {
            if (TabController.Type == eFriendType.Search)
            {
                mTargetUid = targetUid;
                isNeedOpenRecently = true;
                FriendManager.Instance.GetInfo(targetUid);
            }
        }


        private void ShowEmpty(bool isEmpty)
        {
            controller.Transforms["BlacklistTrans"].gameObject.CustomSetActive(false);
            controller.Transforms["ChatWindowTrans"].gameObject.CustomSetActive(!isEmpty);
            controller.GObjects["FriendIconPartner"].CustomSetActive(false);
            bool recentEmpty = (RecentNum + TeamNum) <= 0;
            bool friendEmpty = (FriendNum + BlackNum) <= 0;
            if (TabController.Type == eFriendType.Recently || TabController.Type == eFriendType.Team)
            {
                controller.GObjects["LeftContent"].CustomSetActive(!recentEmpty);
            }
            else if (TabController.Type == eFriendType.My || TabController.Type == eFriendType.Black)
            {
                controller.GObjects["LeftContent"].CustomSetActive(!friendEmpty);
            }
        }

        private void ShowBlacklist(FriendData data)
        {
            controller.Transforms["ChatWindowTrans"].gameObject.CustomSetActive(false);
            controller.Transforms["BlacklistTrans"].gameObject.CustomSetActive(true);
            controller.UiLabels["BlacklistTitleLabel"].text = "[fff348]" + string.Format(EB.Localizer.GetString("ID_PLAYER_ENTER_YOUR_BLACKLIST"), "[ffffff]" + data.Name + "[-]");
        }

        private void ShowFriendUI(FriendData data)
        {
            ShowEmpty(data == null);

            if (data != null)
            {
                if (TabController.Type == eFriendType.Black)
                {
                    ShowBlacklist(data);
                }
                else
                {

                    controller.GObjects["FriendIconPartner"].CustomSetActive(true);
                    ShowFriendChatContent(data);
                    Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(int.Parse(data.Head), data.Skin);
                    if (heroInfo != null)
                    {
                        controller.UiSprites["FriendIconSprite"].spriteName = heroInfo.icon;
                    }
                    controller.UiSprites["FriendFrameSprite"].spriteName = EconemyTemplateManager.Instance.GetHeadFrame(data.Frame).iconId;
                    controller.UiLabels["FriendNameLabel"].text = data.Name;
                    controller.UiLabels["FriendLevelLabel"].text = data.Level.ToString();

                    UpdateLikeState(data);

                }
            }
        }

        private void ShowFriendChatContent(FriendData data)
        {
            if (data != null)
            {
                ChatController.Show(data.Uid, data.Name);
            }
            else
            {
                ChatController.Show(0, "");
            }
        }

        private void UpdateLikeState(FriendData data)
        {
        }

        #region event handler

        public void OnSelectFriendClick(FriendItem item)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            mCurrentItem = item;
            if (CurrentSelectData == item.Data)
            {
                return;
            }

            SetFriendData(item.Data);
            item.OnSelect(true);
        }

        private void SetFriendData(FriendData data)
        {
            ShowFriendUI(data);
            if (data != null)
            {
                data.IsSelect = true;
                FriendManager.Instance.Info.RemoveUnreadMessageId(data.Uid);
                if (FriendManager.Instance.Info.UnreadMessageUIds.Count <= 0)
                {
                    controller.GObjects["RecentRedPoint"].CustomSetActive(false);
                    controller.GObjects["FriendRedPoint"].CustomSetActive(false || FriendManager.Instance.MyFriends.canReceiveVigor);
                }

                if (FriendManager.Instance.Info.UnreadMessageUIds.Count > 0)
                {
                    controller.GObjects["RecentRedPoint"].CustomSetActive(true);
                    for (int i = 0; i < FriendManager.Instance.Info.UnreadMessageUIds.Count; i++)
                    {
                        bool isFriend = FriendManager.Instance.MyFriends.Find(FriendManager.Instance.Info.UnreadMessageUIds[i]) != null;
                        controller.GObjects["FriendRedPoint"].CustomSetActive(isFriend || FriendManager.Instance.MyFriends.canReceiveVigor);
                    }
                }
            }

            if (TabController.Type == eFriendType.My)
            {
                if (mSelectMyFriendData != null && mSelectMyFriendData != data)
                    mSelectMyFriendData.IsSelect = false;

                mSelectMyFriendData = data;
                DynamicScroll.SetDataItems(FriendManager.Instance.MyFriends.List, true);
                OnFriendSendAllButtonState();
                if (mSelectLast > 3)
                {
                    DynamicScroll.MoveTo(mSelectLast > 3 ? mSelectLast - 3 : 0, true);
                    mSelectLast = 3;
                }

            }
            else if (TabController.Type == eFriendType.Black)
            {
                if (mSelectBlacklistData != null && mSelectBlacklistData != data)
                    mSelectBlacklistData.IsSelect = false;

                mSelectBlacklistData = data;
                DynamicScroll.SetDataItems(FriendManager.Instance.BlackLists.List, false);
                if (mSelectLast > 3)
                {
                    DynamicScroll.MoveTo(mSelectLast > 3 ? mSelectLast - 3 : 0, false);
                   EB.Debug.LogError("移到第{0}" , (mSelectLast - 3));
                    mSelectLast = 3;
                }
            }
            else if (TabController.Type == eFriendType.Recently)
            {
                if (mSelectRecentlyFriendData != null && mSelectRecentlyFriendData != data)
                    mSelectRecentlyFriendData.IsSelect = false;

                mSelectRecentlyFriendData = data;
                DynamicScroll.SetDataItems(FriendManager.Instance.Recentlys.List, false);

            }
            else if (TabController.Type == eFriendType.Team)
            {
                if (mSelectTeamFriendData != null && mSelectTeamFriendData != data)
                    mSelectTeamFriendData.IsSelect = false;

                mSelectTeamFriendData = data;
                DynamicScroll.SetDataItems(FriendManager.Instance.Teams.List, false);

            }
        }

        public void MoveToFirstAtRecently()
        {
            if (TabController.Type == eFriendType.Recently)
            {
                if (FriendManager.Instance.Info.NewestSendId != 0)
                {
                    MoveToFirst(eFriendType.Recently, FriendManager.Instance.Info.NewestSendId);
                    DynamicScroll.SetDataItems(FriendManager.Instance.Recentlys.List, false);

                }
            }
        }

        private void MoveToFirst(eFriendType type, long targetUid)
        {
            switch (type)
            {
                case eFriendType.My:
                    var friend = FriendManager.Instance.MyFriends.Find(targetUid);
                    if (friend != null)
                    {
                        if (FriendManager.Instance.MyFriends.List.IndexOf(friend) == 0)
                        {
                            return;
                        }
                        FriendManager.Instance.MyFriends.Remove(targetUid);
                        FriendManager.Instance.MyFriends.List.Insert(0, friend);
                    }
                    break;
                case eFriendType.Black:
                    var blackList = FriendManager.Instance.BlackLists.Find(targetUid);
                    if (blackList != null)
                    {
                        if (FriendManager.Instance.BlackLists.List.IndexOf(blackList) == 0)
                        {
                            return;
                        }
                        FriendManager.Instance.BlackLists.Remove(targetUid);
                        FriendManager.Instance.BlackLists.List.Insert(0, blackList);
                    }
                    break;
                case eFriendType.Recently:
                    var recently = FriendManager.Instance.Recentlys.Find(targetUid);
                    if (recently != null)
                    {
                        if (FriendManager.Instance.Recentlys.List.IndexOf(recently) == 0)
                        {
                            return;
                        }

                        FriendManager.Instance.Recentlys.Remove(targetUid);
                        FriendManager.Instance.Recentlys.List.Insert(0, recently);
                    }
                    else
                    {
                       EB.Debug.LogWarning("while moveToFirst cannot find recently targetuid={0}" , targetUid);
                    }
                    break;
                case eFriendType.Team:
                    var team = FriendManager.Instance.Teams.Find(targetUid);
                    if (team != null)
                    {
                        if (FriendManager.Instance.Teams.List.IndexOf(team) == 0)
                        {
                            return;
                        }
                        FriendManager.Instance.Teams.Remove(targetUid);
                        FriendManager.Instance.Teams.List.Insert(0, team);
                    }
                    break;
            }
        }

        public void DoSelectFriendAction()
        {
            if (mTargetUid != 0)
            {
                FriendData friendData = null;
                if (TabController.Type == eFriendType.My)
                {
                    friendData = FriendManager.Instance.MyFriends.Find(mTargetUid);
                }
                else if (TabController.Type == eFriendType.Black)
                {
                    friendData = FriendManager.Instance.BlackLists.Find(mTargetUid);
                }
                else if (TabController.Type == eFriendType.Recently)
                {
                    friendData = FriendManager.Instance.Recentlys.Find(mTargetUid);
                }
                else if (TabController.Type == eFriendType.Team)
                {
                    friendData = FriendManager.Instance.Teams.Find(mTargetUid);
                }

                mTargetUid = 0;
                SetFriendData(friendData);
                return;
            }


            if (TabController.Type == eFriendType.My)
            {
                if (mSelectMyFriendData != null)
                {
                    if (FriendManager.Instance.MyFriends.Find(mSelectMyFriendData.Uid) == null)
                    {
                        mSelectMyFriendData = null;
                    }
                }

                if (FriendManager.Instance.MyFriends.List.Count > 0 && mSelectMyFriendData == null)
                    SetFriendData(FriendManager.Instance.MyFriends.List[mSelectLast > 3 ? mSelectLast - 3 : 0]);
                else if (FriendManager.Instance.MyFriends.List.Count > 0 && mSelectMyFriendData != null)
                    SetFriendData(mSelectMyFriendData);
                else
                    SetFriendData(null);
            }
            else if (TabController.Type == eFriendType.Black)
            {
                if (FriendManager.Instance.BlackLists.List.Count > 0 && mSelectBlacklistData == null)
                    SetFriendData(FriendManager.Instance.BlackLists.List[mSelectLast > 3 ? mSelectLast - 3 : 0]);
                else if (FriendManager.Instance.BlackLists.List.Count > 0 && mSelectBlacklistData != null)
                    SetFriendData(mSelectBlacklistData);
                else
                    SetFriendData(null);
            }
            else if (TabController.Type == eFriendType.Recently)
            {
                if (FriendManager.Instance.Info.NewestSendId != 0)
                {
                    MoveToFirst(eFriendType.Recently, FriendManager.Instance.Info.NewestSendId);
                }
                if (FriendManager.Instance.Recentlys.List.Count > 0 && mSelectRecentlyFriendData == null)
                    SetFriendData(FriendManager.Instance.Recentlys.List[0]);
                else if (FriendManager.Instance.Recentlys.List.Count > 0 && mSelectRecentlyFriendData != null)
                    SetFriendData(mSelectRecentlyFriendData);
                else
                    SetFriendData(null);
            }
            else if (TabController.Type == eFriendType.Team)
            {
                if (FriendManager.Instance.Teams.List.Count > 0 && mSelectTeamFriendData == null)
                    SetFriendData(FriendManager.Instance.Teams.List[0]);
                else if (FriendManager.Instance.Teams.List.Count > 0 && mSelectTeamFriendData != null)
                    SetFriendData(mSelectTeamFriendData);
                else
                    SetFriendData(null);
            }
        }

        private FriendData CurrentSelectData
        {
            get
            {
                if (TabController.Type == eFriendType.My)
                {
                    return mSelectMyFriendData;
                }
                else if (TabController.Type == eFriendType.Black)
                {
                    return mSelectBlacklistData;
                }
                else if (TabController.Type == eFriendType.Recently)
                {
                    return mSelectRecentlyFriendData;
                }
                else if (TabController.Type == eFriendType.Team)
                {
                    return mSelectTeamFriendData;
                }
                return null;
            }
            set
            {
                if (TabController.Type == eFriendType.My)
                {
                    mSelectMyFriendData = value;
                }
                else if (TabController.Type == eFriendType.Black)
                {
                    mSelectBlacklistData = value;
                }
                else if (TabController.Type == eFriendType.Recently)
                {
                    mSelectRecentlyFriendData = value;
                }
                else if (TabController.Type == eFriendType.Team)
                {
                    mSelectTeamFriendData = value;
                }
            }
        }

        public void OnHeadBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (CurrentSelectData == null)
                return;

            OpenPlayerInfoPanel(CurrentSelectData);
        }

        public void OnLikeBtnClick()
        {
            if (CurrentSelectData.LikeState != eLikeState.None)
            {
                return;
            }

            CurrentSelectData.LikeState = eLikeState.Like;
            CurrentSelectData.LikeNum++;
            var friend = FriendManager.Instance.MyFriends.Find(CurrentSelectData.Uid);
            if (friend != null)
            {
                friend.LikeState = eLikeState.Like;
                friend.LikeNum = CurrentSelectData.LikeNum;
            }
            var recently = FriendManager.Instance.Recentlys.Find(CurrentSelectData.Uid);
            if (recently != null)
            {
                recently.LikeState = eLikeState.Like;
                recently.LikeNum = CurrentSelectData.LikeNum;
            }
            var team = FriendManager.Instance.Teams.Find(CurrentSelectData.Uid);
            if (team != null)
            {
                team.LikeState = eLikeState.Like;
                team.LikeNum = CurrentSelectData.LikeNum;
            }
            UpdateLikeState(CurrentSelectData);

            FriendManager.Instance.Like(CurrentSelectData.Uid, delegate (bool successful)
            {
                if (successful)
                {
                    MessageTemplateManager.ShowMessage(902255);
                    DataLookupsCache.Instance.SetCache("intact.like_list." + CurrentSelectData.Uid, true, true);
                }
            });
        }

        public void OnPkBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");

            //if (CurrentSelectData.OfflineTime != 0)
            //{
            //    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FriendHudController_29924"));
            //    return;
            //}
            OtherPlayerInfoController.PK(CurrentSelectData.Uid);
            //OtherPlayerInfoController.PK(CurrentSelectData.Uid, CurrentSelectData.Name, delegate (EB.Sparx.Response result)
            //{
            //    if (result.sucessful)
            //    {
            //        if (result.hashtable != null)  //902069：对方正忙或已离线！
            //        {
            //            int errCode = EB.Dot.Integer("errCode", result.hashtable, -1);
            //            switch ((PkErrCode)errCode)
            //            {
            //                case PkErrCode.Busying:
            //                    MessageTemplateManager.ShowMessage(902069);
            //                    return;
            //                case PkErrCode.NotReceivePkRequest:
            //                    MessageTemplateManager.ShowMessage(902069);
            //                    return;
            //                default:
            //                    var ht = Johny.HashtablePool.Claim();
            //                    ht.Add("name", CurrentSelectData.Name);
            //                    ht.Add("uid", CurrentSelectData.Uid);
            //                    GlobalMenuManager.Instance.Open("PkSendRequestUI", ht);
            //                    break;
            //            }
            //        }
            //    }
            //});
        }

        public void OnDeleteBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            System.Action callback = delegate ()
            {
                if (mCurrentItem != null && mCurrentItem.ViewIndex > 3)
                {
                    mSelectLast = mCurrentItem.ViewIndex;
                }

                if (TabController.Type == eFriendType.My || TabController.Type == eFriendType.Black)
                {
                    mSelectMyFriendData = null;
                }
                else if (TabController.Type == eFriendType.Recently || TabController.Type == eFriendType.Team)
                {
                    if (CurrentSelectData.IsFriend && mSelectMyFriendData.Uid == CurrentSelectData.Uid)
                    {
                        mSelectMyFriendData = null;
                    }

                    if (TabController.Type == eFriendType.Recently)
                    {
                        mSelectRecentlyFriendData = null;
                    }
                    else if (TabController.Type == eFriendType.Team)
                    {
                        mSelectTeamFriendData = null;
                    }
                }

                FriendManager.Instance.MarkDirty(FriendManager.MyFriendListId);
            };

            if (TabController.Type == eFriendType.My || TabController.Type == eFriendType.Black)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("0", CurrentSelectData.Name);
                MessageTemplateManager.ShowMessage(FriendManager.CodeDeleteFriend, ht, delegate (int result)
                {
                    if (result == 0)
                    {
                        FriendManager.Instance.Delete(CurrentSelectData.Uid, CurrentSelectData.Type, delegate (bool successful)
                        {
                            callback();
                        });
                    }
                });
            }
            else if (TabController.Type == eFriendType.Recently || TabController.Type == eFriendType.Team)
            {
                if (CurrentSelectData.IsFriend)
                {
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("0", CurrentSelectData.Name);
                    MessageTemplateManager.ShowMessage(FriendManager.CodeDeleteFriend, ht, delegate (int result)
                    {
                        if (result == 0)
                        {
                            FriendManager.Instance.Delete(CurrentSelectData.Uid, CurrentSelectData.Type, delegate (bool successful)
                            {
                                callback();
                            });
                        }
                    });
                }
                else
                {
                    FriendManager.Instance.Remove(CurrentSelectData.Uid, CurrentSelectData.Type, delegate (bool successful)
                    {
                        callback();
                    });
                }
            }
        }

        public void OnRemoveBlacklistClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            System.Action callback = delegate ()
            {
                FriendManager.Instance.BlackLists.Remove(mSelectBlacklistData.Uid);
                if (mCurrentItem != null && mCurrentItem.ViewIndex > 3)
                {
                    mSelectLast = mCurrentItem.ViewIndex;
                   EB.Debug.LogError("mSelectLast:{0}" , mSelectLast);
                }

                mSelectBlacklistData = null;

                //DoSelectFriendAction();
                FriendManager.Instance.MarkDirty(FriendManager.MyFriendListId);
            };

            var ht = Johny.HashtablePool.Claim();
            ht.Add("uid", mSelectBlacklistData.Uid);
            ht.Add("callback", callback);
            GlobalMenuManager.Instance.Open("FriendBlacklistRemoveUI", ht);
        }
        #endregion

        private void OnOtherPlayerDelectFriend()
        {
            OnDeleteBtnClick();
        }

        public static void OpenPlayerInfoPanel(BaseFriendData data)
        {
            string headIconStr = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(data.Head, data.Skin).icon;
            var ht = Johny.HashtablePool.Claim();
            ht.Add("uid", data.Uid );
            ht.Add("icon", string.Format("{0}",headIconStr ));
            ht.Add("headFrame",data.Frame);
            ht.Add("name", data.Name);
            ht.Add("level", data.Level);
            ht.Add("fight", data.Fight);
            ht.Add("a_name", data.AlliName);
            GlobalMenuManager.Instance.Open("OtherPlayerInfoView", ht);
        }

        private void onDrag(GameObject go, Vector2 delta)
        {
            if (MyFollowCamera.delTouchDownInView != null)
                MyFollowCamera.delTouchDownInView();
        }

        private void onPress(GameObject go, bool isPress)
        {
            if (MyFollowCamera.delTouchDownInView != null && isPress)
                MyFollowCamera.delTouchDownInView();
        }

        private UIEventListener[] _dontMoveCameraListeners;

        private void AddEventListener()
        {
            var colliders = controller.gameObject.GetComponentsInChildren<BoxCollider>(true);
            for (int j = 0; j < colliders.Length; j++)
            {
                if (colliders[j].transform.GetComponent<UIEventListener>() == null)
                {
                    colliders[j].gameObject.AddComponent<UIEventListener>();
                }
            }
            var EventListeners = controller.gameObject.GetComponentsInChildren<UIEventListener>(true);

            _dontMoveCameraListeners = new UIEventListener[EventListeners.Length];
            int i = 0;
            for (int j = 0; j < EventListeners.Length; j++)
            {
                _dontMoveCameraListeners[i] = EventListeners[i];
                _dontMoveCameraListeners[i].onDrag += onDrag;
                _dontMoveCameraListeners[i].onPress += onPress;
            }
        }


        public void AllSendClick()
        {
            if (FriendManager.Instance.CanAllSend())
            {
                if (!FriendManager.Instance.IsResidueVigorSendNum())
                {
                    MessageTemplateManager.ShowMessage(FriendManager.CodeSendVigorLimit);
                    return;
                }
                //你已达到每日赠送体力上限，无法继续赠送


                FriendManager.Instance.SendAllVigor(delegate (Hashtable hashtable)
                {
                    ArrayList temp;
                    DataLookupsCache.Instance.SearchDataByID<ArrayList>("sendVigorStats.uids", out temp);
                    if (temp.Count > 0)
                    {
                        ResetVigorTime();
                    }
                });
            }
            else
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_FRIEND_NO_VIGOR_SEND"));
            }

        }

        public void AllGetClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (FriendManager.Instance.CanAllReceive())
            {
                if (!FriendManager.Instance.IsResidueVigorReceiveNum())
                {
                    MessageTemplateManager.ShowMessage(FriendManager.CodeGetVitLimit);
                    return;
                }

                //接受体力有次数限制，无二次弹窗提示， 达到上限则无法领取且保	留
                int vigor = BalanceResourceUtil.GetUserVigor();
                if (vigor >= 9999)
                {
                    MessageTemplateManager.ShowMessage(FriendManager.CodeVitMax, null, delegate (int result)
                    {
                        if (result == 0)
                        {
                            ReceiveVigor();
                        }
                    });
                }
                else
                {
                    ReceiveVigor();
                }
            }
            else
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_FRIEND_NO_VIGOR_GET"));
            }

        }

        private void ReceiveVigor()
        {
            FriendManager.Instance.ReceiveAllVigor(delegate (Hashtable hashtable)
            {
                ArrayList temp;
                DataLookupsCache.Instance.SearchDataByID<ArrayList>("receiveVigorStats.uids", out temp);

                if (temp.Count > 0)
                {
                    ResetVigorTime();
                }
            });
        }

        public override void Awake()
        {
            base.Awake();

            TabController = controller.transform.Find("Content/LeftSide/CategoryTab").GetMonoILRComponent<FriendTabController>();
            ChatController = controller.transform.Find("Content/RightSide/ChatWindow").GetMonoILRComponent<FriendChatController>();
            DynamicScroll = controller.transform.Find("Content/LeftSide/Content")
                .GetMonoILRComponent<FriendItemDynamicScrollManager>();
            titleCon = controller.transform.Find("BGs/Middle/BGs/UpButtons/Title").GetMonoILRComponent<TitleListController>();

            UIButton backButton = controller.transform.Find("BGs/CancelBtn").GetComponent<UIButton>();
            backButton.onClick.Add(new EventDelegate(base.OnCancelButtonClick));

            controller.UiButtons["HotfixBtn1"].onClick.Add(new EventDelegate(AllSendClick));
            controller.UiButtons["HotfixBtn2"].onClick.Add(new EventDelegate(AllGetClick));
            //FriendItem
            controller.transform.GetComponent<ConsecutiveClickCoolTrigger>("Content/RightSide/TopLayout/TargetHead/Icon").clickEvent.Add
                (new EventDelegate(OnHeadBtnClick));
            controller.UiButtons["HotfixBtn4"].onClick.Add(new EventDelegate(OnPkBtnClick));
            controller.UiButtons["HotfixBtn5"].onClick.Add(new EventDelegate(OnDeleteBtnClick));
            //ChatItem
            controller.UiButtons["HotfixBtn6"].onClick.Add(new EventDelegate(OnRemoveBlacklistClick));
        }
    }
}
