using System;
using System.Collections.Generic;

namespace MinesweeperSolver.Common.BclExtensions
{
    public static class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
