namespace AppCanzoniere.Application.Tests.PdfImport;

/// <summary>
/// A clock the test moves by hand. Timers fire only when the clock is advanced with fireTimers set, so a test can also
/// play a busy device whose timer callbacks run late, after the time they were due.
/// </summary>
internal sealed class ManualTime : TimeProvider
{
    private readonly List<ManualTimer> _timers = new();
    private long _ticks;

    public override long TimestampFrequency => TimeSpan.TicksPerSecond;

    public override long GetTimestamp() => Interlocked.Read(ref _ticks);

    public override DateTimeOffset GetUtcNow() => DateTimeOffset.UnixEpoch.AddTicks(GetTimestamp());

    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        var timer = new ManualTimer(this, callback, state);
        timer.Change(dueTime, period);
        lock (_timers)
            _timers.Add(timer);
        return timer;
    }

    /// <summary>Moves the clock forward; the timers that come due fire at once unless fireTimers is false.</summary>
    public void Advance(TimeSpan by, bool fireTimers = true)
    {
        Interlocked.Add(ref _ticks, by.Ticks);
        if (!fireTimers)
            return;

        List<ManualTimer> due;
        lock (_timers)
            due = _timers.Where(t => t.DueAt <= GetTimestamp()).ToList();
        foreach (var timer in due)
            timer.Fire();
    }

    private void Remove(ManualTimer timer)
    {
        lock (_timers)
            _timers.Remove(timer);
    }

    private sealed class ManualTimer(ManualTime time, TimerCallback callback, object? state) : ITimer
    {
        public long DueAt { get; private set; } = long.MaxValue;

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            DueAt = dueTime == Timeout.InfiniteTimeSpan ? long.MaxValue : time.GetTimestamp() + dueTime.Ticks;
            return true;
        }

        public void Fire()
        {
            DueAt = long.MaxValue;
            callback(state);
        }

        public void Dispose() => time.Remove(this);

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
