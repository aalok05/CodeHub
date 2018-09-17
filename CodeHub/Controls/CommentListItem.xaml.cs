using CodeHub.Views;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;


namespace CodeHub.Controls
{
	public sealed partial class CommentListItem : UserControl
	{
		public CommentListItem() 
			=> InitializeComponent();

		public void User_Tapped(object sender, TappedRoutedEventArgs e) 
			=> SimpleIoc
				.Default
				.GetInstance<Services.IAsyncNavigationService>()
				.NavigateAsync(typeof(DeveloperProfileView), (DataContext as IssueComment).User);
	}
}
