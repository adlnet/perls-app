using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Float.Core.Analytics;
using Float.Core.Extensions;
using PERLS.Data.Definition;
using PERLS.Data.Definition.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PERLS.DataImplementation.Providers
{
    /// <summary>
    /// A provider cache that serializes to disk.
    /// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public class DiskProviderCache : ICacheStorage
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        const string Extension = "dpc";
        static readonly string LocalFolder = DependencyService.Get<IPlatformFileProcessor>().NoBackupFolder;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskProviderCache"/> class.
        /// </summary>
        /// <param name="fileName">The name of the file to which to serialize data.</param>
        /// <param name="register">Whether or not to register this cache with the cache registry service.</param>
        public DiskProviderCache(string fileName, bool register = true)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException($"File name is required: {fileName}", nameof(fileName));
            }

            if (fileName.EndsWith($".{Extension}", StringComparison.InvariantCultureIgnoreCase))
            {
                FilePath = FilePath = Path.Combine(LocalFolder, fileName);
            }
            else
            {
                FilePath = Path.Combine(LocalFolder, $"{fileName}.{Extension}");
            }

            if (register)
            {
                DependencyService.Get<ICacheRegistryService>().RegisterCache(FilePath);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DiskProviderCache"/> class.
        /// </summary>
        ~DiskProviderCache()
        {
            semaphore.Dispose();
        }

        /// <summary>
        /// Gets the path to the local file.
        /// </summary>
        /// <value>The local file path.</value>
        public string FilePath { get; }

        /// <summary>
        /// Gets all keys contained in this cache.
        /// </summary>
        /// <value>All keys in the current cache.</value>
        public IEnumerable<string> Keys => ReadFromFile<Dictionary<string, object>>(FilePath)?.Keys ?? Enumerable.Empty<string>();

        /// <summary>
        /// Gets all values contained in this cache.
        /// </summary>
        /// <value>All values in the current cache.</value>
        public IEnumerable<object> Values => ReadFromFile<Dictionary<string, object>>(FilePath)?.Values ?? Enumerable.Empty<object>();

        /// <summary>
        /// Get a list of known cache files.
        /// </summary>
        /// <returns>A list of cache files.</returns>
        public static IEnumerable<string> KnownCacheFiles()
        {
            return Directory
                .EnumerateFiles(LocalFolder)
                .Where(file => new FileInfo(file).Extension == $".{Extension}");
        }

        /// <summary>
        /// Clear the local cache folder, erasing all cached data.
        /// </summary>
        public static void ClearCacheFolder()
        {
            if (Directory.Exists(LocalFolder))
            {
                Directory.Delete(LocalFolder, true);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            File.Delete(FilePath);
        }

        /// <inheritdoc />
        public bool ContainsKey(string key)
        {
            var data = ReadFromFile<Dictionary<string, object>>(FilePath);

            if (data == null)
            {
                return false;
            }

            return data.ContainsKey(key);
        }

        /// <inheritdoc />
        public T Get<T>(string key)
        {
            var data = ReadFromFile<Dictionary<string, object>>(FilePath);

            if (data == null)
            {
                return default;
            }

            if (data.TryGetValue(key, out object value) && value is T result)
            {
                return result;
            }

            return default;
        }

        /// <inheritdoc />
        public void Put<T>(string key, T value)
        {
            var data = ReadFromFile<Dictionary<string, object>>(FilePath);

            if (data == null)
            {
                data = new Dictionary<string, object>();
            }

            data[key] = value;
            WriteToFile(data, FilePath);
        }

        /// <inheritdoc />
        public void Delete(string key)
        {
            var data = ReadFromFile<Dictionary<string, object>>(FilePath);

            if (data == null || !data.ContainsKey(key))
            {
                return;
            }

            data.Remove(key);
            WriteToFile(data, FilePath);
        }

        bool WriteToFile<T>(T obj, string path)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"Invalid path for writing to file: {path}", nameof(path));
            }

            // we don't check the entire graph here; it's still possible T has non-serializable properties
            if (!obj.GetType().IsSerializable)
            {
                throw new ArgumentException($"Given type {obj.GetType()} is not serializable.");
            }

            var dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
            {
                // this seems to handle both paths and sub-paths
                Directory.CreateDirectory(dir);
            }

            using (semaphore.UseWait())
            using (var fileStream = GetFileStream(path, FileMode.Create, FileAccess.Write))
            {
                if (fileStream == null)
                {
                    return false;
                }

                try
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(fileStream, obj);
                    return true;
                }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is SerializationException)
                {
                    // if the file is locked, we get an IOException; if readonly, we get unauthorized access
                    // if we're trying to serialize an unserializable type, we get the serialization exception
                    // in any case, we just can't write to the file currently
                    DependencyService.Get<AnalyticsService>().TrackException(e);
                }
                finally
                {
                    // additional handling for rare full disk cases
                    try
                    {
                        fileStream.Close();
                    }
                    catch (IOException e)
                    {
                        DependencyService.Get<AnalyticsService>().TrackException(e);
                    }
                }
            }

            return false;
        }

        T ReadFromFile<T>(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"Invalid path for reading from file: {path}", nameof(path));
            }

            T result = default;

            if (!File.Exists(path))
            {
                return result;
            }

            using (semaphore.UseWait())
            using (var fileStream = GetFileStream(path, FileMode.Open, FileAccess.Read))
            {
                if (fileStream == null)
                {
                    return result;
                }

                if (fileStream.Length == 0)
                {
                    fileStream.Close();
                    return default;
                }

                try
                {
                    var bf = new BinaryFormatter();
                    result = (T)bf.Deserialize(fileStream);
                }
                catch (Exception e) when (e is IOException || e is SerializationException || e is EndOfStreamException)
                {
                    DependencyService.Get<AnalyticsService>().TrackException(e);
                }
                finally
                {
                    fileStream.Close();
                }
            }

            return result;
        }

        FileStream GetFileStream(string path, FileMode fileMode, FileAccess fileAccess)
        {
            FileStream fileStream = null;

            try
            {
                fileStream = new FileStream(path, fileMode, fileAccess);
            }
            catch (IOException e)
            {
                DependencyService.Get<AnalyticsService>().TrackException(e);
                fileStream?.Dispose();
                return null;
            }

            return fileStream;
        }
    }
}
