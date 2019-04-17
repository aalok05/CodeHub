using CodeHub.Helpers;
using Octokit;
using System.Threading.Tasks;

namespace CodeHub.Services
{
	class OrganizationsUtility
	{
		public static async Task<Organization> GetOrganizationInfo(string login)
		{
			try
			{
				return await GlobalHelper.GithubClient.Organization.Get(login);
			}
			catch
			{
				return null;
			}
		}
	}
}
