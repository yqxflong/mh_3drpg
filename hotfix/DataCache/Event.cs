// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class SleepTower : Table {
  public static SleepTower GetRootAsSleepTower(ByteBuffer _bb) { return GetRootAsSleepTower(_bb, new SleepTower()); }
  public static SleepTower GetRootAsSleepTower(ByteBuffer _bb, SleepTower obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SleepTower __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Level { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Floor { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int NormalRecord { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int DifficultRecord { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Param { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int DifficultSleep { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<SleepTower> CreateSleepTower(FlatBufferBuilder builder,
      int level = 0,
      int floor = 0,
      int normal_record = 0,
      int difficult_record = 0,
      float param = 0,
      int difficult_sleep = 0) {
    builder.StartObject(6);
    SleepTower.AddDifficultSleep(builder, difficult_sleep);
    SleepTower.AddParam(builder, param);
    SleepTower.AddDifficultRecord(builder, difficult_record);
    SleepTower.AddNormalRecord(builder, normal_record);
    SleepTower.AddFloor(builder, floor);
    SleepTower.AddLevel(builder, level);
    return SleepTower.EndSleepTower(builder);
  }

  public static void StartSleepTower(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(0, level, 0); }
  public static void AddFloor(FlatBufferBuilder builder, int floor) { builder.AddInt(1, floor, 0); }
  public static void AddNormalRecord(FlatBufferBuilder builder, int normalRecord) { builder.AddInt(2, normalRecord, 0); }
  public static void AddDifficultRecord(FlatBufferBuilder builder, int difficultRecord) { builder.AddInt(3, difficultRecord, 0); }
  public static void AddParam(FlatBufferBuilder builder, float param) { builder.AddFloat(4, param, 0); }
  public static void AddDifficultSleep(FlatBufferBuilder builder, int difficultSleep) { builder.AddInt(5, difficultSleep, 0); }
  public static Offset<SleepTower> EndSleepTower(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SleepTower>(o);
  }
};

public sealed class SleepTowerReward : Table {
  public static SleepTowerReward GetRootAsSleepTowerReward(ByteBuffer _bb) { return GetRootAsSleepTowerReward(_bb, new SleepTowerReward()); }
  public static SleepTowerReward GetRootAsSleepTowerReward(ByteBuffer _bb, SleepTowerReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SleepTowerReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Record { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Reward { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBytes() { return __vector_as_arraysegment(8); }

  public static Offset<SleepTowerReward> CreateSleepTowerReward(FlatBufferBuilder builder,
      int id = 0,
      int record = 0,
      StringOffset rewardOffset = default(StringOffset)) {
    builder.StartObject(3);
    SleepTowerReward.AddReward(builder, rewardOffset);
    SleepTowerReward.AddRecord(builder, record);
    SleepTowerReward.AddId(builder, id);
    return SleepTowerReward.EndSleepTowerReward(builder);
  }

  public static void StartSleepTowerReward(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddRecord(FlatBufferBuilder builder, int record) { builder.AddInt(1, record, 0); }
  public static void AddReward(FlatBufferBuilder builder, StringOffset rewardOffset) { builder.AddOffset(2, rewardOffset.Value, 0); }
  public static Offset<SleepTowerReward> EndSleepTowerReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SleepTowerReward>(o);
  }
};

public sealed class RobotInfo : Table {
  public static RobotInfo GetRootAsRobotInfo(ByteBuffer _bb) { return GetRootAsRobotInfo(_bb, new RobotInfo()); }
  public static RobotInfo GetRootAsRobotInfo(ByteBuffer _bb, RobotInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public RobotInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(4); }
  public int Wave { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Layout { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLayoutBytes() { return __vector_as_arraysegment(8); }
  public string F1 { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetF1Bytes() { return __vector_as_arraysegment(10); }
  public string F2 { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetF2Bytes() { return __vector_as_arraysegment(12); }
  public string F3 { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetF3Bytes() { return __vector_as_arraysegment(14); }
  public string M1 { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetM1Bytes() { return __vector_as_arraysegment(16); }
  public string M2 { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetM2Bytes() { return __vector_as_arraysegment(18); }
  public string M3 { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetM3Bytes() { return __vector_as_arraysegment(20); }
  public string B1 { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetB1Bytes() { return __vector_as_arraysegment(22); }
  public string B2 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetB2Bytes() { return __vector_as_arraysegment(24); }
  public string B3 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetB3Bytes() { return __vector_as_arraysegment(26); }
  public string CharacterId { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCharacterIdBytes() { return __vector_as_arraysegment(28); }
  public string Icon { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(30); }
  public int Level { get { int o = __offset(32); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<RobotInfo> CreateRobotInfo(FlatBufferBuilder builder,
      StringOffset nameOffset = default(StringOffset),
      int wave = 0,
      StringOffset layoutOffset = default(StringOffset),
      StringOffset f1Offset = default(StringOffset),
      StringOffset f2Offset = default(StringOffset),
      StringOffset f3Offset = default(StringOffset),
      StringOffset m1Offset = default(StringOffset),
      StringOffset m2Offset = default(StringOffset),
      StringOffset m3Offset = default(StringOffset),
      StringOffset b1Offset = default(StringOffset),
      StringOffset b2Offset = default(StringOffset),
      StringOffset b3Offset = default(StringOffset),
      StringOffset character_idOffset = default(StringOffset),
      StringOffset iconOffset = default(StringOffset),
      int level = 0) {
    builder.StartObject(15);
    RobotInfo.AddLevel(builder, level);
    RobotInfo.AddIcon(builder, iconOffset);
    RobotInfo.AddCharacterId(builder, character_idOffset);
    RobotInfo.AddB3(builder, b3Offset);
    RobotInfo.AddB2(builder, b2Offset);
    RobotInfo.AddB1(builder, b1Offset);
    RobotInfo.AddM3(builder, m3Offset);
    RobotInfo.AddM2(builder, m2Offset);
    RobotInfo.AddM1(builder, m1Offset);
    RobotInfo.AddF3(builder, f3Offset);
    RobotInfo.AddF2(builder, f2Offset);
    RobotInfo.AddF1(builder, f1Offset);
    RobotInfo.AddLayout(builder, layoutOffset);
    RobotInfo.AddWave(builder, wave);
    RobotInfo.AddName(builder, nameOffset);
    return RobotInfo.EndRobotInfo(builder);
  }

  public static void StartRobotInfo(FlatBufferBuilder builder) { builder.StartObject(15); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddWave(FlatBufferBuilder builder, int wave) { builder.AddInt(1, wave, 0); }
  public static void AddLayout(FlatBufferBuilder builder, StringOffset layoutOffset) { builder.AddOffset(2, layoutOffset.Value, 0); }
  public static void AddF1(FlatBufferBuilder builder, StringOffset f1Offset) { builder.AddOffset(3, f1Offset.Value, 0); }
  public static void AddF2(FlatBufferBuilder builder, StringOffset f2Offset) { builder.AddOffset(4, f2Offset.Value, 0); }
  public static void AddF3(FlatBufferBuilder builder, StringOffset f3Offset) { builder.AddOffset(5, f3Offset.Value, 0); }
  public static void AddM1(FlatBufferBuilder builder, StringOffset m1Offset) { builder.AddOffset(6, m1Offset.Value, 0); }
  public static void AddM2(FlatBufferBuilder builder, StringOffset m2Offset) { builder.AddOffset(7, m2Offset.Value, 0); }
  public static void AddM3(FlatBufferBuilder builder, StringOffset m3Offset) { builder.AddOffset(8, m3Offset.Value, 0); }
  public static void AddB1(FlatBufferBuilder builder, StringOffset b1Offset) { builder.AddOffset(9, b1Offset.Value, 0); }
  public static void AddB2(FlatBufferBuilder builder, StringOffset b2Offset) { builder.AddOffset(10, b2Offset.Value, 0); }
  public static void AddB3(FlatBufferBuilder builder, StringOffset b3Offset) { builder.AddOffset(11, b3Offset.Value, 0); }
  public static void AddCharacterId(FlatBufferBuilder builder, StringOffset characterIdOffset) { builder.AddOffset(12, characterIdOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(13, iconOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(14, level, 0); }
  public static Offset<RobotInfo> EndRobotInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<RobotInfo>(o);
  }
};

public sealed class ExpeditionMapConfig : Table {
  public static ExpeditionMapConfig GetRootAsExpeditionMapConfig(ByteBuffer _bb) { return GetRootAsExpeditionMapConfig(_bb, new ExpeditionMapConfig()); }
  public static ExpeditionMapConfig GetRootAsExpeditionMapConfig(ByteBuffer _bb, ExpeditionMapConfig obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ExpeditionMapConfig __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Stage { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ExpeditionGold { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BoxIcon { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBoxIconBytes() { return __vector_as_arraysegment(8); }
  public string BoxDesc { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBoxDescBytes() { return __vector_as_arraysegment(10); }
  public string RewardBox { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBoxBytes() { return __vector_as_arraysegment(12); }

  public static Offset<ExpeditionMapConfig> CreateExpeditionMapConfig(FlatBufferBuilder builder,
      int stage = 0,
      int expedition_gold = 0,
      StringOffset box_iconOffset = default(StringOffset),
      StringOffset box_descOffset = default(StringOffset),
      StringOffset reward_boxOffset = default(StringOffset)) {
    builder.StartObject(5);
    ExpeditionMapConfig.AddRewardBox(builder, reward_boxOffset);
    ExpeditionMapConfig.AddBoxDesc(builder, box_descOffset);
    ExpeditionMapConfig.AddBoxIcon(builder, box_iconOffset);
    ExpeditionMapConfig.AddExpeditionGold(builder, expedition_gold);
    ExpeditionMapConfig.AddStage(builder, stage);
    return ExpeditionMapConfig.EndExpeditionMapConfig(builder);
  }

  public static void StartExpeditionMapConfig(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddStage(FlatBufferBuilder builder, int stage) { builder.AddInt(0, stage, 0); }
  public static void AddExpeditionGold(FlatBufferBuilder builder, int expeditionGold) { builder.AddInt(1, expeditionGold, 0); }
  public static void AddBoxIcon(FlatBufferBuilder builder, StringOffset boxIconOffset) { builder.AddOffset(2, boxIconOffset.Value, 0); }
  public static void AddBoxDesc(FlatBufferBuilder builder, StringOffset boxDescOffset) { builder.AddOffset(3, boxDescOffset.Value, 0); }
  public static void AddRewardBox(FlatBufferBuilder builder, StringOffset rewardBoxOffset) { builder.AddOffset(4, rewardBoxOffset.Value, 0); }
  public static Offset<ExpeditionMapConfig> EndExpeditionMapConfig(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ExpeditionMapConfig>(o);
  }
};

public sealed class ArenaAward : Table {
  public static ArenaAward GetRootAsArenaAward(ByteBuffer _bb) { return GetRootAsArenaAward(_bb, new ArenaAward()); }
  public static ArenaAward GetRootAsArenaAward(ByteBuffer _bb, ArenaAward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ArenaAward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int RankTop { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RankDown { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Hc { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Gold { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ArenaGold { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int FirstAchieveHc { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int FirstAchieveGold { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RankProgressHc { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<ArenaAward> CreateArenaAward(FlatBufferBuilder builder,
      int rank_top = 0,
      int rank_down = 0,
      int hc = 0,
      int gold = 0,
      int arena_gold = 0,
      int first_achieve_hc = 0,
      int first_achieve_gold = 0,
      int rank_progress_hc = 0) {
    builder.StartObject(8);
    ArenaAward.AddRankProgressHc(builder, rank_progress_hc);
    ArenaAward.AddFirstAchieveGold(builder, first_achieve_gold);
    ArenaAward.AddFirstAchieveHc(builder, first_achieve_hc);
    ArenaAward.AddArenaGold(builder, arena_gold);
    ArenaAward.AddGold(builder, gold);
    ArenaAward.AddHc(builder, hc);
    ArenaAward.AddRankDown(builder, rank_down);
    ArenaAward.AddRankTop(builder, rank_top);
    return ArenaAward.EndArenaAward(builder);
  }

  public static void StartArenaAward(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddRankTop(FlatBufferBuilder builder, int rankTop) { builder.AddInt(0, rankTop, 0); }
  public static void AddRankDown(FlatBufferBuilder builder, int rankDown) { builder.AddInt(1, rankDown, 0); }
  public static void AddHc(FlatBufferBuilder builder, int hc) { builder.AddInt(2, hc, 0); }
  public static void AddGold(FlatBufferBuilder builder, int gold) { builder.AddInt(3, gold, 0); }
  public static void AddArenaGold(FlatBufferBuilder builder, int arenaGold) { builder.AddInt(4, arenaGold, 0); }
  public static void AddFirstAchieveHc(FlatBufferBuilder builder, int firstAchieveHc) { builder.AddInt(5, firstAchieveHc, 0); }
  public static void AddFirstAchieveGold(FlatBufferBuilder builder, int firstAchieveGold) { builder.AddInt(6, firstAchieveGold, 0); }
  public static void AddRankProgressHc(FlatBufferBuilder builder, int rankProgressHc) { builder.AddInt(7, rankProgressHc, 0); }
  public static Offset<ArenaAward> EndArenaAward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ArenaAward>(o);
  }
};

public sealed class SignInReward : Table {
  public static SignInReward GetRootAsSignInReward(ByteBuffer _bb) { return GetRootAsSignInReward(_bb, new SignInReward()); }
  public static SignInReward GetRootAsSignInReward(ByteBuffer _bb, SignInReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SignInReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Month { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SignCount { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(8); }
  public string ItemId { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(10); }
  public int ItemNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<SignInReward> CreateSignInReward(FlatBufferBuilder builder,
      int month = 0,
      int sign_count = 0,
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      int item_num = 0) {
    builder.StartObject(5);
    SignInReward.AddItemNum(builder, item_num);
    SignInReward.AddItemId(builder, item_idOffset);
    SignInReward.AddItemType(builder, item_typeOffset);
    SignInReward.AddSignCount(builder, sign_count);
    SignInReward.AddMonth(builder, month);
    return SignInReward.EndSignInReward(builder);
  }

  public static void StartSignInReward(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddMonth(FlatBufferBuilder builder, int month) { builder.AddInt(0, month, 0); }
  public static void AddSignCount(FlatBufferBuilder builder, int signCount) { builder.AddInt(1, signCount, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(2, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(3, itemIdOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(4, itemNum, 0); }
  public static Offset<SignInReward> EndSignInReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SignInReward>(o);
  }
};

public sealed class DailyReward : Table {
  public static DailyReward GetRootAsDailyReward(ByteBuffer _bb) { return GetRootAsDailyReward(_bb, new DailyReward()); }
  public static DailyReward GetRootAsDailyReward(ByteBuffer _bb, DailyReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DailyReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Type { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTypeBytes() { return __vector_as_arraysegment(6); }
  public string Title { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTitleBytes() { return __vector_as_arraysegment(8); }
  public string Desc { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(10); }
  public string ItemType { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(12); }
  public string ItemId { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(14); }
  public int ItemNum { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType2 { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType2Bytes() { return __vector_as_arraysegment(18); }
  public string ItemId2 { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId2Bytes() { return __vector_as_arraysegment(20); }
  public int ItemNum2 { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType3 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType3Bytes() { return __vector_as_arraysegment(24); }
  public string ItemId3 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId3Bytes() { return __vector_as_arraysegment(26); }
  public int ItemNum3 { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType4 { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType4Bytes() { return __vector_as_arraysegment(30); }
  public string ItemId4 { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId4Bytes() { return __vector_as_arraysegment(32); }
  public int ItemNum4 { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<DailyReward> CreateDailyReward(FlatBufferBuilder builder,
      int id = 0,
      StringOffset typeOffset = default(StringOffset),
      StringOffset titleOffset = default(StringOffset),
      StringOffset descOffset = default(StringOffset),
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      int item_num = 0,
      StringOffset item_type2Offset = default(StringOffset),
      StringOffset item_id2Offset = default(StringOffset),
      int item_num2 = 0,
      StringOffset item_type3Offset = default(StringOffset),
      StringOffset item_id3Offset = default(StringOffset),
      int item_num3 = 0,
      StringOffset item_type4Offset = default(StringOffset),
      StringOffset item_id4Offset = default(StringOffset),
      int item_num4 = 0) {
    builder.StartObject(16);
    DailyReward.AddItemNum4(builder, item_num4);
    DailyReward.AddItemId4(builder, item_id4Offset);
    DailyReward.AddItemType4(builder, item_type4Offset);
    DailyReward.AddItemNum3(builder, item_num3);
    DailyReward.AddItemId3(builder, item_id3Offset);
    DailyReward.AddItemType3(builder, item_type3Offset);
    DailyReward.AddItemNum2(builder, item_num2);
    DailyReward.AddItemId2(builder, item_id2Offset);
    DailyReward.AddItemType2(builder, item_type2Offset);
    DailyReward.AddItemNum(builder, item_num);
    DailyReward.AddItemId(builder, item_idOffset);
    DailyReward.AddItemType(builder, item_typeOffset);
    DailyReward.AddDesc(builder, descOffset);
    DailyReward.AddTitle(builder, titleOffset);
    DailyReward.AddType(builder, typeOffset);
    DailyReward.AddId(builder, id);
    return DailyReward.EndDailyReward(builder);
  }

  public static void StartDailyReward(FlatBufferBuilder builder) { builder.StartObject(16); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddType(FlatBufferBuilder builder, StringOffset typeOffset) { builder.AddOffset(1, typeOffset.Value, 0); }
  public static void AddTitle(FlatBufferBuilder builder, StringOffset titleOffset) { builder.AddOffset(2, titleOffset.Value, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(3, descOffset.Value, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(4, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(5, itemIdOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(6, itemNum, 0); }
  public static void AddItemType2(FlatBufferBuilder builder, StringOffset itemType2Offset) { builder.AddOffset(7, itemType2Offset.Value, 0); }
  public static void AddItemId2(FlatBufferBuilder builder, StringOffset itemId2Offset) { builder.AddOffset(8, itemId2Offset.Value, 0); }
  public static void AddItemNum2(FlatBufferBuilder builder, int itemNum2) { builder.AddInt(9, itemNum2, 0); }
  public static void AddItemType3(FlatBufferBuilder builder, StringOffset itemType3Offset) { builder.AddOffset(10, itemType3Offset.Value, 0); }
  public static void AddItemId3(FlatBufferBuilder builder, StringOffset itemId3Offset) { builder.AddOffset(11, itemId3Offset.Value, 0); }
  public static void AddItemNum3(FlatBufferBuilder builder, int itemNum3) { builder.AddInt(12, itemNum3, 0); }
  public static void AddItemType4(FlatBufferBuilder builder, StringOffset itemType4Offset) { builder.AddOffset(13, itemType4Offset.Value, 0); }
  public static void AddItemId4(FlatBufferBuilder builder, StringOffset itemId4Offset) { builder.AddOffset(14, itemId4Offset.Value, 0); }
  public static void AddItemNum4(FlatBufferBuilder builder, int itemNum4) { builder.AddInt(15, itemNum4, 0); }
  public static Offset<DailyReward> EndDailyReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DailyReward>(o);
  }
};

public sealed class SpecialActivity : Table {
  public static SpecialActivity GetRootAsSpecialActivity(ByteBuffer _bb) { return GetRootAsSpecialActivity(_bb, new SpecialActivity()); }
  public static SpecialActivity GetRootAsSpecialActivity(ByteBuffer _bb, SpecialActivity obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SpecialActivity __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Type { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BattleType { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string DisplayName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDisplayNameBytes() { return __vector_as_arraysegment(10); }
  public string OpenTime { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetOpenTimeBytes() { return __vector_as_arraysegment(12); }
  public int Times { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Desc { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(16); }
  public string StartEndTime { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetStartEndTimeBytes() { return __vector_as_arraysegment(18); }
  public int Level { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool NeedAlliance { get { int o = __offset(22); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public string Icon { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(24); }
  public int NavType { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string NavParameter { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNavParameterBytes() { return __vector_as_arraysegment(28); }
  public string Title { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTitleBytes() { return __vector_as_arraysegment(30); }
  public string Content { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetContentBytes() { return __vector_as_arraysegment(32); }
  public string OpenTip { get { int o = __offset(34); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetOpenTipBytes() { return __vector_as_arraysegment(34); }
  public int Liveness { get { int o = __offset(36); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Award { get { int o = __offset(38); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAwardBytes() { return __vector_as_arraysegment(38); }
  public bool IsShowDaily { get { int o = __offset(40); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int ShowType { get { int o = __offset(42); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Order { get { int o = __offset(44); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<SpecialActivity> CreateSpecialActivity(FlatBufferBuilder builder,
      int id = 0,
      int type = 0,
      int battle_type = 0,
      StringOffset display_nameOffset = default(StringOffset),
      StringOffset open_timeOffset = default(StringOffset),
      int times = 0,
      StringOffset descOffset = default(StringOffset),
      StringOffset start_end_timeOffset = default(StringOffset),
      int level = 0,
      bool need_alliance = false,
      StringOffset iconOffset = default(StringOffset),
      int nav_type = 0,
      StringOffset nav_parameterOffset = default(StringOffset),
      StringOffset titleOffset = default(StringOffset),
      StringOffset contentOffset = default(StringOffset),
      StringOffset open_tipOffset = default(StringOffset),
      int liveness = 0,
      StringOffset awardOffset = default(StringOffset),
      bool is_show_daily = false,
      int show_type = 0,
      int order = 0) {
    builder.StartObject(21);
    SpecialActivity.AddOrder(builder, order);
    SpecialActivity.AddShowType(builder, show_type);
    SpecialActivity.AddAward(builder, awardOffset);
    SpecialActivity.AddLiveness(builder, liveness);
    SpecialActivity.AddOpenTip(builder, open_tipOffset);
    SpecialActivity.AddContent(builder, contentOffset);
    SpecialActivity.AddTitle(builder, titleOffset);
    SpecialActivity.AddNavParameter(builder, nav_parameterOffset);
    SpecialActivity.AddNavType(builder, nav_type);
    SpecialActivity.AddIcon(builder, iconOffset);
    SpecialActivity.AddLevel(builder, level);
    SpecialActivity.AddStartEndTime(builder, start_end_timeOffset);
    SpecialActivity.AddDesc(builder, descOffset);
    SpecialActivity.AddTimes(builder, times);
    SpecialActivity.AddOpenTime(builder, open_timeOffset);
    SpecialActivity.AddDisplayName(builder, display_nameOffset);
    SpecialActivity.AddBattleType(builder, battle_type);
    SpecialActivity.AddType(builder, type);
    SpecialActivity.AddId(builder, id);
    SpecialActivity.AddIsShowDaily(builder, is_show_daily);
    SpecialActivity.AddNeedAlliance(builder, need_alliance);
    return SpecialActivity.EndSpecialActivity(builder);
  }

  public static void StartSpecialActivity(FlatBufferBuilder builder) { builder.StartObject(21); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(1, type, 0); }
  public static void AddBattleType(FlatBufferBuilder builder, int battleType) { builder.AddInt(2, battleType, 0); }
  public static void AddDisplayName(FlatBufferBuilder builder, StringOffset displayNameOffset) { builder.AddOffset(3, displayNameOffset.Value, 0); }
  public static void AddOpenTime(FlatBufferBuilder builder, StringOffset openTimeOffset) { builder.AddOffset(4, openTimeOffset.Value, 0); }
  public static void AddTimes(FlatBufferBuilder builder, int times) { builder.AddInt(5, times, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(6, descOffset.Value, 0); }
  public static void AddStartEndTime(FlatBufferBuilder builder, StringOffset startEndTimeOffset) { builder.AddOffset(7, startEndTimeOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(8, level, 0); }
  public static void AddNeedAlliance(FlatBufferBuilder builder, bool needAlliance) { builder.AddBool(9, needAlliance, false); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(10, iconOffset.Value, 0); }
  public static void AddNavType(FlatBufferBuilder builder, int navType) { builder.AddInt(11, navType, 0); }
  public static void AddNavParameter(FlatBufferBuilder builder, StringOffset navParameterOffset) { builder.AddOffset(12, navParameterOffset.Value, 0); }
  public static void AddTitle(FlatBufferBuilder builder, StringOffset titleOffset) { builder.AddOffset(13, titleOffset.Value, 0); }
  public static void AddContent(FlatBufferBuilder builder, StringOffset contentOffset) { builder.AddOffset(14, contentOffset.Value, 0); }
  public static void AddOpenTip(FlatBufferBuilder builder, StringOffset openTipOffset) { builder.AddOffset(15, openTipOffset.Value, 0); }
  public static void AddLiveness(FlatBufferBuilder builder, int liveness) { builder.AddInt(16, liveness, 0); }
  public static void AddAward(FlatBufferBuilder builder, StringOffset awardOffset) { builder.AddOffset(17, awardOffset.Value, 0); }
  public static void AddIsShowDaily(FlatBufferBuilder builder, bool isShowDaily) { builder.AddBool(18, isShowDaily, false); }
  public static void AddShowType(FlatBufferBuilder builder, int showType) { builder.AddInt(19, showType, 0); }
  public static void AddOrder(FlatBufferBuilder builder, int order) { builder.AddInt(20, order, 0); }
  public static Offset<SpecialActivity> EndSpecialActivity(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SpecialActivity>(o);
  }
};

public sealed class SpecialActivityLevel : Table {
  public static SpecialActivityLevel GetRootAsSpecialActivityLevel(ByteBuffer _bb) { return GetRootAsSpecialActivityLevel(_bb, new SpecialActivityLevel()); }
  public static SpecialActivityLevel GetRootAsSpecialActivityLevel(ByteBuffer _bb, SpecialActivityLevel obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SpecialActivityLevel __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ActivityId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Icon { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(8); }
  public string Map { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMapBytes() { return __vector_as_arraysegment(10); }
  public string RecommendPartner { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRecommendPartnerBytes() { return __vector_as_arraysegment(12); }
  public string Layout { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLayoutBytes() { return __vector_as_arraysegment(14); }
  public string Parameter { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetParameterBytes() { return __vector_as_arraysegment(16); }
  public int Level { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Difficulty { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Gold { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string RewardDesc { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardDescBytes() { return __vector_as_arraysegment(24); }
  public string Name { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(26); }

  public static Offset<SpecialActivityLevel> CreateSpecialActivityLevel(FlatBufferBuilder builder,
      int id = 0,
      int activity_id = 0,
      StringOffset iconOffset = default(StringOffset),
      StringOffset mapOffset = default(StringOffset),
      StringOffset recommend_partnerOffset = default(StringOffset),
      StringOffset layoutOffset = default(StringOffset),
      StringOffset parameterOffset = default(StringOffset),
      int level = 0,
      int difficulty = 0,
      int gold = 0,
      StringOffset reward_descOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset)) {
    builder.StartObject(12);
    SpecialActivityLevel.AddName(builder, nameOffset);
    SpecialActivityLevel.AddRewardDesc(builder, reward_descOffset);
    SpecialActivityLevel.AddGold(builder, gold);
    SpecialActivityLevel.AddDifficulty(builder, difficulty);
    SpecialActivityLevel.AddLevel(builder, level);
    SpecialActivityLevel.AddParameter(builder, parameterOffset);
    SpecialActivityLevel.AddLayout(builder, layoutOffset);
    SpecialActivityLevel.AddRecommendPartner(builder, recommend_partnerOffset);
    SpecialActivityLevel.AddMap(builder, mapOffset);
    SpecialActivityLevel.AddIcon(builder, iconOffset);
    SpecialActivityLevel.AddActivityId(builder, activity_id);
    SpecialActivityLevel.AddId(builder, id);
    return SpecialActivityLevel.EndSpecialActivityLevel(builder);
  }

  public static void StartSpecialActivityLevel(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddActivityId(FlatBufferBuilder builder, int activityId) { builder.AddInt(1, activityId, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(2, iconOffset.Value, 0); }
  public static void AddMap(FlatBufferBuilder builder, StringOffset mapOffset) { builder.AddOffset(3, mapOffset.Value, 0); }
  public static void AddRecommendPartner(FlatBufferBuilder builder, StringOffset recommendPartnerOffset) { builder.AddOffset(4, recommendPartnerOffset.Value, 0); }
  public static void AddLayout(FlatBufferBuilder builder, StringOffset layoutOffset) { builder.AddOffset(5, layoutOffset.Value, 0); }
  public static void AddParameter(FlatBufferBuilder builder, StringOffset parameterOffset) { builder.AddOffset(6, parameterOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(7, level, 0); }
  public static void AddDifficulty(FlatBufferBuilder builder, int difficulty) { builder.AddInt(8, difficulty, 0); }
  public static void AddGold(FlatBufferBuilder builder, int gold) { builder.AddInt(9, gold, 0); }
  public static void AddRewardDesc(FlatBufferBuilder builder, StringOffset rewardDescOffset) { builder.AddOffset(10, rewardDescOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(11, nameOffset.Value, 0); }
  public static Offset<SpecialActivityLevel> EndSpecialActivityLevel(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SpecialActivityLevel>(o);
  }
};

public sealed class NormalActivityInstance : Table {
  public static NormalActivityInstance GetRootAsNormalActivityInstance(ByteBuffer _bb) { return GetRootAsNormalActivityInstance(_bb, new NormalActivityInstance()); }
  public static NormalActivityInstance GetRootAsNormalActivityInstance(ByteBuffer _bb, NormalActivityInstance obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public NormalActivityInstance __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ActivityId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int T { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string S { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSBytes() { return __vector_as_arraysegment(10); }
  public string E { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEBytes() { return __vector_as_arraysegment(12); }

  public static Offset<NormalActivityInstance> CreateNormalActivityInstance(FlatBufferBuilder builder,
      int id = 0,
      int activity_id = 0,
      int t = 0,
      StringOffset sOffset = default(StringOffset),
      StringOffset eOffset = default(StringOffset)) {
    builder.StartObject(5);
    NormalActivityInstance.AddE(builder, eOffset);
    NormalActivityInstance.AddS(builder, sOffset);
    NormalActivityInstance.AddT(builder, t);
    NormalActivityInstance.AddActivityId(builder, activity_id);
    NormalActivityInstance.AddId(builder, id);
    return NormalActivityInstance.EndNormalActivityInstance(builder);
  }

  public static void StartNormalActivityInstance(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddActivityId(FlatBufferBuilder builder, int activityId) { builder.AddInt(1, activityId, 0); }
  public static void AddT(FlatBufferBuilder builder, int t) { builder.AddInt(2, t, 0); }
  public static void AddS(FlatBufferBuilder builder, StringOffset sOffset) { builder.AddOffset(3, sOffset.Value, 0); }
  public static void AddE(FlatBufferBuilder builder, StringOffset eOffset) { builder.AddOffset(4, eOffset.Value, 0); }
  public static Offset<NormalActivityInstance> EndNormalActivityInstance(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<NormalActivityInstance>(o);
  }
};

public sealed class TimeLimitActivity : Table {
  public static TimeLimitActivity GetRootAsTimeLimitActivity(ByteBuffer _bb) { return GetRootAsTimeLimitActivity(_bb, new TimeLimitActivity()); }
  public static TimeLimitActivity GetRootAsTimeLimitActivity(ByteBuffer _bb, TimeLimitActivity obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public TimeLimitActivity __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string Title { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTitleBytes() { return __vector_as_arraysegment(8); }
  public string UiModel { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetUiModelBytes() { return __vector_as_arraysegment(10); }
  public string Parameter1 { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetParameter1Bytes() { return __vector_as_arraysegment(12); }
  public string Parameter2 { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetParameter2Bytes() { return __vector_as_arraysegment(14); }
  public int NavType { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string NavParameter { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNavParameterBytes() { return __vector_as_arraysegment(18); }
  public string Icon { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(20); }
  public string NoticeContent { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNoticeContentBytes() { return __vector_as_arraysegment(22); }
  public string Award1 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAward1Bytes() { return __vector_as_arraysegment(24); }
  public string Award2 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAward2Bytes() { return __vector_as_arraysegment(26); }
  public string Award3 { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAward3Bytes() { return __vector_as_arraysegment(28); }
  public string Award4 { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAward4Bytes() { return __vector_as_arraysegment(30); }

  public static Offset<TimeLimitActivity> CreateTimeLimitActivity(FlatBufferBuilder builder,
      int id = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset titleOffset = default(StringOffset),
      StringOffset ui_modelOffset = default(StringOffset),
      StringOffset parameter1Offset = default(StringOffset),
      StringOffset parameter2Offset = default(StringOffset),
      int nav_type = 0,
      StringOffset nav_parameterOffset = default(StringOffset),
      StringOffset iconOffset = default(StringOffset),
      StringOffset notice_contentOffset = default(StringOffset),
      StringOffset award1Offset = default(StringOffset),
      StringOffset award2Offset = default(StringOffset),
      StringOffset award3Offset = default(StringOffset),
      StringOffset award4Offset = default(StringOffset)) {
    builder.StartObject(14);
    TimeLimitActivity.AddAward4(builder, award4Offset);
    TimeLimitActivity.AddAward3(builder, award3Offset);
    TimeLimitActivity.AddAward2(builder, award2Offset);
    TimeLimitActivity.AddAward1(builder, award1Offset);
    TimeLimitActivity.AddNoticeContent(builder, notice_contentOffset);
    TimeLimitActivity.AddIcon(builder, iconOffset);
    TimeLimitActivity.AddNavParameter(builder, nav_parameterOffset);
    TimeLimitActivity.AddNavType(builder, nav_type);
    TimeLimitActivity.AddParameter2(builder, parameter2Offset);
    TimeLimitActivity.AddParameter1(builder, parameter1Offset);
    TimeLimitActivity.AddUiModel(builder, ui_modelOffset);
    TimeLimitActivity.AddTitle(builder, titleOffset);
    TimeLimitActivity.AddName(builder, nameOffset);
    TimeLimitActivity.AddId(builder, id);
    return TimeLimitActivity.EndTimeLimitActivity(builder);
  }

  public static void StartTimeLimitActivity(FlatBufferBuilder builder) { builder.StartObject(14); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddTitle(FlatBufferBuilder builder, StringOffset titleOffset) { builder.AddOffset(2, titleOffset.Value, 0); }
  public static void AddUiModel(FlatBufferBuilder builder, StringOffset uiModelOffset) { builder.AddOffset(3, uiModelOffset.Value, 0); }
  public static void AddParameter1(FlatBufferBuilder builder, StringOffset parameter1Offset) { builder.AddOffset(4, parameter1Offset.Value, 0); }
  public static void AddParameter2(FlatBufferBuilder builder, StringOffset parameter2Offset) { builder.AddOffset(5, parameter2Offset.Value, 0); }
  public static void AddNavType(FlatBufferBuilder builder, int navType) { builder.AddInt(6, navType, 0); }
  public static void AddNavParameter(FlatBufferBuilder builder, StringOffset navParameterOffset) { builder.AddOffset(7, navParameterOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(8, iconOffset.Value, 0); }
  public static void AddNoticeContent(FlatBufferBuilder builder, StringOffset noticeContentOffset) { builder.AddOffset(9, noticeContentOffset.Value, 0); }
  public static void AddAward1(FlatBufferBuilder builder, StringOffset award1Offset) { builder.AddOffset(10, award1Offset.Value, 0); }
  public static void AddAward2(FlatBufferBuilder builder, StringOffset award2Offset) { builder.AddOffset(11, award2Offset.Value, 0); }
  public static void AddAward3(FlatBufferBuilder builder, StringOffset award3Offset) { builder.AddOffset(12, award3Offset.Value, 0); }
  public static void AddAward4(FlatBufferBuilder builder, StringOffset award4Offset) { builder.AddOffset(13, award4Offset.Value, 0); }
  public static Offset<TimeLimitActivity> EndTimeLimitActivity(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<TimeLimitActivity>(o);
  }
};

public sealed class TimeLimitActivityStage : Table {
  public static TimeLimitActivityStage GetRootAsTimeLimitActivityStage(ByteBuffer _bb) { return GetRootAsTimeLimitActivityStage(_bb, new TimeLimitActivityStage()); }
  public static TimeLimitActivityStage GetRootAsTimeLimitActivityStage(ByteBuffer _bb, TimeLimitActivityStage obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public TimeLimitActivityStage __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ActivityId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Stage { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool IsCost { get { int o = __offset(10); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int Num { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Sort { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Rt1 { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt1Bytes() { return __vector_as_arraysegment(16); }
  public string Ri1 { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi1Bytes() { return __vector_as_arraysegment(18); }
  public int Rn1 { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Rt2 { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt2Bytes() { return __vector_as_arraysegment(22); }
  public string Ri2 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi2Bytes() { return __vector_as_arraysegment(24); }
  public int Rn2 { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Rt3 { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt3Bytes() { return __vector_as_arraysegment(28); }
  public string Ri3 { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi3Bytes() { return __vector_as_arraysegment(30); }
  public int Rn3 { get { int o = __offset(32); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Rt4 { get { int o = __offset(34); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt4Bytes() { return __vector_as_arraysegment(34); }
  public string Ri4 { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi4Bytes() { return __vector_as_arraysegment(36); }
  public int Rn4 { get { int o = __offset(38); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RealmNum { get { int o = __offset(40); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<TimeLimitActivityStage> CreateTimeLimitActivityStage(FlatBufferBuilder builder,
      int id = 0,
      int activity_id = 0,
      int stage = 0,
      bool is_cost = false,
      int num = 0,
      int sort = 0,
      StringOffset rt1Offset = default(StringOffset),
      StringOffset ri1Offset = default(StringOffset),
      int rn1 = 0,
      StringOffset rt2Offset = default(StringOffset),
      StringOffset ri2Offset = default(StringOffset),
      int rn2 = 0,
      StringOffset rt3Offset = default(StringOffset),
      StringOffset ri3Offset = default(StringOffset),
      int rn3 = 0,
      StringOffset rt4Offset = default(StringOffset),
      StringOffset ri4Offset = default(StringOffset),
      int rn4 = 0,
      int realm_num = 0) {
    builder.StartObject(19);
    TimeLimitActivityStage.AddRealmNum(builder, realm_num);
    TimeLimitActivityStage.AddRn4(builder, rn4);
    TimeLimitActivityStage.AddRi4(builder, ri4Offset);
    TimeLimitActivityStage.AddRt4(builder, rt4Offset);
    TimeLimitActivityStage.AddRn3(builder, rn3);
    TimeLimitActivityStage.AddRi3(builder, ri3Offset);
    TimeLimitActivityStage.AddRt3(builder, rt3Offset);
    TimeLimitActivityStage.AddRn2(builder, rn2);
    TimeLimitActivityStage.AddRi2(builder, ri2Offset);
    TimeLimitActivityStage.AddRt2(builder, rt2Offset);
    TimeLimitActivityStage.AddRn1(builder, rn1);
    TimeLimitActivityStage.AddRi1(builder, ri1Offset);
    TimeLimitActivityStage.AddRt1(builder, rt1Offset);
    TimeLimitActivityStage.AddSort(builder, sort);
    TimeLimitActivityStage.AddNum(builder, num);
    TimeLimitActivityStage.AddStage(builder, stage);
    TimeLimitActivityStage.AddActivityId(builder, activity_id);
    TimeLimitActivityStage.AddId(builder, id);
    TimeLimitActivityStage.AddIsCost(builder, is_cost);
    return TimeLimitActivityStage.EndTimeLimitActivityStage(builder);
  }

  public static void StartTimeLimitActivityStage(FlatBufferBuilder builder) { builder.StartObject(19); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddActivityId(FlatBufferBuilder builder, int activityId) { builder.AddInt(1, activityId, 0); }
  public static void AddStage(FlatBufferBuilder builder, int stage) { builder.AddInt(2, stage, 0); }
  public static void AddIsCost(FlatBufferBuilder builder, bool isCost) { builder.AddBool(3, isCost, false); }
  public static void AddNum(FlatBufferBuilder builder, int num) { builder.AddInt(4, num, 0); }
  public static void AddSort(FlatBufferBuilder builder, int sort) { builder.AddInt(5, sort, 0); }
  public static void AddRt1(FlatBufferBuilder builder, StringOffset rt1Offset) { builder.AddOffset(6, rt1Offset.Value, 0); }
  public static void AddRi1(FlatBufferBuilder builder, StringOffset ri1Offset) { builder.AddOffset(7, ri1Offset.Value, 0); }
  public static void AddRn1(FlatBufferBuilder builder, int rn1) { builder.AddInt(8, rn1, 0); }
  public static void AddRt2(FlatBufferBuilder builder, StringOffset rt2Offset) { builder.AddOffset(9, rt2Offset.Value, 0); }
  public static void AddRi2(FlatBufferBuilder builder, StringOffset ri2Offset) { builder.AddOffset(10, ri2Offset.Value, 0); }
  public static void AddRn2(FlatBufferBuilder builder, int rn2) { builder.AddInt(11, rn2, 0); }
  public static void AddRt3(FlatBufferBuilder builder, StringOffset rt3Offset) { builder.AddOffset(12, rt3Offset.Value, 0); }
  public static void AddRi3(FlatBufferBuilder builder, StringOffset ri3Offset) { builder.AddOffset(13, ri3Offset.Value, 0); }
  public static void AddRn3(FlatBufferBuilder builder, int rn3) { builder.AddInt(14, rn3, 0); }
  public static void AddRt4(FlatBufferBuilder builder, StringOffset rt4Offset) { builder.AddOffset(15, rt4Offset.Value, 0); }
  public static void AddRi4(FlatBufferBuilder builder, StringOffset ri4Offset) { builder.AddOffset(16, ri4Offset.Value, 0); }
  public static void AddRn4(FlatBufferBuilder builder, int rn4) { builder.AddInt(17, rn4, 0); }
  public static void AddRealmNum(FlatBufferBuilder builder, int realmNum) { builder.AddInt(18, realmNum, 0); }
  public static Offset<TimeLimitActivityStage> EndTimeLimitActivityStage(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<TimeLimitActivityStage>(o);
  }
};

public sealed class InfiniteChallenge : Table {
  public static InfiniteChallenge GetRootAsInfiniteChallenge(ByteBuffer _bb) { return GetRootAsInfiniteChallenge(_bb, new InfiniteChallenge()); }
  public static InfiniteChallenge GetRootAsInfiniteChallenge(ByteBuffer _bb, InfiniteChallenge obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public InfiniteChallenge __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Chapter { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Award { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAwardBytes() { return __vector_as_arraysegment(6); }
  public string FirstAward { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetFirstAwardBytes() { return __vector_as_arraysegment(8); }
  public string ModelName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetModelNameBytes() { return __vector_as_arraysegment(10); }
  public string CombatLayoutName { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCombatLayoutNameBytes() { return __vector_as_arraysegment(12); }
  public string Name { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(14); }
  public int Level { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<InfiniteChallenge> CreateInfiniteChallenge(FlatBufferBuilder builder,
      int chapter = 0,
      StringOffset awardOffset = default(StringOffset),
      StringOffset first_awardOffset = default(StringOffset),
      StringOffset model_nameOffset = default(StringOffset),
      StringOffset combat_layout_nameOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      int level = 0) {
    builder.StartObject(7);
    InfiniteChallenge.AddLevel(builder, level);
    InfiniteChallenge.AddName(builder, nameOffset);
    InfiniteChallenge.AddCombatLayoutName(builder, combat_layout_nameOffset);
    InfiniteChallenge.AddModelName(builder, model_nameOffset);
    InfiniteChallenge.AddFirstAward(builder, first_awardOffset);
    InfiniteChallenge.AddAward(builder, awardOffset);
    InfiniteChallenge.AddChapter(builder, chapter);
    return InfiniteChallenge.EndInfiniteChallenge(builder);
  }

  public static void StartInfiniteChallenge(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddChapter(FlatBufferBuilder builder, int chapter) { builder.AddInt(0, chapter, 0); }
  public static void AddAward(FlatBufferBuilder builder, StringOffset awardOffset) { builder.AddOffset(1, awardOffset.Value, 0); }
  public static void AddFirstAward(FlatBufferBuilder builder, StringOffset firstAwardOffset) { builder.AddOffset(2, firstAwardOffset.Value, 0); }
  public static void AddModelName(FlatBufferBuilder builder, StringOffset modelNameOffset) { builder.AddOffset(3, modelNameOffset.Value, 0); }
  public static void AddCombatLayoutName(FlatBufferBuilder builder, StringOffset combatLayoutNameOffset) { builder.AddOffset(4, combatLayoutNameOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(5, nameOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(6, level, 0); }
  public static Offset<InfiniteChallenge> EndInfiniteChallenge(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<InfiniteChallenge>(o);
  }
};

public sealed class Ladder : Table {
  public static Ladder GetRootAsLadder(ByteBuffer _bb) { return GetRootAsLadder(_bb, new Ladder()); }
  public static Ladder GetRootAsLadder(ByteBuffer _bb, Ladder obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Ladder __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Stage { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetStageBytes() { return __vector_as_arraysegment(6); }
  public int Point { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Mmr { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MmrWin { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MmrFail { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int DayGold { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int DayDiamond { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int DayLadderGold { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int WeekGold { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int WeekDiamond { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int WeekLadderGold { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<Ladder> CreateLadder(FlatBufferBuilder builder,
      int id = 0,
      StringOffset stageOffset = default(StringOffset),
      int point = 0,
      int mmr = 0,
      int mmr_win = 0,
      int mmr_fail = 0,
      int day_gold = 0,
      int day_diamond = 0,
      int day_ladder_gold = 0,
      int week_gold = 0,
      int week_diamond = 0,
      int week_ladder_gold = 0) {
    builder.StartObject(12);
    Ladder.AddWeekLadderGold(builder, week_ladder_gold);
    Ladder.AddWeekDiamond(builder, week_diamond);
    Ladder.AddWeekGold(builder, week_gold);
    Ladder.AddDayLadderGold(builder, day_ladder_gold);
    Ladder.AddDayDiamond(builder, day_diamond);
    Ladder.AddDayGold(builder, day_gold);
    Ladder.AddMmrFail(builder, mmr_fail);
    Ladder.AddMmrWin(builder, mmr_win);
    Ladder.AddMmr(builder, mmr);
    Ladder.AddPoint(builder, point);
    Ladder.AddStage(builder, stageOffset);
    Ladder.AddId(builder, id);
    return Ladder.EndLadder(builder);
  }

  public static void StartLadder(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddStage(FlatBufferBuilder builder, StringOffset stageOffset) { builder.AddOffset(1, stageOffset.Value, 0); }
  public static void AddPoint(FlatBufferBuilder builder, int point) { builder.AddInt(2, point, 0); }
  public static void AddMmr(FlatBufferBuilder builder, int mmr) { builder.AddInt(3, mmr, 0); }
  public static void AddMmrWin(FlatBufferBuilder builder, int mmrWin) { builder.AddInt(4, mmrWin, 0); }
  public static void AddMmrFail(FlatBufferBuilder builder, int mmrFail) { builder.AddInt(5, mmrFail, 0); }
  public static void AddDayGold(FlatBufferBuilder builder, int dayGold) { builder.AddInt(6, dayGold, 0); }
  public static void AddDayDiamond(FlatBufferBuilder builder, int dayDiamond) { builder.AddInt(7, dayDiamond, 0); }
  public static void AddDayLadderGold(FlatBufferBuilder builder, int dayLadderGold) { builder.AddInt(8, dayLadderGold, 0); }
  public static void AddWeekGold(FlatBufferBuilder builder, int weekGold) { builder.AddInt(9, weekGold, 0); }
  public static void AddWeekDiamond(FlatBufferBuilder builder, int weekDiamond) { builder.AddInt(10, weekDiamond, 0); }
  public static void AddWeekLadderGold(FlatBufferBuilder builder, int weekLadderGold) { builder.AddInt(11, weekLadderGold, 0); }
  public static Offset<Ladder> EndLadder(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Ladder>(o);
  }
};

public sealed class LadderSeasonAward : Table {
  public static LadderSeasonAward GetRootAsLadderSeasonAward(ByteBuffer _bb) { return GetRootAsLadderSeasonAward(_bb, new LadderSeasonAward()); }
  public static LadderSeasonAward GetRootAsLadderSeasonAward(ByteBuffer _bb, LadderSeasonAward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LadderSeasonAward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int MinRank { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MaxRank { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Gold { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Diamond { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LadderGold { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Item { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemBytes() { return __vector_as_arraysegment(14); }

  public static Offset<LadderSeasonAward> CreateLadderSeasonAward(FlatBufferBuilder builder,
      int min_rank = 0,
      int max_rank = 0,
      int gold = 0,
      int diamond = 0,
      int ladder_gold = 0,
      StringOffset itemOffset = default(StringOffset)) {
    builder.StartObject(6);
    LadderSeasonAward.AddItem(builder, itemOffset);
    LadderSeasonAward.AddLadderGold(builder, ladder_gold);
    LadderSeasonAward.AddDiamond(builder, diamond);
    LadderSeasonAward.AddGold(builder, gold);
    LadderSeasonAward.AddMaxRank(builder, max_rank);
    LadderSeasonAward.AddMinRank(builder, min_rank);
    return LadderSeasonAward.EndLadderSeasonAward(builder);
  }

  public static void StartLadderSeasonAward(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddMinRank(FlatBufferBuilder builder, int minRank) { builder.AddInt(0, minRank, 0); }
  public static void AddMaxRank(FlatBufferBuilder builder, int maxRank) { builder.AddInt(1, maxRank, 0); }
  public static void AddGold(FlatBufferBuilder builder, int gold) { builder.AddInt(2, gold, 0); }
  public static void AddDiamond(FlatBufferBuilder builder, int diamond) { builder.AddInt(3, diamond, 0); }
  public static void AddLadderGold(FlatBufferBuilder builder, int ladderGold) { builder.AddInt(4, ladderGold, 0); }
  public static void AddItem(FlatBufferBuilder builder, StringOffset itemOffset) { builder.AddOffset(5, itemOffset.Value, 0); }
  public static Offset<LadderSeasonAward> EndLadderSeasonAward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LadderSeasonAward>(o);
  }
};

public sealed class BossReward : Table {
  public static BossReward GetRootAsBossReward(ByteBuffer _bb) { return GetRootAsBossReward(_bb, new BossReward()); }
  public static BossReward GetRootAsBossReward(ByteBuffer _bb, BossReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BossReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Ranks { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRanksBytes() { return __vector_as_arraysegment(6); }
  public float StageReward { get { int o = __offset(8); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string Reward { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBytes() { return __vector_as_arraysegment(10); }
  public string Box { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBoxBytes() { return __vector_as_arraysegment(12); }

  public static Offset<BossReward> CreateBossReward(FlatBufferBuilder builder,
      int id = 0,
      StringOffset ranksOffset = default(StringOffset),
      float stage_reward = 0,
      StringOffset rewardOffset = default(StringOffset),
      StringOffset boxOffset = default(StringOffset)) {
    builder.StartObject(5);
    BossReward.AddBox(builder, boxOffset);
    BossReward.AddReward(builder, rewardOffset);
    BossReward.AddStageReward(builder, stage_reward);
    BossReward.AddRanks(builder, ranksOffset);
    BossReward.AddId(builder, id);
    return BossReward.EndBossReward(builder);
  }

  public static void StartBossReward(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddRanks(FlatBufferBuilder builder, StringOffset ranksOffset) { builder.AddOffset(1, ranksOffset.Value, 0); }
  public static void AddStageReward(FlatBufferBuilder builder, float stageReward) { builder.AddFloat(2, stageReward, 0); }
  public static void AddReward(FlatBufferBuilder builder, StringOffset rewardOffset) { builder.AddOffset(3, rewardOffset.Value, 0); }
  public static void AddBox(FlatBufferBuilder builder, StringOffset boxOffset) { builder.AddOffset(4, boxOffset.Value, 0); }
  public static Offset<BossReward> EndBossReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BossReward>(o);
  }
};

public sealed class HonorArenaReward : Table {
  public static HonorArenaReward GetRootAsHonorArenaReward(ByteBuffer _bb) { return GetRootAsHonorArenaReward(_bb, new HonorArenaReward()); }
  public static HonorArenaReward GetRootAsHonorArenaReward(ByteBuffer _bb, HonorArenaReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HonorArenaReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Ranks { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRanksBytes() { return __vector_as_arraysegment(6); }
  public string Reward { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBytes() { return __vector_as_arraysegment(8); }
  public string OneHourReward { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetOneHourRewardBytes() { return __vector_as_arraysegment(10); }

  public static Offset<HonorArenaReward> CreateHonorArenaReward(FlatBufferBuilder builder,
      int id = 0,
      StringOffset ranksOffset = default(StringOffset),
      StringOffset rewardOffset = default(StringOffset),
      StringOffset one_hour_rewardOffset = default(StringOffset)) {
    builder.StartObject(4);
    HonorArenaReward.AddOneHourReward(builder, one_hour_rewardOffset);
    HonorArenaReward.AddReward(builder, rewardOffset);
    HonorArenaReward.AddRanks(builder, ranksOffset);
    HonorArenaReward.AddId(builder, id);
    return HonorArenaReward.EndHonorArenaReward(builder);
  }

  public static void StartHonorArenaReward(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddRanks(FlatBufferBuilder builder, StringOffset ranksOffset) { builder.AddOffset(1, ranksOffset.Value, 0); }
  public static void AddReward(FlatBufferBuilder builder, StringOffset rewardOffset) { builder.AddOffset(2, rewardOffset.Value, 0); }
  public static void AddOneHourReward(FlatBufferBuilder builder, StringOffset oneHourRewardOffset) { builder.AddOffset(3, oneHourRewardOffset.Value, 0); }
  public static Offset<HonorArenaReward> EndHonorArenaReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HonorArenaReward>(o);
  }
};

public sealed class ClashOfHeroesReward : Table {
  public static ClashOfHeroesReward GetRootAsClashOfHeroesReward(ByteBuffer _bb) { return GetRootAsClashOfHeroesReward(_bb, new ClashOfHeroesReward()); }
  public static ClashOfHeroesReward GetRootAsClashOfHeroesReward(ByteBuffer _bb, ClashOfHeroesReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ClashOfHeroesReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType1 { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType1Bytes() { return __vector_as_arraysegment(6); }
  public string ItemId1 { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId1Bytes() { return __vector_as_arraysegment(8); }
  public int ItemNum1 { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType2 { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType2Bytes() { return __vector_as_arraysegment(12); }
  public string ItemId2 { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId2Bytes() { return __vector_as_arraysegment(14); }
  public int ItemNum2 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType3 { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType3Bytes() { return __vector_as_arraysegment(18); }
  public string ItemId3 { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId3Bytes() { return __vector_as_arraysegment(20); }
  public int ItemNum3 { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType4 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType4Bytes() { return __vector_as_arraysegment(24); }
  public string ItemId4 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId4Bytes() { return __vector_as_arraysegment(26); }
  public int ItemNum4 { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<ClashOfHeroesReward> CreateClashOfHeroesReward(FlatBufferBuilder builder,
      int id = 0,
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
    builder.StartObject(13);
    ClashOfHeroesReward.AddItemNum4(builder, item_num4);
    ClashOfHeroesReward.AddItemId4(builder, item_id4Offset);
    ClashOfHeroesReward.AddItemType4(builder, item_type4Offset);
    ClashOfHeroesReward.AddItemNum3(builder, item_num3);
    ClashOfHeroesReward.AddItemId3(builder, item_id3Offset);
    ClashOfHeroesReward.AddItemType3(builder, item_type3Offset);
    ClashOfHeroesReward.AddItemNum2(builder, item_num2);
    ClashOfHeroesReward.AddItemId2(builder, item_id2Offset);
    ClashOfHeroesReward.AddItemType2(builder, item_type2Offset);
    ClashOfHeroesReward.AddItemNum1(builder, item_num1);
    ClashOfHeroesReward.AddItemId1(builder, item_id1Offset);
    ClashOfHeroesReward.AddItemType1(builder, item_type1Offset);
    ClashOfHeroesReward.AddId(builder, id);
    return ClashOfHeroesReward.EndClashOfHeroesReward(builder);
  }

  public static void StartClashOfHeroesReward(FlatBufferBuilder builder) { builder.StartObject(13); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddItemType1(FlatBufferBuilder builder, StringOffset itemType1Offset) { builder.AddOffset(1, itemType1Offset.Value, 0); }
  public static void AddItemId1(FlatBufferBuilder builder, StringOffset itemId1Offset) { builder.AddOffset(2, itemId1Offset.Value, 0); }
  public static void AddItemNum1(FlatBufferBuilder builder, int itemNum1) { builder.AddInt(3, itemNum1, 0); }
  public static void AddItemType2(FlatBufferBuilder builder, StringOffset itemType2Offset) { builder.AddOffset(4, itemType2Offset.Value, 0); }
  public static void AddItemId2(FlatBufferBuilder builder, StringOffset itemId2Offset) { builder.AddOffset(5, itemId2Offset.Value, 0); }
  public static void AddItemNum2(FlatBufferBuilder builder, int itemNum2) { builder.AddInt(6, itemNum2, 0); }
  public static void AddItemType3(FlatBufferBuilder builder, StringOffset itemType3Offset) { builder.AddOffset(7, itemType3Offset.Value, 0); }
  public static void AddItemId3(FlatBufferBuilder builder, StringOffset itemId3Offset) { builder.AddOffset(8, itemId3Offset.Value, 0); }
  public static void AddItemNum3(FlatBufferBuilder builder, int itemNum3) { builder.AddInt(9, itemNum3, 0); }
  public static void AddItemType4(FlatBufferBuilder builder, StringOffset itemType4Offset) { builder.AddOffset(10, itemType4Offset.Value, 0); }
  public static void AddItemId4(FlatBufferBuilder builder, StringOffset itemId4Offset) { builder.AddOffset(11, itemId4Offset.Value, 0); }
  public static void AddItemNum4(FlatBufferBuilder builder, int itemNum4) { builder.AddInt(12, itemNum4, 0); }
  public static Offset<ClashOfHeroesReward> EndClashOfHeroesReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ClashOfHeroesReward>(o);
  }
};

public sealed class NationRatingReward : Table {
  public static NationRatingReward GetRootAsNationRatingReward(ByteBuffer _bb) { return GetRootAsNationRatingReward(_bb, new NationRatingReward()); }
  public static NationRatingReward GetRootAsNationRatingReward(ByteBuffer _bb, NationRatingReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public NationRatingReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Rating { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType1 { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType1Bytes() { return __vector_as_arraysegment(6); }
  public string ItemId1 { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId1Bytes() { return __vector_as_arraysegment(8); }
  public int ItemNum1 { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType2 { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType2Bytes() { return __vector_as_arraysegment(12); }
  public string ItemId2 { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId2Bytes() { return __vector_as_arraysegment(14); }
  public int ItemNum2 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType3 { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType3Bytes() { return __vector_as_arraysegment(18); }
  public string ItemId3 { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId3Bytes() { return __vector_as_arraysegment(20); }
  public int ItemNum3 { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType4 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType4Bytes() { return __vector_as_arraysegment(24); }
  public string ItemId4 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId4Bytes() { return __vector_as_arraysegment(26); }
  public int ItemNum4 { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<NationRatingReward> CreateNationRatingReward(FlatBufferBuilder builder,
      int rating = 0,
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
    builder.StartObject(13);
    NationRatingReward.AddItemNum4(builder, item_num4);
    NationRatingReward.AddItemId4(builder, item_id4Offset);
    NationRatingReward.AddItemType4(builder, item_type4Offset);
    NationRatingReward.AddItemNum3(builder, item_num3);
    NationRatingReward.AddItemId3(builder, item_id3Offset);
    NationRatingReward.AddItemType3(builder, item_type3Offset);
    NationRatingReward.AddItemNum2(builder, item_num2);
    NationRatingReward.AddItemId2(builder, item_id2Offset);
    NationRatingReward.AddItemType2(builder, item_type2Offset);
    NationRatingReward.AddItemNum1(builder, item_num1);
    NationRatingReward.AddItemId1(builder, item_id1Offset);
    NationRatingReward.AddItemType1(builder, item_type1Offset);
    NationRatingReward.AddRating(builder, rating);
    return NationRatingReward.EndNationRatingReward(builder);
  }

  public static void StartNationRatingReward(FlatBufferBuilder builder) { builder.StartObject(13); }
  public static void AddRating(FlatBufferBuilder builder, int rating) { builder.AddInt(0, rating, 0); }
  public static void AddItemType1(FlatBufferBuilder builder, StringOffset itemType1Offset) { builder.AddOffset(1, itemType1Offset.Value, 0); }
  public static void AddItemId1(FlatBufferBuilder builder, StringOffset itemId1Offset) { builder.AddOffset(2, itemId1Offset.Value, 0); }
  public static void AddItemNum1(FlatBufferBuilder builder, int itemNum1) { builder.AddInt(3, itemNum1, 0); }
  public static void AddItemType2(FlatBufferBuilder builder, StringOffset itemType2Offset) { builder.AddOffset(4, itemType2Offset.Value, 0); }
  public static void AddItemId2(FlatBufferBuilder builder, StringOffset itemId2Offset) { builder.AddOffset(5, itemId2Offset.Value, 0); }
  public static void AddItemNum2(FlatBufferBuilder builder, int itemNum2) { builder.AddInt(6, itemNum2, 0); }
  public static void AddItemType3(FlatBufferBuilder builder, StringOffset itemType3Offset) { builder.AddOffset(7, itemType3Offset.Value, 0); }
  public static void AddItemId3(FlatBufferBuilder builder, StringOffset itemId3Offset) { builder.AddOffset(8, itemId3Offset.Value, 0); }
  public static void AddItemNum3(FlatBufferBuilder builder, int itemNum3) { builder.AddInt(9, itemNum3, 0); }
  public static void AddItemType4(FlatBufferBuilder builder, StringOffset itemType4Offset) { builder.AddOffset(10, itemType4Offset.Value, 0); }
  public static void AddItemId4(FlatBufferBuilder builder, StringOffset itemId4Offset) { builder.AddOffset(11, itemId4Offset.Value, 0); }
  public static void AddItemNum4(FlatBufferBuilder builder, int itemNum4) { builder.AddInt(12, itemNum4, 0); }
  public static Offset<NationRatingReward> EndNationRatingReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<NationRatingReward>(o);
  }
};

public sealed class NationRankReward : Table {
  public static NationRankReward GetRootAsNationRankReward(ByteBuffer _bb) { return GetRootAsNationRankReward(_bb, new NationRankReward()); }
  public static NationRankReward GetRootAsNationRankReward(ByteBuffer _bb, NationRankReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public NationRankReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Rank { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType1 { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType1Bytes() { return __vector_as_arraysegment(6); }
  public string ItemId1 { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId1Bytes() { return __vector_as_arraysegment(8); }
  public int ItemNum1 { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType2 { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType2Bytes() { return __vector_as_arraysegment(12); }
  public string ItemId2 { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId2Bytes() { return __vector_as_arraysegment(14); }
  public int ItemNum2 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType3 { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType3Bytes() { return __vector_as_arraysegment(18); }
  public string ItemId3 { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId3Bytes() { return __vector_as_arraysegment(20); }
  public int ItemNum3 { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType4 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemType4Bytes() { return __vector_as_arraysegment(24); }
  public string ItemId4 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemId4Bytes() { return __vector_as_arraysegment(26); }
  public int ItemNum4 { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<NationRankReward> CreateNationRankReward(FlatBufferBuilder builder,
      int rank = 0,
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
    builder.StartObject(13);
    NationRankReward.AddItemNum4(builder, item_num4);
    NationRankReward.AddItemId4(builder, item_id4Offset);
    NationRankReward.AddItemType4(builder, item_type4Offset);
    NationRankReward.AddItemNum3(builder, item_num3);
    NationRankReward.AddItemId3(builder, item_id3Offset);
    NationRankReward.AddItemType3(builder, item_type3Offset);
    NationRankReward.AddItemNum2(builder, item_num2);
    NationRankReward.AddItemId2(builder, item_id2Offset);
    NationRankReward.AddItemType2(builder, item_type2Offset);
    NationRankReward.AddItemNum1(builder, item_num1);
    NationRankReward.AddItemId1(builder, item_id1Offset);
    NationRankReward.AddItemType1(builder, item_type1Offset);
    NationRankReward.AddRank(builder, rank);
    return NationRankReward.EndNationRankReward(builder);
  }

  public static void StartNationRankReward(FlatBufferBuilder builder) { builder.StartObject(13); }
  public static void AddRank(FlatBufferBuilder builder, int rank) { builder.AddInt(0, rank, 0); }
  public static void AddItemType1(FlatBufferBuilder builder, StringOffset itemType1Offset) { builder.AddOffset(1, itemType1Offset.Value, 0); }
  public static void AddItemId1(FlatBufferBuilder builder, StringOffset itemId1Offset) { builder.AddOffset(2, itemId1Offset.Value, 0); }
  public static void AddItemNum1(FlatBufferBuilder builder, int itemNum1) { builder.AddInt(3, itemNum1, 0); }
  public static void AddItemType2(FlatBufferBuilder builder, StringOffset itemType2Offset) { builder.AddOffset(4, itemType2Offset.Value, 0); }
  public static void AddItemId2(FlatBufferBuilder builder, StringOffset itemId2Offset) { builder.AddOffset(5, itemId2Offset.Value, 0); }
  public static void AddItemNum2(FlatBufferBuilder builder, int itemNum2) { builder.AddInt(6, itemNum2, 0); }
  public static void AddItemType3(FlatBufferBuilder builder, StringOffset itemType3Offset) { builder.AddOffset(7, itemType3Offset.Value, 0); }
  public static void AddItemId3(FlatBufferBuilder builder, StringOffset itemId3Offset) { builder.AddOffset(8, itemId3Offset.Value, 0); }
  public static void AddItemNum3(FlatBufferBuilder builder, int itemNum3) { builder.AddInt(9, itemNum3, 0); }
  public static void AddItemType4(FlatBufferBuilder builder, StringOffset itemType4Offset) { builder.AddOffset(10, itemType4Offset.Value, 0); }
  public static void AddItemId4(FlatBufferBuilder builder, StringOffset itemId4Offset) { builder.AddOffset(11, itemId4Offset.Value, 0); }
  public static void AddItemNum4(FlatBufferBuilder builder, int itemNum4) { builder.AddInt(12, itemNum4, 0); }
  public static Offset<NationRankReward> EndNationRankReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<NationRankReward>(o);
  }
};

public sealed class NationSpecialEvent : Table {
  public static NationSpecialEvent GetRootAsNationSpecialEvent(ByteBuffer _bb) { return GetRootAsNationSpecialEvent(_bb, new NationSpecialEvent()); }
  public static NationSpecialEvent GetRootAsNationSpecialEvent(ByteBuffer _bb, NationSpecialEvent obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public NationSpecialEvent __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Rating { get { int o = __offset(6); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string Desc { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(8); }
  public string ResultDesc1 { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetResultDesc1Bytes() { return __vector_as_arraysegment(10); }
  public int ResultData1 { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float ResultRating1 { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string ResultDesc2 { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetResultDesc2Bytes() { return __vector_as_arraysegment(16); }
  public int ResultData2 { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float ResultRating2 { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<NationSpecialEvent> CreateNationSpecialEvent(FlatBufferBuilder builder,
      int id = 0,
      float rating = 0,
      StringOffset descOffset = default(StringOffset),
      StringOffset result_desc1Offset = default(StringOffset),
      int result_data1 = 0,
      float result_rating1 = 0,
      StringOffset result_desc2Offset = default(StringOffset),
      int result_data2 = 0,
      float result_rating2 = 0) {
    builder.StartObject(9);
    NationSpecialEvent.AddResultRating2(builder, result_rating2);
    NationSpecialEvent.AddResultData2(builder, result_data2);
    NationSpecialEvent.AddResultDesc2(builder, result_desc2Offset);
    NationSpecialEvent.AddResultRating1(builder, result_rating1);
    NationSpecialEvent.AddResultData1(builder, result_data1);
    NationSpecialEvent.AddResultDesc1(builder, result_desc1Offset);
    NationSpecialEvent.AddDesc(builder, descOffset);
    NationSpecialEvent.AddRating(builder, rating);
    NationSpecialEvent.AddId(builder, id);
    return NationSpecialEvent.EndNationSpecialEvent(builder);
  }

  public static void StartNationSpecialEvent(FlatBufferBuilder builder) { builder.StartObject(9); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddRating(FlatBufferBuilder builder, float rating) { builder.AddFloat(1, rating, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(2, descOffset.Value, 0); }
  public static void AddResultDesc1(FlatBufferBuilder builder, StringOffset resultDesc1Offset) { builder.AddOffset(3, resultDesc1Offset.Value, 0); }
  public static void AddResultData1(FlatBufferBuilder builder, int resultData1) { builder.AddInt(4, resultData1, 0); }
  public static void AddResultRating1(FlatBufferBuilder builder, float resultRating1) { builder.AddFloat(5, resultRating1, 0); }
  public static void AddResultDesc2(FlatBufferBuilder builder, StringOffset resultDesc2Offset) { builder.AddOffset(6, resultDesc2Offset.Value, 0); }
  public static void AddResultData2(FlatBufferBuilder builder, int resultData2) { builder.AddInt(7, resultData2, 0); }
  public static void AddResultRating2(FlatBufferBuilder builder, float resultRating2) { builder.AddFloat(8, resultRating2, 0); }
  public static Offset<NationSpecialEvent> EndNationSpecialEvent(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<NationSpecialEvent>(o);
  }
};

public sealed class InfiniteBuff : Table {
  public static InfiniteBuff GetRootAsInfiniteBuff(ByteBuffer _bb) { return GetRootAsInfiniteBuff(_bb, new InfiniteBuff()); }
  public static InfiniteBuff GetRootAsInfiniteBuff(ByteBuffer _bb, InfiniteBuff obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public InfiniteBuff __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Level { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public int ConditionNum { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Effect { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<InfiniteBuff> CreateInfiniteBuff(FlatBufferBuilder builder,
      int level = 0,
      StringOffset nameOffset = default(StringOffset),
      int condition_num = 0,
      int effect = 0) {
    builder.StartObject(4);
    InfiniteBuff.AddEffect(builder, effect);
    InfiniteBuff.AddConditionNum(builder, condition_num);
    InfiniteBuff.AddName(builder, nameOffset);
    InfiniteBuff.AddLevel(builder, level);
    return InfiniteBuff.EndInfiniteBuff(builder);
  }

  public static void StartInfiniteBuff(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(0, level, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddConditionNum(FlatBufferBuilder builder, int conditionNum) { builder.AddInt(2, conditionNum, 0); }
  public static void AddEffect(FlatBufferBuilder builder, int effect) { builder.AddInt(3, effect, 0); }
  public static Offset<InfiniteBuff> EndInfiniteBuff(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<InfiniteBuff>(o);
  }
};

public sealed class CronJobs : Table {
  public static CronJobs GetRootAsCronJobs(ByteBuffer _bb) { return GetRootAsCronJobs(_bb, new CronJobs()); }
  public static CronJobs GetRootAsCronJobs(ByteBuffer _bb, CronJobs obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public CronJobs __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(4); }
  public string Interval { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIntervalBytes() { return __vector_as_arraysegment(6); }
  public string Type { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTypeBytes() { return __vector_as_arraysegment(8); }
  public bool IsSchedule { get { int o = __offset(10); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public string Scope { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetScopeBytes() { return __vector_as_arraysegment(12); }
  public bool Open { get { int o = __offset(14); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int GetCloseRealmId(int j) { int o = __offset(16); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int CloseRealmIdLength { get { int o = __offset(16); return o != 0 ? __vector_len(o) : 0; } }
  public ArraySegment<byte>? GetCloseRealmIdBytes() { return __vector_as_arraysegment(16); }

  public static Offset<CronJobs> CreateCronJobs(FlatBufferBuilder builder,
      StringOffset nameOffset = default(StringOffset),
      StringOffset intervalOffset = default(StringOffset),
      StringOffset typeOffset = default(StringOffset),
      bool is_schedule = false,
      StringOffset scopeOffset = default(StringOffset),
      bool open = false,
      VectorOffset close_realm_idOffset = default(VectorOffset)) {
    builder.StartObject(7);
    CronJobs.AddCloseRealmId(builder, close_realm_idOffset);
    CronJobs.AddScope(builder, scopeOffset);
    CronJobs.AddType(builder, typeOffset);
    CronJobs.AddInterval(builder, intervalOffset);
    CronJobs.AddName(builder, nameOffset);
    CronJobs.AddOpen(builder, open);
    CronJobs.AddIsSchedule(builder, is_schedule);
    return CronJobs.EndCronJobs(builder);
  }

  public static void StartCronJobs(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddInterval(FlatBufferBuilder builder, StringOffset intervalOffset) { builder.AddOffset(1, intervalOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, StringOffset typeOffset) { builder.AddOffset(2, typeOffset.Value, 0); }
  public static void AddIsSchedule(FlatBufferBuilder builder, bool isSchedule) { builder.AddBool(3, isSchedule, false); }
  public static void AddScope(FlatBufferBuilder builder, StringOffset scopeOffset) { builder.AddOffset(4, scopeOffset.Value, 0); }
  public static void AddOpen(FlatBufferBuilder builder, bool open) { builder.AddBool(5, open, false); }
  public static void AddCloseRealmId(FlatBufferBuilder builder, VectorOffset closeRealmIdOffset) { builder.AddOffset(6, closeRealmIdOffset.Value, 0); }
  public static VectorOffset CreateCloseRealmIdVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartCloseRealmIdVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<CronJobs> EndCronJobs(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<CronJobs>(o);
  }
};

public sealed class PartnerCultivateGift : Table {
  public static PartnerCultivateGift GetRootAsPartnerCultivateGift(ByteBuffer _bb) { return GetRootAsPartnerCultivateGift(_bb, new PartnerCultivateGift()); }
  public static PartnerCultivateGift GetRootAsPartnerCultivateGift(ByteBuffer _bb, PartnerCultivateGift obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public PartnerCultivateGift __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Title { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTitleBytes() { return __vector_as_arraysegment(6); }
  public string Icon { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(8); }
  public float Discount { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<PartnerCultivateGift> CreatePartnerCultivateGift(FlatBufferBuilder builder,
      int id = 0,
      StringOffset titleOffset = default(StringOffset),
      StringOffset iconOffset = default(StringOffset),
      float discount = 0) {
    builder.StartObject(4);
    PartnerCultivateGift.AddDiscount(builder, discount);
    PartnerCultivateGift.AddIcon(builder, iconOffset);
    PartnerCultivateGift.AddTitle(builder, titleOffset);
    PartnerCultivateGift.AddId(builder, id);
    return PartnerCultivateGift.EndPartnerCultivateGift(builder);
  }

  public static void StartPartnerCultivateGift(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddTitle(FlatBufferBuilder builder, StringOffset titleOffset) { builder.AddOffset(1, titleOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(2, iconOffset.Value, 0); }
  public static void AddDiscount(FlatBufferBuilder builder, float discount) { builder.AddFloat(3, discount, 0); }
  public static Offset<PartnerCultivateGift> EndPartnerCultivateGift(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<PartnerCultivateGift>(o);
  }
};

public sealed class HeroMedal : Table {
  public static HeroMedal GetRootAsHeroMedal(ByteBuffer _bb) { return GetRootAsHeroMedal(_bb, new HeroMedal()); }
  public static HeroMedal GetRootAsHeroMedal(ByteBuffer _bb, HeroMedal obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HeroMedal __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Stage { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Rt1 { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt1Bytes() { return __vector_as_arraysegment(6); }
  public string Ri1 { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi1Bytes() { return __vector_as_arraysegment(8); }
  public int Rn1 { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Rt2 { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt2Bytes() { return __vector_as_arraysegment(12); }
  public string Ri2 { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi2Bytes() { return __vector_as_arraysegment(14); }
  public int Rn2 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Rt3 { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt3Bytes() { return __vector_as_arraysegment(18); }
  public string Ri3 { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi3Bytes() { return __vector_as_arraysegment(20); }
  public int Rn3 { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Ert1 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetErt1Bytes() { return __vector_as_arraysegment(24); }
  public string Eri1 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEri1Bytes() { return __vector_as_arraysegment(26); }
  public int Ern1 { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Ert2 { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetErt2Bytes() { return __vector_as_arraysegment(30); }
  public string Eri2 { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEri2Bytes() { return __vector_as_arraysegment(32); }
  public int Ern2 { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Ert3 { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetErt3Bytes() { return __vector_as_arraysegment(36); }
  public string Eri3 { get { int o = __offset(38); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEri3Bytes() { return __vector_as_arraysegment(38); }
  public int Ern3 { get { int o = __offset(40); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<HeroMedal> CreateHeroMedal(FlatBufferBuilder builder,
      int stage = 0,
      StringOffset rt1Offset = default(StringOffset),
      StringOffset ri1Offset = default(StringOffset),
      int rn1 = 0,
      StringOffset rt2Offset = default(StringOffset),
      StringOffset ri2Offset = default(StringOffset),
      int rn2 = 0,
      StringOffset rt3Offset = default(StringOffset),
      StringOffset ri3Offset = default(StringOffset),
      int rn3 = 0,
      StringOffset ert1Offset = default(StringOffset),
      StringOffset eri1Offset = default(StringOffset),
      int ern1 = 0,
      StringOffset ert2Offset = default(StringOffset),
      StringOffset eri2Offset = default(StringOffset),
      int ern2 = 0,
      StringOffset ert3Offset = default(StringOffset),
      StringOffset eri3Offset = default(StringOffset),
      int ern3 = 0) {
    builder.StartObject(19);
    HeroMedal.AddErn3(builder, ern3);
    HeroMedal.AddEri3(builder, eri3Offset);
    HeroMedal.AddErt3(builder, ert3Offset);
    HeroMedal.AddErn2(builder, ern2);
    HeroMedal.AddEri2(builder, eri2Offset);
    HeroMedal.AddErt2(builder, ert2Offset);
    HeroMedal.AddErn1(builder, ern1);
    HeroMedal.AddEri1(builder, eri1Offset);
    HeroMedal.AddErt1(builder, ert1Offset);
    HeroMedal.AddRn3(builder, rn3);
    HeroMedal.AddRi3(builder, ri3Offset);
    HeroMedal.AddRt3(builder, rt3Offset);
    HeroMedal.AddRn2(builder, rn2);
    HeroMedal.AddRi2(builder, ri2Offset);
    HeroMedal.AddRt2(builder, rt2Offset);
    HeroMedal.AddRn1(builder, rn1);
    HeroMedal.AddRi1(builder, ri1Offset);
    HeroMedal.AddRt1(builder, rt1Offset);
    HeroMedal.AddStage(builder, stage);
    return HeroMedal.EndHeroMedal(builder);
  }

  public static void StartHeroMedal(FlatBufferBuilder builder) { builder.StartObject(19); }
  public static void AddStage(FlatBufferBuilder builder, int stage) { builder.AddInt(0, stage, 0); }
  public static void AddRt1(FlatBufferBuilder builder, StringOffset rt1Offset) { builder.AddOffset(1, rt1Offset.Value, 0); }
  public static void AddRi1(FlatBufferBuilder builder, StringOffset ri1Offset) { builder.AddOffset(2, ri1Offset.Value, 0); }
  public static void AddRn1(FlatBufferBuilder builder, int rn1) { builder.AddInt(3, rn1, 0); }
  public static void AddRt2(FlatBufferBuilder builder, StringOffset rt2Offset) { builder.AddOffset(4, rt2Offset.Value, 0); }
  public static void AddRi2(FlatBufferBuilder builder, StringOffset ri2Offset) { builder.AddOffset(5, ri2Offset.Value, 0); }
  public static void AddRn2(FlatBufferBuilder builder, int rn2) { builder.AddInt(6, rn2, 0); }
  public static void AddRt3(FlatBufferBuilder builder, StringOffset rt3Offset) { builder.AddOffset(7, rt3Offset.Value, 0); }
  public static void AddRi3(FlatBufferBuilder builder, StringOffset ri3Offset) { builder.AddOffset(8, ri3Offset.Value, 0); }
  public static void AddRn3(FlatBufferBuilder builder, int rn3) { builder.AddInt(9, rn3, 0); }
  public static void AddErt1(FlatBufferBuilder builder, StringOffset ert1Offset) { builder.AddOffset(10, ert1Offset.Value, 0); }
  public static void AddEri1(FlatBufferBuilder builder, StringOffset eri1Offset) { builder.AddOffset(11, eri1Offset.Value, 0); }
  public static void AddErn1(FlatBufferBuilder builder, int ern1) { builder.AddInt(12, ern1, 0); }
  public static void AddErt2(FlatBufferBuilder builder, StringOffset ert2Offset) { builder.AddOffset(13, ert2Offset.Value, 0); }
  public static void AddEri2(FlatBufferBuilder builder, StringOffset eri2Offset) { builder.AddOffset(14, eri2Offset.Value, 0); }
  public static void AddErn2(FlatBufferBuilder builder, int ern2) { builder.AddInt(15, ern2, 0); }
  public static void AddErt3(FlatBufferBuilder builder, StringOffset ert3Offset) { builder.AddOffset(16, ert3Offset.Value, 0); }
  public static void AddEri3(FlatBufferBuilder builder, StringOffset eri3Offset) { builder.AddOffset(17, eri3Offset.Value, 0); }
  public static void AddErn3(FlatBufferBuilder builder, int ern3) { builder.AddInt(18, ern3, 0); }
  public static Offset<HeroMedal> EndHeroMedal(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HeroMedal>(o);
  }
};

public sealed class AwakenDungeon : Table {
  public static AwakenDungeon GetRootAsAwakenDungeon(ByteBuffer _bb) { return GetRootAsAwakenDungeon(_bb, new AwakenDungeon()); }
  public static AwakenDungeon GetRootAsAwakenDungeon(ByteBuffer _bb, AwakenDungeon obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AwakenDungeon __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Type { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Stage { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(10); }
  public string CombatLayoutName { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCombatLayoutNameBytes() { return __vector_as_arraysegment(12); }
  public string DropAward { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropAwardBytes() { return __vector_as_arraysegment(14); }
  public string FirstAward { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetFirstAwardBytes() { return __vector_as_arraysegment(16); }

  public static Offset<AwakenDungeon> CreateAwakenDungeon(FlatBufferBuilder builder,
      int id = 0,
      int type = 0,
      int stage = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset combat_layout_nameOffset = default(StringOffset),
      StringOffset drop_awardOffset = default(StringOffset),
      StringOffset first_awardOffset = default(StringOffset)) {
    builder.StartObject(7);
    AwakenDungeon.AddFirstAward(builder, first_awardOffset);
    AwakenDungeon.AddDropAward(builder, drop_awardOffset);
    AwakenDungeon.AddCombatLayoutName(builder, combat_layout_nameOffset);
    AwakenDungeon.AddName(builder, nameOffset);
    AwakenDungeon.AddStage(builder, stage);
    AwakenDungeon.AddType(builder, type);
    AwakenDungeon.AddId(builder, id);
    return AwakenDungeon.EndAwakenDungeon(builder);
  }

  public static void StartAwakenDungeon(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(1, type, 0); }
  public static void AddStage(FlatBufferBuilder builder, int stage) { builder.AddInt(2, stage, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(3, nameOffset.Value, 0); }
  public static void AddCombatLayoutName(FlatBufferBuilder builder, StringOffset combatLayoutNameOffset) { builder.AddOffset(4, combatLayoutNameOffset.Value, 0); }
  public static void AddDropAward(FlatBufferBuilder builder, StringOffset dropAwardOffset) { builder.AddOffset(5, dropAwardOffset.Value, 0); }
  public static void AddFirstAward(FlatBufferBuilder builder, StringOffset firstAwardOffset) { builder.AddOffset(6, firstAwardOffset.Value, 0); }
  public static Offset<AwakenDungeon> EndAwakenDungeon(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AwakenDungeon>(o);
  }
};

public sealed class DiamondGift : Table {
  public static DiamondGift GetRootAsDiamondGift(ByteBuffer _bb) { return GetRootAsDiamondGift(_bb, new DiamondGift()); }
  public static DiamondGift GetRootAsDiamondGift(ByteBuffer _bb, DiamondGift obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DiamondGift __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Days { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(8); }
  public string ItemId { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(10); }
  public int ItemNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int OriginalPrice { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ActivityPrice { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<DiamondGift> CreateDiamondGift(FlatBufferBuilder builder,
      int id = 0,
      int days = 0,
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      int item_num = 0,
      int original_price = 0,
      int activity_price = 0) {
    builder.StartObject(7);
    DiamondGift.AddActivityPrice(builder, activity_price);
    DiamondGift.AddOriginalPrice(builder, original_price);
    DiamondGift.AddItemNum(builder, item_num);
    DiamondGift.AddItemId(builder, item_idOffset);
    DiamondGift.AddItemType(builder, item_typeOffset);
    DiamondGift.AddDays(builder, days);
    DiamondGift.AddId(builder, id);
    return DiamondGift.EndDiamondGift(builder);
  }

  public static void StartDiamondGift(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddDays(FlatBufferBuilder builder, int days) { builder.AddInt(1, days, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(2, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(3, itemIdOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(4, itemNum, 0); }
  public static void AddOriginalPrice(FlatBufferBuilder builder, int originalPrice) { builder.AddInt(5, originalPrice, 0); }
  public static void AddActivityPrice(FlatBufferBuilder builder, int activityPrice) { builder.AddInt(6, activityPrice, 0); }
  public static Offset<DiamondGift> EndDiamondGift(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DiamondGift>(o);
  }
};

public sealed class LevelCompensatedReward : Table {
  public static LevelCompensatedReward GetRootAsLevelCompensatedReward(ByteBuffer _bb) { return GetRootAsLevelCompensatedReward(_bb, new LevelCompensatedReward()); }
  public static LevelCompensatedReward GetRootAsLevelCompensatedReward(ByteBuffer _bb, LevelCompensatedReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LevelCompensatedReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Days { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MinLevel { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MaxLevel { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string RewardContent { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardContentBytes() { return __vector_as_arraysegment(12); }

  public static Offset<LevelCompensatedReward> CreateLevelCompensatedReward(FlatBufferBuilder builder,
      int id = 0,
      int days = 0,
      int min_level = 0,
      int max_level = 0,
      StringOffset reward_contentOffset = default(StringOffset)) {
    builder.StartObject(5);
    LevelCompensatedReward.AddRewardContent(builder, reward_contentOffset);
    LevelCompensatedReward.AddMaxLevel(builder, max_level);
    LevelCompensatedReward.AddMinLevel(builder, min_level);
    LevelCompensatedReward.AddDays(builder, days);
    LevelCompensatedReward.AddId(builder, id);
    return LevelCompensatedReward.EndLevelCompensatedReward(builder);
  }

  public static void StartLevelCompensatedReward(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddDays(FlatBufferBuilder builder, int days) { builder.AddInt(1, days, 0); }
  public static void AddMinLevel(FlatBufferBuilder builder, int minLevel) { builder.AddInt(2, minLevel, 0); }
  public static void AddMaxLevel(FlatBufferBuilder builder, int maxLevel) { builder.AddInt(3, maxLevel, 0); }
  public static void AddRewardContent(FlatBufferBuilder builder, StringOffset rewardContentOffset) { builder.AddOffset(4, rewardContentOffset.Value, 0); }
  public static Offset<LevelCompensatedReward> EndLevelCompensatedReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LevelCompensatedReward>(o);
  }
};

public sealed class MainCampaignReward : Table {
  public static MainCampaignReward GetRootAsMainCampaignReward(ByteBuffer _bb) { return GetRootAsMainCampaignReward(_bb, new MainCampaignReward()); }
  public static MainCampaignReward GetRootAsMainCampaignReward(ByteBuffer _bb, MainCampaignReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public MainCampaignReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Stage { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetStageBytes() { return __vector_as_arraysegment(4); }
  public string Rt1 { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt1Bytes() { return __vector_as_arraysegment(6); }
  public string Ri1 { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi1Bytes() { return __vector_as_arraysegment(8); }
  public int Rn1 { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Rt2 { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt2Bytes() { return __vector_as_arraysegment(12); }
  public string Ri2 { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi2Bytes() { return __vector_as_arraysegment(14); }
  public int Rn2 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Rt3 { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt3Bytes() { return __vector_as_arraysegment(18); }
  public string Ri3 { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi3Bytes() { return __vector_as_arraysegment(20); }
  public int Rn3 { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Ert1 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetErt1Bytes() { return __vector_as_arraysegment(24); }
  public string Eri1 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEri1Bytes() { return __vector_as_arraysegment(26); }
  public int Ern1 { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Ert2 { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetErt2Bytes() { return __vector_as_arraysegment(30); }
  public string Eri2 { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEri2Bytes() { return __vector_as_arraysegment(32); }
  public int Ern2 { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Ert3 { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetErt3Bytes() { return __vector_as_arraysegment(36); }
  public string Eri3 { get { int o = __offset(38); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEri3Bytes() { return __vector_as_arraysegment(38); }
  public int Ern3 { get { int o = __offset(40); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<MainCampaignReward> CreateMainCampaignReward(FlatBufferBuilder builder,
      StringOffset stageOffset = default(StringOffset),
      StringOffset rt1Offset = default(StringOffset),
      StringOffset ri1Offset = default(StringOffset),
      int rn1 = 0,
      StringOffset rt2Offset = default(StringOffset),
      StringOffset ri2Offset = default(StringOffset),
      int rn2 = 0,
      StringOffset rt3Offset = default(StringOffset),
      StringOffset ri3Offset = default(StringOffset),
      int rn3 = 0,
      StringOffset ert1Offset = default(StringOffset),
      StringOffset eri1Offset = default(StringOffset),
      int ern1 = 0,
      StringOffset ert2Offset = default(StringOffset),
      StringOffset eri2Offset = default(StringOffset),
      int ern2 = 0,
      StringOffset ert3Offset = default(StringOffset),
      StringOffset eri3Offset = default(StringOffset),
      int ern3 = 0) {
    builder.StartObject(19);
    MainCampaignReward.AddErn3(builder, ern3);
    MainCampaignReward.AddEri3(builder, eri3Offset);
    MainCampaignReward.AddErt3(builder, ert3Offset);
    MainCampaignReward.AddErn2(builder, ern2);
    MainCampaignReward.AddEri2(builder, eri2Offset);
    MainCampaignReward.AddErt2(builder, ert2Offset);
    MainCampaignReward.AddErn1(builder, ern1);
    MainCampaignReward.AddEri1(builder, eri1Offset);
    MainCampaignReward.AddErt1(builder, ert1Offset);
    MainCampaignReward.AddRn3(builder, rn3);
    MainCampaignReward.AddRi3(builder, ri3Offset);
    MainCampaignReward.AddRt3(builder, rt3Offset);
    MainCampaignReward.AddRn2(builder, rn2);
    MainCampaignReward.AddRi2(builder, ri2Offset);
    MainCampaignReward.AddRt2(builder, rt2Offset);
    MainCampaignReward.AddRn1(builder, rn1);
    MainCampaignReward.AddRi1(builder, ri1Offset);
    MainCampaignReward.AddRt1(builder, rt1Offset);
    MainCampaignReward.AddStage(builder, stageOffset);
    return MainCampaignReward.EndMainCampaignReward(builder);
  }

  public static void StartMainCampaignReward(FlatBufferBuilder builder) { builder.StartObject(19); }
  public static void AddStage(FlatBufferBuilder builder, StringOffset stageOffset) { builder.AddOffset(0, stageOffset.Value, 0); }
  public static void AddRt1(FlatBufferBuilder builder, StringOffset rt1Offset) { builder.AddOffset(1, rt1Offset.Value, 0); }
  public static void AddRi1(FlatBufferBuilder builder, StringOffset ri1Offset) { builder.AddOffset(2, ri1Offset.Value, 0); }
  public static void AddRn1(FlatBufferBuilder builder, int rn1) { builder.AddInt(3, rn1, 0); }
  public static void AddRt2(FlatBufferBuilder builder, StringOffset rt2Offset) { builder.AddOffset(4, rt2Offset.Value, 0); }
  public static void AddRi2(FlatBufferBuilder builder, StringOffset ri2Offset) { builder.AddOffset(5, ri2Offset.Value, 0); }
  public static void AddRn2(FlatBufferBuilder builder, int rn2) { builder.AddInt(6, rn2, 0); }
  public static void AddRt3(FlatBufferBuilder builder, StringOffset rt3Offset) { builder.AddOffset(7, rt3Offset.Value, 0); }
  public static void AddRi3(FlatBufferBuilder builder, StringOffset ri3Offset) { builder.AddOffset(8, ri3Offset.Value, 0); }
  public static void AddRn3(FlatBufferBuilder builder, int rn3) { builder.AddInt(9, rn3, 0); }
  public static void AddErt1(FlatBufferBuilder builder, StringOffset ert1Offset) { builder.AddOffset(10, ert1Offset.Value, 0); }
  public static void AddEri1(FlatBufferBuilder builder, StringOffset eri1Offset) { builder.AddOffset(11, eri1Offset.Value, 0); }
  public static void AddErn1(FlatBufferBuilder builder, int ern1) { builder.AddInt(12, ern1, 0); }
  public static void AddErt2(FlatBufferBuilder builder, StringOffset ert2Offset) { builder.AddOffset(13, ert2Offset.Value, 0); }
  public static void AddEri2(FlatBufferBuilder builder, StringOffset eri2Offset) { builder.AddOffset(14, eri2Offset.Value, 0); }
  public static void AddErn2(FlatBufferBuilder builder, int ern2) { builder.AddInt(15, ern2, 0); }
  public static void AddErt3(FlatBufferBuilder builder, StringOffset ert3Offset) { builder.AddOffset(16, ert3Offset.Value, 0); }
  public static void AddEri3(FlatBufferBuilder builder, StringOffset eri3Offset) { builder.AddOffset(17, eri3Offset.Value, 0); }
  public static void AddErn3(FlatBufferBuilder builder, int ern3) { builder.AddInt(18, ern3, 0); }
  public static Offset<MainCampaignReward> EndMainCampaignReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MainCampaignReward>(o);
  }
};

public sealed class InfiniteCompete : Table {
  public static InfiniteCompete GetRootAsInfiniteCompete(ByteBuffer _bb) { return GetRootAsInfiniteCompete(_bb, new InfiniteCompete()); }
  public static InfiniteCompete GetRootAsInfiniteCompete(ByteBuffer _bb, InfiniteCompete obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public InfiniteCompete __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string FirstAward { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetFirstAwardBytes() { return __vector_as_arraysegment(6); }

  public static Offset<InfiniteCompete> CreateInfiniteCompete(FlatBufferBuilder builder,
      int id = 0,
      StringOffset first_awardOffset = default(StringOffset)) {
    builder.StartObject(2);
    InfiniteCompete.AddFirstAward(builder, first_awardOffset);
    InfiniteCompete.AddId(builder, id);
    return InfiniteCompete.EndInfiniteCompete(builder);
  }

  public static void StartInfiniteCompete(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddFirstAward(FlatBufferBuilder builder, StringOffset firstAwardOffset) { builder.AddOffset(1, firstAwardOffset.Value, 0); }
  public static Offset<InfiniteCompete> EndInfiniteCompete(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<InfiniteCompete>(o);
  }
};

public sealed class InfiniteCompeteReward : Table {
  public static InfiniteCompeteReward GetRootAsInfiniteCompeteReward(ByteBuffer _bb) { return GetRootAsInfiniteCompeteReward(_bb, new InfiniteCompeteReward()); }
  public static InfiniteCompeteReward GetRootAsInfiniteCompeteReward(ByteBuffer _bb, InfiniteCompeteReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public InfiniteCompeteReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Ranks { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRanksBytes() { return __vector_as_arraysegment(6); }
  public string Reward { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBytes() { return __vector_as_arraysegment(8); }

  public static Offset<InfiniteCompeteReward> CreateInfiniteCompeteReward(FlatBufferBuilder builder,
      int id = 0,
      StringOffset ranksOffset = default(StringOffset),
      StringOffset rewardOffset = default(StringOffset)) {
    builder.StartObject(3);
    InfiniteCompeteReward.AddReward(builder, rewardOffset);
    InfiniteCompeteReward.AddRanks(builder, ranksOffset);
    InfiniteCompeteReward.AddId(builder, id);
    return InfiniteCompeteReward.EndInfiniteCompeteReward(builder);
  }

  public static void StartInfiniteCompeteReward(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddRanks(FlatBufferBuilder builder, StringOffset ranksOffset) { builder.AddOffset(1, ranksOffset.Value, 0); }
  public static void AddReward(FlatBufferBuilder builder, StringOffset rewardOffset) { builder.AddOffset(2, rewardOffset.Value, 0); }
  public static Offset<InfiniteCompeteReward> EndInfiniteCompeteReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<InfiniteCompeteReward>(o);
  }
};

public sealed class UrEventReward : Table {
  public static UrEventReward GetRootAsUrEventReward(ByteBuffer _bb) { return GetRootAsUrEventReward(_bb, new UrEventReward()); }
  public static UrEventReward GetRootAsUrEventReward(ByteBuffer _bb, UrEventReward obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public UrEventReward __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Ranks { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRanksBytes() { return __vector_as_arraysegment(6); }
  public int Type { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Reward { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardBytes() { return __vector_as_arraysegment(10); }

  public static Offset<UrEventReward> CreateUrEventReward(FlatBufferBuilder builder,
      int id = 0,
      StringOffset ranksOffset = default(StringOffset),
      int type = 0,
      StringOffset rewardOffset = default(StringOffset)) {
    builder.StartObject(4);
    UrEventReward.AddReward(builder, rewardOffset);
    UrEventReward.AddType(builder, type);
    UrEventReward.AddRanks(builder, ranksOffset);
    UrEventReward.AddId(builder, id);
    return UrEventReward.EndUrEventReward(builder);
  }

  public static void StartUrEventReward(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddRanks(FlatBufferBuilder builder, StringOffset ranksOffset) { builder.AddOffset(1, ranksOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(2, type, 0); }
  public static void AddReward(FlatBufferBuilder builder, StringOffset rewardOffset) { builder.AddOffset(3, rewardOffset.Value, 0); }
  public static Offset<UrEventReward> EndUrEventReward(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<UrEventReward>(o);
  }
};

public sealed class HeroBattle : Table {
  public static HeroBattle GetRootAsHeroBattle(ByteBuffer _bb) { return GetRootAsHeroBattle(_bb, new HeroBattle()); }
  public static HeroBattle GetRootAsHeroBattle(ByteBuffer _bb, HeroBattle obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HeroBattle __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Type { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Stage { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(10); }
  public string OurHeroConfig { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetOurHeroConfigBytes() { return __vector_as_arraysegment(12); }
  public string EnemyHeroConfig { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEnemyHeroConfigBytes() { return __vector_as_arraysegment(14); }
  public string Award { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAwardBytes() { return __vector_as_arraysegment(16); }
  public int CostLimit { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Condition { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Desc { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(22); }

  public static Offset<HeroBattle> CreateHeroBattle(FlatBufferBuilder builder,
      int id = 0,
      int type = 0,
      int stage = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset our_hero_configOffset = default(StringOffset),
      StringOffset enemy_hero_configOffset = default(StringOffset),
      StringOffset awardOffset = default(StringOffset),
      int cost_limit = 0,
      int condition = 0,
      StringOffset descOffset = default(StringOffset)) {
    builder.StartObject(10);
    HeroBattle.AddDesc(builder, descOffset);
    HeroBattle.AddCondition(builder, condition);
    HeroBattle.AddCostLimit(builder, cost_limit);
    HeroBattle.AddAward(builder, awardOffset);
    HeroBattle.AddEnemyHeroConfig(builder, enemy_hero_configOffset);
    HeroBattle.AddOurHeroConfig(builder, our_hero_configOffset);
    HeroBattle.AddName(builder, nameOffset);
    HeroBattle.AddStage(builder, stage);
    HeroBattle.AddType(builder, type);
    HeroBattle.AddId(builder, id);
    return HeroBattle.EndHeroBattle(builder);
  }

  public static void StartHeroBattle(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(1, type, 0); }
  public static void AddStage(FlatBufferBuilder builder, int stage) { builder.AddInt(2, stage, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(3, nameOffset.Value, 0); }
  public static void AddOurHeroConfig(FlatBufferBuilder builder, StringOffset ourHeroConfigOffset) { builder.AddOffset(4, ourHeroConfigOffset.Value, 0); }
  public static void AddEnemyHeroConfig(FlatBufferBuilder builder, StringOffset enemyHeroConfigOffset) { builder.AddOffset(5, enemyHeroConfigOffset.Value, 0); }
  public static void AddAward(FlatBufferBuilder builder, StringOffset awardOffset) { builder.AddOffset(6, awardOffset.Value, 0); }
  public static void AddCostLimit(FlatBufferBuilder builder, int costLimit) { builder.AddInt(7, costLimit, 0); }
  public static void AddCondition(FlatBufferBuilder builder, int condition) { builder.AddInt(8, condition, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(9, descOffset.Value, 0); }
  public static Offset<HeroBattle> EndHeroBattle(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HeroBattle>(o);
  }
};

public sealed class BossChallengeActivityConfig : Table {
  public static BossChallengeActivityConfig GetRootAsBossChallengeActivityConfig(ByteBuffer _bb) { return GetRootAsBossChallengeActivityConfig(_bb, new BossChallengeActivityConfig()); }
  public static BossChallengeActivityConfig GetRootAsBossChallengeActivityConfig(ByteBuffer _bb, BossChallengeActivityConfig obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BossChallengeActivityConfig __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ActivityId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(8); }
  public string Layout { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLayoutBytes() { return __vector_as_arraysegment(10); }
  public int Level { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Difficulty { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Vigor { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PartnerAttr { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BonusAdd { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string GetRewardBox(int j) { int o = __offset(22); return o != 0 ? __string(__vector(o) + j * 4) : null; }
  public int RewardBoxLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<BossChallengeActivityConfig> CreateBossChallengeActivityConfig(FlatBufferBuilder builder,
      int id = 0,
      int activity_id = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset layoutOffset = default(StringOffset),
      int level = 0,
      int difficulty = 0,
      int vigor = 0,
      int partner_attr = 0,
      int bonus_add = 0,
      VectorOffset reward_boxOffset = default(VectorOffset)) {
    builder.StartObject(10);
    BossChallengeActivityConfig.AddRewardBox(builder, reward_boxOffset);
    BossChallengeActivityConfig.AddBonusAdd(builder, bonus_add);
    BossChallengeActivityConfig.AddPartnerAttr(builder, partner_attr);
    BossChallengeActivityConfig.AddVigor(builder, vigor);
    BossChallengeActivityConfig.AddDifficulty(builder, difficulty);
    BossChallengeActivityConfig.AddLevel(builder, level);
    BossChallengeActivityConfig.AddLayout(builder, layoutOffset);
    BossChallengeActivityConfig.AddName(builder, nameOffset);
    BossChallengeActivityConfig.AddActivityId(builder, activity_id);
    BossChallengeActivityConfig.AddId(builder, id);
    return BossChallengeActivityConfig.EndBossChallengeActivityConfig(builder);
  }

  public static void StartBossChallengeActivityConfig(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddActivityId(FlatBufferBuilder builder, int activityId) { builder.AddInt(1, activityId, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(2, nameOffset.Value, 0); }
  public static void AddLayout(FlatBufferBuilder builder, StringOffset layoutOffset) { builder.AddOffset(3, layoutOffset.Value, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(4, level, 0); }
  public static void AddDifficulty(FlatBufferBuilder builder, int difficulty) { builder.AddInt(5, difficulty, 0); }
  public static void AddVigor(FlatBufferBuilder builder, int vigor) { builder.AddInt(6, vigor, 0); }
  public static void AddPartnerAttr(FlatBufferBuilder builder, int partnerAttr) { builder.AddInt(7, partnerAttr, 0); }
  public static void AddBonusAdd(FlatBufferBuilder builder, int bonusAdd) { builder.AddInt(8, bonusAdd, 0); }
  public static void AddRewardBox(FlatBufferBuilder builder, VectorOffset rewardBoxOffset) { builder.AddOffset(9, rewardBoxOffset.Value, 0); }
  public static VectorOffset CreateRewardBoxVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartRewardBoxVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<BossChallengeActivityConfig> EndBossChallengeActivityConfig(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BossChallengeActivityConfig>(o);
  }
};

public sealed class BuddyRun : Table {
  public static BuddyRun GetRootAsBuddyRun(ByteBuffer _bb) { return GetRootAsBuddyRun(_bb, new BuddyRun()); }
  public static BuddyRun GetRootAsBuddyRun(ByteBuffer _bb, BuddyRun obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BuddyRun __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string GuessStartTime { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetGuessStartTimeBytes() { return __vector_as_arraysegment(8); }
  public string RunStartTime { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRunStartTimeBytes() { return __vector_as_arraysegment(10); }
  public string EndTime { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetEndTimeBytes() { return __vector_as_arraysegment(12); }
  public int Player1 { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Player2 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Player3 { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BetAmount { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBetAmountBytes() { return __vector_as_arraysegment(20); }
  public int BetMaxTime { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float PrizePercent { get { int o = __offset(24); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<BuddyRun> CreateBuddyRun(FlatBufferBuilder builder,
      int id = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset guess_start_timeOffset = default(StringOffset),
      StringOffset run_start_timeOffset = default(StringOffset),
      StringOffset end_timeOffset = default(StringOffset),
      int player1 = 0,
      int player2 = 0,
      int player3 = 0,
      StringOffset bet_amountOffset = default(StringOffset),
      int bet_max_time = 0,
      float prize_percent = 0) {
    builder.StartObject(11);
    BuddyRun.AddPrizePercent(builder, prize_percent);
    BuddyRun.AddBetMaxTime(builder, bet_max_time);
    BuddyRun.AddBetAmount(builder, bet_amountOffset);
    BuddyRun.AddPlayer3(builder, player3);
    BuddyRun.AddPlayer2(builder, player2);
    BuddyRun.AddPlayer1(builder, player1);
    BuddyRun.AddEndTime(builder, end_timeOffset);
    BuddyRun.AddRunStartTime(builder, run_start_timeOffset);
    BuddyRun.AddGuessStartTime(builder, guess_start_timeOffset);
    BuddyRun.AddName(builder, nameOffset);
    BuddyRun.AddId(builder, id);
    return BuddyRun.EndBuddyRun(builder);
  }

  public static void StartBuddyRun(FlatBufferBuilder builder) { builder.StartObject(11); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddGuessStartTime(FlatBufferBuilder builder, StringOffset guessStartTimeOffset) { builder.AddOffset(2, guessStartTimeOffset.Value, 0); }
  public static void AddRunStartTime(FlatBufferBuilder builder, StringOffset runStartTimeOffset) { builder.AddOffset(3, runStartTimeOffset.Value, 0); }
  public static void AddEndTime(FlatBufferBuilder builder, StringOffset endTimeOffset) { builder.AddOffset(4, endTimeOffset.Value, 0); }
  public static void AddPlayer1(FlatBufferBuilder builder, int player1) { builder.AddInt(5, player1, 0); }
  public static void AddPlayer2(FlatBufferBuilder builder, int player2) { builder.AddInt(6, player2, 0); }
  public static void AddPlayer3(FlatBufferBuilder builder, int player3) { builder.AddInt(7, player3, 0); }
  public static void AddBetAmount(FlatBufferBuilder builder, StringOffset betAmountOffset) { builder.AddOffset(8, betAmountOffset.Value, 0); }
  public static void AddBetMaxTime(FlatBufferBuilder builder, int betMaxTime) { builder.AddInt(9, betMaxTime, 0); }
  public static void AddPrizePercent(FlatBufferBuilder builder, float prizePercent) { builder.AddFloat(10, prizePercent, 0); }
  public static Offset<BuddyRun> EndBuddyRun(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BuddyRun>(o);
  }
};

public sealed class BossChallengeActivityTotalTime : Table {
  public static BossChallengeActivityTotalTime GetRootAsBossChallengeActivityTotalTime(ByteBuffer _bb) { return GetRootAsBossChallengeActivityTotalTime(_bb, new BossChallengeActivityTotalTime()); }
  public static BossChallengeActivityTotalTime GetRootAsBossChallengeActivityTotalTime(ByteBuffer _bb, BossChallengeActivityTotalTime obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BossChallengeActivityTotalTime __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ActivityId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(8); }
  public int BattleTime { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string RewardItem { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRewardItemBytes() { return __vector_as_arraysegment(12); }

  public static Offset<BossChallengeActivityTotalTime> CreateBossChallengeActivityTotalTime(FlatBufferBuilder builder,
      int id = 0,
      int activity_id = 0,
      StringOffset nameOffset = default(StringOffset),
      int battle_time = 0,
      StringOffset reward_itemOffset = default(StringOffset)) {
    builder.StartObject(5);
    BossChallengeActivityTotalTime.AddRewardItem(builder, reward_itemOffset);
    BossChallengeActivityTotalTime.AddBattleTime(builder, battle_time);
    BossChallengeActivityTotalTime.AddName(builder, nameOffset);
    BossChallengeActivityTotalTime.AddActivityId(builder, activity_id);
    BossChallengeActivityTotalTime.AddId(builder, id);
    return BossChallengeActivityTotalTime.EndBossChallengeActivityTotalTime(builder);
  }

  public static void StartBossChallengeActivityTotalTime(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddActivityId(FlatBufferBuilder builder, int activityId) { builder.AddInt(1, activityId, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(2, nameOffset.Value, 0); }
  public static void AddBattleTime(FlatBufferBuilder builder, int battleTime) { builder.AddInt(3, battleTime, 0); }
  public static void AddRewardItem(FlatBufferBuilder builder, StringOffset rewardItemOffset) { builder.AddOffset(4, rewardItemOffset.Value, 0); }
  public static Offset<BossChallengeActivityTotalTime> EndBossChallengeActivityTotalTime(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BossChallengeActivityTotalTime>(o);
  }
};

public sealed class ConditionEvent : Table {
  public static ConditionEvent GetRootAsConditionEvent(ByteBuffer _bb) { return GetRootAsConditionEvent(_bb, new ConditionEvent()); }
  public static ConditionEvent GetRootAsConditionEvent(ByteBuffer _bb, ConditionEvent obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionEvent __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

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
  public RobotInfo GetRobots(int j) { return GetRobots(new RobotInfo(), j); }
  public RobotInfo GetRobots(RobotInfo obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int RobotsLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public ExpeditionMapConfig GetExpeditionMapConfig(int j) { return GetExpeditionMapConfig(new ExpeditionMapConfig(), j); }
  public ExpeditionMapConfig GetExpeditionMapConfig(ExpeditionMapConfig obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ExpeditionMapConfigLength { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public ArenaAward GetArenaAward(int j) { return GetArenaAward(new ArenaAward(), j); }
  public ArenaAward GetArenaAward(ArenaAward obj, int j) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArenaAwardLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }
  public SignInReward GetSignInReward(int j) { return GetSignInReward(new SignInReward(), j); }
  public SignInReward GetSignInReward(SignInReward obj, int j) { int o = __offset(24); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SignInRewardLength { get { int o = __offset(24); return o != 0 ? __vector_len(o) : 0; } }
  public DailyReward GetDailyReward(int j) { return GetDailyReward(new DailyReward(), j); }
  public DailyReward GetDailyReward(DailyReward obj, int j) { int o = __offset(26); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int DailyRewardLength { get { int o = __offset(26); return o != 0 ? __vector_len(o) : 0; } }
  public SpecialActivity GetSpecialActivity(int j) { return GetSpecialActivity(new SpecialActivity(), j); }
  public SpecialActivity GetSpecialActivity(SpecialActivity obj, int j) { int o = __offset(28); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SpecialActivityLength { get { int o = __offset(28); return o != 0 ? __vector_len(o) : 0; } }
  public SpecialActivityLevel GetSpecialActivityLevel(int j) { return GetSpecialActivityLevel(new SpecialActivityLevel(), j); }
  public SpecialActivityLevel GetSpecialActivityLevel(SpecialActivityLevel obj, int j) { int o = __offset(30); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SpecialActivityLevelLength { get { int o = __offset(30); return o != 0 ? __vector_len(o) : 0; } }
  public NormalActivityInstance GetNormalActivityInstance(int j) { return GetNormalActivityInstance(new NormalActivityInstance(), j); }
  public NormalActivityInstance GetNormalActivityInstance(NormalActivityInstance obj, int j) { int o = __offset(32); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int NormalActivityInstanceLength { get { int o = __offset(32); return o != 0 ? __vector_len(o) : 0; } }
  public TimeLimitActivity GetTimeLimitActivity(int j) { return GetTimeLimitActivity(new TimeLimitActivity(), j); }
  public TimeLimitActivity GetTimeLimitActivity(TimeLimitActivity obj, int j) { int o = __offset(34); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TimeLimitActivityLength { get { int o = __offset(34); return o != 0 ? __vector_len(o) : 0; } }
  public TimeLimitActivityStage GetTimeLimitActivityStage(int j) { return GetTimeLimitActivityStage(new TimeLimitActivityStage(), j); }
  public TimeLimitActivityStage GetTimeLimitActivityStage(TimeLimitActivityStage obj, int j) { int o = __offset(36); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TimeLimitActivityStageLength { get { int o = __offset(36); return o != 0 ? __vector_len(o) : 0; } }
  public InfiniteChallenge GetInfinitechallenge(int j) { return GetInfinitechallenge(new InfiniteChallenge(), j); }
  public InfiniteChallenge GetInfinitechallenge(InfiniteChallenge obj, int j) { int o = __offset(38); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int InfinitechallengeLength { get { int o = __offset(38); return o != 0 ? __vector_len(o) : 0; } }
  public Ladder GetLadder(int j) { return GetLadder(new Ladder(), j); }
  public Ladder GetLadder(Ladder obj, int j) { int o = __offset(40); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LadderLength { get { int o = __offset(40); return o != 0 ? __vector_len(o) : 0; } }
  public LadderSeasonAward GetLadderSeasonAward(int j) { return GetLadderSeasonAward(new LadderSeasonAward(), j); }
  public LadderSeasonAward GetLadderSeasonAward(LadderSeasonAward obj, int j) { int o = __offset(42); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LadderSeasonAwardLength { get { int o = __offset(42); return o != 0 ? __vector_len(o) : 0; } }
  public BossReward GetBossreward(int j) { return GetBossreward(new BossReward(), j); }
  public BossReward GetBossreward(BossReward obj, int j) { int o = __offset(44); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BossrewardLength { get { int o = __offset(44); return o != 0 ? __vector_len(o) : 0; } }
  public ClashOfHeroesReward GetClashOfHeroesReward(int j) { return GetClashOfHeroesReward(new ClashOfHeroesReward(), j); }
  public ClashOfHeroesReward GetClashOfHeroesReward(ClashOfHeroesReward obj, int j) { int o = __offset(46); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ClashOfHeroesRewardLength { get { int o = __offset(46); return o != 0 ? __vector_len(o) : 0; } }
  public NationRatingReward GetNationRatingReward(int j) { return GetNationRatingReward(new NationRatingReward(), j); }
  public NationRatingReward GetNationRatingReward(NationRatingReward obj, int j) { int o = __offset(48); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int NationRatingRewardLength { get { int o = __offset(48); return o != 0 ? __vector_len(o) : 0; } }
  public NationRankReward GetNationRankReward(int j) { return GetNationRankReward(new NationRankReward(), j); }
  public NationRankReward GetNationRankReward(NationRankReward obj, int j) { int o = __offset(50); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int NationRankRewardLength { get { int o = __offset(50); return o != 0 ? __vector_len(o) : 0; } }
  public NationSpecialEvent GetNationSpecialEvent(int j) { return GetNationSpecialEvent(new NationSpecialEvent(), j); }
  public NationSpecialEvent GetNationSpecialEvent(NationSpecialEvent obj, int j) { int o = __offset(52); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int NationSpecialEventLength { get { int o = __offset(52); return o != 0 ? __vector_len(o) : 0; } }
  public InfiniteBuff GetInfiniteBuff(int j) { return GetInfiniteBuff(new InfiniteBuff(), j); }
  public InfiniteBuff GetInfiniteBuff(InfiniteBuff obj, int j) { int o = __offset(54); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int InfiniteBuffLength { get { int o = __offset(54); return o != 0 ? __vector_len(o) : 0; } }
  public CronJobs GetCronjobs(int j) { return GetCronjobs(new CronJobs(), j); }
  public CronJobs GetCronjobs(CronJobs obj, int j) { int o = __offset(56); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int CronjobsLength { get { int o = __offset(56); return o != 0 ? __vector_len(o) : 0; } }
  public PartnerCultivateGift GetPartnerCultivateGift(int j) { return GetPartnerCultivateGift(new PartnerCultivateGift(), j); }
  public PartnerCultivateGift GetPartnerCultivateGift(PartnerCultivateGift obj, int j) { int o = __offset(58); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int PartnerCultivateGiftLength { get { int o = __offset(58); return o != 0 ? __vector_len(o) : 0; } }
  public HeroMedal GetHeroMedal(int j) { return GetHeroMedal(new HeroMedal(), j); }
  public HeroMedal GetHeroMedal(HeroMedal obj, int j) { int o = __offset(60); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int HeroMedalLength { get { int o = __offset(60); return o != 0 ? __vector_len(o) : 0; } }
  public AwakenDungeon GetAwakenDungeon(int j) { return GetAwakenDungeon(new AwakenDungeon(), j); }
  public AwakenDungeon GetAwakenDungeon(AwakenDungeon obj, int j) { int o = __offset(62); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AwakenDungeonLength { get { int o = __offset(62); return o != 0 ? __vector_len(o) : 0; } }
  public SleepTower GetSleepTower(int j) { return GetSleepTower(new SleepTower(), j); }
  public SleepTower GetSleepTower(SleepTower obj, int j) { int o = __offset(64); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SleepTowerLength { get { int o = __offset(64); return o != 0 ? __vector_len(o) : 0; } }
  public DiamondGift GetDiamondGift(int j) { return GetDiamondGift(new DiamondGift(), j); }
  public DiamondGift GetDiamondGift(DiamondGift obj, int j) { int o = __offset(66); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int DiamondGiftLength { get { int o = __offset(66); return o != 0 ? __vector_len(o) : 0; } }
  public LevelCompensatedReward GetLevelCompensatedReward(int j) { return GetLevelCompensatedReward(new LevelCompensatedReward(), j); }
  public LevelCompensatedReward GetLevelCompensatedReward(LevelCompensatedReward obj, int j) { int o = __offset(68); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LevelCompensatedRewardLength { get { int o = __offset(68); return o != 0 ? __vector_len(o) : 0; } }
  public MainCampaignReward GetMainCampaignReward(int j) { return GetMainCampaignReward(new MainCampaignReward(), j); }
  public MainCampaignReward GetMainCampaignReward(MainCampaignReward obj, int j) { int o = __offset(70); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MainCampaignRewardLength { get { int o = __offset(70); return o != 0 ? __vector_len(o) : 0; } }
  public UrEventReward GetUrEventReward(int j) { return GetUrEventReward(new UrEventReward(), j); }
  public UrEventReward GetUrEventReward(UrEventReward obj, int j) { int o = __offset(72); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int UrEventRewardLength { get { int o = __offset(72); return o != 0 ? __vector_len(o) : 0; } }
  public HonorArenaReward GetHonorArenaReward(int j) { return GetHonorArenaReward(new HonorArenaReward(), j); }
  public HonorArenaReward GetHonorArenaReward(HonorArenaReward obj, int j) { int o = __offset(74); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int HonorArenaRewardLength { get { int o = __offset(74); return o != 0 ? __vector_len(o) : 0; } }
  public InfiniteCompete GetInfiniteCompete(int j) { return GetInfiniteCompete(new InfiniteCompete(), j); }
  public InfiniteCompete GetInfiniteCompete(InfiniteCompete obj, int j) { int o = __offset(76); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int InfiniteCompeteLength { get { int o = __offset(76); return o != 0 ? __vector_len(o) : 0; } }
  public InfiniteCompeteReward GetInfiniteCompeteReward(int j) { return GetInfiniteCompeteReward(new InfiniteCompeteReward(), j); }
  public InfiniteCompeteReward GetInfiniteCompeteReward(InfiniteCompeteReward obj, int j) { int o = __offset(78); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int InfiniteCompeteRewardLength { get { int o = __offset(78); return o != 0 ? __vector_len(o) : 0; } }
  public HeroBattle GetHeroBattle(int j) { return GetHeroBattle(new HeroBattle(), j); }
  public HeroBattle GetHeroBattle(HeroBattle obj, int j) { int o = __offset(80); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int HeroBattleLength { get { int o = __offset(80); return o != 0 ? __vector_len(o) : 0; } }
  public BossChallengeActivityConfig GetBossChallengeActivityConfig(int j) { return GetBossChallengeActivityConfig(new BossChallengeActivityConfig(), j); }
  public BossChallengeActivityConfig GetBossChallengeActivityConfig(BossChallengeActivityConfig obj, int j) { int o = __offset(82); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BossChallengeActivityConfigLength { get { int o = __offset(82); return o != 0 ? __vector_len(o) : 0; } }
  public SleepTowerReward GetSleepTowerReward(int j) { return GetSleepTowerReward(new SleepTowerReward(), j); }
  public SleepTowerReward GetSleepTowerReward(SleepTowerReward obj, int j) { int o = __offset(84); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SleepTowerRewardLength { get { int o = __offset(84); return o != 0 ? __vector_len(o) : 0; } }
  public BuddyRun GetBuddyRun(int j) { return GetBuddyRun(new BuddyRun(), j); }
  public BuddyRun GetBuddyRun(BuddyRun obj, int j) { int o = __offset(86); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BuddyRunLength { get { int o = __offset(86); return o != 0 ? __vector_len(o) : 0; } }
  public BossChallengeActivityTotalTime GetBossChallengeActivityTotalTime(int j) { return GetBossChallengeActivityTotalTime(new BossChallengeActivityTotalTime(), j); }
  public BossChallengeActivityTotalTime GetBossChallengeActivityTotalTime(BossChallengeActivityTotalTime obj, int j) { int o = __offset(88); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BossChallengeActivityTotalTimeLength { get { int o = __offset(88); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionEvent> CreateConditionEvent(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = false,
      int priority = 0,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset robotsOffset = default(VectorOffset),
      VectorOffset expedition_map_configOffset = default(VectorOffset),
      VectorOffset arena_awardOffset = default(VectorOffset),
      VectorOffset sign_in_rewardOffset = default(VectorOffset),
      VectorOffset daily_rewardOffset = default(VectorOffset),
      VectorOffset special_activityOffset = default(VectorOffset),
      VectorOffset special_activity_levelOffset = default(VectorOffset),
      VectorOffset normal_activity_instanceOffset = default(VectorOffset),
      VectorOffset time_limit_activityOffset = default(VectorOffset),
      VectorOffset time_limit_activity_stageOffset = default(VectorOffset),
      VectorOffset infinitechallengeOffset = default(VectorOffset),
      VectorOffset ladderOffset = default(VectorOffset),
      VectorOffset ladder_season_awardOffset = default(VectorOffset),
      VectorOffset bossrewardOffset = default(VectorOffset),
      VectorOffset clash_of_heroes_rewardOffset = default(VectorOffset),
      VectorOffset nation_rating_rewardOffset = default(VectorOffset),
      VectorOffset nation_rank_rewardOffset = default(VectorOffset),
      VectorOffset nation_special_eventOffset = default(VectorOffset),
      VectorOffset infinite_buffOffset = default(VectorOffset),
      VectorOffset cronjobsOffset = default(VectorOffset),
      VectorOffset partner_cultivate_giftOffset = default(VectorOffset),
      VectorOffset hero_medalOffset = default(VectorOffset),
      VectorOffset awaken_dungeonOffset = default(VectorOffset),
      VectorOffset sleep_towerOffset = default(VectorOffset),
      VectorOffset diamond_giftOffset = default(VectorOffset),
      VectorOffset level_compensated_rewardOffset = default(VectorOffset),
      VectorOffset main_campaign_rewardOffset = default(VectorOffset),
      VectorOffset ur_event_rewardOffset = default(VectorOffset),
      VectorOffset honor_arena_rewardOffset = default(VectorOffset),
      VectorOffset infinite_competeOffset = default(VectorOffset),
      VectorOffset infinite_compete_rewardOffset = default(VectorOffset),
      VectorOffset hero_battleOffset = default(VectorOffset),
      VectorOffset boss_challenge_activity_configOffset = default(VectorOffset),
      VectorOffset sleep_tower_rewardOffset = default(VectorOffset),
      VectorOffset buddy_runOffset = default(VectorOffset),
      VectorOffset boss_challenge_activity_total_timeOffset = default(VectorOffset)) {
    builder.StartObject(43);
    ConditionEvent.AddBossChallengeActivityTotalTime(builder, boss_challenge_activity_total_timeOffset);
    ConditionEvent.AddBuddyRun(builder, buddy_runOffset);
    ConditionEvent.AddSleepTowerReward(builder, sleep_tower_rewardOffset);
    ConditionEvent.AddBossChallengeActivityConfig(builder, boss_challenge_activity_configOffset);
    ConditionEvent.AddHeroBattle(builder, hero_battleOffset);
    ConditionEvent.AddInfiniteCompeteReward(builder, infinite_compete_rewardOffset);
    ConditionEvent.AddInfiniteCompete(builder, infinite_competeOffset);
    ConditionEvent.AddHonorArenaReward(builder, honor_arena_rewardOffset);
    ConditionEvent.AddUrEventReward(builder, ur_event_rewardOffset);
    ConditionEvent.AddMainCampaignReward(builder, main_campaign_rewardOffset);
    ConditionEvent.AddLevelCompensatedReward(builder, level_compensated_rewardOffset);
    ConditionEvent.AddDiamondGift(builder, diamond_giftOffset);
    ConditionEvent.AddSleepTower(builder, sleep_towerOffset);
    ConditionEvent.AddAwakenDungeon(builder, awaken_dungeonOffset);
    ConditionEvent.AddHeroMedal(builder, hero_medalOffset);
    ConditionEvent.AddPartnerCultivateGift(builder, partner_cultivate_giftOffset);
    ConditionEvent.AddCronjobs(builder, cronjobsOffset);
    ConditionEvent.AddInfiniteBuff(builder, infinite_buffOffset);
    ConditionEvent.AddNationSpecialEvent(builder, nation_special_eventOffset);
    ConditionEvent.AddNationRankReward(builder, nation_rank_rewardOffset);
    ConditionEvent.AddNationRatingReward(builder, nation_rating_rewardOffset);
    ConditionEvent.AddClashOfHeroesReward(builder, clash_of_heroes_rewardOffset);
    ConditionEvent.AddBossreward(builder, bossrewardOffset);
    ConditionEvent.AddLadderSeasonAward(builder, ladder_season_awardOffset);
    ConditionEvent.AddLadder(builder, ladderOffset);
    ConditionEvent.AddInfinitechallenge(builder, infinitechallengeOffset);
    ConditionEvent.AddTimeLimitActivityStage(builder, time_limit_activity_stageOffset);
    ConditionEvent.AddTimeLimitActivity(builder, time_limit_activityOffset);
    ConditionEvent.AddNormalActivityInstance(builder, normal_activity_instanceOffset);
    ConditionEvent.AddSpecialActivityLevel(builder, special_activity_levelOffset);
    ConditionEvent.AddSpecialActivity(builder, special_activityOffset);
    ConditionEvent.AddDailyReward(builder, daily_rewardOffset);
    ConditionEvent.AddSignInReward(builder, sign_in_rewardOffset);
    ConditionEvent.AddArenaAward(builder, arena_awardOffset);
    ConditionEvent.AddExpeditionMapConfig(builder, expedition_map_configOffset);
    ConditionEvent.AddRobots(builder, robotsOffset);
    ConditionEvent.AddOptions(builder, optionsOffset);
    ConditionEvent.AddUserConditions(builder, user_conditionsOffset);
    ConditionEvent.AddDateConditions(builder, date_conditionsOffset);
    ConditionEvent.AddPriority(builder, priority);
    ConditionEvent.AddName(builder, nameOffset);
    ConditionEvent.Add_id(builder, _idOffset);
    ConditionEvent.AddEnabled(builder, enabled);
    return ConditionEvent.EndConditionEvent(builder);
  }

  public static void StartConditionEvent(FlatBufferBuilder builder) { builder.StartObject(43); }
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
  public static void AddRobots(FlatBufferBuilder builder, VectorOffset robotsOffset) { builder.AddOffset(7, robotsOffset.Value, 0); }
  public static VectorOffset CreateRobotsVector(FlatBufferBuilder builder, Offset<RobotInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartRobotsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddExpeditionMapConfig(FlatBufferBuilder builder, VectorOffset expeditionMapConfigOffset) { builder.AddOffset(8, expeditionMapConfigOffset.Value, 0); }
  public static VectorOffset CreateExpeditionMapConfigVector(FlatBufferBuilder builder, Offset<ExpeditionMapConfig>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartExpeditionMapConfigVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddArenaAward(FlatBufferBuilder builder, VectorOffset arenaAwardOffset) { builder.AddOffset(9, arenaAwardOffset.Value, 0); }
  public static VectorOffset CreateArenaAwardVector(FlatBufferBuilder builder, Offset<ArenaAward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArenaAwardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSignInReward(FlatBufferBuilder builder, VectorOffset signInRewardOffset) { builder.AddOffset(10, signInRewardOffset.Value, 0); }
  public static VectorOffset CreateSignInRewardVector(FlatBufferBuilder builder, Offset<SignInReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSignInRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDailyReward(FlatBufferBuilder builder, VectorOffset dailyRewardOffset) { builder.AddOffset(11, dailyRewardOffset.Value, 0); }
  public static VectorOffset CreateDailyRewardVector(FlatBufferBuilder builder, Offset<DailyReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDailyRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSpecialActivity(FlatBufferBuilder builder, VectorOffset specialActivityOffset) { builder.AddOffset(12, specialActivityOffset.Value, 0); }
  public static VectorOffset CreateSpecialActivityVector(FlatBufferBuilder builder, Offset<SpecialActivity>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSpecialActivityVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSpecialActivityLevel(FlatBufferBuilder builder, VectorOffset specialActivityLevelOffset) { builder.AddOffset(13, specialActivityLevelOffset.Value, 0); }
  public static VectorOffset CreateSpecialActivityLevelVector(FlatBufferBuilder builder, Offset<SpecialActivityLevel>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSpecialActivityLevelVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddNormalActivityInstance(FlatBufferBuilder builder, VectorOffset normalActivityInstanceOffset) { builder.AddOffset(14, normalActivityInstanceOffset.Value, 0); }
  public static VectorOffset CreateNormalActivityInstanceVector(FlatBufferBuilder builder, Offset<NormalActivityInstance>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartNormalActivityInstanceVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTimeLimitActivity(FlatBufferBuilder builder, VectorOffset timeLimitActivityOffset) { builder.AddOffset(15, timeLimitActivityOffset.Value, 0); }
  public static VectorOffset CreateTimeLimitActivityVector(FlatBufferBuilder builder, Offset<TimeLimitActivity>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTimeLimitActivityVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTimeLimitActivityStage(FlatBufferBuilder builder, VectorOffset timeLimitActivityStageOffset) { builder.AddOffset(16, timeLimitActivityStageOffset.Value, 0); }
  public static VectorOffset CreateTimeLimitActivityStageVector(FlatBufferBuilder builder, Offset<TimeLimitActivityStage>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTimeLimitActivityStageVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddInfinitechallenge(FlatBufferBuilder builder, VectorOffset infinitechallengeOffset) { builder.AddOffset(17, infinitechallengeOffset.Value, 0); }
  public static VectorOffset CreateInfinitechallengeVector(FlatBufferBuilder builder, Offset<InfiniteChallenge>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartInfinitechallengeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLadder(FlatBufferBuilder builder, VectorOffset ladderOffset) { builder.AddOffset(18, ladderOffset.Value, 0); }
  public static VectorOffset CreateLadderVector(FlatBufferBuilder builder, Offset<Ladder>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLadderVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLadderSeasonAward(FlatBufferBuilder builder, VectorOffset ladderSeasonAwardOffset) { builder.AddOffset(19, ladderSeasonAwardOffset.Value, 0); }
  public static VectorOffset CreateLadderSeasonAwardVector(FlatBufferBuilder builder, Offset<LadderSeasonAward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLadderSeasonAwardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBossreward(FlatBufferBuilder builder, VectorOffset bossrewardOffset) { builder.AddOffset(20, bossrewardOffset.Value, 0); }
  public static VectorOffset CreateBossrewardVector(FlatBufferBuilder builder, Offset<BossReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBossrewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddClashOfHeroesReward(FlatBufferBuilder builder, VectorOffset clashOfHeroesRewardOffset) { builder.AddOffset(21, clashOfHeroesRewardOffset.Value, 0); }
  public static VectorOffset CreateClashOfHeroesRewardVector(FlatBufferBuilder builder, Offset<ClashOfHeroesReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartClashOfHeroesRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddNationRatingReward(FlatBufferBuilder builder, VectorOffset nationRatingRewardOffset) { builder.AddOffset(22, nationRatingRewardOffset.Value, 0); }
  public static VectorOffset CreateNationRatingRewardVector(FlatBufferBuilder builder, Offset<NationRatingReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartNationRatingRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddNationRankReward(FlatBufferBuilder builder, VectorOffset nationRankRewardOffset) { builder.AddOffset(23, nationRankRewardOffset.Value, 0); }
  public static VectorOffset CreateNationRankRewardVector(FlatBufferBuilder builder, Offset<NationRankReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartNationRankRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddNationSpecialEvent(FlatBufferBuilder builder, VectorOffset nationSpecialEventOffset) { builder.AddOffset(24, nationSpecialEventOffset.Value, 0); }
  public static VectorOffset CreateNationSpecialEventVector(FlatBufferBuilder builder, Offset<NationSpecialEvent>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartNationSpecialEventVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddInfiniteBuff(FlatBufferBuilder builder, VectorOffset infiniteBuffOffset) { builder.AddOffset(25, infiniteBuffOffset.Value, 0); }
  public static VectorOffset CreateInfiniteBuffVector(FlatBufferBuilder builder, Offset<InfiniteBuff>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartInfiniteBuffVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddCronjobs(FlatBufferBuilder builder, VectorOffset cronjobsOffset) { builder.AddOffset(26, cronjobsOffset.Value, 0); }
  public static VectorOffset CreateCronjobsVector(FlatBufferBuilder builder, Offset<CronJobs>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartCronjobsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddPartnerCultivateGift(FlatBufferBuilder builder, VectorOffset partnerCultivateGiftOffset) { builder.AddOffset(27, partnerCultivateGiftOffset.Value, 0); }
  public static VectorOffset CreatePartnerCultivateGiftVector(FlatBufferBuilder builder, Offset<PartnerCultivateGift>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartPartnerCultivateGiftVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHeroMedal(FlatBufferBuilder builder, VectorOffset heroMedalOffset) { builder.AddOffset(28, heroMedalOffset.Value, 0); }
  public static VectorOffset CreateHeroMedalVector(FlatBufferBuilder builder, Offset<HeroMedal>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartHeroMedalVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAwakenDungeon(FlatBufferBuilder builder, VectorOffset awakenDungeonOffset) { builder.AddOffset(29, awakenDungeonOffset.Value, 0); }
  public static VectorOffset CreateAwakenDungeonVector(FlatBufferBuilder builder, Offset<AwakenDungeon>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAwakenDungeonVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSleepTower(FlatBufferBuilder builder, VectorOffset sleepTowerOffset) { builder.AddOffset(30, sleepTowerOffset.Value, 0); }
  public static VectorOffset CreateSleepTowerVector(FlatBufferBuilder builder, Offset<SleepTower>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSleepTowerVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDiamondGift(FlatBufferBuilder builder, VectorOffset diamondGiftOffset) { builder.AddOffset(31, diamondGiftOffset.Value, 0); }
  public static VectorOffset CreateDiamondGiftVector(FlatBufferBuilder builder, Offset<DiamondGift>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDiamondGiftVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLevelCompensatedReward(FlatBufferBuilder builder, VectorOffset levelCompensatedRewardOffset) { builder.AddOffset(32, levelCompensatedRewardOffset.Value, 0); }
  public static VectorOffset CreateLevelCompensatedRewardVector(FlatBufferBuilder builder, Offset<LevelCompensatedReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLevelCompensatedRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMainCampaignReward(FlatBufferBuilder builder, VectorOffset mainCampaignRewardOffset) { builder.AddOffset(33, mainCampaignRewardOffset.Value, 0); }
  public static VectorOffset CreateMainCampaignRewardVector(FlatBufferBuilder builder, Offset<MainCampaignReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMainCampaignRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddUrEventReward(FlatBufferBuilder builder, VectorOffset urEventRewardOffset) { builder.AddOffset(34, urEventRewardOffset.Value, 0); }
  public static VectorOffset CreateUrEventRewardVector(FlatBufferBuilder builder, Offset<UrEventReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartUrEventRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHonorArenaReward(FlatBufferBuilder builder, VectorOffset honorArenaRewardOffset) { builder.AddOffset(35, honorArenaRewardOffset.Value, 0); }
  public static VectorOffset CreateHonorArenaRewardVector(FlatBufferBuilder builder, Offset<HonorArenaReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartHonorArenaRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddInfiniteCompete(FlatBufferBuilder builder, VectorOffset infiniteCompeteOffset) { builder.AddOffset(36, infiniteCompeteOffset.Value, 0); }
  public static VectorOffset CreateInfiniteCompeteVector(FlatBufferBuilder builder, Offset<InfiniteCompete>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartInfiniteCompeteVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddInfiniteCompeteReward(FlatBufferBuilder builder, VectorOffset infiniteCompeteRewardOffset) { builder.AddOffset(37, infiniteCompeteRewardOffset.Value, 0); }
  public static VectorOffset CreateInfiniteCompeteRewardVector(FlatBufferBuilder builder, Offset<InfiniteCompeteReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartInfiniteCompeteRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHeroBattle(FlatBufferBuilder builder, VectorOffset heroBattleOffset) { builder.AddOffset(38, heroBattleOffset.Value, 0); }
  public static VectorOffset CreateHeroBattleVector(FlatBufferBuilder builder, Offset<HeroBattle>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartHeroBattleVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBossChallengeActivityConfig(FlatBufferBuilder builder, VectorOffset bossChallengeActivityConfigOffset) { builder.AddOffset(39, bossChallengeActivityConfigOffset.Value, 0); }
  public static VectorOffset CreateBossChallengeActivityConfigVector(FlatBufferBuilder builder, Offset<BossChallengeActivityConfig>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBossChallengeActivityConfigVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSleepTowerReward(FlatBufferBuilder builder, VectorOffset sleepTowerRewardOffset) { builder.AddOffset(40, sleepTowerRewardOffset.Value, 0); }
  public static VectorOffset CreateSleepTowerRewardVector(FlatBufferBuilder builder, Offset<SleepTowerReward>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSleepTowerRewardVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBuddyRun(FlatBufferBuilder builder, VectorOffset buddyRunOffset) { builder.AddOffset(41, buddyRunOffset.Value, 0); }
  public static VectorOffset CreateBuddyRunVector(FlatBufferBuilder builder, Offset<BuddyRun>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBuddyRunVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBossChallengeActivityTotalTime(FlatBufferBuilder builder, VectorOffset bossChallengeActivityTotalTimeOffset) { builder.AddOffset(42, bossChallengeActivityTotalTimeOffset.Value, 0); }
  public static VectorOffset CreateBossChallengeActivityTotalTimeVector(FlatBufferBuilder builder, Offset<BossChallengeActivityTotalTime>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBossChallengeActivityTotalTimeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionEvent> EndConditionEvent(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionEvent>(o);
  }
};

public sealed class Event : Table {
  public static Event GetRootAsEvent(ByteBuffer _bb) { return GetRootAsEvent(_bb, new Event()); }
  public static Event GetRootAsEvent(ByteBuffer _bb, Event obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Event __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionEvent GetArray(int j) { return GetArray(new ConditionEvent(), j); }
  public ConditionEvent GetArray(ConditionEvent obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Event> CreateEvent(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Event.AddArray(builder, arrayOffset);
    return Event.EndEvent(builder);
  }

  public static void StartEvent(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionEvent>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Event> EndEvent(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Event>(o);
  }
  public static void FinishEventBuffer(FlatBufferBuilder builder, Offset<Event> offset) { builder.Finish(offset.Value); }
};


}
