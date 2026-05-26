namespace AcaHelpAPI.Services
{
    public interface ICacheService
    {

        Task SetItemInCache <T>(string key, T item, TimeSpan? expirationTime = null);
        Task<T> GetItemInCache<T>(string key);

        Task RemoveItem(string key);

    }
}
