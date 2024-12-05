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
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand {
      new Argument<string>("name", "The name of the torrent file"),
      new Option<bool>("-tv", "Specify the TV category"),
      new Option<bool>("-movies", "Specify the Movies category"),
      new Option<bool>("-games", "Specify the Games category"),
      new Option<bool>("-music", "Specify the Music category")
    };

        var searchCommand =
            new Command("search", "Search for torrents") { new Argument<string>(
            "Name", "The name of the Torrent you wish to search for") };

        searchCommand.Handler = CommandHandler.Create<string>((commandName) => {
            Console.WriteLine($"Subcommand called: {commandName} Executed");
            return Task.CompletedTask;
        });

        rootCommand.Handler = CommandHandler.Create<string, bool, bool, bool, bool>(
            async (name, tv, movies, games, music) => {
                Config.InitializeConfig();

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
                var url = UrlHelper.BuildQueryUrl(name, categories);

                int max_pages = await PageHelper.GetMaxPages(url);
                int page = 1;

                List<List<Torrent>> torrentList = new List<List<Torrent>>();

                max_pages = Math.Min(Config.MaxPages, max_pages);
                while (page <= max_pages)
                {
                    await Task.Delay(1000);

                    var page_url =
                    (page == 1) ? url
                                : UrlHelper.BuildQueryUrl(name, categories, page);
                    var torrents = await PageHelper.ProcessPage(page_url);

                    torrentList.Add(torrents);
                    page++;
                }

                var allTorrents = torrentList.SelectMany(x => x).ToList();
                var sortedList =
                allTorrents.OrderByDescending(x => x.Seeders).ToList();
                TorrentHelper.DisplayTorrents(Config.MaxDisplay, sortedList);
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
                        var magnet = await TorrentHelper.GetMagnetUri(SelectedTorrent);
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
            });

        rootCommand.AddCommand(searchCommand);

        return await rootCommand.InvokeAsync(args);  // Correct invocation here
    }
}
