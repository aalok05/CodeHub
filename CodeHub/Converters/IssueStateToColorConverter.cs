using CodeHub.Helpers;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CodeHub.Converters
{
    public class IssueStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (((Issue)value).State.TryParse(out ItemState state))
            {
                switch (state)
                {
                    case ItemState.Open:
                        return GlobalHelper.GetSolidColorBrush("2CBE4EFF");
                }
            }

            return GlobalHelper.GetSolidColorBrush("CB2431FF");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
