using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.UI.Popups;
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
            reactToPurchaseResult(result);
        }
        private async void second_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateSecondAddOnId);
            reactToPurchaseResult(result);
        }
        private async void third_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateThirdAddOnId);
            reactToPurchaseResult(result);
        }
        private async void fourth_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateFourthAddOnId);
            reactToPurchaseResult(result);
        }

        private async void reactToPurchaseResult(StorePurchaseResult result)
        {
            if(result.Status == StorePurchaseStatus.Succeeded)
            {

                var messageDialog = new MessageDialog("Thanks for your donation! I deeply appreciate your contribution to the development of CodeHub.");

                messageDialog.Commands.Add(new UICommand("OK"));

                messageDialog.CancelCommandIndex = 0;

                await messageDialog.ShowAsync();
            }
            else if(result.Status == StorePurchaseStatus.AlreadyPurchased)
            {
                var messageDialog = new MessageDialog("It seems you have already made this donation.");

                messageDialog.Commands.Add(new UICommand("OK"));

                messageDialog.CancelCommandIndex = 0;

                await messageDialog.ShowAsync();
            }
            else
            {
                var messageDialog = new MessageDialog("There seems to be a problem. Try again later.");

                messageDialog.Commands.Add(new UICommand("OK"));

                messageDialog.CancelCommandIndex = 0;

                await messageDialog.ShowAsync();
            }
        }
    }
}
