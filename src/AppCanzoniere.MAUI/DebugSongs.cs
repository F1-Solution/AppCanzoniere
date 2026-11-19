#if DEBUG
using AppCanzoniere.Application.Diagnostics;
using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.MAUI;

/// <summary>Debug builds only: lists one long invented song so a tester can open it with a tap.</summary>
internal static class DebugSongs
{
    public const string Title = "Canto di prova";

    public static async Task EnsureAsync(AppDbContext context)
    {
        if (await context.Songs.AnyAsync(s => s.Title == Title))
            return;

        context.Songs.Add(SyntheticSong.Create(Title));
        await context.SaveChangesAsync();
    }
}
#endif
