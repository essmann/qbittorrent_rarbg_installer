using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequests
{
    public class PageHelper
    {
        public static async Task<int> GetMaxPages(string QueryURL)
        {
            string url = QueryURL;
            Uri uri = new Uri(url);
            var response = await HttpHelper.GetHTTP(uri);
            var html = HttpHelper.ParseHTTP(response);

            try
            {
                var pagerDiv = html.DocumentNode.SelectSingleNode("//div[@id='pager_links']");
                if (pagerDiv == null)
                    return 1;

                var anchorTags = pagerDiv.SelectNodes("./a");
                if (anchorTags == null || anchorTags.Count == 0)
                    return 1;
                var lastElement = anchorTags.Last();
                return lastElement.InnerText == ">>"
                  ? int.Parse(anchorTags[anchorTags.Count - 2].InnerText)
                  : int.Parse(lastElement.InnerText);
            }
            catch
            {
                return 1;
            }
        }


        public static async Task<List<Torrent>> ProcessPage(string QueryURL)
        {
            string response = await HttpHelper.GetHTTP(new Uri(QueryURL));
            HtmlDocument html = HttpHelper.ParseHTTP(response);
            return TorrentHelper.GetTorrents(html);
        }
    }
}
