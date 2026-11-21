namespace AppCanzoniere.Application.ImportReview;

/// <summary>Reads a picked file once, in memory, within a size limit.</summary>
public static class BoundedReader
{
    private const int ChunkSize = 81920;

    /// <summary>The bytes of the stream; null when it is longer than <paramref name="maxBytes"/>.</summary>
    public static async Task<byte[]?> ReadAsync(Stream input, long maxBytes, CancellationToken cancellationToken = default)
    {
        if (input.CanSeek)
        {
            var left = input.Length - input.Position;
            if (left > maxBytes)
                return null;

            var exact = new byte[left];
            await input.ReadExactlyAsync(exact, cancellationToken).ConfigureAwait(false);
            return exact;
        }

        using var memory = new MemoryStream();
        var buffer = new byte[ChunkSize];
        int read;
        while ((read = await input.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
        {
            if (memory.Length + read > maxBytes)
                return null;

            memory.Write(buffer, 0, read);
        }

        return memory.ToArray();
    }
}
