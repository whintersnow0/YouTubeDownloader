using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;
using Microsoft.Extensions.Logging;
using YouTubeDownloader.Core.Models;
using System.Text.RegularExpressions;

namespace YouTubeDownloader.Core.Services;

public class DownloadService : IDownloadService
{
    private readonly YoutubeClient _youtube;
    private readonly ILogger<DownloadService>? _logger;

    public DownloadService(ILogger<DownloadService>? logger = null)
    {
        _youtube = new YoutubeClient();
        _logger = logger;
    }

    public async Task<VideoInfo> GetVideoInfoAsync(string url)
    {
        var video = await _youtube.Videos.GetAsync(url);
        var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(url);

        var videoStreams = streamManifest.GetVideoOnlyStreams()
     .Select(s => new VideoStream
     {
         Url = s.Url,
         Quality = s.VideoQuality.Label,
         Format = s.Container.Name,
         Size = s.Size.Bytes,
         Bitrate = s.Bitrate.BitsPerSecond
     }).ToList();

        var audioStreams = streamManifest.GetAudioOnlyStreams()
            .Select(s => new AudioStream
            {
                Url = s.Url,
                Format = s.Container.Name,
                Quality = s.Bitrate.KiloBitsPerSecond + " kbps",
                Size = s.Size.Bytes,
                Bitrate = s.Bitrate.BitsPerSecond
            }).ToList();


        return new VideoInfo
        {
            Id = video.Id,
            Title = video.Title,
            Author = video.Author.ChannelTitle,
            Duration = video.Duration ?? TimeSpan.Zero,
            Description = video.Description,
            ThumbnailUrl = video.Thumbnails.GetWithHighestResolution()?.Url ?? "",
            UploadDate = video.UploadDate.DateTime,
            ViewCount = video.Engagement.ViewCount,
            VideoStreams = videoStreams,
            AudioStreams = audioStreams
        };
    }

    public async Task DownloadVideoAsync(string url, string outputPath, DownloadOptions options, IProgress<DownloadProgress> progress)
    {
        var video = await _youtube.Videos.GetAsync(url);
        var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(url);

        IStreamInfo streamInfo;
        if (options.AudioOnly)
        {
            streamInfo = streamManifest.GetAudioOnlyStreams()
                .Where(s => s.Container.Name.Equals(options.AudioFormat, StringComparison.OrdinalIgnoreCase))
                .GetWithHighestBitrate();
        }
        else
        {
            streamInfo = streamManifest.GetMuxedStreams()
                .Where(s => s.VideoQuality.Label == options.Quality)
                .FirstOrDefault() ?? streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
        }

        if (streamInfo == null)
            throw new InvalidOperationException("No suitable stream found.");

        var fileName = SanitizeFileName($"{video.Title}.{streamInfo.Container.Name}");
        var filePath = Path.Combine(outputPath, fileName);

        Directory.CreateDirectory(outputPath);
        await DownloadStreamAsync(streamInfo, filePath, progress);
    }

    public async Task DownloadAudioAsync(string url, string outputPath, AudioFormat format, IProgress<DownloadProgress> progress)
    {
        var video = await _youtube.Videos.GetAsync(url);
        var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(url);

        var audioStreamInfo = streamManifest.GetAudioOnlyStreams()
            .GetWithHighestBitrate();

        var fileName = SanitizeFileName($"{video.Title}.{format.ToString().ToLower()}");
        var filePath = Path.Combine(outputPath, fileName);

        Directory.CreateDirectory(outputPath);
        await DownloadStreamAsync(audioStreamInfo, filePath, progress);
    }

    private async Task DownloadStreamAsync(IStreamInfo streamInfo, string filePath, IProgress<DownloadProgress> progress)
    {
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(streamInfo.Url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? 0;
        var downloadedBytes = 0L;

        using var contentStream = await response.Content.ReadAsStreamAsync();
        using var fileStream = new FileStream(filePath, FileMode.Create);

        var buffer = new byte[8192];

        while (true)
        {
            var bytesRead = await contentStream.ReadAsync(buffer);
            if (bytesRead == 0) break;

            await fileStream.WriteAsync(buffer, 0, bytesRead);
            downloadedBytes += bytesRead;

            progress?.Report(new DownloadProgress
            {
                TotalBytes = totalBytes,
                DownloadedBytes = downloadedBytes,
                Status = "Downloading...",
                CurrentFile = Path.GetFileName(filePath)
            });
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        return Regex.Replace(sanitized, @"\s+", " ").Trim();
    }
}