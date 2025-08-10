using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YouTubeDownloader.Core.Services;
using YouTubeDownloader.Core.Models;

namespace YouTubeDownloader.Console;

static class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        System.Console.WriteLine("YouTube Downloader .NET");
        System.Console.WriteLine("=======================");

        await ShowMenu(host.Services);
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<IDownloadService, DownloadService>();
            });

    static async Task ShowMenu(IServiceProvider services)
    {
        var downloadService = services.GetRequiredService<IDownloadService>();

        while (true)
        {
            System.Console.WriteLine("\n=== MAIN MENU ===");
            System.Console.WriteLine("1. Download Video");
            System.Console.WriteLine("2. Download Audio Only");
            System.Console.WriteLine("3. Get Video Info");
            System.Console.WriteLine("4. Exit");
            System.Console.Write("\nSelect option (1-4): ");

            var choice = System.Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await DownloadVideo(downloadService);
                        break;
                    case "2":
                        await DownloadAudio(downloadService);
                        break;
                    case "3":
                        await ShowVideoInfo(downloadService);
                        break;
                    case "4":
                        return;
                    default:
                        System.Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }
        }
    }

    static async Task DownloadVideo(IDownloadService downloadService)
    {
        System.Console.WriteLine("\n=== VIDEO DOWNLOAD ===");
        System.Console.Write("Enter YouTube URL: ");
        var url = System.Console.ReadLine();

        if (string.IsNullOrWhiteSpace(url)) return;

        System.Console.Write("Enter output directory (or press Enter for Desktop): ");
        var outputPath = System.Console.ReadLine();
        if (string.IsNullOrWhiteSpace(outputPath))
            outputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        System.Console.Write("Enter quality (360p, 720p, 1080p) or press Enter for best: ");
        var quality = System.Console.ReadLine();
        if (string.IsNullOrWhiteSpace(quality)) quality = "720p";

        var options = new DownloadOptions
        {
            OutputPath = outputPath,
            Quality = quality,
            Format = "mp4"
        };

        var progress = new Progress<DownloadProgress>(p =>
        {
            System.Console.Write($"\rProgress: {p.Percentage:F1}% ({FormatBytes(p.DownloadedBytes)}/{FormatBytes(p.TotalBytes)})");
        });

        System.Console.WriteLine("Starting download...");
        await downloadService.DownloadVideoAsync(url, outputPath, options, progress);
        System.Console.WriteLine("\nDownload completed!");

        System.Console.WriteLine("Press any key to continue...");
        System.Console.ReadKey();
    }

    static async Task DownloadAudio(IDownloadService downloadService)
    {
        System.Console.WriteLine("\n=== AUDIO DOWNLOAD ===");
        System.Console.Write("Enter YouTube URL: ");
        var url = System.Console.ReadLine();

        if (string.IsNullOrWhiteSpace(url)) return;

        System.Console.Write("Enter output directory (or press Enter for Desktop): ");
        var outputPath = System.Console.ReadLine();
        if (string.IsNullOrWhiteSpace(outputPath))
            outputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        System.Console.Write("Enter audio format (mp3, aac, ogg) or press Enter for mp3: ");
        var format = System.Console.ReadLine();
        if (string.IsNullOrWhiteSpace(format)) format = "mp3";

        if (!Enum.TryParse<AudioFormat>(format, true, out var audioFormat))
            audioFormat = AudioFormat.Mp3;

        var progress = new Progress<DownloadProgress>(p =>
        {
            System.Console.Write($"\rProgress: {p.Percentage:F1}% ({FormatBytes(p.DownloadedBytes)}/{FormatBytes(p.TotalBytes)})");
        });

        System.Console.WriteLine("Starting audio download...");
        await downloadService.DownloadAudioAsync(url, outputPath, audioFormat, progress);
        System.Console.WriteLine("\nAudio download completed!");

        System.Console.WriteLine("Press any key to continue...");
        System.Console.ReadKey();
    }

    static async Task ShowVideoInfo(IDownloadService downloadService)
    {
        System.Console.WriteLine("\n=== VIDEO INFO ===");
        System.Console.Write("Enter YouTube URL: ");
        var url = System.Console.ReadLine();

        if (string.IsNullOrWhiteSpace(url)) return;

        System.Console.WriteLine("Fetching video information...");
        var videoInfo = await downloadService.GetVideoInfoAsync(url);

        System.Console.WriteLine($"\nTitle: {videoInfo.Title}");
        System.Console.WriteLine($"Author: {videoInfo.Author}");
        System.Console.WriteLine($"Duration: {videoInfo.Duration}");
        System.Console.WriteLine($"Views: {videoInfo.ViewCount:N0}");
        System.Console.WriteLine($"Upload Date: {videoInfo.UploadDate:yyyy-MM-dd}");

        System.Console.WriteLine($"\nAvailable Video Qualities:");
        foreach (var stream in videoInfo.VideoStreams.Take(5))
        {
            System.Console.WriteLine($"  {stream.Quality} ({stream.Format}) - {FormatBytes(stream.Size)}");
        }

        System.Console.WriteLine("Press any key to continue...");
        System.Console.ReadKey();
    }

    static string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int counter = 0;
        decimal number = bytes;

        while (Math.Round(number / 1024) >= 1)
        {
            number /= 1024;
            counter++;
        }

        return $"{number:n1} {suffixes[counter]}";
    }
}