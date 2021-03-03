using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hotfix_LT.UI
{
    public class LegionModel
    {
        //public static string accountDataId = "alliance.account";
        //public static string accountAppliesDataId = "alliance.accountApplies";
        //public static string detailDataId = "alliance.detail";
        //public static string detailAppliesDataId = "alliance.detailApplies";
        //public static string detailMembersDataId = "alliance.detailMembers";
        //public static string listDataId = "alliance.list";
        //public static string configDataId = "alliance.config";

        private static LegionModel _instance;
        public static LegionModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new LegionModel();
            }
            return _instance;
        }

        /// <summary>
        /// 是否已经加入军团
        /// </summary>
        public bool isJoinedLegion;
        /// <summary>
        /// 搜索到的军团数据
        /// </summary>
        public SearchItemData[] searchItemDatas;

        /// <summary>
        /// 申请过的数据
        /// </summary>
        public ApplyItemData[] applyItemDatas;

        /// <summary>
        /// 军团数据
        /// </summary>
        public LegionData legionData;

        /// <summary>
        /// 缓存刷新搜索列表的时间
        /// </summary>
        public float searchListTime = -300;

        /// <summary>
        /// 军团头像id对应的图标name
        /// </summary>
        public Dictionary<int, string> dicLegionSpriteName;

        /// <summary>
        /// 军团头像的底图标
        /// </summary>
        public Dictionary<int, string> dicLegionBGSpriteName;

        public int defaultSpriteKey;


        private LegionModel()
        {
            legionData = new LegionData();
            InitLegionIcon();
            defaultSpriteKey = 0;
        }

        private void InitLegionIcon()
        {
            dicLegionSpriteName = new Dictionary<int, string>()
        {
            {0, "Legion_Badge_Pattern_1"},
            {1, "Legion_Badge_Pattern_2"},
            {2, "Legion_Badge_Pattern_3"},
            {3, "Legion_Badge_Pattern_4"},
            {4, "Legion_Badge_Pattern_5"},
            {5, "Legion_Badge_Pattern_6"},
            {6, "Legion_Badge_Pattern_7"},
            {7, "Legion_Badge_Pattern_8"},
            {8, "Legion_Badge_Pattern_9"},
            {9, "Legion_Badge_Pattern_10"},
            {10, "Legion_Badge_Pattern_11"},
            {11, "Legion_Badge_Pattern_12"},
        };

            dicLegionBGSpriteName = new Dictionary<int, string>()
        {
            {0, "Legion_Badge_Di1"},
            {1, "Legion_Badge_Di2"},
            {2, "Legion_Badge_Di3"},
            {3, "Legion_Badge_Di4"},
        };
        }

        //获取军团职位文本
        public string GetMemberPosName(eAllianceMemberRole type)
        {
            string str = "";
            if (type == eAllianceMemberRole.Owner)
            {
                str = LegionConfig.GetLegionText("ID_LEGION_ROLE_OWNER_NAME");
            }
            else if (type == eAllianceMemberRole.ExtraOwner)
            {
                str = LegionConfig.GetLegionText("ID_LEGION_ROLE_EXTRA_OWNER_NAME");
            }
            else if (type == eAllianceMemberRole.Admin)
            {
                str = LegionConfig.GetLegionText("ID_LEGION_ROLE_ADMIN_NAME");
            }
            else if (type == eAllianceMemberRole.Member)
            {
                str = LegionConfig.GetLegionText("ID_LEGION_ROLE_MEMBER_NAME");
            }

            return str;
        }

        public void SetSearchItemData(AllianceItem[] array)
        {
            List<SearchItemData> listItemData = new List<SearchItemData>();
            for (int i = 0; i < array.Length; i++)
            {
                SearchItemData sd = new SearchItemData();
                sd.id = array[i].Id;
                sd.legionName = array[i].Name;
                sd.legionLevel = array[i].Level;
                sd.conditionLevel = array[i].LimitLevel;
                sd.isNeedApprove = array[i].IsReview;
                sd.totalExp = array[i].TotalExp;
                sd.maxPeopleNum = array[i].MaxMemberCount;
                sd.currentPeopleNum = array[i].MemberCount;
                if (dicLegionSpriteName.ContainsKey(array[i].IconID))
                {
                    sd.iconSpriteName = dicLegionSpriteName[array[i].IconID];
                }

                int legionIconIndex = array[i].IconID % 100;
                int legionBgIconIndex = array[i].IconID / 100;
                if (dicLegionSpriteName.ContainsKey(legionIconIndex))
                {
                    sd.iconSpriteName = dicLegionSpriteName[legionIconIndex];
                }
                if (dicLegionBGSpriteName.ContainsKey(legionBgIconIndex))
                {
                    sd.iconBGSpriteName = dicLegionBGSpriteName[legionBgIconIndex];
                }

                listItemData.Add(sd);
            }

            //排序
            SearchItemData buff;
            for (int i = 0; i < listItemData.Count; i++)
            {
                for (int j = i + 1; j < listItemData.Count; j++)
                {
                    if (listItemData[i].totalExp < listItemData[j].totalExp)
                    {
                        buff = listItemData[i];
                        listItemData[i] = listItemData[j];
                        listItemData[j] = buff;
                    }
                    else if (listItemData[i].totalExp == listItemData[j].totalExp)
                    {
                        if (listItemData[i].currentPeopleNum < listItemData[j].currentPeopleNum)
                        {
                            buff = listItemData[i];
                            listItemData[i] = listItemData[j];
                            listItemData[j] = buff;
                        }
                    }
                }
            }

            searchItemDatas = listItemData.ToArray();
            SearchItemDatasProcess();
        }

        /// <summary>
        /// 可搜索列表进行自身数据维护
        /// </summary>
        private void SearchItemDatasProcess()
        {
            int userLevel = BalanceResourceUtil.GetUserLevel();

            if (searchItemDatas == null)
            {
                return;
            }

            for (int i = 0; i < searchItemDatas.Length; i++)
            {
                searchItemDatas[i].isHasApplyed = false;
                searchItemDatas[i].isReachCondition = true;

                if (applyItemDatas != null) //对已申请的做判断
                {
                    for (int j = 0; j < applyItemDatas.Length; j++)
                    {
                        if (applyItemDatas[j].legionID == searchItemDatas[i].id)
                        {
                            searchItemDatas[i].isReachCondition = false;
                            searchItemDatas[i].isHasApplyed = true;
                            break;
                        }
                    }
                }

                if (searchItemDatas[i].isHasApplyed) //如果已经申请过的
                {
                    continue;
                }

                if (searchItemDatas[i].conditionLevel > userLevel)
                {
                    searchItemDatas[i].isReachCondition = false;
                    continue;
                }

                if (searchItemDatas[i].currentPeopleNum >= searchItemDatas[i].maxPeopleNum)
                {
                    searchItemDatas[i].isReachCondition = false;
                    continue;
                }
            }

            if (LegionEvent.NotifyUpdateSearchItemDatas != null)
            {
                LegionEvent.NotifyUpdateSearchItemDatas(searchItemDatas);
            }
        }

        /// <summary>
        /// 设置申请过的数据 要对搜索列表
        /// </summary>
        public void SetApplyItemDatas(List<AllianceApply> accountApplies)
        {
            applyItemDatas = new ApplyItemData[accountApplies.Count];
            for (int i = 0; i < applyItemDatas.Length; i++)
            {
                AllianceApply apply = accountApplies[i];
                ApplyItemData itemData = new ApplyItemData();
                itemData.legionID = apply.AllianceId;
                itemData.applyTime = apply.ApplyTime;
                applyItemDatas[i] = itemData;
            }

            //维护搜索列表
            SearchItemDatasProcess();
        }

        public void SetAllianceAccount(AllianceAccount data)
        {
            if (data.State == eAllianceState.Joined)
            {
                isJoinedLegion = true;
            }
            else
            {
                if (isJoinedLegion) //如果之前是在军团的 重置军团
                {
                    legionData = new LegionData();
                }
                isJoinedLegion = false;

            }

            legionData.legionID = data.AllianceId;
            //legionData.legionLevel = data.

            if (LegionEvent.NotifyLegionAccount != null)
            {
                LegionEvent.NotifyLegionAccount(data);
            }
        }

        public void SetLegionMembers(List<AllianceMember> members)
        {
            int d = members.Count - legionData.listMember.Count;
            if (d > 0)
            {
                for (int i = 0; i < d; i++)
                {
                    LegionMemberData md = new LegionMemberData();
                    legionData.listMember.Add(md);
                }
            }
            else if (d < 0)
            {
                for (int i = 0; i < -d; i++)
                {
                    legionData.listMember.RemoveAt(legionData.listMember.Count - 1);
                }
            }

            for (int i = 0; i < members.Count; i++)
            {
                SetMember(members[i], legionData.listMember[i]);
            }

            RankLegionMembers();

            if (LegionEvent.NotifyUpdateLegionData != null)
            {
                LegionEvent.NotifyUpdateLegionData(legionData);
            }
        }

        private void SetMember(AllianceMember member, LegionMemberData memberData)
        {
            memberData.uid = member.Uid;
            memberData.memberName = member.Name;
            memberData.level = member.Level;
            memberData.portrait = member.Portrait;
            memberData.templateId = member.templateId;
            memberData.skin = member.Skin;
            memberData.headFrame = member.HeadFrame;
            memberData.todaydonate = member.TodayDonateDegree;
            memberData.totaldonate = member.DonateDegree;
            memberData.dutyType = member.Role;
            memberData.offlineTime = member.OfflineTime;
            memberData.dutyTypeStr = GetMemberPosName(memberData.dutyType);

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(member.OfflineTime);
            if (member.OfflineTime != 0)
            {
                TimeSpan behind = DateTime.Now - dt;
                memberData.offlineHour = (int)(behind.TotalHours);
            }
            else
            {
                memberData.offlineHour = 0;
            }
            memberData.todayDonateTimes = member.TodayDonated.Times;
            if (member.Uid ==LoginManager.Instance.LocalUserId.Value) //如果是用户自己
            {
                memberData.todayLuxuryDonateTimes = member.TodayDonated.LuxuryTimes;
                memberData.todayHcDonateTimes = member.TodayDonated.HcTimes;
                memberData.todayGoldDonateTimes = member.TodayDonated.GoldTimes;
                legionData.userMemberData = memberData;
            }
        }

        //处理排序
        private void RankLegionMembers()
        {
            if (legionData == null || legionData.listMember == null) return;

            //冒泡排序
            for (int i = 0; i < legionData.listMember.Count; i++)
            {
                for (int j = i + 1; j < legionData.listMember.Count; j++)
                {
                    bool isChange = false;
                    if (legionData.listMember[i].offlineTime != 0 && legionData.listMember[j].offlineTime == 0) //在线一定和不在线的交换
                    {
                        isChange = true;
                    }
                    else if (legionData.listMember[i].offlineTime == 0 && legionData.listMember[j].offlineTime != 0)
                    {

                    }
                    else if (((int)legionData.listMember[i].dutyType) < ((int)legionData.listMember[j].dutyType)) //职位
                    {
                        isChange = true;
                    }
                    else if (((int)legionData.listMember[i].dutyType) != ((int)legionData.listMember[j].dutyType))
                    {

                    }
                    else if (legionData.listMember[i].level < legionData.listMember[j].level)//等级
                    {
                        isChange = true;
                    }
                    else if (legionData.listMember[i].level != legionData.listMember[j].level)
                    {

                    }
                    else if (legionData.listMember[i].totaldonate < legionData.listMember[j].totaldonate) //贡献
                    {
                        isChange = true;
                    }

                    if (!isChange) continue;//如果没有需要交换 就下一个

                    LegionMemberData buff = legionData.listMember[i];
                    legionData.listMember[i] = legionData.listMember[j];
                    legionData.listMember[j] = buff;
                }
            }
        }

        /// <summary>
        /// 合并操作成员列表
        /// </summary>
        /// <param name="members"></param>
        public void MergeLegionMembers(List<AllianceMember> members)
        {
            if (members == null) return;


            for (int i = 0; i < members.Count; i++)
            {
                if (members[i] == null)
                {
                    continue;
                }

                bool isFind = false;
                for (int j = 0; j < legionData.listMember.Count; j++)
                {
                    if (legionData.listMember[j].uid == members[i].Uid)
                    {
                        SetMember(members[i], legionData.listMember[j]);
                        isFind = true;
                        break;
                    }
                }

                if (!isFind)
                {
                    LegionMemberData md = new LegionMemberData();
                    legionData.listMember.Add(md);
                    SetMember(members[i], md);
                }
            }

            RankLegionMembers();

            if (LegionEvent.NotifyUpdateLegionData != null)
            {
                LegionEvent.NotifyUpdateLegionData(legionData);
            }
        }

        private void SetLegionTechAddInfo(float levelAdd, int legionLevel, Dictionary<int, LegionTechAdd> addDic, ref int addLevel, ref float add)
        {
            //Debug.LogError("leveladd: "+levelAdd +" add: "+add + "legionLevel : "+ legionLevel);
            if (Mathf.Abs(levelAdd - add) > 0.01f)
            {
                addLevel++;
                add = levelAdd;
                for (int i = 1; i < legionLevel; i++)
                {

                    int nextLegionLevel = addDic[i].nextLegionLevel;
                    if (addDic.ContainsKey(i) && nextLegionLevel == -1)
                    {
                        addDic[i].nextLegionLevel = legionLevel;
                    }
                }
            }

            if (!addDic.ContainsKey(legionLevel))
            {
                LegionTechAdd techAdd = new LegionTechAdd();
                techAdd.techAdd = add;
                techAdd.techLevel = addLevel;
                addDic.Add(legionLevel, techAdd);
            }
        }

        private int _mATKAddLevel = 0;
        private int _mDEFAddLevel = 0;
        private int _mMaxHPAddLevel = 0;
        private float _mATKAdd = 0;
        private float _mDEFAdd = 0;
        private float _mMaxHPAdd = 0;
        public bool isTechConfigInit = false;
        /// <summary>
        /// 军团科技加成信息表
        /// </summary>
        public Dictionary<int, LegionTechAdd> legionATKAddDic = new Dictionary<int, LegionTechAdd>();
        public Dictionary<int, LegionTechAdd> legionDEFAddDic = new Dictionary<int, LegionTechAdd>();
        public Dictionary<int, LegionTechAdd> legionMaxHPAddDic = new Dictionary<int, LegionTechAdd>();

        public void SetAllianceDetail(AllianceDetail detail)
        {
            legionData.legionName = detail.Name;
            legionData.ownerName = detail.OwnerName;
            legionData.legionLevel = detail.Level;
            legionData.logTxt = detail.Notice;
            legionData.maxPeopleNum = detail.MaxMemberCount;
            legionData.currentPeopleNum = detail.MemberCount;
            legionData.currentExp = detail.CurrentExp;
            legionData.mailTimes = detail.MailTimes;
            legionData.todayExp = detail.TodayExp;
            AllianceLevelConfig[] levelConfigs = AlliancesManager.Instance.Config.Levels;

            if (levelConfigs != null)
            {
                for (int i = 0; i < levelConfigs.Length; i++)
                {
                    AllianceLevelConfig levelConfig = levelConfigs[i];
                    if (!isTechConfigInit)
                    {
                        SetLegionTechAddInfo(levelConfig.ATKAdd, levelConfig.Level, legionATKAddDic, ref _mATKAddLevel, ref _mATKAdd);
                        SetLegionTechAddInfo(levelConfig.DEFAdd, levelConfig.Level, legionDEFAddDic, ref _mDEFAddLevel, ref _mDEFAdd);
                        SetLegionTechAddInfo(levelConfig.MaxHPAdd, levelConfig.Level, legionMaxHPAddDic, ref _mMaxHPAddLevel, ref _mMaxHPAdd);
                    }
                    if (levelConfig.Level == legionData.legionLevel)
                    {
                        if (i != levelConfigs.Length - 1)
                        {
                            legionData.growupExp = levelConfigs[i + 1].LevelUpExp;
                            legionData.growupMaxPeopleNum = levelConfigs[i + 1].MaxMemberCount;
                        }
                        else
                        {
                            legionData.growupExp = levelConfig.LevelUpExp;
                            legionData.growupMaxPeopleNum = levelConfig.MaxMemberCount;
                        }
                        legionData.ATKAdd = levelConfig.ATKAdd;
                        legionData.DEFAdd = levelConfig.DEFAdd;
                        legionData.MaxHPAdd = levelConfig.MaxHPAdd;
                        //break;
                    }
                }
            }

            isTechConfigInit = true;
            legionData.ownerUID = detail.OwnerUid;
            legionData.notice = detail.Notice;
            legionData.legionIconID = detail.IconID;
            legionData.conditionLevel = detail.LimitLevel;
            legionData.isNeedApprove = detail.IsReview;

            int legionIconIndex = legionData.legionIconID % 100;
            int legionBgIconIndex = legionData.legionIconID / 100;
            if (dicLegionSpriteName.ContainsKey(legionIconIndex))
            {
                legionData.legionIconSptName = dicLegionSpriteName[legionIconIndex];
            }
            if (dicLegionBGSpriteName.ContainsKey(legionBgIconIndex))
            {
                legionData.legionIconBGSptName = dicLegionBGSpriteName[legionBgIconIndex];
            }

            if (LegionEvent.NotifyUpdateLegionData != null)
            {
                LegionEvent.NotifyUpdateLegionData(legionData);
            }
        }

        public void SetAllianceApplys(List<AllianceApply> applies)
        {
            int d = applies.Count - legionData.listRequestJoin.Count;
            if (d > 0)
            {
                for (int i = 0; i < d; i++)
                {
                    RequestJoinData rjd = new RequestJoinData();
                    legionData.listRequestJoin.Add(rjd);
                }
            }
            else if (d < 0)
            {
                for (int i = 0; i < -d; i++)
                {
                    legionData.listRequestJoin.RemoveAt(legionData.listRequestJoin.Count - 1);
                }
            }

            int index = 0;
            for (int i = 0; i < applies.Count; i++)
            {
                RequestJoinData rjd = legionData.listRequestJoin[index];
                AllianceApply data = applies[i];
                if (data == null) //如果为空就移除
                {
                    legionData.listRequestJoin.RemoveAt(legionData.listRequestJoin.Count - 1);
                    continue;
                }
                rjd.name = data.Name;
                rjd.level = data.Level;
                rjd.approveID = data.Uid;
                rjd.headIcon = data.Portrait;
                rjd.headFrame = data.Frame;
                index++;
            }

            if (LegionEvent.NotifyUpdateLegionData != null)
            {
                LegionEvent.NotifyUpdateLegionData(legionData);
            }
        }

        public void SetLegionMessages(List<AllianceMessage> messages)
        {
            if (messages == null) return;

            legionData.listMessageItem.Clear();

            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Time > legionData.messageLastTime)
                {
                    legionData.messageLastTime = messages[i].Time; //须记下最后一次的时间用来进行增量下载
                    legionData.messageLastRuntimeTime = Time.unscaledTime;
                }
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                DateTime dt = startTime.AddSeconds(messages[i].Time);
                int year = dt.Year;
                int month = dt.Month;
                int day = dt.Day;
                int hour = dt.Hour;
                int min = dt.Minute;

                MessageCellData cellData = new MessageCellData(); //先构建必须构建的单元 
                cellData.hour = hour;
                cellData.min = dt.Minute;

                string hourStr = hour < 10 ? "0" + hour : hour.ToString();
                string minStr = min < 10 ? "0" + min : min.ToString();
                switch (messages[i].Type)
                {
                    case "create":
                        cellData.content = string.Format(LegionConfig.GetLegionText("ID_LEGION_SYSTEM_MESSAGE1"), hourStr, minStr, messages[i].Name);
                        break;
                    case "join":
                        cellData.content = string.Format(LegionConfig.GetLegionText("ID_LEGION_SYSTEM_MESSAGE2"), hourStr, minStr, messages[i].Name);
                        break;
                    case "leave":
                        cellData.content = string.Format(LegionConfig.GetLegionText("ID_LEGION_SYSTEM_MESSAGE3"), hourStr, minStr, messages[i].Name);
                        break;
                    case "kick":
                        cellData.content = string.Format(LegionConfig.GetLegionText("ID_LEGION_SYSTEM_MESSAGE4"), hourStr, minStr, messages[i].Name);
                        break;
                    case "promote":
                        cellData.content = string.Format(LegionConfig.GetLegionText("ID_LEGION_SYSTEM_MESSAGE5"), hourStr, minStr, messages[i].Name);
                        break;
                    case "demote":
                        cellData.content = string.Format(LegionConfig.GetLegionText("ID_LEGION_SYSTEM_MESSAGE6"), hourStr, minStr, messages[i].Name);
                        break;
                    case "newOwner":
                        cellData.content = string.Format(LegionConfig.GetLegionText("ID_LEGION_SYSTEM_MESSAGE7"), hourStr, minStr, messages[i].Name);
                        break;
                    case "levelup":
                        cellData.content = string.Format(LegionConfig.GetLegionText("ID_LEGION_SYSTEM_MESSAGE8"), hourStr, minStr, messages[i].Name);
                        break;
                    default:
                        break;
                }
                bool isFind = false;
                for (int j = 0; j < legionData.listMessageItem.Count; j++)
                {
                    if (legionData.listMessageItem[j].IsContain(year, month, day))
                    {
                        isFind = true;
                        legionData.listMessageItem[j].listCell.Add(cellData);
                        break;
                    }
                }

                if (!isFind)
                {
                    MessageItemData mid = new MessageItemData(); //找不到就创建大节点 并将单元加入
                    mid.year = year;
                    mid.day = day;
                    mid.month = month;
                    mid.listCell.Add(cellData);
                    legionData.listMessageItem.Insert(0, mid);
                }
            }

            if (LegionEvent.NotifyUpdateLegionMessages != null)
            {
                LegionEvent.NotifyUpdateLegionMessages(legionData.listMessageItem);
            }
        }
    }

    public class SearchItemData
    {
        /// <summary> 军团id </summary>
        public int id;
        /// <summary> 军团name </summary>
        public string legionName = "";
        public int legionLevel;
        public int currentPeopleNum;
        public int maxPeopleNum;
        /// <summary> 限制申请等级 </summary>
        public int conditionLevel;
        /// <summary> 是否需要审批 </summary>
        public bool isNeedApprove;
        /// <summary> 是否达到申请条件 </summary>
        public bool isReachCondition;

        /// <summary> 是否已申请过 </summary>
        public bool isHasApplyed;
        /// <summary> 总经验 </summary>
        public int totalExp;
        /// <summary> 军团头像 </summary>
        public string iconSpriteName = "";
        /// <summary> 军团头像底 </summary>
        public string iconBGSpriteName = "";
    }

    public class LegionData
    {
        /// <summary> 军团id</summary>
        public int legionID;
        /// <summary> 军团图标id </summary>
        public int legionIconID;
        /// <summary> 军团名字</summary>
        public string legionName = "";
        /// <summary> 军团等级</summary>
        public int legionLevel;
        /// <summary> 军团当前人数</summary>
        public int currentPeopleNum;
        /// <summary> 军团最大人数</summary>
        public int maxPeopleNum;
        /// <summary> 军团加入等级</summary>
        public int conditionLevel;
        /// <summary> 军团加入是否审批</summary>
        public bool isNeedApprove;
        /// <summary> 军团公告</summary>
        public string logTxt = "";
        /// <summary> 军团经验</summary>
        public int currentExp;
        /// <summary> 今天的军团经验</summary>
        public int todayExp;
        /// <summary> 军团升级需要的经验</summary>
        public int growupExp;
        /// <summary> 升级后的人数 </summary>
        public int growupMaxPeopleNum;
        /// <summary> 邮件发送次数 </summary>
        public int mailTimes;
        /// <summary> 军团科技攻击力加成</summary>
        public float ATKAdd;
        /// <summary> 军团科技防御加成</summary>
        public float DEFAdd;
        /// <summary> 军团科技生命值加成</summary>
        public float MaxHPAdd;

        /// <summary>  拥有者名字/summary>
        public string ownerName = "";
        /// <summary> 军团公告 </summary>
        public string notice = "";
        /// <summary> 军团图标 sprite name </summary>
        public string legionIconSptName = "";
        /// <summary> 军团底标 sprite name </summary>
        public string legionIconBGSptName = "";

        /// <summary>  拥有者的用户ID/summary>
        public long ownerUID;

        /// <summary>
        /// 用户自己的member信息
        /// </summary>
        public LegionMemberData userMemberData;


        /// <summary> 军团成员信息</summary>
        public List<LegionMemberData> listMember = new List<LegionMemberData>();

        /// <summary> 申请加入军团待审批</summary>
        public List<RequestJoinData> listRequestJoin = new List<RequestJoinData>();

        /// <summary>
        /// 最后一次消息的服务器时间戳
        /// </summary>
        public double messageLastTime;

        /// <summary>
        /// 最后一次消息的客户端运行时间 用来做倒计时 避免反复拉取
        /// </summary>
        public float messageLastRuntimeTime;

        /// <summary> 消息列表</summary>
        public List<MessageItemData> listMessageItem = new List<MessageItemData>();
    }


    public class LegionTechBasicinfo
    {
        private List<Data.AllianceTechSkillUpLevel> skillUpLevel;
        public int level{  get;private set;}
        public int skillId { get; private set; }
        public float addtion { get; private set; }
        public int cost { get; private set; }

        public int MaxLevel { get; private set; }
        public LegionTechBasicinfo(int techskillId, List<Data.AllianceTechSkillUpLevel> info,int maxLevel)
        {
            if (techskillId != 0)
            {
                skillId = techskillId;
                skillUpLevel = info;
                MaxLevel = maxLevel;
                RefreshLevelUpdata(0);
            }
        }

        public void RefreshLevelUpdata(int skillLevel)
        {
            if (skillUpLevel != null)
            {
                for (int i = 0; i < skillUpLevel.Count; i++)
                {
                    var tempdata = skillUpLevel[i];
                    if (tempdata.level == skillLevel)
                    {
                        level = skillLevel;
                        addtion = tempdata.addition;
                        cost = tempdata.cost;
                        return;
                    }
                }
            }
        }
        public void Clear()
        {
            skillUpLevel = null;
            level = 0;
            skillId = 0;
            addtion = 0;
            cost = 0;
        }

    }

    public class LegionTechInfo
    {
        public Dictionary<int, int> TechlevelDic;
        public List<LegionTechBasicinfo> TechInfoList;
        bool isTechInfoInit = false;
        public int techchestLevel;
        public Data.AllianceTechChest CurtechChest;
        public int curGold;
        public int curExp;
        public int curVigor;
        private int calTime;
        private int addgold;
        private int addexp;
        private int addvigor;
        private int refreshTimer;

        public LegionTechInfo()
        { 

            TechlevelDic = new Dictionary<int, int>();
        }

        public void UpdataTechInfo(object obj)
        {
            UpdataTechSkill(obj);
            UpdataTechChestInfo(obj);
        }

        private void UpdataTechChestInfo(object obj)
        {
            int preCalTime = calTime;
            calTime = EB.Dot.Integer("last_tech_chest_time", obj, calTime);
            if (calTime != 0 && calTime == preCalTime)
            {
                addgold = EB.Dot.Integer("tech_chest_store.gold", obj, addgold);
                addexp = EB.Dot.Integer("tech_chest_store.xp", obj, addexp);
                addvigor = EB.Dot.Integer("tech_chest_store.vigor", obj, addvigor);
            }
            else if(calTime != preCalTime)
            {
                addgold = EB.Dot.Integer("tech_chest_store.gold", obj, 0);
                addexp = EB.Dot.Integer("tech_chest_store.xp", obj, 0);
                addvigor = EB.Dot.Integer("tech_chest_store.vigor", obj, 0);
            }
            
            CurtechChest = Data.AllianceTemplateManager.Instance.GetAllianceTechChestInfoByLevel(techchestLevel);
            if (CurtechChest == null)
            {
                EB.Debug.LogError("allianceTechChest is null,please Check");
                return;
            }
            if (calTime <= 0)
            {
                curGold = CurtechChest.goldmax;
                curExp = CurtechChest.expmax;
                curVigor = CurtechChest.vigormax;
            }
            else
            {
                int nextRefreshTime = 3605 - EB.Time.Now % 3600;
                int totalAddTimes = Mathf.FloorToInt((float)EB.Time.Now / 3600) - Mathf.FloorToInt((float)calTime / 3600);
                if (totalAddTimes >= 0)
                {
                    int totalgold = addgold + CurtechChest.goldrate * totalAddTimes;
                    int totalexp = addexp + CurtechChest.exprate * totalAddTimes;
                    int totalvigor = addvigor + CurtechChest.vigorrate * totalAddTimes;
                    int perCurGold = curGold;
                    curGold = totalgold >= CurtechChest.goldmax ? CurtechChest.goldmax : totalgold;
                    curExp = totalexp >= CurtechChest.expmax ? CurtechChest.expmax : totalexp;
                    curVigor = totalvigor >= CurtechChest.vigormax ? CurtechChest.vigormax : totalvigor;
                    RemoveTimer();
                    if (nextRefreshTime > 0)
                    {
                        refreshTimer = ILRTimerManager.instance.AddTimer(nextRefreshTime * 1000, 1, OnAutoReflashData);
                    }
                    else
                    {
                        refreshTimer = ILRTimerManager.instance.AddTimer(3600 * 1000, 1, OnAutoReflashData);
                    }
                    if (perCurGold != curGold)
                    {
                        Messenger.Raise(EventName.LegionTechChestUpdate);
                        LegionLogic.GetInstance().isHaveTechResCanRecieve();
                    }
                }
            }
        }

        private void UpdataTechSkill(object obj)
        {
            InitTechSkillInfo();
            Hashtable data = EB.Dot.Object("allianceSkill", obj, null);
            if (data != null)
            {
                int perchestLevel = techchestLevel;
                techchestLevel = 0;
                TechlevelDic.Clear();
                foreach (DictionaryEntry ad in data)
                {
                    int skillId = EB.Dot.Integer("",ad.Key,0);
                    int level = EB.Dot.Integer("", ad.Value, 0);
                    if (skillId != 0)
                    {
                        TechlevelDic.Add(skillId, level);
                        techchestLevel += level;
                    }
                }
                
                if (perchestLevel != techchestLevel)
                {
                    
                    UpdataBasicInfoList();                 
                    Messenger.Raise(EventName.LegionTechSkillLevelUp);
                }
            }


        }
        public void InitTechSkillInfo()
        {
            if (!isTechInfoInit)
            {
                var data = Data.AllianceTemplateManager.Instance.mTechSkillList;
                if (data != null)
                {
                    if (TechInfoList == null)
                    {
                        TechInfoList = new List<LegionTechBasicinfo>();
                    }
                    else
                    {
                        TechInfoList.Clear();
                    }
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (data[i].skillid != 0) 
                        {
                            var tempbasicinfo = new LegionTechBasicinfo(data[i].skillid,data[i].levelinfo,data[i].maxLevel);
                            TechInfoList.Add(tempbasicinfo);
                        }
                    }
                    isTechInfoInit = true;
                }
            }
        }

        private void UpdataBasicInfoList()
        {
            if (isTechInfoInit)
            {
                var data = Data.AllianceTemplateManager.Instance.mTechSkillList;
                for (int i = 0; i < TechInfoList.Count; i++)
                {
                    var tempinfo = TechInfoList[i];
                    if (TechlevelDic.TryGetValue(tempinfo.skillId, out int skilllevel))
                    {
                        if (skilllevel != tempinfo.level)
                        {
                            tempinfo.RefreshLevelUpdata(skilllevel);
                            LegionLogic.GetInstance().IsHaveCouldLevelupTechSkill();
                            //红点刷新

                        }
                    }
                }
            }
        }
        private void OnAutoReflashData(int seq)
        {
            //Data.AllianceTechChest allianceTechChest = Data.AllianceTemplateManager.Instance.GetAllianceTechChestInfoByLevel(techchestLevel);
            if (CurtechChest != null)
            {
                curGold = curGold + CurtechChest.goldrate * 1;
                curExp = curExp + CurtechChest.exprate * 1;
                curVigor = curVigor + CurtechChest.vigorrate * 1;
                if (curGold > CurtechChest.goldmax) curGold = CurtechChest.goldmax;
                if (curExp > CurtechChest.expmax) curExp = CurtechChest.expmax;
                if (curVigor > CurtechChest.vigormax) curVigor = CurtechChest.vigormax;
                Messenger.Raise(EventName.LegionTechChestUpdate);
                LegionLogic.GetInstance().isHaveTechResCanRecieve();
                refreshTimer = ILRTimerManager.instance.AddTimer(3600 * 1000, 1, OnAutoReflashData);
            }
        }

        public void RemoveTimer()
        {
            if (refreshTimer != 0)
            {
                ILRTimerManager.instance.RemoveTimer(refreshTimer);
                refreshTimer = 0;
            }
        }
    }

    public class LegionTechAdd
    {
        public int techLevel = 0;

        public float techAdd = 0;

        public int nextLegionLevel = -1;
    }
    
    
    public class LegionMemberMercenary
    {
        /// <summary> 成员uid</summary>
        public long uid;
        /// <summary> 成员名称</summary>
        public string memberName = "";
        /// <summary> 成员等级</summary>
        public int level;
        /// <summary> 成员头像</summary>
        public string portrait;
        /// <summary> 成员领队Id</summary>
        public int templateId;
        /// <summary> 皮肤</summary>
        public int skin;
        /// <summary> 头像框</summary>
        public string headFrame;
        /// <summary> 成员军团职务</summary>
        public eAllianceMemberRole dutyType;
        /// <summary> 成员军团总贡献</summary>
        public int totaldonate;
        /// <summary> 成员军团当日贡献</summary>
        public int todaydonate;
        /// <summary> 成员状态</summary>
        public byte state;
        /// <summary> 断线时间</summary>
        public double offlineTime;

        /// <summary> 离线小时数 </summary>
        public int offlineHour;
        public int todayLuxuryDonateTimes;
        public int todayHcDonateTimes;
        public int todayGoldDonateTimes;
        /// <summary> 今天捐献次数 </summary>
        public int todayDonateTimes;
        /// <summary> 职位中文</summary>
        public string dutyTypeStr;
    }
    

    public class LegionMemberData
    {
        /// <summary> 成员uid</summary>
        public long uid;
        /// <summary> 成员名称</summary>
        public string memberName = "";
        /// <summary> 成员等级</summary>
        public int level;
        /// <summary> 成员头像</summary>
        public string portrait;
        /// <summary> 成员领队Id</summary>
        public int templateId;
        /// <summary> 皮肤</summary>
        public int skin;
        /// <summary> 头像框</summary>
        public string headFrame;
        /// <summary> 成员军团职务</summary>
        public eAllianceMemberRole dutyType;
        /// <summary> 成员军团总贡献</summary>
        public int totaldonate;
        /// <summary> 成员军团当日贡献</summary>
        public int todaydonate;
        /// <summary> 成员状态</summary>
        public byte state;
        /// <summary> 断线时间</summary>
        public double offlineTime;

        /// <summary> 离线小时数 </summary>
        public int offlineHour;
        public int todayLuxuryDonateTimes;
        public int todayHcDonateTimes;
        public int todayGoldDonateTimes;
        /// <summary> 今天捐献次数 </summary>
        public int todayDonateTimes;
        /// <summary> 职位中文</summary>
        public string dutyTypeStr;
        
        /// <summary> 雇佣兵战力</summary>
        public int br;
    }

    public class RequestJoinData
    {
        public long approveID;
        public string name = "";
        public string headIcon;
        public string headFrame;
        public int level;
    }

    public class ApplyItemData
    {
        /// <summary> 军团id </summary>
        public int legionID;

        /// <summary> 申请时间 </summary>
        public double applyTime;
    }

    public class MessageItemData
    {
        public int year;
        public int month;
        public int day;
        public List<MessageCellData> listCell = new List<MessageCellData>();

        public bool IsContain(int year, int month, int day)
        {
            if (this.year == year && this.month == month && this.day == day)
            {
                return true;
            }
            return false;
        }
    }

    public class MessageCellData
    {
        /// <summary> 小时 </summary>
        public int hour;
        /// <summary> 分钟 </summary>
        public int min;
        /// <summary> 内容 </summary>
        public string content;
    }

    public class PlayerFormationData
    {
        /// <summary> 伙伴heroStats表的单位ID </summary>
        public int templateId;
        /// <summary> 伙伴星级 </summary>
        public int star;
        /// <summary> 伙伴等级 </summary>
        public int level;
        /// <summary> 伙伴巅峰 </summary>
        public int peak;
        /// <summary> 伙伴突破等级 </summary>
        public int awakeLevel;
        /// <summary> 伙伴属性 </summary>
        public int charType;
        /// <summary> 是否觉醒 </summary>
        public int isAwaken;
        /// <summary> 皮肤 </summary>
        public int skin;

        public PlayerFormationData()
        {
            templateId = 0;
            star = 1;
            level = 1;
            awakeLevel = 0;
            charType = 1;
            isAwaken = 0;
            skin = 0;
        }
    }

    public class LegionMemberComparer : IComparer<LegionMemberData>
    {
        public int Compare(LegionMemberData x, LegionMemberData y)
        {
            if (x.offlineTime <= 0 && y.offlineTime > 0)
            {
                return -1;
            }
            else if (y.offlineTime <= 0 && x.offlineTime > 0)
            {
                return 1;
            }

            if (x.dutyType != y.dutyType)
            {
                return (int)y.dutyType - (int)x.dutyType;
            }

            if (x.level != y.level)
            {
                return y.level - x.level;
            }

            if (x.totaldonate != y.totaldonate)
            {
                return y.totaldonate - x.totaldonate;
            }

            return 1;
        }
    }
}

