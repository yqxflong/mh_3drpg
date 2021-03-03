using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using DG.Tweening;

namespace Hotfix_LT.UI
{
    public class AllianceEscortHudController : UIControllerHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TransferDartItemArray = new TransferDartItem[4];
            TransferDartItemArray[0] = t.GetMonoILRComponent<TransferDartItem>("Content/TransferView/UIGrid/0");
            TransferDartItemArray[1] = t.GetMonoILRComponent<TransferDartItem>("Content/TransferView/UIGrid/0 (1)");
            TransferDartItemArray[2] = t.GetMonoILRComponent<TransferDartItem>("Content/TransferView/UIGrid/0 (2)");
            TransferDartItemArray[3] = t.GetMonoILRComponent<TransferDartItem>("Content/TransferView/UIGrid/0 (3)");
            RotateSpeed = 600f;
            ItemScaleTime = 0.3f;
            ItemScaleStartVector = new Vector3(1.05f, 1.05f, 1f);
            ItemScaleEndVector = Vector3.one;

            controller.backButton = t.GetComponent<UIButton>("UIFrame/TopLeftAnchor/CancelBtn");

            controller.FindAndBindingCoolTriggerEvent(GetList("Content/TransferView/GotoRobBtn", "Content/TransferView/TransferBtn",
				"Content/TransferView/RefreshBtn", "Content/RobView/GotoTransferBtn"), GetList(new EventDelegate(OnGotoRobBtnClick),
				new EventDelegate(OnStartTransferBtnClick), new EventDelegate(OnTransferDartRefreshBtnClick), new EventDelegate(OnGotoTransferBtnClick)));

            object obj = EB.JSON.Parse(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("escortRefreshCost"));
            refreshCostHC = EB.Dot.Integer("quantity", (obj as ArrayList)[0], 0);
            LTUIUtil.SetText(controller.UiLabels["RefreshCostHcLabel"], refreshCostHC.ToString());
        }


    
    	public override bool IsFullscreen() { return true; }
    
    	public TransferDartItem[] TransferDartItemArray;
    
    
		private Ease m_RotateTweenType = Ease.OutQuart;
    	public float RotateSpeed = 500;
    	public Vector3 ItemScaleStartVector = new Vector3(1.1f, 1.1f, 1.1f);
    	public Vector3 ItemScaleEndVector = new Vector3(1, 1, 1);
    	public float ItemScaleTime = 0.3f;
		private Ease m_ItemScaleTweenType = Ease.InBack;
    	int mTicketNum;
    	private bool mRotating=false;
    	private int mSelectIndex=0;
    	private bool mIsRefresh;
    	int refreshCostHC;
        private bool hasRegister;
    	public override IEnumerator OnAddToStack()
    	{
    		AlliancesManager.Instance.TransferDartInfo.DataUpdated = false;
    		AlliancesManager.Instance.GetTransferInfo();
    
    		GameDataSparxManager.Instance.RegisterListener(AlliancesManager.dartDataId, OnInfoListener);
    		GameDataSparxManager.Instance.RegisterListener(AlliancesManager.transferDartDataId, OnTransferInfoListener);
            hasRegister = true;
            yield return base.OnAddToStack();
    
    		//Hotfix_LT.Data.SpecialActivityTemplate template = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(9005);
    		//List<Hotfix_LT.Data.NormalActivityInstanceTemplate> instances = Hotfix_LT.Data.EventTemplateManager.Instance.GetNormalActivityInstanceTemplates(9005);
    
            if(TransferDartItemArray[0].mDMono.gameObject.activeInHierarchy) for(int i=0;i< TransferDartItemArray.Length; i++)
            {
                TransferDartItemArray[i].mDMono.GetComponent<TweenScale>().ResetToBeginning();
                TransferDartItemArray[i].mDMono.GetComponent<TweenScale>().PlayForward();
            }
    
    		LTUIUtil.SetText(controller.UiLabels["TransferDartTimeLabel"],Hotfix_LT.Data.EventTemplateManager.Instance.GetActivityOpenTimeStr("escort_start", "escort_stop"));		
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
            if (hasRegister)
            {
                GameDataSparxManager.Instance.UnRegisterListener(AlliancesManager.dartDataId, OnInfoListener);
                GameDataSparxManager.Instance.UnRegisterListener(AlliancesManager.transferDartDataId, OnTransferInfoListener);
                hasRegister = false;
            }
    		DestroySelf();
    		yield break;
    	}
    
    	private void OnInfoListener(string path, INodeData data)
    	{
    		int residueTransferNum = AllianceEscortUtil.GetResidueTransferDartNum();
    		string colorStr = residueTransferNum > 0 ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
    		LTUIUtil.SetText(controller.UiLabels["ResidueTransferDartNumLabel"], string.Format(EB.Localizer.GetString("ID_codefont_in_AllianceEscortHudController_2955"), colorStr,residueTransferNum));
    
    		int residueRobNum = AllianceEscortUtil.GetResidueRobDartNum();
    		string cs = residueRobNum > 0 ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
    		LTUIUtil.SetText(controller.UiLabels["ResidueRobNumLabel"], string.Format(EB.Localizer.GetString("ID_codefont_in_AllianceEscortHudController_3223"), cs, residueRobNum));
    
    		RefreshBtnState();
    	}
    
    	private void OnTransferInfoListener(string path, INodeData data)
    	{
            AllianceTransferDartInfo info = data as AllianceTransferDartInfo;    
    		if (info.DataUpdated)
    		{
    			info.DataUpdated = false;
    			RefreshTransferInfo(info);
    		}
    	}

        void RefreshBtnState()
    	{
            if (controller.gameObject != null)
            {
                AllianceDartData dartData = AlliancesManager.Instance.DartData;
                bool unable = dartData.State == eAllianceDartCurrentState.Transfer || dartData.State == eAllianceDartCurrentState.Transfering;
                controller.CoolTriggers["TransferDartRefreshBtn"].enabled = !unable;
                controller.UiButtons["StartTransferBtn"].isEnabled = !unable;
            }
    	}
    
    	private void RefreshTransferInfo(AllianceTransferDartInfo info)
    	{
    		SetTransferDartResidueRefreshCount();
    
    		RefreshTransferDartMemberList(info.Members);
    	}
    
    	private void SetTransferDartResidueRefreshCount()
    	{
    		if (AlliancesManager.Instance.TransferDartInfo.Members.Count <= 0)
    		{
    			EB.Debug.LogError("TransferDartInfo not init");
    			return;
    		}
    
    		int residueFreeRefreshNum = AllianceEscortUtil.GetResidueTransferRefreshNum();
    		if (residueFreeRefreshNum < 0)
    		{
    			EB.Debug.LogError("residueFreeRefreshNum < 0 num={0}" , residueFreeRefreshNum);
    			residueFreeRefreshNum = 0;
    		}
    		//int totalFreeRefreshNum = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.EscortRefreshTimes);
    
    		if(residueFreeRefreshNum>0)
    			LTUIUtil.SetText(controller.UiLabels["ResidueTransferDartFreeRefreshNumLabel"],residueFreeRefreshNum.ToString());
    		else
    		{
    			mTicketNum = GameItemUtil.GetInventoryItemNum(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("escortRefreshItem").ToString());
    			if (mTicketNum > 0)
    			{
    				LTUIUtil.SetText(controller.UiLabels["TransferDartRefreshTicketNumLabel"], mTicketNum.ToString());
    				controller.UiLabels["TransferDartRefreshTicketNumLabel"].transform.parent.gameObject.SetActive(true);
    				controller.UiLabels["ResidueTransferDartFreeRefreshNumLabel"].transform.parent.gameObject.CustomSetActive(false);
    				controller.UiLabels["TransferDartRefreshCostHCLabel"].transform.parent.gameObject.CustomSetActive(false);
    			}
    			else
    			{
    				controller.UiLabels["TransferDartRefreshTicketNumLabel"].transform.parent.gameObject.SetActive(false);
    				controller.UiLabels["ResidueTransferDartFreeRefreshNumLabel"].transform.parent.gameObject.CustomSetActive(false);
    				controller.UiLabels["TransferDartRefreshCostHCLabel"].transform.parent.gameObject.CustomSetActive(true);
    				if (BalanceResourceUtil.GetUserDiamond() >= refreshCostHC)
    				{
    					controller.UiLabels["TransferDartRefreshCostHCLabel"].color = LT.Hotfix.Utility.ColorUtility.GreenColor;
    				}
    				else
    				{
    					controller.UiLabels["TransferDartRefreshCostHCLabel"].color = LT.Hotfix.Utility.ColorUtility.RedColor;
    				}
    			}			
    		}
    	}
    
    	private void RefreshTransferDartMemberList(List<TransferDartMember> members)
    	{
    		for (int i = 0; i < members.Count; ++i)
    		{
    			TransferDartItemArray[i].Fill(members[i]);
    		}
    
    		mSelectIndex = GetSelectedIndex();
    		if (mIsRefresh)
    		{
    			mIsRefresh = false;
    			PlaySelectAnimation();
    		}
    		else
    		{
    			controller.GObjects["SelectedFrame"].transform.SetParent(TransferDartItemArray[mSelectIndex].mDMono.transform, false);
    		}
    	}
    
    	void PlaySelectAnimation()
    	{
    		mRotating = true;
    
            controller.GObjects["SimulateRotateTarget"].transform.localEulerAngles = Vector3.zero;
    		LTUIUtil.SetGreyButtonEnable(controller.UiButtons["StartTransferBtn"],false);
    
    		int circleNum = Random.Range(8,11);
    		int offsetAngle = mSelectIndex * 90+1;
    		int rotateAngle = 360 * circleNum + offsetAngle;
    		float durationTime = rotateAngle / RotateSpeed;

			Transform target = controller.GObjects["SimulateRotateTarget"].transform;			
			var option = target.DORotate(new Vector3(target.eulerAngles.x, target.eulerAngles.y, -rotateAngle), durationTime, RotateMode.FastBeyond360);
			option.SetEase(m_RotateTweenType);
			option.SetUpdate(true);

			option.OnComplete( delegate () {

				Transform currentSelectItem = TransferDartItemArray[mSelectIndex].mDMono.transform;
				currentSelectItem.localScale = ItemScaleStartVector;

				var option2nd = currentSelectItem.DOScale(ItemScaleEndVector, ItemScaleTime);
				option2nd.SetEase(m_ItemScaleTweenType);

				option2nd.OnComplete( delegate () {

					var cur = AlliancesManager.Instance.TransferDartInfo.GetCurrentSelectDart();
					if (cur != null && cur.DartName.CompareTo("tian") == 0)
					{
						controller.GObjects["TopTransferDartFx"].CustomSetActive(true);
						ILRTimerManager.instance.AddTimer(3000, 1, delegate
						{
							controller.GObjects["TopTransferDartFx"].CustomSetActive(false);
						});
					}
					mRotating = false;
					//LTUIUtil.SetGreyButtonEnable(controller.CoolTriggers["TransferDartRefreshBtn"], true);
					controller.CoolTriggers["TransferDartRefreshBtn"].GetComponent<UISprite>().color = new Color(1, 1, 1, 1);
					LTUIUtil.SetGreyButtonEnable(controller.UiButtons["StartTransferBtn"], true);

				});
			});
    	}

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public void Update()
    	{
    		//base.Update();
    
    		if (!mRotating)
    		{
    			return;
    		}
    
    		float rotateZ =360 - controller.GObjects["SimulateRotateTarget"].transform.localEulerAngles.z;
    		rotateZ = rotateZ <= 0||rotateZ>=360 ? 0 : rotateZ;
    
    		int locationIndex=Mathf.FloorToInt(rotateZ / 90);
    		controller.GObjects["SelectedFrame"].transform.SetParent(TransferDartItemArray[locationIndex].mDMono.transform, false);
    	}
    
    	#region event handler
    	public void OnTransferTabClick(UIToggle uitoggle)
    	{
    		if (!uitoggle.value)
    			return;
    
    		controller.GObjects["TransferViewOBJ"].SetActive(true);
    		controller.GObjects["RobViewOBJ"].SetActive(false);
    	}
    
    	public void OnRobTabClick(UIToggle uitoggle)
    	{
    		if (!uitoggle.value)
    			return;
    
    		controller.GObjects["TransferViewOBJ"].SetActive(false);
    		controller.GObjects["RobViewOBJ"].SetActive(true);
    	}
    
    	public void OnGotoTransferBtnClick()
    	{
    		controller.GObjects["TransferViewOBJ"].SetActive(true);
    		UITweener tw = controller.GObjects["TransferViewOBJ"].GetComponent<UITweener>();
    		tw.ResetToBeginning();
    		tw.PlayForward();
    		controller.GObjects["RobViewOBJ"].SetActive(false);
    	}
    
    	public void OnGotoRobBtnClick()
    	{
    		controller.GObjects["TransferViewOBJ"].SetActive(false);
    		controller.GObjects["RobViewOBJ"].SetActive(true);
    		UITweener tw = controller.GObjects["RobViewOBJ"].GetComponent<UITweener>();
    		tw.ResetToBeginning();
            tw.PlayForward();
            FusionTelemetry.GamePlayData.PostEsortEvent("open","rob");
        }
    
    	public void OnTransferDartRefreshBtnClick()
    	{
    		//var dartData = AlliancesManager.Instance.DartData;
    		//if (dartData.State == eAllianceDartCurrentState.Transfer || dartData.State == eAllianceDartCurrentState.Transfering)
    		//{
    		//	return;
    		//}
    
    		if(mRotating)
    		{
    			return;
    		}
    
    		if (AllianceEscortUtil.GetResidueTransferDartNum() <= 0)
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_AllianceEscortHudController_8870"));
    			return;
    		}
    
    		var cur = AlliancesManager.Instance.TransferDartInfo.GetCurrentSelectDart();
    		if (AllianceEscortUtil.GetResidueTransferRefreshNum() > 0|| mTicketNum > 0)
    		{
    			if (cur!=null && cur.DartName.CompareTo("tian")==0)
    			{
    				MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_AllianceEscortHudController_9174"), null);
    				return;
    			}
    
    			mIsRefresh = true;
                //LTUIUtil.SetGreyButtonEnable(controller.CoolTriggers["TransferDartRefreshBtn"], false);
                controller.CoolTriggers["TransferDartRefreshBtn"].GetComponent<UISprite>().color = new Color(1, 0, 1, 1);
                LTUIUtil.SetGreyButtonEnable(controller.UiButtons["StartTransferBtn"], false);
    			AlliancesManager.Instance.Refresh();
    		}
    		else
    		{
    			if (BalanceResourceUtil.GetUserDiamond() >= refreshCostHC)
    			{
    				if (cur != null && cur.DartName.CompareTo("tian")==0)
    				{
    					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_AllianceEscortHudController_9174"), null);
    					return;
    				}
    
    				mIsRefresh = true;
                    //LTUIUtil.SetGreyButtonEnable(controller.CoolTriggers["TransferDartRefreshBtn"], false);
                    controller.CoolTriggers["TransferDartRefreshBtn"].GetComponent<UISprite>().color = new Color(1, 0, 1, 1);
                    LTUIUtil.SetGreyButtonEnable(controller.UiButtons["StartTransferBtn"],false);
    				AlliancesManager.Instance.Refresh();
    			}
    			else
                {
                    BalanceResourceUtil.HcLessMessage();
                }			
    		}
    	}
    
    	public void OnStartTransferBtnClick()
    	{
    		if (mIsRefresh|| mRotating)
    			return;
    
    		FusionAudio.PostEvent("UI/General/ButtonClick");
    		if (!AllianceEscortUtil.IsMeetTransferCondition())
    		{
    			return;
    		}
    
    		Receive();
    	}
    	#endregion
    
    	public void Receive()
    	{
    		switch (AlliancesManager.Instance.DartData.State)
    		{
    			case eAllianceDartCurrentState.None:
    			case eAllianceDartCurrentState.Robing:
    			case eAllianceDartCurrentState.Rob:
    				//非活动时间内接取运镖任务时提示：当前不在活动时间，不能接受运镖任务。
    				if (AllianceEscortUtil.GetResidueTransferDartNum() > 0)
    				{
    					var selected = GetSelectedDart();
    					AlliancesManager.Instance.DartData.CurrentDartId = selected.Id;
    					StartTransfer(selected);
    				}
    				else
    				{
    					MessageTemplateManager.ShowMessage(902072);
    				}
    				break;
    			case eAllianceDartCurrentState.Transfer:
    			case eAllianceDartCurrentState.Transfering:
    				//运镖期间不能再领取运镖任务
    				MessageTemplateManager.ShowMessage(902071);
    				break;
    		}
    	}
    
    	public void StartTransfer(TransferDartMember dart)
    	{	
    		switch (AlliancesManager.Instance.DartData.State)
    		{
    			case eAllianceDartCurrentState.Transfering:
    				StartPathFind(dart.DartName, dart.TargetNpc);
    				controller.Close();
    				break;
    			default:
    				AlliancesManager.Instance.Start(dart.Id, delegate (bool successful) {
    					if (successful)
    					{
                            //暂时放在这里，防止时序问题
                            if (hasRegister)
                            {
                                GameDataSparxManager.Instance.UnRegisterListener(AlliancesManager.dartDataId, OnInfoListener);
                                GameDataSparxManager.Instance.UnRegisterListener(AlliancesManager.transferDartDataId, OnTransferInfoListener);
                                hasRegister = false;
                            }
                            controller.Close();
                            StartPathFind(dart.DartName, dart.TargetNpc);
                        }
    					else
    					{
    						EB.Debug.LogError("start transfer dart fail");
    					}
    				});
    				break;
    		}
    	}
    
    	void StartPathFind(string dartName,string npc_id)
    	{
    		PlayerManager.LocalPlayerController().transform .GetMonoILRComponent <Player .PlayerHotfixController >().StartTransfer(dartName,npc_id,true,false);
    	}
    
    	int GetSelectedIndex()
    	{
    		var currentSelect = AlliancesManager.Instance.TransferDartInfo.GetCurrentSelectDart();
    		if (currentSelect != null)
    			return AlliancesManager.Instance.TransferDartInfo.GetDartIndex(currentSelect);
    		else
    			Debug.LogError("Not Found select Dart");
    
    		return 0;
    	}
    
    	TransferDartMember GetSelectedDart()
    	{
    		return AlliancesManager.Instance.TransferDartInfo.Members[GetSelectedIndex()];
    	}
    }
}
