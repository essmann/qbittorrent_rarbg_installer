using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequests
{
    public class TorrentHelper
    {
        public static List<Torrent> GetTorrents(HtmlDocument html)
        {
            try
            {
                var tableRows = html.DocumentNode.SelectNodes("//tr[@class='lista2']");
                List<Torrent> torrents = new List<Torrent>();

                foreach (var tr in tableRows)
                {
                    var tds = tr.SelectNodes("./td");
                    var href = tds[1].SelectSingleNode("./a").Attributes[0].Value;
                    var shortenedHref = UrlHelper.ShortenURL(href);
                    string size = tds[4].InnerText;
                    int seeders = int.Parse(tds[5].ChildNodes[0].InnerText);
                    int leechers = int.Parse(tds[6].ChildNodes[0].InnerText);
                    string date = tds[3].InnerText;
                    var url = $"https://rargb.to{href}";
                    torrents.Add(new Torrent(url, seeders, leechers, size, date, shortenedHref));
                }
                return torrents;
            }
            catch
            {
                throw new Exception(
                    "Torrents HTML not found. Invalid URL or no Torrents for this URL."
                );
            }
        }

        public static async Task<string> GetMagnetUri(Torrent torrent)
        {
            string url = torrent.Title;
            Uri uri = new Uri(url);
            var response = await HttpHelper.GetHTTP(uri);
            var html = HttpHelper.ParseHTTP(response);
            var td = html.DocumentNode.SelectSingleNode("//td[@class='lista']");
            var anchors = td.SelectNodes("./a");
            return anchors[0].Attributes[2].Value;
        }

        public static void DisplayTorrents(int MaxPages, List<Torrent> sortedList)
        {
            Console.WriteLine("Sorted List of Torrents by Seeders:");
            if (sortedList.Count < MaxPages)
            {
                MaxPages = sortedList.Count;
            }
            for (int i = 0; i < MaxPages; i++)
            {
                Console.WriteLine(
                    $"[{i}] | {sortedList[i].Href} | {sortedList[i].Seeders} | {DateTime.Parse(sortedList[i].Date).Year}"
                );
                Console.WriteLine("--------------------------------------------------------------");
            }
        }
    }
}
