using System;
using System.Buffers.Binary;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Fivemid.FiveFlat
{
    public struct ByteBuffer : IDisposable
    {
        private NativeArray<byte> _buffer;
        private Allocator _allocator;
        private int _pos;

        private Span<byte> Span { get { return _buffer.AsSpan(); } }

        private ReadOnlySpan<byte> ReadOnlySpan { get { return _buffer.AsReadOnlySpan(); } }

        public ByteBuffer(int size, Allocator allocator)
        {
            _buffer = new NativeArray<byte>(size, allocator);
            _allocator = allocator;
            _pos = 0;
        }

        public ByteBuffer(NativeArray<byte> buffer, int pos = 0)
        {
            _buffer = buffer;
            _allocator = Allocator.Invalid;
            _pos = pos;
        }

        public void Dispose()
        {
            if (_buffer.IsCreated)
                _buffer.Dispose();
        }

        public int Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public int Length { get { return _buffer.Length; } }

        public void Reset()
        {
            _pos = 0;
        }

        // Create a new ByteBuffer on the same underlying data.
        // The new ByteBuffer's position will be same as this buffer's.
        public ByteBuffer Duplicate()
        {
            return new ByteBuffer(_buffer, Position);
        }

        // Increases the size of the ByteBuffer, and copies the old data towards
        // the end of the new buffer.
        public unsafe void GrowFront(int newSize)
        {
            if (_allocator == Allocator.Invalid)
                throw new Exception("This ByteBuffer cannot grow since it was created with an existing NativeArray");

            if ((Length & 0xC0000000) != 0)
                throw new Exception(
                    "ByteBuffer: cannot grow buffer beyond 2 gigabytes.");

            if (newSize < Length)
                throw new Exception("ByteBuffer: cannot truncate buffer.");

            NativeArray<byte> newBuffer = new NativeArray<byte>(newSize, _allocator);
            UnsafeUtility.MemCpy(
                (byte*)newBuffer.GetUnsafePtr() + (newSize - Length),
                (byte*)_buffer.GetUnsafeReadOnlyPtr(),
                Length);
            _buffer = newBuffer;
        }

        public NativeArray<byte> ToArray(int pos, int len)
        {
            return ToArray<byte>(pos, len);
        }

        /// <summary>
        /// Get the wire-size (in bytes) of a type supported by flatbuffers.
        /// </summary>
        /// <param name="t">The type to get the wire size of</param>
        /// <returns></returns>
        public static unsafe int SizeOf<T>() where T : unmanaged
        {
            return sizeof(T);
        }

        /// <summary>
        /// Checks if the Type provided is supported as scalar value
        /// </summary>
        /// <typeparam name="T">The Type to check</typeparam>
        /// <returns>True if the type is a scalar type that is supported, falsed otherwise</returns>
        public static bool IsSupportedType<T>() where T : unmanaged
        {
            return true; // TODO improve me
        }

        public unsafe NativeArray<T> ToArray<T>(int pos, int len)
            where T : unmanaged
        {
            AssertOffsetAndLength(pos, len);
            return _buffer.GetSubArray(pos, sizeof(T) * len).Reinterpret<T>(1);
        }

        public NativeArray<byte> ToSizedArray()
        {
            return ToArray<byte>(Position, Length - Position);
        }

        public NativeArray<byte> ToFullArray()
        {
            return ToArray<byte>(0, Length);
        }

        static public ushort ReverseBytes(ushort input)
        {
            return (ushort)(((input & 0x00FFU) << 8) |
                            ((input & 0xFF00U) >> 8));
        }
        static public uint ReverseBytes(uint input)
        {
            return ((input & 0x000000FFU) << 24) |
                   ((input & 0x0000FF00U) << 8) |
                   ((input & 0x00FF0000U) >> 8) |
                   ((input & 0xFF000000U) >> 24);
        }
        static public ulong ReverseBytes(ulong input)
        {
            return (((input & 0x00000000000000FFUL) << 56) |
                    ((input & 0x000000000000FF00UL) << 40) |
                    ((input & 0x0000000000FF0000UL) << 24) |
                    ((input & 0x00000000FF000000UL) << 8) |
                    ((input & 0x000000FF00000000UL) >> 8) |
                    ((input & 0x0000FF0000000000UL) >> 24) |
                    ((input & 0x00FF000000000000UL) >> 40) |
                    ((input & 0xFF00000000000000UL) >> 56));
        }

        private void AssertOffsetAndLength(int offset, int length)
        {
#if !BYTEBUFFER_NO_BOUNDS_CHECK
            if (offset < 0 ||
                offset > _buffer.Length - length)
                throw new ArgumentOutOfRangeException();
#endif
        }

        public void PutSbyte(int offset, sbyte value)
        {
            AssertOffsetAndLength(offset, sizeof(sbyte));
            _buffer[offset] = (byte)value;
        }

        public void PutByte(int offset, byte value)
        {
            AssertOffsetAndLength(offset, sizeof(byte));
            _buffer[offset] = value;
        }

        public void PutByte(int offset, byte value, int count)
        {
            AssertOffsetAndLength(offset, sizeof(byte) * count);
            Span<byte> span = Span.Slice(offset, count);
            for (var i = 0; i < span.Length; ++i)
                span[i] = value;
        }

        public void PutStringUTF8(int offset, NativeArray<byte> value)
        {
            AssertOffsetAndLength(offset, value.Length);
            Span<byte> span = Span.Slice(offset, value.Length);
            for (var i = 0; i < span.Length; ++i)
                span[i] = value[i];
        }

        public void PutShort(int offset, short value)
        {
            PutUshort(offset, (ushort)value);
        }

        public void PutUshort(int offset, ushort value)
        {
            AssertOffsetAndLength(offset, sizeof(ushort));
            Span<byte> span = Span.Slice(offset);
            BinaryPrimitives.WriteUInt16LittleEndian(span, value);

        }

        public void PutInt(int offset, int value)
        {
            PutUint(offset, (uint)value);
        }

        public void PutUint(int offset, uint value)
        {
            AssertOffsetAndLength(offset, sizeof(uint));
            Span<byte> span = Span.Slice(offset);
            BinaryPrimitives.WriteUInt32LittleEndian(span, value);
        }

        public void PutLong(int offset, long value)
        {
            PutUlong(offset, (ulong)value);
        }

        public void PutUlong(int offset, ulong value)
        {
            AssertOffsetAndLength(offset, sizeof(ulong));
            Span<byte> span = Span.Slice(offset);
            BinaryPrimitives.WriteUInt64LittleEndian(span, value);
        }

        public unsafe void PutFloat(int offset, float value)
        {
            AssertOffsetAndLength(offset, sizeof(float));
            byte* ptr = (byte*)_buffer.GetUnsafePtr();
            {
                if (BitConverter.IsLittleEndian)
                {
                    *(float*)(ptr + offset) = value;
                }
                else
                {
                    *(uint*)(ptr + offset) = ReverseBytes(*(uint*)(&value));
                }
            }
        }

        public unsafe void PutDouble(int offset, double value)
        {
            AssertOffsetAndLength(offset, sizeof(double));
            byte* ptr = (byte*)_buffer.GetUnsafePtr();
            {
                if (BitConverter.IsLittleEndian)
                {
                    *(double*)(ptr + offset) = value;
                }
                else
                {
                    *(ulong*)(ptr + offset) = ReverseBytes(*(ulong*)(&value));
                }
            }
        }

        public sbyte GetSbyte(int index)
        {
            AssertOffsetAndLength(index, sizeof(sbyte));
            return (sbyte)ReadOnlySpan[index];
        }

        public byte Get(int index)
        {
            AssertOffsetAndLength(index, sizeof(byte));
            return ReadOnlySpan[index];
        }

        public short GetShort(int offset)
        {
            return (short)GetUshort(offset);
        }

        public unsafe ushort GetUshort(int offset)
        {
            AssertOffsetAndLength(offset, sizeof(ushort));
            ReadOnlySpan<byte> span = ReadOnlySpan.Slice(offset);
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
        }

        public int GetInt(int offset)
        {
            return (int)GetUint(offset);
        }

        public unsafe uint GetUint(int offset)
        {
            AssertOffsetAndLength(offset, sizeof(uint));
            ReadOnlySpan<byte> span = ReadOnlySpan.Slice(offset);
            return BinaryPrimitives.ReadUInt32LittleEndian(span);
        }

        public long GetLong(int offset)
        {
            return (long)GetUlong(offset);
        }

        public unsafe ulong GetUlong(int offset)
        {
            AssertOffsetAndLength(offset, sizeof(ulong));
            ReadOnlySpan<byte> span = ReadOnlySpan.Slice(offset);
            return BinaryPrimitives.ReadUInt64LittleEndian(span);
        }

        public unsafe float GetFloat(int offset)
        {
            AssertOffsetAndLength(offset, sizeof(float));
            byte* ptr = (byte*)_buffer.GetUnsafeReadOnlyPtr();
            {
                if (BitConverter.IsLittleEndian)
                {
                    return *(float*)(ptr + offset);
                }
                else
                {
                    uint uvalue = ReverseBytes(*(uint*)(ptr + offset));
                    return *(float*)(&uvalue);
                }
            }
        }

        public unsafe double GetDouble(int offset)
        {
            AssertOffsetAndLength(offset, sizeof(double));
            byte* ptr = (byte*)_buffer.GetUnsafeReadOnlyPtr();
            {
                if (BitConverter.IsLittleEndian)
                {
                    return *(double*)(ptr + offset);
                }
                else
                {
                    ulong uvalue = ReverseBytes(*(ulong*)(ptr + offset));
                    return *(double*)(&uvalue);
                }
            }
        }
    }
}
