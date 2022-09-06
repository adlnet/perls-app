using System.Threading.Tasks;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// An interface defining a remote provider.
    /// </summary>
    public interface IRemoteProvider
    {
        /// <summary>
        /// Gets a value indicating whether this provider is reachable.
        /// </summary>
        /// <returns><c>true</c> if the provider is reachable, <c>false</c> otherwise.</returns>
        Task<bool> IsReachable();
    }
}
