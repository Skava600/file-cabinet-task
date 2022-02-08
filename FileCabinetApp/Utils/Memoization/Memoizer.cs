using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Utils.Memoization
{
    public static class Memoizer
    {
        public static Func<A, B, R> Memoize<A, B, R>(Func<A, B, R> func)
        {
            var cache = new Dictionary<(A, B), R>();
            return (a, b) =>
            {
                R value;
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
