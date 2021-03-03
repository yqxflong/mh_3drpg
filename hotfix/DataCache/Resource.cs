// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class VigorInfo : Table {
  public static VigorInfo GetRootAsVigorInfo(ByteBuffer _bb) { return GetRootAsVigorInfo(_bb, new VigorInfo()); }
  public static VigorInfo GetRootAsVigorInfo(ByteBuffer _bb, VigorInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public VigorInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PriceHc { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Vigor { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<VigorInfo> CreateVigorInfo(FlatBufferBuilder builder,
      int id = 0,
      int price_hc = 0,
      int vigor = 0) {
    builder.StartObject(3);
    VigorInfo.AddVigor(builder, vigor);
    VigorInfo.AddPriceHc(builder, price_hc);
    VigorInfo.AddId(builder, id);
    return VigorInfo.EndVigorInfo(builder);
  }

  public static void StartVigorInfo(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddPriceHc(FlatBufferBuilder builder, int priceHc) { builder.AddInt(1, priceHc, 0); }
  public static void AddVigor(FlatBufferBuilder builder, int vigor) { builder.AddInt(2, vigor, 0); }
  public static Offset<VigorInfo> EndVigorInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<VigorInfo>(o);
  }
};

public sealed class GoldInfo : Table {
  public static GoldInfo GetRootAsGoldInfo(ByteBuffer _bb) { return GetRootAsGoldInfo(_bb, new GoldInfo()); }
  public static GoldInfo GetRootAsGoldInfo(ByteBuffer _bb, GoldInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public GoldInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PriceHc { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BaseGold { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LevelIncGold { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate2 { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate3 { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate10 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<GoldInfo> CreateGoldInfo(FlatBufferBuilder builder,
      int id = 0,
      int price_hc = 0,
      int base_gold = 0,
      int level_inc_gold = 0,
      int rate_2 = 0,
      int rate_3 = 0,
      int rate_10 = 0) {
    builder.StartObject(7);
    GoldInfo.AddRate10(builder, rate_10);
    GoldInfo.AddRate3(builder, rate_3);
    GoldInfo.AddRate2(builder, rate_2);
    GoldInfo.AddLevelIncGold(builder, level_inc_gold);
    GoldInfo.AddBaseGold(builder, base_gold);
    GoldInfo.AddPriceHc(builder, price_hc);
    GoldInfo.AddId(builder, id);
    return GoldInfo.EndGoldInfo(builder);
  }

  public static void StartGoldInfo(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddPriceHc(FlatBufferBuilder builder, int priceHc) { builder.AddInt(1, priceHc, 0); }
  public static void AddBaseGold(FlatBufferBuilder builder, int baseGold) { builder.AddInt(2, baseGold, 0); }
  public static void AddLevelIncGold(FlatBufferBuilder builder, int levelIncGold) { builder.AddInt(3, levelIncGold, 0); }
  public static void AddRate2(FlatBufferBuilder builder, int rate2) { builder.AddInt(4, rate2, 0); }
  public static void AddRate3(FlatBufferBuilder builder, int rate3) { builder.AddInt(5, rate3, 0); }
  public static void AddRate10(FlatBufferBuilder builder, int rate10) { builder.AddInt(6, rate10, 0); }
  public static Offset<GoldInfo> EndGoldInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GoldInfo>(o);
  }
};

public sealed class ActionPowerInfo : Table {
  public static ActionPowerInfo GetRootAsActionPowerInfo(ByteBuffer _bb) { return GetRootAsActionPowerInfo(_bb, new ActionPowerInfo()); }
  public static ActionPowerInfo GetRootAsActionPowerInfo(ByteBuffer _bb, ActionPowerInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ActionPowerInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PriceHc { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BaseActionPower { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate2 { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate3 { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate10 { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<ActionPowerInfo> CreateActionPowerInfo(FlatBufferBuilder builder,
      int id = 0,
      int price_hc = 0,
      int base_action_power = 0,
      int rate_2 = 0,
      int rate_3 = 0,
      int rate_10 = 0) {
    builder.StartObject(6);
    ActionPowerInfo.AddRate10(builder, rate_10);
    ActionPowerInfo.AddRate3(builder, rate_3);
    ActionPowerInfo.AddRate2(builder, rate_2);
    ActionPowerInfo.AddBaseActionPower(builder, base_action_power);
    ActionPowerInfo.AddPriceHc(builder, price_hc);
    ActionPowerInfo.AddId(builder, id);
    return ActionPowerInfo.EndActionPowerInfo(builder);
  }

  public static void StartActionPowerInfo(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddPriceHc(FlatBufferBuilder builder, int priceHc) { builder.AddInt(1, priceHc, 0); }
  public static void AddBaseActionPower(FlatBufferBuilder builder, int baseActionPower) { builder.AddInt(2, baseActionPower, 0); }
  public static void AddRate2(FlatBufferBuilder builder, int rate2) { builder.AddInt(3, rate2, 0); }
  public static void AddRate3(FlatBufferBuilder builder, int rate3) { builder.AddInt(4, rate3, 0); }
  public static void AddRate10(FlatBufferBuilder builder, int rate10) { builder.AddInt(5, rate10, 0); }
  public static Offset<ActionPowerInfo> EndActionPowerInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ActionPowerInfo>(o);
  }
};

public sealed class BuddyExpInfo : Table {
  public static BuddyExpInfo GetRootAsBuddyExpInfo(ByteBuffer _bb) { return GetRootAsBuddyExpInfo(_bb, new BuddyExpInfo()); }
  public static BuddyExpInfo GetRootAsBuddyExpInfo(ByteBuffer _bb, BuddyExpInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BuddyExpInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PriceHc { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BaseBuddyExp { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LevelIncBuddyExp { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate2 { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate3 { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate10 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<BuddyExpInfo> CreateBuddyExpInfo(FlatBufferBuilder builder,
      int id = 0,
      int price_hc = 0,
      int base_buddy_exp = 0,
      int level_inc_buddy_exp = 0,
      int rate_2 = 0,
      int rate_3 = 0,
      int rate_10 = 0) {
    builder.StartObject(7);
    BuddyExpInfo.AddRate10(builder, rate_10);
    BuddyExpInfo.AddRate3(builder, rate_3);
    BuddyExpInfo.AddRate2(builder, rate_2);
    BuddyExpInfo.AddLevelIncBuddyExp(builder, level_inc_buddy_exp);
    BuddyExpInfo.AddBaseBuddyExp(builder, base_buddy_exp);
    BuddyExpInfo.AddPriceHc(builder, price_hc);
    BuddyExpInfo.AddId(builder, id);
    return BuddyExpInfo.EndBuddyExpInfo(builder);
  }

  public static void StartBuddyExpInfo(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddPriceHc(FlatBufferBuilder builder, int priceHc) { builder.AddInt(1, priceHc, 0); }
  public static void AddBaseBuddyExp(FlatBufferBuilder builder, int baseBuddyExp) { builder.AddInt(2, baseBuddyExp, 0); }
  public static void AddLevelIncBuddyExp(FlatBufferBuilder builder, int levelIncBuddyExp) { builder.AddInt(3, levelIncBuddyExp, 0); }
  public static void AddRate2(FlatBufferBuilder builder, int rate2) { builder.AddInt(4, rate2, 0); }
  public static void AddRate3(FlatBufferBuilder builder, int rate3) { builder.AddInt(5, rate3, 0); }
  public static void AddRate10(FlatBufferBuilder builder, int rate10) { builder.AddInt(6, rate10, 0); }
  public static Offset<BuddyExpInfo> EndBuddyExpInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BuddyExpInfo>(o);
  }
};

public sealed class ActicketInfo : Table {
  public static ActicketInfo GetRootAsActicketInfo(ByteBuffer _bb) { return GetRootAsActicketInfo(_bb, new ActicketInfo()); }
  public static ActicketInfo GetRootAsActicketInfo(ByteBuffer _bb, ActicketInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ActicketInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PriceHc { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Acticket { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate2 { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate3 { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Rate10 { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<ActicketInfo> CreateActicketInfo(FlatBufferBuilder builder,
      int id = 0,
      int price_hc = 0,
      int acticket = 0,
      int rate_2 = 0,
      int rate_3 = 0,
      int rate_10 = 0) {
    builder.StartObject(6);
    ActicketInfo.AddRate10(builder, rate_10);
    ActicketInfo.AddRate3(builder, rate_3);
    ActicketInfo.AddRate2(builder, rate_2);
    ActicketInfo.AddActicket(builder, acticket);
    ActicketInfo.AddPriceHc(builder, price_hc);
    ActicketInfo.AddId(builder, id);
    return ActicketInfo.EndActicketInfo(builder);
  }

  public static void StartActicketInfo(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddPriceHc(FlatBufferBuilder builder, int priceHc) { builder.AddInt(1, priceHc, 0); }
  public static void AddActicket(FlatBufferBuilder builder, int acticket) { builder.AddInt(2, acticket, 0); }
  public static void AddRate2(FlatBufferBuilder builder, int rate2) { builder.AddInt(3, rate2, 0); }
  public static void AddRate3(FlatBufferBuilder builder, int rate3) { builder.AddInt(4, rate3, 0); }
  public static void AddRate10(FlatBufferBuilder builder, int rate10) { builder.AddInt(5, rate10, 0); }
  public static Offset<ActicketInfo> EndActicketInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ActicketInfo>(o);
  }
};

public sealed class ConditionResource : Table {
  public static ConditionResource GetRootAsConditionResource(ByteBuffer _bb) { return GetRootAsConditionResource(_bb, new ConditionResource()); }
  public static ConditionResource GetRootAsConditionResource(ByteBuffer _bb, ConditionResource obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionResource __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

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
  public VigorInfo GetVigor(int j) { return GetVigor(new VigorInfo(), j); }
  public VigorInfo GetVigor(VigorInfo obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int VigorLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public GoldInfo GetGold(int j) { return GetGold(new GoldInfo(), j); }
  public GoldInfo GetGold(GoldInfo obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int GoldLength { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public ActionPowerInfo GetActionPower(int j) { return GetActionPower(new ActionPowerInfo(), j); }
  public ActionPowerInfo GetActionPower(ActionPowerInfo obj, int j) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ActionPowerLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }
  public BuddyExpInfo GetBuddyExp(int j) { return GetBuddyExp(new BuddyExpInfo(), j); }
  public BuddyExpInfo GetBuddyExp(BuddyExpInfo obj, int j) { int o = __offset(24); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BuddyExpLength { get { int o = __offset(24); return o != 0 ? __vector_len(o) : 0; } }
  public ActicketInfo GetActicket(int j) { return GetActicket(new ActicketInfo(), j); }
  public ActicketInfo GetActicket(ActicketInfo obj, int j) { int o = __offset(26); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ActicketLength { get { int o = __offset(26); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionResource> CreateConditionResource(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = false,
      int priority = 0,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset vigorOffset = default(VectorOffset),
      VectorOffset goldOffset = default(VectorOffset),
      VectorOffset action_powerOffset = default(VectorOffset),
      VectorOffset buddy_expOffset = default(VectorOffset),
      VectorOffset acticketOffset = default(VectorOffset)) {
    builder.StartObject(12);
    ConditionResource.AddActicket(builder, acticketOffset);
    ConditionResource.AddBuddyExp(builder, buddy_expOffset);
    ConditionResource.AddActionPower(builder, action_powerOffset);
    ConditionResource.AddGold(builder, goldOffset);
    ConditionResource.AddVigor(builder, vigorOffset);
    ConditionResource.AddOptions(builder, optionsOffset);
    ConditionResource.AddUserConditions(builder, user_conditionsOffset);
    ConditionResource.AddDateConditions(builder, date_conditionsOffset);
    ConditionResource.AddPriority(builder, priority);
    ConditionResource.AddName(builder, nameOffset);
    ConditionResource.Add_id(builder, _idOffset);
    ConditionResource.AddEnabled(builder, enabled);
    return ConditionResource.EndConditionResource(builder);
  }

  public static void StartConditionResource(FlatBufferBuilder builder) { builder.StartObject(12); }
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
  public static void AddVigor(FlatBufferBuilder builder, VectorOffset vigorOffset) { builder.AddOffset(7, vigorOffset.Value, 0); }
  public static VectorOffset CreateVigorVector(FlatBufferBuilder builder, Offset<VigorInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartVigorVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddGold(FlatBufferBuilder builder, VectorOffset goldOffset) { builder.AddOffset(8, goldOffset.Value, 0); }
  public static VectorOffset CreateGoldVector(FlatBufferBuilder builder, Offset<GoldInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartGoldVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddActionPower(FlatBufferBuilder builder, VectorOffset actionPowerOffset) { builder.AddOffset(9, actionPowerOffset.Value, 0); }
  public static VectorOffset CreateActionPowerVector(FlatBufferBuilder builder, Offset<ActionPowerInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartActionPowerVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBuddyExp(FlatBufferBuilder builder, VectorOffset buddyExpOffset) { builder.AddOffset(10, buddyExpOffset.Value, 0); }
  public static VectorOffset CreateBuddyExpVector(FlatBufferBuilder builder, Offset<BuddyExpInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBuddyExpVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddActicket(FlatBufferBuilder builder, VectorOffset acticketOffset) { builder.AddOffset(11, acticketOffset.Value, 0); }
  public static VectorOffset CreateActicketVector(FlatBufferBuilder builder, Offset<ActicketInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartActicketVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionResource> EndConditionResource(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionResource>(o);
  }
};

public sealed class Resource : Table {
  public static Resource GetRootAsResource(ByteBuffer _bb) { return GetRootAsResource(_bb, new Resource()); }
  public static Resource GetRootAsResource(ByteBuffer _bb, Resource obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Resource __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionResource GetArray(int j) { return GetArray(new ConditionResource(), j); }
  public ConditionResource GetArray(ConditionResource obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Resource> CreateResource(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Resource.AddArray(builder, arrayOffset);
    return Resource.EndResource(builder);
  }

  public static void StartResource(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionResource>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Resource> EndResource(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Resource>(o);
  }
  public static void FinishResourceBuffer(FlatBufferBuilder builder, Offset<Resource> offset) { builder.Finish(offset.Value); }
};


}
