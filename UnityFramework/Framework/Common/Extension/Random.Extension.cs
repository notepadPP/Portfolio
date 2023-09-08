using System.Collections.Generic;
using UnityEngine;
namespace Framework.Common
{
    public static partial class Extension
    {
        private static System.Random _random = new System.Random();
        public static bool RandomBool() => Random.Range(0, 2) == 1;
        public static int Range(int minInclusive, int maxExclusive) => Random.Range(minInclusive, maxExclusive);
        public static float Range(float minInclusive, float maxExclusive) => Random.Range(minInclusive, maxExclusive);
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            int k;
            T temp;
            while (n > 1)
            {
                n--;
                k = _random.Next(n + 1);
                temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }

    }
}