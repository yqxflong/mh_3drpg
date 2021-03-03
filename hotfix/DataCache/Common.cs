// automatically generated, do not modify

namespace GM.DataCache
{

using System;
using FlatBuffers;

public sealed class DateCondition : Table {
  public static DateCondition GetRootAsDateCondition(ByteBuffer _bb) { return GetRootAsDateCondition(_bb, new DateCondition()); }
  public static DateCondition GetRootAsDateCondition(ByteBuffer _bb, DateCondition obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DateCondition __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }


  public static void StartDateCondition(FlatBufferBuilder builder) { builder.StartObject(0); }
  public static Offset<DateCondition> EndDateCondition(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DateCondition>(o);
  }
};

public sealed class UserCondition : Table {
  public static UserCondition GetRootAsUserCondition(ByteBuffer _bb) { return GetRootAsUserCondition(_bb, new UserCondition()); }
  public static UserCondition GetRootAsUserCondition(ByteBuffer _bb, UserCondition obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public UserCondition __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }


  public static void StartUserCondition(FlatBufferBuilder builder) { builder.StartObject(0); }
  public static Offset<UserCondition> EndUserCondition(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<UserCondition>(o);
  }
};

public sealed class Options : Table {
  public static Options GetRootAsOptions(ByteBuffer _bb) { return GetRootAsOptions(_bb, new Options()); }
  public static Options GetRootAsOptions(ByteBuffer _bb, Options obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Options __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }


  public static void StartOptions(FlatBufferBuilder builder) { builder.StartObject(0); }
  public static Offset<Options> EndOptions(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Options>(o);
  }
};

public sealed class Redeem : Table {
  public static Redeem GetRootAsRedeem(ByteBuffer _bb) { return GetRootAsRedeem(_bb, new Redeem()); }
  public static Redeem GetRootAsRedeem(ByteBuffer _bb, Redeem obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Redeem __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string T { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTBytes() { return __vector_as_arraysegment(4); }
  public string N { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNBytes() { return __vector_as_arraysegment(6); }
  public int Q { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<Redeem> CreateRedeem(FlatBufferBuilder builder,
      StringOffset tOffset = default(StringOffset),
      StringOffset nOffset = default(StringOffset),
      int q = 0) {
    builder.StartObject(3);
    Redeem.AddQ(builder, q);
    Redeem.AddN(builder, nOffset);
    Redeem.AddT(builder, tOffset);
    return Redeem.EndRedeem(builder);
  }

  public static void StartRedeem(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddT(FlatBufferBuilder builder, StringOffset tOffset) { builder.AddOffset(0, tOffset.Value, 0); }
  public static void AddN(FlatBufferBuilder builder, StringOffset nOffset) { builder.AddOffset(1, nOffset.Value, 0); }
  public static void AddQ(FlatBufferBuilder builder, int q) { builder.AddInt(2, q, 0); }
  public static Offset<Redeem> EndRedeem(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Redeem>(o);
  }
};


}
