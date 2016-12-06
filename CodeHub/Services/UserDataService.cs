using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using System.Collections.ObjectModel;

namespace CodeHub.Services
{
    /* This class deals with the Authenticated User's data, i.e. the user using the App.
     */
    class UserDataService
    {
        public static async Task<GitHubClient> getAuthenticatedClient()
        {
            try
            {
                var token = await AuthService.GetToken();
                GitHubClient client = new GitHubClient(new ProductHeaderValue("CodeHub"));
                if(token != null)
                {
                    client.Credentials = new Credentials(token);
                }
                return client;
            }
            catch
            {
                GitHubClient client = new GitHubClient(new ProductHeaderValue("CodeHub"));
                return client;
            }
            
        }
        public static async Task<User> getUserInfo()
        {
            try
            {
                var client = await getAuthenticatedClient();
                var user = await client.User.Current();
                return user;
            }
            catch
            {
                return null;
            }

        }

        /* If User's email is not publicly visible, the 'User' object returns null in email filed 
         * Hence we need a separate method in such case.
         */
        public static async Task<string> getUserEmail() 
        {
            try
            {
                var client = await getAuthenticatedClient();
                var result = await client.User.Email.GetAll();
                var s = result[0].Email.ToString();
                return s;
            }
            catch
            {
                return null;
            }
        }
        public static async Task<ObservableCollection<Repository>> getUserRepositories()
        {
            try
            {
                var client = await getAuthenticatedClient();
                ObservableCollection<Repository> repos = new ObservableCollection<Repository>();
                var repo = await client.Repository.GetAllForCurrent();
                foreach (Repository r in repo)
                { 
                    repos.Add(r);
                }
                return repos;
            }
            catch
            {
                return null;
            }
        }
        public static async Task<ObservableCollection<Repository>> getStarredRepositories()
        {
            try
            {
                var client = await getAuthenticatedClient();
                ObservableCollection<Repository> repos = new ObservableCollection<Repository>();
                var repo = await client.Activity.Starring.GetAllForCurrent();
                foreach (Repository r in repo)
                {  
                    repos.Add(r);
                }
                

                return repos;
            }
            catch
            {
                return null;
            }
        }
        public static async Task<ObservableCollection<User>> getAllFollowers(string login)
        {
            try
            {
                ObservableCollection<User> followers = new ObservableCollection<User>();
                var client = await getAuthenticatedClient();

                // return first 100 items
                var firstOneHundred = new ApiOptions
                { 
                    PageSize = 100,
                    PageCount = 1
                };
                var result = await client.User.Followers.GetAll(login, firstOneHundred);
                foreach (User r in result)
                {   
                    followers.Add(r);
                    if (followers.Count > 99)
                        break;
                }
                return followers;
            }
            catch
            {
                return null;
            }
        }
        public static async Task<ObservableCollection<User>> getAllFollowing(string login)
        {
            try
            {
                ObservableCollection<User> following = new ObservableCollection<User>();
                var client = await getAuthenticatedClient();

                // return first 100 items
                var firstOneHundred = new ApiOptions
                {
                    PageSize = 100,
                    PageCount = 1
                };
                var result = await client.User.Followers.GetAllFollowing(login, firstOneHundred);
                foreach (User r in result)
                {
                    following.Add(r);
                }
                return following;
            }
            catch
            {
                return null;
            }
        }
    }
}
