using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class CamapignVictoryExpDataSet : DynamicMonoHotfix
    {
        public UILabel m_Level;
        public UILabel m_Exp_Label;
        public UIProgressBar m_Exp_Progress;
        public UILabel m_Exp_Add_Label;
        public string RewardsPath;
        public TweenScale TweenScale;
        public GameObject LevelUpFx;
        public GameObject LevelUpBarFx;
        public GameObject ThumbFx;

        float LerpHpSpeed = 0.3f;

        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null)
            {
                var count = mDMono.ObjectParamList.Count;

                if (count > 0 && mDMono.ObjectParamList[0] != null)
                {
                    m_Level = ((GameObject)mDMono.ObjectParamList[0]).GetComponentEx<UILabel>();
                }
                if (count > 1 && mDMono.ObjectParamList[1] != null)
                {
                    m_Exp_Label = ((GameObject)mDMono.ObjectParamList[1]).GetComponentEx<UILabel>();
                }
                if (count > 2 && mDMono.ObjectParamList[2] != null)
                {
                    m_Exp_Progress = ((GameObject)mDMono.ObjectParamList[2]).GetComponentEx<UIProgressBar>();
                }
                if (count > 3 && mDMono.ObjectParamList[3] != null)
                {
                    m_Exp_Add_Label = ((GameObject)mDMono.ObjectParamList[3]).GetComponentEx<UILabel>();
                }
                if (count > 4 && mDMono.ObjectParamList[4] != null)
                {
                    TweenScale = ((GameObject)mDMono.ObjectParamList[4]).GetComponentEx<TweenScale>();
                }
                if (count > 5 && mDMono.ObjectParamList[5] != null)
                {
                    LevelUpFx = ((GameObject)mDMono.ObjectParamList[5]);
                }
                if (count > 6 && mDMono.ObjectParamList[6] != null)
                {
                    LevelUpBarFx = ((GameObject)mDMono.ObjectParamList[6]);
                }
                if (count > 7 && mDMono.ObjectParamList[7] != null)
                {
                    ThumbFx = ((GameObject)mDMono.ObjectParamList[7]);
                }
            }

            if (mDMono.StringParamList != null)
            {
                var count = mDMono.StringParamList.Count;

                if (count > 0)
                {
                    RewardsPath = mDMono.StringParamList[0];
                }
            }
        }
    
    	public void SetExp(string heroid)
        {
            if (int.Parse(heroid) < 0)
            {
                var hireInfo = LTInstanceHireUtil.GetHireInfoByHeroId(int.Parse(heroid));
                if (hireInfo != null)
                {
                    m_Level.text = hireInfo.level.ToString();
                    m_Exp_Progress.gameObject.CustomSetActive(false);
                    m_Exp_Add_Label.gameObject.CustomSetActive(false);
                }
                return;
            }
            IDictionary expData;
            int addExp = 0;
    		if (DataLookupsCache.Instance.SearchDataByID<IDictionary>(RewardsPath, out expData))
    		{
                addExp = EB.Dot.Integer("quantity", expData, 0);
    			m_Exp_Add_Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_CamapignVictoryExpDataSet_743"), addExp);
    		}
    		else
    		{
    			m_Exp_Add_Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_CamapignVictoryExpDataSet_743"), 0);
    		}
    
    		int endLevel = 0;
    		int endExpSum = 0;
    		DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.level", heroid), out endLevel);
    		DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.xp", heroid), out endExpSum);
            int endLevelExp = endExpSum - Hotfix_LT.Data.CharacterTemplateManager.Instance.GerHeroPastExpSum(endLevel);
            float endPercent = (float)endLevelExp / (float)Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroLevelInfo(endLevel).buddy_exp;
    
            var partnerData = LTPartnerDataManager.Instance.GetPartnerByHeroId(int.Parse(heroid));
            if (partnerData != null)
            {
                Hotfix_LT.Data.UpGradeInfoTemplate curTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(partnerData.UpGradeId, partnerData.HeroInfo.char_type);
                if (partnerData.Level >= curTpl.level_limit)
                {
                    LTUIUtil.SetLevelText(m_Level.transform.parent.GetComponent<UISprite>(),m_Level,partnerData);
                    m_Exp_Progress.value = endPercent;
                    return;
                }
            }
    
            int startExpSum = endExpSum - addExp;
            int startLevel = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroLevelByExp(startExpSum);
            int startLevelExp = startExpSum - Hotfix_LT.Data.CharacterTemplateManager.Instance.GerHeroPastExpSum(startLevel);
            float startPercent = (float)startLevelExp / (float)Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroLevelInfo(startLevel).buddy_exp;
    
            EB.Coroutines.Run(PlayExpProgressAni(startPercent, endPercent, endLevel - startLevel, startLevel));
    
            if (m_Exp_Label != null)
            {
                LTUIUtil.SetText(m_Exp_Label, endLevelExp + "/" + Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroLevelInfo(endLevel).buddy_exp);
            }
        }
    
        /*public float TestStartPercnt = 0;
    
        public float TestEndPercnt = 0.5f;
    
        public int TestAddLevel = 5;
    
        public int TestStartLevel = 1;
    
        private void OnGUI()
        {
            if (GUILayout.Button("Set", GUILayout.Height(32.0f), GUILayout.Width(130.0f)))
            {
                LTUIUtil.SetText(m_Level, TestStartLevel.ToString());
                m_Exp_Progress.value = TestStartPercnt;
                StartCoroutine(PlayExpProgressAni(TestStartPercnt, TestEndPercnt, TestAddLevel, TestStartLevel));
            }
        }*/
    

        private IEnumerator PlayExpProgressAni(float startPercent, float endPercent, int addLevel, int startLevel)
        {
            LTUIUtil.SetText(m_Level, startLevel.ToString());
            m_Exp_Progress.value = startPercent;
            if (addLevel > 0)
            {
                int tempAddLevel = addLevel;
                while (tempAddLevel >= 0)
                {
                    if (tempAddLevel == addLevel)
                    {
                        yield return LerpExp(startPercent, 1);
                        tempAddLevel = tempAddLevel - 1;
                    }
                    else if (tempAddLevel == 0)
                    {
                        yield return LerpExp(0, endPercent);
                        break;
                    }
                    else
                    {
                        yield return LerpExp(0, 1);
                        tempAddLevel = tempAddLevel - 1;
                    }
                    LTUIUtil.SetText(m_Level, (startLevel + addLevel - tempAddLevel).ToString());
    
                    if (TweenScale != null)
                    {
                        TweenScale.ResetToBeginning();
                        TweenScale.PlayForward();
                    }
                    if (LevelUpFx != null)
                    {
                        LevelUpFx.SetActive(true);
                    }
                    if (LevelUpBarFx != null)
                    {
                        LevelUpBarFx.SetActive(true);
                    }
                }
            }
            else
            {
                yield return LerpExp(startPercent, endPercent);
                LTUIUtil.SetText(m_Level, startLevel.ToString());
            }
            //m_Exp_Progress.value = endPercent;
        }
    
        public void OnTSFinished()
        {
            LevelUpFx.SetActive(false);
            LevelUpBarFx.SetActive(false);
        }
    
        private IEnumerator LerpExp(float start, float end)
        {
            if (ThumbFx != null)
            {
                ThumbFx.CustomSetActive(true);
            }
            m_Exp_Progress.value = start;
            float time = 0;
            float totalTime = (end - start) * 3f;
            while (time < totalTime)
            {
                float temp = Mathf.Lerp(start, end, time / totalTime);
                m_Exp_Progress.value = temp;
                time += Time.deltaTime;
                yield return null;
            }
            m_Exp_Progress.value = end;
            if (ThumbFx != null)
            {
                ThumbFx.CustomSetActive(false);
            }
        }
    }
}
