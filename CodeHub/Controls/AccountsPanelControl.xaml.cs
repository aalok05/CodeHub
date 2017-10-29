using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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
    public sealed partial class AccountsPanelControl : UserControl
    {
        public AccountsPanelControl()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<Models.Account> Accounts
        {
            get { return (ObservableCollection<Models.Account>)GetValue(AccountsProperty); }
            set { SetValue(AccountsProperty, value); }
        }

        public static readonly DependencyProperty AccountsProperty =
            DependencyProperty.Register(nameof(Accounts), typeof(ObservableCollection<Models.Account>), typeof(AccountsPanelControl), new PropertyMetadata(false));


        public static readonly DependencyProperty SignInCommandProperty =
            DependencyProperty.Register(nameof(SignInCommand), typeof(ICommand), typeof(AccountsPanelControl), new PropertyMetadata(false));

        public ICommand SignInCommand
        {
            get { return (ICommand)GetValue(SignInCommandProperty); }
            set { SetValue(SignInCommandProperty, value); }
        }

        private async void CloseWhatsNew_Tapped(object sender, RoutedEventArgs e)
        {
            await this.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
            this.Visibility = Visibility.Collapsed;
        }

    }
}
