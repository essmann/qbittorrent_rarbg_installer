using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.CommandLine;
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

            // Set fixed column widths for index, seeders, URL, and details
            int indexWidth = 5;
            int seedersWidth = 10;
            int urlColumnWidth = 50;  // For the URL column
            int detailsColumnWidth = 40; // For the Details column (adjustable)

            // Print headers with adjusted spacing
            Console.WriteLine("{0,-" + indexWidth + "} | {1,-" + seedersWidth + "} | {2,-" + urlColumnWidth + "} | {3,-" + detailsColumnWidth + "}",
                "Index", "Seeders", "Torrent URL", "Details");

            if (sortedList.Count < MaxPages)
            {
                MaxPages = sortedList.Count;
            }

            for (int i = 0; i < MaxPages; i++)
            {
                // Limit URL length if necessary
                string truncatedUrl = sortedList[i].Href.Length > urlColumnWidth ? sortedList[i].Href.Substring(0, urlColumnWidth - 3) + "..." : sortedList[i].Href;

                // Construct the details part with resolution, rip, and codec
                string details = $"Resolution: {sortedList[i].Res ?? "N/A"}, Rip: {sortedList[i].Rip ?? "N/A"}, Codec: {sortedList[i].Codec ?? "N/A"}";

                // Display the index, seeders, URL, and details with proper formatting
                Console.WriteLine("{0,-" + indexWidth + "} | {1,-" + seedersWidth + "} | {2,-" + urlColumnWidth + "} | {3,-" + detailsColumnWidth + "}",
                    i, sortedList[i].Seeders, truncatedUrl, details);
            }
        }

    }
}
