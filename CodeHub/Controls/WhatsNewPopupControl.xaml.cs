using UICompositionAnimations;
using UICompositionAnimations.Enums;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CodeHub.Controls
{
	public sealed partial class WhatsNewPopupControl : UserControl
	{
		public WhatsNewPopupControl() 
			=> InitializeComponent();

		public string WhatsNewText
		{
			get => (string)GetValue(WhatsNewTextroperty);
			set => SetValue(WhatsNewTextroperty, value);
		}

		public static readonly DependencyProperty WhatsNewTextroperty =
		  DependencyProperty.Register(nameof(WhatsNewText), typeof(string), typeof(WhatsNewPopupControl), new PropertyMetadata(false));

		private async void CloseWhatsNew_Tapped(object sender, RoutedEventArgs e)
		{
			//await this.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
			Visibility = Visibility.Collapsed;
		}
	}
}
