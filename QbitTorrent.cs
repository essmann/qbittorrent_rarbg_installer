using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequests
{
    public  class QbitTorrent
    {
        public static string qbittorrentUrl = "http://localhost:8080";
        public static string username = "essmann";
        public static string password = "123456";

        public static async Task AddTorrent(string magnetUri)
        {
            try
            {
                using HttpClient client = new HttpClient();

                // Step 1: Authenticate
                var authContent = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

                var authResponse = await client.PostAsync($"{qbittorrentUrl}/api/v2/auth/login", authContent);
                if (!authResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to authenticate with qBittorrent.");
                    return;
                }

                Console.WriteLine("Authenticated with qBittorrent.");

                // Step 2: Add Magnet URI
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
