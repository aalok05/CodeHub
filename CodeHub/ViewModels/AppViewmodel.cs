using BackgroundTasks.Helpers;
using CodeHub.Helpers;
using CodeHub.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Notifications.Helpers;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Services.Store;
using Windows.System.Profile;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;
using XmlElement = Windows.Data.Xml.Dom.XmlElement;

namespace CodeHub.ViewModels
{
    public class AppViewmodel : ViewModelBase
    {
        #region properties
        public bool _isLoggedin;
        public bool IsLoggedin
        {
            get => _isLoggedin;
            set => Set(() => IsLoggedin, ref _isLoggedin, value);
        }

        public bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public User _user;
        public User User
        {
            get => _user;
            set => Set(() => User, ref _user, value);
        }

        public bool _IsNotificationsUnread;
        public bool IsNotificationsUnread
        {
            get => _IsNotificationsUnread;
            set => Set(() => IsNotificationsUnread, ref _IsNotificationsUnread, value);
        }

        private bool _isDesktopAdsVisible;
        public bool IsDesktopAdsVisible
        {
            get => _isDesktopAdsVisible;
            set => Set(() => IsDesktopAdsVisible, ref _isDesktopAdsVisible, value);
        }

        private bool _isMobileAdsVisible;
        public bool IsMobileAdsVisible
        {
            get => _isMobileAdsVisible;
            set => Set(() => IsMobileAdsVisible, ref _isMobileAdsVisible, value);
        }

        public string WhatsNewText
              => "Hi all! \nHere's the changelog for v2.4.14\n\n\x2022 Added Turkish translations\n\x2022 Minor fluent UI improvements\n\x2022 Target build updated to 17134.0\n\x2022 Clicking on notifications now lands you on the specific issue or PR  \n\n NOTE: Please update to Fall creator's update or above to get latest CodeHub updates.";

        private ObservableCollection<Octokit.Notification> _notifications;
        public ObservableCollection<Octokit.Notification> UnreadNotifications
        {
            get => _notifications;
            set => Set(() => UnreadNotifications, ref _notifications, value);
        }
        #endregion

        private const string donateFirstAddOnId = "9pd0r1dxkt8j";
        private const string donateSecondAddOnId = "9msvqcz4pbws";
        private const string donateThirdAddOnId = "9n571g3nr2cs";
        private const string donateFourthAddOnId = "9nsmgzx3p43x";
        private const string donateFifthAddOnId = "9phrhpvhscdv";
        private const string donateSixthAddOnId = "9nnqdq0kq21j";

        async Task<(long, int, Issue, string)> ProcessIssueNotification(Octokit.Notification notification)
        {
            var number = int.Parse(notification.Subject.Url.Split('/').Last().Split('?').First());
            var repoId = notification.Repository.Id;
            var issue = await IssueUtility.GetIssue(repoId, number);
            var subtitle = $"Issue {number}";
            return (repoId, number, issue, subtitle);
        }

        async Task<(long, int, PullRequest, string)> ProcessPullRequestNotification(Octokit.Notification notification)
        {
            var number = int.Parse(notification.Subject.Url.Split('/').Last().Split('?').First());
            var repoId = notification.Repository.Id;
            var pr = await PullRequestUtility.GetPullRequest(repoId, number);
            var subtitle = $"Pull Request {number}";
            return (repoId, number, pr, subtitle);
        }

        public AppViewmodel()
        {
            UnreadNotifications = new ObservableCollection<Octokit.Notification>();
            UnreadNotifications.CollectionChanged += UnreadNotifications_CollectionChanged;
        }

        public async void MarkdownTextBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
            }
            catch (UriFormatException)
            {
                MessageDialog dialog = new MessageDialog("Incorrect URI Format");
                await dialog.ShowAsync();
            }
        }

        private async void UnreadNotifications_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (Octokit.Notification item in e.NewItems)
                {
                    if (SettingsService.Get<bool>(SettingsKeys.IsToastEnabled))
                    {
                        await ShowToast(item, ToastNotificationScenario.Reminder);
                    }
                }
            }
            await UpdateTiles(
                SettingsService.Get<bool>(SettingsKeys.IsLiveTilesEnabled),
                SettingsService.Get<bool>(SettingsKeys.IsLiveTilesBadgeEnabled),
                SettingsService.Get<bool>(SettingsKeys.IsLiveTileUpdateAllBadgesEnabled)
            );
        }

        public void Navigate(Type pageType)
        {
            SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(pageType, User);
        }

        public void GoBack()
        {
            SimpleIoc.Default.GetInstance<IAsyncNavigationService>().GoBackAsync();
        }

        public void UpdateUnreadNotificationIndicator(bool IsUnread)
        {
            IsNotificationsUnread = IsUnread;
        }

        public async Task CheckForUnreadNotifications()
        {
            var unread = await NotificationsService.GetAllNotificationsForCurrentUser(false, false);
            if (unread != null)
            {
                foreach (var item in unread)
                {
                    UnreadNotifications.Add(item);
                }

                if (UnreadNotifications.Count > 0)
                {
                    UpdateUnreadNotificationIndicator(true);
                }
                else
                {
                    UpdateUnreadNotificationIndicator(false);
                }
            }
        }

        public async Task<bool> HasAlreadyDonated()
        {
            try
            {
                if (SettingsService.Get<bool>(SettingsKeys.HasUserDonated))
                {
                    return true;
                }
                else
                {
                    StoreContext WindowsStore = StoreContext.GetDefault();

                    string[] productKinds = { "Durable" };
                    List<string> filterList = new List<string>(productKinds);

                    StoreProductQueryResult queryResult = await WindowsStore.GetUserCollectionAsync(filterList);

                    if (queryResult.ExtendedError != null)
                    {
                        return false;
                    }

                    foreach (KeyValuePair<string, StoreProduct> item in queryResult.Products)
                    {
                        if (item.Value != null)
                        {
                            if (item.Value.IsInUserCollection)
                            {
                                SettingsService.Save(SettingsKeys.HasUserDonated, true, true);
                                return true;
                            }
                        }
                        return false;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task ConfigureAdsVisibility()
        {
            if (await HasAlreadyDonated())
            {
                GlobalHelper.HasAlreadyDonated = true;
                ToggleAdsVisiblity();
            }
            else
            {
                SettingsService.Save(SettingsKeys.IsAdsEnabled, true);

                if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                {
                    IsMobileAdsVisible = true;
                    IsDesktopAdsVisible = false;
                }
                else
                {
                    IsDesktopAdsVisible = true;
                    IsMobileAdsVisible = false;
                }
            }
        }

        public void ToggleAdsVisiblity()
        {
            if (SettingsService.Get<bool>(SettingsKeys.IsAdsEnabled))
            {
                if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                {
                    IsMobileAdsVisible = true;
                    IsDesktopAdsVisible = false;
                }
                else
                {
                    IsDesktopAdsVisible = true;
                    IsMobileAdsVisible = false;
                }
            }
            else
            {
                IsMobileAdsVisible = IsDesktopAdsVisible = false;
            }
        }


        public async Task UpdateTiles(bool updateTileEnabled, bool updateBadgeEnabled, bool updateAllBadgesEnabled)
        {
            string GetStringFromEnum<T>(T @enum)
                where T : Enum
                => @enum.ToString().ToLower();

            XmlDocument GetBadgeUpdateXml()
            {
                // Get the blank badge XML payload for a badge number
                var badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

                // Set the value of the badge in the XML to our number
                var badgeElement = badgeXml.SelectSingleNode("/badge") as XmlElement;
                badgeElement.SetAttribute("value", UnreadNotifications.Count.ToString());
                return badgeXml;
            }

            async Task<XmlDocument> GetNotificationUpdateXml(
                TilesTextStyles titleStyle = TilesTextStyles.Title,
                TilesTextStyles subTitleStyle = TilesTextStyles.BodySubtle,
                TilesTextStyles bodyStyle = TilesTextStyles.Body,
                TilesVisualBrandings visualBindings = TilesVisualBrandings.NameAndLogo,
                TilesBindingHintPresentation? hintPresentation = null)
            {
                var newIssuesCount = UnreadNotifications.Where(n => n.Subject.Type.ToLower().Equals("issue")).Count();
                var newPrCount = UnreadNotifications.Where(n => n.Subject.Type.ToLower().Equals("pullrequest")).Count();

                (long, int, Issue, string)? processedIssue = null;
                (long, int, PullRequest, string)? processedPR = null;
                var newNotification = UnreadNotifications.First();
                var isIssue = newNotification.Subject.Type.ToLower().Equals("issue");
                var isPR = newNotification.Subject.Type.ToLower().Equals("pullrequest");
                if (isIssue)
                {
                    processedIssue = await ProcessIssueNotification(newNotification);
                }
                else if (isPR)
                {
                    processedPR = await ProcessPullRequestNotification(newNotification);
                }

                var issue = processedIssue?.Item3;
                var pr = processedPR?.Item3;
                var itemId = isIssue ? issue.Number : pr.Number;

                var user = isIssue ? (issue.ClosedBy ?? issue.User) : (pr.MergedBy ?? pr.User);
                var title = isIssue ? System.Security.SecurityElement.Escape(issue.Title) : System.Security.SecurityElement.Escape(pr.Title);
                var subtitle = processedIssue?.Item4 ?? processedPR?.Item4;
                var body = isIssue
                         ? System.Security.SecurityElement.Escape(issue.Body)
                         : System.Security.SecurityElement.Escape(pr.Body);
                body = body
                    .Substring(0, body.Length >= 50 ? 49 : (body.Length == 0 ? 0 : body.Length - 1));
                var status = issue?.State.StringValue ?? pr?.State.StringValue;
                var notificationType = isIssue ? "Issue" : "Pull Request";
                var notificationId = isIssue ? issue.Number : pr.Number;

                string titleStyleString = GetStringFromEnum(titleStyle),
                       subTitleStyleString = GetStringFromEnum(subTitleStyle),
                       bodyStyleString = GetStringFromEnum(bodyStyle),
                       brandingsString = GetStringFromEnum(visualBindings),
                       hintPresentationString = hintPresentation != null ? GetStringFromEnum(hintPresentation.Value) : null,
                       hintPresentationAttr = hintPresentation != null ? $"hint-presentation='{hintPresentationString}'" : null;



                var doc = new XmlDocument();
                doc.LoadXml($@"
                <tile version='3'>
                    <visual branding='{brandingsString}'>

                        <binding template='TileMedium'>
                            <text hint-wrap='true'>{newIssuesCount} new Issues</text>
                            <text hint-wrap='true'>{newPrCount} new PRs</text>
                        </binding>

                        <binding template='TileWide'>
                            <text hint-wrap='true'>{newIssuesCount} new Issues</text>
                            <text hint-wrap='true'>{newPrCount} new PRs</text>
                        </binding>

                        <binding template='TileLarge' {hintPresentationAttr ?? ""}>                            
                            <text hint-style='{titleStyleString}'>{title}</text>
                            <text hint-style='{subTitleStyleString}' hint-wrap='true'>{notificationType} {notificationId} {status} by {user.Name ?? user.Login}</text>
                            <text hint-style='{bodyStyleString}' hint-wrap='true'>{body}</text>
                        </binding>

                    </visual>
                </tile>");
                return doc;
            }

            void UpdateBadge(XmlDocument badgeXml)
            {
                // Create the badge notification
                BadgeNotification badge = new BadgeNotification(badgeXml);

                var badgeUpdater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();

                // And update the badge
                badgeUpdater.Update(badge);
            }

            async Task UpdateTileNotification()
            {
                var notification = new TileNotification(await GetNotificationUpdateXml());
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
            }

            if (updateBadgeEnabled)
            {
                UpdateBadge(GetBadgeUpdateXml());
            }
            if (updateTileEnabled)
            {
                UpdateTileNotification(await GetNotificationUpdateXml());
            }
            if (updateAllBadgesEnabled)
            {
                var secondaryTiles = await SecondaryTile.FindAllAsync();
                if (secondaryTiles != null && secondaryTiles.Count > 0)
                {
                    foreach (var secondaryTile in secondaryTiles)
                    {
                        if (updateTileEnabled)
                        {
                            await secondaryTile.UpdateAsync();
                        }
                    }
                }
            }
        }

        public async Task ShowToast(Octokit.Notification notification, ToastNotificationScenario? scenario)
        {
            async Task<string> MakeToastNotificationXml(string scenarioInText = "default")
            {
                (long, int, Issue, string)? processedIssue = null;
                (long, int, PullRequest, string)? processedPR = null;
                var isIssue = notification.Subject.Type.ToLower().Equals("issue");
                var isPR = notification.Subject.Type.ToLower().Equals("pullrequest");
                if (isIssue)
                {
                    processedIssue = await ProcessIssueNotification(notification);
                }
                else if (isPR)
                {
                    processedPR = await ProcessPullRequestNotification(notification);
                }

                var issue = processedIssue?.Item3;
                var pr = processedPR?.Item3;
                var itemId = isIssue ? issue.Number : pr.Number;

                (string, string) user = isIssue ? (issue.User.AvatarUrl, issue.User.Name) : (pr.User.AvatarUrl, pr.User.Name);
                var title = isIssue ? System.Security.SecurityElement.Escape(issue.Title) : System.Security.SecurityElement.Escape(pr.Title);
                var subtitle = processedIssue?.Item4 ?? processedPR?.Item4;
                var body = isIssue ? System.Security.SecurityElement.Escape(issue.Body) : System.Security.SecurityElement.Escape(pr.Body);
                body = body
                    .Substring(0, body.Length >= 50 ? 49 : (body.Length == 0 ? 0 : body.Length - 1));
                var status = issue?.State.StringValue ?? pr?.State.StringValue;

                var icon = "";
                if (isIssue)
                {
                    icon = status.ToLower() == "closed"
                         ? "ms-appx:///Assets/Images/git/git-issue-closed.png"
                         : (status.ToLower() == "reopened" ? "ms-appx:///Assets/Images/git/git-issue-reopened.png" : "ms-appx:///Assets/Images/git/git-issue-opened.png");
                }
                else if (isPR)
                {
                    icon = status.ToLower() == "open" ? "ms-appx:///Assets/Images/git/git-pull-request.png" : "ms-appx:///Assets/Images/git/git-merge.png";
                }

                return $@"<toast launch='action=viewEvent&amp;eventId={itemId}' scenario='{scenarioInText}'>
							<visual>
								<binding template='ToastGeneric'>
									<text>{user.Item2}</text>
									<text>{title}</text>
									<text>{body}</text>
									<image placement='appLogoOverride' hint-crop='circle' src='{user.Item1}' />						
									<group>						
										<subgroup hint-weight='1'>								
											<image src='{icon}' />
										</subgroup>
										<subgroup hint-weight='1'>	
											<text hint-align='center'>{subtitle}</text>	
											<text hint-align='center'>{status}</text>
										</subgroup>		
									</group>
								</binding>
							</visual>
							<actions>
								<action
									activationType='background'
									arguments='action=markRead&amp;issueId={itemId}'
									content='Mark As Read'/>
								<action
									activationType='background'
									arguments='dismiss'
									content='Dismiss'/>
							</actions>

					</toast>";
            }

            string scenarioTxt = "", xml = "";
            if (scenario != null)
            {
                scenarioTxt = scenario.Value == ToastNotificationScenario.Alarm ? "alarm" : (scenario.Value == ToastNotificationScenario.Reminder ? "reminder" : "incomingCall");
            }
            if (scenario != null)
            {
                xml = await MakeToastNotificationXml(scenarioTxt);
            }
            else
            {
                xml = await MakeToastNotificationXml();
            }

            ToastHelper.PopCustomToast(xml);
        }
    }
}
