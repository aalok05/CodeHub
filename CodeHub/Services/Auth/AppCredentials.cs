using System;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace CodeHub.Services
{
	class AppCredentials
	{
		/* These methods get App key and secret from an xml file called app.config. Create your app.config in the followinf format:
		* 
		*     <?xml version="1.0" encoding="utf-8" ?>
		*       <configuration>
		*          <appSettings>
		*            <add key="AppKey" value="[key here]"/>
		*            <add key="AppSecret" value="[secret here]"/>
		*          </appSettings>
		*        </configuration>
		*/

		public static async Task<string> GetAppKey()
		{
			var file = await StorageFile
				   .GetFileFromApplicationUriAsync(new Uri(string.Format("ms-appx:///app.config")));

			var xmlConfiguration = await XmlDocument.LoadFromFileAsync(file);

			var node = xmlConfiguration
						.DocumentElement
						.SelectSingleNode("./appSettings/add[@key='AppKey']/@value");

			if (node.NodeValue == null)
			{
				return null;
			}

			return (string)node.NodeValue;
		}
		public static async Task<string> GetAppSecret()
		{
			var file = await StorageFile
				    .GetFileFromApplicationUriAsync(new Uri(string.Format("ms-appx:///app.config")));

			var xmlConfiguration = await XmlDocument.LoadFromFileAsync(file);

			var node = xmlConfiguration
						.DocumentElement
						.SelectSingleNode("./appSettings/add[@key='AppSecret']/@value");
			if (node.NodeValue == null)
			{
				return null;
			}

			return (string)node.NodeValue;
		}
	}
}
