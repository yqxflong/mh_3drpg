using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EB.IAP
{	
	public class Config
	{
		// public key for Android
		public string                           PublicKey = string.Empty;
		public System.Action<Transaction> 	        Verify = null; // a user defined verification callback
        public System.Action<Transaction>           VerifyAgain = null; // a user defined verification callback
        public System.Action 			 	        OnEnumerate = null;
		public System.Action<string, Transaction>   OnPurchaseFailed = null;
		public System.Action<Transaction>           OnPurchaseCanceled 	= null;
		public System.Action<System.Action>             OnPurchaseNoResult = null;

        public System.Action<string>                OnShowMessage = null;
#if UNITY_ANDROID
        public EB.Sparx.WalletConfig.ReceiptPersistanceInterface ReceiptPersistance = null;
#endif
	}
	
	public enum ItemType
	{
		Consumable,
		NonConsumable,
		Subscription
	}
	
	public class Item
	{
		public string productId 		= string.Empty;
		public int payoutId				= 0;
		public float  cost 				= 0;
		public int cents				= 0;
		public string localizedCost 	= string.Empty;
		public bool   valid 			= false;
		public bool  show				= true;
		public ItemType type			= ItemType.Consumable;
		public int 	  value				= 0;
		public Hashtable metadata		= null;
		public string currencyCode		= string.Empty;
		public string localizedTitle	= string.Empty;
		public string longName			= string.Empty;
		public string localizedDesc		= string.Empty;
		public string developerPayload	= string.Empty;
		public int bonusCurrency 		= 0;
		public bool includesBonus		= true;
		public string icon				= string.Empty;
		public bool twoMultiple			= false;
        public int buyLimit             = 0;
        public string category          = string.Empty;
        public int categoryValue        = -1;
        public int dayBuyLimit          = 0;
        public float discount           = 0;
        public string discountStr       = string.Empty;
        public int limitNum             = -1;
		public int weeklyBuyLimit		= 0;
		public int monthlyBuyLimit		= 0;
		public int order				= 0;

        public List<Sparx.RedeemerItem> redeemers = new List<EB.Sparx.RedeemerItem>();
		
        public string LimitedTimeGiftId = string.Empty;

        public Item( string trkId, Hashtable data )
		{
			
			InitData(data);
			Hashtable showmeta = EB.Dot.Object("showmeta", data, Johny.HashtablePool.Claim());
			order = EB.Dot.Integer("order", showmeta, 0);
			var redeemersData = EB.Dot.Array("redeemers", data, new ArrayList());
			foreach (object candidate in redeemersData)
			{
				Hashtable redeemer = candidate as Hashtable;
				if (redeemer != null)
				{
					var item = new Sparx.RedeemerItem(redeemer);
					if (item.IsValid == true)
					{
						redeemers.Add(item);
					}
				}
			}
			developerPayload = trkId + "{platform}" + "@=>@" + EB.Dot.String("payoutid", data, string.Empty);
		}

		private void InitData(Hashtable data)
		{
			cost = EB.Dot.Single("cost", data, 0);
			cents = int.Parse((cost * 100.00f).ToString());
			value = EB.Dot.Integer("numOfIGC", data, 0);
            string coststr  = EB.Dot.String("coststring", data, string.Empty);
            if (coststr.Contains("ID_"))
            {
                localizedCost = EB.Localizer.GetString(coststr);
            }
            else
            {
                localizedCost = coststr;
            }

            productId = EB.Dot.String("thirdPartyId", data, string.Empty);
			type = EB.IAP.ItemType.Consumable;
			show = EB.Dot.Bool("show", data, false);
			payoutId = Dot.Integer("payoutid", data, 0);
			metadata = Dot.Object("metadata", data, Johny.HashtablePool.Claim());
			bonusCurrency = EB.Dot.Integer("igcBonus", data, 0);
			localizedTitle = EB.Dot.String("longName", data, string.Empty);

			string name = EB.Dot.String("longName", data, string.Empty);
			longName = EB.Localizer.GetString(name);

            string desc = EB.Dot.String("description", data, string.Empty);
            if (desc.StartsWith("ID_"))
            {
                localizedDesc = EB.Localizer.GetString(desc);
            }
            else
            {
                localizedDesc = desc;
            }
			includesBonus = EB.Dot.Bool("includesBonus", data, true);
			icon = EB.Dot.String("itemIcon", data, string.Empty);
			twoMultiple = EB.Dot.Bool("twoMultiple", data, false);
			buyLimit = Dot.Integer("buyLimit", data, 0);
			limitNum = Dot.Integer("limitNum", data, -1);
			category = EB.Dot.String("category", data, string.Empty);
			dayBuyLimit = Dot.Integer("dayBuyLimit", data, 0);
			discount = Dot.Single("discount", data, 0);
			weeklyBuyLimit = Dot.Integer("weeklyBuyLimit", data, 0);
			monthlyBuyLimit = Dot.Integer("monthlyBuyLimit", data, 0);
			
			if (category.CompareTo("hc") == 0)
			{
				categoryValue = 2;
			}
			else if (category.CompareTo("gift") == 0)
			{
				categoryValue = 1;
			}
			else if (category.CompareTo("gift1") == 0)
			{
				categoryValue = 3;
			}
			else if (category.CompareTo("gift2") == 0)
			{
				categoryValue = 4;
			}
			else if (category.CompareTo("gift3") == 0)
			{
				categoryValue = 5;
			}
			else if (category.CompareTo("mcard") == 0)
			{
				categoryValue = 0;
			}
		}
		

		public Item(object  freeGiftItem )
		{
			Hashtable data =  EB.Dot.Object("payout", freeGiftItem, null);
			ArrayList redeem = EB.Dot.Array("redeem", freeGiftItem, null);
			InitData(data);
			order = EB.Dot.Integer("order", data, 0);			
			this.cost = 0;
			for (int i = 0; i < redeem.Count; i++)
			{
				if (redeem[i] != null)
				{
					Sparx.RedeemerItem item = new Sparx.RedeemerItem(redeem[i] as Hashtable);
					if (item.IsValid == true)
					{
						redeemers.Add(item);
					}
				}
			}
		}
		
		public bool ContainsRedeemer(Sparx.RedeemerItem item)
		{
			foreach(Sparx.RedeemerItem redeemer in redeemers)
			{
				if(redeemer.IsSameItem(item))
				{
					return true;
				}
			}
			
			return false;
		}
	}
	
	public class Transaction
	{
		public string transactionId  	= string.Empty;
		public string productId			= string.Empty;
		public string payload			= string.Empty;
		public string signature			= string.Empty;

        public string limitedTimeGiftId = string.Empty;
        
        public string platform			= EB.Sparx.Device.MobilePlatform;

        public string serverPayload     = string.Empty;

        public string IAPPlatform       = string.Empty;
    }

    public class SupplementTransaction : Transaction
    {
		//补发重试间隔
        public static int[] retryIntervalList = { 2, 5, 8, 16, 32, 48, 64, 80, 96, 112, 100000 }; //s
		//补发重试最大时间
        public static int OverTime = 120; //s
		//订单最大缓存数量
        public static int PoolCount = 5;
        public int crateTime = 0;
        public int retryCount = 0;

        public SupplementTransaction(Transaction tran)
        {
            this.transactionId = tran.transactionId;
            this.productId = tran.productId;
            this.payload = tran.payload;
            this.signature = tran.signature;

            this.limitedTimeGiftId = tran.limitedTimeGiftId;
            this.platform = tran.platform;
            this.serverPayload = tran.serverPayload;
            this.IAPPlatform = tran.IAPPlatform;

            ResetCreatTime();
        }

        public void ResetCreatTime()
        {
            this.crateTime = EB.Time.Now;
            this.retryCount = 0;
        }

        public bool ChackRetry()
        {
            return ((EB.Time.Now - crateTime)> retryIntervalList[retryCount]);
        }

        public bool ChackRemove()
        {
            return  retryCount>=10|| (EB.Time.Now - crateTime > OverTime);
        }
    }

    public class Manager
	{
		Internal.Provider _provider;
        
        Internal.ProviderPayWay CurPayWay = Internal.ProviderPayWay.none;
        Dictionary<Internal.ProviderPayWay, Internal.Provider> _providerDic;

        public string ProviderName {
            get
            {
                return _provider.Name;
            }
        }
		
		public Manager( Config config )
		{
			_provider = Internal.ProviderFactory.Create(config);
            
            if (_provider == null)
            {
                _providerDic = new Dictionary<Internal.ProviderPayWay, Internal.Provider>();
                foreach(Internal.ProviderPayWay em in System.Enum.GetValues(typeof(Internal.ProviderPayWay)))
                {
                    var providerTemp = Internal.ProviderFactory.Create(config, em);
                    if (providerTemp != null)
                    {
                        _providerDic.Add(em, providerTemp);
                        if (_provider == null)
                        {
                            _provider = providerTemp;
                        }
                    }
                }
            }
		}
        
        public void ChangeProvider(string payway)
        {

            CurPayWay = (Internal.ProviderPayWay)Enum.Parse(typeof(Internal.ProviderPayWay), payway);
            if (_providerDic != null)
            {
                _provider = _providerDic[CurPayWay];
            }
        }

        public void PurchaseItem( Item item ) 
		{
			_provider.PurchaseItem(item); 
		}
		
		public void Enumerate( List<Item> items )
		{
			_provider.Enumerate( items);
		}
		
		public void Complete( Transaction t )
		{
            if(_providerDic!=null)
            {
                var temp=(Internal.ProviderPayWay)Enum.Parse(typeof(Internal.ProviderPayWay), t.platform);
                _providerDic[temp].Complete(t);
                return;
            }
			_provider.Complete(t);
		}

        public void OnPayCallbackFromServer(string transactionId)
        {
            _provider.OnPayCallbackFromServer(transactionId);
        }

        public string GetPayload(Transaction t)
        {
            if (_providerDic != null)
            {
                var temp = (Internal.ProviderPayWay)Enum.Parse(typeof(Internal.ProviderPayWay), t.platform);
                return _providerDic[temp].GetPayload(t);
            }
            return _provider.GetPayload(t);
        }
	}
}

