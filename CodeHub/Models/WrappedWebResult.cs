using System;
using Windows.Web.Http;

namespace CodeHub.Models
{
    /// <summary>
    /// A struct that wraps a web request result
    /// </summary>
    /// <typeparam name="T">The expected return type</typeparam>
    public struct WrappedWebResult<T> where T : class
    {
        /// <summary>
        /// Gets the available result
        /// </summary>
        public T Result { get; }

        /// <summary>
        /// Gets the status code, if available
        /// </summary>
        public HttpStatusCode? StatusCode { get; }

        /// <summary>
        /// Gets the exception generated during the request, if present
        /// </summary>
        public Exception RequestException { get; }

        // Private constructor
        private WrappedWebResult(T result, HttpStatusCode? status, Exception e = null)
        {
            Result = result;
            StatusCode = status;
            RequestException = e;
        }

        // Implicit converter for successful results
        public static implicit operator WrappedWebResult<T>(T result) => new WrappedWebResult<T>(result, HttpStatusCode.Ok);

        // Implicit converters for faulted results
        public static implicit operator WrappedWebResult<T>(HttpStatusCode status) => new WrappedWebResult<T>(null, status);

        // Failed web call
        public static implicit operator WrappedWebResult<T>(Exception e) => new WrappedWebResult<T>(null, null, e);
    }
}