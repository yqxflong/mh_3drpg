using UnityEngine;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class NationBattleScoreBoardController : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            MotionFrameTween = t.GetComponent<UITweener>();
            CellItemList = new List<NationBattleScoreBoardCell>();
            CellItemList.Add(t.GetMonoILRComponent<NationBattleScoreBoardCell>("MotionFrame/ListGrid/Item"));
            CellItemList.Add(t.GetMonoILRComponent<NationBattleScoreBoardCell>("MotionFrame/ListGrid/Item (1)"));
            CellItemList.Add(t.GetMonoILRComponent<NationBattleScoreBoardCell>("MotionFrame/ListGrid/Item (2)"));
            t.GetComponent<UIButton>("MotionFrame/TopSide/CloseBtn").onClick.Add(new EventDelegate(OnCloseBtnClick));

        }
    
    	public UITweener MotionFrameTween;
    	public List<NationBattleScoreBoardCell> CellItemList;
    
    	public void OnOpenBtnClick()
        {
            if (MotionFrameTween.gameObject.activeSelf) return;
            MotionFrameTween.gameObject.CustomSetActive(true);
            NationManager.Instance.RefreshCityScore(1, null);
            MotionFrameTween.onFinished.Clear();
            MotionFrameTween.onFinished.Add(new EventDelegate(SetTweenObjActive));
            MotionFrameTween.PlayForward();
    	}
    
    	public void OnCloseBtnClick()
    	{
            MotionFrameTween.onFinished.Clear();
            MotionFrameTween.onFinished.Add(new EventDelegate(SetTweenObjDisactive));
            MotionFrameTween.PlayReverse();
    		NationManager.Instance.RefreshCityScore(0, null);
    	}
    
        public void SetTweenObjActive()
        {
            MotionFrameTween.gameObject.CustomSetActive(true);
        }
        public void SetTweenObjDisactive()
        {
            MotionFrameTween.gameObject.CustomSetActive(false);
        }
    
        public override  void OnEnable()
        {
            GameDataSparxManager.Instance.RegisterListener(NationManager.ScoreRankDataId, OnListDataListener);
        }
    
        public override void OnDisable()
        {
            GameDataSparxManager.Instance.UnRegisterListener(NationManager.ScoreRankDataId, OnListDataListener);
        }
    
    	void OnListDataListener(string path, INodeData data)
    	{
    		NationScoreRankList rankList = data as NationScoreRankList;
    		List<NationScore> nationScoreList = rankList.GetSortedDataList();
    		if (nationScoreList != null)
    		{
    			for (int cellIndex = 0; cellIndex < nationScoreList.Count; ++cellIndex)
    			{
    				CellItemList[cellIndex].Fill(nationScoreList[cellIndex]);
    			}
    
    			for (int excessIndex = nationScoreList.Count; excessIndex < CellItemList.Count; ++excessIndex)
    			{
    				CellItemList[excessIndex].Fill(null);
    			}
    		}
    		else
    		{
    			Debug.LogError("nationScoreRank is null");
    		}
    	}
    }
}
