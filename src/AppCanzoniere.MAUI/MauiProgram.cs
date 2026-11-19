using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.PageTurning;
using AppCanzoniere.Application.Services;
using AppCanzoniere.Domain.Interfaces;
using AppCanzoniere.Infrastructure.Data;
using AppCanzoniere.Infrastructure.Repositories;
using AppCanzoniere.MAUI.Pages;
using AppCanzoniere.MAUI.Services;
using AppCanzoniere.MAUI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace AppCanzoniere.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("JetBrainsMono-Regular.ttf", "JetBrainsMono");
            });

#if WINDOWS
        builder.ConfigureLifecycleEvents(events => events.AddWindows(windows => windows
            .OnActivated((window, _) => Platforms.Windows.WindowKeyHook.Attach(window))
            .OnWindowCreated(window => Platforms.Windows.MinimizeWatcher.Attach(window))));
#endif

#if WINDOWS
        // The reader is started on demand from the PdfWorker folder next to the app.
        builder.Services.AddSingleton<AppCanzoniere.Application.PdfImport.IPdfWordExtractor>(_ =>
            new AppCanzoniere.Infrastructure.PdfImport.IsolatedPdfWordExtractor(
                AppCanzoniere.Infrastructure.PdfImport.PdfWorkerLaunch.LocateForApp(AppContext.BaseDirectory)
                ?? throw new FileNotFoundException("The PDF reader is missing from the app folder.")));
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Database
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "appcanzoniere.db");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // Repositories
        builder.Services.AddScoped<ISongRepository, SongRepository>();
        builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
        builder.Services.AddScoped<ITagRepository, TagRepository>();
        builder.Services.AddScoped<ILiturgicalMomentRepository, LiturgicalMomentRepository>();
        builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();

        // Application Services
        builder.Services.AddSingleton<IChordProParser, ChordProParser>();
        builder.Services.AddSingleton<IChordTransposer, ChordTransposer>();
        builder.Services.AddSingleton<ISongDisplayService, SongDisplayService>();
        builder.Services.AddSingleton<SongScreenPresenter>();
        builder.Services.AddSingleton<IHardwareKeySource, HardwareKeySource>();
        builder.Services.AddSingleton(_ => new ScreenWakeLock(on => DeviceDisplay.Current.KeepScreenOn = on));
        builder.Services.AddSingleton<IPreferenceStorage, PreferencesStorage>();
        builder.Services.AddSingleton<IChordDisplaySettingsStore, ChordDisplaySettingsStore>();

        // Cloud Storage Providers
        builder.Services.AddSingleton<LocalStorageProvider>();
        // TODO: Add other providers when implemented
        // builder.Services.AddSingleton<OneDriveStorageProvider>();
        // builder.Services.AddSingleton<GoogleDriveStorageProvider>();
        // builder.Services.AddSingleton<DropboxStorageProvider>();

        // Cloud Sync Service
        builder.Services.AddSingleton<ICloudSyncService, CloudSyncService>();

        // AI Services - Use Mock by default, can be replaced with real service
        builder.Services.AddSingleton<IChordSuggestionService, MockChordSuggestionService>();
        // Uncomment to use real Perplexity service (requires API key):
        // builder.Services.AddSingleton<IChordSuggestionService, PerplexityChordSuggestionService>();

        // OCR Services - Use Mock by default
        builder.Services.AddSingleton<IOcrService, MockOcrService>();
        // Uncomment to use Azure Computer Vision (requires API key):
        // builder.Services.AddHttpClient<IOcrService, AzureComputerVisionService>();

        // Audio Recognition Services - Use Mock by default
        builder.Services.AddSingleton<IAudioRecognitionService, MockAudioRecognitionService>();
        // Uncomment to use ACRCloud (requires API key):
        // builder.Services.AddSingleton<IAudioRecognitionService, ACRCloudAudioRecognitionService>();

        // Token Storage (SecureStorage)
        builder.Services.AddSingleton<ITokenStorage, SecureTokenStorage>();

        // Backend API Services
        builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:5001/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddHttpClient<IBackendSongService, BackendSongService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:5001/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // ViewModels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<SongDetailViewModel>();
        builder.Services.AddTransient<SongMetadataViewModel>();
        builder.Services.AddTransient<AiChordSuggestionViewModel>();
        builder.Services.AddTransient<OcrImportViewModel>();
        builder.Services.AddTransient<AudioRecognitionViewModel>();
        builder.Services.AddTransient<CloudSyncViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<PublicSongsViewModel>();
        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<AlbumListViewModel>();
        builder.Services.AddTransient<AlbumDetailViewModel>();

        // Pages
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<SongDetailPage>();
        builder.Services.AddTransient<SongMetadataPage>();
        builder.Services.AddTransient<AiChordSuggestionPage>();
        builder.Services.AddTransient<OcrImportPage>();
        builder.Services.AddTransient<AudioRecognitionPage>();
        builder.Services.AddTransient<CloudSyncPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<PublicSongsPage>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<AlbumListPage>();
        builder.Services.AddTransient<AlbumDetailPage>();

        var app = builder.Build();

        // Initialize database - create tables if they don't exist
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        return app;
    }
}
