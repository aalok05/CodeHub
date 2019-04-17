using UICompositionAnimations;
using UICompositionAnimations.Enums;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CodeHub.Controls
{
	public sealed partial class CommitFileListItem : UserControl
	{
		public CommitFileListItem() 
			=> InitializeComponent();

		private async void Expander_Click(object sender, RoutedEventArgs e)
		{
			if (PatchText.Visibility == Visibility.Visible)
			{
				ExpanderIcon.Glyph = "\uE0E5";

                await PatchText.Animation()
                    .Opacity(1, 0)
                    .Scale(1, 0.98f, Easing.SineEaseInOut)
                    .Duration(100)
                    .StartAsync();
                PatchText.Visibility = Visibility.Collapsed;
			}
			else
			{
				ExpanderIcon.Glyph = "\uE0E4";

				PatchText.Visibility = Visibility.Visible;
                await PatchText.Animation()
                    .Opacity(0, 1)
                    .Scale(0.98f, 1, Easing.SineEaseInOut)
                    .Duration(100)
                    .StartAsync();
            }
		}
	}
}
