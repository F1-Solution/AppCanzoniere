using AppCanzoniere.Infrastructure.PdfImport;

// Reads one PDF (file or stdin) and writes the result as one JSON document to stdout.
// Exit codes: 0 answered, 1 any failure, 2 bad arguments, 3 deadline backstop reached.
const string Usage = "Usage: AppCanzoniere.PdfWorker <pdf path | -> [--deadline-seconds N] [--limits JSON]";
if (!TryParse(args, out var seconds, out var limits))
{
    Console.Error.WriteLine(Usage);
    return 2;
}

PdfPigWordExtractor extractor;
try
{
    extractor = new PdfPigWordExtractor(limits);
}
catch (ArgumentOutOfRangeException)
{
    Console.Error.WriteLine(Usage);
    return 2;
}

// A backstop in case the parent vanishes while a file keeps the reader busy.
using var watchdog = seconds is { } deadline
    ? new Timer(_ => Environment.Exit(3), null, TimeSpan.FromSeconds(deadline), Timeout.InfiniteTimeSpan)
    : null;

try
{
    await using var input = args[0] == "-" ? Console.OpenStandardInput() : File.OpenRead(args[0]);
    var result = await extractor.ExtractAsync(input);
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

static bool TryParse(string[] args, out int? seconds, out PdfExtractionLimits? limits)
{
    seconds = null;
    limits = null;
    if (args.Length == 0)
        return false;

    for (var i = 1; i < args.Length; i += 2)
    {
        if (i + 1 >= args.Length)
            return false;

        if (args[i] == "--deadline-seconds" && seconds is null && int.TryParse(args[i + 1], out var value) && value > 0)
            seconds = value;
        else if (args[i] == PdfWorkerLimits.Switch && limits is null && PdfWorkerLimits.TryParse(args[i + 1]) is { } parsed)
            limits = parsed;
        else
            return false;
    }

    return true;
}
