using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>A Windows Job Object that holds one reader and its descendants and kills them all when closed.</summary>
internal sealed class ReaderJob : IDisposable
{
    private const uint KillOnJobClose = 0x2000;
    private const int ExtendedLimitInformation = 9;

    private readonly JobHandle _handle;

    private ReaderJob(JobHandle handle) => _handle = handle;

    /// <summary>Creates a job and puts the process in it; null off Windows or when anything fails, so the caller falls back to killing the process.</summary>
    public static ReaderJob? TryAssign(Process process)
    {
        if (!OperatingSystem.IsWindows())
            return null;

        JobHandle? handle = null;
        try
        {
            handle = CreateJobObject(IntPtr.Zero, null);
            if (handle.IsInvalid || !SetKillOnClose(handle) || !AssignProcessToJobObject(handle, process.SafeHandle))
            {
                handle.Dispose();
                return null;
            }

            return new ReaderJob(handle);
        }
        catch (Exception exception) when (exception is Win32Exception or InvalidOperationException or DllNotFoundException or EntryPointNotFoundException)
        {
            handle?.Dispose();
            return null;
        }
    }

    /// <summary>Kills every process still in the job; safe to call more than once.</summary>
    public void Terminate()
    {
        try
        {
            TerminateJobObject(_handle, 1);
        }
        catch (ObjectDisposedException)
        {
            // Already closed, which killed everything.
        }
    }

    public void Dispose() => _handle.Dispose();

    private static bool SetKillOnClose(JobHandle job)
    {
        var info = new ExtendedLimits { Basic = { LimitFlags = KillOnJobClose } };
        return SetInformationJobObject(job, ExtendedLimitInformation, ref info, (uint)Marshal.SizeOf<ExtendedLimits>());
    }

    private sealed class JobHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public JobHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle() => CloseHandle(handle);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BasicLimits
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public uint LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public uint ActiveProcessLimit;
        public UIntPtr Affinity;
        public uint PriorityClass;
        public uint SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ExtendedLimits
    {
        public BasicLimits Basic;
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern JobHandle CreateJobObject(IntPtr attributes, string? name);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetInformationJobObject(JobHandle job, int infoClass, ref ExtendedLimits info, uint length);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AssignProcessToJobObject(JobHandle job, SafeHandle process);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TerminateJobObject(JobHandle job, uint exitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr handle);
}
