using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CodeHub.ViewModels;

namespace CodeHub.Views
{
    public sealed partial class CommitsView : Page
    {
        public CommitsViewmodel ViewModel;

        public CommitsView()
        {
            InitializeComponent();
            ViewModel = new CommitsViewmodel();
            DataContext = ViewModel;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.Load(e.Parameter);
        }
    }
}
