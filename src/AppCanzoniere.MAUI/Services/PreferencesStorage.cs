using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.MAUI.Services;

/// <summary>Device-local string preferences backed by MAUI Preferences.</summary>
public sealed class PreferencesStorage : IPreferenceStorage
{
    public string? Get(string key) => Preferences.Default.ContainsKey(key) ? Preferences.Default.Get(key, string.Empty) : null;

    public void Set(string key, string value) => Preferences.Default.Set(key, value);
}
