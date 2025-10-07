using LCW.Core.CrossCuttingConcerns.Caching.Microsoft;
using LCW.Core.CrossCuttingConcerns.Caching;
using LCW.Core.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using LCW.Core.DataAccess.CDSWebApi;
using Polly.Extensions.Http;
using Polly;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LCW.Core.DependencyResolvers
{
    public class CoreModule : ICoreModule
    {
        private readonly string WebAPIClientName = "WebAPI";
        public Uri BaseAddress { get; }
        private readonly Config config;

        public CoreModule()
        {
            config = App.InitializeApp();
            BaseAddress = new Uri($"{config.Url}/api/data/v{config.Version}/");
        }
        public void Load(IServiceCollection services)
        {
            services.AddSingleton<Stopwatch>();
            services.AddSingleton<IHttpClientService, Service>();

            services.AddHttpClient(
                     name: WebAPIClientName,
                     configureClient: ConfigureHttpClient);
        }

        void ConfigureHttpClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = BaseAddress;
            httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutInSeconds);
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", "null");
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
