using CodeHub.Services;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace CodeHub.Helpers
{
    public class NotificationModel
    {
        public long RepositoryId
        {
            get; private set;
        }
        public int Number
        {
            get; private set;
        }
        public Issue Issue
        {
            get; private set;
        }
        public PullRequest PullRequest
        {
            get; private set;
        }
        public string Subtitle
        {
            get; private set;
        }

        private NotificationModel(long repositoryId, string subTitle)
        {
            RepositoryId = repositoryId;
            Subtitle = subTitle;
        }

        private void SetNotificationType<T>(T item)
        {
            if (item is Issue issue)
            {
                Issue = issue;
                Number = issue.Number;
            }
            else if (item is PullRequest pr)
            {
                PullRequest = pr;
                Number = pr.Number;
            }
        }

        public NotificationModel(
            long repositoryId,
            Issue item,
            string subtitle
        )
            : this(repositoryId, subtitle)
        {
            SetNotificationType(item);
        }

        public NotificationModel(
            long repositoryId,
            PullRequest item,
            string subtitle
        )
            : this(repositoryId, subtitle)
        {
            SetNotificationType(item);
        }

        public bool IsIssue()
        {
            return Issue != null && PullRequest == null;
        }

        public bool IsPullRequest()
        {
            return Issue == null && PullRequest != null;
        }

        public void SetSubtitle(string subtitle)
        {
            Subtitle = subtitle;
        }
    }

    public static class OctokitNotificationHelper
    {
        private static async Task<NotificationModel> ProcessNotification(this Octokit.Notification notification)
        {
            NotificationModel result = null;
            var isIssue = notification.Subject.Type.ToLower() == "issue";
            var isPR = notification.Subject.Type.ToLower() == "pullrequest";
            if (int.TryParse(notification.Subject.Url.Split('/').Last().Split('#').First(), out int number))
            {
                var subtitle = "";
                var repo = notification.Repository;
                var repoName = repo.FullName ?? repo.Name;
                if (isIssue)
                {
                    var issue = await IssueUtility.GetIssue(repo.Id, number);
                    subtitle = $"Issue {number}";
                    result = new NotificationModel(repo.Id, issue, subtitle);
                }
                else if (isPR)
                {
                    var pr = await PullRequestUtility.GetPullRequest(repo.Id, number);
                    subtitle = $"PR {number}";
                    result = new NotificationModel(repo.Id, pr, subtitle);
                }
                subtitle = !StringHelper.IsNullOrEmptyOrWhiteSpace(subtitle)
                          ? $"{subtitle} in {repoName}"
                          : repoName;
                result.SetSubtitle(subtitle);
            }

            return result ?? throw new NullReferenceException(nameof(result));
        }

        public static async Task<TileNotification> BuildTiles(
                this Octokit.Notification notification,
                TilesTextStyles titleStyle = TilesTextStyles.Title,
                TilesTextStyles subTitleStyle = TilesTextStyles.BodySubtle,
                TilesTextStyles bodyStyle = TilesTextStyles.Body,
                TilesVisualBrandings visualBindings = TilesVisualBrandings.NameAndLogo,
                TilesBindingHintPresentation? hintPresentation = null)
        {
            int newIssuesCount = 0,
                newPrCount = 0;
            var isIssue = notification.Subject.Type.ToLower().Equals("issue");
            var repoId = notification.Repository.Id;
            var itemNumber = 0;
            User user = null;
            string title = null,
                   subtitle = null,
                   body = null,
                   status = null,
                   notificationId = notification.Id,
                   icon = null;

            var tile = new TileNotification(new XmlDocument());

            if (isIssue)
            {
                newIssuesCount++;
                var threadId = int.Parse(notification.Subject.Url.Split('/').Last().Split('?').First());
                var processedIssue = await notification.ProcessNotification();
                var issue = processedIssue.Issue;
                itemNumber = issue.Number;
                user = await UserService.GetUserInfo(issue.User.Login);
                title = SecurityElement.Escape(issue.Title);
                subtitle = SecurityElement.Escape(processedIssue.Subtitle);
                body = issue.Body;
                status = issue.State.StringValue;
                tile.Tag = $"I{issue.Id}+R{repoId}";
                icon = status.ToLower() == "closed"
                     ? "git-issue-closed.png"
                     : (status.ToLower() == "reopened"
                       ? "git-issue-reopened.png"
                       : "git-issue-opened.png");
            }
            else
            {
                newPrCount++;
                var processedPR = await notification.ProcessNotification();
                var pr = processedPR.PullRequest;
                itemNumber = pr.Number;
                user = await UserService.GetUserInfo(pr.User.Login);
                title = SecurityElement.Escape(pr.Title);
                subtitle = SecurityElement.Escape(processedPR.Subtitle);
                body = pr.Body;
                status = pr.State.StringValue;
                tile.Tag = $"P{pr.Id}+R{repoId}";
                icon = status.ToLower() == "open"
                      ? "git-pull-request.png"
                      : "git-merge.png";
            }
            body = SecurityElement.Escape(
                    body
                        .Substring(
                            0,
                            body.Length >= 50
                                         ? 49
                                         : (body.Length == 0
                                           ? 0 :
                                           body.Length - 1)
                        )
                    );

            string titleStyleString = titleStyle.ToString().ToLower(),
                   subTitleStyleString = subTitleStyle.ToString().ToLower(),
                   bodyStyleString = bodyStyle.ToString().ToLower(),
                   brandingsString = visualBindings.ToString().ToLower(),
                   hintPresentationString = hintPresentation != null ? hintPresentation.Value.ToString().ToLower() : null,
                   hintPresentationAttr = hintPresentation != null ? $"hint-presentation='{hintPresentationString}'" : null;


            var docXml = $@"
                <tile version='3'>
                    <visual branding='{brandingsString}' baseUri='Assets/Images/git/'>

                        <binding template='TileMedium'>
                            <image placement='peek' src='{user.AvatarUrl}' />
                            <group>
                                <subgroup hint-weight='33'>
                                    <image src='{icon}' />
                                </subgroup>
                                <subgroup>
                                    <text hint-style='{titleStyleString}'>
                                        {title}
                                    </text>
                                    <text hint-style='{subTitleStyleString}'>
                                        {subtitle}
                                    </text>
                                    <text hint-style='{subTitleStyleString}'>
                                        {notification.Repository.FullName}
                                    </text>
                                </subgroup>
                            </group>
                        </binding>

                        <binding template='TileLarge'>
                            <image placement='background' src='{user.AvatarUrl}' />
                            <group>
                                <subgroup hint-weight='33'>
                                    <image src='{icon}' />
                                </subgroup>
                                <subgroup>
                                    <text hint-style='{titleStyleString}'>
                                        {title}
                                    </text>
                                    <text hint-style='{subTitleStyleString}'>
                                        {subtitle}
                                    </text>
                                    <text hint-style='{subTitleStyleString}'>
                                        {notification.Repository.FullName}
                                    </text>
                                    <text hint-style='{subTitleStyleString}' hint-wrap='true' hint-maxLines='3'>
                                        {body}
                                    </text>
                                </subgroup>
                            </group>
                        </binding>

                        <binding template='TileWide'>
                            <image placement='background' src='{user.AvatarUrl}' />
                            <group>
                                <subgroup hint-weight='33'>
                                    <image src='{icon}' />
                                </subgroup>
                                <subgroup>
                                    <text hint-style='{titleStyleString}'>
                                        {title}
                                    </text>
                                    <text hint-style='{subTitleStyleString}'>
                                        {subtitle}
                                    </text>
                                    <text hint-style='{subTitleStyleString}'>
                                        {notification.Repository.FullName}
                                    </text>
                                    <text hint-style='{subTitleStyleString}' hint-wrap='true' hint-maxLines='5'>
                                        {body}
                                    </text>
                                </subgroup>
                            </group>
                        </binding>

                    </visual>
                </tile>";

            tile.Content.LoadXml(docXml);
            return tile;
        }

        public static async Task<ToastNotification> BuildToast(this Octokit.Notification notification, ToastNotificationScenario scenario = ToastNotificationScenario.Default)
        {
            return await BuildToast(notification, scenario.ToString().ToLower());
        }

        public static async Task<ToastNotification> BuildToast(this Octokit.Notification notification, string scenario = "default")
        {
            NotificationModel processedNotification = null;
            Issue issue = null;
            PullRequest pr = null;
            User user = null,
                 closedBy = null;
            DateTimeOffset stateTime;
            var repoId = notification.Repository.Id;
            var notificationId = notification.Id;
            var itemNumber = int.Parse(notification.Subject.Url.Split('/').Last().Split('#').First());
            string title = null,
                   subtitle = null,
                   body = null,
                   status = null,
                   icon = null,
                   launchArgs = $"notificationId={notificationId}&repoId={repoId}",
                   readArgs = $"notificationId={notificationId}&repoId={repoId}",
                   tag = $"N{notificationId}+R{repoId}",
                   group = null,
                   stateTimeString = null,
                   dateformatString = "dd'/'MM'/'yy H:mm:ss zzz";

            var isIssue = notification.Subject.Type.ToLower().Equals("issue");
            var isPR = notification.Subject.Type.ToLower().Equals("pullrequest");
            processedNotification = await notification.ProcessNotification();

            if (isIssue)
            {
                issue = processedNotification.Issue;
                title = SecurityElement.Escape(issue.Title);
                subtitle = SecurityElement.Escape(processedNotification.Subtitle);
                body = issue.Body;
                user = await UserService.GetUserInfo(issue.User.Login);
                launchArgs += $"&action=showIssue&issueNumber={issue.Number}";
                readArgs += $"&action=markIssueAsRead&issueNumber={issue.Number}";
                tag += $"+I{issue.Number}";
                group = "Issues";
                stateTime = (issue.ClosedAt ?? issue.UpdatedAt) ?? issue.CreatedAt;
                stateTime = stateTime.ToLocalTime();
                stateTimeString = stateTime.ToString(dateformatString);
                if (issue.State.StringValue.ToLower() == "open")
                {
                    icon = "git-issue-opened.png";
                    status = $"opened by {user.Name ?? user.Login} at {stateTimeString}";
                }
                else if (issue.State.StringValue.ToLower() == "reopened")
                {
                    icon = "git-issue-reopened.png";
                    status = $"re-opened by {user.Name ?? user.Login} at {stateTimeString}";
                }
                else if (issue.State.StringValue.ToLower() == "closed")
                {
                    icon = "git-issue-closed.png";
                    closedBy = issue.ClosedBy;
                    status = $"closed by {closedBy.Name ?? closedBy.Login} at {stateTimeString}";
                }
            }
            else if (isPR)
            {
                pr = processedNotification.PullRequest;
                title = SecurityElement.Escape(pr.Title);
                subtitle = SecurityElement.Escape(processedNotification.Subtitle);
                body = pr.Body;
                user = await UserService.GetUserInfo(pr.User.Login);
                launchArgs += $"&action=showPr&prNumber={pr.Number}";
                readArgs += $"&action=markPrAsRead&prNumber={pr.Number}";
                tag += $"+P{pr.Number}";
                group = "PullRequests";
                stateTime = (pr.ClosedAt ?? pr.MergedAt) ?? pr.CreatedAt;
                stateTime = stateTime.ToLocalTime();
                stateTimeString = stateTime.ToString(dateformatString);
                if (pr.State.StringValue.ToLower() == "open")
                {
                    icon = "git-pull-request.png";
                    status = $"opened by {user.Name ?? user.Login} at {  stateTime.ToLocalTime().ToString() }";
                }
                else if (pr.State.StringValue.ToLower() == "reopened")
                {
                    icon = "git-pull-request.png";
                    status = $"re-opened by {user.Name ?? user.Login} at {stateTimeString}";
                }
                else if (pr.State.StringValue.ToLower() == "closed")
                {
                    if (pr.Merged)
                    {
                        icon = "git-merge.png";
                        closedBy = pr.MergedBy;
                        status = $"merged by {closedBy.Name ?? closedBy.Login} at {stateTimeString}";
                    }
                    else
                    {
                        icon = "";
                        status = $"closed at {stateTimeString}";
                    }
                }
            }

            body = body
                .Substring(0, body.Length >= 50 ? 49 : (body.Length == 0 ? 0 : body.Length - 1));

            body = SecurityElement.Escape(body ?? "");
            launchArgs = SecurityElement.Escape(launchArgs ?? "");
            readArgs = SecurityElement.Escape(readArgs ?? "");

            var xml = new XmlDocument();
            xml.LoadXml($@"<toast activationType='foreground' launch='{launchArgs}' scenario='{scenario}'>
							<visual baseUri='Assets/Images/git/'>
								<binding template='ToastGeneric'>
									<text>{title ?? ""}</text>
                                    <text placement='attribution'>{subtitle ?? ""}</text>
									<image placement='appLogoOverride' hint-crop='circle' src='{icon}' />						
									<group>						
										<subgroup hint-weight='33'>
											<image hint-crop='circle' src='{user.AvatarUrl ?? ""}' />
									        <text hint-style='headingSubtle' hint-align='center'>{user.Name ?? user.Login}</text>
										</subgroup>
										<subgroup>	
                                            <text hint-style='bodySubtle' hint-align='center' hint-wrap='true'>{status ?? ""}</text>  
									        <text hint-style='body' hint-align='center' hint-wrap='true'>{body ?? ""}</text>
                                        </subgroup>		
									</group>
								</binding>
							</visual>
							<actions>
								<action
									activationType='background'
									arguments='{readArgs ?? ""}'
									content='Mark As Read' />
								<action
									activationType='system'
									arguments='dismiss'
									content='Dismiss' />
							</actions>

					</toast>");
            var toast = new ToastNotification(xml)
            {
                Tag = tag,
                Group = group
            };
            return processedNotification != null
                 ? toast
                 : throw new NullReferenceException();
        }

        public static async Task ShowToasts(this ICollection<Octokit.Notification> collection, BackgroundTaskDeferral deferral = null)
        {
            if (collection == null)
            {
                throw new NullReferenceException($"${nameof(collection)} cannot be null");
            }

            collection = new ObservableCollection<Octokit.Notification>(collection.OrderBy(n => n.UpdatedAt));
            var toastNotifications = ToastNotificationManager.History.GetHistory();

            if (toastNotifications != null && toastNotifications.Count > 0)
            {
                foreach (var toast in toastNotifications)
                {
                    Octokit.Notification notification = null;
                    try
                    {
                        notification = await toast.GetNotification();
                    }
                    finally
                    {
                        if (notification != null && collection != null && !collection.Any(n => n.Id == notification.Id) && !StringHelper.IsNullOrEmptyOrWhiteSpace(toast.Tag) && !StringHelper.IsNullOrEmptyOrWhiteSpace(toast.Group))
                        {
                            ToastNotificationManager.History.Remove(toast.Tag, toast.Group);
                        }
                    }
                }
                toastNotifications = ToastNotificationManager.History.GetHistory();
            }
            if (collection != null && collection.Count() > 0)
            {
                foreach (var notification in collection)
                {
                    ToastNotification toast = null;
                    try
                    {
                        toast = await notification.BuildToast(ToastNotificationScenario.Reminder);
                    }
                    finally
                    {
                        if (toast != null && !StringHelper.IsNullOrEmptyOrWhiteSpace(toast.Tag) && !StringHelper.IsNullOrEmptyOrWhiteSpace(toast.Group) && (toastNotifications != null || toastNotifications.Count >= 0) && !toastNotifications.Any(t => t.Like(toast)))
                        {
                            ToastHelper.PopCustomToast(toast, toast.Tag, toast.Group);
                        }
                    }
                }
            }

            if (deferral != null)
            {
                deferral?.Complete();
            }
        }
    }
}
