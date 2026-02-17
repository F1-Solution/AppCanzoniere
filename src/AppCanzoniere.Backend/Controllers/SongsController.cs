using AppCanzoniere.Backend.DTOs;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppCanzoniere.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongsController : ControllerBase
{
    private readonly ISongRepository _songRepository;
    private readonly ILogger<SongsController> _logger;

    public SongsController(ISongRepository songRepository, ILogger<SongsController> logger)
    {
        _songRepository = songRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SongResponseDto>>> GetPublicSongs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var songs = await _songRepository.GetAllAsync();
        var publicSongs = songs
            .Where(s => s.IsPublic)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(SongResponseDto.FromEntity);
        return Ok(publicSongs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SongResponseDto>> GetSong(Guid id)
    {
        var song = await _songRepository.GetByIdAsync(id);

        if (song == null)
        {
            return NotFound();
        }

        if (!song.IsPublic && !User.Identity!.IsAuthenticated)
        {
            return Unauthorized();
        }

        if (!song.IsPublic)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }
        }

        return Ok(SongResponseDto.FromEntity(song));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<SongResponseDto>>> SearchSongs([FromQuery] string query)
    {
        var songs = await _songRepository.GetAllAsync();
        var publicSongs = songs
            .Where(s => s.IsPublic &&
                   (s.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    s.Artist.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .Take(50)
            .Select(SongResponseDto.FromEntity);

        return Ok(publicSongs);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SongResponseDto>> CreateSong([FromBody] CreateSongDto dto)
    {
        var song = new Song
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Artist = dto.Artist,
            Language = dto.Language ?? "en",
            Type = dto.Type,
            ChordProContent = dto.ChordProContent,
            MusicXmlContent = dto.MusicXmlContent,
            JsonContent = dto.JsonContent,
            PrimaryFormat = dto.PrimaryFormat,
            OriginalKey = dto.OriginalKey ?? "C",
            IsPublic = dto.IsPublic,
            CopyrightStatus = dto.CopyrightStatus,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _songRepository.AddAsync(song);
        return CreatedAtAction(nameof(GetSong), new { id = song.Id }, SongResponseDto.FromEntity(song));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSong(Guid id, [FromBody] UpdateSongDto dto)
    {
        var existingSong = await _songRepository.GetByIdAsync(id);

        if (existingSong == null)
        {
            return NotFound();
        }

        existingSong.Title = dto.Title;
        existingSong.Artist = dto.Artist;
        existingSong.Language = dto.Language ?? existingSong.Language;
        existingSong.Type = dto.Type;
        existingSong.ChordProContent = dto.ChordProContent;
        existingSong.MusicXmlContent = dto.MusicXmlContent;
        existingSong.JsonContent = dto.JsonContent;
        existingSong.PrimaryFormat = dto.PrimaryFormat;
        existingSong.OriginalKey = dto.OriginalKey ?? existingSong.OriginalKey;
        existingSong.IsPublic = dto.IsPublic;
        existingSong.CopyrightStatus = dto.CopyrightStatus;
        existingSong.UpdatedAt = DateTime.UtcNow;

        await _songRepository.UpdateAsync(existingSong);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSong(Guid id)
    {
        var song = await _songRepository.GetByIdAsync(id);

        if (song == null)
        {
            return NotFound();
        }

        await _songRepository.DeleteAsync(id);
        return NoContent();
    }
}
