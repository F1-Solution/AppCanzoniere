using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppCanzoniere.MAUI;

public partial class App : global::Microsoft.Maui.Controls.Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        // Initialize database
        InitializeDatabaseAsync(serviceProvider).ConfigureAwait(false);
    }

    private async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Apply migrations
        await dbContext.Database.MigrateAsync();

        // Seed initial data
        var seeder = new DatabaseSeeder(dbContext);
        await seeder.SeedAsync();
#if DEBUG
        await DebugSongs.EnsureAsync(dbContext);
#endif
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        TokenPalette.Apply(RequestedTheme);
        RequestedThemeChanged += (_, e) => TokenPalette.Apply(e.RequestedTheme);
        var shell = new AppShell();
#if DEBUG
        ImportTestHook.Log("create window");
        if (ImportTestHook.IsRequested)
        {
            EventHandler? openImport = null;
            openImport = async (_, _) =>
            {
                shell.Loaded -= openImport;
                ImportTestHook.Log("shell loaded");
                await ImportTestHook.RunAsync(shell);
            };
            shell.Loaded += openImport;
        }

        if (UiTestHook.IsRequested)
        {
            EventHandler? open = null;
            open = async (_, _) =>
            {
                shell.Loaded -= open;
                await UiTestHook.OpenSongAsync(shell);
            };
            shell.Loaded += open;
        }
#endif
        return new Window(shell);
    }
}