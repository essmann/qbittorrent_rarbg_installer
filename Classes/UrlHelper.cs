using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpRequests
{
    public class UrlHelper
    {
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
        public static string ShortenURL(string href)
        {
            string pattern = @"^/torrent/|\.html$";
            return Regex.Replace(href, pattern, "");
        }

    }
}
