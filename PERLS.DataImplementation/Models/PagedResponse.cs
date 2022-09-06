using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Models
{
    /// <summary>
    /// Page information for the paged response.
    /// </summary>
    [Serializable]
    public struct Pager : IEquatable<Pager>, IPager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Pager"/> struct.
        /// </summary>
        /// <param name="pager">The pager.</param>
        public Pager(IPager pager)
        {
            if (pager == null)
            {
                throw new ArgumentNullException(nameof(pager));
            }

            this.CurrentPage = pager.CurrentPage;
            this.TotalPages = pager.TotalPages;
            this.TotalItems = pager.TotalItems;
            this.ItemsPerPage = pager.ItemsPerPage;
        }

        /// <inheritdoc />
        [JsonProperty("current_page")]
        public int CurrentPage { get; private set; }

        /// <inheritdoc />
        [JsonProperty("total_pages")]
        public int TotalPages { get; private set; }

        /// <inheritdoc />
        [JsonProperty("total_items")]
        public int TotalItems { get; private set; }

        /// <inheritdoc />
        [JsonProperty("items_per_page")]
        public int ItemsPerPage { get; private set; }

        /// <summary>
        /// Determine if two <see cref="Pager"/> are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if both instances are equivalent, <c>false</c> otherwise.</returns>
        public static bool operator ==(Pager left, Pager right) => left.Equals(right);

        /// <summary>
        /// Determine if two <see cref="Pager"/> are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if both instances are equivalent, <c>false</c> otherwise.</returns>
        public static bool operator !=(Pager left, Pager right) => !(left == right);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is Pager pager)
            {
                return Equals(pager);
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode() => (CurrentPage, TotalPages, TotalItems, ItemsPerPage).GetHashCode();

        /// <inheritdoc />
        public bool Equals(Pager other) => (CurrentPage, TotalPages, TotalItems, ItemsPerPage) == (other.CurrentPage, other.TotalPages, other.TotalItems, other.ItemsPerPage);
    }

    /// <summary>
    /// A paged response.
    /// </summary>
    /// <typeparam name="T">The type of each row.</typeparam>
    [Serializable]
    [JsonObject]
    public class PagedResponse<T> : PartialResponse, IPagedResponse<T>
    {
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList Rows { get; internal set; } = new List<T>();

        /// <inheritdoc />
        public int CurrentPage => Pager.CurrentPage;

        /// <inheritdoc />
        public int PageCount => Pager.TotalPages;

        /// <summary>
        /// Gets the number of rows currently loaded.
        /// </summary>
        /// <value>The number of rows currently loaded.</value>
        public int RowCount => Rows.Count;

        /// <summary>
        /// Gets the total number of rows available.
        /// </summary>
        /// <value>The total number of rows available.</value>
        public int TotalRowCount => Pager.TotalItems;

        /// <summary>
        /// Gets the number of rows returned on each page.
        /// </summary>
        /// <value>The number of rows returned on each page.</value>
        public int PageSize => Pager.ItemsPerPage;

        /// <inheritdoc />
        public bool IsLastPage => CurrentPage + 1 >= PageCount;

        /// <inheritdoc/>
        public IPager Pager => JsonPager;

        /// <summary>
        /// Gets more detailed pager information.
        /// </summary>
        /// <value>The pager.</value>
        /// <remarks>
        /// This is intended to merely be an implementation detail,
        /// but is needed to be a public getter in order for JSON.NET to
        /// deserialize it correctly.
        /// </remarks>
        [JsonProperty("pager")]
        internal Pager JsonPager { get; private set; }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Rows).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Rows).GetEnumerator();
    }
}
