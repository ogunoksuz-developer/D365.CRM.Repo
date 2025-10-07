using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace PowerApps.Samples.Messages
{
    // This class must be instantiated by either:
    // - The Service.SendAsync<T> method
    // - The HttpResponseMessage.As<T> extension in Extensions.cs

    /// <summary>
    /// Contains the response from RetrieveSharedPrincipalsAndAccessRequest
    /// </summary>
    public sealed class RetrieveSharedPrincipalsAndAccessResponse : HttpResponseMessage
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

        public List<PrincipalAccess> PrincipalAccesses => JsonConvert.DeserializeObject<List<PrincipalAccess>>(_jObject[nameof(PrincipalAccesses)].ToString());
    }
}
