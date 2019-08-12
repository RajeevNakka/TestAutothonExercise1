using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAutothonLib
{
    public static class Extensions
    {
        public static string ExcludeSymbols(this string src) => new string(src.Where(char.IsLetterOrDigit).ToArray());
        public static float FindSimilarity(this string string1, string string2, bool excludeSymbols = true)
        {
            if (excludeSymbols)
            {
                string1 = ExcludeSymbols(string1);
                string2 = ExcludeSymbols(string2);
            }
            float dis = ComputeDistance(string1, string2);
            float maxLen = string1.Length;
            if (maxLen < string2.Length)
                maxLen = string2.Length;
            if (maxLen == 0.0F)
                return 1.0F;
            else
                return 1.0F - dis / maxLen;
        }
        private static int ComputeDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] distance = new int[n + 1, m + 1]; // matrix
            if (n == 0) return m;
            if (m == 0) return n;
            //init1
            for (int i = 0; i <= n; distance[i, 0] = i++) ;
            for (int j = 0; j <= m; distance[0, j] = j++) ;
            //find min distance
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t.Substring(j - 1, 1) ==
                        s.Substring(i - 1, 1) ? 0 : 1);
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1,
                    distance[i, j - 1] + 1),
                    distance[i - 1, j - 1] + cost);
                }
            }
            return distance[n, m];
        }
    }
}
