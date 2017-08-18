using CodeHub.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.Services.Store;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using CodeHub.Helpers;
using GalaSoft.MvvmLight.Messaging;
using Windows.System;
using CodeHub.Services;

namespace CodeHub.Views
{
    public sealed partial class DonateView : SettingsDetailPageBase
    {
        private const string donateFirstAddOnId = "[Donate_first_tier_id]";
        private const string donateSecondAddOnId = "[Donate_second_tier_id]";
        private const string donateThirdAddOnId = "[Donate_third_tier_id]";
        private const string donateFourthAddOnId = "[Donate_fourth_tier_id]";
        private const string donateFifthAddOnId = "[Donate_fifth_tier_id]";
        private const string donateSixthAddOnId = "[Donate_sixth_tier_id]";

        private static readonly StoreContext WindowsStore = StoreContext.GetDefault();

        private AppViewmodel ViewModel;
        public DonateView()
        {
            this.InitializeComponent();
            ViewModel = new AppViewmodel();
            this.DataContext = ViewModel;
        }
        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState != null)
                TryNavigateBackForDesktopState(e.NewState.Name);
        }

        private async void First_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.isLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateFirstAddOnId);
            ViewModel.isLoading = false;
            ReactToPurchaseResult(result);
        }
        private async void Second_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.isLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateSecondAddOnId);
            ViewModel.isLoading = false;
            ReactToPurchaseResult(result);
        }
        private async void Third_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.isLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateThirdAddOnId);
            ViewModel.isLoading = false;
            ReactToPurchaseResult(result);
        }
        private async void Fourth_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.isLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateFourthAddOnId);
            ViewModel.isLoading = false;
            ReactToPurchaseResult(result);
        }
        private async void Fifth_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.isLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateFifthAddOnId);
            ViewModel.isLoading = false;
            ReactToPurchaseResult(result);
        }
        private async void Sixth_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.isLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateSixthAddOnId);
            ViewModel.isLoading = false;
            ReactToPurchaseResult(result);
        }

        private void ReactToPurchaseResult(StorePurchaseResult result)
        {
            if (result.Status == StorePurchaseStatus.Succeeded)
            {
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType {
                    Message = "Thanks for your donation! I deeply appreciate your contribution to the development of CodeHub",
                    Glyph = "\uED54"
                });

                SettingsService.Save<bool>(SettingsKeys.IsAdsEnabled, false);
                GlobalHelper.HasAlreadyDonated = true;
                Messenger.Default.Send(new GlobalHelper.AdsEnabledMessageType());
            }
            else if (result.Status == StorePurchaseStatus.AlreadyPurchased)
            {
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType
                {
                    Message = "It seems you have already made this donation",
                    Glyph = "\uE783"
                });
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType
                {
                    Message = "There seems to be a problem. Try again later",
                    Glyph = "\uE783"
                });
            }
        }

        private async void PatreonButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(
               new Uri("https://www.patreon.com/aalok05"));
        }
    }
}
