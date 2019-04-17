

namespace EdgeAuth
{
    /// <summary>
    /// Contains the status of the authentication operation.
    /// </summary>
    public enum WebAuthenticationStatus
    {
        /// <summary>
        /// The operation succeeded, and the response data is available.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The operation was canceled by the user.
        /// </summary>
        UserCancel = 1,

        /// <summary>
        /// The operation failed because a specific HTTP error was returned, for example 404.
        /// </summary>
        ErrorHttp = 2,
    }
}
