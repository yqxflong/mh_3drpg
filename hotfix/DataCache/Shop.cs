// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class ShopInfo : Table {
  public static ShopInfo GetRootAsShopInfo(ByteBuffer _bb) { return GetRootAsShopInfo(_bb, new ShopInfo()); }
  public static ShopInfo GetRootAsShopInfo(ByteBuffer _bb, ShopInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ShopInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ShopType { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetShopTypeBytes() { return __vector_as_arraysegment(6); }
  public string ShopBalanceType { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetShopBalanceTypeBytes() { return __vector_as_arraysegment(8); }
  public string Name { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(10); }
  public int LevelLimit { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RefreshTime1 { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RefreshTime2 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RefreshTime3 { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int RefreshTime4 { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string RefreshBalanceType { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRefreshBalanceTypeBytes() { return __vector_as_arraysegment(22); }
  public int MaxItemNum { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<ShopInfo> CreateShopInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset shop_typeOffset = default(StringOffset),
      StringOffset shop_balance_typeOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      int level_limit = 0,
      int refresh_time_1 = 0,
      int refresh_time_2 = 0,
      int refresh_time_3 = 0,
      int refresh_time_4 = 0,
      StringOffset refresh_balance_typeOffset = default(StringOffset),
      int max_item_num = 0) {
    builder.StartObject(11);
    ShopInfo.AddMaxItemNum(builder, max_item_num);
    ShopInfo.AddRefreshBalanceType(builder, refresh_balance_typeOffset);
    ShopInfo.AddRefreshTime4(builder, refresh_time_4);
    ShopInfo.AddRefreshTime3(builder, refresh_time_3);
    ShopInfo.AddRefreshTime2(builder, refresh_time_2);
    ShopInfo.AddRefreshTime1(builder, refresh_time_1);
    ShopInfo.AddLevelLimit(builder, level_limit);
    ShopInfo.AddName(builder, nameOffset);
    ShopInfo.AddShopBalanceType(builder, shop_balance_typeOffset);
    ShopInfo.AddShopType(builder, shop_typeOffset);
    ShopInfo.AddId(builder, id);
    return ShopInfo.EndShopInfo(builder);
  }

  public static void StartShopInfo(FlatBufferBuilder builder) { builder.StartObject(11); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddShopType(FlatBufferBuilder builder, StringOffset shopTypeOffset) { builder.AddOffset(1, shopTypeOffset.Value, 0); }
  public static void AddShopBalanceType(FlatBufferBuilder builder, StringOffset shopBalanceTypeOffset) { builder.AddOffset(2, shopBalanceTypeOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(3, nameOffset.Value, 0); }
  public static void AddLevelLimit(FlatBufferBuilder builder, int levelLimit) { builder.AddInt(4, levelLimit, 0); }
  public static void AddRefreshTime1(FlatBufferBuilder builder, int refreshTime1) { builder.AddInt(5, refreshTime1, 0); }
  public static void AddRefreshTime2(FlatBufferBuilder builder, int refreshTime2) { builder.AddInt(6, refreshTime2, 0); }
  public static void AddRefreshTime3(FlatBufferBuilder builder, int refreshTime3) { builder.AddInt(7, refreshTime3, 0); }
  public static void AddRefreshTime4(FlatBufferBuilder builder, int refreshTime4) { builder.AddInt(8, refreshTime4, 0); }
  public static void AddRefreshBalanceType(FlatBufferBuilder builder, StringOffset refreshBalanceTypeOffset) { builder.AddOffset(9, refreshBalanceTypeOffset.Value, 0); }
  public static void AddMaxItemNum(FlatBufferBuilder builder, int maxItemNum) { builder.AddInt(10, maxItemNum, 0); }
  public static Offset<ShopInfo> EndShopInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ShopInfo>(o);
  }
};

public sealed class CommonShopInfo : Table {
  public static CommonShopInfo GetRootAsCommonShopInfo(ByteBuffer _bb) { return GetRootAsCommonShopInfo(_bb, new CommonShopInfo()); }
  public static CommonShopInfo GetRootAsCommonShopInfo(ByteBuffer _bb, CommonShopInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public CommonShopInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(6); }
  public string ItemId { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(8); }
  public string ItemName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemNameBytes() { return __vector_as_arraysegment(10); }
  public int ItemNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BalanceName { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBalanceNameBytes() { return __vector_as_arraysegment(14); }
  public int BalanceNum { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Weight { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int IsMustIn { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Position { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LevelLimitMin { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LevelLimitMax { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<CommonShopInfo> CreateCommonShopInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      StringOffset item_nameOffset = default(StringOffset),
      int item_num = 0,
      StringOffset balance_nameOffset = default(StringOffset),
      int balance_num = 0,
      int weight = 0,
      int is_must_in = 0,
      int position = 0,
      int level_limit_min = 0,
      int level_limit_max = 0) {
    builder.StartObject(12);
    CommonShopInfo.AddLevelLimitMax(builder, level_limit_max);
    CommonShopInfo.AddLevelLimitMin(builder, level_limit_min);
    CommonShopInfo.AddPosition(builder, position);
    CommonShopInfo.AddIsMustIn(builder, is_must_in);
    CommonShopInfo.AddWeight(builder, weight);
    CommonShopInfo.AddBalanceNum(builder, balance_num);
    CommonShopInfo.AddBalanceName(builder, balance_nameOffset);
    CommonShopInfo.AddItemNum(builder, item_num);
    CommonShopInfo.AddItemName(builder, item_nameOffset);
    CommonShopInfo.AddItemId(builder, item_idOffset);
    CommonShopInfo.AddItemType(builder, item_typeOffset);
    CommonShopInfo.AddId(builder, id);
    return CommonShopInfo.EndCommonShopInfo(builder);
  }

  public static void StartCommonShopInfo(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(1, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(2, itemIdOffset.Value, 0); }
  public static void AddItemName(FlatBufferBuilder builder, StringOffset itemNameOffset) { builder.AddOffset(3, itemNameOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(4, itemNum, 0); }
  public static void AddBalanceName(FlatBufferBuilder builder, StringOffset balanceNameOffset) { builder.AddOffset(5, balanceNameOffset.Value, 0); }
  public static void AddBalanceNum(FlatBufferBuilder builder, int balanceNum) { builder.AddInt(6, balanceNum, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(7, weight, 0); }
  public static void AddIsMustIn(FlatBufferBuilder builder, int isMustIn) { builder.AddInt(8, isMustIn, 0); }
  public static void AddPosition(FlatBufferBuilder builder, int position) { builder.AddInt(9, position, 0); }
  public static void AddLevelLimitMin(FlatBufferBuilder builder, int levelLimitMin) { builder.AddInt(10, levelLimitMin, 0); }
  public static void AddLevelLimitMax(FlatBufferBuilder builder, int levelLimitMax) { builder.AddInt(11, levelLimitMax, 0); }
  public static Offset<CommonShopInfo> EndCommonShopInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<CommonShopInfo>(o);
  }
};

public sealed class MysteryShopInfo : Table {
  public static MysteryShopInfo GetRootAsMysteryShopInfo(ByteBuffer _bb) { return GetRootAsMysteryShopInfo(_bb, new MysteryShopInfo()); }
  public static MysteryShopInfo GetRootAsMysteryShopInfo(ByteBuffer _bb, MysteryShopInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public MysteryShopInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(6); }
  public string ItemId { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(8); }
  public string ItemName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemNameBytes() { return __vector_as_arraysegment(10); }
  public int ItemNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BalanceName { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBalanceNameBytes() { return __vector_as_arraysegment(14); }
  public int BalanceNum { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Weight { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int IsMustIn { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Position { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<MysteryShopInfo> CreateMysteryShopInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      StringOffset item_nameOffset = default(StringOffset),
      int item_num = 0,
      StringOffset balance_nameOffset = default(StringOffset),
      int balance_num = 0,
      int weight = 0,
      int is_must_in = 0,
      int position = 0) {
    builder.StartObject(10);
    MysteryShopInfo.AddPosition(builder, position);
    MysteryShopInfo.AddIsMustIn(builder, is_must_in);
    MysteryShopInfo.AddWeight(builder, weight);
    MysteryShopInfo.AddBalanceNum(builder, balance_num);
    MysteryShopInfo.AddBalanceName(builder, balance_nameOffset);
    MysteryShopInfo.AddItemNum(builder, item_num);
    MysteryShopInfo.AddItemName(builder, item_nameOffset);
    MysteryShopInfo.AddItemId(builder, item_idOffset);
    MysteryShopInfo.AddItemType(builder, item_typeOffset);
    MysteryShopInfo.AddId(builder, id);
    return MysteryShopInfo.EndMysteryShopInfo(builder);
  }

  public static void StartMysteryShopInfo(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(1, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(2, itemIdOffset.Value, 0); }
  public static void AddItemName(FlatBufferBuilder builder, StringOffset itemNameOffset) { builder.AddOffset(3, itemNameOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(4, itemNum, 0); }
  public static void AddBalanceName(FlatBufferBuilder builder, StringOffset balanceNameOffset) { builder.AddOffset(5, balanceNameOffset.Value, 0); }
  public static void AddBalanceNum(FlatBufferBuilder builder, int balanceNum) { builder.AddInt(6, balanceNum, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(7, weight, 0); }
  public static void AddIsMustIn(FlatBufferBuilder builder, int isMustIn) { builder.AddInt(8, isMustIn, 0); }
  public static void AddPosition(FlatBufferBuilder builder, int position) { builder.AddInt(9, position, 0); }
  public static Offset<MysteryShopInfo> EndMysteryShopInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<MysteryShopInfo>(o);
  }
};

public sealed class ArenaShopInfo : Table {
  public static ArenaShopInfo GetRootAsArenaShopInfo(ByteBuffer _bb) { return GetRootAsArenaShopInfo(_bb, new ArenaShopInfo()); }
  public static ArenaShopInfo GetRootAsArenaShopInfo(ByteBuffer _bb, ArenaShopInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ArenaShopInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(6); }
  public string ItemId { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(8); }
  public string ItemName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemNameBytes() { return __vector_as_arraysegment(10); }
  public int ItemNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BalanceName { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBalanceNameBytes() { return __vector_as_arraysegment(14); }
  public int BalanceNum { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Weight { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int IsMustIn { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Position { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<ArenaShopInfo> CreateArenaShopInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      StringOffset item_nameOffset = default(StringOffset),
      int item_num = 0,
      StringOffset balance_nameOffset = default(StringOffset),
      int balance_num = 0,
      int weight = 0,
      int is_must_in = 0,
      int position = 0) {
    builder.StartObject(10);
    ArenaShopInfo.AddPosition(builder, position);
    ArenaShopInfo.AddIsMustIn(builder, is_must_in);
    ArenaShopInfo.AddWeight(builder, weight);
    ArenaShopInfo.AddBalanceNum(builder, balance_num);
    ArenaShopInfo.AddBalanceName(builder, balance_nameOffset);
    ArenaShopInfo.AddItemNum(builder, item_num);
    ArenaShopInfo.AddItemName(builder, item_nameOffset);
    ArenaShopInfo.AddItemId(builder, item_idOffset);
    ArenaShopInfo.AddItemType(builder, item_typeOffset);
    ArenaShopInfo.AddId(builder, id);
    return ArenaShopInfo.EndArenaShopInfo(builder);
  }

  public static void StartArenaShopInfo(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(1, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(2, itemIdOffset.Value, 0); }
  public static void AddItemName(FlatBufferBuilder builder, StringOffset itemNameOffset) { builder.AddOffset(3, itemNameOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(4, itemNum, 0); }
  public static void AddBalanceName(FlatBufferBuilder builder, StringOffset balanceNameOffset) { builder.AddOffset(5, balanceNameOffset.Value, 0); }
  public static void AddBalanceNum(FlatBufferBuilder builder, int balanceNum) { builder.AddInt(6, balanceNum, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(7, weight, 0); }
  public static void AddIsMustIn(FlatBufferBuilder builder, int isMustIn) { builder.AddInt(8, isMustIn, 0); }
  public static void AddPosition(FlatBufferBuilder builder, int position) { builder.AddInt(9, position, 0); }
  public static Offset<ArenaShopInfo> EndArenaShopInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ArenaShopInfo>(o);
  }
};

public sealed class LadderShopInfo : Table {
  public static LadderShopInfo GetRootAsLadderShopInfo(ByteBuffer _bb) { return GetRootAsLadderShopInfo(_bb, new LadderShopInfo()); }
  public static LadderShopInfo GetRootAsLadderShopInfo(ByteBuffer _bb, LadderShopInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public LadderShopInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(6); }
  public string ItemId { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(8); }
  public string ItemName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemNameBytes() { return __vector_as_arraysegment(10); }
  public int ItemNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BalanceName { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBalanceNameBytes() { return __vector_as_arraysegment(14); }
  public int BalanceNum { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Weight { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int IsMustIn { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Position { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<LadderShopInfo> CreateLadderShopInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      StringOffset item_nameOffset = default(StringOffset),
      int item_num = 0,
      StringOffset balance_nameOffset = default(StringOffset),
      int balance_num = 0,
      int weight = 0,
      int is_must_in = 0,
      int position = 0) {
    builder.StartObject(10);
    LadderShopInfo.AddPosition(builder, position);
    LadderShopInfo.AddIsMustIn(builder, is_must_in);
    LadderShopInfo.AddWeight(builder, weight);
    LadderShopInfo.AddBalanceNum(builder, balance_num);
    LadderShopInfo.AddBalanceName(builder, balance_nameOffset);
    LadderShopInfo.AddItemNum(builder, item_num);
    LadderShopInfo.AddItemName(builder, item_nameOffset);
    LadderShopInfo.AddItemId(builder, item_idOffset);
    LadderShopInfo.AddItemType(builder, item_typeOffset);
    LadderShopInfo.AddId(builder, id);
    return LadderShopInfo.EndLadderShopInfo(builder);
  }

  public static void StartLadderShopInfo(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(1, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(2, itemIdOffset.Value, 0); }
  public static void AddItemName(FlatBufferBuilder builder, StringOffset itemNameOffset) { builder.AddOffset(3, itemNameOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(4, itemNum, 0); }
  public static void AddBalanceName(FlatBufferBuilder builder, StringOffset balanceNameOffset) { builder.AddOffset(5, balanceNameOffset.Value, 0); }
  public static void AddBalanceNum(FlatBufferBuilder builder, int balanceNum) { builder.AddInt(6, balanceNum, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(7, weight, 0); }
  public static void AddIsMustIn(FlatBufferBuilder builder, int isMustIn) { builder.AddInt(8, isMustIn, 0); }
  public static void AddPosition(FlatBufferBuilder builder, int position) { builder.AddInt(9, position, 0); }
  public static Offset<LadderShopInfo> EndLadderShopInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<LadderShopInfo>(o);
  }
};

public sealed class ExpeditionShopInfo : Table {
  public static ExpeditionShopInfo GetRootAsExpeditionShopInfo(ByteBuffer _bb) { return GetRootAsExpeditionShopInfo(_bb, new ExpeditionShopInfo()); }
  public static ExpeditionShopInfo GetRootAsExpeditionShopInfo(ByteBuffer _bb, ExpeditionShopInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ExpeditionShopInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(6); }
  public string ItemId { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(8); }
  public string ItemName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemNameBytes() { return __vector_as_arraysegment(10); }
  public int ItemNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BalanceName { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBalanceNameBytes() { return __vector_as_arraysegment(14); }
  public int BalanceNum { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Weight { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int IsMustIn { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Position { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<ExpeditionShopInfo> CreateExpeditionShopInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      StringOffset item_nameOffset = default(StringOffset),
      int item_num = 0,
      StringOffset balance_nameOffset = default(StringOffset),
      int balance_num = 0,
      int weight = 0,
      int is_must_in = 0,
      int position = 0) {
    builder.StartObject(10);
    ExpeditionShopInfo.AddPosition(builder, position);
    ExpeditionShopInfo.AddIsMustIn(builder, is_must_in);
    ExpeditionShopInfo.AddWeight(builder, weight);
    ExpeditionShopInfo.AddBalanceNum(builder, balance_num);
    ExpeditionShopInfo.AddBalanceName(builder, balance_nameOffset);
    ExpeditionShopInfo.AddItemNum(builder, item_num);
    ExpeditionShopInfo.AddItemName(builder, item_nameOffset);
    ExpeditionShopInfo.AddItemId(builder, item_idOffset);
    ExpeditionShopInfo.AddItemType(builder, item_typeOffset);
    ExpeditionShopInfo.AddId(builder, id);
    return ExpeditionShopInfo.EndExpeditionShopInfo(builder);
  }

  public static void StartExpeditionShopInfo(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(1, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(2, itemIdOffset.Value, 0); }
  public static void AddItemName(FlatBufferBuilder builder, StringOffset itemNameOffset) { builder.AddOffset(3, itemNameOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(4, itemNum, 0); }
  public static void AddBalanceName(FlatBufferBuilder builder, StringOffset balanceNameOffset) { builder.AddOffset(5, balanceNameOffset.Value, 0); }
  public static void AddBalanceNum(FlatBufferBuilder builder, int balanceNum) { builder.AddInt(6, balanceNum, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(7, weight, 0); }
  public static void AddIsMustIn(FlatBufferBuilder builder, int isMustIn) { builder.AddInt(8, isMustIn, 0); }
  public static void AddPosition(FlatBufferBuilder builder, int position) { builder.AddInt(9, position, 0); }
  public static Offset<ExpeditionShopInfo> EndExpeditionShopInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ExpeditionShopInfo>(o);
  }
};

public sealed class AllianceShopInfo : Table {
  public static AllianceShopInfo GetRootAsAllianceShopInfo(ByteBuffer _bb) { return GetRootAsAllianceShopInfo(_bb, new AllianceShopInfo()); }
  public static AllianceShopInfo GetRootAsAllianceShopInfo(ByteBuffer _bb, AllianceShopInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AllianceShopInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(6); }
  public string ItemId { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(8); }
  public string ItemName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemNameBytes() { return __vector_as_arraysegment(10); }
  public int ItemNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BalanceName { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBalanceNameBytes() { return __vector_as_arraysegment(14); }
  public int BalanceNum { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Weight { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int IsMustIn { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Position { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<AllianceShopInfo> CreateAllianceShopInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      StringOffset item_nameOffset = default(StringOffset),
      int item_num = 0,
      StringOffset balance_nameOffset = default(StringOffset),
      int balance_num = 0,
      int weight = 0,
      int is_must_in = 0,
      int position = 0) {
    builder.StartObject(10);
    AllianceShopInfo.AddPosition(builder, position);
    AllianceShopInfo.AddIsMustIn(builder, is_must_in);
    AllianceShopInfo.AddWeight(builder, weight);
    AllianceShopInfo.AddBalanceNum(builder, balance_num);
    AllianceShopInfo.AddBalanceName(builder, balance_nameOffset);
    AllianceShopInfo.AddItemNum(builder, item_num);
    AllianceShopInfo.AddItemName(builder, item_nameOffset);
    AllianceShopInfo.AddItemId(builder, item_idOffset);
    AllianceShopInfo.AddItemType(builder, item_typeOffset);
    AllianceShopInfo.AddId(builder, id);
    return AllianceShopInfo.EndAllianceShopInfo(builder);
  }

  public static void StartAllianceShopInfo(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(1, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(2, itemIdOffset.Value, 0); }
  public static void AddItemName(FlatBufferBuilder builder, StringOffset itemNameOffset) { builder.AddOffset(3, itemNameOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(4, itemNum, 0); }
  public static void AddBalanceName(FlatBufferBuilder builder, StringOffset balanceNameOffset) { builder.AddOffset(5, balanceNameOffset.Value, 0); }
  public static void AddBalanceNum(FlatBufferBuilder builder, int balanceNum) { builder.AddInt(6, balanceNum, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(7, weight, 0); }
  public static void AddIsMustIn(FlatBufferBuilder builder, int isMustIn) { builder.AddInt(8, isMustIn, 0); }
  public static void AddPosition(FlatBufferBuilder builder, int position) { builder.AddInt(9, position, 0); }
  public static Offset<AllianceShopInfo> EndAllianceShopInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AllianceShopInfo>(o);
  }
};

public sealed class BossChallenge1ShopInfo : Table {
  public static BossChallenge1ShopInfo GetRootAsBossChallenge1ShopInfo(ByteBuffer _bb) { return GetRootAsBossChallenge1ShopInfo(_bb, new BossChallenge1ShopInfo()); }
  public static BossChallenge1ShopInfo GetRootAsBossChallenge1ShopInfo(ByteBuffer _bb, BossChallenge1ShopInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BossChallenge1ShopInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(6); }
  public string ItemId { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(8); }
  public string ItemName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemNameBytes() { return __vector_as_arraysegment(10); }
  public int ItemNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BalanceName { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBalanceNameBytes() { return __vector_as_arraysegment(14); }
  public int BalanceNum { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Weight { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int IsMustIn { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Position { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BuyNum { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<BossChallenge1ShopInfo> CreateBossChallenge1ShopInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      StringOffset item_nameOffset = default(StringOffset),
      int item_num = 0,
      StringOffset balance_nameOffset = default(StringOffset),
      int balance_num = 0,
      int weight = 0,
      int is_must_in = 0,
      int position = 0,
      int buy_num = 0) {
    builder.StartObject(11);
    BossChallenge1ShopInfo.AddBuyNum(builder, buy_num);
    BossChallenge1ShopInfo.AddPosition(builder, position);
    BossChallenge1ShopInfo.AddIsMustIn(builder, is_must_in);
    BossChallenge1ShopInfo.AddWeight(builder, weight);
    BossChallenge1ShopInfo.AddBalanceNum(builder, balance_num);
    BossChallenge1ShopInfo.AddBalanceName(builder, balance_nameOffset);
    BossChallenge1ShopInfo.AddItemNum(builder, item_num);
    BossChallenge1ShopInfo.AddItemName(builder, item_nameOffset);
    BossChallenge1ShopInfo.AddItemId(builder, item_idOffset);
    BossChallenge1ShopInfo.AddItemType(builder, item_typeOffset);
    BossChallenge1ShopInfo.AddId(builder, id);
    return BossChallenge1ShopInfo.EndBossChallenge1ShopInfo(builder);
  }

  public static void StartBossChallenge1ShopInfo(FlatBufferBuilder builder) { builder.StartObject(11); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(1, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(2, itemIdOffset.Value, 0); }
  public static void AddItemName(FlatBufferBuilder builder, StringOffset itemNameOffset) { builder.AddOffset(3, itemNameOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(4, itemNum, 0); }
  public static void AddBalanceName(FlatBufferBuilder builder, StringOffset balanceNameOffset) { builder.AddOffset(5, balanceNameOffset.Value, 0); }
  public static void AddBalanceNum(FlatBufferBuilder builder, int balanceNum) { builder.AddInt(6, balanceNum, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(7, weight, 0); }
  public static void AddIsMustIn(FlatBufferBuilder builder, int isMustIn) { builder.AddInt(8, isMustIn, 0); }
  public static void AddPosition(FlatBufferBuilder builder, int position) { builder.AddInt(9, position, 0); }
  public static void AddBuyNum(FlatBufferBuilder builder, int buyNum) { builder.AddInt(10, buyNum, 0); }
  public static Offset<BossChallenge1ShopInfo> EndBossChallenge1ShopInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BossChallenge1ShopInfo>(o);
  }
};

public sealed class BossChallenge2ShopInfo : Table {
  public static BossChallenge2ShopInfo GetRootAsBossChallenge2ShopInfo(ByteBuffer _bb) { return GetRootAsBossChallenge2ShopInfo(_bb, new BossChallenge2ShopInfo()); }
  public static BossChallenge2ShopInfo GetRootAsBossChallenge2ShopInfo(ByteBuffer _bb, BossChallenge2ShopInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BossChallenge2ShopInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(6); }
  public string ItemId { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(8); }
  public string ItemName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemNameBytes() { return __vector_as_arraysegment(10); }
  public int ItemNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BalanceName { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBalanceNameBytes() { return __vector_as_arraysegment(14); }
  public int BalanceNum { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Weight { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int IsMustIn { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Position { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BuyNum { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<BossChallenge2ShopInfo> CreateBossChallenge2ShopInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      StringOffset item_nameOffset = default(StringOffset),
      int item_num = 0,
      StringOffset balance_nameOffset = default(StringOffset),
      int balance_num = 0,
      int weight = 0,
      int is_must_in = 0,
      int position = 0,
      int buy_num = 0) {
    builder.StartObject(11);
    BossChallenge2ShopInfo.AddBuyNum(builder, buy_num);
    BossChallenge2ShopInfo.AddPosition(builder, position);
    BossChallenge2ShopInfo.AddIsMustIn(builder, is_must_in);
    BossChallenge2ShopInfo.AddWeight(builder, weight);
    BossChallenge2ShopInfo.AddBalanceNum(builder, balance_num);
    BossChallenge2ShopInfo.AddBalanceName(builder, balance_nameOffset);
    BossChallenge2ShopInfo.AddItemNum(builder, item_num);
    BossChallenge2ShopInfo.AddItemName(builder, item_nameOffset);
    BossChallenge2ShopInfo.AddItemId(builder, item_idOffset);
    BossChallenge2ShopInfo.AddItemType(builder, item_typeOffset);
    BossChallenge2ShopInfo.AddId(builder, id);
    return BossChallenge2ShopInfo.EndBossChallenge2ShopInfo(builder);
  }

  public static void StartBossChallenge2ShopInfo(FlatBufferBuilder builder) { builder.StartObject(11); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(1, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(2, itemIdOffset.Value, 0); }
  public static void AddItemName(FlatBufferBuilder builder, StringOffset itemNameOffset) { builder.AddOffset(3, itemNameOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(4, itemNum, 0); }
  public static void AddBalanceName(FlatBufferBuilder builder, StringOffset balanceNameOffset) { builder.AddOffset(5, balanceNameOffset.Value, 0); }
  public static void AddBalanceNum(FlatBufferBuilder builder, int balanceNum) { builder.AddInt(6, balanceNum, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(7, weight, 0); }
  public static void AddIsMustIn(FlatBufferBuilder builder, int isMustIn) { builder.AddInt(8, isMustIn, 0); }
  public static void AddPosition(FlatBufferBuilder builder, int position) { builder.AddInt(9, position, 0); }
  public static void AddBuyNum(FlatBufferBuilder builder, int buyNum) { builder.AddInt(10, buyNum, 0); }
  public static Offset<BossChallenge2ShopInfo> EndBossChallenge2ShopInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BossChallenge2ShopInfo>(o);
  }
};

public sealed class BossChallenge3ShopInfo : Table {
  public static BossChallenge3ShopInfo GetRootAsBossChallenge3ShopInfo(ByteBuffer _bb) { return GetRootAsBossChallenge3ShopInfo(_bb, new BossChallenge3ShopInfo()); }
  public static BossChallenge3ShopInfo GetRootAsBossChallenge3ShopInfo(ByteBuffer _bb, BossChallenge3ShopInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BossChallenge3ShopInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string ItemType { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemTypeBytes() { return __vector_as_arraysegment(6); }
  public string ItemId { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemIdBytes() { return __vector_as_arraysegment(8); }
  public string ItemName { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetItemNameBytes() { return __vector_as_arraysegment(10); }
  public int ItemNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string BalanceName { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetBalanceNameBytes() { return __vector_as_arraysegment(14); }
  public int BalanceNum { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Weight { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int IsMustIn { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Position { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int BuyNum { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<BossChallenge3ShopInfo> CreateBossChallenge3ShopInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset item_typeOffset = default(StringOffset),
      StringOffset item_idOffset = default(StringOffset),
      StringOffset item_nameOffset = default(StringOffset),
      int item_num = 0,
      StringOffset balance_nameOffset = default(StringOffset),
      int balance_num = 0,
      int weight = 0,
      int is_must_in = 0,
      int position = 0,
      int buy_num = 0) {
    builder.StartObject(11);
    BossChallenge3ShopInfo.AddBuyNum(builder, buy_num);
    BossChallenge3ShopInfo.AddPosition(builder, position);
    BossChallenge3ShopInfo.AddIsMustIn(builder, is_must_in);
    BossChallenge3ShopInfo.AddWeight(builder, weight);
    BossChallenge3ShopInfo.AddBalanceNum(builder, balance_num);
    BossChallenge3ShopInfo.AddBalanceName(builder, balance_nameOffset);
    BossChallenge3ShopInfo.AddItemNum(builder, item_num);
    BossChallenge3ShopInfo.AddItemName(builder, item_nameOffset);
    BossChallenge3ShopInfo.AddItemId(builder, item_idOffset);
    BossChallenge3ShopInfo.AddItemType(builder, item_typeOffset);
    BossChallenge3ShopInfo.AddId(builder, id);
    return BossChallenge3ShopInfo.EndBossChallenge3ShopInfo(builder);
  }

  public static void StartBossChallenge3ShopInfo(FlatBufferBuilder builder) { builder.StartObject(11); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddItemType(FlatBufferBuilder builder, StringOffset itemTypeOffset) { builder.AddOffset(1, itemTypeOffset.Value, 0); }
  public static void AddItemId(FlatBufferBuilder builder, StringOffset itemIdOffset) { builder.AddOffset(2, itemIdOffset.Value, 0); }
  public static void AddItemName(FlatBufferBuilder builder, StringOffset itemNameOffset) { builder.AddOffset(3, itemNameOffset.Value, 0); }
  public static void AddItemNum(FlatBufferBuilder builder, int itemNum) { builder.AddInt(4, itemNum, 0); }
  public static void AddBalanceName(FlatBufferBuilder builder, StringOffset balanceNameOffset) { builder.AddOffset(5, balanceNameOffset.Value, 0); }
  public static void AddBalanceNum(FlatBufferBuilder builder, int balanceNum) { builder.AddInt(6, balanceNum, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(7, weight, 0); }
  public static void AddIsMustIn(FlatBufferBuilder builder, int isMustIn) { builder.AddInt(8, isMustIn, 0); }
  public static void AddPosition(FlatBufferBuilder builder, int position) { builder.AddInt(9, position, 0); }
  public static void AddBuyNum(FlatBufferBuilder builder, int buyNum) { builder.AddInt(10, buyNum, 0); }
  public static Offset<BossChallenge3ShopInfo> EndBossChallenge3ShopInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BossChallenge3ShopInfo>(o);
  }
};

public sealed class ConditionShop : Table {
  public static ConditionShop GetRootAsConditionShop(ByteBuffer _bb) { return GetRootAsConditionShop(_bb, new ConditionShop()); }
  public static ConditionShop GetRootAsConditionShop(ByteBuffer _bb, ConditionShop obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionShop __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

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
  public ShopInfo GetShop(int j) { return GetShop(new ShopInfo(), j); }
  public ShopInfo GetShop(ShopInfo obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ShopLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public CommonShopInfo GetCommon(int j) { return GetCommon(new CommonShopInfo(), j); }
  public CommonShopInfo GetCommon(CommonShopInfo obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int CommonLength { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public MysteryShopInfo GetMystery(int j) { return GetMystery(new MysteryShopInfo(), j); }
  public MysteryShopInfo GetMystery(MysteryShopInfo obj, int j) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int MysteryLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }
  public ArenaShopInfo GetArena(int j) { return GetArena(new ArenaShopInfo(), j); }
  public ArenaShopInfo GetArena(ArenaShopInfo obj, int j) { int o = __offset(24); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArenaLength { get { int o = __offset(24); return o != 0 ? __vector_len(o) : 0; } }
  public LadderShopInfo GetLadder(int j) { return GetLadder(new LadderShopInfo(), j); }
  public LadderShopInfo GetLadder(LadderShopInfo obj, int j) { int o = __offset(26); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int LadderLength { get { int o = __offset(26); return o != 0 ? __vector_len(o) : 0; } }
  public ExpeditionShopInfo GetExpedition(int j) { return GetExpedition(new ExpeditionShopInfo(), j); }
  public ExpeditionShopInfo GetExpedition(ExpeditionShopInfo obj, int j) { int o = __offset(28); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ExpeditionLength { get { int o = __offset(28); return o != 0 ? __vector_len(o) : 0; } }
  public AllianceShopInfo GetAlliance(int j) { return GetAlliance(new AllianceShopInfo(), j); }
  public AllianceShopInfo GetAlliance(AllianceShopInfo obj, int j) { int o = __offset(30); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AllianceLength { get { int o = __offset(30); return o != 0 ? __vector_len(o) : 0; } }
  public BossChallenge1ShopInfo GetBosschallenge1(int j) { return GetBosschallenge1(new BossChallenge1ShopInfo(), j); }
  public BossChallenge1ShopInfo GetBosschallenge1(BossChallenge1ShopInfo obj, int j) { int o = __offset(32); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int Bosschallenge1Length { get { int o = __offset(32); return o != 0 ? __vector_len(o) : 0; } }
  public BossChallenge2ShopInfo GetBosschallenge2(int j) { return GetBosschallenge2(new BossChallenge2ShopInfo(), j); }
  public BossChallenge2ShopInfo GetBosschallenge2(BossChallenge2ShopInfo obj, int j) { int o = __offset(34); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int Bosschallenge2Length { get { int o = __offset(34); return o != 0 ? __vector_len(o) : 0; } }
  public BossChallenge3ShopInfo GetBosschallenge3(int j) { return GetBosschallenge3(new BossChallenge3ShopInfo(), j); }
  public BossChallenge3ShopInfo GetBosschallenge3(BossChallenge3ShopInfo obj, int j) { int o = __offset(36); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int Bosschallenge3Length { get { int o = __offset(36); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionShop> CreateConditionShop(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = false,
      int priority = 0,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset shopOffset = default(VectorOffset),
      VectorOffset commonOffset = default(VectorOffset),
      VectorOffset mysteryOffset = default(VectorOffset),
      VectorOffset arenaOffset = default(VectorOffset),
      VectorOffset ladderOffset = default(VectorOffset),
      VectorOffset expeditionOffset = default(VectorOffset),
      VectorOffset allianceOffset = default(VectorOffset),
      VectorOffset bosschallenge1Offset = default(VectorOffset),
      VectorOffset bosschallenge2Offset = default(VectorOffset),
      VectorOffset bosschallenge3Offset = default(VectorOffset)) {
    builder.StartObject(17);
    ConditionShop.AddBosschallenge3(builder, bosschallenge3Offset);
    ConditionShop.AddBosschallenge2(builder, bosschallenge2Offset);
    ConditionShop.AddBosschallenge1(builder, bosschallenge1Offset);
    ConditionShop.AddAlliance(builder, allianceOffset);
    ConditionShop.AddExpedition(builder, expeditionOffset);
    ConditionShop.AddLadder(builder, ladderOffset);
    ConditionShop.AddArena(builder, arenaOffset);
    ConditionShop.AddMystery(builder, mysteryOffset);
    ConditionShop.AddCommon(builder, commonOffset);
    ConditionShop.AddShop(builder, shopOffset);
    ConditionShop.AddOptions(builder, optionsOffset);
    ConditionShop.AddUserConditions(builder, user_conditionsOffset);
    ConditionShop.AddDateConditions(builder, date_conditionsOffset);
    ConditionShop.AddPriority(builder, priority);
    ConditionShop.AddName(builder, nameOffset);
    ConditionShop.Add_id(builder, _idOffset);
    ConditionShop.AddEnabled(builder, enabled);
    return ConditionShop.EndConditionShop(builder);
  }

  public static void StartConditionShop(FlatBufferBuilder builder) { builder.StartObject(17); }
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
  public static void AddShop(FlatBufferBuilder builder, VectorOffset shopOffset) { builder.AddOffset(7, shopOffset.Value, 0); }
  public static VectorOffset CreateShopVector(FlatBufferBuilder builder, Offset<ShopInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartShopVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddCommon(FlatBufferBuilder builder, VectorOffset commonOffset) { builder.AddOffset(8, commonOffset.Value, 0); }
  public static VectorOffset CreateCommonVector(FlatBufferBuilder builder, Offset<CommonShopInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartCommonVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddMystery(FlatBufferBuilder builder, VectorOffset mysteryOffset) { builder.AddOffset(9, mysteryOffset.Value, 0); }
  public static VectorOffset CreateMysteryVector(FlatBufferBuilder builder, Offset<MysteryShopInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartMysteryVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddArena(FlatBufferBuilder builder, VectorOffset arenaOffset) { builder.AddOffset(10, arenaOffset.Value, 0); }
  public static VectorOffset CreateArenaVector(FlatBufferBuilder builder, Offset<ArenaShopInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArenaVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddLadder(FlatBufferBuilder builder, VectorOffset ladderOffset) { builder.AddOffset(11, ladderOffset.Value, 0); }
  public static VectorOffset CreateLadderVector(FlatBufferBuilder builder, Offset<LadderShopInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartLadderVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddExpedition(FlatBufferBuilder builder, VectorOffset expeditionOffset) { builder.AddOffset(12, expeditionOffset.Value, 0); }
  public static VectorOffset CreateExpeditionVector(FlatBufferBuilder builder, Offset<ExpeditionShopInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartExpeditionVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAlliance(FlatBufferBuilder builder, VectorOffset allianceOffset) { builder.AddOffset(13, allianceOffset.Value, 0); }
  public static VectorOffset CreateAllianceVector(FlatBufferBuilder builder, Offset<AllianceShopInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAllianceVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBosschallenge1(FlatBufferBuilder builder, VectorOffset bosschallenge1Offset) { builder.AddOffset(14, bosschallenge1Offset.Value, 0); }
  public static VectorOffset CreateBosschallenge1Vector(FlatBufferBuilder builder, Offset<BossChallenge1ShopInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBosschallenge1Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBosschallenge2(FlatBufferBuilder builder, VectorOffset bosschallenge2Offset) { builder.AddOffset(15, bosschallenge2Offset.Value, 0); }
  public static VectorOffset CreateBosschallenge2Vector(FlatBufferBuilder builder, Offset<BossChallenge2ShopInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBosschallenge2Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBosschallenge3(FlatBufferBuilder builder, VectorOffset bosschallenge3Offset) { builder.AddOffset(16, bosschallenge3Offset.Value, 0); }
  public static VectorOffset CreateBosschallenge3Vector(FlatBufferBuilder builder, Offset<BossChallenge3ShopInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBosschallenge3Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionShop> EndConditionShop(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionShop>(o);
  }
};

public sealed class Shop : Table {
  public static Shop GetRootAsShop(ByteBuffer _bb) { return GetRootAsShop(_bb, new Shop()); }
  public static Shop GetRootAsShop(ByteBuffer _bb, Shop obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Shop __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionShop GetArray(int j) { return GetArray(new ConditionShop(), j); }
  public ConditionShop GetArray(ConditionShop obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Shop> CreateShop(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Shop.AddArray(builder, arrayOffset);
    return Shop.EndShop(builder);
  }

  public static void StartShop(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionShop>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Shop> EndShop(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Shop>(o);
  }
  public static void FinishShopBuffer(FlatBufferBuilder builder, Offset<Shop> offset) { builder.Finish(offset.Value); }
};


}
