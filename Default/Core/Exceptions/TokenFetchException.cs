using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.Exceptions
{
    public class TokenFetchException : Exception
    {
        public TokenFetchException() : base() { }

        public TokenFetchException(string message) : base(message) { }

        public TokenFetchException(string message, Exception innerException) : base(message, innerException) { }
    }
}

