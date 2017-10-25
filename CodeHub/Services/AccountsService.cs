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
        private static StorageFolder _settingsFolder = ApplicationData.Current.LocalFolder;

        /// <summary>
        /// Get all authenticated GitHub users
        /// </summary>
        /// <returns></returns>
        public async static Task<ObservableCollection<Account>> GetAllUsers()
        {
            try
            {
                StorageFile sf = await _settingsFolder.GetFileAsync(SETTINGS_FILENAME);
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
                StorageFile file = await _settingsFolder.CreateFileAsync(SETTINGS_FILENAME, CreationCollisionOption.ReplaceExisting);
                string content = await FileIO.ReadTextAsync(file);
                ObservableCollection<Account> allUsers = JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
                allUsers.Add(user);
                await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(allUsers));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
