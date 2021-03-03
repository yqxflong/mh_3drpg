using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class NationBattleSelectTeamController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;

            PathTabLabeList = new UILabel[3];
            PathTabLabeList[0] = t.GetComponent<UILabel>("Content/PathTabGrid/0/Label");
            PathTabLabeList[1] = t.GetComponent<UILabel>("Content/PathTabGrid/1/Label");
            PathTabLabeList[2] = t.GetComponent<UILabel>("Content/PathTabGrid/2/Label");

            UIPathToggles = new UIToggle[3];
            UIPathToggles[0] = t.GetComponent<UIToggle>("Content/PathTabGrid/0");
            UIPathToggles[1] = t.GetComponent<UIToggle>("Content/PathTabGrid/1");
            UIPathToggles[2] = t.GetComponent<UIToggle>("Content/PathTabGrid/2");

            TeamItems = new NationBattleSelectTeamCell[3];
            TeamItems[0] = t.GetMonoILRComponent<NationBattleSelectTeamCell>("Content/TeamList/0");
            TeamItems[1] = t.GetMonoILRComponent<NationBattleSelectTeamCell>("Content/TeamList/1");
            TeamItems[2] = t.GetMonoILRComponent<NationBattleSelectTeamCell>("Content/TeamList/2");

            FormationPartnerItems = new FormationPartnerItemEx[12];
            FormationPartnerItems[0] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/0/PartnerUIGrid/0_Pos", "Hotfix_LT.UI.FormationPartnerItemEx");
            FormationPartnerItems[1] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/0/PartnerUIGrid/0_Pos (1)", "Hotfix_LT.UI.FormationPartnerItemEx");
            FormationPartnerItems[2] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/0/PartnerUIGrid/0_Pos (2)", "Hotfix_LT.UI.FormationPartnerItemEx");
            FormationPartnerItems[3] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/0/PartnerUIGrid/0_Pos (3)", "Hotfix_LT.UI.FormationPartnerItemEx");

            FormationPartnerItems[4] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/1/PartnerUIGrid/0_Pos", "Hotfix_LT.UI.FormationPartnerItemEx");
            FormationPartnerItems[5] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/1/PartnerUIGrid/0_Pos (1)", "Hotfix_LT.UI.FormationPartnerItemEx");
            FormationPartnerItems[6] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/1/PartnerUIGrid/0_Pos (2)", "Hotfix_LT.UI.FormationPartnerItemEx");
            FormationPartnerItems[7] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/1/PartnerUIGrid/0_Pos (3)", "Hotfix_LT.UI.FormationPartnerItemEx");

            FormationPartnerItems[8] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/2/PartnerUIGrid/0_Pos", "Hotfix_LT.UI.FormationPartnerItemEx");
            FormationPartnerItems[9] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/2/PartnerUIGrid/0_Pos (1)", "Hotfix_LT.UI.FormationPartnerItemEx");
            FormationPartnerItems[10] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/2/PartnerUIGrid/0_Pos (2)", "Hotfix_LT.UI.FormationPartnerItemEx");
            FormationPartnerItems[11] = t.GetMonoILRComponentByClassPath<FormationPartnerItemEx>("Content/TeamList/2/PartnerUIGrid/0_Pos (3)", "Hotfix_LT.UI.FormationPartnerItemEx");

            Path = string.Empty;
            LeftReviveTimeLabel = t.GetComponent<UILabel>("Content/TimeBtn/Countdown");
            ReviveButton = t.GetComponent<UIButton>("Content/ReviveBtn");
            TimeButton = t.GetComponent<UIButton>("Content/TimeBtn");
            isAllTeamDeath = false;
            ReviveCostLabel = t.GetComponent<UILabel>("Content/ReviveBtn/HC");
            controller.backButton = t.GetComponent<UIButton>("LTPopFrame/CloseBtn");

            t.GetComponent<UIButton>("Content/ReviveBtn").onClick.Add(new EventDelegate(() => OnReviveBtnClick(null)));
            t.GetComponent<UIButton>("Content/TimeBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));


            t.GetComponent<UIToggle>("Content/PathTabGrid/0").onChange.Add(new EventDelegate(() => OnSelectPathClick(t.GetComponent<UIToggle>("Content/PathTabGrid/0"))));
            t.GetComponent<UIToggle>("Content/PathTabGrid/1").onChange.Add(new EventDelegate(() => OnSelectPathClick(t.GetComponent<UIToggle>("Content/PathTabGrid/1"))));
            t.GetComponent<UIToggle>("Content/PathTabGrid/2").onChange.Add(new EventDelegate(() => OnSelectPathClick(t.GetComponent<UIToggle>("Content/PathTabGrid/2"))));
        }



        public override bool ShowUIBlocker { get { return true; } }

        public UILabel[] PathTabLabeList;
        public UIToggle[] UIPathToggles;
        public NationBattleSelectTeamCell[] TeamItems;
        public FormationPartnerItemEx[] FormationPartnerItems;
        public string Path;
        public UILabel LeftReviveTimeLabel;
        public UIButton ReviveButton;
        public UIButton TimeButton;
        public bool isAllTeamDeath;
        public UILabel ReviveCostLabel;
        private Coroutine ReviveCoroutine;

        public string[] teamStartActionState;

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            isAllTeamDeath = true;
            //ReviveButton.gameObject.CustomSetActive(true);
            //TimeButton.gameObject.CustomSetActive(true);
            Hashtable ht = param as Hashtable;
            int toggleIndex = (int)ht["path"];
            bool isAttack = (bool)ht["isAttack"];
            if (isAttack)
            {
                LTUIUtil.SetText(PathTabLabeList[0], EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamController_1093"));
                LTUIUtil.SetText(PathTabLabeList[1], EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamController_1143"));
                LTUIUtil.SetText(PathTabLabeList[2], EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamController_1193"));
            }
            else
            {
                Vector3 pos = UIPathToggles[0].transform.parent.transform.localPosition;
                UIPathToggles[0].transform.parent.transform.localPosition = new Vector3(1083, pos.y, pos.z);
                LTUIUtil.SetText(PathTabLabeList[0], EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamController_1435"));
                LTUIUtil.SetText(PathTabLabeList[1], EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamController_1485"));
                LTUIUtil.SetText(PathTabLabeList[2], EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamController_1535"));
            }

            SetPathName(toggleIndex);
            UIPathToggles[toggleIndex].Start();
            UIPathToggles[toggleIndex].Set(true, true);
        }

        public override IEnumerator OnAddToStack()
        {
            InitTeamFormation();
            NationManager.Instance.GetTeamInfo(null);
            GameDataSparxManager.Instance.RegisterListener(NationManager.AccountDataId, OnNationAccountListener);
            isRevive = false;
            return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            GameDataSparxManager.Instance.UnRegisterListener(NationManager.AccountDataId, OnNationAccountListener);
            DestroySelf();
            yield break;
        }

        void InitTeamFormation()
        {
            teamStartActionState = new string[TeamItems.Length];
            int stateIndex = 0;
            for (int teamIndex = 0; teamIndex < NationUtil.TeamNames.Length; ++teamIndex)
            {
                List<TeamMemberData> teamMemDataList = LTFormationDataManager.Instance.GetTeamMemList(NationUtil.TeamNames[teamIndex]);

                string teamNation = "nation" + (teamIndex + 1);

                if (teamMemDataList.Count == 0)
                {
                    teamStartActionState[stateIndex++] = teamNation;
                }

                int startIndexInAllPartner = teamIndex * 4;

                List<OtherPlayerPartnerData> fillPartnerDataList = new List<OtherPlayerPartnerData>();

                List<int> fillPosList = new List<int>();
                for (int teamMemIndex = 0; teamMemIndex < teamMemDataList.Count; ++teamMemIndex)
                {
                    var memberData = teamMemDataList[teamMemIndex];
                    if (memberData.Pos >= 4)
                    {
                       EB.Debug.LogError("memberData.Pos >= 4 for team:{0},Pos:{1}", NationUtil.TeamNames[teamIndex], memberData.Pos);
                        continue;
                    }
                    fillPosList.Add(memberData.Pos);
                    OtherPlayerPartnerData partnerData = new OtherPlayerPartnerData();
                    partnerData.HeroID = memberData.HeroID;
                    int HeroUpGradeId = 0;
                    DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.awake.level", partnerData.HeroID), out HeroUpGradeId);
                    var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(memberData.InfoID);
                    partnerData.Name = charTpl.name;
                    partnerData.Attr = charTpl.char_type;
                    partnerData.Icon = charTpl.icon;
                    partnerData.QualityLevel = charTpl.role_grade;
                    partnerData.UpGradeId = HeroUpGradeId;
                    Hashtable heroStatData = null;
                    if (!DataLookupsCache.Instance.SearchDataByID<Hashtable>("heroStats." + memberData.HeroID, out heroStatData))
                    {
                       EB.Debug.LogError("not hero data for heroid={0}" , memberData.HeroID);
                        continue;
                    }
                    partnerData.Level = EB.Dot.Integer("stat.level", heroStatData, 0);
                    partnerData.Star = EB.Dot.Integer("star", heroStatData, 0);
                    partnerData.awakenLevel = EB.Dot.Integer("stat.awaken", heroStatData, 0);
                    fillPartnerDataList.Add(partnerData);

                    int fillIndex = startIndexInAllPartner + memberData.Pos;
                    FormationPartnerItems[fillIndex].Fill(partnerData);
                }

                for (int idx = 0; idx < 4; ++idx)
                {
                    if (!fillPosList.Contains(idx))
                    {
                        FormationPartnerItems[startIndexInAllPartner + idx].Fill(null);
                    }
                }
            }
        }

        void OnNationAccountListener(string path, INodeData data)
        {
            int rTs = 0;
            NationAccount account = data as NationAccount;
            if (account.TeamDataUpdated)
            {
                account.TeamDataUpdated = false;
                for (int i = 0; i < TeamItems.Length; ++i)
                {
                    TeamItems[i].Fill(account.TeamList[i]);
                    TeamItems[i].ctrl = this;
                    eTeamState state = account.TeamList[i].RealState;
                    if (state == eTeamState.Available || state == eTeamState.InTheWar)
                    {
                        //非复活状态
                        ReviveButton.gameObject.CustomSetActive(false);
                        TimeButton.gameObject.CustomSetActive(false);
                        isAllTeamDeath = false;
                    }
                }

                if (!isAllTeamDeath)
                {
                    return;
                }

                for (int i = 0; i < TeamItems.Length; i++)
                {
                    TeamItems[i].OffBtn.gameObject.CustomSetActive(false);
                    TeamItems[i].GoOnButton.gameObject.CustomSetActive(false);
                    TeamItems[i].Mask.gameObject.CustomSetActive(true);
                    if (TeamItems[i].TeamData.ReviveTs != -2)
                    {
                        rTs = TeamItems[i].TeamData.ReviveTs;
                    }
                }
                ReviveButton.gameObject.CustomSetActive(true);
                TimeButton.gameObject.CustomSetActive(true);
                LTUIUtil.SetText(ReviveCostLabel, NationManager.Instance.Config.TeamReviveCost.ToString());
                if (BalanceResourceUtil.GetUserDiamond() < NationManager.Instance.Config.TeamReviveCost)
                    ReviveCostLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor;
                else
                    ReviveCostLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
                if (ReviveCoroutine != null)
                    StopCoroutine(ReviveCoroutine);
                if (controller.gameObject.activeSelf)
                {
                    ReviveCoroutine = StartCoroutine(ReviveTimer(rTs));
                }
            }
        }

        public void OnSelectPathClick(UIToggle sender)
        {
            if (!sender.value)
                return;

            int pathIndex = int.Parse(sender.gameObject.name);
            SetPathName(pathIndex);
        }

        void SetPathName(int pathIndex)
        {
            if (pathIndex == 0)
            {
                Path = "up";
            }
            else if (pathIndex == 1)
            {
                Path = "median";
            }
            else if (pathIndex == 2)
            {
                Path = "down";
            }
        }

        //[ContextMenu("AddFormationPartnerItems")]
        //public void AddFormationPartnerItems()
        //{
        //	FormationPartnerItems = new List<FormationPartnerItemEx>();
        //	FormationPartnerItems.AddRange(controller.transform.GetComponentsInChildren<FormationPartnerItemEx>());
        //}

        IEnumerator ReviveTimer(int endTs)
        {
            while (true)
            {
                yield return null;
                int ts = endTs - EB.Time.Now;
                ts = ts < 0 ? 0 : ts;
                System.DateTime dt = EB.Time.FromPosixTime(ts);//System.TimeZone.CurrentTimeZone.ToLocalTime();
                string lefttime = dt.ToString("mm:ss");
                LTUIUtil.SetText(LeftReviveTimeLabel, lefttime);
                //Debug.LogError("Ends : "+ endTs);
                //Debug.LogError("time now : "+ EB.Time.Now);
                if ((endTs - EB.Time.Now) <= -1)
                {
                    //Debug.LogError("Timer is in ");
                    ReviveButton.gameObject.CustomSetActive(false);
                    TimeButton.gameObject.CustomSetActive(false);
                    for (int i = 0; i < TeamItems.Length; i++)
                    {
                        TeamItems[i].GoOnButton.gameObject.CustomSetActive(true);
                        TeamItems[i].TeamData.State = eTeamState.Available;
                        TeamItems[i].Mask.gameObject.CustomSetActive(false);
                    }
                    yield break;
                }
            }
        }

        bool isRevive;
        public void OnReviveBtnClick(GameObject sender)
        {
            if (BalanceResourceUtil.GetUserDiamond() < NationManager.Instance.Config.TeamReviveCost)
            {
                BalanceResourceUtil.HcLessMessage();
                return;
            }

            if (isRevive)
            {
                return;
            }
            isRevive = true;

            NationManager.Instance.Revive(delegate (bool successful)
            {
                if (successful)
                {
                    for (int i = 0; i < TeamItems.Length; i++)
                    {
                        TeamItems[i].GoOnButton.gameObject.CustomSetActive(true);
                        TeamItems[i].OffBtn.gameObject.CustomSetActive(false);
                        TeamItems[i].TeamData.State = eTeamState.Available;
                    }
                    ReviveButton.gameObject.CustomSetActive(false);
                    TimeButton.gameObject.CustomSetActive(false);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_NationBattleSelectTeamController_9444"));
                }
            });
        }
    }
}
