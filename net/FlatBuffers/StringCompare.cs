using System;
using Unity.Collections;

namespace Fivemid.FiveFlat
{
    public static class StringCompare
    {
        public static int CompareTo(this NativeArray<byte>? s1, NativeArray<byte>? s2)
        {
            if (!s1.HasValue || !s2.HasValue)
                return s1.HasValue.CompareTo(s2.HasValue);

            int minLength = Math.Min(s1.Value.Length, s2.Value.Length);
            for (int i = 0; i < minLength; i++)
            {
                int compare = s1.Value[i].CompareTo(s2.Value[i]);
                if (compare != 0) return compare;
            }

            return s1.Value.Length.CompareTo(s2.Value.Length);
        }
    }
}
