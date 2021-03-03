using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public enum ePressTipAnchorType
    {
        RightUp = 1,
        RightDown = 2,
        LeftDown = 3,
        LeftUp = 4
    }

    public class UITooltipManager : MenuManager
    {
        [System.Serializable]
        public struct TooltipLibraryEntry
        {
            public string systemName;
            public string contextName;
            public GameObject tooltipObject;
        }

        public List<TooltipLibraryEntry> tooltipLibrary;

        public GameObject tooltipDataPanelRoot;

        public float PanelOffsetX = 0;
        public float PanelOffsetY = 0;

        public string ShowToolTipSound = "UI/General/ToolTip/Open";
        public string HideToolTipSound = "UI/General/ToolTip/Close";

        private Dictionary<string, GameObject> m_tooltipDataPanelDic;
        private GameObject m_currentTooltipObject;
        private string m_tooltipSystem;

        [HideInInspector] public string curTemplateid = null;
        [HideInInspector] public string selectTemplateid = null;

        public void CheakToolTip()
        {
            if (curTemplateid != null)
            {
                DisplayTooltipSrc(curTemplateid, "Generic", "default");
                curTemplateid = null;
            }
        }
        public void CleanToolTip()
        {
            if (selectTemplateid != null) selectTemplateid = null;
            if (curTemplateid != null) curTemplateid = null;
        }

        private static UITooltipManager instance;
        public static UITooltipManager Instance
        {
            get
            {
                if (instance == null)
                {
                    EB.Debug.LogWarning("UITooltipManager is not awake yet!");
                }
                return instance;
            }
        }

        private int m_AwakeLoadedFinishedCount = 0;

        void CheckInitToolTips()
        {
            if(m_AwakeLoadedFinishedCount == 2){
                for (int i = 0; i < tooltipLibrary.Count; i++)
                {
                    if (!m_tooltipDataPanelDic.ContainsKey(tooltipLibrary[i].tooltipObject.name))
                    {
                        GameObject tooltipObj = NGUITools.AddChild(tooltipDataPanelRoot, tooltipLibrary[i].tooltipObject);
                        tooltipObj.CustomSetActive(false);
                        tooltipObj.name = tooltipLibrary[i].tooltipObject.name;
                        m_tooltipDataPanelDic.Add(tooltipObj.name, tooltipObj);
                    }
                }
            }
        }

        public override void Awake()
        {
            base.Awake();

            instance = this;

            tooltipDataPanelRoot = mDMono.transform.FindEx("DataPanels").gameObject;
            PanelOffsetX = -0.6f;
            PanelOffsetY = -0.2f;

            m_tooltipDataPanelDic = new Dictionary<string, GameObject>();

            tooltipLibrary = new List<TooltipLibraryEntry>(){
                default(TooltipLibraryEntry), default(TooltipLibraryEntry)
            };

            m_AwakeLoadedFinishedCount = 0;

            string str_generic = "_GameAssets/Res/Prefabs/UIPrefabs/Tooltip/DataPanelNew_generic_default";
            EB.Assets.LoadAsync(str_generic, typeof(GameObject), o=>{
                if(!o){
                    return;
                }
                tooltipLibrary[0] = new TooltipLibraryEntry()
                {
                    systemName = "Generic",
                    contextName = "default",
                    tooltipObject = o as GameObject,
                };
                m_AwakeLoadedFinishedCount+= 1;
                CheckInitToolTips();
            });

            string str_skill = "_GameAssets/Res/Prefabs/UIPrefabs/Tooltip/DataPanelNew_skill_default";
            EB.Assets.LoadAsync(str_skill, typeof(GameObject), o=>{
                if(!o)
                {
                    return;
                }
                tooltipLibrary[1] = new TooltipLibraryEntry() 
                {
                    systemName = "Skill",
                    contextName = "default",
                    tooltipObject = o as GameObject,
                };
                m_AwakeLoadedFinishedCount+=1;
                CheckInitToolTips();
            });
        }

        public override void OnDestroy()
        {
            instance = null;

            base.OnDestroy();
        }

        public void DisplayTooltip(string dataID)
        {
            if (string.IsNullOrEmpty(dataID) || m_currentTooltipObject != null) return;

            ClearTooltipCache();
            StoreTooltipDataIntoCache(dataID);

            if (!string.IsNullOrEmpty(m_tooltipSystem))
            {
                TooltipObjectDisplay();
            }
            else
            {
                EB.Debug.LogWarning("Tooltip Target is NULL!!");
            }
        }

        public void DisplayTooltipForPress(string dataID, string tooltipSystem, string context, Vector3 pos, ePressTipAnchorType anchor, bool isSkillPanel = false, bool isCanUse = false, bool isHaveUse = false, System.Action useCallback = null)
        {
            if (string.IsNullOrEmpty(dataID) || m_currentTooltipObject != null) return;
            m_tooltipSystem = tooltipSystem;
            ClearTooltipCache();
            StoreData("context", context);
            StoreData("target", dataID);

            if (!string.IsNullOrEmpty(m_tooltipSystem))
            {
                TooltipObjectDisplayPress(pos, anchor, isSkillPanel, isCanUse, isHaveUse, useCallback);
            }
            else
            {
                EB.Debug.LogWarning("Tooltip Target is NULL!!");
            }
        }

        /// <summary>
        /// 显示技能描述弹框
        /// </summary>
        public void DisplaySkillTip(string dataID, Vector3 pos, ePressTipAnchorType anchor)
        {
            if (string.IsNullOrEmpty(dataID) || m_currentTooltipObject != null) return;
            DisplayTooltipForPress(dataID, "Skill", "default", pos, anchor, false, false, false, delegate () { });
        }

        /// <summary>
        /// 显示物品原始信息框
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="tooltipSystem"></param>
        /// <param name="context"></param>
        public void DisplayTooltipSrc(string dataID, string tooltipSystem, string context)
        {
            if (string.IsNullOrEmpty(dataID) || m_currentTooltipObject != null) return;
            m_tooltipSystem = tooltipSystem;
            ClearTooltipCache();
            StoreData("context", context);
            StoreData("target", dataID);
            StoreData("unhave", "0");
            UITooltipManager.Instance.selectTemplateid = dataID;
            if (!string.IsNullOrEmpty(m_tooltipSystem))
            {
                TooltipObjectDisplay();
            }
            else
            {
                EB.Debug.LogWarning("Tooltip Target is NULL!!");
            }
        }

        public static void DisplayTooltipSrcFromILR(string dataID, string tooltipSystem, string context)
        {
            instance.DisplayTooltipSrc(dataID, tooltipSystem, context);
        }

        /// <summary>
        /// 显示资源来源框
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="tooltipSystem"></param>
        /// <param name="context"></param>
        public void DisplayResSrcTooltip(string id, string tooltipSystem, string context, Vector3 pos, ePressTipAnchorType anchor)
        {
            if (string.IsNullOrEmpty(id) || m_currentTooltipObject != null) return;
            m_tooltipSystem = tooltipSystem;
            ClearTooltipCache();
            StoreData("context", context);
            StoreData("target", id);
            if (!string.IsNullOrEmpty(m_tooltipSystem))
            {
                ResSrcTooltipObjectDisplay(pos, anchor);
            }
            else
            {
                EB.Debug.LogWarning("Tooltip Target is NULL!!");
            }
        }

        void StoreTooltipDataIntoCache(string dataID)
        {
            object dataObj;
            DataLookupsCache.Instance.SearchDataByID(dataID, out dataObj);

            if (null != dataObj && !string.IsNullOrEmpty(dataObj.ToString()))
            {
                if (dataObj is IDictionary)
                {
                    SetContextData((IDictionary)dataObj);
                    StoreData("target", dataID);
                }
                else
                {
                    object refDataObj;
                    DataLookupsCache.Instance.SearchDataByID(dataObj.ToString(), out refDataObj);

                    if (null != refDataObj)
                    {
                        SetContextData((IDictionary)refDataObj);
                        StoreData("target", dataObj.ToString());
                    }
                }
            }
        }

        void SetContextData(IDictionary targetHash)
        {
            string context = "default";
            if (targetHash["system"] != null && targetHash["system"].ToString() == "Equipment" &&
               targetHash["location"] != null && targetHash["location"].ToString() == "bag_items")
            {
                string type = targetHash["equipment_type"].ToString();

                if (type == "Ring" || type == "Jewelry")
                {
                    object dataObjL;
                    object dataObjR;

                    string dataIdL = string.Format("equippedItems.{0}0", type);
                    string dataIdR = string.Format("equippedItems.{0}1", type);

                    bool hasL = DataLookupsCache.Instance.SearchDataByID(dataIdL, out dataObjL);
                    bool hasR = DataLookupsCache.Instance.SearchDataByID(dataIdR, out dataObjR);

                    if (hasL && hasR)
                    {
                        context = "triple";
                        StoreData("compare", dataIdL);
                        StoreData("compare_2", dataIdR);
                    }
                    else if (hasL || hasR)
                    {
                        context = "double";
                        StoreData("compare", hasL ? dataIdL : dataIdR);
                    }
                }
                else
                {
                    object dataObj;

                    string dataId = string.Format("equippedItems.{0}", type);
                    bool hasObj = DataLookupsCache.Instance.SearchDataByID(dataId, out dataObj);

                    if (hasObj && !string.IsNullOrEmpty(dataObj.ToString()))
                    {
                        context = "double";
                        StoreData("compare", dataObj.ToString());
                    }
                }
            }

            if (targetHash["system"] != null)
            {
                m_tooltipSystem = targetHash["system"].ToString();
            }

            Hashtable tooltipContextData = Johny.HashtablePool.Claim();
            tooltipContextData["context"] = context;
            Hashtable tooltipData = Johny.HashtablePool.Claim();
            tooltipData["tooltip"] = tooltipContextData;
            DataLookupsCache.Instance.CacheData(tooltipData);
            Johny.HashtablePool.Release(tooltipData);
            tooltipData = null;
        }

        void StoreData(string location, string dataID)
        {
            Hashtable tooltipTargetData = Johny.HashtablePool.Claim();
            tooltipTargetData[location] = dataID;
            Hashtable tooltipData = Johny.HashtablePool.Claim();
            tooltipData["tooltip"] = tooltipTargetData;
            DataLookupsCache.Instance.CacheData(tooltipData);
            Johny.HashtablePool.Release(tooltipData);
            tooltipData = null;
        }

        void TooltipObjectDisplay()
        {
            if (!string.IsNullOrEmpty(m_tooltipSystem))
            {
                string tooltipContext = "default";

                DataLookupsCache.Instance.SearchDataByID<string>("tooltip.context", out tooltipContext);

                TooltipLibraryEntry targetEntry = tooltipLibrary.Find(x =>
                                                                      x.systemName == m_tooltipSystem &&
                                                                      x.contextName == tooltipContext
                                                                      );

                if (null != targetEntry.tooltipObject && null == m_currentTooltipObject)
                {
                    MessageDialog.HideCurrent();
                    m_currentTooltipObject = m_tooltipDataPanelDic[targetEntry.tooltipObject.name];
                    m_currentTooltipObject.CustomSetActive(true);
                    if (m_currentTooltipObject.GetComponent<UIController>())
                        m_currentTooltipObject.GetComponent<UIController>().Open();
                }
            }
        }
        void TooltipObjectDisplayPress(Vector3 pos, ePressTipAnchorType anchor, bool isSkillPanel = false, bool isCanUse = false, bool isHaveUse = false, System.Action useCallback = null)
        {
            if (!string.IsNullOrEmpty(m_tooltipSystem))
            {
                string tooltipContext = "default";

                DataLookupsCache.Instance.SearchDataByID<string>("tooltip.context", out tooltipContext);

                TooltipLibraryEntry targetEntry = tooltipLibrary.Find(x =>
                                                                      x.systemName == m_tooltipSystem &&
                                                                      x.contextName == tooltipContext
                                                                      );

                if (null != targetEntry.tooltipObject && null == m_currentTooltipObject)
                {
                    MessageDialog.HideCurrent();
                    m_currentTooltipObject = m_tooltipDataPanelDic[targetEntry.tooltipObject.name];

                    //if (m_currentTooltipObject.GetComponent<UISkillDescContorller>())
                    //{
                    //    m_currentTooltipObject.GetComponent<UISkillDescContorller>().SetData(pos);
                    //}
                    var ilr = m_currentTooltipObject.GetComponent<DynamicMonoILR>();
                    if (ilr != null && ilr.hotfixClassPath.Equals("Hotfix_LT.UI.UISkillDescContorller"))
                    {
                        ilr.FloatParamList = new List<float>() { pos.x, pos.y, pos.z };
                    }

                    m_currentTooltipObject.CustomSetActive(true);
                    if (m_currentTooltipObject.GetComponent<UIController>())
                    {
                        m_currentTooltipObject.GetComponent<UIController>().Open();
                    }
                }
            }
        }

        void ResSrcTooltipObjectDisplay(Vector3 pos, ePressTipAnchorType anchor)
        {
            if (!string.IsNullOrEmpty(m_tooltipSystem))
            {
                string tooltipContext = "default";

                DataLookupsCache.Instance.SearchDataByID<string>("tooltip.context", out tooltipContext);

                TooltipLibraryEntry targetEntry = tooltipLibrary.Find(x =>
                                                                      x.systemName == m_tooltipSystem &&
                                                                      x.contextName == tooltipContext
                                                                      );

                if (null != targetEntry.tooltipObject && null == m_currentTooltipObject)
                {
                    MessageDialog.HideCurrent();
                    m_currentTooltipObject = m_tooltipDataPanelDic[targetEntry.tooltipObject.name];
                    m_currentTooltipObject.GetComponent<UITooltipPressPos>().SetPosAndAnchor(pos, anchor);
                    m_currentTooltipObject.CustomSetActive(true);
                    if (m_currentTooltipObject.GetComponent<UIController>())
                        m_currentTooltipObject.GetComponent<UIController>().Open();
                }
            }
        }

        public void HideTooltipForPress()
        {
            foreach (KeyValuePair<string, GameObject> tooltipPair in m_tooltipDataPanelDic)
            {
                tooltipPair.Value.CustomSetActive(false);
            }

            ClearTooltipCache();
            //Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.PressReleaseEvent, OnPressReleaseEvent);
            if (m_currentTooltipObject != null && m_currentTooltipObject.GetComponent<UIController>())
                m_currentTooltipObject.GetComponent<UIController>().Close();
            m_currentTooltipObject = null;
            m_tooltipSystem = string.Empty;
        }

        //void OnPressReleaseEvent()
        //{
        //    HideTooltipForPress();
        //}

        public void HideTooltip()
        {
            foreach (KeyValuePair<string, GameObject> tooltipPair in m_tooltipDataPanelDic)
            {
                tooltipPair.Value.CustomSetActive(false);
            }

            ClearTooltipCache();
            m_currentTooltipObject = null;
            m_tooltipSystem = string.Empty;
        }

        void ClearTooltipCache()
        {
            Hashtable tooltipClearHash = Johny.HashtablePool.Claim();
            tooltipClearHash["tooltip"] = null;
            DataLookupsCache.Instance.CacheData(tooltipClearHash);
            Johny.HashtablePool.Release(tooltipClearHash);
            tooltipClearHash = null;
        }
    }
}