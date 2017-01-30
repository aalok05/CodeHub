using System;
using Windows.Web.Http;
using JetBrains.Annotations;

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
        [CanBeNull]
        public T Result { get; }

        /// <summary>
        /// Gets the status code, if available
        /// </summary>
        public HttpStatusCode? StatusCode { get; }

        /// <summary>
        /// Gets the exception generated during the request, if present
        /// </summary>
        [CanBeNull]
        public Exception RequestException { get; }

        // Private constructor
        private WrappedWebResult([CanBeNull] T result, HttpStatusCode? status, [CanBeNull] Exception e)
        {
            Result = result;
            StatusCode = status;
            RequestException = e;
        }

        // Implicit converter for successful results
        public static implicit operator WrappedWebResult<T>([NotNull] T result) => new WrappedWebResult<T>(result, HttpStatusCode.Ok, null);

        // Implicit converters for faulted results
        public static implicit operator WrappedWebResult<T>(HttpStatusCode status) => new WrappedWebResult<T>(null, status, null);

        // Failed web call
        public static implicit operator WrappedWebResult<T>([NotNull] Exception e) => new WrappedWebResult<T>(null, null, e);
    }
}