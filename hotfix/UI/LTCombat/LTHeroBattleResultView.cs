using UnityEngine;
using System.Collections;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTHeroBattleResultView : DynamicMonoHotfix
    {
        public UILabel starLabel;
        public UIProgressBar winProgressBar;
        public UILabel victoryLabel;
        public TweenAlpha winLightSpt;
        public System.Action onShownAnimCompleted;
        
        private bool isWon;
        public UITexture cupUITexture;
        public Texture[] CupTextures;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            starLabel = t.GetComponent<UILabel>("BottomCenterHold/Cup/Star_Label");
            winProgressBar = t.GetComponent<UIProgressBar>("BottomCenterHold/WinProgressBar");
            victoryLabel = t.GetComponent<UILabel>("BottomCenterHold/WinProgressBar/Progress_Label (4)");
            winLightSpt = t.GetComponent<TweenAlpha>("BottomCenterHold/WinProgressBar/Light");
            
            cupUITexture = mDMono.transform.GetComponent<UITexture>("BottomCenterHold/Cup");
            if (mDMono.ObjectParamList!=null)
            {
                CupTextures = new Texture[mDMono.ObjectParamList.Count];
                for (int i = 0; i < mDMono.ObjectParamList.Count; i++)
                {
                    CupTextures[i] = (Texture)mDMono.ObjectParamList[i];
                }
            }
        }

        public void Show(bool isWon)
        {
    		this.isWon = isWon;
            LTNewHeroBattleManager.GetInstance().GetNewHeroBattleInfo((bol) => SetData());
        }
        
        void SetData()
        {
            int type = (int)LTNewHeroBattleManager.GetInstance().GetCacheCurrentType();
            cupUITexture.SetTexture(CupTextures[type]);
            starLabel.gradientBottom = LTNewHeroBattleHudController.colorList[type];
            int maxCount;
            int winCount=0;  
            if (type < 2)
            {
                maxCount= LTNewHeroBattleHudController.MaxLay;  
                int id = LTNewHeroBattleManager.GetInstance().GetCacheFinishLayer();
                winCount = EventTemplateManager.Instance.GetNextHeroBattleData(id).Stage-1;
                if (isWon) winCount++;
            }
            else
            {
                 maxCount = EventTemplateManager.Instance.GetClashOfHeroesRewardTpls().Count;
                 winCount= LTNewHeroBattleManager.GetInstance().NewHeroBattleCurWinCount;  
            }

            if (isWon)
    		{
                float value =(float)(winCount - 1) / (float)maxCount;
                winProgressBar.value = value;
                LTUIUtil.SetText(starLabel, (winCount - 1).ToString());
                LTUIUtil.SetText(victoryLabel, string.Format("{0}/{1}", (winCount - 1), maxCount));
                if (LTNewHeroBattleManager.GetInstance().IsTodayWinOneTeam())
                {
                    LTDailyDataManager.Instance.SetDailyDataRefreshState();
                }
                if (winCount > 0)
                {
                    StartCoroutine(StartProgressGrowAnim(winCount - 1, maxCount));
                }
                else
                {
                    if (onShownAnimCompleted != null)
                    {
                        onShownAnimCompleted();
                    }
                }
    		}
    		else
    		{
                float value = (float)winCount / (float)maxCount;
                winProgressBar.value = value;
                LTUIUtil.SetText(starLabel, winCount.ToString());
                LTUIUtil.SetText(victoryLabel, string.Format("{0}/{1}", winCount, maxCount));

                if (onShownAnimCompleted != null)
                {
                    onShownAnimCompleted();
                }
            }
    	}
    
        private WaitForSeconds wait05 = new WaitForSeconds(0.5f);

        IEnumerator StartProgressGrowAnim(int cur,int max)
    	{
    		yield return wait05;		
            
            winLightSpt.SetOnFinished(delegate () {
                if (onShownAnimCompleted != null)
                {
                    onShownAnimCompleted();
                }
            });

            winLightSpt.ResetToBeginning();
            winLightSpt.PlayForward();
            float value = (float)(cur + 1) / (float)max;
            winProgressBar.value = value;
            LTUIUtil.SetText(starLabel, (cur + 1).ToString());
            LTUIUtil.SetText(victoryLabel, string.Format("{0}/{1}", (cur + 1), max));
        }
    }
}
