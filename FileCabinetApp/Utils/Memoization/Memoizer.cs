using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Utils.Memoization
{
    /// <summary>
    /// Memozer.
    /// </summary>
    public static class Memoizer
    {
        /// <summary>
        /// Memoizes function with two parameters and returning value.
        /// </summary>
        /// <typeparam name="TA"> First type parameter.</typeparam>
        /// <typeparam name="TB"> Second type parameter. </typeparam>
        /// <typeparam name="TR"> Result type. </typeparam>
        /// <param name="func"> Function to memoize. </param>
        /// <returns> memoized Function. </returns>
        public static Func<TA, TB, TR> Memoize<TA, TB, TR>(Func<TA, TB, TR> func)
        {
            var cache = new Dictionary<(TA, TB), TR>();
            return (a, b) =>
            {
                TR? value;
                if (cache.TryGetValue((a, b), out value))
                {
                    return value;
                }

                value = func(a, b);
                cache.Add((a, b), value);
                return value;
            };
        }
    }
}
