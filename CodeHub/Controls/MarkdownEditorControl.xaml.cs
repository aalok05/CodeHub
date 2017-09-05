using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class MarkdownEditorControl : UserControl
    {
        public MarkdownEditorControl()
        {
            this.InitializeComponent();

            EditZone.Focus(FocusState.Programmatic);
        }

        public string MarkdownText
        {
            get { return (string)GetValue(MarkdownTextProperty); }
            set { SetValue(MarkdownTextProperty, value); }
        }

        public static readonly DependencyProperty MarkdownTextProperty =
            DependencyProperty.Register(nameof(MarkdownText), typeof(string), typeof(MarkdownEditorControl), new PropertyMetadata(false));

        private void EditZone_TextChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MarkdownText = Toolbar.Formatter?.Text;
            Previewer.Text = string.IsNullOrWhiteSpace(MarkdownText) ? "Nothing to Preview" : MarkdownText;
        }

        public async void MarkdownTextBlock_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
        }

    }
}
