using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.DataAccess.CDSWebApi.Messages
{
    public class MergeRequest : HttpRequestMessage
    {
        public MergeRequest(JObject target, JObject subOrdinate, JObject updateContent = null, string partitionId = null)
        {
            Method = HttpMethod.Post;
            RequestUri = new Uri(
                uriString: "Merge",
                uriKind: UriKind.Relative);
            Content = new StringContent(
                        content: new JObject()
                        {
                            ["Target"] = target.ToString(),
                            ["Subordinate"] = subOrdinate.ToString(),
                        }.ToString(),
                        encoding: Encoding.UTF8,
                        mediaType: "application/json");
        }
    }
}
