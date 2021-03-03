namespace Hotfix_LT.UI
{
    public class LTPartnerSkillBreakAniData
    {
        public LTPartnerSkillBreakController skillBreakCon;
        public int oldLv;
        public int oldExp;
        public int curLv;
        public int curExp;
        public SliderAni sliderAni;
        public NumAni numAni;

        public LTPartnerSkillBreakAniData()
        { }

        public LTPartnerSkillBreakAniData(LTPartnerSkillBreakController skillBreakCon, SliderAni sliderAni, NumAni numAni)
        {
            this.skillBreakCon = skillBreakCon;
            this.sliderAni = sliderAni;
            this.numAni = numAni;
        }
    }


    public class LTPartnerSkillBreakAniManager
    {
        private static LTPartnerSkillBreakAniManager instance;

        private LTPartnerSkillBreakAniData curAniData;
        private bool isLvUp = false;
        private int LvUpCount;
        private int aniPlayCount;
        private float aniPlayUseTime;

        private bool isBegin = false;

        public static LTPartnerSkillBreakAniManager Instance
        {
            get { return instance = instance ?? new LTPartnerSkillBreakAniManager(); }
        }

        public void SetAniData(LTPartnerSkillBreakAniData data)
        {
            curAniData = data;
            PlayAni();
        }

        private void PlayAni()
        {
            OnBegin();
            if (curAniData.oldLv == curAniData.curLv)
            {
                isLvUp = false;
                aniPlayUseTime = 0.8f;
                PlayAllAni(curAniData.curLv, curAniData.oldExp, curAniData.curExp);
            }
            else
            {
                isLvUp = true;
                LvUpCount = curAniData.curLv - curAniData.oldLv;
                aniPlayUseTime = LvUpCount <= 1 && curAniData.curExp <= 0 ? 0.8f : 0.5f;
                aniPlayCount = 0;
                PlayAllAni(curAniData.oldLv, curAniData.oldExp);
            }
        }

        private void PlayAllAni(int lv, int initExp, int goalExp = -1)
        {
            int maxExp = LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[lv];
            goalExp = goalExp == -1 ? maxExp : goalExp;
            float initValue = (float)initExp / maxExp;
            float goalValue = (float)goalExp / maxExp;
            PlaySliderAni(initValue, goalValue, (goalValue - initValue) * aniPlayUseTime);
            PlayNumAni(initExp, goalExp, maxExp, (goalValue - initValue) * aniPlayUseTime);
        }

        private int mTimer = -1;
        private void PlaySliderAni(float initValue, float goalValue, float playTime)
        {
            FusionAudio.PostEvent("UI/Partner/ExpSlider", true);
            curAniData.sliderAni.SetAniData(initValue, goalValue, playTime);
            mTimer = ILRTimerManager.instance.AddTimer((int)(playTime * 1000), 1, delegate (int seq)
            {
                ILRTimerManager.instance.RemoveTimer(mTimer);
                FusionAudio.PostEvent("UI/Partner/ExpSlider", false);
            });
        }

        private void PlayNumAni(int initExp, int goalExp, int maxExp, float playTime)
        {
            //FusionAudio.PostEvent("UI/Partner/SkillBreakUp", true);
            curAniData.numAni.SetAniData(initExp, goalExp, maxExp, playTime);
        }

        private void Refresh()
        {
            curAniData.skillBreakCon.RefreshSkillLabel(curAniData.oldLv + aniPlayCount);
        }

        private void OnBegin()
        {
            isBegin = true;
            curAniData.skillBreakCon.IsPlayingAni = true;
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerRollAniSucc, OnPartnerRollAniSuccFunc);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerRollAniBreak, OnPartnerRollAniBreakFunc);
        }

        private void OnEnd()
        {
            isBegin = false;
            curAniData.skillBreakCon.IsPlayingAni = false;
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerRollAniSucc, OnPartnerRollAniSuccFunc);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerRollAniBreak, OnPartnerRollAniBreakFunc);
        }

        public void StopAni()
        {
            if (curAniData == null) return;

            curAniData.sliderAni.StopAni();
            curAniData.numAni.StopAni();
            OnEnd();
        }

        private void OnPartnerRollAniSuccFunc()
        {
            if (isLvUp)
            {
                aniPlayCount++;
                if (aniPlayCount == LvUpCount)
                {
                    Refresh();
                    if (curAniData.curExp > 0)
                    {
                        PlayAllAni(curAniData.curLv, 0, curAniData.curExp);
                    }
                    else if (curAniData.curLv < LTPartnerConfig.MAX_SKILL_LEVEL)
                    {
                        aniPlayUseTime = 0;
                        PlayAllAni(curAniData.curLv, 0, 0);
                    }

                    if (curAniData.curLv >= LTPartnerConfig.MAX_SKILL_LEVEL)
                    {
                        curAniData.skillBreakCon.SetStateMax();
                        OnEnd();
                    }
                }
                else if (aniPlayCount < LvUpCount)
                {
                    Refresh();
                    PlayAllAni(curAniData.oldLv + aniPlayCount, 0);
                }
                else
                {
                    OnEnd();
                }
            }
            else
            {
                OnEnd();
            }
        }

        private void OnPartnerRollAniBreakFunc()
        {
            if (!isBegin)
            {
                return;
            }
            OnEnd();
        }

    }

}