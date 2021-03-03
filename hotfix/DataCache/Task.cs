// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class TaskInfo : Table {
  public static TaskInfo GetRootAsTaskInfo(ByteBuffer _bb) { return GetRootAsTaskInfo(_bb, new TaskInfo()); }
  public static TaskInfo GetRootAsTaskInfo(ByteBuffer _bb, TaskInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public TaskInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string TaskId { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTaskIdBytes() { return __vector_as_arraysegment(4); }
  public string TaskName { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTaskNameBytes() { return __vector_as_arraysegment(6); }
  public string PreTask { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPreTaskBytes() { return __vector_as_arraysegment(8); }
  public int RoleLevel { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ReceivingMeans { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string SceneId { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSceneIdBytes() { return __vector_as_arraysegment(14); }
  public string NpcId { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNpcIdBytes() { return __vector_as_arraysegment(16); }
  public string Tips { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTipsBytes() { return __vector_as_arraysegment(18); }
  public int IsAutoComplete { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string CommitSceneId { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCommitSceneIdBytes() { return __vector_as_arraysegment(22); }
  public string CommitNpcId { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCommitNpcIdBytes() { return __vector_as_arraysegment(24); }
  public int TaskType { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int TargetType { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string TargetParameter1 { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTargetParameter1Bytes() { return __vector_as_arraysegment(30); }
  public string TargetParameter2 { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTargetParameter2Bytes() { return __vector_as_arraysegment(32); }
  public string TargetParameter3 { get { int o = __offset(34); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTargetParameter3Bytes() { return __vector_as_arraysegment(34); }
  public string TargetTips { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTargetTipsBytes() { return __vector_as_arraysegment(36); }
  public int GetXp(int j) { int o = __offset(38); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int XpLength { get { int o = __offset(38); return o != 0 ? __vector_len(o) : 0; } }
  public ArraySegment<byte>? GetXpBytes() { return __vector_as_arraysegment(38); }
  public int GetGold(int j) { int o = __offset(40); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int GoldLength { get { int o = __offset(40); return o != 0 ? __vector_len(o) : 0; } }
  public ArraySegment<byte>? GetGoldBytes() { return __vector_as_arraysegment(40); }
  public int Hc { get { int o = __offset(42); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AllianceDonate { get { int o = __offset(44); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Reward1 { get { int o = __offset(46); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetReward1Bytes() { return __vector_as_arraysegment(46); }
  public int Count1 { get { int o = __offset(48); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Reward2 { get { int o = __offset(50); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetReward2Bytes() { return __vector_as_arraysegment(50); }
  public int Count2 { get { int o = __offset(52); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Reward3 { get { int o = __offset(54); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetReward3Bytes() { return __vector_as_arraysegment(54); }
  public int Count3 { get { int o = __offset(56); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string HeroShard { get { int o = __offset(58); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetHeroShardBytes() { return __vector_as_arraysegment(58); }
  public int ShardCount { get { int o = __offset(60); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AcceptDialogue { get { int o = __offset(62); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CompleteDialogue { get { int o = __offset(64); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AchievementPoint { get { int o = __offset(66); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ActivityPoint { get { int o = __offset(68); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ResType { get { int o = __offset(70); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetResTypeBytes() { return __vector_as_arraysegment(70); }
  public int ResCount { get { int o = __offset(72); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int FunctionLimit { get { int o = __offset(74); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<TaskInfo> CreateTaskInfo(FlatBufferBuilder builder,
      StringOffset task_idOffset = default(StringOffset),
      StringOffset task_nameOffset = default(StringOffset),
      StringOffset pre_taskOffset = default(StringOffset),
      int role_level = 0,
      int receiving_means = 0,
      StringOffset scene_idOffset = default(StringOffset),
      StringOffset npc_idOffset = default(StringOffset),
      StringOffset tipsOffset = default(StringOffset),
      int is_auto_complete = 0,
      StringOffset commit_scene_idOffset = default(StringOffset),
      StringOffset commit_npc_idOffset = default(StringOffset),
      int task_type = 0,
      int target_type = 0,
      StringOffset target_parameter_1Offset = default(StringOffset),
      StringOffset target_parameter_2Offset = default(StringOffset),
      StringOffset target_parameter_3Offset = default(StringOffset),
      StringOffset target_tipsOffset = default(StringOffset),
      VectorOffset xpOffset = default(VectorOffset),
      VectorOffset goldOffset = default(VectorOffset),
      int hc = 0,
      int alliance_donate = 0,
      StringOffset reward_1Offset = default(StringOffset),
      int count_1 = 0,
      StringOffset reward_2Offset = default(StringOffset),
      int count_2 = 0,
      StringOffset reward_3Offset = default(StringOffset),
      int count_3 = 0,
      StringOffset hero_shardOffset = default(StringOffset),
      int shard_count = 0,
      int accept_dialogue = 0,
      int complete_dialogue = 0,
      int achievement_point = 0,
      int activity_point = 0,
      StringOffset res_typeOffset = default(StringOffset),
      int res_count = 0,
      int function_limit = 0) {
    builder.StartObject(36);
    TaskInfo.AddFunctionLimit(builder, function_limit);
    TaskInfo.AddResCount(builder, res_count);
    TaskInfo.AddResType(builder, res_typeOffset);
    TaskInfo.AddActivityPoint(builder, activity_point);
    TaskInfo.AddAchievementPoint(builder, achievement_point);
    TaskInfo.AddCompleteDialogue(builder, complete_dialogue);
    TaskInfo.AddAcceptDialogue(builder, accept_dialogue);
    TaskInfo.AddShardCount(builder, shard_count);
    TaskInfo.AddHeroShard(builder, hero_shardOffset);
    TaskInfo.AddCount3(builder, count_3);
    TaskInfo.AddReward3(builder, reward_3Offset);
    TaskInfo.AddCount2(builder, count_2);
    TaskInfo.AddReward2(builder, reward_2Offset);
    TaskInfo.AddCount1(builder, count_1);
    TaskInfo.AddReward1(builder, reward_1Offset);
    TaskInfo.AddAllianceDonate(builder, alliance_donate);
    TaskInfo.AddHc(builder, hc);
    TaskInfo.AddGold(builder, goldOffset);
    TaskInfo.AddXp(builder, xpOffset);
    TaskInfo.AddTargetTips(builder, target_tipsOffset);
    TaskInfo.AddTargetParameter3(builder, target_parameter_3Offset);
    TaskInfo.AddTargetParameter2(builder, target_parameter_2Offset);
    TaskInfo.AddTargetParameter1(builder, target_parameter_1Offset);
    TaskInfo.AddTargetType(builder, target_type);
    TaskInfo.AddTaskType(builder, task_type);
    TaskInfo.AddCommitNpcId(builder, commit_npc_idOffset);
    TaskInfo.AddCommitSceneId(builder, commit_scene_idOffset);
    TaskInfo.AddIsAutoComplete(builder, is_auto_complete);
    TaskInfo.AddTips(builder, tipsOffset);
    TaskInfo.AddNpcId(builder, npc_idOffset);
    TaskInfo.AddSceneId(builder, scene_idOffset);
    TaskInfo.AddReceivingMeans(builder, receiving_means);
    TaskInfo.AddRoleLevel(builder, role_level);
    TaskInfo.AddPreTask(builder, pre_taskOffset);
    TaskInfo.AddTaskName(builder, task_nameOffset);
    TaskInfo.AddTaskId(builder, task_idOffset);
    return TaskInfo.EndTaskInfo(builder);
  }

  public static void StartTaskInfo(FlatBufferBuilder builder) { builder.StartObject(36); }
  public static void AddTaskId(FlatBufferBuilder builder, StringOffset taskIdOffset) { builder.AddOffset(0, taskIdOffset.Value, 0); }
  public static void AddTaskName(FlatBufferBuilder builder, StringOffset taskNameOffset) { builder.AddOffset(1, taskNameOffset.Value, 0); }
  public static void AddPreTask(FlatBufferBuilder builder, StringOffset preTaskOffset) { builder.AddOffset(2, preTaskOffset.Value, 0); }
  public static void AddRoleLevel(FlatBufferBuilder builder, int roleLevel) { builder.AddInt(3, roleLevel, 0); }
  public static void AddReceivingMeans(FlatBufferBuilder builder, int receivingMeans) { builder.AddInt(4, receivingMeans, 0); }
  public static void AddSceneId(FlatBufferBuilder builder, StringOffset sceneIdOffset) { builder.AddOffset(5, sceneIdOffset.Value, 0); }
  public static void AddNpcId(FlatBufferBuilder builder, StringOffset npcIdOffset) { builder.AddOffset(6, npcIdOffset.Value, 0); }
  public static void AddTips(FlatBufferBuilder builder, StringOffset tipsOffset) { builder.AddOffset(7, tipsOffset.Value, 0); }
  public static void AddIsAutoComplete(FlatBufferBuilder builder, int isAutoComplete) { builder.AddInt(8, isAutoComplete, 0); }
  public static void AddCommitSceneId(FlatBufferBuilder builder, StringOffset commitSceneIdOffset) { builder.AddOffset(9, commitSceneIdOffset.Value, 0); }
  public static void AddCommitNpcId(FlatBufferBuilder builder, StringOffset commitNpcIdOffset) { builder.AddOffset(10, commitNpcIdOffset.Value, 0); }
  public static void AddTaskType(FlatBufferBuilder builder, int taskType) { builder.AddInt(11, taskType, 0); }
  public static void AddTargetType(FlatBufferBuilder builder, int targetType) { builder.AddInt(12, targetType, 0); }
  public static void AddTargetParameter1(FlatBufferBuilder builder, StringOffset targetParameter1Offset) { builder.AddOffset(13, targetParameter1Offset.Value, 0); }
  public static void AddTargetParameter2(FlatBufferBuilder builder, StringOffset targetParameter2Offset) { builder.AddOffset(14, targetParameter2Offset.Value, 0); }
  public static void AddTargetParameter3(FlatBufferBuilder builder, StringOffset targetParameter3Offset) { builder.AddOffset(15, targetParameter3Offset.Value, 0); }
  public static void AddTargetTips(FlatBufferBuilder builder, StringOffset targetTipsOffset) { builder.AddOffset(16, targetTipsOffset.Value, 0); }
  public static void AddXp(FlatBufferBuilder builder, VectorOffset xpOffset) { builder.AddOffset(17, xpOffset.Value, 0); }
  public static VectorOffset CreateXpVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartXpVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddGold(FlatBufferBuilder builder, VectorOffset goldOffset) { builder.AddOffset(18, goldOffset.Value, 0); }
  public static VectorOffset CreateGoldVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartGoldVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHc(FlatBufferBuilder builder, int hc) { builder.AddInt(19, hc, 0); }
  public static void AddAllianceDonate(FlatBufferBuilder builder, int allianceDonate) { builder.AddInt(20, allianceDonate, 0); }
  public static void AddReward1(FlatBufferBuilder builder, StringOffset reward1Offset) { builder.AddOffset(21, reward1Offset.Value, 0); }
  public static void AddCount1(FlatBufferBuilder builder, int count1) { builder.AddInt(22, count1, 0); }
  public static void AddReward2(FlatBufferBuilder builder, StringOffset reward2Offset) { builder.AddOffset(23, reward2Offset.Value, 0); }
  public static void AddCount2(FlatBufferBuilder builder, int count2) { builder.AddInt(24, count2, 0); }
  public static void AddReward3(FlatBufferBuilder builder, StringOffset reward3Offset) { builder.AddOffset(25, reward3Offset.Value, 0); }
  public static void AddCount3(FlatBufferBuilder builder, int count3) { builder.AddInt(26, count3, 0); }
  public static void AddHeroShard(FlatBufferBuilder builder, StringOffset heroShardOffset) { builder.AddOffset(27, heroShardOffset.Value, 0); }
  public static void AddShardCount(FlatBufferBuilder builder, int shardCount) { builder.AddInt(28, shardCount, 0); }
  public static void AddAcceptDialogue(FlatBufferBuilder builder, int acceptDialogue) { builder.AddInt(29, acceptDialogue, 0); }
  public static void AddCompleteDialogue(FlatBufferBuilder builder, int completeDialogue) { builder.AddInt(30, completeDialogue, 0); }
  public static void AddAchievementPoint(FlatBufferBuilder builder, int achievementPoint) { builder.AddInt(31, achievementPoint, 0); }
  public static void AddActivityPoint(FlatBufferBuilder builder, int activityPoint) { builder.AddInt(32, activityPoint, 0); }
  public static void AddResType(FlatBufferBuilder builder, StringOffset resTypeOffset) { builder.AddOffset(33, resTypeOffset.Value, 0); }
  public static void AddResCount(FlatBufferBuilder builder, int resCount) { builder.AddInt(34, resCount, 0); }
  public static void AddFunctionLimit(FlatBufferBuilder builder, int functionLimit) { builder.AddInt(35, functionLimit, 0); }
  public static Offset<TaskInfo> EndTaskInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<TaskInfo>(o);
  }
};

public sealed class MonsterPool : Table {
  public static MonsterPool GetRootAsMonsterPool(ByteBuffer _bb) { return GetRootAsMonsterPool(_bb, new MonsterPool()); }
  public static MonsterPool GetRootAsMonsterPool(ByteBuffer _bb, MonsterPool obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public MonsterPool __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Level { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetLevelBytes() { return __vector_as_arraysegment(4); }
  public string MonsterId { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMonsterIdBytes() { return __vector_as_arraysegment(6); }
  public int ShardNumber { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Weight { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<MonsterPool> CreateMonsterPool(FlatBufferBuilder builder,
      StringOffset levelOffset = default(StringOffset),
      StringOffset monster_idOffset = default(StringOffset),
      int shard_number = 0,
      int weight = 0) {
    builder.StartObject(4);
    MonsterPool.AddWeight(builder, weight);
    MonsterPool.AddShardNumber(builder, shard_number);
    MonsterPool.AddMonsterId(builder, monster_idOffset);
    MonsterPool.AddLevel(builder, levelOffset);
    return MonsterPool.EndMonsterPool(builder);
  }

  public static void StartMonsterPool(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddLevel(FlatBufferBuilder builder, StringOffset levelOffset) { builder.AddOffset(0, levelOffset.Value, 0); }
  public static void AddMonsterId(FlatBufferBuilder builder, StringOffset monsterIdOffset) { builder.AddOffset(1, monsterIdOffset.Value, 0); }
  public static void AddShardNumber(FlatBufferBuilder builder, int shardNumber) { builder.AddInt(2, shardNumber, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(3, weight, 0); }
  public static Offset<MonsterPool> EndMonsterPool(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MonsterPool>(o);
  }
};

public sealed class BattlePass : Table {
  public static BattlePass GetRootAsBattlePass(ByteBuffer _bb) { return GetRootAsBattlePass(_bb, new BattlePass()); }
  public static BattlePass GetRootAsBattlePass(ByteBuffer _bb, BattlePass obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BattlePass __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
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
  public string Rt4 { get { int o = __offset(24); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt4Bytes() { return __vector_as_arraysegment(24); }
  public string Ri4 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi4Bytes() { return __vector_as_arraysegment(26); }
  public int Rn4 { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<BattlePass> CreateBattlePass(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
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
      int rn4 = 0) {
    builder.StartObject(13);
    BattlePass.AddRn4(builder, rn4);
    BattlePass.AddRi4(builder, ri4Offset);
    BattlePass.AddRt4(builder, rt4Offset);
    BattlePass.AddRn3(builder, rn3);
    BattlePass.AddRi3(builder, ri3Offset);
    BattlePass.AddRt3(builder, rt3Offset);
    BattlePass.AddRn2(builder, rn2);
    BattlePass.AddRi2(builder, ri2Offset);
    BattlePass.AddRt2(builder, rt2Offset);
    BattlePass.AddRn1(builder, rn1);
    BattlePass.AddRi1(builder, ri1Offset);
    BattlePass.AddRt1(builder, rt1Offset);
    BattlePass.AddId(builder, idOffset);
    return BattlePass.EndBattlePass(builder);
  }

  public static void StartBattlePass(FlatBufferBuilder builder) { builder.StartObject(13); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddRt1(FlatBufferBuilder builder, StringOffset rt1Offset) { builder.AddOffset(1, rt1Offset.Value, 0); }
  public static void AddRi1(FlatBufferBuilder builder, StringOffset ri1Offset) { builder.AddOffset(2, ri1Offset.Value, 0); }
  public static void AddRn1(FlatBufferBuilder builder, int rn1) { builder.AddInt(3, rn1, 0); }
  public static void AddRt2(FlatBufferBuilder builder, StringOffset rt2Offset) { builder.AddOffset(4, rt2Offset.Value, 0); }
  public static void AddRi2(FlatBufferBuilder builder, StringOffset ri2Offset) { builder.AddOffset(5, ri2Offset.Value, 0); }
  public static void AddRn2(FlatBufferBuilder builder, int rn2) { builder.AddInt(6, rn2, 0); }
  public static void AddRt3(FlatBufferBuilder builder, StringOffset rt3Offset) { builder.AddOffset(7, rt3Offset.Value, 0); }
  public static void AddRi3(FlatBufferBuilder builder, StringOffset ri3Offset) { builder.AddOffset(8, ri3Offset.Value, 0); }
  public static void AddRn3(FlatBufferBuilder builder, int rn3) { builder.AddInt(9, rn3, 0); }
  public static void AddRt4(FlatBufferBuilder builder, StringOffset rt4Offset) { builder.AddOffset(10, rt4Offset.Value, 0); }
  public static void AddRi4(FlatBufferBuilder builder, StringOffset ri4Offset) { builder.AddOffset(11, ri4Offset.Value, 0); }
  public static void AddRn4(FlatBufferBuilder builder, int rn4) { builder.AddInt(12, rn4, 0); }
  public static Offset<BattlePass> EndBattlePass(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BattlePass>(o);
  }
};

public sealed class LimitedTimeGift : Table {
  public static LimitedTimeGift GetRootAsLimitedTimeGift(ByteBuffer _bb) { return GetRootAsLimitedTimeGift(_bb, new LimitedTimeGift()); }
  public static LimitedTimeGift GetRootAsLimitedTimeGift(ByteBuffer _bb, LimitedTimeGift obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LimitedTimeGift __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public int TriggerType { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string TargetParameter { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTargetParameterBytes() { return __vector_as_arraysegment(8); }
  public int TriggerOpenTime { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Duration { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Gift { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetGiftBytes() { return __vector_as_arraysegment(14); }
  public int Processing { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<LimitedTimeGift> CreateLimitedTimeGift(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      int trigger_type = 0,
      StringOffset target_parameterOffset = default(StringOffset),
      int trigger_open_time = 0,
      int duration = 0,
      StringOffset giftOffset = default(StringOffset),
      int processing = 0) {
    builder.StartObject(7);
    LimitedTimeGift.AddProcessing(builder, processing);
    LimitedTimeGift.AddGift(builder, giftOffset);
    LimitedTimeGift.AddDuration(builder, duration);
    LimitedTimeGift.AddTriggerOpenTime(builder, trigger_open_time);
    LimitedTimeGift.AddTargetParameter(builder, target_parameterOffset);
    LimitedTimeGift.AddTriggerType(builder, trigger_type);
    LimitedTimeGift.AddId(builder, idOffset);
    return LimitedTimeGift.EndLimitedTimeGift(builder);
  }

  public static void StartLimitedTimeGift(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddTriggerType(FlatBufferBuilder builder, int triggerType) { builder.AddInt(1, triggerType, 0); }
  public static void AddTargetParameter(FlatBufferBuilder builder, StringOffset targetParameterOffset) { builder.AddOffset(2, targetParameterOffset.Value, 0); }
  public static void AddTriggerOpenTime(FlatBufferBuilder builder, int triggerOpenTime) { builder.AddInt(3, triggerOpenTime, 0); }
  public static void AddDuration(FlatBufferBuilder builder, int duration) { builder.AddInt(4, duration, 0); }
  public static void AddGift(FlatBufferBuilder builder, StringOffset giftOffset) { builder.AddOffset(5, giftOffset.Value, 0); }
  public static void AddProcessing(FlatBufferBuilder builder, int processing) { builder.AddInt(6, processing, 0); }
  public static Offset<LimitedTimeGift> EndLimitedTimeGift(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LimitedTimeGift>(o);
  }
};

public sealed class ConditionTask : Table {
  public static ConditionTask GetRootAsConditionTask(ByteBuffer _bb) { return GetRootAsConditionTask(_bb, new ConditionTask()); }
  public static ConditionTask GetRootAsConditionTask(ByteBuffer _bb, ConditionTask obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionTask __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

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
  public TaskInfo GetTasks(int j) { return GetTasks(new TaskInfo(), j); }
  public TaskInfo GetTasks(TaskInfo obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int TasksLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public MonsterPool GetMonsterPool(int j) { return GetMonsterPool(new MonsterPool(), j); }
  public MonsterPool GetMonsterPool(MonsterPool obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MonsterPoolLength { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public BattlePass GetBattlePass(int j) { return GetBattlePass(new BattlePass(), j); }
  public BattlePass GetBattlePass(BattlePass obj, int j) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BattlePassLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }
  public LimitedTimeGift GetLimitedTimeGift(int j) { return GetLimitedTimeGift(new LimitedTimeGift(), j); }
  public LimitedTimeGift GetLimitedTimeGift(LimitedTimeGift obj, int j) { int o = __offset(24); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LimitedTimeGiftLength { get { int o = __offset(24); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionTask> CreateConditionTask(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = false,
      int priority = 0,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset tasksOffset = default(VectorOffset),
      VectorOffset monster_poolOffset = default(VectorOffset),
      VectorOffset battle_passOffset = default(VectorOffset),
      VectorOffset limited_time_giftOffset = default(VectorOffset)) {
    builder.StartObject(11);
    ConditionTask.AddLimitedTimeGift(builder, limited_time_giftOffset);
    ConditionTask.AddBattlePass(builder, battle_passOffset);
    ConditionTask.AddMonsterPool(builder, monster_poolOffset);
    ConditionTask.AddTasks(builder, tasksOffset);
    ConditionTask.AddOptions(builder, optionsOffset);
    ConditionTask.AddUserConditions(builder, user_conditionsOffset);
    ConditionTask.AddDateConditions(builder, date_conditionsOffset);
    ConditionTask.AddPriority(builder, priority);
    ConditionTask.AddName(builder, nameOffset);
    ConditionTask.Add_id(builder, _idOffset);
    ConditionTask.AddEnabled(builder, enabled);
    return ConditionTask.EndConditionTask(builder);
  }

  public static void StartConditionTask(FlatBufferBuilder builder) { builder.StartObject(11); }
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
  public static void AddTasks(FlatBufferBuilder builder, VectorOffset tasksOffset) { builder.AddOffset(7, tasksOffset.Value, 0); }
  public static VectorOffset CreateTasksVector(FlatBufferBuilder builder, Offset<TaskInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartTasksVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMonsterPool(FlatBufferBuilder builder, VectorOffset monsterPoolOffset) { builder.AddOffset(8, monsterPoolOffset.Value, 0); }
  public static VectorOffset CreateMonsterPoolVector(FlatBufferBuilder builder, Offset<MonsterPool>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMonsterPoolVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBattlePass(FlatBufferBuilder builder, VectorOffset battlePassOffset) { builder.AddOffset(9, battlePassOffset.Value, 0); }
  public static VectorOffset CreateBattlePassVector(FlatBufferBuilder builder, Offset<BattlePass>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBattlePassVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLimitedTimeGift(FlatBufferBuilder builder, VectorOffset limitedTimeGiftOffset) { builder.AddOffset(10, limitedTimeGiftOffset.Value, 0); }
  public static VectorOffset CreateLimitedTimeGiftVector(FlatBufferBuilder builder, Offset<LimitedTimeGift>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLimitedTimeGiftVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionTask> EndConditionTask(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionTask>(o);
  }
};

public sealed class Task : Table {
  public static Task GetRootAsTask(ByteBuffer _bb) { return GetRootAsTask(_bb, new Task()); }
  public static Task GetRootAsTask(ByteBuffer _bb, Task obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Task __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionTask GetArray(int j) { return GetArray(new ConditionTask(), j); }
  public ConditionTask GetArray(ConditionTask obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Task> CreateTask(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Task.AddArray(builder, arrayOffset);
    return Task.EndTask(builder);
  }

  public static void StartTask(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionTask>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Task> EndTask(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Task>(o);
  }
  public static void FinishTaskBuffer(FlatBufferBuilder builder, Offset<Task> offset) { builder.Finish(offset.Value); }
};


}
