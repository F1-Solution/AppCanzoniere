using AppCanzoniere.Application.DTOs;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Services;
using AppCanzoniere.Infrastructure.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppCanzoniere.MAUI.ViewModels;

public partial class CloudSyncViewModel : ObservableObject
{
    private readonly ICloudSyncService _cloudSyncService;
    private readonly LocalStorageProvider _localStorageProvider;

    [ObservableProperty]
    private string _selectedProvider = "Local";

    [ObservableProperty]
    private bool _isAuthenticated;

    [ObservableProperty]
    private bool _isSyncing;

    [ObservableProperty]
    private string? _lastSyncTime;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private bool _hasConflicts;

    [ObservableProperty]
    private ObservableCollection<SyncConflict> _conflicts = new();

    public List<string> AvailableProviders { get; } = new()
    {
        "Local",
        "OneDrive (Coming Soon)",
        "Google Drive (Coming Soon)",
        "Dropbox (Coming Soon)"
    };

    public CloudSyncViewModel(
        ICloudSyncService cloudSyncService,
        LocalStorageProvider localStorageProvider)
    {
        _cloudSyncService = cloudSyncService;
        _localStorageProvider = localStorageProvider;

        // Set default provider
        _cloudSyncService.SetProvider(_localStorageProvider);
        IsAuthenticated = true;
    }

    public async Task InitializeAsync()
    {
        await UpdateLastSyncTimeAsync();
    }

    [RelayCommand]
    private async Task ChangeProviderAsync()
    {
        if (SelectedProvider == "Local")
        {
            _cloudSyncService.SetProvider(_localStorageProvider);
            IsAuthenticated = true;
            StatusMessage = "Local storage provider selected";
        }
        else
        {
            await Shell.Current.DisplayAlert(
                "Coming Soon",
                $"{SelectedProvider} integration is not yet implemented. Please use Local storage for now.",
                "OK"
            );
            SelectedProvider = "Local";
        }
    }

    [RelayCommand]
    private async Task AuthenticateAsync()
    {
        if (_cloudSyncService.CurrentProvider == null) return;

        try
        {
            var result = await _cloudSyncService.CurrentProvider.AuthenticateAsync();

            if (result.Success)
            {
                IsAuthenticated = true;
                StatusMessage = $"Authenticated as {result.UserEmail}";
                
                await Shell.Current.DisplayAlert(
                    "Authentication Success",
                    $"Successfully authenticated with {_cloudSyncService.CurrentProvider.ProviderName}",
                    "OK"
                );
            }
            else
            {
                IsAuthenticated = false;
                StatusMessage = $"Authentication failed: {result.ErrorMessage}";
                
                await Shell.Current.DisplayAlert(
                    "Authentication Failed",
                    result.ErrorMessage ?? "Unknown error",
                    "OK"
                );
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Authentication error: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task SyncNowAsync()
    {
        if (_cloudSyncService.CurrentProvider == null)
        {
            await Shell.Current.DisplayAlert("Error", "No cloud provider selected", "OK");
            return;
        }

        if (!IsAuthenticated)
        {
            await Shell.Current.DisplayAlert("Error", "Please authenticate first", "OK");
            return;
        }

        IsSyncing = true;
        StatusMessage = "Syncing...";

        try
        {
            var result = await _cloudSyncService.SyncDatabaseAsync();

            if (result.Success)
            {
                StatusMessage = $"Sync completed: {result.FilesUploaded} uploaded, {result.FilesDownloaded} downloaded";
                
                await UpdateLastSyncTimeAsync();

                await Shell.Current.DisplayAlert(
                    "Sync Complete",
                    $"Successfully synced database.\nUploaded: {result.FilesUploaded}\nDownloaded: {result.FilesDownloaded}",
                    "OK"
                );
            }
            else if (result.Conflicts > 0)
            {
                HasConflicts = true;
                var conflicts = await _cloudSyncService.DetectConflictsAsync();
                Conflicts = new ObservableCollection<SyncConflict>(conflicts);
                
                StatusMessage = $"Sync conflicts detected: {result.Conflicts}";
                
                await Shell.Current.DisplayAlert(
                    "Conflicts Detected",
                    $"Found {result.Conflicts} conflicts. Please resolve them manually.",
                    "OK"
                );
            }
            else
            {
                StatusMessage = $"Sync failed: {result.ErrorMessage}";
                
                await Shell.Current.DisplayAlert(
                    "Sync Failed",
                    result.ErrorMessage ?? "Unknown error",
                    "OK"
                );
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Sync error: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", $"Sync error: {ex.Message}", "OK");
        }
        finally
        {
            IsSyncing = false;
        }
    }

    [RelayCommand]
    private async Task ResolveConflictsAsync()
    {
        if (Conflicts.Count == 0) return;

        try
        {
            var result = await _cloudSyncService.ResolveConflictsAsync(Conflicts.ToList());

            if (result.Success)
            {
                HasConflicts = false;
                Conflicts.Clear();
                StatusMessage = "Conflicts resolved successfully";

                await Shell.Current.DisplayAlert(
                    "Success",
                    "Conflicts resolved successfully",
                    "OK"
                );
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    result.ErrorMessage ?? "Failed to resolve conflicts",
                    "OK"
                );
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Conflict resolution error: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ExportDatabaseAsync()
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"appcanzoniere_backup_{timestamp}.db";
            var exportPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                fileName
            );

            var result = await _cloudSyncService.ExportDatabaseAsync(exportPath);

            if (result != null)
            {
                await Shell.Current.DisplayAlert(
                    "Export Complete",
                    $"Database exported to:\n{result}",
                    "OK"
                );
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to export database", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Export error: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ImportDatabaseAsync()
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select database backup to import",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/x-sqlite3", "application/octet-stream" } },
                    { DevicePlatform.iOS, new[] { "public.database" } },
                    { DevicePlatform.WinUI, new[] { ".db", ".sqlite", ".sqlite3" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.database" } }
                })
            });

            if (result != null)
            {
                var confirm = await Shell.Current.DisplayAlert(
                    "Confirm Import",
                    "This will replace your current database. Continue?",
                    "Yes",
                    "No"
                );

                if (confirm)
                {
                    var success = await _cloudSyncService.ImportDatabaseAsync(result.FullPath);

                    if (success)
                    {
                        await Shell.Current.DisplayAlert(
                            "Import Complete",
                            "Database imported successfully. Please restart the app.",
                            "OK"
                        );
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Failed to import database", "OK");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Import error: {ex.Message}", "OK");
        }
    }

    private async Task UpdateLastSyncTimeAsync()
    {
        var lastSync = await _cloudSyncService.GetLastSyncTimeAsync();
        
        if (lastSync.HasValue)
        {
            var timeAgo = DateTime.UtcNow - lastSync.Value;
            if (timeAgo.TotalMinutes < 1)
            {
                LastSyncTime = "Just now";
            }
            else if (timeAgo.TotalHours < 1)
            {
                LastSyncTime = $"{(int)timeAgo.TotalMinutes} minutes ago";
            }
            else if (timeAgo.TotalDays < 1)
            {
                LastSyncTime = $"{(int)timeAgo.TotalHours} hours ago";
            }
            else
            {
                LastSyncTime = lastSync.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            }
        }
        else
        {
            LastSyncTime = "Never";
        }
    }
}
