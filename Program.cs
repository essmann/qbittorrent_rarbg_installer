using HtmlAgilityPack;
using System.Net;
using System.Text;
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
        Uri uri = new Uri(url);


        var response = await GetHTTP(uri);
        var html = ParseHTTP(response);
        int max_pages = GetMaxPages(html);

        do
        {

        } while (true);
        //Automation begins here

        ////title[@lang='en']
        //Find number of pages
        
        //Parses the HTTP response content and turns it into nodes and shit just like BeautifulSoup in python

        ////
        ///



    }
}