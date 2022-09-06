using System;
using System.Collections.Generic;

namespace PERLS.Data.Definition.Services
{
    /// <summary>
    /// A service that stores and retrieves cached file locations.
    /// </summary>
    public interface ICacheRegistryService
    {
        /// <summary>
        /// Add a new cache to the registry.
        /// </summary>
        /// <param name="cacheLocation">The location of the cache.</param>
        void RegisterCache(string cacheLocation);

        /// <summary>
        /// Remove an existing cache from the registry.
        /// </summary>
        /// <param name="cacheLocation">The location of the cache.</param>
        void UnregisterCache(string cacheLocation);

        /// <summary>
        /// List all registered cache locations.
        /// </summary>
        /// <returns>A list of cache locations.</returns>
        IEnumerable<string> RegisteredCaches();

        /// <summary>
        /// Clear all registered caches, without removing them from the registry.
        /// </summary>
        void ClearRegisteredCaches();
    }
}
