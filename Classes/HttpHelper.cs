using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequests
{
    internal class HttpHelper 
    {
        static readonly HttpClient client = new HttpClient();
        public static async Task<string> GetHTTP(Uri uri)
        {
            try
            {
                client.DefaultRequestHeaders.Add("User-Agent", Config.UserAgent);
                using HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return "";
            }
            catch (Exception e)
            {
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
    }
}
