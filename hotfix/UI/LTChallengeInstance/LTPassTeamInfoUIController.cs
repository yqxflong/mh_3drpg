using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hotfix_LT.UI
{
    public class LTPassTeamInfoItem : DynamicMonoHotfix
    {
        private GameObject EmptyObj;
        private GameObject PlayerInfoObj;
        private UISprite PlayerIcon;
        private UISprite PlayerFrame;
        private UILabel PlayerLevel;
        private UIGrid ItemGrid;
        private FormationPartnerItem ItemPrefab;

        private UILabel InfoLabel;
        private UILabel TimeLabel;

        private List<FormationPartnerItem> ItemList;

        public override void Awake()
        {
            base.Awake();
            EmptyObj = mDMono.transform.Find("Empty").gameObject;

            PlayerInfoObj = mDMono.transform.Find("PlayerInfo").gameObject;
            PlayerIcon = PlayerInfoObj.transform.Find("Icon").GetComponent<UISprite>();
            PlayerFrame = PlayerIcon.transform.Find("Frame").GetComponent<UISprite>();
            PlayerLevel = PlayerInfoObj.transform.Find("LevelBG/Level").GetComponent<UILabel>();
            ItemGrid = PlayerInfoObj.transform.Find("UIGrid").GetComponent<UIGrid>();
            ItemPrefab = ItemGrid.transform.Find("Item").GetMonoILRComponent<FormationPartnerItem>();

            InfoLabel = mDMono.transform.Find("InfoLabel").GetComponent<UILabel>();
            TimeLabel = mDMono.transform.Find("TimeLabel").GetComponent<UILabel>();

            ItemList = new List<FormationPartnerItem>();
        }

        public void OnFill(Hashtable hash, string timeStr = null)
        {
            if (hash == null || hash.Count == 0)
            {
                EmptyObj.CustomSetActive(true);
                PlayerInfoObj.CustomSetActive(false);
                InfoLabel.text = EB.Localizer.GetString("ID_PASS_INFO_NOBODY_IN_RANK");
                TimeLabel.text = string.Format("{0}{1}", timeStr, EB.Localizer.GetString("ID_NATION_BATTLE_BUFF_FULL_CALL"));
            }
            else
            {
                if (hash.Values!=null)
                foreach (Hashtable Temp in hash.Values)
                {
                    EmptyObj.CustomSetActive(false);
                    PlayerInfoObj.CustomSetActive(true);
                    string Tid = EB.Dot.String("userTeam.normal.userInfo.leaderId", Temp, null);
                    Tid = (Tid == null) ? "10011" : Tid;
                    int skin = EB.Dot.Integer("userTeam.normal.userInfo.skin", Temp, 0);
                    string characterid = Hotfix_LT.Data.CharacterTemplateManager.Instance.TemplateidToCharacterid(Tid);
                    var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(characterid, skin);

                    PlayerIcon.spriteName = charTpl.icon;
                    string frameStr = EB.Dot.String("userTeam.normal.userInfo.headFrame", Temp, null);
                    PlayerFrame.spriteName = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetHeadFrame(frameStr).iconId;
                    PlayerLevel.text = EB.Dot.String("userTeam.normal.userInfo.level", Temp, null);

                    InfoLabel.text = EB.Dot.String("userTeam.normal.userInfo.name", Temp, null);
                    TimeLabel.text = timeStr;

                    var teamInfo = LTFormationDataManager.Instance.GetOtherPalyerPartnerDataList("normal", Temp);
                    for (int i = 0; i < 6; ++i)
                    {
                        if (teamInfo.Count > i)//有数据
                        {
                            if (ItemList.Count > i)
                            {
                                ItemList[i].Fill(teamInfo[i]);
                            }
                            else
                            {
                                FormationPartnerItem item =UIControllerHotfix.InstantiateEx<FormationPartnerItem>(ItemPrefab, ItemGrid.transform);
                                item.Fill(teamInfo[i]);
                                ItemList.Add(item);
                            }
                        }
                        else//无数据
                        {
                            if (ItemList.Count > i)
                            {
                                ItemList[i].Fill(null);
                            }
                        }
                    }
                    ItemGrid.repositionNow = true;
                }
            }
        }
    }

    public class LTPassTeamInfoUIController : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }
        public override bool IsFullscreen() { return false; }

        private bool hasRequest = false;

        private const string TheFastStr = "fast";
        private const string TheNearStr = "newest";
        private LTPassTeamInfoItem TheFast, TheNear;

        public override void Awake()
        {
            base.Awake();
            UIButton backBtn = controller.transform.Find("BG/Top/CloseBtn").GetComponent<UIButton>();
            controller.backButton = backBtn;
            TheFast = controller.transform.GetMonoILRComponent<LTPassTeamInfoItem>("Content/TheFast");
            TheNear = controller.transform.GetMonoILRComponent<LTPassTeamInfoItem>("Content/TheNear");
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            if (!hasRequest)
            {
                EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/lostchallengecampaign/getTeamViews");
                request.AddData("level", LTInstanceMapModel.Instance.CurLevelNum);
                LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
                {
                    if (data != null)
                    {
                        hasRequest = true;
                        var array = Hotfix_LT.EBCore.Dot.Array("teamView", data, null);
                        TheFast.OnFill(null, EB.Localizer.GetString("ID_PASS_INFO_TIME"));
                        TheNear.OnFill(null, EB.Localizer.GetString("ID_PASS_INFO_TIME2"));
                        if (array != null)
                        {
                            for (int i = 0; i < array.Count; ++i)
                            {
                                Hashtable hash = array[i] as Hashtable;
                                string type = EB.Dot.String("type", hash, null);
                                if (type.Equals(TheFastStr))
                                {
                                    int time = EB.Dot.Integer("fastTime", hash, 0);
                                    string timeTemp = string.Format(EB.Localizer.GetString("ID_PASS_INFO_TIME3"), time / 60 / 60, time / 60 % 60, time % 60);
                                    string timeStr = string.Format("{0}{1}", EB.Localizer.GetString("ID_PASS_INFO_TIME"), timeTemp);
                                    TheFast.OnFill(EB.Dot.Object("teamView", hash, null), timeStr);
                                }
                                else if (type.Equals(TheNearStr))
                                {
                                    int time = EB.Dot.Integer("ts", hash, 0);
                                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                                    DateTime dt = startTime.AddSeconds(time);
                                    string timeTemp = string.Format(EB.Localizer.GetString("ID_PASS_INFO_TIME4"), dt.Year, dt.Month, dt.Day);
                                    string timeStr = string.Format("{0}{1}", EB.Localizer.GetString("ID_PASS_INFO_TIME2"), timeTemp);
                                    TheNear.OnFill(EB.Dot.Object("teamView", hash, null), timeStr);
                                }
                            }
                        }
                    }
                });
            }
        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    }
}
