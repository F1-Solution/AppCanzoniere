using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;

namespace AppCanzoniere.Backend.DTOs;

public record SongResponseDto(
    Guid Id,
    string Title,
    string Artist,
    string Language,
    SongType Type,
    string? ChordProContent,
    string? MusicXmlContent,
    string? JsonContent,
    ContentFormat PrimaryFormat,
    string OriginalKey,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsPublic,
    CopyrightStatus CopyrightStatus,
    List<string> Tags,
    List<string> LiturgicalMoments,
    List<BibleReferenceDto> BibleReferences,
    List<string> YouTubeLinks
)
{
    public static SongResponseDto FromEntity(Song song) => new(
        song.Id,
        song.Title,
        song.Artist,
        song.Language,
        song.Type,
        song.ChordProContent,
        song.MusicXmlContent,
        song.JsonContent,
        song.PrimaryFormat,
        song.OriginalKey,
        song.CreatedAt,
        song.UpdatedAt,
        song.IsPublic,
        song.CopyrightStatus,
        song.SongTags?.Select(st => st.Tag?.Name ?? "").Where(n => n != "").ToList() ?? [],
        song.SongLiturgicalMoments?.Select(slm => slm.LiturgicalMoment?.Name ?? "").Where(n => n != "").ToList() ?? [],
        song.BibleReferences?.Select(br => new BibleReferenceDto(br.Book, br.Chapter, br.VerseStart, br.VerseEnd, br.DisplayText)).ToList() ?? [],
        song.YouTubeLinks?.Select(yl => $"https://www.youtube.com/watch?v={yl.VideoId}").ToList() ?? []
    );
}

public record BibleReferenceDto(string Book, int? Chapter, int? VerseStart, int? VerseEnd, string DisplayText);

public record CreateSongDto(
    string Title,
    string Artist,
    string? Language,
    SongType Type,
    string? ChordProContent,
    string? MusicXmlContent,
    string? JsonContent,
    ContentFormat PrimaryFormat,
    string? OriginalKey,
    bool IsPublic,
    CopyrightStatus CopyrightStatus
);

public record UpdateSongDto(
    string Title,
    string Artist,
    string? Language,
    SongType Type,
    string? ChordProContent,
    string? MusicXmlContent,
    string? JsonContent,
    ContentFormat PrimaryFormat,
    string? OriginalKey,
    bool IsPublic,
    CopyrightStatus CopyrightStatus
);
