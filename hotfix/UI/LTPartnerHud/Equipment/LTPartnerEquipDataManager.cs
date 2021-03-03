using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EB;
using EB.Sparx;
using Hotfix_LT.Data;
using LT.Hotfix.Utility;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// Eid=装备特有id(唯一);ECid=装备基础信息id（拿读表数据）;
    /// </summary>
    public class BaseEquipmentInfo
    {
        //10条属性
        public int Eid;//装备特有id(唯一)
        public string ECid;//装备基础信息id（拿读表数据）

        public override bool Equals(object obj){
            if (obj == null || !(obj is BaseEquipmentInfo)){
                return false;
            }
            
            //装备判断Eid 锻造液判断Ecid
            if (this.Eid == (obj as BaseEquipmentInfo).Eid ||(this.ECid == (obj as BaseEquipmentInfo).ECid&& this.SuitType == -1)){
                return true;
            }

            return false;
        }

        public override int GetHashCode(){
            return Eid;
        }

        public int SuitType
        {
            get
            {
                if (LTPartnerEquipDataManager.Instance.isEquipUpItem(this.ECid)) return -1;//如果属于装备锻造液的话
                EquipmentItemTemplate temp=Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipment(this.ECid);
                if (temp!=null)
                {
                    string i = temp.SuitType;
                    return int.Parse(i);
                }
                else
                {
                    return -1;
                }
            }
        }//什么套装(读表)~未实现

        public int Num;//锻造液数量

        public string IconName;//图片名
        public string CellPos;//穿戴时装备的部位
        public EquipPartType Type; //装备位置
        public bool isDress;//装备状态（没穿/穿了：0/1）
        public int EquipLevel;//装备等级
        public int QualityLevel;//装备品阶
        public string SuitIcon
        {
            get
            {
                if (LTPartnerEquipDataManager.Instance.isEquipUpItem(this.ECid)) return "";//如果属于装备锻造液的话
                string i = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(this.SuitType).SuitIcon;
                return i;
            }

        }

        public bool isLock;//是否锁定~未实现

        public BaseEquipmentInfo()
        {
        }
        public BaseEquipmentInfo(string itemId)
        {
            this.ECid = itemId;
            if (Hotfix_LT.Data.EconemyTemplateManager.Instance != null)
            {
                var item = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGeneral(this.ECid);
                IconName = item.IconId;
                QualityLevel = item.QualityLevel;
            }
        }
    }

    public class DetailedEquipmentInfo : BaseEquipmentInfo
    {
        //追加7条属性
        public DetailedEquipmentInfo(int EID) { Eid = EID; }

        public EquipmentAttr MainAttributes;//主属性
        public List<EquipmentAttr> ExAttributes;//附加属性
        public string Name//名字
        {
            get
            {
                if (string.IsNullOrEmpty(this.ECid)) return null;
                string str = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipment(this.ECid).Name;
                return str;
            }
        }

        public string FirstSuitAttr//第一条套装属性,读表
        {
            get
            {
                if (string.IsNullOrEmpty(this.ECid)) return null;
                int attrId1 = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipment(this.ECid).SuitAttrId_1;
                if (attrId1 == 0) return null;
                Hotfix_LT.Data.SkillTemplate suitAttr = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(attrId1);
                string str = suitAttr.Description;
                return str;
            }
        }

        public string SecondSuitAttr//第二条套装属性，读表
        {
            get
            {
                if (string.IsNullOrEmpty(this.ECid)) return null;
                int attrId2 = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipment(this.ECid).SuitAttrId_2;
                if (attrId2 == 0) return null;
                Hotfix_LT.Data.SkillTemplate suitAttr = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(attrId2);
                string str = suitAttr.Description;
                return str;
            }
        }

        public int BaseExp//基础自带经验值
        {
            get
            {
                int i = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipment(this.ECid).BaseExp;
                return i;
            }
        }

        public int Exp;//经验
    }

    public class EquipmentAttr
    {
        public string Name;
        public float Value;
        public EquipmentAttr(string name, float value)
        {
            Name = name;
            Value = value;
        }
    }

    public enum EquipPartType
    {
        none = 0,
        part1 = 1,//1号位
        part2,
        part3,
        part4,
        part5,
        part6,
    }

    public class LTPartnerEquipDataManager : ManagerUnit
    {
        #region About Instance
        private static LTPartnerEquipDataManager instance = null;
        public static LTPartnerEquipDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTPartnerEquipDataManager>();
                }
                return instance;
            }
        }
        #endregion

        private LTPartnerEquipDataAPI Api;

        #region 当前所有装备
        private List<BaseEquipmentInfo> AllBaseEquipInfoList = new List<BaseEquipmentInfo>(512);
        public List<BaseEquipmentInfo> CurAllBaseEquipInfoList
        {
            get{
                return AllBaseEquipInfoList;
            }
        }
        #endregion

        public Dictionary<int, int> SuitTypeAndCountDicWithoutEquiped = new Dictionary<int, int>(512);
        public Dictionary<int, int> SuitTypeAndCountDic = new Dictionary<int, int>(512);
        public Dictionary<int, int> EquipSynSuitTypeAndCountDic = new Dictionary<int, int>(512);
        public LTPartnerData CurrentPartnerData;
        private int equipSynConsume = -1;
        public int EquipSynConsume
        {
            get
            {
                if (equipSynConsume == -1)
                {
                    equipSynConsume = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("EquipSynCostItem_1425");
                }
                return equipSynConsume;
            }
        }

        public static int GetLegendEquipmentEcidByPartSuit(EquipPartType type, int suitType)
        {
            int part = (int)type;
            if (suitType >= 1 && suitType <= 21 || suitType == 30)
            {

                return 10 * 10000 + suitType * 100 + part * 10 + 7;
            }
            return -1;
        }

        /// <summary>
        /// 获取仓库里所有装备并按品质,等级,装备位置排序
        /// </summary>
        /// <returns></returns>
        public List<BaseEquipmentInfo> GetAllEquipsWithOrdered()
        {
            //按条件排序
            AllBaseEquipInfoList.Sort((a, b) =>
            {
                if (a.QualityLevel < b.QualityLevel)
                    return -1;
                else if (a.QualityLevel > b.QualityLevel)
                    return 1;
                else if (a.EquipLevel < b.EquipLevel)
                    return -1;
                else if (a.EquipLevel > b.EquipLevel)
                    return 1;
                else if (a.Eid > b.Eid)
                    return -1;
                else
                    return 1;
            });

            return AllBaseEquipInfoList;
        }

        /// <summary>
        /// 仓库里剩下的装备
        /// </summary>
        /// <returns></returns>
        public List<BaseEquipmentInfo> GetFreeEquipInfoList()
        {
            List<BaseEquipmentInfo> data = new List<BaseEquipmentInfo>();
            for (int i = 0; i < AllBaseEquipInfoList.Count; i++)
            {
                if (!AllBaseEquipInfoList[i].isDress) data.Add(AllBaseEquipInfoList[i]);
            }
            return data;
        }

        public bool isMaxEquipNum
        {
            get
            {
                int EquipCount = 0;
                for (var i = 0; i < AllBaseEquipInfoList.Count; i++)
                {
                    var data = AllBaseEquipInfoList[i];
                    if (!data.isDress && data.SuitType != -1)
                    {
                        EquipCount++;
                    }
                }
                int extraAdd = LTPartnerConfig.Equip_BASE_MAX_VALUE + VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.InvEquipCount);
                return EquipCount >= extraAdd;
            }
        }

        public bool isMaxEquipNumOneKey(int num)
        {
            int EquipCount = 0;
            for (var i = 0; i < AllBaseEquipInfoList.Count; i++)
            {
                var data = AllBaseEquipInfoList[i];

                if (!data.isDress && data.SuitType != -1)
                {
                    EquipCount++;
                }
            }
            int extraAdd = LTPartnerConfig.Equip_BASE_MAX_VALUE + VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.InvEquipCount);
            return EquipCount >(extraAdd - num);
        }

        public override void Connect()
        {
            State = SubSystemState.Connected;
        }

        public SubSystemState State { get; set; }

        public override void Disconnect(bool isLogout)
        {
            State = SubSystemState.Disconnected;
        }
        public override void Initialize(Config config)
        {
            Api = new LTPartnerEquipDataAPI();
            Api.ErrorHandler += ErrorHandler;
        }
        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        public override void OnLoggedIn()
        {
            //EquipUpItem
            UpdateEquipUpItemInfo();
            UpdateEqupmentInfo();
        }

        /// <summary>
        /// key为锻造液物品id，value为所添加的经验。
        /// </summary>
        private Dictionary<string, int> EquipUpItemDic = new Dictionary<string, int>();
        /// <summary>
        /// 强化时，添加到消耗池里面的物品
        /// </summary>
        private Dictionary<string, int> EquipUpItemNumDic = new Dictionary<string, int>();
        private Dictionary<string, int> EquipSynItemNumDic = new Dictionary<string, int>();

        public EquipPartType CurType;//当前选择的装备类型
        public int CurSuitType;
        public EquipmentSortType CurSortType = EquipmentSortType.Level;
        public List<int> UpLevelSelectList = new List<int>();

        public void GetSelectUpLevelItemExp(DetailedEquipmentInfo Equip, out int UpLevel, out int UpExp)
        {
            int totleExp = 0;
            for (int i = 0; i < UpLevelSelectList.Count; ++i)
            {
                totleExp += LTPartnerEquipDataManager.Instance.GetTotleExpByEid(UpLevelSelectList[i]);
            }
            foreach (var item in EquipUpItemNumDic)
            {
                totleExp += LTPartnerEquipDataManager.Instance.getEquipUpItemExp(item.Key) * item.Value;
            }
            UpExp = totleExp;

            int level = 0;
            totleExp += Equip.Exp;
            var temp = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetLevelUpListByQuality(Equip.QualityLevel);
            for (int i = 0; i < temp.Count; ++i)
            {
                if (totleExp < temp[i].TotalNeedExp)
                {
                    level = i;
                    break;
                }
            }
            UpLevel = level;
        }

        #region(装备强化)

        public int findEidByEquipUpItemId(string id)
        {
            int eid = 0;
            for (int i = 0; i < AllBaseEquipInfoList.Count; i++)
            {
                if (AllBaseEquipInfoList[i].ECid == id) eid = AllBaseEquipInfoList[i].Eid;
            }
            return eid;
        }
        public Dictionary<string, int> getEquipUpItemNumDic()
        {
            return EquipUpItemNumDic;
        }
        public void cleanEquipUpItemNum()
        {
            EquipUpItemNumDic.Clear();
        }
        public int getEquipUpItemNum(string id)
        {
            if (EquipUpItemNumDic.ContainsKey(id)) return EquipUpItemNumDic[id];
            return 0;
        }
        public void addEquipUpItemNum(string id)
        {
            if (EquipUpItemNumDic.ContainsKey(id))
            {
                EquipUpItemNumDic[id]++;
            }
            else
            {
                EquipUpItemNumDic.Add(id, 1);
            }
        }
        public void removeEquipUpItemNum(string id)
        {
            if (EquipUpItemNumDic.ContainsKey(id))
            {
                if (EquipUpItemNumDic[id] > 0)
                {
                    EquipUpItemNumDic[id]--;
                    if (EquipUpItemNumDic[id] == 0) EquipUpItemNumDic.Remove(id);
                }
                else
                {
                    EquipUpItemNumDic.Remove(id);
                }
            }
        }
        public bool isEquipUpItem(string id)
        {
            return id != null && EquipUpItemDic.ContainsKey(id);
        }
        public int getEquipUpItemExp(string id)
        {
            if (isEquipUpItem(id)) return EquipUpItemDic[id];
            else return 0;
        }

        private void UpdateEquipUpItemInfo()
        {
            EquipUpItemDic = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetEquipUpItemDic();
        }
        #endregion


        #region(红15装备合成)
        public List<BaseEquipmentInfo> GetSynEquipList()
        {
            List<BaseEquipmentInfo> list = new List<BaseEquipmentInfo>();
            UpdateEqupmentInfo();
            for (int i = 0; i < AllBaseEquipInfoList.Count; i++)
            {
                if (AllBaseEquipInfoList[i].QualityLevel == 6 && AllBaseEquipInfoList[i].Type != EquipPartType.none)
                {
                    list.Add(AllBaseEquipInfoList[i]);
                }
            }
            return list;
        }
        public void CleanEquipSynItemNum()
        {
            EquipSynItemNumDic.Clear();
        }
        #endregion


        /// <summary>
        /// 根据装备部位和品质获取图标
        /// </summary>
        /// <param name="type">部位</param>
        /// <param name="Quality">品质</param>
        /// <returns></returns>
        public string GetEquipIconBuyTypeAndQua(EquipPartType type, int Quality)
        {
            if (type == EquipPartType.none || Quality < 1 || Quality > 7) return null;
            return string.Format("{0}{1}", LTPartnerEquipConfig.EquipIconTypeStr[(int)type], LTPartnerEquipConfig.EquipIconQualityStr[Quality]);
        }

        HashSet<BaseEquipmentInfo> _infoSet = new HashSet<BaseEquipmentInfo>();

        #region cpu 409ms
        /// <summary>
        /// 缓存信息与仓库同步
        /// </summary>
        public void UpdateEqupmentInfo()
        {
            Hashtable datas;
            AllBaseEquipInfoList.Clear();
            SuitTypeAndCountDicWithoutEquiped.Clear();
            SuitTypeAndCountDic.Clear();
            EquipSynSuitTypeAndCountDic.Clear();
            cleanEquipUpItemNum();
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("inventory", out datas);
            if(datas == null)
            {
                EB.Debug.LogError("LTPartnerEquipDataManager.UpdateEqupmentInfo datas is null");
            }else
            //所有装备解析
            {
                _infoSet.Clear();
                var it = datas.GetEnumerator();
                while (it.MoveNext())
                {
                    var info = AddOneEquipToList(it.Value);

                    if (info != null)
                    {
                        if (_infoSet.Contains(info))
                        {
                            _infoSet.Remove(info);
                        }

                        _infoSet.Add(info);
                    }
                }
                AllBaseEquipInfoList.AddRange(_infoSet);
                _infoSet.Clear();
            }

            //不存在的锻造液也存于列表中
            {
                var it = EquipUpItemDic.GetEnumerator();
                while(it.MoveNext())     
                {
                    if (!EquipUpItemNumDic.ContainsKey(it.Current.Key))
                    {
                        AllBaseEquipInfoList.Add(new BaseEquipmentInfo(it.Current.Key));
                    }
                }
            }

            cleanEquipUpItemNum();
        }

        /// <summary>
        /// 背包改动回调
        /// </summary>
        /// <param name="inventory_new"></param>
        public void OnInventoryChanged(object inventory_new)
        {
            if(inventory_new is Hashtable){
                var it = (inventory_new as Hashtable).GetEnumerator();
                while(it.MoveNext()){
                    bool hasTheEquip = false;
                    int theEid = Convert.ToInt32(it.Key);
                    var val = it.Value;
                    for(int i = 0; i < AllBaseEquipInfoList.Count; i++){
                        if(AllBaseEquipInfoList[i].Eid == theEid){
                            if(val == null)
                            {
                                AllBaseEquipInfoList.RemoveAt(i);
                                break;
                            }else{
                                hasTheEquip = true;
                            }
                        }
                    }

                    string ecid = EB.Dot.String("economy_id", val, null);
                    //如果是锻造液也要添加
                    if (!hasTheEquip || EquipUpItemDic.ContainsKey(ecid))
                    {
                        //for (int i = 0; i < AllBaseEquipInfoList.Count; i++)
                        //{
                        //    if (AllBaseEquipInfoList[i].ECid.Equals(ecid))
                        //    {
                        //        if(AllBaseEquipInfoList[i].Num == 0) AllBaseEquipInfoList.RemoveAt(i);
                        //        break;
                        //    }
                        //}

                        var info = AddOneEquipToList(val);

                        if (info != null)
                        {
                            //装入当前装备缓存
                            if (AllBaseEquipInfoList.Contains(info))
                            {
                                AllBaseEquipInfoList.Remove(info);
                            }

                            AllBaseEquipInfoList.Add(info);
                        }
                    }
                }
                
                cleanEquipUpItemNum();
            }
        }

        /// <summary>
        /// 添加单个装备
        /// </summary>
        /// <param name="data"></param>
        private BaseEquipmentInfo AddOneEquipToList(object data)
        {
	        if (data == null)
	        {
				//EB.Debug.LogError("data is nil!");伙伴存在没装备的情况
				return null;
	        }
            
            string key = EB.Dot.String("economy_id", data, null);

            if (string.IsNullOrEmpty(key))
            {
	            EB.Debug.LogError("key is nil!");
	            return null;
			}

            string Temp = EB.Dot.String("system", data, null);

            if (Temp != "Equipment" && !EquipUpItemDic.ContainsKey(key))
            {
                return null;
            }

            BaseEquipmentInfo info = new BaseEquipmentInfo();
            info.Eid = EB.Dot.Integer("inventory_id", data, info.Eid);
            info.ECid = EB.Dot.String("economy_id", data, info.ECid);
            info.Num = EB.Dot.Integer("num", data, info.Num);
            info.EquipLevel = EB.Dot.Integer("currentLevel", data, info.EquipLevel);

            if (EquipUpItemDic.ContainsKey(info.ECid))
            {
                info.IconName = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGeneral(info.ECid).IconId;
                addEquipUpItemNum(info.ECid);
            }
            else
            {
                info.IconName = EB.Dot.String("iconId", data, null);
            }

            string location = EB.Dot.String("location", data, null);
            switch (location)
            {
                case "equipment": { info.isDress = true; }; break;
                default: { info.isDress = false; }; break;
            }
            info.isLock = EB.Dot.Bool("lock", data, info.isLock);
            info.QualityLevel = EB.Dot.Integer("qualityLevel", data, info.QualityLevel);
            info.CellPos = EB.Dot.String("cell", data, null);
            string EquipTypeStr = EB.Dot.String("equipment_type", data, null);
            switch (EquipTypeStr)
            {
                case "1": info.Type = EquipPartType.part1; break;
                case "2": info.Type = EquipPartType.part2; break;
                case "3": info.Type = EquipPartType.part3; break;
                case "4": info.Type = EquipPartType.part4; break;
                case "5": info.Type = EquipPartType.part5; break;
                case "6": info.Type = EquipPartType.part6; break;
                default: info.Type = EquipPartType.none; break;
            }

            if (info.isDress == false && info.SuitType != -1)
            {
                if (SuitTypeAndCountDicWithoutEquiped.ContainsKey(info.SuitType))
                {
                    SuitTypeAndCountDicWithoutEquiped[info.SuitType] += 1;
                }
                else
                {
                    SuitTypeAndCountDicWithoutEquiped.Add(info.SuitType, 1);
                }

                if (SuitTypeAndCountDicWithoutEquiped.ContainsKey(-1))
                {
                    SuitTypeAndCountDicWithoutEquiped[-1] += 1;
                }
                else
                {
                    SuitTypeAndCountDicWithoutEquiped.Add(-1, 1);
                }
            }

            if (info.SuitType != -1) {
                if (SuitTypeAndCountDic.ContainsKey(info.SuitType)) {
                    SuitTypeAndCountDic[info.SuitType] += 1;
                } else {
                    SuitTypeAndCountDic.Add(info.SuitType, 1);
                }

                if (SuitTypeAndCountDic.ContainsKey(-1)) {
                    SuitTypeAndCountDic[-1] += 1;
                } else {
                    SuitTypeAndCountDic.Add(-1, 1);
                }
            }

            return info;
        }
        #endregion

        public void ReflashSynSuitInfo(List<BaseEquipmentInfo> infos)
        {
            for (int i = 0; i < infos.Count; i++)
            {
                if (EquipSynSuitTypeAndCountDic.ContainsKey(infos[i].SuitType))
                {
                    EquipSynSuitTypeAndCountDic[infos[i].SuitType] += 1;
                }
                else
                {
                    EquipSynSuitTypeAndCountDic.Add(infos[i].SuitType, 1);
                }
                if (EquipSynSuitTypeAndCountDic.ContainsKey(-1))
                {
                    EquipSynSuitTypeAndCountDic[-1] += 1;
                }
                else
                {
                    EquipSynSuitTypeAndCountDic.Add(-1, 1);
                }
            }
        }


        public DetailedEquipmentInfo GetEquipmentInfoByEID(int EID)
        {
            Hashtable data;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(string.Format("inventory.{0}", EID), out data);
            if (data == null)
            {
                EB.Debug.LogError("LTPartnerEquipDataManager.GetEquipmentInfoByEID not found this Equip,EId = {0}", EID);
                return null;
            }
            DetailedEquipmentInfo info = PreseDetailedEquipmentInfor(data, EID);
            return info;
        }

        public DetailedEquipmentInfo GetEquipmentInfoByEID(Hashtable data)
        {
            DetailedEquipmentInfo info = PreseDetailedEquipmentInfor(data);
            return info;
        }

        private DetailedEquipmentInfo PreseDetailedEquipmentInfor(object data, int EID = 0)
        {
            int eid = EID;
            if (eid == 0)
            {
                eid = EB.Dot.Integer("inventory_id", data, 0);
            }
            DetailedEquipmentInfo info = new DetailedEquipmentInfo(eid);
            info.ECid = EB.Dot.String("economy_id", data, info.ECid);
            info.EquipLevel = EB.Dot.Integer("currentLevel", data, info.EquipLevel);
            info.IconName = EB.Dot.String("iconId", data, null);
            string location = EB.Dot.String("location", data, null);
            switch (location)
            {
                case "equipment": { info.isDress = true; }; break;
                default: { info.isDress = false; }; break;
            }
            info.isLock = EB.Dot.Bool("lock", data, info.isLock);
            info.QualityLevel = EB.Dot.Integer("qualityLevel", data, info.QualityLevel);
            info.CellPos = EB.Dot.String("cell", data, null);
            string EquipTypeStr = EB.Dot.String("equipment_type", data, null);
            switch (EquipTypeStr)
            {
                case "1": info.Type = EquipPartType.part1; break;
                case "2": info.Type = EquipPartType.part2; break;
                case "3": info.Type = EquipPartType.part3; break;
                case "4": info.Type = EquipPartType.part4; break;
                case "5": info.Type = EquipPartType.part5; break;
                case "6": info.Type = EquipPartType.part6; break;
                default: info.Type = EquipPartType.none; break;
            }

            //新增
            //info.Name = EB.Dot.String("name", data, info.Name);名字改由读表获得

            string attrName = EB.Dot.String("attrs.main.name", data, null);
            int attrId = EB.Dot.Integer("attrs.main.attrId", data, 0);

            float value;
            Hotfix_LT.Data.EquipmentAttribute m_data = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipAttrInfo(attrId);
            if (info.EquipLevel >= 15)
            {
                value = m_data.finalValue;
            }
            else
            {
                value = EB.Dot.Single("attrs.main.value", data, 0);
                value += m_data.addValue * info.EquipLevel;
            }
            info.MainAttributes = new EquipmentAttr(attrName, value);

            ArrayList attrList = Hotfix_LT.EBCore.Dot.Array("attrs.ex", data, null);
            info.ExAttributes = new List<EquipmentAttr>();
            if (attrList != null)
            {
                for (var i = 0; i < attrList.Count; i++)
                {
                    object obj = attrList[i];

                    if (obj != null)
                    {
                        Hashtable Data = obj as Hashtable;
                        if (Data != null)
                        {
                            string attrName_ex = EB.Dot.String("name", Data, null);
                            float value_ex = EB.Dot.Single("value", Data, 0);
                            int attrId_ex = EB.Dot.Integer("attrId", Data, 0);
                            int level_ex = EB.Dot.Integer("level", Data, 0);
                            Hotfix_LT.Data.EquipmentAttribute ex_data = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipAttrInfo(attrId_ex);
                            value_ex += ex_data.addValue * level_ex;
                            EquipmentAttr data_ex = new EquipmentAttr(attrName_ex, value_ex);
                            info.ExAttributes.Add(data_ex);
                        }
                    }
                }
            }
            info.Exp = EB.Dot.Integer("currentExp", data, 0);
            return info;
        }



        public int GetTotleExpByEid(int eid)
        {
            float Level = 0;
            float Exp = 0;
            DataLookupsCache.Instance.SearchDataByID<float>(string.Format("inventory.{0}.currentLevel", eid), out Level);
            DataLookupsCache.Instance.SearchDataByID<float>(string.Format("inventory.{0}.currentExp", eid), out Exp);
            Exp += (float)LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eid)?.BaseExp;
            float temp = Exp * (0.88f - 0.02f * Level);
            return (int)((Level < 4) ? Exp : temp);
        }

        /// <summary>
        /// 穿戴装备或替换装备
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="heroid"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void RequireEquip(int eid, int heroid, EquipPartType type, System.Action<bool> callback, bool isUpdateInfo = true)
        {
            Api.RequestEquip(eid, heroid, type, delegate (Hashtable result)
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }

                DataLookupsCache.Instance.CacheData(result);

                if (isUpdateInfo)
                {
                    UpdateEqupmentInfo();
                }
                
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        /// <summary>
        /// 卸载装备
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="heroid"></param>
        /// <param name="callback"></param>
        public void RequireUnEquip(int eid, int heroid, System.Action<bool> callback, bool isUpdateInfo = true)
        {
            Api.RequestUnEquip(eid, heroid, delegate (Hashtable result)
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }

                DataLookupsCache.Instance.CacheData(result);

                if (isUpdateInfo)
                {
                    UpdateEqupmentInfo();
                }

                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }
        public void RequireUpLevel(int eid, List<int> list, List<Hashtable> costList, System.Action<bool> callback = null)
        {
            Api.RequestEquipmentLevelUp(eid, list, costList, delegate (Hashtable result)
             {
                 if (result == null)
                 {
                     if (callback != null)
                     {
                         callback(false);
                     }
                     return;
                 }
                 DataLookupsCache.Instance.CacheData(result);
                 UpdateEqupmentInfo();
                 if (callback != null)
                 {
                     callback(result != null);
                 }
             });
        }

        public void RequireLock(int eid, bool locked, System.Action<bool> callback = null)
        {
            Api.RequestEquipmentLock(eid, locked, delegate (Hashtable result)
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                UpdateEqupmentInfo();
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }
        public void RequireEquipSyn(int eid1, int eid2, System.Action<bool, Hashtable> callback = null)
        {
            Api.RequestEquipmentSyn(eid1, eid2, delegate (Hashtable result)
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false, null);
                    }
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                UpdateEqupmentInfo();

                if (callback != null)
                {
                    callback(result != null, result);
                }
            });



        }
        /// <summary>
        /// 卸载伙伴身上的所有装备
        /// </summary>
        /// <param name="heroId"></param>
        /// <param name="callback"></param>
        public void RequireUnEquipAll(int heroId, System.Action<bool> callback = null)
        {
            LTPartnerData parData = LTPartnerDataManager.Instance.GetPartnerByHeroId(heroId);

            if (parData == null)
            {
                EB.Debug.LogError("LTPartnerEquipDataManager RequireUnEquipAll heroid is Error, heroid = {0}", heroId);
                callback?.Invoke(false);
                return;
            }

            HeroEquipmentInfo[] infos = parData.EquipmentsInfo;
            List<HeroEquipmentInfo> eInfos = new List<HeroEquipmentInfo>();

            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Eid > 0)
                {
                    eInfos.Add(infos[i]);
                }
            }

            if (eInfos.Count == 0)
            {
                callback?.Invoke(true);
                return;
            }

            if (isMaxEquipNumOneKey(eInfos.Count))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_MailApi_1124"));
                return;
            }

            for (int i = 0; i < eInfos.Count; i++)
            {
                if (i == eInfos.Count - 1)
                {
                    RequireUnEquip(eInfos[i].Eid, heroId, (success) =>
                    {
                        if (!success)
                        {
                            return;
                        }

                        callback?.Invoke(success);
                    }, true);
                }
                else
                {
                    RequireUnEquip(eInfos[i].Eid, heroId, (success) =>
                    {
                        if (!success)
                        {
                            return;
                        }
                    }, false);
                }
            }
        }

        #region 请求指定英雄的装备能上装的所有装备
        private List<ComParam> _RequireEquipAll_list = new List<ComParam>(6);
        public void RequireEquipAll(int heroId, System.Action<bool> callback)
        {
            LTPartnerData parData = LTPartnerDataManager.Instance.GetPartnerByHeroId(heroId);
            if (parData == null)
            {
                EB.Debug.LogError("LTPartnerEquipDataManager RequireUnEquipAll heroid is Error, heroid = {0}", heroId);
                return;
            }

            QuerySuitList(parData, _RequireEquipAll_list);
            for (int i = 0; i < _RequireEquipAll_list.Count; i++)
            {
                if (i == _RequireEquipAll_list.Count - 1)
                {
                    RequireEquip(_RequireEquipAll_list[i].Eid, heroId, _RequireEquipAll_list[i].type, callback, true);
                }
                else
                {
                    RequireEquip(_RequireEquipAll_list[i].Eid, heroId, _RequireEquipAll_list[i].type, null, false);
                }
            }
            _RequireEquipAll_list.Clear();
        }
        #endregion

        #region 找到对应英雄的6个位置的合适装备
        public struct ComParam
        {
            public int Eid;
            public EquipPartType type;

            public ComParam(int Eid, int type)
            {
                this.Eid = Eid;
                this.type = (EquipPartType)type;
            }
        }

        /// <summary>
        /// 找到此英雄6个装备中空缺的位置对应的装备
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="suitList">返回6个部位中有空缺位置对应的装备</param>
        private void QuerySuitList(LTPartnerData pd, List<ComParam> suitList)
        {
            var orderedAllEquip = GetAllEquipsWithOrdered();
            Hashtable equipIds;
            DataLookupsCache.Instance.SearchDataByID(string.Format("heroStats.{0}.equip", pd.HeroId), out equipIds);
            for(int index = 1; index <= 6; index++)
            {
                int eid = 0;
                if(equipIds != null && equipIds.Contains(index)){
                    eid = (int)equipIds[index];
                }
                if(eid == 0 && orderedAllEquip.Count>1){//此位置为空
                    for(int j = orderedAllEquip.Count-1; j >=0; j--)
                    {
                        var theEquip = orderedAllEquip[j];
                        if (theEquip.Type == (EquipPartType)index && !theEquip.isDress){
                            suitList.Add(new ComParam(theEquip.Eid, index));
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 此英雄是否有任何可以上装的装备
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public bool HasAnySuitEquip(LTPartnerData pd)
        {
            Hashtable equipIds;
            DataLookupsCache.Instance.SearchDataByID($"heroStats.{pd.HeroId}.equip", out equipIds);
            for(int index = 1; index <= 6; index++)
            {
                string strIndex = index.ToString();
                int eid = 0;
                if(equipIds != null && equipIds.Contains(strIndex)){
                    eid = Convert.ToInt32(equipIds[strIndex]);
                }
                if(eid == 0){
                    for(int j = 0; j < CurAllBaseEquipInfoList.Count; j++)
                    {
                        var theEquip = CurAllBaseEquipInfoList[j];
                        if (theEquip.Type == (EquipPartType)index && !theEquip.isDress){
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        /// <summary>
        /// 没事别调用这个代码
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="heroid"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void DebugRequireEquip(int eid, int heroid, EquipPartType type, System.Action<bool> callback = null)
        {
            Api.RequestEquip(eid, heroid, type, delegate (Hashtable result)
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                UpdateEqupmentInfo();
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        public void RequestGetEquipmentPresetList(long uid, System.Action<Hashtable> callback = null)
        {
            Api.RequestGetEquipmentPresetList(uid, callback);
        }

        public void RequestAddEquipmentPreset(long uid, string presetName, Hashtable presetData, System.Action<Hashtable> callback = null)
        {
            Api.RequestAddEquipmentPreset(uid, presetName, presetData, callback);
        }

        public void RequestDeleteEquipmentPreset(long uid, string presetName, System.Action<Hashtable> callback = null)
        {
            Api.RequestDeleteEquipmentPreset(uid, presetName, callback);
        }
    }
}