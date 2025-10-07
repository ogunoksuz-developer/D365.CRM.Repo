using LCW.Interfaces.Abstract;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LCW.Business.Concrete
{
    public class LiveApiManager : ILiveApiService
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly IHttpClientFactory _httpClientFactory;

        public LiveApiManager(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _baseUrl = configuration["LiveApiUrl"];
            _apiKey = configuration["LiveApiKey"];
            _httpClientFactory = httpClientFactory;
        }

        public async Task ClearCache(string cacheKey)
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{_baseUrl}/Defination/ClearCacheByKey?key={cacheKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("ApiKey", _apiKey);

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"Error while clearing cache. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}";
                throw new HttpRequestException(errorMessage);
            }
        }
    }
}
