using Octokit;

namespace CodeHub.ViewModels
{
	public class CommentViewmodel : AppViewmodel
	{
		public IssueComment _comment;
		public IssueComment Comment
		{
			get => _comment;
			set => Set(() => Comment, ref _comment, value);
		}

		public void Load(IssueComment comment)
		{
			Comment = comment;
		}
	}
}
