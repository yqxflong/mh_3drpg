using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public enum InstanceType
    {
        Challenge,
        AlienMaze,
    }
    
    public class LTChallengeInstanceHpCtrl : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            isAlienMaze = mDMono .BoolParamList[0];
            HpSlider = t.GetComponent<UISlider>();
            HPForeground = t.GetComponent<UISprite>("HP");
            HpLabel = t.GetComponent<UILabel>("Label");
            hpTween = HpLabel.GetComponent<TweenScale>();
            teamInfoPath = (isAlienMaze) ? awTeamInfoPath : chTeamInfoPath;
            
            RequestHerosHP();
        }
        public bool isAlienMaze = false;//由界面上勾选去控制
    
        public enum HPEventType
        {
            INIT,
            ADD,
            REMOVE,
            BOOM,
            WENYI,
            SHENGJI,
        }
    
        private static string teamInfoPath = "userTeam.lt_challenge_camp.formation_info";
        private const string chTeamInfoPath = "userTeam.lt_challenge_camp.formation_info";
        private const string awTeamInfoPath = "userTeam.lt_aw_camp.formation_info";
    
        public static string curHpInfoPath = "userCampaignStatus.challengeChapters.team";
        public static string curGlobalHpLoss = "userCampaignStatus.challengeChapters.majordata.globalHPLoss";

        public UISlider HpSlider;

        public UISprite HPForeground;

        public UILabel HpLabel;
    
        public TweenScale hpTween;
    
        private bool isInitHp = false;
    
        public static Hashtable GetHeroDataById(int id, int pos)
        {
            Hashtable hero = Johny.HashtablePool.Claim();
    
            hero.Add("id", id);
            hero.Add("pos", pos);
    
            string templateId = string.Empty;
            if (id < 0)//雇佣兵
            {
                var hireInfo = LTInstanceHireUtil.GetHireInfoByHeroId(id);
                if (hireInfo != null)
                {
                    hero.Add("tid", hireInfo.character_id);
                }
            }
            else if (id > 0)
            {
                var partnerInfo = LTPartnerDataManager.Instance.GetPartnerByHeroId(id);
                if (partnerInfo != null)
                {
                    hero.Add("tid", partnerInfo.StatId.ToString());
                }
            }
            return hero;
        }

        public void RequestHerosHP(bool update = true)
        {
            ArrayList teamInfo = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>(teamInfoPath, out teamInfo);
            ArrayList heroes = Johny.ArrayListPool.Claim();
            for (int i = 0; i < teamInfo.Count; i++)
            {
                int heroId = EB.Dot.Integer("hero_id", teamInfo[i], 0);
                if (heroId != 0)
                {
                    heroes.Add(GetHeroDataById(heroId, LTFormationDataManager.Instance.GetIndex(heroId, isAlienMaze)));
                }
            }
            LTInstanceMapModel.Instance.RequestChallengeGetHeroHp(heroes, delegate
            {
                if (update) UpdateProgress();
            }, isAlienMaze ? LTInstanceConfig.AlienMazeTypeStr : null);
        }

        public void UpdateHp(HPEventType type,bool HasHPInfoData= false, object param = null)
        {
            float percent = 0;
            if (param != null)
            {
                if (type == HPEventType.REMOVE)
                {
                    PlayHPAni();
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHpCtrl_1315"), param));
                    percent = -float.Parse(param.ToString());
                }
                else if (type == HPEventType.BOOM)
                {
                    PlayHPAni();
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHpCtrl_1544"), param));
                    percent = -float.Parse(param.ToString());
                }
                else if (type == HPEventType.ADD)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHpCtrl_1742"), param));
                    percent = float.Parse(param.ToString());
                }
                else if (type == HPEventType.WENYI)
                {
                    percent = -float.Parse(param.ToString());
    
                }
                else if (type == HPEventType.SHENGJI)
                {
                    percent = float.Parse(param.ToString());
                }
            }
    
            UpdateHpData(percent, HasHPInfoData);
        }
    
        public bool mIsDeath = false;
    
        private void UpdateHpData(float percent, bool HasHPInfoData)
        {
            Hashtable curHpInfo;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(curHpInfoPath, out curHpInfo);
    
            if (curHpInfo == null)
            {
                return;
            }
    
            List<int> dieHero = new List<int>();
            foreach (DictionaryEntry pair in curHpInfo)
            {
                int heroId = EB.Dot.Integer("id", pair.Value, 0);
                if (heroId != 0)
                {
                    float curHp = 0;
                    DataLookupsCache.Instance.SearchDataByID<float>(string.Format("{0}.{1}.HP", curHpInfoPath, heroId), out curHp);
    
                    float maxHp = 0;
                    DataLookupsCache.Instance.SearchDataByID<float>(string.Format("{0}.{1}.MHP", curHpInfoPath, heroId), out maxHp);
    
                    if (!HasHPInfoData)
                        {
                        if (curHp > 0)
                        {
                            curHp = Math.Min(curHp + percent / 100f * maxHp, maxHp);
                        }
                        DataLookupsCache.Instance.CacheData(string.Format("{0}.{1}.HP", curHpInfoPath, heroId), curHp);
                    }
    
                    if ((int)curHp <= 0)
                    {
                        if (!dieHero.Contains(heroId))
                        {
                            dieHero.Add(heroId);
                        }
                    }
                }
            }
    
            int curHpSum = 0;
            int maxHpSum = 0;
            GetHpSum(out curHpSum, out maxHpSum);
    
            UpdateProgress(curHpSum, maxHpSum);
    
            if (curHpSum <= 0 && !mIsDeath && percent != 0)//触发炸弹死亡时
            {
                LTInstanceMapModel.Instance.RequestChallengeDeath(delegate
                {
                    mIsDeath = true;
                    Hotfix_LT.Messenger.Raise(EventName.OnChallengeFinished);
                    //if (LTInstanceMapModel.Instance.EventHandle.OnChallengeFinished != null)
                    //{
                    //    mIsDeath = true;
                    //    LTInstanceMapModel.Instance.EventHandle.OnChallengeFinished();
                    //}
                });
            }
            else if (curHpSum > 0)//当伙伴阵亡且未团灭时
            {
                for (int i = 0; i < dieHero.Count; i++)
                {
                    if (IsHeroInFormation(dieHero[i].ToString()))
                    {
                        LTFormationDataManager.Instance.RequestRemoveHeroFormation(dieHero[i], (isAlienMaze)?"lt_aw_camp":"lt_challenge_camp", delegate
                        {
                            UpdateProgress();
                        });
                    }
                }
                //if (FormationUtil.GetGlobalHPLoss()>=100)//当全局血量到达100%需要单独更新阵上伙伴血量
                //{
                //    RequestHerosHP(false);
                //}
            }
        }
    
        private bool IsHeroInFormation(string heroId)
        {
            ArrayList teamInfo = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>(teamInfoPath, out teamInfo);
            for (int i = 0; i < teamInfo.Count; i++)
            {
                int id = EB.Dot.Integer("hero_id", teamInfo[i], 0);
                if (id.ToString() == heroId && id > 0)
                {
                    return true;
                }
            }
            return false;
        }
    
        public void RestDeathState()
        {
            mIsDeath = false;
    
            ArrayList teamInfo = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>(teamInfoPath, out teamInfo);
            ArrayList heroes = Johny.ArrayListPool.Claim();
            for (int i = 0; i < teamInfo.Count; i++)
            {
                int heroId = EB.Dot.Integer("hero_id", teamInfo[i], 0);
                 if (heroId != 0)
                {
                    heroes.Add(GetHeroDataById(heroId, LTFormationDataManager.Instance.GetIndex(heroId, isAlienMaze)));
                }
            }
            LTInstanceMapModel.Instance.RequestChallengeGetHeroHp(heroes, delegate
            {
                UpdateProgress();
            },isAlienMaze ? LTInstanceConfig.AlienMazeTypeStr : null);
        }
    
        public void DeathState()
        {
            Hashtable curHpInfo;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(curHpInfoPath, out curHpInfo);
            if (curHpInfo == null)
            {
                return;
            }
            DataLookupsCache.Instance.CacheData("combat.heroHp",Johny.ArrayListPool.Claim());
            foreach (DictionaryEntry pair in curHpInfo)
            {
                int heroId = EB.Dot.Integer("id", pair.Value, 0);
                if (heroId != 0)
                {
                    DataLookupsCache.Instance .CacheData(string.Format("{0}.{1}.HP", curHpInfoPath, heroId),0);
                }
            }
        }
    
        private void PlayHPAni()
        {
            hpTween.SetOnFinished(OnHPAniEnd);
            hpTween.ResetToBeginning();
            hpTween.PlayForward();
            HpLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor;
        }
    
        private void OnHPAniEnd()
        {
            HpLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
        }
    
        private void UpdateProgress()
        {
            int curHpSum = 0;
            int maxHpSum = 0;
            GetHpSum(out curHpSum, out maxHpSum);
    
            UpdateProgress(curHpSum, maxHpSum);
        }
    
        private void UpdateProgress(int allHp, int maxHp)
        {
            float percent = (float)allHp / (float)maxHp;
            percent = Mathf.Min(percent, 1);
            HpSlider.value = percent;
            if (HpSlider.value >= 0.5f)
            {
                HPForeground.color = Color.green;
            }
            else if(HpSlider.value >= 0.2f)
            {
                HPForeground.color = Color.yellow;
            }
            else
            {
                HPForeground.color = Color.red;
            }
            LTUIUtil.SetText(HpLabel, (percent.ToString("0%")));
        }
    
        private static void GetHpSum(out int curHpSum, out int maxHpSum)
        {
            curHpSum = 0;
            maxHpSum = 0;
    
            ArrayList teamInfo = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>(teamInfoPath, out teamInfo);
    
            if (teamInfo != null)
            {
                for (int i = 0; i < teamInfo.Count; i++)
                {
                    var obj = teamInfo[i];
                    if(obj is Hashtable
                       && (obj as Hashtable).Count == 0){
                        continue;
                    }

                    int heroId = EB.Dot.Integer("hero_id", obj, 0);
                    if (heroId != 0)
                    {
                        float maxHp = 0;
                        DataLookupsCache.Instance.SearchDataByID<float>($"{curHpInfoPath}.{heroId.ToString()}.MHP", out maxHp);
                        float curHp = 0;
                        DataLookupsCache.Instance.SearchDataByID<float>($"{curHpInfoPath}.{heroId.ToString()}.HP", out curHp);
                        maxHpSum += (int)maxHp;
                        curHpSum +=Mathf .Max((int)curHp,0);
                    }
                }
            }
        }
    
        /// <summary>
        /// 清空血量
        /// </summary>
        public static void RestHpSum()
        {
            DataLookupsCache.Instance.CacheData(curHpInfoPath, null);
        }
    
        public static float GetCurHpSum()
        {
            int curHpSum = 0;
            int maxHpSum = 0;
            GetHpSum(out curHpSum, out maxHpSum);
            return curHpSum;
        }
    
        public static void UpdateInstanceHpFromCombat(bool isfast = false)
        {
            ArrayList combatHpList;
            if (DataLookupsCache.Instance.SearchDataByID("combat.heroHp", out combatHpList))
            {
                for (int i = 0; i < combatHpList.Count; i++)
                {
                    var data = combatHpList[i];
                    float combatHp = (float)EB.Dot.Double("hp", data, 0);
                    int heroId = 0;
                    if (isfast)
                    {
                        heroId = EB.Dot.Integer("hero_id", data, 0);
                    }
                    else
                    {
                        heroId = EB.Dot.Integer("id", data, 0);
                    }
    
                    float instanceHp = 0;
                    if (DataLookupsCache.Instance.SearchDataByID(string.Format("{0}.{1}.HP", curHpInfoPath, heroId), out instanceHp))
                    {
                        DataLookupsCache.Instance.CacheData(string.Format("{0}.{1}.HP", curHpInfoPath, heroId), combatHp);
                    }
                }
            }
        }
    
        public static float GetCombatHPPercent(string heroId)
        {
            ArrayList combatHpList;
            if (DataLookupsCache.Instance.SearchDataByID("combat.heroHp", out combatHpList))
            {
                for (int i = 0; i < combatHpList.Count; i++)
                {
                    var data = combatHpList[i];
                    if (heroId == EB.Dot.Integer("id", data, 0).ToString())
                    {
                        return (float)EB.Dot.Double("hpp", data, 0);
                    }
                }
            }
            return 0;
        }
    
        public static float GetCombatHp(string heroId)
        {
            ArrayList combatHpList;
            if (DataLookupsCache.Instance.SearchDataByID("combat.heroHp", out combatHpList))
            {
                for (int i = 0; i < combatHpList.Count; i++)
                {
                    var data = combatHpList[i];
                    if (heroId == EB.Dot.Integer("id", data, 0).ToString())
                    {
                        return (float)EB.Dot.Double("hp", data, 0);
                    }
                }
            }
            return 0;
        }
    }
}
