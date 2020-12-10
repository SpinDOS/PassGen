using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PassGen.Xamarin.Service
{
    public interface ISaltStorage
    {
        Task<string> GetSalt();
        Task SetSalt(string salt);
        Task ClearSalt();
    }
    
    public sealed class SaltStorage : ISaltStorage
    {
        private const string SaltKey = "PG_SALT";

        public Task<string> GetSalt() => SecureStorage.GetAsync(SaltKey);

        public Task SetSalt(string salt) => SecureStorage.SetAsync(SaltKey, salt);

        public Task ClearSalt()
        {
            SecureStorage.Remove(SaltKey);
            return Task.CompletedTask;
        }
    }
}