using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LCW.Core.DataAccess.CDSWebApi
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        Task<T> SendAsync<T>(HttpRequestMessage request) where T : HttpResponseMessage;

        Task<FetchXmlResponse> FetchXml(FetchXmlRequest fetchXmlRequest);

        Task<JObject> Retrieve(EntityReference entityReference, string query,
            bool includeAnnotations = false,
            string eTag = null,
            string partitionId = null);

        Task<RetrieveMultipleResponse> RetrieveMultiple(
            string queryUri,
            int? maxPageSize = null,
            bool includeAnnotations = false);
    }
}
