using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.Views;
using Octokit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static CodeHub.Helpers.GlobalHelper;
using Windows.Foundation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;

namespace CodeHub.ViewModels
{
    public class MainViewmodel : AppViewmodel 
    {
        #region properties
        public string _headerText;
        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                Set(() => HeaderText, ref _headerText, value);
            }
        }

        public string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                Set(() => Email, ref _email, value);
            }
        }


        public bool _isStartLoading;
        public bool isStartLoading
        {
            get
            {
                return _isStartLoading;
            }
            set
            {
                Set(() => isStartLoading, ref _isStartLoading, value);
            }
        }

        public ObservableCollection<HamItem> _HamItems = new ObservableCollection<HamItem>();
        public ObservableCollection<HamItem> HamItems
        {

            get { return _HamItems; }
            set
            {
                Set(() => HamItems, ref _HamItems, value);
            }
        }

        public string _busyScreenText;
        public string BusyScreenText
        {
            get { return _busyScreenText; }
            set
            {
                Set(() => BusyScreenText, ref _busyScreenText, value);
            }
        }
        #endregion
        public MainViewmodel()
        {
            /*SVG geometry data for icons, which will be loaded by XamlReader*/
            var trendingSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">F0 M12,16z M0,0z M5.05,0.31C5.86,2.48 5.46,3.69 4.53,4.62 3.55,5.67 1.98,6.45 0.9,7.98 -0.55,10.03 -0.8,14.51 4.43,15.68 2.23,14.52 1.76,11.16 4.13,9.07 3.52,11.1 4.66,12.4 6.07,11.93 7.46,11.46 8.37,12.46 8.34,13.6 8.32,14.38 8.03,15.04 7.21,15.41 10.63,14.82 11.99,11.99 11.99,9.85 11.99,7.01 9.46,6.63 10.74,4.24 9.22,4.37 8.71,5.37 8.85,6.99 8.94,8.07 7.83,8.79 6.99,8.32 6.32,7.91 6.33,7.13 6.93,6.54 8.18,5.31 8.68,2.45 5.05,0.32L5.03,0.3 5.05,0.31z</Geometry>";
            var profileSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">F0 M12,16z M0,0z M12,14.002A0.998,0.998,0,0,1,11.002,15L1.001,15A1,1,0,0,1,0,13.999L0,13C0,10.367 4,9 4,9 4,9 4.229,8.591 4,8 3.159,7.38 3.056,6.41 3,4 3.173,1.587 4.867,1 6,1 7.133,1 8.827,1.586 9,4 8.944,6.41 8.841,7.38 8,8 7.771,8.59 8,9 8,9 8,9 12,10.367 12,13L12,14.002z</Geometry>";
            var myRepoSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">F0 M12,16z M0,0z M4,9L3,9 3,8 4,8 4,9z M4,6L3,6 3,7 4,7 4,6z M4,4L3,4 3,5 4,5 4,4z M4,2L3,2 3,3 4,3 4,2z M12,1L12,13C12,13.55,11.55,14,11,14L6,14 6,16 4.5,14.5 3,16 3,14 1,14C0.45,14,0,13.55,0,13L0,1C0,0.45,0.45,0,1,0L11,0C11.55,0,12,0.45,12,1z M11,11L1,11 1,13 3,13 3,12 6,12 6,13 11,13 11,11z M11,1L2,1 2,10 11,10 11,1z</Geometry>";

            HamItems = new ObservableCollection<HamItem>(){
                            new HamItem() { Label = "Trending",
                                            Symbol = (Geometry)XamlReader.Load(trendingSymbol),
                                            DestPage = typeof(HomeView) },
                            new HamItem() { Label = "Profile",
                                            Symbol =(Geometry)XamlReader.Load(profileSymbol),
                                            DestPage =typeof(ProfileView) },
                            new HamItem() { Label = "My Repositories",
                                            Symbol =(Geometry)XamlReader.Load(myRepoSymbol),
                                            DestPage =typeof(MyReposView) },
                };

            HeaderText = HamItems[0].Label.ToUpper();
            HamItems[0].IsSelected = true;

        }
        #region commands
        private RelayCommand _signInCommand;
        public RelayCommand SignInCommand
        {
            get
            {
                return _signInCommand
                    ?? (_signInCommand = new RelayCommand(
                                          async () =>
                                          {
                                              AuthService service = new AuthService();
                                              BusyScreenText = "Signing in...";
                                              isLoading = true;

                                              if (await service.Authenticate())
                                              { 
                                                  var user = await UserDataService.getUserInfo();
                                                  if (user != null)
                                                  {
                                                      GlobalHelper.UserLogin = user.Login;
                                                      isLoggedin = true;
                                                      Messenger.Default.Send<User>(user); //Sending Sign in message to all viewModels
                                                      User = user;
                                                      if (user.Email == null)
                                                      {
                                                          /*  If User's email is not visible publicly, the email field will return null
                                                           *  In this case we have to get the email separately
                                                           */
                                                           this.Email = await UserDataService.getUserEmail();
                                                      }
                                                      else
                                                      {
                                                          this.Email = user.Email;
                                                      }
                                                  }
                                              }
                                              isLoading = false;
                                             
                                          }));
            }
        }

        private RelayCommand _signOutCommand;
        public RelayCommand SignOutCommand
        {
            get
            {
                return _signOutCommand
                    ?? (_signOutCommand = new RelayCommand(
                                          async () =>
                                          {
                                              BusyScreenText = "Signing out...";
                                              isLoading = true;

                                              if (await AuthService.signOut())
                                              {
                                                  isLoggedin = false;
                                                  User = null;

                                                  //Sending Sign Out message to all viewModels
                                                  Messenger.Default.Send<GlobalHelper.SignOutMessageType>(new GlobalHelper.SignOutMessageType());
                                                  HamItemClicked(HamItems[0]);
                                              }
                                              isLoading = false;
                                          }));
            }
        }

        public RelayCommand _loadCommand;
        public RelayCommand LoadCommand
        {
            get
            {
                return _loadCommand
                    ?? (_loadCommand = new RelayCommand(
                                          async () =>
                                          {
                                              if (await AuthService.checkAuth())
                                              {
                                                  isLoggedin = true;
                                              }
                                              if (!GlobalHelper.IsInternet())
                                              {
                                                  HasInternet = false;
                                              }
                                              else
                                              {
                                                  HasInternet = true;
                                                  isStartLoading = true;
                                                  if (isLoggedin == true)
                                                  {
                                                      var user = await UserDataService.getUserInfo();

                                                      if (user != null)
                                                      {
                                                          User = user;
                                                          GlobalHelper.UserLogin = user.Login;
                                                          Messenger.Default.Send<User>(user); //Sending Sign in message to all viewModels
                                                        
                                                          if (user.Email == null)
                                                          {
                                                              /*  If User's email is not visible publicly, the email field will return null
                                                              *   In this case we have to get the email separately
                                                              */
                                                              this.Email = await UserDataService.getUserEmail();
                                                          }
                                                          else
                                                          {
                                                              this.Email = user.Email;
                                                          }
                                                      }
                                                  }
                                                  isStartLoading = false;

                                              }
                                          }));
            }
        }

        public RelayCommand _closeInternetPopup;
        public RelayCommand CloseInternetPopup
        {
            get
            {
                return _closeInternetPopup
                   ?? (_closeInternetPopup = new RelayCommand(() =>
                   {
                       HasInternet = true;
                   }));
            }
        }

        #endregion

        #region events and methods
        public void NavigateToSettings()
        {
            foreach (var i in HamItems)
            {
                i.IsSelected = false;
            }
            Navigate(typeof(SettingsView));
        }
        public void NavigateToSearch()
        {
            foreach (var i in HamItems)
            {
                i.IsSelected = false;
            }
            Navigate(typeof(SearchView));
        }
        public void HamItemClicked(HamItem item)
        {
            foreach(var i in HamItems)
            {
                i.IsSelected = false;
            }
            item.IsSelected = true;
            Navigate(item.DestPage);
        }
        public void RecieveNoInternetMessage(NoInternetMessageType empty)
        {
            HasInternet = false;
        }
        public void RecieveInternetMessage(HasInternetMessageType empty)
        {
            HasInternet = true;
            if (isLoggedin == false)
            {
                LoadCommand.Execute(null);
            }
        }
        public void setHeadertext(string pageName)
        {
            HeaderText = pageName.ToUpper();
        }

        public void mainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (var i in HamItems)
            {
                if (e.SourcePageType == i.DestPage)
                {
                    i.IsSelected = true;
                }
                else
                {
                    i.IsSelected = false;
                }
            }
        }
        #endregion
    }
}
