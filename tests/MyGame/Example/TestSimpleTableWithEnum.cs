// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace MyGame.Example
{

using global::Unity.Collections;
using global::System;
using global::System.Collections.Generic;
using global::Fivemid.FiveFlat;

internal partial struct TestSimpleTableWithEnum : IFlatBufferObject
{
  private Table __p;
  public ref ByteBuffer ByteBuffer { get { return ref __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FIVE_FLAT_23_12_04(); }
  public static TestSimpleTableWithEnum GetRootAsTestSimpleTableWithEnum(ref ByteBuffer _bb) { return GetRootAsTestSimpleTableWithEnum(ref _bb, new TestSimpleTableWithEnum()); }
  public static TestSimpleTableWithEnum GetRootAsTestSimpleTableWithEnum(ref ByteBuffer _bb, TestSimpleTableWithEnum obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, ref _bb)); }
  public void __init(int _i, ref ByteBuffer _bb) { __p = new Table(_i, ref _bb); }
  public TestSimpleTableWithEnum __assign(int _i, ref ByteBuffer _bb) { __init(_i, ref _bb); return this; }

  public readonly MyGame.Example.Color Color { get { int o = __p.__offset(4); return o != 0 ? (MyGame.Example.Color)__p.bb.Get(o + __p.bb_pos) : MyGame.Example.Color.Green; } }
  public bool MutateColor(MyGame.Example.Color color) { int o = __p.__offset(4); if (o != 0) { __p.bb.PutByte(o + __p.bb_pos, (byte)color); return true; } else { return false; } }

  public static Offset<MyGame.Example.TestSimpleTableWithEnum> CreateTestSimpleTableWithEnum(ref FlatBufferBuilder builder,
      MyGame.Example.Color color = MyGame.Example.Color.Green) {
    builder.StartTable(1);
    TestSimpleTableWithEnum.AddColor(ref builder, color);
    return TestSimpleTableWithEnum.EndTestSimpleTableWithEnum(ref builder);
  }

  public static void StartTestSimpleTableWithEnum(ref FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddColor(ref FlatBufferBuilder builder, MyGame.Example.Color color) { builder.AddByte(0, (byte)color, 2); }
  public static Offset<MyGame.Example.TestSimpleTableWithEnum> EndTestSimpleTableWithEnum(ref FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<MyGame.Example.TestSimpleTableWithEnum>(o);
  }
  public TestSimpleTableWithEnumT UnPack() {
    var _o = new TestSimpleTableWithEnumT();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(TestSimpleTableWithEnumT _o) {
    _o.Color = this.Color;
  }
  public static Offset<MyGame.Example.TestSimpleTableWithEnum> Pack(ref FlatBufferBuilder builder, TestSimpleTableWithEnumT _o) {
    if (_o == null) return default(Offset<MyGame.Example.TestSimpleTableWithEnum>);
    return CreateTestSimpleTableWithEnum(
      ref builder,
      _o.Color);
  }
}

internal partial class TestSimpleTableWithEnumT
{
  [Newtonsoft.Json.JsonProperty("color")]
  public MyGame.Example.Color Color { get; set; }

  public TestSimpleTableWithEnumT() {
    this.Color = MyGame.Example.Color.Green;
  }
}


static public class TestSimpleTableWithEnumVerify
{
  static public bool Verify(Fivemid.FiveFlat.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyField(tablePos, 4 /*Color*/, 1 /*MyGame.Example.Color*/, 1, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}

}
