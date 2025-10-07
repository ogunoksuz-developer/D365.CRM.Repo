using System;
using System.Collections.Generic;
using System.Net.Http;

namespace PowerApps.Samples.Batch
{
    public class ChangeSet
    {
        private List<HttpRequestMessage> _requests = new();

        public ChangeSet(List<HttpRequestMessage> requests)
        {
            Requests = requests;
        }

        /// <summary>
        /// Sets Requests to send with the change set
        /// </summary>
        public List<HttpRequestMessage> Requests
        {
            set
            {
                _requests = new List<HttpRequestMessage>();
                foreach (var request in value)
                {
                    if (request.Method == HttpMethod.Get)
                    {
                        throw new ArgumentException("ChangeSets cannot contain GET requests.");
                    }
                    _requests.Add(request);
                }
            }
            get { return _requests; }
        }

    }
}
