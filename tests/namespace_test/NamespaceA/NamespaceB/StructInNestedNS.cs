// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace NamespaceA.NamespaceB
{

using global::Unity.Collections;
using global::System;
using global::System.Collections.Generic;
using global::Fivemid.FiveFlat;

public struct StructInNestedNS : IFlatBufferObject
{
  private Struct __p;
  public ref ByteBuffer ByteBuffer { get { return ref __p.bb; } }
  public void __init(int _i, ref ByteBuffer _bb) { __p = new Struct(_i, ref _bb); }
  public StructInNestedNS __assign(int _i, ref ByteBuffer _bb) { __init(_i, ref _bb); return this; }

  public readonly int A { get { return __p.bb.GetInt(__p.bb_pos + 0); } }
  public void MutateA(int a) { __p.bb.PutInt(__p.bb_pos + 0, a); }
  public readonly int B { get { return __p.bb.GetInt(__p.bb_pos + 4); } }
  public void MutateB(int b) { __p.bb.PutInt(__p.bb_pos + 4, b); }

  public static Offset<NamespaceA.NamespaceB.StructInNestedNS> CreateStructInNestedNS(ref FlatBufferBuilder builder, int A, int B) {
    builder.Prep(4, 8);
    builder.PutInt(B);
    builder.PutInt(A);
    return new Offset<NamespaceA.NamespaceB.StructInNestedNS>(builder.Offset);
  }
  public StructInNestedNST UnPack() {
    var _o = new StructInNestedNST();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(StructInNestedNST _o) {
    _o.A = this.A;
    _o.B = this.B;
  }
  public static Offset<NamespaceA.NamespaceB.StructInNestedNS> Pack(ref FlatBufferBuilder builder, StructInNestedNST _o) {
    if (_o == null) return default(Offset<NamespaceA.NamespaceB.StructInNestedNS>);
    return CreateStructInNestedNS(
      ref builder,
      _o.A,
      _o.B);
  }
}

public class StructInNestedNST
{
  [Newtonsoft.Json.JsonProperty("a")]
  public int A { get; set; }
  [Newtonsoft.Json.JsonProperty("b")]
  public int B { get; set; }

  public StructInNestedNST() {
    this.A = 0;
    this.B = 0;
  }
}


}
