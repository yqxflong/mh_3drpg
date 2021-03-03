// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class GameConfig : Table {
  public static GameConfig GetRootAsGameConfig(ByteBuffer _bb) { return GetRootAsGameConfig(_bb, new GameConfig()); }
  public static GameConfig GetRootAsGameConfig(ByteBuffer _bb, GameConfig obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public GameConfig __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Key { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetKeyBytes() { return __vector_as_arraysegment(4); }
  public float Value { get { int o = __offset(6); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string StrValue { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetStrValueBytes() { return __vector_as_arraysegment(8); }
  public string Comment { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCommentBytes() { return __vector_as_arraysegment(10); }

  public static Offset<GameConfig> CreateGameConfig(FlatBufferBuilder builder,
      StringOffset keyOffset = default(StringOffset),
      float value = 0,
      StringOffset str_valueOffset = default(StringOffset),
      StringOffset commentOffset = default(StringOffset)) {
    builder.StartObject(4);
    GameConfig.AddComment(builder, commentOffset);
    GameConfig.AddStrValue(builder, str_valueOffset);
    GameConfig.AddValue(builder, value);
    GameConfig.AddKey(builder, keyOffset);
    return GameConfig.EndGameConfig(builder);
  }

  public static void StartGameConfig(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddKey(FlatBufferBuilder builder, StringOffset keyOffset) { builder.AddOffset(0, keyOffset.Value, 0); }
  public static void AddValue(FlatBufferBuilder builder, float value) { builder.AddFloat(1, value, 0); }
  public static void AddStrValue(FlatBufferBuilder builder, StringOffset strValueOffset) { builder.AddOffset(2, strValueOffset.Value, 0); }
  public static void AddComment(FlatBufferBuilder builder, StringOffset commentOffset) { builder.AddOffset(3, commentOffset.Value, 0); }
  public static Offset<GameConfig> EndGameConfig(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GameConfig>(o);
  }
};

public sealed class ConditionNewGameConfig : Table {
  public static ConditionNewGameConfig GetRootAsConditionNewGameConfig(ByteBuffer _bb) { return GetRootAsConditionNewGameConfig(_bb, new ConditionNewGameConfig()); }
  public static ConditionNewGameConfig GetRootAsConditionNewGameConfig(ByteBuffer _bb, ConditionNewGameConfig obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionNewGameConfig __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

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
  public GameConfig GetGameConfig(int j) { return GetGameConfig(new GameConfig(), j); }
  public GameConfig GetGameConfig(GameConfig obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int GameConfigLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionNewGameConfig> CreateConditionNewGameConfig(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = false,
      int priority = 0,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset GameConfigOffset = default(VectorOffset)) {
    builder.StartObject(8);
    ConditionNewGameConfig.AddGameConfig(builder, GameConfigOffset);
    ConditionNewGameConfig.AddOptions(builder, optionsOffset);
    ConditionNewGameConfig.AddUserConditions(builder, user_conditionsOffset);
    ConditionNewGameConfig.AddDateConditions(builder, date_conditionsOffset);
    ConditionNewGameConfig.AddPriority(builder, priority);
    ConditionNewGameConfig.AddName(builder, nameOffset);
    ConditionNewGameConfig.Add_id(builder, _idOffset);
    ConditionNewGameConfig.AddEnabled(builder, enabled);
    return ConditionNewGameConfig.EndConditionNewGameConfig(builder);
  }

  public static void StartConditionNewGameConfig(FlatBufferBuilder builder) { builder.StartObject(8); }
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
  public static void AddGameConfig(FlatBufferBuilder builder, VectorOffset GameConfigOffset) { builder.AddOffset(7, GameConfigOffset.Value, 0); }
  public static VectorOffset CreateGameConfigVector(FlatBufferBuilder builder, Offset<GameConfig>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartGameConfigVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionNewGameConfig> EndConditionNewGameConfig(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionNewGameConfig>(o);
  }
};

public sealed class NewGameConfig : Table {
  public static NewGameConfig GetRootAsNewGameConfig(ByteBuffer _bb) { return GetRootAsNewGameConfig(_bb, new NewGameConfig()); }
  public static NewGameConfig GetRootAsNewGameConfig(ByteBuffer _bb, NewGameConfig obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public NewGameConfig __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionNewGameConfig GetArray(int j) { return GetArray(new ConditionNewGameConfig(), j); }
  public ConditionNewGameConfig GetArray(ConditionNewGameConfig obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<NewGameConfig> CreateNewGameConfig(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    NewGameConfig.AddArray(builder, arrayOffset);
    return NewGameConfig.EndNewGameConfig(builder);
  }

  public static void StartNewGameConfig(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionNewGameConfig>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<NewGameConfig> EndNewGameConfig(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<NewGameConfig>(o);
  }
  public static void FinishNewGameConfigBuffer(FlatBufferBuilder builder, Offset<NewGameConfig> offset) { builder.Finish(offset.Value); }
};


}
