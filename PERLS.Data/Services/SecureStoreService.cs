using System.Threading.Tasks;
using Float.Core.Persistence;
using Xamarin.Essentials;

namespace PERLS.Services
{
    /// <summary>
    /// The implmentation for the secure store dependency service.
    /// </summary>
    public class SecureStoreService : ISecureStore
    {
        /// <inheritdoc />
        public bool Delete(string key)
        {
            return SecureStorage.Remove(key);
        }

        /// <inheritdoc />
        public string Get(string key)
        {
            return SecureStorage.GetAsync(key).Result;
        }

        /// <inheritdoc />
        public Task<string> GetAsync(string key)
        {
            return SecureStorage.GetAsync(key);
        }

        /// <inheritdoc />
        public bool Put(string key, string str)
        {
            var temp = SecureStorage.SetAsync(key, str);
            temp.Wait();
            return temp.Exception == null;
        }
    }
}
