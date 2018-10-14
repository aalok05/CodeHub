using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
	public class MyOrganizationsViewmodel : AppViewmodel
	{
		public bool _ZeroOrganizations;
		/// <summary>
		/// 'No Organizations' TextBlock will display if this is true
		/// </summary>
		public bool ZeroOrganizations
		{
			get => _ZeroOrganizations;
			set => Set(() => ZeroOrganizations, ref _ZeroOrganizations, value);
		}

		public ObservableCollection<Organization> _Organizations;
		public ObservableCollection<Organization> Organizations
		{
			get => _Organizations;
			set => Set(() => Organizations, ref _Organizations, value);

		}

		public async Task Load()
		{
			if (GlobalHelper.IsInternet())
			{
				if (User != null)
				{
					IsLoggedin = true;
					IsLoading = true;
					if (Organizations == null)
					{
						Organizations = new ObservableCollection<Organization>();

						await LoadOrganizations();
					}
					IsLoading = false;
				}
				else
				{
					IsLoggedin = false;
				}
			}

		}
		public async void RefreshCommand(object sender, EventArgs e)
		{
			if (!GlobalHelper.IsInternet())
			{
				//Sending NoInternet message to all viewModels
				Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
			}
			else
			{
				IsLoading = true;
				if (User != null)
				{
					await LoadOrganizations();
				}
			}
			IsLoading = false;
		}
		public void OrganizationDetailNavigateCommand(object sender, ItemClickEventArgs e)
		{
			var languageLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

			SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(
					typeof(DeveloperProfileView), 
					(e.ClickedItem as Organization).Login, 
					languageLoader.GetString("pageTitle_OrganizationView")
				);
		}

		public void RecieveSignOutMessage(GlobalHelper.SignOutMessageType empty)
		{
			IsLoggedin = false;
			User = null;
			Organizations = null;
		}
		public async void RecieveSignInMessage(User user)
		{
			IsLoading = true;
			if (user != null)
			{
				IsLoggedin = true;
				User = user;
				await LoadOrganizations();
			}
			IsLoading = false;

		}
		private async Task LoadOrganizations()
		{
			var orgs = await UserService.GetAllOrganizations();
			if (orgs == null || orgs.Count == 0)
			{
				ZeroOrganizations = true;
				if (Organizations != null)
				{
					Organizations.Clear();
				}
			}
			else
			{
				ZeroOrganizations = false;
				Organizations = orgs;
			}
		}
	}
}
