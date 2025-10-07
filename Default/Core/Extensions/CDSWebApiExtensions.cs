using LCW.Core.DataAccess.CDSWebApi;
using PowerApps.Samples.Batch;
using PowerApps.Samples.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LCW.Core.Extensions
{
    /// <summary>
    /// Contains extension methods to clone HttpRequestMessage and HttpContent types.
    /// </summary>
    public static class CdsWebApiExtensions
    {

        /// <summary>
        /// Clones a HttpRequestMessage instance
        /// </summary>
        /// <param name="request">The HttpRequestMessage to clone.</param>
        /// <returns>A copy of the HttpRequestMessage</returns>
        public static HttpRequestMessage Clone(this HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = request.Content.Clone(),
                Version = request.Version
            };

            foreach (KeyValuePair<string, object> prop in request.Options)
            {
                clone.Options.Set(new HttpRequestOptionsKey<object>(prop.Key), prop.Value);
            }

            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }
        /// <summary>
        /// Clones a HttpContent instance
        /// </summary>
        /// <param name="content">The HttpContent to clone</param>
        /// <returns>A copy of the HttpContent</returns>
        public static HttpContent Clone(this HttpContent content)
        {
            if (content == null) return null;

            HttpContent clone;

            switch (content)
            {
                case StringContent sc:
                    clone = new StringContent(sc.ReadAsStringAsync().Result);
                    break;
                default:
                    throw new UnsupportedContentTypeException(content.GetType());
            }

            clone.Headers.Clear();
            foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
            {
                clone.Headers.Add(header.Key, header.Value);
            }

            return clone;
        }



        /// <summary>
        /// Converts HttpResponseMessage to derived type
        /// </summary>
        /// <typeparam name="T">The type derived from HttpResponseMessage</typeparam>
        /// <param name="response">The HttpResponseMessage</param>
        /// <returns></returns>
        public static T As<T>(this HttpResponseMessage response) where T : HttpResponseMessage
        {
            T typedResponse = (T)Activator.CreateInstance(typeof(T));

            //Copy the properties
            typedResponse.StatusCode = response.StatusCode;
            response.Headers.ToList().ForEach(h => {
                typedResponse.Headers.TryAddWithoutValidation(h.Key, h.Value);
            });
            typedResponse.Content = response.Content;
            return typedResponse;
        }
    }
}
