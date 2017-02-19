using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using CodeHub.Helpers;
using Microsoft.Toolkit.Uwp;
using ColorHelper = Microsoft.Toolkit.Uwp.ColorHelper;

namespace CodeHub.Models
{
    /// <summary>
    /// Represents a single day in the contributions chart for a given user
    /// </summary>
    public class ContributionsDataModel
    {
        /// <summary>
        /// Gets the number of commits for the current instance
        /// </summary>
        public int Commits { get; }

        /// <summary>
        /// Gets the date of the current instance
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Returns a string that indicates the number of commits and the date for the current instance
        /// </summary>
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

        /// <summary>
        /// Gets or sets the calculated frequency value based on the average in the current chart
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Gets the right display color for the current instance based on the frequency value
        /// </summary>
        public SolidColorBrush Fill
        {
            get
            {
                // No fill color if no commits are present
                if (Commits == 0) return null;

                // Normalize the frequency in the [0..1] range
                double frequency = Frequency;
                if (frequency > 1) frequency = 1;
                else if (frequency < 0) frequency = 0;

                // Get the app HSL accent color and calculate the brightness delta
                Color color = XAMLHelper.GetResourceValue<Color>("AppPrimaryColor");
                HslColor hls = color.ToHsl();
                double delta = frequency < 0.5 ? frequency * 0.1 : -(frequency * 0.2);
                delta += hls.L;
                if (delta > 1) delta = 1;
                else if (delta < 0) delta = 0;
                hls.L = delta;
                return new SolidColorBrush(ColorHelper.FromHsl(hls.H, hls.S, hls.L));
            }
        }
    }
}
