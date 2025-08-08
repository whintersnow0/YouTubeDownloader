namespace YouTubeDownloader.Core.Models;

public class VideoInfo
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public string Description { get; set; } = string.Empty;
    public long ViewCount { get; set; }
    public DateTime UploadDate { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public List<VideoStream> VideoStreams { get; set; } = new();
    public List<AudioStream> AudioStreams { get; set; } = new();
}