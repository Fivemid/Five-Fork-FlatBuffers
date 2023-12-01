// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace MyGame.Example
{

using global::Unity.Collections;
using global::System;
using global::System.Collections.Generic;
using global::Fivemid.FiveFlat;

public struct StructOfStructsOfStructs : IFlatBufferObject
{
  private Struct __p;
  public ref ByteBuffer ByteBuffer { get { return ref __p.bb; } }
  public void __init(int _i, ref ByteBuffer _bb) { __p = new Struct(_i, ref _bb); }
  public StructOfStructsOfStructs __assign(int _i, ref ByteBuffer _bb) { __init(_i, ref _bb); return this; }

  public readonly MyGame.Example.StructOfStructs A { get { return (new MyGame.Example.StructOfStructs()).__assign(__p.bb_pos + 0, ref __p.bb); } }

  public static Offset<MyGame.Example.StructOfStructsOfStructs> CreateStructOfStructsOfStructs(ref FlatBufferBuilder builder, uint a_a_Id, uint a_a_Distance, short a_b_A, sbyte a_b_B, uint a_c_Id, uint a_c_Distance) {
    builder.Prep(4, 20);
    builder.Prep(4, 20);
    builder.Prep(4, 8);
    builder.PutUint(a_c_Distance);
    builder.PutUint(a_c_Id);
    builder.Prep(2, 4);
    builder.Pad(1);
    builder.PutSbyte(a_b_B);
    builder.PutShort(a_b_A);
    builder.Prep(4, 8);
    builder.PutUint(a_a_Distance);
    builder.PutUint(a_a_Id);
    return new Offset<MyGame.Example.StructOfStructsOfStructs>(builder.Offset);
  }
  public StructOfStructsOfStructsT UnPack() {
    var _o = new StructOfStructsOfStructsT();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(StructOfStructsOfStructsT _o) {
    _o.A = this.A.UnPack();
  }
  public static Offset<MyGame.Example.StructOfStructsOfStructs> Pack(ref FlatBufferBuilder builder, StructOfStructsOfStructsT _o) {
    if (_o == null) return default(Offset<MyGame.Example.StructOfStructsOfStructs>);
    var _a_a_id = _o.A.A.Id;
    var _a_a_distance = _o.A.A.Distance;
    var _a_b_a = _o.A.B.A;
    var _a_b_b = _o.A.B.B;
    var _a_c_id = _o.A.C.Id;
    var _a_c_distance = _o.A.C.Distance;
    return CreateStructOfStructsOfStructs(
      ref builder,
      _a_a_id,
      _a_a_distance,
      _a_b_a,
      _a_b_b,
      _a_c_id,
      _a_c_distance);
  }
}

public class StructOfStructsOfStructsT
{
  [Newtonsoft.Json.JsonProperty("a")]
  public MyGame.Example.StructOfStructsT A { get; set; }

  public StructOfStructsOfStructsT() {
    this.A = new MyGame.Example.StructOfStructsT();
  }
}


}
