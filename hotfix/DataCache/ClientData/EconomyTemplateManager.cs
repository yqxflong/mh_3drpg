using Hotfix_LT.Player;
using Hotfix_LT.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GM.DataCache;
using System.Linq;
using Unity.Standard.ScriptsWarp;

namespace Hotfix_LT.Data
{
    public abstract class EconemyItemTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int QualityLevel { get; set; }
        public string IconId { get; set; }
        public string Desc { get; set; }
        public List<DropDataBase> DropDatas = new List<DropDataBase>();
        public string DropChickId1 { get; set; }
        public string DropChickId2 { get; set; }
        public string DropChickId3 { get; set; }
    }

    public enum DropType
    {
        None,
        Scene,//主线副本
        Challenge,//挑战副本
        Store,//商店
        Lottery,//抽奖
        ExtremityTrial,//极限试炼
        LuckyReward,//幸运悬赏
        Treasures,//夺宝奇兵
        Activity,//活动
        Charge,//充值

        Alliance,//军团副本
        Maze,//异界迷宫
        Sleep,//睡梦之塔
        Awaken,//觉醒副本
        Compose,//合成

        ExtremityCompete,//极限试炼竞速模式
    }



    public class GeneralItemTemplate : EconemyItemTemplate
    {
        public string System { get; set; }
        public int Exp { get; set; }
        public bool CanUse { get; set; }
        public string CompoundItem { get; set; }
        public int NeedNum { get; set; }
    }

    public class EquipmentItemTemplate : EconemyItemTemplate
    {
        public string[] Models { get; set; }
        public CombatAttributes Attributes { get; set; }
        public int IncEnchant { get; set; }
        public int UpgradeId;
        public int SuitAttrId_1;
        public int SuitAttrId_2;
        public ResourceContainer UpgradeMaterials { get; set; }
        public string SuitType;
        public string SuitIcon;
        public string SuitName;
        public int BaseExp;
        public string SuitItemId;
    }

    public class GemItemTemplate : EconemyItemTemplate
    {
        public int Level { get; set; }
        public CombatAttributes Attributes { get; set; }
        public string Type { get; set; }
    }

    public class SuitAttribute
    {
        public int id;
        public string attr;
        public float value;
        public int all;
    }

    public class SuitTypeInfo
    {
        public int SuitType;
        public string TypeName;
        public string SuitIcon;
        public int SuitAttr2;
        public int SuitAttr4;
        public List<DropDataBase> DropDatas = new List<DropDataBase>();
    }

    public class EquipmentLevelUp
    {
        public string id;
        public int level;
        public int needExp;
        public int TotalNeedExp;
    }

    public class EquipmentAttribute
    {
        public int id;
        public float addValue;
        public float finalValue;
    }

    public class EquipAttributeRate
    {
        public int star;

        public List<float> rating;
    }

    public class SelectBox
    {
        public string id;
        public int index;
        public string rt1;
        public string ri1;
        public int rn1;
    }

    public class HeadFrame
    {
        public string id;
        public int num;
        public string name;
        public string iconId;
        public string desc;

        public HeadFrame()
        {
            id = "0";
            num = 0;
            name = iconId = desc = string.Empty;
        }
    }

    public class EconemyTemplateManager
    {
        private ConditionEconomy conditionSet;

        private static EconemyTemplateManager sInstance = null;
        private Dictionary<int, EconemyItemTemplate> mItems = new Dictionary<int, EconemyItemTemplate>();
        private Dictionary<int, SuitAttribute> mSuitAttr = new Dictionary<int, SuitAttribute>();
        private Dictionary<int, SuitTypeInfo> mSuitInfo = new Dictionary<int, SuitTypeInfo>();
        private Dictionary<int, EquipmentLevelUp> mEquipmentLevelup = new Dictionary<int, EquipmentLevelUp>();
        private Dictionary<int, EquipmentAttribute> mEquipAttrs = new Dictionary<int, EquipmentAttribute>();
        private Dictionary<int, EquipAttributeRate> mEquipAttrsRate = new Dictionary<int, EquipAttributeRate>();
        private Dictionary<string, List<SelectBox>> mSelectBoxDic = new Dictionary<string, List<SelectBox>>();
        private List<HeadFrame> mHeadFrameList = new List<HeadFrame>();

        public static EconemyTemplateManager Instance
        {
            get { return sInstance = sInstance ?? new EconemyTemplateManager(); }
        }

        private EconemyTemplateManager()
        {

        }

        public static void ClearUp()
        {
            if (sInstance != null)
            {
                sInstance.mItems.Clear();
                sInstance.mSuitAttr.Clear();
                sInstance.mSuitInfo.Clear();
                sInstance.mEquipmentLevelup.Clear();
                sInstance.mEquipAttrs.Clear();
                sInstance.mEquipAttrsRate.Clear();
                sInstance.mSelectBoxDic.Clear();
                sInstance.mHeadFrameList.Clear();
            }
        }

        public bool InitFromDataCache(GM.DataCache.Economy economy)
        {
            if (economy == null)
            {
                EB.Debug.LogError("InitFromDataCache: economy is null");
                return false;
            }

            conditionSet = economy.GetArray(0);

            mItems = new Dictionary<int, EconemyItemTemplate>(conditionSet.GenericItemsLength + conditionSet.EquipmentLength /*+ conditionSet.GemsLength*/);

            if (!InitItems(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init items failed");
                return false;
            }

            if (!InitEquipments(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init equipments failed");
                return false;
            }
            if (!InitSuitAttrs(conditionSet))
            {
                EB.Debug.LogError("InitSuitAttrs: init levelUps failed");
                return false;
            }
            if (!InitEquipmentLevel(conditionSet))
            {
                EB.Debug.LogError("InitEquipmentLevel: init EquipmentLevel failed");
                return false;
            }
            if (!InitEquipmentAttr(conditionSet))
            {
                EB.Debug.LogError("InitEquipmentAttr: init EquipmentLevel failed");
                return false;
            }
            if (!InitEquipmentAttrRate(conditionSet))
            {
                EB.Debug.LogError("InitEquipmentAttrRate: init EquipmentAttrRate failed");
                return false;
            }
            //if (!InitSelectBox(conditionSet))
            //{
            //    EB.Debug.LogError("InitSelectBox: init SelectBox failed");
            //    return false;
            //}

            if (!InitHeadFrame(conditionSet))
            {
                EB.Debug.LogError("InitHeadFrame: init HeadFrame failed");
                return false;
            }
            return true;
        }

        private bool InitItems(GM.DataCache.ConditionEconomy items)
        {
            if (items == null)
            {
                EB.Debug.LogError("InitItems: items is null");
                return false;
            }

            for (int i = 0; i < items.GenericItemsLength; ++i)
            {
                var item = ParseItem(items.GetGenericItems(i));
                if (mItems.ContainsKey(item.Id))
                {
                    EB.Debug.LogError("InitItems: {0} exists", item.Id);
                    mItems.Remove(item.Id);
                }
                mItems.Add(item.Id, item);
            }
            return true;
        }

        private GeneralItemTemplate ParseItem(GM.DataCache.GenericItem obj)
        {
            GeneralItemTemplate item = new GeneralItemTemplate();
            item.Id = int.Parse(obj.Id);
            item.Name = EB.Localizer.GetTableString(string.Format("ID_economy_generic_items_{0}_name", item.Id), obj.Name);//;
            item.Desc = EB.Localizer.GetTableString(string.Format("ID_economy_generic_items_{0}_desc", item.Id), obj.Desc); //;
            item.System = obj.System;
            item.IconId = obj.IconId;
            item.QualityLevel = obj.QualityLevel;
            item.Exp = obj.Exp;
            item.CanUse = obj.CanUse;
            item.CompoundItem = obj.CompoundItem;
            item.NeedNum = obj.NeedNum;
            item.DropChickId1 = obj.DropChickId1;
            item.DropChickId2 = obj.DropChickId2;
            item.DropChickId3 = obj.DropChickId3;
            SetDropData(item.DropChickId1, item.DropDatas);
            SetDropData(item.DropChickId2, item.DropDatas);
            SetDropData(item.DropChickId3, item.DropDatas);
            return item;
        }

        public static void SetDropData(string s, List<DropDataBase> dropDataList)
        {
            if (string.IsNullOrEmpty(s))
                return;
			StringView @string = new StringView(s);
			var views = @string.Split2List(':');
			StringView type = new StringView();
			string text = string.Empty;
			if (views != null)
            {
                type = views[0];
                if (views.Count > 1)
                {
                    text = views[1].ToString();
                }
            }
            else
            {
                EB.Debug.LogError("DropData Split error s={0}" , s);
                type = new StringView(s);
            }
            if (type.Equals(new StringView("scene")))
            {
                DropDataBase data = new SceneDropData(DropType.Scene, text, "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("challenge")))
            {
                DropDataBase data = new ChallengeDropData(DropType.Challenge, text, "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("store")))
            {
                text = text.ToLower();
                DropDataBase data = new StoreDropData(DropType.Store, text, "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("lottery")))
            {
                DropDataBase data = new LotteryDropData(DropType.Lottery, text, "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("extremityTrial")))
            {
                DropDataBase data = new ExtremityTrialDropData(DropType.ExtremityTrial, text, "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("luckyReward")))
            {
                DropDataBase data = new LuckyRewardDropData(DropType.LuckyReward, "", "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("treasures")))
            {
                DropDataBase data = new TreasuresDropData(DropType.Treasures, "", "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("activity")))
            {
                DropDataBase data = new ActivityDropData(DropType.Activity, text, "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("charge")))
            {
                DropDataBase data = new ChargeDropData(DropType.Charge, text, "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("alliance")))
            {
                DropDataBase data = new AllianceDropData(DropType.Alliance, "", "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("maze")))
            {
                DropDataBase data = new MazeDropData(DropType.Maze, "", "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("sleep")))
            {
                DropDataBase data = new SleepDropData(DropType.Sleep, "", "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("awaken")))
            {
                DropDataBase data = new AwakenDropData(DropType.Awaken, text, "");
                dropDataList.Add(data);
            }
            else if (type.Equals(new StringView("compose")))
            {
                DropDataBase data = new ComposeDropData(DropType.Compose, text, "");
                dropDataList.Add(data);

            }
            else if (type.Equals(new StringView("extremityCompete")))
            {
                DropDataBase data = new ExtremityCompeteDropData(DropType.ExtremityCompete, text, "");
                dropDataList.Add(data);
            }
            else
                EB.Debug.LogError("drop type error type={0}" , type.ToString());
        }

        private bool InitEquipments(GM.DataCache.ConditionEconomy equips)
        {
            if (equips == null)
            {
                EB.Debug.LogError("InitEquipments: equips is null");
                return false;
            }

            for (int i = 0; i < equips.EquipmentLength; ++i)
            {
                var equip = ParseEquipment(equips.GetEquipment(i));
                var suit = ParseSuitType(equip);
                if (mItems.ContainsKey(equip.Id))
                {
                    EB.Debug.LogError("InitEquipments: {0} exists", equip.Id);
                    mItems.Remove(equip.Id);
                }
                mItems.Add(equip.Id, equip);
                if (mSuitInfo.ContainsKey(int.Parse(equip.SuitType)))
                {
                    mSuitInfo.Remove(int.Parse(equip.SuitType));
                }
                mSuitInfo.Add(int.Parse(equip.SuitType), suit);
            }
            return true;
        }

        private EquipmentItemTemplate ParseEquipment(GM.DataCache.EquipmentItem obj)
        {
            EquipmentItemTemplate equip = new EquipmentItemTemplate();
            equip.Id = int.Parse(obj.Id);

			using (ZString.Block())
			{
				ZString strID = ZString.Format("ID_economy_equipment_{0}_name", equip.Id);
				equip.Name = EB.Localizer.GetTableString(strID, obj.Name); ;//equip.Name;

				strID = ZString.Format("ID_economy_equipment_{0}_desc", equip.Id);
				equip.Desc = EB.Localizer.GetTableString(strID, obj.Desc); ;//equip.Description;

				strID = ZString.Format("ID_economy_equipment_{0}_suit_name", equip.Id);
				equip.SuitName = EB.Localizer.GetTableString(strID, obj.SuitName); ;//equip.SuitName;
			}

			equip.QualityLevel = obj.QualityLevel;
            equip.SuitAttrId_1 = obj.SuitAttributeId1;
            equip.SuitAttrId_2 = obj.SuitAttributeId2;
            equip.IconId = obj.IconId;
            equip.SuitType = obj.SuitType;
            equip.SuitIcon = obj.SuitIcon;           
            equip.BaseExp = obj.Exp;
            equip.SuitItemId = obj.SuitDropId;
            EconemyItemTemplate tempItem = null;
            if (!string.IsNullOrEmpty(equip.SuitItemId)) tempItem = GetItem(equip.SuitItemId);
            if (tempItem != null)
            {
                equip.DropChickId1 = tempItem.DropChickId1;
                equip.DropChickId2 = tempItem.DropChickId2;
                equip.DropChickId3 = tempItem.DropChickId3;
            }
            else
            {
                equip.DropChickId1 = obj.DropChickId1;
                equip.DropChickId2 = obj.DropChickId2;
                equip.DropChickId3 = obj.DropChickId3;
            }
            SetDropData(equip.DropChickId1, equip.DropDatas);
            SetDropData(equip.DropChickId2, equip.DropDatas);
            SetDropData(equip.DropChickId3, equip.DropDatas);

            equip.UpgradeMaterials = new ResourceContainer();
            do
            {
                int id = 0, amount = 0;

                if (id <= 0 || amount <= 0) break;
                if (equip.UpgradeMaterials.Items.ContainsKey(id))
                {
                    EB.Debug.LogError("ParseEquipment: material {0} exists in {1}", id, equip.Id);
                    equip.UpgradeMaterials.Items.Remove(id);
                }
                equip.UpgradeMaterials.Items.Add(id, amount);

                if (id <= 0 || amount <= 0) break;
                if (equip.UpgradeMaterials.Items.ContainsKey(id))
                {
                    EB.Debug.LogError("ParseEquipment: material {0} exists in {1}", id, equip.Id);
                    equip.UpgradeMaterials.Items.Remove(id);
                }
                equip.UpgradeMaterials.Items.Add(id, amount);

                if (id <= 0 || amount <= 0) break;
                if (equip.UpgradeMaterials.Items.ContainsKey(id))
                {
                    EB.Debug.LogError("ParseEquipment: material {0} exists in {1}", id, equip.Id);
                    equip.UpgradeMaterials.Items.Remove(id);
                }
                equip.UpgradeMaterials.Items.Add(id, amount);

                if (id <= 0 || amount <= 0) break;
                if (equip.UpgradeMaterials.Items.ContainsKey(id))
                {
                    EB.Debug.LogError("ParseEquipment: material {0} exists in {1}", id, equip.Id);
                    equip.UpgradeMaterials.Items.Remove(id);
                }
                equip.UpgradeMaterials.Items.Add(id, amount);

                if (id <= 0 || amount <= 0) break;
                if (equip.UpgradeMaterials.Items.ContainsKey(id))
                {
                    EB.Debug.LogError("ParseEquipment: material {0} exists in {1}", id, equip.Id);
                    equip.UpgradeMaterials.Items.Remove(id);
                }
                equip.UpgradeMaterials.Items.Add(id, amount);

                if (id <= 0 || amount <= 0) break;
                if (equip.UpgradeMaterials.Items.ContainsKey(id))
                {
                    EB.Debug.LogError("ParseEquipment: material {0} exists in {1}", id, equip.Id);
                    equip.UpgradeMaterials.Items.Remove(id);
                }
                equip.UpgradeMaterials.Items.Add(id, amount);
            } while (false);

            return equip;
        }

        private SuitTypeInfo ParseSuitType(EquipmentItemTemplate obj)
        {
            SuitTypeInfo info = new SuitTypeInfo();
            info.SuitType = int.Parse(obj.SuitType);
            info.TypeName = obj.SuitName;
            info.SuitIcon = obj.SuitIcon;
            info.SuitAttr2 = obj.SuitAttrId_1;
            info.SuitAttr4 = obj.SuitAttrId_2;
            info.DropDatas = obj.DropDatas;
            return info;
        }

        public bool InitEquipmentLevel(GM.DataCache.ConditionEconomy EquipmentLevelup)
        {
            if (EquipmentLevelup == null)
            {
                EB.Debug.LogError("InitEquipmentLevel: equipmentLevel is null");
                return false;
            }
            mEquipmentLevelup = new Dictionary<int, EquipmentLevelUp>();
            for (int i = 0; i < EquipmentLevelup.EquipmentsLevelUpLength; ++i)
            {
                var EquipUp = ParseEquipmentLevelup(EquipmentLevelup.GetEquipmentsLevelUp(i));
                if (mEquipmentLevelup.ContainsKey(int.Parse(EquipUp.id)))
                {
                    EB.Debug.LogError("InitEquipmentLevelUp: {0} exists", EquipUp.id);
                    mEquipmentLevelup.Remove(int.Parse(EquipUp.id));
                }
                mEquipmentLevelup.Add(int.Parse(EquipUp.id), EquipUp);
            }

            return true;
        }

        private EquipmentLevelUp ParseEquipmentLevelup(GM.DataCache.EquipmentLevelUp obj)
        {
            EquipmentLevelUp equipUp = new EquipmentLevelUp();
            equipUp.id = obj.Id;
            equipUp.level = obj.Level;
            equipUp.needExp = obj.NeedExp;
            equipUp.TotalNeedExp = obj.TotalNeedExp;
            return equipUp;
        }

        public bool InitSuitAttrs(GM.DataCache.ConditionEconomy suitAttributes)
        {
            if (suitAttributes == null)
            {
                EB.Debug.LogError("InitSuitAttrs: suitAttributes is null");
                return false;
            }

            mSuitAttr = new Dictionary<int, SuitAttribute>();
            for (int i = 0; i < suitAttributes.SuitAttributeLength; i++)
            {
                var suitAttr = ParseSuitAttrs(suitAttributes.GetSuitAttribute(i));
                if (mSuitAttr.ContainsKey(suitAttr.id))
                {
                    mSuitAttr.Remove(suitAttr.id);
                }
                mSuitAttr.Add(suitAttr.id, suitAttr);
            }
            return true;
        }

        private SuitAttribute ParseSuitAttrs(GM.DataCache.SuitAttribute obj)
        {
            SuitAttribute data = new SuitAttribute();
            data.id = obj.Id;
            data.attr = obj.Attr;
            data.value = obj.Value;
            data.all = obj.All;
            return data;
        }

        public bool InitEquipmentAttr(GM.DataCache.ConditionEconomy EquipmentAttributes)
        {
            if (EquipmentAttributes == null)
            {
                EB.Debug.LogError("InitEquipmentAttr: EquipmentAttributes is null");
                return false;
            }

            mEquipAttrs = new Dictionary<int, EquipmentAttribute>();
            for (int i = 0; i < EquipmentAttributes.EquipAttributeLength; i++)
            {
                var equipAttr = ParseEquipAttrs(EquipmentAttributes.GetEquipAttribute(i));
                if (mEquipAttrs.ContainsKey(equipAttr.id))
                {
                    mEquipAttrs.Remove(equipAttr.id);
                }
                mEquipAttrs.Add(equipAttr.id, equipAttr);
            }
            return true;
        }
        private EquipmentAttribute ParseEquipAttrs(GM.DataCache.EquipAttribute obj)
        {
            EquipmentAttribute data = new EquipmentAttribute();
            data.id = obj.Id;
            data.addValue = obj.AddValue;
            data.finalValue = obj.FinalValue;
            return data;
        }

        public bool InitEquipmentAttrRate(GM.DataCache.ConditionEconomy EquipmentAttriRate)
        {
            if (EquipmentAttriRate == null)
            {
                EB.Debug.LogError("InitEquipmentAttrRate: EquipmentAttriRate is null");
                return false;
            }

            mEquipAttrsRate = new Dictionary<int, EquipAttributeRate>();
            for (int i = 0; i < EquipmentAttriRate.EquipAttributeRateLength; i++)
            {
                var equipAttr = ParseEquipAttrsRate(EquipmentAttriRate.GetEquipAttributeRate(i));
                if (mEquipAttrsRate.ContainsKey(equipAttr.star))
                {
                    mEquipAttrsRate.Remove(equipAttr.star);
                }
                mEquipAttrsRate.Add(equipAttr.star, equipAttr);
            }
            return true;
        }
        private EquipAttributeRate ParseEquipAttrsRate(GM.DataCache.EquipAttributeRate obj)
        {
            EquipAttributeRate data = new EquipAttributeRate();
            data.star = obj.Star;
            data.rating = new List<float>()
        {
            obj.Rating0,
            obj.Rating1,
            obj.Rating2,
            obj.Rating3,
            obj.Rating4
        };

            return data;
        }

        public bool InitSelectBox(GM.DataCache.ConditionEconomy selectBox)
        {
            if (selectBox == null)
            {
                EB.Debug.LogError("InitSelectBox: SelectBox is null");
                return false;
            }

            for (int i = 0; i < selectBox.SelectBoxLength; i++)
            {
                SelectBox selectBoxInfo = ParseSelectBox(selectBox.GetSelectBox(i));
                if (!mSelectBoxDic.ContainsKey(selectBoxInfo.id))
                {
                    mSelectBoxDic.Add(selectBoxInfo.id, new List<SelectBox>());
                }
                mSelectBoxDic[selectBoxInfo.id].Add(selectBoxInfo);
            }
            return true;
        }
        private SelectBox ParseSelectBox(GM.DataCache.SelectBox obj)
        {
            SelectBox selectBoxInfo = new SelectBox();

            selectBoxInfo.id = obj.Id;
            selectBoxInfo.index = obj.Index;
            selectBoxInfo.rt1 = obj.Rt1;
            selectBoxInfo.ri1 = obj.Ri1;
            selectBoxInfo.rn1 = obj.Rn1;
            return selectBoxInfo;
        }

        public bool InitHeadFrame(GM.DataCache.ConditionEconomy headFrame)
        {
            if (headFrame == null)
            {
                EB.Debug.LogError("InitHeadFrame: HeadFrame is null");
                return false;
            }

            mHeadFrameList = new List<HeadFrame>();
            for (int i = 0; i < headFrame.HeadFrameLength; i++)
            {
                HeadFrame selectBoxInfo = ParseHeadFrame(headFrame.GetHeadFrame(i));
                if (selectBoxInfo != null && !string.IsNullOrEmpty(selectBoxInfo.id))
                {
                    mHeadFrameList.Add(selectBoxInfo);
                }
            }
            return true;
        }
        private HeadFrame ParseHeadFrame(GM.DataCache.HeadFrame obj)
        {
            HeadFrame headFrame = new HeadFrame();

            headFrame.id = obj.Id;
            headFrame.num = obj.Num;
            headFrame.name = EB.Localizer.GetTableString(string.Format("ID_economy_head_frame_{0}_{1}_name", headFrame.id, headFrame.num), obj.Name);
            headFrame.iconId = obj.IconId;
            headFrame.desc = EB.Localizer.GetTableString(string.Format("ID_economy_head_frame_{0}_{1}_desc", headFrame.id, headFrame.num), obj.Desc);
            return headFrame;
        }

        public HeadFrame GetHeadFrame(string data)//id_num
        {
            HeadFrame temp = new HeadFrame();
            if (string.IsNullOrEmpty(data))
            {
                return new HeadFrame();
            }
            else
            {
                string[] split = data.Split('_');
                HeadFrame frame = EconemyTemplateManager.Instance.GetHeadFrame(split[0], int.Parse(split[1]));
                return frame;
            }
        }

        public HeadFrame GetHeadFrame(string id, int num)
        {
            HeadFrame temp = new HeadFrame();
            for (int i = 0; i < mHeadFrameList.Count; ++i)
            {
                if (mHeadFrameList[i].id.Equals(id) && mHeadFrameList[i].num == num)
                {
                    temp = mHeadFrameList[i];
                    break;
                }
            }
            return temp;
        }

        public List<HeadFrame> GetAllHeadFrame()
        {
            return mHeadFrameList;
        }

        public EconemyItemTemplate GetItem(int id)
        {
            EconemyItemTemplate ret;
            if (mItems.TryGetValue(id, out ret))
            {
                return ret;
            }
            else
            {
                EB.Debug.LogWarning("GetItem: item not found, id = {0}", id);
                return mItems[1051];
            }
        }

        public EconemyItemTemplate GetItem(string id)
        {
            int idValue = 0;
            int.TryParse(id, out idValue);
            if (idValue > 0)
            {
                return GetItem(idValue);
            }
            else
            {
                return null;
            }
        }

        public GeneralItemTemplate GetGeneral(int id)
        {
            return GetItem(id) as GeneralItemTemplate;
        }

        public GeneralItemTemplate GetGeneral(string id)
        {
            int general_id = 0;
            if (!int.TryParse(id, out general_id))
            {
                EB.Debug.LogError("General item id is not int");
                return null;
            }
            return GetGeneral(general_id);
        }

        public EquipmentItemTemplate GetEquipment(int id)
        {
            EconemyItemTemplate item = GetItem(id);
            if (item != null && item is EquipmentItemTemplate)
            {
                return item as EquipmentItemTemplate;
            }
            else
            {
                EB.Debug.LogWarning("GetEquipment: equipment not found, id = {0}", id);
                return null;
            }
        }

        public EquipmentItemTemplate GetEquipment(string id)
        {
            int EquipCid;
            if(id!=null && int.TryParse(id,out EquipCid))
            {
                return GetEquipment(EquipCid);
            }
            return null;
        }

        public GemItemTemplate GetGem(int id)
        {
            EconemyItemTemplate item = GetItem(id);
            if (item != null && item is GemItemTemplate)
            {
                return item as GemItemTemplate;
            }
            else
            {
                EB.Debug.LogWarning("GetGem: gem not found, id = {0}", id);
                return null;
            }
        }

        public GemItemTemplate GetGem(string id)
        {
            return GetGem(int.Parse(id));
        }

        public static string GetItemQuantityLevel(string ItemId)
        {
            var item = EconemyTemplateManager.Instance.GetItem(ItemId);
            if (item != null)
            {
                return item.QualityLevel.ToString();
            }

            return string.Empty;
        }

        public static string GetItemIcon(string ItemId)
        {
            var item = EconemyTemplateManager.Instance.GetItem(ItemId);
            if (item != null && !string.IsNullOrEmpty(item.IconId))
            {
                return item.IconId;
            }

            EB.Debug.LogWarning("GetItemIcon: icon is empty {0}", ItemId);
            return string.Empty;
        }

        public static string GetItemName(string ItemId)
        {
            var item = EconemyTemplateManager.Instance.GetItem(ItemId);
            if (item != null)
            {
                return item.Name;
            }

            return string.Empty;
        }

        public static string GetEquipSuitIcon(string ItemId)
        {
            var item = EconemyTemplateManager.Instance.GetItem(ItemId);
            if (item != null && item is EquipmentItemTemplate)
            {
                return ((EquipmentItemTemplate)item).SuitIcon;
            }

            return string.Empty;
        }

        public static string GetPartitionName(string raceModel, string ItemId)
        {
            if (string.IsNullOrEmpty(raceModel))
            {
                EB.Debug.LogWarning("RaceModel IsNullOrEmpty{0}" , ItemId);
                return string.Empty;
            }
            int race = System.Array.IndexOf(CharacterConstants.RaceHeroNames, raceModel.Replace("model_", ""));
            var equip = EconemyTemplateManager.Instance.GetEquipment(ItemId);
            if (equip != null)
            {
                if (equip.Models == null)
                {
                    EB.Debug.LogError("Models IsNullOrEmpty for equip={0}" , ItemId);
                }//CrashReport  equip id
                return equip.Models[race];
            }
            else
            {
                return string.Empty;
            }
        }

        public SuitAttribute GetSuitAttrByID(int id)
        {
            if (mSuitAttr.ContainsKey(id))
            {
                return mSuitAttr[id];
            }
            else
            {
                return null;
            }
        }

        public SuitTypeInfo GetSuitTypeInfoByEcidSuitType(int SuitType)
        {
            if (mSuitInfo.ContainsKey(SuitType))
            {
                return mSuitInfo[SuitType];
            }
            else
            {
                return null;
            }
        }

        public Dictionary<int, SuitTypeInfo> GetSuitTypeInfoDics()
        {
            return mSuitInfo;

        }

        public List<SuitTypeInfo> GetSuitTypeInfos()
        {
            List<SuitTypeInfo> infos = new List<SuitTypeInfo>();
            SuitTypeInfo AllSuitType = new SuitTypeInfo();
            AllSuitType.SuitType = -1;
            AllSuitType.TypeName = EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_54509");
            infos.Add(AllSuitType);

            var enumerator = mSuitInfo.Values.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var info = enumerator.Current;
                infos.Add(info);
            }
            return infos;
        }

        public EquipmentLevelUp GetEquipUpExpInfoByID(int id)
        {
            if (mEquipmentLevelup.ContainsKey(id))
            {
                return mEquipmentLevelup[id];
            }
            else
            {
                return null;
            }
        }
        public int SetEquipUpID(int quality, int level)
        {
            int i = 3 * 1000 + quality * 100 + level + 1;
            return i;
        }

        public List<EquipmentLevelUp> GetLevelUpListByQuality(int quality)
        {
            List<EquipmentLevelUp> mList = new List<EquipmentLevelUp>();
            for (int i = 1; i <= 15; ++i)
            {
                int tempId = 3 * 1000 + quality * 100 + i;
                EquipmentLevelUp data = GetEquipUpExpInfoByID(tempId);
                if (data != null) mList.Add(data);
            }
            /* mList.Sort((a, b) => {
                 if (int.Parse (a.id )> int.Parse(b.id))
                     return -1;
                 else if (int.Parse(a.id) < int.Parse(b.id))
                     return 1;
                 else if (a.level > b.level)
                     return -1;
                 else
                     return 1;
             });*/
            return mList;
        }
        public EquipmentAttribute GetEquipAttrInfo(int Id)
        {
            if (mEquipAttrs.ContainsKey(Id))
            {
                return mEquipAttrs[Id];
            }
            else
            {
                return null;
            }
        }

        public EquipAttributeRate GetEquipAttributeRate(int star)
        {
            if (mEquipAttrsRate.ContainsKey(star))
            {
                return mEquipAttrsRate[star];
            }

            return null;
        }

        /// <summary>
        /// 返回箱子的物品列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<SelectBox> GetSelectBoxListById(string id)
        {
            if (mSelectBoxDic.Count < 1)
            {
                int len = conditionSet.SelectBoxLength;
                for (int i = 0; i < len; i++)
                {
                    SelectBox selectBoxInfo = ParseSelectBox(conditionSet.GetSelectBox(i));
                    if (mSelectBoxDic.TryGetValue(selectBoxInfo.id, out List<SelectBox> data) == false)
                    {
                        var d = new List<SelectBox>
                        {
                            selectBoxInfo
                        };
                        mSelectBoxDic.Add(selectBoxInfo.id, d);
                    }
                    else
                    {
                        data.Add(selectBoxInfo);
                    }
                }
            }

            mSelectBoxDic.TryGetValue(id, out List<SelectBox> info);

            return info;
        }

        /// <summary>
        /// 根据物品id获取装备角标（如果该物品不是装备箱子则返回string.Empty）
        /// </summary>
        /// <param name="itemID">物品ID</param>
        /// <returns></returns>
        public string GetEquipSuit(string itemID)
        {
            string suitIcon = string.Empty;
            //目前物品表里面没有任何字段可以获取该物品是不是装备箱子，和策划商量只能通过物品id来判断，by:db,2019/4/2 
            int id = 0;
            if (!int.TryParse(itemID, out id))
            {
                EB.Debug.LogError("EconomyTemplateManager GetEquipSuit is Error, itemID is noNumber, itemID : {0}", itemID);
                return suitIcon;
            }

            if (id / 1000 != 3)
            {
                return suitIcon;
            }

            int suit = id % 100;
            SuitTypeInfo suitInfo = GetSuitTypeInfoByEcidSuitType(suit);
            if (suitInfo == null)
            {
                EB.Debug.LogError("EconomyTemplateManager GetEquipSuit SuitValue is Error, SuitValue : {0}", suit);
                return suitIcon;
            }

            suitIcon = suitInfo.SuitIcon;

            return suitIcon;
        }

        /// <summary>
        /// 根据物品id获取材料的阶级数（如果该物品不是材料箱子或者是材料箱子但只是0阶箱子，都只会返回0）
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public int GetGoodsGradeNum(string itemID)
        {
            //目前物品表里面没有任何字段可以获取该物品是不是材料箱子，和策划商量只能通过物品id来判断。ps:此处未来肯定会修改，蛋疼（虽然策划再三保证过不改）。  by:db,2019/4/3
            int id = 0;
            if (!int.TryParse(itemID, out id))
            {
                EB.Debug.LogError("EconomyTemplateManager GetGoodsGradeNum is Error, itemID is noNumber, itemID : {0}", itemID);
                return 0;
            }

            if (id / 100 != 15)
            {
                return 0;
            }

            //目前只有1501~1503， 蓝色材料箱开始才有阶级数，1502是蓝色材料箱，每三个为同一颜色，分别为0阶，1阶，2阶
            int grade = 0;
            if (id > 1501)
            {
                grade = (id - 1502) % 3;
            }

            return grade;
        }
    }

    #region 物品掉落跳转逻辑

    public abstract class DropDataBase
    {
        public DropType Type = DropType.None;
        public string Index1 = "";
        public string Index2 = "";
        public int FuncID;
        public bool IsOpen;

        public DropDataBase(DropType type, string index1, string index2)
        {
            Type = type;
            Index1 = index1;
            Index2 = index2;
        }

        public abstract void ShowName(UILabel label);

        public abstract void ShowBG(UISprite sprite);

        public abstract void GotoDrop(UIController curController = null);

        public void SetNameAndStatus(UILabel label, string text, bool enable)
        {
            LTUIUtil.SetText(label, text);
            label.transform.parent.GetChild(3).gameObject.SetActive(!enable);
        }

        public bool IsFuncOpen()
        {
            var func = FuncTemplateManager.Instance.GetFunc(FuncID);
            if (func != null)
            {
                return func.IsConditionOK();
            }
            //if (BalanceResourceUtil.GetUserLevel() >= GetFuncOpenLevel())
            //{
            //    return true;
            //}

            return false;
        }

        //public int GetFuncOpenLevel()
        //{
        //    return FuncTemplateManager.Instance.GetFunc(FuncID).NeedLevel;
        //}

        public string GetFuncLockStr()
        {
            return FuncTemplateManager.Instance.GetFunc(FuncID).GetConditionStrSpecial();
        }

        public string GetFuncName()
        {
            return FuncTemplateManager.Instance.GetFunc(FuncID).display_name;
        }
    }

    /// <summary>
    /// 主线副本
    /// </summary>
    public class SceneDropData : DropDataBase
    {
        private int chapterID;
        private int partID;
        private bool isPass;

        public SceneDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10036;
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!isPass)
            {
                if (partID != 0)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_57964"));
                }
                else
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_58111"));
                }
                return;
            }

            if (chapterID == 0)
            {
                if (curController != null)
                {
                    curController.Close();
                }

                GlobalMenuManager.Instance.ComebackToMianMenu();
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                GlobalMenuManager.Instance.Open("LTInstanceMapHud", null);
            }
            else if (chapterID != 0)
            {
                if (curController != null)
                {
                    curController.Close();
                }

                if (partID != 0)
                {
                    Hashtable data = Johny.HashtablePool.Claim();
                    data.Add("id", partID);
                    if (curController != null)
                    {
                        var itemController = curController.transform.GetMonoILRComponentByClassPath<GenericItemController>("Hotfix_LT.UI.GenericItemController", false);
                        data.Add("targetItemId", itemController.ItemInfo.EconomyId);
                    }

                    GlobalMenuManager.Instance.Open("LTMainInstanceCampaignView", data);
                }
                else
                {
                    int id = chapterID * 100 + 1;
                    Hashtable data = Johny.HashtablePool.Claim();
                    data.Add("id", id);
                    if (curController != null)
                    {
                        var itemController = curController.transform.GetMonoILRComponentByClassPath<GenericItemController>("Hotfix_LT.UI.GenericItemController", false);
                        data.Add("targetItemId", itemController.ItemInfo.EconomyId);
                    }

                    GlobalMenuManager.Instance.Open("LTMainInstanceCampaignView", data);
                }
            }
        }

        public override void ShowName(UILabel label)
        {
            if (!string.IsNullOrEmpty(Index1))
            {
                string[] ss = Index1.Split('_');
                int.TryParse(ss[0], out chapterID);
                if (ss.Length > 1)
                {
                    int.TryParse(ss[1], out partID);
                }
            }

            if (chapterID == 0 && partID == 0)
            {
                isPass = true;
            }
            else if (chapterID != 0 && partID != 0)
            {
                isPass = GetChapterPassOrNot(partID.ToString());
            }
            else if (chapterID != 0 && partID == 0)
            {
                isPass = LTInstanceUtil.IsPreChapterComplete(chapterID.ToString());
            }

            IsOpen = isPass;
            SetNameAndStatus(label, GetName(), IsOpen);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Zhuxianfuben";
        }

        private string GetName()
        {
            StringBuilder str = new StringBuilder(GetFuncName());

            if (chapterID != 0)
            {
                LostMainChapterTemplate chapter = SceneTemplateManager.Instance.GetLostMainChatpterTplById(chapterID.ToString());
                if (chapter != null)
                {
                    str.Append("\n" + chapter.Name);

                    if (partID != 0)
                    {
                        LostMainCampaignsTemplate campaign = SceneTemplateManager.Instance.GetLostMainCampaignTplById(partID.ToString());
                        if (chapter != null)
                        {
                            str.Append("：" + campaign.Name);
                        }
                        else
                        {
                            str.Append(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_60966"));
                        }
                    }
                    else
                    {
                        str.Append(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_61090"));
                    }
                }
                else
                {
                    str.Append(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_60966"));
                }
            }
            else
            {
                str.Append(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_61282"));
            }

            return str.ToString();
        }

        public bool GetChapterPassOrNot(string campaignId)
        {
            LostMainCampaignsTemplate campaignData = SceneTemplateManager.Instance.GetLostMainCampaignTplById(campaignId);
            if (campaignData == null)
            {
                return false;
            }

            LostMainChapterTemplate chapterData = SceneTemplateManager.Instance.GetLostMainChatpterTplById(campaignData.ChapterId);
            if (chapterData == null)
            {
                return false;
            }
            int isComplete = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("userCampaignStatus.normalChapters.{0}.campaigns.{1}.complete", campaignId.Substring(0, 3), campaignId), out isComplete);

            return BalanceResourceUtil.GetUserLevel() >= chapterData.LevelLimit&&LTInstanceUtil.GetIsChapterLimitConditionComplete(campaignData.ChapterId, out int num) && LTInstanceUtil.IsPreChapterComplete(campaignData.ChapterId) && isComplete == 1;
        }

    }

    /// <summary>
    /// 挑战副本
    /// </summary>
    public class ChallengeDropData : DropDataBase
    {
        public ChallengeDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10065;
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }

            if (!IsTimeOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_62898"));
                return;
            }

            if (curController != null)
            {
                curController.Close();
            }

            GlobalMenuManager.Instance.Open("LTChallengeInstanceSelectHud");
        }

        public override void ShowName(UILabel label)
        {
            string name = string.Format("{0}\n{1}", GetFuncName(), GetWeekStr());
            IsOpen = IsFuncOpen() && IsTimeOpen();
            SetNameAndStatus(label, name, IsOpen);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Tiaozhanfuben";
        }

        private string GetWeekStr()
        {
            StringBuilder str = new StringBuilder();
            if (!string.IsNullOrEmpty(Index1))
            {
                string[] tempStr = Index1.Split(',');
                for (int i = 0; i < tempStr.Length; i++)
                {
                    str.Append(EB.Localizer.GetString(GameStringValue.WeekDic[int.Parse(tempStr[i])]));

                    if (i < tempStr.Length - 1)
                    {
                        str.Append("、");
                    }
                }
            }
            return str.ToString();
        }

        private bool IsTimeOpen()
        {
            if (string.IsNullOrEmpty(Index1))
            {
                return true;
            }
            
            System.DateTime curTime = Data.ZoneTimeDiff.GetServerTime();
            int week = (int)curTime.DayOfWeek;
            if (curTime.Hour < 5)//凌晨五点前算前一天
            {
                week = ((week + 7) - 1) % 7;
            }

            week = week == 0 ? 7 : week;

            string[] tempStr = Index1.Split(',');
            for (int i = 0; i < tempStr.Length; i++)
            {
                if (week == int.Parse(tempStr[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// 商店
    /// </summary>
    public class StoreDropData : DropDataBase
    {
        //	herobattle  英雄交锋
        //	mystery     神秘
        //	arena       角斗场
        //	expedition  英雄交锋
        //	ladder      天梯
        //	alliance    国家

        private ShopTemplate shopData;
        private string shopName;

        public StoreDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!UIStoreController.IsStoreEnable(shopData.id))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_65166"), shopName));
                return;
            }

            GlobalMenuManager.Instance.Open("LTStoreUI", Index1);
        }

        public override void ShowName(UILabel label)
        {
            if (shopData == null)
            {
                string shopType = Index1 == "nation" ? "expedition" : Index1;
                shopData = ShopTemplateManager.Instance.GetShopByShopType(shopType);
                shopName = shopData == null ? "" : shopData.name;
            }
            IsOpen = UIStoreController.IsStoreEnable(shopData.id);
            SetNameAndStatus(label, shopName, IsOpen);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Shangdian";
        }
    }

    /// <summary>
    /// 抽奖
    /// </summary>
    public class LotteryDropData : DropDataBase
    {
        public LotteryDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10011;
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }

            object param = null;
            if (!string.IsNullOrEmpty(Index1))
            {
                param = (DrawCardType)int.Parse(Index1);
            }

            GlobalMenuManager.Instance.Open("LTDrawCardTypeUI", param);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Choujiang";
        }

        public override void ShowName(UILabel label)
        {
            string name = string.IsNullOrEmpty(Index1) ? EB.Localizer.GetString("ID_LOTTERY") : (DrawCardType)int.Parse(Index1) == DrawCardType.gold ? EB.Localizer.GetString("ID_GOLD_LOTTERY") : EB.Localizer.GetString("ID_HC_LOTTERY");
            IsOpen = IsFuncOpen();
            SetNameAndStatus(label, name, IsOpen);
        }
    }

    /// <summary>
    /// 极限试炼
    /// </summary>
    public class ExtremityTrialDropData : DropDataBase
    {
        public ExtremityTrialDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10015;
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }

            GlobalMenuManager.Instance.Open("LTUltimateTrialHudView");
        }

        public override void ShowName(UILabel label)
        {
            IsOpen = IsFuncOpen();
            SetNameAndStatus(label, GetName(IsOpen), IsOpen);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Jixianshilian";
        }

        private string GetName(bool isOpen)
        {
            StringBuilder str = new StringBuilder(GetFuncName());
            if (isOpen)
            {
                str.Append("\n");

                //极限试炼
                int times = 0;
                DataLookupsCache.Instance.SearchDataByID<int>("infiniteChallenge.info.currentTimes", out times);
            
                int dayDisCountTime = 0;
                int oldVigor = 0;
                int NewVigor = 0;
                NewGameConfigTemplateManager.Instance.GetEnterVigor(eBattleType.InfiniteChallenge, out dayDisCountTime, out NewVigor, out oldVigor);
                int curDisCountTime = dayDisCountTime - times;
                times= Mathf.Max(0, curDisCountTime);

                if (!string.IsNullOrEmpty(Index1))
                {
                    string[] strs = Index1.Split('_');
                    int floor = 0;
                    int.TryParse(strs[0], out floor);
                    str.Append(string.Format(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_68326"), floor));
                    if (strs.Length > 1)
                    {
                        int part = 0;
                        int.TryParse(strs[1], out part);
                        str.Append(string.Format(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_68536"), part));
                    }
                    else
                    {
                        str.Append(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_68646"));
                    }
                    // str.Append(string.Format("（{0}/{1}）", times, totalTimes));
                }
                else
                {
                    str.Append(string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_2985"), times));
                }
            }
            return str.ToString();
        }
    }

    /// <summary>
    /// 极限试炼竞速模式
    /// </summary>
    public class ExtremityCompeteDropData : DropDataBase
    {
        public ExtremityCompeteDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10015;
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }
            //if (!LTUltimateTrialDataManager.Instance.IsCanCompete())
            //{
            //    var mtpl = EventTemplateManager.Instance.GetInfiniteChallengeTpl(LTUltimateTrialDataManager.Instance.GetCompeteCondition());
            //    string temp = string.Format(EB.Localizer.GetString("ID_ULTIMATE_COMPETE_TIP"), mtpl.level);
            //    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, temp);
            //    return;
            //}
            //GlobalMenuManager.Instance.Open("LTUltimateTrialCompeteHud");
            GlobalMenuManager.Instance.Open("LTUltimateTrialHudView");
        }

        public override void ShowName(UILabel label)
        {
            IsOpen = IsFuncOpen();//&& LTUltimateTrialDataManager.Instance.IsCanCompete();
            SetNameAndStatus(label, GetName(IsOpen), IsOpen);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Jixianshilian";
        }

        private string GetName(bool isOpen)
        {
            StringBuilder str = new StringBuilder(GetFuncName());
            str.Append("\n");
            str.Append(EB.Localizer.GetString("ID_ULTIMATE_COMPETE_COMPETE"));
            return str.ToString();
        }
    }

    /// <summary>
    /// 幸运悬赏
    /// </summary>
    public class LuckyRewardDropData : DropDataBase
    {
        public LuckyRewardDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10068;
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (AllianceUtil.GetIsInTransferDart(null))
            {
                return;
            }

            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }

            if (IsRuning())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_69508"));
                return;
            }

            if (curController != null)
            {
                curController.Close();
            }

            GlobalMenuManager.Instance.ComebackToMianMenu();

            var encounterTpl = SceneTemplateManager.Instance.GetMainLandEncounter(FuncID);
            WorldMapPathManager.Instance.StartPathFindToNpcFly(MainLandLogic.GetInstance().CurrentSceneName, encounterTpl.mainland_name, encounterTpl.locator);
        }

        public override void ShowName(UILabel label)
        {
            IsOpen = IsFuncOpen() && !IsRuning();
            SetNameAndStatus(label, GetName(IsOpen), IsOpen);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Xuanshang";
        }

        private string GetName(bool isOpen)
        {
            StringBuilder str = new StringBuilder(GetFuncName());
            if (isOpen)
            {
                str.Append("\n");

                str.Append(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_68812"));
                str.Append(string.Format("{0}/{1}", LTBountyTaskHudController.CurHantTimes, LTBountyTaskHudController.TotalHantTimes));
            }

            return str.ToString();
        }

        private bool IsRuning()
        {
            return UI.TaskSystem.GetState(LTBountyTaskHudController.TaskID().ToString()) == UI.TaskSystem.RUNNING;
        }
    }

    /// <summary>
    /// 夺宝奇兵
    /// </summary>
    public class TreasuresDropData : DropDataBase
    {

        public TreasuresDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10073;
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }

            if (!IsTimeOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_62898"));
                return;
            }

            if (curController != null)
            {
                curController.Close();
            }

            GlobalMenuManager.Instance.ComebackToMianMenu();
            Hotfix_LT.Messenger.Raise("LTSpeedSnatchEvent.DropFunc");
        }

        public override void ShowName(UILabel label)
        {
            IsOpen = IsFuncOpen() && IsTimeOpen();
            SetNameAndStatus(label, GetFuncName(), IsOpen);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Duobaoqibing";
        }

        private bool IsTimeOpen()
        {
            return true;// EventTemplateManager.Instance.GetRealmIsOpen("main_land_ghost_start") && EventTemplateManager .Instance .IsTimeOK("main_land_ghost_start", "main_land_ghost_stop");//临时判断
        }
    }

    /// <summary>
    /// 运营活动
    /// </summary>
    public class ActivityDropData : DropDataBase
    {
        public ActivityDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
        }

        public override void GotoDrop(UIController curController = null)
        {
        }

        public override void ShowName(UILabel label)
        {
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Choujiang";
        }
    }

    /// <summary>
    /// 充值
    /// </summary>
    public class ChargeDropData : DropDataBase
    {
        public ChargeDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10026;
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }

            GlobalMenuManager.Instance.Open("LTChargeStoreHud");
        }

        public override void ShowName(UILabel label)
        {
            SetNameAndStatus(label, EB.Localizer.GetString("ID_CHARGE_PREFERENTIAL_GIFT"), IsFuncOpen());
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Shangdian";
        }
    }

    /// <summary>
    /// 军团副本
    /// </summary>
    public class AllianceDropData : DropDataBase
    {
        public AllianceDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10019;//与军团一起
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }

            //是否已加入了军团
            if (!LegionModel.GetInstance().isJoinedLegion)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDailyHudController_10226"));
                return;
            }

            GlobalMenuManager.Instance.Open("LTLegionFBUI");
        }

        public override void ShowName(UILabel label)
        {
            IsOpen = IsFuncOpen();
            SetNameAndStatus(label, EB.Localizer.GetString("ID_ALLIANCE_COPY"), IsOpen);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Xuanshang";
        }

    }

    /// <summary>
    /// 异界迷宫
    /// </summary>
    public class MazeDropData : DropDataBase
    {
        public MazeDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10084;
        }

        public override void GotoDrop(UIController curController = null)
        {
            FuncTemplateManager.Instance.GetFunc(FuncID);
            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }

            GlobalMenuManager.Instance.Open("LTAlienMazeHud");
        }

        public override void ShowName(UILabel label)
        {
            IsOpen = IsFuncOpen();
            SetNameAndStatus(label, GetFuncName(), IsOpen);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Yijiemigong";
        }

    }

    /// <summary>
    /// 睡梦之塔
    /// </summary>
    public class SleepDropData : DropDataBase
    {
        public SleepDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10085;
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }

            GlobalMenuManager.Instance.Open("LTClimbingTowerHud");
        }

        public override void ShowName(UILabel label)
        {
            IsOpen = IsFuncOpen();
            SetNameAndStatus(label, GetFuncName(), IsOpen);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Shuimengzhita";
        }

    }

    /// <summary>
    /// 觉醒副本
    /// </summary>
    public class AwakenDropData : DropDataBase
    {
        private eRoleAttr roleAttr = eRoleAttr.None;
        public AwakenDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10083;
            if (string.IsNullOrEmpty(index1))
            {
                roleAttr = eRoleAttr.None;
            }
            else
            {
                roleAttr = (eRoleAttr)int.Parse(Index1);
            }

        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }

            if (!IsTimeOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_62898"));
                return;
            }
            if (roleAttr == eRoleAttr.None)
            {
                GlobalMenuManager.Instance.Open("LTAwakeningInstanceHud");
            }
            else
            {
                GlobalMenuManager.Instance.PushCache("LTAwakeningInstanceHud");
                GlobalMenuManager.Instance.Open("LTAwakeningInstanceSelectHud", roleAttr);
            }
        }
        private bool IsTimeOpen()
        {
            if (roleAttr == eRoleAttr.None)
            {
                return true;
            }

            System.DateTime curTime = UI.TaskSystem.TimeSpanToDateTime(EB.Time.Now);
            int week = Convert.ToInt32(curTime.DayOfWeek);

            string NewGameConfig = string.Empty;
            switch (roleAttr)
            {
                case eRoleAttr.Feng: NewGameConfig = "WindOpenWeek"; break;
                case eRoleAttr.Shui: NewGameConfig = "WaterOpenWeek"; break;
                default: NewGameConfig = "FireOpenWeek"; break;
            }
            string goodsStrS = NewGameConfigTemplateManager.Instance.GetGameConfigStrValue(NewGameConfig);
            string[] tempStr = goodsStrS.Split(',');
            for (int i = 0; i < tempStr.Length; i++)
            {
                if (week == int.Parse(tempStr[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public override void ShowName(UILabel label)
        {
            string formatStr = "{0}";
            string funcName = string.Empty;
            if (roleAttr == eRoleAttr.None)
            {
                funcName = GetFuncName();
            }
            else
            {
                formatStr = "{0}\n{1}";
                switch (roleAttr)
                {
                    case eRoleAttr.Feng: funcName = EB.Localizer.GetString("ID_AWAKENDUNGEON_WIND"); ; break;
                    case eRoleAttr.Shui: funcName = EB.Localizer.GetString("ID_AWAKENDUNGEON_WATER"); ; break;
                    default: funcName = EB.Localizer.GetString("ID_AWAKENDUNGEON_FIRE"); ; break;
                }
            }

            string name = string.Format(formatStr, funcName, GetWeekStr());
            IsOpen = IsFuncOpen() && IsTimeOpen();
            SetNameAndStatus(label, name, IsOpen);
        }

        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Juexing";
        }

        private string GetWeekStr()
        {
            StringBuilder str = new StringBuilder();
            if (!string.IsNullOrEmpty(Index1))
            {
                string NewGameConfig = string.Empty;
                switch (roleAttr)
                {
                    case eRoleAttr.Feng: NewGameConfig = "WindOpenWeek"; break;
                    case eRoleAttr.Shui: NewGameConfig = "WaterOpenWeek"; break;
                    default: NewGameConfig = "FireOpenWeek"; break;
                }
                string goodsStrS = NewGameConfigTemplateManager.Instance.GetGameConfigStrValue(NewGameConfig);
                string[] tempStr = goodsStrS.Split(',');
                for (int i = 0; i < tempStr.Length; i++)
                {
                    int week = int.Parse(tempStr[i]);
                    week = week == 0 ? 7 : week;
                    str.Append(EB.Localizer.GetString(GameStringValue.WeekDic[week]));

                    if (i < tempStr.Length - 1)
                    {
                        str.Append("、");
                    }
                }
            }
            return str.ToString();
        }
    }

    /// <summary>
    /// 合成
    /// </summary>
    public class ComposeDropData : DropDataBase
    {
        public ComposeDropData(DropType type, string index1, string index2) : base(type, index1, index2)
        {
            FuncID = 10054;
        }

        public override void GotoDrop(UIController curController = null)
        {
            if (!IsFuncOpen())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, GetFuncLockStr());
                return;
            }

            GlobalMenuManager.Instance.Open("LTAwakeningGenericTrans", Index1);
        }

        public override void ShowName(UILabel label)
        {
            IsOpen = IsFuncOpen();
            SetNameAndStatus(label, EB.Localizer.GetString("ID_AWAKENDUNGEON_TRANS"), IsOpen);
        }


        public override void ShowBG(UISprite sprite)
        {
            sprite.spriteName = "Goods_Source_Shangdian";
        }
    }

    #endregion
}