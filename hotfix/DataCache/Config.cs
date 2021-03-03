// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class ConfigEntity : Table {
  public static ConfigEntity GetRootAsConfigEntity(ByteBuffer _bb) { return GetRootAsConfigEntity(_bb, new ConfigEntity()); }
  public static ConfigEntity GetRootAsConfigEntity(ByteBuffer _bb, ConfigEntity obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ConfigEntity __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(4); }
  public string Version { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetVersionBytes() { return __vector_as_arraysegment(6); }
  public sbyte GetBuffer(int j) { int o = __offset(8); return o != 0 ? bb.GetSbyte(__vector(o) + j * 1) : (sbyte)0; }
  public int BufferLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }
  public ArraySegment<byte>? GetBufferBytes() { return __vector_as_arraysegment(8); }

  public static Offset<ConfigEntity> CreateConfigEntity(FlatBufferBuilder builder,
      StringOffset nameOffset = default(StringOffset),
      StringOffset versionOffset = default(StringOffset),
      VectorOffset bufferOffset = default(VectorOffset)) {
    builder.StartObject(3);
    ConfigEntity.AddBuffer(builder, bufferOffset);
    ConfigEntity.AddVersion(builder, versionOffset);
    ConfigEntity.AddName(builder, nameOffset);
    return ConfigEntity.EndConfigEntity(builder);
  }

  public static void StartConfigEntity(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddVersion(FlatBufferBuilder builder, StringOffset versionOffset) { builder.AddOffset(1, versionOffset.Value, 0); }
  public static void AddBuffer(FlatBufferBuilder builder, VectorOffset bufferOffset) { builder.AddOffset(2, bufferOffset.Value, 0); }
  public static VectorOffset CreateBufferVector(FlatBufferBuilder builder, sbyte[] data) { builder.StartVector(1, data.Length, 1); for (int i = data.Length - 1; i >= 0; i--) builder.AddSbyte(data[i]); return builder.EndVector(); }
  public static void StartBufferVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
  public static Offset<ConfigEntity> EndConfigEntity(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ConfigEntity>(o);
  }
};

public sealed class Config : Table {
  public static Config GetRootAsConfig(ByteBuffer _bb) { return GetRootAsConfig(_bb, new Config()); }
  public static Config GetRootAsConfig(ByteBuffer _bb, Config obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Config __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ConfigEntity GetArray(int j) { return GetArray(new ConfigEntity(), j); }
  public ConfigEntity GetArray(ConfigEntity obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ArrayLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Config> CreateConfig(FlatBufferBuilder builder,
      VectorOffset arrayOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Config.AddArray(builder, arrayOffset);
    return Config.EndConfig(builder);
  }

  public static void StartConfig(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddArray(FlatBufferBuilder builder, VectorOffset arrayOffset) { builder.AddOffset(0, arrayOffset.Value, 0); }
  public static VectorOffset CreateArrayVector(FlatBufferBuilder builder, Offset<ConfigEntity>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartArrayVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Config> EndConfig(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Config>(o);
  }
  public static void FinishConfigBuffer(FlatBufferBuilder builder, Offset<Config> offset) { builder.Finish(offset.Value); }
};


}
