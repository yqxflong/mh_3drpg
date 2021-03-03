using System.Collections;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTStoryChapter : DynamicMonoHotfix
    {
        public UIPanel panel;

        public TweenAlpha chapterAlphaAni;
        public TweenPosition chapterPosAni;
        public TweenScale chapterScaleAni;

        public TweenAlpha whiteMask;
        public TweenAlpha blackMask;

        public TweenAlpha talkAni;
        public UILabel talkLab;
        public UILabel talkShadowLab;

        public GameObject skipObj;

        protected Hotfix_LT.Data.ChapterStory storyData;
        protected bool isInitStoryTalkSucc;

        #region 动画参数
        /// <summary>平移动画参数</summary>
        public Vector3[] chapterPos;
        /// <summary>缩放动画参数</summary>
        public float[] chapterScale;
        /// <summary>动画时间</summary>
        public float[] chapterDuration;
        /// <summary>淡入淡出动画</summary>
        public float[] chapterAlphaDuration;

        /// <summary>白屏动画参数</summary>
        public float[] whiteMaskAlphaValue;
        /// <summary>白屏动画时间</summary>
        public float[] whiteMaskDuration;

        /// <summary>黑屏动画时间</summary>
        public float[] blackMaskDuration;

        /// <summary>剧情播放时机点</summary>
        public float[] playTime;

        /// <summary>剧情文本</summary>
        public string[] storyTalk;
        /// <summary>剧情文本播放时机点</summary>
        public float[] storyPlayShowTime;

        /// <summary>文字淡入的动画时间</summary>
        public float storyShowDuration;
        /// <summary>文字淡出的动画时间</summary>
        public float storyEndDuration;
        /// <summary>文字停留的最长时间（除去淡入淡出时间）</summary>
        public float storyMaxShowTime;
        #endregion

        public virtual void PlayAni()
        {
            storyData = Hotfix_LT.Data.GuideTemplateManager.Instance.GetChapterStoryByChpaterId(mDMono.gameObject.name);
            if (storyData == null)
            {
                //EB.Debug.LogError(string.Format("Guide,Hotfix_LT.Data.ChapterStory is Null, stroyName : {0}", gameObject.name));
            }

            InitStoryTalk();
            StartCoroutine(StoryAni());
            StartCoroutine(StoryTalkAni());
        }

        protected virtual IEnumerator StoryAni()
        {
            yield return null;
            Hotfix_LT.Messenger.Raise(EventName.OnStoryPlaySucc);
        }

        protected virtual IEnumerator StoryTalkAni()
        {
            int index = 0;
            while (index < storyTalk.Length)
            {
                float showTime = GetTalkStopShowTime(index);
                yield return new WaitForSeconds(showTime);
                PlayTalkAlpha(true);
                talkLab.text = talkShadowLab.text = storyTalk[index];

                float endTime = GetTalkStopEndTime(index);
                yield return new WaitForSeconds(endTime);
                PlayTalkAlpha(false);

                index++;
            }
        }

        protected void PlayBgm()
        {
            if (storyData != null && !string.IsNullOrEmpty(storyData.bgm))
            {
                FusionAudio.PostBGMEvent(storyData.bgm, true);
                FusionAudio.StartBGM();
            }
        }

        public void StopBgm()
        {
            if (storyData != null && !string.IsNullOrEmpty(storyData.bgm))
            {
                FusionAudio.StopBGM();
            }
        }

        protected void PlayTweenPos(TweenPosition tweenPos, Vector3 from, Vector3 to, float duration)
        {
            if (tweenPos == null)
            {
                return;
            }

            if (duration <= 0)
            {
                tweenPos.transform.localPosition = to;
                return;
            }

            tweenPos.from = from;
            tweenPos.to = to;
            tweenPos.duration = duration;
            tweenPos.ResetToBeginning();
            tweenPos.PlayForward();
        }

        protected void PlayTweenScale(TweenScale tweenScale, Vector3 from, Vector3 to, float duration)
        {
            if (tweenScale == null)
            {
                return;
            }

            if (duration <= 0)
            {
                tweenScale.transform.localScale = to;
                return;
            }

            tweenScale.from = from;
            tweenScale.to = to;
            tweenScale.duration = duration;
            tweenScale.ResetToBeginning();
            tweenScale.PlayForward();
        }

        protected void PlayChapterTweenPos(int index)
        {
            if (chapterPosAni == null)
            {
                return;
            }

            if (chapterDuration[index] <= 0)
            {
                chapterPosAni.transform.localPosition = chapterPos[index];
                return;
            }

            chapterPosAni.from = index <= 0 ? Vector3.zero : chapterPos[index - 1];
            chapterPosAni.to = chapterPos[index];
            chapterPosAni.duration = chapterDuration[index];
            chapterPosAni.ResetToBeginning();
            chapterPosAni.PlayForward();
        }

        protected void PlayChapterTweenScale(int index)
        {
            if (chapterScaleAni == null)
            {
                return;
            }

            if (chapterDuration[index] <= 0)
            {
                chapterScaleAni.transform.localScale = Vector3.one * chapterScale[index];
                return;
            }

            chapterScaleAni.from = index <= 0 ? Vector3.one : Vector3.one * chapterScale[index - 1];
            chapterScaleAni.to = Vector3.one * chapterScale[index];
            chapterScaleAni.duration = chapterDuration[index];
            chapterScaleAni.ResetToBeginning();
            chapterScaleAni.PlayForward();
        }

        protected void PlayChapterAlpha(bool isForward, float duration)
        {
            if (chapterAlphaAni == null)
            {
                return;
            }

            chapterAlphaAni.from = isForward ? 1 : 0;
            chapterAlphaAni.to = isForward ? 0 : 1;
            chapterAlphaAni.duration = duration;
            chapterAlphaAni.ResetToBeginning();
            chapterAlphaAni.PlayForward();
        }

        protected void PlayWhiteMaskAlpha(float from, float to, float duration)
        {
            if (whiteMask == null)
            {
                return;
            }

            whiteMask.from = from;
            whiteMask.to = to;
            whiteMask.duration = duration;
            whiteMask.ResetToBeginning();
            whiteMask.PlayForward();
        }

        protected void PlayBlackMaskAlpha(bool isForward, float duration)
        {
            if (blackMask == null)
            {
                return;
            }

            blackMask.from = isForward ? 0 : 1;
            blackMask.to = isForward ? 1 : 0;
            blackMask.duration = duration;
            blackMask.ResetToBeginning();
            blackMask.PlayForward();
        }

        protected void PlayTalkAlpha(bool isShow)
        {
            if (talkAni == null)
            {
                return;
            }

            talkAni.from = isShow ? 0 : 1;
            talkAni.to = isShow ? 1 : 0;
            talkAni.duration = isShow ? storyShowDuration : storyEndDuration;
            talkAni.ResetToBeginning();
            talkAni.PlayForward();
        }

        protected void PlayTween(UITweener tween)
        {
            if (tween == null)
            {
                return;
            }

            tween.ResetToBeginning();
            tween.PlayForward();
        }

        protected void PlayTweenArray(UITweener tween, Vector3[] vArray, float[] durations, int firstIndex = 0, int lastIndex = 0)
        {
            if (lastIndex >= vArray.Length)
            {
                EB.Debug.LogError("StoryChapter PlayTweenArray Error, lastIndex = {0}, vArray.Length = {1}", lastIndex, vArray.Length);
                return;
            }

            if (lastIndex == 0)
            {
                lastIndex = vArray.Length - 1;
            }

            if (firstIndex >= lastIndex)
            {
                EB.Debug.LogError("StoryChapter PlayTweenArray Error, firstIndex = {0}, lastIndex = {1}", firstIndex, lastIndex);
                return;
            }

            int index = firstIndex;
            if (tween is TweenPosition)
            {
                TweenPosition tw = tween as TweenPosition;
                EventDelegate.Callback call = () => { };
                call = () =>
                {
                    if (index >= lastIndex)
                    {
                        return;
                    }
                    PlayTweenPos(tw, vArray[index], vArray[index + 1], durations[index + 1]);
                    index++;
                    tw.SetOnFinished(() =>
                    {
                        call();
                    });
                };
                call();
            }
            else if (tween is TweenScale)
            {
                TweenScale tw = tween as TweenScale;
                EventDelegate.Callback call = () => { };
                call = () =>
                {
                    if (index >= lastIndex)
                    {
                        return;
                    }
                    PlayTweenScale(tw, vArray[index], vArray[index + 1], durations[index + 1]);
                    index++;
                    tw.SetOnFinished(() =>
                    {
                        call();
                    });
                };
                call();
            }
        }

        protected float GetStopTime(int index)
        {
            if (index == 0)
            {
                return playTime[0];
            }
            float time = playTime[index] - playTime[index - 1];

            return time;
        }

        protected float GetTalkStopShowTime(int index)
        {
            if (index == 0)
            {
                return storyPlayShowTime[0];
            }
            float time = storyPlayShowTime[index] - storyPlayShowTime[index - 1];

            time -= GetTalkStopEndTime(index - 1);

            return time;
        }

        protected float GetTalkStopEndTime(int index)
        {
            if (index + 1 >= storyPlayShowTime.Length)
            {
                return storyShowDuration + storyMaxShowTime;
            }
            float time = storyPlayShowTime[index + 1] - storyPlayShowTime[index];
            time = time >= storyShowDuration + storyMaxShowTime + storyEndDuration ? storyShowDuration + storyMaxShowTime : time - storyEndDuration;

            return time;
        }

        protected virtual void InitStoryTalk()
        {
            if (storyPlayShowTime == null || storyPlayShowTime.Length <= 0)
            {
                return;
            }

            //暂时不需要读这张表配置，文本读其他表
            //if (storyData != null && !string.IsNullOrEmpty(storyData.aside))
            //{
            //    storyTalk = storyData.aside.Split(';');
            //    if (storyTalk.Length == storyPlayShowTime.Length)
            //    {
            //        isInitStoryTalkSucc = true;
            //        return;
            //    }
            //    //EB.Debug.LogError(string.Format("storyTalk.Length : {0}, storyPlayShowTime.Length : {1}", storyTalk.Length, storyPlayShowTime.Length));
            //}

            isInitStoryTalkSucc = false;
            //EB.Debug.LogError(string.Format("Guide,Hotfix_LT.Data.ChapterStory Aside is Error, stroyName : {0}", gameObject.name));
        }
    }
}
