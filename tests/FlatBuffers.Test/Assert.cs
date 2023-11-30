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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;

namespace Fivemid.FiveFlat.Tests.Exported
{

    public class AssertFailedException : Exception
    {
        private readonly object _expected;
        private readonly object _actual;

        public AssertFailedException(object expected, object actual)
        {
            _expected = expected;
            _actual = actual;
        }

        public override string Message
        {
            get { return string.Format("Expected {0} but saw {1}", _expected, _actual); }
        }
    }

    public class AssertArrayFailedException : Exception
    {
        private readonly int _index;
        private readonly object _expected;
        private readonly object _actual;

        public AssertArrayFailedException(int index, object expected, object actual)
        {
            _index = index;
            _expected = expected;
            _actual = actual;
        }

        public override string Message
        {
            get { return string.Format("Expected {0} at index {1} but saw {2}", _expected, _index, _actual); }
        }
    }

    public class AssertUnexpectedThrowException : Exception
    {
        private readonly object _expected;

        public AssertUnexpectedThrowException(object expected)
        {
            _expected = expected;
        }

        public override string Message
        {
            get { return string.Format("Expected exception of type {0}", _expected); }
        }
    }

    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual)
        {
            if (!expected.Equals(actual))
            {
                throw new AssertFailedException(expected, actual);
            }
        }

        public static void StringEqual(string? expected, NativeArray<byte>? actual) {
          StringEqual(FlatBufferBuilder.EncodeString(expected, Allocator.Temp), actual);
        }

        public static void StringEqual(NativeArray<byte>? expected, string? actual) {
          StringEqual(expected, FlatBufferBuilder.EncodeString(actual, Allocator.Temp));
        }

        public static void StringEqual(NativeArray<byte>? expected, NativeArray<byte>? actual) {
          if (expected.HasValue != actual.HasValue) {
            throw new AssertFailedException(expected, actual);
          }

          if (expected.HasValue && actual.HasValue)
            ArrayEqual(expected.Value, actual.Value);
        }

        public static void ArrayEqual<T>(T[] expected, NativeArray<T> actual) where T : struct {
          ArrayEqual(new NativeArray<T>(expected, Allocator.Temp), actual);
        }

        public static void ArrayEqual<T>(NativeArray<T> expected, NativeArray<T> actual) where T : struct {
            if (expected.Length != actual.Length)
            {
                throw new AssertFailedException(expected, actual);
            }

            for(var i = 0; i < expected.Length; ++i)
            {
                if (!expected[i].Equals(actual[i]))
                {
                    throw new AssertArrayFailedException(i, expected, actual);
                }
            }
        }

        public static void ArrayEqual<T>(ArraySegment<T> expected, T[] actual)
        {
#if NETCOREAPP
            ArrayEqual(expected.ToArray(), actual);
#else
            if (expected.Count != actual.Length)
            {
                throw new AssertFailedException(expected, actual);
            }

            for (var i = 0; i < expected.Count; ++i)
            {
                if (!expected.Array[expected.Offset + i].Equals(actual[i]))
                {
                    throw new AssertArrayFailedException(i, expected, actual);
                }
            }
#endif
    }

        public static void IsTrue(bool value)
        {
            if (!value)
            {
                throw new AssertFailedException(true, value);
            }
        }

        public static void IsFalse(bool value)
        {
            if (value)
            {
                throw new AssertFailedException(false, value);
            }
        }

        public static void Throws<T>(Action action) where T : Exception
        {
            var caught = false;
            try
            {
                action();
            }
            catch (T)
            {
                caught = true;
            }

            if (!caught)
            {
                throw new AssertUnexpectedThrowException(typeof (T));
            }
        }
    }
}
