// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace MyGame.Example2
{

using global::Unity.Collections;
using global::System;
using global::System.Collections.Generic;
using global::Fivemid.FiveFlat;

public struct Monster : IFlatBufferObject
{
  private Table __p;
  public ref ByteBuffer ByteBuffer { get { return ref __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FIVE_FLAT_23_11_29(); }
  public static Monster GetRootAsMonster(ref ByteBuffer _bb) { return GetRootAsMonster(ref _bb, new Monster()); }
  public static Monster GetRootAsMonster(ref ByteBuffer _bb, Monster obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, ref _bb)); }
  public void __init(int _i, ref ByteBuffer _bb) { __p = new Table(_i, ref _bb); }
  public Monster __assign(int _i, ref ByteBuffer _bb) { __init(_i, ref _bb); return this; }


  public static void StartMonster(ref FlatBufferBuilder builder) { builder.StartTable(0); }
  public static Offset<MyGame.Example2.Monster> EndMonster(ref FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<MyGame.Example2.Monster>(o);
  }
  public MonsterT UnPack() {
    var _o = new MonsterT();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(MonsterT _o) {
  }
  public static Offset<MyGame.Example2.Monster> Pack(ref FlatBufferBuilder builder, MonsterT _o) {
    if (_o == null) return default(Offset<MyGame.Example2.Monster>);
    StartMonster(ref builder);
    return EndMonster(ref builder);
  }
}

public class MonsterT
{

  public MonsterT() {
  }
}


static public class MonsterVerify
{
  static public bool Verify(Fivemid.FiveFlat.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyTableEnd(tablePos);
  }
}

}
