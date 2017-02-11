using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using CodeHub.Services;
using System.Collections.ObjectModel;

namespace CodeHub.Services
{
    class OrganizationsUtility
    {
        public static async Task<Organization> GetOrganizationInfo(string login)
        {
            try
            {
                GitHubClient client = await UserUtility.GetAuthenticatedClient();
                return await client.Organization.Get(login);
            }
            catch
            {
                return null;
            }
        }
    }
}
