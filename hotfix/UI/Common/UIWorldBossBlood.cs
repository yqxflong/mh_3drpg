using System;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class UIWorldBossBlood : DataLookupHotfix
    {
        public UISprite BossIcon;
        public UILabel NameLabel;
        public UILabel HpBarNumLabel;
        public UILabel HpLabel;
        public UIProgressBar ProcessBar;
        public UIProgressBar ProcessBGBar;
        public UIProgressBar NewProcessBar;
        public UISprite CurHpSp;
        public UISprite NextHpSp;
        public UIBackTimeComponent CountDownTime;
    
        public bool isOpenDebugWorldBoss;//unity编辑器下才会使用的字段
        
        private int HpBarMaxNum = 10;
        private long m_Max = 0;
        private long m_Left = 0;
        private long m_Offset = 0;
        //0蓝，1红，2橙
        private Color[] HipeToColor = new Color[3] { new Color32(0, 126, 227, 255), new Color32(233, 25, 56, 255), new Color32(255, 133, 0, 255) };

        public override void Awake()
        {
            base.Awake();

            if (mDL.BoolParamList != null)
            {
                var count = mDL.BoolParamList.Count;

                if (count > 0)
                {
                    isOpenDebugWorldBoss = mDL.BoolParamList[0];
                }
            }

            if (mDL.ObjectParamList != null)
            {
                var count = mDL.ObjectParamList.Count;

                if (count > 0 && mDL.ObjectParamList[0] != null)
                {
                    BossIcon = ((GameObject)mDL.ObjectParamList[0]).GetComponentEx<UISprite>();
                }
                if (count > 1 && mDL.ObjectParamList[1] != null)
                {
                    NameLabel = ((GameObject)mDL.ObjectParamList[1]).GetComponentEx<UILabel>();
                }
                if (count > 2 && mDL.ObjectParamList[2] != null)
                {
                    HpBarNumLabel = ((GameObject)mDL.ObjectParamList[2]).GetComponentEx<UILabel>();
                }
                if (count > 3 && mDL.ObjectParamList[3] != null)
                {
                    HpLabel = ((GameObject)mDL.ObjectParamList[3]).GetComponentEx<UILabel>();
                }
                if (count > 4 && mDL.ObjectParamList[4] != null)
                {
                    ProcessBar = ((GameObject)mDL.ObjectParamList[4]).GetComponentEx<UISlider>();
                }
                if (count > 5 && mDL.ObjectParamList[5] != null)
                {
                    ProcessBGBar = ((GameObject)mDL.ObjectParamList[5]).GetComponentEx<UISlider>();
                }
                if (count > 6 && mDL.ObjectParamList[6] != null)
                {
                    NewProcessBar = ((GameObject)mDL.ObjectParamList[6]).GetComponentEx<UIProgressBar>();
                }
                if (count > 7 && mDL.ObjectParamList[7] != null)
                {
                    CurHpSp = ((GameObject)mDL.ObjectParamList[7]).GetComponentEx<UISprite>();
                }
                if (count > 8 && mDL.ObjectParamList[8] != null)
                {
                    NextHpSp = ((GameObject)mDL.ObjectParamList[8]).GetComponentEx<UISprite>();
                }
                if (count > 9 && mDL.ObjectParamList[9] != null)
                {
                    CountDownTime = ((GameObject)mDL.ObjectParamList[9]).GetMonoILRComponent<UIBackTimeComponent>();
                }
            }
        }

        public override void OnEnable()
        {
            int monsterID = LTWorldBossDataManager.Instance.GetWorldBossMonsterID();
            if (monsterID == 0)
            {
                return;
            }
            Hotfix_LT.Data.MonsterInfoTemplate monsterTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(monsterID);
            if (monsterTpl == null)
            {
                EB.Debug.LogError("LTWorldBossHudController InitBoss, monsterTpl is Error monsterID = {0}", monsterID);
                return;
            }
    
            Hotfix_LT.Data.HeroInfoTemplate info = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(monsterTpl.character_id);
            if (info == null)
            {
                EB.Debug.LogError("LTWorldBossHudController InitBoss, info is Error monsterTpl.character_id = {0}", monsterTpl.character_id);
                return;
            }
    
            HpBarMaxNum = monsterTpl.hp_number;
            if(BossIcon !=null)BossIcon.spriteName = info.icon;
            NameLabel.text = info.name;
    
            if(CountDownTime!=null) CountDownTime.EndTime = LTWorldBossDataManager.Instance.GetBossEndTime();
    
            object obj = null;
            DataLookupsCache.Instance.SearchDataByID("world_boss.blood", out obj);
            if (obj != null)
            {
                OnLookupUpdate("world_boss", obj);
            }
    
            if (isOpenDebugWorldBoss)
            {
                LTWorldBossDataManager.Instance.IsOpenDebugWorldBoss = true;
            }
        }
    
        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);
            if (dataID != null && value != null)
            {
                m_Max = EB.Dot.Long("m", value, m_Max);
                m_Left = EB.Dot.Long("l", value, m_Left);
                m_Offset = EB.Dot.Long("o", value, m_Offset);
                if (m_Max == 0 || m_Left < 0 || m_Left > m_Max)
                {
                    EB.Debug.LogWarning(string.Format("worldboss blood data is illegale max={0} left={1}", m_Max, m_Left));
                    return;
                }

                long bloodNum = (long)Math.Ceiling(((float)m_Left) / m_Max * HpBarMaxNum);
                if(HpBarNumLabel!=null) HpBarNumLabel.text = HpBarNumLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("x{0}", bloodNum);
                if(ProcessBar!=null&& ProcessBGBar!=null) ProcessBar.value = ProcessBGBar.value = GetBossBloodBarValue(m_Left, m_Max);
                if (NewProcessBar != null) NewProcessBar.value = (float)m_Left / (float)m_Max;
                HpLabel.text = HpLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}/{1}({2}%)", m_Left.ToString("F1"), m_Max.ToString("F1"), (((float)m_Left) / (float)m_Max * 100).ToString("F1"));
                if (CurHpSp !=null)SetBossBloodBar(bloodNum);
            }
        }
    
        private float GetBossBloodBarValue(float curHP, float maxHp)
        {
            float v = (float)curHP / maxHp;
            float value = v * HpBarMaxNum - (v * HpBarMaxNum);
            if (value <= 0)
            {
                return 1;
            }
    
            return value;
        }
    
        private void SetBossBloodBar(long bloodNum)
        {
            CurHpSp.color = HipeToColor[bloodNum % 3];
            if (bloodNum <= 1)
            {
                NextHpSp.gameObject.CustomSetActive(false);
            }
            else
            {
                NextHpSp.gameObject.CustomSetActive(true);
                NextHpSp.color = HipeToColor[(bloodNum - 1) % 3];
            }
        }
    }
}
