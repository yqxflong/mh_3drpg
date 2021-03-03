// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class Categories : Table {
  public static Categories GetRootAsCategories(ByteBuffer _bb) { return GetRootAsCategories(_bb, new Categories()); }
  public static Categories GetRootAsCategories(ByteBuffer _bb, Categories obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Categories __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(4); }
  public Item GetItems(int j) { return GetItems(new Item(), j); }
  public Item GetItems(Item obj, int j) { int o = __offset(6); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ItemsLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Categories> CreateCategories(FlatBufferBuilder builder,
      StringOffset nameOffset = default(StringOffset),
      VectorOffset itemsOffset = default(VectorOffset)) {
    builder.StartObject(2);
    Categories.AddItems(builder, itemsOffset);
    Categories.AddName(builder, nameOffset);
    return Categories.EndCategories(builder);
  }

  public static void StartCategories(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddItems(FlatBufferBuilder builder, VectorOffset itemsOffset) { builder.AddOffset(1, itemsOffset.Value, 0); }
  public static VectorOffset CreateItemsVector(FlatBufferBuilder builder, Offset<Item>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartItemsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Categories> EndCategories(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Categories>(o);
  }
};

public sealed class Item : Table {
  public static Item GetRootAsItem(ByteBuffer _bb) { return GetRootAsItem(_bb, new Item()); }
  public static Item GetRootAsItem(ByteBuffer _bb, Item obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Item __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Type { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTypeBytes() { return __vector_as_arraysegment(4); }
  public string Data { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetDataBytes() { return __vector_as_arraysegment(6); }
  public int Weight { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Min { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Max { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<Item> CreateItem(FlatBufferBuilder builder,
      StringOffset typeOffset = default(StringOffset),
      StringOffset dataOffset = default(StringOffset),
      int weight = 0,
      int min = 0,
      int max = 0) {
    builder.StartObject(5);
    Item.AddMax(builder, max);
    Item.AddMin(builder, min);
    Item.AddWeight(builder, weight);
    Item.AddData(builder, dataOffset);
    Item.AddType(builder, typeOffset);
    return Item.EndItem(builder);
  }

  public static void StartItem(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddType(FlatBufferBuilder builder, StringOffset typeOffset) { builder.AddOffset(0, typeOffset.Value, 0); }
  public static void AddData(FlatBufferBuilder builder, StringOffset dataOffset) { builder.AddOffset(1, dataOffset.Value, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(2, weight, 0); }
  public static void AddMin(FlatBufferBuilder builder, int min) { builder.AddInt(3, min, 0); }
  public static void AddMax(FlatBufferBuilder builder, int max) { builder.AddInt(4, max, 0); }
  public static Offset<Item> EndItem(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Item>(o);
  }
};

public sealed class Boxes : Table {
  public static Boxes GetRootAsBoxes(ByteBuffer _bb) { return GetRootAsBoxes(_bb, new Boxes()); }
  public static Boxes GetRootAsBoxes(ByteBuffer _bb, Boxes obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Boxes __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public CategoriesData GetCategories(int j) { return GetCategories(new CategoriesData(), j); }
  public CategoriesData GetCategories(CategoriesData obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int CategoriesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public string Name { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(6); }

  public static Offset<Boxes> CreateBoxes(FlatBufferBuilder builder,
      VectorOffset categoriesOffset = default(VectorOffset),
      StringOffset nameOffset = default(StringOffset)) {
    builder.StartObject(2);
    Boxes.AddName(builder, nameOffset);
    Boxes.AddCategories(builder, categoriesOffset);
    return Boxes.EndBoxes(builder);
  }

  public static void StartBoxes(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddCategories(FlatBufferBuilder builder, VectorOffset categoriesOffset) { builder.AddOffset(0, categoriesOffset.Value, 0); }
  public static VectorOffset CreateCategoriesVector(FlatBufferBuilder builder, Offset<CategoriesData>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartCategoriesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(1, nameOffset.Value, 0); }
  public static Offset<Boxes> EndBoxes(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Boxes>(o);
  }
};

public sealed class CategoriesData : Table {
  public static CategoriesData GetRootAsCategoriesData(ByteBuffer _bb) { return GetRootAsCategoriesData(_bb, new CategoriesData()); }
  public static CategoriesData GetRootAsCategoriesData(ByteBuffer _bb, CategoriesData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public CategoriesData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Category { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetCategoryBytes() { return __vector_as_arraysegment(4); }
  public int Weight { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Group { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetGroupBytes() { return __vector_as_arraysegment(8); }

  public static Offset<CategoriesData> CreateCategoriesData(FlatBufferBuilder builder,
      StringOffset categoryOffset = default(StringOffset),
      int weight = 0,
      StringOffset groupOffset = default(StringOffset)) {
    builder.StartObject(3);
    CategoriesData.AddGroup(builder, groupOffset);
    CategoriesData.AddWeight(builder, weight);
    CategoriesData.AddCategory(builder, categoryOffset);
    return CategoriesData.EndCategoriesData(builder);
  }

  public static void StartCategoriesData(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddCategory(FlatBufferBuilder builder, StringOffset categoryOffset) { builder.AddOffset(0, categoryOffset.Value, 0); }
  public static void AddWeight(FlatBufferBuilder builder, int weight) { builder.AddInt(1, weight, 0); }
  public static void AddGroup(FlatBufferBuilder builder, StringOffset groupOffset) { builder.AddOffset(2, groupOffset.Value, 0); }
  public static Offset<CategoriesData> EndCategoriesData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<CategoriesData>(o);
  }
};

public sealed class ConditionGacha : Table {
  public static ConditionGacha GetRootAsConditionGacha(ByteBuffer _bb) { return GetRootAsConditionGacha(_bb, new ConditionGacha()); }
  public static ConditionGacha GetRootAsConditionGacha(ByteBuffer _bb, ConditionGacha obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConditionGacha __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Categories GetCategories(int j) { return GetCategories(new Categories(), j); }
  public Categories GetCategories(Categories obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int CategoriesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public Boxes GetBoxes(int j) { return GetBoxes(new Boxes(), j); }
  public Boxes GetBoxes(Boxes obj, int j) { int o = __offset(6); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BoxesLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<ConditionGacha> CreateConditionGacha(FlatBufferBuilder builder,
      VectorOffset categoriesOffset = default(VectorOffset),
      VectorOffset boxesOffset = default(VectorOffset)) {
    builder.StartObject(2);
    ConditionGacha.AddBoxes(builder, boxesOffset);
    ConditionGacha.AddCategories(builder, categoriesOffset);
    return ConditionGacha.EndConditionGacha(builder);
  }

  public static void StartConditionGacha(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddCategories(FlatBufferBuilder builder, VectorOffset categoriesOffset) { builder.AddOffset(0, categoriesOffset.Value, 0); }
  public static VectorOffset CreateCategoriesVector(FlatBufferBuilder builder, Offset<Categories>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartCategoriesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBoxes(FlatBufferBuilder builder, VectorOffset boxesOffset) { builder.AddOffset(1, boxesOffset.Value, 0); }
  public static VectorOffset CreateBoxesVector(FlatBufferBuilder builder, Offset<Boxes>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBoxesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<ConditionGacha> EndConditionGacha(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConditionGacha>(o);
  }
};

public sealed class Gacha : Table {
  public static Gacha GetRootAsGacha(ByteBuffer _bb) { return GetRootAsGacha(_bb, new Gacha()); }
  public static Gacha GetRootAsGacha(ByteBuffer _bb, Gacha obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Gacha __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConditionGacha GetArray(int j) { return GetArray(new ConditionGacha(), j); }
  public ConditionGacha GetArray(ConditionGacha obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Gacha> CreateGacha(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Gacha.AddArray(builder, arrayOffset);
    return Gacha.EndGacha(builder);
  }

  public static void StartGacha(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConditionGacha>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Gacha> EndGacha(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Gacha>(o);
  }
  public static void FinishGachaBuffer(FlatBufferBuilder builder, Offset<Gacha> offset) { builder.Finish(offset.Value); }
};


}
