using LCW.Core.Exceptions;
using LCW.Core.Extensions;
using LCW.Core.LCW.Entities.Concrete;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Batch;
using PowerApps.Samples.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace LCW.Core.DataAccess.CDSWebApi
{
    public class Service : IHttpClientService, IDisposable
    {
        // Service configuration data passed into the constructor
        private readonly Config config;
        private readonly string WebAPIClientName = "WebAPI";
        private bool _disposedValue;
        private string _sessionToken = null;

        private readonly IHttpClientFactory _httpClientFactory;


        /// <summary>
        /// The constructor for the service
        /// </summary>
        /// <param name="configParam"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public Service(IHttpClientFactory httpClientFactory)
        {
            Config configParam = App.InitializeApp();
            _httpClientFactory = httpClientFactory;
            config = configParam;
            BaseAddress = new Uri($"{config.Url}/api/data/v{config.Version}/");
        }


        /// <summary>
        /// Processes requests and returns responses. Manages Service Protection Limit errors.
        /// </summary>
        /// <param name="request">The request to send.</param>
        /// <returns>The response from the HttpClient</returns>
        /// <exception cref="Exception"></exception>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            // Session token used by elastic tables to enable strong consistency
            // See https://learn.microsoft.com/power-apps/developer/data-platform/use-elastic-tables?tabs=webapi#sending-the-session-token
            if (!string.IsNullOrWhiteSpace(_sessionToken) && request.Method == HttpMethod.Get)
            {
                request.Headers.Add("MSCRM.SessionToken", _sessionToken);
            }

            // Get the named HttpClient from the IHttpClientFactory
            var client = _httpClientFactory.CreateClient(WebAPIClientName);

            // Set the access token using the function from the Config passed to the constructor
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

            HttpResponseMessage response = await client.SendAsync(request);


            // Capture the current session token value
            // See https://learn.microsoft.com/power-apps/developer/data-platform/use-elastic-tables?tabs=webapi#getting-the-session-token
            if (response.Headers.Contains("x-ms-session-token"))
            {
                _sessionToken = response.Headers.GetValues("x-ms-session-token")?.FirstOrDefault()?.ToString();
            }

            // Throw an exception if the request is not successful
            if (!response.IsSuccessStatusCode)
            {
                ServiceException exception = await ParseError(response);
                throw exception;

            }
            return response;
        }

        /// <summary>
        /// Processes requests with typed responses
        /// </summary>
        /// <typeparam name="T">The type derived from HttpResponseMessage</typeparam>
        /// <param name="request">The request</param>
        /// <returns></returns>
        public async Task<T> SendAsync<T>(HttpRequestMessage request) where T : HttpResponseMessage
        {
            HttpResponseMessage response = await SendAsync(request);

            // 'As' method is Extension of HttpResponseMessage see Extensions.cs
            return response.As<T>();
        }


        public static async Task<ServiceException> ParseError(HttpResponseMessage response)
        {
            string requestId = string.Empty;
            if (response.Headers.Contains("REQ_ID"))
            {
                requestId = response.Headers.GetValues("REQ_ID").FirstOrDefault();
            }

            var content = await response.Content.ReadAsStringAsync();
            ODataError oDataError = null;

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                oDataError = System.Text.Json.JsonSerializer.Deserialize<ODataError>(content, options);
            }
            catch (Exception)
            {
                // Error may not be in correct OData Error format, so keep trying...
            }

            if (oDataError?.Error != null)
            {

                var exception = new ServiceException(oDataError.Error.Message)
                {
                    ODataError = oDataError,
                    Content = content,
                    ReasonPhrase = response.ReasonPhrase,
                    HttpStatusCode = response.StatusCode,
                    RequestId = requestId
                };
                return exception;
            }
            else
            {
                try
                {
                    ODataExceptionModel oDataException = System.Text.Json.JsonSerializer.Deserialize<ODataExceptionModel>(content);

                    ServiceException otherException = new(oDataException.Message)
                    {
                        Content = content,
                        ReasonPhrase = response.ReasonPhrase,
                        HttpStatusCode = response.StatusCode,
                        RequestId = requestId
                    };
                    return otherException;

                }
                catch (Exception)
                {
                    // Error may not be in correct OData Error format, so keep trying...
                }

                //When nothing else works
                ServiceException exception = new(response.ReasonPhrase)
                {
                    Content = content,
                    ReasonPhrase = response.ReasonPhrase,
                    HttpStatusCode = response.StatusCode,
                    RequestId = requestId
                };
                return exception;
            }
        }

        public async Task<FetchXmlResponse> FetchXml(FetchXmlRequest fetchXmlRequest)
        {
            // Sending the request as a batch to mitigate issues where FetchXml length exceeds the 
            // max length for a URI sent in the query. This way it will be sent in the body.
            BatchRequest batchRequest = new(BaseAddress)
            {
                SetRequests = new List<HttpRequestMessage> { fetchXmlRequest }
            };

            try
            {
                BatchResponse batchResponse = await SendAsync<BatchResponse>(batchRequest);

                HttpResponseMessage firstResponse = batchResponse.HttpResponseMessages.FirstOrDefault();

                FetchXmlResponse fetchXmlResponse = firstResponse.As<FetchXmlResponse>();

                return fetchXmlResponse;
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException(ex.Message);
            }



        }

        /// <summary>
        /// Retrieves a record.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityReference">A reference to the record to retrieve</param>
        /// <param name="query">The query string parameters</param>
        /// <param name="includeAnnotations">Whether to include annotations with the data.</param>
        /// <param name="eTag">The current ETag value to compare.</param>
        /// <returns></returns>
        public async Task<JObject> Retrieve(EntityReference entityReference, string query,
         bool includeAnnotations = false,
        string eTag = null,
        string partitionId = null)
        {
            RetrieveRequest request = new(
                entityReference: entityReference,
                query: query,
                includeAnnotations: includeAnnotations,
                eTag: eTag,
                partitionid: partitionId);

            try
            {
                RetrieveResponse response = await SendAsync<RetrieveResponse>(request);
                return response.Record;
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceException("Error retrieving the record.", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException("An unexpected error occurred.", ex);
            }
        }


        /// <summary>
        /// Retrieves the results of an OData query.
        /// </summary>
        /// <param name="service">The Service.</param>
        /// <param name="queryUri">An absolute or relative Uri.</param>
        /// <param name="maxPageSize">The maximum number of records to return in a page.</param>
        /// <param name="includeAnnotations">Whether to include annotations with the results.</param>
        /// <returns></returns>
        public async Task<RetrieveMultipleResponse> RetrieveMultiple(
            string queryUri,
            int? maxPageSize = null,
            bool includeAnnotations = false)
        {
            RetrieveMultipleRequest request = new(
                queryUri: queryUri,
                maxPageSize: maxPageSize,
                includeAnnotations: includeAnnotations);


            try
            {
                var response = await SendAsync<RetrieveMultipleResponse>(request);

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceException("Error retrieving the record.", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException("An unexpected error occurred.", ex);
            }
        }

        /// <summary>
        /// Returns an Access token for the app based on username and password from appsettings.json
        /// </summary>
        /// <returns>An Access token</returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> GetAccessToken()
        {

            var request = new HttpRequestMessage(HttpMethod.Post, config.TokenUrl)
            {
                Content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                    new("resource", @$"{config.Url}/api/data/v{config.Version}/"),
                     new("client_id", config.ClientId),
                     new("client_secret", config.ClientSecret),
                     new("username", config.UserName),
                     new("password", config.Password),
                     new("grant_type", "password")
            })
            };

            // Get the named HttpClient from the IHttpClientFactory
            var client = _httpClientFactory.CreateClient(WebAPIClientName);

            HttpResponseMessage response = await client.SendAsync(request);

            var responseMessage = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseMessage))
                throw new TokenFetchException("Failed to get access token.");

            var accessToken = JsonConvert.DeserializeObject<AccessToken>(responseMessage);

            if (accessToken != null)
            {
                accessToken.Expires = DateTime.UtcNow.AddSeconds(accessToken.ExpiresIn);
                return accessToken.Token;
            }
            throw new TokenFetchException("Could not fetch token");

        }

        /// <summary>
        /// The BaseAddress property of the WebAPI httpclient.
        /// </summary>
        public Uri BaseAddress { get; }


        ~Service() => Dispose(false);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _disposedValue = true;
            }
        }
    }
}