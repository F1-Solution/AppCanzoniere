namespace AppCanzoniere.Application.PageTurning;

/// <summary>Reference-counted keep-screen-on switch shared by every screen that needs it.</summary>
public sealed class ScreenWakeLock
{
    private readonly Action<bool> _apply;
    private readonly object _gate = new();
    private int _holders;

    public ScreenWakeLock(Action<bool> apply) => _apply = apply;

    /// <summary>Keeps the screen on until the returned lease is disposed; disposing twice has no further effect.</summary>
    public IDisposable Acquire()
    {
        lock (_gate)
        {
            if (++_holders == 1)
                _apply(true);
        }

        return new Lease(this);
    }

    private void Release()
    {
        lock (_gate)
        {
            if (--_holders == 0)
                _apply(false);
        }
    }

    private sealed class Lease(ScreenWakeLock owner) : IDisposable
    {
        private int _released;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _released, 1) == 0)
                owner.Release();
        }
    }
}
