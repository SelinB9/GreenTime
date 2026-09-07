using System;

namespace GreenTime.Models;

public class CuratedVideo
{
    public Guid Id { get; set; }
    public Guid PlaylistId { get; set; } // Foreign Key
    public string YoutubeVideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;//kapak resmi
    public int DurationSeconds { get; set; }//video toplam uzunluk saniye cinsi

    // İlişkili playlist'e referans (Navigation Property)
    public CuratedPlaylist Playlist { get; set; } = null!;
}