# YouTube Downloader

A robust .NET library and console application for downloading YouTube videos and audio streams with an interactive menu interface.

## Features

- Interactive console menu for easy navigation
- Download video-only streams in various qualities (360p, 720p, 1080p)
- Download audio-only streams in multiple formats (MP3, AAC, OGG)
- Download high-quality video with audio (separate streams merged)
- Extract comprehensive video metadata and stream information
- Real-time progress tracking with file size formatting
- Clean, modular architecture with dependency injection support
- Error handling and user-friendly interface

## Installation

### Clone Repository
```bash
git clone https://github.com/whintersnow0/YouTubeDownloader.git
cd YouTubeDownloader
```

### Build
```bash
dotnet build
```

### Run Console Application
```bash
dotnet run --project YouTubeDownloader.Console
```

## Console Application Usage

The application provides an interactive menu with the following options:

1. **Download Video** - Download video-only streams
2. **Download Audio Only** - Extract audio in various formats
3. **Download Video with Audio (High Quality)** - Best quality video with audio
4. **Get Video Info** - Display video metadata and available qualities
5. **Exit** - Close the application

### Example Workflow
1. Run the application
2. Select option from the menu
3. Enter YouTube URL when prompted
4. Choose output directory (defaults to Desktop)
5. Select quality/format options
6. Monitor real-time download progress

## Library Usage

### Basic Video Download
```csharp
using YouTubeDownloader.Core.Services;
using YouTubeDownloader.Core.Models;

var downloadService = new DownloadService();

var options = new DownloadOptions 
{ 
    Quality = "720p", 
    AudioOnly = false 
};

var progress = new Progress<DownloadProgress>(p =>
{
    var percentage = p.TotalBytes > 0 ? (double)p.DownloadedBytes / p.TotalBytes * 100 : 0;
    Console.WriteLine($"{p.Status} - {percentage:F1}% - {p.CurrentFile}");
});

await downloadService.DownloadVideoAsync(url, outputPath, options, progress);
```

### Audio-Only Download
```csharp
await downloadService.DownloadAudioAsync(url, outputPath, AudioFormat.Mp3, progress);
```

### Get Video Information
```csharp
var videoInfo = await downloadService.GetVideoInfoAsync(url);
Console.WriteLine($"Title: {videoInfo.Title}");
Console.WriteLine($"Duration: {videoInfo.Duration}");
Console.WriteLine($"Views: {videoInfo.ViewCount:N0}");
```

## Project Structure

```
YouTubeDownloader/
├── YouTubeDownloader.Core/          # Core library
│   ├── Models/                      # Data models
│   ├── Services/                    # Download services
│   └── Enums/                       # Enumerations
├── YouTubeDownloader.Console/       # Console application
└── YouTubeDownloader.Tests/         # Unit tests
```

## Dependencies

- [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) - YouTube video extraction
- Microsoft.Extensions.Logging - Logging support

## Requirements

- .NET 8.0 or higher

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.
