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
        private static bool IPhasbeenRead = false;
        public static void SetQbittorrentUrlFromFile()
        {
            // Get the path from the environment variable
            string? basePath = Environment.GetEnvironmentVariable("rarbg_cli_path");
            string filePath = "server_url.txt";
            if (string.IsNullOrEmpty(basePath))
            {
                Console.WriteLine("Environment variable 'rarbg_cli_path' is not set.");
                return;
            }

            // Combine the base path (from environment variable) with the file path
            string fullPath = Path.Combine(basePath, filePath);
            try
            {
                
                // Read all text from the file (ensure file contains only the IP address)
                string ipAddress = File.ReadAllText(fullPath).Trim();

                // If the IP is valid, set it to qbittorrentUrl
                if (Uri.IsWellFormedUriString(ipAddress, UriKind.Absolute))
                {
                    qbittorrentUrl = ipAddress;
                    Console.WriteLine($"qbittorrentUrl set to: {qbittorrentUrl}");
                }
                else
                {
                    Console.WriteLine("Invalid IP address in file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
        }
        public static async Task Authenticate()
        {
            if (!IPhasbeenRead)
            {
                SetQbittorrentUrlFromFile();
                IPhasbeenRead = true;
            }
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
