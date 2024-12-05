using HtmlAgilityPack;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.ComponentModel;
using HttpRequests;

class Program
{
   

    
    public static void InitializeConfig()
    {
        string cwd = Directory.GetCurrentDirectory();
        string projectDirectory = Directory.GetParent(cwd).Parent.Parent.FullName;
        string fileName = "config.txt";
        string filePath = Path.Combine(projectDirectory, fileName);

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            Config.SaveToFile(filePath);
            Console.WriteLine($"Config.txt file created at {cwd}");
        }
    }

   

    public class Torrent
    {
        public string Title { get; set; }
        public int Seeders { get; set; }
        public int Leechers { get; set; }
        public string Size { get; set; }
        public string Date { get; set; }
        public string Href { get; set; }

        public Torrent(
            string title,
            int seeders,
            int leechers,
            string size,
            string date,
            string href
        )
        {
            Title = title;
            Seeders = seeders;
            Leechers = leechers;
            Size = size;
            Date = date;
            Href = href;
        }
    }

    public static string ShortenURL(string href)
    {
        string pattern = @"^/torrent/|\.html$";
        return Regex.Replace(href, pattern, "");
    }

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
                var shortenedHref = ShortenURL(href);
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
    public static string BuildQueryUrl(string name, List<string> categories, int page = 1)
    {
        var searchQuery = $"search/{page}/?search={Uri.EscapeDataString(name)}";
        var url = Config.BaseUrl + searchQuery;
        foreach (var category in categories)
        {
            string categoryString = $"&category[]={Uri.EscapeDataString(category)}";
            url += categoryString;
        }
        Console.WriteLine(url);
        return url;
        //var url = Config.BaseUrl + $"search/{page}/?search={Uri.EscapeDataString(search)}&category[]={Uri.EscapeDataString(category)}";
    }

    public static async Task<List<Torrent>> ProcessPage(string QueryURL)
    {
        string response = await HttpHelper.GetHTTP(new Uri(QueryURL));
        HtmlDocument html = HttpHelper.ParseHTTP(response);
        return GetTorrents(html);
    }

    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand
        {
            new Argument<string>("name", "The name of the torrent file"),
            new Option<bool>("-tv", "Specify the TV category"),
            new Option<bool>("-movies", "Specify the Movies category"),
            new Option<bool>("-games", "Specify the Games category"),
            new Option<bool>("-music", "Specify the Music category")
        };

        var searchCommand = new Command("search", "Search for torrents")
        {
            new Argument<string>("Name", "The name of the Torrent you wish to search for")
        };

        searchCommand.Handler = CommandHandler.Create<string>(
            (commandName) =>
            {
                Console.WriteLine($"Subcommand called: {commandName} Executed");
                return Task.CompletedTask;
            }
        );

        rootCommand.Handler = CommandHandler.Create<string, bool, bool, bool, bool>(
            async (name, tv, movies, games, music) =>
            {
                // InitializeConfig();



                List<string> categories = new List<string>();
                if (tv)
                {
                    categories.Add("tv");
                }
                if (movies)
                {
                    categories.Add("movies");
                }
                if (games)
                {
                    categories.Add("games");
                }
                if (music)
                {
                    categories.Add("music");
                }
                var url = BuildQueryUrl(name, categories);

                int max_pages = await GetMaxPages(url);
                int page = 1;

                List<List<Torrent>> torrentList = new List<List<Torrent>>();

                max_pages = Math.Min(Config.MaxPages, max_pages);
                while (page <= max_pages)
                {
                    await Task.Delay(1000);

                    var page_url = (page == 1) ? url : BuildQueryUrl(name, categories, page);
                    var torrents = await ProcessPage(page_url);

                    torrentList.Add(torrents);
                    page++;
                }

                var allTorrents = torrentList.SelectMany(x => x).ToList();
                var sortedList = allTorrents.OrderByDescending(x => x.Seeders).ToList();
                DisplayTorrents(Config.MaxDisplay, sortedList);
                while (true)
                {
                    Console.WriteLine("Select a Torrent: ");
                    string? input = Console.ReadLine();
                    if (input == "quit")
                    {
                        break;
                    }

                    try
                    {
                        int index = int.Parse(input);
                        Torrent SelectedTorrent = sortedList[index];
                        Console.WriteLine($"Selected torrent: {SelectedTorrent.Title}");
                        var magnet = await GetMagnetUri(SelectedTorrent);
                        await QbitTorrent.AddTorrent(magnet);
                        break;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Index does not exist.");
                        continue;
                    }
                    catch
                    {
                        Console.WriteLine("Torrent does not exist");
                    }
                }

                return 0;
            }
        );

        rootCommand.AddCommand(searchCommand);

        return await rootCommand.InvokeAsync(args); // Correct invocation here
    }
}
