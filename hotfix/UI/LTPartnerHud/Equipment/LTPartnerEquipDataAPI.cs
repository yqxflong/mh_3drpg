using System.Collections;
using System.Collections.Generic;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    public class LTPartnerEquipDataAPI : EB.Sparx.SparxAPI
    {
        public LTPartnerEquipDataAPI()
        {
            endPoint = Hub.Instance.ApiEndPoint;
        }
        private int BlockService(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate (EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();

                ProcessResult(response, dataHandler);
            });
        }
        public System.Func<EB.Sparx.Response, bool> errorProcessFun;
        private void ProcessResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);

            if (response.error != null)
            {
                EB.Debug.LogError(response.error);
            }

            if (errorProcessFun != null)
            {
                if (!errorProcessFun(response))
                {
                    if (ProcessResponse(response))
                    {
                        dataHandler(response.hashtable);
                    }
                    else
                    {
                        dataHandler(null);
                    }
                }
                else
                {
                    if (response.sucessful)
                    {
                        dataHandler(response.hashtable);

                    }
                    else
                    {
                        dataHandler(null);
                    }
                }
                errorProcessFun = null;
            }
            else
            {
                if (ProcessResponse(response))
                {
                    dataHandler(response.hashtable);
                }
                else
                {
                    dataHandler(null);
                }
            }

        }
        private void DefaultDataHandler(Hashtable alliance)
        {
            EB.Debug.Log("LTPartnerEquipDataAPI.DefaultDataHandler: call default data handler");
        }


        public void RequestEquip(int Eid, int heroID, EquipPartType toCell, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/equipment/equip");
            request.AddData("inventoryId", Eid);
            request.AddData("toCell", ((int)toCell).ToString());
            request.AddData("heroId", heroID);
            errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObject = (string)response.error;
                    switch (strObject)
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                        case "equipment can't equip this cell":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipDataAPI_3031"));
                                return true;
                            }
                        case "miss buddyId":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipDataAPI_3279"));
                                return true;
                            }
                        case "ID_ERROR_ITEM_ALREADY_EQUIP":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                    }
                }
                return false;
            };
            BlockService(request, dataHandler);
        }
        public void RequestUnEquip(int Eid, int heroID, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/equipment/unEquip");
            request.AddData("inventoryId", Eid);
            request.AddData("heroId", heroID);
            errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObject = (string)response.error;
                    EB.Debug.LogError(strObject);
                    switch (strObject)
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                        case "MAX_EQUIPMENT_NUM":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_MailApi_1124"));
                                return true;
                            }
                        case "unequip fail:hero haven't such equipment":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                    }
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        public void RequestEquipmentLevelUp(int Eid, List<int> list, List<Hashtable> costList, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/equipment/eatEquip");
            request.AddData("toInventoryId", Eid);
            ArrayList listTemp = new ArrayList(list);
            request.AddData("spendInventoryId", listTemp);
            request.AddData("costs", costList);
            errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObject = (string)response.error;
                    switch (strObject)
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                        case "already 15 level":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipCellController_3552"));
                                return true;
                            }
                        case "ID_ERROR_ITEM_IS_NOT_FOUND":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipDataAPI_6051"));
                                return true;
                            }
                        case "too much materials":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipDataAPI_6301"));
                                return true;
                            }
                        case "ID_ERROR_NOT_FOUND_ALL":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipDataAPI_6051"));
                                return true;
                            }
                    }
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        public void RequestEquipmentLock(int Eid, bool locked, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/equipment/lockOrUnlock");
            request.AddData("inventoryId", Eid);
            request.AddData("lockOrUnlock", locked);
            errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObject = (string)response.error;
                    switch (strObject)
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                    }
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        public void RequestEquipmentSyn(int Eid1, int Eid2, System.Action<Hashtable> dataHandler)
        {

            EB.Sparx.Request request = endPoint.Post("/gaminventory/combineExEquipment");
            request.AddData("invId1", Eid1);
            request.AddData("invId2", Eid2);
            errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObject = (string)response.error;
                    switch (strObject)
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                    }
                }
                return false;
            };
            BlockService(request, dataHandler);
        }

        public void RequestGetEquipmentPresetList(long uid, System.Action<Hashtable> dataHandler)
        {
            Request request = endPoint.Post("/equipment/getPreset");
            request.AddData("uid", uid);
 
            errorProcessFun = (Response response) =>
            {
                if (response.error != null)
                {
                    string strObject = (string)response.error;

                    switch (strObject)
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                    }
                }

                return false;
            };

            BlockService(request, dataHandler);
        }

        public void RequestAddEquipmentPreset(long uid, string presetName, Hashtable presetData, System.Action<Hashtable> dataHandler)
        {
            Request request = endPoint.Post("/equipment/setPreset");
            request.AddData("uid", uid);
            request.AddData("presetName", presetName);
            request.AddData("presetData", presetData);

            errorProcessFun = (Response response) =>
            {
                if (response.error != null)
                {
                    string strObject = (string)response.error;

                    switch (strObject)
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                    }
                }

                return false;
            };

            BlockService(request, dataHandler);
        }

        public void RequestDeleteEquipmentPreset(long uid, string presetName, System.Action<Hashtable> dataHandler)
        {
            Request request = endPoint.Post("/equipment/unPreset");
            request.AddData("uid", uid);
            request.AddData("presetName", presetName);

            errorProcessFun = (Response response) =>
            {
                if (response.error != null)
                {
                    string strObject = (string)response.error;

                    switch (strObject)
                    {
                        case "Read Timed out":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDrawCardAPI_3009"));
                                return true;
                            }
                    }
                }

                return false;
            };

            BlockService(request, dataHandler);
        }
    }
}