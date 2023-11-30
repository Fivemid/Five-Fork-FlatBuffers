// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace union_value_collsion
{

using global::Unity.Collections;
using global::System;
using global::System.Collections.Generic;
using global::Fivemid.FiveFlat;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum Value : byte
{
  NONE = 0,
  IntValue = 1,
};

public class ValueUnion {
  public Value Type { get; set; }
  public object Value_ { get; set; }

  public ValueUnion() {
    this.Type = Value.NONE;
    this.Value_ = null;
  }

  public T As<T>() where T : class { return this.Value_ as T; }
  public union_value_collsion.IntValueT AsIntValue() { return this.As<union_value_collsion.IntValueT>(); }
  public static ValueUnion FromIntValue(union_value_collsion.IntValueT _intvalue) { return new ValueUnion{ Type = Value.IntValue, Value_ = _intvalue }; }

  public static int Pack(ref Fivemid.FiveFlat.FlatBufferBuilder builder, ValueUnion _o) {
    switch (_o.Type) {
      default: return 0;
      case Value.IntValue: return union_value_collsion.IntValue.Pack(ref builder, _o.AsIntValue()).Value;
    }
  }
}

public class ValueUnion_JsonConverter : Newtonsoft.Json.JsonConverter {
  public override bool CanConvert(System.Type objectType) {
    return objectType == typeof(ValueUnion) || objectType == typeof(System.Collections.Generic.List<ValueUnion>);
  }
  public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer) {
    var _olist = value as System.Collections.Generic.List<ValueUnion>;
    if (_olist != null) {
      writer.WriteStartArray();
      foreach (var _o in _olist) { this.WriteJson(writer, _o, serializer); }
      writer.WriteEndArray();
    } else {
      this.WriteJson(writer, value as ValueUnion, serializer);
    }
  }
  public void WriteJson(Newtonsoft.Json.JsonWriter writer, ValueUnion _o, Newtonsoft.Json.JsonSerializer serializer) {
    if (_o == null) return;
    serializer.Serialize(writer, _o.Value_);
  }
  public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer) {
    var _olist = existingValue as System.Collections.Generic.List<ValueUnion>;
    if (_olist != null) {
      for (var _j = 0; _j < _olist.Count; ++_j) {
        reader.Read();
        _olist[_j] = this.ReadJson(reader, _olist[_j], serializer);
      }
      reader.Read();
      return _olist;
    } else {
      return this.ReadJson(reader, existingValue as ValueUnion, serializer);
    }
  }
  public ValueUnion ReadJson(Newtonsoft.Json.JsonReader reader, ValueUnion _o, Newtonsoft.Json.JsonSerializer serializer) {
    if (_o == null) return null;
    switch (_o.Type) {
      default: break;
      case Value.IntValue: _o.Value_ = serializer.Deserialize<union_value_collsion.IntValueT>(reader); break;
    }
    return _o;
  }
}



static public class ValueVerify
{
  static public bool Verify(Fivemid.FiveFlat.Verifier verifier, byte typeId, uint tablePos)
  {
    bool result = true;
    switch((Value)typeId)
    {
      case Value.IntValue:
        result = union_value_collsion.IntValueVerify.Verify(verifier, tablePos);
        break;
      default: result = true;
        break;
    }
    return result;
  }
}

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum Other : byte
{
  NONE = 0,
  IntValue = 1,
};

public class OtherUnion {
  public Other Type { get; set; }
  public object Value { get; set; }

  public OtherUnion() {
    this.Type = Other.NONE;
    this.Value = null;
  }

  public T As<T>() where T : class { return this.Value as T; }
  public union_value_collsion.IntValueT AsIntValue() { return this.As<union_value_collsion.IntValueT>(); }
  public static OtherUnion FromIntValue(union_value_collsion.IntValueT _intvalue) { return new OtherUnion{ Type = Other.IntValue, Value = _intvalue }; }

  public static int Pack(ref Fivemid.FiveFlat.FlatBufferBuilder builder, OtherUnion _o) {
    switch (_o.Type) {
      default: return 0;
      case Other.IntValue: return union_value_collsion.IntValue.Pack(ref builder, _o.AsIntValue()).Value;
    }
  }
}

public class OtherUnion_JsonConverter : Newtonsoft.Json.JsonConverter {
  public override bool CanConvert(System.Type objectType) {
    return objectType == typeof(OtherUnion) || objectType == typeof(System.Collections.Generic.List<OtherUnion>);
  }
  public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer) {
    var _olist = value as System.Collections.Generic.List<OtherUnion>;
    if (_olist != null) {
      writer.WriteStartArray();
      foreach (var _o in _olist) { this.WriteJson(writer, _o, serializer); }
      writer.WriteEndArray();
    } else {
      this.WriteJson(writer, value as OtherUnion, serializer);
    }
  }
  public void WriteJson(Newtonsoft.Json.JsonWriter writer, OtherUnion _o, Newtonsoft.Json.JsonSerializer serializer) {
    if (_o == null) return;
    serializer.Serialize(writer, _o.Value);
  }
  public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer) {
    var _olist = existingValue as System.Collections.Generic.List<OtherUnion>;
    if (_olist != null) {
      for (var _j = 0; _j < _olist.Count; ++_j) {
        reader.Read();
        _olist[_j] = this.ReadJson(reader, _olist[_j], serializer);
      }
      reader.Read();
      return _olist;
    } else {
      return this.ReadJson(reader, existingValue as OtherUnion, serializer);
    }
  }
  public OtherUnion ReadJson(Newtonsoft.Json.JsonReader reader, OtherUnion _o, Newtonsoft.Json.JsonSerializer serializer) {
    if (_o == null) return null;
    switch (_o.Type) {
      default: break;
      case Other.IntValue: _o.Value = serializer.Deserialize<union_value_collsion.IntValueT>(reader); break;
    }
    return _o;
  }
}



static public class OtherVerify
{
  static public bool Verify(Fivemid.FiveFlat.Verifier verifier, byte typeId, uint tablePos)
  {
    bool result = true;
    switch((Other)typeId)
    {
      case Other.IntValue:
        result = union_value_collsion.IntValueVerify.Verify(verifier, tablePos);
        break;
      default: result = true;
        break;
    }
    return result;
  }
}

public struct IntValue : IFlatBufferObject
{
  private Table __p;
  public ref ByteBuffer ByteBuffer { get { return ref __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FIVE_FLAT_23_11_29(); }
  public static IntValue GetRootAsIntValue(ref ByteBuffer _bb) { return GetRootAsIntValue(ref _bb, new IntValue()); }
  public static IntValue GetRootAsIntValue(ref ByteBuffer _bb, IntValue obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, ref _bb)); }
  public void __init(int _i, ref ByteBuffer _bb) { __p = new Table(_i, ref _bb); }
  public IntValue __assign(int _i, ref ByteBuffer _bb) { __init(_i, ref _bb); return this; }

  public int Value { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }

  public static Offset<union_value_collsion.IntValue> CreateIntValue(ref FlatBufferBuilder builder,
      int value = 0) {
    builder.StartTable(1);
    IntValue.AddValue(ref builder, value);
    return IntValue.EndIntValue(ref builder);
  }

  public static void StartIntValue(ref FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddValue(ref FlatBufferBuilder builder, int value) { builder.AddInt(0, value, 0); }
  public static Offset<union_value_collsion.IntValue> EndIntValue(ref FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<union_value_collsion.IntValue>(o);
  }
  public IntValueT UnPack() {
    var _o = new IntValueT();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(IntValueT _o) {
    _o.Value = this.Value;
  }
  public static Offset<union_value_collsion.IntValue> Pack(ref FlatBufferBuilder builder, IntValueT _o) {
    if (_o == null) return default(Offset<union_value_collsion.IntValue>);
    return CreateIntValue(
      ref builder,
      _o.Value);
  }
}

public class IntValueT
{
  [Newtonsoft.Json.JsonProperty("value")]
  public int Value { get; set; }

  public IntValueT() {
    this.Value = 0;
  }
}


static public class IntValueVerify
{
  static public bool Verify(Fivemid.FiveFlat.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyField(tablePos, 4 /*Value*/, 4 /*int*/, 4, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}
public struct Collide : IFlatBufferObject
{
  private Table __p;
  public ref ByteBuffer ByteBuffer { get { return ref __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FIVE_FLAT_23_11_29(); }
  public static Collide GetRootAsCollide(ref ByteBuffer _bb) { return GetRootAsCollide(ref _bb, new Collide()); }
  public static Collide GetRootAsCollide(ref ByteBuffer _bb, Collide obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, ref _bb)); }
  public void __init(int _i, ref ByteBuffer _bb) { __p = new Table(_i, ref _bb); }
  public Collide __assign(int _i, ref ByteBuffer _bb) { __init(_i, ref _bb); return this; }

  public NativeArray<byte>? Collide_ { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
  public NativeArray<byte>? Value { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }

  public static Offset<union_value_collsion.Collide> CreateCollide(ref FlatBufferBuilder builder,
      StringOffset collideOffset = default(StringOffset),
      StringOffset valueOffset = default(StringOffset)) {
    builder.StartTable(2);
    Collide.AddValue(ref builder, valueOffset);
    Collide.AddCollide(ref builder, collideOffset);
    return Collide.EndCollide(ref builder);
  }

  public static void StartCollide(ref FlatBufferBuilder builder) { builder.StartTable(2); }
  public static void AddCollide(ref FlatBufferBuilder builder, StringOffset collideOffset) { builder.AddOffset(0, collideOffset.Value, 0); }
  public static void AddValue(ref FlatBufferBuilder builder, StringOffset valueOffset) { builder.AddOffset(1, valueOffset.Value, 0); }
  public static Offset<union_value_collsion.Collide> EndCollide(ref FlatBufferBuilder builder) {
    int o = builder.EndTable();
    builder.Required(o, 4);  // collide
    return new Offset<union_value_collsion.Collide>(o);
  }

  public static int CompareCollide(ref FlatBufferBuilder builder, Offset<Collide> o1, Offset<Collide> o2) {
    return new Collide().__assign(builder._bb.Length - o1.Value, ref builder._bb).Collide_.CompareTo(new Collide().__assign(builder._bb.Length - o2.Value, ref builder._bb).Collide_);
  }

  public static Collide? __lookup_by_key(int vectorLocation, NativeArray<byte>? key, ref ByteBuffer bb) {
    Collide obj_ = new Collide();
    int span = bb.GetInt(vectorLocation - 4);
    int start = 0;
    while (span != 0) {
      int middle = span / 2;
      int tableOffset = Table.__indirect(vectorLocation + 4 * (start + middle), ref bb);
      obj_.__assign(tableOffset, ref bb);
      int comp = obj_.Collide_.CompareTo(key);
      if (comp > 0) {
        span = middle;
      } else if (comp < 0) {
        middle++;
        start += middle;
        span -= middle;
      } else {
        return obj_;
      }
    }
    return null;
  }
  public CollideT UnPack() {
    var _o = new CollideT();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(CollideT _o) {
    _o.Collide_ = FlatBufferBuilder.DecodeString(this.Collide_);
    _o.Value = FlatBufferBuilder.DecodeString(this.Value);
  }
  public static Offset<union_value_collsion.Collide> Pack(ref FlatBufferBuilder builder, CollideT _o) {
    if (_o == null) return default(Offset<union_value_collsion.Collide>);
    var _collide = _o.Collide_ == null ? default(StringOffset) : builder.CreateString(_o.Collide_);
    var _value = _o.Value == null ? default(StringOffset) : builder.CreateString(_o.Value);
    return CreateCollide(
      ref builder,
      _collide,
      _value);
  }
}

public class CollideT
{
  [Newtonsoft.Json.JsonProperty("collide")]
  public string Collide_ { get; set; }
  [Newtonsoft.Json.JsonProperty("value")]
  public string Value { get; set; }

  public CollideT() {
    this.Collide_ = null;
    this.Value = null;
  }
}


static public class CollideVerify
{
  static public bool Verify(Fivemid.FiveFlat.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyString(tablePos, 4 /*Collide*/, true)
      && verifier.VerifyString(tablePos, 6 /*Value*/, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}
public struct Collision : IFlatBufferObject
{
  private Table __p;
  public ref ByteBuffer ByteBuffer { get { return ref __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FIVE_FLAT_23_11_29(); }
  public static Collision GetRootAsCollision(ref ByteBuffer _bb) { return GetRootAsCollision(ref _bb, new Collision()); }
  public static Collision GetRootAsCollision(ref ByteBuffer _bb, Collision obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, ref _bb)); }
  public static bool VerifyCollision(ByteBuffer _bb) {Fivemid.FiveFlat.Verifier verifier = new Fivemid.FiveFlat.Verifier(_bb); return verifier.VerifyBuffer("", false, CollisionVerify.Verify); }
  public void __init(int _i, ref ByteBuffer _bb) { __p = new Table(_i, ref _bb); }
  public Collision __assign(int _i, ref ByteBuffer _bb) { __init(_i, ref _bb); return this; }

  public union_value_collsion.Value SomeValueType { get { int o = __p.__offset(4); return o != 0 ? (union_value_collsion.Value)__p.bb.Get(o + __p.bb_pos) : union_value_collsion.Value.NONE; } }
  public TTable? SomeValue<TTable>() where TTable : struct, IFlatBufferObject { int o = __p.__offset(6); return o != 0 ? (TTable?)__p.__union<TTable>(o + __p.bb_pos) : null; }
  public union_value_collsion.IntValue SomeValueAsIntValue() { return SomeValue<union_value_collsion.IntValue>().Value; }
  public union_value_collsion.Other ValueType { get { int o = __p.__offset(8); return o != 0 ? (union_value_collsion.Other)__p.bb.Get(o + __p.bb_pos) : union_value_collsion.Other.NONE; } }
  public TTable? Value<TTable>() where TTable : struct, IFlatBufferObject { int o = __p.__offset(10); return o != 0 ? (TTable?)__p.__union<TTable>(o + __p.bb_pos) : null; }
  public union_value_collsion.IntValue ValueAsIntValue() { return Value<union_value_collsion.IntValue>().Value; }
  public union_value_collsion.Collision? Collide(int j) { int o = __p.__offset(12); return o != 0 ? (union_value_collsion.Collision?)(new union_value_collsion.Collision()).__assign(__p.__indirect(__p.__vector(o) + j * 4), ref __p.bb) : null; }
  public int CollideLength { get { int o = __p.__offset(12); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<union_value_collsion.Collision> CreateCollision(ref FlatBufferBuilder builder,
      union_value_collsion.Value some_value_type = union_value_collsion.Value.NONE,
      int some_valueOffset = 0,
      union_value_collsion.Other value_type = union_value_collsion.Other.NONE,
      int valueOffset = 0,
      VectorOffset collideOffset = default(VectorOffset)) {
    builder.StartTable(5);
    Collision.AddCollide(ref builder, collideOffset);
    Collision.AddValue(ref builder, valueOffset);
    Collision.AddSomeValue(ref builder, some_valueOffset);
    Collision.AddValueType(ref builder, value_type);
    Collision.AddSomeValueType(ref builder, some_value_type);
    return Collision.EndCollision(ref builder);
  }

  public static void StartCollision(ref FlatBufferBuilder builder) { builder.StartTable(5); }
  public static void AddSomeValueType(ref FlatBufferBuilder builder, union_value_collsion.Value someValueType) { builder.AddByte(0, (byte)someValueType, 0); }
  public static void AddSomeValue(ref FlatBufferBuilder builder, int someValueOffset) { builder.AddOffset(1, someValueOffset, 0); }
  public static void AddValueType(ref FlatBufferBuilder builder, union_value_collsion.Other valueType) { builder.AddByte(2, (byte)valueType, 0); }
  public static void AddValue(ref FlatBufferBuilder builder, int valueOffset) { builder.AddOffset(3, valueOffset, 0); }
  public static void AddCollide(ref FlatBufferBuilder builder, VectorOffset collideOffset) { builder.AddOffset(4, collideOffset.Value, 0); }
  public static VectorOffset CreateCollideVector(ref FlatBufferBuilder builder, Span<Offset<union_value_collsion.Collision>> data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartCollideVector(ref FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<union_value_collsion.Collision> EndCollision(ref FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<union_value_collsion.Collision>(o);
  }
  public static void FinishCollisionBuffer(ref FlatBufferBuilder builder, Offset<union_value_collsion.Collision> offset) { builder.Finish(offset.Value); }
  public static void FinishSizePrefixedCollisionBuffer(ref FlatBufferBuilder builder, Offset<union_value_collsion.Collision> offset) { builder.FinishSizePrefixed(offset.Value); }
  public CollisionT UnPack() {
    var _o = new CollisionT();
    this.UnPackTo(_o);
    return _o;
  }
  public void UnPackTo(CollisionT _o) {
    _o.SomeValue = new union_value_collsion.ValueUnion();
    _o.SomeValue.Type = this.SomeValueType;
    switch (this.SomeValueType) {
      default: break;
      case union_value_collsion.Value.IntValue:
        _o.SomeValue.Value_ = this.SomeValue<union_value_collsion.IntValue>().HasValue ? this.SomeValue<union_value_collsion.IntValue>().Value.UnPack() : null;
        break;
    }
    _o.Value = new union_value_collsion.OtherUnion();
    _o.Value.Type = this.ValueType;
    switch (this.ValueType) {
      default: break;
      case union_value_collsion.Other.IntValue:
        _o.Value.Value = this.Value<union_value_collsion.IntValue>().HasValue ? this.Value<union_value_collsion.IntValue>().Value.UnPack() : null;
        break;
    }
    _o.Collide = new List<union_value_collsion.CollisionT>();
    for (var _j = 0; _j < this.CollideLength; ++_j) {_o.Collide.Add(this.Collide(_j).HasValue ? this.Collide(_j).Value.UnPack() : null);}
  }
  public static Offset<union_value_collsion.Collision> Pack(ref FlatBufferBuilder builder, CollisionT _o) {
    if (_o == null) return default(Offset<union_value_collsion.Collision>);
    var _some_value_type = _o.SomeValue == null ? union_value_collsion.Value.NONE : _o.SomeValue.Type;
    var _some_value = _o.SomeValue == null ? 0 : union_value_collsion.ValueUnion.Pack(ref builder, _o.SomeValue);
    var _value_type = _o.Value == null ? union_value_collsion.Other.NONE : _o.Value.Type;
    var _value = _o.Value == null ? 0 : union_value_collsion.OtherUnion.Pack(ref builder, _o.Value);
    var _collide = default(VectorOffset);
    if (_o.Collide != null) {
      var __collide = new Offset<union_value_collsion.Collision>[_o.Collide.Count];
      for (var _j = 0; _j < __collide.Length; ++_j) { __collide[_j] = union_value_collsion.Collision.Pack(ref builder, _o.Collide[_j]); }
      _collide = CreateCollideVector(ref builder, __collide);
    }
    return CreateCollision(
      ref builder,
      _some_value_type,
      _some_value,
      _value_type,
      _value,
      _collide);
  }
}

public class CollisionT
{
  [Newtonsoft.Json.JsonProperty("some_value_type")]
  private union_value_collsion.Value SomeValueType {
    get {
      return this.SomeValue != null ? this.SomeValue.Type : union_value_collsion.Value.NONE;
    }
    set {
      this.SomeValue = new union_value_collsion.ValueUnion();
      this.SomeValue.Type = value;
    }
  }
  [Newtonsoft.Json.JsonProperty("some_value")]
  [Newtonsoft.Json.JsonConverter(typeof(union_value_collsion.ValueUnion_JsonConverter))]
  public union_value_collsion.ValueUnion SomeValue { get; set; }
  [Newtonsoft.Json.JsonProperty("value_type")]
  private union_value_collsion.Other ValueType {
    get {
      return this.Value != null ? this.Value.Type : union_value_collsion.Other.NONE;
    }
    set {
      this.Value = new union_value_collsion.OtherUnion();
      this.Value.Type = value;
    }
  }
  [Newtonsoft.Json.JsonProperty("value")]
  [Newtonsoft.Json.JsonConverter(typeof(union_value_collsion.OtherUnion_JsonConverter))]
  public union_value_collsion.OtherUnion Value { get; set; }
  [Newtonsoft.Json.JsonProperty("collide")]
  public List<union_value_collsion.CollisionT> Collide { get; set; }

  public CollisionT() {
    this.SomeValue = null;
    this.Value = null;
    this.Collide = null;
  }

  public static CollisionT DeserializeFromJson(string jsonText) {
    return Newtonsoft.Json.JsonConvert.DeserializeObject<CollisionT>(jsonText);
  }
  public string SerializeToJson() {
    return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
  }
  public static CollisionT DeserializeFromBinary(NativeArray<byte> fbBuffer) {
    ByteBuffer bb = new ByteBuffer(fbBuffer);
    return Collision.GetRootAsCollision(ref bb).UnPack();
  }
  public NativeArray<byte> SerializeToBinary(Allocator allocator) {
    var fbb = new FlatBufferBuilder(0x10000, allocator);
    Collision.FinishCollisionBuffer(ref fbb, Collision.Pack(ref fbb, this));
    return fbb._bb.ToSizedArray();
  }
}


static public class CollisionVerify
{
  static public bool Verify(Fivemid.FiveFlat.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyField(tablePos, 4 /*SomeValueType*/, 1 /*union_value_collsion.Value*/, 1, false)
      && verifier.VerifyUnion(tablePos, 4, 6 /*SomeValue*/, union_value_collsion.ValueVerify.Verify, false)
      && verifier.VerifyField(tablePos, 8 /*ValueType*/, 1 /*union_value_collsion.Other*/, 1, false)
      && verifier.VerifyUnion(tablePos, 8, 10 /*Value*/, union_value_collsion.OtherVerify.Verify, false)
      && verifier.VerifyVectorOfTables(tablePos, 12 /*Collide*/, union_value_collsion.CollisionVerify.Verify, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}

}
