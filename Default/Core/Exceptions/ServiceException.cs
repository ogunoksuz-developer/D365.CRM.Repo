using PowerApps.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.Exceptions
{
    /// <summary>
    /// An exception that captures data returned by the Web API
    /// </summary>
    public class ServiceException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public string ReasonPhrase { get; set; }

        public ODataError ODataError { get; set; }

        public string Content { get; set; }

        public string RequestId { get; set; }

        public ServiceException()
        {
        }

        public ServiceException(string message)
            : base(message)
        {
        }

        public ServiceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
