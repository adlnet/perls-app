using System;
using System.Collections.Generic;
using System.Linq;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Cache;
using PERLS.Data.ViewModels;

namespace PERLS.Data.Extensions
{
    /// <summary>
    /// Extensions for IEnumerable that are related to the app data.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Retrieves the underlying items represented by the teasers.
        /// </summary>
        /// <param name="teasers">The teasers.</param>
        /// <returns>The underlying items.</returns>
        public static IEnumerable<IItem> GetItems(this IEnumerable<TeaserViewModel> teasers)
        {
            return teasers.Select(teaser => teaser.ModelItem);
        }

        /// <summary>
        /// Rotates an <see cref="IEnumerable{T}"/> to the left by a random amount.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to return elements from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with the source values rotated.</returns>
        public static IEnumerable<T> RotateRandomly<T>(this IEnumerable<T> source)
        {
            return source.Rotate(new Random().Next(source.Count()));
        }

        /// <summary>
        /// Rotates the items in the enumerable.
        /// You can think of this like cutting a deck of cards:
        /// the <paramref name="amount"/> is where you're making the cut
        /// and then then the bottom half gets moved to the top half.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to return elements from.</param>
        /// <param name="amount">The amount to rotate by. A positive value will rotate left while a negative value will rotate right.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with the source values rotated.</returns>
        public static IEnumerable<T> Rotate<T>(this IEnumerable<T> source, int amount)
        {
            if (Math.Abs(amount) > source.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "amount must be less than the number of items");
            }

            if (amount > 0)
            {
                // Rotate left
                return source.Skip(amount).Concat(source.Take(amount));
            }
            else if (amount < 0)
            {
                // Rotate right
                return source.Rotate(source.Count() - Math.Abs(amount));
            }

            return source;
        }

        /// <summary>
        /// Explicitly marks that this list of items was derived from the local cache.
        /// </summary>
        /// <param name="items">The list of items.</param>
        /// <returns>The same list of items, but now marked as being derived from cache.</returns>
        public static CacheDerivedItems AsCacheDerived(this IEnumerable<IItem> items)
        {
            return new CacheDerivedItems(items);
        }

        /// <summary>
        /// Equivalent to the usual C# string join method, with a more fluent interface.
        /// </summary>
        /// <param name="items">The items to join.</param>
        /// <param name="separator">The separator between items.</param>
        /// <returns>The newly joined string.</returns>
        public static string Join(this IEnumerable<string> items, string separator) => string.Join(separator, items);
    }
}
