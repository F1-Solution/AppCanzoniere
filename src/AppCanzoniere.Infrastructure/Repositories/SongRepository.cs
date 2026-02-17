using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AppCanzoniere.Infrastructure.Repositories;

public class SongRepository : ISongRepository
{
    private readonly AppDbContext _context;

    public SongRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Song?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Songs.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Song>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Songs
            .OrderBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Song>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Songs
            .Where(s => s.Title.Contains(searchTerm) || s.Artist.Contains(searchTerm))
            .OrderBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Song>> GetSongsByTagsAsync(List<Guid> tagIds, CancellationToken cancellationToken = default)
    {
        return await _context.Songs
            .Where(s => s.SongTags.Any(st => tagIds.Contains(st.TagId)))
            .Include(s => s.SongTags)
                .ThenInclude(st => st.Tag)
            .OrderBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<Song> AddAsync(Song song, CancellationToken cancellationToken = default)
    {
        _context.Songs.Add(song);
        await _context.SaveChangesAsync(cancellationToken);
        return song;
    }

    public async Task UpdateAsync(Song song, CancellationToken cancellationToken = default)
    {
        song.UpdatedAt = DateTime.UtcNow;
        song.Version++;
        _context.Songs.Update(song);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var song = await GetByIdAsync(id, cancellationToken);
        if (song != null)
        {
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<AdvancedSearchResult<Song>> AdvancedSearchAsync(AdvancedSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        
        // Start with base query
        IQueryable<Song> query = _context.Songs.AsQueryable();
        int filtersApplied = 0;

        // Full-text search
        if (!string.IsNullOrWhiteSpace(criteria.SearchQuery))
        {
            var searchLower = criteria.SearchQuery.ToLower();
            
            if (criteria.EnableFuzzySearch)
            {
                // Fuzzy search: use LIKE with wildcards for simplicity
                query = query.Where(s =>
                    s.Title.ToLower().Contains(searchLower) ||
                    s.Artist.ToLower().Contains(searchLower) ||
                    (s.ChordProContent != null && s.ChordProContent.ToLower().Contains(searchLower)));
            }
            else
            {
                // Exact search
                query = query.Where(s =>
                    s.Title.Contains(criteria.SearchQuery) ||
                    s.Artist.Contains(criteria.SearchQuery) ||
                    (s.ChordProContent != null && s.ChordProContent.Contains(criteria.SearchQuery)));
            }
            filtersApplied++;
        }

        // Filter by language
        if (!string.IsNullOrWhiteSpace(criteria.Language))
        {
            query = query.Where(s => s.Language == criteria.Language);
            filtersApplied++;
        }

        // Filter by original key
        if (!string.IsNullOrWhiteSpace(criteria.OriginalKey))
        {
            query = query.Where(s => s.OriginalKey == criteria.OriginalKey);
            filtersApplied++;
        }

        // Filter by tags (Note: SongTags use int TagId, but criteria uses int - needs adjustment for Guid)
        // Assuming Song entity has navigation property SongTags
        if (criteria.TagIds != null && criteria.TagIds.Any())
        {
            // Convert int to Guid if needed
            var tagGuids = criteria.TagIds.Select(id => new Guid(id.ToString("X32"))).ToList();
            query = query.Where(s => s.SongTags.Any(st => tagGuids.Contains(st.TagId)));
            filtersApplied++;
        }

        // Filter by liturgical moment - skip if not applicable
        // (Song entity doesn't have LiturgicalMoments field currently)
        
        // Filter by verses count - skip if not applicable  
        // (Song entity doesn't have Verses count field currently)

        // Filter by created date range
        if (criteria.CreatedFrom.HasValue || criteria.CreatedTo.HasValue)
        {
            if (criteria.CreatedFrom.HasValue)
            {
                query = query.Where(s => s.CreatedAt >= criteria.CreatedFrom.Value);
                filtersApplied++;
            }
            if (criteria.CreatedTo.HasValue)
            {
                query = query.Where(s => s.CreatedAt <= criteria.CreatedTo.Value);
                filtersApplied++;
            }
        }

        // Filter by modified date range
        if (criteria.ModifiedFrom.HasValue || criteria.ModifiedTo.HasValue)
        {
            if (criteria.ModifiedFrom.HasValue)
            {
                query = query.Where(s => s.UpdatedAt >= criteria.ModifiedFrom.Value);
                filtersApplied++;
            }
            if (criteria.ModifiedTo.HasValue)
            {
                query = query.Where(s => s.UpdatedAt <= criteria.ModifiedTo.Value);
                filtersApplied++;
            }
        }

        // Filter by media attachments - skip if not applicable
        // (Song entity doesn't have PdfPath, Mp3Path, YouTubeUrl fields currently)

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = criteria.SortBy switch
        {
            SongSortOrder.Title => criteria.SortAscending
                ? query.OrderBy(s => s.Title)
                : query.OrderByDescending(s => s.Title),
            SongSortOrder.Artist => criteria.SortAscending
                ? query.OrderBy(s => s.Artist)
                : query.OrderByDescending(s => s.Artist),
            SongSortOrder.LastModified => criteria.SortAscending
                ? query.OrderBy(s => s.UpdatedAt)
                : query.OrderByDescending(s => s.UpdatedAt),
            SongSortOrder.CreatedAt => criteria.SortAscending
                ? query.OrderBy(s => s.CreatedAt)
                : query.OrderByDescending(s => s.CreatedAt),
            _ => query.OrderByDescending(s => s.CreatedAt)
        };

        // Apply pagination
        query = query.Skip(criteria.Skip).Take(Math.Min(criteria.MaxResults, 1000));

        // Execute query
        var items = await query.ToListAsync(cancellationToken);

        sw.Stop();

        return new AdvancedSearchResult<Song>
        {
            Items = items,
            TotalCount = totalCount,
            ExecutionTimeMs = sw.ElapsedMilliseconds,
            Query = criteria.SearchQuery,
            FiltersApplied = filtersApplied,
            FuzzySearchUsed = criteria.EnableFuzzySearch
        };
    }
}
