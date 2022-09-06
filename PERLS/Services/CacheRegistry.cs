using System.Collections.Generic;
using System.IO;
using PERLS.Data.Definition.Services;

namespace PERLS.Services
{
    /// <summary>
    /// A registry of cache file locations.
    /// </summary>
    public class CacheRegistry : ICacheRegistryService
    {
        readonly List<string> uris = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheRegistry"/> class.
        /// </summary>
        public CacheRegistry()
        {
        }

        /// <inheritdoc />
        public void RegisterCache(string cacheLocation)
        {
            uris.Add(cacheLocation);
        }

        /// <inheritdoc />
        public void UnregisterCache(string cacheLocation)
        {
            uris.Remove(cacheLocation);
        }

        /// <inheritdoc />
        public IEnumerable<string> RegisteredCaches()
        {
            return uris;
        }

        /// <inheritdoc />
        public void ClearRegisteredCaches()
        {
            var caches = RegisteredCaches();

            foreach (var cache in caches)
            {
                if (File.Exists(cache))
                {
                    File.Delete(cache);
                }
            }
        }
    }
}
