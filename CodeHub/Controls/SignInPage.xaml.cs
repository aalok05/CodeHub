using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CodeHub.Controls
{
    public sealed partial class SignInPage : UserControl
    {
        public SignInPage()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("SignInCommand", typeof(ICommand), typeof(SignInPage),null);
        public ICommand SignInCommand
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
    }
}
