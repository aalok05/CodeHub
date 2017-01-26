using GalaSoft.MvvmLight;
using Windows.UI.Xaml.Documents;

namespace CodeHub.Models
{
    public class SettingsItem : ObservableObject
    {
        public string _mainText;
        public string MainText
        {
            get
            {
                return _mainText;
            }
            set
            {
                Set(() => MainText, ref _mainText, value);
            }
        }
        public string _subText;
        public string SubText
        {
            get
            {
                return _subText;
            }
            set
            {
                Set(() => SubText, ref _subText, value);
            }
        }
        public string _glyphString;
        public string GlyphString
        {
            get
            {
                return _glyphString;
            }
            set
            {
                Set(() => GlyphString, ref _glyphString, value);
            }
        }
    }
}
