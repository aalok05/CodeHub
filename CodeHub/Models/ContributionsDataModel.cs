using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using CodeHub.Helpers;
using ColorHelper = Microsoft.Toolkit.Uwp.ColorHelper;

namespace CodeHub.Models
{
    public class ContributionsDataModel
    {
        public int Commits { get; }

        public DateTime Date { get; }

        public override String ToString()
        {
            if (Commits == 0) return $"No contributions on {Date:D}";
            if (Commits == 1) return $"1 commit on {Date:D}";
            return $"{Commits} commits on {Date:D}";
        }

        public ContributionsDataModel(int commits, DateTime date)
        {
            Commits = commits;
            Date = date;
        }

        public double Frequency { get; set; }

        public SolidColorBrush Fill
        {
            get
            {
                if (Commits == 0) return null;
                double frequency = Frequency;
                if (frequency > 1) frequency = 1;
                else if (frequency < 0) frequency = 0;
                Color color = XAMLHelper.GetResourceValue<Color>("AppPrimaryColor");
                var hls = ColorHelper.ToHsl(color);
                var delta = frequency < 0.5 ? frequency * 0.1 : -(frequency * 0.2);
                delta += hls.L;
                if (delta > 1) delta = 1;
                else if (delta < 0) delta = 0;
                hls.L = delta;
                color = frequency < 0.5 ? color.GetLight(frequency * 0.5) : color.GetLight(-(frequency * 0.5));
                return new SolidColorBrush(ColorHelper.FromHsl(hls.H, hls.S, hls.L));
            }
        }
    }
}
