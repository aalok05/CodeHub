using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using CodeHub.Services;
using System.Collections.ObjectModel;
using CodeHub.Helpers;

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
