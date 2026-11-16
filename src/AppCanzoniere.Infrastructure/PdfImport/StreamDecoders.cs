using System.IO.Compression;
using System.Runtime.InteropServices;

namespace AppCanzoniere.Infrastructure.PdfImport;

internal enum DecodeStatus
{
    Decoded,
    OverCap,
    Unreadable,
}

/// <summary>The output of a capped decoder: the bytes, or why there are none.</summary>
internal readonly record struct Decoded(DecodeStatus Status, ReadOnlyMemory<byte> Bytes)
{
    public static readonly Decoded OverCap = new(DecodeStatus.OverCap, ReadOnlyMemory<byte>.Empty);

    public static readonly Decoded Unreadable = new(DecodeStatus.Unreadable, ReadOnlyMemory<byte>.Empty);

    public bool IsDecoded => Status == DecodeStatus.Decoded;

    public static Decoded Of(BoundedOutput output) => new(DecodeStatus.Decoded, output.ToArray());
}

/// <summary>
/// Decoders for the PDF filters PdfPig 0.1.16 decodes, giving PdfPig's bytes but never more than a cap so a compression
/// bomb stops after cap bytes, and reporting as unreadable the inputs on which PdfPig fails too.
/// </summary>
internal static class StreamDecoders
{
    private const int ChunkBytes = 16 * 1024;

    // Flate as PdfPig reads it: the two zlib header bytes are skipped unread and the checksum is never checked.
    public static Decoded Flate(ReadOnlyMemory<byte> input, int cap)
    {
        var skip = Math.Min(2, input.Length);
        var packed = MemoryMarshal.TryGetArray(input, out var segment)
            ? new MemoryStream(segment.Array!, segment.Offset + skip, segment.Count - skip, writable: false)
            : new MemoryStream(input[skip..].ToArray(), writable: false);
        var output = new BoundedOutput(cap, input.Length * 2);
        try
        {
            using var deflate = new DeflateStream(packed, CompressionMode.Decompress);
            var chunk = new byte[ChunkBytes];
            int read;
            while ((read = deflate.Read(chunk)) > 0)
            {
                if (!output.Write(chunk.AsSpan(0, read)))
                    return Decoded.OverCap;
            }

            return Decoded.Of(output);
        }
        catch (InvalidDataException)
        {
            return Decoded.Unreadable;
        }
    }

    public static Decoded RunLength(ReadOnlySpan<byte> input, int cap)
    {
        const byte EndOfData = 128;
        var output = new BoundedOutput(cap, input.Length);
        var i = 0;
        while (i < input.Length && input[i] != EndOfData)
        {
            var length = input[i];
            var ok = length < EndOfData
                ? i + 1 + length < input.Length && output.Write(input.Slice(i + 1, length + 1))
                : i + 1 < input.Length && output.Fill(input[i + 1], 257 - length);
            if (!ok)
                return output.Count + (length < EndOfData ? length + 1 : 257 - length) > cap ? Decoded.OverCap : Decoded.Unreadable;
            i += length < EndOfData ? length + 2 : 2;
        }

        return Decoded.Of(output);
    }

    public static Decoded AsciiHex(ReadOnlySpan<byte> input, int cap)
    {
        var output = new BoundedOutput(cap, input.Length / 2);
        Span<byte> pair = stackalloc byte[2];
        var count = 0;
        for (var i = 0; i < input.Length && input[i] != '>'; i++)
        {
            if (IsHexWhitespace(input[i]) || input[i] == '<')
                continue;
            pair[count++] = input[i];
            if (count < 2)
                continue;
            if (WriteHex(pair, output) is { } failure)
                return failure;
            count = 0;
        }

        if (count == 1)
            pair[1] = (byte)'0';
        return count > 0 && WriteHex(pair, output) is { } last ? last : Decoded.Of(output);
    }

    public static Decoded Ascii85(ReadOnlySpan<byte> input, int cap)
    {
        const byte Zero = (byte)'z';
        var output = new BoundedOutput(cap, input.Length);
        Span<byte> group = stackalloc byte[5];
        var count = 0;
        for (var i = 0; i < input.Length; i++)
        {
            var b = input[i];
            if (b is 13 or 10 or 32)
                continue;
            if (b == '~' && (i == input.Length - 1 || input[i + 1] == '>'))
                break;
            if (b == Zero)
            {
                if (count > 0)
                    return Decoded.Unreadable;
                if (!output.Fill(0, 4))
                    return Decoded.OverCap;
                continue;
            }

            group[count++] = unchecked((byte)(b - 33));
            if (count == 5 && !WriteBase85(group, count, output))
                return Decoded.OverCap;
            count %= 5;
        }

        return count < 2 || WriteBase85(group, count, output) ? Decoded.Of(output) : Decoded.OverCap;
    }

    private static bool IsHexWhitespace(byte b) => b is 0 or 9 or 10 or 12 or 13 or 32;

    private static Decoded? WriteHex(ReadOnlySpan<byte> pair, BoundedOutput output)
    {
        var high = HexValue(pair[0]);
        var low = HexValue(pair[1]);
        if (high < 0 || low < 0)
            return Decoded.Unreadable;
        return output.Write((byte)((high * 16) + low)) ? null : Decoded.OverCap;
    }

    private static int HexValue(byte b) => b switch
    {
        >= (byte)'0' and <= (byte)'9' => b - '0',
        >= (byte)'A' and <= (byte)'F' => b - 'A' + 10,
        >= (byte)'a' and <= (byte)'f' => b - 'a' + 10,
        _ => -1,
    };

    // A group of count characters gives count - 1 bytes; a short final group is padded with 'u' as PdfPig does.
    private static bool WriteBase85(Span<byte> group, int count, BoundedOutput output)
    {
        for (var k = count; k < 5; k++)
            group[k] = 84;
        var value = unchecked((group[0] * 52200625) + (group[1] * 614125) + (group[2] * 7225) + (group[3] * 85) + group[4]);
        Span<byte> bytes = stackalloc byte[] { (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
        return output.Write(bytes[..(count - 1)]);
    }
}
