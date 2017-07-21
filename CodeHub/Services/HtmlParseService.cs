using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using static CodeHub.ViewModels.TrendingViewmodel;
using CodeHub.Helpers;

namespace CodeHub.Services
{
    class HtmlParseService
    {
        /// <summary>
        /// Scrapes the HTML page and gets the repository name and owner name
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public async static Task<List<Tuple<string, string>>> ExtractTrendingRepos(TimeRange range)
        {
            /*The github API does not offer an endpoint to get trending repositories
            This make shift method is for scraping the html page and getting the
            repository name and owner name*/

            string url = "";

            List<Tuple<string, string>> repoNames = new List<Tuple<string, string>>();
            HtmlWeb web = new HtmlWeb();
            IEnumerable<HtmlNode> h3;
            HtmlDocument doc = new HtmlDocument();

            switch (range)
            {
                case TimeRange.TODAY:

                    url = "https://github.com/trending?since=daily";
                    doc = await web.LoadFromWebAsync(url);
                    h3 = doc.DocumentNode.Descendants("h3");
                    foreach (var i in h3)
                    {
                        var s = i.Descendants("a").First();
                        var names = s.Attributes["href"].Value.Split('/');
                        repoNames.Add(new Tuple<string, string>(names[1], names[2]));
                    }
                    GlobalHelper.TrendingTodayRepoNames = repoNames;

                    break;

                case TimeRange.WEEKLY:

                    url = "https://github.com/trending?since=weekly";
                    doc = await web.LoadFromWebAsync(url);
                    h3 = doc.DocumentNode.Descendants("h3");
                    foreach (var i in h3)
                    {
                        var s = i.Descendants("a").First();
                        var names = s.Attributes["href"].Value.Split('/');
                        repoNames.Add(new Tuple<string, string>(names[1], names[2]));
                    }
                    GlobalHelper.TrendingWeekRepoNames = repoNames;

                    break;

                case TimeRange.MONTHLY:

                    url = "https://github.com/trending?since=monthly";
                    doc = await web.LoadFromWebAsync(url);
                    h3 = doc.DocumentNode.Descendants("h3");
                    foreach (var i in h3)
                    {
                        var s = i.Descendants("a").First();
                        var names = s.Attributes["href"].Value.Split('/');
                        repoNames.Add(new Tuple<string, string>(names[1], names[2]));
                    }
                    GlobalHelper.TrendingMonthRepoNames = repoNames;

                    break;

            }

            return repoNames;
        }
    }
}
