using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Types;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from the RetrieveTotalRecordCountRequest 
    /// </summary>
    public sealed class RetrieveTotalRecordCountResponse : HttpResponseMessage
    {
        // Cache the async content
        private string? _content;

        //Provides JObject for property getters
        private JObject _jObject
        {
            get
            {
                _content ??= Content.ReadAsStringAsync().GetAwaiter().GetResult();

                return JObject.Parse(_content);
            }
        }

        /// <summary>
        /// Gets the collection of results for the RetrieveTotalRecordCount Function.
        /// </summary>
        public EntityRecordCountCollection EntityRecordCountCollection
        {
            get
            {
                return JsonConvert.DeserializeObject<EntityRecordCountCollection>(_jObject["EntityRecordCountCollection"].ToString());
            }
        }
    }
}
