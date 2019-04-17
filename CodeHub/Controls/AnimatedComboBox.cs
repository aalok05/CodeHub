using CodeHub.Helpers;
using System.Collections;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Controls
{
	/// <summary>
	/// A simple ComboBox with an open/close animation for the drop down menu
	/// </summary>
	public class AnimatedComboBox : ComboBox
	{
		// The element that contains the drop down menu to animate
		private FrameworkElement _TranslationElement;

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			_TranslationElement = GetTemplateChild("CoreGrid") as FrameworkElement;
		}

		// Animate the drop down menu
		protected override async void OnDropDownOpened(object e)
		{
			if (_TranslationElement != null)
			{
				// Pick the animation direction depending on the current position
				int? count = ((ItemsSource as ICollection)?.Count ?? (ItemsSource as IEnumerable)?.Count())
						   ?? Items?.Count;
				bool down = count == null || ((SelectedIndex < 1 || SelectedIndex <= count / 2) &&
										!(count == 2 && SelectedIndex == 1));
				float start = down ? -16 : 16;

                _TranslationElement.Animation().Offset(Axis.Y, start);

                // Animate using Windows.UI.Composition animations to avoid frame drops
                await _TranslationElement.Animation()
                    .Translation(Axis.Y, start, 0, Easing.CircleEaseOut)
                    .Duration(250)
                    .StartAsync();

			}
			base.OnDropDownOpened(e);
		}
	}
}
