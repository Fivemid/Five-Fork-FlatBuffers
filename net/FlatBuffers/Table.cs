/*
 * Copyright 2014 Google Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Text;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Fivemid.FiveFlat
{
    /// <summary>
    /// All tables in the generated code derive from this struct, and add their own accessors.
    /// </summary>
    public readonly unsafe struct Table
    {
        public readonly int bb_pos;
        private readonly ByteBuffer* _bb;

        public ref ByteBuffer bb { get { return ref *_bb; } }

        // Re-init the internal state with an external buffer {@code ByteBuffer} and an offset within.
        public Table(int _i, ref ByteBuffer _bb)
        {
            this._bb = (ByteBuffer*)UnsafeUtility.AddressOf(ref _bb);
            bb_pos = _i;
        }

        // Look up a field in the vtable, return an offset into the object, or 0 if the field is not
        // present.
        public int __offset(int vtableOffset)
        {
            int vtable = bb_pos - bb.GetInt(bb_pos);
            return vtableOffset < bb.GetShort(vtable) ? (int)bb.GetShort(vtable + vtableOffset) : 0;
        }

        public static int __offset(int vtableOffset, int offset, ref ByteBuffer bb)
        {
            int vtable = bb.Length - offset;
            return bb.GetShort(vtable + vtableOffset - bb.GetInt(vtable)) + vtable;
        }

        // Retrieve the relative offset stored at "offset"
        public int __indirect(int offset)
        {
            return offset + bb.GetInt(offset);
        }

        public static int __indirect(int offset, ref ByteBuffer bb)
        {
            return offset + bb.GetInt(offset);
        }

        public NativeArray<byte>? __string(int offset)
        {
            int stringOffset = bb.GetInt(offset);
            if (stringOffset == 0)
                return null;

            offset += stringOffset;
            var len = bb.GetInt(offset);
            var startPos = offset + sizeof(int);
            return bb.ToArray(startPos, len);
        }

        // Get the length of a vector whose offset is stored at "offset" in this object.
        public int __vector_len(int offset)
        {
            offset += bb_pos;
            offset += bb.GetInt(offset);
            return bb.GetInt(offset);
        }

        // Get the start of data of a vector whose offset is stored at "offset" in this object.
        public int __vector(int offset)
        {
            offset += bb_pos;
            return offset + bb.GetInt(offset) + sizeof(int); // data starts after the length
        }

        // Initialize any Table-derived type to point to the union at the given offset.
        public T __union_as<T>(int offset) where T : struct, IFlatBufferObject
        {
            T t = new T();
            t.__init(__indirect(offset), ref bb);
            return t;
        }

        public Union<TType> __union<TType>(TType type, int offsetValue) where TType : unmanaged
        {
            return new Union<TType>(type, __indirect(offsetValue), ref bb);
        }

        public Union<TType> __union_none<TType>() where TType : unmanaged
        {
            return new Union<TType>(default(TType), 0, ref bb);
        }

        public static bool __has_identifier(ByteBuffer bb, string ident)
        {
            if (ident.Length != FlatBufferConstants.FileIdentifierLength)
                throw new ArgumentException("FlatBuffers: file identifier must be length " + FlatBufferConstants.FileIdentifierLength, "ident");

            for (var i = 0; i < FlatBufferConstants.FileIdentifierLength; i++)
            {
                if (ident[i] != (char)bb.Get(bb.Position + sizeof(int) + i)) return false;
            }

            return true;
        }
    }
}
