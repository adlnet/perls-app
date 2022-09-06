namespace PERLS.Data.Definition
{
    /// <summary>
    /// Interface for the pager.
    /// </summary>
    public interface IPager
    {
        /// <summary>
        /// Gets the current page index.
        /// </summary>
        /// <value>The current page index.</value>
        int CurrentPage { get; }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        /// <value>The total number of pages.</value>
        int TotalPages { get; }

        /// <summary>
        /// Gets the total number of rows.
        /// </summary>
        /// <value>The total number of rows.</value>
        int TotalItems { get; }

        /// <summary>
        /// Gets the number of rows per page.
        /// </summary>
        /// <value>The number of rows per page.</value>
        int ItemsPerPage { get; }
    }
}
