using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChatper1 : LTStoryChapter
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            panel = t.GetComponent<UIPanel>("PanelChapter");
            chapterAlphaAni = t.GetComponentEx<TweenAlpha>();
            chapterPosAni = t.GetComponentEx<TweenPosition>();
            chapterScaleAni = t.GetComponentEx<TweenScale>();
            whiteMask = t.GetComponent<TweenAlpha>("PanelChapter/Panel/WhiteMask");

            chapterPos = new Vector3[3];
            chapterPos[0] = new Vector3(910.0f, -581.0f, 0.0f);
            chapterPos[1] = new Vector3(-563.0f, 293.0f, 0.0f);
            chapterPos[2] = new Vector3(-2565.0f, 718.0f, 0.0f);

            chapterScale = new float[3];
            chapterScale[0] = 1.95f;
            chapterScale[1] = 3.1f;
            chapterScale[2] = 7.8f;

            chapterDuration = new float[3];
            chapterDuration[0] = 4f;
            chapterDuration[1] = 3f;
            chapterDuration[2] = 4.5f;

            chapterAlphaDuration = new float[4];
            chapterAlphaDuration[0] = 1f;
            chapterAlphaDuration[1] = 1.5f;
            chapterAlphaDuration[2] = 0.8f;
            chapterAlphaDuration[3] = 1.8f;

            whiteMaskAlphaValue = new float[3];
            whiteMaskAlphaValue[0] = 0.8f;
            whiteMaskAlphaValue[1] = 0.5f;
            whiteMaskAlphaValue[2] = 0.8f;

            whiteMaskDuration = new float[3];
            whiteMaskDuration[0] = 1.6f;
            whiteMaskDuration[1] = 0.8f;
            whiteMaskDuration[2] = 1.6f;

            playTime = new float[10];
            playTime[0] = 1.7f;
            playTime[1] = 3.2f;
            playTime[2] = 7.2f;
            playTime[3] = 9f;
            playTime[4] = 9.8f;
            playTime[5] = 9.96f;
            playTime[6] = 15f;
            playTime[7] = 19f;
            playTime[8] = 26f;
            playTime[9] = 28f;

            storyPlayShowTime = new float[4];
            storyPlayShowTime[0] = 1.7f;
            storyPlayShowTime[1] = 9f;
            storyPlayShowTime[2] = 15f;
            storyPlayShowTime[3] = 21f;

            storyShowDuration = 1f;
            storyEndDuration = 1f;
            storyMaxShowTime = 4f;

            wolf1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Panel/Biology/Wolf1");
            wolf2PosAni = t.GetComponent<TweenPosition>("PanelChapter/Panel/Biology/Wolf2");
            human1ScaleAni = t.GetComponent<TweenScale>("PanelChapter/Panel/Human1");
            human1PosAni = t.GetComponent<TweenPosition>("PanelChapter/Panel/Human1");
            human2AlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human2");
            human2OtherAlphaAni = t.GetComponent<TweenAlpha>("PanelChapter/Human/Human2Clone");
            human2PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human2");
            human3PosAni = t.GetComponent<TweenPosition>("PanelChapter/Human/Human3");
            panel2 = t.GetComponent<UIPanel>("PanelChapter/Panel");
            FX = t.FindEx("PanelChapter/FX").gameObject;

            human1Scale = new Vector3[2];
            human1Scale[0] = new Vector3(1.1f, 1.1f, 1.0f);
            human1Scale[1] = new Vector3(1.0f, 1.0f, 1.0f);

            human1Duration = new float[2];
            human1Duration[0] = 4f;
            human1Duration[1] = 0.8f;
        }

        //public TweenScale environment3ScaleAni; 临时去掉
        //public TweenAlpha environment3AlphaAni;
    
        public TweenPosition wolf1PosAni;
        public TweenPosition wolf2PosAni;
    
        public TweenScale human1ScaleAni;
        public TweenPosition human1PosAni;
        public TweenAlpha human2AlphaAni;
        public TweenAlpha human2OtherAlphaAni;
        public TweenPosition human2PosAni;
        public TweenPosition human3PosAni;
    
        public UIPanel panel2;
    
        public GameObject FX;
    
        private TweenAlpha OtherTextAni;
    
        #region 动画参数
    
        public Vector3[] human1Scale;
        public float[] human1Duration;
        #endregion
    
        protected override IEnumerator StoryAni()
        {
            PlayBgm();
    
            PlayBlackMaskAlpha(false, 0);
    
            if (OtherTextAni == null)
            {
                OtherTextAni = mDMono.transform.parent.Find("Generally/Other/Text").GetComponent<TweenAlpha>();
            }

            if (panel2 != null)
            {
                panel2.depth = panel.depth + 1;
                panel2.sortingOrder = panel.sortingOrder + 1;
            }

            if (OtherTextAni != null)
            {
                OtherTextAni.gameObject.CustomSetActive(true);
            }

            PlayTween(OtherTextAni);
    
            //0
            float time0 = GetStopTime(0);// 0.5f;
            yield return new WaitForSeconds(time0);
    
            PlayChapterAlpha(false, chapterAlphaDuration[0]);
            PlayWhiteMaskAlpha(whiteMaskAlphaValue[0], 0, whiteMaskDuration[0]);
    
            FX.CustomSetActive(true);
            skipObj.CustomSetActive(true);
    
            //1
            float time1 = GetStopTime(1); //1.5f;
            yield return new WaitForSeconds(time1);
    
            PlayChapterTweenPos(0);
            PlayChapterTweenScale(0);
            PlayTweenScale(human1ScaleAni, Vector3.one, human1Scale[0], human1Duration[0]);
    
            PlayTween(wolf1PosAni);
    
            //environment3ScaleAni.ResetToBeginning();
            //environment3ScaleAni.PlayForward();
    
            //environment3AlphaAni.ResetToBeginning();
            //environment3AlphaAni.PlayForward();
    
            //2
            float time2 = GetStopTime(2);//4f;
            yield return new WaitForSeconds(time2);
    
            PlayChapterAlpha(true, chapterAlphaDuration[1]);
            PlayWhiteMaskAlpha(0, whiteMaskAlphaValue[1], whiteMaskDuration[1]);
            
            PlayTweenScale(human1ScaleAni, human1Scale[0], Vector3.one, human1Duration[0]);
    
            //3
            float time3 = GetStopTime(3);//3f;
            yield return new WaitForSeconds(time3);
    
            PlayChapterAlpha(false, chapterAlphaDuration[2]);
            PlayWhiteMaskAlpha(whiteMaskAlphaValue[2], 0, whiteMaskDuration[2]);
            
            PlayTweenScale(human1ScaleAni, Vector3.one, human1Scale[1], human1Duration[1]);
    
            //4
            float time4 = GetStopTime(4); //human1Duration[1];
            yield return new WaitForSeconds(time4);
            
            PlayTweenScale(human1ScaleAni, human1Scale[1], Vector3.one, human1Duration[1] / 4);
    
            //5
            float time5 = GetStopTime(5);//human1Duration[1] / 5;
            yield return new WaitForSeconds(time5);
    
            PlayChapterTweenPos(1);
            PlayChapterTweenScale(1);
    
            PlayTween(human1PosAni);
            PlayTween(human2PosAni);
            PlayTween(human3PosAni);
            PlayTween(wolf2PosAni);
    
            //6
            float time6 = GetStopTime(6);//5f;
            yield return new WaitForSeconds(time6);
    
            PlayTween(human2AlphaAni);
            PlayTween(human2OtherAlphaAni);
    
            //7
            float time7 = GetStopTime(7);//4f;
            yield return new WaitForSeconds(time7);
    
            PlayChapterTweenPos(2);
            PlayChapterTweenScale(2);
    
            //8
            float time8 = GetStopTime(8);//7f;
            yield return new WaitForSeconds(time8);
    
            PlayChapterAlpha(true, chapterAlphaDuration[3]);
    
            //9
            float time9 = GetStopTime(9);
            yield return new WaitForSeconds(time9);
    
            if (mDMono != null)
            {
                mDMono.gameObject.CustomSetActive(false);
            }

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
                //"那是W.D530年，苏格拉底尚不知战争为何物",
                //"为游历世界，他担任阿拉里克东征军的随军书记",
                //"他一路记载自己的所见所闻",
                //"却不知早已被副将贝尔特朗盯上",
    
                EB.Localizer.GetString("ID_STORY_CHAPTER1_1_1"),
                EB.Localizer.GetString("ID_STORY_CHAPTER1_1_2"),
                EB.Localizer.GetString("ID_STORY_CHAPTER1_1_3"),
                EB.Localizer.GetString("ID_STORY_CHAPTER1_1_4"),
            };
        }
    }
}
