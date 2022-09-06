using System;
using System.Threading.Tasks;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Defines a method to be used to handle loading pages of a paged response.
    /// </summary>
    public interface IPagedResponseHandler
    {
        /// <summary>
        /// Loads the next page of the paged response.
        /// </summary>
        /// <typeparam name="T">The type of items held by the paged response.</typeparam>
        /// <param name="response">The response used to load the next page.</param>
        /// <returns>A task to retrieve the next page of the given paged response.</returns>
        Task<IPagedResponse<T>> LoadMore<T>(IPagedResponse<T> response);
    }
}
