using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hotfix_LT.UI
{
    public class MailController : UIControllerHotfix
    {
        public override bool IsFullscreen() { return true; }

        public MailDynamicScroll MailListScroll;
        public List<LTShowItem> RewardItems;

        private MailItemData MailItemData;
        private bool mFirstOpen;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            MailListScroll = t.GetMonoILRComponent<MailDynamicScroll>("Content/MailList/Placeholder/Grid");

            var itemRoot = t.FindEx("Content/RightContentFrame/Content/Award/Item");
            RewardItems = new List<LTShowItem>();

            for (var i = 0; i < itemRoot.childCount; i++)
            {
                var item = itemRoot.GetChild(i).GetMonoILRComponent<LTShowItem>();
                RewardItems.Add(item);
            }

            controller.backButton = t.GetComponent<UIButton>("UINormalFrame/CancelBtn");

            controller.FindAndBindingBtnEvent(new List<string>(){ "Content/TestSend", "Content/OneKeyReceiveBtn",
				"Content/RightContentFrame/Content/Award/HasReceivedSprite", "Content/OneKeyDeleteBtn" }, new List<EventDelegate>(){
				new EventDelegate(OnTestSendClick), new EventDelegate(OnOneKeyClick), new EventDelegate(SingleDelete), new EventDelegate(OneKeyDelete)
			});

            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/RightContentFrame/Content/Award/ReceiveBtn").clickEvent.Add(new EventDelegate(OnReceiveBtnClick));
        }

    	public override IEnumerator OnAddToStack()
    	{
            mFirstOpen = true;
    
            MailBoxManager.Instance.MailList.DataUpdated = false;
            MailBoxManager.Instance.GetMailList();
            
            GameDataSparxManager.Instance.RegisterListener(MailBoxManager.ListDataId, OnMailListListener);
            yield return base.OnAddToStack();	    
        }
    
    	public override IEnumerator OnRemoveFromStack()
    	{
            GameDataSparxManager.Instance.UnRegisterListener(MailBoxManager.ListDataId, OnMailListListener);
            DestroySelf();
    		yield break;
    	}
    
    	public override void StartBootFlash()
    	{
			SetCurrentPanelAlpha(1);
			UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
    		for (int j = 0; j < tweeners.Length; ++j)
    		{
    			tweeners[j].tweenFactor = 0;
    			tweeners[j].PlayForward();
    		}
    	}
    
    	private void OnMailListListener(string path, INodeData data)
    	{
            MailList mails = data as MailList;

    		if (mails == null || !mails.DataUpdated)
    		{
    			return;
    		}

    		mails.DataUpdated = false;
    
    		if (mFirstOpen && mails.Mails != null)
    		{
    			for (int i = 0; i < mails.Mails.Count - 1; i++)
    			{
    				for (int j = 0; j < mails.Mails.Count - 1 - i; j++)
    				{
    					if (mails.Mails[j].Time < mails.Mails[j + 1].Time)
    					{
    						var bottle = mails.Mails[j];
    						mails.Mails[j] = mails.Mails[j + 1];
    						mails.Mails[j + 1] = bottle;
    					}
    				}
    			}
    			for (int i = 0; i < mails.Mails.Count - 1; i++)
    			{
    				for (int j = 0; j < mails.Mails.Count - 1 - i; j++)
    				{
    					if (mails.Mails[j].HasRead == true &&
    						mails.Mails[j + 1].HasRead == false)
    					{
    						var bottle = mails.Mails[j];
    						mails.Mails[j] = mails.Mails[j + 1];
    						mails.Mails[j + 1] = bottle;
    					}
    				}
    			}
    		}

            if (mails.Mails != null)
            {
                System.Array sourceArr = mails.Mails.ToArray();
                int sourceLength = sourceArr.Length;
                int copyLength = sourceLength > MailBoxManager.Instance.CapacityNum ? MailBoxManager.Instance.CapacityNum : sourceLength;
                MailItemData[] targetArr = new MailItemData[copyLength];
                System.Array.Copy(sourceArr, targetArr, copyLength);
                if (targetArr.Length > 0)
                {
                    //如果在邮件界面 服务器主动推送就不会刷新右侧界面的Bug
                    if (mFirstOpen || targetArr.Length == 1)
                    {
                        OnClickMailCell(targetArr[0]);
                    }

                    if (MailListScroll != null)
                    {
                        MailListScroll.SetItemDatas(targetArr);
                    }

                    LTUIUtil.SetText(controller.UiLabels["UnReadLabel"], mails.Mails.FindAll(m => !m.HasRead).Count.ToString());
                    LTUIUtil.SetText(controller.UiLabels["MailCapacityLabel"], string.Format("{0}/{1}", mails.Mails.Count, MailBoxManager.Instance.CapacityNum));
                    controller.GObjects["ContentFrame"].gameObject.SetActive(true);
                }
                else
                {
                    controller.GObjects["ContentFrame"].gameObject.SetActive(false);
                }
            }

    		bool HaveUnReceivedMail = false;

            if (MailBoxManager.Instance.MailList != null && MailBoxManager.Instance.MailList.Mails != null)
            {
                for (var i = 0; i < MailBoxManager.Instance.MailList.Mails.Count; i++)
                {
                    var mail = MailBoxManager.Instance.MailList.Mails[i];

                    if (!mail.HasReceived && mail.ItemCount > 0)
                    {
                        HaveUnReceivedMail = true;
                    }
                }
            }

    		controller.GObjects["OneKeyRec"].SetActive(HaveUnReceivedMail);
    		controller.GObjects["OneKeyDel"].SetActive(!HaveUnReceivedMail);
    		mFirstOpen = false;
    	}
    
    	public void OnClickMailCell(MailItemData mailData)
    	{
    		if (SelectMail(mailData))
    		{
    			MailBoxManager.Instance.MailList.DataUpdated = true;
    			GameDataSparxManager.Instance.SetDirty(MailBoxManager.ListDataId);
    		}
    	}
    
    	bool SelectMail(MailItemData mailData)
    	{
    		if (mailData.IsSelect)
    		{
    			return false;
    		}
    
    		MailBoxManager.Instance.MailList.Mails.ForEach(m => m.IsSelect = false);
    		mailData.IsSelect = true;
    		if (!mailData.HasRead)
    		{
    			MailBoxManager.Instance.HasRead(mailData.MailId);
    			mailData.HasRead = true;
    		}
    		SetMailContent(mailData);
    		return true;
    	}
    
    	public void OnOneKeyClick() {
            bool HaveUnReceivedMail = false;
    		//List<ShowItemData> awardDatas = new List<ShowItemData>();
    		for (var i = 0; i < MailBoxManager.Instance.MailList.Mails.Count; i++)
    		{
                var mail = MailBoxManager.Instance.MailList.Mails[i];

                if (!mail.HasReceived && mail.ItemCount > 0)
    			{
    				//mail.HasRead = true;
    				HaveUnReceivedMail = true;
    				//awardDatas.AddRange(mail.Rewards.ItemList);
    			}
    		}
    
    		if (!HaveUnReceivedMail)
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_MailController_4041"));
    		}
    		else
    		{
    			MailBoxManager.Instance.OneKeyReceive(delegate (Hashtable result)
    			{
    				if (result!=null)
    				{
    					MailBoxManager.Instance.GetMailList();
    					MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_MailController_4300"));
    					List<LTShowItemData> awardDatas = GameUtils.ParseAwardArr(Hotfix_LT.EBCore.Dot.Array("mailbox.claimResult",result,null));
    					SetMailContent(MailItemData);
    
                        //上传友盟获得钻石，系统
                        FusionTelemetry.ItemsUmengCurrency(awardDatas, "邮件获得");
    
                        if (awardDatas.Count>0) GlobalMenuManager.Instance.Open("LTShowRewardView", awardDatas);
    
    					if (MailItemData.ItemCount > 0)
    					{
    						controller.Transforms["ReceiveBtn"].gameObject.CustomSetActive(false);
    						controller.UiSprites["HasReceivedBtn"].gameObject.CustomSetActive(true);
    					}
    					else
    					{
    						controller.Transforms["ReceiveBtn"].gameObject.CustomSetActive(false);
    						controller.UiSprites["HasReceivedBtn"].gameObject.CustomSetActive(false);
    					}
    				}				
    			});
    		}
        }
    
    
    	public void OneKeyDelete()
    	{
    		MailBoxManager.Instance.OneKeyDelete(delegate (Hashtable result)
    		{
    			if (result!=null)
    			{
    				MailBoxManager.Instance.MailList.DataUpdated = true;
    				MailBoxManager.Instance.GetMailList();
    				GameDataSparxManager.Instance.SetDirty(MailBoxManager.ListDataId);
    			    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_DELETE_SUCCESS"));
                }				
    		});
    	}
    
    
    	public void SingleDelete()
    	{
    		MailBoxManager.Instance.SingleDelete(MailItemData.MailId, (result) =>
    		{
    			mFirstOpen = true;
    			MailBoxManager.Instance.MailList.Mails.ForEach(m => m.IsSelect = false);
    			MailBoxManager.Instance.MailList.DataUpdated = true;
    			MailBoxManager.Instance.GetMailList();
    			//	GameDataSparxManager.Instance.SetDirty(MailBoxManager.ListDataId);
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_DELETE_SUCCESS"));
    		});
    		
    	}

        private int ContainLeftBraceNum(string text)
        {
            var chars = text.ToCharArray();
            var len = chars.Length;
            var count = 0;

            for (var i = 0; i < len; i++)
            {
                if (chars[i].Equals('{'))
                {
                    count++;
                }
            }

            return count;
        }

        public void SetMailContent(MailItemData data)
    	{
    		MailItemData = data;
    		LTUIUtil.SetText(controller.UiLabels["Title"], EB.Localizer.GetString(MailItemData.Title));
    		string mailText = EB.Localizer.GetString(MailItemData.Text);
            int count = ContainLeftBraceNum(mailText);
            string[] args = data.TextParams;

            if (args.Length < count)
            {
                List<string> list = new List<string>(args);
                list.AddRange(new string[count - args.Length]);
                args = list.ToArray();
            }

    		LTUIUtil.SetText(controller.UiLabels["ContentLabel"], string.Format(mailText, args));
    		LTUIUtil.SetText(controller.UiLabels["ValidTime"],(MailItemData.ReMainTime / 86400).ToString() + EB.Localizer.GetString("ID_codefont_in_MailController_5921"));

            if (controller.UiScrollViews["ContentScrollView"] != null)
            {
                controller.UiScrollViews["ContentScrollView"].SetDragAmount(0, 0, false);

                var uiPanel = controller.UiScrollViews["ContentScrollView"].GetComponent<UIPanel>();

                if (uiPanel != null)
                {
                    controller.UiScrollViews["ContentScrollView"].enabled = controller.UiLabels["ContentLabel"].height > uiPanel.GetViewSize().y;
                }
            }

            var bc = controller.UiLabels["ContentLabel"].GetComponent<BoxCollider>();

            if (bc != null)
            {
                bc.size = new Vector3(controller.UiLabels["ContentLabel"].width, controller.UiLabels["ContentLabel"].height, 0);
                bc.center = new Vector3(0, -controller.UiLabels["ContentLabel"].height / 2, 0);
            }
            
            if (MailItemData.ItemCount > 0)
    		{
    			controller.UiWidgets["DescBg"].height = 305;
    			RewardItems[0].mDMono.transform.parent.gameObject.SetActive(true);
    			controller.GObjects["RewardBGRoot"].gameObject.SetActive(true);
    
    			int itemCount = MailItemData.ItemCount;
    			for (int i = 0; i < 5; ++i)
    			{
    				if (i >= itemCount)
    				{
    					RewardItems[i].mDMono.gameObject.SetActive(false);
    				}
    				else
    				{
                        var item = MailItemData.Rewards.GetItem(i);
                        RewardItems[i].LTItemData = new LTShowItemData(item.id, item.count, item.type, false);
                        RewardItems[i].mDMono.gameObject.SetActive(true);
    				}
    			}
    
    
    			controller.Transforms["ReceiveBtn"].gameObject.SetActive(!MailItemData.HasReceived);
    			controller.UiSprites["HasReceivedBtn"].gameObject.SetActive(MailItemData.HasReceived);
    		}
    		else
    		{
    			controller.UiWidgets["DescBg"].height = 450;
    			RewardItems[0].mDMono.transform.parent.gameObject.SetActive(false);
    			controller.GObjects["RewardBGRoot"].gameObject.SetActive(false);
    
    			controller.Transforms["ReceiveBtn"].gameObject.SetActive(false);
    			controller.UiSprites["HasReceivedBtn"].gameObject.SetActive(true);
    		}
    	}
    	
    	public void OnReceiveBtnClick()
    	{
            if (MailItemData == null)
            {
                EB.Debug.LogError("MailItemData is null");
                return;
            }

    		MailBoxManager.Instance.ReceiveGift(MailItemData.MailId, delegate(bool successful) {
                //string itemName = GameItemUtil.GetNameWithColor(MailItemData.Rewards.GetItem(i).id, MailItemData.Rewards.GetItem(i).type);
    
                if (successful)
                {
                    MailItemData.HasReceived = true;

                    if (controller != null && controller.Transforms.ContainsKey("ReceiveBtn"))
                    {
                        controller.Transforms["ReceiveBtn"].gameObject.SetActive(!MailItemData.HasReceived);
                    }

                    if (controller != null && controller.UiSprites.ContainsKey("HasReceivedBtn"))
                    {
                        controller.UiSprites["HasReceivedBtn"].gameObject.SetActive(MailItemData.HasReceived);
                    }

                    if (MailBoxManager.Instance.MailList != null)
                    {
                        MailBoxManager.Instance.MailList.DataUpdated = true;
                    }

                    GameDataSparxManager.Instance.SetDirty(MailBoxManager.ListDataId);

                    if (MailItemData.Rewards != null)
                    {
                        //上传友盟获得钻石，系统
                        FusionTelemetry.ItemsUmengCurrency(MailItemData.Rewards.ItemList, "邮件获得");

                        GlobalMenuManager.Instance.Open("LTShowRewardView", MailItemData.Rewards.ItemList);
                    }
                }
            });
    	}

        public void RightContentPlayAmi()
        {
            UITweener[] tweeners = controller.TweenScales["RightContentTween"].transform.GetComponents<UITweener>();
            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }
    
    	public void OnTestSendClick()
    	{
    		MailBoxManager.Instance.SendUserMail(AllianceUtil.GetLocalUid(), "测试邮件", "123456sssssssss");
    	}
    }
}
