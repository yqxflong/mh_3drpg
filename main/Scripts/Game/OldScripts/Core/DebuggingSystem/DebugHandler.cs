using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;

#if UNITY_EDITOR
using UnityEditor;
#endif
using LitJson;
using System.IO;
using ILRuntime.CLR.Method;

public class DebugHandler : MonoBehaviour
#if DEBUG
    , IDebuggable
#endif
{
#if DEBUG

    void Awake()
    {
        DebugSystem.RegisterSystem("Game Debugger", this);
    }

    void OnDestroy()
    {
        DebugSystem.UnregisterSystem(this);
    }

    public void OnDrawDebug()
    {

    }

    public void OnDebugGUI()
    {

    }

    public enum eGemType
    {
        MATK,
        PATK,
        Charisma,
        Block,
        MDEF,
        PDEF,
        Crit,
        MaxHP
    }

    public enum eBuffType
    {
        POTION_PATK_1,
        POTION_PATK_2,
        SCROLL_PATK_1,
        POTION_MATK_1

    }

    GUISkin skin = null;

    private string goldAmountStr = "0";
    //private string addItemEconomyId = string.Empty;
    private string inputStr1 = string.Empty;
    private string inputStr2 = string.Empty;

    private int tabSize = Screen.width / 30;

    //private bool isEquipmentsReady = false;
    //private bool addGear = false;
    private bool addGem = false;
    private bool convertGem = false;
    private bool addItem = false;
    private bool sellItem = false;
    private bool equipmentRecycle = false;
    private bool refine = false;
    private bool bAutoBuy = false;
    private bool equip = false;
    private bool unEquip = false;
    private bool equipmentSocket = false;
    private string selectedSocketInventoryId;
    private bool equipment = false;
    private bool addBuff = false;
    private bool cancelBuff = false;
    private bool enchant = false;
    private bool synthesize = false;
    private bool setcampaign = false;
    private bool partner = false;
    private bool activityracing = false;

    private Dictionary<string, bool> gemConvertToggleDic = new Dictionary<string, bool>();
    private Dictionary<string, bool> gemRecycleToggleDic = new Dictionary<string, bool>();
    private List<string> selectedRecycleInventoryIdList = new List<string>();
    private Dictionary<string, bool> refineToggleDic = new Dictionary<string, bool>();
    private Dictionary<string, bool> lockStatsToggleDic = new Dictionary<string, bool>();
    private List<string> lockStatsList = new List<string>();
    private Dictionary<string, bool> equipToggleDic = new Dictionary<string, bool>();
    private Dictionary<string, bool> unEquipToggleDic = new Dictionary<string, bool>();
    private Dictionary<string, bool> gemSocketToggleDic = new Dictionary<string, bool>();
    private Dictionary<string, bool> equipmentOnBagToggleDic = new Dictionary<string, bool>();
    private Dictionary<string, string[]> equipmentSoltToggleDic = new Dictionary<string, string[]>();

    public void OnDebugPanelGUI()
    {
        GUILayout.BeginVertical();

        if (skin == null)
        {
            skin = Instantiate(GUI.skin);
            skin.textField.stretchHeight = true;
            skin.textField.fontSize = 32;
            skin.textField.alignment = TextAnchor.MiddleLeft;
        }

        GUISkin originSkin = GUI.skin;
        GUI.skin = skin;
        GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
        GUILayoutOption btnWidth = GUILayout.Width(330);

#if UNITY_EDITOR
        DEBUG_EditorOnlyCalls();
#endif

        ChangeCacheData();
        AddGold();
        AddM3Res();
        AddItem(btnStyle, btnWidth);
        SellItem(btnStyle, btnWidth);
        GetPartnerMaxPower(btnStyle, btnWidth);
        CancelBuff(btnStyle, btnWidth);
        SetCampaign(btnStyle, btnWidth);
        SetRes(btnStyle, btnWidth);
        SetTask(btnStyle, btnWidth);
        AllianceTools(btnStyle, btnWidth);
        RaceBattleTools(btnStyle, btnWidth);
        NationWarTools(btnStyle, btnWidth);
        ClashOfHeroTools(btnStyle, btnWidth);
        HeroTools(btnStyle, btnWidth);
        WorldBossTools(btnStyle, btnWidth);
        ChargeTools(btnStyle, btnWidth);
        DartTools(btnStyle, btnWidth);
        GuideTools(btnStyle, btnWidth);
        PayTools(btnStyle, btnWidth);
        ParticleTestTools(btnStyle, btnWidth);
        DisppearMesh(btnStyle, btnWidth);
        DisablePostEffect(btnStyle, btnWidth);
        Performance(btnStyle, btnWidth);
        NotificationTestTools(btnStyle, btnWidth);
        CrashCatchTestTools(btnStyle, btnWidth);
        FashionTools(btnStyle, btnWidth);
        AnimalTools(btnStyle, btnWidth);
        CameraTools(btnStyle, btnWidth);
        LegionWarTools(btnStyle, btnWidth);
        BugTestTools(btnStyle, btnWidth);
        //AddGem (btnStyle, btnWidth);
        //ConvertGem (btnStyle, btnWidth);
        //EquipmentRecycle (btnStyle, btnWidth);
        //UseItem (btnStyle, btnWidth);
        //Equip(btnStyle,btnWidth);
        //UnEquip(btnStyle,btnWidth);
        //Refine (btnStyle, btnWidth);
        //EquipmentSocket (btnStyle, btnWidth);
        //Equipment (btnStyle, btnWidth);
        //equip = GUILayout.Toggle(equip, "Equip", btnStyle);
        //Synthesize (btnStyle, btnWidth);		
        //EnchantsDebug (btnStyle, btnWidth);
        //ChangeHeroTemplate(btnStyle, btnWidth);
        //ChangeHeroColor(btnStyle, btnWidth);
        //ChangeEquipmentColor(btnStyle, btnWidth);	
        ActivityRacing(btnStyle, btnWidth);
        PrintLoadRecord(btnStyle, btnWidth);
        ChallengeTools(btnStyle, btnWidth);


        GUILayout.EndVertical();

        GUI.skin = originSkin;
    }

    private bool Toggle(bool value, string content)
    {
        if (GUILayout.Button(new GUIContent(content, value ? DebugSystem.ToggleOnTexture() : DebugSystem.ToggleOffTexture()), "Toggle"))
        {
            value = !value;
        }
        return value;
    }

#if UNITY_EDITOR
    public void DEBUG_EditorOnlyCalls()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Icons"))
        {
            InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
            invManager.GetAllEconomyIds(GetAllIdsCallback);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Equipment"))
        {
            InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
            invManager.GetAllEquipmentInfo(GetAllEconomyInfoCallback);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Async Gems"))
        {
            AsyncManager manager = new AsyncManager(Hub.Instance.ApiEndPoint);
            manager.RequestRemoteData("gems", delegate (string error, object result)
            {
                if (error == null)
                {
                    DataLookupsCache.Instance.CacheData(result as IDictionary);
                }
            });
        }
        GUILayout.EndHorizontal();
    }
#endif

    void ChangeCacheData()
    {
        // change cache data
        GUILayout.BeginHorizontal();

        inputStr1 = GUILayout.TextField(inputStr1, 100, GUILayout.Width(200));
        inputStr2 = GUILayout.TextField(inputStr2, 100, GUILayout.Width(200));

        if (GUILayout.Button("Change Cache Data"))
        {
            DataLookupsCache.Instance.CacheData(inputStr1, inputStr2);
        }

        GUILayout.EndHorizontal();
    }

    void AddGold()
    {
        // add gold
        GUILayout.BeginHorizontal();

        object result;
        string resultStr = string.Empty;
        if (DataLookupsCache.Instance.SearchDataByID("res.gold.v", out result))
        {
            resultStr = result == null ? "NULL" : result.ToString();
        }
        else
        {
            resultStr = "Invalid Path";
        }
        GUILayout.Label("Gold: " + resultStr);

        goldAmountStr = GUILayout.TextField(goldAmountStr, 9, GUILayout.Width(100));

        if (GUILayout.Button("Add Gold"))
        {
            int goldAmount = 0;
            int.TryParse(goldAmountStr, out goldAmount);
            if (goldAmount > 0)
            {
                ResourcesManager resManager = SparxHub.Instance.GetManager<ResourcesManager>();
                resManager.AddGold(goldAmount);
            }
        }
        GUILayout.EndHorizontal();
    }

    void AddM3Res()
    {
        // add gold
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add M3 Res"))
        {
            ResourcesManager resManager = SparxHub.Instance.GetManager<ResourcesManager>();
            resManager.AddM3Res();
        }
        GUILayout.EndHorizontal();
    }

    void AddGem(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        addGem = GUILayout.Toggle(addGem, "Add Gem", btnStyle);
        if (addGem)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(tabSize);
            GUILayout.BeginVertical();
            foreach (string gemType in System.Enum.GetNames(typeof(eGemType)))
            {
                if (GUILayout.Button(string.Format("Add GEM_{0}_1", gemType), btnWidth))
                {
                    InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                    invManager.AddItemToInv(string.Format("GEM_{0}_1", gemType), 1, BuyEquipCallback);
                }
            }

            for (int i = 1; i < 10; i++)
            {
                if (GUILayout.Button(string.Format("Add Luckstone_{0}", i), btnWidth))
                {
                    InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                    invManager.AddItemToInv(string.Format("ITM_ENCHANT_LuckStone_{0}", i), 1, BuyEquipCallback);
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }

    void ConvertGem(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        convertGem = GUILayout.Toggle(convertGem, "Convert Gem", btnStyle);
        if (convertGem)
        {
            object itemInfo;

            DataLookupsCache.Instance.SearchDataByID("inventory", out itemInfo);
            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList = (Hashtable)itemInfo;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();

                    foreach (DictionaryEntry item in itemList)
                    {
                        if (item.Value is Hashtable)
                        {
                            Hashtable itemContent = (Hashtable)item.Value;
                            if (itemContent["system"].ToString() == "Gem")
                            {
                                string keyName = item.Key.ToString();
                                if (!gemConvertToggleDic.ContainsKey(keyName))
                                {
                                    gemConvertToggleDic.Add(keyName, false);
                                }

                                string itemName = string.Format("{0} ({1})", itemContent["economy_id"].ToString(), item.Key.ToString());
                                gemConvertToggleDic[keyName] = GUILayout.Toggle(gemConvertToggleDic[keyName], itemName, btnStyle);

                                if (gemConvertToggleDic[keyName])
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(tabSize);
                                    GUILayout.BeginVertical();

                                    foreach (string gemType in System.Enum.GetNames(typeof(eGemType)))
                                    {
                                        if (gemType != itemContent["attribute"].ToString())
                                        {
                                            if (GUILayout.Button(gemType, btnWidth))
                                            {
                                                string gemName = "GEM_" + gemType + "_" + itemContent["level"].ToString();
                                                InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                                                invManager.ConvertGem(item.Key.ToString(), itemContent["num"].ToString(), gemName, BuyEquipCallback);
                                            }
                                        }
                                    }
                                    GUILayout.EndVertical();
                                    GUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
        }
    }

    private string sellnum = "1";
    void SellItem(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        sellItem = GUILayout.Toggle(sellItem, "Sell Item", btnStyle);
        if (sellItem)
        {
            object itemInfo;
            DataLookupsCache.Instance.SearchDataByID("inventory", out itemInfo);
            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList = (Hashtable)itemInfo;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();

                    foreach (DictionaryEntry item in itemList)
                    {
                        if (item.Value is Hashtable)
                        {
                            Hashtable itemContent = (Hashtable)item.Value;
                            //string economyName = /*EB.Dot.String("name",GameItemStaticData.GetItemData(itemContent["economy_id"].ToString()),"");*/ EconemyTemplateManager.GetItemName(itemContent["economy_id"].ToString());
                            string economyName = string.Empty;
                            string itemName = string.Format("{0}({1},{2},num:{3})", economyName, itemContent["economy_id"].ToString(), item.Key.ToString(), itemContent["num"].ToString());
                            GUILayout.BeginHorizontal();
                            sellnum = GUILayout.TextField(sellnum, 10, GUILayout.Width(100));
                            if (GUILayout.Button(itemName, GUILayout.Width(480)))
                            {
                                InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                                invManager.RemoveItem(item.Key.ToString(), int.Parse(sellnum), BuyEquipCallback);
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
        }
    }

    void GetPartnerMaxPower(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        partner = GUILayout.Toggle(partner, "Get Partner Max Power", btnStyle);
        if (partner)
        {
            GUILayout.Label("点完之后会出现转圈，打log，等log刷完就好了，此间最好不要乱点，不然，概不负责");

            GUILayout.BeginHorizontal();

            GUILayout.Label("伙伴StatsId：");
            PartnerGMTool.Instance.PartnerStatsID = GUILayout.TextField(PartnerGMTool.Instance.PartnerStatsID, btnWidth);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("装备套装Id（1~21）：");
            PartnerGMTool.Instance.EquipID = GUILayout.TextField(PartnerGMTool.Instance.EquipID, btnWidth);

            GUILayout.EndHorizontal();

            if (GUILayout.Button("一键毕业", btnWidth))
            {
                PartnerGMTool.Instance.MaxPower();
            }
            if (GUILayout.Button("满级满阶", btnWidth))
            {
                PartnerGMTool.Instance.PartnerLevelUp();
            }
            if (GUILayout.Button("满星", btnWidth))
            {
                PartnerGMTool.Instance.PartnerStarUp();
            }
            if (GUILayout.Button("满技能", btnWidth))
            {
                PartnerGMTool.Instance.PartnerSkillUp();
            }
            if (GUILayout.Button("满装备", btnWidth))
            {
                PartnerGMTool.Instance.PartnerEquipAllAndUp();
            }
            if (GUILayout.Button("刷新伙伴", btnWidth))
            {
                PartnerGMTool.Instance.InitPartnerData();
            }
        }
    }

#if UNITY_EDITOR
    void GetAllIdsCallback(Response res)
    {
        if (res.str != null)
        {
            EB.Debug.Log(res.str);

            char[] delimiterChars = { ',' };

            string[] ids = res.str.Split(delimiterChars);

            //GameObject root = new GameObject ("UI Icon Helper");
            string folderRoot = "Assets/_GameAssets/Res/Textures/UI/DynamicIcons";

            //find the defaut texture
            string default_path = folderRoot + "/" + "default.png";
            Texture2D default_texture = (Texture2D)AssetDatabase.LoadAssetAtPath(default_path, typeof(Texture2D));
            if (default_texture == null)
            {
                EB.Debug.LogError("Cannot find default texture at (" + default_path + ").  Please add and try again.");
                return;
            }

            for (int i = 0; i < ids.Length; i++)
            {
                EB.Debug.Log(ids[i]);

                string path = folderRoot + "/" + ids[i] + ".png";
                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
                if (texture == null)
                {
                    AssetDatabase.CopyAsset(default_path, path);
                }
            }
        }
    }

    void GetAllEconomyInfoCallback(Response res)
    {
        if (res.str != null)
        {
            EB.Debug.Log(res.str);

            char[] delimiterChars = { ',' };

            string[] ids = res.str.Split(delimiterChars);

            //GameObject root = new GameObject ("Equipment Helper");

            for (int i = 0; i < ids.Length / 2; i++)
            {
                string name = ids[i];
                string type = ids[i + 1];

                string[] split = name.Split(new char[] { '_' });

                if (split == null || split.Length <= 1)
                {
                    EB.Debug.Log("Invalid Economy ID on server " + name);
                    continue;
                }

                SpawnEquipmentFromEconomyID(name, type);
            }
        }
    }

    public static T[] GetAtPath<T>(string path)
    {
        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path.Replace("Assets/", string.Empty));

        foreach (string fileName in fileEntries)
        {
            int assetPathIndex = fileName.IndexOf("Assets");
            string localPath = fileName.Substring(assetPathIndex);

            UnityEngine.Object t = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

            if (t != null)
                al.Add(t);
        }
        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }

    public static void SpawnEquipmentFromEconomyID(string name, string type, GameObject defaultObject = null)
    {
        string folderRoot = "Assets/_GameAssets/Res/MISC/Character";
        string[] split = name.Split(new char[] { '_' });

        string eqp_char_class = split[1];

        //Macintosh HD ▸ Users ▸ mmcmanus ▸ gam ▸ client_gam ▸ Source ▸ Assets ▸ GM ▸ GameAssets ▸ Equipment ▸ Weapon
        string default_path = folderRoot;
        string add_to_path = "invalid";
        if (type == "Weapon")
        {
            add_to_path = "Weapon/";
            default_path += "/_DefaultWeapon.prefab";
        }
        else if (type == "Head")
        {
            add_to_path = "";
            default_path += "/_DefaultHead.prefab";
        }
        else if (type == "Armor")
        {
            add_to_path = "";
            default_path += "/_DefaultArmor.prefab";
        }
        else if (type == "Wings")
        {
            add_to_path = "";
            default_path += "/_DefaultWings.prefab";
        }

        if (add_to_path != "invalid")
        {
            //EB.Debug.Log (ids [i]);
            //EB.Debug.Log (ids [i + 1]);

            GameObject default_prefab = (GameObject)AssetDatabase.LoadAssetAtPath(default_path, typeof(GameObject));
            if (default_prefab == null)
            {
                EB.Debug.LogError("Cannot find default prefab at (" + default_path + ").  Please add and try again.");
                return;
            }

            string[] classes = null;
            if (eqp_char_class == "WMA")
            {
                classes = new string[3];
                classes[0] = "Warrior";
                classes[1] = "Mage";
                classes[2] = "Archer";
            }
            else
            {
                classes = new string[1];
                if (eqp_char_class == "W")
                {
                    classes[0] = "Warrior";
                }
                else if (eqp_char_class == "M")
                {
                    classes[0] = "Mage";
                }
                else
                {
                    classes[0] = "Archer";
                }
            }

            EB.Debug.Log("//////////////////////////////////////////////////////////");
            EB.Debug.Log(default_path);
            if (defaultObject != null)
            {
                default_path = AssetDatabase.GetAssetPath(defaultObject);
            }

            for (int j = 0; j < classes.Length; j++)
            {
                string path = folderRoot + "/" + classes[j] + "/Male/" + add_to_path + "(" + name + ")" + "_male" + classes[j] + ".prefab";
                EB.Debug.Log(path);
                //AssetDatabase.CopyAsset (default_path, path);
                path = folderRoot + "/" + classes[j] + "/Female/" + add_to_path + "(" + name + ")" + "_female" + classes[j] + ".prefab";
                EB.Debug.Log(path);
                //AssetDatabase.CopyAsset (default_path, path);
            }


            /*string path = folderRoot + add_to_path + ids [i] + "-prefab.prefab";
					Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath (path, typeof(Texture2D));
					if (texture == null) {

						AssetDatabase.CopyAsset (default_path, path);
					}*/
        }
    }


#endif

    //	void GetAllEquipments()
    //	{
    //		InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
    //		invManager.GetAllEquipments();
    //	}
    //		
    //	void BuyEquipment(string itemID, int amount)
    //	{
    //		InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
    //		invManager.BuyEquipment(itemID, amount, BuyEquipCallback);
    //	}

    void EquipmentRecycle(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        equipmentRecycle = GUILayout.Toggle(equipmentRecycle, "Equipment Recycle", btnStyle);

        if (equipmentRecycle)
        {
            object itemInfo;

            DataLookupsCache.Instance.SearchDataByID("inventory", out itemInfo);
            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList = (Hashtable)itemInfo;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();

                    foreach (DictionaryEntry item in itemList)
                    {
                        if (item.Value is Hashtable)
                        {
                            Hashtable itemContent = (Hashtable)item.Value;
                            if (CheckRecycleItemStatus(itemContent))
                            {
                                string keyName = item.Key.ToString();
                                string itemName = string.Format("{0} ({1})", itemContent["economy_id"].ToString(), keyName);

                                if (!gemRecycleToggleDic.ContainsKey(keyName))
                                {
                                    gemRecycleToggleDic.Add(keyName, false);
                                }

                                gemRecycleToggleDic[keyName] = GUILayout.Toggle(gemRecycleToggleDic[keyName], itemName, btnStyle);

                                if (gemRecycleToggleDic[keyName])
                                {
                                    if (!selectedRecycleInventoryIdList.Contains(keyName))
                                    {
                                        selectedRecycleInventoryIdList.Add(keyName);
                                    }
                                }
                                else
                                {
                                    if (selectedRecycleInventoryIdList.Contains(keyName))
                                    {
                                        selectedRecycleInventoryIdList.Remove(keyName);
                                    }
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Recycle", btnWidth))
            {
                if (selectedRecycleInventoryIdList.Count > 0)
                {
                    string[] inventoryIdList = new string[selectedRecycleInventoryIdList.Count];
                    for (int i = 0; i < selectedRecycleInventoryIdList.Count; i++)
                    {
                        inventoryIdList[i] = selectedRecycleInventoryIdList[i];
                    }

                    InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                    invManager.EquipmentRecycle(inventoryIdList, RecycleCallback);
                }
            }
        }
    }

    private bool CheckRecycleItemStatus(Hashtable item)
    {
        return (item["system"].ToString() == "Equipment") && (item["qualityLevel"].ToString() != "Poor");
    }

    private int socket_slotNum = 1;
    private string socket_gemId = "10111";
    private string sLotNumStr = "1";

    void EquipmentSocket(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        equipmentSocket = GUILayout.Toggle(equipmentSocket, "Equipment Socket", btnStyle);

        if (equipmentSocket)
        {

            sLotNumStr = GUILayout.TextField(sLotNumStr, 5, GUILayout.Width(100));
            socket_gemId = GUILayout.TextField(socket_gemId, 20, GUILayout.Width(100));
            socket_slotNum = System.Convert.ToInt32(sLotNumStr);
            object itemInfo;

            DataLookupsCache.Instance.SearchDataByID("inventory", out itemInfo);
            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList = (Hashtable)itemInfo;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();

                    foreach (DictionaryEntry item in itemList)
                    {
                        if (item.Value is Hashtable)
                        {
                            Hashtable itemContent = (Hashtable)item.Value;
                            if (itemContent["system"].ToString() == "Equipment")
                            {
                                string keyName = item.Key.ToString();
                                string itemName = string.Format("{0} ({1})", itemContent["economy_id"].ToString(), keyName);

                                if (!gemSocketToggleDic.ContainsKey(keyName))
                                {
                                    gemSocketToggleDic.Add(keyName, false);
                                }

                                gemSocketToggleDic[keyName] = GUILayout.Toggle(gemSocketToggleDic[keyName], itemName, btnStyle);

                                if (gemSocketToggleDic[keyName])
                                {
                                    selectedSocketInventoryId = keyName;
                                }

                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            DataLookupsCache.Instance.SearchDataByID("equipment", out itemInfo);
            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList = (Hashtable)itemInfo;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();

                    foreach (DictionaryEntry item in itemList)
                    {
                        if (item.Value is Hashtable)
                        {
                            Hashtable itemContent = (Hashtable)item.Value;
                            if (itemContent["system"].ToString() == "Equipment")
                            {
                                string keyName = item.Key.ToString();
                                string itemName = string.Format("{0} ({1})", itemContent["economy_id"].ToString(), keyName);

                                if (!gemSocketToggleDic.ContainsKey(keyName))
                                {
                                    gemSocketToggleDic.Add(keyName, false);
                                }

                                gemSocketToggleDic[keyName] = GUILayout.Toggle(gemSocketToggleDic[keyName], itemName, btnStyle);

                                if (gemSocketToggleDic[keyName])
                                {
                                    selectedSocketInventoryId = keyName;
                                }

                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("OpenSocket", btnWidth))
            {
                //string slotNum = "slot1";
                InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                invManager.EquipmentOpenSocket(selectedSocketInventoryId, socket_slotNum, BuyEquipCallback);
            }
            if (GUILayout.Button("SocketGem", btnWidth))
            {
                //				string slotNum = "slot1";
                InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                invManager.EquipmentSocketGem(selectedSocketInventoryId, socket_slotNum, socket_gemId, BuyEquipCallback);
            }
            if (GUILayout.Button("UnSocketGem", btnWidth))
            {
                //				string slotNum = "slot1";
                InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                invManager.EquipmentUnsocketGem(selectedSocketInventoryId, socket_slotNum, BuyEquipCallback);
            }
        }
    }

    private string addItem_itemStr = "EQP_W_ARM_EP_50";
    private string addItem_num = "1";

    void AddItem(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        addItem = GUILayout.Toggle(addItem, "Add Item", btnStyle);
        if (addItem)
        {
            GUILayout.BeginHorizontal();

            addItem_itemStr = GUILayout.TextField(addItem_itemStr, 100, GUILayout.Width(500));
            addItem_num = GUILayout.TextField(addItem_num, 5, GUILayout.Width(100));

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Add It", btnWidth))
            {
                int itemAmount = 0;
                int.TryParse(addItem_num, out itemAmount);
                if (itemAmount > 0)
                {
                    InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                    invManager.AddItemToInv(addItem_itemStr, itemAmount, BuyEquipCallback);
                }
            }
        }
    }

    void Equip(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        equip = GUILayout.Toggle(equip, "Equip", btnStyle);
        if (equip)
        {
            object itemInfo;

            DataLookupsCache.Instance.SearchDataByID("inventory", out itemInfo);
            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList = (Hashtable)itemInfo;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();

                    foreach (DictionaryEntry item in itemList)
                    {
                        if (item.Value is Hashtable)
                        {
                            Hashtable itemContent = (Hashtable)item.Value;
                            if (itemContent["system"].ToString() == "Equipment" && itemContent["location"].ToString() == "bag")
                            {
                                string keyName = item.Key.ToString();
                                if (!equipToggleDic.ContainsKey(keyName))
                                {
                                    equipToggleDic.Add(keyName, false);
                                }

                                string itemName = string.Format("{0} (id:{1},subtype:{2})", itemContent["economy_id"].ToString(), item.Key.ToString(), itemContent["subtype"]);
                                equipToggleDic[keyName] = GUILayout.Button(itemName, btnStyle);

                                if (equipToggleDic[keyName])
                                {
                                    InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                                    invManager.Equip(System.Convert.ToInt32(item.Key.ToString()), BuyEquipCallback);
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                }
            }
        }
    }

    void UnEquip(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        unEquip = GUILayout.Toggle(unEquip, "UnEquip", btnStyle);
        if (unEquip)
        {
            object itemInfo;

            DataLookupsCache.Instance.SearchDataByID("inventory", out itemInfo);

            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList = (Hashtable)itemInfo;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();

                    foreach (DictionaryEntry item in itemList)
                    {
                        if (item.Value is Hashtable)
                        {
                            Hashtable itemContent = (Hashtable)item.Value;

                            if (itemContent["system"].ToString() == "Equipment" && itemContent["location"].ToString() == "equipment")
                            {
                                string keyName = item.Key.ToString();
                                if (!unEquipToggleDic.ContainsKey(keyName))
                                {
                                    unEquipToggleDic.Add(keyName, false);
                                }

                                string itemName = string.Format("{0} (id:{1},subtype:{2})", itemContent["economy_id"].ToString(), item.Key.ToString(), itemContent["subtype"]);
                                unEquipToggleDic[keyName] = GUILayout.Button(itemName, btnStyle);

                                if (unEquipToggleDic[keyName])
                                {
                                    InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                                    invManager.UnEquip(System.Convert.ToInt32(item.Key.ToString()), BuyEquipCallback);
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
        }
    }

    void Equipment(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        equipment = GUILayout.Toggle(equipment, "Equipment", btnStyle);

        if (equipment)
        {
            object itemInfo;

            string[] equipSlotNames = new string[8] {
                "Weapon",
                "Head",
                "Armor",
                "Brooch",
                "JeweleryL",
                "JeweleryR",
                "RingL",
                "RingR"
            };

            GUILayout.BeginHorizontal();
            GUILayout.Space(tabSize);
            GUILayout.BeginVertical();

            foreach (string slotName in equipSlotNames)
            {
                if (!equipmentSoltToggleDic.ContainsKey(slotName))
                {
                    equipmentSoltToggleDic.Add(slotName, new string[3] { "0", slotName, "false" });
                }
                bool a = GUILayout.Toggle(bool.Parse(equipmentSoltToggleDic[slotName][2]), equipmentSoltToggleDic[slotName][1], btnStyle);
                equipmentSoltToggleDic[slotName][2] = System.Convert.ToString(a);

                //equipmentSoltToggleDic[slotName][1] =  slotName;

            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Equip", btnWidth))
            {
                foreach (KeyValuePair<string, bool> kvp in equipmentOnBagToggleDic)
                {
                    if (kvp.Value == true)
                    {
                        InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                        invManager.Equip(System.Convert.ToInt32(kvp.Key), BuyEquipCallback);
                        equipmentOnBagToggleDic.Remove(kvp.Key);
                    }
                }

            }
            if (GUILayout.Button("UnEquip", btnWidth))
            {
                foreach (string slotName in equipSlotNames)
                {
                    if (equipmentSoltToggleDic[slotName][2] == bool.TrueString && equipmentSoltToggleDic[slotName][0] != "0")
                    {
                        InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                        invManager.UnEquip(System.Convert.ToInt32(equipmentSoltToggleDic[slotName][0]), BuyEquipCallback);
                    }
                }

            }
            if (GUILayout.Button("Drag", btnWidth))
            {
                string toCell = "";
                string itemInvID = "";
                foreach (string slotName in equipSlotNames)
                {
                    if (equipmentSoltToggleDic[slotName][2] == bool.TrueString)
                    {
                        toCell = slotName;
                    }
                }
                foreach (string slotName in equipSlotNames)
                {
                    if (equipmentSoltToggleDic[slotName][2] == bool.TrueString && toCell != slotName)
                    {
                        itemInvID = equipmentSoltToggleDic[slotName][0];
                    }
                }
                foreach (KeyValuePair<string, bool> kvp in equipmentOnBagToggleDic)
                {
                    if (kvp.Value == true)
                    {
                        itemInvID = kvp.Key;
                    }
                }
                if (itemInvID != "" && toCell != "")
                {
                    InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                    invManager.Drag(System.Convert.ToInt32(itemInvID), toCell, BuyEquipCallback);
                }
            }

            foreach (string slotName in equipSlotNames)
            {
                equipmentSoltToggleDic[slotName][0] = "0";
                equipmentSoltToggleDic[slotName][1] = slotName;
            }

            DataLookupsCache.Instance.SearchDataByID("inventory", out itemInfo);
            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList = (Hashtable)itemInfo;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();

                    foreach (DictionaryEntry item in itemList)
                    {
                        if (item.Value is Hashtable)
                        {
                            Hashtable itemContent = (Hashtable)item.Value;
                            if (itemContent["system"].ToString() == "Equipment" && itemContent["location"].ToString() == "equipment")
                            {
                                string keyName = item.Key.ToString();

                                string itemName = string.Format("{0} (id:{1},subtype:{2},cell:{3})", itemContent["economy_id"].ToString(), item.Key.ToString(), itemContent["subtype"], itemContent["cell"]);

                                string cell = itemContent["cell"].ToString();

                                equipmentSoltToggleDic[cell][0] = keyName;
                                equipmentSoltToggleDic[cell][1] = itemName;

                            }
                            if (itemContent["system"].ToString() == "Equipment" && itemContent["location"].ToString() == "bag")
                            {
                                string keyName = item.Key.ToString();
                                string itemName = string.Format("{0} (id:{1},subtype:{2})", itemContent["economy_id"].ToString(), item.Key.ToString(), itemContent["subtype"]);
                                if (!equipmentOnBagToggleDic.ContainsKey(keyName))
                                {
                                    equipmentOnBagToggleDic.Add(keyName, false);
                                }
                                equipmentOnBagToggleDic[keyName] = GUILayout.Toggle(equipmentOnBagToggleDic[keyName], itemName, btnStyle);

                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }

        }
    }



    void Refine(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        refine = GUILayout.Toggle(refine, "Refine", btnStyle);
        if (refine)
        {
            object itemInfo;

            DataLookupsCache.Instance.SearchDataByID("inventory", out itemInfo);
            //DataLookupsCache.Instance.SearchDataByID ("equipment", out itemInfo);
            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList = (Hashtable)itemInfo;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();

                    string strInventoryId = "0";
                    foreach (DictionaryEntry item in itemList)
                    {
                        if (item.Value is Hashtable)
                        {
                            Hashtable itemContent = (Hashtable)item.Value;
                            if (itemContent["system"].ToString() == "Equipment")
                            {
                                string keyName = item.Key.ToString();
                                if (!refineToggleDic.ContainsKey(keyName))
                                {
                                    refineToggleDic.Add(keyName, false);
                                }
                                string itemName = string.Format("{0} (id:{1},subtype:{2})", itemContent["economy_id"].ToString(), item.Key.ToString(), itemContent["subtype"]);
                                refineToggleDic[keyName] = GUILayout.Toggle(refineToggleDic[keyName], itemName, btnStyle);

                                for (int i = 0; i < refineToggleDic.Count; i++)
                                {
                                    if (refineToggleDic[keyName])
                                    {
                                        strInventoryId = keyName;
                                    }
                                }

                                bAutoBuy = GUILayout.Toggle(bAutoBuy, "Auto Buy", btnStyle);

                                ArrayList rndAttrList = new ArrayList();
                                ArrayList refiningAttrList = new ArrayList();
                                if (itemContent["randomAttr"] is ArrayList)
                                {
                                    rndAttrList = (ArrayList)itemContent["randomAttr"];
                                }
                                if (itemContent["refiningAttr"] is ArrayList)
                                {
                                    refiningAttrList = (ArrayList)itemContent["refiningAttr"];
                                }

                                for (int i = 0; i < 5; i++)
                                {
                                    string strLockIdx = i.ToString();
                                    if (!lockStatsToggleDic.ContainsKey(strLockIdx))
                                    {
                                        lockStatsToggleDic.Add(strLockIdx, false);
                                    }

                                    // EB.Debug.Log("itemContent.randomAttr[i] = " + JsonFormatter.PrettyPrint(JsonMapper.ToJson(rndAttrList[i]).ToString()));
                                    string rndAttr = "";
                                    if (rndAttrList.Count > i)
                                    {
                                        rndAttr = JsonMapper.ToJson(rndAttrList[i]).ToString();
                                    }

                                    string refiningAttr = "";
                                    if (refiningAttrList.Count > i)
                                    {
                                        refiningAttr = JsonMapper.ToJson(refiningAttrList[i]).ToString();
                                    }

                                    string strFlag = " Unlock";
                                    if (lockStatsToggleDic[strLockIdx] == true)
                                    {
                                        strFlag = " Lock";
                                    }
                                    string strLockName = rndAttr + strFlag + strLockIdx + " -> " + refiningAttr;
                                    lockStatsToggleDic[strLockIdx] = GUILayout.Toggle(lockStatsToggleDic[strLockIdx], strLockName, btnStyle);
                                }

                                for (int i = 0; i < lockStatsToggleDic.Count; i++)
                                {
                                    string strLockIdx = i.ToString();
                                    if (lockStatsToggleDic[strLockIdx])
                                    {
                                        if (!lockStatsList.Contains(strLockIdx))
                                        {
                                            lockStatsList.Add(strLockIdx);
                                        }
                                    }
                                    else
                                    {
                                        if (lockStatsList.Contains(strLockIdx))
                                        {
                                            lockStatsList.Remove(strLockIdx);
                                        }
                                    }
                                }

                                if (GUILayout.Button("Refine", btnWidth))
                                {
                                    string[] lockIdxList = new string[lockStatsList.Count];
                                    if (lockStatsList.Count > 0)
                                    {
                                        for (int i = 0; i < lockStatsList.Count; i++)
                                        {
                                            lockIdxList[i] = lockStatsList[i];
                                        }
                                    }

                                    InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                                    invManager.EquipmentRefine(strInventoryId, lockIdxList, bAutoBuy, RefineCallback);
                                }

                                if (GUILayout.Button("Change", btnWidth))
                                {
                                    InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                                    invManager.ChangeRandomStats(strInventoryId, ChangeRandomStatsCallback);
                                }

                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
        }
    }


    void BuyEquipCallback(bool ifSuccess)
    {
        if (ifSuccess)
        {
            //InventoryDialog.Instance.UpdateInventory();
        }
    }

    void RecycleCallback(bool ifSuccess)
    {
        selectedRecycleInventoryIdList.Clear();
    }



    void ChangeRandomStatsCallback(bool ifSuccess)
    {
        lockStatsList.Clear();
    }

    void UseItem(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        addBuff = GUILayout.Toggle(addBuff, "Use Item", btnStyle);
        if (addBuff)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(tabSize);
            GUILayout.BeginVertical();

            object itemInfo;
            DataLookupsCache.Instance.SearchDataByID("inventory", out itemInfo);
            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList = (Hashtable)itemInfo;

                    foreach (DictionaryEntry item in itemList)
                    {
                        if (item.Value is Hashtable)
                        {
                            Hashtable itemContent = (Hashtable)item.Value;
                            string itemName = string.Format("{0} ({1})", itemContent["economy_id"].ToString(), item.Key.ToString());

                            if (itemContent["system"].ToString() == "Buff" || itemContent["system"].ToString() == "SynthesisUnlock")
                            {
                                if (GUILayout.Button(itemName, btnWidth))
                                {
                                    InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                                    invManager.useItem(item.Key.ToString(), 1, BuyEquipCallback);
                                }
                            }
                        }
                    }
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }

    void CancelBuff(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        cancelBuff = GUILayout.Toggle(cancelBuff, "Cancel Buff", btnStyle);
        if (cancelBuff)
        {
            object itemInfo;
            DataLookupsCache.Instance.SearchDataByID("buffs", out itemInfo);
            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList = (Hashtable)itemInfo;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();

                    foreach (DictionaryEntry item in itemList)
                    {
                        if (item.Value is Hashtable)
                        {
                            Hashtable itemContent = (Hashtable)item.Value;
                            string itemName = string.Format("{0}", itemContent["economy_id"].ToString());

                            //if (GUILayout.Button(itemName, btnWidth))
                            //{
                            //    BuffManager buffManager = SparxHub.Instance.GetManager<BuffManager>();
                            //    buffManager.CancelBuff(item.Key.ToString(), BuyEquipCallback);
                            //}
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
        }
    }

    void RefineCallback(bool ifSuccess)
    {
        lockStatsList.Clear();
    }

    private int synthesize_count = 1;
    private string synthesizeCountStr = "1";
    private string synthesize_target = "";
    private string synthesize_material = "";
    private string goldCostStr = "0";
    private int gemCount = 11;
    private int equipCount = 5;
    private bool isGem = false;
    private string retSynthesisMessage = "";

    void Synthesize(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        synthesize = GUILayout.Toggle(synthesize, "Synthesize", btnStyle);

        if (synthesize)
        {

            object itemInfo;

            DataLookupsCache.Instance.SearchDataByID("synthesis", out itemInfo);
            if (null != itemInfo)
            {
                if (itemInfo is Hashtable)
                {
                    Hashtable itemList0 = (Hashtable)itemInfo;

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();
                    ArrayList itemList = (ArrayList)itemList0["Gem"];
                    GUILayout.Label("Gem: ");

                    int currentCount = 0;
                    foreach (Hashtable item in itemList)
                    {
                        Hashtable itemContent = (Hashtable)item;
                        if (itemContent["category"].ToString() == "Gem")
                        {
                            ++currentCount;
                            if (currentCount > gemCount)
                            {
                                break;
                            }

                            string keyName = item["id"].ToString();
                            string itemName = string.Format("{0}", itemContent["id"].ToString());
                            if (itemContent["synthesis_unlock"].ToString() == "True")
                            {
                                InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();

                                if (!invManager.isItemUnlocked(keyName))
                                {
                                    continue;
                                }
                            }
                            if (!gemSocketToggleDic.ContainsKey(keyName))
                            {
                                gemSocketToggleDic.Add(keyName, false);
                            }

                            gemSocketToggleDic[keyName] = GUILayout.Toggle(gemSocketToggleDic[keyName], itemName, btnStyle);

                            if (gemSocketToggleDic[keyName])
                            {
                                synthesize_target = keyName;
                                isGem = true;
                                goldCostStr = item["cost_gold"].ToString();
                                synthesize_material = item["item_a"] + "*" + item["count_a"] + "   ";
                                if (item["count_b"].ToString() != "0")
                                {
                                    synthesize_material += item["item_b"] + "*" + item["count_b"];
                                    if (item["count_c"].ToString() != "0")
                                    {
                                        synthesize_material += item["item_c"] + "*" + item["count_c"];
                                        if (item["count_d"].ToString() != "0")
                                        {
                                            synthesize_material += item["item_d"] + "*" + item["count_d"];
                                        }
                                    }
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.Space(tabSize);
                    GUILayout.BeginVertical();
                    itemList = (ArrayList)itemList0["Equip"];
                    GUILayout.Label("Equip: ");

                    currentCount = 0;
                    foreach (Hashtable item in itemList)
                    {
                        Hashtable itemContent = (Hashtable)item;
                        if (itemContent["category"].ToString() == "Equip")
                        {
                            ++currentCount;
                            if (currentCount > equipCount)
                            {
                                break;
                            }
                            string keyName = item["id"].ToString();
                            string itemName = string.Format("{0}", itemContent["id"].ToString());

                            if (!gemSocketToggleDic.ContainsKey(keyName))
                            {
                                gemSocketToggleDic.Add(keyName, false);
                            }

                            gemSocketToggleDic[keyName] = GUILayout.Toggle(gemSocketToggleDic[keyName], itemName, btnStyle);

                            if (gemSocketToggleDic[keyName])
                            {
                                synthesize_target = keyName;
                                goldCostStr = item["cost_gold"].ToString();
                                isGem = false;
                                synthesize_material = item["item_a"] + "*" + item["count_a"] + "   ";
                                if (item["count_b"].ToString() != "0")
                                {
                                    synthesize_material += item["item_b"] + "*" + item["count_b"] + "   ";
                                    if (item["count_c"].ToString() != "0")
                                    {
                                        synthesize_material += item["item_c"] + "*" + item["count_c"] + "   ";
                                        if (item["count_d"].ToString() != "0")
                                        {
                                            synthesize_material += item["item_d"] + "*" + item["count_d"];
                                        }
                                    }
                                }
                            }
                        }
                    }

                    GUILayout.Label("Item: ");
                    itemList = (ArrayList)itemList0["Item"];

                    currentCount = 0;
                    foreach (Hashtable item in itemList)
                    {
                        Hashtable itemContent = (Hashtable)item;
                        if (itemContent["category"].ToString() == "Item")
                        {
                            ++currentCount;
                            if (currentCount > equipCount)
                            {
                                break;
                            }
                            string keyName = item["id"].ToString();
                            string itemName = string.Format("{0}", itemContent["id"].ToString());

                            if (!gemSocketToggleDic.ContainsKey(keyName))
                            {
                                gemSocketToggleDic.Add(keyName, false);
                            }

                            gemSocketToggleDic[keyName] = GUILayout.Toggle(gemSocketToggleDic[keyName], itemName, btnStyle);

                            if (gemSocketToggleDic[keyName])
                            {
                                synthesize_target = keyName;
                                goldCostStr = item["cost_gold"].ToString();
                                isGem = false;
                                synthesize_material = item["item_a"] + "*" + item["count_a"] + "   ";
                                if (item["count_b"].ToString() != "0")
                                {
                                    synthesize_material += item["item_b"] + "*" + item["count_b"] + "   ";
                                    if (item["count_c"].ToString() != "0")
                                    {
                                        synthesize_material += item["item_c"] + "*" + item["count_c"] + "   ";
                                        if (item["count_d"].ToString() != "0")
                                        {
                                            synthesize_material += item["item_d"] + "*" + item["count_d"];
                                        }
                                    }
                                }
                            }
                        }
                    }

                    itemList = (ArrayList)itemList0["Advancement Help"];
                    GUILayout.Label("Advancement Help: ");

                    currentCount = 0;
                    foreach (Hashtable item in itemList)
                    {
                        Hashtable itemContent = (Hashtable)item;
                        if (itemContent["category"].ToString() == "Equip")
                        {
                            ++currentCount;
                            if (currentCount > equipCount)
                            {
                                break;
                            }
                            string keyName = item["id"].ToString();
                            string itemName = string.Format("{0}", itemContent["id"].ToString());

                            if (!gemSocketToggleDic.ContainsKey(keyName))
                            {
                                gemSocketToggleDic.Add(keyName, false);
                            }

                            gemSocketToggleDic[keyName] = GUILayout.Toggle(gemSocketToggleDic[keyName], itemName, btnStyle);

                            if (gemSocketToggleDic[keyName])
                            {
                                synthesize_target = keyName;
                                goldCostStr = item["cost_gold"].ToString();
                                isGem = false;
                                synthesize_material = item["item_a"] + "*" + item["count_a"] + "   ";
                                if (item["count_b"].ToString() != "0")
                                {
                                    synthesize_material += item["item_b"] + "*" + item["count_b"] + "   ";
                                    if (item["count_c"].ToString() != "0")
                                    {
                                        synthesize_material += item["item_c"] + "*" + item["count_c"] + "   ";
                                        if (item["count_d"].ToString() != "0")
                                        {
                                            synthesize_material += item["item_d"] + "*" + item["count_d"];
                                        }
                                    }
                                }
                            }
                        }
                        //}
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.Label(retSynthesisMessage);
            GUILayout.Label("Target: " + synthesize_target);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Material: ");
            GUILayout.TextField(synthesize_material, 200, GUILayout.Width(650));
            GUILayout.EndHorizontal();
            GUILayout.Label("Gold cost: " + goldCostStr);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Count: ");
            synthesizeCountStr = GUILayout.TextField(synthesizeCountStr, 5, GUILayout.Width(200));
            synthesize_count = System.Convert.ToInt32(synthesizeCountStr);


            if (GUILayout.Button("Make", GUILayout.Width(200)))
            {
                InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                invManager.synthesize(synthesize_target, synthesize_count, SynthesisCallback);
            }
            if (isGem)
            {
                if (GUILayout.Button("Instant Create", GUILayout.Width(200)))
                {
                    InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();
                    invManager.instantCreate(synthesize_target, synthesize_count, SynthesisCallback);
                }
            }

            GUILayout.EndHorizontal();
        }
    }

    void SynthesisCallback(bool ifSuccess, string msg)
    {
        if (ifSuccess)
        {
            retSynthesisMessage = "";
        }
        else
        {
            retSynthesisMessage = "Error: " + msg;
        }
    }

    #region ActivityRacing
    private string activity_racing_group = "1";
    private string activity_racing_num = "1";

    private void ActivityRacing(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        activityracing = GUILayout.Toggle(activityracing, "ActivityRacing", btnStyle);

        if (!activityracing)
            return;

        GUILayout.BeginVertical();
        GUILayout.Label("Group: ");
        activity_racing_group = GUILayout.TextField(activity_racing_group);
        if(!int.TryParse(activity_racing_group, out int nGroup))
        {
            return;
        }

        if(GUILayout.Button("StartBet"))
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTActivityRacingManager", "Instance", "DebugRequestEnterBet", nGroup);
        }

        if(GUILayout.Button("StartCalc"))
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTActivityRacingManager", "Instance", "DebugRequestCalcResult", nGroup);
        }

        GUILayout.Label("Num: ");
        activity_racing_num = GUILayout.TextField(activity_racing_num);
        if(GUILayout.Button("Bet"))
        {
            int nNum = int.Parse(activity_racing_num);
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTActivityRacingManager", "Instance", "RequestBet", nGroup, nNum);
        }

        if(GUILayout.Button("AddBet"))
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTActivityRacingManager", "Instance", "RequestAddBet", nGroup);
        }

        if(GUILayout.Button("SendAward"))
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTActivityRacingManager", "Instance", "DebugRequestSendReward", nGroup);
        }

        if(GUILayout.Button("CleanStatus"))
        {
            GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTActivityRacingManager", "Instance", "DebugRequestCleanStatus", nGroup);
        }
        GUILayout.EndVertical();
    }
    #endregion

    #region enchantments
    private void EnchantsDebug(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        enchant = GUILayout.Toggle(enchant, "Enchant", btnStyle);

        if (!enchant)
            return;

        enchantDebug_itemStr = GUILayout.TextField(enchantDebug_itemStr);
        enchantDebug_luckstroneStr = GUILayout.TextField(enchantDebug_luckstroneStr);
        GUILayout.Label(enchantDebug_successStr);

        if (GUILayout.Button("Enchant Item"))
        {
            try
            {
                InventoryManager invManager = SparxHub.Instance.GetManager<InventoryManager>();

                int equipmentID = int.Parse(enchantDebug_itemStr);

                object nextLevelNode;
                string nextLevelNodeDataID = string.Format(@"inventory[""{0}""].enchantments.nextLevel", equipmentID);
                bool hasNextLevel = DataLookupsCache.Instance.SearchDataByID(nextLevelNodeDataID, out nextLevelNode);

                if (!hasNextLevel)
                {
                    enchantDebug_successStr = "This equipment already reached maxl level, you can't enchant it anymore.";
                    return;
                }

                int luckstoneID = !string.IsNullOrEmpty(enchantDebug_luckstroneStr) ? int.Parse(enchantDebug_luckstroneStr) : -1;

                invManager.EnchantItem(equipmentID, luckstoneID, (enchantSuccess) =>
                {
                    enchantDebug_successStr = enchantSuccess ? "Enchant Successful" : "Enchant Failed";
                });
            }
            catch (System.Exception e)
            {
                enchantDebug_successStr = "Something went wrong !";
                throw e;
            }
        }
    }

    private string enchantDebug_itemStr = "Item InventoryID";
    private string enchantDebug_luckstroneStr = "Luckstone InventoryID (empty if don't use luckstone)";
    private string enchantDebug_successStr;
    #endregion

    #region ChangeHeroTemplate

    private bool changeHeroTemplate = false;

    void ChangeHeroTemplate(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        changeHeroTemplate = GUILayout.Toggle(changeHeroTemplate, "Change Hero Template", btnStyle);
        if (changeHeroTemplate)
        {
            if (GUILayout.Button("Get Hero Template", btnWidth))
            {
                GetHeroTemplate();
            }

            object heroInfo;
            DataLookupsCache.Instance.SearchDataByID("heroStats.templates", out heroInfo);

            if (null != heroInfo && heroInfo is Hashtable)
            {
                Hashtable heroTmpList = (Hashtable)heroInfo;
                GUILayout.BeginHorizontal();
                GUILayout.Space(tabSize);
                GUILayout.BeginVertical();

                foreach (DictionaryEntry heroTmp in heroTmpList)
                {
                    string currentTmp;
                    DataLookupsCache.Instance.SearchDataByID<string>("heroStats.currentTemplate", out currentTmp);

                    if (heroTmp.Key.ToString() == currentTmp)
                    {
                        GUILayout.TextArea(string.Format("Current Hero Template -> {0}: {1}", heroTmp.Key.ToString(), heroTmp.Value.ToString()), new GUIStyle(GUI.skin.box));
                    }
                    else
                    {
                        if (GUILayout.Button(string.Format("{0}: {1}", heroTmp.Key.ToString(), heroTmp.Value.ToString()), btnWidth))
                        {
                            SendChangeHeroTmpRequest(heroTmp.Key.ToString());
                        }
                    }
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }
    }

    void GetHeroTemplate()
    {
        DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
        EB.Sparx.Request request = lookupsManager.EndPoint.Get("/herostats/getHeroTemplates");
        lookupsManager.Service(request, OnHeroTemplatesCallback);
    }

    void SendChangeHeroTmpRequest(string tmpIndexStr)
    {
        int tmpIndex = int.Parse(tmpIndexStr);
        DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
        EB.Sparx.Request request = lookupsManager.EndPoint.Post("/herostats/setHeroTemplates");
        request.AddData("id", tmpIndex);
        lookupsManager.Service(request, OnHeroTemplatesCallback);
    }

    void OnHeroTemplatesCallback(Response res)
    {
        if (res.result != null)
        {
            var result = res.result as Hashtable;
            if (result != null && result["isSuccess"] != null)
            {
                if (bool.Parse(result["isSuccess"].ToString()))
                {
                    //TODO: RELOGIN
                }
            }
        }
    }

    #endregion

    #region ChangeHeroColor
    private bool changeHeroColor = false;
    //public string skinColor = "1";
    //public string hairColor = "1";
    //public string eyeColor = "1";

    public int skinColorIndex = 0;
    public int hairColorIndex = 0;
    public int eyeColorIndex = 0;
    public string[] colorIndices = { "1", "2", "3", "4", "5", "6", "7", "8" };
    void ChangeHeroColor(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        if (HeroColorPresets.Instance == null)
        {
            return;
        }

        if (colorIndices.Length != HeroColorPresets.Instance.PresetColorCount)
        {
            colorIndices = new string[HeroColorPresets.Instance.PresetColorCount];
            for (int i = 0; i < HeroColorPresets.Instance.PresetColorCount; i++)
            {
                colorIndices[i] = string.Format("{0}", i + 1);
            }
        }

        changeHeroColor = GUILayout.Toggle(changeHeroColor, "Change Hero Color", btnStyle);
        if (!changeHeroColor)
        {
            return;
        }

        GUILayout.BeginHorizontal();
        {
            //int skinColorIndex = 0;
            GUILayout.Label("Skin Color Index", GUILayout.Width(160));
            skinColorIndex = GUILayout.SelectionGrid(skinColorIndex, colorIndices, 3, btnStyle);
            //skinColor = GUILayout.TextField(skinColor);
            //if(GUILayout.Button("Apply"))
            //{
            //if(int.TryParse(skinColor, out skinColorIndex))
            //{
            DataLookupsCache.Instance.CacheData("colorIndex.skinColorIndex", skinColorIndex + 1);
            //}
            //}
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        {
            ///int hairColorIndex = 0;
            GUILayout.Label("Hair Color Index", GUILayout.Width(160));
            hairColorIndex = GUILayout.SelectionGrid(hairColorIndex, colorIndices, 3, btnStyle);
            //hairColor = GUILayout.TextField(hairColor);
            //if(GUILayout.Button("Apply"))
            //{
            //if(int.TryParse(hairColor, out hairColorIndex))
            //{
            DataLookupsCache.Instance.CacheData("colorIndex.hairColorIndex", hairColorIndex + 1);
            //}
            //}
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        {
            //int eyeColorIndex = 0;
            GUILayout.Label("Eye Color Index", GUILayout.Width(160));
            eyeColorIndex = GUILayout.SelectionGrid(eyeColorIndex, colorIndices, 3, btnStyle);
            //eyeColor = GUILayout.TextField(eyeColor);
            //if(GUILayout.Button("Apply"))
            //{
            //if(int.TryParse(eyeColor, out eyeColorIndex))
            //{
            DataLookupsCache.Instance.CacheData("colorIndex.eyesColorIndex", eyeColorIndex + 1);
            //}
            //}
        }
        GUILayout.EndHorizontal();
    }

    #endregion

    #region ChangeEquipmentColor
    bool changeEquipmentColor = false;
    //string inventoryID = string.Empty;
    //string equipmentColor = string.Empty;
    int equipmentColorIndex = -1;
    void ChangeEquipmentColor(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        if (HeroColorPresets.Instance == null)
        {
            return;
        }

        if (colorIndices.Length != HeroColorPresets.Instance.PresetColorCount)
        {
            colorIndices = new string[HeroColorPresets.Instance.PresetColorCount];
            for (int i = 0; i < HeroColorPresets.Instance.PresetColorCount; i++)
            {
                colorIndices[i] = string.Format("{0}", i + 1);
            }
        }

        changeEquipmentColor = GUILayout.Toggle(changeEquipmentColor, "Change Equipment Color", btnStyle);
        if (!changeEquipmentColor)
        {
            return;
        }

        object value = null;
        DataLookupsCache.Instance.SearchDataByID<object>("equippedItems", out value);
        if (value == null)
        {
            return;
        }

        if (!(value is IDictionary))
        {
            return;
        }

        IDictionary equipedItems = value as IDictionary;
        if (equipedItems == null)
        {
            return;
        }

        //if(equipedItems.Contains("Armor"))
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Equipment Color Index", GUILayout.Width(160));
                //inventoryID = equipedItems["Armor"].ToString();
                equipmentColorIndex = GUILayout.SelectionGrid(equipmentColorIndex, colorIndices, 3, btnStyle);
                DataLookupsCache.Instance.CacheData("colorIndex.equipmentColorIndex", equipmentColorIndex + 1);
                //equipmentColor = GUILayout.TextField(equipmentColor);
                //if(GUILayout.Button("Apply"))
                //{
                //int equipmentColorIndex = 0;
                //if(int.TryParse(equipmentColor, out equipmentColorIndex))
                //
                //IDictionaryEnumerator enumerator = equipedItems.GetEnumerator();
                //while(enumerator.MoveNext())
                //{
                //DataLookupsCache.Instance.CacheData(string.Format("{0}.equipmentColorIndex", enumerator.Value.ToString()), equipmentColorIndex + 1);
                //}

                //}
                //}

            }
            GUILayout.EndHorizontal();
        }
    }

    private string campaingname = "";
    void SetCampaign(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        setcampaign = GUILayout.Toggle(setcampaign, "Set Campaign", btnStyle);
        if (setcampaign)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open All", btnWidth))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/fastcampaign/debugOpenAllCampaign");
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Times", btnWidth))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/fastcampaign/debugSetAllCampaignTimesToZero");
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            campaingname = GUILayout.TextField(campaingname, 10, GUILayout.Width(200));
            if (GUILayout.Button("Open To", GUILayout.Width(200)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/fastcampaign/debugOpenToCertainCampaign");
                request.AddData("campaignName", campaingname);
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();
        }
    }

    private bool setres = false;
    private string vigor_num = "10";
    private string hc_num = "10";
    private string gold_num = "10";
    private string ore_num = "10";
    private string food_num = "10";
    private string exp_num = "10";
    private string trainingRune_num = "10";
    private string arenaGold_num = "10";
    private string expeditionGold_num = "10";
    private string allianceDonate_num = "10";
    private string allianceGold_num = "10";
    private string buddy_exp = "10";
    private string action_power = "10";
    private string ordinary_prop = "10";
    private string senior_prop = "10";
    private string heroGold_num = "10";
    private string ladderGold_num = "10";
    private string chall_camp_point = "10";
    private string poten_gold_point = "10";

    void SetRes(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        setres = GUILayout.Toggle(setres, "Set Resources", btnStyle);
        if (setres)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("vigor", GUILayout.Width(200));
            vigor_num = GUILayout.TextField(vigor_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("vigor", int.Parse(vigor_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("hc", GUILayout.Width(200));
            hc_num = GUILayout.TextField(hc_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("hc", int.Parse(hc_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("gold", GUILayout.Width(200));
            gold_num = GUILayout.TextField(gold_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("gold", int.Parse(gold_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ore", GUILayout.Width(200));
            ore_num = GUILayout.TextField(ore_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("ore", int.Parse(ore_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("food", GUILayout.Width(200));
            food_num = GUILayout.TextField(food_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("food", int.Parse(food_num));
            }
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("exp", GUILayout.Width(200));
            exp_num = GUILayout.TextField(exp_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("xp", int.Parse(exp_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("training rune", GUILayout.Width(200));
            trainingRune_num = GUILayout.TextField(trainingRune_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("training-rune", int.Parse(trainingRune_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("arena gold", GUILayout.Width(200));
            arenaGold_num = GUILayout.TextField(arenaGold_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("arena-gold", int.Parse(arenaGold_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("hero gold", GUILayout.Width(200));
            heroGold_num = GUILayout.TextField(heroGold_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("hero-gold", int.Parse(heroGold_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ladder gold", GUILayout.Width(200));
            ladderGold_num = GUILayout.TextField(ladderGold_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("ladder-gold", int.Parse(ladderGold_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("expedition gold", GUILayout.Width(200));
            expeditionGold_num = GUILayout.TextField(expeditionGold_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("expedition-gold", int.Parse(expeditionGold_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("alliance donate", GUILayout.Width(200));
            allianceDonate_num = GUILayout.TextField(allianceDonate_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("alliance-donate", int.Parse(allianceDonate_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("alliance gold", GUILayout.Width(200));
            allianceGold_num = GUILayout.TextField(allianceGold_num, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("alliance-gold", int.Parse(allianceGold_num));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("buddy exp", GUILayout.Width(200));
            buddy_exp = GUILayout.TextField(buddy_exp, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("buddy-exp", int.Parse(buddy_exp));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("action power", GUILayout.Width(200));
            action_power = GUILayout.TextField(action_power, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("action-power", int.Parse(action_power));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ordinary prop", GUILayout.Width(200));
            ordinary_prop = GUILayout.TextField(ordinary_prop, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("ordinary-prop", int.Parse(ordinary_prop));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("senior prop", GUILayout.Width(200));
            senior_prop = GUILayout.TextField(senior_prop, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("senior-prop", int.Parse(senior_prop));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("chall-camp-point", GUILayout.Width(200));
            chall_camp_point = GUILayout.TextField(chall_camp_point, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("chall-camp-point", int.Parse(chall_camp_point));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("poten-gold", GUILayout.Width(200));
            poten_gold_point = GUILayout.TextField(poten_gold_point, 10, GUILayout.Width(100));
            if (GUILayout.Button("Set", btnWidth))
            {
                EB.Sparx.Hub.Instance.GetManager<EB.Sparx.ResourcesManager>().SetResRPC("poten-gold", int.Parse(poten_gold_point));
            }
            GUILayout.EndHorizontal();
        }
    }

    private bool settask = false;
    private string task_id = "";
    void SetTask(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        settask = GUILayout.Toggle(settask, "Task", btnStyle);
        if (settask)
        {
            task_id = GUILayout.TextField(task_id, 10, GUILayout.Width(100));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Accept", GUILayout.Width(150)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/mhjtasks/debugAccept");
                request.AddData("task_id", task_id);
                lookupsManager.Service(request);
            }

            if (GUILayout.Button("Complete", GUILayout.Width(150)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/mhjtasks/debugFinish");
                request.AddData("task_id", task_id);
                lookupsManager.Service(request);
            }

            if (GUILayout.Button("Delete", GUILayout.Width(150)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/mhjtasks/debugDelete");
                request.AddData("task_id", task_id);
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();
        }

    }
    #endregion

    #region Alliance tools
    bool allianceTools = false;
    string allianceBalance = string.Empty;
    string allianceLiveness = string.Empty;
    void AllianceTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        allianceTools = GUILayout.Toggle(allianceTools, "Alliance Tools", btnStyle);
        if (allianceTools)
        {
            GUILayout.BeginHorizontal();
            allianceBalance = GUILayout.TextField(allianceBalance, 10, GUILayout.Width(200));
            if (GUILayout.Button("Set Balance", GUILayout.Width(200)))
            {
                int balance = 0;
                //ToDo:暂时屏蔽
                //if (AlliancesManager.Instance.Account.State == eAllianceState.Joined && int.TryParse(allianceBalance, out balance))
                //{
                //    AlliancesManager.Instance.DebugSetBalance(balance);
                //}
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            allianceLiveness = GUILayout.TextField(allianceLiveness, 10, GUILayout.Width(200));
            if (GUILayout.Button("Set Liveness", GUILayout.Width(200)))
            {
                int liveness = 0;
                //ToDo:暂时屏蔽
                //if (AlliancesManager.Instance.Account.State == eAllianceState.Joined && int.TryParse(allianceLiveness, out liveness))
                //{
                //    AlliancesManager.Instance.DebugSetLiveness(liveness);
                //}
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Liveness", GUILayout.Width(200)))
            {
                int liveness = 0;
                //ToDo:暂时屏蔽
                //if (AlliancesManager.Instance.Account.State == eAllianceState.Joined && int.TryParse(allianceLiveness, out liveness))
                //{
                //    AlliancesManager.Instance.DebugSetLiveness(liveness);
                //}
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Member", GUILayout.Width(200)))
            {
                //ToDo:暂时屏蔽
                //DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                //EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugAddOneUserToAlliance");
                //request.AddData("worldId", LoginManager.Instance.LocalUser.WorldId);
                //request.AddData("aid", AlliancesManager.Instance.Account.AllianceId);
                //lookupsManager.Service(request, delegate (EB.Sparx.Response res)
                //{
                //    if (res.sucessful)
                //        AlliancesManager.Instance.RequestAllianceMemberList();
                //});
            }
            GUILayout.EndHorizontal();
        }
    }
    #endregion

    #region RaceBattle tools
    bool raceBattleTools = false;
    string prepare = "120";
    string battle = "60";
    string state = "";
    void RaceBattleTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        raceBattleTools = GUILayout.Toggle(raceBattleTools, "RaceBattle Tools", btnStyle);
        if (raceBattleTools)
        {
            GUILayout.BeginVertical();
            //ToDo:暂时屏蔽
            //AlliancesManager.Instance.BattleInfo.IsNeedApplyCondition = Toggle(AlliancesManager.Instance.BattleInfo.IsNeedApplyCondition, "NeedApplyConditon?");
            GUILayout.BeginHorizontal();
            GUILayout.Label("draw stage", GUILayout.Width(200));
            if (GUILayout.Button("start", GUILayout.Width(100)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetDrawStart");
                lookupsManager.Service(request);
            }
            if (GUILayout.Button("end", GUILayout.Width(100)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetDrawEnd");
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("add player", GUILayout.Width(200));
            if (GUILayout.Button("add one", GUILayout.Width(100)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugDragOneUserToBattleField");
                //ToDo:暂时屏蔽
                //request.AddData("aid", AlliancesManager.Instance.Account.AllianceId);
                lookupsManager.Service(request, delegate (EB.Sparx.Response res)
                {
                    ///if (res.sucessful)
                    //AlliancesManager.Instance.EnterBattleField(delegate (bool successful) { });
                });
            }
            if (GUILayout.Button("add all", GUILayout.Width(100)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugDragUserToBattleField");
                //ToDo:暂时屏蔽
                //request.AddData("aid", AlliancesManager.Instance.Account.AllianceId);
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("qualifiers1 stage", GUILayout.Width(200));
                prepare = GUILayout.TextField(prepare, 10, GUILayout.Width(100));
                battle = GUILayout.TextField(battle, 10, GUILayout.Width(100));
                if (GUILayout.Button("start", GUILayout.Width(100)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetQualifiers1Start");
                    request.AddData("prepareTime", prepare);
                    request.AddData("battleTime", int.Parse(prepare) + int.Parse(battle));
                    lookupsManager.Service(request);
                }
                if (GUILayout.Button("end", GUILayout.Width(100)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetQualifiers1End");
                    lookupsManager.Service(request);
                }
            }
            GUILayout.EndHorizontal();

            //GUILayout.Space(tabSize / 2);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("qualifiers2 stage", GUILayout.Width(200));
                prepare = GUILayout.TextField(prepare, 10, GUILayout.Width(100));
                battle = GUILayout.TextField(battle, 10, GUILayout.Width(100));
                if (GUILayout.Button("start", GUILayout.Width(100)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetQualifiers2Start");
                    request.AddData("prepareTime", prepare);
                    request.AddData("battleTime", int.Parse(prepare) + int.Parse(battle));
                    lookupsManager.Service(request);
                }
                if (GUILayout.Button("end", GUILayout.Width(100)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetQualifiers2End");
                    lookupsManager.Service(request);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("8_4 stage", GUILayout.Width(200));
                prepare = GUILayout.TextField(prepare, 10, GUILayout.Width(100));
                battle = GUILayout.TextField(battle, 10, GUILayout.Width(100));
                if (GUILayout.Button("start", GUILayout.Width(100)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetQuarterfinalStart");
                    request.AddData("prepareTime", prepare);
                    request.AddData("battleTime", int.Parse(prepare) + int.Parse(battle));
                    lookupsManager.Service(request);
                }
                if (GUILayout.Button("end", GUILayout.Width(100)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetQuarterfinalEnd");
                    lookupsManager.Service(request);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("4_2 stage", GUILayout.Width(200));
                prepare = GUILayout.TextField(prepare, 10, GUILayout.Width(100));
                battle = GUILayout.TextField(battle, 10, GUILayout.Width(100));
                if (GUILayout.Button("start", GUILayout.Width(100)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetSemifinalStart");
                    request.AddData("prepareTime", prepare);
                    request.AddData("battleTime", int.Parse(prepare) + int.Parse(battle));
                    lookupsManager.Service(request);
                }
                if (GUILayout.Button("end", GUILayout.Width(100)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetSemifinalEnd");
                    lookupsManager.Service(request);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("2_1 stage", GUILayout.Width(200));
                prepare = GUILayout.TextField(prepare, 10, GUILayout.Width(100));
                battle = GUILayout.TextField(battle, 10, GUILayout.Width(100));
                if (GUILayout.Button("start", GUILayout.Width(100)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetFinalStart");
                    request.AddData("prepareTime", prepare);
                    request.AddData("battleTime", int.Parse(prepare) + int.Parse(battle));
                    lookupsManager.Service(request);
                }
                if (GUILayout.Button("end", GUILayout.Width(100)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugSetFinalEnd");
                    lookupsManager.Service(request);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("state", GUILayout.Width(200));
                state = GUILayout.TextField(state, 20, GUILayout.Width(200));
                if (GUILayout.Button("refresh", GUILayout.Width(100)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugGetAllianceBattleStatus");
                    lookupsManager.Service(request, delegate (EB.Sparx.Response res)
                    {
                        if (res.result != null)
                        {
                            state = EB.Dot.String("", res.result, "");
                        }
                    });
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("clear", GUILayout.Width(200));
                if (GUILayout.Button("clear", GUILayout.Width(200)))
                {
                    DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                    EB.Sparx.Request request = lookupsManager.EndPoint.Post("/alliancesbattle/debugClearAllianceBattleStatus");
                    lookupsManager.Service(request);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
    #endregion

    #region NationWar tools
    bool nationWarTools = false;
    string realmId = "1";
    string stage = "1";
    //string addRobotToSide;
    //string addRobotToNation;
    //string workRobotToSide;
    //string workRobotToPath;
    void NationWarTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        nationWarTools = GUILayout.Toggle(nationWarTools, "NationWar Tools", btnStyle);
        if (nationWarTools)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("StartEvent", GUILayout.Width(400)))
            {
                int realmId_int = 1, stage_int = 1;
                int.TryParse(realmId, out realmId_int);
                int.TryParse(stage, out stage_int);
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.StartEvent(realmId_int, stage_int, null);
            }
            GUILayout.Label("realmId:", GUILayout.Width(250));
            realmId = GUILayout.TextField(realmId, 10, GUILayout.Width(200));
            GUILayout.Label("stage:", GUILayout.Width(250));
            stage = GUILayout.TextField(stage, 10, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("StopEvent", GUILayout.Width(400)))
            {
                int realmId_int = 1, stage_int = 1;
                int.TryParse(realmId, out realmId_int);
                int.TryParse(stage, out stage_int);
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.StopEvent(realmId_int, stage_int, null);
            }
            GUILayout.Label("realmId:", GUILayout.Width(250));
            realmId = GUILayout.TextField(realmId, 10, GUILayout.Width(200));
            GUILayout.Label("stage:", GUILayout.Width(250));
            stage = GUILayout.TextField(stage, 10, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("realmId:", GUILayout.Width(250));
            realmId = GUILayout.TextField(realmId, 10, GUILayout.Width(200));
            if (GUILayout.Button("ResetNationRank", GUILayout.Width(600)))
            {
                int realmId_int = 1;
                int.TryParse(realmId, out realmId_int);
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.ResetNationRank(realmId_int, null);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            //ToDo: 暂时屏蔽，方便解耦
            //GUILayout.Label("MyNation:" + NationManager.Instance.Account.NationName, GUILayout.Width(600));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("AddRobot:", GUILayout.Width(600));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(100));
            if (GUILayout.Button("Persian_Attack", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.AddRobot("persian", "attack", null);
            }
            if (GUILayout.Button("Persian_Defend", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.AddRobot("persian", "defend", null);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(100));
            if (GUILayout.Button("Roman_Attack", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.AddRobot("roman", "attack", null);
            }
            if (GUILayout.Button("Roman_Defend", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.AddRobot("roman", "defend", null);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(100));
            if (GUILayout.Button("Egypt_Attack", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.AddRobot("egypt", "attack", null);
            }
            if (GUILayout.Button("Egypt_Defend", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.AddRobot("egypt", "defend", null);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("WorkRobot:", GUILayout.Width(600));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(100));
            if (GUILayout.Button("Attack_Up", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.RobotWork("attack", "up", null);
            }
            if (GUILayout.Button("Defend_Up", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.RobotWork("defend", "up", null);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(100));
            if (GUILayout.Button("Attack_Median", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.RobotWork("attack", "median", null);
            }
            if (GUILayout.Button("Defend_Median", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.RobotWork("defend", "median", null);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(100));
            if (GUILayout.Button("Attack_Down", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.RobotWork("attack", "down", null);
            }
            if (GUILayout.Button("Defend_Down", GUILayout.Width(600)))
            {
                //ToDo: 暂时屏蔽，方便解耦
                //NationManager.Instance.RobotWork("defend", "down", null);
            }
            GUILayout.EndHorizontal();
        }
    }
    #endregion

    #region ClashOfHero tools
    bool cohtools;
    void ClashOfHeroTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        cohtools = GUILayout.Toggle(cohtools, "ClashOfHero Tools", btnStyle);
        if (cohtools)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start", btnWidth))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/clashofheroes/startEvent");
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start match", btnWidth))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/clashofheroes/match");
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();
        }
    }
    #endregion

    #region Hero tools
    bool herotools = false;
    string heroid = string.Empty;
    string heroshardnum = string.Empty;
    void HeroTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        herotools = GUILayout.Toggle(herotools, "Hero Tools", btnStyle);
        if (herotools)
        {
            GUILayout.BeginHorizontal();
            heroid = GUILayout.TextField(heroid, 10, GUILayout.Width(200));
            if (GUILayout.Button("Add Hero", GUILayout.Width(200)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/herostats/debugAddHero");
                request.AddData("characterId", heroid);
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            heroid = GUILayout.TextField(heroid, 10, GUILayout.Width(200));
            heroshardnum = GUILayout.TextField(heroshardnum, 10, GUILayout.Width(200));
            if (GUILayout.Button("Add HeroShard", GUILayout.Width(200)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/herostats/debugAddHeroShard");
                request.AddData("characterId", heroid);
                request.AddData("num", heroshardnum);
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();
        }
    }
    #endregion

    #region Fashion tools

    bool fashionTools = false;
    string modelId = string.Empty;
    string patchNum = string.Empty;
    void FashionTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        fashionTools = GUILayout.Toggle(fashionTools, "Fashion Tools", btnStyle);
        if (fashionTools)
        {
            GUILayout.BeginHorizontal();
            modelId = GUILayout.TextField(modelId, 10, GUILayout.Width(200));
            patchNum = GUILayout.TextField(patchNum, 10, GUILayout.Width(200));
            if (GUILayout.Button("AddFashionPatch", GUILayout.Width(200)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/fashion/debugAddFashionShard");
                request.AddData("modelId", modelId);
                request.AddData("num", patchNum);
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();
        }
    }

    #region Animal tools

    bool animalTools = false;
    string infoId = string.Empty;
    string shardNum = string.Empty;
    void AnimalTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        animalTools = GUILayout.Toggle(animalTools, "Animal Tools", btnStyle);
        if (animalTools)
        {
            GUILayout.BeginHorizontal();
            infoId = GUILayout.TextField(infoId, 10, GUILayout.Width(200));
            shardNum = GUILayout.TextField(shardNum, 10, GUILayout.Width(200));
            if (GUILayout.Button("AddShard", GUILayout.Width(200)))
            {
                DataLookupSparxManager lookupManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupManager.EndPoint.Post("/animal/debugAddAnimalShard");
                request.AddData("infoId", infoId);
                request.AddData("num", shardNum);
                lookupManager.Service(request);
            }
            GUILayout.EndHorizontal();
        }
    }

    bool cameraTools = false;
    void CameraTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        cameraTools = GUILayout.Toggle(cameraTools, "Camera Tools", btnStyle);
        if (cameraTools)
        {
            GUILayout.BeginVertical();
            PlayerCameraComponent.IsUseCameraEffect = Toggle(PlayerCameraComponent.IsUseCameraEffect, "IsUseCameraEffect?");
            GUILayout.EndVertical();
        }
    }

    string showText = "test";
    void PrintLoadRecord(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        if (GUILayout.Button("PrintLoadRecord", btnStyle))
        {
            showText = EB.Assets.PrintCostLog();
        }
        GUILayout.Label(showText);
    }
    

    bool challengeTools = false;
    void ChallengeTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        challengeTools = GUILayout.Toggle(challengeTools, "Challenge Tools", btnStyle);
        if (challengeTools)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open QuickBattle", GUILayout.Width(300)))
            {
                GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTInstanceMapModel", "SetQuickBattleOpen");
            }
            if (GUILayout.Button("Close QuickBattle", GUILayout.Width(300)))
            {
                GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTInstanceMapModel", "SetQuickBattleClose");
            }
            if (GUILayout.Button("Open DebugBattle", GUILayout.Width(300)))
            {
                GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTInstanceMapModel", "SetDebugBattleOpen");
            }
            if (GUILayout.Button("Close DebugBattle", GUILayout.Width(300)))
            {
                GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTInstanceMapModel", "SetDebugBattleClose");
            }
            GUILayout.EndHorizontal();
        }
    }


    bool legionWarTools = false;
    string barrageCount = "100";
    bool openTools = false;
    void LegionWarTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        legionWarTools = GUILayout.Toggle(legionWarTools, "LegionWar Tools", btnStyle);
        if (legionWarTools)
        {
            GUILayout.BeginHorizontal();
            barrageCount = GUILayout.TextField(barrageCount, 10, GUILayout.Width(200));
            if (GUILayout.Button((openTools) ? "Open" : "Close", GUILayout.Width(200)))
            {
                openTools = !openTools;
                //if (openTools)
                //{
                //    if (LTBarrageHudController.Instance != null)
                //    {
                //        LTBarrageHudController.Instance.count = int.Parse(barrageCount);
                //        LTBarrageHudController.Instance.isDebug = true;
                //    }
                //}
                //else
                //{
                //    if (LTBarrageHudController.Instance != null) LTBarrageHudController.Instance.isDebug = false;
                //}
            }
            GUILayout.EndHorizontal();
        }
    }

    bool openBugTest = false;
    void BugTestTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        openBugTest = GUILayout.Toggle(openBugTest, "openBugTest Tools", btnStyle);
        if (openBugTest)
        {
            GUILayout.BeginHorizontal();
            string strtest = null;
            strtest = ((bool)GetHotfixMthod.Instance.GetHotfixTypeAttribute("Hotfix_LT.UI.GuideNodeManager", "IsVirtualBtnGuide", null,false)).ToString();
            GUILayout.TextField(strtest, 10, GUILayout.Width(200));
            //GUILayout.TextField(string.Format(GuideNodeManager.IsVirtualBtnGuide.ToString()), 10, GUILayout.Width(200));
            GUILayout.TextField(string.Format(UICamera.isOverUI.ToString()), 10, GUILayout.Width(200));

            if (UICamera.hoveredObject != null) GUILayout.TextField(string.Format(UICamera.hoveredObject.name), 10, GUILayout.Width(200));
            for (int i = 0, imax = UICamera.activeTouches.Count; i < imax; ++i)
            {
                UICamera.MouseOrTouch touch = UICamera.activeTouches[i];
                if (touch.pressed != null && touch.pressed != UICamera.fallThrough && NGUITools.FindInParents<UIRoot>(touch.pressed) != null)
                    GUILayout.TextField(string.Format(touch.current.name), 10, GUILayout.Width(200));
            }

            if (UICamera.controller.pressed != null && UICamera.controller.pressed != UICamera.fallThrough && NGUITools.FindInParents<UIRoot>(UICamera.controller.pressed) != null)
                GUILayout.TextField(string.Format(UICamera.controller.current.name), 10, GUILayout.Width(200));

            //GUILayout.TextField(string.Format(DialoguePlayUtil.Instance.gameObject.activeSelf.ToString()), 10, GUILayout.Width(200));
            GUILayout.TextField(string.Format(UIStack.Instance.GetHighestSortingOrder().ToString()), 10, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            object instance = null;
            string test4 = null, test5 = null, test6 = null;
            if (instance != null)
            {
                test4 = (string)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.NpcManager", "Instance", "GetTest1");
                test5 = (string)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.NpcManager", "Instance", "GetTest2");
                test6 = (string)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.NpcManager", "Instance", "GetTest3");
            }

            if (instance != null/*NpcManager.Instance != null*/)
            {
                GUILayout.TextField(/*string.Format(NpcManager.Instance.GetTest1().ToString())*/test4, 10, GUILayout.Width(200));
                GUILayout.TextField(/*string.Format(NpcManager.Instance.GetTest2().ToString())*/test5, 10, GUILayout.Width(200));
                GUILayout.TextField(/*string.Format(NpcManager.Instance.GetTest3().ToString())*/test6, 10, GUILayout.Width(200));
            }
            string test1 = null, test2 = null, test3 = null;

            instance = GetHotfixMthod.Instance.GetHotfixTypeAttribute("Hotfix_LT.UI.PlayerManagerForFilter", "Instance", null);
            if (instance != null)
            {
                test1 = (string)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.PlayerManagerForFilter", "Instance", "GetTest1");
                test2 = (string)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.PlayerManagerForFilter", "Instance", "GetTest2");
                test3 = (string)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.PlayerManagerForFilter", "Instance", "GetTest3");
            }

            if (/*PlayerManagerForFilter.Instance*/instance != null)
            {
                GUILayout.TextField(/*string.Format(PlayerManagerForFilter.Instance.GetTest1().ToString()),*/ test1, 10, GUILayout.Width(200));
                GUILayout.TextField(/*string.Format(PlayerManagerForFilter.Instance.GetTest2().ToString()),*/ test2, 10, GUILayout.Width(200));
                GUILayout.TextField(/*string.Format(PlayerManagerForFilter.Instance.GetTest3().ToString()),*/ test3, 10, GUILayout.Width(200));
            }
            GUILayout.EndHorizontal();
        }

    }

    #endregion

    #endregion

    #region WorldBoss tools
    bool bosstools = false;
    void WorldBossTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        bosstools = GUILayout.Toggle(bosstools, "World Boss Tools", btnStyle);
        if (bosstools)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("InitBossJob", GUILayout.Width(200)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/worldboss/InitBossJob");
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("pushReward", GUILayout.Width(200)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/worldboss/pushReward");
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("clearBoss", GUILayout.Width(200)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/worldboss/ClearBossJob");
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("startBossJob", GUILayout.Width(200)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/worldboss/startBossJob");
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();
        }
    }
    #endregion

    #region Charge tools
    bool chargeTools = false;
    void ChargeTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        chargeTools = GUILayout.Toggle(chargeTools, "Charge Tools", btnStyle);
        if (chargeTools)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("BuyMonthCard", GUILayout.Width(300)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/sign_in/debugBuyMonthCard");
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();
        }
    }
    #endregion

    #region Charge tools
    bool dartTools = false;
    void DartTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
//        dartTools = GUILayout.Toggle(dartTools, "Dart Tools", btnStyle); TODOX
//        if (dartTools)
//        {
//            GUILayout.BeginVertical();

//            FuncTemplateManager.IsNeedArenaRankLimit = Toggle(FuncTemplateManager.IsNeedArenaRankLimit, "Need Arena Rank Limit?");

//            if (GUILayout.Button("Transfer Dart", GUILayout.Width(250)))
//            {
//                SpecialActivityTemplate temp = EventTemplateManager.Instance.GetSpecialActivity(9005);
//                switch (temp.nav_type)
//                {
//                    case eActivityNavType.NpcFind:
//                        string[] splits = temp.nav_parameter.Split(';');
//                        if (splits != null && splits.Length == 2)
//                        {
//                            string scene_name = null;
//#if ILRuntime
//                            object instance = HotfixILRManager.GetInstance().appdomain.Invoke(GetHotfixMthod.Instance.GetHotfixMethodInILR("Hotfix_LT.UI.MainLandLogic", "GetInstance", 0), null, null);
//                            if (instance != null)
//                            {
//                                scene_name = (string)GetHotfixMthod.Instance.GetHotfixTypeAttributeInILR("Hotfix_LT.UI.MainLandLogic", "CurrentSceneName", instance,true,true);
//                            }
//#else
//                            object instance = GetHotfixMthod.Instance.GetHotfixMethodInMono("Hotfix_LT.UI.MainLandLogic", "GetInstance", null).Invoke(null,null);
//                            if (instance != null)
//                            {
//                                scene_name = (string)GetHotfixMthod.Instance.GetHotfixTypeAttributeInMono("Hotfix_LT.UI.MainLandLogic", "CurrentSceneName", instance,true,true);
//                            }
//#endif
//                            WorldMapPathManager.Instance.StartPathFindToNpcFly(scene_name/*MainLandLogic.GetInstance().CurrentSceneName*/, splits[0], splits[1]);
//                        }
//                        break;
//                    default:
//                        EB.Debug.LogError("SpecialActivityTemplate nav_type error =" + temp.nav_type);
//                        break;
//                }
//            }
//            if (GUILayout.Button("OpenTransferDartPanel", GUILayout.Width(400)))
//            {
//                //ToDo: 暂时屏蔽，方便解耦
//                //GlobalMenuManager.Instance.Open("DartTransferView");
//            }
//            if (GUILayout.Button("OpenRobDartPanel", GUILayout.Width(400)))
//            {
//                //ToDo: 暂时屏蔽，方便解耦
//                //GlobalMenuManager.Instance.Open("DartRobView");
//            }
//            GUILayout.EndHorizontal();
//        }
    }
    #endregion

    #region Guide tools
    bool guidetools = false;
    string guideid = string.Empty;
    void GuideTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        guidetools = GUILayout.Toggle(guidetools, "Guide Tools", btnStyle);
        if (guidetools)
        {
            GUILayout.BeginHorizontal();
            guideid = GUILayout.TextField(guideid, 10, GUILayout.Width(200));
            if (GUILayout.Button("Set ID", GUILayout.Width(200)))
            {
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.GuideManager", "Instance", "PostProgressToServer", int.Parse(guideid));
            }
            GUILayout.EndHorizontal();
        }
    }
    #endregion

    #region Pay tools
    bool paytools = false;
    string paynum = string.Empty;
    void PayTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        paytools = GUILayout.Toggle(paytools, "Pay Tools", btnStyle);
        if (paytools)
        {
            GUILayout.BeginHorizontal();
            paynum = GUILayout.TextField(paynum, 10, GUILayout.Width(200));
            if (GUILayout.Button("Set Num", GUILayout.Width(200)))
            {
                DataLookupSparxManager lookupsManager = EB.Sparx.Hub.Instance.GetManager<DataLookupSparxManager>();
                EB.Sparx.Request request = lookupsManager.EndPoint.Post("/userres/debugBuyHc");
                request.AddData("num", paynum);
                lookupsManager.Service(request);
            }
            GUILayout.EndHorizontal();
        }
    }
    #endregion

    #region ParticleTest tools
    bool particletools = false;
    bool effecttools = false;
    bool fxtools = false;
    private GameObject effect;
    private GameObject fx;
    void ParticleTestTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        particletools = GUILayout.Toggle(particletools, "Particle Tools", btnStyle);
        if (particletools)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Particle open", GUILayout.Width(200)))
            {
                if (fx == null) return;
                fx.SetActive(true);
            }

            if (GUILayout.Button("Particle close", GUILayout.Width(200)))
            {
                fx = GameObject.Find("FX");
                fx.SetActive(false);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Effect open", GUILayout.Width(200)))
            {
                if (effect == null) return;
                effect.SetActive(true);
            }

            if (GUILayout.Button("Effect close", GUILayout.Width(200)))
            {
                effect = GameObject.Find("Effect");
                effect.SetActive(false);
            }
            GUILayout.EndHorizontal();

            fxtools = GUILayout.Toggle(fxtools, "FX", btnStyle);
            if (fxtools)
            {
                effect = GameObject.Find("FX");
                if (effect != null)
                {
                    GUILayout.BeginVertical();
                    for (int i = 0; i < effect.transform.childCount; i++)
                    {
                        Transform child = effect.transform.GetChild(i);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(child.name, GUILayout.Width(480)))
                        {
                            child.gameObject.SetActive(false);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
            }
            effecttools = GUILayout.Toggle(effecttools, "Effect", btnStyle);
            if (effecttools)
            {
                effect = GameObject.Find("Effect");
                if (effect != null)
                {
                    GUILayout.BeginVertical();
                    for (int i = 0; i < effect.transform.childCount; i++)
                    {
                        Transform child = effect.transform.GetChild(i);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(child.name, GUILayout.Width(480)))
                        {
                            child.gameObject.SetActive(false);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
            }
        }
    }
    #endregion

    bool disppear = false;
    void DisppearMesh(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        disppear = GUILayout.Toggle(disppear, "Disppear Mesh", btnStyle);
        if (disppear)
        {
            GameObject obj = GameObject.Find("z_merged");
            if (null != obj)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(tabSize);
                GUILayout.BeginVertical();

                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    string itemName = obj.transform.GetChild(i).name;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(itemName, GUILayout.Width(640)))
                    {
                        obj.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }
    }

    public static bool shadowok = true;
    public static bool posteffect = true;
    bool PostEffect = false;
    void DisablePostEffect(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        PostEffect = GUILayout.Toggle(PostEffect, "Disable PostEffect", btnStyle);
        if (PostEffect)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("PostEffect OK", GUILayout.Width(640)))
            {
                PostFXManagerTrigger tt = Camera.main.GetComponent<PostFXManagerTrigger>();
                tt.enabled = true;
            }
            if (GUILayout.Button("PostEffect No", GUILayout.Width(640)))
            {
                PostFXManagerTrigger tt = Camera.main.GetComponent<PostFXManagerTrigger>();
                tt.enabled = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Shadow OK", GUILayout.Width(640)))
            {
                Light[] lights = Light.GetLights(LightType.Directional, LayerMask.NameToLayer("Default"));
                if (lights != null && lights.Length > 0)
                {
                    lights[0].enabled = true;
                }
            }
            if (GUILayout.Button("Shadow No", GUILayout.Width(640)))
            {
                Light[] lights = Light.GetLights(LightType.Directional, LayerMask.NameToLayer("Default"));
                if (lights != null && lights.Length > 0)
                {
                    lights[0].enabled = false;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Nav No", GUILayout.Width(640)))
            {
                GameObject obj = GameObject.Find("NavMesh (1)");
                if (null != obj)
                {
                    obj.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

        }
    }

    bool _performance = false;
    void Performance(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        _performance = GUILayout.Toggle(_performance, "Performance", btnStyle);
        if (_performance)
        {
            GUILayout.BeginVertical();
            GUILayout.TextField("Set Before Login!!!");
            if (GUILayout.Button("High", GUILayout.Width(640)))
            {
                EB.Sparx.PerformanceManager.EnablePerformanceDebug = true;
                EB.Sparx.PerformanceManager.PerfrmaceLevelDebug = 3;
            }
            if (GUILayout.Button("Medium", GUILayout.Width(640)))
            {
                EB.Sparx.PerformanceManager.EnablePerformanceDebug = true;
                EB.Sparx.PerformanceManager.PerfrmaceLevelDebug = 2;
            }
            if (GUILayout.Button("Low", GUILayout.Width(640)))
            {
                EB.Sparx.PerformanceManager.EnablePerformanceDebug = true;
                EB.Sparx.PerformanceManager.PerfrmaceLevelDebug = 1;
            }
            GUILayout.EndVertical();
        }
    }

    bool crashtools = false;
    void CrashCatchTestTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        crashtools = GUILayout.Toggle(crashtools, "Crash Tools", btnStyle);
        if (crashtools)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Make Bug"))
            {
                DebugSystem.Close();
                EB.BugReport.MakeBug();
            }

            if (GUILayout.Button("Create Crash"))
            {
                DebugSystem.Close();
                EB.BugReport.CreateCrash();
            }

            if (GUILayout.Button("Logout"))
            {
                DebugSystem.Close();
                SparxHub.Instance.Disconnect(true);
            }

            if (GUILayout.Button("Relogin"))
            {
                DebugSystem.Close();
                SparxHub.Instance.Disconnect(false);
            }

            if (GUILayout.Button("Fatal Error"))
            {
                DebugSystem.Close();
                SparxHub.Instance.FatalError("Fatal Error");
            }

            GUILayout.EndHorizontal();
        }
    }

    bool notificationtools = false;
    bool repeatPerDay = false;
    string notificationDelay = "10.0";
    void NotificationTestTools(GUIStyle btnStyle, GUILayoutOption btnWidth)
    {
        notificationtools = GUILayout.Toggle(notificationtools, "Notification Tools", btnStyle);
        if (notificationtools)
        {
            repeatPerDay = Toggle(repeatPerDay, "Repeat Per Day?");

            GUILayout.BeginHorizontal();

            GUILayout.Label("Delay:", GUILayout.Width(150));
            notificationDelay = GUILayout.TextField(notificationDelay);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Schedule Local Notification"))
            {
                double delay = 10.0;
                if (!double.TryParse(notificationDelay, out delay))
                {
                    delay = 10.0f;
                }
                if (repeatPerDay)
                {
                    SparxHub.Instance.PushManager.ScheduleDailyLocalNotification("Test Title", "Test Message", System.DateTime.Now.AddSeconds(delay));
                }
                else
                {
                    SparxHub.Instance.PushManager.ScheduleOnceLocalNotification("Test Title", "Test Message", System.DateTime.Now.AddSeconds(delay));
                }
            }

            if (GUILayout.Button("Clear Local Notification"))
            {
                SparxHub.Instance.PushManager.CleanNotification();
            }

            GUILayout.EndHorizontal();
        }
    }

#endif
}



