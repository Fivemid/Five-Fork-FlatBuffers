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
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using MyGame.Example;
using optional_scalars;
using KeywordTest;
using Unity.Collections;

namespace Fivemid.FiveFlat.Tests.Exported
{
    [FlatBuffersTestClass]
    public class FlatBuffersExampleTests
    {
        public void RunTests()
        {
            CanCreateNewFlatBufferFromScratch();
            CanReadCppGeneratedWireFile();
            TestEnums();
        }

        [FlatBuffersTestMethod]
        public void CanCreateNewFlatBufferFromScratch()
        {
            CanCreateNewFlatBufferFromScratch(true);
            CanCreateNewFlatBufferFromScratch(false);
        }

        private void CanCreateNewFlatBufferFromScratch(bool sizePrefix)
        {
            // Second, let's create a FlatBuffer from scratch in C#, and test it also.
            // We use an initial size of 1 to exercise the reallocation algorithm,
            // normally a size larger than the typical FlatBuffer you generate would be
            // better for performance.
            var fbb = new FlatBufferBuilder(1, Allocator.Temp);

            StringOffset[] names = { fbb.CreateString("Frodo"), fbb.CreateString("Barney"), fbb.CreateString("Wilma") };
            Offset<Monster>[] off = new Offset<Monster>[3];
            Monster.StartMonster(ref fbb);
            Monster.AddName(ref fbb, names[0]);
            off[0] = Monster.EndMonster(ref fbb);
            Monster.StartMonster(ref fbb);
            Monster.AddName(ref fbb, names[1]);
            off[1] = Monster.EndMonster(ref fbb);
            Monster.StartMonster(ref fbb);
            Monster.AddName(ref fbb, names[2]);
            off[2] = Monster.EndMonster(ref fbb);
            Array.Sort(off, (o1, o2) => Monster.CompareMonster(ref fbb, o1, o2));
            var sortMons = fbb.CreateVectorOfTables(off.AsSpan());

            // We set up the same values as monsterdata.json:

            var str = fbb.CreateString("MyMonster");
            var test1 = fbb.CreateString("test1");
            var test2 = fbb.CreateString("test2");


            Monster.StartInventoryVector(ref fbb, 5);
            for (int i = 4; i >= 0; i--)
            {
                fbb.AddByte((byte)i);
            }
            var inv = fbb.EndVector();

            var fred = fbb.CreateString("Fred");
            Monster.StartMonster(ref fbb);
            Monster.AddName(ref fbb, fred);
            var mon2 = Monster.EndMonster(ref fbb);

            Monster.StartTest4Vector(ref fbb, 2);
            MyGame.Example.Test.CreateTest(ref fbb, (short)10, (sbyte)20);
            MyGame.Example.Test.CreateTest(ref fbb, (short)30, (sbyte)40);
            var test4 = fbb.EndVector();

            Monster.StartTestarrayofstringVector(ref fbb, 2);
            fbb.AddOffset(test2.Value);
            fbb.AddOffset(test1.Value);
            var testArrayOfString = fbb.EndVector();

            Monster.StartMonster(ref fbb);
            Monster.AddPos(ref fbb, Vec3.CreateVec3(ref fbb, 1.0f, 2.0f, 3.0f, 3.0,
                                                     Color.Green, (short)5, (sbyte)6));
            Monster.AddHp(ref fbb, (short)80);
            Monster.AddName(ref fbb, str);
            Monster.AddInventory(ref fbb, inv);
            Monster.AddTestType(ref fbb, Any.Monster);
            Monster.AddTest(ref fbb, mon2.Value);
            Monster.AddTest4(ref fbb, test4);
            Monster.AddTestarrayofstring(ref fbb, testArrayOfString);
            Monster.AddTestbool(ref fbb, true);
            Monster.AddTestarrayoftables(ref fbb, sortMons);
            var mon = Monster.EndMonster(ref fbb);

            if (sizePrefix)
            {
                Monster.FinishSizePrefixedMonsterBuffer(ref fbb, mon);
            }
            else
            {
                Monster.FinishMonsterBuffer(ref fbb, mon);
            }

            // Dump to output directory so we can inspect later, if needed
            var data = fbb._bb.ToSizedArray();
            string filename = @"monsterdata_cstest" + (sizePrefix ? "_sp" : "") + ".mon";
            File.WriteAllBytes(filename, data.ToArray());

            // Remove the size prefix if necessary for further testing
            ref ByteBuffer dataBuffer = ref fbb._bb;
            if (sizePrefix)
            {
                Assert.AreEqual(ByteBufferUtil.GetSizePrefix(ref dataBuffer) + FlatBufferConstants.SizePrefixLength,
                                dataBuffer.Length - dataBuffer.Position);
                dataBuffer = ByteBufferUtil.RemoveSizePrefix(ref dataBuffer);
            }

            // Now assert the buffer
            TestBuffer(ref dataBuffer);

            //Attempt to mutate Monster fields and check whether the buffer has been mutated properly
            // revert to original values after testing
            Monster monster = Monster.GetRootAsMonster(ref dataBuffer);


            // mana is optional and does not exist in the buffer so the mutation should fail
            // the mana field should retain its default value
            Assert.AreEqual(monster.MutateMana((short)10), false);
            Assert.AreEqual(monster.Mana, (short)150);

            // Accessing a vector of sorted by the key tables
            Assert.StringEqual(monster.Testarrayoftables(0).Value.Name, "Barney");
            Assert.StringEqual(monster.Testarrayoftables(1).Value.Name, "Frodo");
            Assert.StringEqual(monster.Testarrayoftables(2).Value.Name, "Wilma");

            // Example of searching for a table by the key
            Assert.IsTrue(monster.TestarrayoftablesByKey(FlatBufferBuilder.EncodeString("Frodo", Allocator.Temp)) != null);
            Assert.StringEqual(monster.TestarrayoftablesByKey(FlatBufferBuilder.EncodeString("Frodo", Allocator.Temp)).Value.Name, "Frodo");
            Assert.IsTrue(monster.TestarrayoftablesByKey(FlatBufferBuilder.EncodeString("Barney", Allocator.Temp)) != null);
            Assert.StringEqual(monster.TestarrayoftablesByKey(FlatBufferBuilder.EncodeString("Barney", Allocator.Temp)).Value.Name, "Barney");
            Assert.IsTrue(monster.TestarrayoftablesByKey(FlatBufferBuilder.EncodeString("Wilma", Allocator.Temp)) != null);
            Assert.StringEqual(monster.TestarrayoftablesByKey(FlatBufferBuilder.EncodeString("Wilma", Allocator.Temp)).Value.Name, "Wilma");

            // testType is an existing field
            Assert.AreEqual(monster.TestType, Any.Monster);

            //mutate the inventory vector
            Assert.AreEqual(monster.MutateInventory(0, 1), true);
            Assert.AreEqual(monster.MutateInventory(1, 2), true);
            Assert.AreEqual(monster.MutateInventory(2, 3), true);
            Assert.AreEqual(monster.MutateInventory(3, 4), true);
            Assert.AreEqual(monster.MutateInventory(4, 5), true);

            for (int i = 0; i < monster.InventoryLength; i++)
            {
                Assert.AreEqual(monster.Inventory(i), i + 1);
            }

            //reverse mutation
            Assert.AreEqual(monster.MutateInventory(0, 0), true);
            Assert.AreEqual(monster.MutateInventory(1, 1), true);
            Assert.AreEqual(monster.MutateInventory(2, 2), true);
            Assert.AreEqual(monster.MutateInventory(3, 3), true);
            Assert.AreEqual(monster.MutateInventory(4, 4), true);

            // get a struct field and edit one of its fields
            Vec3 pos = (Vec3)monster.Pos;
            Assert.AreEqual(pos.X, 1.0f);
            pos.MutateX(55.0f);
            Assert.AreEqual(pos.X, 55.0f);
            pos.MutateX(1.0f);
            Assert.AreEqual(pos.X, 1.0f);

            TestBuffer(ref dataBuffer);
            TestObjectAPI(Monster.GetRootAsMonster(ref dataBuffer));
        }

        private void TestBuffer(ref ByteBuffer bb)
        {
            bool test = Monster.VerifyMonster(bb);
            Assert.AreEqual(true, test);

            Monster monster = Monster.GetRootAsMonster(ref bb);

            Assert.AreEqual(80, monster.Hp);
            Assert.AreEqual(150, monster.Mana);
            Assert.StringEqual("MyMonster", monster.Name);

            var pos = monster.Pos.Value;
            Assert.AreEqual(1.0f, pos.X);
            Assert.AreEqual(2.0f, pos.Y);
            Assert.AreEqual(3.0f, pos.Z);

            Assert.AreEqual(3.0f, pos.Test1);
            Assert.AreEqual(Color.Green, pos.Test2);
            var t = (MyGame.Example.Test)pos.Test3;
            Assert.AreEqual((short)5, t.A);
            Assert.AreEqual((sbyte)6, t.B);

            Assert.AreEqual(Any.Monster, monster.TestType);

            var monster2 = monster.Test<Monster>().Value;
            Assert.StringEqual("Fred", monster2.Name);


            Assert.AreEqual(5, monster.InventoryLength);
            var invsum = 0;
            for (var i = 0; i < monster.InventoryLength; i++)
            {
                invsum += monster.Inventory(i);
            }
            Assert.AreEqual(10, invsum);

            var test0 = monster.Test4(0).Value;
            var test1 = monster.Test4(1).Value;
            Assert.AreEqual(2, monster.Test4Length);

            Assert.AreEqual(100, test0.A + test0.B + test1.A + test1.B);

            Assert.AreEqual(2, monster.TestarrayofstringLength);
            Assert.StringEqual("test1", monster.Testarrayofstring(0));
            Assert.StringEqual("test2", monster.Testarrayofstring(1));

            Assert.AreEqual(true, monster.Testbool);
        }

        [FlatBuffersTestMethod]
        public void CanReadCppGeneratedWireFile()
        {
            using var fromAssembly = Assembly.GetAssembly(typeof(FlatBuffersExampleTests))
                                           .GetManifestResourceStream("Fivemid.FiveFlat.Tests.Exported.Resources.monsterdata_test.mon")!;
            var data = ReadAllBytes(fromAssembly);
            var bb   = new ByteBuffer(new NativeArray<byte>(data, Allocator.Temp));
            TestBuffer(ref bb);
            TestObjectAPI(Monster.GetRootAsMonster(ref bb));

            static byte[] ReadAllBytes(Stream input) {
              using var memoryStream = new MemoryStream();
              input.CopyTo(memoryStream);
              return memoryStream.ToArray();
            }
        }

        [FlatBuffersTestMethod]
        public void CanReadJsonFile()
        {
            using var fromAssembly = Assembly.GetAssembly(typeof(FlatBuffersExampleTests))
                                           .GetManifestResourceStream("Fivemid.FiveFlat.Tests.Exported.Resources.monsterdata_test.json")!;
            var jsonText = ReadAllText(fromAssembly);
            var mon = MonsterT.DeserializeFromJson(jsonText);
            var fbb = new FlatBufferBuilder(1, Allocator.Temp);
            Monster.FinishMonsterBuffer(ref fbb, Monster.Pack(ref fbb, mon));
            TestBuffer(ref fbb._bb);

            static string ReadAllText(Stream input) {
              using var reader = new StreamReader(input);
              return reader.ReadToEnd();
            }
        }

        [FlatBuffersTestMethod]
        public void TestEnums()
        {
            Assert.AreEqual("Red", Color.Red.ToString());
            Assert.AreEqual("Blue", Color.Blue.ToString());
            Assert.AreEqual("NONE", Any.NONE.ToString());
            Assert.AreEqual("Monster", Any.Monster.ToString());
        }

        [FlatBuffersTestMethod]
        public void TestVectorOfEnums()
        {
            const string monsterName = "TestVectorOfEnumsMonster";
            var colorVec = new Color[] { Color.Red, Color.Green, Color.Blue };
            var fbb = new FlatBufferBuilder(32, Allocator.Temp);
            var str1 = fbb.CreateString(monsterName);
            var vec1 = Monster.CreateVectorOfEnumsVector(ref fbb, colorVec);
            Monster.StartMonster(ref fbb);
            Monster.AddName(ref fbb, str1);
            Monster.AddVectorOfEnums(ref fbb, vec1);
            var monster1 = Monster.EndMonster(ref fbb);
            Monster.FinishMonsterBuffer(ref fbb, monster1);

            var mons = Monster.GetRootAsMonster(ref fbb._bb);
            // var colors = mons.GetVectorOfEnumsArray();
            // Assert.ArrayEqual(colorVec, colors);

            TestObjectAPI(mons);
        }

        [FlatBuffersTestMethod]
        public void TestNestedFlatBuffer()
        {
            const string nestedMonsterName = "NestedMonsterName";
            const short nestedMonsterHp = 600;
            const short nestedMonsterMana = 1024;
            // Create nested buffer as a Monster type
            var fbb1 = new FlatBufferBuilder(16, Allocator.Temp);
            var str1 = fbb1.CreateString(nestedMonsterName);
            Monster.StartMonster(ref fbb1);
            Monster.AddName(ref fbb1, str1);
            Monster.AddHp(ref fbb1, nestedMonsterHp);
            Monster.AddMana(ref fbb1, nestedMonsterMana);
            var monster1 = Monster.EndMonster(ref fbb1);
            Monster.FinishMonsterBuffer(ref fbb1, monster1);
            var fbb1Bytes = fbb1.SizedByteArray();
            fbb1 = default;

            // Create a Monster which has the first buffer as a nested buffer
            var fbb2 = new FlatBufferBuilder(16, Allocator.Temp);
            var str2 = fbb2.CreateString("My Monster");
            var nestedBuffer = Monster.CreateTestnestedflatbufferVector(ref fbb2, fbb1Bytes);
            Monster.StartMonster(ref fbb2);
            Monster.AddName(ref fbb2, str2);
            Monster.AddHp(ref fbb2, 50);
            Monster.AddMana(ref fbb2, 32);
            Monster.AddTestnestedflatbuffer(ref fbb2, nestedBuffer);
            var monster = Monster.EndMonster(ref fbb2);
            Monster.FinishMonsterBuffer(ref fbb2, monster);

            // Now test the data extracted from the nested buffer
            var mons = Monster.GetRootAsMonster(ref fbb2._bb);
            var nestedMonster = mons.GetTestnestedflatbufferAsMonster().Value;

            Assert.AreEqual(nestedMonsterMana, nestedMonster.Mana);
            Assert.AreEqual(nestedMonsterHp, nestedMonster.Hp);
            Assert.StringEqual(nestedMonsterName, nestedMonster.Name);

            TestObjectAPI(mons);
            TestObjectAPI(nestedMonster);
        }

        [FlatBuffersTestMethod]
        public void TestFixedLenghtArrays()
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(100, Allocator.Temp);

            float   a;
            int[]   b = new int[15];
            sbyte   c;
            int[,]  d_a = new int[2, 2];
            TestEnum[]  d_b = new TestEnum[2];
            TestEnum[,] d_c = new TestEnum[2, 2];
            long[,]     d_d = new long[2, 2];
            int         e;
            long[]      f = new long[2];

            a = 0.5f;
            for (int i = 0; i < 15; i++) b[i] = i;
            c = 1;
            d_a[0, 0] = 1;
            d_a[0, 1] = 2;
            d_a[1, 0] = 3;
            d_a[1, 1] = 4;
            d_b[0] = TestEnum.B;
            d_b[1] = TestEnum.C;
            d_c[0, 0] = TestEnum.A;
            d_c[0, 1] = TestEnum.B;
            d_c[1, 0] = TestEnum.C;
            d_c[1, 1] = TestEnum.B;
            d_d[0, 0] = -1;
            d_d[0, 1] = 1;
            d_d[1, 0] = -2;
            d_d[1, 1] = 2;
            e = 2;
            f[0] = -1;
            f[1] = 1;

            Offset<ArrayStruct> arrayOffset = ArrayStruct.CreateArrayStruct(
                ref builder, a, b, c, d_a, d_b, d_c, d_d, e, f);

            // Create a table with the ArrayStruct.
            ArrayTable.StartArrayTable(ref builder);
            ArrayTable.AddA(ref builder, arrayOffset);
            Offset<ArrayTable> tableOffset = ArrayTable.EndArrayTable(ref builder);

            ArrayTable.FinishArrayTableBuffer(ref builder, tableOffset);

            ArrayTable table = ArrayTable.GetRootAsArrayTable(ref builder._bb);

            Assert.AreEqual(table.A.Value.A, 0.5f);
            for (int i = 0; i < 15; i++) Assert.AreEqual(table.A.Value.B(i), i);
            Assert.AreEqual(table.A.Value.C, (sbyte)1);
            Assert.AreEqual(table.A.Value.D(0).A(0), 1);
            Assert.AreEqual(table.A.Value.D(0).A(1), 2);
            Assert.AreEqual(table.A.Value.D(1).A(0), 3);
            Assert.AreEqual(table.A.Value.D(1).A(1), 4);
            Assert.AreEqual(table.A.Value.D(0).B, TestEnum.B);
            Assert.AreEqual(table.A.Value.D(1).B, TestEnum.C);
            Assert.AreEqual(table.A.Value.D(0).C(0), TestEnum.A);
            Assert.AreEqual(table.A.Value.D(0).C(1), TestEnum.B);
            Assert.AreEqual(table.A.Value.D(1).C(0), TestEnum.C);
            Assert.AreEqual(table.A.Value.D(1).C(1), TestEnum.B);
            Assert.AreEqual(table.A.Value.D(0).D(0), -1);
            Assert.AreEqual(table.A.Value.D(0).D(1), 1);
            Assert.AreEqual(table.A.Value.D(1).D(0), -2);
            Assert.AreEqual(table.A.Value.D(1).D(1), 2);
            Assert.AreEqual(table.A.Value.E, 2);
            Assert.AreEqual(table.A.Value.F(0), -1);
            Assert.AreEqual(table.A.Value.F(1), 1);

            TestObjectAPI(table);
        }

        [FlatBuffersTestMethod]
        public void TestUnionVector()
        {
            var fbb = new FlatBufferBuilder(100, Allocator.Temp);
            var rapunzel = Rapunzel.CreateRapunzel(ref fbb, 40).Value;

            var characterTypes = new[]
            {
                Character.MuLan,
                Character.Belle,
                Character.Other,
            };
            var characterTypesOffset = Movie.CreateCharactersTypeVector(ref fbb, characterTypes);

            var characters = new[]
            {
                Attacker.CreateAttacker(ref fbb, 10).Value,
                BookReader.CreateBookReader(ref fbb, 20).Value,
                fbb.CreateString("Chip").Value,
            };
            var charactersOffset = Movie.CreateCharactersVector(ref fbb, characters);

            var movieOffset = Movie.CreateMovie(
                ref fbb,
                Character.Rapunzel,
                rapunzel,
                characterTypesOffset,
                charactersOffset);
            Movie.FinishMovieBuffer(ref fbb, movieOffset);

            var movie = Movie.GetRootAsMovie(ref fbb._bb);
            Assert.AreEqual(Character.Rapunzel, movie.MainCharacterType);
            Assert.AreEqual(40, movie.MainCharacter<Rapunzel>().Value.HairLength);

            Assert.AreEqual(3, movie.CharactersLength);
            Assert.AreEqual(Character.MuLan, movie.CharactersType(0));
            Assert.AreEqual(10, movie.Characters<Attacker>(0).Value.SwordAttackDamage);
            Assert.AreEqual(Character.Belle, movie.CharactersType(1));
            Assert.AreEqual(20, movie.Characters<BookReader>(1).Value.BooksRead);
            Assert.AreEqual(Character.Other, movie.CharactersType(2));
            Assert.StringEqual("Chip", movie.CharactersAsString(2));

            TestObjectAPI(movie);
        }

        [FlatBuffersTestMethod]
        public void TestUnionUtility()
        {
            var movie = new MovieT
            {
                MainCharacter = CharacterUnion.FromRapunzel(new RapunzelT { HairLength = 40 }),
                Characters = new System.Collections.Generic.List<CharacterUnion>
                {
                    CharacterUnion.FromMuLan(new AttackerT { SwordAttackDamage = 10 }),
                    CharacterUnion.FromBelle(new BookReaderT { BooksRead = 20 }),
                    CharacterUnion.FromOther("Chip"),
                },
            };

            var fbb = new FlatBufferBuilder(100, Allocator.Temp);
            Movie.FinishMovieBuffer(ref fbb, Movie.Pack(ref fbb, movie));

            TestObjectAPI(Movie.GetRootAsMovie(ref fbb._bb));
        }

        private void AreEqual(Monster a, MonsterT b)
        {
            Assert.AreEqual(a.Hp, b.Hp);
            Assert.AreEqual(a.Mana, b.Mana);
            Assert.StringEqual(a.Name, b.Name);

            var posA = a.Pos;
            var posB = b.Pos;
            if (posA != null)
            {
                Assert.AreEqual(posA.Value.X, posB.X);
                Assert.AreEqual(posA.Value.Y, posB.Y);
                Assert.AreEqual(posA.Value.Z, posB.Z);

                Assert.AreEqual(posA.Value.Test1, posB.Test1);
                Assert.AreEqual(posA.Value.Test2, posB.Test2);
                var tA = posA.Value.Test3;
                var tB = posB.Test3;
                Assert.AreEqual(tA.A, tB.A);
                Assert.AreEqual(tA.B, tB.B);
            }

            Assert.AreEqual(a.TestType, b.Test.Type);
            if (a.TestType == Any.Monster)
            {
                var monster2A = a.Test<Monster>().Value;
                var monster2B = b.Test.AsMonster();
                Assert.StringEqual(monster2A.Name, monster2B.Name);
            }

            Assert.AreEqual(a.InventoryLength, b.Inventory.Count);
            for (var i = 0; i < a.InventoryLength; ++i)
            {
                Assert.AreEqual(a.Inventory(i), b.Inventory[i]);
            }

            Assert.AreEqual(a.Test4Length, b.Test4.Count);
            for (var i = 0; i < a.Test4Length; ++i)
            {
                var t4A = a.Test4(i);
                var t4B = b.Test4[i];
                Assert.AreEqual(t4A.Value.A, t4B.A);
                Assert.AreEqual(t4A.Value.B, t4B.B);
            }

            Assert.AreEqual(a.TestarrayofstringLength, b.Testarrayofstring.Count);
            for (var i = 0; i < a.TestarrayofstringLength; ++i)
            {
                Assert.StringEqual(a.Testarrayofstring(i), b.Testarrayofstring[i]);
            }

            Assert.AreEqual(a.Testbool, b.Testbool);

            Assert.AreEqual(a.TestarrayofboolsLength, b.Testarrayofbools.Count);
            for (var i = 0; i < a.TestarrayofboolsLength; ++i)
            {
                Assert.AreEqual(a.Testarrayofbools(i), b.Testarrayofbools[i]);
            }

            Assert.AreEqual(a.VectorOfLongsLength, b.VectorOfLongs.Count);
            for (var i = 0; i < a.VectorOfLongsLength; ++i)
            {
                Assert.AreEqual(a.VectorOfLongs(i), b.VectorOfLongs[i]);
            }

            Assert.AreEqual(a.VectorOfDoublesLength, b.VectorOfDoubles.Count);
            for (var i = 0; i < a.VectorOfDoublesLength; ++i)
            {
                Assert.AreEqual(a.VectorOfDoubles(i), b.VectorOfDoubles[i]);
            }

            Assert.AreEqual(a.VectorOfEnumsLength, b.VectorOfEnums.Count);
            for (var i = 0; i < a.VectorOfEnumsLength; ++i)
            {
                Assert.AreEqual(a.VectorOfEnums(i), b.VectorOfEnums[i]);
            }
        }

        private void AreEqual(Monster a, Monster b)
        {
            Assert.AreEqual(a.Hp, b.Hp);
            Assert.AreEqual(a.Mana, b.Mana);
            Assert.StringEqual(a.Name, b.Name);

            var posA = a.Pos;
            var posB = b.Pos;
            if (posA != null)
            {
                Assert.AreEqual(posA.Value.X, posB.Value.X);
                Assert.AreEqual(posA.Value.Y, posB.Value.Y);
                Assert.AreEqual(posA.Value.Z, posB.Value.Z);

                Assert.AreEqual(posA.Value.Test1, posB.Value.Test1);
                Assert.AreEqual(posA.Value.Test2, posB.Value.Test2);
                var tA = posA.Value.Test3;
                var tB = posB.Value.Test3;
                Assert.AreEqual(tA.A, tB.A);
                Assert.AreEqual(tA.B, tB.B);
            }

            Assert.AreEqual(a.TestType, b.TestType);
            if (a.TestType == Any.Monster)
            {
                var monster2A = a.Test<Monster>().Value;
                var monster2B = b.Test<Monster>().Value;
                Assert.StringEqual(monster2A.Name, monster2B.Name);
            }

            Assert.AreEqual(a.InventoryLength, b.InventoryLength);
            for (var i = 0; i < a.InventoryLength; ++i)
            {
                Assert.AreEqual(a.Inventory(i), b.Inventory(i));
            }

            Assert.AreEqual(a.Test4Length, b.Test4Length);
            for (var i = 0; i < a.Test4Length; ++i)
            {
                var t4A = a.Test4(i);
                var t4B = b.Test4(i);
                Assert.AreEqual(t4A.Value.A, t4B.Value.A);
                Assert.AreEqual(t4A.Value.B, t4B.Value.B);
            }

            Assert.AreEqual(a.TestarrayofstringLength, b.TestarrayofstringLength);
            for (var i = 0; i < a.TestarrayofstringLength; ++i)
            {
                Assert.StringEqual(a.Testarrayofstring(i), b.Testarrayofstring(i));
            }

            Assert.AreEqual(a.Testbool, b.Testbool);

            Assert.AreEqual(a.TestarrayofboolsLength, b.TestarrayofboolsLength);
            for (var i = 0; i < a.TestarrayofboolsLength; ++i)
            {
                Assert.AreEqual(a.Testarrayofbools(i), b.Testarrayofbools(i));
            }

            Assert.AreEqual(a.VectorOfLongsLength, b.VectorOfLongsLength);
            for (var i = 0; i < a.VectorOfLongsLength; ++i)
            {
                Assert.AreEqual(a.VectorOfLongs(i), b.VectorOfLongs(i));
            }

            Assert.AreEqual(a.VectorOfDoublesLength, b.VectorOfDoublesLength);
            for (var i = 0; i < a.VectorOfDoublesLength; ++i)
            {
                Assert.AreEqual(a.VectorOfDoubles(i), b.VectorOfDoubles(i));
            }

            Assert.AreEqual(a.VectorOfEnumsLength, b.VectorOfEnumsLength);
            for (var i = 0; i < a.VectorOfEnumsLength; ++i)
            {
                Assert.AreEqual(a.VectorOfEnums(i), b.VectorOfEnums(i));
            }
        }

        private void TestObjectAPI(Monster a)
        {
            var b = a.UnPack();
            AreEqual(a, b);

            var fbb = new FlatBufferBuilder(1, Allocator.Temp);
            fbb.Finish(Monster.Pack(ref fbb, b).Value);
            var c = Monster.GetRootAsMonster(ref fbb._bb);
            AreEqual(a, c);

            var jsonText = b.SerializeToJson();
            var d = MonsterT.DeserializeFromJson(jsonText);
            AreEqual(a, d);

            var fbBuffer = b.SerializeToBinary(Allocator.Temp);
            Assert.IsTrue(Monster.MonsterBufferHasIdentifier(new ByteBuffer(fbBuffer)));
            var e = MonsterT.DeserializeFromBinary(fbBuffer);
            AreEqual(a, e);
        }

        private void AreEqual(ArrayTable a, ArrayTableT b)
        {
            Assert.AreEqual(a.A.Value.A, b.A.A);

            for (int i = 0; i < 15; ++i)
            {
                Assert.AreEqual(a.A.Value.B(i), b.A.B[i]);
            }

            Assert.AreEqual(a.A.Value.C, b.A.C);

            for (int i = 0; i < 2; ++i)
            {
                var ad = a.A.Value.D(i);
                var bd = b.A.D[i];

                for (int j = 0; j < 2; ++j)
                {
                    Assert.AreEqual(ad.A(j), bd.A[j]);
                }

                Assert.AreEqual(ad.B, bd.B);

                for (int j = 0; j < 2; ++j)
                {
                    Assert.AreEqual(ad.C(j), bd.C[j]);
                }

                for (int j = 0; j < 2; ++j)
                {
                    Assert.AreEqual(ad.D(j), bd.D[j]);
                }
            }

            Assert.AreEqual(a.A.Value.E, b.A.E);

            for (int i = 0; i < 2; ++i)
            {
                Assert.AreEqual(a.A.Value.F(i), b.A.F[i]);
            }
        }

        private void AreEqual(ArrayTable a, ArrayTable b)
        {
            Assert.AreEqual(a.A.Value.A, b.A.Value.A);

            for (int i = 0; i < 15; ++i)
            {
                Assert.AreEqual(a.A.Value.B(i), b.A.Value.B(i));
            }

            Assert.AreEqual(a.A.Value.C, b.A.Value.C);

            for (int i = 0; i < 2; ++i)
            {
                var ad = a.A.Value.D(i);
                var bd = b.A.Value.D(i);

                for (int j = 0; j < 2; ++j)
                {
                    Assert.AreEqual(ad.A(j), bd.A(j));
                }

                Assert.AreEqual(ad.B, bd.B);

                for (int j = 0; j < 2; ++j)
                {
                    Assert.AreEqual(ad.C(j), bd.C(j));
                }

                for (int j = 0; j < 2; ++j)
                {
                    Assert.AreEqual(ad.D(j), bd.D(j));
                }
            }

            Assert.AreEqual(a.A.Value.E, b.A.Value.E);

            for (int i = 0; i < 2; ++i)
            {
                Assert.AreEqual(a.A.Value.F(i), b.A.Value.F(i));
            }
        }

        private void TestObjectAPI(ArrayTable a)
        {
            var b = a.UnPack();
            AreEqual(a, b);

            var fbb = new FlatBufferBuilder(1, Allocator.Temp);
            fbb.Finish(ArrayTable.Pack(ref fbb, b).Value);
            var c = ArrayTable.GetRootAsArrayTable(ref fbb._bb);
            AreEqual(a, c);

            var jsonText = b.SerializeToJson();
            var d = ArrayTableT.DeserializeFromJson(jsonText);
            AreEqual(a, d);

            var fbBuffer = b.SerializeToBinary(Allocator.Temp);
            Assert.IsTrue(ArrayTable.ArrayTableBufferHasIdentifier(new ByteBuffer(fbBuffer)));
            var e = ArrayTableT.DeserializeFromBinary(fbBuffer);
            AreEqual(a, e);
        }

        private void AreEqual(Movie a, MovieT b)
        {
            Assert.AreEqual(a.MainCharacterType, b.MainCharacter.Type);
            Assert.AreEqual(a.MainCharacter<Rapunzel>().Value.HairLength, b.MainCharacter.AsRapunzel().HairLength);

            Assert.AreEqual(a.CharactersLength, b.Characters.Count);
            Assert.AreEqual(a.CharactersType(0), b.Characters[0].Type);
            Assert.AreEqual(a.Characters<Attacker>(0).Value.SwordAttackDamage, b.Characters[0].AsMuLan().SwordAttackDamage);
            Assert.AreEqual(a.CharactersType(1), b.Characters[1].Type);
            Assert.AreEqual(a.Characters<BookReader>(1).Value.BooksRead, b.Characters[1].AsBelle().BooksRead);
            Assert.AreEqual(a.CharactersType(2), b.Characters[2].Type);
            Assert.StringEqual(a.CharactersAsString(2), b.Characters[2].AsOther());
        }

        private void AreEqual(Movie a, Movie b)
        {
            Assert.AreEqual(a.MainCharacterType, b.MainCharacterType);
            Assert.AreEqual(a.MainCharacter<Rapunzel>().Value.HairLength, b.MainCharacter<Rapunzel>().Value.HairLength);

            Assert.AreEqual(a.CharactersLength, b.CharactersLength);
            Assert.AreEqual(a.CharactersType(0), b.CharactersType(0));
            Assert.AreEqual(a.Characters<Attacker>(0).Value.SwordAttackDamage, b.Characters<Attacker>(0).Value.SwordAttackDamage);
            Assert.AreEqual(a.CharactersType(1), b.CharactersType(1));
            Assert.AreEqual(a.Characters<BookReader>(1).Value.BooksRead, b.Characters<BookReader>(1).Value.BooksRead);
            Assert.AreEqual(a.CharactersType(2), b.CharactersType(2));
            Assert.StringEqual(a.CharactersAsString(2), b.CharactersAsString(2));
        }

        private void TestObjectAPI(Movie a)
        {
            var b = a.UnPack();
            AreEqual(a, b);

            var fbb = new FlatBufferBuilder(1, Allocator.Temp);
            fbb.Finish(Movie.Pack(ref fbb, b).Value);
            var c = Movie.GetRootAsMovie(ref fbb._bb);
            AreEqual(a, c);

            var jsonText = b.SerializeToJson();
            var d = MovieT.DeserializeFromJson(jsonText);
            AreEqual(a, d);

            var fbBuffer = b.SerializeToBinary(Allocator.Temp);
            Assert.IsTrue(Movie.MovieBufferHasIdentifier(new ByteBuffer(fbBuffer)));
            var e = MovieT.DeserializeFromBinary(fbBuffer);
            AreEqual(a, e);
        }

        // For use in TestParallelAccess test case.
        static private int _comparisons = 0;
        static private int _failures = 0;
        static private void KeepComparing(Monster mon, int count, float floatValue, double doubleValue)
        {
            int i = 0;
            while (++i <= count)
            {
                Interlocked.Add(ref _comparisons, 1);
                if(mon.Pos.Value.Test1 != doubleValue || mon.Pos.Value.Z != floatValue) {
                    Interlocked.Add(ref _failures, 1);
                }
            }
        }

        [FlatBuffersTestMethod]
        public void TestParallelAccess() {
            // Tests that reading from a flatbuffer over multiple threads is thread-safe in regard to double and float
            // values, since they previously were non-thread safe
            const float floatValue = 3.141592F;
            const double doubleValue = 1.618033988;

            var fbb = new FlatBufferBuilder(1, Allocator.Temp);
            var str = fbb.CreateString("ParallelTest");
            Monster.StartMonster(ref fbb);
            Monster.AddPos(ref fbb, Vec3.CreateVec3(ref fbb, 1.0f, 2.0f, floatValue, doubleValue,
                                                     Color.Green, (short)5, (sbyte)6));

            Monster.AddName(ref fbb, str);
            Monster.FinishMonsterBuffer(ref fbb, Monster.EndMonster(ref fbb));

            var mon = Monster.GetRootAsMonster(ref fbb._bb);

            var pos = mon.Pos.Value;
            Assert.AreEqual(pos.Test1, doubleValue);
            Assert.AreEqual(pos.Z, floatValue);

            const int thread_count = 10;
            const int reps = 1000000;

            // Need to use raw Threads since Tasks are not supported in .NET 3.5
            Thread[] threads = new Thread[thread_count];
            for(int i = 0; i < thread_count; i++) {
               threads[i] = new Thread(() => KeepComparing(mon, reps, floatValue, doubleValue));
            }
            for(int i = 0; i < thread_count; i++) {
               threads[i].Start();
            }
            for(int i = 0; i < thread_count; i++) {
               threads[i].Join();
            }

            // Make sure the threads actually did the comparisons.
            Assert.AreEqual(thread_count * reps, _comparisons);

            // Make sure we never read the values incorrectly.
            Assert.AreEqual(0, _failures);
        }

        [FlatBuffersTestMethod]
        public void TestScalarOptional_EmptyBuffer() {
            var fbb = new FlatBufferBuilder(1, Allocator.Temp);
            ScalarStuff.StartScalarStuff(ref fbb);
            var offset = ScalarStuff.EndScalarStuff(ref fbb);
            ScalarStuff.FinishScalarStuffBuffer(ref fbb, offset);

            ScalarStuff scalarStuff = ScalarStuff.GetRootAsScalarStuff(ref fbb._bb);
            Assert.AreEqual((sbyte)0, scalarStuff.JustI8);
            Assert.AreEqual(null, scalarStuff.MaybeI8);
            Assert.AreEqual((sbyte)42, scalarStuff.DefaultI8);
            Assert.AreEqual((byte)0, scalarStuff.JustU8);
            Assert.AreEqual(null, scalarStuff.MaybeU8);
            Assert.AreEqual((byte)42, scalarStuff.DefaultU8);

            Assert.AreEqual((short)0, scalarStuff.JustI16);
            Assert.AreEqual(null, scalarStuff.MaybeI16);
            Assert.AreEqual((short)42, scalarStuff.DefaultI16);
            Assert.AreEqual((ushort)0, scalarStuff.JustU16);
            Assert.AreEqual(null, scalarStuff.MaybeU16);
            Assert.AreEqual((ushort)42, scalarStuff.DefaultU16);

            Assert.AreEqual((int)0, scalarStuff.JustI32);
            Assert.AreEqual(null, scalarStuff.MaybeI32);
            Assert.AreEqual((int)42, scalarStuff.DefaultI32);
            Assert.AreEqual((uint)0, scalarStuff.JustU32);
            Assert.AreEqual(null, scalarStuff.MaybeU32);
            Assert.AreEqual((uint)42, scalarStuff.DefaultU32);

            Assert.AreEqual((long)0, scalarStuff.JustI64);
            Assert.AreEqual(null, scalarStuff.MaybeI64);
            Assert.AreEqual((long)42, scalarStuff.DefaultI64);
            Assert.AreEqual((ulong)0, scalarStuff.JustU64);
            Assert.AreEqual(null, scalarStuff.MaybeU64);
            Assert.AreEqual((ulong)42, scalarStuff.DefaultU64);

            Assert.AreEqual((float)0.0F, scalarStuff.JustF32);
            Assert.AreEqual(null, scalarStuff.MaybeF32);
            Assert.AreEqual((float)42.0F, scalarStuff.DefaultF32);

            Assert.AreEqual((double)0.0, scalarStuff.JustF64);
            Assert.AreEqual(null, scalarStuff.MaybeF64);
            Assert.AreEqual((double)42.0, scalarStuff.DefaultF64);

            Assert.AreEqual(false, scalarStuff.JustBool);
            Assert.AreEqual(null, scalarStuff.MaybeBool);
            Assert.AreEqual(true, scalarStuff.DefaultBool);

            Assert.AreEqual(OptionalByte.None, scalarStuff.JustEnum);
            Assert.AreEqual(null, scalarStuff.MaybeEnum);
            Assert.AreEqual(OptionalByte.One, scalarStuff.DefaultEnum);
        }

        [FlatBuffersTestMethod]
        public void TestScalarOptional_Construction() {
            var fbb = new FlatBufferBuilder(1, Allocator.Temp);
            ScalarStuff.StartScalarStuff(ref fbb);
            ScalarStuff.AddJustI8(ref fbb, 5);
            ScalarStuff.AddMaybeI8(ref fbb, 5);
            ScalarStuff.AddDefaultI8(ref fbb, 5);
            ScalarStuff.AddJustU8(ref fbb, 6);
            ScalarStuff.AddMaybeU8(ref fbb, 6);
            ScalarStuff.AddDefaultU8(ref fbb, 6);

            ScalarStuff.AddJustI16(ref fbb, 7);
            ScalarStuff.AddMaybeI16(ref fbb, 7);
            ScalarStuff.AddDefaultI16(ref fbb, 7);
            ScalarStuff.AddJustU16(ref fbb, 8);
            ScalarStuff.AddMaybeU16(ref fbb, 8);
            ScalarStuff.AddDefaultU16(ref fbb, 8);

            ScalarStuff.AddJustI32(ref fbb, 9);
            ScalarStuff.AddMaybeI32(ref fbb, 9);
            ScalarStuff.AddDefaultI32(ref fbb, 9);
            ScalarStuff.AddJustU32(ref fbb, 10);
            ScalarStuff.AddMaybeU32(ref fbb, 10);
            ScalarStuff.AddDefaultU32(ref fbb, 10);

            ScalarStuff.AddJustI64(ref fbb, 11);
            ScalarStuff.AddMaybeI64(ref fbb, 11);
            ScalarStuff.AddDefaultI64(ref fbb, 11);
            ScalarStuff.AddJustU64(ref fbb, 12);
            ScalarStuff.AddMaybeU64(ref fbb, 12);
            ScalarStuff.AddDefaultU64(ref fbb, 12);

            ScalarStuff.AddJustF32(ref fbb, 13.0f);
            ScalarStuff.AddMaybeF32(ref fbb, 13.0f);
            ScalarStuff.AddDefaultF32(ref fbb, 13.0f);
            ScalarStuff.AddJustF64(ref fbb, 14.0);
            ScalarStuff.AddMaybeF64(ref fbb, 14.0);
            ScalarStuff.AddDefaultF64(ref fbb, 14.0);

            ScalarStuff.AddJustBool(ref fbb, true);
            ScalarStuff.AddMaybeBool(ref fbb, true);
            ScalarStuff.AddDefaultBool(ref fbb, false); // note this is the opposite

            ScalarStuff.AddJustEnum(ref fbb, OptionalByte.Two);
            ScalarStuff.AddMaybeEnum(ref fbb, OptionalByte.Two);
            ScalarStuff.AddDefaultEnum(ref fbb, OptionalByte.Two);

            var offset = ScalarStuff.EndScalarStuff(ref fbb);
            ScalarStuff.FinishScalarStuffBuffer(ref fbb, offset);

            ScalarStuff scalarStuff = ScalarStuff.GetRootAsScalarStuff(ref fbb._bb);
            Assert.AreEqual((sbyte)5, scalarStuff.JustI8);
            Assert.AreEqual((sbyte)5, scalarStuff.MaybeI8);
            Assert.AreEqual((sbyte)5, scalarStuff.DefaultI8);
            Assert.AreEqual((byte)6, scalarStuff.JustU8);
            Assert.AreEqual((byte)6, scalarStuff.MaybeU8);
            Assert.AreEqual((byte)6, scalarStuff.DefaultU8);

            Assert.AreEqual((short)7, scalarStuff.JustI16);
            Assert.AreEqual((short)7, scalarStuff.MaybeI16);
            Assert.AreEqual((short)7, scalarStuff.DefaultI16);
            Assert.AreEqual((ushort)8, scalarStuff.JustU16);
            Assert.AreEqual((ushort)8, scalarStuff.MaybeU16);
            Assert.AreEqual((ushort)8, scalarStuff.DefaultU16);

            Assert.AreEqual((int)9, scalarStuff.JustI32);
            Assert.AreEqual((int)9, scalarStuff.MaybeI32);
            Assert.AreEqual((int)9, scalarStuff.DefaultI32);
            Assert.AreEqual((uint)10, scalarStuff.JustU32);
            Assert.AreEqual((uint)10, scalarStuff.MaybeU32);
            Assert.AreEqual((uint)10, scalarStuff.DefaultU32);

            Assert.AreEqual((long)11, scalarStuff.JustI64);
            Assert.AreEqual((long)11, scalarStuff.MaybeI64);
            Assert.AreEqual((long)11, scalarStuff.DefaultI64);
            Assert.AreEqual((ulong)12, scalarStuff.JustU64);
            Assert.AreEqual((ulong)12, scalarStuff.MaybeU64);
            Assert.AreEqual((ulong)12, scalarStuff.DefaultU64);

            Assert.AreEqual((float)13.0F, scalarStuff.JustF32);
            Assert.AreEqual((float)13.0F, scalarStuff.MaybeF32);
            Assert.AreEqual((float)13.0F, scalarStuff.DefaultF32);

            Assert.AreEqual((double)14.0, scalarStuff.JustF64);
            Assert.AreEqual((double)14.0, scalarStuff.MaybeF64);
            Assert.AreEqual((double)14.0, scalarStuff.DefaultF64);

            Assert.AreEqual(true, scalarStuff.JustBool);
            Assert.AreEqual(true, scalarStuff.MaybeBool);
            Assert.AreEqual(false, scalarStuff.DefaultBool);

            Assert.AreEqual(OptionalByte.Two, scalarStuff.JustEnum);
            Assert.AreEqual(OptionalByte.Two, scalarStuff.MaybeEnum);
            Assert.AreEqual(OptionalByte.Two, scalarStuff.DefaultEnum);
        }

        [FlatBuffersTestMethod]
        public void TestScalarOptional_Construction_CreatorMethod() {
            var fbb = new FlatBufferBuilder(1, Allocator.Temp);

            var offset = ScalarStuff.CreateScalarStuff(ref fbb,5,5,5,6,6,6,7,7,7,
                8,8,8,9,9,9,10,10,10,11,11,11,12,12,12,13.0f,13.0f,13.0f,14.0,
                14.0,14.0,true,true,false,OptionalByte.Two,OptionalByte.Two,
                OptionalByte.Two);
            ScalarStuff.FinishScalarStuffBuffer(ref fbb, offset);

            ScalarStuff scalarStuff = ScalarStuff.GetRootAsScalarStuff(ref fbb._bb);
            Assert.AreEqual((sbyte)5, scalarStuff.JustI8);
            Assert.AreEqual((sbyte)5, scalarStuff.MaybeI8);
            Assert.AreEqual((sbyte)5, scalarStuff.DefaultI8);
            Assert.AreEqual((byte)6, scalarStuff.JustU8);
            Assert.AreEqual((byte)6, scalarStuff.MaybeU8);
            Assert.AreEqual((byte)6, scalarStuff.DefaultU8);

            Assert.AreEqual((short)7, scalarStuff.JustI16);
            Assert.AreEqual((short)7, scalarStuff.MaybeI16);
            Assert.AreEqual((short)7, scalarStuff.DefaultI16);
            Assert.AreEqual((ushort)8, scalarStuff.JustU16);
            Assert.AreEqual((ushort)8, scalarStuff.MaybeU16);
            Assert.AreEqual((ushort)8, scalarStuff.DefaultU16);

            Assert.AreEqual((int)9, scalarStuff.JustI32);
            Assert.AreEqual((int)9, scalarStuff.MaybeI32);
            Assert.AreEqual((int)9, scalarStuff.DefaultI32);
            Assert.AreEqual((uint)10, scalarStuff.JustU32);
            Assert.AreEqual((uint)10, scalarStuff.MaybeU32);
            Assert.AreEqual((uint)10, scalarStuff.DefaultU32);

            Assert.AreEqual((long)11, scalarStuff.JustI64);
            Assert.AreEqual((long)11, scalarStuff.MaybeI64);
            Assert.AreEqual((long)11, scalarStuff.DefaultI64);
            Assert.AreEqual((ulong)12, scalarStuff.JustU64);
            Assert.AreEqual((ulong)12, scalarStuff.MaybeU64);
            Assert.AreEqual((ulong)12, scalarStuff.DefaultU64);

            Assert.AreEqual((float)13.0F, scalarStuff.JustF32);
            Assert.AreEqual((float)13.0F, scalarStuff.MaybeF32);
            Assert.AreEqual((float)13.0F, scalarStuff.DefaultF32);

            Assert.AreEqual((double)14.0, scalarStuff.JustF64);
            Assert.AreEqual((double)14.0, scalarStuff.MaybeF64);
            Assert.AreEqual((double)14.0, scalarStuff.DefaultF64);

            Assert.AreEqual(true, scalarStuff.JustBool);
            Assert.AreEqual(true, scalarStuff.MaybeBool);
            Assert.AreEqual(false, scalarStuff.DefaultBool);

            Assert.AreEqual(OptionalByte.Two, scalarStuff.JustEnum);
            Assert.AreEqual(OptionalByte.Two, scalarStuff.MaybeEnum);
            Assert.AreEqual(OptionalByte.Two, scalarStuff.DefaultEnum);
        }


        [FlatBuffersTestMethod]
        public void TestKeywordEscaping() {
            Assert.AreEqual((int)KeywordTest.@public.NONE, 0);

            Assert.AreEqual((int)KeywordTest.ABC.@void, 0);
            Assert.AreEqual((int)KeywordTest.ABC.where, 1);
            Assert.AreEqual((int)KeywordTest.ABC.@stackalloc, 2);

            var fbb = new FlatBufferBuilder(1, Allocator.Temp);
            var offset = KeywordsInTable.CreateKeywordsInTable(
                ref fbb, KeywordTest.ABC.@stackalloc, KeywordTest.@public.NONE);
            fbb.Finish(offset.Value);

            KeywordsInTable keywordsInTable =
                KeywordsInTable.GetRootAsKeywordsInTable(ref fbb._bb);

            Assert.AreEqual(keywordsInTable.Is, KeywordTest.ABC.@stackalloc);
            Assert.AreEqual(keywordsInTable.Private, KeywordTest.@public.NONE);
        }


        [FlatBuffersTestMethod]
        public void AddOptionalEnum_WhenPassNull_ShouldWorkProperly() {
          var fbb = new FlatBufferBuilder(1, Allocator.Temp);
          ScalarStuff.StartScalarStuff(ref fbb);
          ScalarStuff.AddMaybeEnum(ref fbb, null);
          var offset = ScalarStuff.EndScalarStuff(ref fbb);
          ScalarStuff.FinishScalarStuffBuffer(ref fbb, offset);

          ScalarStuff scalarStuff = ScalarStuff.GetRootAsScalarStuff(ref fbb._bb);
          Assert.AreEqual(null, scalarStuff.MaybeEnum);
        }


        [FlatBuffersTestMethod]
        public void SortKey_WithDefaultedValue_IsFindable() {
            // This checks if using the `key` attribute that includes the
            // default value (e.g., 0) is still searchable. This is a regression
            // test for https://github.com/google/flatbuffers/issues/7380.
            var fbb = new FlatBufferBuilder(1, Allocator.Temp);

            // Create a vector of Stat objects, with Count being the key.
            var stat_offsets = new Offset<Stat>[4];
            for(ushort i = 0; i < stat_offsets.Length; i++) {
                Stat.StartStat(ref fbb);
                Stat.AddCount(ref fbb, i);
                stat_offsets[stat_offsets.Length - 1 - i] = Stat.EndStat(ref fbb);
            }

            // Ensure the sort works.
            Array.Sort(stat_offsets, (o1, o2) => Stat.CompareStat(ref fbb, o1, o2));
            var sort = fbb.CreateVectorOfTables(stat_offsets.AsSpan());

            // Create the monster with the sorted vector of Stat objects.
            var str = fbb.CreateString("MyMonster");
            Monster.StartMonster(ref fbb);
            Monster.AddName(ref fbb, str);
            Monster.AddScalarKeySortedTables(ref fbb, sort);
            fbb.Finish(Monster.EndMonster(ref fbb).Value);

            // Get the monster.
            var monster = Monster.GetRootAsMonster(ref fbb._bb);

            // Ensure each key is findable.
            for(ushort i =0 ; i < stat_offsets.Length; i++) {
                Assert.IsTrue(monster.ScalarKeySortedTablesByKey(i) != null);
                Assert.AreEqual(monster.ScalarKeySortedTablesByKey(i).Value.Count, i);
            }
        }
    }
}
