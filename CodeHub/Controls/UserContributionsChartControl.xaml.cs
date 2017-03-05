using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CodeHub.Helpers;
using CodeHub.Models;
using JetBrains.Annotations;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CodeHub.Controls
{
    public sealed partial class UserContributionsChartControl : UserControl, INotifyPropertyChanged
    {
        public UserContributionsChartControl()
        {
            this.Loaded += (s, e) =>
            {
                width = (ActualWidth - LeftColumn.ActualWidth) / 53;
                OnPropertyChanged(nameof(width));
            };
            this.InitializeComponent();
        }

        public double width { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public String[] Months
        {
            get { return (String[])GetValue(MonthsProperty); }
            set { SetValue(MonthsProperty, value); }
        }

        public static readonly DependencyProperty MonthsProperty = DependencyProperty.Register(
            nameof(Months), typeof(ContributionsDataModel[,]), typeof(UserContributionsChartControl),
            new PropertyMetadata(default(String[]), OnMonthsPropertyChanged));

        private static void OnMonthsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UserContributionsChartControl @this = d.To<UserContributionsChartControl>();
            String[] months = e.NewValue.To<String[]>();
            if (months == null || months.Length < 12) return;
            @this.MonthBlock1.Text = months[0];
            @this.MonthBlock2.Text = months[1];
            @this.MonthBlock3.Text = months[2];
            @this.MonthBlock4.Text = months[3];
            @this.MonthBlock5.Text = months[4];
            @this.MonthBlock6.Text = months[5];
            @this.MonthBlock7.Text = months[6];
            @this.MonthBlock8.Text = months[7];
            @this.MonthBlock9.Text = months[8];
            @this.MonthBlock10.Text = months[9];
            @this.MonthBlock11.Text = months[10];
        }

        public ContributionsDataModel[,] Source
        {
            get { return (ContributionsDataModel[,])GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(ContributionsDataModel[,]), typeof(UserContributionsChartControl), 
            new PropertyMetadata(default(ContributionsDataModel[,]), OnSourcePropertyChanged));

        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Unpack the data
            UserContributionsChartControl @this = d.To<UserContributionsChartControl>();
            ContributionsDataModel[,] data = e.NewValue.To<ContributionsDataModel[,]>();
            if (data == null) return;

            // Get the max number of commits
            for (int i = 0; i < 7; i++)
            {
                for (int y = 0; y < 53; y++)
                {
                    if (data[i, y] == null)
                    {
                        data[i, y] = new ContributionsDataModel(0, DateTime.Now);
                    }
                }
            }

            int max = 0, valid = 0, sum = 0;
            foreach (ContributionsDataModel entry in data)
            {
                if (entry?.Commits > max) max = entry.Commits;
                if (entry?.Commits > 0)
                {
                    valid++;
                    sum += entry.Commits;
                }
            }
            int mean = sum / valid;

            foreach (ContributionsDataModel entry in data)
            {
                if (entry != null) entry.Frequency = (double)entry.Commits / mean;
            }
            @this.list.ItemsSource = data.Cast<ContributionsDataModel>();
            @this.LoadingRing.IsActive = false;
        }
    }
}
