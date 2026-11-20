using AppCanzoniere.Infrastructure.PdfImport;

namespace AppCanzoniere.Infrastructure.Tests.PdfImport;

public class PdfWorkerLaunchTests : IDisposable
{
    private readonly string _app = Path.Combine(Path.GetTempPath(), "workerlaunch-" + Guid.NewGuid().ToString("N"));

    public PdfWorkerLaunchTests() => Directory.CreateDirectory(_app);

    public void Dispose() => Directory.Delete(_app, recursive: true);

    private static string Executable => OperatingSystem.IsWindows() ? "AppCanzoniere.PdfWorker.exe" : "AppCanzoniere.PdfWorker";

    private static void Touch(string folder, string name)
    {
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, name), string.Empty);
    }

    [Fact]
    public void LocateForApp_FindsTheWorkerInItsSubfolder()
    {
        var folder = Path.Combine(_app, PdfWorkerLaunch.AppFolder);
        Touch(folder, Executable);

        var launch = PdfWorkerLaunch.LocateForApp(_app);

        Assert.NotNull(launch);
        Assert.Equal(Path.Combine(folder, Executable), launch!.FileName);
    }

    [Fact]
    public void LocateForApp_PrefersTheSubfolderOverTheAppFolder()
    {
        Touch(_app, Executable);
        var folder = Path.Combine(_app, PdfWorkerLaunch.AppFolder);
        Touch(folder, Executable);

        Assert.Equal(Path.Combine(folder, Executable), PdfWorkerLaunch.LocateForApp(_app)!.FileName);
    }

    [Fact]
    public void LocateForApp_FallsBackToTheAppFolder()
    {
        Touch(_app, Executable);

        Assert.Equal(Path.Combine(_app, Executable), PdfWorkerLaunch.LocateForApp(_app)!.FileName);
    }

    [Fact]
    public void LocateForApp_ReturnsNullWhenTheWorkerIsMissing()
    {
        Assert.Null(PdfWorkerLaunch.LocateForApp(_app));
    }
}
