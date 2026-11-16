namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>A growing byte buffer that refuses to hold more than its cap.</summary>
internal sealed class BoundedOutput
{
    private const int InitialBytes = 4096;

    private readonly int _cap;
    private byte[] _buffer;

    public BoundedOutput(int cap, int expected = InitialBytes)
    {
        _cap = cap;
        _buffer = new byte[Math.Clamp(expected, 1, Math.Max(1, cap))];
    }

    public int Count { get; private set; }

    /// <summary>The bytes written so far.</summary>
    public byte[] ToArray() => _buffer.AsSpan(0, Count).ToArray();

    /// <summary>Appends the bytes, or returns false when they would take the buffer past its cap.</summary>
    public bool Write(ReadOnlySpan<byte> bytes)
    {
        if (!Reserve(bytes.Length))
            return false;
        bytes.CopyTo(_buffer.AsSpan(Count));
        Count += bytes.Length;
        return true;
    }

    public bool Write(byte value) => Fill(value, 1);

    /// <summary>Appends count copies of value, or returns false when they would take the buffer past its cap.</summary>
    public bool Fill(byte value, int count)
    {
        if (!Reserve(count))
            return false;
        _buffer.AsSpan(Count, count).Fill(value);
        Count += count;
        return true;
    }

    private bool Reserve(int more)
    {
        if (more < 0 || more > _cap - Count)
            return false;
        var needed = Count + more;
        if (needed > _buffer.Length)
            Array.Resize(ref _buffer, (int)Math.Min(_cap, Math.Max(needed, 2L * _buffer.Length)));
        return true;
    }
}
