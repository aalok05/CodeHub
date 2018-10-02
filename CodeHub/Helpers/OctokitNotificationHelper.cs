using CodeHub.Services;
using Octokit;
using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace CodeHub.Helpers
{
    public class NotificationModel
    {
        public long RepositoryId { get; private set; }
        public int Number { get; private set; }
        public Issue Issue { get; private set; }
        public PullRequest PullRequest { get; private set; }
        public string Subtitle { get; private set; }

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
            else
            {
                throw new NotImplementedException();
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
    }

    public static class OctokitNotificationHelper
    {

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
                var processedIssue = await notification.ProcessNotification<Issue>();
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
                var processedPR = await notification.ProcessNotification<PullRequest>();

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
            User user = null;
            var repoId = notification.Repository.Id;
            var notificationId = notification.Id;
            string title = null,
                   subtitle = null,
                   body = null,
                   status = null,
                   icon = null,
                   launchArgs = $"notificationId={notificationId}&repoId={repoId}",
                   readArgs = $"notificationId={notificationId}&repoId={repoId}",
                   tag = $"N{notificationId}",
                   group = null;
            var itemNumber = 0;

            var isIssue = notification.Subject.Type.ToLower().Equals("issue");

            if (isIssue)
            {
                processedNotification = await notification.ProcessNotification<Issue>();
                issue = processedNotification.Issue;
                title = SecurityElement.Escape(issue.Title);
                subtitle = SecurityElement.Escape(processedNotification.Subtitle);
                body = SecurityElement.Escape(issue.Body);
                itemNumber = issue.Number;
                status = issue.State.StringValue;
                user = await UserService.GetUserInfo(issue.User.Login);
                icon = status.ToLower() == "closed"
                     ? "git-issue-closed.png"
                     : (status.ToLower() == "reopened"
                       ? "git-issue-reopened.png"
                       : "git-issue-opened.png");
                launchArgs += $"&action=showIssue&issueId={issue.Number}";
                readArgs += $"&action=markIssueAsRead&issueId={issue.Number}";
                tag += $"+I{itemNumber}+R{repoId}";
                group = "Issue";
            }
            else
            {
                processedNotification = await notification.ProcessNotification<PullRequest>();
                pr = processedNotification.PullRequest;
                title = SecurityElement.Escape(pr.Title);
                subtitle = SecurityElement.Escape(processedNotification.Subtitle);
                body = SecurityElement.Escape(pr.Body);
                itemNumber = pr.Number;
                status = pr.State.StringValue;
                user = await UserService.GetUserInfo(pr.User.Login);
                icon = status.ToLower() == "open"
                     ? "git-pull-request.png"
                     : "git-merge.png";
                launchArgs += $"&action=showPR&prId={pr.Number}";
                readArgs += $"&action=markPrAsRead&prId={pr.Number}";
                tag+= $"+P{pr.Number}+R{repoId}";
                group = "PullRequests";
            }

            body = body
                .Substring(0, body.Length >= 50 ? 49 : (body.Length == 0 ? 0 : body.Length - 1));

            body = SecurityElement.Escape(body);
            launchArgs = SecurityElement.Escape(launchArgs);
            readArgs = SecurityElement.Escape(readArgs);

            var xml = new XmlDocument();
            xml.LoadXml($@"<toast activationType='foreground' launch='{launchArgs}' scenario='{scenario}'>
							<visual baseUri='Assets/Images/git/'>
								<binding template='ToastGeneric'>
									<text>{title}</text>
									<image placement='appLogoOverride' hint-crop='circle' src='{icon}' />						
									<group>						
										<subgroup hint-weight='33'>								
											<image hint-crop='circle' src='{user.AvatarUrl}' />
									        <text hint-style='headingSubtle'>{user.Name}</text>
										</subgroup>
										<subgroup>	
                                            <text hint-style='bodySubtle' hint-align='center'>{notification.Repository.FullName}</text>
											<text hint-style='subtitleSubtle' hint-align='center'>{subtitle}</text>	
											<text hint-style='captionSubtle' hint-align='center'>{status}</text>
									        <text hint-style='body' hint-align='center' hint-wrap='true'>{body}</text>
                                        </subgroup>		
									</group>
								</binding>
							</visual>
							<actions>
								<action
									activationType='background'
									arguments='{readArgs}'
									content='Mark As Read' />
								<action
									activationType='system'
									arguments='dismiss'
									content='Dismiss' />
							</actions>

					</toast>");
            var toast = new ToastNotification(xml);
            toast.Tag = tag;
            toast.Group = group;
            return toast;
        }
        private static async Task<NotificationModel> ProcessNotification<T>(this Octokit.Notification notification)
        {
            NotificationModel result = null;
            var number = int.Parse(notification.Subject.Url.Split('/').Last().Split('?').First());
            var repoId = notification.Repository.Id;

            if (typeof(T) == typeof(Issue))
            {
                var issue = await IssueUtility.GetIssue(repoId, number);
                var subtitle = $"Issue {number}";
                result = new NotificationModel(repoId, issue, subtitle);
            }
            if (typeof(T) == typeof(PullRequest))
            {
                var pr = await PullRequestUtility.GetPullRequest(repoId, number);
                var subtitle = $"Pull Request {number}";
                result = new NotificationModel(repoId, pr, subtitle);
            }

            return result ?? throw new InvalidOperationException("Invalid type");
        }
    }
}
