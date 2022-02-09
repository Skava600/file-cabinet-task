using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Utils.Memoization
{
    public static class Memoizer
    {
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
