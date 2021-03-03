// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class AllianceWarReward : Table {
  public static AllianceWarReward GetRootAsAllianceWarReward(ByteBuffer _bb) { return GetRootAsAllianceWarReward(_bb, new AllianceWarReward()); }
  public static AllianceWarReward GetRootAsAllianceWarReward(ByteBuffer _bb, AllianceWarReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AllianceWarReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Stage { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Result { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType1 { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType1Bytes() { return __vector_as_arraysegment(8); }
  public string ItemId1 { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId1Bytes() { return __vector_as_arraysegment(10); }
  public int ItemNum1 { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType2 { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType2Bytes() { return __vector_as_arraysegment(14); }
  public string ItemId2 { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId2Bytes() { return __vector_as_arraysegment(16); }
  public int ItemNum2 { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType3 { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType3Bytes() { return __vector_as_arraysegment(20); }
  public string ItemId3 { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId3Bytes() { return __vector_as_arraysegment(22); }
  public int ItemNum3 { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType4 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType4Bytes() { return __vector_as_arraysegment(26); }
  public string ItemId4 { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId4Bytes() { return __vector_as_arraysegment(28); }
  public int ItemNum4 { get { int o = __offset(30); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<AllianceWarReward> CreateAllianceWarReward(FlatBufferBuilder builder,
      int stage = 0,
      int result = 0,
      StringOffset item_type1Offset = default(StringOffset),
      StringOffset item_id1Offset = default(StringOffset),
      int item_num1 = 0,
      StringOffset item_type2Offset = default(StringOffset),
      StringOffset item_id2Offset = default(StringOffset),
      int item_num2 = 0,
      StringOffset item_type3Offset = default(StringOffset),
      StringOffset item_id3Offset = default(StringOffset),
      int item_num3 = 0,
      StringOffset item_type4Offset = default(StringOffset),
      StringOffset item_id4Offset = default(StringOffset),
      int item_num4 = 0) {
    builder.StartObject(14);
    AllianceWarReward.AddItemNum4(builder, item_num4);
    AllianceWarReward.AddItemId4(builder, item_id4Offset);
    AllianceWarReward.AddItemType4(builder, item_type4Offset);
    AllianceWarReward.AddItemNum3(builder, item_num3);
    AllianceWarReward.AddItemId3(builder, item_id3Offset);
    AllianceWarReward.AddItemType3(builder, item_type3Offset);
    AllianceWarReward.AddItemNum2(builder, item_num2);
    AllianceWarReward.AddItemId2(builder, item_id2Offset);
    AllianceWarReward.AddItemType2(builder, item_type2Offset);
    AllianceWarReward.AddItemNum1(builder, item_num1);
    AllianceWarReward.AddItemId1(builder, item_id1Offset);
    AllianceWarReward.AddItemType1(builder, item_type1Offset);
    AllianceWarReward.AddResult(builder, result);
    AllianceWarReward.AddStage(builder, stage);
    return AllianceWarReward.EndAllianceWarReward(builder);
  }

  public static void StartAllianceWarReward(FlatBufferBuilder builder) { builder.StartObject(14); }
  public static void AddStage(FlatBufferBuilder builder, int stage) { builder.AddInt(0, stage, 0); }
  public static void AddResult(FlatBufferBuilder builder, int result) { builder.AddInt(1, result, 0); }
  public static void AddItemType1(FlatBufferBuilder builder, StringOffset itemType1Offset) { builder.AddOffset(2, itemType1Offset.Value, 0); }
  public static void AddItemId1(FlatBufferBuilder builder, StringOffset itemId1Offset) { builder.AddOffset(3, itemId1Offset.Value, 0); }
  public static void AddItemNum1(FlatBufferBuilder builder, int itemNum1) { builder.AddInt(4, itemNum1, 0); }
  public static void AddItemType2(FlatBufferBuilder builder, StringOffset itemType2Offset) { builder.AddOffset(5, itemType2Offset.Value, 0); }
  public static void AddItemId2(FlatBufferBuilder builder, StringOffset itemId2Offset) { builder.AddOffset(6, itemId2Offset.Value, 0); }
  public static void AddItemNum2(FlatBufferBuilder builder, int itemNum2) { builder.AddInt(7, itemNum2, 0); }
  public static void AddItemType3(FlatBufferBuilder builder, StringOffset itemType3Offset) { builder.AddOffset(8, itemType3Offset.Value, 0); }
  public static void AddItemId3(FlatBufferBuilder builder, StringOffset itemId3Offset) { builder.AddOffset(9, itemId3Offset.Value, 0); }
  public static void AddItemNum3(FlatBufferBuilder builder, int itemNum3) { builder.AddInt(10, itemNum3, 0); }
  public static void AddItemType4(FlatBufferBuilder builder, StringOffset itemType4Offset) { builder.AddOffset(11, itemType4Offset.Value, 0); }
  public static void AddItemId4(FlatBufferBuilder builder, StringOffset itemId4Offset) { builder.AddOffset(12, itemId4Offset.Value, 0); }
  public static void AddItemNum4(FlatBufferBuilder builder, int itemNum4) { builder.AddInt(13, itemNum4, 0); }
  public static Offset<AllianceWarReward> EndAllianceWarReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AllianceWarReward>(o);
  }
};

public sealed class AllianceLevelInfo : Table {
  public static AllianceLevelInfo GetRootAsAllianceLevelInfo(ByteBuffer _bb) { return GetRootAsAllianceLevelInfo(_bb, new AllianceLevelInfo()); }
  public static AllianceLevelInfo GetRootAsAllianceLevelInfo(ByteBuffer _bb, AllianceLevelInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AllianceLevelInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Level { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LevelupExp { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MaxMember { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float ATKAdd { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float DEFAdd { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float MaxHPAdd { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<AllianceLevelInfo> CreateAllianceLevelInfo(FlatBufferBuilder builder,
      int id = 0,
      int level = 0,
      int levelup_exp = 0,
      int max_member = 0,
      float ATK_add = 0,
      float DEF_add = 0,
      float MaxHP_add = 0) {
    builder.StartObject(7);
    AllianceLevelInfo.AddMaxHPAdd(builder, MaxHP_add);
    AllianceLevelInfo.AddDEFAdd(builder, DEF_add);
    AllianceLevelInfo.AddATKAdd(builder, ATK_add);
    AllianceLevelInfo.AddMaxMember(builder, max_member);
    AllianceLevelInfo.AddLevelupExp(builder, levelup_exp);
    AllianceLevelInfo.AddLevel(builder, level);
    AllianceLevelInfo.AddId(builder, id);
    return AllianceLevelInfo.EndAllianceLevelInfo(builder);
  }

  public static void StartAllianceLevelInfo(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(1, level, 0); }
  public static void AddLevelupExp(FlatBufferBuilder builder, int levelupExp) { builder.AddInt(2, levelupExp, 0); }
  public static void AddMaxMember(FlatBufferBuilder builder, int maxMember) { builder.AddInt(3, maxMember, 0); }
  public static void AddATKAdd(FlatBufferBuilder builder, float ATKAdd) { builder.AddFloat(4, ATKAdd, 0); }
  public static void AddDEFAdd(FlatBufferBuilder builder, float DEFAdd) { builder.AddFloat(5, DEFAdd, 0); }
  public static void AddMaxHPAdd(FlatBufferBuilder builder, float MaxHPAdd) { builder.AddFloat(6, MaxHPAdd, 0); }
  public static Offset<AllianceLevelInfo> EndAllianceLevelInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AllianceLevelInfo>(o);
  }
};

public sealed class AllianceTechnologyInfo : Table {
  public static AllianceTechnologyInfo GetRootAsAllianceTechnologyInfo(ByteBuffer _bb) { return GetRootAsAllianceTechnologyInfo(_bb, new AllianceTechnologyInfo()); }
  public static AllianceTechnologyInfo GetRootAsAllianceTechnologyInfo(ByteBuffer _bb, AllianceTechnologyInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AllianceTechnologyInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int TechLevel { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CostBalance { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AllianceLevelLimit { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MaxSkillLevel { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<AllianceTechnologyInfo> CreateAllianceTechnologyInfo(FlatBufferBuilder builder,
      int id = 0,
      int tech_level = 0,
      int cost_balance = 0,
      int alliance_level_limit = 0,
      int max_skill_level = 0) {
    builder.StartObject(5);
    AllianceTechnologyInfo.AddMaxSkillLevel(builder, max_skill_level);
    AllianceTechnologyInfo.AddAllianceLevelLimit(builder, alliance_level_limit);
    AllianceTechnologyInfo.AddCostBalance(builder, cost_balance);
    AllianceTechnologyInfo.AddTechLevel(builder, tech_level);
    AllianceTechnologyInfo.AddId(builder, id);
    return AllianceTechnologyInfo.EndAllianceTechnologyInfo(builder);
  }

  public static void StartAllianceTechnologyInfo(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddTechLevel(FlatBufferBuilder builder, int techLevel) { builder.AddInt(1, techLevel, 0); }
  public static void AddCostBalance(FlatBufferBuilder builder, int costBalance) { builder.AddInt(2, costBalance, 0); }
  public static void AddAllianceLevelLimit(FlatBufferBuilder builder, int allianceLevelLimit) { builder.AddInt(3, allianceLevelLimit, 0); }
  public static void AddMaxSkillLevel(FlatBufferBuilder builder, int maxSkillLevel) { builder.AddInt(4, maxSkillLevel, 0); }
  public static Offset<AllianceTechnologyInfo> EndAllianceTechnologyInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AllianceTechnologyInfo>(o);
  }
};

public sealed class AllianceSkillInfo : Table {
  public static AllianceSkillInfo GetRootAsAllianceSkillInfo(ByteBuffer _bb) { return GetRootAsAllianceSkillInfo(_bb, new AllianceSkillInfo()); }
  public static AllianceSkillInfo GetRootAsAllianceSkillInfo(ByteBuffer _bb, AllianceSkillInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AllianceSkillInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillLevel { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Cost { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MaxHP { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PATK { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PDEF { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MATK { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MDEF { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Critical { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CriticalHit { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Penetration { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SpellPenetration { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int DamageReduction { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int HealRecover { get { int o = __offset(30); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<AllianceSkillInfo> CreateAllianceSkillInfo(FlatBufferBuilder builder,
      int id = 0,
      int skill_level = 0,
      int cost = 0,
      int MaxHP = 0,
      int PATK = 0,
      int PDEF = 0,
      int MATK = 0,
      int MDEF = 0,
      int critical = 0,
      int critical_hit = 0,
      int penetration = 0,
      int spell_penetration = 0,
      int damage_reduction = 0,
      int heal_recover = 0) {
    builder.StartObject(14);
    AllianceSkillInfo.AddHealRecover(builder, heal_recover);
    AllianceSkillInfo.AddDamageReduction(builder, damage_reduction);
    AllianceSkillInfo.AddSpellPenetration(builder, spell_penetration);
    AllianceSkillInfo.AddPenetration(builder, penetration);
    AllianceSkillInfo.AddCriticalHit(builder, critical_hit);
    AllianceSkillInfo.AddCritical(builder, critical);
    AllianceSkillInfo.AddMDEF(builder, MDEF);
    AllianceSkillInfo.AddMATK(builder, MATK);
    AllianceSkillInfo.AddPDEF(builder, PDEF);
    AllianceSkillInfo.AddPATK(builder, PATK);
    AllianceSkillInfo.AddMaxHP(builder, MaxHP);
    AllianceSkillInfo.AddCost(builder, cost);
    AllianceSkillInfo.AddSkillLevel(builder, skill_level);
    AllianceSkillInfo.AddId(builder, id);
    return AllianceSkillInfo.EndAllianceSkillInfo(builder);
  }

  public static void StartAllianceSkillInfo(FlatBufferBuilder builder) { builder.StartObject(14); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddSkillLevel(FlatBufferBuilder builder, int skillLevel) { builder.AddInt(1, skillLevel, 0); }
  public static void AddCost(FlatBufferBuilder builder, int cost) { builder.AddInt(2, cost, 0); }
  public static void AddMaxHP(FlatBufferBuilder builder, int MaxHP) { builder.AddInt(3, MaxHP, 0); }
  public static void AddPATK(FlatBufferBuilder builder, int PATK) { builder.AddInt(4, PATK, 0); }
  public static void AddPDEF(FlatBufferBuilder builder, int PDEF) { builder.AddInt(5, PDEF, 0); }
  public static void AddMATK(FlatBufferBuilder builder, int MATK) { builder.AddInt(6, MATK, 0); }
  public static void AddMDEF(FlatBufferBuilder builder, int MDEF) { builder.AddInt(7, MDEF, 0); }
  public static void AddCritical(FlatBufferBuilder builder, int critical) { builder.AddInt(8, critical, 0); }
  public static void AddCriticalHit(FlatBufferBuilder builder, int criticalHit) { builder.AddInt(9, criticalHit, 0); }
  public static void AddPenetration(FlatBufferBuilder builder, int penetration) { builder.AddInt(10, penetration, 0); }
  public static void AddSpellPenetration(FlatBufferBuilder builder, int spellPenetration) { builder.AddInt(11, spellPenetration, 0); }
  public static void AddDamageReduction(FlatBufferBuilder builder, int damageReduction) { builder.AddInt(12, damageReduction, 0); }
  public static void AddHealRecover(FlatBufferBuilder builder, int healRecover) { builder.AddInt(13, healRecover, 0); }
  public static Offset<AllianceSkillInfo> EndAllianceSkillInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AllianceSkillInfo>(o);
  }
};

public sealed class CampaignBuff : Table {
  public static CampaignBuff GetRootAsCampaignBuff(ByteBuffer _bb) { return GetRootAsCampaignBuff(_bb, new CampaignBuff()); }
  public static CampaignBuff GetRootAsCampaignBuff(ByteBuffer _bb, CampaignBuff obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public CampaignBuff __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BuffIcon { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBuffIconBytes() { return __vector_as_arraysegment(6); }
  public int BuffId { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BuffDesc { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBuffDescBytes() { return __vector_as_arraysegment(10); }

  public static Offset<CampaignBuff> CreateCampaignBuff(FlatBufferBuilder builder,
      int id = 0,
      StringOffset buff_iconOffset = default(StringOffset),
      int buff_id = 0,
      StringOffset buff_descOffset = default(StringOffset)) {
    builder.StartObject(4);
    CampaignBuff.AddBuffDesc(builder, buff_descOffset);
    CampaignBuff.AddBuffId(builder, buff_id);
    CampaignBuff.AddBuffIcon(builder, buff_iconOffset);
    CampaignBuff.AddId(builder, id);
    return CampaignBuff.EndCampaignBuff(builder);
  }

  public static void StartCampaignBuff(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddBuffIcon(FlatBufferBuilder builder, StringOffset buffIconOffset) { builder.AddOffset(1, buffIconOffset.Value, 0); }
  public static void AddBuffId(FlatBufferBuilder builder, int buffId) { builder.AddInt(2, buffId, 0); }
  public static void AddBuffDesc(FlatBufferBuilder builder, StringOffset buffDescOffset) { builder.AddOffset(3, buffDescOffset.Value, 0); }
  public static Offset<CampaignBuff> EndCampaignBuff(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<CampaignBuff>(o);
  }
};

public sealed class AllianceWarConfig : Table {
  public static AllianceWarConfig GetRootAsAllianceWarConfig(ByteBuffer _bb) { return GetRootAsAllianceWarConfig(_bb, new AllianceWarConfig()); }
  public static AllianceWarConfig GetRootAsAllianceWarConfig(ByteBuffer _bb, AllianceWarConfig obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AllianceWarConfig __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(4); }
  public float Value { get { int o = __offset(6); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<AllianceWarConfig> CreateAllianceWarConfig(FlatBufferBuilder builder,
      StringOffset nameOffset = default(StringOffset),
      float value = 0) {
    builder.StartObject(2);
    AllianceWarConfig.AddValue(builder, value);
    AllianceWarConfig.AddName(builder, nameOffset);
    return AllianceWarConfig.EndAllianceWarConfig(builder);
  }

  public static void StartAllianceWarConfig(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddValue(FlatBufferBuilder builder, float value) { builder.AddFloat(1, value, 0); }
  public static Offset<AllianceWarConfig> EndAllianceWarConfig(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AllianceWarConfig>(o);
  }
};

public sealed class ConditionAlliance : Table {
  public static ConditionAlliance GetRootAsConditionAlliance(ByteBuffer _bb) { return GetRootAsConditionAlliance(_bb, new ConditionAlliance()); }
  public static ConditionAlliance GetRootAsConditionAlliance(ByteBuffer _bb, ConditionAlliance obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionAlliance __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string _id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? Get_idBytes() { return __vector_as_arraysegment(4); }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public bool Enabled { get { int o = __offset(8); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int Priority { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public GM.DataCache.DateCondition GetDateConditions(int j) { return GetDateConditions(new GM.DataCache.DateCondition(), j); }
  public GM.DataCache.DateCondition GetDateConditions(GM.DataCache.DateCondition obj, int j) { int o = __offset(12); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int DateConditionsLength { get { int o = __offset(12); return o != 0 ? __vector_len(o) : 0; } }
  public GM.DataCache.UserCondition GetUserConditions(int j) { return GetUserConditions(new GM.DataCache.UserCondition(), j); }
  public GM.DataCache.UserCondition GetUserConditions(GM.DataCache.UserCondition obj, int j) { int o = __offset(14); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int UserConditionsLength { get { int o = __offset(14); return o != 0 ? __vector_len(o) : 0; } }
  public GM.DataCache.Options Options { get { return GetOptions(new GM.DataCache.Options()); } }
  public GM.DataCache.Options GetOptions(GM.DataCache.Options obj) { int o = __offset(16); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public AllianceLevelInfo GetLevel(int j) { return GetLevel(new AllianceLevelInfo(), j); }
  public AllianceLevelInfo GetLevel(AllianceLevelInfo obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LevelLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public AllianceTechnologyInfo GetTechnology(int j) { return GetTechnology(new AllianceTechnologyInfo(), j); }
  public AllianceTechnologyInfo GetTechnology(AllianceTechnologyInfo obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TechnologyLength { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public AllianceSkillInfo GetSkill(int j) { return GetSkill(new AllianceSkillInfo(), j); }
  public AllianceSkillInfo GetSkill(AllianceSkillInfo obj, int j) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SkillLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }
  public CampaignBuff GetCampaignBuff(int j) { return GetCampaignBuff(new CampaignBuff(), j); }
  public CampaignBuff GetCampaignBuff(CampaignBuff obj, int j) { int o = __offset(24); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int CampaignBuffLength { get { int o = __offset(24); return o != 0 ? __vector_len(o) : 0; } }
  public AllianceWarReward GetAllianceWarReward(int j) { return GetAllianceWarReward(new AllianceWarReward(), j); }
  public AllianceWarReward GetAllianceWarReward(AllianceWarReward obj, int j) { int o = __offset(26); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AllianceWarRewardLength { get { int o = __offset(26); return o != 0 ? __vector_len(o) : 0; } }
  public AllianceWarConfig GetAllianceWarConfig(int j) { return GetAllianceWarConfig(new AllianceWarConfig(), j); }
  public AllianceWarConfig GetAllianceWarConfig(AllianceWarConfig obj, int j) { int o = __offset(28); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AllianceWarConfigLength { get { int o = __offset(28); return o != 0 ? __vector_len(o) : 0; } }
  public AlliancesFBBoss GetAllianceFbBoss(int j) { return GetAllianceFbBoss(new AlliancesFBBoss(), j); }
  public AlliancesFBBoss GetAllianceFbBoss(AlliancesFBBoss obj, int j) { int o = __offset(30); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AllianceFbBossLength { get { int o = __offset(30); return o != 0 ? __vector_len(o) : 0; } }
  public AlliancesFBHurt GetAllianceFbHurt(int j) { return GetAllianceFbHurt(new AlliancesFBHurt(), j); }
  public AlliancesFBHurt GetAllianceFbHurt(AlliancesFBHurt obj, int j) { int o = __offset(32); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AllianceFbHurtLength { get { int o = __offset(32); return o != 0 ? __vector_len(o) : 0; } }
  public DonateChest GetDonateChest(int j) { return GetDonateChest(new DonateChest(), j); }
  public DonateChest GetDonateChest(DonateChest obj, int j) { int o = __offset(34); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int DonateChestLength { get { int o = __offset(34); return o != 0 ? __vector_len(o) : 0; } }
  public TechSkill GetTechSkill(int j) { return GetTechSkill(new TechSkill(), j); }
  public TechSkill GetTechSkill(TechSkill obj, int j) { int o = __offset(36); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TechSkillLength { get { int o = __offset(36); return o != 0 ? __vector_len(o) : 0; } }
  public TechLevelChest GetTechLevelChest(int j) { return GetTechLevelChest(new TechLevelChest(), j); }
  public TechLevelChest GetTechLevelChest(TechLevelChest obj, int j) { int o = __offset(38); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TechLevelChestLength { get { int o = __offset(38); return o != 0 ? __vector_len(o) : 0; } }
  public TechSkillLevelUp GetTechSkillLevelUp(int j) { return GetTechSkillLevelUp(new TechSkillLevelUp(), j); }
  public TechSkillLevelUp GetTechSkillLevelUp(TechSkillLevelUp obj, int j) { int o = __offset(40); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TechSkillLevelUpLength { get { int o = __offset(40); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionAlliance> CreateConditionAlliance(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = false,
      int priority = 0,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset levelOffset = default(VectorOffset),
      VectorOffset technologyOffset = default(VectorOffset),
      VectorOffset skillOffset = default(VectorOffset),
      VectorOffset campaign_buffOffset = default(VectorOffset),
      VectorOffset alliance_war_rewardOffset = default(VectorOffset),
      VectorOffset alliance_war_configOffset = default(VectorOffset),
      VectorOffset alliance_fb_bossOffset = default(VectorOffset),
      VectorOffset alliance_fb_hurtOffset = default(VectorOffset),
      VectorOffset donate_chestOffset = default(VectorOffset),
      VectorOffset tech_skillOffset = default(VectorOffset),
      VectorOffset tech_level_chestOffset = default(VectorOffset),
      VectorOffset tech_skill_level_upOffset = default(VectorOffset)) {
    builder.StartObject(19);
    ConditionAlliance.AddTechSkillLevelUp(builder, tech_skill_level_upOffset);
    ConditionAlliance.AddTechLevelChest(builder, tech_level_chestOffset);
    ConditionAlliance.AddTechSkill(builder, tech_skillOffset);
    ConditionAlliance.AddDonateChest(builder, donate_chestOffset);
    ConditionAlliance.AddAllianceFbHurt(builder, alliance_fb_hurtOffset);
    ConditionAlliance.AddAllianceFbBoss(builder, alliance_fb_bossOffset);
    ConditionAlliance.AddAllianceWarConfig(builder, alliance_war_configOffset);
    ConditionAlliance.AddAllianceWarReward(builder, alliance_war_rewardOffset);
    ConditionAlliance.AddCampaignBuff(builder, campaign_buffOffset);
    ConditionAlliance.AddSkill(builder, skillOffset);
    ConditionAlliance.AddTechnology(builder, technologyOffset);
    ConditionAlliance.AddLevel(builder, levelOffset);
    ConditionAlliance.AddOptions(builder, optionsOffset);
    ConditionAlliance.AddUserConditions(builder, user_conditionsOffset);
    ConditionAlliance.AddDateConditions(builder, date_conditionsOffset);
    ConditionAlliance.AddPriority(builder, priority);
    ConditionAlliance.AddName(builder, nameOffset);
    ConditionAlliance.Add_id(builder, _idOffset);
    ConditionAlliance.AddEnabled(builder, enabled);
    return ConditionAlliance.EndConditionAlliance(builder);
  }

  public static void StartConditionAlliance(FlatBufferBuilder builder) { builder.StartObject(19); }
  public static void Add_id(FlatBufferBuilder builder, StringOffset IdOffset) { builder.AddOffset(0, IdOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddEnabled(FlatBufferBuilder builder, bool enabled) { builder.AddBool(2, enabled, false); }
  public static void AddPriority(FlatBufferBuilder builder, int priority) { builder.AddInt(3, priority, 0); }
  public static void AddDateConditions(FlatBufferBuilder builder, VectorOffset dateConditionsOffset) { builder.AddOffset(4, dateConditionsOffset.Value, 0); }
  public static VectorOffset CreateDateConditionsVector(FlatBufferBuilder builder, Offset<GM.DataCache.DateCondition>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDateConditionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddUserConditions(FlatBufferBuilder builder, VectorOffset userConditionsOffset) { builder.AddOffset(5, userConditionsOffset.Value, 0); }
  public static VectorOffset CreateUserConditionsVector(FlatBufferBuilder builder, Offset<GM.DataCache.UserCondition>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartUserConditionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddOptions(FlatBufferBuilder builder, Offset<GM.DataCache.Options> optionsOffset) { builder.AddOffset(6, optionsOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, VectorOffset levelOffset) { builder.AddOffset(7, levelOffset.Value, 0); }
  public static VectorOffset CreateLevelVector(FlatBufferBuilder builder, Offset<AllianceLevelInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLevelVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTechnology(FlatBufferBuilder builder, VectorOffset technologyOffset) { builder.AddOffset(8, technologyOffset.Value, 0); }
  public static VectorOffset CreateTechnologyVector(FlatBufferBuilder builder, Offset<AllianceTechnologyInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTechnologyVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSkill(FlatBufferBuilder builder, VectorOffset skillOffset) { builder.AddOffset(9, skillOffset.Value, 0); }
  public static VectorOffset CreateSkillVector(FlatBufferBuilder builder, Offset<AllianceSkillInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSkillVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddCampaignBuff(FlatBufferBuilder builder, VectorOffset campaignBuffOffset) { builder.AddOffset(10, campaignBuffOffset.Value, 0); }
  public static VectorOffset CreateCampaignBuffVector(FlatBufferBuilder builder, Offset<CampaignBuff>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartCampaignBuffVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAllianceWarReward(FlatBufferBuilder builder, VectorOffset allianceWarRewardOffset) { builder.AddOffset(11, allianceWarRewardOffset.Value, 0); }
  public static VectorOffset CreateAllianceWarRewardVector(FlatBufferBuilder builder, Offset<AllianceWarReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAllianceWarRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAllianceWarConfig(FlatBufferBuilder builder, VectorOffset allianceWarConfigOffset) { builder.AddOffset(12, allianceWarConfigOffset.Value, 0); }
  public static VectorOffset CreateAllianceWarConfigVector(FlatBufferBuilder builder, Offset<AllianceWarConfig>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAllianceWarConfigVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAllianceFbBoss(FlatBufferBuilder builder, VectorOffset allianceFbBossOffset) { builder.AddOffset(13, allianceFbBossOffset.Value, 0); }
  public static VectorOffset CreateAllianceFbBossVector(FlatBufferBuilder builder, Offset<AlliancesFBBoss>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAllianceFbBossVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAllianceFbHurt(FlatBufferBuilder builder, VectorOffset allianceFbHurtOffset) { builder.AddOffset(14, allianceFbHurtOffset.Value, 0); }
  public static VectorOffset CreateAllianceFbHurtVector(FlatBufferBuilder builder, Offset<AlliancesFBHurt>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAllianceFbHurtVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDonateChest(FlatBufferBuilder builder, VectorOffset donateChestOffset) { builder.AddOffset(15, donateChestOffset.Value, 0); }
  public static VectorOffset CreateDonateChestVector(FlatBufferBuilder builder, Offset<DonateChest>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDonateChestVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTechSkill(FlatBufferBuilder builder, VectorOffset techSkillOffset) { builder.AddOffset(16, techSkillOffset.Value, 0); }
  public static VectorOffset CreateTechSkillVector(FlatBufferBuilder builder, Offset<TechSkill>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTechSkillVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTechLevelChest(FlatBufferBuilder builder, VectorOffset techLevelChestOffset) { builder.AddOffset(17, techLevelChestOffset.Value, 0); }
  public static VectorOffset CreateTechLevelChestVector(FlatBufferBuilder builder, Offset<TechLevelChest>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTechLevelChestVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTechSkillLevelUp(FlatBufferBuilder builder, VectorOffset techSkillLevelUpOffset) { builder.AddOffset(18, techSkillLevelUpOffset.Value, 0); }
  public static VectorOffset CreateTechSkillLevelUpVector(FlatBufferBuilder builder, Offset<TechSkillLevelUp>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTechSkillLevelUpVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionAlliance> EndConditionAlliance(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionAlliance>(o);
  }
};

public sealed class Alliance : Table {
  public static Alliance GetRootAsAlliance(ByteBuffer _bb) { return GetRootAsAlliance(_bb, new Alliance()); }
  public static Alliance GetRootAsAlliance(ByteBuffer _bb, Alliance obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Alliance __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionAlliance GetArray(int j) { return GetArray(new ConditionAlliance(), j); }
  public ConditionAlliance GetArray(ConditionAlliance obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Alliance> CreateAlliance(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Alliance.AddArray(builder, arrayOffset);
    return Alliance.EndAlliance(builder);
  }

  public static void StartAlliance(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionAlliance>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Alliance> EndAlliance(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Alliance>(o);
  }
  public static void FinishAllianceBuffer(FlatBufferBuilder builder, Offset<Alliance> offset) { builder.Finish(offset.Value); }
};

public sealed class AlliancesFBBoss : Table {
  public static AlliancesFBBoss GetRootAsAlliancesFBBoss(ByteBuffer _bb) { return GetRootAsAlliancesFBBoss(_bb, new AlliancesFBBoss()); }
  public static AlliancesFBBoss GetRootAsAlliancesFBBoss(ByteBuffer _bb, AlliancesFBBoss obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AlliancesFBBoss __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MonsterId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Donate { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Challenge { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Reward { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBytes() { return __vector_as_arraysegment(12); }

  public static Offset<AlliancesFBBoss> CreateAlliancesFBBoss(FlatBufferBuilder builder,
      int id = 0,
      int monster_id = 0,
      int donate = 0,
      int challenge = 0,
      StringOffset rewardOffset = default(StringOffset)) {
    builder.StartObject(5);
    AlliancesFBBoss.AddReward(builder, rewardOffset);
    AlliancesFBBoss.AddChallenge(builder, challenge);
    AlliancesFBBoss.AddDonate(builder, donate);
    AlliancesFBBoss.AddMonsterId(builder, monster_id);
    AlliancesFBBoss.AddId(builder, id);
    return AlliancesFBBoss.EndAlliancesFBBoss(builder);
  }

  public static void StartAlliancesFBBoss(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddMonsterId(FlatBufferBuilder builder, int monsterId) { builder.AddInt(1, monsterId, 0); }
  public static void AddDonate(FlatBufferBuilder builder, int donate) { builder.AddInt(2, donate, 0); }
  public static void AddChallenge(FlatBufferBuilder builder, int challenge) { builder.AddInt(3, challenge, 0); }
  public static void AddReward(FlatBufferBuilder builder, StringOffset rewardOffset) { builder.AddOffset(4, rewardOffset.Value, 0); }
  public static Offset<AlliancesFBBoss> EndAlliancesFBBoss(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AlliancesFBBoss>(o);
  }
};

public sealed class AlliancesFBHurt : Table {
  public static AlliancesFBHurt GetRootAsAlliancesFBHurt(ByteBuffer _bb) { return GetRootAsAlliancesFBHurt(_bb, new AlliancesFBHurt()); }
  public static AlliancesFBHurt GetRootAsAlliancesFBHurt(ByteBuffer _bb, AlliancesFBHurt obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AlliancesFBHurt __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BossId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Hurt { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Reward { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBytes() { return __vector_as_arraysegment(10); }

  public static Offset<AlliancesFBHurt> CreateAlliancesFBHurt(FlatBufferBuilder builder,
      int id = 0,
      int boss_id = 0,
      int hurt = 0,
      StringOffset rewardOffset = default(StringOffset)) {
    builder.StartObject(4);
    AlliancesFBHurt.AddReward(builder, rewardOffset);
    AlliancesFBHurt.AddHurt(builder, hurt);
    AlliancesFBHurt.AddBossId(builder, boss_id);
    AlliancesFBHurt.AddId(builder, id);
    return AlliancesFBHurt.EndAlliancesFBHurt(builder);
  }

  public static void StartAlliancesFBHurt(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddBossId(FlatBufferBuilder builder, int bossId) { builder.AddInt(1, bossId, 0); }
  public static void AddHurt(FlatBufferBuilder builder, int hurt) { builder.AddInt(2, hurt, 0); }
  public static void AddReward(FlatBufferBuilder builder, StringOffset rewardOffset) { builder.AddOffset(3, rewardOffset.Value, 0); }
  public static Offset<AlliancesFBHurt> EndAlliancesFBHurt(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AlliancesFBHurt>(o);
  }
};

public sealed class DonateChest : Table {
  public static DonateChest GetRootAsDonateChest(ByteBuffer _bb) { return GetRootAsDonateChest(_bb, new DonateChest()); }
  public static DonateChest GetRootAsDonateChest(ByteBuffer _bb, DonateChest obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DonateChest __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Score { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Reward { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBytes() { return __vector_as_arraysegment(8); }

  public static Offset<DonateChest> CreateDonateChest(FlatBufferBuilder builder,
      int id = 0,
      int score = 0,
      StringOffset rewardOffset = default(StringOffset)) {
    builder.StartObject(3);
    DonateChest.AddReward(builder, rewardOffset);
    DonateChest.AddScore(builder, score);
    DonateChest.AddId(builder, id);
    return DonateChest.EndDonateChest(builder);
  }

  public static void StartDonateChest(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddScore(FlatBufferBuilder builder, int score) { builder.AddInt(1, score, 0); }
  public static void AddReward(FlatBufferBuilder builder, StringOffset rewardOffset) { builder.AddOffset(2, rewardOffset.Value, 0); }
  public static Offset<DonateChest> EndDonateChest(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DonateChest>(o);
  }
};

public sealed class TechSkill : Table {
  public static TechSkill GetRootAsTechSkill(ByteBuffer _bb) { return GetRootAsTechSkill(_bb, new TechSkill()); }
  public static TechSkill GetRootAsTechSkill(ByteBuffer _bb, TechSkill obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public TechSkill __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int SkillId { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CharType { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LevelLimit { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AdditionType { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string SkillIcon { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSkillIconBytes() { return __vector_as_arraysegment(12); }
  public string SkillName { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSkillNameBytes() { return __vector_as_arraysegment(14); }
  public string SkillDes { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSkillDesBytes() { return __vector_as_arraysegment(16); }

  public static Offset<TechSkill> CreateTechSkill(FlatBufferBuilder builder,
      int skill_id = 0,
      int char_type = 0,
      int level_limit = 0,
      int addition_type = 0,
      StringOffset skill_iconOffset = default(StringOffset),
      StringOffset skill_nameOffset = default(StringOffset),
      StringOffset skill_desOffset = default(StringOffset)) {
    builder.StartObject(7);
    TechSkill.AddSkillDes(builder, skill_desOffset);
    TechSkill.AddSkillName(builder, skill_nameOffset);
    TechSkill.AddSkillIcon(builder, skill_iconOffset);
    TechSkill.AddAdditionType(builder, addition_type);
    TechSkill.AddLevelLimit(builder, level_limit);
    TechSkill.AddCharType(builder, char_type);
    TechSkill.AddSkillId(builder, skill_id);
    return TechSkill.EndTechSkill(builder);
  }

  public static void StartTechSkill(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddSkillId(FlatBufferBuilder builder, int skillId) { builder.AddInt(0, skillId, 0); }
  public static void AddCharType(FlatBufferBuilder builder, int charType) { builder.AddInt(1, charType, 0); }
  public static void AddLevelLimit(FlatBufferBuilder builder, int levelLimit) { builder.AddInt(2, levelLimit, 0); }
  public static void AddAdditionType(FlatBufferBuilder builder, int additionType) { builder.AddInt(3, additionType, 0); }
  public static void AddSkillIcon(FlatBufferBuilder builder, StringOffset skillIconOffset) { builder.AddOffset(4, skillIconOffset.Value, 0); }
  public static void AddSkillName(FlatBufferBuilder builder, StringOffset skillNameOffset) { builder.AddOffset(5, skillNameOffset.Value, 0); }
  public static void AddSkillDes(FlatBufferBuilder builder, StringOffset skillDesOffset) { builder.AddOffset(6, skillDesOffset.Value, 0); }
  public static Offset<TechSkill> EndTechSkill(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<TechSkill>(o);
  }
};

public sealed class TechLevelChest : Table {
  public static TechLevelChest GetRootAsTechLevelChest(ByteBuffer _bb) { return GetRootAsTechLevelChest(_bb, new TechLevelChest()); }
  public static TechLevelChest GetRootAsTechLevelChest(ByteBuffer _bb, TechLevelChest obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public TechLevelChest __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Level { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GoldRate { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GoldMax { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ExpRate { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ExpMax { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int VigorRate { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int VigorMax { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<TechLevelChest> CreateTechLevelChest(FlatBufferBuilder builder,
      int level = 0,
      int gold_rate = 0,
      int gold_max = 0,
      int exp_rate = 0,
      int exp_max = 0,
      int vigor_rate = 0,
      int vigor_max = 0) {
    builder.StartObject(7);
    TechLevelChest.AddVigorMax(builder, vigor_max);
    TechLevelChest.AddVigorRate(builder, vigor_rate);
    TechLevelChest.AddExpMax(builder, exp_max);
    TechLevelChest.AddExpRate(builder, exp_rate);
    TechLevelChest.AddGoldMax(builder, gold_max);
    TechLevelChest.AddGoldRate(builder, gold_rate);
    TechLevelChest.AddLevel(builder, level);
    return TechLevelChest.EndTechLevelChest(builder);
  }

  public static void StartTechLevelChest(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(0, level, 0); }
  public static void AddGoldRate(FlatBufferBuilder builder, int goldRate) { builder.AddInt(1, goldRate, 0); }
  public static void AddGoldMax(FlatBufferBuilder builder, int goldMax) { builder.AddInt(2, goldMax, 0); }
  public static void AddExpRate(FlatBufferBuilder builder, int expRate) { builder.AddInt(3, expRate, 0); }
  public static void AddExpMax(FlatBufferBuilder builder, int expMax) { builder.AddInt(4, expMax, 0); }
  public static void AddVigorRate(FlatBufferBuilder builder, int vigorRate) { builder.AddInt(5, vigorRate, 0); }
  public static void AddVigorMax(FlatBufferBuilder builder, int vigorMax) { builder.AddInt(6, vigorMax, 0); }
  public static Offset<TechLevelChest> EndTechLevelChest(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<TechLevelChest>(o);
  }
};

public sealed class TechSkillLevelUp : Table {
  public static TechSkillLevelUp GetRootAsTechSkillLevelUp(ByteBuffer _bb) { return GetRootAsTechSkillLevelUp(_bb, new TechSkillLevelUp()); }
  public static TechSkillLevelUp GetRootAsTechSkillLevelUp(ByteBuffer _bb, TechSkillLevelUp obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public TechSkillLevelUp __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int SkillId { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Level { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Addition { get { int o = __offset(8); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int Cost { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<TechSkillLevelUp> CreateTechSkillLevelUp(FlatBufferBuilder builder,
      int skill_id = 0,
      int level = 0,
      float addition = 0,
      int cost = 0) {
    builder.StartObject(4);
    TechSkillLevelUp.AddCost(builder, cost);
    TechSkillLevelUp.AddAddition(builder, addition);
    TechSkillLevelUp.AddLevel(builder, level);
    TechSkillLevelUp.AddSkillId(builder, skill_id);
    return TechSkillLevelUp.EndTechSkillLevelUp(builder);
  }

  public static void StartTechSkillLevelUp(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddSkillId(FlatBufferBuilder builder, int skillId) { builder.AddInt(0, skillId, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(1, level, 0); }
  public static void AddAddition(FlatBufferBuilder builder, float addition) { builder.AddFloat(2, addition, 0); }
  public static void AddCost(FlatBufferBuilder builder, int cost) { builder.AddInt(3, cost, 0); }
  public static Offset<TechSkillLevelUp> EndTechSkillLevelUp(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<TechSkillLevelUp>(o);
  }
};


}
