// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class GenericItem : Table {
  public static GenericItem GetRootAsGenericItem(ByteBuffer _bb) { return GetRootAsGenericItem(_bb, new GenericItem()); }
  public static GenericItem GetRootAsGenericItem(ByteBuffer _bb, GenericItem obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public GenericItem __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string System { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSystemBytes() { return __vector_as_arraysegment(8); }
  public string IconId { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconIdBytes() { return __vector_as_arraysegment(10); }
  public int QualityLevel { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Desc { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(14); }
  public string Desc2 { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDesc2Bytes() { return __vector_as_arraysegment(16); }
  public int Exp { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool CanUse { get { int o = __offset(20); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int MaxOverlap { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ShowInInventory { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string DropChickId1 { get { int o = __offset(26); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId1Bytes() { return __vector_as_arraysegment(26); }
  public string DropChickId2 { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId2Bytes() { return __vector_as_arraysegment(28); }
  public string DropChickId3 { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId3Bytes() { return __vector_as_arraysegment(30); }
  public string DropDesc { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropDescBytes() { return __vector_as_arraysegment(32); }
  public bool AutoUse { get { int o = __offset(34); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public string DropBox { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropBoxBytes() { return __vector_as_arraysegment(36); }
  public string CompoundItem { get { int o = __offset(38); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCompoundItemBytes() { return __vector_as_arraysegment(38); }
  public int NeedNum { get { int o = __offset(40); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<GenericItem> CreateGenericItem(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      StringOffset systemOffset = default(StringOffset),
      StringOffset icon_idOffset = default(StringOffset),
      int quality_level = 0,
      StringOffset descOffset = default(StringOffset),
      StringOffset desc2Offset = default(StringOffset),
      int exp = 0,
      bool can_use = false,
      int max_overlap = 0,
      int show_in_inventory = 0,
      StringOffset drop_chick_id_1Offset = default(StringOffset),
      StringOffset drop_chick_id_2Offset = default(StringOffset),
      StringOffset drop_chick_id_3Offset = default(StringOffset),
      StringOffset drop_descOffset = default(StringOffset),
      bool auto_use = false,
      StringOffset drop_boxOffset = default(StringOffset),
      StringOffset compound_itemOffset = default(StringOffset),
      int need_num = 0) {
    builder.StartObject(19);
    GenericItem.AddNeedNum(builder, need_num);
    GenericItem.AddCompoundItem(builder, compound_itemOffset);
    GenericItem.AddDropBox(builder, drop_boxOffset);
    GenericItem.AddDropDesc(builder, drop_descOffset);
    GenericItem.AddDropChickId3(builder, drop_chick_id_3Offset);
    GenericItem.AddDropChickId2(builder, drop_chick_id_2Offset);
    GenericItem.AddDropChickId1(builder, drop_chick_id_1Offset);
    GenericItem.AddShowInInventory(builder, show_in_inventory);
    GenericItem.AddMaxOverlap(builder, max_overlap);
    GenericItem.AddExp(builder, exp);
    GenericItem.AddDesc2(builder, desc2Offset);
    GenericItem.AddDesc(builder, descOffset);
    GenericItem.AddQualityLevel(builder, quality_level);
    GenericItem.AddIconId(builder, icon_idOffset);
    GenericItem.AddSystem(builder, systemOffset);
    GenericItem.AddName(builder, nameOffset);
    GenericItem.AddId(builder, idOffset);
    GenericItem.AddAutoUse(builder, auto_use);
    GenericItem.AddCanUse(builder, can_use);
    return GenericItem.EndGenericItem(builder);
  }

  public static void StartGenericItem(FlatBufferBuilder builder) { builder.StartObject(19); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddSystem(FlatBufferBuilder builder, StringOffset systemOffset) { builder.AddOffset(2, systemOffset.Value, 0); }
  public static void AddIconId(FlatBufferBuilder builder, StringOffset iconIdOffset) { builder.AddOffset(3, iconIdOffset.Value, 0); }
  public static void AddQualityLevel(FlatBufferBuilder builder, int qualityLevel) { builder.AddInt(4, qualityLevel, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(5, descOffset.Value, 0); }
  public static void AddDesc2(FlatBufferBuilder builder, StringOffset desc2Offset) { builder.AddOffset(6, desc2Offset.Value, 0); }
  public static void AddExp(FlatBufferBuilder builder, int exp) { builder.AddInt(7, exp, 0); }
  public static void AddCanUse(FlatBufferBuilder builder, bool canUse) { builder.AddBool(8, canUse, false); }
  public static void AddMaxOverlap(FlatBufferBuilder builder, int maxOverlap) { builder.AddInt(9, maxOverlap, 0); }
  public static void AddShowInInventory(FlatBufferBuilder builder, int showInInventory) { builder.AddInt(10, showInInventory, 0); }
  public static void AddDropChickId1(FlatBufferBuilder builder, StringOffset dropChickId1Offset) { builder.AddOffset(11, dropChickId1Offset.Value, 0); }
  public static void AddDropChickId2(FlatBufferBuilder builder, StringOffset dropChickId2Offset) { builder.AddOffset(12, dropChickId2Offset.Value, 0); }
  public static void AddDropChickId3(FlatBufferBuilder builder, StringOffset dropChickId3Offset) { builder.AddOffset(13, dropChickId3Offset.Value, 0); }
  public static void AddDropDesc(FlatBufferBuilder builder, StringOffset dropDescOffset) { builder.AddOffset(14, dropDescOffset.Value, 0); }
  public static void AddAutoUse(FlatBufferBuilder builder, bool autoUse) { builder.AddBool(15, autoUse, false); }
  public static void AddDropBox(FlatBufferBuilder builder, StringOffset dropBoxOffset) { builder.AddOffset(16, dropBoxOffset.Value, 0); }
  public static void AddCompoundItem(FlatBufferBuilder builder, StringOffset compoundItemOffset) { builder.AddOffset(17, compoundItemOffset.Value, 0); }
  public static void AddNeedNum(FlatBufferBuilder builder, int needNum) { builder.AddInt(18, needNum, 0); }
  public static Offset<GenericItem> EndGenericItem(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<GenericItem>(o);
  }
};

public sealed class EquipmentItem : Table {
  public static EquipmentItem GetRootAsEquipmentItem(ByteBuffer _bb) { return GetRootAsEquipmentItem(_bb, new EquipmentItem()); }
  public static EquipmentItem GetRootAsEquipmentItem(ByteBuffer _bb, EquipmentItem obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public EquipmentItem __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public string System { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSystemBytes() { return __vector_as_arraysegment(8); }
  public int QualityLevel { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Slot { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSlotBytes() { return __vector_as_arraysegment(12); }
  public string Desc { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(14); }
  public string IconId { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconIdBytes() { return __vector_as_arraysegment(16); }
  public string SuitType { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSuitTypeBytes() { return __vector_as_arraysegment(18); }
  public string MainAttributeId { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetMainAttributeIdBytes() { return __vector_as_arraysegment(20); }
  public string ExAttributeId { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetExAttributeIdBytes() { return __vector_as_arraysegment(22); }
  public int SuitAttributeId1 { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SuitAttributeId2 { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string DropChickId1 { get { int o = __offset(28); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId1Bytes() { return __vector_as_arraysegment(28); }
  public string DropChickId2 { get { int o = __offset(30); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId2Bytes() { return __vector_as_arraysegment(30); }
  public string DropChickId3 { get { int o = __offset(32); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId3Bytes() { return __vector_as_arraysegment(32); }
  public int Exp { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string SuitIcon { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSuitIconBytes() { return __vector_as_arraysegment(36); }
  public string SuitName { get { int o = __offset(38); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSuitNameBytes() { return __vector_as_arraysegment(38); }
  public string SuitDropId { get { int o = __offset(40); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetSuitDropIdBytes() { return __vector_as_arraysegment(40); }

  public static Offset<EquipmentItem> CreateEquipmentItem(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      StringOffset systemOffset = default(StringOffset),
      int quality_level = 0,
      StringOffset slotOffset = default(StringOffset),
      StringOffset descOffset = default(StringOffset),
      StringOffset icon_idOffset = default(StringOffset),
      StringOffset suit_typeOffset = default(StringOffset),
      StringOffset main_attribute_idOffset = default(StringOffset),
      StringOffset ex_attribute_idOffset = default(StringOffset),
      int suit_attribute_id_1 = 0,
      int suit_attribute_id_2 = 0,
      StringOffset drop_chick_id_1Offset = default(StringOffset),
      StringOffset drop_chick_id_2Offset = default(StringOffset),
      StringOffset drop_chick_id_3Offset = default(StringOffset),
      int exp = 0,
      StringOffset suit_iconOffset = default(StringOffset),
      StringOffset suit_nameOffset = default(StringOffset),
      StringOffset suit_drop_idOffset = default(StringOffset)) {
    builder.StartObject(19);
    EquipmentItem.AddSuitDropId(builder, suit_drop_idOffset);
    EquipmentItem.AddSuitName(builder, suit_nameOffset);
    EquipmentItem.AddSuitIcon(builder, suit_iconOffset);
    EquipmentItem.AddExp(builder, exp);
    EquipmentItem.AddDropChickId3(builder, drop_chick_id_3Offset);
    EquipmentItem.AddDropChickId2(builder, drop_chick_id_2Offset);
    EquipmentItem.AddDropChickId1(builder, drop_chick_id_1Offset);
    EquipmentItem.AddSuitAttributeId2(builder, suit_attribute_id_2);
    EquipmentItem.AddSuitAttributeId1(builder, suit_attribute_id_1);
    EquipmentItem.AddExAttributeId(builder, ex_attribute_idOffset);
    EquipmentItem.AddMainAttributeId(builder, main_attribute_idOffset);
    EquipmentItem.AddSuitType(builder, suit_typeOffset);
    EquipmentItem.AddIconId(builder, icon_idOffset);
    EquipmentItem.AddDesc(builder, descOffset);
    EquipmentItem.AddSlot(builder, slotOffset);
    EquipmentItem.AddQualityLevel(builder, quality_level);
    EquipmentItem.AddSystem(builder, systemOffset);
    EquipmentItem.AddName(builder, nameOffset);
    EquipmentItem.AddId(builder, idOffset);
    return EquipmentItem.EndEquipmentItem(builder);
  }

  public static void StartEquipmentItem(FlatBufferBuilder builder) { builder.StartObject(19); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddSystem(FlatBufferBuilder builder, StringOffset systemOffset) { builder.AddOffset(2, systemOffset.Value, 0); }
  public static void AddQualityLevel(FlatBufferBuilder builder, int qualityLevel) { builder.AddInt(3, qualityLevel, 0); }
  public static void AddSlot(FlatBufferBuilder builder, StringOffset slotOffset) { builder.AddOffset(4, slotOffset.Value, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(5, descOffset.Value, 0); }
  public static void AddIconId(FlatBufferBuilder builder, StringOffset iconIdOffset) { builder.AddOffset(6, iconIdOffset.Value, 0); }
  public static void AddSuitType(FlatBufferBuilder builder, StringOffset suitTypeOffset) { builder.AddOffset(7, suitTypeOffset.Value, 0); }
  public static void AddMainAttributeId(FlatBufferBuilder builder, StringOffset mainAttributeIdOffset) { builder.AddOffset(8, mainAttributeIdOffset.Value, 0); }
  public static void AddExAttributeId(FlatBufferBuilder builder, StringOffset exAttributeIdOffset) { builder.AddOffset(9, exAttributeIdOffset.Value, 0); }
  public static void AddSuitAttributeId1(FlatBufferBuilder builder, int suitAttributeId1) { builder.AddInt(10, suitAttributeId1, 0); }
  public static void AddSuitAttributeId2(FlatBufferBuilder builder, int suitAttributeId2) { builder.AddInt(11, suitAttributeId2, 0); }
  public static void AddDropChickId1(FlatBufferBuilder builder, StringOffset dropChickId1Offset) { builder.AddOffset(12, dropChickId1Offset.Value, 0); }
  public static void AddDropChickId2(FlatBufferBuilder builder, StringOffset dropChickId2Offset) { builder.AddOffset(13, dropChickId2Offset.Value, 0); }
  public static void AddDropChickId3(FlatBufferBuilder builder, StringOffset dropChickId3Offset) { builder.AddOffset(14, dropChickId3Offset.Value, 0); }
  public static void AddExp(FlatBufferBuilder builder, int exp) { builder.AddInt(15, exp, 0); }
  public static void AddSuitIcon(FlatBufferBuilder builder, StringOffset suitIconOffset) { builder.AddOffset(16, suitIconOffset.Value, 0); }
  public static void AddSuitName(FlatBufferBuilder builder, StringOffset suitNameOffset) { builder.AddOffset(17, suitNameOffset.Value, 0); }
  public static void AddSuitDropId(FlatBufferBuilder builder, StringOffset suitDropIdOffset) { builder.AddOffset(18, suitDropIdOffset.Value, 0); }
  public static Offset<EquipmentItem> EndEquipmentItem(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EquipmentItem>(o);
  }
};

public sealed class EquipAttributePool : Table {
  public static EquipAttributePool GetRootAsEquipAttributePool(ByteBuffer _bb) { return GetRootAsEquipAttributePool(_bb, new EquipAttributePool()); }
  public static EquipAttributePool GetRootAsEquipAttributePool(ByteBuffer _bb, EquipAttributePool obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public EquipAttributePool __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Slot { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Star { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Rate { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int AttributeId { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<EquipAttributePool> CreateEquipAttributePool(FlatBufferBuilder builder,
      int id = 0,
      int slot = 0,
      int star = 0,
      float rate = 0,
      int attribute_id = 0) {
    builder.StartObject(5);
    EquipAttributePool.AddAttributeId(builder, attribute_id);
    EquipAttributePool.AddRate(builder, rate);
    EquipAttributePool.AddStar(builder, star);
    EquipAttributePool.AddSlot(builder, slot);
    EquipAttributePool.AddId(builder, id);
    return EquipAttributePool.EndEquipAttributePool(builder);
  }

  public static void StartEquipAttributePool(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddSlot(FlatBufferBuilder builder, int slot) { builder.AddInt(1, slot, 0); }
  public static void AddStar(FlatBufferBuilder builder, int star) { builder.AddInt(2, star, 0); }
  public static void AddRate(FlatBufferBuilder builder, float rate) { builder.AddFloat(3, rate, 0); }
  public static void AddAttributeId(FlatBufferBuilder builder, int attributeId) { builder.AddInt(4, attributeId, 0); }
  public static Offset<EquipAttributePool> EndEquipAttributePool(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EquipAttributePool>(o);
  }
};

public sealed class EquipAttribute : Table {
  public static EquipAttribute GetRootAsEquipAttribute(ByteBuffer _bb) { return GetRootAsEquipAttribute(_bb, new EquipAttribute()); }
  public static EquipAttribute GetRootAsEquipAttribute(ByteBuffer _bb, EquipAttribute obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public EquipAttribute __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Star { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Desc { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(8); }
  public float MinValue { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float MaxValue { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float AddValue { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float FinalValue { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<EquipAttribute> CreateEquipAttribute(FlatBufferBuilder builder,
      int id = 0,
      int star = 0,
      StringOffset descOffset = default(StringOffset),
      float min_value = 0,
      float max_value = 0,
      float add_value = 0,
      float final_value = 0) {
    builder.StartObject(7);
    EquipAttribute.AddFinalValue(builder, final_value);
    EquipAttribute.AddAddValue(builder, add_value);
    EquipAttribute.AddMaxValue(builder, max_value);
    EquipAttribute.AddMinValue(builder, min_value);
    EquipAttribute.AddDesc(builder, descOffset);
    EquipAttribute.AddStar(builder, star);
    EquipAttribute.AddId(builder, id);
    return EquipAttribute.EndEquipAttribute(builder);
  }

  public static void StartEquipAttribute(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddStar(FlatBufferBuilder builder, int star) { builder.AddInt(1, star, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(2, descOffset.Value, 0); }
  public static void AddMinValue(FlatBufferBuilder builder, float minValue) { builder.AddFloat(3, minValue, 0); }
  public static void AddMaxValue(FlatBufferBuilder builder, float maxValue) { builder.AddFloat(4, maxValue, 0); }
  public static void AddAddValue(FlatBufferBuilder builder, float addValue) { builder.AddFloat(5, addValue, 0); }
  public static void AddFinalValue(FlatBufferBuilder builder, float finalValue) { builder.AddFloat(6, finalValue, 0); }
  public static Offset<EquipAttribute> EndEquipAttribute(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EquipAttribute>(o);
  }
};

public sealed class EquipAttributeRate : Table {
  public static EquipAttributeRate GetRootAsEquipAttributeRate(ByteBuffer _bb) { return GetRootAsEquipAttributeRate(_bb, new EquipAttributeRate()); }
  public static EquipAttributeRate GetRootAsEquipAttributeRate(ByteBuffer _bb, EquipAttributeRate obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public EquipAttributeRate __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Star { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Rating0 { get { int o = __offset(6); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Rating1 { get { int o = __offset(8); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Rating2 { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Rating3 { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Rating4 { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<EquipAttributeRate> CreateEquipAttributeRate(FlatBufferBuilder builder,
      int star = 0,
      float rating_0 = 0,
      float rating_1 = 0,
      float rating_2 = 0,
      float rating_3 = 0,
      float rating_4 = 0) {
    builder.StartObject(6);
    EquipAttributeRate.AddRating4(builder, rating_4);
    EquipAttributeRate.AddRating3(builder, rating_3);
    EquipAttributeRate.AddRating2(builder, rating_2);
    EquipAttributeRate.AddRating1(builder, rating_1);
    EquipAttributeRate.AddRating0(builder, rating_0);
    EquipAttributeRate.AddStar(builder, star);
    return EquipAttributeRate.EndEquipAttributeRate(builder);
  }

  public static void StartEquipAttributeRate(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddStar(FlatBufferBuilder builder, int star) { builder.AddInt(0, star, 0); }
  public static void AddRating0(FlatBufferBuilder builder, float rating0) { builder.AddFloat(1, rating0, 0); }
  public static void AddRating1(FlatBufferBuilder builder, float rating1) { builder.AddFloat(2, rating1, 0); }
  public static void AddRating2(FlatBufferBuilder builder, float rating2) { builder.AddFloat(3, rating2, 0); }
  public static void AddRating3(FlatBufferBuilder builder, float rating3) { builder.AddFloat(4, rating3, 0); }
  public static void AddRating4(FlatBufferBuilder builder, float rating4) { builder.AddFloat(5, rating4, 0); }
  public static Offset<EquipAttributeRate> EndEquipAttributeRate(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EquipAttributeRate>(o);
  }
};

public sealed class SuitAttribute : Table {
  public static SuitAttribute GetRootAsSuitAttribute(ByteBuffer _bb) { return GetRootAsSuitAttribute(_bb, new SuitAttribute()); }
  public static SuitAttribute GetRootAsSuitAttribute(ByteBuffer _bb, SuitAttribute obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SuitAttribute __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Desc { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(6); }
  public string Attr { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetAttrBytes() { return __vector_as_arraysegment(8); }
  public float Value { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int All { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<SuitAttribute> CreateSuitAttribute(FlatBufferBuilder builder,
      int id = 0,
      StringOffset descOffset = default(StringOffset),
      StringOffset attrOffset = default(StringOffset),
      float value = 0,
      int all = 0) {
    builder.StartObject(5);
    SuitAttribute.AddAll(builder, all);
    SuitAttribute.AddValue(builder, value);
    SuitAttribute.AddAttr(builder, attrOffset);
    SuitAttribute.AddDesc(builder, descOffset);
    SuitAttribute.AddId(builder, id);
    return SuitAttribute.EndSuitAttribute(builder);
  }

  public static void StartSuitAttribute(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(1, descOffset.Value, 0); }
  public static void AddAttr(FlatBufferBuilder builder, StringOffset attrOffset) { builder.AddOffset(2, attrOffset.Value, 0); }
  public static void AddValue(FlatBufferBuilder builder, float value) { builder.AddFloat(3, value, 0); }
  public static void AddAll(FlatBufferBuilder builder, int all) { builder.AddInt(4, all, 0); }
  public static Offset<SuitAttribute> EndSuitAttribute(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SuitAttribute>(o);
  }
};

public sealed class AnimalInfo : Table {
  public static AnimalInfo GetRootAsAnimalInfo(ByteBuffer _bb) { return GetRootAsAnimalInfo(_bb, new AnimalInfo()); }
  public static AnimalInfo GetRootAsAnimalInfo(ByteBuffer _bb, AnimalInfo obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AnimalInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }
  public int Quality { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ShardNum { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GoldNum { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Icon { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(14); }
  public string Intruduction { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIntruductionBytes() { return __vector_as_arraysegment(16); }
  public string DropChickId1 { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId1Bytes() { return __vector_as_arraysegment(18); }
  public string DropChickId2 { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId2Bytes() { return __vector_as_arraysegment(20); }
  public string DropChickId3 { get { int o = __offset(22); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId3Bytes() { return __vector_as_arraysegment(22); }

  public static Offset<AnimalInfo> CreateAnimalInfo(FlatBufferBuilder builder,
      int id = 0,
      StringOffset nameOffset = default(StringOffset),
      int quality = 0,
      int shard_num = 0,
      int gold_num = 0,
      StringOffset iconOffset = default(StringOffset),
      StringOffset intruductionOffset = default(StringOffset),
      StringOffset drop_chick_id_1Offset = default(StringOffset),
      StringOffset drop_chick_id_2Offset = default(StringOffset),
      StringOffset drop_chick_id_3Offset = default(StringOffset)) {
    builder.StartObject(10);
    AnimalInfo.AddDropChickId3(builder, drop_chick_id_3Offset);
    AnimalInfo.AddDropChickId2(builder, drop_chick_id_2Offset);
    AnimalInfo.AddDropChickId1(builder, drop_chick_id_1Offset);
    AnimalInfo.AddIntruduction(builder, intruductionOffset);
    AnimalInfo.AddIcon(builder, iconOffset);
    AnimalInfo.AddGoldNum(builder, gold_num);
    AnimalInfo.AddShardNum(builder, shard_num);
    AnimalInfo.AddQuality(builder, quality);
    AnimalInfo.AddName(builder, nameOffset);
    AnimalInfo.AddId(builder, id);
    return AnimalInfo.EndAnimalInfo(builder);
  }

  public static void StartAnimalInfo(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static void AddQuality(FlatBufferBuilder builder, int quality) { builder.AddInt(2, quality, 0); }
  public static void AddShardNum(FlatBufferBuilder builder, int shardNum) { builder.AddInt(3, shardNum, 0); }
  public static void AddGoldNum(FlatBufferBuilder builder, int goldNum) { builder.AddInt(4, goldNum, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(5, iconOffset.Value, 0); }
  public static void AddIntruduction(FlatBufferBuilder builder, StringOffset intruductionOffset) { builder.AddOffset(6, intruductionOffset.Value, 0); }
  public static void AddDropChickId1(FlatBufferBuilder builder, StringOffset dropChickId1Offset) { builder.AddOffset(7, dropChickId1Offset.Value, 0); }
  public static void AddDropChickId2(FlatBufferBuilder builder, StringOffset dropChickId2Offset) { builder.AddOffset(8, dropChickId2Offset.Value, 0); }
  public static void AddDropChickId3(FlatBufferBuilder builder, StringOffset dropChickId3Offset) { builder.AddOffset(9, dropChickId3Offset.Value, 0); }
  public static Offset<AnimalInfo> EndAnimalInfo(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AnimalInfo>(o);
  }
};

public sealed class AnimalUpGrade : Table {
  public static AnimalUpGrade GetRootAsAnimalUpGrade(ByteBuffer _bb) { return GetRootAsAnimalUpGrade(_bb, new AnimalUpGrade()); }
  public static AnimalUpGrade GetRootAsAnimalUpGrade(ByteBuffer _bb, AnimalUpGrade obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AnimalUpGrade __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int InfoId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Grade { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillId { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int NextGradeId { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Material1 { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Quantity1 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Material2 { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Quantity2 { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Material3 { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Quantity3 { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int UnlockLevel { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<AnimalUpGrade> CreateAnimalUpGrade(FlatBufferBuilder builder,
      int id = 0,
      int info_id = 0,
      int grade = 0,
      int skill_id = 0,
      int next_grade_id = 0,
      int material_1 = 0,
      int quantity_1 = 0,
      int material_2 = 0,
      int quantity_2 = 0,
      int material_3 = 0,
      int quantity_3 = 0,
      int unlock_level = 0) {
    builder.StartObject(12);
    AnimalUpGrade.AddUnlockLevel(builder, unlock_level);
    AnimalUpGrade.AddQuantity3(builder, quantity_3);
    AnimalUpGrade.AddMaterial3(builder, material_3);
    AnimalUpGrade.AddQuantity2(builder, quantity_2);
    AnimalUpGrade.AddMaterial2(builder, material_2);
    AnimalUpGrade.AddQuantity1(builder, quantity_1);
    AnimalUpGrade.AddMaterial1(builder, material_1);
    AnimalUpGrade.AddNextGradeId(builder, next_grade_id);
    AnimalUpGrade.AddSkillId(builder, skill_id);
    AnimalUpGrade.AddGrade(builder, grade);
    AnimalUpGrade.AddInfoId(builder, info_id);
    AnimalUpGrade.AddId(builder, id);
    return AnimalUpGrade.EndAnimalUpGrade(builder);
  }

  public static void StartAnimalUpGrade(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddInfoId(FlatBufferBuilder builder, int infoId) { builder.AddInt(1, infoId, 0); }
  public static void AddGrade(FlatBufferBuilder builder, int grade) { builder.AddInt(2, grade, 0); }
  public static void AddSkillId(FlatBufferBuilder builder, int skillId) { builder.AddInt(3, skillId, 0); }
  public static void AddNextGradeId(FlatBufferBuilder builder, int nextGradeId) { builder.AddInt(4, nextGradeId, 0); }
  public static void AddMaterial1(FlatBufferBuilder builder, int material1) { builder.AddInt(5, material1, 0); }
  public static void AddQuantity1(FlatBufferBuilder builder, int quantity1) { builder.AddInt(6, quantity1, 0); }
  public static void AddMaterial2(FlatBufferBuilder builder, int material2) { builder.AddInt(7, material2, 0); }
  public static void AddQuantity2(FlatBufferBuilder builder, int quantity2) { builder.AddInt(8, quantity2, 0); }
  public static void AddMaterial3(FlatBufferBuilder builder, int material3) { builder.AddInt(9, material3, 0); }
  public static void AddQuantity3(FlatBufferBuilder builder, int quantity3) { builder.AddInt(10, quantity3, 0); }
  public static void AddUnlockLevel(FlatBufferBuilder builder, int unlockLevel) { builder.AddInt(11, unlockLevel, 0); }
  public static Offset<AnimalUpGrade> EndAnimalUpGrade(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AnimalUpGrade>(o);
  }
};

public sealed class AnimalLevelUp : Table {
  public static AnimalLevelUp GetRootAsAnimalLevelUp(ByteBuffer _bb) { return GetRootAsAnimalLevelUp(_bb, new AnimalLevelUp()); }
  public static AnimalLevelUp GetRootAsAnimalLevelUp(ByteBuffer _bb, AnimalLevelUp obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AnimalLevelUp __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Level { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int NeedExp { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MaxHP { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PATK { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PDEF { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MATK { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MDEF { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Speed { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Critical { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CriticalHit { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Stun { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<AnimalLevelUp> CreateAnimalLevelUp(FlatBufferBuilder builder,
      int id = 0,
      int level = 0,
      int need_exp = 0,
      int MaxHP = 0,
      int PATK = 0,
      int PDEF = 0,
      int MATK = 0,
      int MDEF = 0,
      int speed = 0,
      int critical = 0,
      int critical_hit = 0,
      int stun = 0) {
    builder.StartObject(12);
    AnimalLevelUp.AddStun(builder, stun);
    AnimalLevelUp.AddCriticalHit(builder, critical_hit);
    AnimalLevelUp.AddCritical(builder, critical);
    AnimalLevelUp.AddSpeed(builder, speed);
    AnimalLevelUp.AddMDEF(builder, MDEF);
    AnimalLevelUp.AddMATK(builder, MATK);
    AnimalLevelUp.AddPDEF(builder, PDEF);
    AnimalLevelUp.AddPATK(builder, PATK);
    AnimalLevelUp.AddMaxHP(builder, MaxHP);
    AnimalLevelUp.AddNeedExp(builder, need_exp);
    AnimalLevelUp.AddLevel(builder, level);
    AnimalLevelUp.AddId(builder, id);
    return AnimalLevelUp.EndAnimalLevelUp(builder);
  }

  public static void StartAnimalLevelUp(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(1, level, 0); }
  public static void AddNeedExp(FlatBufferBuilder builder, int needExp) { builder.AddInt(2, needExp, 0); }
  public static void AddMaxHP(FlatBufferBuilder builder, int MaxHP) { builder.AddInt(3, MaxHP, 0); }
  public static void AddPATK(FlatBufferBuilder builder, int PATK) { builder.AddInt(4, PATK, 0); }
  public static void AddPDEF(FlatBufferBuilder builder, int PDEF) { builder.AddInt(5, PDEF, 0); }
  public static void AddMATK(FlatBufferBuilder builder, int MATK) { builder.AddInt(6, MATK, 0); }
  public static void AddMDEF(FlatBufferBuilder builder, int MDEF) { builder.AddInt(7, MDEF, 0); }
  public static void AddSpeed(FlatBufferBuilder builder, int speed) { builder.AddInt(8, speed, 0); }
  public static void AddCritical(FlatBufferBuilder builder, int critical) { builder.AddInt(9, critical, 0); }
  public static void AddCriticalHit(FlatBufferBuilder builder, int criticalHit) { builder.AddInt(10, criticalHit, 0); }
  public static void AddStun(FlatBufferBuilder builder, int stun) { builder.AddInt(11, stun, 0); }
  public static Offset<AnimalLevelUp> EndAnimalLevelUp(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AnimalLevelUp>(o);
  }
};

public sealed class Wing : Table {
  public static Wing GetRootAsWing(ByteBuffer _bb) { return GetRootAsWing(_bb, new Wing()); }
  public static Wing GetRootAsWing(ByteBuffer _bb, Wing obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Wing __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int WingId { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ModelId { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MaxHP { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PATK { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int PDEF { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MATK { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int MDEF { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Speed { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Critical { get { int o = __offset(20); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CriticalHit { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Stun { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LevelUpNeedId { get { int o = __offset(26); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LevelUpNeedNum { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int WingLevel { get { int o = __offset(30); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int LevelUpNeedExp { get { int o = __offset(32); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int NextLevelId { get { int o = __offset(34); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<Wing> CreateWing(FlatBufferBuilder builder,
      int wing_id = 0,
      int model_id = 0,
      int MaxHP = 0,
      int PATK = 0,
      int PDEF = 0,
      int MATK = 0,
      int MDEF = 0,
      int Speed = 0,
      int Critical = 0,
      int Critical_hit = 0,
      int Stun = 0,
      int level_up_need_id = 0,
      int level_up_need_num = 0,
      int wing_level = 0,
      int level_up_need_exp = 0,
      int next_level_id = 0) {
    builder.StartObject(16);
    Wing.AddNextLevelId(builder, next_level_id);
    Wing.AddLevelUpNeedExp(builder, level_up_need_exp);
    Wing.AddWingLevel(builder, wing_level);
    Wing.AddLevelUpNeedNum(builder, level_up_need_num);
    Wing.AddLevelUpNeedId(builder, level_up_need_id);
    Wing.AddStun(builder, Stun);
    Wing.AddCriticalHit(builder, Critical_hit);
    Wing.AddCritical(builder, Critical);
    Wing.AddSpeed(builder, Speed);
    Wing.AddMDEF(builder, MDEF);
    Wing.AddMATK(builder, MATK);
    Wing.AddPDEF(builder, PDEF);
    Wing.AddPATK(builder, PATK);
    Wing.AddMaxHP(builder, MaxHP);
    Wing.AddModelId(builder, model_id);
    Wing.AddWingId(builder, wing_id);
    return Wing.EndWing(builder);
  }

  public static void StartWing(FlatBufferBuilder builder) { builder.StartObject(16); }
  public static void AddWingId(FlatBufferBuilder builder, int wingId) { builder.AddInt(0, wingId, 0); }
  public static void AddModelId(FlatBufferBuilder builder, int modelId) { builder.AddInt(1, modelId, 0); }
  public static void AddMaxHP(FlatBufferBuilder builder, int MaxHP) { builder.AddInt(2, MaxHP, 0); }
  public static void AddPATK(FlatBufferBuilder builder, int PATK) { builder.AddInt(3, PATK, 0); }
  public static void AddPDEF(FlatBufferBuilder builder, int PDEF) { builder.AddInt(4, PDEF, 0); }
  public static void AddMATK(FlatBufferBuilder builder, int MATK) { builder.AddInt(5, MATK, 0); }
  public static void AddMDEF(FlatBufferBuilder builder, int MDEF) { builder.AddInt(6, MDEF, 0); }
  public static void AddSpeed(FlatBufferBuilder builder, int Speed) { builder.AddInt(7, Speed, 0); }
  public static void AddCritical(FlatBufferBuilder builder, int Critical) { builder.AddInt(8, Critical, 0); }
  public static void AddCriticalHit(FlatBufferBuilder builder, int CriticalHit) { builder.AddInt(9, CriticalHit, 0); }
  public static void AddStun(FlatBufferBuilder builder, int Stun) { builder.AddInt(10, Stun, 0); }
  public static void AddLevelUpNeedId(FlatBufferBuilder builder, int levelUpNeedId) { builder.AddInt(11, levelUpNeedId, 0); }
  public static void AddLevelUpNeedNum(FlatBufferBuilder builder, int levelUpNeedNum) { builder.AddInt(12, levelUpNeedNum, 0); }
  public static void AddWingLevel(FlatBufferBuilder builder, int wingLevel) { builder.AddInt(13, wingLevel, 0); }
  public static void AddLevelUpNeedExp(FlatBufferBuilder builder, int levelUpNeedExp) { builder.AddInt(14, levelUpNeedExp, 0); }
  public static void AddNextLevelId(FlatBufferBuilder builder, int nextLevelId) { builder.AddInt(15, nextLevelId, 0); }
  public static Offset<Wing> EndWing(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Wing>(o);
  }
};

public sealed class WingModel : Table {
  public static WingModel GetRootAsWingModel(ByteBuffer _bb) { return GetRootAsWingModel(_bb, new WingModel()); }
  public static WingModel GetRootAsWingModel(ByteBuffer _bb, WingModel obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public WingModel __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Id { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Model { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetModelBytes() { return __vector_as_arraysegment(6); }
  public string Icon { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconBytes() { return __vector_as_arraysegment(8); }
  public string Name { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(10); }
  public string DropChickId1 { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId1Bytes() { return __vector_as_arraysegment(12); }
  public string DropChickId2 { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId2Bytes() { return __vector_as_arraysegment(14); }
  public string DropChickId3 { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDropChickId3Bytes() { return __vector_as_arraysegment(16); }
  public int GenericItems { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<WingModel> CreateWingModel(FlatBufferBuilder builder,
      int id = 0,
      StringOffset modelOffset = default(StringOffset),
      StringOffset iconOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      StringOffset drop_chick_id_1Offset = default(StringOffset),
      StringOffset drop_chick_id_2Offset = default(StringOffset),
      StringOffset drop_chick_id_3Offset = default(StringOffset),
      int generic_items = 0) {
    builder.StartObject(8);
    WingModel.AddGenericItems(builder, generic_items);
    WingModel.AddDropChickId3(builder, drop_chick_id_3Offset);
    WingModel.AddDropChickId2(builder, drop_chick_id_2Offset);
    WingModel.AddDropChickId1(builder, drop_chick_id_1Offset);
    WingModel.AddName(builder, nameOffset);
    WingModel.AddIcon(builder, iconOffset);
    WingModel.AddModel(builder, modelOffset);
    WingModel.AddId(builder, id);
    return WingModel.EndWingModel(builder);
  }

  public static void StartWingModel(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddId(FlatBufferBuilder builder, int id) { builder.AddInt(0, id, 0); }
  public static void AddModel(FlatBufferBuilder builder, StringOffset modelOffset) { builder.AddOffset(1, modelOffset.Value, 0); }
  public static void AddIcon(FlatBufferBuilder builder, StringOffset iconOffset) { builder.AddOffset(2, iconOffset.Value, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(3, nameOffset.Value, 0); }
  public static void AddDropChickId1(FlatBufferBuilder builder, StringOffset dropChickId1Offset) { builder.AddOffset(4, dropChickId1Offset.Value, 0); }
  public static void AddDropChickId2(FlatBufferBuilder builder, StringOffset dropChickId2Offset) { builder.AddOffset(5, dropChickId2Offset.Value, 0); }
  public static void AddDropChickId3(FlatBufferBuilder builder, StringOffset dropChickId3Offset) { builder.AddOffset(6, dropChickId3Offset.Value, 0); }
  public static void AddGenericItems(FlatBufferBuilder builder, int genericItems) { builder.AddInt(7, genericItems, 0); }
  public static Offset<WingModel> EndWingModel(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<WingModel>(o);
  }
};

public sealed class EquipmentLevelUp : Table {
  public static EquipmentLevelUp GetRootAsEquipmentLevelUp(ByteBuffer _bb) { return GetRootAsEquipmentLevelUp(_bb, new EquipmentLevelUp()); }
  public static EquipmentLevelUp GetRootAsEquipmentLevelUp(ByteBuffer _bb, EquipmentLevelUp obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public EquipmentLevelUp __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public int Star { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Level { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int NeedExp { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int TotalNeedExp { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<EquipmentLevelUp> CreateEquipmentLevelUp(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      int star = 0,
      int level = 0,
      int need_exp = 0,
      int total_need_exp = 0) {
    builder.StartObject(5);
    EquipmentLevelUp.AddTotalNeedExp(builder, total_need_exp);
    EquipmentLevelUp.AddNeedExp(builder, need_exp);
    EquipmentLevelUp.AddLevel(builder, level);
    EquipmentLevelUp.AddStar(builder, star);
    EquipmentLevelUp.AddId(builder, idOffset);
    return EquipmentLevelUp.EndEquipmentLevelUp(builder);
  }

  public static void StartEquipmentLevelUp(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddStar(FlatBufferBuilder builder, int star) { builder.AddInt(1, star, 0); }
  public static void AddLevel(FlatBufferBuilder builder, int level) { builder.AddInt(2, level, 0); }
  public static void AddNeedExp(FlatBufferBuilder builder, int needExp) { builder.AddInt(3, needExp, 0); }
  public static void AddTotalNeedExp(FlatBufferBuilder builder, int totalNeedExp) { builder.AddInt(4, totalNeedExp, 0); }
  public static Offset<EquipmentLevelUp> EndEquipmentLevelUp(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EquipmentLevelUp>(o);
  }
};

public sealed class SelectBox : Table {
  public static SelectBox GetRootAsSelectBox(ByteBuffer _bb) { return GetRootAsSelectBox(_bb, new SelectBox()); }
  public static SelectBox GetRootAsSelectBox(ByteBuffer _bb, SelectBox obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SelectBox __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public int Index { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Rt1 { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRt1Bytes() { return __vector_as_arraysegment(8); }
  public string Ri1 { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetRi1Bytes() { return __vector_as_arraysegment(10); }
  public int Rn1 { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<SelectBox> CreateSelectBox(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      int index = 0,
      StringOffset rt1Offset = default(StringOffset),
      StringOffset ri1Offset = default(StringOffset),
      int rn1 = 0) {
    builder.StartObject(5);
    SelectBox.AddRn1(builder, rn1);
    SelectBox.AddRi1(builder, ri1Offset);
    SelectBox.AddRt1(builder, rt1Offset);
    SelectBox.AddIndex(builder, index);
    SelectBox.AddId(builder, idOffset);
    return SelectBox.EndSelectBox(builder);
  }

  public static void StartSelectBox(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddIndex(FlatBufferBuilder builder, int index) { builder.AddInt(1, index, 0); }
  public static void AddRt1(FlatBufferBuilder builder, StringOffset rt1Offset) { builder.AddOffset(2, rt1Offset.Value, 0); }
  public static void AddRi1(FlatBufferBuilder builder, StringOffset ri1Offset) { builder.AddOffset(3, ri1Offset.Value, 0); }
  public static void AddRn1(FlatBufferBuilder builder, int rn1) { builder.AddInt(4, rn1, 0); }
  public static Offset<SelectBox> EndSelectBox(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SelectBox>(o);
  }
};

public sealed class HeadFrame : Table {
  public static HeadFrame GetRootAsHeadFrame(ByteBuffer _bb) { return GetRootAsHeadFrame(_bb, new HeadFrame()); }
  public static HeadFrame GetRootAsHeadFrame(ByteBuffer _bb, HeadFrame obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HeadFrame __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Id { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIdBytes() { return __vector_as_arraysegment(4); }
  public int Num { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Name { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(8); }
  public string IconId { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetIconIdBytes() { return __vector_as_arraysegment(10); }
  public string Desc { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDescBytes() { return __vector_as_arraysegment(12); }

  public static Offset<HeadFrame> CreateHeadFrame(FlatBufferBuilder builder,
      StringOffset idOffset = default(StringOffset),
      int num = 0,
      StringOffset nameOffset = default(StringOffset),
      StringOffset icon_idOffset = default(StringOffset),
      StringOffset descOffset = default(StringOffset)) {
    builder.StartObject(5);
    HeadFrame.AddDesc(builder, descOffset);
    HeadFrame.AddIconId(builder, icon_idOffset);
    HeadFrame.AddName(builder, nameOffset);
    HeadFrame.AddNum(builder, num);
    HeadFrame.AddId(builder, idOffset);
    return HeadFrame.EndHeadFrame(builder);
  }

  public static void StartHeadFrame(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddId(FlatBufferBuilder builder, StringOffset idOffset) { builder.AddOffset(0, idOffset.Value, 0); }
  public static void AddNum(FlatBufferBuilder builder, int num) { builder.AddInt(1, num, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(2, nameOffset.Value, 0); }
  public static void AddIconId(FlatBufferBuilder builder, StringOffset iconIdOffset) { builder.AddOffset(3, iconIdOffset.Value, 0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset descOffset) { builder.AddOffset(4, descOffset.Value, 0); }
  public static Offset<HeadFrame> EndHeadFrame(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HeadFrame>(o);
  }
};

public sealed class ConditionEconomy : Table {
  public static ConditionEconomy GetRootAsConditionEconomy(ByteBuffer _bb) { return GetRootAsConditionEconomy(_bb, new ConditionEconomy()); }
  public static ConditionEconomy GetRootAsConditionEconomy(ByteBuffer _bb, ConditionEconomy obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionEconomy __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

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
  public GenericItem GetGenericItems(int j) { return GetGenericItems(new GenericItem(), j); }
  public GenericItem GetGenericItems(GenericItem obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int GenericItemsLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public EquipmentItem GetEquipment(int j) { return GetEquipment(new EquipmentItem(), j); }
  public EquipmentItem GetEquipment(EquipmentItem obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int EquipmentLength { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public EquipAttributePool GetEquipAttributePool(int j) { return GetEquipAttributePool(new EquipAttributePool(), j); }
  public EquipAttributePool GetEquipAttributePool(EquipAttributePool obj, int j) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int EquipAttributePoolLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }
  public EquipAttribute GetEquipAttribute(int j) { return GetEquipAttribute(new EquipAttribute(), j); }
  public EquipAttribute GetEquipAttribute(EquipAttribute obj, int j) { int o = __offset(24); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int EquipAttributeLength { get { int o = __offset(24); return o != 0 ? __vector_len(o) : 0; } }
  public EquipAttributeRate GetEquipAttributeRate(int j) { return GetEquipAttributeRate(new EquipAttributeRate(), j); }
  public EquipAttributeRate GetEquipAttributeRate(EquipAttributeRate obj, int j) { int o = __offset(26); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int EquipAttributeRateLength { get { int o = __offset(26); return o != 0 ? __vector_len(o) : 0; } }
  public SuitAttribute GetSuitAttribute(int j) { return GetSuitAttribute(new SuitAttribute(), j); }
  public SuitAttribute GetSuitAttribute(SuitAttribute obj, int j) { int o = __offset(28); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SuitAttributeLength { get { int o = __offset(28); return o != 0 ? __vector_len(o) : 0; } }
  public AnimalInfo GetAnimalInfo(int j) { return GetAnimalInfo(new AnimalInfo(), j); }
  public AnimalInfo GetAnimalInfo(AnimalInfo obj, int j) { int o = __offset(30); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AnimalInfoLength { get { int o = __offset(30); return o != 0 ? __vector_len(o) : 0; } }
  public AnimalUpGrade GetAnimalUpGrade(int j) { return GetAnimalUpGrade(new AnimalUpGrade(), j); }
  public AnimalUpGrade GetAnimalUpGrade(AnimalUpGrade obj, int j) { int o = __offset(32); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AnimalUpGradeLength { get { int o = __offset(32); return o != 0 ? __vector_len(o) : 0; } }
  public AnimalLevelUp GetAnimalLevelUp(int j) { return GetAnimalLevelUp(new AnimalLevelUp(), j); }
  public AnimalLevelUp GetAnimalLevelUp(AnimalLevelUp obj, int j) { int o = __offset(34); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AnimalLevelUpLength { get { int o = __offset(34); return o != 0 ? __vector_len(o) : 0; } }
  public Wing GetWing(int j) { return GetWing(new Wing(), j); }
  public Wing GetWing(Wing obj, int j) { int o = __offset(36); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int WingLength { get { int o = __offset(36); return o != 0 ? __vector_len(o) : 0; } }
  public WingModel GetWingModel(int j) { return GetWingModel(new WingModel(), j); }
  public WingModel GetWingModel(WingModel obj, int j) { int o = __offset(38); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int WingModelLength { get { int o = __offset(38); return o != 0 ? __vector_len(o) : 0; } }
  public EquipmentLevelUp GetEquipmentsLevelUp(int j) { return GetEquipmentsLevelUp(new EquipmentLevelUp(), j); }
  public EquipmentLevelUp GetEquipmentsLevelUp(EquipmentLevelUp obj, int j) { int o = __offset(40); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int EquipmentsLevelUpLength { get { int o = __offset(40); return o != 0 ? __vector_len(o) : 0; } }
  public SelectBox GetSelectBox(int j) { return GetSelectBox(new SelectBox(), j); }
  public SelectBox GetSelectBox(SelectBox obj, int j) { int o = __offset(42); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SelectBoxLength { get { int o = __offset(42); return o != 0 ? __vector_len(o) : 0; } }
  public HeadFrame GetHeadFrame(int j) { return GetHeadFrame(new HeadFrame(), j); }
  public HeadFrame GetHeadFrame(HeadFrame obj, int j) { int o = __offset(44); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int HeadFrameLength { get { int o = __offset(44); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionEconomy> CreateConditionEconomy(FlatBufferBuilder builder,
      StringOffset _idOffset = default(StringOffset),
      StringOffset nameOffset = default(StringOffset),
      bool enabled = false,
      int priority = 0,
      VectorOffset date_conditionsOffset = default(VectorOffset),
      VectorOffset user_conditionsOffset = default(VectorOffset),
      Offset<GM.DataCache.Options> optionsOffset = default(Offset<GM.DataCache.Options>),
      VectorOffset generic_itemsOffset = default(VectorOffset),
      VectorOffset equipmentOffset = default(VectorOffset),
      VectorOffset equip_attribute_poolOffset = default(VectorOffset),
      VectorOffset equip_attributeOffset = default(VectorOffset),
      VectorOffset equip_attribute_rateOffset = default(VectorOffset),
      VectorOffset suit_attributeOffset = default(VectorOffset),
      VectorOffset animal_infoOffset = default(VectorOffset),
      VectorOffset animal_up_gradeOffset = default(VectorOffset),
      VectorOffset animal_level_upOffset = default(VectorOffset),
      VectorOffset wingOffset = default(VectorOffset),
      VectorOffset wing_modelOffset = default(VectorOffset),
      VectorOffset equipments_level_upOffset = default(VectorOffset),
      VectorOffset select_boxOffset = default(VectorOffset),
      VectorOffset head_frameOffset = default(VectorOffset)) {
    builder.StartObject(21);
    ConditionEconomy.AddHeadFrame(builder, head_frameOffset);
    ConditionEconomy.AddSelectBox(builder, select_boxOffset);
    ConditionEconomy.AddEquipmentsLevelUp(builder, equipments_level_upOffset);
    ConditionEconomy.AddWingModel(builder, wing_modelOffset);
    ConditionEconomy.AddWing(builder, wingOffset);
    ConditionEconomy.AddAnimalLevelUp(builder, animal_level_upOffset);
    ConditionEconomy.AddAnimalUpGrade(builder, animal_up_gradeOffset);
    ConditionEconomy.AddAnimalInfo(builder, animal_infoOffset);
    ConditionEconomy.AddSuitAttribute(builder, suit_attributeOffset);
    ConditionEconomy.AddEquipAttributeRate(builder, equip_attribute_rateOffset);
    ConditionEconomy.AddEquipAttribute(builder, equip_attributeOffset);
    ConditionEconomy.AddEquipAttributePool(builder, equip_attribute_poolOffset);
    ConditionEconomy.AddEquipment(builder, equipmentOffset);
    ConditionEconomy.AddGenericItems(builder, generic_itemsOffset);
    ConditionEconomy.AddOptions(builder, optionsOffset);
    ConditionEconomy.AddUserConditions(builder, user_conditionsOffset);
    ConditionEconomy.AddDateConditions(builder, date_conditionsOffset);
    ConditionEconomy.AddPriority(builder, priority);
    ConditionEconomy.AddName(builder, nameOffset);
    ConditionEconomy.Add_id(builder, _idOffset);
    ConditionEconomy.AddEnabled(builder, enabled);
    return ConditionEconomy.EndConditionEconomy(builder);
  }

  public static void StartConditionEconomy(FlatBufferBuilder builder) { builder.StartObject(21); }
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
  public static void AddGenericItems(FlatBufferBuilder builder, VectorOffset genericItemsOffset) { builder.AddOffset(7, genericItemsOffset.Value, 0); }
  public static VectorOffset CreateGenericItemsVector(FlatBufferBuilder builder, Offset<GenericItem>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartGenericItemsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEquipment(FlatBufferBuilder builder, VectorOffset equipmentOffset) { builder.AddOffset(8, equipmentOffset.Value, 0); }
  public static VectorOffset CreateEquipmentVector(FlatBufferBuilder builder, Offset<EquipmentItem>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartEquipmentVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEquipAttributePool(FlatBufferBuilder builder, VectorOffset equipAttributePoolOffset) { builder.AddOffset(9, equipAttributePoolOffset.Value, 0); }
  public static VectorOffset CreateEquipAttributePoolVector(FlatBufferBuilder builder, Offset<EquipAttributePool>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartEquipAttributePoolVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEquipAttribute(FlatBufferBuilder builder, VectorOffset equipAttributeOffset) { builder.AddOffset(10, equipAttributeOffset.Value, 0); }
  public static VectorOffset CreateEquipAttributeVector(FlatBufferBuilder builder, Offset<EquipAttribute>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartEquipAttributeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEquipAttributeRate(FlatBufferBuilder builder, VectorOffset equipAttributeRateOffset) { builder.AddOffset(11, equipAttributeRateOffset.Value, 0); }
  public static VectorOffset CreateEquipAttributeRateVector(FlatBufferBuilder builder, Offset<EquipAttributeRate>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartEquipAttributeRateVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSuitAttribute(FlatBufferBuilder builder, VectorOffset suitAttributeOffset) { builder.AddOffset(12, suitAttributeOffset.Value, 0); }
  public static VectorOffset CreateSuitAttributeVector(FlatBufferBuilder builder, Offset<SuitAttribute>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSuitAttributeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAnimalInfo(FlatBufferBuilder builder, VectorOffset animalInfoOffset) { builder.AddOffset(13, animalInfoOffset.Value, 0); }
  public static VectorOffset CreateAnimalInfoVector(FlatBufferBuilder builder, Offset<AnimalInfo>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAnimalInfoVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAnimalUpGrade(FlatBufferBuilder builder, VectorOffset animalUpGradeOffset) { builder.AddOffset(14, animalUpGradeOffset.Value, 0); }
  public static VectorOffset CreateAnimalUpGradeVector(FlatBufferBuilder builder, Offset<AnimalUpGrade>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAnimalUpGradeVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAnimalLevelUp(FlatBufferBuilder builder, VectorOffset animalLevelUpOffset) { builder.AddOffset(15, animalLevelUpOffset.Value, 0); }
  public static VectorOffset CreateAnimalLevelUpVector(FlatBufferBuilder builder, Offset<AnimalLevelUp>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAnimalLevelUpVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddWing(FlatBufferBuilder builder, VectorOffset wingOffset) { builder.AddOffset(16, wingOffset.Value, 0); }
  public static VectorOffset CreateWingVector(FlatBufferBuilder builder, Offset<Wing>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartWingVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddWingModel(FlatBufferBuilder builder, VectorOffset wingModelOffset) { builder.AddOffset(17, wingModelOffset.Value, 0); }
  public static VectorOffset CreateWingModelVector(FlatBufferBuilder builder, Offset<WingModel>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartWingModelVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEquipmentsLevelUp(FlatBufferBuilder builder, VectorOffset equipmentsLevelUpOffset) { builder.AddOffset(18, equipmentsLevelUpOffset.Value, 0); }
  public static VectorOffset CreateEquipmentsLevelUpVector(FlatBufferBuilder builder, Offset<EquipmentLevelUp>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartEquipmentsLevelUpVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSelectBox(FlatBufferBuilder builder, VectorOffset selectBoxOffset) { builder.AddOffset(19, selectBoxOffset.Value, 0); }
  public static VectorOffset CreateSelectBoxVector(FlatBufferBuilder builder, Offset<SelectBox>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSelectBoxVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHeadFrame(FlatBufferBuilder builder, VectorOffset headFrameOffset) { builder.AddOffset(20, headFrameOffset.Value, 0); }
  public static VectorOffset CreateHeadFrameVector(FlatBufferBuilder builder, Offset<HeadFrame>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartHeadFrameVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionEconomy> EndConditionEconomy(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionEconomy>(o);
  }
};

public sealed class Economy : Table {
  public static Economy GetRootAsEconomy(ByteBuffer _bb) { return GetRootAsEconomy(_bb, new Economy()); }
  public static Economy GetRootAsEconomy(ByteBuffer _bb, Economy obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Economy __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionEconomy GetArray(int j) { return GetArray(new ConditionEconomy(), j); }
  public ConditionEconomy GetArray(ConditionEconomy obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Economy> CreateEconomy(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Economy.AddArray(builder, arrayOffset);
    return Economy.EndEconomy(builder);
  }

  public static void StartEconomy(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionEconomy>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Economy> EndEconomy(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Economy>(o);
  }
  public static void FinishEconomyBuffer(FlatBufferBuilder builder, Offset<Economy> offset) { builder.Finish(offset.Value); }
};


}
