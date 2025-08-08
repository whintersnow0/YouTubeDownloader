namespace YouTubeDownloader.Core.Models;

public class AudioStream
{
    public string Url { get; set; } = string.Empty;
    public string Quality { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public long Size { get; set; }
    public long Bitrate { get; set; }
}