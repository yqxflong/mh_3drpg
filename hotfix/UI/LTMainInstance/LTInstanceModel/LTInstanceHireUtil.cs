using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTInstanceHireUtil
    {
        /// <summary>
        /// 根据雇佣兵列表伪造PartnerDataList(仅限战前界面使用)
        /// </summary>
        /// <returns></returns>
        public static List<LTPartnerData> GetHireList()
        {
            ArrayList hireList = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("userCampaignStatus.challengeChapters.majordata.hirelist", out hireList);
            return GetHirePartnerList(hireList);
        }

        /// <summary>
        /// 根据雇佣兵基本信息伪造PartnerDataList(仅限战前界面使用)
        /// </summary>
        /// <param name="monsterList"></param>
        /// <returns></returns>
        private static List<LTPartnerData> GetHirePartnerList(ArrayList monsterList)
        {
            List<LTPartnerData> hirePartnerList = new List<LTPartnerData>();
            if (monsterList == null)
            {
                return hirePartnerList;
            }
            for (int i = 0; i < monsterList.Count; i++)
            {
                var tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(int.Parse(monsterList[i].ToString()));
                if (tpl != null)
                {
                    var hirePartner = GetPartnerDataByInfoId(tpl, i);
                    if (hirePartner != null)
                    {
                        hirePartner.IsHire = true;
                        hirePartnerList.Add(hirePartner);
                    }
                }
            }
            return hirePartnerList;
        }

        /// <summary>
        /// 根据雇佣兵基础信息伪造PartnerData(仅限战前界面使用)
        /// </summary>
        /// <param name="monsterInfo"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static LTPartnerData GetPartnerDataByInfoId(Hotfix_LT.Data.MonsterInfoTemplate monsterInfo, int index)
        {
            var tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStatByInfoId(int.Parse(monsterInfo.character_id));
            if (tpl != null)
            {
                LTPartnerData partnerData = new LTPartnerData();
                partnerData.StatId = tpl.id;
                partnerData.InfoId = int.Parse(monsterInfo.character_id);
                partnerData.HireHeroId = index + 1;
                partnerData.HeroStat = tpl;
                partnerData.HeroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(partnerData.InfoId);
                partnerData.IsHire = true;
                partnerData.HireLevel = monsterInfo.level;
                partnerData.HireUpGradeId = monsterInfo.upgrade;
                partnerData.HireStar = monsterInfo.star;
                return partnerData;
            }
            return null;
        }

        /// <summary>
        /// 根据heroId伪造PartnerData(仅限战前界面使用)
        /// </summary>
        /// <param name="heroId">正数heroId</param>
        /// <returns></returns>
        public static LTPartnerData GetHirePartnerDataByHeroId(int heroId)
        {
            Hotfix_LT.Data.MonsterInfoTemplate monsterInfo = GetHireInfoByHeroId(-heroId);
            int index = heroId - 1;
            if (monsterInfo != null)
            {
                return GetPartnerDataByInfoId(monsterInfo, index);
            }
            return null;
        }

        /// <summary>
        /// 根据heroId获取雇佣兵基础信息(禁止战前界面使用)
        /// </summary>
        /// <param name="heroId">负数heroId</param>
        /// <returns></returns>
        public static Hotfix_LT.Data.MonsterInfoTemplate GetHireInfoByHeroId(int heroId)
        {
            int index = -heroId - 1;
            ArrayList hireList = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("userCampaignStatus.challengeChapters.majordata.hirelist", out hireList);
            if (hireList != null)
            {
                if (hireList.Count > index)
                {
                    var tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(int.Parse(hireList[index].ToString()));
                    return tpl;
                }
            }
            return null;
        }
        
    }
}