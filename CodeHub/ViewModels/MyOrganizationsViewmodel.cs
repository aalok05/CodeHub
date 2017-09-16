using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
            get
            {
                return _ZeroOrganizations;
            }
            set
            {
                Set(() => ZeroOrganizations, ref _ZeroOrganizations, value);
            }
        }

        public ObservableCollection<Organization> _Organizations;
        public ObservableCollection<Organization> Organizations
        {
            get
            {
                return _Organizations;
            }
            set
            {
                Set(() => Organizations, ref _Organizations, value);
            }

        }

        public async Task Load()
        {
            if (!GlobalHelper.IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
            }
            else
            {
                if (User != null)
                {
                    isLoggedin = true;
                    isLoading = true;
                    if (Organizations == null)
                    {
                        Organizations = new ObservableCollection<Organization>();
                        
                        await LoadOrganizations();
                    }
                    isLoading = false;
                }
                else
                {
                    isLoggedin = false;
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
                isLoading = true;
                if (User != null)
                {
                    await LoadOrganizations();
                }
            }
            isLoading = false;
        }
        public void OrganizationDetailNavigateCommand(object sender, ItemClickEventArgs e)
        {
            var languageLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), languageLoader.GetString("pageTitle_OrganizationView"), (e.ClickedItem as Organization).Login);
        }

        public void RecieveSignOutMessage(GlobalHelper.SignOutMessageType empty)
        {
            isLoggedin = false;
            User = null;
            Organizations = null;
        }
        public async void RecieveSignInMessage(User user)
        {
            isLoading = true;
            if (user != null)
            {
                isLoggedin = true;
                User = user;
                await LoadOrganizations();
            }
            isLoading = false;

        }
        private async Task LoadOrganizations()
        {
            var orgs = await UserUtility.GetAllOrganizations();
            if (orgs.Count == 0 || orgs == null)
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
