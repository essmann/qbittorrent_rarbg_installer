using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequests
{
    public class Torrent
    {
        public string Title { get; set; }
        public int Seeders { get; set; }
        public int Leechers { get; set; }
        public string Size { get; set; }
        public string Date { get; set; }
        public string Href { get; set; }
        public string? Res { get; set; }
        public string? Codec { get; set; }
        public string? Rip { get; set; }

        public string? Bluray { get; set; }
        public string? Audio { get; set; }
        public string? Extended { get; set; }
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
}
