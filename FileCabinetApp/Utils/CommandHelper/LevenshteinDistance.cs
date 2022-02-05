using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Utils.CommandHelper
{
    public static class LevenshteinDistance
    {
        public static int Calculate(string source1, string source2)
        {
            if (string.IsNullOrEmpty(source1) && string.IsNullOrEmpty(source2))
            {
                return 0;
            }

            if (string.IsNullOrEmpty(source1))
            {
                return source2.Length;
            }

            if (string.IsNullOrEmpty(source2))
            {
                return source1.Length;
            }

            int[,] matrix = new int[source1.Length + 1, source2.Length + 1];
            for (int i = 0; i <= source1.Length; i++)
            {
                matrix[i, 0] = i;
            }

            for (int j = 0; j <= source2.Length; j++)
            {
                matrix[0, j] = j;
            }

            for (int i = 1; i <= source1.Length; i++)
            {
                for (int j = 1; j <= source2.Length; j++)
                {
                    int d1 = matrix[i, j - 1] + 1;
                    int d2 = matrix[i - 1, j] + 1;
                    int d3 = matrix[i - 1, j - 1] + (char.ToUpperInvariant(source1[i - 1]).Equals(char.ToUpperInvariant(source2[j - 1])) ? 0 : 1);
                    matrix[i, j] = Math.Min(d1, Math.Min(d2, d3));
                }
            }

            return matrix[source1.Length, source2.Length];
        }
    }
}
