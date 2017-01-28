using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CodeHub.Helpers;
using UICompositionAnimations;

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
                await _TranslationElement.SetVisualOffsetAsync(TranslationAxis.Y, start);

                // Animate using Windows.UI.Composition animations to avoid frame drops
                _TranslationElement.StartCompositionSlideAnimation(TranslationAxis.Y, start, 0, 250, null, EasingFunctionNames.CircleEaseOut);
            }
            base.OnDropDownOpened(e);
        }
    }
}
