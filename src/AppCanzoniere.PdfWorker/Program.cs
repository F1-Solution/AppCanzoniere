using AppCanzoniere.Infrastructure.PdfImport;

// Usage: AppCanzoniere.PdfWorker <pdf path | -> [--deadline-seconds N]
// Reads one PDF (file or stdin), writes the result as one JSON document to stdout. Exit 0 when it answered, 1 on any failure, 2 on bad arguments.
if (args.Length is not (1 or 3) || (args.Length == 3 && (args[1] != "--deadline-seconds" || !int.TryParse(args[2], out var seconds) || seconds <= 0)))
{
    Console.Error.WriteLine("Usage: AppCanzoniere.PdfWorker <pdf path | -> [--deadline-seconds N]");
    return 2;
}

// A backstop in case the parent vanishes while a file keeps the reader busy.
using var watchdog = args.Length == 3
    ? new Timer(_ => Environment.Exit(3), null, TimeSpan.FromSeconds(int.Parse(args[2])), Timeout.InfiniteTimeSpan)
    : null;

try
{
    await using var input = args[0] == "-" ? Console.OpenStandardInput() : File.OpenRead(args[0]);
    var result = await new PdfPigWordExtractor().ExtractAsync(input);
    await using var output = Console.OpenStandardOutput();
    PdfWorkerProtocol.Write(output, result);
    await output.FlushAsync();
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception.ToString());
    return 1;
}
