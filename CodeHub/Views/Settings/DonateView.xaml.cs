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

namespace CodeHub.Views.Settings
{
    public sealed partial class DonateView : SettingsDetailPageBase
    {
        private const string donateFirstAddOnId = "9pd0r1dxkt8j";
        private const string donateSecondAddOnId = "9msvqcz4pbws";
        private const string donateThirdAddOnId = "9n571g3nr2cs";
        private const string donateFourthAddOnId = "9nsmgzx3p43x";
        private const string donateFifthAddOnId = "9phrhpvhscdv";
        private const string donateSixthAddOnId = "9nnqdq0kq21j";

        private static readonly StoreContext WindowsStore = StoreContext.GetDefault();

        private AppViewmodel ViewModel;
        public DonateView()
        {
            InitializeComponent();
            ViewModel = new AppViewmodel();
            DataContext = ViewModel;
        }
        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState != null)
                TryNavigateBackForDesktopState(e.NewState.Name);
        }

        private async void First_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.IsLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateFirstAddOnId);
            ViewModel.IsLoading = false;
            ReactToPurchaseResult(result);
        }
        private async void Second_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.IsLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateSecondAddOnId);
            ViewModel.IsLoading = false;
            ReactToPurchaseResult(result);
        }
        private async void Third_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.IsLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateThirdAddOnId);
            ViewModel.IsLoading = false;
            ReactToPurchaseResult(result);
        }
        private async void Fourth_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.IsLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateFourthAddOnId);
            ViewModel.IsLoading = false;
            ReactToPurchaseResult(result);
        }
        private async void Fifth_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.IsLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateFifthAddOnId);
            ViewModel.IsLoading = false;
            ReactToPurchaseResult(result);
        }
        private async void Sixth_tier_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.IsLoading = true;
            StorePurchaseResult result = await WindowsStore.RequestPurchaseAsync(donateSixthAddOnId);
            ViewModel.IsLoading = false;
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
