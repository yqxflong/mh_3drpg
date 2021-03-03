using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class UIStorePageController : DynamicMonoHotfix
    {
        public UIStoreGridScroll m_Scroll;
        public UIServerRequest m_GetDataRequest;
        public UIServerRequest m_RefreshRequest;
        public UIServerRequest m_BuyRequest;
        public string m_DataID;
        public string m_StoreType;
        public string m_StoreName;
        private string m_CurStoreGold
        {
            get
            {
                switch (m_StoreType)
                {
                    case "alliance": return "alliance-gold";
                    case "arena": return "arena-gold";
                    case "ladder": return "ladder-gold";
                    case "herobattle": return "hero-gold";
                    case "expedition": return "nation-gold";
                    case "honor_arena": return "honorarena-gold";
                    default: return "gold";
                }
            }
        }
        public int m_StoreId;
        public int m_Columns = 2;
        public UIButton RefreshBtnRoot;

        public UILabel RefreshCostLabel;
        public UILabel m_RefreshTimeLabel;
        public UISprite RefreshCostSprite;

        public UILabel NewCurrencyCommonLabel;
        public UISprite NewCurrencyCommonSprite;
        public GameObject RefreshFxObj;

        protected int m_RefreshCost = 0;
        protected string m_RefreshCostId = "";
        protected int m_LastRefreshTime = 0;
        protected int m_NextRefreshTime = 0;
        protected float m_Discount = 0;
        protected HashSet<string> m_resSet;

        private int refreshBlueCostCache;
        private UIControllerILR _controllerILR;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            _controllerILR = t.GetComponentInParent<UIControllerILR>();
            Transform store_root = _controllerILR.transform;
            m_Scroll = store_root.GetMonoILRComponent<UIStoreGridScroll>("Store/NewBlacksmithView/BlacksmithViews/Shared/SlotsContainer/Placeholder/Grid");
            m_GetDataRequest = t.GetComponent<UIServerRequest>("Extra/Get");
            m_RefreshRequest = t.GetComponent<UIServerRequest>("Extra/Refresh");
            m_BuyRequest = store_root.GetComponent<UIServerRequest>("Store/NewBlacksmithView/BlacksmithViews/Shared/Extra/Buy");

            RefreshBtnRoot = store_root.GetComponent<UIButton>("Store/BGs/Bottom/Button", false);
            RefreshCostLabel = store_root.GetComponent<UILabel>("Store/BGs/Bottom/Button/Cost/Diamand/Label", false);
            m_RefreshTimeLabel = store_root.GetComponent<UILabel>("Store/BGs/Bottom/RefreshTime", false);
            RefreshCostSprite = store_root.GetComponent<UISprite>("Store/BGs/Bottom/Button/Cost/Diamand/Sprite", false);
            NewCurrencyCommonLabel = store_root.GetComponent<UILabel>("UINormalFrameBG/NewCurrency/Table/1_Common/Label");
            NewCurrencyCommonSprite = store_root.GetComponent<UISprite>("UINormalFrameBG/NewCurrency/Table/1_Common/Icon");
            RefreshFxObj = store_root.FindEx("Store/NewBlacksmithView/BlacksmithViews/Shared/Fx").gameObject;

            UIServerRequestHotFix mysGetRequest = t.GetMonoILRComponent<UIServerRequestHotFix>("Extra/Get");
            mysGetRequest.response = OnRequestStoreData;
            t.GetComponent<UIServerRequest>("Extra/Get").onResponse.Add(new EventDelegate(mysGetRequest.mDMono, "OnFetchData"));

            UIServerRequestHotFix mysRefreshRequest = t.GetMonoILRComponent<UIServerRequestHotFix>("Extra/Refresh");
            mysRefreshRequest.response = OnRefresh;
            t.GetComponent<UIServerRequest>("Extra/Refresh").onResponse.Add(new EventDelegate(mysRefreshRequest.mDMono, "OnFetchData"));

            UIServerRequestHotFix buyRequest = t.parent.GetMonoILRComponent<UIServerRequestHotFix>("Shared/Extra/Buy");

            if (buyRequest.response == null)
            {
                buyRequest.response = OnBuy;
                t.parent.GetComponent<UIServerRequest>("Shared/Extra/Buy").onResponse.Add(new EventDelegate(buyRequest.mDMono, "OnFetchData"));
            }

            m_DataID = mDMono.StringParamList[0];
            m_StoreType = mDMono.StringParamList[1];
            m_StoreName = mDMono.StringParamList[2];
            m_StoreId = mDMono.IntParamList[0];
            m_Columns = mDMono.IntParamList[1];

            CronRefreshExcuter re = CreateShopRefresher();
            AutoRefreshingManager.Instance.AddCronRefreshExcuter(re);
        }

        public override void Start()
    	{
    		m_resSet = new HashSet<string>();
    		var tpl = Hotfix_LT.Data.ShopTemplateManager.Instance.GetShop(m_StoreId);
    		string resstr = tpl.shop_balance_type;
    		string [] resstrs =resstr.Split(',');
    		if(resstrs!=null)
    		{
    			for(int i=0;i<resstrs.Length;i++)
    			{
    				m_resSet.Add(resstrs[i]);
    			}
    		}
    		m_resSet.Add(tpl.refresh_balance_type);
    		if(NeedRefreshData()) RequestSoreData();
    	}
    
    	bool NeedRefreshData()
    	{
    		IDictionary data = null;
    		if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>(m_DataID, out data) || data == null) return true;
    		int nextfreshtime=0;
    		nextfreshtime=EB.Dot.Integer("nextRefreshTime", data, nextfreshtime);
    		if (EB.Time.Now >= nextfreshtime) return true;
    		return false;
    	}

        public override void OnEnable()
    	{
            Hotfix_LT.Messenger.AddListener<StoreItemData>(EventName.StoreBuyEvent, OnStoreBuyEvent);
    	}

        public override void OnDisable()
    	{
            Hotfix_LT.Messenger.RemoveListener<StoreItemData>(EventName.StoreBuyEvent, OnStoreBuyEvent);
    	}

        public void OnStoreBuyEvent(StoreItemData data)
        {
            if (data == null || (_controllerILR != null && !_controllerILR.Visibility))
    		{
    			return;
    		}
    
    		BuyClick(data);
    	}

        public override void OnDestroy()
    	{
    		AutoRefreshingManager.Instance.RemoveCronRefreshExcuter(m_DataID);
    		m_buytarget = null;
        }
    
    	/// <summary>
    	/// 请求数据
    	/// </summary>
    	public void RequestSoreData()
    	{
    		m_GetDataRequest.SendRequest();
    		LoadingSpinner.Show();
        }
        public void OnRequestStoreData(EB.Sparx.Response res)
    	{
    		LoadingSpinner.Hide();
    		if (res.sucessful)
    		{
    			//界面刷新由datalookup
    			//RefreshData();
    		}
    		else if (res.fatal)
    		{
    			SparxHub.Instance.FatalError(res.localizedError);
    		}
    		else
    		{
    			MessageTemplateManager.ShowMessage(901018, null, delegate (int result)
    			{
    				if (result == 0)
    				{
    					m_GetDataRequest.SendRequest();
    					LoadingSpinner.Show();
    				}
    			});
    		}
    	}
        /// <summary>
        /// 刷新按钮  
        /// </summary>
    	public void RefreshBtnClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
    
    		//提示刷新花费
    		if (BalanceResourceUtil.GetResValue(m_RefreshCostId) < m_RefreshCost)
    		{
    			if (m_RefreshCostId.Equals("hc"))
                {
                    BalanceResourceUtil.HcLessMessage();
                    return;
    			}
    			else
    			{
					var ht = Johny.HashtablePool.Claim();
					ht.Add("0", BalanceResourceUtil.GetResName(m_RefreshCostId));
    				MessageTemplateManager.ShowMessage(902022, ht, null);
					Johny.HashtablePool.Release(ht);
    				return;
    			}
    		}
    		if (m_RefreshCostId == "hc")
    		{
    			refreshBlueCostCache = m_RefreshCost;
    		}

			{
				var ht = Johny.HashtablePool.Claim();
				ht.Add("0", m_RefreshCost);
				ht.Add("1", BalanceResourceUtil.GetResName(m_RefreshCostId));
				MessageTemplateManager.ShowMessage(902020, ht, delegate (int result)
				{
					if (result == 0)
					{
						m_RefreshRequest.SendRequest();
						LoadingSpinner.Show();
					}
				});
				Johny.HashtablePool.Release(ht);
			}
        }
    
        private IEnumerator RefreshFxPlay()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 0.5f);
            float Delay =0.1f;
            for (var i = 0; i < m_Scroll.mDMono.transform.childCount; i++)
            {
                Transform child = m_Scroll.mDMono.transform.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    child.GetComponent<TweenScale >().delay = Delay;
                    child.GetComponent<TweenScale>().ResetToBeginning();
                    child.GetComponent<TweenScale>().PlayForward();
                    Delay += 0.1f;
                }
            }
            //RefreshFxObj.CustomSetActive(true);
            //yield return new WaitForSeconds(0.5f);
            //RefreshFxObj.CustomSetActive(false);
            yield break;
        }
    
    	public void OnRefresh(EB.Sparx.Response res)
    	{
    		LoadingSpinner.Hide();
    		if (res.sucessful)
    		{
    			//界面刷新由datalookup
    			if (m_RefreshCostId == "hc")
    			{
    				FusionTelemetry.PostBuy(((int)FusionTelemetry.UseHC.hc_resetstore).ToString(), 1, refreshBlueCostCache);
    			}
                ShowRefreshPrice();
                StartCoroutine(RefreshFxPlay());
            }
    		else if (res.fatal)
    		{
    			SparxHub.Instance.FatalError(res.localizedError);
    		}
    		else
    		{
    			MessageTemplateManager.ShowMessage(901018, null, delegate (int result)
    			{
    				if (result == 0)
    				{
    					m_RefreshRequest.SendRequest();
    					LoadingSpinner.Show();
    				}
    			});
    		}
            isRefresh = false;
    	}
    
    	private static StoreItemData m_buytarget=null;
    	/// <summary>
    	/// 购买  
    	/// </summary>
    	public  void BuyClick(StoreItemData target)
    	{
    		if (target.sell_out) return;
            int isCanBuyMessageId = 0;
            if (!GameItemUtil.GetItemIsCanBuy(target.id, target.type, out isCanBuyMessageId))
            {
                MessageTemplateManager.ShowMessage(isCanBuyMessageId);
                return;
            }
    
    		if (BalanceResourceUtil.GetResValue(target.cost_id) < target.cost)
    		{
    			if (target.cost_id.Equals("hc"))
                {
                    BalanceResourceUtil.HcLessMessage();
                    return;
    			}
    			else if(target.cost_id.Equals("gold"))
    			{
    				MessageTemplateManager.ShowMessage(901031, null, delegate (int result)
    				{
    					if (result == 0)
    					{
    						InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
    						GlobalMenuManager.Instance.Open("LTResourceShopUI");
    					}
    				});
    				return;
    			}
    			else
    			{
					var ht = Johny.HashtablePool.Claim();
					ht.Add("0", BalanceResourceUtil.GetResName(target.cost_id) );
    				MessageTemplateManager.ShowMessage(902022, ht, null);
					Johny.HashtablePool.Release(ht);
    				return;
    			}			
    		}
    		m_buytarget = target;
            m_BuyRequest.parameters[0].parameter = target.store_type;
            m_BuyRequest.parameters[1].parameter = target.buy_id.ToString();
            m_BuyRequest.parameters[2].parameter = m_buytarget.cost.ToString();
            m_BuyRequest.SendRequest();
            LoadingSpinner.Show();
        }
    
    	public void OnBuy(EB.Sparx.Response res)
    	{
    		LoadingSpinner.Hide();
    		if (res.sucessful)
    		{
    			//界面刷新由datalookup
    			if (m_buytarget != null)
    			{
                    if (m_buytarget.cost_id == "hc")
                    {
                    	FusionTelemetry.PostBuy(((int)FusionTelemetry.UseHC.hc_store).ToString(), 1, m_buytarget.cost);
                    }
                    //上传友盟，商店物品购买
                    string id= string.Format("shop{0}", m_buytarget.sid);
                    FusionTelemetry.PostEvent(id);
    
                    string colorname = LTItemInfoTool.GetInfo(m_buytarget.id, m_buytarget.type,true).name;
    				Hashtable data = Johny.HashtablePool.Claim();
    				data.Add("0", m_buytarget.num);
    				data.Add("1", colorname);
    				MessageTemplateManager.ShowMessage(901097, data, null);
                    if(LTPartnerEquipDataManager.Instance.isEquipUpItem(m_buytarget.id)) Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);//装备锻造液数量可能发生变化需要通知发送下
                }
                GlobalMenuManager.Instance.CloseMenu("LTStoreBuyUI");
    		}
    		else if (res.fatal)
    		{
    			SparxHub.Instance.FatalError(res.localizedError);
    		}
    		else
    		{
    			if("ID_STORE_DATA_OLD".Equals(res.error.ToString()))
    			{
    				MessageTemplateManager.ShowMessage(902063, null, delegate (int result)
    				{
    					if (result == 0)
    					{
    						RequestSoreData();
    					}
    				});
    			}
    			else
    			{
    				MessageTemplateManager.ShowMessage(901018, null, delegate (int result)
    				{
    					if (result == 0)
    					{
    						if (m_buytarget != null)
    						{
    							m_BuyRequest.parameters[0].parameter = m_buytarget.store_type;
    							m_BuyRequest.parameters[1].parameter = m_buytarget.buy_id.ToString();
    							m_BuyRequest.parameters[2].parameter = m_buytarget.cost.ToString();
    							m_BuyRequest.SendRequest();
    							LoadingSpinner.Show();
    						}
    					}
    				});
    			}
    		}
    	}
    
        private bool isRefresh = false;
        private IEnumerator RefreshTime(long time) {
            while (true) {
                long timeStamp = time - EB.Time.Now;
                if (timeStamp<0)
                {
                    if (!isRefresh)
                    {
                        isRefresh = true;
                        RequestSoreData();
                    }

                    if (m_RefreshTimeLabel != null)
                    {
                        m_RefreshTimeLabel.text = string.Empty;
                    }
                }
                else
                {
                    System.DateTime dt = EB.Time.FromPosixTime(timeStamp);
                    string timeStr = dt.ToString("HH:mm:ss");

                    if (m_RefreshTimeLabel != null)
                    {
                        m_RefreshTimeLabel.text = timeStr + EB.Localizer.GetString("ID_codefont_in_UIStorePageController_10956");
                    }
                }
                yield return null;
            }
        }
    
        private IEnumerator act;
        /// <summary>
        /// 获取数据刷新
        /// </summary>
        public virtual void RefreshData()
    	{
    		string title= EB.Localizer.GetString(m_StoreName);
    		IDictionary data = null;
    		if (DataLookupsCache.Instance.SearchDataByID<IDictionary>(m_DataID, out data) && data != null)
    		{
    			m_RefreshCost = EB.Dot.Integer("refreshCost.quantity", data, m_RefreshCost);
    			m_RefreshCostId = EB.Dot.String("refreshCost.data", data, m_RefreshCostId);
    			m_LastRefreshTime= EB.Dot.Integer("lastRefreshTime", data, m_LastRefreshTime);
                
    			int time= EB.Dot.Integer("nextRefreshTime", data, m_NextRefreshTime);
                
    		    m_NextRefreshTime = time;
    
                ShowRefreshPrice();
    
                int limit = EB.Dot.Integer("unlimited", data, 0);
                if (limit == 1) {
                    if (m_RefreshTimeLabel != null)
                    {
                        m_RefreshTimeLabel.gameObject.CustomSetActive(false);
                    }

                    if (RefreshBtnRoot != null)
                    {
                        RefreshBtnRoot.gameObject.CustomSetActive(false);
                    }
                }
                else {
                    if (m_RefreshTimeLabel != null)
                    {
                        m_RefreshTimeLabel.gameObject.CustomSetActive(true);
                    }

                    if (RefreshBtnRoot != null)
                    {
                        RefreshBtnRoot.gameObject.CustomSetActive(true);
                        RefreshBtnRoot.onClick.Clear();
                        RefreshBtnRoot.onClick.Add(new EventDelegate(RefreshBtnClick));
                    }
                }
    
                if (NewCurrencyCommonLabel!= null&&NewCurrencyCommonSprite != null) {
                    if (m_CurStoreGold == "gold")
                    {
                        NewCurrencyCommonLabel.transform.parent.gameObject.SetActive(false);
                    }
                    else
                    {
                        NewCurrencyCommonLabel.transform.parent.gameObject.SetActive(true);
                    }
                    NewCurrencyCommonLabel.text = BalanceResourceUtil.NumFormat( BalanceResourceUtil.GetResValue(m_CurStoreGold).ToString());
                    NewCurrencyCommonSprite.spriteName = BalanceResourceUtil.GetResSpriteName(m_CurStoreGold);
                }
                
                if (act != null)
                {
                    StopCoroutine(act);
                    act = null;
                }
                act = RefreshTime(m_NextRefreshTime);
                StartCoroutine(act);
    
    			List<StoreItemData> datas = new List<StoreItemData>();
    			ArrayList items = Hotfix_LT.EBCore.Dot.Array("itemList", data, null);
    
                float discount = 1;
                bool isGoldVIP = LTChargeManager.Instance.IsGoldVIP();
                if (!m_StoreType.Equals("bosschallenge1") && !m_StoreType.Equals("bosschallenge2") && !m_StoreType.Equals("bosschallenge3"))
                {
                    discount = VIPTemplateManager.Instance.GetVIPPercent(VIPPrivilegeKey.ShopDiscount);
                }
    
    			if (items != null && items.Count > 0)
    			{
    				for (int i = 0; i < items.Count; i++)
    				{
                        int sid = EB.Dot.Integer("id", items[i], 0);
                        string id = EB.Dot.String("redeems.data", items[i], "");
    					string type = EB.Dot.String("redeems.type", items[i], "");
    					int buy_num = EB.Dot.Integer("redeems.quantity", items[i], 0);
    					int have = GameItemUtil.GetItemAlreadyHave(id, type);
    					string cost_id = EB.Dot.String("spends.data", items[i], "");
    					int cost_num = EB.Dot.Integer("spends.quantity", items[i], 0);
    					int num = EB.Dot.Integer("num", items[i], 0);
                        float mdiscount = EB.Dot.Single("discount", items[i], 1);
                        bool sell_out = num > 0|| num==-1 ? false : true;
    					int weight = 1;

                        float result = cost_num * discount + 0.05f;
                        cost_num = (int)(result);

                        if (!string.IsNullOrEmpty(id)) {
                            StoreItemData itemdata = new StoreItemData(sid,id, type, buy_num, have, cost_id, cost_num, sell_out, weight, i, m_StoreType, mdiscount, num);
                            datas.Add(itemdata);
                        }
    				}
    				int left = datas.Count % m_Columns;
    				if(left>0)//补全
    				{
    					left = m_Columns - left;
    					for(int i=0;i<left;i++)
    					{
    						StoreItemData itemdata = new StoreItemData(0,"", "", 1, 1, "", 1, true, 1, i, m_StoreType,1);
    						datas.Add(itemdata);
    					}
    				}
    
    				m_Scroll.SetItemDatas(datas);
    			}
    			else EB.Debug.LogWarning( "{0}===no data items", m_DataID);
    
            }
    		else
    		{
    			EB.Debug.LogWarning("{0}===no data", m_DataID);
    		}
    	}
    
        private void ShowRefreshPrice() {
            if (RefreshCostLabel != null) 
            {
                RefreshCostLabel.text = m_RefreshCost.ToString();

                if (m_RefreshCost > BalanceResourceUtil.GetResValue(m_RefreshCostId))
                { 
                    RefreshCostLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor; 
                }
                else
                { 
                    RefreshCostLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
                }

                if (RefreshCostSprite != null)
                {
                    RefreshCostSprite.spriteName = BalanceResourceUtil.GetResSpriteName(m_RefreshCostId);
                }
            }
        }
    
    	CronRefreshExcuter CreateShopRefresher()
    	{
    		var tpl = Hotfix_LT.Data.ShopTemplateManager.Instance.GetShop(m_StoreId);
    		string time = "";
    		int i = 0;
    		if (tpl.refresh_time_1!=-1)
    		{
    			time = time + tpl.refresh_time_1;
    			i = 1;
    		}
    		if (tpl.refresh_time_2 != -1)
    		{
    			if(i==0)
    			{
    				time = time + tpl.refresh_time_2;
    				i = 1;
    			}
    			else time = time +","+ tpl.refresh_time_2; 
    		}
    
    		if (tpl.refresh_time_3 != -1)
    		{
    			if (i == 0)
    			{
    				time = time + tpl.refresh_time_3;
    				i = 1;
    			}
    			else time = time + "," + tpl.refresh_time_3;
    		}
    
    		if (tpl.refresh_time_4 != -1)
    		{
    			if (i == 0)
    			{
    				time = time + tpl.refresh_time_4;
    				i = 1;
    			}
    			else time = time + "," + tpl.refresh_time_4;
    		}
    		time = string.Format("0 {0} * * *", time);
    		CronRefreshExcuter re = new CronRefreshExcuter(m_DataID);
    		Hashtable data = Johny.HashtablePool.Claim();
    		data.Add("regular", time);
    		data.Add("url", "/shops/getShopInfo");
    		data.Add("time_path", "");
    		data.Add("level", 0);
			var ht = Johny.HashtablePool.Claim();
			ht.Add("shopType", tpl.shop_type);
    		data.Add("parameters", ht);
    		re.Init(data);
    		return re;
    	}
    }
}
