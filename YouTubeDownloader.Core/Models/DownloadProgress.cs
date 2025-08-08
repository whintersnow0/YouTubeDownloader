namespace YouTubeDownloader.Core.Models;

public class DownloadProgress
{
    public long TotalBytes { get; set; }
    public long DownloadedBytes { get; set; }
    public double Percentage => TotalBytes > 0 ? (double)DownloadedBytes / TotalBytes * 100 : 0;
    public string Status { get; set; } = string.Empty;
    public string CurrentFile { get; set; } = string.Empty;
}