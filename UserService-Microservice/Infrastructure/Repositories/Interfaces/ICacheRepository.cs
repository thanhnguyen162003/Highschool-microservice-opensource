namespace Infrastructure.Repositories.Interfaces
{
	public interface ICacheRepository
	{
		/// <summary>
		/// Get cache value by key
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		Task<T> GetAsync<T>(string key);

		/// <summary>
		/// Get cache value by key
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		Task<T> GetAsync<T>(string type, string key);

		/// <summary>
		/// Set cache value by key
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		Task SetAsync<T>(string type, string key, T value);

		/// <summary>
		/// Set cache value by key
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="timeCache"></param>
		/// <returns></returns>
		Task SetAsync<T>(string type, string key, T value, int timeCache);

		/// <summary>
		/// Remove cache value by key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		Task RemoveAsync(string key);

		/// <summary>
		/// Remove cache value by key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		Task RemoveAsync(string type, string key);
	}
}
