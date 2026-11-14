namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>
/// LZW as PdfPig 0.1.16 decodes it (9 to 12 bit codes, most significant bit first), writing at most a cap of bytes.
/// Running out of data before the end code, or a code PdfPig cannot look up, is unreadable as it is for PdfPig.
/// </summary>
internal static class LzwDecoder
{
    private const int ClearTable = 256;
    private const int EndOfData = 257;
    private const int FirstFreeCode = 258;

    // Twelve-bit codes address 4096 entries; PdfPig keeps counting past them, but those entries are never read.
    private const int AddressableCodes = 4096;

    public static Decoded Decode(ReadOnlySpan<byte> input, bool earlyChange, int cap)
    {
        var table = new Table();
        var output = new BoundedOutput(cap, input.Length * 2);
        var reader = new BitReader(input);
        var lateBy = earlyChange ? 0 : 1;
        var width = 9;
        var previous = -1;
        while (reader.TryRead(width, out var code))
        {
            if (code == ClearTable)
            {
                (table.Count, previous, width) = (FirstFreeCode, -1, 9);
                continue;
            }

            if (code == EndOfData)
                return Decoded.Of(output);
            if (previous >= table.Count || (code >= table.Count && previous < 0))
                return Decoded.Unreadable;

            var known = code < table.Count;
            var source = known ? code : previous;
            if (!table.Write(source, output) || (!known && !output.Write(table.First[source])))
                return Decoded.OverCap;
            if (previous >= 0)
                table.Add(previous, known ? table.First[code] : table.First[previous]);

            previous = code;
            width = Width(table.Count, lateBy);
        }

        return Decoded.Unreadable;
    }

    private static int Width(int count, int lateBy) =>
        count < 511 + lateBy ? 9 : count < 1023 + lateBy ? 10 : count < 2047 + lateBy ? 11 : 12;

    // Entries as prefix code plus last byte, so a string is rebuilt by walking its prefixes backwards.
    private sealed class Table
    {
        private readonly int[] _prefix = new int[AddressableCodes];
        private readonly byte[] _last = new byte[AddressableCodes];
        private readonly int[] _length = new int[AddressableCodes];
        private readonly byte[] _scratch = new byte[AddressableCodes + 1];

        public Table()
        {
            for (var i = 0; i < ClearTable; i++)
                (_prefix[i], _last[i], _length[i], First[i]) = (-1, (byte)i, 1, (byte)i);
        }

        public int Count { get; set; } = FirstFreeCode;

        public byte[] First { get; } = new byte[AddressableCodes];

        public void Add(int prefix, byte last)
        {
            if (Count < AddressableCodes)
                (_prefix[Count], _last[Count], _length[Count], First[Count]) = (prefix, last, _length[prefix] + 1, First[prefix]);
            Count++;
        }

        public bool Write(int code, BoundedOutput output)
        {
            var length = _length[code];
            for (int k = length - 1, c = code; k >= 0; k--, c = _prefix[c])
                _scratch[k] = _last[c];
            return output.Write(_scratch.AsSpan(0, length));
        }
    }

    private ref struct BitReader(ReadOnlySpan<byte> data)
    {
        private readonly ReadOnlySpan<byte> _data = data;
        private long _bit;

        public bool TryRead(int width, out int value)
        {
            value = 0;
            if (_bit + width > 8L * _data.Length)
                return false;
            for (var k = 0; k < width; k++, _bit++)
                value = (value << 1) | ((_data[(int)(_bit >> 3)] >> (7 - (int)(_bit & 7))) & 1);
            return true;
        }
    }
}
