// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using global::Unity.Collections;
using global::System;
using global::System.Collections.Generic;
using global::Fivemid.FiveFlat;

public struct HandFan : IFlatBufferObject
{
  private Table __p;
  public ref ByteBuffer ByteBuffer { get { return ref __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FIVE_FLAT_23_12_04(); }
  public static HandFan GetRootAsHandFan(ref ByteBuffer _bb) { return GetRootAsHandFan(ref _bb, new HandFan()); }
  public static HandFan GetRootAsHandFan(ref ByteBuffer _bb, HandFan obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, ref _bb)); }
  public void __init(int _i, ref ByteBuffer _bb) { __p = new Table(_i, ref _bb); }
  public HandFan __assign(int _i, ref ByteBuffer _bb) { __init(_i, ref _bb); return this; }

  public readonly int Length { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public bool MutateLength(int length) { int o = __p.__offset(4); if (o != 0) { __p.bb.PutInt(o + __p.bb_pos, length); return true; } else { return false; } }

  public static Offset<HandFan> CreateHandFan(ref FlatBufferBuilder builder,
      int length = 0) {
    builder.StartTable(1);
    HandFan.AddLength(ref builder, length);
    return HandFan.EndHandFan(ref builder);
  }

  public static void StartHandFan(ref FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddLength(ref FlatBufferBuilder builder, int length) { builder.AddInt(0, length, 0); }
  public static Offset<HandFan> EndHandFan(ref FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<HandFan>(o);
  }
  public HandFanT UnPack() {
    var _o = new HandFanT();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(HandFanT _o) {
    _o.Length = this.Length;
  }
  public static Offset<HandFan> Pack(ref FlatBufferBuilder builder, HandFanT _o) {
    if (_o == null) return default(Offset<HandFan>);
    return CreateHandFan(
      ref builder,
      _o.Length);
  }
}

public class HandFanT
{
  [Newtonsoft.Json.JsonProperty("length")]
  public int Length { get; set; }

  public HandFanT() {
    this.Length = 0;
  }
}


static public class HandFanVerify
{
  static public bool Verify(Fivemid.FiveFlat.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyField(tablePos, 4 /*Length*/, 4 /*int*/, 4, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}
