namespace AppCanzoniere.Infrastructure.Tests;

/// <summary>A fact that is skipped, and shown as skipped, when the tests do not run on Windows.</summary>
public sealed class WindowsFactAttribute : FactAttribute
{
    public WindowsFactAttribute()
    {
        if (!OperatingSystem.IsWindows())
            Skip = "Runs on Windows only.";
    }
}
