using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using JetBrains.Annotations;

namespace CodeHub.Models
{
    /// <summary>
    /// A class that represents a pinned repository from a user page
    /// </summary>
    public sealed class PinnedUserRepository
    {
        /// <summary>
        /// Gets the name of the repository
        /// </summary>
        [NotNull]
        public String Name { get; }

        /// <summary>
        /// Gets the public HTML url for the repository
        /// </summary>
        [NotNull]
        public Uri URL { get; }
        
        /// <summary>
        /// Gets the description of the repository
        /// </summary>
        [CanBeNull]
        public String Description { get; }

        /// <summary>
        /// Gets the repository main language
        /// </summary>
        [NotNull]
        public String Language { get; }

        /// <summary>
        /// Gets the theme color of the repository language
        /// </summary>
        public SolidColorBrush LanguageBrush => new SolidColorBrush(LanguageColor);

        private readonly Color LanguageColor;

        /// <summary>
        /// Gets the number of stars for the repository
        /// </summary>
        public int Stars { get; }

        /// <summary>
        /// Gets the number of times this repository has been forked
        /// </summary>
        public int Forks { get; }

        public PinnedUserRepository([NotNull] String name, [NotNull] String url, [CanBeNull] String description,
            String language, Color color, int stars, int forks)
        {
            Name = name;
            URL = new Uri($"https://www.github.com{url}");
            Description = description;
            Language = language;
            LanguageColor = color;
            Stars = stars;
            Forks = forks;
        }
    }
}
