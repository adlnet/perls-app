using Float.Core.Persistence;

namespace PERLS.Data.Definition
{
    /// <summary>
    /// Defines an interface for a cached provider's internal cache.
    /// This could be implemented with a dictionary (for an in-memory cache) or by serializing to file.
    /// </summary>
    public interface ICacheStorage : IKeyValueStore
    {
        /// <summary>
        /// Determines whether or not the given key has a value in this cache.
        /// </summary>
        /// <param name="key">The key for which to determine the status.</param>
        /// <returns><c>true</c> if the given key has a value in this cache, false otherwise.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Clears all keys and values in this cache.
        /// </summary>
        void Clear();
    }
}
