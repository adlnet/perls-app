using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Float.Core.Extensions;
using PERLS.Data.Definition;

namespace PERLS.DataImplementation.Providers
{
    /// <summary>
    /// A base class for cached providers.
    /// </summary>
    /// <typeparam name="T">The type of source provider.</typeparam>
    public abstract class BaseCachedProvider<T> : ICachedProvider where T : IRemoteProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCachedProvider{T}"/> class.
        /// </summary>
        /// <param name="source">The source remote provider to cache.</param>
        /// <param name="cache">The cache to use when storing items.</param>
        public BaseCachedProvider(T source, ICacheStorage cache)
        {
            if (source is ICachedProvider)
            {
                throw new ArgumentException("Source must not be a cached provider.");
            }

            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        /// Gets the source provider. Generally, this should only be accessed by subclasses.
        /// </summary>
        /// <value>The source provider.</value>
        public T Source { get; }

        /// <summary>
        /// Gets the cache provider. Generally, this should only be accessed by subclasses.
        /// </summary>
        /// <value>The cache provider.</value>
        public ICacheStorage Cache { get; }

        /// <inheritdoc />
        public void ClearCache()
        {
            Cache.Clear();
        }

        /// <summary>
        /// Returns the cached result for the caller if one is available; if not, checks the source for reachability and returns (and caches) from the source instead.
        /// </summary>
        /// <typeparam name="TResult">The type of the result object.</typeparam>
        /// <param name="uncached">A task to await to retrieve the remote resource.</param>
        /// <param name="defaultValue">A default value to return if no cached result is available. Defaults to <c>default</c>.</param>
        /// <param name="caller">An auto-derived parameter that is used as a cache key.</param>
        /// <returns>The cached result if available, a default value if the source is not reachable, or a value from the remote provider.</returns>
        internal Task<TResult> GetCachedResult<TResult>(Task<TResult> uncached, TResult defaultValue = default, [CallerMemberName] string caller = null)
        {
            return GetCachedResult(async () => await uncached.ConfigureAwait(false), defaultValue, caller);
        }

        /// <summary>
        /// Returns the cached result for the caller if one is available; if not, checks the source for reachability and returns (and caches) from the source instead.
        /// </summary>
        /// <typeparam name="TResult">The type of the result object.</typeparam>
        /// <param name="uncached">A method to invoke to retrieve the remote resource.</param>
        /// <param name="defaultValue">A default value to return if no cached result is available. Defaults to <c>default</c>.</param>
        /// <param name="caller">An auto-derived parameter that is used as a cache key.</param>
        /// <returns>The cached result if available, a default value if the source is not reachable, or a value from the remote provider.</returns>
        internal async Task<TResult> GetCachedResult<TResult>(Func<Task<TResult>> uncached, TResult defaultValue = default, [CallerMemberName] string caller = null)
        {
            if (uncached == null)
            {
                throw new ArgumentNullException(nameof(uncached));
            }

            if (string.IsNullOrWhiteSpace(caller))
            {
                throw new ArgumentException("No caller member name received.", nameof(caller));
            }

            if (!await Source.IsReachable().ConfigureAwait(false) && GetCached(defaultValue, caller) is TResult cachedResult)
            {
                return cachedResult;
            }

            TResult sourceResult;

            try
            {
                sourceResult = await uncached().ConfigureAwait(false);
            }
            catch (Exception e) when (e.IsOfflineException())
            {
                var cached = GetCached(defaultValue, caller);

                // If there is no cached result here, forward the exception to the caller
                // so they know why the attempt to fulfill the request failed.
                if (EqualityComparer<TResult>.Default.Equals(cached, default))
                {
                    throw;
                }

                return cached;
            }

            Cache.Put(caller, sourceResult);
            return sourceResult;
        }

        /// <summary>
        /// Make a cached call to a remote provider. If the remote is not available, this will be called when it is next available.
        /// </summary>
        /// <param name="task">A task to retrieve data from the remote provider.</param>
        /// <param name="caller">An auto-derived parameter that is used as a cache key.</param>
        internal async Task CachedCall(Task task, [CallerMemberName] string caller = null)
        {
            await CachedCall(async () => await task.ConfigureAwait(false), caller).ConfigureAwait(false);
        }

        /// <summary>
        /// Make a cached call to a remote provider. If the remote is not available, this will be called when it is next available.
        /// </summary>
        /// <param name="uncached">The call to make on the source provider.</param>
        /// <param name="caller">An auto-derived parameter that is used as a cache key.</param>
        internal async Task CachedCall(Func<Task> uncached, [CallerMemberName] string caller = null)
        {
            if (uncached == null)
            {
                throw new ArgumentNullException(nameof(uncached));
            }

            if (string.IsNullOrWhiteSpace(caller))
            {
                throw new ArgumentException("No caller member name received.", nameof(caller));
            }

            if (await Source.IsReachable().ConfigureAwait(false))
            {
                await uncached().ConfigureAwait(false);
            }
            else
            {
                // queue this up for later?
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

        TResult GetCached<TResult>(TResult defaultValue, string key)
        {
            if (Cache.ContainsKey(key))
            {
                var cachedResult = Cache.Get<TResult>(key);

                if (cachedResult is TResult typedResult)
                {
                    return typedResult;
                }
                else
                {
                    throw new InvalidOperationException($"Object received from cache was incorrect type. Received {cachedResult.GetType().Name}, expected {typeof(TResult).Name}");
                }
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
