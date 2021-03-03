using UnityEngine;
using System.Collections;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class OtherPlayerInfoController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            m_Icon = t.GetComponent<UISprite>("Up/Icon/Sprite");
            m_Frame = t.GetComponent<UISprite>("Up/Icon/Sprite/Frame");
            m_PlayerName = t.GetComponent<UILabel>("Up/Name");
            m_PlayerNameShadow = t.GetComponent<UILabel>("Up/Name/Name (1)");
            m_GangName = t.GetComponent<UILabel>("Up/Legion/Value");
            m_Fight = t.GetComponent<UILabel>("Up/Fight/Value");
            m_Level = t.GetComponent<UILabel>("Up/Sprite/Lvl");
            m_PK = t.GetComponent<UIButton>("Btns/PK");
            m_AddFriend = t.GetComponent<UIButton>("Btns/Add");
            m_DelectFriend = t.GetComponent<UIButton>("Btns/Delect");
            m_Blacklist = t.GetComponent<UIButton>("Btns/Blacklist");
            controller.backButton = t.GetComponent<UIButton>("Btns/MeiYongDeAnNiu_BuYaoShan");

            t.GetComponent<UIButton>("Btns/Add").onClick.Add(new EventDelegate(OnAddFriendClick));
            t.GetComponent<UIButton>("Btns/Delect").onClick.Add(new EventDelegate(OnDelectFriendClick));
            t.GetComponent<UIButton>("Btns/PK").onClick.Add(new EventDelegate(OnPKClick));
            t.GetComponent<UIButton>("Btns/Check").onClick.Add(new EventDelegate(OnCheakInfoBtnClick));
            t.GetComponent<UIButton>("Btns/Chat").onClick.Add(new EventDelegate(OnChatClick));
            t.GetComponent<UIButton>("Btns/Blacklist").onClick.Add(new EventDelegate(OnBlacklistClick));
        }

        public enum OtherPlayerInfoViewType
        {
            FROM_CHAT = 0,
            FROM_PLAYER_INTACT = 1,
        }

        public override bool ShowUIBlocker { get { return true; } }
        public UISprite m_Icon;
        public UISprite m_Frame;
        public UILabel m_PlayerName, m_PlayerNameShadow;
        public UILabel m_GangName;
        public UILabel m_Fight;
        public UILabel m_Level;

        public UIButton m_PK;
        public UIButton m_AddFriend;
        public UIButton m_DelectFriend;
        public UIButton m_Blacklist;

        private long m_Uid;
        public long Uid
        {
            set
            {
                m_Uid = value;
                if (m_Uid != 0)
                {
                    ShowUI();
                }
            }
        }
        private int otherPlayerLevel;
        private OtherPlayerInfoViewType curType = OtherPlayerInfoViewType.FROM_CHAT;
        private string curPlayerName = string.Empty;

        void ShowUI()
        {
            m_Icon.spriteName = GetIconByUid(m_Uid);
            m_Frame.spriteName = GetFrameByUid(m_Uid);
            m_PlayerName.text = m_PlayerNameShadow.text = GetNameByUid(m_Uid);
            m_GangName.text = GetGangNameByUid(m_Uid);
            m_Fight.text = GetFightByUid(m_Uid).ToString();
            m_Level.text = GetLevelByUid(m_Uid).ToString();
            otherPlayerLevel = GetLevelByUid(m_Uid);
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            m_PK.gameObject.CustomSetActive(SceneLogicManager.getSceneType() != SceneLogicManager.SCENE_COMBAT);
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

        #region event handler

        public void OnCheakInfoBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            Hashtable mainData = Johny.HashtablePool.Claim();
            mainData.Add("name", m_PlayerName.text);
            mainData.Add("icon", m_Icon.spriteName);
            mainData.Add("headFrame", m_Frame.spriteName);
            int level = 0;
            int.TryParse(m_Level.text, out level);
            mainData.Add("level", level);
            Hashtable viewData = Johny.HashtablePool.Claim();
            viewData["mainData"] = mainData;
            viewData["infoType"] = eOtherPlayerInfoType.canInteraction;
            viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM0] = m_Uid;
            viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1_TYPE1;
            viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2_TYPE1;
            //viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM3] = data;

            GlobalMenuManager.Instance.Open("LTCheckPlayerFormationInfoUI", viewData);
        }

        public void OnAddFriendClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            var functpl = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10057);
            if (!functpl.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, functpl.GetConditionStrSpecial());
                return;
            }
            if (FriendManager.Instance.Info.MyFriendNum >= FriendManager.Instance.Config.MaxFriendNum)
            {
                MessageTemplateManager.ShowMessage(FriendManager.CodeFriendNumMax);
                return;
            }

            if (FriendManager.Instance.MyFriends.Find(m_Uid) != null)
            {
                MessageTemplateManager.ShowMessage(FriendManager.CodeHasFriend);
                controller.Close();
                return;
            }

            var ht = Johny.HashtablePool.Claim();
            ht.Add( "uid", m_Uid);
            ht.Add("addWay", eFriendAddWay.Normal);
            GlobalMenuManager.Instance.Open("FriendApplyUI", ht);
            controller.Close();
        }

        public void OnDelectFriendClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            var ht = Johny.HashtablePool.Claim();
            ht.Add("0", curPlayerName);
            MessageTemplateManager.ShowMessage(FriendManager.CodeDeleteFriend, ht, delegate (int result)
            {
                if (result == 0)
                {
                    FriendManager.Instance.Delete(m_Uid, eFriendType.My, delegate (bool successful)
                    {
                        FriendManager.Instance.MarkDirty(FriendManager.MyFriendListId);
                    });
                }
            });
            controller.Close();
        }

        public void OnChatClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            var functpl = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10057);
            if (!functpl.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, functpl.GetConditionStrSpecial());
                return;
            }
            if (!FriendHudController.sOpen)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("type", eFriendType.Recently);
                ht.Add("uid", m_Uid);
                GlobalMenuManager.Instance.Open("FriendHud", ht);
            }
            else
            {
               Messenger.Raise(Hotfix_LT.EventName.FriendOpenRecentlyEvent,m_Uid);
                
            }
            controller.Close();
        }

        public void OnPKClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            PK(m_Uid);
            //PK(m_Uid, m_PlayerName.text, OnInvitePK);
        }

        public void OnBlacklistClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            var functpl = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10057);
            if (!functpl.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, functpl.GetConditionStrSpecial());
                return;
            }
            if (FriendManager.Instance.BlackLists.List.Count >= FriendManager.Instance.Config.MaxBlacklistNum)
            {
                LTPartnerDataManager.Instance.ShowPartnerMessage(EB.Localizer.GetString("ID_codefont_in_OtherPlayerInfoController_5966"));
                return;
            }
            if (FriendManager.Instance.BlackLists.Find(m_Uid) != null)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_OtherPlayerInfoController_6170"));
                return;
            }

            MessageTemplateManager.ShowMessage(FriendManager.CodeTrueIntoBlack, null, delegate (int result)
             {
                 if (result == 0)
                 {
                     FriendManager.Instance.Blacklist(m_Uid, delegate (bool successful)
                     {
                         FriendData f = new FriendData();
                         f.Uid = m_Uid;
                         FriendManager.Instance.BlackLists.Add(f);
                         Messenger.Raise(Hotfix_LT.EventName.FriendAddToBlacklistEvent,m_Uid);
                         var ht = Johny.HashtablePool.Claim();
                         ht.Add("0", m_PlayerName.text);
                         MessageTemplateManager.ShowMessage(FriendManager.CodeIntoBlack, ht, null);
                     });
                 }
             });
            controller.Close();
        }
        #endregion

        static public void PK(long uid)
        {
            if (AllianceUtil.GetIsInTransferDart("ID_PLAYER_NO_BLANK"))
            {
                return;
            }

            if (GetIsRedName(uid))
            {
                int aid = GetAidByUid(uid);
                if (aid > 0 && AllianceUtil.GetIsInAlliance(uid))
                {
                    MessageTemplateManager.ShowMessage(902095);
                    return;
                }
            }
            SocialIntactManager.Instance.SocialCombat(uid, (resp)=>
            {
                DataLookupsCache.Instance.CacheData(resp.hashtable);
            });
        }

        //暂未使用，需要保留防止后面重新启用
        static public void PK(long uid, string targetName, System.Action<EB.Sparx.Response> OnInvitePK)
        {
            if (AllianceUtil.GetIsInTransferDart("ID_PLAYER_NO_BLANK"))
            {
                return;
            }

            if (GetIsRedName(uid))
            {
                int aid = GetAidByUid(uid);
                if (aid > 0 && AllianceUtil.GetIsInAlliance(uid))
                {
                    MessageTemplateManager.ShowMessage(902095);
                    return;
                }
            }
            SocialIntactManager.Instance.InvitePVP(uid, OnInvitePK);
        }

        static private IEnumerator ShowTimeoutMsgCoroutine()
        {
            yield return new WaitForSeconds(11f);
            if (PKRejectMessage.IsPkReject)
                yield break;
            MessageTemplateManager.ShowMessage(902069);
        }

        public void OnInvitePK(EB.Sparx.Response result)
        {
            EB.Debug.Log("OnInvitePK: {0}" ,result.text);
            if (!result.sucessful)
                MessageDialog.Show(EB.Localizer.GetString("ID_DIALOG_TITLE_TIPS"), result.localizedError, EB.Localizer.GetString("ID_DIALOG_TITLE_CONFIRM"), null, false, true, true, null, NGUIText.Alignment.Center);
            controller.Close();

            if (result.hashtable != null)
            {
                int errCode = EB.Dot.Integer("errCode", result.hashtable, -1);
                switch ((PkErrCode)errCode)
                {
                    case PkErrCode.Busying:
                        MessageTemplateManager.ShowMessage(902069);
                        return;
                    case PkErrCode.NotReceivePkRequest:
                        EB.Debug.Log("receivePkRequest = false");
                        MessageTemplateManager.ShowMessage(902069);
                        return;
                    default:
                        break;
                }
            }
            if (curType == OtherPlayerInfoViewType.FROM_CHAT)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("name", curPlayerName);
                ht.Add("uid", m_Uid);
                GlobalMenuManager.Instance.Open("PkSendRequestUI", ht);
                var ht2 = Johny.HashtablePool.Claim();
                ht2.Add("0", curPlayerName);
                MessageTemplateManager.ShowMessage(902145, ht2, null);
                Johny.HashtablePool.Release(ht2);
            }
            else
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("name", GetNameByUid(m_Uid) );
                ht.Add("uid", m_Uid);
                GlobalMenuManager.Instance.Open("PkSendRequestUI", ht);
                var ht2 = Johny.HashtablePool.Claim();
                ht2.Add("0", GetNameByUid(m_Uid) );
                MessageTemplateManager.ShowMessage(902145, ht2, null);
                Johny.HashtablePool.Release(ht2);
            }
        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }

        public override void SetMenuData(object _menuData)
        {
            if (_menuData == null)
            {
                long uid;
                if (DataLookupsCache.Instance.SearchDataByID<long>("intact.player", out uid))
                {
                    Uid = uid;
                }
                curType = OtherPlayerInfoViewType.FROM_PLAYER_INTACT;
            }
            else
            {
                Hashtable hashData = _menuData as Hashtable;
                string icon = "";
                string frame = "0_0";
                string name = "";
                string alliance_name = "";
                int level = 0;
                int fight = 0;
                m_Uid = EB.Dot.Long("uid", hashData, m_Uid); 
                icon = EB.Dot.String("icon", hashData, icon);
                frame = EB.Dot.String("headFrame", hashData, frame);
                name = EB.Dot.String("name", hashData, name);
                alliance_name = EB.Dot.String("a_name", hashData, alliance_name);
                level = EB.Dot.Integer("level", hashData, level);
                otherPlayerLevel = level;
                fight = EB.Dot.Integer("fight", hashData, fight);

                m_Icon.spriteName = icon;
                m_Frame.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(frame).iconId;
                m_PlayerName.text = m_PlayerNameShadow.text = name;
                m_GangName.text = alliance_name;
                m_Fight.text = fight.ToString();
                m_Level.text = level.ToString();
                
                m_GangName.gameObject.CustomSetActive(true);
                m_Fight.gameObject.CustomSetActive(true);
                m_Level.gameObject.CustomSetActive(true);

                curType = OtherPlayerInfoViewType.FROM_CHAT;
                curPlayerName = name;
            }

            if (FriendManager.Instance.MyFriends.Find(m_Uid) != null)
            {
                m_AddFriend.gameObject.CustomSetActive(false);
                m_DelectFriend.gameObject.CustomSetActive(true);
            }
            else
            {
                m_AddFriend.gameObject.CustomSetActive(true);
                m_DelectFriend.gameObject.CustomSetActive(false);
            }
        }



        public static string GetIconByUid(long uid)
        {
            string templateid;
            if (DataLookupsCache.Instance.SearchDataByID<string>(SceneLogicManager.getMultyPlayerSceneType() + ".pl." + uid + ".tid", out templateid))
            {
                string characterid = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacterid(templateid);
                int skin = 0;
                DataLookupsCache.Instance.SearchIntByID(SceneLogicManager.getMultyPlayerSceneType() + ".pl." + uid + ".skin", out skin);
                var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterid, skin);
                if (charTpl != null)
                {
                    return charTpl.icon /*+ "_" + type*/;
                }
            }
            return null;
        }

        public static string GetFrameByUid(long uid)
        {
            string temp = "";
            string name = "";
            if (DataLookupsCache.Instance.SearchDataByID<string>(SceneLogicManager.getMultyPlayerSceneType() + ".pl." + uid + ".hf", out temp))
            {
                name = EconemyTemplateManager.Instance.GetHeadFrame(temp).iconId;
            }
            return name;
        }

        public static string GetNameByUid(long uid)
        {
            string name = "";
            if (DataLookupsCache.Instance.SearchDataByID<string>(SceneLogicManager.getMultyPlayerSceneType() + ".pl." + uid + ".un", out name))
            {
                return name;
            }
            return name;
        }

        public static int GetAidByUid(long uid)
        {
            int aid;
            if (!DataLookupsCache.Instance.SearchIntByID(SceneLogicManager.getMultyPlayerSceneType() + ".pl." + uid + ".aid", out aid))
            {
                EB.Debug.LogError("playerIntact SearchDataByID aid fail scenename={0}", SceneLogicManager.getMultyPlayerSceneType());
                return 0;
            }
            return aid;
        }


        public static bool GetIsRedName(long uid)
        {
            bool isRedName;
            if (!DataLookupsCache.Instance.SearchDataByID<bool>(SceneLogicManager.getMultyPlayerSceneType() + ".pl." + uid + ".state.R", out isRedName))
            {
                EB.Debug.Log("<color=red>searchdata redname fail uid=</color>{0}" ,uid);
                return false;
            }
            return isRedName;
        }

        public static string GetGangNameByUid(long uid)
        {
            string name = "";
            if (DataLookupsCache.Instance.SearchDataByID<string>(SceneLogicManager.getMultyPlayerSceneType() + ".pl." + uid + ".gang_name", out name))
            {
                return name;
            }
            return name;
        }

        public static int GetFightByUid(long uid)
        {
            int fight;
            if (DataLookupsCache.Instance.SearchIntByID(SceneLogicManager.getMultyPlayerSceneType() + ".pl." + uid + ".fight", out fight))
            {
                return fight;
            }
            return fight;
        }

        public static int GetLevelByUid(long uid)
        {
            int level;
            if (DataLookupsCache.Instance.SearchIntByID(SceneLogicManager.getMultyPlayerSceneType() + ".pl." + uid + ".level", out level))
            {
                return level;
            }
            return level;
        }

    }
}
