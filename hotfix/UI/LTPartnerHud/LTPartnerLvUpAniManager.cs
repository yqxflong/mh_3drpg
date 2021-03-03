using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPartnerLvUpAniData
    {
        public LTPartnerCultivateController culCon;
        public int oldLv;
        public int oldExp;
        public int curLv;
        public int curExp;
        public LTAttributesData oldAttr;
        public SliderAni sliderAni;
        public NumAni numAni;
        public LTPartnerData parData;

        public LTPartnerLvUpAniData()
        { }

        public LTPartnerLvUpAniData(LTPartnerCultivateController culCon, SliderAni sliderAni, NumAni numAni)
        {
            this.culCon = culCon;
            this.sliderAni = sliderAni;
            this.numAni = numAni;
        }
    }

    public class LTPartnerLvUpAniManager
    {
        private static LTPartnerLvUpAniManager instance;

        private LTPartnerLvUpAniData curAniData;
        private bool isLvUp = false;
        private int LvUpCount;
        private int aniPlayCount;
        private float aniPlayUseTime;

        private bool isBegin = false;

        public static LTPartnerLvUpAniManager Instance
        {
            get { return instance = instance ?? new LTPartnerLvUpAniManager(); }
        }

        public void SetAniData(LTPartnerLvUpAniData data)
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
            Hotfix_LT.Data.HeroLevelInfoTemplate levelTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroLevelInfo(lv);
            if (levelTpl != null)
            {
                goalExp = goalExp == -1 ? levelTpl.buddy_exp : goalExp;
                float initValue = (float)initExp / levelTpl.buddy_exp;
                float goalValue = (float)goalExp / levelTpl.buddy_exp;
                PlaySliderAni(initValue, goalValue, (goalValue - initValue) * aniPlayUseTime);
                PlayNumAni(initExp, goalExp, levelTpl.buddy_exp, (goalValue - initValue) * aniPlayUseTime);
            }
        }

        private void SliderAniTimer(float playTime)
        {
            FusionAudio.PostEvent("UI/Partner/ExpSlider", false);
        }

        private int mTimer = -1;
        private void PlaySliderAni(float initValue, float goalValue, float playTime)
        {
            FusionAudio.PostEvent("UI/Partner/ExpSlider", true);
            curAniData.sliderAni.SetAniData(initValue, goalValue, playTime);
        }

        private void PlayNumAni(int initExp, int goalExp, int maxExp, float playTime)
        {
            curAniData.numAni.SetAniData(initExp, goalExp, maxExp, playTime);
        }

        private void Refresh()
        {
            LTAttributesData oldLevelAttr = AttributesManager.GetPartnerAttributesByParnterData(curAniData.parData, curAniData.oldLv + aniPlayCount - 1);
            LTAttributesData addLevelAttr = AttributesManager.GetPartnerAttributesByParnterData(curAniData.parData,  curAniData.oldLv + aniPlayCount);
            addLevelAttr.Sub(oldLevelAttr);
            LTAttributesData oldAttrData = new LTAttributesData(curAniData.oldAttr);
            curAniData.oldAttr.Add(addLevelAttr);
            curAniData.culCon.PlayAttrAni(oldAttrData, curAniData.oldAttr, curAniData.oldLv + aniPlayCount);
        }

        private bool AddListener;
        private void OnBegin()
        {
            isBegin = true;
            curAniData.culCon.IsPlayingAni = true;
            if (AddListener==false)
            {
                Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerRollAniSucc, OnPartnerRollAniSuccFunc);
                Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerRollAniBreak, OnPartnerRollAniBreakFunc);
                AddListener = true;
            }
        }

        private void OnEnd()
        {
            isBegin = false;
            FusionAudio.PostEvent("UI/Partner/ExpSlider", false);
            curAniData.culCon.IsPlayingAni = false;
            if (AddListener==true)
            {
                Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerRollAniSucc, OnPartnerRollAniSuccFunc);
                Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerRollAniBreak, OnPartnerRollAniBreakFunc);
                AddListener = false;
            }
        }

        public void StopAni()
        {
            if (curAniData == null) return;

            curAniData.sliderAni.StopAni();
            FusionAudio.PostEvent("UI/Partner/ExpSlider", false);
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
                    else
                    {
                        aniPlayUseTime = 0;
                        PlayAllAni(curAniData.curLv, 0, 0);
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
