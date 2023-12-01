using Unity.Collections.LowLevel.Unsafe;

namespace Fivemid.FiveFlat
{
    public readonly unsafe struct Union<TType> where TType : unmanaged
    {
        private readonly TType type;
        private readonly int value_pos;
        private readonly ByteBuffer* _bb;

        private ref ByteBuffer bb { get { return ref *_bb; } }

        public TType Type { get { return type; } }

        public TValue Value<TValue>() where TValue : struct, IFlatBufferObject
        {
            TValue value = new TValue();
            value.__init(value_pos, ref bb);
            return value;
        }

        public Union(TType type, int valuePos, ref ByteBuffer bb)
        {
            _bb = (ByteBuffer*)UnsafeUtility.AddressOf(ref bb);
            this.type = type;
            value_pos = valuePos;
        }
    }
}
