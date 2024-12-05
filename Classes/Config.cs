using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpRequests;

public static class Config
{
    public const string BaseUrl = "https://rargb.to/";
    public const string UserAgent =  "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";
    public static int MaxPages = 2;
    public static int MaxDisplay = 15;

    public static void InitializeConfig()
    {
        string cwd = Directory.GetCurrentDirectory();
        string projectDirectory = Directory.GetParent(cwd).Parent.Parent.FullName;
        string fileName = "config.txt";
        string filePath = Path.Combine(projectDirectory, fileName);

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            Config.SaveToFile(filePath);
            Console.WriteLine($"Config.txt file created at {cwd}");
        }
    }
    public static void SaveToFile(string filePath)
    {
        var configData = new { BaseUrl, UserAgent, MaxPages, MaxDisplay };

        string jsonString = JsonSerializer.Serialize(
            configData,
            new JsonSerializerOptions { WriteIndented = true }
        );

        using (
            var fileStream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None
            )
        )
        using (var writer = new StreamWriter(fileStream))
        {
            writer.Write(jsonString);
        }
        Console.WriteLine($"Configuration saved to {filePath}");
    }
}
