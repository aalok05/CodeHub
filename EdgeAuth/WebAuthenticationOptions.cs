using System;

namespace EdgeAuth
{
    /// <summary>
    /// Contains the options available to the asynchronous operation.
    /// </summary>
    [Flags]
    public enum WebAuthenticationOptions
    {
        /// <summary>
        /// No options are requested.
        /// </summary>
        None = 0,

        /// <summary>
        /// Tells the web authentication broker to not render any UI. 
        /// This option will throw an exception if used with AuthenticateAndContinue; AuthenticateSilentlyAsync, which includes this option implicitly, should be used instead.
        /// </summary>
        SilentMode = 1,

        /// <summary>
        /// Tells the web authentication broker to return the window title string of the webpage in the ResponseData property.
        /// </summary>
        UseTitle = 2,

        /// <summary>
        /// Tells the web authentication broker to return the body of the HTTP POST in the ResponseData property. 
        /// For use with single sign-on (SSO) only.
        /// </summary>
        UseHttpPost = 4,

        /// <summary>
        /// Tells the web authentication broker to render the webpage in an app container that supports privateNetworkClientServer, enterpriseAuthentication, and sharedUserCertificate capabilities. 
        /// Note the application that uses this flag must have these capabilities as well.
        /// </summary>
        UseCorporateNetwork = 8,
    }
}
