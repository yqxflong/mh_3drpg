namespace Hotfix_LT.UI
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class ShowFriendSearchContent : DynamicMonoHotfix
    {
        public UIInput InputLabel;
        public UILabel SearchTipLabel;
        public FriendRecommendDynamicScroll RecommendDynamicScroll;
        public FriendApplyItemDynamicScroll ApplyDynamicScroll;

        UIButton HotfixBtn0;
        UIButton HotfixBtn1;
        ConsecutiveClickCoolTrigger HotfixCoolBtn0;
        ConsecutiveClickCoolTrigger HotfixCoolBtn1;
        public override void Awake()
        {
            base.Awake();

            InputLabel = mDMono.transform.Find("TopInput/Search/Input").GetComponent<UIInput>();
            SearchTipLabel = mDMono.transform.Find("RecommendResult/Title/Label").GetComponent<UILabel>();
            RecommendDynamicScroll = mDMono.transform.Find("RecommendResult/ScrollViewPanel/Placeholder/Grid").GetMonoILRComponent<FriendRecommendDynamicScroll>();
            ApplyDynamicScroll = mDMono.transform.Find("FriendApply/ScrollViewPanel/Placeholder/Grid").GetMonoILRComponent<FriendApplyItemDynamicScroll>();
            HotfixBtn0 = mDMono.transform.Find("TopInput/SearchBtn").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(OnSearchBtnClick));
            // HotfixBtn1 =  mDMono.transform.Find("RecommendResult/ScrollViewPanel/Placeholder/Grid/Item/Add").GetComponent<UIButton>();
            // HotfixBtn1.onClick.Add(new EventDelegate());
            // HotfixCoolBtn0 =  mDMono.transform.Find("FriendApply/ScrollViewPanel/Placeholder/Grid/Item/Agree").GetComponent<ConsecutiveClickCoolTrigger>();
            // HotfixCoolBtn0.clickEvent.Add(new EventDelegate());
            // HotfixCoolBtn1 =  mDMono.transform.Find("FriendApply/ScrollViewPanel/Placeholder/Grid/Item/Reject").GetComponent<ConsecutiveClickCoolTrigger>();
            // HotfixCoolBtn1.clickEvent.Add(new EventDelegate());
        }

        public void OnOpenContent()
        {
            FriendManager.Instance.GetRecommendList();
            FriendManager.Instance.GetApplyList();
            GameDataSparxManager.Instance.RegisterListener(FriendManager.RecommendListId, OnRecommendListener);
            GameDataSparxManager.Instance.RegisterListener(FriendManager.ApplyListId, OnApplyListener);
            SearchTipLabel.text = SearchTipLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_ShowFriendSearchContent_697");
        }

        public void OnLeaveContent()
        {
            if (GameDataSparxManager.Instance.HasListener(FriendManager.RecommendListId))
                GameDataSparxManager.Instance.UnRegisterListener(FriendManager.RecommendListId, OnRecommendListener);
            if (GameDataSparxManager.Instance.HasListener(FriendManager.ApplyListId))
                GameDataSparxManager.Instance.UnRegisterListener(FriendManager.ApplyListId, OnApplyListener);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnLeaveContent();
        }

        private void OnRecommendListener(string path, INodeData data)
        {
            RecommendList list = data as RecommendList;
            int count = list.List.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                if (list.List[i].Uid.Equals(LoginManager.Instance.LocalUserId.Value) ||
                    FriendManager.Instance.MyFriends.Find(list.List[i].Uid) != null ||
                    FriendManager.Instance.BlackLists.Find(list.List[i].Uid) != null
                    ) list.List.Remove(list.List[i]);
            }
            RecommendDynamicScroll.SetItemDatas(list.List.ToArray());
        }

        private void OnApplyListener(string path, INodeData data)
        {
            ApplyList list = data as ApplyList;
            ApplyDynamicScroll.SetItemDatas(list.List.ToArray());
        }

        #region event hanlder

        public void OnSearchBtnClick()
        {
            string str = string.Empty;
            if (string.IsNullOrEmpty(InputLabel.value))
            {
                str = EB.Localizer.GetString("ID_INPUT_EMPTY");
            }

            if (InputLabel.value.IndexOf(" ") >= 0)
            {
                str = EB.Localizer.GetString("ID_INPUT_CONTAINS_SPACE");
            }

            if (InputLabel.value.IndexOf("\n") >= 0)
            {
                str = EB.Localizer.GetString("ID_INPUT_CONTAINS_NEWLINE");
            }

            if (!string.IsNullOrEmpty(str))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, str);
                return;
            }

            if (InputLabel.value.IndexOf('#') == 0 && InputLabel.value.Length >= 2)
            {
                int uid;
                int.TryParse(InputLabel.value.Substring(1), out uid);
                long localUid = AllianceUtil.GetLocalUid();
                if (uid == localUid)
                {
                    MessageTemplateManager.ShowMessage(FriendManager.CodeCouldNotFindSelf);
                    return;
                }
            }
            else if (InputLabel.value == LTGameSettingController.GetPlayerName())
            {
                MessageTemplateManager.ShowMessage(FriendManager.CodeCouldNotFindSelf);
                return;
            }

            FriendManager.Instance.Search(InputLabel.value, delegate (Hashtable result) {
                if (result != null && result.Count > 0)
                {
                    object searchResult = EB.Dot.Object("friendsInfo.search", result, null);
                    if (searchResult != null)
                    {
                        List<RecommendFriendData> searchList = new List<RecommendFriendData>();
                        searchList = Hotfix_LT.EBCore.Dot.List<RecommendFriendData, long>(null, searchResult, searchList, delegate (object value, long uid)
                        {
                            RecommendFriendData friend = new RecommendFriendData();
                            friend.Uid = EB.Dot.Long("uid", value, friend.Uid);
                            friend.Name = EB.Dot.String("name", value, friend.Name);
                            friend.Level = EB.Dot.Integer("level", value, friend.Level);
                            friend.Head = EB.Dot.String("portrait", value, friend.Head);
                            friend.Skin = EB.Dot.Integer("skin", value, friend.Skin);
                            friend.Frame = EB.Dot.String("headFrame", value, friend.Frame);
                            friend.Fight = EB.Dot.Integer("battleRating", value, friend.Fight);
                            friend.AlliName = EB.Dot.String("allianceName", value, friend.AlliName);
                            friend.OfflineTime = EB.Dot.Single("offlineTime", value, friend.OfflineTime);
                            friend.IsFriend = EB.Dot.Bool("isFriend", value, friend.IsFriend);
                            friend.Desc = EB.Dot.String("desc", value, friend.Desc);
                            return friend;
                        });
                        RecommendDynamicScroll.SetItemDatas(searchList.ToArray());

                        SearchTipLabel.text = SearchTipLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_ShowFriendSearchContent_4033");
                    }
                }
                else
                {
                    MessageTemplateManager.ShowMessage(FriendManager.CodeHasNotPlayer);
                }
            });
        }
        #endregion
    }

}