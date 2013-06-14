using System;

namespace Utilities.Helpers
{
    public class Levenshtein
    {
        public static int CalculateDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(target))
                return 0;
            if (string.IsNullOrEmpty(source))
                return target.Length;
            if (string.IsNullOrEmpty(target))
                return source.Length;

            if (source.Length > target.Length)
                Swap(ref source, ref target);

            int m = target.Length;
            int n = source.Length;
            var distance = new int[2, m + 1];

            for (int j = 1; j <= m; j++)
                distance[0, j] = j;

            int currentRow = 0;
            for (int i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                int previousRow = currentRow ^ 1;
                for (int j = 1; j <= m; j++)
                {
                    int cost = target[j - 1] == source[i - 1] ? 0 : 1;
                    distance[currentRow, j] =
                        Math.Min(Math.Min(distance[previousRow, j] + 1, distance[currentRow, j - 1] + 1),
                            distance[previousRow, j - 1] + cost);
                }
            }

            return distance[currentRow, m];
        }

        private static void Swap(ref string source, ref string target)
        {
            string temp = target;
            target = source;
            source = temp;
        }
    }
}