using HtmlAgilityPack;
using HttpRequests;
using System.ComponentModel.Design;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
class Program
{
    static readonly HttpClient client = new HttpClient();
    public static class Config
    {
        public const string BaseUrl = "https://rargb.to/";
        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";
        public static int MaxPages = 2;
        public static int MaxDisplay = 15;

        public static void SaveToFile(string filePath)
        {
            var configData = new
            {
                BaseUrl,
                UserAgent,
                MaxPages,
                MaxDisplay
            };

            string jsonString = JsonSerializer.Serialize(configData, new JsonSerializerOptions { WriteIndented = true });

            // Ensure proper file access
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(jsonString);
            }
            Console.WriteLine($"Configuration saved to {filePath}");
        }
        
       

    }
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
                
                //.Close();
            Console.WriteLine($"Config.txt file created at {cwd}");
        }
        else
        {
                //Pass for now
        }
    }
    public static async Task<string> GetHTTP(Uri uri)
    {
        try
        {
            client.DefaultRequestHeaders.Add("User-Agent", Config.UserAgent);

            // Send a GET request asynchronously
            using HttpResponseMessage response = await client.GetAsync(uri);

            // Ensure the request was successful (Status Code 200-299)
            response.EnsureSuccessStatusCode();

            // Read the content of the response as a string asynchronously
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
        catch (HttpRequestException e)
        {
            // Handle specific HttpRequestException errors
            Console.WriteLine($"Request error: {e.Message}");
            return "";
        }
        catch (Exception e)
        {
            // Handle other exceptions (e.g., network issues)
            Console.WriteLine($"An error occurred: {e.Message}");
            return "";
        }
    

}
    public static HtmlDocument ParseHTTP(string response)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(response);
        return htmlDoc;

    }
    public static Dictionary<string, string> GetArgumentsCLI(string[] args)
    {
        Dictionary<string, string> searchParams = new Dictionary<string, string>();
       
        try
        {
            if (args.Length == 0) { throw new Exception("No arguments inputted"); }
            searchParams["search"] = args[0]; 
            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    if ( arg == "-tv"){
                        searchParams["category"] = arg.Replace("-", "");
                    }
                    else if(arg == "-all")
                    {
                        searchParams["category"] = arg.Replace("-", "");
                    }
                    else if(arg == "-movies")
                    {
                        searchParams["category"] = arg.Replace("-", "");
                    }
                }

            }
            return searchParams;
        }

        catch
        {
            throw new Exception(); //temporary
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
       public Torrent(string title, int seeders, int leechers, string size, string date, string href) {
            this.Title = title;
            this.Seeders = seeders;
            this.Leechers = leechers;
            this.Size = size;
            this.Date = date;
            this.Href = href;
        }
    }
    public static string ShortenURL(string href) {

        string pattern = @"^/torrent/|\.html$";
        string result = Regex.Replace(href, pattern, "");
        return result;


    }
    public void FilterByQuality() { }
    public static List<Torrent> GetTorrents(HtmlDocument html) {
        try
        {
            var tableRows = html.DocumentNode.SelectNodes("//tr[@class='lista2']");


            List<Torrent> torrents = new List<Torrent>();
            
            foreach (var tr in tableRows)
            {
                var tds = tr.SelectNodes("./td");

                var href = tds[1].SelectSingleNode("./a").Attributes[0].Value;
                var shortenedHref =  ShortenURL(href);
                string size = tds[4].InnerText;
                int seeders = int.Parse(tds[5].ChildNodes[0].InnerText);
                int leechers = int.Parse(tds[6].ChildNodes[0].InnerText);
                string date = tds[3].InnerText;
                var url = $"https://rargb.to{href}";
                Torrent torrent = new Torrent(url, seeders, leechers, size, date, shortenedHref);
                torrents.Add(torrent);

            }
            return torrents;
        }
        catch
        {
            throw new Exception("Torrents HTML not found. Invalid URL or no Torrents for this URL.");
        }
    }
    public static async Task<string> GetMagnetUri(Torrent torrent)
    {
        string url = torrent.Title;
        Uri uri = new Uri(url);
        var response = await GetHTTP(uri);
        var html = ParseHTTP(response);

        var td = html.DocumentNode.SelectSingleNode("//td[@class='lista']");
        var anchors = td.SelectNodes("./a");
        var magnet = anchors[0].Attributes[2].Value;
        return magnet;
    }
    public static void DisplayTorrents(int MaxPages, List<Torrent> sortedList) {
        //Display torrents
        Console.WriteLine("Sorted List of Torrents by Seeders:");
        if(sortedList.Count < MaxPages) { MaxPages = sortedList.Count; }
        for (int i = 0; i < MaxPages; i++)
        {
            Console.WriteLine($"[{i}] | {sortedList[i].Href} | {sortedList[i].Seeders} | {DateTime.Parse(sortedList[i].Date).Year} ");
            Console.WriteLine("--------------------------------------------------------------");
        }

    }
    public static async Task<int> GetMaxPages(string search, string category) {

        string url = Config.BaseUrl + $"search/?search={Uri.EscapeDataString(search)}&category[]={Uri.EscapeDataString(category)}";
        if (category == "all") { url = url.Split("&category[]")[0]; }
        Uri uri = new Uri(url);
        var response = await GetHTTP(uri);
        var html = ParseHTTP(response);
        try //If there are number of pages, unpopular movies may not have this
        {
            var pagerDiv = html.DocumentNode.SelectSingleNode("//div[@id='pager_links']");
            if (pagerDiv == null) return 1;
           
            var anchor_tags = pagerDiv.SelectNodes("./a");
            if (anchor_tags == null || anchor_tags.Count == 0) return 1;
            var last_element = anchor_tags.Last();
            int length = anchor_tags.Count();
            if (last_element.InnerText == ">>")
            {

                var second_to_last = anchor_tags[length - 2];
                int max_pages = int.Parse(second_to_last.InnerText);
                return max_pages;
            }
            else
            {
                int max_pages = int.Parse(last_element.InnerText);
                return max_pages;
            }


        }
        catch //shit movies with like 4-5 links like battleship potemkin
        {
            int max_pages = 1;
            return max_pages;
        }
    }
    private static string BuildPageUrl(int page, string search, string category)
    {
        var page_url = Config.BaseUrl + $"search/{page}/?search={Uri.EscapeDataString(search)}&category[]={Uri.EscapeDataString(category)}";
        if (category == "all") { page_url = page_url.Split("&category[]")[0]; }
        return page_url;
    }
    public static async Task<List<Torrent>> ProcessPage(int page, string search, string category)
    {
        string pageUrl = BuildPageUrl(page, search, category);
        string response = await GetHTTP(new Uri(pageUrl));
        HtmlDocument html = ParseHTTP(response);
        return GetTorrents(html);
    }
    public static async Task Main(string[] args)
    {
        InitializeConfig();
        Dictionary<string, string> searchParams =  GetArgumentsCLI(args);
        string search = searchParams["search"];
        string category = searchParams["category"];

        int max_pages = await GetMaxPages(search, category);
        int page = 1;

        List<List<Torrent>> torrentList = new List<List<Torrent>>();

        //Main loop for page indexing
        max_pages = Math.Min(Config.MaxPages, max_pages);
        while (page<=max_pages) 
        {
            await Task.Delay(1000);

            var page_url = BuildPageUrl(page, search, category);
            var torrents = await ProcessPage(page, search, category);
            torrentList.Add(torrents);
            page++;

        }
        List<Torrent> sortedList = torrentList.SelectMany(t => t).OrderByDescending(t => t.Seeders).ToList();

        DisplayTorrents(Config.MaxDisplay, sortedList); //-> prints all torrents

        while (true)
        {
            Console.WriteLine("Select a Torrent: ");
            string? input = Console.ReadLine();
            if(input == "quit") { break; }

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
            catch {

                Console.WriteLine("Torrent does not exist");
            }
        }


    }
}