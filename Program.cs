using HtmlAgilityPack;
using System.ComponentModel.Design;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
class Program
{
    static readonly HttpClient client = new HttpClient();
    public static async Task<string> GetHTTP(Uri uri)
    {
        try
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

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
        Console.WriteLine($"Command Line Arguments: {args}");
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
    public static int GetMaxPages(HtmlDocument html) {

        try //If there are number of pages, unpopular movies may not have this
        {
            var pagerDiv = html.DocumentNode.SelectSingleNode("//div[@id='pager_links']");
            Console.WriteLine(pagerDiv);
            var anchor_tags = pagerDiv.SelectNodes("./a");
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
    public static async Task Main(string[] args)
    {
        
        Dictionary<string, string> searchParams =  GetArgumentsCLI(args);
        string search = searchParams["search"];
        string category = searchParams["category"];

        //Creating initial URL
        string rarbgUrl = "https://rargb.to/";
        string url = rarbgUrl + $"search/?search={Uri.EscapeDataString(search)}&category[]={Uri.EscapeDataString(category)}";
        if (category == "all") { url = url.Split("&category[]")[0]; }
        Uri uri = new Uri(url);

        //Getting Page count
        var response = await GetHTTP(uri);
        var html = ParseHTTP(response);
        int max_pages = GetMaxPages(html);
        int page = 1;


        
        List<List<Torrent>> torrentList = new List<List<Torrent>>();

        //Main loop for page indexing
        max_pages = 2; //TESTING PURPOSES
        while (page<=max_pages) 
        {
            await Task.Delay(1000);

            //Get URL
            var page_url = rarbgUrl + $"search/{page}/?search={Uri.EscapeDataString(search)}&category[]={Uri.EscapeDataString(category)}";
            if (category == "all") { page_url = page_url.Split("&category[]")[0]; }
            Uri page_uri = new Uri(page_url);
            //Parse
            response = await GetHTTP(page_uri);
            html = ParseHTTP(response);
            
            Console.WriteLine(page_url);
            //Gets all Torrents from each site
            var torrents = GetTorrents(html);
            torrentList.Add(torrents);
            page++;

        }
        List<Torrent> sortedList = torrentList.SelectMany(t => t).OrderByDescending(t => t.Seeders).ToList();

        //Display torrents
        Console.WriteLine("Sorted List of Torrents by Seeders:");
        for(int i = 0; i<sortedList.Count; i++)
        {
            Console.WriteLine($"[{i}] | {sortedList[i].Href} | {sortedList[i].Seeders} | {DateTime.Parse( sortedList[i].Date).Year} ");
            Console.WriteLine("--------------------------------------------------------------");
        }




    }
}