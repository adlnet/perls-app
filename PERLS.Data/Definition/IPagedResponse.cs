using System.Collections;
using System.Collections.Generic;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// The interface for a typed paged response.
    /// </summary>
    /// <typeparam name="TRow">The type of each row.</typeparam>
    public interface IPagedResponse<out TRow> : IEnumerable<TRow>, IPartialResponse
    {
        /// <summary>
        /// Gets the current page index.
        /// </summary>
        /// <value>The current page index.</value>
        int CurrentPage { get; }

        /// <summary>
        /// Gets the total number of pages available.
        /// </summary>
        /// <value>The total number of pages available.</value>
        int PageCount { get; }

        /// <summary>
        /// Gets a value indicating whether this represents the last page of the response.
        /// </summary>
        /// <value><c>true</c> if all pages have been loaded.</value>
        bool IsLastPage { get; }

        /// <summary>
        /// Gets the pager to be used by the paged response.
        /// </summary>
        /// <value>The pager to be used by the paged response.</value>
        IPager Pager { get; }

        /// <summary>
        /// Gets the list of rows on the page.
        /// </summary>
        /// <value>The list of rows on the page.</value>
        IList Rows { get; }
    }
}
