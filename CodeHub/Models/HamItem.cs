using GalaSoft.MvvmLight;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CodeHub.Models
{
    public class HamItem : ObservableObject
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                SelectedVisual = value ? Visibility.Visible : Visibility.Collapsed;
                RaisePropertyChanged("IsSelected");
            }
        }

        private Visibility _selectedVisual = Visibility.Collapsed;
        public Visibility SelectedVisual
        {
            get { return _selectedVisual; }
            set
            {
                _selectedVisual = value;
                RaisePropertyChanged("SelectedVisual");
            }
        }

        public  string Label { get; set; }
        public Geometry Symbol { get; set; }
        public Type DestPage { get; set; }
    }
}
