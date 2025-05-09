using Application.Common.Models.MasterDataModel;
using Domain.Constants;
using Newtonsoft.Json;

namespace Application.Services.MasterData
{
    public class MasterDataService : IMasterDataService
    {
        private IEnumerable<Cirriculum> Cirriculum { get; set; }

        private readonly SemaphoreSlim _lock = new(1, 1);
        private DateTime _lastUpdated = DateTime.MinValue;

        public MasterDataService()
        {
            Cirriculum = new List<Cirriculum>();
        }

        public async Task<IEnumerable<Cirriculum>> GetCirriculum()
        {
            if(Cirriculum == null || !Cirriculum.Any() || DateTime.Now - _lastUpdated > TimeSpan.FromMinutes(5))
            {
                await InitializeAsync();
            }

            return this.Cirriculum!;
        }

        private async Task InitializeAsync()
        {
            await _lock.WaitAsync();
            try
            {
                var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(150)
                };

                var response = await client.GetAsync($"{UrlConstant.DocumentService}/api/v1/curriculum");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    this.Cirriculum = JsonConvert.DeserializeObject<IEnumerable<Cirriculum>>(content) ?? Enumerable.Empty<Cirriculum>();
                    _lastUpdated = DateTime.Now;
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            } finally
            {
                _lock.Release();
            }
        }

    }
}
