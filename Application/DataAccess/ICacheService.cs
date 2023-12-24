namespace Application.DataAccess;

/// <summary>
/// Cache service contract
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets data by specific key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T">Expected type of cached value</typeparam>
    /// <returns><see langword="null"/> or an instance of <typeparamref name="T"/></returns>
    Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets data into cache by specific key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T">Type of object to set</typeparam>
    /// <returns></returns>
    Task<bool> SetDataAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    Task<bool> GetOrSetDataAsync<T>(string key, ref T output, CancellationToken cancellationToken = default);

    Task<bool> RemoveDataAsync(string key, CancellationToken cancellationToken = default);
}