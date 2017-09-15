using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Controls
{
    public sealed partial class MarkdownEditorControl : UserControl
    {
        public MarkdownEditorControl()
        {
            this.InitializeComponent();
        }

        public string MarkdownText
        {
            get { return (string)GetValue(MarkdownTextProperty); }
            set { SetValue(MarkdownTextProperty, value);    }
        }

        public static readonly DependencyProperty MarkdownTextProperty =
            DependencyProperty.Register(nameof(MarkdownText), typeof(string), typeof(MarkdownEditorControl), new PropertyMetadata(false));

        private void EditZone_TextChanged(object sender, RoutedEventArgs e)
        {
            MarkdownText = Toolbar.Formatter?.Text;
            Previewer.Text = string.IsNullOrWhiteSpace(Toolbar.Formatter?.Text) ? new Windows.ApplicationModel.Resources.ResourceLoader().GetString("markdownComment_NoContent") : Toolbar.Formatter?.Text;
        }

        public async void MarkdownTextBlock_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
        }

        public void SetMarkdowntext(string text)
        {
            EditZone.Document.SetText(TextSetOptions.ApplyRtfDocumentDefaults, text);
        }
    }
}
