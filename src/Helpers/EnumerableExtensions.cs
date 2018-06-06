using System;
using System.Collections.Generic;

namespace AppTiles.Helpers
{
    public static class EnumerableExtensions
    {
        public static (List<T> trueList, List<T> falseList) Separate<T>(this IEnumerable<T> enumerable,
            Func<T, bool> predicate)
        {
            var trueList = new List<T>();
            var falseList = new List<T>();
            foreach (var item in enumerable)
            {
                if(predicate(item))
                    trueList.Add(item);
                else
                    falseList.Add(item);
            }

            return (trueList, falseList);
        }
    }
}
