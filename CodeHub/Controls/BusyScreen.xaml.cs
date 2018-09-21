using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Controls
{
	public sealed partial class BusyScreen : UserControl
	{
		public BusyScreen()
		{
			InitializeComponent();
		}
		public string BusyText
		{
			get => (string)GetValue(BusyTextProperty);
			set => SetValue(BusyTextProperty, value);
		}

		public static readonly DependencyProperty BusyTextProperty =
		  DependencyProperty.Register(nameof(BusyText), typeof(string), typeof(BusyScreen), new PropertyMetadata(false));

		public bool IsBusy
		{
			get => (bool)GetValue(IsBusyProperty);
			set => SetValue(IsBusyProperty, value);
		}

		public static readonly DependencyProperty IsBusyProperty =
		  DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(BusyScreen), new PropertyMetadata(false));

	}
}