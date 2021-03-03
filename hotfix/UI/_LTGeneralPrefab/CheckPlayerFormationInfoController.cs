using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Debug = EB.Debug;
using Language.Lua;

namespace Hotfix_LT.UI
{
    public class CheckPlayerFormationInfoController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }

        public UIWidget BG;
        public Transform InfoTran, PartnerTran;
        public GameObject BtnsObject;

        public UISprite PlayerIconSprite;
        public UISprite PlayerFrameSprite;
        public UILabel NameLabel;
        public UILabel LevelLabel;
        public UILabel AllianceNameLabel;
        public UILabel CombatPowerLabel;

        public Transform PartnerRoot;
        public List<FormationPartnerItem> PartnerItemList;
        public UIButton m_AddFriendBtn;
        public UIButton m_DelectFriendBtn;

        private long m_Uid;

        //int数组代表——BG\任务信息\阵容 前一的高度或后二的位置
        private Dictionary<eOtherPlayerInfoType, int[]> BGHeightDic = new Dictionary<eOtherPlayerInfoType, int[]>()
        {
            { eOtherPlayerInfoType.only ,new int[3]{736 ,200,-40}},
            { eOtherPlayerInfoType.canInteraction ,new int[3]{736,200,-40}},
            { eOtherPlayerInfoType.secret ,new int[3]{460 ,0,90}},
        };

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            BG = t.GetComponent<UIWidget>("BG");
            BtnsObject = t.FindEx("Content/Btns").gameObject;
            InfoTran = t.FindEx("Content/Base");
            PartnerTran = t.FindEx("Content/Formation");
            PlayerIconSprite = t.GetComponent<UISprite>("Content/Base/IconBG/Icon");
            PlayerFrameSprite= t.GetComponent<UISprite>("Content/Base/IconBG/Icon/Frame");
            NameLabel = t.GetComponent<UILabel>("Content/Base/Name");
            LevelLabel = t.GetComponent<UILabel>("Content/Base/LevelBG/Level");
            AllianceNameLabel = t.GetComponent<UILabel>("Content/Base/AllianceName");
            CombatPowerLabel = t.GetComponent<UILabel>("Content/Base/CombatPower/Container/Base");
            PartnerRoot = t.FindEx("Content/Formation/UIGrid");

            PartnerItemList = new List<FormationPartnerItem>();
            var childCount = PartnerRoot.childCount;

            for (var i = 0; i < childCount; i++)
            {
                PartnerItemList.Add(PartnerRoot.GetChild(i).GetMonoILRComponent<FormationPartnerItem>());
            }

            t.GetComponent<UIButton>("BG/Top/CloseBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButton>("Content/Btns/PK").onClick.Add(new EventDelegate(OnPKClick));
            t.GetComponent<UIButton>("Content/Btns/Chat").onClick.Add(new EventDelegate(OnChatClick));
            t.GetComponent<UIButton>("Content/Btns/Blacklist").onClick.Add(new EventDelegate(OnBlacklistClick));

            m_AddFriendBtn = t.GetComponent<UIButton>("Content/Btns/Add");
            m_AddFriendBtn.onClick.Add(new EventDelegate(OnAddFriendClick));
            m_DelectFriendBtn = t.GetComponent<UIButton>("Content/Btns/Delete");
            m_DelectFriendBtn.onClick.Add(new EventDelegate(OnDelectFriendClick));
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);

            Hashtable checkData = param as Hashtable;
            eOtherPlayerInfoType infoType = ParseInfoType(checkData);
            //在ilr模式下无法转成枚举
           // eOtherPlayerInfoType infoType = Hotfix_LT.EBCore.Dot.Enum<eOtherPlayerInfoType>("infoType", checkData, 0);
            m_Uid = EB.Dot.Long(SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM0, checkData, 0);

            int[] heightInfo = BGHeightDic[infoType];
            BG.height = heightInfo[0];

            InfoTran.localPosition = new Vector2(InfoTran.localPosition.x, heightInfo[1]);
            InfoTran.gameObject.CustomSetActive(infoType != eOtherPlayerInfoType.secret);
            PartnerTran.localPosition = new Vector2(InfoTran.localPosition.x, heightInfo[2]);
          
            if (infoType == eOtherPlayerInfoType.canInteraction)
            {
                if (FriendManager.Instance.MyFriends.Find(m_Uid) != null)
                {
                    m_AddFriendBtn.gameObject.CustomSetActive(false);
                    m_DelectFriendBtn.gameObject.CustomSetActive(true);
                }
                else
                {
                    m_AddFriendBtn.gameObject.CustomSetActive(true);
                    m_DelectFriendBtn.gameObject.CustomSetActive(false);
                }
            }

            Hashtable mainData = EB.Dot.Object("mainData", checkData, null);
            ShowMainInfo(mainData);

            string type = EB.Dot.String(SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1, checkData, null);
            string dataType = EB.Dot.String(SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2, checkData, null);
            Hashtable data = EB.Dot.Object(SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM3, checkData, null);
            var listener = this;
            string otherPlayerName = EB.Dot.String("name", mainData, "");

            PartnerRoot.gameObject.CustomSetActive(false);
            CombatPowerLabel.gameObject.CustomSetActive(false);
            ShowOtherPlayerData(m_Uid, type, dataType, data, listener, otherPlayerName);
            BtnsObject.CustomSetActive(infoType == eOtherPlayerInfoType.canInteraction);
        }

        private void Test()
        {
          
        }

        private void ShowOtherPlayerData(long m_Uid, string type, string dataType, Hashtable data, CheckPlayerFormationInfoController listener, string otherPlayerName)
        {
            LTFormationDataManager.Instance.GetOtherPlayerData(m_Uid, type, dataType, data, delegate (List<OtherPlayerPartnerData> partnerDataList, string allianceName)
            {
                if (partnerDataList == null)
                {
                    controller.Close();
                    return;
                }

                if (listener == null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(allianceName))
                {
                    LTUIUtil.SetText(AllianceNameLabel, string.Format("【{0}】", allianceName));
                }
                else
                {
                    LTUIUtil.SetText(AllianceNameLabel, EB.Localizer.GetString("ID_codefont_in_CheckPlayerFormationInfoController_2801"));
                }

                var count = partnerDataList.Count;

                for (int i = 0; i < count; i++)
                {
                    partnerDataList[i].otherPlayerName = otherPlayerName;
                }

                PartnerRoot.gameObject.CustomSetActive(true);
                ShowFormationInfo(partnerDataList);
                float allpower = 0f;
                for (int i = 0; i < partnerDataList.Count; i++)
                {
                    if (partnerDataList[i]!=null && partnerDataList[i].FinalAttributes != null)
                    {
                        allpower += partnerDataList[i].GetOtherPower();
                    }
                }
                LTUIUtil.SetText(CombatPowerLabel, ((int)allpower).ToString());
                CombatPowerLabel.gameObject.CustomSetActive(true);

            });
       
        }

        public eOtherPlayerInfoType ParseInfoType(Hashtable checkData)
        {
            eOtherPlayerInfoType infoType=eOtherPlayerInfoType.only;
            if (checkData["infoType"]!=null)
            {
                infoType =(eOtherPlayerInfoType)Enum.Parse(typeof(eOtherPlayerInfoType), checkData["infoType"].ToString()) ;
            }
            return infoType;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            PlayerIconSprite.spriteName = PlayerFrameSprite.spriteName = string.Empty;
            LTUIUtil.SetText(NameLabel, string.Empty);
            LTUIUtil.SetText(LevelLabel, string.Empty);
            LTUIUtil.SetText(AllianceNameLabel, string.Empty);
            DestroySelf();
            yield break;
        }

        public void ShowMainInfo(Hashtable mainInfo)
        {
            if (mainInfo == null)
            {
                return;
            }

            PlayerIconSprite.spriteName = EB.Dot.String("icon", mainInfo, "");
            PlayerFrameSprite.spriteName = EB.Dot.String("headFrame", mainInfo, "");
            LTUIUtil.SetText(NameLabel, EB.Dot.String("name", mainInfo, ""));
            LTUIUtil.SetText(LevelLabel, EB.Dot.Integer("level", mainInfo, 0).ToString());
        }

        public void ShowFormationInfo(List<OtherPlayerPartnerData> dataList)
        {
            if (dataList == null || dataList.Count <= 0)
            {
                return;
            }

            var len1 = PartnerItemList.Count;
            var len2 = dataList.Count;

            for (int i = 0; i < len1; ++i)
            {
                var item = PartnerItemList[i];

                if (i < len2)
                {
                    item.mDMono.gameObject.CustomSetActive(true);
                    item.Fill(dataList[i]);
                }
                else
                {
                    item.mDMono.gameObject.CustomSetActive(false);
                }
            }

            PartnerRoot.GetComponent<UIGrid>().enabled = true;
        }

        #region event handler
        public void OnAddFriendClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            //int level = BalanceResourceUtil.GetUserLevel(); ;
            var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10057);
            if (!func.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, func.GetConditionStr());
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
            ht.Add( "0", NameLabel.text);
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
            Johny.HashtablePool.Release(ht);
        }

        public void OnChatClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10057);
            if (!func.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, func.GetConditionStr());
                return;
            }

            if (!FriendHudController.sOpen)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("type", eFriendType.Recently);
                ht.Add( "uid", m_Uid);
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
            string Name = NameLabel.text;
            OtherPlayerInfoController.PK(m_Uid);
            //OtherPlayerInfoController.PK(m_Uid, Name, delegate (EB.Sparx.Response result)
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
            //                    ht.Add("name", Name);
            //                    ht.Add("uid", m_Uid );
            //                    GlobalMenuManager.Instance.Open("PkSendRequestUI", ht);
            //                    break;
            //            }
            //        }
            //    }
            //});
        }

        public void OnBlacklistClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10057);
            if (!func.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, func.GetConditionStr());
                return;
            }

            if (FriendManager.Instance.BlackLists.List.Count >= FriendManager.Instance.Config.MaxBlacklistNum)
            {
                LTPartnerDataManager.Instance.ShowPartnerMessage(EB.Localizer.GetString("ID_codefont_in_OtherPlayerInfoController_5966"));   // 暂时
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
                        ht.Add( "0", NameLabel.text);
                        MessageTemplateManager.ShowMessage(FriendManager.CodeIntoBlack, ht, null);
                    });
                }
            });
            controller.Close();
        }
        #endregion
    }
}