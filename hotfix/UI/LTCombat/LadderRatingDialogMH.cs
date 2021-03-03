using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class LadderRatingDialogMH : DynamicMonoHotfix
    {
        public UILabel m_StageNameLabel;
        public UILabel m_ScoreLabel;
        public DynamicUISprite m_StageIcon;
        public UIProgressBar ProgressBar;
        public UILabel ProgressBarLabel;
        public UILabel TitleLabel;

        [System.NonSerialized]
        public bool IsWon;
        bool iconTweenerFinished;
        public float LerpProgressSpeed = 1;
        Coroutine LerpCoroutine;
        public System.Action onShownAnimCompleted;
        public List<GameObject> FxList;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_StageNameLabel = t.GetComponent<UILabel>("Container/Stage/Name");
            m_ScoreLabel = t.GetComponent<UILabel>("Container/AddNum");
            m_StageIcon = t.GetComponent<DynamicUISprite>("Container/Stage/Icon");
            ProgressBar = t.GetComponent<UIProgressBar>("Container/ProgressBar");
            ProgressBarLabel = t.GetComponent<UILabel>("Container/ProgressBar/Label");
            TitleLabel = t.GetComponent<UILabel>("Container/Title");
            LerpProgressSpeed = 0.15f;

            FxList = new List<GameObject>();
            FxList.Add(t.FindEx("Container/Stage/Icon/fx").gameObject);
            FxList.Add(t.FindEx("Container/Stage/Icon/fx (1)").gameObject);
            FxList.Add(t.FindEx("Container/Stage/Icon/fx (2)").gameObject);
            FxList.Add(t.FindEx("Container/Stage/Icon/fx (3)").gameObject);
            FxList.Add(t.FindEx("Container/Stage/Icon/fx (4)").gameObject);
            FxList.Add(t.FindEx("Container/Stage/Icon/fx (5)").gameObject);
            FxList.Add(t.FindEx("Container/Stage/Icon/fx (6)").gameObject);

            ProgressBar.gameObject.SetActive(false);
            m_ScoreLabel.gameObject.SetActive(false);
        }

        private void Reset()
    	{
    	    mDMono.transform.localScale = Vector3.one;
    	}

        public override void OnEnable()
    	{
    		Reset();
    		Show();
    	}
    
    	private void Show()
    	{
    		string currentStage="qingtong";
    		int point=0;
    		int changePoint=0;
            //float mul=1;
    		DataLookupsCache.Instance.SearchDataByID<string>("ladder.stage", out currentStage);
    		DataLookupsCache.Instance.SearchIntByID("ladder.point", out point);
    		DataLookupsCache.Instance.SearchIntByID("ladder.changePoint", out changePoint);
    		m_StageIcon.spriteName = LadderController.GetStageSpriteName(currentStage);
            for (int i = 1; i < GameStringValue.Ladder_Stage_Names.Length; i++)//青铜无特效因此i初始为0
            {
                FxList[i - 1].CustomSetActive(currentStage == GameStringValue.Ladder_Stage_Names[i]);
            }
            LTUIUtil.SetText(m_StageNameLabel, LadderController.GetStageCharacterName(currentStage));
    		UITweener[] tws = m_StageIcon.GetComponents<UITweener>();
            for (int i = 0; i < tws.Length; i++)
    		{
	            tws[i].ResetToBeginning();
	            tws[i].PlayForward();
	            tws[i].SetOnFinished(delegate() {
    				iconTweenerFinished = true;
    			});
    		}
    
    		float previousProgress;
    		float targetProgressValue;
    		int stageScore;
    		if (!currentStage.Equals(GameStringValue.Ladder_Stage_Names[GameStringValue.Ladder_Stage_Names.Length - 1])) //不是最后一个段位
    		{
    			int stage_index = System.Array.IndexOf(GameStringValue.Ladder_Stage_Names, currentStage);
    			int currentNeed = LadderManager.Instance.Config.GetStageNeedScore(currentStage);
    			stageScore = LadderManager.Instance.Config.GetStageNeedScore(GameStringValue.Ladder_Stage_Names[stage_index+1]);
    			if ((stageScore - currentNeed == 0))
    			{
    				EB.Debug.LogError("ladder nextNeed - currentNeed = 0");
    				previousProgress = 1;
    				targetProgressValue = 1;
    			}
    			else
    			{
    				targetProgressValue = (point - currentNeed) / (float)(stageScore - currentNeed);
    				if (changePoint > 0)
    				{
    					if (point - changePoint - currentNeed <= 0)
    						previousProgress = 0;
    					else
    						previousProgress = (point - changePoint - currentNeed) / (float)(stageScore - currentNeed);
    				}
    				else
    				{
    					previousProgress = (point - changePoint - currentNeed) / (float)(stageScore - currentNeed);
    					previousProgress = previousProgress > 1 ? 1 : previousProgress;
    				}
    			}
    		}
    		else
    		{
    			previousProgress = 1;
    			targetProgressValue = 1;
    			stageScore = LadderManager.Instance.Config.GetStageNeedScore(currentStage);
    		}
    
            TitleLabel.text = TitleLabel.transform.GetChild(0).GetComponent<UILabel>().text = (IsWon) ? EB.Localizer.GetString("ID_BATTLE_WIN") : EB.Localizer.GetString("ID_BATTLE_LOSE");
            StartCoroutine(LerpProgress(previousProgress, targetProgressValue,changePoint,point, stageScore));	
    	}
    
    	IEnumerator LerpProgress(float previous_value, float target_value,int changePoint,int targetScore,int stageScore)
    	{
    		while (!iconTweenerFinished)
    		{
    			yield return null;
    		}
    
    		ProgressBar.gameObject.SetActive(true);
    		m_ScoreLabel.gameObject.SetActive(true);
    		if (previous_value == target_value)
    		{
    			ProgressBar.value = target_value;
    			LTUIUtil.SetText(ProgressBarLabel, string.Format("{0}/{1}", targetScore, stageScore));
    			if (changePoint > 0)
    			{
    				m_ScoreLabel.color = LT.Hotfix.Utility.ColorUtility.GreenColor;
    				LTUIUtil.SetText(m_ScoreLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LadderRatingDialogMH_3548"), changePoint)); //
    			}
    			else
    			{
    				m_ScoreLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor;
    				LTUIUtil.SetText(m_ScoreLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LadderRatingDialogMH_3702"), Mathf.Abs(changePoint))); //
    			}
    			if (onShownAnimCompleted != null)
    			{
    				onShownAnimCompleted();
    			}
    			yield break;
    		}
    
    		ProgressBar.value = previous_value;
    		if (changePoint > 0)
    		{
    			m_ScoreLabel.color = LT.Hotfix.Utility.ColorUtility.GreenColor;
    			LTUIUtil.SetText(m_ScoreLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LadderRatingDialogMH_3548"), 0)); //
    		}
    		else
    		{
    			m_ScoreLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor;
    			LTUIUtil.SetText(m_ScoreLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LadderRatingDialogMH_3702"),0)); //
    		}
    
    		float lerpTime = 0;
    		float transition_value = previous_value;
    		while (Mathf.Abs(transition_value - target_value) >= 0.01f)
    		{
    			transition_value = Mathf.Lerp(previous_value, target_value, 1.0f - Mathf.Cos(lerpTime * Mathf.PI * LerpProgressSpeed * 2));
    			lerpTime += Time.deltaTime;
    
    			ProgressBar.value = transition_value;
    			int transitionChangePoint = (int)(((transition_value - previous_value) / (target_value - previous_value)) * changePoint);
    
    			if (changePoint > 0)
    			{
    				LTUIUtil.SetText(ProgressBarLabel, string.Format("{0}/{1}", targetScore-changePoint+ transitionChangePoint, stageScore));
    				LTUIUtil.SetText(m_ScoreLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LadderRatingDialogMH_3548"), transitionChangePoint)); //
    			}
    			else
    			{
    				LTUIUtil.SetText(ProgressBarLabel, string.Format("{0}/{1}", targetScore - changePoint + transitionChangePoint, stageScore));
    				LTUIUtil.SetText(m_ScoreLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LadderRatingDialogMH_3702"), Mathf.Abs(transitionChangePoint))); //
    			}
    			yield return null;
    		}
    		ProgressBar.value = target_value;
    		LTUIUtil.SetText(ProgressBarLabel, string.Format("{0}/{1}", targetScore, stageScore));
    		if (changePoint > 0)
    		{
    			m_ScoreLabel.color = LT.Hotfix.Utility.ColorUtility.GreenColor;
    			LTUIUtil.SetText(m_ScoreLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LadderRatingDialogMH_3548"), changePoint)); //
    		}
    		else
    		{
    			m_ScoreLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor;
    			LTUIUtil.SetText(m_ScoreLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LadderRatingDialogMH_3702"), Mathf.Abs(changePoint))); //
    		}
    
    		if (onShownAnimCompleted != null)
    		{
    			onShownAnimCompleted();
    		}
    		yield break;
    	}

        public override void OnDisable()
    	{
    		StopAllCoroutines();
    	}
    
    	[ContextMenu("TestLerp")]
    	public void TestLerp()
    	{
    		iconTweenerFinished = true;
    		if(LerpCoroutine!=null)
    			StopCoroutine(LerpCoroutine);
    		LerpCoroutine=StartCoroutine(LerpProgress(0.99f,0.6f,-9,33,99));
    	}
    }
}
