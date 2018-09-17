using JetBrains.Annotations;
using System;
using Windows.Web.Http;

namespace CodeHub.Models
{
	/// <summary>
	/// A struct that wraps a web request result
	/// </summary>
	/// <typeparam name="T">The expected return type</typeparam>
	public struct WrappedWebResult<T> : IEquatable<WrappedWebResult<T>>, IEquatable<T> where T : class
	{
		/// <summary>
		/// Gets the available result
		/// </summary>
		[CanBeNull]
		public T Result { get; }

		/// <summary>
		/// Gets the resulting status for this request
		/// </summary>
		public WebRequestStatus Status { get; }

		// Private constructor
		private WrappedWebResult([CanBeNull] T result, WebRequestStatus status)
		{
			Result = result;
			Status = status;
		}

		/// <summary>
		/// Calls the default Equals method for the inner result of the wrapped instance
		/// </summary>
		/// <param name="other">The other instance to compare</param>
		public bool Equals(WrappedWebResult<T> other) 
			=> Equals(other.Result);

		/// <summary>
		/// Calls the default Equals method for another value of the same type as the result of this instance
		/// </summary>
		/// <param name="other">The other value to compare</param>
		public bool Equals([CanBeNull] T other) 
			=> (Result == null && other == null) || Result?.Equals(other) == true;

		// Implicit cast for the inner result
		public static implicit operator T(WrappedWebResult<T> wrappedResult) 
			=> wrappedResult.Result;

		// Implicit converter for successful results
		public static implicit operator WrappedWebResult<T>([NotNull] T result) 
			=> new WrappedWebResult<T>(result, WebRequestStatus.RequestCompleted);

		// Implicit converters for faulted results
		public static implicit operator WrappedWebResult<T>(WebRequestStatus status) 
			=> new WrappedWebResult<T>(null, status);
	}

	/// <summary>
	/// Indicates the status of a completed web operation
	/// </summary>
	public enum WebRequestStatus
	{
		RequestCompleted,
		OperationCanceled,
		UnknownError
	}

	/// <summary>
	/// A struct that wraps an HTTP web request result
	/// </summary>
	/// <typeparam name="T">The expected return type</typeparam>
	public struct WrappedHTTPWebResult<T> : IEquatable<WrappedHTTPWebResult<T>>, IEquatable<T> where T : class
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
		private WrappedHTTPWebResult([CanBeNull] T result, HttpStatusCode? status, [CanBeNull] Exception e)
		{
			Result = result;
			StatusCode = status;
			RequestException = e;
		}

		/// <summary>
		/// Calls the default Equals method for the inner result of the wrapped instance
		/// </summary>
		/// <param name="other">The other instance to compare</param>
		public bool Equals(WrappedHTTPWebResult<T> other) 
			=> Equals(other.Result);

		/// <summary>
		/// Calls the default Equals method for another value of the same type as the result of this instance
		/// </summary>
		/// <param name="other">The other value to compare</param>
		public bool Equals([CanBeNull] T other) 
			=> (Result == null && other == null) || Result?.Equals(other) == true;

		// Implicit cast for the inner result
		public static implicit operator T(WrappedHTTPWebResult<T> wrappedResult) 
			=> wrappedResult.Result;

		// Implicit converter for successful results
		public static implicit operator WrappedHTTPWebResult<T>([CanBeNull] T result) 
			=> new WrappedHTTPWebResult<T>(result, HttpStatusCode.Ok, null);

		// Implicit converters for faulted results
		public static implicit operator WrappedHTTPWebResult<T>(HttpStatusCode status) 
			=> new WrappedHTTPWebResult<T>(null, status, null);

		// Failed web call
		public static implicit operator WrappedHTTPWebResult<T>([NotNull] Exception e) 
			=> new WrappedHTTPWebResult<T>(null, null, e);
	}
}