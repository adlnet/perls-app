using System;
using System.Collections;
using System.Collections.Generic;

namespace PERLS.Data.Extensions
{
    /// <summary>
    /// The List Extensions.
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Removes the items.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="toRemove">The items to remove.</param>
        public static void RemoveRange(this IList list, IEnumerable toRemove)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (toRemove == null)
            {
                throw new ArgumentNullException(nameof(toRemove));
            }

            foreach (var eachItem in toRemove)
            {
                list.Remove(eachItem);
            }
        }

        /// <summary>
        /// Removes the items from the list.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="toRemove">The items to remove.</param>
        public static void RemoveRange<T>(this IList<T> list, IEnumerable<T> toRemove)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (toRemove == null)
            {
                throw new ArgumentNullException(nameof(toRemove));
            }

            foreach (var eachItem in toRemove)
            {
                list.Remove(eachItem);
            }
        }
    }
}
