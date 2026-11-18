using System.Diagnostics;
using System.Runtime.InteropServices;

// Test-only stand-in for the PDF worker. Arguments: <mode> <pid file> [arguments the host adds].
// It writes its process id to the pid file first, so tests can check that it is gone afterwards.
var mode = args.Length > 0 ? args[0] : "ok";
if (args.Length > 1)
    File.WriteAllText(args[1], Environment.ProcessId.ToString());

var stdout = Console.OpenStandardOutput();
switch (mode)
{
    case "ok":
        Write(stdout, """{"v":2,"s":"Success","r":"","p":[{"n":1,"w":595,"h":842,"i":false,"x":[{"t":"ciao","l":10,"b":20,"r":40,"u":30}]}]}""");
        return 0;
    case "crash":
        Console.Error.WriteLine("boom");
        return 134;
    case "oom":
        Console.Error.WriteLine("Unhandled exception. System.OutOfMemoryException: out of memory");
        return 134;
    case "hang":
        Thread.Sleep(Timeout.Infinite);
        return 0;
    case "hang-with-grandchild":
        // A child of the worker must die with it.
        Process.Start(new ProcessStartInfo(Environment.ProcessPath!, [typeof(Program).Assembly.Location, "hang", args[1] + ".child"]) { UseShellExecute = false });
        Thread.Sleep(Timeout.Infinite);
        return 0;
    case "orphan-then-hang":
        // The grandchild's parent exits first, so only the Job Object can still reach the grandchild.
        OrphanGrandchild(args[1]);
        Thread.Sleep(Timeout.Infinite);
        return 0;
    case "orphan-then-ok":
        OrphanGrandchild(args[1]);
        Write(stdout, WordAnswer("ciao"));
        return 0;
    case "spawn-hold-and-exit":
        // Second step of the orphan modes: args are <mode> <own pid file> <grandchild pid file>.
        var held = Process.Start(new ProcessStartInfo(Environment.ProcessPath!, [typeof(Program).Assembly.Location, "hold", args[2]]) { UseShellExecute = false })!;
        for (var i = 0; i < 100 && !File.Exists(args[2]); i++)
            Thread.Sleep(50);
        held.Dispose();
        return 0;
    case "env":
        // Reports whether the host's secret and the path reached this process.
        var secret = Environment.GetEnvironmentVariable("PERPLEXITY_API_KEY") is null ? "no" : "yes";
        var path = Environment.GetEnvironmentVariable("PATH") is null ? "no" : "yes";
        Write(stdout, WordAnswer($"secret={secret};path={path}"));
        return 0;
    case "args":
        // Echoes the arguments the host added, as the text of the only word.
        Write(stdout, WordAnswer(string.Join(' ', args.Skip(2))));
        return 0;
    case "exit-holding-stderr":
        // Exits normally while a grandchild keeps the inherited stderr pipe open.
        Process.Start(new ProcessStartInfo(Environment.ProcessPath!, [typeof(Program).Assembly.Location, "hold", args[1] + ".child"]) { UseShellExecute = false, RedirectStandardOutput = true });
        Write(stdout, WordAnswer("ciao"));
        return 0;
    case "hold":
        Thread.Sleep(TimeSpan.FromSeconds(20));
        return 0;
    case "object-flood":
        // Minimal objects, many of them, inside the output cap.
        Write(stdout, "{\"v\":2,\"s\":\"Success\",\"r\":\"\",\"p\":[");
        var minimal = System.Text.Encoding.UTF8.GetBytes(string.Concat(Enumerable.Repeat("{\"x\":[]},", 10_000)));
        for (var i = 0; i < 300; i++)
            stdout.Write(minimal);
        Write(stdout, "{\"x\":[]}]}");
        return 0;
    case "flood":
        var chunk = new byte[1 << 16];
        Array.Fill(chunk, (byte)'a');
        while (true)
            stdout.Write(chunk);
    case "garbage":
        Write(stdout, "this is not json");
        return 0;
    case "empty":
        return 0;
    case "bad-values":
        Write(stdout, """{"v":2,"s":"Success","r":"","p":[{"n":1,"w":595,"h":842,"i":false,"x":[{"t":"ciao","l":1e300,"b":20,"r":4e300,"u":30}]}]}""");
        return 0;
    case "bad-version":
        Write(stdout, """{"v":99,"s":"Empty","r":"","p":[]}""");
        return 0;
    case "too-many-pages":
        Write(stdout, "{\"v\":2,\"s\":\"NoTextLayer\",\"r\":\"\",\"p\":[" + string.Join(',', Enumerable.Range(1, 600).Select(n => $"{{\"n\":{n},\"w\":1,\"h\":1,\"i\":true,\"x\":[]}}")) + "]}");
        return 0;
    case "memory":
        // Native memory, which a managed heap limit cannot see; each block is touched so it counts in the working set.
        var blocks = new List<IntPtr>();
        while (true)
        {
            var block = Marshal.AllocHGlobal(8 << 20);
            Marshal.Copy(new byte[8 << 20], 0, block, 8 << 20);
            blocks.Add(block);
            Thread.Sleep(5);
        }
    default:
        return 2;
}

static void OrphanGrandchild(string pidFile)
{
    using var middle = Process.Start(new ProcessStartInfo(Environment.ProcessPath!, [typeof(Program).Assembly.Location, "spawn-hold-and-exit", pidFile + ".mid", pidFile + ".child"]) { UseShellExecute = false })!;
    middle.WaitForExit();
}

static string WordAnswer(string text) =>
    "{\"v\":2,\"s\":\"Success\",\"r\":\"\",\"p\":[{\"n\":1,\"w\":595,\"h\":842,\"i\":false,\"x\":[{\"t\":" + System.Text.Json.JsonSerializer.Serialize(text) + ",\"l\":10,\"b\":20,\"r\":40,\"u\":30}]}]}";

static void Write(Stream stream, string text)
{
    var bytes = System.Text.Encoding.UTF8.GetBytes(text);
    stream.Write(bytes);
    stream.Flush();
}
