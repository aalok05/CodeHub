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
				await PatchText.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 0.98f, 100, null, 0, EasingFunctionNames.SineEaseInOut);
				PatchText.Visibility = Visibility.Collapsed;
			}
			else
			{
				ExpanderIcon.Glyph = "\uE0E4";
				PatchText.SetVisualOpacity(0);
				PatchText.Visibility = Visibility.Visible;
				await PatchText.StartCompositionFadeScaleAnimationAsync(0, 1, 0.98f, 1, 100, null, 0, EasingFunctionNames.SineEaseInOut);
			}
		}
	}
}
