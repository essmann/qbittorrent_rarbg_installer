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
            return url;
            //var url = Config.BaseUrl + $"search/{page}/?search={Uri.EscapeDataString(search)}&category[]={Uri.EscapeDataString(category)}";
        }
        public static string ShortenURL(string href)
        {
            string pattern = @"^/torrent/|\.html$";
            return Regex.Replace(href, pattern, "");
        }
        public static void GetUrlInfo(Torrent torrent)
        {
            string pattern = @"(?<resolution>\d{3,4}p)(?=-|_|$)|(?<codec>x265|x264|HEVC|DD\+)|(?<audio>5\.1|7\.1|Stereo)|(?<bluray>BluRay)|(?<extended>Extended|Unrated|Director's\sCut)|(?<rip>WEB-DL|WEBRip|BluRay|BDRip|CamRip)";



            MatchCollection matches = Regex.Matches(torrent.Title, pattern, RegexOptions.IgnoreCase);

            // Process matches
            foreach (Match match in matches)
            {
                if (match.Groups["resolution"].Success && string.IsNullOrEmpty(torrent.Res))
                {
                   
                    torrent.Res = match.Groups["resolution"].Value;
                }

                if (match.Groups["codec"].Success && string.IsNullOrEmpty(torrent.Codec))
                {
                    
                    torrent.Codec = match.Groups["codec"].Value;
                }

                if (match.Groups["audio"].Success && string.IsNullOrEmpty(torrent.Audio))
                {
                    torrent.Audio = match.Groups["audio"].Value;
                }

                if (match.Groups["bluray"].Success && string.IsNullOrEmpty(torrent.Bluray))
                {
                    
                    torrent.Bluray = match.Groups["bluray"].Value;
                }

                if (match.Groups["rip"].Success && string.IsNullOrEmpty(torrent.Rip))
                {
                    torrent.Rip = match.Groups["rip"].Value;
                }

                if (match.Groups["extended"].Success && string.IsNullOrEmpty(torrent.Extended))
                {
                    
                    torrent.Extended = match.Groups["extended"].Value;
                }
            }
        }

    }
}
