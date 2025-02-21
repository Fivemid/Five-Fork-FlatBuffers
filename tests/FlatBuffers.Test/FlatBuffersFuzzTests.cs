/*
 * Copyright 2015 Google Inc. All rights reserved.
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
using Unity.Collections;

namespace Fivemid.FiveFlat.Tests.Exported
{
    [FlatBuffersTestClass]
    public class FlatBuffersFuzzTests
    {
        private readonly Lcg _lcg = new Lcg();

        [FlatBuffersTestMethod]
        public void TestObjects()
        {
            CheckObjects(11, 100);
        }

        [FlatBuffersTestMethod]
        public void TestNumbers()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            Assert.ArrayEqual(new byte[] { 0 }, builder._bb.ToFullArray());
            builder.AddBool(true);
            Assert.ArrayEqual(new byte[] { 1 }, builder._bb.ToFullArray());
            builder.AddSbyte(-127);
            Assert.ArrayEqual(new byte[] { 129, 1 }, builder._bb.ToFullArray());
            builder.AddByte(255);
            Assert.ArrayEqual(new byte[] { 0, 255, 129, 1 }, builder._bb.ToFullArray()); // First pad
            builder.AddShort(-32222);
            Assert.ArrayEqual(new byte[] { 0, 0, 0x22, 0x82, 0, 255, 129, 1 }, builder._bb.ToFullArray()); // Second pad
            builder.AddUshort(0xFEEE);
            Assert.ArrayEqual(new byte[] { 0xEE, 0xFE, 0x22, 0x82, 0, 255, 129, 1 }, builder._bb.ToFullArray()); // no pad
            builder.AddInt(-53687092);
            Assert.ArrayEqual(new byte[] { 0, 0, 0, 0, 204, 204, 204, 252, 0xEE, 0xFE, 0x22, 0x82, 0, 255, 129, 1 }, builder._bb.ToFullArray()); // third pad
            builder.AddUint(0x98765432);
            Assert.ArrayEqual(new byte[] { 0x32, 0x54, 0x76, 0x98, 204, 204, 204, 252, 0xEE, 0xFE, 0x22, 0x82, 0, 255, 129, 1 }, builder._bb.ToFullArray()); // no pad
        }

        [FlatBuffersTestMethod]
        public void TestNumbers64()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.AddUlong(0x1122334455667788);
            Assert.ArrayEqual(new byte[] { 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 }, builder._bb.ToFullArray());

            builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.AddLong(0x1122334455667788);
            Assert.ArrayEqual(new byte[] { 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 }, builder._bb.ToFullArray());
        }

        [FlatBuffersTestMethod]
        public void TestVector_1xUInt8()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartVector(sizeof(byte), 1, 1);
            Assert.ArrayEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, builder._bb.ToFullArray());
            builder.AddByte(1);
            Assert.ArrayEqual(new byte[] { 0, 0, 0, 0, 1, 0, 0, 0 }, builder._bb.ToFullArray());
            builder.EndVector();
            Assert.ArrayEqual(new byte[] { 1, 0, 0, 0, 1, 0, 0, 0 }, builder._bb.ToFullArray());
        }

        [FlatBuffersTestMethod]
        public void TestVector_2xUint8()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartVector(sizeof(byte), 2, 1);
            Assert.ArrayEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, builder._bb.ToFullArray());
            builder.AddByte(1);
            Assert.ArrayEqual(new byte[] { 0, 0, 0, 0, 0, 1, 0, 0 }, builder._bb.ToFullArray());
            builder.AddByte(2);
            Assert.ArrayEqual(new byte[] { 0, 0, 0, 0, 2, 1, 0, 0 }, builder._bb.ToFullArray());
            builder.EndVector();
            Assert.ArrayEqual(new byte[] { 2, 0, 0, 0, 2, 1, 0, 0 }, builder._bb.ToFullArray());
        }

        [FlatBuffersTestMethod]
        public void TestVector_1xUInt16()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartVector(sizeof(ushort), 1, 1);
            Assert.ArrayEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, builder._bb.ToFullArray());
            builder.AddUshort(1);
            Assert.ArrayEqual(new byte[] { 0, 0, 0, 0, 1, 0, 0, 0 }, builder._bb.ToFullArray());
            builder.EndVector();
            Assert.ArrayEqual(new byte[] { 1, 0, 0, 0, 1, 0, 0, 0 }, builder._bb.ToFullArray());
        }

        [FlatBuffersTestMethod]
        public void TestVector_2xUInt16()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartVector(sizeof(ushort), 2, 1);
            Assert.ArrayEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, builder._bb.ToFullArray());
            builder.AddUshort(0xABCD);
            Assert.ArrayEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0xCD, 0xAB }, builder._bb.ToFullArray());
            builder.AddUshort(0xDCBA);
            Assert.ArrayEqual(new byte[] { 0, 0, 0, 0, 0xBA, 0xDC, 0xCD, 0xAB }, builder._bb.ToFullArray());
            builder.EndVector();
            Assert.ArrayEqual(new byte[] { 2, 0, 0, 0, 0xBA, 0xDC, 0xCD, 0xAB }, builder._bb.ToFullArray());
        }

        [FlatBuffersTestMethod]
        public void TestCreateAsciiString()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.CreateString("foo");
            Assert.ArrayEqual(new byte[] { 3, 0, 0, 0, (byte)'f', (byte)'o', (byte)'o', 0 }, builder._bb.ToFullArray());

            builder.CreateString("moop");
            Assert.ArrayEqual(new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,  // Padding to 32 bytes
                4, 0, 0, 0,
                (byte)'m', (byte)'o', (byte)'o', (byte)'p',
                0, 0, 0, 0, // zero terminator with 3 byte pad
                3, 0, 0, 0,
                (byte)'f', (byte)'o', (byte)'o', 0
            }, builder._bb.ToFullArray());
        }

        [FlatBuffersTestMethod]
        public void TestCreateArbitarytring()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.CreateString("\x01\x02\x03");
            Assert.ArrayEqual(new byte[]
            {
                3, 0, 0, 0,
                0x01, 0x02, 0x03, 0
            }, builder._bb.ToFullArray()); // No padding
            builder.CreateString("\x04\x05\x06\x07");
            Assert.ArrayEqual(new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,  // Padding to 32 bytes
                4, 0, 0, 0,
                0x04, 0x05, 0x06, 0x07,
                0, 0, 0, 0, // zero terminator with 3 byte pad
                3, 0, 0, 0,
                0x01, 0x02, 0x03, 0
            }, builder._bb.ToFullArray()); // No padding
        }

        [FlatBuffersTestMethod]
        public void TestEmptyVTable()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(0);
            Assert.ArrayEqual(new byte[] { 0 }, builder._bb.ToFullArray());
            builder.EndTable();
            Assert.ArrayEqual(new byte[]
            {
                4, 0, 4, 0,
                4, 0, 0, 0
            },
                builder._bb.ToFullArray());
        }

        [FlatBuffersTestMethod]
        public void TestVTableWithOneBool()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(1);
            Assert.ArrayEqual(new byte[] { 0 }, builder._bb.ToFullArray());
            builder.AddBool(0, true, false);
            builder.EndTable();
            Assert.ArrayEqual(new byte[]
            {
                0, 0, // padding to 16 bytes
                6, 0, // vtable bytes
                8, 0, // object length inc vtable offset
                7, 0, // start of bool value
                6, 0, 0, 0, // int32 offset for start of vtable
                0, 0, 0, // padding
                1, // value 0
            },
                builder._bb.ToFullArray());
            var verifier = new Verifier(builder._bb);
            var offset = 8;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart((uint)offset));
            // First field must be bool
            Assert.IsTrue(verifier.VerifyField((uint)offset, 4, 1, 1, true));
            // Check Error: Second field
            Assert.IsFalse(verifier.VerifyField((uint)offset, 6, 1, 1, true));
            // Check Error: First field too big alignment
            Assert.IsFalse(verifier.VerifyField((uint)offset, 4, 1, 2, true));
            // Check Error: First size to big
            Assert.IsFalse(verifier.VerifyField((uint)offset, 4, 2, 1, true));
    }

    [FlatBuffersTestMethod]
        public void TestVTableWithOneBool_DefaultValue()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(1);
            Assert.ArrayEqual(new byte[] { 0 }, builder._bb.ToFullArray());
            builder.AddBool(0, false, false);
            builder.EndTable();
            Assert.ArrayEqual(new byte[]
            {
                // No padding.
                4, 0, // vtable bytes
                4, 0, // end of object from here
                // entry 0 is not stored (trimmed end of vtable)
                4, 0, 0, 0, // int32 offset for start of vtable
            },
                builder._bb.ToFullArray());
            var verifier = new Verifier(builder._bb);
            var offset = 4;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart((uint)offset));
            // First field must be bool
            Assert.IsTrue(verifier.VerifyField((uint)offset, 4, 1, 1, false));
            // Error Check: First field not present
            Assert.IsFalse(verifier.VerifyField((uint)offset, 4, 1, 1, true));
        }

        [FlatBuffersTestMethod]
        public void TestVTableWithOneInt16()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(1);
            Assert.ArrayEqual(new byte[] { 0 }, builder._bb.ToFullArray());
            builder.AddShort(0, 0x789A, 0);
            int offset = builder.EndTable();
            Assert.ArrayEqual(new byte[]
            {
                0, 0, // padding to 16 bytes
                6, 0, // vtable bytes
                8, 0, // object length inc vtable offset
                6, 0, // start of int16 value
                6, 0, 0, 0, // int32 offset for start of vtable
                0, 0, // padding
                0x9A, 0x78, //value 0
            },
                builder._bb.ToFullArray());
            var verifier = new Verifier(builder._bb);
            offset += builder._bb.Position;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart((uint)offset));
            // First field must be ushort
            Assert.IsTrue(verifier.VerifyField((uint)offset, 4, 2, 2, true));
            // Check Error: Second field
            Assert.IsFalse(verifier.VerifyField((uint)offset, 6, 2, 2, true));
            // Check Error: First field too big alignment
            Assert.IsFalse(verifier.VerifyField((uint)offset, 4, 4, 2, true));
            // Check Error: First field size to big
            Assert.IsFalse(verifier.VerifyField((uint)offset, 4, 2, 4, true));
        }

        [FlatBuffersTestMethod]
        public void TestVTableWithTwoInt16()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(2);
            Assert.ArrayEqual(new byte[] { 0 }, builder._bb.ToFullArray());
            builder.AddShort(0, 0x3456, 0);
            builder.AddShort(1, 0x789A, 0);
            int offset = builder.EndTable();
            Assert.ArrayEqual(new byte[]
            {
                8, 0, // vtable bytes
                8, 0, // object length inc vtable offset
                6, 0, // start of int16 value 0
                4, 0, // start of int16 value 1
                8, 0, 0, 0, // int32 offset for start of vtable
                0x9A, 0x78, // value 1
                0x56, 0x34, // value 0
            },
                builder._bb.ToFullArray());
            var verifier = new Verifier(builder._bb);
            offset += builder._bb.Position;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart((uint)offset));
            // First field must be ushort
            Assert.IsTrue(verifier.VerifyField((uint)offset, 4, 2, 2, true));
            // Check Error: Second field
            Assert.IsTrue(verifier.VerifyField((uint)offset, 6, 2, 2, true));
            // Check Error: Second field too big alignment
            Assert.IsFalse(verifier.VerifyField((uint)offset, 4, 4, 2, true));
            // Check Error: Second field size to big
            Assert.IsFalse(verifier.VerifyField((uint)offset, 4, 2, 4, true));
        }

        [FlatBuffersTestMethod]
        public void TestVTableWithInt16AndBool()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(2);
            Assert.ArrayEqual(new byte[] { 0 }, builder._bb.ToFullArray());
            builder.AddShort(0, 0x3456, 0);
            builder.AddBool(1, true, false);
            int offset = builder.EndTable();
            Assert.ArrayEqual(new byte[]
            {
                8, 0, // vtable bytes
                8, 0, // object length inc vtable offset
                6, 0, // start of int16 value 0
                5, 0, // start of bool value 1
                8, 0, 0, 0, // int32 offset for start of vtable
                0, 1, // padding + value 1
                0x56, 0x34, // value 0
            },
                builder._bb.ToFullArray());
            var verifier = new Verifier(builder._bb);
            offset += builder._bb.Position;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart((uint)offset));
            // First field must be ushort
            Assert.IsTrue(verifier.VerifyField((uint)offset, 4, 2, 2, true));
            // Check Error: Second field must be bool
            Assert.IsTrue(verifier.VerifyField((uint)offset, 6, 1, 1, true));
            // Check Error: Second field too big alignment
            Assert.IsFalse(verifier.VerifyField((uint)offset, 4, 4, 2, true));
            // Check Error: Second field size to big
            Assert.IsFalse(verifier.VerifyField((uint)offset, 4, 2, 4, true));
        }

        [FlatBuffersTestMethod]
        public void TestVTableWithEmptyVector()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartVector(sizeof(byte), 0, 1);
            var vecEnd = builder.EndVector();

            builder.StartTable(1);

            builder.AddOffset(0, vecEnd.Value, 0);
            builder.EndTable();
            Assert.ArrayEqual(new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0,       // Padding to 32 bytes
                6, 0, // vtable bytes
                8, 0, // object length inc vtable offset
                4, 0, // start of vector offset value 0
                6, 0, 0, 0, // int32 offset for start of vtable
                4, 0, 0, 0,
                0, 0, 0, 0,
            },
                builder._bb.ToFullArray());
            var verifier = new Verifier(builder._bb);
            uint checkOffset = 20;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            // First field must be vector with element size 1
            Assert.IsTrue(verifier.VerifyVectorOfData(checkOffset, 4, 1, true));
        }

        [FlatBuffersTestMethod]
        public void TestVTableWithEmptyVectorAndScalars()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartVector(sizeof(byte), 0, 1);
            var vecEnd = builder.EndVector();

            builder.StartTable(2);
            builder.AddShort(0, 55, 0);
            builder.AddOffset(1, vecEnd.Value, 0);
            builder.EndTable();
            Assert.ArrayEqual(new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0, // Padding to 32 bytes
                8, 0, // vtable bytes
                12, 0, // object length inc vtable offset
                10, 0,     // offset to int16 value 0
                4, 0, // start of vector offset value 1
                8, 0, 0, 0, // int32 offset for start of vtable
                8, 0, 0, 0, // value 1
                0, 0, 55, 0, // value 0
                0, 0, 0, 0, // length of vector (not in sctruc)
            },
                builder._bb.ToFullArray());
            var verifier = new Verifier(builder._bb);
            uint checkOffset = 16;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            // First field must be short
            Assert.IsTrue(verifier.VerifyField(checkOffset, 4, 2, 2, true));
            // Second field must be vector with element size 1
            Assert.IsTrue(verifier.VerifyVectorOfData(checkOffset, 6, 2, true));
       }


        [FlatBuffersTestMethod]
        public void TestVTableWith_1xInt16_and_Vector_or_2xInt16()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartVector(sizeof(short), 2, 1);
            builder.AddShort(0x1234);
            builder.AddShort(0x5678);
            var vecEnd = builder.EndVector();

            builder.StartTable(2);
            builder.AddOffset(1, vecEnd.Value, 0);
            builder.AddShort(0, 55, 0);
            builder.EndTable();
            Assert.ArrayEqual(new byte[]
            {
                0, 0, 0, 0, // Padding to 32 bytes
                8, 0, // vtable bytes
                12, 0, // object length
                6, 0,     // start of value 0 from end of vtable
                8, 0,     // start of value 1 from end of buffer
                8, 0, 0, 0, // int32 offset for start of vtable
                0, 0, 55, 0,    // padding + value 0
                4, 0, 0, 0, // position of vector from here
                2, 0, 0, 0, // length of vector
                0x78, 0x56,       // vector value 0
                0x34, 0x12,       // vector value 1
            },
                builder._bb.ToFullArray());
            var verifier = new Verifier(builder._bb);
            uint checkOffset = 12;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            // Second field must be vector with element size 2
            Assert.IsTrue(verifier.VerifyVectorOfData(checkOffset, 6, 2, true));
            // Check Error: Second field with too big size
            Assert.IsFalse(verifier.VerifyVectorOfData(checkOffset, 6, 4, true));
            // First field must be short
            Assert.IsTrue(verifier.VerifyField(checkOffset, 4, 2, 2, true));
        }

        [FlatBuffersTestMethod]
        public void TestVTableWithAStruct_of_int8_int16_int32()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(1);
            builder.Prep(4+4+4, 0);
            builder.AddSbyte(55);
            builder.Pad(3);
            builder.AddShort(0x1234);
            builder.Pad(2);
            builder.AddInt(0x12345678);
            var structStart = builder.Offset;
            builder.AddStruct(0, structStart, 0);
            builder.EndTable();
            Assert.ArrayEqual(new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, // Padding to 32 bytes
                6, 0, // vtable bytes
                16, 0, // object length
                4, 0,     // start of struct from here
                6, 0, 0, 0, // int32 offset for start of vtable
                0x78, 0x56, 0x34, 0x12,  // struct value 2
                0x00, 0x00, 0x34, 0x12, // struct value 1
                0x00, 0x00, 0x00, 55, // struct value 0
            },
                builder._bb.ToFullArray());
            var verifier = new Verifier(builder._bb);
            uint checkOffset = 16;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            // First field must be a struct with 12 bytes
            Assert.IsTrue(verifier.VerifyField(checkOffset, 4, 12, 4, true));
            // Check Error: First field with more than 12 bytes
            Assert.IsFalse(verifier.VerifyField(checkOffset, 4, 16, 4, true));
        }


        [FlatBuffersTestMethod]
        public void TestVTableWithAVectorOf_2xStructOf_2xInt8()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartVector(sizeof(byte)*2, 2, 1);
            builder.AddByte(33);
            builder.AddByte(44);
            builder.AddByte(55);
            builder.AddByte(66);
            var vecEnd = builder.EndVector();

            builder.StartTable(1);
            builder.AddOffset(0, vecEnd.Value, 0);
            builder.EndTable();

            Assert.ArrayEqual(new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, // Padding to 32 bytes
                6, 0, // vtable bytes
                8, 0, // object length
                4, 0,     // offset of vector offset
                6, 0, 0, 0, // int32 offset for start of vtable
                4, 0, 0, 0, // Vector start offset
                2, 0, 0, 0, // Vector len
                66, // vector 1, 1
                55, // vector 1, 0
                44, // vector 0, 1
                33, // vector 0, 0
            },
                builder._bb.ToFullArray());
            var verifier = new Verifier(builder._bb);
            uint checkOffset = 16;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            // First field must be vector with element size 2
            Assert.IsTrue(verifier.VerifyVectorOfData(checkOffset, 4, 2, true));
        }

        [FlatBuffersTestMethod]
        public void TestVTableWithSomeElements()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(2);
            builder.AddByte(0, 33, 0);
            builder.AddShort(1, 66, 0);
            var off = builder.EndTable();
            builder.Finish(off);

            byte[] padded = new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0, //Padding to 32 bytes
                12, 0, 0, 0,     // root of table, pointing to vtable offset
                8, 0, // vtable bytes
                8, 0, // object length
                7, 0, // start of value 0
                4, 0, // start of value 1
                8, 0, 0, 0, // int32 offset for start of vtable
                66, 0, // value 1
                0, 33, // value 0

            };
            Assert.ArrayEqual(padded, builder._bb.ToFullArray());

            // no padding in sized array
            byte[] unpadded = new byte[padded.Length - 12];
            Buffer.BlockCopy(padded, 12, unpadded, 0, unpadded.Length);
            Assert.ArrayEqual(unpadded, builder._bb.ToSizedArray());

            var verifier = new Verifier(builder._bb);
            uint checkOffset = builder._bb.GetUint(builder._bb.Position) + (uint)builder._bb.Position;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            // First field must be a struct with 12 bytes
            Assert.IsTrue(verifier.VerifyField(checkOffset, 4, 1, 1, true));
            // Second field must be a struct with 12 bytes
            Assert.IsTrue(verifier.VerifyField(checkOffset, 6, 2, 2, true));
        }

        [FlatBuffersTestMethod]
        public void TestVTableWithStrings()
        {
            var builder = new FlatBufferBuilder(64, Allocator.Temp);
            var str1 = builder.CreateString("foo");
            var str2 = builder.CreateString("foobar");
            builder.StartTable(2);
            builder.AddOffset(0, str1.Value, 0);
            builder.AddOffset(1, str2.Value, 0);
            var off = builder.EndTable();
            builder.Finish(off);

            byte[] padded = new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0, //Padding to 32 bytes
                12, 0, 0, 0, // root of table, pointing to vtable offset
                8, 0, // vtable bytes
                12, 0, // object length
                8, 0, // start of value 0
                4, 0, // start of value 1
                8, 0, 0, 0, // int32 offset for start of vtable
                8, 0, 0, 0, // pointer to string
                16, 0, 0, 0, // pointer to string
                6, 0, 0, 0, // length of string
                102, 111, 111, 98, 97, 114, 0, 0, // "foobar" + padding
                3, 0, 0, 0, // length of string
                102, 111, 111, 0 // "bar"
            };
            Assert.ArrayEqual(padded, builder._bb.ToFullArray());

            var verifier = new Verifier(builder._bb);
            uint checkOffset = builder._bb.GetUint(builder._bb.Position) + (uint)builder._bb.Position;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            // First field string check
            Assert.IsTrue(verifier.VerifyString(checkOffset, 4, true));
            // Second field string check
            Assert.IsTrue(verifier.VerifyString(checkOffset, 6, true));
        }

        [FlatBuffersTestMethod]
        public void TestVTableWithVectorOfStrings()
        {
            var builder = new FlatBufferBuilder(64, Allocator.Temp);
            var str1 = builder.CreateString("foo");
            var str2 = builder.CreateString("foobar");
            builder.StartVector(sizeof(int), 2, 1);
            builder.AddOffset(str1.Value);
            builder.AddOffset(str2.Value);
            var vec = builder.EndVector();
            builder.StartTable(1);
            builder.AddOffset(0, vec.Value, 0);
            var off = builder.EndTable();
            builder.Finish(off);

            byte[] padded = new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0, //Padding to 32 bytes
                12, 0, 0, 0, // root of table, pointing to vtable offset
                0, 0, // padding
                6, 0, // vtable bytes
                8, 0, // object length
                4, 0, // start of value 0
                6, 0, 0, 0, // int32 offset for start of vtable
                4, 0, 0, 0, // pointer to vector
                2, 0, 0, 0, // length of vector
                8, 0, 0, 0, // int32 offset to string 1
                16, 0, 0, 0, // int32 offset to string 2
                6, 0, 0, 0, // length of string
                102, 111, 111, 98, 97, 114, 0, 0, // "foobar" + padding
                3, 0, 0, 0, // length of string
                102, 111, 111, 0 // "bar"
            };
            Assert.ArrayEqual(padded, builder._bb.ToFullArray());

            var verifier = new Verifier(builder._bb);
            uint checkOffset = builder._bb.GetUint(builder._bb.Position) + (uint)builder._bb.Position;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            // First field string check
            Assert.IsTrue(verifier.VerifyVectorOfStrings(checkOffset, 4, true));
        }

        [FlatBuffersTestMethod]
        public void TestTwoFinishTable()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(2);
            builder.AddByte(0, 33, 0);
            builder.AddByte(1, 44, 0);
            var off0 = builder.EndTable();
            builder.Finish(off0);

            builder.StartTable(3);
            builder.AddByte(0, 55, 0);
            builder.AddByte(1, 66, 0);
            builder.AddByte(2, 77, 0);
            var off1 = builder.EndTable();
            builder.Finish(off1);

            Assert.ArrayEqual(new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,       // padding to 64 bytes
                16, 0, 0, 0,     // root of table, pointing to vtable offset (obj1)
                0, 0, // padding

                10, 0, // vtable bytes
                8, 0, // object length
                7, 0, // start of value 0
                6, 0, // start of value 1
                5, 0, // start of value 2
                10, 0, 0, 0, // int32 offset for start of vtable
                0, // pad
                77, // values 2, 1, 0
                66,
                55,

                12, 0, 0, 0,     // root of table, pointing to vtable offset (obj0)
                8, 0, // vtable bytes
                8, 0, // object length
                7, 0, // start of value 0
                6, 0, // start of value 1
                8, 0, 0, 0, // int32 offset for start of vtable
                0, 0, // pad
                44, // value 1, 0
                33,
            },
                builder._bb.ToFullArray());

            // check obj1
            var verifier = new Verifier(builder._bb);
            uint checkOffset = builder._bb.GetUint(builder._bb.Position) + (uint)builder._bb.Position;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            // First field must be a struct with 12 bytes
            Assert.IsTrue(verifier.VerifyField(checkOffset, 4, 1, 1, true));
            // Second field must be a struct with 12 bytes
            Assert.IsTrue(verifier.VerifyField(checkOffset, 6, 1, 1, true));
            // Third field must be a struct with 12 bytes
            Assert.IsTrue(verifier.VerifyField(checkOffset, 8, 1, 1, true));
            // Check Error: 4. field did not exist
            Assert.IsFalse(verifier.VerifyField(checkOffset, 10, 1, 1, true));

            // check obj0
            checkOffset = 56;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            // First field must be a struct with 12 bytes
            Assert.IsTrue(verifier.VerifyField(checkOffset, 4, 1, 1, true));
            // Second field must be a struct with 12 bytes
            Assert.IsTrue(verifier.VerifyField(checkOffset, 6, 1, 1, true));
            // Check Error: 3. field did not exist
            Assert.IsFalse(verifier.VerifyField(checkOffset, 8, 1, 1, true));
            // Check Error: 4. field did not exist
            Assert.IsFalse(verifier.VerifyField(checkOffset, 10, 1, 1, true));
        }

        [FlatBuffersTestMethod]
        public void TestBunchOfBools()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(8);
            for (var i = 0; i < 8; i++)
            {
                builder.AddBool(i, true, false);
            }
            var off = builder.EndTable();
            builder.Finish(off);

            byte[] padded = new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,       // padding to 64 bytes

                24, 0, 0, 0,     // root of table, pointing to vtable offset (obj0)
                20, 0, // vtable bytes
                12, 0, // object length
                11, 0, // start of value 0
                10, 0, // start of value 1
                9, 0, // start of value 2
                8, 0, // start of value 3
                7, 0, // start of value 4
                6, 0, // start of value 5
                5, 0, // start of value 6
                4, 0, // start of value 7

                20, 0, 0, 0, // int32 offset for start of vtable

                1, 1, 1, 1,  // values
                1, 1, 1, 1,

            };
            Assert.ArrayEqual(padded, builder._bb.ToFullArray());

            // no padding in sized array
            byte[] unpadded = new byte[padded.Length - 28];
            Buffer.BlockCopy(padded, 28, unpadded, 0, unpadded.Length);
            Assert.ArrayEqual(unpadded, builder._bb.ToSizedArray());

            var verifier = new Verifier(builder._bb);
            uint checkOffset = builder._bb.GetUint(builder._bb.Position) + (uint)builder._bb.Position;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            for (var i = 0; i < 8; i++)
            {
                Assert.IsTrue(verifier.VerifyField(checkOffset, (short)(4 + i * 2), 1, 1, true));
            }
            Assert.IsFalse(verifier.VerifyField(checkOffset, (short)(4 + 8 * 2), 1, 1, true));
        }

        [FlatBuffersTestMethod]
        public void TestBunchOfBoolsSizePrefixed()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(8);
            for (var i = 0; i < 8; i++)
            {
                builder.AddBool(i, true, false);
            }
            var off = builder.EndTable();
            builder.FinishSizePrefixed(off);

            byte[] padded = new byte[]
            {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0,      // padding to 64 bytes

                36, 0, 0, 0,     // size prefix
                24, 0, 0, 0,     // root of table, pointing to vtable offset (obj0)
                20, 0, // vtable bytes
                12, 0, // object length
                11, 0, // start of value 0
                10, 0, // start of value 1
                9, 0, // start of value 2
                8, 0, // start of value 3
                7, 0, // start of value 4
                6, 0, // start of value 5
                5, 0, // start of value 6
                4, 0, // start of value 7

                20, 0, 0, 0, // int32 offset for start of vtable

                1, 1, 1, 1,  // values
                1, 1, 1, 1,

            };
            Assert.ArrayEqual(padded, builder._bb.ToFullArray());

            // no padding in sized array
            byte[] unpadded = new byte[padded.Length - 24];
            Buffer.BlockCopy(padded, 24, unpadded, 0, unpadded.Length);
            Assert.ArrayEqual(unpadded, builder._bb.ToSizedArray());
        }

        [FlatBuffersTestMethod]
        public void TestWithFloat()
        {
            var builder = new FlatBufferBuilder(1, Allocator.Temp);
            builder.StartTable(1);
            builder.AddFloat(0, 1, 0);
            builder.EndTable();


            Assert.ArrayEqual(new byte[]
            {
                0, 0,
                6, 0, // vtable bytes
                8, 0, // object length
                4, 0, // start of value 0
                6, 0, 0, 0, // int32 offset for start of vtable
                0, 0, 128, 63,  // value

            },
                builder._bb.ToFullArray());
            var verifier = new Verifier(builder._bb);
            uint checkOffset = 8;
            // table must be ok
            Assert.IsTrue(verifier.VerifyTableStart(checkOffset));
            // First Field must be float
            Assert.IsTrue(verifier.VerifyField(checkOffset, 4, 4, 4, true));
            // Check Error: First Field with to big size
            Assert.IsFalse(verifier.VerifyField(checkOffset, 4, 8, 4, true));
            // Check Error: First Field with to big padding
            Assert.IsFalse(verifier.VerifyField(checkOffset, 4, 4, 8, true));
        }

        private void CheckObjects(int fieldCount, int objectCount)
        {
            _lcg.Reset();

            const int testValuesMax = 11;

            var builder = new FlatBufferBuilder(1, Allocator.Temp);

            var objects = new int[objectCount];

            for (var i = 0; i < objectCount; ++i)
            {
                builder.StartTable(fieldCount);

                for (var j = 0; j < fieldCount; ++j)
                {
                    var fieldType = _lcg.Next()%testValuesMax;

                    switch (fieldType)
                    {
                        case 0:
                        {
                            builder.AddBool(j, FuzzTestData.BoolValue, false);
                            break;
                        }
                        case 1:
                        {
                            builder.AddSbyte(j, FuzzTestData.Int8Value, 0);
                            break;
                        }
                        case 2:
                        {
                            builder.AddByte(j, FuzzTestData.UInt8Value, 0);
                            break;
                        }
                        case 3:
                        {
                            builder.AddShort(j, FuzzTestData.Int16Value, 0);
                            break;
                        }
                        case 4:
                        {
                            builder.AddUshort(j, FuzzTestData.UInt16Value, 0);
                            break;
                        }
                        case 5:
                        {
                            builder.AddInt(j, FuzzTestData.Int32Value, 0);
                            break;
                        }
                        case 6:
                        {
                            builder.AddUint(j, FuzzTestData.UInt32Value, 0);
                            break;
                        }
                        case 7:
                        {
                            builder.AddLong(j, FuzzTestData.Int64Value, 0);
                            break;
                        }
                        case 8:
                        {
                            builder.AddUlong(j, FuzzTestData.UInt64Value, 0);
                            break;
                        }
                        case 9:
                        {
                            builder.AddFloat(j, FuzzTestData.Float32Value, 0);
                            break;
                        }
                        case 10:
                        {
                            builder.AddDouble(j, FuzzTestData.Float64Value, 0);
                            break;
                        }
                        default:
                            throw new Exception("Unreachable");
                    }

                }

                var offset = builder.EndTable();

                // Store the object offset
                objects[i] = offset;
            }

            _lcg.Reset();

            // Test all objects are readable and return expected values...
            for (var i = 0; i < objectCount; ++i)
            {
                var table = new TestTable(ref builder._bb, builder._bb.Length - objects[i]);

                for (var j = 0; j < fieldCount; ++j)
                {
                    var fieldType = _lcg.Next() % testValuesMax;
                    var fc = 2 + j; // 2 == VtableMetadataFields
                    var f = fc * 2;

                    switch (fieldType)
                    {
                        case 0:
                        {
                            Assert.AreEqual(FuzzTestData.BoolValue, table.GetSlot(f, false));
                            break;
                        }
                        case 1:
                        {
                            Assert.AreEqual(FuzzTestData.Int8Value, table.GetSlot(f, (sbyte)0));
                            break;
                        }
                        case 2:
                        {
                            Assert.AreEqual(FuzzTestData.UInt8Value, table.GetSlot(f, (byte)0));
                            break;
                        }
                        case 3:
                        {
                            Assert.AreEqual(FuzzTestData.Int16Value, table.GetSlot(f, (short)0));
                            break;
                        }
                        case 4:
                        {
                            Assert.AreEqual(FuzzTestData.UInt16Value, table.GetSlot(f, (ushort)0));
                            break;
                        }
                        case 5:
                        {
                            Assert.AreEqual(FuzzTestData.Int32Value, table.GetSlot(f, (int)0));
                            break;
                        }
                        case 6:
                        {
                            Assert.AreEqual(FuzzTestData.UInt32Value, table.GetSlot(f, (uint)0));
                            break;
                        }
                        case 7:
                        {
                            Assert.AreEqual(FuzzTestData.Int64Value, table.GetSlot(f, (long)0));
                            break;
                        }
                        case 8:
                        {
                            Assert.AreEqual(FuzzTestData.UInt64Value, table.GetSlot(f, (ulong)0));
                            break;
                        }
                        case 9:
                        {
                            Assert.AreEqual(FuzzTestData.Float32Value, table.GetSlot(f, (float)0));
                            break;
                        }
                        case 10:
                        {
                            Assert.AreEqual(FuzzTestData.Float64Value, table.GetSlot(f, (double)0));
                            break;
                        }
                        default:
                            throw new Exception("Unreachable");
                    }

                }

            }

        }
    }
}
