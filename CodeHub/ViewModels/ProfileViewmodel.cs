using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using Octokit;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using CodeHub.Models;
using HtmlAgilityPack;

namespace CodeHub.ViewModels
{
    public class ProfileViewmodel : AppViewmodel
    {
        public bool _isOrganization;
        public bool IsOrganization
        {
            get
            {
                return _isOrganization;
            }
            set
            {
                Set(() => IsOrganization, ref _isOrganization, value);
            }
        }

        public ObservableCollection<User> _followers;
        public ObservableCollection<User> Followers
        {
            get
            {
                return _followers;
            }
            set
            {
                Set(() => Followers, ref _followers, value);
            }
        }

        public ObservableCollection<User> _following;
        public ObservableCollection<User> Following
        {
            get
            {
                return _following;
            }
            set
            {
                Set(() => Following, ref _following, value);
            }
        }

        public ContributionsDataModel[,] _Data;

        public ContributionsDataModel[,] Data
        {
            get { return _Data; }
            set { Set(() => Data, ref _Data, value); }
        }

        public String[] _Months;

        public String[] Months
        {
            get { return _Months; }
            set { Set(() => Months, ref _Months, value); }
        }

        public async Task Load()
        {
            if (!GlobalHelper.IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); 
            }
            else
            {
                //Sending Internet available message to all viewModels
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType());

                isLoading = true;
               
                if (User != null)
                {
                    if (GlobalHelper.NewFollowActivity)
                    {
                        User = await UserUtility.getUserInfo(User.Login);
                        GlobalHelper.NewFollowActivity = false;
                    }

                    // Load the full HTML body
                    WebView view = new WebView();
                    TaskCompletionSource<String> tcs = new TaskCompletionSource<String>();
                    view.NavigationCompleted += (s, e) =>
                    {
                        view.InvokeScriptAsync("eval", new[] { "document.documentElement.outerHTML;" }).AsTask().ContinueWith(t =>
                        {
                            tcs.SetResult(t.Status == TaskStatus.RanToCompletion ? t.Result : null);
                        });
                    };

                    // Manually set the user agent to get the full desktop site
                    String userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; ARM; Trident/7.0; Touch; rv:11.0; WPDesktop) like Gecko";
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, 
                        new Uri(User.HtmlUrl));
                    httpRequestMessage.Headers.Append("User-Agent", userAgent);
                    view.NavigateWithHttpRequestMessage(httpRequestMessage);
                    String html = await tcs.Task;

                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(html);

                    HtmlNode
                        graph = document.DocumentNode
                            ?.Descendants("svg")
                            ?.FirstOrDefault(node => node.Attributes?.Any(att => att.Name?.Equals("class") == true &&
                                                                                 att.Value?.Equals("js-calendar-graph-svg") == true) == true),
                        root = graph?.Descendants("g")?.FirstOrDefault();
                    List<String> months = new List<String>();
                    Months = graph?.Descendants("text").Where(
                            node => node.Attributes.AttributesWithName("class")?.FirstOrDefault()?.Value?.Equals("month") == true)
                            .Select(node => node.InnerText).ToArray();
                    if (root != null)
                    {
                        int? test = root?.Descendants("g").Count();
                        ContributionsDataModel[,] data = new ContributionsDataModel[7, 53]; // 7 days per week by 53 weeks
                        int i = 0;
                        foreach (var week in root.Descendants("g"))
                        {
                            int y = 0;
                            foreach (var day in week.Descendants("rect"))
                            {
                                int count = int.Parse(day.Attributes.AttributesWithName("data-count").First().Value);
                                DateTime date = DateTime.Parse(day.Attributes.AttributesWithName("data-date").First().Value);
                                data[y, i] = new ContributionsDataModel(count, date);
                                y++;
                            }
                            i++;
                        }
                        Data = data;
                    }

                    // Load the user images
                    await TryLoadUserAvatarImagesAsync();

                    isLoggedin = true;
                    if (User.Type == AccountType.Organization)
                    {
                        IsOrganization = true;
                    }
                    else
                    {
                        if (User.Followers < 300 && User.Followers > 0)
                        {
                            Followers = await UserDataService.getAllFollowers(User.Login);
                        }

                        if (User.Following < 300 && User.Following > 0)
                        {
                            Following = await UserDataService.getAllFollowing(User.Login);
                        }
                    }
                }
                else
                {
                    isLoggedin = false;
                }
               
                isLoading = false;
            }
        }
        public void RecieveSignOutMessage(GlobalHelper.SignOutMessageType empty)
        {
            isLoggedin = false;
            User = null;
        }
        public void ProfileTapped(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(DeveloperProfileView), ((User)e.ClickedItem).Login, "Profile");
        }
        public void RecieveSignInMessage(User user)
        {
            isLoading = true;
            if (user != null)
            {
                isLoggedin = true;
                User = user;
            }
            isLoading = false;
        }
    }
}
