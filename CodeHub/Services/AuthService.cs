using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Storage;

namespace CodeHub.Services
{
    class AuthService
    {
        #region App Creds
        GitHubClient client = new GitHubClient(new ProductHeaderValue("CodeHub"));
        Uri endUri = new Uri("http://example.com/path");
        static string TOKEN_FILE_NAME = "CodeHubToken93dc7.dat";
        #endregion

        /// <summary>
        /// Opens OAuth window using WebAuthenticationBroker class and returns true is authentication is successful
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Authenticate()
        {
            try
            {
                string clientId = await AppCredentials.getAppKey();
                OauthLoginRequest request = new OauthLoginRequest(clientId)
                {
                    Scopes = { "user", "repo" },
                };

                Uri oauthLoginUrl = client.Oauth.GetGitHubLoginUrl(request);

                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                           WebAuthenticationOptions.None,
                                                           oauthLoginUrl,
                                                           endUri
                                                           );
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var response = WebAuthenticationResult.ResponseData;

                    var result = await Authorize(response);

                    if (result)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                    return false;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Makes call for Access Token
        /// </summary>
        /// <param name="response">Response string containing 'code' token, used for getting access token</param>
        /// <returns></returns>
        private async Task<bool> Authorize(string response)
        {
            try
            {
                string responseData = response.Substring(response.IndexOf("code"));
                String[] keyValPairs = responseData.Split('=');
                string code = keyValPairs[1];

                string clientId = await AppCredentials.getAppKey();
                string appSecret = await AppCredentials.getAppSecret();

                var request = new OauthTokenRequest(clientId, appSecret, code);
                var token = await client.Oauth.CreateAccessToken(request);
                if (token != null)
                {
                    client.Credentials = new Credentials(token.AccessToken);
                    await SaveToken(token.AccessToken);
                }
                return true;
            }
            catch
            {
                return false;
            }



        }
    
        /// <summary>
        /// Saves access token in device
        /// </summary>
        /// <param name="token">Access token</param>
        /// <returns></returns>
        private async Task<bool> SaveToken(string token)
        {
            try
            {
                StorageFile savedFile = await ApplicationData.Current.LocalFolder.
                  CreateFileAsync(TOKEN_FILE_NAME, CreationCollisionOption.ReplaceExisting);

                using (Stream writeStream = await savedFile.OpenStreamForWriteAsync())
                {
                    DataContractSerializer serializer =
                        new DataContractSerializer(typeof(string));

                    serializer.WriteObject(writeStream, token);
                    await writeStream.FlushAsync();
                    writeStream.Dispose();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets Access token if stored in device
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetToken()
        {
            try
            {
                var readStream =
                await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(TOKEN_FILE_NAME);
                if (readStream == null)
                {
                    return null;
                }
                DataContractSerializer serializer =
                    new DataContractSerializer(typeof(string));

                var token = serializer.ReadObject(readStream).ToString();
                return token;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if user's device has an access token
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> checkAuth()
        {
            try
            {
                var token = await GetToken();
                    if (token != null)
                        return true;
                    else
                        return false;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Deletes the access token from user's device
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> signOut()
        {
            try
            {
                StorageFile savedFile = await ApplicationData.Current.LocalFolder.GetFileAsync(TOKEN_FILE_NAME);

                if (savedFile != null)
                {
                    await savedFile.DeleteAsync();
                    return true;
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

    }
}
