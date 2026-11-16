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
        Write(stdout, """{"v":1,"s":"Success","r":"","p":[{"n":1,"w":595,"h":842,"i":false,"x":[{"t":"ciao","l":10,"b":20,"r":40,"u":30}]}]}""");
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
        Write(stdout, """{"v":1,"s":"Success","r":"","p":[{"n":1,"w":595,"h":842,"i":false,"x":[{"t":"ciao","l":1e300,"b":20,"r":4e300,"u":30}]}]}""");
        return 0;
    case "bad-version":
        Write(stdout, """{"v":99,"s":"Empty","r":"","p":[]}""");
        return 0;
    case "too-many-pages":
        Write(stdout, "{\"v\":1,\"s\":\"NoTextLayer\",\"r\":\"\",\"p\":[" + string.Join(',', Enumerable.Range(1, 600).Select(n => $"{{\"n\":{n},\"w\":1,\"h\":1,\"i\":true,\"x\":[]}}")) + "]}");
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

static void Write(Stream stream, string text)
{
    var bytes = System.Text.Encoding.UTF8.GetBytes(text);
    stream.Write(bytes);
    stream.Flush();
}
