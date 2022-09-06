using System.Threading.Tasks;

namespace PERLS.Data.ViewModels
{
    /// <summary>
    /// A view model that has a live connection to it's data source.
    /// </summary>
    public interface IRefreshableViewModel
    {
        /// <summary>
        /// Gets a value indicating whether the data in the view model is derived from the local cache.
        /// </summary>
        /// <value><c>true</c> if the results came from the local cache instead of the server.</value>
        bool IsCacheDerived { get; }

        /// <summary>
        /// Refresh the data in this view model.
        /// </summary>
        /// <returns>The refresh task.</returns>
        Task Refresh();
    }
}
