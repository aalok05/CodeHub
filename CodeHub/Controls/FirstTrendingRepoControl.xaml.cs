using Octokit;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Controls
{
	public sealed partial class FirstTrendingRepoControl : UserControl
	{
		public FirstTrendingRepoControl() 
			=> InitializeComponent();

		public Repository Repository
		{
			get => (Repository)GetValue(RepositoryProperty);
			set => SetValue(RepositoryProperty, value);
		}

		public static readonly DependencyProperty RepositoryProperty =
		  DependencyProperty.Register(nameof(Repository), typeof(Repository), typeof(FirstTrendingRepoControl), new PropertyMetadata(false));
	}
}
