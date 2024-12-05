using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequests
{
    
    public  class QbitTorrent
    {
        
        private static readonly HttpClient client = new HttpClient();
        public static string qbittorrentUrl = "http://10.0.0.7:8080";
        public static string username = Environment.GetEnvironmentVariable("qbtusername");
        public static string password = Environment.GetEnvironmentVariable("qbtpassword");

        private static bool isAuthenticated = false;

        public static async Task Authenticate()
        {
            if (isAuthenticated) return;

            var authContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password)
        });

            var authResponse = await client.PostAsync($"{qbittorrentUrl}/api/v2/auth/login", authContent);
            if (!authResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to authenticate with qBittorrent.");
            }

            Console.WriteLine("Authenticated with qBittorrent.");
            isAuthenticated = true;
        }
        public static async Task AddTorrent(string magnetUri)
        {
            try
            {
                await Authenticate(); // Authenticate only if needed

                var addContent = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("urls", magnetUri)
            });

                var addResponse = await client.PostAsync($"{qbittorrentUrl}/api/v2/torrents/add", addContent);
                if (!addResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to add magnet link.");
                    return;
                }

                Console.WriteLine("Magnet link successfully added to qBittorrent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        
    }
       
    }
}
