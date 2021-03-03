using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;
using System.Text;
using Hotfix_LT.Data;


namespace Hotfix_LT.UI
{
    public class LTPartnerHandbookManager : ManagerUnit
    {
        private static LTPartnerHandbookManager sInstance = null;

        public static LTPartnerHandbookManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<LTPartnerHandbookManager>(); }
        }

        public override void Connect()
        {
            //State = SubSystemState.Connected;
        }

        public override void Disconnect(bool isLogout)
        {
            //State = SubSystemState.Disconnected;
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        public PartnerHandbookApi Api
        {
            get; private set;
        }

        public const string ListDataId = "mannualInfo.mannualInfo";
        public const string LevelDataId = "mannualInfo.level";
        public const string SpointDataId = "mannualInfo.point";
        public const string BreakLevelId = "mannualInfo.mannualInfo.{0}.break";

        public HandbookList TheHandbookList = new HandbookList();
        public static bool isAllHandBookOpen = false;//是否开启全图鉴
        private int unlockLevel = -1;
        public int UnLockLevel         
        {
            get
            {
                if(unlockLevel == -1)
                {
                    unlockLevel = CharacterTemplateManager.Instance.GetMannualUnlockLevel();
                }
                return unlockLevel;
            }
        }
        
        public override void Initialize(Config config)
        {
            Instance.Api = new PartnerHandbookApi();
            Instance.Api.ErrorHandler += ErrorHandler;
            TheHandbookList = GameDataSparxManager.Instance.Register<HandbookList>(ListDataId);
            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, Instance.DataRefresh);

        }

        public override void OnLoggedIn()
        {
            base.OnLoggedIn();

            isRequest = false;
            timeLimit = 0;
        }

        public int GetHandBookLevel()
        {
            int level;
            DataLookupsCache.Instance.SearchIntByID(LevelDataId, out level);
            return level;
        }

        public int GetHandBookSpoint()
        {
            int point;
            DataLookupsCache.Instance.SearchIntByID(SpointDataId, out point);
            return point;
        }

        public int GetBeralLevel(Hotfix_LT.Data.eRoleAttr role)
        {
            int level;
            DataLookupsCache.Instance.SearchIntByID(string.Format(BreakLevelId, (int)role), out level);
            return level;
        }

        public static int GetAllHandboolUnlockLevel(int page)//获取全图鉴解锁等级
        {
            return (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue(string.Format("mannual_unlock_{0}Page_level", page));
        }

        public float GetAttrAddValue(LTPartnerData partnerData)
        {
            float addNum = 0f;
            if (partnerData != null)
            {
                MannualRoleGradeTemplate roleGrade = CharacterTemplateManager.Instance.GetMannualRoleGradeTempleteByRoleGrade(partnerData.HeroInfo.role_grade);
                if (roleGrade != null)
                {
                    addNum =roleGrade.star_addition * 100f * (float)partnerData.Star;
                }
            }
            return addNum;
        }

        public string GetAttAddNum(LTPartnerData partnerData, IHandBookAddAttrType Type)
        {
            StringBuilder AddStr = new StringBuilder();
            AddStr.Remove(0, AddStr.Length);
            switch (Type)
            {
                case IHandBookAddAttrType.ATTR_CritP:
                    AddStr.Append(EB.Localizer.GetString("ID_ATTR_CritP"));
                    break;
                case IHandBookAddAttrType.ATTR_CRIresist:
                    AddStr.Append(EB.Localizer.GetString("ID_ATTR_CRIresist"));
                    break;
                case IHandBookAddAttrType.ATTR_Speed:
                    AddStr.Append(EB.Localizer.GetString("ID_ATTR_Speed"));
                    break;
                case IHandBookAddAttrType.ID_ATTR_DMGincrease:
                    AddStr.Append(EB.Localizer.GetString("ID_ATTR_DMGincrease"));
                    break;
                case IHandBookAddAttrType.ID_ATTR_DMGreduction:
                    AddStr.Append(EB.Localizer.GetString("ID_ATTR_DMGreduction"));
                    break;
                default:
                    EB.Debug.LogError("NO This Card : {0}" ,Type);
                    break;
            }

            AddStr.Append("+");
            AddStr.Append(GetAttrAddValue(partnerData).ToString("f1"));
            AddStr.Append("%");
            return AddStr.ToString();
        }

        public string GetType(Hotfix_LT.Data.eRoleAttr attr)
        {
            switch (attr)
            {
                case Hotfix_LT.Data.eRoleAttr.Feng: return "ID_FENG";
                case Hotfix_LT.Data.eRoleAttr.Shui: return "ID_SHUI";
                default: return "ID_HUO";
            }
        }

        /// <summary>
        /// 获取指定图鉴位置的加成属性
        /// </summary>
        /// <param name="attr">属性类型</param>
        /// <returns></returns>
        public float GetHandbookAddtionFromCard(LTPartnerData partner, IHandBookAddAttrType attr)
        {
            if (partner == null)
            {
                EB.Debug.LogError("Get Partner is null");
                return 0;
            }
            //int race = partner.HeroInfo.race;
            //int charType = (int)partner.HeroInfo.char_type;
            float addNum = 0;
            if (TheHandbookList == null)
            {
                EB.Debug.LogError("LTPartnerHandbookManager.GetHandbookAddtionFromCard TheHandbookList is null");
                return 0;
            }
            List<HandbookData> handbookDatas = TheHandbookList.Handbooks;
            if(handbookDatas == null)
            {
                EB.Debug.LogError("LTPartnerHandbookManager.GetHandbookAddtionFromCard handbookDatas is null");
                return 0;
            }
            for (var i = 0; i < handbookDatas.Count; i++)
            {
                HandbookData bookData = handbookDatas[i];
                if (bookData.HandbookId == partner.HeroInfo.char_type)
                {
                    for (int j = 0; j < (LTPartnerConfig.MAX_HANDBOOKPAGE + 1); j++)//兼容多页签
                    {
                        LTPartnerData cardPartner = bookData.CardsInfo[(int)attr + j * 5].SetHandBookCard();
                        if (cardPartner != null)
                        {
                            Hotfix_LT.Data.MannualRoleGradeTemplate roleGrade = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualRoleGradeTempleteByRoleGrade(cardPartner.HeroInfo.role_grade);
                            if (roleGrade != null)
                            {
                                addNum += cardPartner.Star * roleGrade.star_addition * 100;
                            }
                        }
                    }

                }
            }
            return addNum;
        }

        /// <summary>
        /// 计算单张图鉴卡片得分
        /// </summary>
        /// <param name="mannualBreakTemplate"></param>
        /// <param name="heroGrade"></param>
        /// <param name="heroLevel"></param>
        /// <returns></returns>
        public int HandleHandbookCardScore(Hotfix_LT.Data.MannualBreakTemplate mannualBreakTemplate, int heroGrade, int upgradeLevel)
        {
            Hotfix_LT.Data.MannualRoleGradeTemplate _CurrentRoleGradeInfo =
                Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualRoleGradeTempleteByRoleGrade(heroGrade);
            int baseScore = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetCharacterScoreByLevel(upgradeLevel);
            int originScore = (int)Math.Round(baseScore * _CurrentRoleGradeInfo.score_addition);
            int Score = originScore + (int)((float)originScore * mannualBreakTemplate.score_promotion);
            return Score;
        }

        /// <summary>
        /// 获取对应图鉴的加成属性
        /// </summary>
        /// <param name="handbookId">1,2,3</param>
        /// <returns></returns>
        public float GetHanbookAddtion(LTPartnerData partner)
        {
            //int race = partner.HeroInfo.race;
            //int charType = (int) partner.HeroInfo.char_type;
            float addNum = 0;
            if (TheHandbookList != null)
            {
                HandbookData handbookDatas = TheHandbookList.Find(partner.HeroInfo.char_type);
                if (handbookDatas != null)
                {
                    Hotfix_LT.Data.MannualBreakTemplate temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetBreakTemplateByLevel(handbookDatas.HandbookId, handbookDatas.BreakLevel);
                    int score = 0;// bookData.Score;
                    Hotfix_LT.Data.MannualScoreTemplate _scoreTemplate = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualScoreTemplateByScore(score);
                    addNum += _scoreTemplate.attribute_addition;
                }
            }
            return addNum;
        }

        /// <summary>
        /// 获取对应图鉴的加成属性
        /// </summary>
        /// <param name="handbookId">1,2,3,7,8,9,10,11</param>
        /// <returns></returns>
        public float GetHanbookAddtion(Hotfix_LT.Data.eRoleAttr handbookId)
        {
            List<HandbookData> handbookDatas = TheHandbookList.Handbooks;
            for (var i = 0; i < handbookDatas.Count; i++)
            {
                HandbookData bookData = handbookDatas[i];
                if (bookData.HandbookId == handbookId)
                {
                    int score = 0;//; bookData.Score;
                    Hotfix_LT.Data.MannualScoreTemplate _scoreTemplate = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualScoreTemplateByScore(score);
                    return _scoreTemplate.attribute_addition;
                }
            }
            EB.Debug.Log("can not find handbook id : {0}", handbookId);
            return 0;
        }

        /// <summary>
        /// 获取图鉴所有等级
        /// </summary>
        /// <returns></returns>
        public List<int> GetHandbookLevelList()
        {
            List<int> handBookLevels = new List<int>();
            List<HandbookData> handbookDatas = TheHandbookList.Handbooks;
            for (var i = 0; i < handbookDatas.Count; i++)
            {
                HandbookData bookData = handbookDatas[i];
                int score = 0;// bookData.Score;
                Hotfix_LT.Data.MannualScoreTemplate _scoreTemplate = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualScoreTemplateByScore(score);
                handBookLevels.Add(_scoreTemplate.id);
            }

            return handBookLevels;
        }

        public int GetTotleScore()
        {
            List<LTPartnerData> generalPL = LTPartnerDataManager.Instance.GetOwnPartnerList();
            int totle = 0;
            for (int i = 0; i < generalPL.Count; ++i)
            {
                totle += Hotfix_LT.Data.CharacterTemplateManager.Instance.GetCharacterUpgradeScore(generalPL[i].UpGradeId);
                Hotfix_LT.Data.MannualRoleGradeTemplate roleGrade = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualRoleGradeTempleteByRoleGrade(generalPL[i].HeroInfo.role_grade);
                if (roleGrade != null)
                    totle += roleGrade.ScoreList[generalPL[i].Star - 1];
            }
            return totle;
        }

        public Dictionary<string, int> PartnerScores = new Dictionary<string, int>();

        private void DataRefresh()
        {
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.handbook, HasHandBookRedPoint() ? 1:0);
        }

        //检查所有图鉴
        public bool HasHandBookRedPoint()
        {
            Hotfix_LT.Data.FuncTemplate m_FuncTpl = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10049);
            if (!m_FuncTpl.IsConditionOK()) return false;

            return IsHandBookCanLevelUp() || IsHandBookCanBreakUp() ||IsHandPartnerCanUp(eRoleAttr.Feng)
                   ||IsHandPartnerCanUp(eRoleAttr.Shui)||IsHandPartnerCanUp(eRoleAttr.Huo)|| IsCanGetScore();
        }

        public bool IsHandBookCanLevelUp()
        {
            int curLevel = LTPartnerHandbookManager.Instance.GetHandBookLevel();
            var nextHandBookInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualScoreTemplateById(curLevel + 1);
            return nextHandBookInfo != null && LTPartnerHandbookManager.Instance.GetHandBookSpoint() >= (nextHandBookInfo != null ? nextHandBookInfo.totleScore : 0);
        }


        public bool IsHandBookCanBreakUp(Hotfix_LT.Data.eRoleAttr attr = Hotfix_LT.Data.eRoleAttr.None)
        {
            if (GetHandBookLevel() < UnLockLevel) return false;
            if (TheHandbookList != null && TheHandbookList.Handbooks != null)
            {
                for (var i = 0; i < TheHandbookList.Handbooks.Count; i++)
                {
                    HandbookData handbook = TheHandbookList.Handbooks[i];
                    if (attr != Hotfix_LT.Data.eRoleAttr.None && attr != handbook.HandbookId) continue;
                    int curLevel = LTPartnerHandbookManager.Instance.GetHandBookLevel();
                    var curHandBookInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualScoreTemplateById(curLevel);
                    if (curHandBookInfo == null || handbook.BreakLevel >= curHandBookInfo.levelLimit)
                    {
                        continue;
                    }

                    var data = LTPartnerHandbookManager.Instance.TheHandbookList.Find(handbook.HandbookId);
                    Hotfix_LT.Data.MannualBreakTemplate breakThrough = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetBreakTemplateByLevel(data.HandbookId, data.BreakLevel + 1);
                    if (breakThrough != null)
                    {
                        int curCount = GameItemUtil.GetInventoryItemNum(breakThrough.material_1);
                        int nextCount = breakThrough.quantity_1;
                        if (curCount < nextCount)
                        {
                            continue;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsHandPartnerCanUp(eRoleAttr handbookId)
        {
            if (GetHandBookLevel() < UnLockLevel) return false;
            if (TheHandbookList == null)
            {
                EB.Debug.LogError("LTPartnerHandbookManager.IsHandPartnerCanUp TheHandbookList is null");
                return false;
            }
            HandbookData templist = TheHandbookList.Find(handbookId);
            if(templist == null)
            {
                EB.Debug.LogError("LTPartnerHandbookManager.IsHandPartnerCanUp templist is null");
                return false;
            }
            return templist.HasAvailableCard;
        }

        private int timeLimit = 0;
        private bool isHasScore = false;
        public bool IsCanGetScore()
        {
            if (timeLimit < EB.Time.Now)
            {
                timeLimit = EB.Time.Now + 3600;//每一小时更新
                isHasScore = (GetTotleScore() > GetHandBookSpoint());
            }
            return isHasScore;
        }
        public void ResetHasScore()
        {
            isHasScore = false;
        }

        private bool isRequest = false;
        public void GetHandbookList()
        {
            Api.GetHandbookList(-1, NewFetchDataHandler);
        }


        public void TakeTheField(string buddyId, int type, int position, System.Action<Hashtable> callback)
        {
            if (isRequest) return;
            isRequest = true;
            Api.TakeTheField(buddyId, type, position, delegate (Hashtable result)
            {
                isRequest = false;
                callback(result);
                FetchDataHandler((result));
            });
        }

        public void QuitTheField(string buddyId, int type, int position, System.Action<Hashtable> callback)
        {
            if (isRequest) return;
            isRequest = true;
            Api.QuitTheField(buddyId, type, position, delegate (Hashtable result)
            {
                isRequest = false;
                callback(result);
                FetchDataHandler((result));
            });
        }

        public void TransferField(string buddyId, int fromType, int toType, int fromPosition, int toPosition, System.Action<Hashtable> callback)
        {
            if (isRequest) return;
            isRequest = true;
            Api.TransferField(buddyId, fromType, toType, fromPosition, toPosition, delegate (Hashtable result)
            {
                isRequest = false;
                callback(result);
                FetchDataHandler((result));
            });
        }

        public void UnLockCardPosition(int type, int position)
        {
            Api.UnLockCardPosition(type, position, FetchDataHandler);
        }

        public void BreakThrough(int type, System.Action<Hashtable> callback)
        {
            if (isRequest) return;
            isRequest = true;
            Api.BreakThrough(type, delegate (Hashtable result)
            {
                isRequest = false;
                if (result != null)
                    callback(result);
                FetchDataHandler((result));
            });
        }

        public void GetPoint(int point, System.Action<Hashtable> callback)
        {
            if (isRequest) return;
            isRequest = true;
            Api.GetPoint(point, delegate (Hashtable result)
            {
                isRequest = false;
                DataLookupsCache.Instance.CacheData(result);
                if (result != null) callback(result);
            });
        }

        public void MagicBookLevelUp(int level, System.Action<Hashtable> callback)
        {
            Api.MagicBookLevelUp(level, delegate (Hashtable result)
            {
                isRequest = false;
                DataLookupsCache.Instance.CacheData(result);
                if (result != null) callback(result);
            });
        }

        private void NewFetchDataHandler(Hashtable data)
        {
            if (data != null)
            {
                DataLookupsCache.Instance.CacheData(data);
                GameDataSparxManager.Instance.ProcessIncomingData(data, false);
            }
            else
               EB.Debug.LogWarning("Handbook Data is null");
        }

        private void FetchDataHandler(Hashtable data)
        {
            if (data != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(data, false);
            }
            GetHandbookList();
        }

        private void MergeDataHandler(Hashtable data)
        {
            if (data != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(data, true);
            }
        }
    }
}