namespace YouTubeDownloader.Core.Models;

public class DownloadOptions
{
    public string OutputPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    public string Quality { get; set; } = "720p";
    public string Format { get; set; } = "mp4";
    public bool AudioOnly { get; set; } = false;
    public string AudioFormat { get; set; } = "mp3";
}