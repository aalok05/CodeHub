using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using static CodeHub.ViewModels.HomeViewmodel;

namespace CodeHub.Services
{
    class HtmlParseService
    {
        /*The github API does not offer an endpoint to get trending repositories
         This make shift method is for scraping the html page and getting the
         repository name and owner name*/
        public async static Task<List<Tuple<string, string>>> ExtractTrendingRepos(TimeRange range)
        {
            string url = "";

            List<Tuple<string, string>> repoNames = new List<Tuple<string, string>>();

            if (range == TimeRange.TODAY)
            {
                url = "https://github.com/trending?since=daily";
            }
            if (range == TimeRange.WEEKLY)
            {
                url = "https://github.com/trending?since=weekly";
            }
            if (range == TimeRange.MONTHLY)
            {
                url = "https://github.com/trending?since=monthly";
            }


            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(url);
            var h3 = doc.DocumentNode.Descendants("h3");

            foreach (var i in h3)
            {
                var s = i.Descendants("a").First();
                var names = s.Attributes["href"].Value.Split('/');
                repoNames.Add(new Tuple<string, string>(names[1], names[2]));
            }
            return repoNames;
        }
    }
}
