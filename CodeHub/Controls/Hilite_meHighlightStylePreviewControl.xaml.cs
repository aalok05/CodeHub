using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using CodeHub.Helpers;
using CodeHub.Services.Hilite_me;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace CodeHub.Controls
{
    /// <summary>
    /// A control that displays a visual preview of the different code highlight options
    /// </summary>
    public sealed partial class Hilite_meHighlightStylePreviewControl : UserControl
    {
        public Hilite_meHighlightStylePreviewControl()
        {
            this.InitializeComponent();
            this.SizeChanged += (s, e) => ClippingRect.Rect = new Rect(0, 0, ActualWidth, ActualHeight);
        }

        // Blurs the control with an animation
        private Task BlurAsync(double value, TimeSpan duration)
        {
            BlurBehaviour.AutomaticallyStart = false;
            BlurBehaviour.Value = value;
            BlurBehaviour.Duration = duration.TotalMilliseconds;
            BlurBehaviour.StartAnimation();
            return Task.Delay(duration);
        }

        /// <summary>
        /// Gets or sets whether or not the line numbers are visible
        /// </summary>
        public bool LineNumbersVisible
        {
            get { return (bool)GetValue(LineNumbersVisibleProperty); }
            set { SetValue(LineNumbersVisibleProperty, value); }
        }

        public static readonly DependencyProperty LineNumbersVisibleProperty = DependencyProperty.Register(
            nameof(LineNumbersVisible), typeof(SyntaxHighlightStyleEnum), typeof(Hilite_meHighlightStylePreviewControl),
            new PropertyMetadata(false, OnLineNumbersVisiblePropertyChanged));

        private static async void OnLineNumbersVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Unpack the parameters and lock the UI
            Hilite_meHighlightStylePreviewControl @this = d.To<Hilite_meHighlightStylePreviewControl>();
            await @this.AnimationSemaphore.WaitAsync();

            // Update the preview
            if (@this.ReadLocalValue(HighlightStyleProperty) != DependencyProperty.UnsetValue)
            {
                bool light = LightStyles.Contains(@this.HighlightStyle);
                @this.FadeCanvas.Background = new SolidColorBrush(light ? Colors.White : Colors.Black);
                @this.WebControl.DefaultBackgroundColor = light ? Colors.White : Colors.Black;
                String preview = await @this.LoadHTMLPreviewAsync();
                @this.WebControl.NavigateToString(preview);
            }
            @this.AnimationSemaphore.Release();
        }

        /// <summary>
        /// Gets or sets the currently selected code highlight style
        /// </summary>
        public SyntaxHighlightStyleEnum HighlightStyle
        {
            get { return (SyntaxHighlightStyleEnum)GetValue(HighlightStyleProperty); }
            set { SetValue(HighlightStyleProperty, value); }
        }

        public static readonly DependencyProperty HighlightStyleProperty = DependencyProperty.Register(
            nameof(HighlightStyle), typeof(SyntaxHighlightStyleEnum), typeof(Hilite_meHighlightStylePreviewControl),
            new PropertyMetadata(DependencyProperty.UnsetValue, OnHighlightStylePropertyChanged));

        // Private semaphore to avoid concurrent accesses
        private readonly SemaphoreSlim AnimationSemaphore = new SemaphoreSlim(1);

        // The duration of each fade in/out animation
        private const int AnimationDuration = 400;

        // Gets a collection of the highlight styles with a white background
        private static readonly IReadOnlyCollection<SyntaxHighlightStyleEnum> LightStyles = new[]
        {
            SyntaxHighlightStyleEnum.Borland, SyntaxHighlightStyleEnum.Colorful, SyntaxHighlightStyleEnum.Emacs,
            SyntaxHighlightStyleEnum.Perldoc, SyntaxHighlightStyleEnum.VS
        };

        // Indicates the transition needed when changing the highlight preview
        private enum HighlightTransitionType
        {
            None,
            DarkToLight,
            LightToDark
        }

        private static async void OnHighlightStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Unpack the parameters and lock the UI
            Hilite_meHighlightStylePreviewControl @this = d.To<Hilite_meHighlightStylePreviewControl>();
            await @this.AnimationSemaphore.WaitAsync();
            SyntaxHighlightStyleEnum style = e.NewValue.To<SyntaxHighlightStyleEnum>();

            // Check if there's a background transition
            HighlightTransitionType transition = HighlightTransitionType.None;
            if (e.OldValue != DependencyProperty.UnsetValue && e.OldValue != null)
            {
                SyntaxHighlightStyleEnum old = e.OldValue.To<SyntaxHighlightStyleEnum>();
                if (LightStyles.Contains(style) && !LightStyles.Contains(old))
                {
                    // Dark to light transition needed
                    transition = HighlightTransitionType.DarkToLight;
                }
                else if (!LightStyles.Contains(style) && LightStyles.Contains(old))
                {
                    // Light to dark transition
                    transition = HighlightTransitionType.LightToDark;
                }
            }
            else
            {
                bool light = LightStyles.Contains(style);
                @this.FadeCanvas.Background = new SolidColorBrush(light ? Colors.White : Colors.Black);
                @this.WebControl.DefaultBackgroundColor = light ? Colors.White : Colors.Black;
            }

            // Function to load the sample HTML
            Task<String> htmlTask = @this.LoadHTMLPreviewAsync();

            // Await the out animations and the HTML loading
            List<Task> outTasks = new List<Task>
            {
                @this.BlurAsync(20, TimeSpan.FromMilliseconds(AnimationDuration)),
                htmlTask
            };
            if (transition != HighlightTransitionType.None)
            {
                @this.FadeCanvas.Background = new SolidColorBrush(transition == HighlightTransitionType.LightToDark ? Colors.Black : Colors.White);
                outTasks.Add(
                    @this.WebControl.StartCompositionFadeScaleAnimationAsync(1, 0.4f, 1, 1.05f, AnimationDuration, null, null, EasingFunctionNames.Linear));
            }
            else
            {
                outTasks.Add(
                    @this.WebControl.StartCompositionScaleAnimationAsync(1, 1.05f, AnimationDuration, null, EasingFunctionNames.Linear));
            }
            await Task.WhenAll(outTasks);

            // Show the HTML, animate back in and then release the semaphore
            @this.WebControl.SetVisualOpacity(1);
            @this.WebControl.NavigateToString(htmlTask.Result);
            await Task.WhenAll(
                @this.WebControl.StartCompositionScaleAnimationAsync(1.05f, 1, AnimationDuration, null, EasingFunctionNames.Linear),
                @this.BlurAsync(0, TimeSpan.FromMilliseconds(AnimationDuration)));
            if (@this.LoadingRing.Visibility == Visibility.Visible) @this.LoadingRing.Visibility = Visibility.Collapsed;
            @this.AnimationSemaphore.Release();
        }

        // Loads the right preview HTML content to display on the control
        private async Task<String> LoadHTMLPreviewAsync()
        {
            StringBuilder builder = new StringBuilder("ms-appx:///Services/Hilite-me/Samples/");
            if (LineNumbersVisible) builder.Append("LineNumbers/");
            builder.Append($"{HighlightStyle.ToString().ToLowerInvariant()}.html");
            Uri uri = new Uri(builder.ToString());
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            return await FileIO.ReadTextAsync(file);
        }
    }
}
