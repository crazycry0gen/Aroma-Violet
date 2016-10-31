using System;
using System.Collections.Generic;

namespace StratCorp.CorpSMS
{
    public static class Extensions
    {
        /// <summary>
        /// Performs and action on each element of a sequnce.
        /// </summary>
        public static void ForAll<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (T item in sequence)
            {
                action(item);
            }
        }
    }
}
