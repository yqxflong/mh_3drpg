using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTInstanceUtil
    {
        /// <summary>
        /// 计算前置章节是否完成
        /// </summary>
        /// <param name="curChapterId"></param>
        /// <returns></returns>
        public static bool IsPreChapterComplete(string curChapterId)
        {
            Hotfix_LT.Data.LostMainChapterTemplate chapterData = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById(curChapterId);
            if (chapterData == null)
            {
                return false;
            }

            if (chapterData.ForwardChapterId == "0")
            {
                return true;
            }

            return IsChapterComplete(chapterData.ForwardChapterId);
        }

        /// <summary>
        /// 第一章节是否已完成
        /// </summary>
        /// <returns></returns>
        public static bool IsFirstChapterCompleted()
        {
            return IsChapterComplete("101");
        }

        /// <summary>
        /// 计算当前章节是否完成
        /// </summary>
        /// <param name="chapterId"></param>
        /// <returns></returns>
        public static bool IsChapterComplete(string chapterId)
        {
            Hotfix_LT.Data.LostMainCampaignsTemplate bossCampaignData = Hotfix_LT.Data.SceneTemplateManager.Instance.GetBossLostMainCampaignTplByChapterId(chapterId);
            if (bossCampaignData == null)
            {
                return false;
            }

            int isComplete = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("userCampaignStatus.normalChapters.{0}.campaigns.{1}.complete", chapterId, bossCampaignData.Id), out isComplete);

            return isComplete > 0;
        }

        /// <summary>
        /// 计算当前小关是否完成
        /// </summary>
        /// <param name="chapterId"></param>
        /// <returns></returns>
        public static bool IsCampaignsComplete(string campaignId)
        {
            string chapterId = campaignId.Substring(0, 3);
            int isComplete = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("userCampaignStatus.normalChapters.{0}.campaigns.{1}.complete", chapterId, campaignId), out isComplete);
            return isComplete > 0;
        }

        /// <summary>
        /// 根据关卡ID判定关卡所在章节能否进入
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        public static bool GetChapterIsPassByCampaignId(string campaignId)
        {
            Hotfix_LT.Data.LostMainCampaignsTemplate campaignData = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(campaignId);
            if (campaignData == null)
            {
                return false;
            }

            Hotfix_LT.Data.LostMainChapterTemplate chapterData = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById(campaignData.ChapterId);
            if (chapterData == null)
            {
                return false;
            }

            return BalanceResourceUtil.GetUserLevel() >= chapterData.LevelLimit && LTInstanceUtil.IsPreChapterComplete(campaignData.ChapterId)&& GetIsChapterLimitConditionComplete(chapterData,out int num);
        }

        /// <summary>
        /// 根据章节ID判定能否进入章节
        /// </summary>
        public static bool GetChapterIsPassByChapterId(string chapterId)
        {
            Data.LostMainChapterTemplate chapterData = Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById(chapterId);

            if (chapterData == null)
            {
                return false;
            }

            return BalanceResourceUtil.GetUserLevel() >= chapterData.LevelLimit && IsPreChapterComplete(chapterId);
        }

        /// <summary>
        /// 在指定章节前（含指定章节）是否存在未通关关卡
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        public static bool HasUnfinishedCampaignBefore(string campaignId)
        {
            Data.LostMainCampaignsTemplate campaignData = Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(campaignId);

            if (campaignData == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(campaignData.ChapterId))
            {
                return false;
            }

            var id = int.Parse(campaignData.ChapterId);

            for (var i = 101; i <= id; i++) 
            {
                Data.LostMainChapterTemplate chapterData = Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById(i.ToString());

                if (chapterData == null || chapterData.CampaignList == null)
                {
                    continue;
                }

                for (var j = 0; j < chapterData.CampaignList.Count; j++)
                {
                    if (!IsCampaignsComplete(chapterData.CampaignList[j]))
                    {
                        return true;
                    }
                } 
            }

            return false;
        }

        public static int GetMaxChapterId()
        {
            int maxChapterId = 101;
            Hashtable chapterData;

            if (DataLookupsCache.Instance.SearchDataByID("userCampaignStatus.normalChapters", out chapterData))
            {
                foreach (DictionaryEntry data in chapterData)
                {
                    int chapterId;
                    string str = data.Key.ToString();

                    if (int.TryParse(str, out chapterId))
                    {
                        maxChapterId = Mathf.Max(chapterId, maxChapterId);
                    }
                }
            }

            return maxChapterId;
        }

        /// <summary>
        /// 获取当前章节完成度
        /// </summary>
        /// <param name="chapterId"></param>
        /// <param name="allStarNum"></param>
        /// <param name="completeNum"></param>
        public static int GetChapterCurStarNum(string chapterId)
        {
            int allStarNum = 0;
            Hashtable campaignList = Johny.HashtablePool.Claim();
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(string.Format("userCampaignStatus.normalChapters.{0}.campaigns", chapterId), out campaignList);
            if (campaignList != null)
            {
                foreach (DictionaryEntry campaignData in campaignList)
                {
                    Hotfix_LT.Data.LostMainCampaignsTemplate tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(campaignData.Key.ToString());
                    if (tpl != null)
                    {
                        int starNum = EB.Dot.Integer("star", campaignData.Value, 0);
                        allStarNum += starNum;
                    }
                }
            }
            return allStarNum;
        }

        public static bool GetChapterIsPerfectComplete(string chapterId)
        {
            int max = 0;
            int cur = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("userCampaignStatus.normalChapters.{0}.progress.max", chapterId), out max);
            DataLookupsCache.Instance.SearchIntByID(string.Format("userCampaignStatus.normalChapters.{0}.progress.cur", chapterId), out cur);
            if (max != 0)
            {
                return cur == max;
            }
            return false;
        }

        /// <summary>
        /// 获取章节星星数
        /// </summary>
        /// <param name="chapterId"></param>
        /// <returns></returns>
        public static int GetChapterStarNum(string chapterId)
        {
            int chapterStarNum = 0;
            Hashtable campaignList = Johny.HashtablePool.Claim();
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(string.Format("userCampaignStatus.normalChapters.{0}.campaigns", chapterId), out campaignList);
            if (campaignList != null)
            {
                foreach (DictionaryEntry campaignData in campaignList)
                {
                    int starNum = EB.Dot.Integer("star", campaignData.Value, 0);
                    chapterStarNum += starNum;
                }
            }
            return chapterStarNum;
        }

        /// <summary>
        /// 查看章节宝箱是否领取
        /// </summary>
        /// <param name="chapterId"></param>
        /// <param name="star"></param>
        /// <returns></returns>
        public static bool IsChapterStarBoxOpen(string chapterId, int star)
        {
            bool isOpen = false;
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userCampaignStatus.normalChapters.{0}.box.{1}", chapterId, star), out isOpen);
            return isOpen;
        }

        /// <summary>
        /// 生成星级宝箱奖励数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<LTShowItemData> ParseStarBoxData(Hashtable data)
        {
            List<LTShowItemData> itemList = new List<LTShowItemData>();
            if (data != null)
            {
                foreach (DictionaryEntry item in data)
                {
                    string id = EB.Dot.String("economy_id", item.Value, string.Empty);
                    int curNum = EB.Dot.Integer("num", item.Value, 0);
                    int pastNum = GameItemUtil.GetInventoryItemNum(id);
                    //string type = EB.Dot.String("system", item.Value, string.Empty);
                    LTShowItemData itemData = new LTShowItemData(id, curNum - pastNum, LTShowItemType.TYPE_GAMINVENTORY);
                    itemList.Add(itemData);
                }
            }
            return itemList;
        }

        /// <summary>
        /// 获取扫荡数据
        /// </summary>
        /// <returns></returns>
        public static List<LTMainInstanceBlitzData> GetBlitzData()
        {
            List<LTMainInstanceBlitzData> itemList = new List<LTMainInstanceBlitzData>();
            ArrayList rewardList = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("combat.rewards", out rewardList);
            for (int i = 0; i < rewardList.Count; i++)
            {
                LTMainInstanceBlitzData itemData = new LTMainInstanceBlitzData();
                itemData.ItemList = new List<LTShowItemData>();
                int count = EB.Dot.Integer("exp", rewardList[i], 0);
                if (count > 0)
                {
                    itemData.ItemList.Add(new LTShowItemData("exp", count, "res", false));
                }

                count = EB.Dot.Integer("poten-gold", rewardList[i], 0);
                if (count > 0)
                {
                    itemData.ItemList.Add(new LTShowItemData("poten-gold", count, "res", false));
                }
                itemData.ItemList.Add(new LTShowItemData("gold", EB.Dot.Integer("gold", rewardList[i], 0), "res", false));
                itemData.ItemList.Add(new LTShowItemData("buddy-exp", EB.Dot.Integer("buddyExp.quantity", rewardList[i], 0), "res", false));
                ArrayList rewardItemList = Hotfix_LT.EBCore.Dot.Array("rewardItems", rewardList[i], null);
                if (rewardItemList != null)
                {
                    for (int j = 0; j < rewardItemList.Count; j++)
                    {
                        string id = EB.Dot.String("data", rewardItemList[j], string.Empty);
                        int num = EB.Dot.Integer("quantity", rewardItemList[j], 0);
                        string type = EB.Dot.String("type", rewardItemList[j], string.Empty);
                        if (!string.IsNullOrEmpty(id))
                        {
                            bool isHave = false;
                            for (int k = 0; k < itemData.ItemList.Count; k++)
                            {
                                if (id == itemData.ItemList[k].id)
                                {
                                    isHave = true;
                                    itemData.ItemList[k].count += num;
                                    break;
                                }
                            }

                            if (!isHave)
                            {
                                itemData.ItemList.Add(new LTShowItemData(id, num, type, false));
                            }
                        }
                    }
                }
                itemData.Index = i + 1;
                itemList.Add(itemData);
            }
            return itemList;
        }

        /// <summary>
        /// 获取快速获取进阶材料数据，本质还是读取的扫荡数据，只是修改输出形式
        /// </summary>
        /// <returns></returns>
        public static List<LTShowItemData> GetQuicklyBlitzData()
        {
            List<LTShowItemData> itemDatas = new List<LTShowItemData>();
            Dictionary<string, LTShowItemData> itemDataStat = new Dictionary<string, LTShowItemData>();
            ArrayList rewardList = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("combat.rewards", out rewardList);
            if (rewardList == null)
            {
                return null;
            }
            for (int i = 0; i < rewardList.Count; i++)
            {
                LTShowItemData temp;
                int count = EB.Dot.Integer("exp", rewardList[i], 0);
                if (count > 0)
                {

                    if (itemDataStat.ContainsKey("exp"))
                    {
                        itemDataStat["exp"].count += count;
                    }
                    else
                    {
                        temp = new LTShowItemData("exp", count, "res");
                        itemDataStat.Add("exp", temp);
                    }
                }

                count = EB.Dot.Integer("poten-gold", rewardList[i], 0);
                if (count > 0)
                {
                    if (itemDataStat.ContainsKey("poten-gold"))
                    {
                        itemDataStat["poten-gold"].count += count;
                    }
                    else
                    {
                        temp = new LTShowItemData("poten-gold", count, "res");
                        itemDataStat.Add("poten-gold", temp);
                    }
                }
                temp = new LTShowItemData("gold", EB.Dot.Integer("gold", rewardList[i], 0), "res");
                if (itemDataStat.ContainsKey("gold"))
                {
                    itemDataStat["gold"].count += temp.count;
                }
                else
                {

                    itemDataStat.Add("gold", temp);
                }
                temp = new LTShowItemData("buddy-exp", EB.Dot.Integer("buddyExp.quantity", rewardList[i], 0), "res");
                if (itemDataStat.ContainsKey("buddy-exp"))
                {
                    itemDataStat["buddy-exp"].count += temp.count;
                }
                else
                {

                    itemDataStat.Add("buddy-exp", temp);
                }
                ArrayList rewardItemList = Hotfix_LT.EBCore.Dot.Array("rewardItems", rewardList[i], null);
                if (rewardItemList != null)
                {
                    for (int j = 0; j < rewardItemList.Count; j++)
                    {
                        string id = EB.Dot.String("data", rewardItemList[j], string.Empty);
                        int num = EB.Dot.Integer("quantity", rewardItemList[j], 0);
                        string type = EB.Dot.String("type", rewardItemList[j], string.Empty);
                        if (!string.IsNullOrEmpty(id))
                        {
                            if (itemDataStat.ContainsKey(id))
                            {
                                itemDataStat[id].count += num;
                            }
                            else
                            {
                                itemDataStat.Add(id, new LTShowItemData(id, num, type));
                            }
                        }
                    }
                }

            }
            foreach (var item in itemDataStat)
            {
                itemDatas.Add(item.Value);
            }
            return itemDatas;
        }

        public static List<LTAwakeningInstanceBlitzData> GetBlitzDataChange()
        {
            List<LTAwakeningInstanceBlitzData> itemList = new List<LTAwakeningInstanceBlitzData>();
            ArrayList rewardList = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("combat.rewards", out rewardList);
            for (int i = 0; i < rewardList.Count; i++)
            {
                LTAwakeningInstanceBlitzData itemData = new LTAwakeningInstanceBlitzData();
                itemData.ItemList = new List<LTShowItemData>();
                int count = EB.Dot.Integer("exp", rewardList[i], 0);
                if (count > 0)
                {
                    itemData.ItemList.Add(new LTShowItemData("exp", count, "res"));
                }

                count = EB.Dot.Integer("poten-gold", rewardList[i], 0);
                if (count > 0)
                {
                    itemData.ItemList.Add(new LTShowItemData("poten-gold", count, "res"));
                }
                itemData.ItemList.Add(new LTShowItemData("gold", EB.Dot.Integer("gold", rewardList[i], 0), "res"));
                //            itemData.ItemList.Add(new LTShowItemData("buddy-exp", EB.Dot.Integer("buddyExp.quantity", rewardList[i], 0), "res"));
                ArrayList rewardItemList = Hotfix_LT.EBCore.Dot.Array("rewardItems", rewardList[i], null);
                if (rewardItemList != null)
                {
                    for (int j = 0; j < rewardItemList.Count; j++)
                    {
                        string id = EB.Dot.String("data", rewardItemList[j], string.Empty);
                        int num = EB.Dot.Integer("quantity", rewardItemList[j], 0);
                        string type = EB.Dot.String("type", rewardItemList[j], string.Empty);
                        if (!string.IsNullOrEmpty(id))
                        {
                            bool isHave = false;
                            for (int k = 0; k < itemData.ItemList.Count; k++)
                            {
                                if (id == itemData.ItemList[k].id)
                                {
                                    isHave = true;
                                    itemData.ItemList[k].count += num;
                                    break;
                                }
                            }

                            if (!isHave && num > 0)
                            {
                                itemData.ItemList.Add(new LTShowItemData(id, num, type));
                            }
                        }
                    }
                }
                itemData.Index = i + 1;
                itemList.Add(itemData);
            }
            return itemList;
        }

        public static List<LTShowItemData> GetChallengeBlitzData(Hashtable data)
        {
            List<LTShowItemData> itemList = new List<LTShowItemData>();
            Hashtable inventoryData = EB.Dot.Object("inventory", data, null);
            foreach (DictionaryEntry pair in inventoryData)
            {
                string id = EB.Dot.String("economy_id", pair.Value, string.Empty);
                var curNum = 0;
                DataLookupsCache.Instance.SearchIntByID("inventory." + pair.Key.ToString() + ".num", out curNum);
                int num = EB.Dot.Integer("num", pair.Value, 0) - curNum;
                string type = EB.Dot.String("system", pair.Value, string.Empty) == "HeroShard" ? LTShowItemType.TYPE_HEROSHARD : LTShowItemType.TYPE_GAMINVENTORY;
                if (!string.IsNullOrEmpty(id))
                {
                    itemList.Add(new LTShowItemData(id, num, type));
                }
            }
            return itemList;
        }

        public static int GetChallengeLevel(int data)
        {
            var temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeChapterById(data);
            return temp.CurChapter;
        }

        //获取主线副本章节特殊解锁条件是否满足
        public static bool GetIsChapterLimitConditionComplete(Data.LostMainChapterTemplate chaptertpl, out int curnum)
        {
            curnum = 0;
            if (chaptertpl == null)
            {
                EB.Debug.LogWarning("NULL of lostmainchapter info, please cheak Scene form");
                return false;
            }
            int needplayer = chaptertpl.Limitparam1;
            int upgrandeid = chaptertpl.Limitparam2;
            if (needplayer <= 0) return true;
            List<LTPartnerData> partnerlist = LTPartnerDataManager.Instance.GetGeneralPartnerList();
            if (partnerlist == null)
            {
                EB.Debug.LogError("partnerlist == null");
                return true;
            }

            for (int i = 0; i < partnerlist.Count; i++)
            {
                int upgrade = partnerlist[i].UpGradeId;
                if (upgrade >= upgrandeid)
                {
                    needplayer -= 1;
                    curnum += 1;
                    if (needplayer <= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //获取当前完成最大关卡
        public static int GetCurMaxCompleteCampaignId(Hashtable data)
        {
            if (data == null || data.Count <= 0) return 0;
            int maxchapter = 0;
            foreach (DictionaryEntry fl in data)
            {
                int curChapter = EB.Dot.Integer("", fl.Key, 0);
                if (curChapter > maxchapter)
                {
                    maxchapter = curChapter;
                }
            }
            int maxcapiD = 0;
            GetCurCompleteCampagin(maxchapter, data, out maxcapiD);
            return maxcapiD;
        }

        private static void GetCurCompleteCampagin(int curchapter, Hashtable data, out int maxcapiD)
        {
            maxcapiD = 0;
            Hashtable lagest = EB.Dot.Object(string.Format("{0}.campaigns", curchapter), data, null);
            if (lagest == null && curchapter <= 101)
            {
                maxcapiD = 0;
                return;
            }
            else if(lagest == null && curchapter>101)
            {
                curchapter -= 1;
                GetCurCompleteCampagin(curchapter, data, out maxcapiD);
            }
            else
            {
                foreach (DictionaryEntry fl in lagest)
                {
                    if (EB.Dot.Integer("complete", fl.Value, 0) >= 1)
                    {
                        int campid = EB.Dot.Integer("", fl.Key, 0);
                        maxcapiD = maxcapiD > campid ? maxcapiD : campid;
                    }
                }
                if(maxcapiD == 0)
                {
                    curchapter -= 1;
                    GetCurCompleteCampagin(curchapter, data, out maxcapiD);
                }
            }
        }

        public static int GetCurMaxCompleteCampaignId()
        {
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userCampaignStatus.normalChapters", out Hashtable data);
            if (data == null || data.Count <= 0) return 0;
            return GetCurMaxCompleteCampaignId(data);
        }

        public static bool GetIsChapterLimitConditionComplete(string chapterId, out int curnum)
        {
            Hotfix_LT.Data.LostMainChapterTemplate chapterData = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById(chapterId);
            return GetIsChapterLimitConditionComplete(chapterData, out curnum);
        }
        
        public static bool GetChapterIsOpen(int chapterId)
        {
            Hashtable chapterData = Johny.HashtablePool.Claim();
            DataLookupsCache.Instance.SearchHashtableByID("userCampaignStatus.normalChapters", out chapterData);
            int maxChapterId = 101;
            foreach (DictionaryEntry data in chapterData)
            {
                int chapterIntId = int.Parse(data.Key.ToString());
                if (chapterIntId > maxChapterId)
                {
                    maxChapterId = chapterIntId;
                }
            }
            Hotfix_LT.Data.LostMainChapterTemplate tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetNextChapter(maxChapterId.ToString());
            if (tpl != null && LTInstanceUtil.IsChapterComplete(maxChapterId.ToString()) && BalanceResourceUtil.GetUserLevel() >= tpl.LevelLimit && LTInstanceUtil.GetIsChapterLimitConditionComplete(tpl, out int num))
            {
                maxChapterId = int.Parse(tpl.Id);
            }

            return chapterId <= maxChapterId;
        }

    }
}