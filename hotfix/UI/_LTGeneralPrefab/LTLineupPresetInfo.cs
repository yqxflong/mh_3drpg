using DocumentFormat.OpenXml.Packaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTLineupPresetInfo : DynamicMonoHotfix
    {
        private UILabel _labOrder;
        private UIButton _btnSave;
        private UIButton _btnUse;
        private FormationPartnerItem[] _items;
        private GameObject[] _deathFlags;
        private GameObject[] _sleepFlags;
        private int _lineupIndex;
        private string _lineupType;
        private System.Action _callback;
        private eBattleType _battleType;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            _labOrder = t.GetComponent<UILabel>("Order");
            _btnSave = t.GetComponent<UIButton>("Btn_Save");
            _btnSave.onClick.Add(new EventDelegate(OnBtnSaveClicked));
            _btnSave.gameObject.SetActive(false);
            _btnUse = t.GetComponent<UIButton>("Btn_Use");
            _btnUse.onClick.Add(new EventDelegate(OnBtnUseClicked));
            _btnUse.gameObject.SetActive(false);

            var center = t.FindEx("Center");
            _items = new FormationPartnerItem[center.childCount];
            _deathFlags = new GameObject[center.childCount];
            _sleepFlags = new GameObject[center.childCount];

            for (var i = 0; i < center.childCount; i++)
            {
                _items[i] = center.GetChild(i).GetChild(0).GetMonoILRComponent<FormationPartnerItem>();
                _items[i].mDMono.gameObject.SetActive(false);
                _deathFlags[i] = _items[i].mDMono.transform.FindEx("DeathFlag").gameObject;
                _sleepFlags[i] = _items[i].mDMono.transform.FindEx("SleepFlag").gameObject;
            }
        }

        private int _maxMemberCount = 6;
        private bool _canClickSaveBtn = true;

        private void OnBtnSaveClicked()
        {
            if (!_canClickSaveBtn)
            {
                return;
            }

            _canClickSaveBtn = false;

            if (IsExsitPartner())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_LINEUP_PRESET_SAVE_TIPS"), (int reseult) =>
                {
                    switch (reseult)
                    {
                        case 0:
                            // 确定按钮
                            SaveLineupPresetData(() => _canClickSaveBtn = true);
                            break;
                        case 1:
                        case 2:
                            _canClickSaveBtn = true;
                            break;
                    }
                });
            }
            else
            {
                SaveLineupPresetData(() => _canClickSaveBtn = true);
            }
        }

        private void SaveLineupPresetData(System.Action callback)
        {
            string teamName = FormationUtil.GetCurrentTeamName(_battleType);
            List<TeamMemberData> list = LTFormationDataManager.Instance.GetTeamMemList(teamName);

            if (list == null || list.Count < 1)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_HONOR_ARENA_RAKN_BATTLE_TIP"));
                callback?.Invoke();
                return;
            }

            var lineupInfo = new int[_maxMemberCount];

            for (var i = 0; i < list.Count; i++)
            {
                var member = list[i];

                // 阵容只允许保存非雇佣角色
                if (!member.IsHire)
                {
                    lineupInfo[member.Pos] = member.HeroID;
                }
            }

            var uid = LoginManager.Instance.LocalUser.Id.Value;
            LTFormationDataManager.Instance.API.SaveLineupPreset(uid, _lineupType, _lineupIndex, lineupInfo, (ht) => {
                RefreshLineupPresetDataCache(lineupInfo);
                SetData(lineupInfo);
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_SAVE_SUCCESSFULLY"));
                callback?.Invoke();
            });
        }

        private void RefreshLineupPresetDataCache(int[] lineupInfo)
        {
            ArrayList arrayList;

            if (DataLookupsCache.Instance.SearchDataByID("lineup_preset", out arrayList))
            {
                for (var i = 0; i < arrayList.Count; i++)
                {
                    var type = EB.Dot.String("lineup_type", arrayList[i], "");

                    if (type.Equals(_lineupType))
                    {
                        DataLookupsCache.Instance.CacheData(string.Format("lineup_preset[{0}].lineup_infos[{1}]", i, _lineupIndex), lineupInfo);
                        break;
                    }
                }
            }
        }

        private object GetLineupPresetData()
        {
            ArrayList arrayList;
            object data = null;

            if (DataLookupsCache.Instance.SearchDataByID("lineup_preset", out arrayList))
            {
                for (var i = 0; i < arrayList.Count; i++)
                {
                    var type = EB.Dot.String("lineup_type", arrayList[i], "");

                    if (type.Equals(_lineupType))
                    {
                        DataLookupsCache.Instance.SearchDataByID(string.Format("lineup_preset[{0}].lineup_infos[{1}]", i, _lineupIndex), out data);
                        break;
                    }
                }
            }

            return data;
        }

        private bool IsExsitPartner()
        {
            object data = GetLineupPresetData();

            if (data == null)
            {
                return false;
            }

            ArrayList lineupInfo;

            if (data is int[])
            {
                lineupInfo = new ArrayList(data as int[]);
            }
            else
            {
                lineupInfo = data as ArrayList;
            }

            for (var i = 0; i < lineupInfo.Count; i++)
            {
                int heroId = System.Convert.ToInt32(lineupInfo[i]);

                if (heroId != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool _canClickUseBtn = true;

        private void OnBtnUseClicked()
        {
            if (!_canClickUseBtn)
            {
                return;
            }

            _canClickUseBtn = false;

            object data = GetLineupPresetData();

            if (data == null)
            {
                EB.Debug.LogError("阵容信息为空");
                GlobalMenuManager.Instance.CloseMenu("LTLineupPresetUI");
                _canClickUseBtn = true;
                return;
            }

            ArrayList lineupInfo;

            if (data is int[])
            {
                lineupInfo = new ArrayList(data as int[]);
            }
            else
            {
                lineupInfo = data as ArrayList;
            }

            string teamName = FormationUtil.GetCurrentTeamName(_battleType);
            List<TeamMemberData> originTeamMembers = new List<TeamMemberData>(LTFormationDataManager.Instance.GetTeamMemList(teamName));
            int count = 0;
            bool hasAlivePartner = false;

            // 上阵英雄至少要保留一个，故先处理新英雄上阵再处理旧英雄下阵
            for (var i = 0; i < lineupInfo.Count; i++)
            {
                int heroId = System.Convert.ToInt32(lineupInfo[i]);
                bool isAlive = true;
                bool isSleepTower = false;

                if (heroId != 0 && (_battleType == eBattleType.ChallengeCampaign || _battleType == eBattleType.AlienMazeBattle))
                {
                    isAlive = FormationUtil.IsAlive(heroId, false);
                }

                if (_battleType == eBattleType.SleepTower && LTClimingTowerHudController.Instance != null)
                {
                    if (heroId != 0)
                    {
                        isAlive = LTClimingTowerHudController.Instance.CanUpTeam(heroId);
                    }

                    isSleepTower = true;
                }

                if (heroId != 0 && isAlive)
                {
                    hasAlivePartner = true;

                    var memberData = originTeamMembers.Find(member => member.HeroID == heroId);
                    originTeamMembers.Remove(memberData);
                    if (LTFormationDataManager.Instance.CurTeamMemberData != null)
                    {
                        TeamMemberData temp = LTFormationDataManager.Instance.CurTeamMemberData;
                        if (temp.Pos == i)
                        {
                            LTFormationDataManager.Instance.UnUseAllianceMercenary(temp.HeroID, i, null);
                        }
                    }
                    LTFormationDataManager.Instance.RequestDragHeroToFormationPos(heroId, i, teamName, () => 
                    {
                        count += 1;

                        if (count >= lineupInfo.Count)
                        {
                            RemoveFromLineup(originTeamMembers, teamName, () => _canClickUseBtn = true);
                        }
                    });
                }
                else
                {
                    count += 1;

                    if (count >= lineupInfo.Count)
                    {
                        if (hasAlivePartner)
                        {
                            RemoveFromLineup(originTeamMembers, teamName, () => _canClickUseBtn = true);
                        }
                        else
                        {
                            _canClickUseBtn = true;

                            var tips = isSleepTower ? EB.Localizer.GetString("ID_LINEUP_PRESET_ALL_SLEEPED") : EB.Localizer.GetString("ID_LINEUP_PRESET_ALL_KILLED");
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, tips);
                        }
                    }
                }
            }
        }

        private void RemoveFromLineup(List<TeamMemberData> originTeamMembers, string teamName, System.Action callback)
        {
            if (originTeamMembers.Count < 1)
            {
                CloseAndCallback();
                callback?.Invoke();
                return;
            }

            var count = 0;

            for (var i = 0; i < originTeamMembers.Count; i++)
            {
                var member = originTeamMembers[i];
                var heroId = member.IsHire ? -member.HeroID : member.HeroID;

                if (IsInTeam(teamName, heroId))
                {
                    LTFormationDataManager.Instance.RequestRemoveHeroFormation(heroId, teamName, () =>
                    {
                        count += 1;

                        if (count >= originTeamMembers.Count)
                        {
                            CloseAndCallback();
                            callback?.Invoke();
                        }
                    });
                }
                else
                {
                    count += 1;

                    if (count >= originTeamMembers.Count)
                    {
                        CloseAndCallback();
                        callback?.Invoke();
                    }
                }
            }
        }

        private bool IsInTeam(string teamName, int heroId)
        {
            var currentMembers = LTFormationDataManager.Instance.GetTeamMemList(teamName);
            var memberData = currentMembers.Find(member => member.HeroID == heroId);
            return memberData != null;
        }

        private void CloseAndCallback()
        {
            GlobalMenuManager.Instance.CloseMenu("LTLineupPresetUI");
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_LINEUP_USE_SUCCESSFULLY"));
            _callback?.Invoke();
        }

        private int SetData(int[] lineupInfo)
        {
            // 一组阵容中的出战人数
            var battleCount = 0;

            for (var i = 0; i < lineupInfo.Length; i++)
            {
                var heroId = lineupInfo[i];
                var item = _items[i];

                if (heroId == 0)
                {
                    item.mDMono.gameObject.SetActive(false);
                }
                else
                {
                    item.mDMono.gameObject.SetActive(true);
                    item.Fill(GetPartnerData(heroId));

                    var partnerData = LTPartnerDataManager.Instance.GetPartnerByHeroId(heroId);
                    item.SetCharTypeFx((PartnerGrade)partnerData.HeroInfo.role_grade, partnerData.HeroInfo.char_type);
                    battleCount += 1;

                    _deathFlags[i].SetActive(false);
                    _sleepFlags[i].SetActive(false);

                    if (_battleType == eBattleType.ChallengeCampaign || _battleType == eBattleType.AlienMazeBattle)
                    {
                        _deathFlags[i].SetActive(!FormationUtil.IsAlive(heroId, false));
                    }


                    if (_battleType == eBattleType.SleepTower && LTClimingTowerHudController.Instance != null)
                    {
                        _sleepFlags[i].SetActive(!LTClimingTowerHudController.Instance.CanUpTeam(heroId));
                    }
                }
            }

            return battleCount;
        }

        public void SetData(int lineupIndex, int[] lineupInfo, string lineupType, bool showSavePanel, int battleType, System.Action callback)
        {
            _lineupIndex = lineupIndex;
            _lineupType = lineupType;
            _labOrder.text = string.Format("{0}{1}", EB.Localizer.GetString("ID_LINEUP"), lineupIndex + 1);
            _btnSave.gameObject.SetActive(showSavePanel);
            _btnUse.gameObject.SetActive(!showSavePanel);
            _callback = callback;
            _battleType = (eBattleType)battleType;

            var battleCount = SetData(lineupInfo);

            // 一组阵容中无人出战，屏蔽使用按钮
            if (!showSavePanel && battleCount == 0)
            {
                _btnUse.gameObject.SetActive(false);
            }
        }

        private OtherPlayerPartnerData GetPartnerData(int heroId)
        {
            int quality;
            int addLevel;
            var partnerData = LTPartnerDataManager.Instance.GetPartnerByHeroId(heroId);

            if (partnerData == null)
            {
                EB.Debug.LogError("Can't get partner by HeroId: " + heroId);
                return null;
            }

            LTPartnerDataManager.GetPartnerQuality(partnerData.UpGradeId, out quality, out addLevel);

            var data = new OtherPlayerPartnerData();
            data.Name = partnerData.HeroInfo.name;
            data.Attr = partnerData.HeroInfo.char_type;
            data.Icon = partnerData.HeroInfo.icon;
            data.QualityLevel = quality;
            data.Level = partnerData.Level;
            data.Star = partnerData.Star;
            data.UpGradeId = partnerData.UpGradeId;
            data.awakenLevel = partnerData.IsAwaken;
            return data;
        }
    }
}
