using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using CodeHub.Helpers;
using CodeHub.Services.Hilite_me;
using UICompositionAnimations;

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
            this.Unloaded += (s, e) => ElementCompositionPreview.SetElementChildVisual(BlurBorder, null);
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
        /// Gets or sets the currently selected code highlight style
        /// </summary>
        public SyntaxHighlightStyle HighlightStyle
        {
            get { return (SyntaxHighlightStyle)GetValue(HighlightStyleProperty); }
            set { SetValue(HighlightStyleProperty, value); }
        }

        public static readonly DependencyProperty HighlightStyleProperty = DependencyProperty.Register(
            nameof(HighlightStyle), typeof(SyntaxHighlightStyle), typeof(Hilite_meHighlightStylePreviewControl),
            new PropertyMetadata(DependencyProperty.UnsetValue, OnHighlightStylePropertyChanged));

        // Private semaphore to avoid concurrent accesses
        private readonly SemaphoreSlim AnimationSemaphore = new SemaphoreSlim(1);

        // The duration of each fade in/out animation
        private const int AnimationDuration = 400;

        private static async void OnHighlightStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Unpack the parameters and lock the UI
            Hilite_meHighlightStylePreviewControl @this = d.To<Hilite_meHighlightStylePreviewControl>();
            await @this.AnimationSemaphore.WaitAsync();
            SyntaxHighlightStyle style = e.NewValue.To<SyntaxHighlightStyle>();

            // Function to load the sample HTML
            Func<SyntaxHighlightStyle, Task<String>> f = async s =>
            {
                Uri uri = new Uri($"ms-appx:///Services/Hilite-me/Samples/{s.ToString().ToLowerInvariant()}.html");
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                return await FileIO.ReadTextAsync(file);
            };
            Task<String> htmlTask = f(style);

            // Await the out animations and the HTML loading
            await Task.WhenAll(
                @this.WebControl.StartCompositionFadeScaleAnimationAsync(1, 0.8f, 1, 1.05f, AnimationDuration, null, null, EasingFunctionNames.Linear),
                @this.BlurAsync(20, TimeSpan.FromMilliseconds(AnimationDuration)),
                htmlTask);

            // Show the HTML, animate back in and then release the semaphore
            @this.WebControl.NavigateToString(htmlTask.Result);
            await Task.WhenAll(
                @this.WebControl.StartCompositionFadeScaleAnimationAsync(null, 1, 1.05f, 1, AnimationDuration, null, null, EasingFunctionNames.Linear),
                @this.BlurAsync(0, TimeSpan.FromMilliseconds(AnimationDuration)));
            @this.AnimationSemaphore.Release();
        }
    }
}
