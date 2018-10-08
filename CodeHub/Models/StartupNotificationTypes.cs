namespace CodeHub.Models
{
    public static class StartupNotificationTypes
    {
        public static StartupNotificationGroup MainGroup = new StartupNotificationGroup("notifications scenarios", new IStartupNotification[]
        {
            new StartupNotification("Issues", typeof(Views.IssueDetailView)),
            new StartupNotification("PullRequests", typeof(Views.PullRequestDetailView))
        });
    }
}
