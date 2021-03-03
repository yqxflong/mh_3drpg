using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using Hotfix_LT.Player;

namespace Hotfix_LT.UI
{
    [System.Serializable]
    public class FilterSettingsValue
    {
        public System.Predicate<IDictionary> Filter; // Filter must never be NULL. If no filter needed, set one that always returns TRUE
        public InspectablePredicate InspectableFilter; // InspectableFilter is never NULL cause it shows up in the Inspector
        public Vector2 Size;
    }

    [System.Serializable]
    public class InspectableSelectionMapEntry
    {
        public InspectablePredicate Predicate;
        public UIMHBindableItemDataLookUp Slot;
    }

    public class UIInventoryBagLogic : DynamicMonoHotfix
    {
        private static UIInventoryBagLogic _Instance;
        public static UIInventoryBagLogic Instance
        {
            get { return _Instance; }
        }
    
        public string m_InventoryBagPath;
        public UIInventoryGridScroll m_gridScroll;
        public System.Comparison<object> SortingComparison;
        protected UIGrid m_Grid;
        protected int m_Row;
        protected int m_Column;
        public FilterSettingsValue FilterSetting;
    
        private void InitFilterSettings()
        {
            FilterSetting.Filter = FilterSetting.InspectableFilter.ToNativePredicate();
        }
    
        public class SelectionFilter
        {
            public System.Predicate<IDictionary> Predicate;
            public UIMHBindableItemDataLookUp Slot;
        }
    
        public List<InspectableSelectionMapEntry> m_TargetSlotsMapEntrys;
        private List<SelectionFilter> m_TargetSlotsFilter;
    
        private void InitSelectionHandling()
        {
            m_TargetSlotsFilter = new List<SelectionFilter>();
            for (var i = 0; i < m_TargetSlotsMapEntrys.Count; i++)
            {
                InspectableSelectionMapEntry entry = m_TargetSlotsMapEntrys[i];
                SelectionFilter filter = new SelectionFilter();
                filter.Slot = entry.Slot;
                filter.Predicate = entry.Predicate.ToNativePredicate();
                m_TargetSlotsFilter.Add(filter);
            }
        }
    
        public UIMHBindableItemDataLookUp GetAvaliableSlot(IDictionary itemdata)
        {
    
            for (int i = 0; i < m_TargetSlotsFilter.Count; i++)
            {
                if (m_TargetSlotsFilter[i].Predicate(itemdata))
                {
                    return m_TargetSlotsFilter[i].Slot;
                }
            }
    
            return null;
        }
    
        public UIMHBindableItemDataLookUp GetEmptyAvaliableSlot(IDictionary itemdata)
        {
    
            for (int i = 0; i < m_TargetSlotsFilter.Count; i++)
            {
                if (m_TargetSlotsFilter[i].Predicate(itemdata) && m_TargetSlotsFilter[i].Slot.IsEmpty)
                {
                    return m_TargetSlotsFilter[i].Slot;
                }
            }
    
            return null;
        }
    
        private void ItemsCollectionList(List<DictionaryEntry> ll)
        {
            IDictionary itemDataCollection;

            DataLookupsCache.Instance.SearchDataByID<IDictionary>(m_InventoryBagPath, out itemDataCollection);
            if (itemDataCollection == null)
                return;

            System.Predicate<IDictionary> filter = FilterSetting.Filter;
            var it = itemDataCollection.GetEnumerator();
            while(it.MoveNext()){
                var ht = it.Value as IDictionary;
                if(filter(ht)){
                    ll.Add(new DictionaryEntry(it.Key, it.Value));
                }
            }
        }
    
        protected string GetItemDataIDFromKey(string key)
        {
            return m_InventoryBagPath + "." + key;
        }

        public override void Awake()
        {
            base.Awake();

            if (mDMono.StringParamList != null && mDMono.StringParamList.Count > 0)
            {
                m_InventoryBagPath = mDMono.StringParamList[0];
            }

            var t = mDMono.transform;
            m_gridScroll = t.GetMonoILRComponentByClassPath<UIInventoryGridScroll>("Hotfix_LT.UI.UIInventoryGridScroll");

            FilterSetting = new FilterSettingsValue();
            FilterSetting.Size = new Vector2(4, 5);
            FilterSetting.InspectableFilter = new InspectablePredicate();
            FilterSetting.InspectableFilter.Query = new List<InspectablePredicate.Part>()
            {
                new InspectablePredicate.Part()
                {
                    PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                    PropertyName = "location",
                    ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.NOT_EQUAL_TO,
                    Value = "equipment"
                },
                new InspectablePredicate.Part()
                {
                    PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                    PropertyName = "location",
                    ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.NOT_EQUAL_TO,
                    Value = "socket"
                }
            };

            m_TargetSlotsMapEntrys = new List<InspectableSelectionMapEntry>()
            {
                new InspectableSelectionMapEntry()
                {
                    Predicate = new InspectablePredicate()
                    {
                        Query = new List<InspectablePredicate.Part>()
                        {
                            new InspectablePredicate.Part()
                            {
                                PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                                PropertyName = "system",
                                ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.EQUAL_TO,
                                Value = "Equipment"
                            },
                            new InspectablePredicate.Part()
                            {
                                PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                                PropertyName = "equipment_type",
                                ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.EQUAL_TO,
                                Value = "1"
                            }
                        }
                    }
                },
                new InspectableSelectionMapEntry()
                {
                    Predicate = new InspectablePredicate()
                    {
                        Query = new List<InspectablePredicate.Part>()
                        {
                            new InspectablePredicate.Part()
                            {
                                PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                                PropertyName = "system",
                                ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.EQUAL_TO,
                                Value = "Equipment"
                            },
                            new InspectablePredicate.Part()
                            {
                                PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                                PropertyName = "equipment_type",
                                ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.EQUAL_TO,
                                Value = "2"
                            }
                        }
                    }
                },
                new InspectableSelectionMapEntry()
                {
                    Predicate = new InspectablePredicate()
                    {
                        Query = new List<InspectablePredicate.Part>()
                        {
                            new InspectablePredicate.Part()
                            {
                                PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                                PropertyName = "system",
                                ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.EQUAL_TO,
                                Value = "Equipment"
                            },
                            new InspectablePredicate.Part()
                            {
                                PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                                PropertyName = "equipment_type",
                                ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.EQUAL_TO,
                                Value = "3"
                            }
                        }
                    }
                },
                new InspectableSelectionMapEntry()
                {
                    Predicate = new InspectablePredicate()
                    {
                        Query = new List<InspectablePredicate.Part>()
                        {
                            new InspectablePredicate.Part()
                            {
                                PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                                PropertyName = "system",
                                ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.EQUAL_TO,
                                Value = "Equipment"
                            },
                            new InspectablePredicate.Part()
                            {
                                PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                                PropertyName = "equipment_type",
                                ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.EQUAL_TO,
                                Value = "4"
                            }
                        }
                    }
                },
                new InspectableSelectionMapEntry()
                {
                    Predicate = new InspectablePredicate()
                    {
                        Query = new List<InspectablePredicate.Part>()
                        {
                            new InspectablePredicate.Part()
                            {
                                PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                                PropertyName = "system",
                                ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.EQUAL_TO,
                                Value = "Equipment"
                            },
                            new InspectablePredicate.Part()
                            {
                                PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND,
                                PropertyName = "equipment_type",
                                ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.EQUAL_TO,
                                Value = "5"
                            }
                        }
                    }
                }
            };
        }

        public override void Start()
        {
            _Instance = this;

            m_Grid = mDMono.transform.GetComponentEx<UIGrid>();

            if (m_Grid.arrangement == UIGrid.Arrangement.Horizontal)
            {
                m_Column = (int)FilterSetting.Size.x;
                m_Row = (int)FilterSetting.Size.y;
            }
            else
            {
                m_Row = (int)FilterSetting.Size.x;
                m_Column = (int)FilterSetting.Size.y;
            }
            InitFilterSettings();
            InitSelectionHandling();
            SortingComparison = SortingComparisonHandler.Comparison;
        }
    
        public override void OnDestroy()
        {
            _Instance = null;
        }
    
        public object FirstItem = null;
        public IDictionary FirstItemData = null;

        private List<DictionaryEntry> _RefeshBag_dics = new List<DictionaryEntry>(512);

        private void RefreshBag_OrderDics(){
            if (SortingComparison != null)
            {
                SortingComparisonHandler._dicQuickQuery.Clear();
                for(int i = 0 ; i < _RefeshBag_dics.Count; i++)
                {
                    var it = _RefeshBag_dics[i];
                    var ht = it.Value as IDictionary;
                    List<int> ll;
                    if(!SortingComparisonHandler._dicQuickQuery.TryGetValue(it.Key, out ll))
                    {
                        ll = new List<int>{-1,-1,-1, -1};
                        SortingComparisonHandler._dicQuickQuery[it.Key] = ll;
                    }

                    ll[0] = Convert.ToInt32(ht["economy_id"]);
                    ll[1] = Convert.ToInt32(ht["qualityLevel"]);
                    ll[2] = Convert.ToInt32(ht["inventory_id"]);
                    ll[3] = SortingComparisonHandler.GetSystemOrderIndex((string)ht["system"]);
                }
                _RefeshBag_dics.Sort((x, y) => SortingComparison(x.Key, y.Key));
                SortingComparisonHandler._dicQuickQuery.Clear();
            }
        }

        private void RefreshBagInner()
        {
            //准备数据
            _RefeshBag_dics.Clear();
            ItemsCollectionList(_RefeshBag_dics);
            RefreshBag_OrderDics();
            
            List<UIInventoryBagCellData> bagdatas = new List<UIInventoryBagCellData>();
            for (int i = 0; i < _RefeshBag_dics.Count; i++)
            {
                bagdatas.Add(new UIInventoryBagCellData(GetItemDataIDFromKey(_RefeshBag_dics[i].Key.ToString())));
                if (FirstItem == null)
                {
                    FirstItem = _RefeshBag_dics[i].Value;
                    DataLookupsCache.Instance.SearchDataByID<IDictionary>(bagdatas[bagdatas.Count - 1].m_DataID, out FirstItemData);
                }
            }
            int less = _RefeshBag_dics.Count % m_Column == 0 ? 0 : m_Column - _RefeshBag_dics.Count % m_Column;
            for (int i = 0; i < less; i++)
            {
                bagdatas.Add(new UIInventoryBagCellData(null));
            }
    
            for (int i = bagdatas.Count; i < 25; i++)
            {
                // 保证背包ui的显示，至少显示5*5；
                bagdatas.Add(null);
            }
            m_gridScroll.SetItemDatas(bagdatas);
            if (FirstItem != null)
            {
                this.SetBagContent(FirstItem);
                ShowBagContent.Instance.BagItemData = FirstItemData;
            }
            else{
                this.SetBagContent(null);
            }

            _RefeshBag_dics.Clear();
        }

        public void RefeshBag(int id = 0)
        {
            if (id == 0) {
                RefreshBagInner(); 
                return; 
            }

            //搜索数据
            _RefeshBag_dics.Clear();
            ItemsCollectionList(_RefeshBag_dics);
            RefreshBag_OrderDics();

            List<UIInventoryBagCellData> bagdatas;
            int length;
            ClassifyItems(id, _RefeshBag_dics, out bagdatas, out FirstItem, out FirstItemData, out length, FirstItem != null);
    
            int less = length % m_Column == 0 ? 0 : m_Column - length % m_Column;
            for (int i = 0; i < less; i++)
            {
                bagdatas.Add(new UIInventoryBagCellData(null));
            }
    
            if (FirstItem != null)
            {
                this.SetBagContent(FirstItem);
                ShowBagContent.Instance.BagItemData = FirstItemData;
            }
            else
                this.SetBagContent(null);
    
            for (int i = bagdatas.Count; i < 25; i++)
            {
                // 保证背包ui的显示，至少显示5*5；
                bagdatas.Add(null);
            }
            m_gridScroll.SetItemDatas(bagdatas);

            _RefeshBag_dics.Clear();
        }
    
        private void ClassifyItems(int id, List<DictionaryEntry> entries, out List<UIInventoryBagCellData> bagdatas, out object FirstItem, out IDictionary FirstItemData, out int length, bool FirstIsFill = false)
        {
            length = 0;
            if (FirstIsFill)
            {
                FirstItem = this.FirstItem;
                FirstItemData = this.FirstItemData;
            }
            else
            {
                FirstItem = null;
                FirstItemData = null;
            }
            bagdatas = new List<UIInventoryBagCellData>();
            if (id == 1)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    int templateid = EB.Dot.Integer("economy_id", entries[i].Value, 0);
                    Data.EconemyItemTemplate eit = Data.EconemyTemplateManager.Instance.GetItem(templateid);
                    Data.GeneralItemTemplate eitGen = eit as Data.GeneralItemTemplate;
                    if (eitGen != null && (eitGen.System.Equals("Generic") || eitGen.System.Equals("SelectBox")) && (eit as Data.GeneralItemTemplate).CanUse)
                    {
                        bagdatas.Add(new UIInventoryBagCellData(GetItemDataIDFromKey(entries[i].Key.ToString())));
                        length++;
                        if (!FirstIsFill)
                        {
                            FirstItem = entries[i].Value;
                            FirstIsFill = true;
                            DataLookupsCache.Instance.SearchDataByID<IDictionary>(bagdatas[bagdatas.Count - 1].m_DataID, out FirstItemData);
                        }
                    }
                }
            }
            else if (id == 2)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    int templateid = EB.Dot.Integer("economy_id", entries[i].Value, 0);
                    Hotfix_LT.Data.EconemyItemTemplate eit = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(templateid);
                    if (eit is Hotfix_LT.Data.EquipmentItemTemplate)
                    {
                        bagdatas.Add(new UIInventoryBagCellData(GetItemDataIDFromKey(entries[i].Key.ToString())));
                        length++;
                        if (!FirstIsFill)
                        {
                            FirstItem = entries[i].Value;
                            FirstIsFill = true;
                            DataLookupsCache.Instance.SearchDataByID<IDictionary>(bagdatas[bagdatas.Count - 1].m_DataID, out FirstItemData);
                        }
                    }
                }
            }
            else if (id == 3)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    int templateid = EB.Dot.Integer("economy_id", entries[i].Value, 0);
                    Hotfix_LT.Data.EconemyItemTemplate eit = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(templateid);
                    if (eit is Hotfix_LT.Data.GeneralItemTemplate && (eit as Hotfix_LT.Data.GeneralItemTemplate).System == "HeroShard")
                    {
                        bagdatas.Add(new UIInventoryBagCellData(GetItemDataIDFromKey(entries[i].Key.ToString())));
                        length++;
                        if (!FirstIsFill)
                        {
                            FirstItem = entries[i].Value;
                            FirstIsFill = true;
                            DataLookupsCache.Instance.SearchDataByID<IDictionary>(bagdatas[bagdatas.Count - 1].m_DataID, out FirstItemData);
                        }
                    }
                }
            }
            else if (id == 4)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    int templateid = EB.Dot.Integer("economy_id", entries[i].Value, 0);
                    Hotfix_LT.Data.EconemyItemTemplate eit = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(templateid);
                    if (eit is Hotfix_LT.Data.GeneralItemTemplate && (eit as Hotfix_LT.Data.GeneralItemTemplate).System == "Generic" && !(eit as Hotfix_LT.Data.GeneralItemTemplate).CanUse)
                    {
                        bagdatas.Add(new UIInventoryBagCellData(GetItemDataIDFromKey(entries[i].Key.ToString())));
                        length++;
                        if (!FirstIsFill)
                        {
                            FirstItem = entries[i].Value;
                            FirstIsFill = true;
                            DataLookupsCache.Instance.SearchDataByID<IDictionary>(bagdatas[bagdatas.Count - 1].m_DataID, out FirstItemData);
                        }
                    }
                }
            }
            else
            {
                EB.Debug.LogError(EB.Localizer.GetString("ID_codefont_in_UIInventoryBagLogic_11306"));
            }
        }
    
        private void SetBagContent(object cell)
        {
            if (cell != null)
            {
                ShowBagContent.Instance.Icon.transform.parent.gameObject.CustomSetActive(true);
                ShowBagContent.Instance.SetBagContentData(cell as IDictionary);
            }
            else
            {
                ShowBagContent.Instance.Icon.transform.parent.gameObject.CustomSetActive(false);
                ShowBagContent.Instance.SetBagContentData(null);
            }
            LTInventoryAllController.SelectFirst(EB.Dot.String("inventory_id", cell, null));
        }
    
        public void RefreshBagItem()
        {
            if (LTInventoryAllController._CurrentSelectCell != null)
            {
                LTInventoryAllController._CurrentSelectCell.UpdateUI();
                ShowBagContent.Instance.SetBagContent(LTInventoryAllController._CurrentSelectCell);
            }
        }

        
        /// <summary>
        /// 不支持异步，Coroutine等
        /// </summary>
        private static class SortingComparisonHandler
        {
            // comparison rule from the spec :
            //
            // Sorting of inventory items passes through four filters in this order:
            // System -> (Equip, Gem, etc.) -> Level -> (Descending) -> Quality -> (Descending)
            //
            // 1) Priority Weight - a configurable value to allow items to be prioritised over others in the case where there may be promotional items that need to be pushed to the front of the list
            //
            // 2) System - Type of item, whether it is equipment, gems or scrolls. Systems are prioritised as follows:
            //	Equipment > Gem > LuckStone > Scroll > Potion > Chests > Daru > Kyanite > Anything else
            //
            // 3) Level - The character level requirement of the item, sorted in descending order
            // 4) Quality - The quality of the item, either Godly, Epic, Uncommon, Common, or Poor in order from best to worst
            // 5) Slot - The character equipment slot it goes into in order of Wings, Medal, Weapon, Armor, Helmet, Necklace, Rings, Jewellery, and then anything else that doesn't fit into an equip slot doesn't require this filter pass

            /// <summary>
            /// 0: economy_id 1: qualityLevel 2: inventory_id 3：system
            /// </summary>
            /// <returns></returns>
            public static Dictionary<object, List<int>> _dicQuickQuery = new Dictionary<object, List<int>>();

            #region cpu 67ms -> 11 ms
            public static int Comparison(object x, object y)
            {
                int result = 0;
                var ll_x = _dicQuickQuery[x];
                var ll_y = _dicQuickQuery[y];
                int sysindex_generic = GetSystemOrderIndex("Generic");
                int sysindex_selectbox = GetSystemOrderIndex("SelectBox");
                if ((ll_x[3] == sysindex_generic || ll_x[3] == sysindex_selectbox) 
                    && (ll_y[3] == sysindex_generic || ll_y[3] == sysindex_selectbox))
                {
                    result = 0;
                }
                else
                {
                    result = ll_x[3] - ll_y[3];
                }
    
                if (result != 0)
                {
                    return result;
                }
    

                int economyId_x = ll_x[0];
                int economyId_y = ll_y[0];

                bool isGeneric = (ll_x[3] == sysindex_generic || ll_x[3] == sysindex_selectbox);
                if (isGeneric && result == 0)
                {
                    var GeneralEitx = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGeneral(economyId_x);
                    var GeneralEity = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGeneral(economyId_y);
                    if (!GeneralEitx.CanUse && GeneralEity.CanUse)
                    {
                        result = 1;
                    }
                    else if (GeneralEitx.CanUse && !GeneralEity.CanUse)
                    {
                        result = -1;
                    }
                    else
                    {
                        //把召唤书碎片排到可使用物品的最后（策划需求）
                        if (!string.IsNullOrEmpty(GeneralEitx.CompoundItem) && string.IsNullOrEmpty(GeneralEity.CompoundItem))
                        {
                            result = 1;
                        }
                        else if (string.IsNullOrEmpty(GeneralEitx.CompoundItem) && !string.IsNullOrEmpty(GeneralEity.CompoundItem))
                        {
                            result = -1;
                        }
                        else
                        {
                            result = 0;
                        }
                    }
                }
    
                if (result == 0)
                {
                    //按品质排序
                    int quality_x = ll_x[1];
                    int quality_y = ll_y[1];
                    result = QualityOrder.IndexOf(quality_x) - QualityOrder.IndexOf(quality_y);
                }


                if (result == 0)
                {
                    //按id排序
                    result = economyId_x - economyId_y;
                }

                if (result == 0)
                {
                    //按仓库id排序
                    int inventoryId_x = _dicQuickQuery[x][2];
                    int inventoryId_y = _dicQuickQuery[y][2];
                    result = inventoryId_x - inventoryId_y;
                }

                return result;
            }
            #endregion
    
            public static int GetSystemOrderIndex(string system)
            {
                int index = SystemOrder.IndexOf(system);
                if(index == -1)
                {
                    index = SystemOrder.Count;
                    SystemOrder.Add(system);
                    return index;
                }
                else{
                    return index;
                }
            }
    
            private static List<string> SystemOrder = new List<string> {
                EconomyConstants.System.GENERIC,
                EconomyConstants.System.SELECTBOX,
                EconomyConstants.System.EQUIPMENT,
                EconomyConstants.System.GEM
            };
    
            private static List<int> QualityOrder = new List<int> {
                int.Parse(EconomyConstants.Quality.HALLOWS),
                int.Parse(EconomyConstants.Quality.LEGENDARY),
                int.Parse(EconomyConstants.Quality.EPIC),
                int.Parse(EconomyConstants.Quality.UNCOMMON),
                int.Parse(EconomyConstants.Quality.COMMON),
                int.Parse(EconomyConstants.Quality.POOR)
            };
        }
    }
}
