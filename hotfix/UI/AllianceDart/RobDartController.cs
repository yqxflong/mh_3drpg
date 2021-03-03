using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class RobDartController : DynamicMonoHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();
            clickLimit = false;
            var t = mDMono.transform;
            TransferDartTimeLabel = t.GetComponent<UILabel>("Tips/ActivityTimeLabel/TimeLabel");
            ResidueRobNumLabel = t.GetComponent<UILabel>("ResidueNum/Label");
            ResidueTransferDartNumLabel = t.GetComponent<UILabel>("GotoTransferBtn/ResidueNum");
            PageNumLabel = t.GetComponent<UILabel>("PageNum");
            LeftPageBtn = t.GetComponent<UIButton>("PageLeftBtn");
            RightPageBtn = t.GetComponent<UIButton>("PageRightBtn");
            ItemArray = new RobDartItem[4];
            ItemArray[0] = t.GetMonoILRComponent<RobDartItem>("UIGrid/0");
            ItemArray[1] = t.GetMonoILRComponent<RobDartItem>("UIGrid/0 (1)");
            ItemArray[2] = t.GetMonoILRComponent<RobDartItem>("UIGrid/0 (2)");
            ItemArray[3] = t.GetMonoILRComponent<RobDartItem>("UIGrid/0 (3)");

            t.GetComponent<UIButton>("PageLeftBtn").onClick.Add(new EventDelegate(OnLeftPageClick));
            t.GetComponent<UIButton>("PageRightBtn").onClick.Add(new EventDelegate(OnRightPageClick));
            t.GetComponent<UIButton>("RefreshBtn").onClick.Add(new EventDelegate(OnRefreshClick));            
        }
    
    	public UILabel TransferDartTimeLabel;
    	public UILabel ResidueRobNumLabel;
    	public UILabel ResidueTransferDartNumLabel;
    	public UILabel PageNumLabel;
    	public RobDartItem[] ItemArray;
    	public UIButton LeftPageBtn;
    	public UIButton RightPageBtn;
    	private int MaxPageCount;
    	private int CurPageIndex;
        private bool clickLimit;
        private float Timer;
    
        public override void OnEnable()
    	{
			AlliancesManager.Instance.GetRobInfo();
    		GameDataSparxManager.Instance.RegisterListener(AlliancesManager.dartDataId, OnInfoListener);
    		GameDataSparxManager.Instance.RegisterListener(AlliancesManager.robDartDataId, OnRobInfoListener);
    	}
    
        public override void OnDisable()
    	{
    		GameDataSparxManager.Instance.UnRegisterListener(AlliancesManager.dartDataId, OnInfoListener);
    		GameDataSparxManager.Instance.UnRegisterListener(AlliancesManager.robDartDataId, OnRobInfoListener);
    	}
    
        public void Update()
        {
            if (clickLimit)
            {
                if (Timer < 1.2f)
                {
                    Timer += Time.deltaTime;
                }
                else
                {
                    Timer = 0;
                    clickLimit = false;
                    ErasureMonoUpdater();
                }
            }
        }
    
        private void OnInfoListener(string path, INodeData data)
    	{
    		AllianceEscortUtil.FormatResidueRobDartNum(ResidueRobNumLabel);
    
    		int residueTransferNum = AllianceEscortUtil.GetResidueTransferDartNum();
    		string colorStr = residueTransferNum > 0 ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
    		LTUIUtil.SetText(ResidueTransferDartNumLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_AllianceEscortHudController_2955"), colorStr, residueTransferNum));
            
    		LTUIUtil.SetText(TransferDartTimeLabel, Hotfix_LT.Data.EventTemplateManager.Instance.GetActivityOpenTimeStr("escort_start", "escort_stop"));
    	}
    
    	private void OnRobInfoListener(string path, INodeData data)
    	{
    		AllianceRobDartInfo info = data as AllianceRobDartInfo;
    
    		MaxPageCount = Mathf.CeilToInt(info.Members.Count/4.0f);
    		MaxPageCount = MaxPageCount <= 0 ? 1 : MaxPageCount;
    
    		CurPageIndex = 0;
    		RefreshPage();
    	}
    
    	void RefreshPage()
    	{
    		var members = AlliancesManager.Instance.RobDartInfo.Members;
    		LTUIUtil.SetText(PageNumLabel , string.Format("{0}/{1}", CurPageIndex + 1, MaxPageCount));
    		LeftPageBtn.isEnabled=CurPageIndex > 0;
    		RightPageBtn.isEnabled=CurPageIndex+1 < MaxPageCount;
    
    		int startIndex = CurPageIndex*4;
    		int endIndex = CurPageIndex * 4 + 3;
    		if (endIndex >= members.Count)
    			endIndex = members.Count - 1;
    		int fillIndex = 0;
    		for (int indexInDataArray = startIndex; indexInDataArray <= endIndex;++fillIndex, ++indexInDataArray)
    		{
    			ItemArray[fillIndex].Fill(members[indexInDataArray]);
    		}
    		for (int index = fillIndex; index < 4; ++index)
    		{
    			ItemArray[index].Clear();
    		}
    	}
    
    	public void OnLeftPageClick()
    	{
    		if (CurPageIndex > 0)
    			CurPageIndex--;
    
    		RefreshPage();
    	}
    
    	public void OnRightPageClick()
    	{
    		if (CurPageIndex+1 < MaxPageCount)
    			CurPageIndex++;
    
    		RefreshPage();
    	}
    		
    	public void OnRefreshClick()
    	{
    		AlliancesManager.Instance.GetRobInfo();
    	}
    
        public void OnRobClick(RobDartItem data)
        {          
            if (clickLimit)
            {
                EB.Debug.Log("OnRobClick °´Å¥CDÖÐÀ¹½ØÂß¼­Î´Ö´ÐÐ");
                return;
            }
            EB.Debug.Log("OnRobClick Ö´ÐÐ¾üÍÅÀ¹½ØÂß¼­");
            clickLimit = true;
            RegisterMonoUpdater();
            if (!AllianceEscortUtil.IsMeetRobCondition())
            {
                return;
            }   
            var mItemData = data.GetItemData();
            switch (AlliancesManager.Instance.DartData.State)
            {
                case eAllianceDartCurrentState.Transfer:
                case eAllianceDartCurrentState.Transfering:
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("0", EB.Localizer.GetString("ID_ROB_DART"));
                    MessageTemplateManager.ShowMessage(902053, ht, null);
                    break;
                case eAllianceDartCurrentState.None:
                case eAllianceDartCurrentState.Robing:
                case eAllianceDartCurrentState.Rob:
                    AlliancesManager.Instance.RobDartInfo.RobAwards = mItemData.Award;
                    AlliancesManager.Instance.Rob(mItemData.Uid, mItemData.DartId, delegate (Hashtable result)
                    {
                        if (result != null)
                        {
                            GlobalMenuManager.Instance.CloseMenu("LTAllianceEscortHud");
                            string targetSceneName = EB.Dot.String("escortAndRob.robDart.info.targetSceneName", result, string.Empty);
                            string strPos = EB.Dot.String("escortAndRob.robDart.info.pos", result, string.Empty);
                            Vector3 to_pos = GM.LitJsonExtension.ImportVector3(strPos);
                            Vector3 from_pos = new Vector3(25.0f, 0.0f, 45.0f);
    
                            //SceneLogic.Transfer_RobDart(MainLandLogic.GetInstance().CurrentSceneName, from_pos, 90.0f, targetSceneName, to_pos, 90.0f, mItemData.Uid, mItemData.DartId);
                            //AlliancesManager.Instance.Fight(mItemData.Uid, mItemData.DartId);
                            AlliancesManager.Instance.RobFight(mItemData.Uid, mItemData.DartId);
                        }
                    });
                    break;
            }
        }
    
    }
}
