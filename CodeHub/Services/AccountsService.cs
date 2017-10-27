using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Collections.ObjectModel;
using CodeHub.Models;

namespace CodeHub.Services
{
    public class AccountsService
    {
        private const string SETTINGS_FILENAME = "Settings.json";

        /// <summary>
        /// Get all authenticated GitHub users
        /// </summary>
        /// <returns></returns>
        public async static Task<ObservableCollection<Account>> GetAllUsers()
        {
            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILENAME);
                if (sf == null) return null;

                string content = await FileIO.ReadTextAsync(sf);
                return JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
            }
            catch
            { return null; }
        }

        /// <summary>
        /// Add a GitHub user to list of authenticated users
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async static Task<bool> AddUser(Account user)
        {
            try
            {
                try
                {
                    await ApplicationData.Current.LocalFolder.CreateFileAsync(SETTINGS_FILENAME, CreationCollisionOption.FailIfExists);
                }
                catch{ }
                
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILENAME);
                string content = await FileIO.ReadTextAsync(file);
                ObservableCollection<Account> allUsers = JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
                if(allUsers != null)
                {
                    var sameUser = allUsers.Where(x => x.Id == user.Id);
                    if (sameUser.Count() == 0)
                    {
                        allUsers.Add(user);
                        await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(allUsers));

                    }
                    else
                    {
                        sameUser.First().IsActive = true;
                    }
                }
                else
                {
                    await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(new ObservableCollection<Account> { user }));
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public async static Task<bool> RemoveUser(string userId)
        {
            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILENAME);
                if (sf == null) return false;

                string content = await FileIO.ReadTextAsync(sf);
                var users = JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
                users.Remove(users.Where(x => x.Id.ToString() == userId).First());
                await FileIO.WriteTextAsync(sf, JsonConvert.SerializeObject(users));
                return true;
            }
            catch
            { return false; }
        }

        /// <summary>
        /// Marks an account as Inactive
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async static Task<bool> SignOutOfAccount(string userId)
        {
            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILENAME);
                if (sf == null) return false;

                string content = await FileIO.ReadTextAsync(sf);
                var users = JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
                (users.Where(x => x.Id.ToString() == userId).First()).IsActive = false;
                await FileIO.WriteTextAsync(sf, JsonConvert.SerializeObject(users));
                return true;
            }
            catch
            { return false; }
        }
    }
}
