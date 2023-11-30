// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using global::Unity.Collections;
using global::System;
using global::System.Collections.Generic;
using global::Fivemid.FiveFlat;

public struct FallingTub : IFlatBufferObject
{
  private Struct __p;
  public ref ByteBuffer ByteBuffer { get { return ref __p.bb; } }
  public void __init(int _i, ref ByteBuffer _bb) { __p = new Struct(_i, ref _bb); }
  public FallingTub __assign(int _i, ref ByteBuffer _bb) { __init(_i, ref _bb); return this; }

  public int Weight { get { return __p.bb.GetInt(__p.bb_pos + 0); } }
  public void MutateWeight(int weight) { __p.bb.PutInt(__p.bb_pos + 0, weight); }

  public static Offset<FallingTub> CreateFallingTub(ref FlatBufferBuilder builder, int Weight) {
    builder.Prep(4, 4);
    builder.PutInt(Weight);
    return new Offset<FallingTub>(builder.Offset);
  }
  public FallingTubT UnPack() {
    var _o = new FallingTubT();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(FallingTubT _o) {
    _o.Weight = this.Weight;
  }
  public static Offset<FallingTub> Pack(ref FlatBufferBuilder builder, FallingTubT _o) {
    if (_o == null) return default(Offset<FallingTub>);
    return CreateFallingTub(
      ref builder,
      _o.Weight);
  }
}

public class FallingTubT
{
  [Newtonsoft.Json.JsonProperty("weight")]
  public int Weight { get; set; }

  public FallingTubT() {
    this.Weight = 0;
  }
}

