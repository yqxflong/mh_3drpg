using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB.IAP;
using System;

namespace EB.Sparx
{	
	public class WalletConfig
	{
#if UNITY_ANDROID
		public interface ReceiptPersistanceInterface 
		{
			void AddPendingPurchaseReceipt(string receipt, string api);
			
			bool RemovePendingPurchaseReceipt(string receipt);
			
			Hashtable GetPendingPurchaseReceipts();
		}
		public ReceiptPersistanceInterface ReceiptPersistance = null;
#endif

		public WalletListener Listener = new DefaultWalletListener();
		public string  IAPPublicKey = string.Empty;
	}
	
	public class PayoutSale
	{
		public class PayoutBanner
		{
			public PayoutBanner( Hashtable data )
			{
				this.Title = string.Empty;
				this.SubTitle = string.Empty;
				this.Chevron = string.Empty;
				this.Image = string.Empty;
				
				if( data != null )
				{
					this.Title = EB.Dot.String( "title", data, string.Empty );
					this.SubTitle = EB.Dot.String( "subtitle", data, string.Empty );
					this.Chevron = EB.Dot.String( "chevron", data, string.Empty );
					this.Image = EB.Dot.String( "image", data, string.Empty );
				}
			}
			
			public override string ToString()
			{
				return string.Format("{0}:{1} Image:{2} Chevron:{3}", this.Title, this.SubTitle, this.Image, this.Chevron );
			}
			
			public string Title { get; private set; }
			public string SubTitle { get; private set; }
			public string Chevron { get; private set; }
			public string Image { get; private set; }
		}
	
		public PayoutSale( Hashtable data )
		{
			this.Title = string.Empty;
			this.Description = string.Empty;
			this.Chevron = string.Empty;
			this.Image = string.Empty;
			this.Flash = string.Empty;
			this.Colour = Color.white;
			this.Notification = string.Empty;
			this.EndTime = 0;
			
			if( data != null )
			{
				this.Title = EB.Dot.String( "title", data, string.Empty );
				this.Description = EB.Dot.String( "desc", data, string.Empty );
				this.Chevron = EB.Dot.String ("chevron", data, string.Empty );
				this.Image = EB.Dot.String( "image", data, string.Empty );
				this.Flash = EB.Dot.String( "flash", data, string.Empty );
				this.Colour = EB.Dot.Colour( "colour", data, Color.white );
				this.Notification = EB.Dot.String( "notification", data, string.Empty );
				this.Banner = new PayoutBanner( EB.Dot.Object( "banner", data, null ) );
				this.EndTime = EB.Dot.Integer( "endtime", data, 0 );
			}
			else
			{
				this.Banner = new PayoutBanner( null );
			}
		}
		
		public override string ToString()
		{
			return string.Format("{0}:{1} Chevron:{7} Image:{2} Flash:{3} Colour:{4} Notification:{5} Banner:{6}", this.Title, this.Description, this.Image, this.Flash, this.Colour, this.Notification, this.Banner, this.Chevron );
		}
		
		public string Title { get; private set; }
		public string Description { get; private set; }
		public string Chevron { get; private set; }
		public string Image { get; private set; }
		public string Flash { get; private set; }
		public Color Colour { get; private set; }
		public string Notification { get; private set; }
		public PayoutBanner Banner { get; private set; }
		public int EndTime { get; private set; }
		public int SecondsRemaining
		{
			get
			{
				int remaining = -1;
				if( this.EndTime > 0 )
				{
					remaining = this.EndTime - EB.Time.Now;
				}
				return remaining;
			}
		}
	}
	
	public class WalletManager : SubSystem, Updatable
	{
		WalletConfig			_config;
		WalletAPI				_api;
		EB.SafeInt				_balance;
		IAP.Manager				_iapManager;
		
		PayoutSale				_sale;
		List<RedeemerItem>		_bonusItems;
		List<IAP.Item> 			_payouts;
		List<IAP.Transaction>	_verify;
        List<IAP.SupplementTransaction> _VerifyAgain;
        int						_lastFetchTime;
		object					_fetchTimeout = null;
		int						_fetchWait = 1;
		
		string 					_externalId = string.Empty;//使用的地方需要自己加入平台后缀，处理多渠道支付问题
		bool					_enumerated = false;
		bool					_fetchBalance = false;

        private const string TransactionUrl = "WalletManagerTransactionIDStr";

        public int Balance { get { return _balance; } }
		
		public PayoutSale Sale { get { return _sale; } }
		public RedeemerItem[] BonusItems { get { return _bonusItems.ToArray(); } }
		public IAP.Item[] Payouts { get { return _payouts.ToArray(); } }
		
		public int LastFetchTime { get { return _lastFetchTime; } } 
		
		public bool UpdateOffline { get { return false;} }
		
		public bool PayoutsContainRedeemer(EB.Sparx.RedeemerItem candidate)
		{
			foreach(IAP.Item item in _payouts)
			{
				if(item.ContainsRedeemer(candidate))
				{
					return true;
				}
			}
			
			foreach(RedeemerItem item in _bonusItems)
			{
				if(item.IsSameItem(candidate))
				{
					return true;
				}
			}
			
			return false;
		}
		
		public override void Initialize (Config config)
		{
			_balance 	= 0;
			_api 		= new WalletAPI(Hub.ApiEndPoint);
			_config 	= config.WalletConfig;
			_bonusItems = new List<RedeemerItem>();
			_payouts	= new List<IAP.Item>();
			_verify 	= new List<IAP.Transaction>();
            _VerifyAgain = new List<SupplementTransaction>();
            _lastFetchTime = 0;
						
			if (_config.Listener == null )
			{
				throw new System.ArgumentNullException("Missing wallet listener");
			}
		}
		
		public override void Connect ()
		{
			State = SubSystemState.Connecting;

            isRequest = false;
            _lastFetchTime = 0;
			_sale = null;
			_bonusItems.Clear();
			_payouts.Clear();
			_verify.Clear();
            _VerifyAgain.Clear();

            if (_iapManager == null)
			{
				var iapConfig = new IAP.Config();
				iapConfig.OnEnumerate += OnEnumerate;
				iapConfig.Verify += OnVerify;
                iapConfig.VerifyAgain += OnVerifyAgain;

                iapConfig.OnPurchaseFailed += OnPurchaseFailed;
				iapConfig.OnPurchaseCanceled += OnPurchaseCanceled;
				iapConfig.PublicKey = _config.IAPPublicKey;
				iapConfig.OnPurchaseNoResult += OnPurchaseNoResult;
#if UNITY_ANDROID
				iapConfig.ReceiptPersistance = _config.ReceiptPersistance;				
#endif
				_iapManager = new IAP.Manager( iapConfig );
			}
			
			var wallet = Dot.Object("wallet", Hub.DataStore.LoginDataStore.LoginData, null);
			if ( wallet != null)
			{
				OnFetch(null, wallet);
			}
			else
			{
				Fetch();	
			}
			
			_fetchWait = Dot.Integer("offer_delay", Hub.DataStore.LoginDataStore.LoginData, _fetchWait);
			FetchOffersDelayed();
            
            RequestPayout();
        }

		public override void OnEnteredForeground() 
		{
			base.OnEnteredForeground();
			if (_fetchBalance)
			{
				_fetchBalance = false;
				this.Fetch();
			}
		}
		
		public void GetOfferUrl( string offerName, System.Action<string,string> callback )
		{
			_fetchBalance = true;
			_api.FetchOfferUrl(_iapManager.ProviderName, offerName, callback );
		}

        private int now = 0;
        public void Update ()
		{
            if (isRequest) return;
			
			#region 正常验单核心运转
            if (_verify.Count > 0)
            {
				EB.Debug.Log("【商城】_externalId: {0}===_enumerated: ", _externalId, _enumerated.ToString());
				if (!string.IsNullOrEmpty(_externalId) && _enumerated)
				{
					var transaction = _verify[0];
					_verify.RemoveAt(0);
                    EB.Debug.Log("【商城】申请发货:productId:{0};transactionId:{1}", transaction.productId, transaction.transactionId);
                    VerifyPayout(transaction);
				}
			}
			#endregion

			#region 验单核心运转
            if (_VerifyAgain.Count > 0)
            {
                if (now < EB.Time.Now)
                {
                    int now = EB.Time.Now;
                    for (int i = _VerifyAgain.Count - 1; i >= 0; --i)
                    {
                        if (_VerifyAgain[i].ChackRemove())
                        {
							EB.Debug.Log("【商城】申请补单:_VerifyAgain[i].ChackRemove()");
                            _VerifyAgain.RemoveAt(i);
                        }
                        else if (_VerifyAgain[i].ChackRetry())
                        {
                            var transaction = _VerifyAgain[i];
                            _VerifyAgain.RemoveAt(i);
                            transaction.retryCount++;
                            EB.Debug.Log("【商城】申请补单:productId:{0};transactionId:{1}; retryCount:{2}", transaction.productId, transaction.transactionId, transaction.retryCount);
                            VerifyPayoutAgain(transaction);
                            break;
                        }
                    }
                }
            }
			#endregion
        }

		///找到指定的订单号的订单从补单列表移除
        private void CheckSupplementTransaction(string id)
        {
            for (int i = _VerifyAgain.Count - 1; i >= 0; --i)
            {
                if (_VerifyAgain[i].transactionId.Equals(id))
                {
                    _VerifyAgain.RemoveAt(i);
                }
            }
        }
        
		///超过订单列表最大数量时，移除一个最早的订单
        private void RemoveSomeTransaction()
        {
            if (_VerifyAgain.Count < SupplementTransaction.PoolCount) return;
            int index = 0;
            int time = _VerifyAgain[0].crateTime;
            for (int i = 1; i<_VerifyAgain.Count; ++i)
            {
                if(time > _VerifyAgain[i].crateTime)
                {
                    index = i;
                    time = _VerifyAgain[i].crateTime;
                }
            }
            _VerifyAgain.RemoveAt(index);
        }

		#region About 正常验单
        private void VerifyPayout( IAP.Transaction transaction )
		{
			IAP.Item payout = _payouts.Find(delegate(IAP.Item obj) {
				return obj.productId == transaction.productId;
			});

            if (payout == null)
            {
                EB.Debug.Log("【商城】无法验证，找不到商品:productId:{0};transactionId:{1}", transaction.productId, transaction.transactionId);
                _verify.Add(transaction);
                return;
            }
            Hashtable data = Johny.HashtablePool.Claim();
			data["cents"] = payout.cents;
			data["currency"] = payout.currencyCode;
            data["externalTrkid"] = _externalId + (string.IsNullOrEmpty(transaction.IAPPlatform)?_iapManager.ProviderName:transaction.IAPPlatform);
            data["payoutid"] = payout.payoutId;
			data["platform"] = transaction.platform;

            if(!string.IsNullOrEmpty(transaction.limitedTimeGiftId))data["limitedTimeGiftId"] = transaction.limitedTimeGiftId;

            if (_iapManager.ProviderName == "itunes")
			{
				data["receipt-data"] = transaction.payload;
			}
			else
			{
				data["response-data"] = transaction.payload;
				data["response-signature"] = transaction.signature;
			}

            CheckSupplementTransaction(transaction.transactionId);
            EB.Debug.Log("【商城】校验:{0}", transaction.transactionId);
			isRequest = true;
            _api.VerifyPayout(_iapManager.ProviderName, data, delegate(string err, Hashtable res){
                EB.Debug.Log("【商城】校验回调:{0}", transaction.transactionId);
				isRequest = false;
                OnVerifyPayout(payout, transaction, err, res);
                if (chargeSuccessCallBack != null && res != null && string.IsNullOrEmpty(err))
                {
                    chargeSuccessCallBack(res);
                }
            });
		}
		///正常验单回调
        private void OnVerifyPayout( IAP.Item item, IAP.Transaction trans, string err, Hashtable data )
		{
			if (!string.IsNullOrEmpty(err))
			{
                EB.Debug.Log("【商城】订单校验有错误transactionId:{0}", trans.transactionId);
                if (EB.Dot.Bool("verify.retry", data, true))
				{
                    EB.Debug.Log("【商城】订单再次重新尝试校验transactionId:{0}", trans.transactionId);
                    int delay = (int)EB.Dot.Single("verify.delay", data, 3.0f) * 1000;
					EB.Coroutines.SetTimeout(delegate ()
					{
						if (!_verify.Contains(trans))
						{
							_verify.Add(trans);
						}
					}, delay);
				}
				else
				{
                    EB.Debug.Log("【商城】订单无效移除，并加进补单列表transactionId:{0};err:{1}", trans.transactionId, err);
					_config.Listener.OnOfferPurchaseFailed(err);
                    CheckSupplementTransaction(trans.transactionId);
                    RemoveSomeTransaction();
                    _VerifyAgain.Add(new SupplementTransaction(trans));
                    SparxTransactionHelper.DelTransInLocal(trans);
				}
				FetchOffersDelayed(); 
				return;
			}
            
			_lastFetchTime = 0;
			Fetch();
			FetchOffers();
			_iapManager.Complete(trans);

			_config.Listener.OnOfferPurchaseRedeemer(data);
			_config.Listener.OnOfferPurchaseSuceeded(item, trans);
            EB.Debug.Log("【商城】订单成功transactionId:{0}", trans.transactionId);
            SparxTransactionHelper.DelTransInLocal(trans);
            CheckSupplementTransaction(trans.transactionId);
        }
		#endregion

		#region About 补单验单
        private bool isRequest = false;
        private void VerifyPayoutAgain(IAP.SupplementTransaction transaction)
        {
            IAP.Item payout = _payouts.Find(delegate (IAP.Item obj) {
                return obj.productId == transaction.productId;
            });
            if (payout == null)
            {
                EB.Debug.LogError("Cant find payout!!");
                return;
            }
            Hashtable data = new Hashtable();
            data["cents"] = payout.cents;
            data["currency"] = payout.currencyCode;
            data["externalTrkid"] = _externalId + (string.IsNullOrEmpty(transaction.IAPPlatform) ? _iapManager.ProviderName : transaction.IAPPlatform);
            data["payoutid"] = payout.payoutId;
            data["platform"] = transaction.platform;
            if (!string.IsNullOrEmpty(transaction.limitedTimeGiftId)) data["limitedTimeGiftId"] = transaction.limitedTimeGiftId;
            if (_iapManager.ProviderName == "itunes")
            {
                data["receipt-data"] = transaction.payload;
            }
            else
            {
                data["response-data"] = transaction.payload;
                data["response-signature"] = transaction.signature;
            }

            EB.Debug.Log("【商城】再次校验:{0}", transaction.transactionId);
			isRequest = true;
            _api.VerifyPayout(_iapManager.ProviderName, data, delegate (string err, Hashtable res) {
                EB.Debug.Log("【商城】再次校验回调:{0}", transaction.transactionId);
                isRequest = false;
                OnVerifyPayoutAgain(payout, transaction, err, res);
                if (chargeSuccessCallBack != null && res != null && string.IsNullOrEmpty(err))
                {
                    chargeSuccessCallBack(res);
                }
            });
        }
		///补单验单回调
        private void OnVerifyPayoutAgain(IAP.Item item, IAP.SupplementTransaction trans, string err, Hashtable data)
        {
            if (!string.IsNullOrEmpty(err))
            {
                CheckSupplementTransaction(trans.transactionId);
                RemoveSomeTransaction();
                _VerifyAgain.Add(trans);
                return;
            }
            _lastFetchTime = 0;
            Fetch();
            FetchOffers();
            _iapManager.Complete(trans);
            _config.Listener.OnOfferPurchaseRedeemer(data);
            _config.Listener.OnOfferPurchaseSuceeded(item, trans);
        }
		#endregion

        public override void Disconnect (bool isLogout)
		{
			_balance = 0;
		}

		public override void Async (string message, object payload)
        {
            switch (message.ToLower())
			{
				case "sync":
					{
						Fetch();
					}
					break;
				case "offers":
					{
						FetchOffers();
					}
					break;
				case "payouts":
					{
						FetchPayouts();
					}
					break;
                case "pay_callback_msg":
                    {
                        string transactionId = EB.Dot.String("transactionId", payload, string.Empty);
                        _iapManager.OnPayCallbackFromServer(transactionId);
                    }
                    break;
			}
		}
		
		#region About 获取最新商品信息
		private void FetchOffersDelayed()
		{
			_enumerated = false;
			Coroutines.ClearTimeout(_fetchTimeout);
            _fetchTimeout = Coroutines.SetTimeout(FetchOffers, _fetchWait * 1000);
		}
		private void FetchOffers()
		{
			if (_iapManager == null)
			{
				_config.Listener.OnOffersFetched();
				return;
			}
			
			_lastFetchTime = Time.Now;
			FetchPayouts();
		}
		private void FetchPayouts()
		{
			if (_iapManager == null)
			{
				_config.Listener.OnOffersFetched();
				return;
			}
			
			_api.FetchPayouts(_iapManager.ProviderName, OnLoadPayouts);
		}
		public void FetchPayouts(Action<string, Hashtable> callback)
		{
			if (_iapManager == null)
			{
				return;
			}
			
			_api.FetchPayouts(_iapManager.ProviderName, (string err, Hashtable data) => {
				OnLoadPayouts(err, data);
				callback(err, data);
			});
		}
		
		void OnLoadPayouts(string err,Hashtable data)
		{
			if (!string.IsNullOrEmpty(err))
			{
				EB.Debug.LogError("获取商品失败:" + err);
				_config.Listener.OnOffersFetched();
				return;
			}
			
			_externalId = EB.Dot.String("externalTrkid", data, _externalId);
            int end=_externalId.LastIndexOf(':');
            _externalId=_externalId.Substring(0,end+1);//去掉平台数据，后面用到的自己去拼，处理多平台问题
            _sale = null;
			_bonusItems.Clear();
			_payouts.Clear();
			_enumerated = false;
			
			var currentSet = EB.Dot.Find<object>("data.payoutSets.0", data);
            if (currentSet != null)
			{
				var sale = EB.Dot.Object("sale", currentSet, null );
				if( sale != null )
				{
					_sale = new PayoutSale( sale );
				}
				
				var bonus = EB.Dot.Array( "redeemers", currentSet, new ArrayList() );
				foreach( object candidate in bonus )
				{
					Hashtable item = candidate as Hashtable;
					if( item != null )
					{
						RedeemerItem redeemerItem = new RedeemerItem( item );
						if( redeemerItem.IsValid == true )
						{
							_bonusItems.Add( redeemerItem );
						}
					}
				}
				
				var payouts = Dot.Array("payouts", currentSet, new ArrayList());
                EB.Debug.Log("【商城】加载商品成功，数量:{0}", payouts.Count);
				foreach( Hashtable payout in payouts )	
				{
					var item = new EB.IAP.Item( _externalId, payout );
					_payouts.Add(item);
				}
			}
			_iapManager.Enumerate(_payouts);

            _config.Listener.OnOffersFetchSuceeded();
        }
		#endregion

        private System.Action<Hashtable> chargeSuccessCallBack;
        private System.Action<bool> loadingUIEvent;
        public void PurchaseOffer( IAP.Item item, Hashtable table = null) 
		{
            if (table != null)
            {
                if (table.ContainsKey("callBack"))
                {
                    chargeSuccessCallBack = table["callBack"] as System.Action<Hashtable>;
                }
                if (table.ContainsKey("loadingEvent"))
                {
                    loadingUIEvent = table["loadingEvent"] as System.Action<bool>;
                }
            }

			if ( _iapManager != null)
			{
				_iapManager.PurchaseItem( item );
			}
			else
			{
				_config.Listener.OnOfferPurchaseFailed("ID_SPARX_ERROR_MUST_BE_LOGGED_IN");
			}
		}

        public void ShowOrHideLoadingUI(bool isShow)
        {
            if (loadingUIEvent != null)
            {
                loadingUIEvent(isShow);
            }
        }

		public void Fetch()
		{
			_api.Fetch(OnFetch);
		}
		
		public int Credit( int value, string reason ) 
		{
			return _api.Credit( value, reason, OnCredit );
		}
		
		public int Debit( int value, string reason )
		{
			return _api.Debit( value, reason, OnDebit );
		}
		
		void OnCredit( int id, string error, Hashtable data )
		{
			if (!string.IsNullOrEmpty(error))
			{
				FatalError(error);	
				return;
			}
			
			StoreBalance(data);	
			_config.Listener.OnCreditSuceeded(id);
		}
		
		void OnDebit( int id, string error, Hashtable data )
		{
			if (!string.IsNullOrEmpty(error))
			{
				_config.Listener.OnDebitFailed(id);	
				return;
			}
			
			StoreBalance(data);	
			
			_config.Listener.OnDebitSuceeded(id);
		}
			
		public void StoreBalance( Hashtable data )
		{
			_balance = Dot.Integer("balance", data, Balance );
			
			_config.Listener.OnBalanceUpdated(Balance);
		}

		public void BuyPayout(IAP.Item item, System.Action<string, IAP.Transaction> callback, object extraInfo = null)
		{
            if (!string.IsNullOrEmpty(item.LimitedTimeGiftId))//新增限时礼包的物品关联处理
            {
                if (extraInfo == null)
                {
                    var ht = Johny.HashtablePool.Claim();
					ht["limitedTimeGiftId"] = item.LimitedTimeGiftId;
					extraInfo = ht;
                }
                else
                {
                    ((Hashtable)extraInfo).Add("limitedTimeGiftId", item.LimitedTimeGiftId); 
                }
            }

            _api.BuyPayout(string.Format ("{0}{1}", _externalId, _iapManager.ProviderName) , item.payoutId, _iapManager.ProviderName, item.value, new ArrayList(), delegate (string error, Hashtable data)
			{
				if (!string.IsNullOrEmpty(error))
				{
					callback(error, null);
					return;
				}

				if (data == null || data.Count == 0)
				{
					callback("BuyPayout result is empty", null);
					return;
				}

				IAP.Transaction transaction = new IAP.Transaction();
				transaction.transactionId = EB.Dot.String("store.info.transactionid", data, null);
				transaction.productId = item.productId;
#if USE_TENCENTSDK || USE_VIVOSDK || USE_HUAWEISDK
				transaction.payload = EB.Dot.String("store.info.extraInfo", data, null);
#else
				transaction.payload = EB.Dot.String("store.info.extraInfo", data, item.developerPayload.Replace("{platform}", _iapManager.ProviderName));
#endif
				transaction.signature = EB.Dot.String("store.info.sign", data, transaction.signature);
                transaction.platform = _iapManager.ProviderName;
                transaction.serverPayload = _iapManager.GetPayload(transaction);

                transaction.IAPPlatform = _iapManager.ProviderName;//多渠道支付时用到

                callback(null, transaction);

                //生成订单后把订单存储在本地
                SparxTransactionHelper.AddTransInLocal(transaction);
			}, extraInfo);
		}

		void OnFetch( string error, Hashtable data )
		{
			if (!string.IsNullOrEmpty(error))
			{
				FatalError(error);
				return;
			}
			
			StoreBalance(data);	
			
			if ( State == SubSystemState.Connecting )
			{
				State = SubSystemState.Connected;
			}
		}
        
        /// <summary> 向服务器申请重连补单 </summary>
        private void RequestPayout()
        {
            EB.Debug.Log("【商城】申请重连补单!");
            List<Transaction> list = SparxTransactionHelper.GetAllVaildLoacalTrans();

            if (list.Count < 0)
            {
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                IAP.Transaction trans = list[i];
                if (trans == null)
                {
                    EB.Debug.Log("【商城】本地订单是空的,序号i = {0}", i);
                    continue;
                }

                EB.Debug.Log("【商城】添加校验订单:{0}", trans.transactionId);
                _verify.Add(trans);
            }

           EB.Debug.Log("【商城】添加的需要校验的订单数量:{0}", _verify.Count);

        }


		#region SDK那边的回调
		void OnEnumerate()
		{
            EB.Debug.Log("【商城】WalletManager.OnEnumerate()");
			_payouts.RemoveAll(delegate(IAP.Item obj) {
				return obj.valid == false;
			});
			_enumerated = true;
			_config.Listener.OnOffersFetched();
		}
	
		void OnVerify( IAP.Transaction transaction )
		{
            EB.Debug.Log("【商城】支付去校验:{0}", transaction.transactionId);
			_verify.Add(transaction);
		}
		
        void OnVerifyAgain(IAP.Transaction transaction)
        {
            if (transaction == null)
            {
                EB.Debug.LogWarning("【商城】OnVerifyAgain调用中transaction为null");
                return;
            }
            EB.Debug.Log("【商城】支付去补单:{0}", transaction.transactionId);
            CheckSupplementTransaction(transaction.transactionId);
            RemoveSomeTransaction();
            _VerifyAgain.Add(new SupplementTransaction(transaction));
        }

        void OnPurchaseFailed( string error, IAP.Transaction transaction)
		{
            EB.Debug.Log("【商城】支付失败:{0}" + error);
			_config.Listener.OnOfferPurchaseFailed(error);
            if(transaction != null)
            {
                SparxTransactionHelper.DelTransInLocal(transaction);
            }
		}
		
		void OnPurchaseCanceled(IAP.Transaction transaction)
		{
            EB.Debug.Log("【商城】支付取消");
			_config.Listener.OnOfferPurchaseCanceled();
            if (transaction != null)
            {
                SparxTransactionHelper.DelTransInLocal(transaction);
            }
        }

		void OnPurchaseNoResult(System.Action callback)
		{
            EB.Debug.Log("【商城】支付没结果");
			_config.Listener.OnOfferPurchaseNoResult(callback);
		}
		#endregion

        /// <summary>根据id获取商品</summary>
        public bool GetGiftItem(int id,out EB.IAP.Item item)
        {
            EB.IAP.Item[] tempArray = EB.Sparx.Hub.Instance.WalletManager.Payouts;
            for (int i = 0; i < tempArray.Length; i++)
            {
                if (tempArray[i].payoutId == id)
                {
                    item=tempArray[i];
                    return true;
                }
            }
            item = null;
            return false;
        }

        /// <summary> 根据id判断商品是否存在 </summary>
        public bool GetGiftItem(int id)
        {
            EB.IAP.Item[] tempArray = EB.Sparx.Hub.Instance.WalletManager.Payouts;
            for (int i = 0; i < tempArray.Length; i++)
            {
                if (tempArray[i].payoutId == id)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>多渠道时改变渠道</summary>
        public void ChangeProvider(string providerName)
        {
            _iapManager.ChangeProvider(providerName);
        }
    }
}
