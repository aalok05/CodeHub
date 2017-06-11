using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using System.Collections.ObjectModel;

namespace CodeHub.Services
{
    class GistUtility
    {
        /// <summary>
        /// Gets all gists for a user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static async Task<ObservableCollection<Gist>> GetGistsForUser(string login)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var gists = await client.Gist.GetAllForUser(login);
                return new ObservableCollection<Gist>(new List<Gist>(gists));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a gist
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static async Task<Gist> CreateGist(NewGist newGist)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                return await client.Gist.Create(newGist);
            }
            catch
            {
                return null;
            }
        }
    }
}
