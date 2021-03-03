using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Hotfix_LT.UI
{
    public class CommonExecuteParse
    {
        EventDelegate delForbidOtherFinish = null;
        EventDelegate delTouchMengBanFinish = null;
        EventDelegate delMonologHide = null;
        EventDelegate delClickForceForbid = null;

        //private int _groupId;
        public CommonExecuteParse(/*int groupId*/)
        {
            delForbidOtherFinish = new EventDelegate(OnForbidOtherFinish);
            delTouchMengBanFinish = new EventDelegate(OnTouchMengBanFinish);
            delMonologHide = new EventDelegate(OnMonologHide);
            delClickForceForbid = new EventDelegate(ClickForceForbid);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.None, OnNone);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.OpenView, OpenView);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.CloseView, CloseView);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.TouchMengBan, TouchMengBan);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.Monolog, Monolog);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.ForbidOther, ForbidOther);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.WaitTime, WaitTime);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.ForceForbid, ForceForbid);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetGuideType, SetGuideType);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetLockCamera, SetLockCamera);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.ClickButton, ClickButton);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.OpenObject, OpenObject);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.CloseObject, CloseObject);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.MoveTips, MoveTips);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetNodeCompleted, SetNodeCompleted);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetNodeOnceUndone, SetNodeOnceUndone);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.ShowDragUI, ShowDragUI);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetVirtualBtn, SetVirtualBtn);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetComponentAction, SetComponentAction);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetFirstBattleValue, SetFirstBattleValue);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetBackToMianMenu, SetBackToMianMenu);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetFuncOpen, SetFuncOpen);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetCompletedByTargetNode, SetCompletedByTargetNode);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetSpecialCam, SetSpecialCam);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetNewChapterOpen, SetNewChapterOpen);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetCombatBtnMod, SetCombatBtnMod);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SelectPartnerInPartnerView, SelectPartnerInPartnerView);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.ForbidInstanceFloor, ForbidInstanceFloor);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.MakePartnerLevelUp, MakePartnerLevelUp);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.MakePartnerDressEquip, MakePartnerDressEquip);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.ReturnToMianMenu, ReturnToMianMenu);
            NodeMessageManager.GetInstance().AddExecute(/*_groupId,*/ NodeMessageManager.SetGuideFailState, SetGuideFailState);

            GuideNodeManager.ExecuteJump += OnExecuteJump;
            GuideNodeManager.ExecuteGuideAudio += SetMonologAudio;
        }

        public void Dispose()
        {
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.None, OnNone);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.OpenView, OpenView);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.CloseView, CloseView);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.TouchMengBan, TouchMengBan);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.Monolog, Monolog);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.ForbidOther, ForbidOther);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.WaitTime, WaitTime);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.ForceForbid, ForceForbid);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetGuideType, SetGuideType);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetLockCamera, SetLockCamera);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.ClickButton, ClickButton);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.OpenObject, OpenObject);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.CloseObject, CloseObject);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.MoveTips, MoveTips);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetNodeCompleted, SetNodeCompleted);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetNodeOnceUndone, SetNodeOnceUndone);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.ShowDragUI, ShowDragUI);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetVirtualBtn, SetVirtualBtn);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetComponentAction, SetComponentAction);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetFirstBattleValue, SetFirstBattleValue);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetBackToMianMenu, SetBackToMianMenu);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetFuncOpen, SetFuncOpen);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetCompletedByTargetNode, SetCompletedByTargetNode);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetSpecialCam, SetSpecialCam);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetNewChapterOpen, SetNewChapterOpen);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetCombatBtnMod, SetCombatBtnMod);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SelectPartnerInPartnerView, SelectPartnerInPartnerView);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.ForbidInstanceFloor, ForbidInstanceFloor);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.MakePartnerLevelUp, MakePartnerLevelUp);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.MakePartnerDressEquip, MakePartnerDressEquip);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.ReturnToMianMenu, ReturnToMianMenu);
            NodeMessageManager.GetInstance().RemoveExecute(/*_groupId,*/ NodeMessageManager.SetGuideFailState, SetGuideFailState);

            GuideNodeManager.ExecuteJump -= OnExecuteJump;
            GuideNodeManager.ExecuteGuideAudio -= SetMonologAudio;
        }

        void DispatchExecuteReceipt(string em, bool isTure)
        {
            if (isTure)
            {
                string UmengID = GuideNodeManager.GetInstance().CurrentUmengID();
                if (UmengID != null)
                {
                    FusionTelemetry.GuideData.PostEvent(UmengID, GuideNodeManager.GetInstance().CurrentStepID(), LoginManager.Instance.LocalUser.CreateTime);
                    GlobalUtils.FBSendRecordEvent(UmengID);
                }
            }
            NodeMessageManager.GetInstance().DispatchExecuteReceipt(/*_groupId,*/ em, isTure);
        }

        void OnNone(string str) //找不到的类型直接返回成功
        {
            DispatchExecuteReceipt(NodeMessageManager.None, true);
        }

        void OpenView(string viewName)
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
            switch (viewName)
            {
                case "LTHeroBattleMatch":
                    GlobalMenuManager.Instance.Open("LTHeroBattleMatch", LTHeroBattleModel.GetInstance().matchData);
                    break;
                default:
                    GlobalMenuManager.Instance.Open(viewName);
                    break;
            }
            DispatchExecuteReceipt(NodeMessageManager.OpenView, true);
        }

        void CloseView(string viewName)
        {
            if (viewName == "ToolTip")
            {
                //Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PressReleaseEvent);
            }
            else
            {
                GlobalMenuManager.Instance.CloseMenu(viewName);
            }

            DispatchExecuteReceipt(NodeMessageManager.CloseView, true);
        }

        GameObject viewForbidOtherTarget;
        /// <summary>
        /// 多样性处理 主要是有一些是用release or press处理响应 
        /// </summary>
        /// <param name="et"></param>
        void ProcessUIEventTrigger(UIEventTrigger et)
        {
			if (et.onDragStart.Count > 0)
			{
				et.onDragStart.Add(delForbidOtherFinish);
				return;
			}
			if (et.onDragEnd.Count > 0)
			{
				et.onDragEnd.Add(delForbidOtherFinish);
				return;
			}
			if (et.onDragOut.Count > 0)
			{
				et.onDragOut.Add(delForbidOtherFinish);
				return;
			}
			if (et.onDragOver.Count > 0)
			{
				et.onDragOver.Add(delForbidOtherFinish);
				return;
			}
			if (et.onDrag.Count > 0)
			{
				et.onDrag.Add(delForbidOtherFinish);
				return;
			}
			et.onClick.Add(delForbidOtherFinish);//只添加完成回调到点击事件，防止出现点击事件无法正常触发
        }

        void OnClickForbidOtherFinish(GameObject go)
        {
            OnForbidOtherFinish();
        }

		void OnPressForbidOtherFinish(GameObject go, bool state)
		{
			OnForbidOtherFinish();
		}

		void OnForbidOtherFinish()
        {
            if (viewForbidOtherTarget != null)
            {
                ConsecutiveClickCoolTrigger ccct = viewForbidOtherTarget.GetComponent<ConsecutiveClickCoolTrigger>();
                if (ccct != null && ccct.enabled)
                {
                    RemoveFinishEvent(ccct.clickEvent);
                }
                else if (viewForbidOtherTarget.GetComponent<UIButton>() != null)
                {
                    RemoveFinishEvent(viewForbidOtherTarget.GetComponent<UIButton>().onClick);
                }
                else if (viewForbidOtherTarget.GetComponent<UIEventTrigger>() != null)
                {
                    UIEventTrigger et = viewForbidOtherTarget.GetComponent<UIEventTrigger>();

                    if (et.onClick != null && et.onClick.Contains(delForbidOtherFinish))
                    {
                        RemoveFinishEvent(et.onClick);
                    }
                    else if (et.onDragStart != null && et.onDragStart.Contains(delForbidOtherFinish))
                    {
                        RemoveFinishEvent(et.onDragStart);
                    }
                    else if (et.onDragEnd != null && et.onDragEnd.Contains(delForbidOtherFinish))
                    {
                        RemoveFinishEvent(et.onDragEnd);
                    }
					else if (et.onDragOut != null && et.onDragOut.Contains(delForbidOtherFinish))
					{
						RemoveFinishEvent(et.onDragOut);
					}
					else if (et.onDragOver != null && et.onDragOver.Contains(delForbidOtherFinish))
					{
						RemoveFinishEvent(et.onDragOver);
					}
					else if (et.onDrag != null && et.onDrag.Contains(delForbidOtherFinish))
					{
						RemoveFinishEvent(et.onDrag);
					}
				}
                else if (viewForbidOtherTarget.GetComponent<UIEventListener>() != null)
                {
                    UIEventListener el = viewForbidOtherTarget.GetComponent<UIEventListener>();
                    if (el.onClick != null) el.onClick -= OnClickForbidOtherFinish;
                }
            }
            viewForbidOtherTarget = null;
			MengBanController.Instance.RestoreFinger();
			MengBanController.Instance.Hide();
            GuideNodeManager.IsVirtualBtnGuide = false;
            DispatchExecuteReceipt(NodeMessageManager.ForbidInstanceFloor, true);
            DispatchExecuteReceipt(NodeMessageManager.ForbidOther, true);
        }
        
        private void RemoveFinishEvent(List<EventDelegate> list) //ILR下直接Remove会有问题
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == delForbidOtherFinish)
                {
                    list.RemoveAt(i);
                    return;
                }

            }
        }

        void ForbidOther(string path)
        {
            string[] data = path.Split(',');

			if (data.Length > 0)
			{
				if (data[0].Equals("FindGate"))
					data[0] = GuideNodeManager.GateString;

				viewForbidOtherTarget = GameObject.Find(data[0]);
			}

			if (GuideToolController.Instance.controller.gameObject.activeSelf)
			{
				GuideToolController.Instance.SetLTMoveTipsClose();
			}

            if (data.Length > 1)
            {
				if (int.TryParse(data[1], out int isNum))
				{
					GuideNodeManager.FuncOpenGuideId = data[1];
				}
				else
				{
					string text = data[1];
					if(!text.StartsWith("ModifyFinger"))
					{
						MengBanController.Instance.FingerJudgeStr = text;
					}
					else
					{
						string modification = text.Replace("ModifyFinger:", string.Empty);
						MengBanController.Instance.SetFinger(modification);						
					}
				}
            }

            GuideNodeManager.IsVirtualBtnGuide = true;
            GuideNodeManager.currentGuideId = GuideNodeManager.GetInstance().CurrentGroupID();

			if (viewForbidOtherTarget == null) //为空 执行失败返回  隐藏的情况交由超时处理
            {
                EB.Debug.LogError("GuideNode ForbidOther no found {0}" , path);
                MengBanController.Instance.RemoveCollider();
                MengBanController.Instance.Hide();
                MengBanController.Instance.FingerJudgeStr = null;
				MengBanController.Instance.RestoreFinger();
				GuideNodeManager.FuncOpenGuideId = string.Empty;
                DispatchExecuteReceipt(NodeMessageManager.ForbidOther, false);
                GuideNodeManager.IsVirtualBtnGuide = false;
                GuideNodeManager.currentGuideId = -1;
                return;
            }
            ConsecutiveClickCoolTrigger ccct = viewForbidOtherTarget.GetComponent<ConsecutiveClickCoolTrigger>();
            if (ccct != null && ccct.enabled)
            {
                MengBanController.Instance.RemoveCollider();//移除不需要的事件
                ccct.clickEvent.Add(delForbidOtherFinish);
                MengBanController.Instance.SetMiddle(viewForbidOtherTarget.GetComponent<UIWidget>(), 0, "");
            }
            else if (viewForbidOtherTarget.GetComponent<UIButton>() != null)
            {
                MengBanController.Instance.RemoveCollider();//移除不需要的事件
                viewForbidOtherTarget.GetComponent<UIButton>().onClick.Add(delForbidOtherFinish);
                MengBanController.Instance.SetMiddle(viewForbidOtherTarget.GetComponent<UIWidget>(), 0, "");
            }
            else if (viewForbidOtherTarget.GetComponent<UIEventTrigger>() != null)
            {
                MengBanController.Instance.RemoveCollider();//移除不需要的事件
                UIEventTrigger et = viewForbidOtherTarget.GetComponent<UIEventTrigger>();
                ProcessUIEventTrigger(et);
                MengBanController.Instance.SetMiddle(viewForbidOtherTarget.GetComponent<UIWidget>(), 0, "");
            }
            else if (viewForbidOtherTarget.GetComponent<UIEventListener>() != null)
            {
                MengBanController.Instance.RemoveCollider();//移除不需要的事件
                viewForbidOtherTarget.GetComponent<UIEventListener>().onClick += OnClickForbidOtherFinish;
				
				MengBanController.Instance.SetMiddle(viewForbidOtherTarget.GetComponent<UIWidget>(), 0, "");
            }
            else if (viewForbidOtherTarget.GetComponent <MoveController>() != null && viewForbidOtherTarget.GetComponent<Hotfix_LT.Combat.Combatant>().Data.Hp > 0)
            {
                if (viewForbidOtherTarget != null)
                {
                    Vector2 vector2 = Camera.main.WorldToScreenPoint(new Vector3(viewForbidOtherTarget.transform.Find("HealthBarTarget").position.x, viewForbidOtherTarget.transform.Find("HealthBarTarget").position.y / 2, viewForbidOtherTarget.transform.Find("HealthBarTarget").position.z));//position);
                    if (SceneLogic.BattleType == eBattleType.FirstBattle)
                    {
                        GuideNodeManager.VirtualBtnStr = "4";
                        GuideToolController.Instance.SetVirtualBtn(vector2,600,700);
                    }
                    else
                    {
                        GuideNodeManager.VirtualBtnStr = viewForbidOtherTarget.transform.parent.name;
                        GuideToolController.Instance.SetVirtualBtn(vector2);
                    }
                    viewForbidOtherTarget = GuideToolController.Instance.m_VirtualBtn.gameObject;
                    MengBanController.Instance.RemoveCollider();//移除不需要的事件
                    viewForbidOtherTarget.GetComponent<ConsecutiveClickCoolTrigger>().clickEvent.Add(delForbidOtherFinish);
                    MengBanController.Instance.SetMiddle(viewForbidOtherTarget.GetComponent<UIWidget>(), 0, "");
                }
                else
                {
                    GuideNodeManager.IsVirtualBtnGuide = false;
                    GuideNodeManager.currentGuideId = -1;
                    EB.Debug.LogError("Guide SetVirtualBtn not find {0}" , path);
                }
            }
            else
            {
                MengBanController.Instance.RemoveCollider();
				MengBanController.Instance.RestoreFinger();
				MengBanController.Instance.Hide();
                MengBanController.Instance.FingerJudgeStr = null;
                GuideNodeManager.FuncOpenGuideId = string.Empty;
                DispatchExecuteReceipt(NodeMessageManager.ForbidOther, false);
                GuideNodeManager.IsVirtualBtnGuide = false;
                GuideNodeManager.currentGuideId = -1;
            }
        }

        void TouchMengBan(string str)
        {
            if (string.IsNullOrEmpty(str))//全屏
            {
                MengBanController.Instance.mengbanHelper.Set(UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight, Screen.width / 2, Screen.height / 2, UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight);
                MengBanController.Instance.SetMiddle(Vector3.zero, new Vector2(UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight), 0, string.Empty, true);
            }
            else//暗幕
            {
                string[] split = str.Split(',');
                if (split.Length < 3)
                {
                    EB.Debug.LogError("TouchMengBan split.Length < 3");
                    DispatchExecuteReceipt(NodeMessageManager.TouchMengBan, false);
                }
                string path = split[0];
                string width = split[1];
                string height = split[2];
                GameObject Selectobj = GameObject.Find(split[0]);
                if (Selectobj == null)
                {
                    EB.Debug.LogError("split[0] Obj is null!");
                    DispatchExecuteReceipt(NodeMessageManager.TouchMengBan, false);
                }
                else
                {
                    Vector2 v2 = UICamera.mainCamera.WorldToScreenPoint(Selectobj.transform.position);
                    if ((float)Screen.width / Screen.height > (float)UIRoot.list[0].manualWidth / UIRoot.list[0].manualHeight)
                    {
                        float scaleWidth = (float)UIRoot.list[0].manualHeight * (float)Screen.width / Screen.height;
                        MengBanController.Instance.mengbanHelper.Set(scaleWidth, UIRoot.list[0].manualHeight, v2.x, v2.y, float.Parse(width), float.Parse(height));
                    }
                    else
                    {
                        float scaleHeight = (float)UIRoot.list[0].manualWidth * (float)Screen.height / Screen.width;
                        MengBanController.Instance.mengbanHelper.Set(UIRoot.list[0].manualWidth, scaleHeight, v2.x, v2.y, float.Parse(width), float.Parse(height));
                    }
                    MengBanController.Instance.SetMiddle(Vector3.zero, new Vector2(UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight), 0, string.Empty, true);
                }
            }

            /*{
                int x = int.Parse(split[0]);
                int y = int.Parse(split[1]);
                int width = int.Parse(split[2]);
                int height = int.Parse(split[3]);
                bool isShifting = false;
                int tipAnchor = 0;
                string content = "";
                if (split.Length == 5)
                {
                    isShifting = true;
                }
                if(split.Length==6)
                {
                    tipAnchor  = int.Parse(split[4]);
                    content = split[5]; 
                }
                float screenHeight = Screen.height;
                float screenWidth = Screen.width;
                float screenScaleH = 1.0f;
                float screenScaleW = 1.0f;
                float primalX = (float)x/ (float)UIRoot.list[0].manualWidth* screenHeight;
                float primalY = (float)y/ (float)UIRoot.list[0].manualHeight* screenWidth;
                x = x - ((UIRoot.list[0].manualWidth - width)>>1);    //换算中点的偏移
                y = -y + ((UIRoot.list[0].manualHeight - height)>>1); //换算中点的偏移
                if ((float)Screen.width / Screen.height < (float)UIRoot.list[0].manualWidth / UIRoot.list[0].manualHeight)
                {
                    if (!isShifting)
                    {
                        screenScaleW = (float)UIRoot.list[0].manualHeight / (float)UIRoot.list[0].manualWidth * (float)screenWidth / (float)screenHeight;
                        MengBanController.Instance.mengbanHelper.Set(UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight, primalX, primalY, width, height);
                    }
                    else
                    {
                        MengBanController.Instance.mengbanHelper.Set(UIRoot.list[0].manualWidth , UIRoot.list[0].manualWidth * screenHeight  / screenWidth, primalX, primalY, width, height);
                    }
                    MengBanController.Instance.SetMiddle(new Vector3(x* screenScaleW, y , 0), new Vector2(width* screenScaleW, height), tipAnchor, content, true);
                }
                else
                {
                    if (!isShifting)
                    {
                        screenScaleH = (float)UIRoot.list[0].manualWidth / (float)UIRoot.list[0].manualHeight * (float)screenHeight / (float)screenWidth;
                        MengBanController.Instance.mengbanHelper.Set(UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight, primalX, primalY, width, height);
                    }
                    else
                    {
                        MengBanController.Instance.mengbanHelper.Set(UIRoot.list[0].manualHeight * screenWidth/ screenHeight, UIRoot.list[0].manualHeight, primalX, primalY, width, height);
                    }
                    MengBanController.Instance.SetMiddle(new Vector3(x, y * screenScaleH, 0), new Vector2(width, height * screenScaleH), tipAnchor, content, true);
                }*/

            MengBanController.Instance.RemoveCollider();
            MengBanController.Instance.AddCollider(delTouchMengBanFinish);
        }

        void OnTouchMengBanFinish()
        {
            MengBanController.Instance.Hide();
            MengBanController.Instance.RemoveCollider(delTouchMengBanFinish);
            DispatchExecuteReceipt(NodeMessageManager.TouchMengBan, true);
            GuideNodeManager.IsVirtualBtnGuide = false;
            GuideNodeManager.currentGuideId = -1;
        }

        void Monolog(string str)
        {
            string[] split = str.Split(',');
            if (split.Length < 4)
            {
                EB.Debug.LogError("Monolog split.Length < 4 ");
                DispatchExecuteReceipt(NodeMessageManager.Monolog, false);
            }
            else
            {
                string content = EB.Localizer.GetString(split[0]);
                int x = int.Parse(split[1]);
                int y = int.Parse(split[2]);
                int width = int.Parse(split[3]);

                int moveIcon = 0;
                if (split.Length == 5)
                {
                    moveIcon = int.Parse(split[4]);
                }

                //文字处理 支持从Dialogue取文字
                int dialogueID = 0;
                int.TryParse(content, out dialogueID);
                if (dialogueID != 0)
                {
                    DialogueData buff = Hotfix_LT.Data.DialogueTemplateManager.Instance.GetDialogueData(dialogueID);
                    if (buff != null && buff.Steps.Length > 0)
                    {
                        string strText = buff.Steps[0].Context;
                        content = strText;
                    }
                }

                float screenHeight = Screen.height;
                float screenWidth = Screen.width;
                float screenScaleH = 1.0f;
                float screenScaleW = 1.0f;
                //int primalX = x;
                //int primalY = y;
                //已经修订为左上标点
                x = x - (UIRoot.list[0].manualWidth >> 1);
                y = -y + (UIRoot.list[0].manualHeight >> 1);
                moveIcon = moveIcon - (UIRoot.list[0].manualWidth >> 1);
                if (UIRoot.list[0].fitHeight)
                {
                    screenScaleW = (float)UIRoot.list[0].manualHeight / (float)UIRoot.list[0].manualWidth * screenWidth / screenHeight;
                    MengBanController.Instance.SetMonolog(x * screenScaleW, y, width, content);
                    if (split.Length == 5)
                    {
                        MengBanController.Instance.SetLogIcon(false);
                        MengBanController.Instance.SetBigIcon(moveIcon * screenScaleW);
                    }
                    else
                    {
                        MengBanController.Instance.SetLogIcon(true);
                    }
                }
                else
                {
                    screenScaleH = (float)UIRoot.list[0].manualWidth / (float)UIRoot.list[0].manualHeight * screenHeight / screenWidth;
                    MengBanController.Instance.SetMonolog(x, y * screenScaleH, width, content);
                    if (split.Length == 5)
                    {
                        MengBanController.Instance.SetLogIcon(false);
                        MengBanController.Instance.SetBigIcon(moveIcon);
                    }
                    else
                    {
                        MengBanController.Instance.SetLogIcon(true);
                    }
                }
                MengBanController.Instance.mengbanHelper.Set(UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight, Screen.width / 2, Screen.height / 2, UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight);
                MengBanController.Instance.SetMiddle(Vector3.zero, new Vector2(UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight), 0, string.Empty, true);
                OnMonologAudio();

                MengBanController.Instance.RemoveCollider();
                MengBanController.Instance.AddCollider(delMonologHide);
                DispatchExecuteReceipt(NodeMessageManager.Monolog, true);
            }
        }

        private string GDEid = null;
        private int AudioTimer = 0;
        void OnMonologAudio()
        {
            if (GDEid != null)
            {
                string oldAudioEventName = Hotfix_LT.Data.GuideAudioTemplateManager.Instance.GetGDEAudio(GDEid);
                if (oldAudioEventName != null)
                {
                    FusionAudio.PostEvent(oldAudioEventName, false);
                    GDEid = null;
                }
            }
            GDEid = GuideNodeManager.GetInstance().CurrentStepID().ToString();// _groupId.ToString();
            string audioEventName = Hotfix_LT.Data.GuideAudioTemplateManager.Instance.GetGDEAudio(GDEid);
            if (audioEventName != null)
            {
                AudioTimer = EB.Time.Now;
                if (!DialoguePlayUtil.Instance.State)
                {
                    FusionAudio.PostEvent(audioEventName, true);
                }
                else
                {
                    SetMonologAudio(false);
                }
            }
        }

        bool isResume = false;
        void SetMonologAudio(bool value)
        {
            if (value)
            {
                if (!isResume)
                {
                    return;
                }
            }
            else
            {
                if (AudioTimer< EB.Time.Now - 1)
                {
                    return;
                }
            }
            string audioEventName = Hotfix_LT.Data.GuideAudioTemplateManager.Instance.GetGDEAudio(GDEid);
            if (audioEventName != null)
            {
                isResume = !value;
                FusionAudio.PostEvent(audioEventName, value);
            }
        }

        void OnMonologHide()
        {
            MengBanController.Instance.Hide();
            MengBanController.Instance.RemoveCollider(delMonologHide);
            GuideNodeManager.IsVirtualBtnGuide = false;
            GuideNodeManager.currentGuideId = -1;

        }

        private float _endtime;
        private Coroutine _cWaitToEnd;
        private bool _isShowProgress;
        private bool _useMengban;
        void WaitTime(string str)
        {
            if (_cWaitToEnd != null)
            {
                EB.Coroutines.Stop(_cWaitToEnd);
            }
            string[] Splits = str.Split(',');
            _isShowProgress = false;
            _endtime = float.Parse(Splits[0]);
            _useMengban = true;
            if (Splits.Length > 1) _useMengban = false;
            _cWaitToEnd = EB.Coroutines.Run(WaitToEnd());
        }

        IEnumerator WaitToEnd()
        {
            float time = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - time < _endtime)
            {
                if (_isShowProgress && _useMengban)
                {
                    MengBanController.Instance.SetProgress(_endtime, _endtime - (Time.realtimeSinceStartup - time));
                }
                yield return null;
            }
            if (_useMengban)
            {
                MengBanController.Instance.SetProgress(_endtime, 0);
                MengBanController.Instance.Hide();
            }
            DispatchExecuteReceipt(NodeMessageManager.WaitTime, true);
        }

        private int _clickForceForbidNum;
        void ForceForbid(string str)
        {
            if (NodeMessageManager.Sucess.Equals(str))
            {
                _clickForceForbidNum = 0;
                //打开透明禁止面板
                MengBanController.Instance.mengbanHelper.Set(UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight, Screen.width / 2, Screen.height / 2, UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight);
                MengBanController.Instance.SetMiddle(Vector3.zero, new Vector2(UIRoot.list[0].manualWidth, UIRoot.list[0].manualHeight), 0, "", true);

                MengBanController.Instance.RemoveCollider();
                MengBanController.Instance.AddCollider(delClickForceForbid);
            }
            else
            {
                MengBanController.Instance.Hide();
            }
            DispatchExecuteReceipt(NodeMessageManager.ForceForbid, true);
        }

        private void ClickForceForbid()
        {
            if (_clickForceForbidNum > 10)
            {
                try
                {
                    MessageTemplateManager.ShowMessage(901099, null, delegate (int result)
                    {
                        if (result == 0)
                        {
                            CloseGuideNode();
                        }
                    });
                }
                catch
                {
                    CloseGuideNode();
                }
            }
            GuideNodeManager.IsVirtualBtnGuide = false;
            GuideNodeManager.currentGuideId = -1;
        }

        void CloseGuideNode()
        {
            MengBanController.Instance.UnFobiddenAll();
            MengBanController.Instance.Hide();
            GuideNodeManager.GetInstance().MainClose();
            GuideNodeManager.IsGuide = false;
        }

        void OnExecuteJump(string em)
        {
            JumpStep(em);
        }

        private void JumpStep(string em)
        {
            if (viewForbidOtherTarget != null)
            {
                ConsecutiveClickCoolTrigger ccct = viewForbidOtherTarget.GetComponent<ConsecutiveClickCoolTrigger>();
                if (ccct != null && ccct.enabled)
                {
                    RemoveFinishEvent(ccct.clickEvent);
                }
                else if (viewForbidOtherTarget.GetComponent<UIButton>() != null)
                {
                    RemoveFinishEvent(viewForbidOtherTarget.GetComponent<UIButton>().onClick);
                }
                else if (viewForbidOtherTarget.GetComponent<UIEventTrigger>() != null)
                {
                    UIEventTrigger et = viewForbidOtherTarget.GetComponent<UIEventTrigger>();

                    if (et.onClick != null && et.onClick.Contains(delForbidOtherFinish))
                    {
                        RemoveFinishEvent(et.onClick);
                    }
                    else if (et.onDragStart != null && et.onDragStart.Contains(delForbidOtherFinish))
                    {
                        RemoveFinishEvent(et.onDragStart);
                    }
                    else if (et.onDragEnd != null && et.onDragEnd.Contains(delForbidOtherFinish))
                    {
                        RemoveFinishEvent(et.onDragEnd);
                    }
                    else if (et.onDragOut != null && et.onDragOut.Contains(delForbidOtherFinish))
                    {
                        RemoveFinishEvent(et.onDragOut);
                    }
                    else if (et.onDragOver != null && et.onDragOver.Contains(delForbidOtherFinish))
                    {
                        RemoveFinishEvent(et.onDragOver);
                    }
                    else if (et.onDrag != null && et.onDrag.Contains(delForbidOtherFinish))
                    {
                        RemoveFinishEvent(et.onDrag);
                    }
                }
                else if (viewForbidOtherTarget.GetComponent<UIEventListener>() != null)
                {
                    UIEventListener el = viewForbidOtherTarget.GetComponent<UIEventListener>();
                    if (el.onClick != null) el.onClick -= OnClickForbidOtherFinish;
                }
            }
            viewForbidOtherTarget = null;
            MengBanController.Instance.RemoveCollider();
            MengBanController.Instance.Hide();
            GuideToolController.Instance.Hide();
            Time.timeScale = 1;
            DispatchExecuteReceipt(em, false); //发送执行失败
        }

        private void SetGuideType(string str)
        {
            int id = int.Parse(str);
            if (id > 100) GuideNodeManager.partnerStatID = id;
            if (id == 99)//设置成第一个ssr
            {
                var partners = LTPartnerDataManager.Instance.GetPartnerListByGrade((int)PartnerGrade.SSR);
                if (partners != null && partners.Count > 0)
                {
                    GuideNodeManager.partnerStatID = partners[0].StatId;
                }
            }
            GuideNodeManager.IsGuide = (id >= 1);
            DispatchExecuteReceipt(NodeMessageManager.SetGuideType, true);
        }

        private void SetLockCamera(string str)
        {
            int isInt = int.Parse(str);
            bool isLock = isInt == 1;
            MyFollowCamera.Instance.isActive = !isLock;
            DispatchExecuteReceipt(NodeMessageManager.SetLockCamera, true);
        }

        private void ClickButton(string path)
        {
            GameObject btnGO = GameObject.Find(path);
            if (btnGO != null)
            {
                ConsecutiveClickCoolTrigger ccct = btnGO.GetComponent<ConsecutiveClickCoolTrigger>();
                if (ccct != null)
                {
                    EventDelegate.Execute(ccct.clickEvent);
                    DispatchExecuteReceipt(NodeMessageManager.ClickButton, true);
                    return;
                }
                UIButton btn = btnGO.GetComponent<UIButton>();
                if (btn != null)
                {
                    btn.OnClickAction();
                    DispatchExecuteReceipt(NodeMessageManager.ClickButton, true);
                    return;
                }
            }
            DispatchExecuteReceipt(NodeMessageManager.ClickButton, false);

        }

        private void OpenObject(string path)
        {
            GameObject go = GameObject.Find(path);
            if (go != null)
            {
                go.SetActive(true);
            }
            else
            {

                int rootEndIndex = path.IndexOf('/');
                string rootName = path.Substring(0, rootEndIndex);
                string otherPath = path.Substring(rootEndIndex + 1);
                GameObject root = GameObject.Find(rootName);
                if (root != null)
                {
                    Transform target = root.transform.Find(otherPath);
                    if (target != null)
                    {
                        target.gameObject.SetActive(true);
                    }
                }
            }

            DispatchExecuteReceipt(NodeMessageManager.OpenObject, true); //开启莫对象不阻塞
        }

        private void CloseObject(string path)
        {
            GameObject go = GameObject.Find(path);
            if (go != null)
            {
                go.SetActive(false);
            }
            else
            {
                int rootEndIndex = path.IndexOf('/');
                string rootName = path.Substring(0, rootEndIndex);
                string otherPath = path.Substring(rootEndIndex + 1);
                GameObject root = GameObject.Find(rootName);
                if (root != null)
                {
                    Transform target = root.transform.Find(otherPath);
                    if (target != null)
                    {
                        target.gameObject.SetActive(false);
                    }
                }
            }

            DispatchExecuteReceipt(NodeMessageManager.CloseObject, true);//关闭莫对象不阻塞
        }

        private void MoveTips(string str)
        {
            string[] split = str.Split(',');

            if (split.Length == 0)
            {
                EB.Debug.LogError("MoveTips split.Length = 0 ");
                DispatchExecuteReceipt(NodeMessageManager.MoveTips, false);
                return;
            }
            int isInt = int.Parse(split[0]);
            bool isOpen = isInt == 1;
            if (isOpen)
            {
                int index = int.Parse(split[1]);
                GuideToolController.Instance.SetLTMoveTips(isOpen, index);
                DispatchExecuteReceipt(NodeMessageManager.MoveTips, true);
            }
            else
            {
                GuideToolController.Instance.SetLTMoveTips(isOpen);
                DispatchExecuteReceipt(NodeMessageManager.MoveTips, true);
            }
        }

        private void SetNodeCompleted(string str)
        {
            int nodeId = int.Parse(str);
            GuideNode node = GuideNodeManager.GetInstance().GetNode(nodeId);
            if (node != null)
            {
                GuideNodeManager.GetInstance().SetLinkCompleted(node, true);
            }
            else
            {
                EB.Debug.LogError("SetNodeCompleted no find id={0}" , str);
            }
            DispatchExecuteReceipt(NodeMessageManager.SetNodeCompleted, true); //不阻塞
        }

        private void SetNodeOnceUndone(string str)
        {
            int nodeId = int.Parse(str);
            GuideNode node = GuideNodeManager.GetInstance().GetNode(nodeId);
            if (node != null)
            {
                GuideNodeManager.GetInstance().SetLinkOnceUndone(node, true, true);
            }
            else
            {
                EB.Debug.LogError("SetNodeOnceUndone no find id={0}" , str);
            }
            DispatchExecuteReceipt(NodeMessageManager.SetNodeOnceUndone, true); //不阻塞
        }

        public void ShowDragUI(string str)
        {
            string[] split = str.Split(',');
            bool isOpen = (int.Parse(split[0]) == 1);
            if (split.Length <= 1)
            {
                GuideToolController.Instance.SetLTDargGuide(isOpen);
                DispatchExecuteReceipt(NodeMessageManager.ShowDragUI, true);
                return;
            }
            else if (split.Length >= 3)
            {
                string target = split[1];
                string text = EB.Localizer.GetString(split[2]);
                int TW = int.Parse(split[3]);
                GuideToolController.Instance.SetLTDargGuide(isOpen, target, text, TW);
                OnMonologAudio();
                DispatchExecuteReceipt(NodeMessageManager.ShowDragUI, true);
                return;
            }
            DispatchExecuteReceipt(NodeMessageManager.ShowDragUI, false);
        }

        public void SetVirtualBtn(string path)
        {
            GameObject go = GameObject.Find(path);
            if (go != null)
            {
                Vector2 vector2 = Camera.main.WorldToScreenPoint(go.transform.position);
                GuideToolController.Instance.SetVirtualBtn(vector2);
            }
            else
            {
                EB.Debug.LogError("Guide SetVirtualBtn not find {0}" , path);
            }

            DispatchExecuteReceipt(NodeMessageManager.SetVirtualBtn, true);
        }

        public void SetComponentAction(string str)
        {
            string[] split = str.Split(',');
            if (split.Length > 2)
            {
                bool isAction = (int.Parse(split[0]) == 1);
                string ComponentName = split[1];
                string path = split[2];
                GameObject go = GameObject.Find(path);
                if (go != null)
                {
                    switch (ComponentName)
                    {
                        case "box":
                            {
                                BoxCollider box = go.GetComponent<BoxCollider>();
                                if (box != null)
                                {
                                    box.enabled = isAction;
                                }
                            }
                            break;
                        default:
                            {
                                EB.Debug.LogError("SetComponentAction can't find ComponentName——{0}" , ComponentName);
                            }
                            break;
                    }
                    DispatchExecuteReceipt(NodeMessageManager.SetComponentAction, true);
                    return;
                }
            }
            DispatchExecuteReceipt(NodeMessageManager.SetComponentAction, false);
        }

        public void SetFirstBattleValue(string str)
        {
            if (str != null)
            {
                GuideNodeManager.VirtualBtnStr = str;
                DispatchExecuteReceipt(NodeMessageManager.SetFirstBattleValue, true);
            }
            else DispatchExecuteReceipt(NodeMessageManager.SetFirstBattleValue, false);
        }

        public void SetBackToMianMenu(string str)
        {
            if (str != null)
            {
                GlobalMenuManager.Instance.ClearCache();
            }
            else
            {
                GuideNodeManager.isFuncOpenGuide = true;
                if (SceneLogicManager.isLCCampaign())
                {
                    if(BattleResultScreenController.Instance != null)
                    {
                        if (LTInstanceMapModel.Instance.NotMainChapterId()) return;
                        LTInstanceMapModel.Instance.ClearInstanceData();
                        LTInstanceMapModel.Instance.RequestLeaveChapter("main", null);
                    }
                }
            }

            DispatchExecuteReceipt(NodeMessageManager.SetBackToMianMenu, true);
        }

        private int _timerSeq = -1;

        /// <summary>
        /// 返回主城
        /// </summary>
        /// <param name="str">时间（秒）</param>
        public void ReturnToMianMenu(string str)
        {
            GlobalMenuManager.Instance.ClearCache();
            
            if (SceneLogicManager.isLCCampaign())
            {
                if (BattleResultScreenController.Instance != null)
                {
                    LTInstanceMapModel.Instance.ClearInstanceData();
                    LTInstanceMapModel.Instance.RequestLeaveChapter("main", null);
                }
                else if (LTInstanceMapModel.Instance.IsChallengeEntering())
                {
                    LTInstanceMapModel.Instance.RequestLeaveChapter("", delegate
                    {
                        if (LTInstanceMapModel.Instance.IsInsatnceViewAction())
                        {
                            LTInstanceMapModel.Instance.SwitchViewAction(false, true, delegate
                            {
                                LTInstanceMapModel.Instance.ClearInstanceData();
                            });
                        }
                    });
                }
            }

            if (BattleResultScreenController.Instance != null)
            {
                BattleResultScreenController.Instance.SafeContinue();
            }

            DispatchExecuteReceipt(NodeMessageManager.ReturnToMianMenu, true);

            TimerManager.instance.RemoveTimer(_timerSeq);
            float second;

            if (!float.TryParse(str, out second))
            {
                second = 1f;
            }

            _timerSeq = TimerManager.instance.AddTimer(Mathf.CeilToInt(second * 1000), 1, seq => GlobalMenuManager.Instance.ComebackToMianMenu());
        }

        public void SetFuncOpen(string str)
        {
            if (str != null)
            {
                string[] split = str.Split(',');
                FunctionOpenController.Instance.ResetIndex();
                if (split.Length == 1)
                {
                    FunctionOpenController.Instance.Show(int.Parse(str), delegate
                    {
                        DispatchExecuteReceipt(NodeMessageManager.SetFuncOpen, true);
                    });
                }
                else
                {
                    FunctionOpenController.Instance.Show(int.Parse(split[0]), int.Parse(split[1]), delegate
                    {
                        DispatchExecuteReceipt(NodeMessageManager.SetFuncOpen, true);
                    });
                }
            }
            else DispatchExecuteReceipt(NodeMessageManager.SetFuncOpen, false);
        }

        public void SetSpecialCam(string str)
        {
            GuideToolController.Instance.SetSpecialCam(delegate
            {
                DispatchExecuteReceipt(NodeMessageManager.SetSpecialCam, true);
            });
        }

        public void SetCompletedByTargetNode(string str)
        {
            int nodeId = int.Parse(str);
            GuideNode node = GuideNodeManager.GetInstance().GetNode(nodeId);
            if (node != null)
            {
                for (int i = 0; i < GuideNodeManager.GetInstance().listStartNode.Count; i++)
                {
                    if (GuideNodeManager.GetInstance().listStartNode[i].GroupID == node.GroupID && GuideNodeManager.GetInstance().listStartNode[i].StepID < node.StepID)
                    {
                        GuideNodeManager.GetInstance().SetLinkCompleted(GuideNodeManager.GetInstance().listStartNode[i], true, false);
                    }
                }
                GuideNodeManager.GetInstance().SetLinkCompleted(node, true);
            }
            else
            {
                EB.Debug.LogError("SetNodeCompleted no find id={0}" , str);
            }
            DispatchExecuteReceipt(NodeMessageManager.SetCompletedByTargetNode, true);
        }

        public void SetNewChapterOpen(string str)
        {
            if (str == "Delay")
            {
                LTInstanceMapModel.Instance.SetCampOver(true);
                DispatchExecuteReceipt(NodeMessageManager.SetNewChapterOpen, true);
            }
            else
            {
                InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
                System.Action callBack = delegate
                {
                    InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
                    DispatchExecuteReceipt(NodeMessageManager.SetNewChapterOpen, true);
                };
                GlobalMenuManager.Instance.Open("LTInstanceNewChapterView", callBack);
            }
        }

        public void SetCombatBtnMod(string str)
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 1f);
            if (GuideNodeEvent.CombatBtnEvent != null)
            {
                GuideNodeEvent.CombatBtnEvent(str);
            }
            DispatchExecuteReceipt(NodeMessageManager.SetCombatBtnMod, true);
        }

        /// <summary>
        /// 在伙伴界面选择指定伙伴
        /// </summary>
        /// <param name="statId"></param>
        public void SelectPartnerInPartnerView(string statId)
        {
            if (int.TryParse(statId, out int id) && id > 0)
            {
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSelect, id);
                DispatchExecuteReceipt(NodeMessageManager.SelectPartnerInPartnerView, true);
            }
            else
            {
                DispatchExecuteReceipt(NodeMessageManager.SelectPartnerInPartnerView, false);
            }
        }

        public void ForbidInstanceFloor(string str)
        {
            string[] data = str.Split(',');

            if (data.Length > 0)
            {
                viewForbidOtherTarget = GameObject.Find(data[0]);
            }
            if (viewForbidOtherTarget == null)
            {
                DispatchExecuteReceipt(NodeMessageManager.ForbidInstanceFloor, false);
                GuideNodeManager.currentGuideId = -1;
                return;
            }
            GuideNodeManager.currentGuideId = GuideNodeManager.GetInstance().CurrentGroupID();
            //Vector2 vector2 = Camera.main.WorldToScreenPoint(viewForbidOtherTarget.transform.position);
            LTInstanceNodeTemp temp= viewForbidOtherTarget.transform.parent.GetMonoILRComponent<LTInstanceNodeTemp>();
            System.Action newAction = delegate { if (temp != null) temp.OnFloorClick(); };
            GuideToolController.Instance.SetVirtualBtn(viewForbidOtherTarget.transform, 250,250, newAction);
            viewForbidOtherTarget = GuideToolController.Instance.m_VirtualBtn.gameObject;
            MengBanController.Instance.RemoveCollider();//移除不需要的事件
            viewForbidOtherTarget.GetComponent<ConsecutiveClickCoolTrigger>().clickEvent.Add(delForbidOtherFinish);
            MengBanController.Instance.SetMiddle(viewForbidOtherTarget.GetComponent<UIWidget>(), 0, "");
        }

        /// <summary>
        /// 在伙伴界面指定伙伴进行升级
        /// </summary>
        /// <param name="statId"></param>
        private void MakePartnerLevelUp(string statId)
        {
            if (LTPartnerDataManager.Instance != null)
            {
                var heroIds = LTPartnerDataManager.Instance.GetGoIntoBattleList();

                if (heroIds != null) {
                    int minLevel = 999;
                    LTPartnerData ltPartnerData = null;

                    for (var i = 0; i < heroIds.Count; i++) {
                        var partnerData = LTPartnerDataManager.Instance.GetPartnerByHeroId(heroIds[i]);

                        if (partnerData != null && partnerData.Level < minLevel) {
                            minLevel = partnerData.Level;
                            ltPartnerData = partnerData;
                        }
                    }

                    if (ltPartnerData != null) {
                        SelectPartnerInPartnerView(ltPartnerData.StatId.ToString());
                    }
                }
            }

            DispatchExecuteReceipt(NodeMessageManager.MakePartnerLevelUp, true);
        }

        /// <summary>
        /// 在伙伴界面指定伙伴进行一键穿戴
        /// </summary>
        /// <param name="str"></param>
        private void MakePartnerDressEquip(string str)
        {
            if (LTPartnerDataManager.Instance != null)
            {
                var heroIds = LTPartnerDataManager.Instance.GetGoIntoBattleList();

                if (heroIds != null) {
                    int min = 999;
                    LTPartnerData ltPartnerData = null;

                    for (var i = 0; i < heroIds.Count; i++) {
                        var partnerData = LTPartnerDataManager.Instance.GetPartnerByHeroId(heroIds[i]);

                        if (partnerData == null) {
                            EB.Debug.LogError("CommonExecuteParse.MakePartnerDressEquip -> partnerData is null. HeroId = " + heroIds[i]);
                            continue;
                        }

                        var equipCount = partnerData.EquipmentCount;

                        if (equipCount < min) {
                            min = equipCount;
                            ltPartnerData = partnerData;
                        }
                    }

                    if (ltPartnerData != null) {
                        SelectPartnerInPartnerView(ltPartnerData.StatId.ToString());
                    }
                }
            }

            DispatchExecuteReceipt(NodeMessageManager.MakePartnerDressEquip, true);
        }

        public void SetGuideFailState(string str)
        {
            GuideNodeManager.GuideFailState = str;
            DispatchExecuteReceipt(NodeMessageManager.SetGuideFailState, true);
        }
    }
}
