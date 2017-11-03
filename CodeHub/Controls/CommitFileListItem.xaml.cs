using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace CodeHub.Controls
{
    public sealed partial class CommitFileListItem : UserControl
    {
        public CommitFileListItem()
        {
            this.InitializeComponent();
        }

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
