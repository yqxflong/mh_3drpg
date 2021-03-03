using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
    public class InventoryManager : Manager
    {

        #region implemented EB.Sparx.Manager
        public override void Initialize(Config config)
        {
        }

        public override void Async(string message, object options)
        {
        }
        #endregion

        public void GetAllEconomyIds(System.Action<Response> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Get("/equipment/DEBUG_getAllEconomyIds");
            lookupsManager.Service(request, callback, false);
        }

        public void GetAllEquipmentInfo(System.Action<Response> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Get("/equipment/DEBUG_getEquipmentClientData");
            lookupsManager.Service(request, callback, false);
        }

        public void AddItemToInv(string economyId, int amount, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/gaminventory/debugAdd");

            request.AddData("economyId", economyId);
            request.AddData("num", amount);

            lookupsManager.Service(request, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void RemoveItem(string inventoryId, int amount, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            //EB.Sparx.Request request = lookupsManager.EndPoint.Post("/gaminventory/sell");
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/gaminventory/debugRemove");
            request.AddData("inventoryId", inventoryId);
            request.AddData("num", amount);

            lookupsManager.Service(request, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void ConvertGem(string inventoryId, string amount, string economyId, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/gems/convert");

            request.AddData("inventoryId", inventoryId);
            request.AddData("economyId", economyId);
            request.AddData("num", amount);

            lookupsManager.Service(request, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void EquipmentRecycle(string[] inventoryIdList, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/recycling/equipmentRecycle");

            request.AddData("inventoryIdList", inventoryIdList);

            lookupsManager.Service(request, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void EnchantItem(int itemInvID, int luckstoneInvID, System.Action<bool> callback)
        {
            DataLookupSparxManager data_lookups_mgr = SparxHub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request req = data_lookups_mgr.EndPoint.Post("/enchantments/enchant");

            //req.AddData ("itemID", itemInvID);
            req.AddData("inventoryId", itemInvID);

            if (luckstoneInvID != -1)
                req.AddData("luckStoneID", luckstoneInvID);

            data_lookups_mgr.Service(req, (response) =>
            {
                var jobject = response.result as Hashtable;
                var jobject2 = jobject["enchantments"] as Hashtable;
                bool enchantSuccess = (bool)jobject2["success"];
                callback(enchantSuccess);
            });
        }


        public void EquipmentRefine(string inventoryId, string[] lockStatsList, bool autoBuy, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/refining/refineEquipment");

            request.AddData("inventoryId", inventoryId);
            request.AddData("lockStats", lockStatsList);
            request.AddData("autoBuy", autoBuy);

            lookupsManager.Service(request, delegate (Response result)
            {
                EB.Debug.Log("refineEquipment : result = ", result);
                callback(result.sucessful);
            }, true);

        }


        public void ChangeRandomStats(string inventoryId, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/refining/changeRandomStats");

            request.AddData("inventoryId", inventoryId);

            lookupsManager.Service(request, delegate (Response result)
            {
                EB.Debug.Log("changeRandomStats : result = ", result);
                callback(result.sucessful);
            }, true);
        }


        public void Equip(int itemInvID, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request req = lookupsManager.EndPoint.Post("/equipment/equip");

            req.AddData("inventoryId", itemInvID);

            lookupsManager.Service(req, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void UnEquip(int itemInvID, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request req = lookupsManager.EndPoint.Post("/equipment/unEquip");

            req.AddData("inventoryId", itemInvID);

            lookupsManager.Service(req, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void Drag(int itemInvID, string toCell, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request req = lookupsManager.EndPoint.Post("/equipment/drag");

            req.AddData("inventoryId", itemInvID);
            req.AddData("toCell", toCell);

            lookupsManager.Service(req, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void EquipmentOpenSocket(string equipmentInvID, int slotNum, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request req = lookupsManager.EndPoint.Post("/socketing/openSocket");

            req.AddData("inventoryId", equipmentInvID);
            req.AddData("slotNum", slotNum);

            lookupsManager.Service(req, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void EquipmentSocketGem(string equipmentInvID, int slotNum, string gemInvId, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request req = lookupsManager.EndPoint.Post("/socketing/socketGem");

            req.AddData("inventoryId", equipmentInvID);
            req.AddData("slotNum", slotNum);
            req.AddData("gemId", gemInvId);

            lookupsManager.Service(req, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void EquipmentUnsocketGem(string equipmentInvID, int slotNum, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request req = lookupsManager.EndPoint.Post("/socketing/unsocketGem");

            req.AddData("inventoryId", equipmentInvID);
            req.AddData("slotNum", slotNum);

            lookupsManager.Service(req, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void useItem(string inventoryId, int amount, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/gaminventory/useItem");
            request.AddData("inventoryId", inventoryId);
            request.AddData("num", amount);

            lookupsManager.Service(request, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void useInventory(string inventoryId, int amount, System.Action<bool> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/gaminventory/use");
            request.AddData("inventoryId", inventoryId);
            request.AddData("num", amount);

            lookupsManager.Service(request, delegate (Response result)
            {
                callback(result.sucessful);
            }, true);
        }

        public void synthesize(string targetEconomyId, int count, System.Action<bool, string> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/synthesis/synthesize");

            request.AddData("targetEconomyId", targetEconomyId);
            request.AddData("number", count);

            lookupsManager.Service(request, delegate (Response result)
            {
                if (result.sucessful)
                {
                    callback(result.sucessful, "");
                }
                else
                {
                    callback(result.sucessful, result.error.ToString());
                }
            }, true);
        }

        public void instantCreate(string targetEconomyId, int count, System.Action<bool, string> callback)
        {
            DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
            EB.Sparx.Request request = lookupsManager.EndPoint.Post("/synthesis/instantCreate");

            request.AddData("targetEconomyId", targetEconomyId);
            request.AddData("number", count);

            lookupsManager.Service(request, delegate (Response result)
            {
                if (result.sucessful)
                {
                    callback(result.sucessful, "");
                }
                else
                {
                    callback(result.sucessful, result.error.ToString());
                }
            }, true);
        }

        public bool isItemUnlocked(string targetEconomyId)
        {
            bool unlocked = false;
            string id = "synthesis.synthesis_unlocks." + targetEconomyId;

            //object itemInfo;
            DataLookupsCache.Instance.SearchDataByID<bool>(id, out unlocked);
            if (unlocked)
            {
                return true;
            }
            return false;
        }

    }
}

