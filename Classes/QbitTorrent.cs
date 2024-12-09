using HttpRequests.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RarbgCLI.Classes
{

    public class QbitTorrent
    {

        private static readonly HttpClient client = new HttpClient();
        public static string qbittorrentUrl = "http://0.0.0.0:8080";

        public static string? Username { get; set; }
        public static string? Password { get; set; }

        private static bool isAuthenticated = false;

        private static bool EnvironmentVarChecked = false;

        public static void CheckAndSetEnvironmentVariables()
        {
            // Get the path from the environment variable
            string? programDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string? projectRoot = Directory.GetParent(programDirectory).Parent.Parent.Parent.FullName; //lmao
            Console.WriteLine(programDirectory);  // For debugging
            Console.WriteLine(projectRoot);  // For debugging
           
            string? filePath = "config.json";
           
            
            string fullPath = Path.Combine(projectRoot, filePath);
            
                // Read JSON from a file
                try
                {
                    string json = File.ReadAllText(fullPath);

                    // Deserialize into the Config object
                    JSONconfig? config = JsonSerializer.Deserialize<JSONconfig>(json);
                    //set variables
                    if (config.Password == null || config.Username == null)
                    {
                        throw new Exception($"Username or Password has not been set in {fullPath}");
                    }
                    // If the IP is valid, set it to qbittorrentUrl
                    if (Uri.IsWellFormedUriString(config.IPAddress, UriKind.Absolute))
                    {
                        qbittorrentUrl = config.IPAddress;
                        Console.WriteLine($"qbittorrentUrl set to: {qbittorrentUrl}");
                    }
                    else
                    {
                        throw new Exception("Invalid IP address in file.");
                    }
                    Username = config.Username;
                    Password = config.Password;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading file: {ex.Message}");
                }
            }
        


        public static async Task Authenticate()
        {
            if (!EnvironmentVarChecked)
            {
                CheckAndSetEnvironmentVariables();
                EnvironmentVarChecked = true;
            }

            if (isAuthenticated) return;

            var authContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("username", Username),
            new KeyValuePair<string, string>("password", Password)
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
