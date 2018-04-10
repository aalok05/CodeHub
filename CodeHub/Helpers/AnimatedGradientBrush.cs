using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CodeHub.Helpers
{
    //This will be used for the new sign in experience
    public class AnimatedGradientBrush : XamlCompositionBrushBase
    {
        private CompositionLinearGradientBrush _gradientBrush;

        private static readonly Color GradientStop1StartColor = Color.FromArgb(255, 251, 218, 97);
        private static readonly Color GradientStop2StartColor = Color.FromArgb(255, 255, 90, 205);

        protected override void OnConnected()
        {
            var compositor = Window.Current.Compositor;

            // Assign the gradient brush to the CompositionBrush.
            _gradientBrush = compositor.CreateLinearGradientBrush();
            CompositionBrush = _gradientBrush;

            // Initially, we set the end point to be (0,0) 'cause we want to animate it at start.
            // If you don't want this behavior, simply set it to a different value within (1,1).
            _gradientBrush.EndPoint = Vector2.Zero;

            // Create gradient initial colors.
            var gradientStop1 = compositor.CreateColorGradientStop();
            gradientStop1.Offset = 0.0f;
            gradientStop1.Color = GradientStop1StartColor;
            var gradientStop2 = compositor.CreateColorGradientStop();
            gradientStop2.Offset = 1.0f;
            gradientStop2.Color = GradientStop2StartColor;
            _gradientBrush.ColorStops.Add(gradientStop1);
            _gradientBrush.ColorStops.Add(gradientStop2);

            Window.Current.SizeChanged += OnWindowSizeChanged;

            // There are 3 animations going on here:
            //
            // First, we kick off an EndPoint offset animation to smoothly transition a
            // solid background to a gradient.
            //
            // Once it's finished, we then kick off TWO other animations simultaneously. 
            // These TWO animations include a set of gradient stop color animations and
            // a rotation animation that rotates the gradient brush.

            var linearEase = compositor.CreateLinearEasingFunction();
            var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += (s, e) =>
            {
                StartGradientColorAnimations();
                StartGradientRotationAnimation();
            };
            var endPointOffsetAnimation = compositor.CreateVector2KeyFrameAnimation();
            endPointOffsetAnimation.Duration = TimeSpan.FromSeconds(3);
            endPointOffsetAnimation.InsertKeyFrame(1.0f, Vector2.One);
            _gradientBrush.StartAnimation(nameof(_gradientBrush.EndPoint), endPointOffsetAnimation);
            batch.End();

            void StartGradientColorAnimations()
            {
                var color1Animation = compositor.CreateColorKeyFrameAnimation();
                color1Animation.Duration = TimeSpan.FromSeconds(10);
                color1Animation.IterationBehavior = AnimationIterationBehavior.Forever;
                color1Animation.Direction = AnimationDirection.Alternate;
                color1Animation.InsertKeyFrame(0.0f, GradientStop1StartColor, linearEase);
                color1Animation.InsertKeyFrame(0.5f, Color.FromArgb(255, 65, 88, 208), linearEase);
                color1Animation.InsertKeyFrame(1.0f, Color.FromArgb(255, 43, 210, 255), linearEase);
                gradientStop1.StartAnimation(nameof(gradientStop1.Color), color1Animation);

                var color2Animation = compositor.CreateColorKeyFrameAnimation();
                color2Animation.Duration = TimeSpan.FromSeconds(10);
                color2Animation.IterationBehavior = AnimationIterationBehavior.Forever;
                color2Animation.Direction = AnimationDirection.Alternate;
                color2Animation.InsertKeyFrame(0.0f, GradientStop2StartColor, linearEase);
                color1Animation.InsertKeyFrame(0.5f, Color.FromArgb(255, 200, 80, 192), linearEase);
                color2Animation.InsertKeyFrame(1.0f, Color.FromArgb(255, 43, 255, 136), linearEase);
                gradientStop2.StartAnimation(nameof(gradientStop2.Color), color2Animation);
            }

            void StartGradientRotationAnimation()
            {
                var rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
                rotationAnimation.Duration = TimeSpan.FromSeconds(15);
                rotationAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
                rotationAnimation.InsertKeyFrame(1.0f, 360.0f, linearEase);
                _gradientBrush.StartAnimation(nameof(_gradientBrush.RotationAngleInDegrees), rotationAnimation);
            }
        }

        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e) =>
            _gradientBrush.CenterPoint = e.Size.ToVector2() / 2;

        protected override void OnDisconnected()
        {
            Window.Current.SizeChanged -= OnWindowSizeChanged;

            CompositionBrush?.Dispose();
            CompositionBrush = null;
        }
    }

}
