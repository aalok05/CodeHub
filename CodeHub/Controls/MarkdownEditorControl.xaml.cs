using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
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

        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            EditZone.Focus(FocusState.Programmatic);
            base.OnGotFocus(e);
        }
        public string MarkdownText
        {
            get { return (string)GetValue(MarkdownTextProperty); }
            set
            {
                SetValue(MarkdownTextProperty, value);
                EditZone.Document.SetText(TextSetOptions.ApplyRtfDocumentDefaults, (string)GetValue(MarkdownTextProperty));
            }
        }

        public static readonly DependencyProperty MarkdownTextProperty =
            DependencyProperty.Register(nameof(MarkdownText), typeof(string), typeof(MarkdownEditorControl), new PropertyMetadata(false));

        private void EditZone_TextChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Previewer.Text = string.IsNullOrWhiteSpace(MarkdownText) ? "Nothing to Preview" : Toolbar.Formatter?.Text;
        }

        public async void MarkdownTextBlock_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
        }

    }
}
