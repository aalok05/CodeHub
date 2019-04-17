using CodeHub.Models;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeHub.Services
{
	public class AccountsService
	{
		private const string SETTINGS_FILENAME = "Settings.json";

		/// <summary>
		/// Get all available accounts
		/// </summary>
		/// <returns></returns>
		public static async Task<ObservableCollection<Account>> GetAllUsers()
		{
			try
			{
				var sf = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILENAME);
				if (sf == null)
				{
					return null;
				}

				var content = await FileIO.ReadTextAsync(sf);
				return JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Adds an account to list of available accounts
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static async Task<bool> AddUser(Account user)
		{
			try
			{
				//Try to create a Settings.json file, fail if it already exists
				try { await ApplicationData.Current.LocalFolder.CreateFileAsync(SETTINGS_FILENAME, CreationCollisionOption.FailIfExists); }
				catch { }

				var file = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILENAME);
				var content = await FileIO.ReadTextAsync(file);
				var allUsers = JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
				if (allUsers != null)
				{
					var sameUser = allUsers.Where(x => x.Id == user.Id);
					if (sameUser.Count() == 0)
					{
						//mark all existing users as inactive and add new active user
						foreach (var u in allUsers)
						{
							u.IsActive = false;
						}
						allUsers.Add(user);
						await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(allUsers));

					}
					else
					{
						//The user already exists
						allUsers.Where(x => x.Id == user.Id).First().IsActive = true;
						await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(allUsers));
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

		/// <summary>
		/// Deletes an account from list of available accounts
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static async Task<bool> RemoveUser(string userId)
		{
			try
			{
				var sf = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILENAME);
				if (sf == null)
				{
					return false;
				}

				var content = await FileIO.ReadTextAsync(sf);
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
		public static async Task<bool> SignOutOfAccount(string userId)
		{
			try
			{
				var sf = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILENAME);
				if (sf == null)
				{
					return false;
				}

				var content = await FileIO.ReadTextAsync(sf);
				var users = JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
				users.Where(x => x.Id.ToString() == userId).First().IsActive = false;
				await FileIO.WriteTextAsync(sf, JsonConvert.SerializeObject(users));
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Marks an account as active
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static async Task<bool> MakeAccountActive(string userId)
		{
			try
			{
				var sf = await ApplicationData.Current.LocalFolder.GetFileAsync(SETTINGS_FILENAME);
				if (sf == null)
				{
					return false;
				}

				var content = await FileIO.ReadTextAsync(sf);
				var users = JsonConvert.DeserializeObject<ObservableCollection<Account>>(content);
				foreach (var u in users)
				{
					if (u.Id.ToString() == userId)
					{
						u.IsActive = true;
					}
					else
					{
						u.IsActive = false;
					}
				}
				await FileIO.WriteTextAsync(sf, JsonConvert.SerializeObject(users));
				return true;
			}
			catch
			{ return false; }
		}
	}
}
