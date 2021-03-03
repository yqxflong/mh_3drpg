using UnityEngine;

namespace Hotfix_LT.UI
{
    public class CamapignVictoryHpDataSet : DynamicMonoHotfix
    {
        public UILabel LevelLabel;
        public UIProgressBar HpProgress;
        public UILabel PercentLabel;
        public UILabel DeathLabel;

        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    LevelLabel = ((GameObject)mDMono.ObjectParamList[0]).GetComponentEx<UILabel>();
                }
                if (count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    HpProgress = ((GameObject)mDMono.ObjectParamList[1]).GetComponentEx<UIProgressBar>();
                }
                if (count > 2 && mDMono.ObjectParamList[2] != null)
                {
                    PercentLabel = ((GameObject)mDMono.ObjectParamList[2]).GetComponentEx<UILabel>();
                }
                if (count > 3 && mDMono.ObjectParamList[3] != null)
                {
                    DeathLabel = ((GameObject)mDMono.ObjectParamList[3]).GetComponentEx<UILabel>();
                }
            }
        }
    
        public void SetHp(string heroId,bool isShowTempHp)
        {
            //float fullHp = 0;
            int level = 0;
            int peak = 0;
            float percent = 0;
            if (int.Parse(heroId) < 0)//是否雇佣
            {
                var hireInfo = LTInstanceHireUtil.GetHireInfoByHeroId(int.Parse(heroId));
                if (hireInfo != null)
                {
                    float fullHp = hireInfo.base_MaxHP;
                    float curHp = 0;
                    if (isShowTempHp)
                    {
                        curHp = LTHotfixManager.GetManager<SceneManager>().GetChallengeTempHp(heroId);
                    }
                    else
                    {
                        curHp = LTChallengeInstanceHpCtrl.GetCombatHp(heroId);
                    }
                    percent = curHp / fullHp;
                    level = hireInfo.level;
                }
            }
            else
            {
                if (isShowTempHp)
                {
                    float curHp = LTHotfixManager.GetManager<SceneManager>().GetChallengeTempHp(heroId);
                    float fullHp = 0;
                    DataLookupsCache.Instance.SearchDataByID<float>(string.Format("userCampaignStatus.challengeChapters.team.{0}.MHP", heroId), out fullHp);
                    percent = curHp / fullHp;
                }
                else
                {
                    percent = LTChallengeInstanceHpCtrl.GetCombatHPPercent(heroId);
                }
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.level", heroId), out level);
                
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.proficiency.1.1", heroId), out peak);
            }
    
            percent = Mathf.Min(percent, 1);
    
            PercentLabel.text = percent.ToString("0%");
            HpProgress.value = percent;
    
            DeathLabel.gameObject.SetActive(percent <= 0);
            PercentLabel.gameObject.SetActive(percent > 0);
    
            LTUIUtil.SetLevelText(LevelLabel.transform.parent.GetComponent<UISprite>(),LevelLabel,level+peak);

        }
    }
}
