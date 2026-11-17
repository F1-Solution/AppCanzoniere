namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>How to start a PDF reader process: the program and the arguments that come before the ones the host adds.</summary>
public sealed record PdfWorkerLaunch(string FileName, IReadOnlyList<string> LeadingArguments)
{
    private const string WorkerName = "AppCanzoniere.PdfWorker";

    /// <summary>Finds the worker next to the app: its own executable first, then its library run by the dotnet host.</summary>
    public static PdfWorkerLaunch? Locate(string directory)
    {
        var executable = Path.Combine(directory, OperatingSystem.IsWindows() ? WorkerName + ".exe" : WorkerName);
        if (File.Exists(executable))
            return new PdfWorkerLaunch(executable, Array.Empty<string>());

        var library = Path.Combine(directory, WorkerName + ".dll");
        return File.Exists(library) ? new PdfWorkerLaunch(DotnetHost(), new[] { library }) : null;
    }

    /// <summary>The dotnet host that runs the current process, or the one on the path.</summary>
    public static string DotnetHost() =>
        Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") is { Length: > 0 } path ? path : "dotnet";
}
