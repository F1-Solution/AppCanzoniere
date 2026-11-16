namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Why a reader process was stopped by the host.</summary>
internal enum ChildVerdict
{
    None,
    InputTooLarge,
    OutputTooLarge,
    OverMemory
}

/// <summary>A cancellation shared by the tasks around one reader process; the first reason given is kept.</summary>
internal sealed class ChildAbort
{
    private readonly CancellationTokenSource _source;
    private int _verdict;

    public ChildAbort(CancellationToken parent) => _source = CancellationTokenSource.CreateLinkedTokenSource(parent);

    public CancellationToken Token => _source.Token;

    public ChildVerdict Verdict => (ChildVerdict)Volatile.Read(ref _verdict);

    public void Cancel(ChildVerdict reason)
    {
        Interlocked.CompareExchange(ref _verdict, (int)reason, 0);
        _source.Cancel();
    }

    /// <summary>Stops the helper tasks once the process is done.</summary>
    public void Finish()
    {
        _source.Cancel();
    }
}
