using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace CodeHub.Converters
{
	class EventTypeToSymbolConverter : IValueConverter
	{
		/// <summary>
		/// Converts Event type to an SVG symbol
		/// The symbol indicates what action was done in an event
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			switch ((string)value)
			{
				case "ForkEvent":
					var forkSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">F0 M10,16z M0,0z M10,5C10,3.89,9.11,3,8,3A1.993,1.993,0,0,0,7,6.72L7,7.02C6.98,7.54 6.77,8 6.37,8.4 5.97,8.8 5.51,9.01 4.99,9.03 4.16,9.05 3.51,9.19 2.99,9.48L2.99,4.72A1.993,1.993,0,0,0,1.99,1C0.88,1,0,1.89,0,3A2,2,0,0,0,1,4.72L1,11.28C0.41,11.63 0,12.27 0,13 0,14.11 0.89,15 2,15 3.11,15 4,14.11 4,13 4,12.47 3.8,12 3.47,11.64 3.56,11.58 3.95,11.23 4.06,11.17 4.31,11.06 4.62,11 5,11 6.05,10.95 6.95,10.55 7.75,9.75 8.55,8.95 8.95,7.77 9,6.73L8.98,6.73C9.59,6.37,10,5.73,10,5z M2,1.8C2.66,1.8 3.2,2.35 3.2,3 3.2,3.65 2.65,4.2 2,4.2 1.35,4.2 0.8,3.65 0.8,3 0.8,2.35 1.35,1.8 2,1.8z M2,14.21C1.34,14.21 0.8,13.66 0.8,13.01 0.8,12.36 1.35,11.81 2,11.81 2.65,11.81 3.2,12.36 3.2,13.01 3.2,13.66 2.65,14.21 2,14.21z M8,6.21C7.34,6.21 6.8,5.66 6.8,5.01 6.8,4.36 7.35,3.81 8,3.81 8.65,3.81 9.2,4.36 9.2,5.01 9.2,5.66 8.65,6.21 8,6.21z</Geometry>";
					return (Geometry)XamlReader.Load(forkSymbol);

				case "PullRequestEvent":
					var pullReqSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">M11 11.28V5c-.03-.78-.34-1.47-.94-2.06C9.46 2.35 8.78 2.03 8 2H7V0L4 3l3 3V4h1c.27.02.48.11.69.31.21.2.3.42.31.69v6.28A1.993 1.993 0 0 0 10 15a1.993 1.993 0 0 0 1-3.72zm-1 2.92c-.66 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2zM4 3c0-1.11-.89-2-2-2a1.993 1.993 0 0 0-1 3.72v6.56A1.993 1.993 0 0 0 2 15a1.993 1.993 0 0 0 1-3.72V4.72c.59-.34 1-.98 1-1.72zm-.8 10c0 .66-.55 1.2-1.2 1.2-.65 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2zM2 4.2C1.34 4.2.8 3.65.8 3c0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2z</Geometry>";
					return (Geometry)XamlReader.Load(pullReqSymbol);

				case "IssueCommentEvent":
				case "PullRequestReviewCommentEvent":
				case "CommitCommentEvent":
					var CommentSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">M15 1H6c-.55 0-1 .45-1 1v2H1c-.55 0-1 .45-1 1v6c0 .55.45 1 1 1h1v3l3-3h4c.55 0 1-.45 1-1V9h1l3 3V9h1c.55 0 1-.45 1-1V2c0-.55-.45-1-1-1zM9 11H4.5L3 12.5V11H1V5h4v3c0 .55.45 1 1 1h3v2zm6-3h-2v1.5L11.5 8H6V2h9v6z</Geometry>";
					return (Geometry)XamlReader.Load(CommentSymbol);

				case "PushEvent":
					var pushSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">M10.86 7c-.45-1.72-2-3-3.86-3-1.86 0-3.41 1.28-3.86 3H0v2h3.14c.45 1.72 2 3 3.86 3 1.86 0 3.41-1.28 3.86-3H14V7h-3.14zM7 10.2c-1.22 0-2.2-.98-2.2-2.2 0-1.22.98-2.2 2.2-2.2 1.22 0 2.2.98 2.2 2.2 0 1.22-.98 2.2-2.2 2.2z</Geometry>";
					return (Geometry)XamlReader.Load(pushSymbol);

				case "IssuesEvent":
					var issueSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">M7 2.3c3.14 0 5.7 2.56 5.7 5.7s-2.56 5.7-5.7 5.7A5.71 5.71 0 0 1 1.3 8c0-3.14 2.56-5.7 5.7-5.7zM7 1C3.14 1 0 4.14 0 8s3.14 7 7 7 7-3.14 7-7-3.14-7-7-7zm1 3H6v5h2V4zm0 6H6v2h2v-2z</Geometry>";
					return (Geometry)XamlReader.Load(issueSymbol);

				case "WatchEvent":
					var watchSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">M14 6l-4.9-.64L7 1 4.9 5.36 0 6l3.6 3.26L2.67 14 7 11.67 11.33 14l-.93-4.74z</Geometry>";
					return (Geometry)XamlReader.Load(watchSymbol);


				default:
					var feedSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">M7 1C3.14 1 0 4.14 0 8s3.14 7 7 7c.48 0 .94-.05 1.38-.14-.17-.08-.2-.73-.02-1.09.19-.41.81-1.45.2-1.8-.61-.35-.44-.5-.81-.91-.37-.41-.22-.47-.25-.58-.08-.34.36-.89.39-.94.02-.06.02-.27 0-.33 0-.08-.27-.22-.34-.23-.06 0-.11.11-.2.13-.09.02-.5-.25-.59-.33-.09-.08-.14-.23-.27-.34-.13-.13-.14-.03-.33-.11s-.8-.31-1.28-.48c-.48-.19-.52-.47-.52-.66-.02-.2-.3-.47-.42-.67-.14-.2-.16-.47-.2-.41-.04.06.25.78.2.81-.05.02-.16-.2-.3-.38-.14-.19.14-.09-.3-.95s.14-1.3.17-1.75c.03-.45.38.17.19-.13-.19-.3 0-.89-.14-1.11-.13-.22-.88.25-.88.25.02-.22.69-.58 1.16-.92.47-.34.78-.06 1.16.05.39.13.41.09.28-.05-.13-.13.06-.17.36-.13.28.05.38.41.83.36.47-.03.05.09.11.22s-.06.11-.38.3c-.3.2.02.22.55.61s.38-.25.31-.55c-.07-.3.39-.06.39-.06.33.22.27.02.5.08.23.06.91.64.91.64-.83.44-.31.48-.17.59.14.11-.28.3-.28.3-.17-.17-.19.02-.3.08-.11.06-.02.22-.02.22-.56.09-.44.69-.42.83 0 .14-.38.36-.47.58-.09.2.25.64.06.66-.19.03-.34-.66-1.31-.41-.3.08-.94.41-.59 1.08.36.69.92-.19 1.11-.09.19.1-.06.53-.02.55.04.02.53.02.56.61.03.59.77.53.92.55.17 0 .7-.44.77-.45.06-.03.38-.28 1.03.09.66.36.98.31 1.2.47.22.16.08.47.28.58.2.11 1.06-.03 1.28.31.22.34-.88 2.09-1.22 2.28-.34.19-.48.64-.84.92s-.81.64-1.27.91c-.41.23-.47.66-.66.8 3.14-.7 5.48-3.5 5.48-6.84 0-3.86-3.14-7-7-7L7 1zm1.64 6.56c-.09.03-.28.22-.78-.08-.48-.3-.81-.23-.86-.28 0 0-.05-.11.17-.14.44-.05.98.41 1.11.41.13 0 .19-.13.41-.05.22.08.05.13-.05.14zM6.34 1.7c-.05-.03.03-.08.09-.14.03-.03.02-.11.05-.14.11-.11.61-.25.52.03-.11.27-.58.3-.66.25zm1.23.89c-.19-.02-.58-.05-.52-.14.3-.28-.09-.38-.34-.38-.25-.02-.34-.16-.22-.19.12-.03.61.02.7.08.08.06.52.25.55.38.02.13 0 .25-.17.25zm1.47-.05c-.14.09-.83-.41-.95-.52-.56-.48-.89-.31-1-.41-.11-.1-.08-.19.11-.34.19-.15.69.06 1 .09.3.03.66.27.66.55.02.25.33.5.19.63h-.01z</Geometry>";
					return (Geometry)XamlReader.Load(feedSymbol);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) 
			=> throw new NotImplementedException();
	}
}