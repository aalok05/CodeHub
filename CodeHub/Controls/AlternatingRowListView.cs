using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CodeHub.Controls
{
    public class AlternatingRowListView : ListView
    {
        public static readonly DependencyProperty OddRowBackgroundProperty =
            DependencyProperty.Register(nameof(OddRowBackground), typeof(Brush), typeof(AlternatingRowListView), null);

        public Brush OddRowBackground
        {
            get { return (Brush)GetValue(OddRowBackgroundProperty); }
            set { SetValue(OddRowBackgroundProperty, (Brush)value); }
        }

        public static readonly DependencyProperty EvenRowBackgroundProperty =
            DependencyProperty.Register(nameof(EvenRowBackground), typeof(Brush), typeof(AlternatingRowListView), null);

        public Brush EvenRowBackground
        {
            get { return (Brush)GetValue(EvenRowBackgroundProperty); }
            set { SetValue(EvenRowBackgroundProperty, (Brush)value); }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ListViewItem listViewItem = element as ListViewItem;
            if (listViewItem != null)
            {
                int index = IndexFromContainer(element);

                listViewItem.Background = (index + 1) % 2 == 1 ? OddRowBackground : EvenRowBackground;
            }

        }
    }

}
