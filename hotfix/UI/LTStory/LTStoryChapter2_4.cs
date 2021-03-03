using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChapter2_4 : LTStoryChapter
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            panel = t.GetComponent<UIPanel>("PanelChapter");
            chapterAlphaAni = t.GetComponentEx<TweenAlpha>();
            chapterPosAni = t.GetComponentEx<TweenPosition>();
            chapterScaleAni = t.GetComponentEx<TweenScale>();

            chapterPos = new Vector3[2];
            chapterPos[0] = new Vector3(3019.0f, -513.0f, 0.0f);
            chapterPos[1] = new Vector3(0.0f, 0.0f, 0.0f);

            chapterScale = new float[2];
            chapterScale[0] = 3.6f;
            chapterScale[1] = 1;

            chapterDuration = new float[2];
            chapterDuration[0] = 0;
            chapterDuration[1] = 9;

            blackMaskDuration = new float[2];
            blackMaskDuration[0] = 1;
            blackMaskDuration[1] = 1.5f;

            playTime = new float[3];
            playTime[0] = 1;
            playTime[1] = 11.5f;
            playTime[2] = 13.5f;

            storyPlayShowTime = new float[4];
            storyPlayShowTime[0] = 1;
            storyPlayShowTime[1] = 4.5f;
            storyPlayShowTime[2] = 9;
            storyPlayShowTime[3] = 12.5f;

            storyShowDuration = 1f;
            storyEndDuration = 1f;
            storyMaxShowTime = 3f;

            Environment2PosAni = t.GetComponent<TweenPosition>("PanelChapter/BG/BG2");
            HumanPosAni = t.GetComponent<TweenPosition>("PanelChapter/Human");
            HumanScaleAni = t.GetComponent<TweenScale>("PanelChapter/Human");

            HumanPos = new Vector3[6];
            HumanPos[0] = new Vector3(220.0f, 0.0f, 0.0f);
            HumanPos[1] = new Vector3(55.0f, 0.0f, 0.0f);
            HumanPos[2] = new Vector3(-40.0f, 0.0f, 0.0f);
            HumanPos[3] = new Vector3(-180.0f, 100.0f, 0.0f);
            HumanPos[4] = new Vector3(-120.0f, 130.0f, 0.0f);
            HumanPos[5] = new Vector3(485.0f, 400.0f, 0.0f);

            HumanPosDuration = new float[6];
            HumanPosDuration[0] = 0;
            HumanPosDuration[1] = 7.5f;
            HumanPosDuration[2] = 1.3f;
            HumanPosDuration[3] = 0.7f;
            HumanPosDuration[4] = 0.3f;
            HumanPosDuration[5] = 2;

            HumanScale = new float[6];
            HumanScale[0] = 1;
            HumanScale[1] = 1;
            HumanScale[2] = 1.2f;
            HumanScale[3] = 1.6f;
            HumanScale[4] = 1.7f;
            HumanScale[5] = 2.2f;

            HumanScaleDuration = new float[6];
            HumanScaleDuration[0] = 0;
            HumanScaleDuration[1] = 7.5f;
            HumanScaleDuration[2] = 1.3f;
            HumanScaleDuration[3] = 0.7f;
            HumanScaleDuration[4] = 0.3f;
            HumanScaleDuration[5] = 2;
        }

        public TweenPosition Environment2PosAni;
        public TweenPosition HumanPosAni;
        public TweenScale HumanScaleAni;
    
        #region 动画参数
        public Vector3[] HumanPos;
        public float[] HumanPosDuration;
        public float[] HumanScale;
        public float[] HumanScaleDuration;
        #endregion
    
        protected override IEnumerator StoryAni()
        {
            PlayBgm();
    
            //0
            float time0 = GetStopTime(0);
            yield return new WaitForSeconds(time0);
    
            skipObj.CustomSetActive(true);
            PlayBlackMaskAlpha(false, blackMaskDuration[0]);
    
            PlayChapterTweenPos(1);
            PlayChapterTweenScale(1);
    
            PlayTween(Environment2PosAni);
    
            PlayTweenArray(HumanPosAni, HumanPos, HumanPosDuration);
            Vector3[] vc = new Vector3[HumanScale.Length];
            for (int i = 0; i < vc.Length; i++)
            {
                vc[i] = Vector3.one * HumanScale[i];
            }
            PlayTweenArray(HumanScaleAni, vc, HumanScaleDuration);
    
            //1
            float time1 = GetStopTime(1);
            yield return new WaitForSeconds(time1);
    
            PlayBlackMaskAlpha(true, blackMaskDuration[1]);
    
            //2
            float time2 = GetStopTime(2);
            yield return new WaitForSeconds(time2);

            Hotfix_LT.Messenger.Raise(EventName.OnStoryPlaySucc);
        }
    
        protected override void InitStoryTalk()
        {
            base.InitStoryTalk();
            if (isInitStoryTalkSucc)
            {
                return;
            }
    
            storyTalk = new string[]
            {
    
                //"正准备大开杀戒的阿拉里克大吃一惊",
                //"翅膀残缺的苏格拉底竟飞上半空",
                //"真不知道这个年轻人，还有着怎样的潜力",
                //"",
    
                EB.Localizer.GetString("ID_STORY_CHAPTER2_4_1"),
                EB.Localizer.GetString("ID_STORY_CHAPTER2_4_2"),
                EB.Localizer.GetString("ID_STORY_CHAPTER2_4_3"),
                "",//空字段，用于控制结束时间
            };
        }
    
    }
}
