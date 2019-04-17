using CodeHub.Helpers;
using Octokit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
				var gists = await GlobalHelper.GithubClient.Gist.GetAllForUser(login);
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
				return await GlobalHelper.GithubClient.Gist.Create(newGist);
			}
			catch
			{
				return null;
			}
		}
	}
}
