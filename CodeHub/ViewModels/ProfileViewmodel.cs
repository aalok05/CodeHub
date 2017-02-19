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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using CodeHub.Models;
using HtmlAgilityPack;
using Microsoft.Toolkit.Uwp;

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

        /// <summary>
        /// Gets the matrix that represents the contributions chart of the current user
        /// </summary>
        public ContributionsDataModel[,] Data
        {
            get { return _Data; }
            private set { Set(() => Data, ref _Data, value); }
        }

        public String[] _Months;

        /// <summary>
        /// Gets the list of months to display in the horizontal axis of the contributions chart
        /// </summary>
        public String[] Months
        {
            get { return _Months; }
            private set { Set(() => Months, ref _Months, value); }
        }

        public IEnumerable<PinnedUserRepository> _PinnedRepositories;

        /// <summary>
        /// The public pinned repositories for the current user
        /// </summary>
        public IEnumerable<PinnedUserRepository> PinnedRepositories
        {
            get { return _PinnedRepositories; }
            private set { Set(() => PinnedRepositories, ref _PinnedRepositories, value); }
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

                    // Load the HTML document
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(html);

                    // Get the contributions chart node
                    HtmlNode
                        graph = document.DocumentNode
                            ?.Descendants("svg")
                            ?.FirstOrDefault(node => node.Attributes?.Any(att => att.Name?.Equals("class") == true &&
                                                                                 att.Value?.Equals("js-calendar-graph-svg") == true) == true),
                        root = graph?.Descendants("g")?.FirstOrDefault();
                    Months = graph?.DescendantsWithAttribute("text", "class", "month").Select(node => node.InnerText).ToArray();

                    // Extract the chart data
                    if (root != null)
                    {
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

                    // Parse the pinned repositories
                    HtmlNode repoList = document.DocumentNode?.DescendantsWithAttribute("ol", "class", "pinned-repos-list mb-4").FirstOrDefault();
                    IReadOnlyList<HtmlNode> repos = repoList?.DescendantsWithAttribute("li", "class", "pinned-repo-item p-3 mb-3 border border-gray-dark rounded-1 public source").ToList();
                    if (repos?.Any() == true)
                    {
                        PinnedRepositories =
                            from repo in repos
                            let urlNode = repo.DescendantsWithAttribute("a", "class", "text-bold").First()
                            let url = urlNode.Attributes.AttributesWithName("href").First().Value
                            let nameNode = urlNode.DescendantsWithAttribute("span", "class", "repo js-repo").First()
                            let descriptionNode = repo.DescendantsWithAttribute("p", "class", "pinned-repo-desc text-gray text-small d-block mt-2 mb-3").FirstOrDefault()
                            let lanNode = repo.DescendantsWithAttribute("span", "class", "repo-language-color pinned-repo-meta").First()
                            let language = lanNode.NextSibling.InnerText.Trim(' ', '\n', '\r')
                            let colorAtt = lanNode.Attributes.AttributesWithName("style").First().Value
                            let colorStr = colorAtt.Substring(colorAtt.Length - 7, 6)
                            let color = $"#FF{colorStr}".ToColor()
                            let features = repo.DescendantsWithAttribute("a", "class", "pinned-repo-meta muted-link")
                            let starsNode = features.FirstOrDefault(child => child.DescendantsWithAttribute("svg", "class", "octicon octicon-star").Any())
                            let stars = starsNode == null ? 0 : int.Parse(Regex.Match(starsNode.InnerText, @"\d+").Value)
                            let forksNode = features.FirstOrDefault(child => child.DescendantsWithAttribute("svg", "class", "octicon octicon-repo-forked").Any())
                            let forks = forksNode == null ? 0 : int.Parse(Regex.Match(forksNode.InnerText, @"\d+").Value)
                            select new PinnedUserRepository(
                                nameNode.InnerText, url, descriptionNode?.InnerText,
                                language, color, stars, forks);
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
