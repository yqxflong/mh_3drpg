using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class CommonConditionParse
    {
        public static string FocusViewName;

        //private int _groupId;
        public CommonConditionParse(/*int groupId*/)
        {
            //_groupId = groupId;
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.None, OnNone);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetGuideComplete, GetGuideComplete);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetFocusView, GetFocusView);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetBattleType, GetBattleType);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetDialogueState, GetDialogueState);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.BattleReady, BattleReady);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.CurMapNode, CurMapNode);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetPathObjAction, GetPathObjAction);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetParticipantNum, GetParticipantNum);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetNeedSetSkill, GetNeedSetSkill);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetGuideType, GetGuideType);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetFuncIsOpen, GetFuncIsOpen);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetFuncBtnAction, GetFuncBtnAction);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetFindGate, GetFindGate);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetMengBanState, GetMengBanState);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetChallengeDiedAction, GetChallengeDiedAction);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetChallengeLevel, GetChallengeLevel);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.CheckBattleIsFailed, CheckBattleIsFailed);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.OnLessThanLevel, OnLessThanLevel);
            NodeMessageManager.GetInstance().AddCondition(/*_groupId,*/ NodeMessageManager.GetGuideFailState, GetGuideFailState);
        }

        public void Dispose()
        {
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.None, OnNone);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetGuideComplete, GetGuideComplete);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetFocusView, GetFocusView);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetDialogueState, GetDialogueState);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.BattleReady, BattleReady);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.CurMapNode, CurMapNode);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetPathObjAction, GetPathObjAction);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetParticipantNum, GetParticipantNum);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetNeedSetSkill, GetNeedSetSkill);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetGuideType, GetGuideType);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetFuncIsOpen, GetFuncIsOpen);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetFuncBtnAction, GetFuncBtnAction);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetFindGate, GetFindGate);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetMengBanState, GetMengBanState);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetChallengeDiedAction, GetChallengeDiedAction);//GetChallengeLevel
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetChallengeLevel, GetChallengeLevel);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.CheckBattleIsFailed, CheckBattleIsFailed);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.OnLessThanLevel, OnLessThanLevel);
            NodeMessageManager.GetInstance().RemoveCondition(/*_groupId,*/ NodeMessageManager.GetGuideFailState, GetGuideFailState);
        }

        public static void SetFocusViewName(string name)
        {
            FocusViewName = name;
        }
        
        void DispatchConditionReceipt(string em, string paramater)
        {
            NodeMessageManager.GetInstance().DispatchConditionReceipt(/*_groupId,*/ em, paramater);
        }

        private void OnNone(string str)
        {
            DispatchConditionReceipt(NodeMessageManager.None, string.Empty);
        }

        private void GetFocusView(string s)
        {
            DispatchConditionReceipt(NodeMessageManager.GetFocusView, FocusViewName);
        }

        private void GetGuideComplete(string strGuideNode)
        {
            int guideNodeID = 0;
            int.TryParse(strGuideNode, out guideNodeID);
            string result = NodeMessageManager.Fail;
            if (guideNodeID != 0)
            {
                GuideNode node = GuideNodeManager.GetInstance().GetNode(guideNodeID);

                if (node == null)
                {
                    EB.Debug.LogError("GetGuideComplete guidNode == null guideNodeStepID={0}" , guideNodeID);
                }
                else
                {
                    if (GuideNodeManager.GetInstance().IsLinkCompleted(node))
                    {
                        result = NodeMessageManager.Sucess;
                    }
                }
            }
            else
            {
                string[] split = strGuideNode.Split(',');
                for (int i = 0; i < split.Length; i++)
                {
                    guideNodeID = int.Parse(split[i]);
                    GuideNode node = GuideNodeManager.GetInstance().GetNode(guideNodeID);

                    if (node == null)
                    {
                        EB.Debug.LogError("GetGuideComplete guidNode == null guideNodeStepID={0}" , guideNodeID);
                    }
                    else
                    {
                        if (GuideNodeManager.GetInstance().IsLinkCompleted(node))
                        {
                            result = NodeMessageManager.Sucess;
                            break;
                        }
                    }
                }
            }
            DispatchConditionReceipt(NodeMessageManager.GetGuideComplete, result);
        }

        private void GetBattleType(string str)
        {
            string strEnum = SceneLogic.BattleType.ToString();
            DispatchConditionReceipt(NodeMessageManager.GetBattleType, strEnum);
        }

        private void GetDialogueState(string str)
        {
            bool state = DialoguePlayUtil.Instance.State;
            DispatchConditionReceipt(NodeMessageManager.GetDialogueState, state ? NodeMessageManager.Sucess : NodeMessageManager.Fail);
        }

        private void BattleReady(string str)
        {
            string rstr = NodeMessageManager.Fail;

            // bool isReady = Hotfix_LT.Messenger.RaiseEx<bool>(Hotfix_LT.EventName.IsReady);
            bool isReady = (bool)GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "IsReady");

            if (LTCombatEventReceiver.Instance!=null)
            {
                if (LTCombatEventReceiver.Instance.Ready)
                {
                    rstr = NodeMessageManager.Sucess;
                }
            }
            if (isReady)
            {
                rstr = NodeMessageManager.Sucess;
            }
            DispatchConditionReceipt(NodeMessageManager.BattleReady, rstr);
        }

        //弃用
        private void CurMapNode(string path)
        {
            DispatchConditionReceipt(NodeMessageManager.CurMapNode, NodeMessageManager.Fail);
        }

        private void GetPathObjAction(string path)
        {
            if (GameObject.Find(path) != null && GameObject.Find(path).activeInHierarchy)
            {
                DispatchConditionReceipt(NodeMessageManager.GetPathObjAction, NodeMessageManager.Sucess);
            }
            else
            {
                DispatchConditionReceipt(NodeMessageManager.GetPathObjAction, NodeMessageManager.Fail);
            }
        }

        private void GetParticipantNum(string str)
        {
            int num = 0;
            Hashtable userTeamData;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userTeam", out userTeamData);
            ArrayList teamHash = Hotfix_LT.EBCore.Dot.Array("team1.formation_info", userTeamData, null);
            for (var i = 0; i < teamHash.Count; i++)
            {
                var teamMemData = teamHash[i];
                IDictionary teamMemDataDic = teamMemData as IDictionary;
                if (teamMemDataDic == null || !teamMemDataDic.Contains(SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID) || teamMemDataDic[SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID] == null)
                    continue;
                int nHeroID = EB.Dot.Integer(SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID, teamMemDataDic, 0); ;
                if (nHeroID <= 0) continue;
                num++;
            }
            DispatchConditionReceipt(NodeMessageManager.GetParticipantNum, num.ToString());
        }

        private void GetNeedSetSkill(string str)
        {
            DispatchConditionReceipt(NodeMessageManager.GetNeedSetSkill, (Combat.CombatSyncData.Instance.NeedSetSkill) ? NodeMessageManager.Sucess : NodeMessageManager.Fail);
        }

        private void GetGuideType(string str)
        {
            DispatchConditionReceipt(NodeMessageManager.GetGuideType, (GuideNodeManager.IsGuide) ? NodeMessageManager.Sucess : NodeMessageManager.Fail);
        }

        private void GetFuncIsOpen(string str)
        {
            int funcId = int.Parse(str);
            var Func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(funcId);
            DispatchConditionReceipt(NodeMessageManager.GetFuncIsOpen, (Func.IsConditionOK()) ? NodeMessageManager.Sucess : NodeMessageManager.Fail);
        }

        private void GetFuncBtnAction(string str)
        {
            if (GameObject.Find("MainHudUI/PanelDynamic/Anchor_Center/LTMainMenu/Edge/Bottom/NPCFuncList") != null && GameObject.Find("MainHudUI/PanelDynamic/Anchor_Center/LTMainMenu/Edge/Bottom/NPCFuncList").activeSelf)
            {
                GameObject obj = GameObject.Find("MainHudUI/PanelDynamic/Anchor_Center/LTMainMenu/Edge/Bottom/NPCFuncList");
                var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(int.Parse(str));
                if (obj.transform.Find("Grid/IconBtn1/Label").GetComponent<UILabel>().text == func.display_name) DispatchConditionReceipt(NodeMessageManager.GetFuncBtnAction, NodeMessageManager.Sucess);
                else DispatchConditionReceipt(NodeMessageManager.GetFuncBtnAction, NodeMessageManager.Fail);
            }
            else
            {
                DispatchConditionReceipt(NodeMessageManager.GetFuncBtnAction, NodeMessageManager.Fail);
            }
        }

        private void GetFindGate(string str)
        {
            DispatchConditionReceipt(NodeMessageManager.GetFindGate, (string.IsNullOrEmpty(GuideNodeManager.GateString)) ? NodeMessageManager.Fail : NodeMessageManager.Sucess);
        }

        private void GetMengBanState(string str)
        {
            DispatchConditionReceipt(NodeMessageManager.GetMengBanState, (MengBanController.Instance.m_State) ? NodeMessageManager.Sucess : NodeMessageManager.Fail);
        }

        private void GetChallengeDiedAction(string str)
        {
            bool isCombating = false;
            if (LTCombatHudController.Instance != null && Combat.CombatSyncData.Instance != null)
            {
                isCombating = Combat.CombatSyncData.Instance.GetDiedCharacterList();
            }
            DispatchConditionReceipt(NodeMessageManager.GetChallengeDiedAction, (isCombating) ? NodeMessageManager.Sucess : NodeMessageManager.Fail);
        }

        private void GetChallengeLevel(string str)
        {
            bool isSucess = false;
            int CurLevelNum = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.level", out CurLevelNum);
            if (str.Equals("boss"))
            {
                var data = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeChapterById(CurLevelNum);
                isSucess = data.IsBoss;
            }
            else
            {
                int level = int.Parse(str);
                isSucess = (level == CurLevelNum);
            }
            DispatchConditionReceipt(NodeMessageManager.GetChallengeLevel, isSucess ? NodeMessageManager.Sucess : NodeMessageManager.Fail);
        }

        private string GetBattleIsFailedKey(eBattleType type)
        {
            return string.Format("{0}-{1}-{2}", LoginManager.Instance.LocalUserId.Value, NodeMessageManager.CheckBattleIsFailed, type.ToString());
        }

        /// <summary>
        /// 检查战斗是否为失败
        /// </summary>
        /// <param name="str"></param>
        private void CheckBattleIsFailed(string str)
        {
            bool isFail = false;
            int level = 0;

            switch (SceneLogic.BattleType)
            {
                case eBattleType.InfiniteChallenge:
                    if (BattleResultScreenController.Instance != null)
                    {
                        DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.currentlayer", out level);
                        isFail = BattleResultScreenController.Instance.DefeatRating.activeSelf;
                    }
                    break;
                case eBattleType.AwakeningBattle:
                    if (BattleResultScreenController.Instance != null)
                    {
                        level = LTAwakeningInstanceManager.Instance.BattleType;
                        isFail = BattleResultScreenController.Instance.DefeatRating.activeSelf;
                    }
                    break;
                case eBattleType.MainCampaignBattle:
                    if (BattleResultScreenController.Instance != null)
                    {
                        level = LTMainInstanceCampaignCtrl.CampaignId;
                        isFail = BattleResultScreenController.Instance.DefeatRating.activeSelf;
                    }
                    break;             
                case eBattleType.ChallengeCampaign:
                    if (LTChallengeInstanceDefaultCtrl.Instance != null)
                    {
                        DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.level", out level);
                        isFail = !LTChallengeInstanceDefaultCtrl.Instance.isWon;
                    }
                    break;
            }

            string result = "None";

            if (isFail && LTPartnerDataManager.Instance != null)
            {
                string key = GetBattleIsFailedKey(SceneLogic.BattleType);
                string value = PlayerPrefs.GetString(key);
                var wordList = new HashSet<string>(value.Split(','));

                // 同一场战斗，只在首次失败时触发引导
                if (!wordList.Contains(level.ToString()))
                {
                    if (LTPartnerDataManager.Instance.HasPartnerCanLevelUp())
                    {
                        result = "Level";
                    }
                    else if (LTPartnerDataManager.Instance.HasEquipmentCanDress())
                    {
                        result = "Equipment";
                    }

                    PlayerPrefs.SetString(key, string.Format("{0},{1}", value, level));
                    PlayerPrefs.Save();
                }
            }

            DispatchConditionReceipt(NodeMessageManager.CheckBattleIsFailed, result);
        }

        /// <summary>
        /// 当玩家等级小于指定等级
        /// </summary>
        /// <param name="level"></param>
        public void OnLessThanLevel(string str)
        {
            int funcId = int.Parse(str);
            var Func = Data.FuncTemplateManager.Instance.GetFunc(funcId);
            int user_level = (EB.Sparx.Hub.Instance.LevelRewardsManager.Level == 0) ? LoginManager.Instance.LocalUser.Level : EB.Sparx.Hub.Instance.LevelRewardsManager.Level;
            bool isSucess = user_level < Func.conditionParam;
            DispatchConditionReceipt(NodeMessageManager.OnLessThanLevel, isSucess ? NodeMessageManager.Sucess : NodeMessageManager.Fail);
        }

        public void GetGuideFailState(string str)
        {
            DispatchConditionReceipt(NodeMessageManager.GetGuideFailState, GuideNodeManager.GuideFailState);
        }
    }
}