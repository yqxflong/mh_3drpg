using UnityEngine;
using System.Collections;
using System.Globalization;

namespace EB.Sparx
{
    public class ResourcesManager : SubSystem
    {
        public System.Action<object> OnResourcesUpdateListener;

        #region implemented EB.Sparx.Manager
        public override void Initialize(Config config)
        {

        }

        public override void Connect()
        {
            State = EB.Sparx.SubSystemState.Connected;
        }

        public override void Disconnect(bool isLogout)
        {

        }

        public override void Async(string message, object payload)
        {
            switch (message)
            {
                case "update":
                    OnResourcesUpdate(payload);
                    break;
            }
        }
        #endregion

        void OnResourcesUpdate(object payload)
        {
            if (payload is Hashtable)
            {
                Hashtable data = payload as Hashtable;
				if (data.Contains("resource") && data.Contains("balance"))
				{
					string resourceName = data["resource"].ToString();
					double value = double.Parse(data["balance"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

					if (string.IsNullOrEmpty(resourceName)) return;
					DataLookupsCache.Instance.CacheData("res." + resourceName + ".v", value);

					long nextGrowthTime, nextFullGrowthTime;
					long.TryParse(data["nextGrowthTime"].ToString(), out nextGrowthTime);
					long.TryParse(data["nextFullGrowthTime"].ToString(), out nextFullGrowthTime);
					if (nextGrowthTime > 0)
						DataLookupsCache.Instance.CacheData("res." + resourceName + ".ts", nextGrowthTime);

					if (nextFullGrowthTime > 0)
						DataLookupsCache.Instance.CacheData("res." + resourceName + ".nf", nextFullGrowthTime);
				}
				if (data.Contains("resource") && data.Contains("max"))
				{
					string resourceName = data["resource"].ToString();
					int value = int.Parse(data["max"].ToString());

					if (string.IsNullOrEmpty(resourceName)) return;
					DataLookupsCache.Instance.CacheData("res." + resourceName + ".max", value);
				}
            }
            else if (payload is ArrayList)
            {
                ArrayList array = payload as ArrayList;
                foreach (object obj in array)
                {
                    Hashtable data = obj as Hashtable;
                    if (data.Contains("resource") && data.Contains("balance"))
                    {
                        string resourceName = data["resource"].ToString();

                        double value = double.Parse(data["balance"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

                        if (string.IsNullOrEmpty(resourceName))
                            return;
                        DataLookupsCache.Instance.CacheData("res." + resourceName + ".v", value);

                        long nextGrowthTime, nextFullGrowthTime;
                        long.TryParse(data["nextGrowthTime"].ToString(), out nextGrowthTime);
                        long.TryParse(data["nextFullGrowthTime"].ToString(), out nextFullGrowthTime);
                        if (nextGrowthTime > 0)
                            DataLookupsCache.Instance.CacheData("res." + resourceName + ".ts", nextGrowthTime);

                        if (nextFullGrowthTime > 0)
                            DataLookupsCache.Instance.CacheData("res." + resourceName + ".nf", nextFullGrowthTime);
                    }
					if (data.Contains("resource") && data.Contains("max"))
					{
						string resourceName = data["resource"].ToString();
						int value = int.Parse(data["max"].ToString());

						if (string.IsNullOrEmpty(resourceName)) return;
						DataLookupsCache.Instance.CacheData("res." + resourceName + ".max", value);
					}
				}
            }
            if (OnResourcesUpdateListener != null) OnResourcesUpdateListener(payload);
        }

//#if DEBUG
        public void AddGold(int goldAmount)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/userres/addGold");
            request.AddData("amount", goldAmount);
            lookupsManager.Service(request, null);
        }

        public void AddM3Res()
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/userres/debugAddM3Res");
            lookupsManager.Service(request, null);
        }

        public void BuyGoldToHc()
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/userres/buyGoldToHc");
            lookupsManager.Service(request, null);
        }

        public void AddVipHc(int hc, System.Action<EB.Sparx.Response> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/userres/addVipHc");
            request.AddData("hc", hc);
            lookupsManager.Service(request, callback);
        }
        
        public void AddTicket(int ticket, System.Action<EB.Sparx.Response> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/userres/addTicket");
            request.AddData("num", ticket);
            lookupsManager.Service(request, callback);
        }

        public void SetResRPC(string type, int num)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/userres/debugSetRes");
            request.AddData("type", type);
            request.AddData("num", num);
            lookupsManager.Service(request, null);
        }
//#endif
    }
}
