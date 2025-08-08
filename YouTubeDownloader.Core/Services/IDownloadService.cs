using YouTubeDownloader.Core.Models;

namespace YouTubeDownloader.Core.Services;

public interface IDownloadService
{
    Task<VideoInfo> GetVideoInfoAsync(string url);
    Task DownloadVideoAsync(string url, string outputPath, DownloadOptions options, IProgress<DownloadProgress> progress);
    Task DownloadAudioAsync(string url, string outputPath, AudioFormat format, IProgress<DownloadProgress> progress);
}