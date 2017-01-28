using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace CodeHub.Views
{
    public sealed partial class DonateView : SettingsDetailPageBase
    {
        private const string donateFirstAddOnId = "[Donate_first_tier_id]";
        private const string donateSecondAddOnId = "[Donate_second_tier_id]";
        private const string donateThirdAddOnId = "[Donate_third_tier_id]";
        private const string donateFourthAddOnId = "[Donate_fourth_tier_id]";

        private static readonly StoreContext WindowsStore = StoreContext.GetDefault();
        public DonateView()
        {
            this.InitializeComponent();
        }
        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            TryNavigateBackForDesktopState(e.NewState.Name);
        }

        private async void first_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateFirstAddOnId);
        }
        private async void second_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateSecondAddOnId);
        }
        private async void third_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateThirdAddOnId);
        }
        private async void fourth_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateFourthAddOnId);
        }
    }
}
