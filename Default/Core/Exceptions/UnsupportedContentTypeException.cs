using System;

namespace LCW.Core.Extensions
{
    public class UnsupportedContentTypeException : Exception
    {
        public UnsupportedContentTypeException(Type contentType)
            : base($"Content type '{contentType}' is not implemented for HttpContent.Clone extension method.")
        {
        }
    }
}
